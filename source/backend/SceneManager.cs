// <copyright file="SceneManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Backend.Narrative;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Per-scene manager that takes data from CinematicDirector, loads .tscn, runs the scene, signals completion.
/// </summary>
public partial class SceneManager : Node
{
    private static readonly string[] DefaultHandlerNames = { "NarrativeUi", "GhostUi" };

    private readonly IReadOnlyList<string> handlerPreferences;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneManager"/> class (parameterless for Godot autoload).
    /// </summary>
    public SceneManager()
    {
        this.SceneData = new StoryScriptElement();
        this.AdditionalData = new object();
        this.handlerPreferences = DefaultHandlerNames;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneManager"/> class.
    /// </summary>
    /// <param name="scene">The scene data.</param>
    /// <param name="data">Additional data for the scene.</param>
    /// <param name="preferredHandlers">Optional ordered list of handler node names to search for.</param>
    public SceneManager(StoryScriptElement scene, object data, IReadOnlyList<string>? preferredHandlers = null)
    {
        this.SceneData = scene;
        this.AdditionalData = data;
        this.handlerPreferences = preferredHandlers ?? DefaultHandlerNames;
    }

    /// <summary>
    /// Gets or sets the <see cref="StoryScriptElement"/> containing the narrative data for this scene.
    /// This includes lines, choices, pauses, and other metadata required to drive the UI handler.
    /// </summary>
    protected StoryScriptElement SceneData { get; set; }

    /// <summary>
    /// Gets or sets an opaque payload that can be used by derived managers to store any additional
    /// context required for the scene (e.g., gameplay state, references to other services, etc.).
    /// The type is deliberately generic to allow flexibility while keeping the base class lightweight.
    /// </summary>
    protected object AdditionalData { get; set; }

    /// <summary>
    /// Runs the scene by loading .tscn, displaying narrative, handling UI, signals when done.
    /// </summary>
    /// <returns>A task that completes when the scene is finished.</returns>
    /// <summary>
    /// Runs the scene by loading .tscn, displaying narrative, handling UI, signals when done.
    /// </summary>
    /// <returns>A task that completes when the scene is finished.</returns>
    public virtual async Task<SceneResult> RunSceneAsync()
    {
        try
        {
            GD.Print($"[SceneManager] Running scene: {this.SceneData.Id ?? "unknown"}");

            // Get the scene tree to add UI nodes
            var tree = Engine.GetMainLoop() as SceneTree;
            if (tree == null)
            {
                GD.PrintErr("[SceneManager] Could not get scene tree");
                return new SceneResult(this.SceneData, null);
            }

            // Try to find or instantiate the UI handler for this scene
            // Subclasses can override this behavior
            var handler = this.GetOrCreateUiHandler(tree);
            if (handler == null)
            {
                GD.PrintErr("[SceneManager] No UI handler available for scene");
                return new SceneResult(this.SceneData, null);
            }

            // Display the scene using the handler
            return await this.RunSceneWithHandlerAsync(handler);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[SceneManager] Error running scene: {ex.Message}");
            GD.PrintErr(ex.StackTrace);
            return new SceneResult(this.SceneData, null);
        }
    }

    /// <summary>
    /// Gets or creates the UI handler for this scene.
    /// Subclasses override this to provide stage-specific handlers.
    /// </summary>
    /// <param name="tree">The active scene tree.</param>
    /// <returns>The story handler instance, or null if unavailable.</returns>
    protected virtual NarrativeUi? GetOrCreateUiHandler(SceneTree tree)
    {
        var root = tree.Root;
        if (root == null)
        {
            return null;
        }

        foreach (var handlerName in this.handlerPreferences)
        {
            if (string.IsNullOrWhiteSpace(handlerName))
            {
                continue;
            }

            if (root.FindChild(handlerName, recursive: true, owned: false) is NarrativeUi handler)
            {
                return handler;
            }
        }

        return null;
    }

    /// <summary>
    /// Runs the scene using the given UI handler.
    /// Displays narrative, handles choices, applies effects.
    /// </summary>
    /// <param name="handler">The story handler to use for display.</param>
    /// <returns>A task that completes when the scene finishes.</returns>
    protected virtual async Task<SceneResult> RunSceneWithHandlerAsync(NarrativeUi handler)
    {
        GD.Print($"[SceneManager] Displaying lines for scene: {this.SceneData.Id}");

        if (this.SceneData.Lines?.Count > 0)
        {
            await WaitForInteractionAsync(handler, "display_finished", () => handler.DisplayLinesAsync(this.SceneData.Lines));
        }

        await handler.ApplySceneEffectsAsync(this.SceneData);

        ChoiceOption? selectedChoice = null;

        if (!string.IsNullOrWhiteSpace(this.SceneData.Question) && this.SceneData.Choice?.Count > 0)
        {
            selectedChoice = await WaitForInteractionAsync(
                handler,
                "choice_made",
                () => handler.PresentChoiceAsync(
                    this.SceneData.Question,
                    this.SceneData.Owner ?? "system",
                    this.SceneData.Choice));

            GD.Print($"[SceneManager] Selected choice: {selectedChoice?.Id}");

            if (selectedChoice != null)
            {
                await handler.ProcessChoiceAsync(selectedChoice);
            }
            else
            {
                GD.PrintErr("[SceneManager] No choice was selected; skipping ProcessChoiceAsync.");
            }
        }

        if (this.SceneData.Pause.HasValue && this.SceneData.Pause.Value > 0)
        {
            await Task.Delay(this.SceneData.Pause.Value);
        }

        GD.Print($"[SceneManager] Completed scene: {this.SceneData.Id}");
        return new SceneResult(this.SceneData, selectedChoice);
    }

    private static async Task WaitForInteractionAsync(NarrativeUi handler, string eventType, Func<Task> action)
    {
        await WaitForInteractionAsync<object?>(handler, eventType, async () =>
        {
            await action();
            return null;
        });
    }

    private static async Task<T> WaitForInteractionAsync<T>(NarrativeUi handler, string eventType, Func<Task<T>> action)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var tcs = new TaskCompletionSource<Variant>();
        Callable callable = default;

        callable = Callable.From<string, Variant>((evt, context) =>
        {
            if (!string.Equals(evt, eventType, StringComparison.Ordinal))
            {
                return;
            }

            if (handler.IsConnected(OmegaContainer.SignalName.InteractionComplete, callable))
            {
                handler.Disconnect(OmegaContainer.SignalName.InteractionComplete, callable);
            }

            tcs.TrySetResult(context);
        });

        handler.Connect(OmegaContainer.SignalName.InteractionComplete, callable);

        try
        {
            var result = await action();
            await tcs.Task;
            return result;
        }
        finally
        {
            if (handler.IsConnected(OmegaContainer.SignalName.InteractionComplete, callable))
            {
                handler.Disconnect(OmegaContainer.SignalName.InteractionComplete, callable);
            }
        }
    }
}
