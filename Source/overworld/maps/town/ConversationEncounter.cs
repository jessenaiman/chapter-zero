using Godot;
using System.Threading.Tasks;

/// <summary>
/// An interaction that triggers a pre-combat dialogue, initiates combat,
/// and then plays victory or loss dialogue based on the outcome.
/// </summary>
[Tool]
[GlobalClass]
public partial class ConversationEncounter : Interaction
{
    /// <summary>
    /// The timeline to play before combat begins.
    /// </summary>
    [Export]
    public Resource PreCombatTimeline { get; set; } = null!; // DialogicTimeline

    /// <summary>
    /// The timeline to play if the player wins the combat.
    /// </summary>
    [Export]
    public Resource VictoryTimeline { get; set; } = null!; // DialogicTimeline

    /// <summary>
    /// The timeline to play if the player loses the combat.
    /// </summary>
    [Export]
    public Resource LossTimeline { get; set; } = null!; // DialogicTimeline

    /// <summary>
    /// The combat arena scene to load for the battle.
    /// </summary>
    [Export]
    public PackedScene CombatArena { get; set; } = null!;

    /// <summary>
    /// Execute the conversation encounter sequence.
    /// </summary>
    protected async void Execute()
    {
        var dialogic = GetNode("/root/Dialogic");
        if (dialogic != null && PreCombatTimeline != null)
        {
            // Start the pre-combat timeline
            dialogic.Call("start_timeline", PreCombatTimeline);

            // Wait for the timeline to finish
            await ToSignal(dialogic, "timeline_ended");
        }

        // Let other systems know that combat has been triggered
        var fieldEvents = GetNode("/root/FieldEvents");
        if (fieldEvents != null && CombatArena != null)
        {
            fieldEvents.EmitSignal("combat_triggered", CombatArena);
        }

        // Wait for combat to finish
        bool didPlayerWin = false;
        var combatEvents = GetNode("/root/CombatEvents");
        if (combatEvents != null)
        {
            var result = await ToSignal(combatEvents, "combat_finished");
            if (result.Length > 0 && result[0].AsBool())
            {
                didPlayerWin = true;
            }
        }

        // The combat ends with a covered screen, so we fix that here
        var transition = GetNode("/root/Transition");
        if (transition != null)
        {
            transition.CallDeferred("clear", 0.2);
            await ToSignal(transition, "finished");
        }

        // Run post-combat events
        if (dialogic != null)
        {
            if (didPlayerWin && VictoryTimeline != null)
            {
                dialogic.Call("start_timeline", VictoryTimeline);
            }
            else if (!didPlayerWin && LossTimeline != null)
            {
                dialogic.Call("start_timeline", LossTimeline);
            }

            await ToSignal(dialogic, "timeline_ended");
        }
    }

    public override async void Run()
    {
        Execute();
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        base.Run();
    }
}
