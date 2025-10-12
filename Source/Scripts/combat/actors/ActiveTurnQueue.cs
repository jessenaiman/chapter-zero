using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Responsible for Battlers, managing their turns, action order, and lifespans.
/// The ActiveTurnQueue sorts Battlers neatly into a queue as they are ready to act. Time is paused
/// as Battlers act and is resumed once actors have finished acting. The queue ceases once the player
/// or enemy Battlers have been felled, signaling that the combat has finished.
/// Note: the turn queue defers action/target selection to either AI or player input. While
/// time is slowed for player input, it is not stopped completely which may result in an AI Battler
/// acting while the player is taking their turn.
/// </summary>
public partial class ActiveTurnQueue : Node2D
{
    /// <summary>
    /// The slow-motion value of time_scale used when the player is navigating action/target menus.
    /// </summary>
    private const float SlowTimeScale = 0.05f;

    /// <summary>
    /// Emitted once a player has won or lost a battle, indicating whether or not it may be considered a
    /// victory for the player. All combat animations have finished playing.
    /// </summary>
    [Signal]
    public delegate void CombatFinishedEventHandler(bool isPlayerVictory);

    /// <summary>
    /// Allows pausing the Active Time Battle during combat intro, a cutscene, or combat end.
    /// </summary>
    public bool IsActive
    {
        get => isActive;
        set
        {
            if (value != isActive)
            {
                isActive = value;
                foreach (var battler in Battlers.GetAllBattlers())
                {
                    if (battler != null)
                    {
                        battler.IsActive = isActive;
                    }
                }
            }
        }
    }

    /// <summary>
    /// A list of the combat participants, in BattlerList form. This object is created by the turn
    /// queue from children Battlers and then made available to other combat systems.
    /// </summary>
    public BattlerList Battlers { get; private set; }

    // Private fields
    private bool isActive = true;
    private float timeScale = 1.0f;
    private BattlerAction activeAction = null;
    private bool isPlayerMenuOpen = false;

    // Battlers may select their action at any point, where they will be cached in this dictionary.
    // The battlers will not act, however, until the queue receives their ready_to_act signal and validates the action.
    // Key = Battler, Value = named dictionary with two entries: 'action' and 'targets'.
    private Dictionary<Battler, Dictionary<string, object>> cachedActions = new Dictionary<Battler, Dictionary<string, object>>();

    public override void _Ready()
    {
        // The time scale slows down whenever the user is picking an action. Connect to UI signals here
        // to adjust accordingly to whether or not the player is navigating the target/action menus.
        // Note: In a real implementation, you'd connect to actual UI signals
        // CombatEvents.PlayerBattlerSelected += OnPlayerBattlerSelected;
        // CombatEvents.ActionSelected += OnActionSelected;

        // Combat participants are children of the ActiveTurnQueue. Create the data structure that will
        // track battlers and be passed across states.
        var players = GetChildren().OfType<Battler>().Where(b => b.IsPlayer).ToArray();
        var enemies = GetChildren().OfType<Battler>().Where(b => !b.IsPlayer).ToArray();

        Battlers = new BattlerList(players, enemies);
        Battlers.BattlersDowned += OnCombatSideDowned;

        foreach (var battler in Battlers.GetAllBattlers())
        {
            // Setup Battler AIs to make use of the BattlerList object (needed to pick targets).
            if (battler.Ai != null)
            {
                battler.Ai.Setup(battler, Battlers);
            }

            // Battlers will act as their ready_to_act signal is emitted. The turn queue will allow them
            // to act if another action is not currently underway.
            battler.ReadyToAct += () => OnBattlerReadyToAct(battler);

            // Remove any cached actions whenever the Battler is downed.
            if (battler.Stats != null)
            {
                battler.Stats.HealthDepleted += () => cachedActions.Remove(battler);
            }
        }

        // The ActiveTurnQueue uses _process to wait for animations to finish at combat end, so disable
        // _process for now.
        SetProcess(false);

        // Don't begin combat until the state has been setup. I.e. intro animations, UI is ready, etc.
        IsActive = false;
    }

    /// <summary>
    /// The active turn queue waits until all battlers have finished their animations before emitting the
    /// finished signal.
    /// </summary>
    public override void _Process(double delta)
    {
        // Only track the animations of the losing team, as the winning team will animate their idle
        // poses indefinitely.
        var trackedBattlers = Battlers.HasPlayerWon ? Battlers.Enemies : Battlers.Players;

        foreach (var child in trackedBattlers)
        {
            // If there are still playing BattlerAnims, don't finish the battle yet.
            // Note: This is a simplified check - in a real implementation you'd check if animations are playing
            if (IsAnyAnimationPlaying(child))
            {
                return;
            }
        }

        // There are no defeat animations being played. Combat can now finish.
        SetProcess(false);
        EmitSignal(SignalName.CombatFinished, Battlers.HasPlayerWon);
    }

    /// <summary>
    /// Checks if any animations are playing for the given battler
    /// </summary>
    private bool IsAnyAnimationPlaying(Battler battler)
    {
        // This is a placeholder implementation - in a real game you'd check if animations are playing
        // For now, we'll return false to allow the combat to finish
        return false;
    }

    /// <summary>
    /// Callback when a battler is ready to act
    /// </summary>
    private void OnBattlerReadyToAct(Battler battler)
    {
        if (activeAction != null)
        {
            return;
        }

        // Check, first of all, to see if there is a cached action registered to this Battler.
        if (!cachedActions.ContainsKey(battler))
        {
            return;
        }

        var actionData = cachedActions[battler];
        var action = (BattlerAction)actionData["action"];
        var targets = (List<Battler>)actionData["targets"];

        // Filter targets to only include those that can be targeted by the action
        var validTargets = targets.Where(t => action.CanTargetBattler(t)).ToList();

        if (action.CanExecute(battler, validTargets))
        {
            cachedActions.Remove(battler);
            activeAction = action;

            // Execute the action asynchronously
            ExecuteActionAsync(battler, action, validTargets);
        }
    }

    /// <summary>
    /// Executes an action asynchronously
    /// </summary>
    private async Task ExecuteActionAsync(Battler battler, BattlerAction action, List<Battler> targets)
    {
        await battler.ActAsync(action, targets.ToArray());
        activeAction = null;
    }

    /// <summary>
    /// Callback when a combat side is downed
    /// </summary>
    private void OnCombatSideDowned()
    {
        // On combat end, a number of systems will animate out (the UI fades, for example).
        // However, the battlers also end with animations: celebration or death animations. The
        // combat cannot truly end until these animations have finished, so wait for children
        // Battlers to 'wrap up' from this point onwards.
        // This is done with the ActiveTurnQueue's process function, which will check each frame
        // to see if the losing team's final animations have finished.
        SetProcess(true);

        // Don't allow anyone else to act.
        IsActive = false;
    }

    /// <summary>
    /// Callback when player battler is selected
    /// </summary>
    private void OnPlayerBattlerSelected(Battler battler)
    {
        isPlayerMenuOpen = battler != null;
        UpdateTimeScale();
    }

    /// <summary>
    /// Callback when action is selected
    /// </summary>
    private void OnActionSelected(BattlerAction action, Battler source, List<Battler> targets)
    {
        // If the action passed is null, unqueue the source Battler from any cached actions.
        if (action == null)
        {
            cachedActions.Remove(source);
        }
        else
        {
            // Otherwise, cache the action for execution whenever the Battler is ready to act.
            var actionData = new Dictionary<string, object>
            {
                ["action"] = action,
                ["targets"] = targets
            };
            cachedActions[source] = actionData;

            // Note that the battler only emits its ready_to_act signal once upon reaching 100
            // readiness. If the battler is currently ready to act, re-emit the signal now.
            if (source.IsReadyToAct())
            {
                OnBattlerReadyToAct(source);
            }
        }
    }

    /// <summary>
    /// Updates the time scale based on the current state
    /// </summary>
    private void UpdateTimeScale()
    {
        if (activeAction != null)
        {
            timeScale = 0;
        }
        else if (isPlayerMenuOpen)
        {
            timeScale = SlowTimeScale;
        }
        else
        {
            timeScale = 1;
        }

        // Apply the time scale to all battlers
        foreach (var battler in Battlers.GetAllBattlers())
        {
            if (battler != null)
            {
                battler.TimeScale = timeScale;
            }
        }
    }
}
