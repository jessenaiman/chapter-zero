// <copyright file="ConversationEncounter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Godot;

/// <summary>
/// An interaction that triggers a pre-combat dialogue, initiates combat,
/// and then plays victory or loss dialogue based on the outcome.
/// </summary>
[Tool]
[GlobalClass]
public partial class ConversationEncounter : Interaction
{
    /// <summary>
    /// Gets or sets the timeline to play before combat begins.
    /// </summary>
    [Export]
    public Resource PreCombatTimeline { get; set; } = null!; // DialogicTimeline

    /// <summary>
    /// Gets or sets the timeline to play if the player wins the combat.
    /// </summary>
    [Export]
    public Resource VictoryTimeline { get; set; } = null!; // DialogicTimeline

    /// <summary>
    /// Gets or sets the timeline to play if the player loses the combat.
    /// </summary>
    [Export]
    public Resource LossTimeline { get; set; } = null!; // DialogicTimeline

    /// <summary>
    /// Gets or sets the combat arena scene to load for the battle.
    /// </summary>
    [Export]
    public PackedScene CombatArena { get; set; } = null!;

    /// <summary>
    /// Execute the conversation encounter sequence.
    /// </summary>
    protected async void Execute()
    {
        var dialogic = this.GetNode("/root/Dialogic");
        if (dialogic != null && this.PreCombatTimeline != null)
        {
            // Start the pre-combat timeline
            dialogic.Call("start_timeline", this.PreCombatTimeline);

            // Wait for the timeline to finish
            await this.ToSignal(dialogic, "timeline_ended");
        }

        // Let other systems know that combat has been triggered
        var fieldEvents = this.GetNode("/root/FieldEvents");
        if (fieldEvents != null && this.CombatArena != null)
        {
            fieldEvents.EmitSignal("combat_triggered", this.CombatArena);
        }

        // Wait for combat to finish
        bool didPlayerWin = false;
        var combatEvents = this.GetNode("/root/CombatEvents");
        if (combatEvents != null)
        {
            var result = await this.ToSignal(combatEvents, "combat_finished");
            if (result.Length > 0 && result[0].AsBool())
            {
                didPlayerWin = true;
            }
        }

        // The combat ends with a covered screen, so we fix that here
        var transition = this.GetNode("/root/Transition");
        if (transition != null)
        {
            transition.CallDeferred("clear", 0.2);
            await this.ToSignal(transition, "finished");
        }

        // Run post-combat events
        if (dialogic != null)
        {
            if (didPlayerWin && this.VictoryTimeline != null)
            {
                dialogic.Call("start_timeline", this.VictoryTimeline);
            }
            else if (!didPlayerWin && this.LossTimeline != null)
            {
                dialogic.Call("start_timeline", this.LossTimeline);
            }

            await this.ToSignal(dialogic, "timeline_ended");
        }
    }

    /// <inheritdoc/>
    public override async void Run()
    {
        this.Execute();
        await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
        base.Run();
    }
}
