// <copyright file="BeatSceneBase.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Infrastructure;

/// <summary>
/// Base class for all narrative beat scenes across all stages.
/// Handles automatic transition to the next beat from stage_manifest.json.
/// Provides infrastructure for loading narrative content and dynamically rendering UI.
/// Subclasses must override CurrentBeatId and StageManifestPath.
/// </summary>
public abstract partial class BeatSceneBase : Control
{
    protected SceneManager? SceneManager { get; private set; }

    /// <summary>
    /// Gets the ID of this beat (e.g., "beat_1_boot_sequence").
    /// Subclasses must override this.
    /// </summary>
    protected abstract string CurrentBeatId { get; }

    /// <summary>
    /// Gets the path to the stage manifest JSON file.
    /// Subclasses must override this (e.g., "res://source/stages/stage_1/stage_manifest.json").
    /// </summary>
    protected abstract string StageManifestPath { get; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        SceneManager = GetNode<SceneManager>("/root/SceneManager");

        if (SceneManager == null)
        {
            GD.PrintErr($"[{GetType().Name}] SceneManager not found in scene tree");
        }
    }

    /// <summary>
    /// Transitions to the next beat in the sequence.
    /// Loads the stage_manifest.json and uses it to determine the next scene.
    /// </summary>
    protected void TransitionToNextBeat()
    {
        var stageManifestLoader = new StageManifestLoader();
        var stageManifest = stageManifestLoader.LoadManifest(StageManifestPath);

        if (stageManifest == null)
        {
            GD.PrintErr($"[{GetType().Name}] Failed to load stage manifest: {StageManifestPath}");
            return;
        }

        var nextBeatId = stageManifest.GetNextSceneId(CurrentBeatId);

        if (string.IsNullOrEmpty(nextBeatId))
        {
            GD.Print($"[{GetType().Name}] No next beat found. Stage complete.");
            TransitionToNextStage(stageManifest);
            return;
        }

        var nextBeat = stageManifest.GetScene(nextBeatId);

        if (nextBeat == null)
        {
            GD.PrintErr($"[{GetType().Name}] Next beat '{nextBeatId}' not found in manifest");
            return;
        }

        GD.Print($"[{GetType().Name}] Transitioning from {CurrentBeatId} to {nextBeatId}: {nextBeat.SceneFile}");

        if (SceneManager != null)
        {
            SceneManager.TransitionToScene(nextBeat.SceneFile);
        }
        else
        {
            GD.PrintErr($"[{GetType().Name}] SceneManager not available for transition");
        }
    }

    /// <summary>
    /// Transitions to the next stage using the nextStagePath from the manifest.
    /// </summary>
    /// <param name="stageManifest">The current stage's manifest.</param>
    protected virtual void TransitionToNextStage(StageManifest stageManifest)
    {
        if (string.IsNullOrEmpty(stageManifest.NextStagePath))
        {
            GD.PrintErr($"[{GetType().Name}] No next stage path configured in manifest");
            return;
        }

        GD.Print($"[{GetType().Name}] Transitioning to next stage: {stageManifest.NextStagePath}");

        if (SceneManager != null)
        {
            SceneManager.TransitionToScene(stageManifest.NextStagePath);
        }
    }

    /// <summary>
    /// Helper method to add a text label to the content area.
    /// Subclasses can use this to dynamically create UI elements from narrative data.
    /// </summary>
    /// <param name="parent">The parent container node.</param>
    /// <param name="text">The text to display.</param>
    /// <param name="fontSize">Optional font size.</param>
    /// <returns>The created Label node.</returns>
    protected Label AddTextLabel(Node parent, string text, int fontSize = 18)
    {
        var label = new Label
        {
            Text = text,
            AutowrapMode = TextServer.AutowrapMode.Word,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };

        // Apply terminal styling
        label.AddThemeColorOverride("font_color", new Color(0.2f, 1.0f, 0.4f, 1.0f));
        label.AddThemeFontSizeOverride("font_size", fontSize);

        parent.AddChild(label);
        return label;
    }

    /// <summary>
    /// Helper method to add a button to the content area.
    /// </summary>
    /// <param name="parent">The parent container node.</param>
    /// <param name="buttonText">The button text.</param>
    /// <param name="onPressed">Callback when button is pressed.</param>
    /// <returns>The created Button node.</returns>
    protected Button AddButton(Node parent, string buttonText, System.Action onPressed)
    {
        var button = new Button
        {
            Text = buttonText,
            SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
        };

        // Apply terminal styling
        button.AddThemeColorOverride("font_color", new Color(0.2f, 1.0f, 0.4f, 1.0f));
        button.AddThemeFontSizeOverride("font_size", 16);

        button.Pressed += () => onPressed();

        parent.AddChild(button);
        return button;
    }
}
