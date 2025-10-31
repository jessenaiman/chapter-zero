// <copyright file="SceneManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend;

using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Backend.Narrative;

/// <summary>
/// Per-scene manager that takes data from CinematicDirector, loads .tscn, runs the scene, signals completion.
/// </summary>
public partial class SceneManager : Node
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SceneManager"/> class (parameterless for Godot autoload).
    /// </summary>
    public SceneManager()
    {
        this.SceneData = new StoryScriptElement();
        this.AdditionalData = new object();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneManager"/> class.
    /// </summary>
    /// <param name="scene">The scene data.</param>
    /// <param name="data">Additional data for the scene.</param>
    public SceneManager(StoryScriptElement scene, object data)
    {
        this.SceneData = scene;
        this.AdditionalData = data;
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
    public virtual async Task RunSceneAsync()
    {
        try
        {
            GD.Print($"[SceneManager] Running scene: {this.SceneData.Id ?? "unknown"}");

            // Get the scene tree to add UI nodes
            var tree = Engine.GetMainLoop() as SceneTree;
            if (tree == null)
            {
                GD.PrintErr("[SceneManager] Could not get scene tree");
                return;
            }

            // Try to find or instantiate the UI handler for this scene
            // Subclasses can override this behavior
            var handler = this.GetOrCreateUiHandler(tree);
            if (handler == null)
            {
                GD.PrintErr("[SceneManager] No UI handler available for scene");
                return;
            }

            // Display the scene using the handler
            await this.RunSceneWithHandlerAsync(handler);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[SceneManager] Error running scene: {ex.Message}");
            GD.PrintErr(ex.StackTrace);
        }
    }

    /// <summary>
    /// Gets or creates the UI handler for this scene.
    /// Subclasses override this to provide stage-specific handlers.
    /// </summary>
    /// <param name="tree">The active scene tree.</param>
    /// <returns>The story handler instance, or null if unavailable.</returns>
    protected virtual IStoryHandler? GetOrCreateUiHandler(SceneTree tree)
    {
        // Base implementation: try to find any IStoryHandler in the scene tree
        var root = tree.Root;
        return root?.FindChild("NarrativeUi", recursive: true, owned: false) as IStoryHandler
            ?? root?.FindChild("GhostUi", recursive: true, owned: false) as IStoryHandler;
    }

    /// <summary>
    /// Runs the scene using the given UI handler.
    /// Displays narrative, handles choices, applies effects.
    /// </summary>
    /// <param name="handler">The story handler to use for display.</param>
    /// <returns>A task that completes when the scene finishes.</returns>
    protected virtual async Task RunSceneWithHandlerAsync(IStoryHandler handler)
    {
        // Boot sequence for first scene
        GD.Print($"[SceneManager] Displaying lines for scene: {this.SceneData.Id}");

        // Display narrative lines
        if (this.SceneData.Lines?.Count > 0)
        {
            await handler.DisplayLinesAsync(this.SceneData.Lines).ConfigureAwait(false);
        }

        // Apply scene effects (pause, shader effects, etc.)
        await handler.ApplySceneEffectsAsync(this.SceneData).ConfigureAwait(false);

        // Present choices if available
        if (!string.IsNullOrWhiteSpace(this.SceneData.Question) && this.SceneData.Choice?.Count > 0)
        {
            ChoiceOption selectedChoice = await handler.PresentChoiceAsync(
                this.SceneData.Question,
                this.SceneData.Owner ?? "system",
                this.SceneData.Choice).ConfigureAwait(false);

            GD.Print($"[SceneManager] Selected choice: {selectedChoice.Id}");

            // Process the choice if a valid selection was made
            // The handler may return null when the player cancels or no choice is available.
            // Guard against null to avoid passing a null argument to ProcessChoiceAsync (CS8604).
            if (selectedChoice != null)
            {
                await handler.ProcessChoiceAsync(selectedChoice).ConfigureAwait(false);
            }
            else
            {
                GD.PrintErr("[SceneManager] No choice was selected; skipping ProcessChoiceAsync.");
            }
        }

        // Final pause if specified
        if (this.SceneData.Pause.HasValue && this.SceneData.Pause.Value > 0)
        {
            await Task.Delay(this.SceneData.Pause.Value).ConfigureAwait(false);
        }

        GD.Print($"[SceneManager] Completed scene: {this.SceneData.Id}");
    }
}
