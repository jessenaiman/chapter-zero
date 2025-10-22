
// <copyright file="OpeningCutscene.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Overworld.Maps;
/// <summary>
/// The opening cutscene that plays at the start of the game.
/// This cutscene shows the introductory dialogue and sets up the initial game state.
/// </summary>
[GlobalClass]
public partial class OpeningCutscene : OmegaSpiral.Source.Scripts.Field.cutscenes.Cutscene
{
    /// <summary>
    /// Gets or sets the Dialogic timeline to play during the opening cutscene.
    /// </summary>
    [Export]
    public Resource Timeline { get; set; } = null!;

    /// <summary>
    /// Execute the opening cutscene sequence.
    /// Shows the background, plays the dialogue timeline, transitions to the game,
    /// starts the background music, and cleans up the cutscene.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync()
    {
        await this.ShowBackgroundAsync();
        await this.PlayDialogueAsync();
        await this.PlayTransitionAsync();
        this.HideBackground();
        this.StartMusic();
        await this.ClearTransitionAsync();
        this.CallDeferred("queue_free");
    }

    private async Task ShowBackgroundAsync()
    {
        var background = this.GetNode<Node2D>("Background");
        var colorRect = background?.GetNode<ColorRect>("ColorRect");
        if (colorRect != null)
        {
            colorRect.Show();
        }
    }

    private async Task PlayDialogueAsync()
    {
        if (this.Timeline == null)
        {
            return;
        }

        var dialogic = this.GetNode("/root/Dialogic");
        if (dialogic != null)
        {
            dialogic.Call("start_timeline", this.Timeline);
            await this.ToSignal(dialogic, "timeline_ended");
        }
    }

    private async Task PlayTransitionAsync()
    {
        var transition = this.GetNode("/root/Transition");
        if (transition != null)
        {
            transition.Call("cover");
            await this.ToSignal(transition, "finished");
        }
    }

    private void HideBackground()
    {
        var background = this.GetNode<Node2D>("Background");
        var colorRect = background?.GetNode<ColorRect>("ColorRect");
        if (colorRect != null)
        {
            colorRect.Hide();
        }
    }

    private void StartMusic()
    {
        var musicPlayer = this.GetNode("/root/Music");
        if (musicPlayer != null)
        {
            var musicResource = GD.Load<AudioStream>("res://assets/music/Apple Cider.mp3");
            musicPlayer.Call("play", musicResource);
        }
    }

    private async Task ClearTransitionAsync()
    {
        var transition = this.GetNode("/root/Transition");
        if (transition != null)
        {
            transition.Call("clear", 2.0f);
            await this.ToSignal(transition, "finished");
        }
    }
}
