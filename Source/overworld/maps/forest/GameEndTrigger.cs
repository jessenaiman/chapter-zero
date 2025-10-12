// <copyright file="GameEndTrigger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Godot;

/// <summary>
/// A trigger that plays the game ending sequence when the player reaches the end of the forest.
/// Moves the player to a specific position, plays a dialogue, and triggers a final animation.
/// </summary>
[Tool]
[GlobalClass]
public partial class GameEndTrigger : Trigger
{
    private Gamepiece? gamepiece;
    private Godot.Timer? timer;

    /// <summary>
    /// Gets or sets the Dialogic timeline to play during the ending sequence.
    /// </summary>
    [Export]
    public Resource Timeline { get; set; } = null!; // DialogicTimeline

    /// <summary>
    /// Gets or sets the animation player for the ghost lunge animation.
    /// </summary>
    [Export]
    public AnimationPlayer GhostAnimationPlayer { get; set; } = null!;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            this.timer = this.GetNode<Godot.Timer>("Timer");
        }
    }

    /// <summary>
    /// Execute the game ending sequence.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync()
    {
        if (this.gamepiece == null)
        {
            return;
        }

        // Get the Gameboard singleton
        var gameboard = this.GetNode("/root/Gameboard");
        if (gameboard != null)
        {
            var destinationPixel = (Vector2)gameboard.Call("cell_to_pixel", new Vector2I(53, 30));
            this.gamepiece.Call("move_to", destinationPixel);
            await this.ToSignal(this.gamepiece, "arrived");
        }

        // Wait for a moment
        if (this.timer != null)
        {
            this.timer.Start();
            await this.ToSignal(this.timer, Godot.Timer.SignalName.Timeout);
        }

        // Start the Dialogic timeline
        var dialogic = this.GetNode("/root/Dialogic");
        if (dialogic != null && this.Timeline != null)
        {
            dialogic.Call("start_timeline", this.Timeline);
            await this.ToSignal(dialogic, "timeline_ended");
        }

        // Wait for another moment
        if (this.timer != null)
        {
            this.timer.Start();
            await this.ToSignal(this.timer, Godot.Timer.SignalName.Timeout);
        }

        // Note that the lunge animation also includes a screen transition and some text
        if (this.GhostAnimationPlayer != null)
        {
            this.GhostAnimationPlayer.Play("lunge");
            await this.ToSignal(this.GhostAnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        }

        // Lock input by waiting forever (infinite wait)
        await this.ToSignal(this.GetTree(), SceneTree.SignalName.TreeChanged);
    }

    /// <summary>
    /// Called when an area enters the trigger.
    /// </summary>
    /// <param name="area">The area that entered the trigger.</param>
    protected void OnAreaEntered(Area2D area)
    {
        if (area == null)
        {
            return;
        }

        if (!Engine.IsEditorHint())
        {
            this.gamepiece = area.Owner as Gamepiece;
        }
    }
}
