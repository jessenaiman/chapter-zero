using System;
using Godot;

namespace OmegaSpiral.Source.Stages.Stage2.Beats;

/// <summary>
/// Base class for Echo Chamber beat scenes. Provides shared helpers for UI construction and flow.
/// </summary>
[GlobalClass]
public partial class BeatSceneBase : Control
{
    /// <summary>
    /// Gets the identifier of the current beat (matches manifest ID).
    /// </summary>
    protected virtual string CurrentBeatId => string.Empty;

    /// <summary>
    /// Gets the stage manifest path used to determine sequencing.
    /// </summary>
    protected virtual string StageManifestPath => string.Empty;

    /// <summary>
    /// Adds a rich-text label to the specified container.
    /// </summary>
    protected void AddTextLabel(Control container, string text, int fontSize = 14)
    {
        var label = new RichTextLabel
        {
            Text = text,
            CustomMinimumSize = new Vector2(0, 24)
        };

        // TODO: Hook up shared theming to respect fontSize parameter.
        container.AddChild(label);
    }

    /// <summary>
    /// Adds a button with a callback to the specified container.
    /// </summary>
    protected void AddButton(Control container, string text, Action onPressed)
    {
        var button = new Button
        {
            Text = text
        };

        button.Pressed += onPressed;
        container.AddChild(button);
    }

    /// <summary>
    /// Placeholder beat navigation hook. Integrate with EchoHub once orchestrator ready.
    /// </summary>
    protected virtual void TransitionToNextBeat()
    {
        GD.Print($"[BeatSceneBase] Transitioning from {CurrentBeatId} (manifest: {StageManifestPath})");
    }
}
