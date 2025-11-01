// <copyright file="CombatEncounterTrigger.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Common.ScreenTransitions;
using OmegaSpiral.Source.Scripts.Field;
using OmegaSpiral.Source.Scripts.Combat;
using Gamepiece = OmegaSpiral.Source.Scripts.Field.gamepieces.Gamepiece;

namespace OmegaSpiral.Source.Overworld.Maps;

/// <summary>
/// A trigger that initiates combat when the player enters its area.
/// Handles pre-combat dialogue (using Dialogic), combat execution with Active Time Battle system,
/// and post-combat outcomes (victory/loss dialogues).
/// Based on conversation_encounter.gd from godot-open-rpg demo.
/// </summary>
[Tool]
[GlobalClass]
public partial class CombatEncounterTrigger : OmegaSpiral.Source.Scripts.Field.cutscenes.Trigger
{
    /// <summary>
    /// Gets or sets the Dialogic timeline to play before combat starts.
    /// </summary>
    [Export]
    public string? PreCombatTimeline { get; set; }

    /// <summary>
    /// Gets or sets the Dialogic timeline to play when the player wins.
    /// </summary>
    [Export]
    public string? VictoryTimeline { get; set; }

    /// <summary>
    /// Gets or sets the Dialogic timeline to play when the player loses.
    /// </summary>
    [Export]
    public string? LossTimeline { get; set; }

    /// <summary>
    /// Gets or sets the combat arena scene to instantiate for this encounter.
    /// </summary>
    [Export]
    public PackedScene? CombatArena { get; set; }

    private Node? dialogic;
    private CombatEvents? combatEvents;
    private FieldEvents? fieldEvents;
    private ScreenTransition? transition;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        if (!Engine.IsEditorHint())
        {
            // Get autoload references (Dialogic 2.x is a GDScript autoload)
            this.dialogic = this.GetNodeOrNull<Node>("/root/Dialogic");
            this.combatEvents = this.GetNodeOrNull<CombatEvents>("/root/CombatEvents");
            this.fieldEvents = this.GetNodeOrNull<FieldEvents>("/root/FieldEvents");
            this.transition = this.GetNodeOrNull<ScreenTransition>("/root/Transition");

            if (this.dialogic == null)
            {
                GD.PrintErr($"{this.Name}: Dialogic autoload not found!");
            }

            if (this.combatEvents == null)
            {
                GD.PrintErr($"{this.Name}: CombatEvents autoload not found!");
            }

            if (this.fieldEvents == null)
            {
                GD.PrintErr($"{this.Name}: FieldEvents autoload not found!");
            }

            if (this.transition == null)
            {
                GD.PrintErr($"{this.Name}: Transition autoload not found!");
            }
        }
    }

    /// <summary>
    /// Executes the combat encounter trigger sequence.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        // Play pre-combat dialogue (Dialogic 2.x API)
        if (!string.IsNullOrEmpty(this.PreCombatTimeline) && this.dialogic != null)
        {
            this.dialogic.Call("start", this.PreCombatTimeline);
            await this.ToSignal(this.dialogic, "timeline_ended");
        }

        // Trigger combat and wait for outcome
        if (this.CombatArena != null && this.fieldEvents != null && this.combatEvents != null)
        {
            // Emit the combat trigger signal with the arena scene
            this.fieldEvents.EmitSignal(FieldEvents.SignalName.CombatTriggered, this.CombatArena);

            // Wait for combat to finish and get the result
            var combatResult = await this.ToSignal(this.combatEvents, CombatEvents.SignalName.CombatFinished);
            bool didPlayerWin = (bool) combatResult[0];

            // Combat ends with a covered screen, clear it
            if (this.transition != null)
            {
                await this.transition.ClearScreen(0.2f).ConfigureAwait(false);
            }

            // Play appropriate post-combat dialogue (Dialogic 2.x API)
            if (didPlayerWin && !string.IsNullOrEmpty(this.VictoryTimeline) && this.dialogic != null)
            {
                this.dialogic.Call("start", this.VictoryTimeline);
                await this.ToSignal(this.dialogic, "timeline_ended");

                // If this was a roaming encounter, remove it from the field after victory
                this.DeactivateEncounter();
            }
            else if (!didPlayerWin && !string.IsNullOrEmpty(this.LossTimeline) && this.dialogic != null)
            {
                this.dialogic.Call("start", this.LossTimeline);
                await this.ToSignal(this.dialogic, "timeline_ended");

                // TODO: Implement game over logic
                GD.Print("Game Over - Player party defeated!");
            }
        }
        else
        {
            GD.PrintErr($"{this.Name}: Missing required components for combat encounter!");
        }
    }

    /// <summary>
    /// Deactivates this encounter after victory, typically removing roaming enemies from the field.
    /// </summary>
    private void DeactivateEncounter()
    {
        // Deactivate the trigger so it can't be triggered again
        this.IsActive = false;

        // If the parent is a Gamepiece (roaming enemy), remove it
        if (this.GetParent() is Gamepiece enemy)
        {
            enemy.CallDeferred("queue_free");
        }
    }

    /// <summary>
    /// Gets configuration warnings for the editor.
    /// </summary>
    /// <returns>An array of configuration warning strings.</returns>
    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new System.Collections.Generic.List<string>(base._GetConfigurationWarnings());

        if (this.CombatArena == null)
        {
            warnings.Add("CombatArena scene must be assigned!");
        }

        if (string.IsNullOrEmpty(this.PreCombatTimeline))
        {
            warnings.Add("PreCombatTimeline should be set (optional but recommended).");
        }

        if (string.IsNullOrEmpty(this.VictoryTimeline))
        {
            warnings.Add("VictoryTimeline should be set (optional but recommended).");
        }

        if (string.IsNullOrEmpty(this.LossTimeline))
        {
            warnings.Add("LossTimeline should be set (optional but recommended).");
        }

        return warnings.ToArray();
    }
}
