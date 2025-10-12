// <copyright file="OpeningCutscene.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Godot;

/// <summary>
/// The opening cutscene that plays at the start of the game.
/// This cutscene shows the introductory dialogue and sets up the initial game state.
/// </summary>
public partial class OpeningCutscene : Cutscene
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
        // Show the background color rect
        var background = this.GetNode<Node2D>("Background");
        var colorRect = background?.GetNode<ColorRect>("ColorRect");
        if (colorRect != null)
        {
            colorRect.Show();
        }

        // Start the dialogue timeline
        if (this.Timeline != null)
        {
            var dialogic = this.GetNode("/root/Dialogic");
            if (dialogic != null)
            {
                dialogic.Call("start_timeline", this.Timeline);
                await this.ToSignal(dialogic, "timeline_ended");
            }
        }

        // Cover the screen with a transition
        var transition = this.GetNode("/root/Transition");
        if (transition != null)
        {
            transition.Call("cover");
            await this.ToSignal(transition, "finished");
        }

        // Hide the background
        if (colorRect != null)
        {
            colorRect.Hide();
        }

        // Start the background music
        var musicPlayer = this.GetNode("/root/Music");
        if (musicPlayer != null)
        {
            var musicResource = GD.Load<AudioStream>("res://assets/music/Apple Cider.mp3");
            musicPlayer.Call("play", musicResource);
        }

        // Clear the transition
        if (transition != null)
        {
            transition.Call("clear", 2.0f);
            await this.ToSignal(transition, "finished");
        }

        // Queue the cutscene for removal
        this.CallDeferred("queue_free");
    }
}
