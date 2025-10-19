
// <copyright file="Field.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Combat;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Field.cutscenes;
using OmegaSpiral.Source.Scripts.Field.gamepieces;
using OmegaSpiral.Source.Scripts.Field.gamepieces.Controllers;

namespace OmegaSpiral.Source.Scripts.Field;
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
    public PackedScene? PlayerController { get; set; }

    /// <summary>
    /// Gets or sets the first gamepiece that the player will control. This may be null and assigned via an
    /// introductory cutscene instead.
    /// </summary>
    [Export]
    public Gamepiece? PlayerDefaultGamepiece { get; set; }

    /// <summary>
    /// Gets or sets the cutscene that will play on starting a new game.
    /// </summary>
    [Export]
    public Cutscene? OpeningCutscene { get; set; }

    /// <summary>
    /// Emitted when the player requests to exit the field exploration act.
    /// </summary>
    [Signal]
    public delegate void ActExitRequestedEventHandler();

    /// <inheritdoc/>
    public override void _Ready()
    {
        GD.Randomize();

        // Get autoload references
        var player = this.GetNode<Player>("/root/Player");
        var combatEvents = this.GetNode<CombatEvents>("/root/CombatEvents");
        var camera = this.GetNode<FieldCamera>("/root/FieldCamera");

        // Assign proper controllers to player gamepieces whenever they change.
        player.GamepieceChanged += () =>
        {
            Gamepiece? newGp = player.Gamepiece;
            camera.Gamepiece = newGp;

            // Free up any lingering controller(s).
            if (this.PlayerController != null)
            {
                foreach (Node controller in this.GetTree().GetNodesInGroup("PlayerController"))
                {
                    controller.QueueFree();
                }
            }

            if (newGp != null && this.PlayerController != null)
            {
                Node newController = this.PlayerController.Instantiate();
                System.Diagnostics.Debug.Assert(newController is PlayerController, "The Field game state requires a valid PlayerController set in the editor!");

                newGp.AddChild(newController);
                ((PlayerController) newController).IsActive = true;
            }
        };

        if (this.PlayerDefaultGamepiece != null)
        {
            player.Gamepiece = this.PlayerDefaultGamepiece;
        }

        // The field state must pause/unpause with combat accordingly.
        // Note that pausing/unpausing input is already wrapped up in triggers, which are what will
        // initiate combat.
        combatEvents.CombatInitiated += () => this.Hide();
        combatEvents.CombatFinished += (_ => this.Show());

        camera.Scale = this.Scale;
        camera.MakeCurrent();
        camera.ResetPosition();

        if (this.OpeningCutscene != null)
        {
            this.OpeningCutscene.Run();
        }

        GD.Print("Field: RPG exploration act ready - press ESC to exit");
    }

    /// <inheritdoc/>
    public override void _Input(InputEvent @event)
    {
        // Check for exit condition - ESC key exits the RPG act
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
        {
            GD.Print("Field: Exit requested - returning to Omega Spiral act sequence");
            this.EmitSignal(SignalName.ActExitRequested);
        }
    }
}
