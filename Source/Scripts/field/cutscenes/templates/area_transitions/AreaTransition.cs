// <copyright file="AreaTransition.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Timer = Godot.Timer; // Resolve ambiguity between Godot.Timer and System.Threading.Timer

/// <summary>
/// Base class for area transitions that move the player between different areas of the game.
/// Area transitions are triggers that, when entered, move the player to a new location
/// and potentially trigger other events like music changes or camera effects.
/// </summary>
[GlobalClass]
public partial class AreaTransition : Trigger
{
    /// <summary>
    /// Gets or sets the coordinates where the player will arrive after the transition.
    /// </summary>
    [Export]
    public Vector2 ArrivalCoordinates
    {
        get => this.arrivalCoordinates;
        set
        {
            this.arrivalCoordinates = value;

            // Update destination position in editor
            if (Engine.IsEditorHint())
            {
                if (!this.IsInsideTree())
                {
                    this.CallDeferred(nameof(this.UpdateDestinationPosition));
                    return;
                }

                this.UpdateDestinationPosition();
            }
        }
    }

    /// <summary>
    /// Gets or sets the audio stream to play when entering this area.
    /// </summary>
    [Export]
    public AudioStream NewMusic { get; set; }

    /// <summary>
    /// The blackout timer used to wait between fade-out and fade-in during transitions.
    /// No delay looks odd, so this provides a brief pause in darkness.
    /// </summary>
    private Timer blackoutTimer;

    /// <summary>
    /// The coordinates where the player will arrive after the transition.
    /// </summary>
    private Vector2 arrivalCoordinates = Vector2.Zero;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        // Get reference to the blackout timer
        this.blackoutTimer = this.GetNode<Timer>("BlackoutTimer");

        if (!Engine.IsEditorHint())
        {
            // Remove the destination marker in game (only for editor preview)
            var destination = this.GetNode<Node2D>("Destination");
            if (destination != null)
            {
                destination.QueueFree();
            }
        }
    }

    /// <summary>
    /// Update the destination position in the editor based on arrival coordinates.
    /// </summary>
    private void UpdateDestinationPosition()
    {
        var destination = this.GetNode<Node2D>("Destination");
        if (destination != null)
        {
            destination.Position = this.ArrivalCoordinates - this.Position;
        }
    }

    /// <summary>
    /// Activate the area transition's logic.
    /// Handles the transition process including screen covering, moving the player,
    /// and revealing the screen again.
    /// </summary>
    /// <param name="triggeringObject">The object that triggered this transition.</param>
    public virtual async void Activate(Node2D triggeringObject)
    {
        // Convert the triggering object to an Area2D to get the gamepiece
        var area = triggeringObject as Area2D;
        if (area == null)
        {
            return;
        }

        // Pausing the field immediately will deactivate physics objects, which are in the middle of
        // processing (hence _on_area_entered). We need to wait a frame before pausing anything.
        await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);

        // Cover the screen to hide the area transition.
        await Transition.Instance.Cover(0.25f);

        // Move the gamepiece to its new position and update the camera immediately.
        var gamepiece = area.Owner as Gamepiece;
        if (gamepiece != null)
        {
            gamepiece.StopMoving(); // Assuming Gamepiece has a StopMoving method
            gamepiece.Position = this.ArrivalCoordinates;

            // Update the gamepiece registry with the new cell position
            var newCell = Gameboard.PixelToCell(this.ArrivalCoordinates);
            GamepieceRegistry.Instance.MoveGamepiece(gamepiece, newCell);

            // Reset the camera position
            Camera.Instance.ResetPosition();
        }

        // Let the screen rest in darkness for a little while. Revealing the screen immediately with no
        // delay looks 'off'.
        this.blackoutTimer.Start();
        await this.ToSignal(this.blackoutTimer, Timer.SignalName.Timeout);

        // All kinds of shenanigans could happen once the screen blacks out. It may be asynchronous, so
        // give the opportunity for the designer to run a lengthy event.
        await OnBlackout().ConfigureAwait(false);

        // Reveal the screen and unpause the field gamestate.
        await Transition.Instance.Clear(0.10f);
    }

    /// <summary>
    /// Callback that occurs during the blackout phase of the transition.
    /// Override this method to add custom behavior during the transition blackout.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    protected virtual async Task OnBlackout()
    {
        // Play new music if specified
        if (this.NewMusic != null)
        {
            Music.Instance?.Play(this.NewMusic);
        }
    }
}
