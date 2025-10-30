
// <copyright file="ConversationEncounter.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Field;

namespace OmegaSpiral.Source.Overworld.Maps.town;
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

    /// <inheritdoc/>
    public override async void Run()
    {
        this.Execute();
        await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
        base.Run();
    }

    /// <summary>
    /// Execute the conversation encounter sequence.
    /// </summary>
    protected async void Execute()
    {
        await this.RunPreCombatTimeline().ConfigureAwait(false);
        await this.TriggerCombat().ConfigureAwait(false);
        bool didPlayerWin = await this.WaitForCombatResult().ConfigureAwait(false);
        await this.HandlePostCombatTransition().ConfigureAwait(false);
        await this.RunPostCombatTimeline(didPlayerWin).ConfigureAwait(false);
    }

    private async Task RunPreCombatTimeline()
    {
        var dialogic = this.GetNode("/root/Dialogic");
        if (dialogic == null || this.PreCombatTimeline == null)
        {
            return;
        }

        dialogic.Call("start_timeline", this.PreCombatTimeline);
        await this.ToSignal(dialogic, "timeline_ended");
    }

    private Task TriggerCombat()
    {
        var fieldEvents = GetNode("/root/FieldEvents");
        if (fieldEvents != null && CombatArena != null)
        {
            fieldEvents.EmitSignal("combat_triggered", CombatArena);
        }

        return Task.CompletedTask;
    }

    private async Task<bool> WaitForCombatResult()
    {
        var combatEvents = this.GetNode("/root/CombatEvents");
        if (combatEvents == null)
        {
            return false;
        }

        var result = await this.ToSignal(combatEvents, "combat_finished");
        return result.Length > 0 && result[0].AsBool();
    }

    private async Task HandlePostCombatTransition()
    {
        var transition = this.GetNode("/root/Transition");
        if (transition == null)
        {
            return;
        }

        transition.CallDeferred("clear", 0.2);
        await this.ToSignal(transition, "finished");
    }

    private async Task RunPostCombatTimeline(bool didPlayerWin)
    {
        var dialogic = this.GetNode("/root/Dialogic");
        if (dialogic == null)
        {
            return;
        }

        Resource? timeline = didPlayerWin ? this.VictoryTimeline : this.LossTimeline;
        if (timeline == null)
        {
            return;
        }

        dialogic.Call("start_timeline", timeline);
        await this.ToSignal(dialogic, "timeline_ended");
    }
}
