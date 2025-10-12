// <copyright file="Field.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// The cutscene that will play on starting a new game.
/// </summary>
public partial class Field : Node2D
{
    /// <summary>
    /// Gets or sets a PlayerController that will be dynamically assigned to whichever Gamepiece the player currently
    /// controls.
    /// </summary>
    [Export]
    public PackedScene PlayerController { get; set; }

    /// <summary>
    /// Gets or sets the first gamepiece that the player will control. This may be null and assigned via an
    /// introductory cutscene instead.
    /// </summary>
    [Export]
    public Gamepiece PlayerDefaultGamepiece { get; set; }

    /// <summary>
    /// Gets or sets the cutscene that will play on starting a new game.
    /// </summary>
    [Export]
    public Cutscene OpeningCutscene { get; set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        GD.Randomize();

        // Assign proper controllers to player gamepieces whenever they change.
        Player.GamepieceChanged += () =>
        {
            Gamepiece newGp = Player.Gamepiece;
            Camera.Gamepiece = newGp;

            // Free up any lingering controller(s).
            foreach (Node controller in this.GetTree().GetNodesInGroup(this.PlayerController.Group))
            {
                controller.QueueFree();
            }

            if (newGp != null)
            {
                Node newController = this.PlayerController.Instantiate();
                GD.Assert(newController is PlayerController, "The Field game state requires a valid " +
                        "PlayerController set in the editor!");

                newGp.AddChild(newController);
                ((PlayerController)newController).IsActive = true;
            }
        };

        Player.Gamepiece = this.PlayerDefaultGamepiece;

        // The field state must pause/unpause with combat accordingly.
        // Note that pausing/unpausing input is already wrapped up in triggers, which are what will
        // initiate combat.
        CombatEvents.CombatInitiated += () => this.Hide();
        CombatEvents.CombatFinished += (bool isVictory) => this.Show();

        Camera.Scale = this.Scale;
        Camera.MakeCurrent();
        Camera.ResetPosition();

        if (this.OpeningCutscene != null)
        {
            this.OpeningCutscene.Run.CallDeferred();
        }
    }
}
