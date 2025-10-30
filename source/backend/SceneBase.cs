// <copyright file="SceneBase.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Backend;

/// <summary>
/// Shared base class for beat/scene scripts across stages.
/// Provides lightweight helpers for dynamic Ui construction and beat progression hooks.
/// Implements <see cref="IManifestAware"/> to support standalone scene debugging and testing.
/// </summary>
[GlobalClass]
public abstract partial class SceneBase : Control, IManifestAware
{
    private StageManifest? _LoadedManifest;

    /// <summary>
    /// Gets the identifier of the current beat (should align with manifest entry where applicable).
    /// </summary>
    protected virtual string CurrentBeatId => string.Empty;

    /// <summary>
    /// Gets the stage manifest path used for determining sequencing.
    /// Implements <see cref="IManifestAware.ManifestPath"/>.
    /// </summary>
    public virtual string ManifestPath => string.Empty;

    /// <summary>
    /// Gets the loaded manifest associated with this scene (for debugging/testing).
    /// </summary>
    public object? LoadedManifest => _LoadedManifest;

    /// <summary>
    /// Gets the loaded manifest data as the strongly-typed StageManifest.
    /// </summary>
    protected StageManifest? LoadedStageManifest => _LoadedManifest;

    /// <summary>
    /// Adds a <see cref="RichTextLabel"/> to the specified container.
    /// </summary>
    /// <param name="container">Parent node that will host the label.</param>
    /// <param name="text">Text to display.</param>
    /// <param name="fontSize">Optional font size hint for future theming.</param>
    protected void AddTextLabel(Control container, string text, int fontSize = 14)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        var label = new RichTextLabel
        {
            Text = text,
            CustomMinimumSize = new Vector2(0, 24)
        };

        // TODO: Wire up project-wide theme support for font size overrides.
        container.AddChild(label);
    }

    /// <summary>
    /// Adds a <see cref="Button"/> and associates a callback with its Pressed event.
    /// </summary>
    /// <param name="container">Parent node that will host the button.</param>
    /// <param name="text">Button text.</param>
    /// <param name="onPressed">Callback invoked when the button is pressed.</param>
    protected void AddButton(Control container, string text, Action onPressed)
    {
        if (container == null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        if (onPressed == null)
        {
            throw new ArgumentNullException(nameof(onPressed));
        }

        var button = new Button
        {
            Text = text
        };

        button.Pressed += onPressed;
        container.AddChild(button);
    }

    /// <summary>
    /// Hook for advancing to the next beat/scene. Stages should override to integrate with their orchestrators.
    /// </summary>
    protected virtual void TransitionToNextBeat()
    {
        GD.Print($"[SceneBase] Request transition from {CurrentBeatId} (manifest: {ManifestPath})");
    }

    /// <summary>
    /// Loads the stage manifest for this scene. Useful for standalone scene debugging and testing.
    /// Implements <see cref="IManifestAware.LoadManifest"/>.
    /// </summary>
    public void LoadManifest()
    {
        if (string.IsNullOrWhiteSpace(ManifestPath))
        {
            GD.Print($"[SceneBase] ManifestPath not defined for {Name}; standalone debugging disabled");
            return;
        }

        try
        {
            var loader = new StageManifestLoader();
            _LoadedManifest = loader.LoadManifest(ManifestPath);

            if (_LoadedManifest == null)
            {
                GD.PrintErr($"[SceneBase] Failed to load stage manifest from: {ManifestPath}");
                return;
            }

            GD.Print($"[SceneBase] Successfully loaded stage manifest for scene {Name}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[SceneBase] Error loading manifest: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the active <see cref="SceneManager"/> for the current scene tree, if any.
    /// </summary>
    /// <returns>The active stage manager or <c>null</c> if none registered.</returns>
    protected SceneManager? GetSceneManager()
    {
        return GetNodeOrNull<SceneManager>("/root/SceneManager");
    }

    /// <summary>
    /// Awards affinity points to the active stage manager, if available.
    /// </summary>
    /// <param name="alignmentId">Alignment identifier (e.g., light, shadow, ambition).</param>
    /// <param name="points">Points to award (can be negative).</param>
    protected void AwardAffinity(string alignmentId, int points)
    {
        GetSceneManager()?.SetDreamweaverThread(alignmentId);
    }
}
