// Archived Stage 2-specific BeatSceneBase helper. Functionality now lives in shared SceneBase.
/*
using Godot;
using System;

namespace OmegaSpiral.Source.Stages.Stage2.Beats;

[GlobalClass]
public partial class BeatSceneBase : Control
{
    protected virtual string CurrentBeatId => string.Empty;
    protected virtual string StageManifestPath => string.Empty;

    protected void AddTextLabel(Control container, string text, int fontSize = 14)
    {
        var label = new RichTextLabel
        {
            Text = text,
            CustomMinimumSize = new Vector2(0, 24)
        };
        container.AddChild(label);
    }

    protected void AddButton(Control container, string text, Action onPressed)
    {
        var button = new Button
        {
            Text = text
        };
        button.Pressed += onPressed;
        container.AddChild(button);
    }

    protected virtual void TransitionToNextBeat()
    {
        GD.Print($"[BeatSceneBase] Transitioning from {CurrentBeatId} (manifest: {StageManifestPath})");
    }
}
*/
