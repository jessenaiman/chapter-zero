
// <copyright file="Field.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Combat;
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

        // Get autoload references with error handling
        var player = this.GetNodeOrNull<Player>("/root/Player");
        var combatEvents = this.GetNodeOrNull<CombatEvents>("/root/CombatEvents");
        var camera = this.GetNodeOrNull<FieldCamera>("/root/FieldCamera");

        if (player == null)
        {
            GD.PushError("Field: Player autoload not found. Player functionality will not work.");
            return; // Cannot continue without player autoload
        }
        if (combatEvents == null)
        {
            GD.PushError("Field: CombatEvents autoload not found. Combat system will not work.");
        }
        if (camera == null)
        {
            GD.PushError("Field: FieldCamera autoload not found. Camera functionality will not work.");
            return; // Cannot continue without camera autoload
        }

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
        if (combatEvents != null)
        {
            combatEvents.CombatInitiated += () => this.Hide();
            combatEvents.CombatFinished += (_ => this.Show());
        }

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
