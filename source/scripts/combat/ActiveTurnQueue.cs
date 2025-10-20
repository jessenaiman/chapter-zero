
using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;
using OmegaSpiral.Source.Combat.Actions;

namespace OmegaSpiral.Source.Scripts.Combat;
/// <summary>
/// Responsible for Battlers, managing their turns, action order, and lifespans.
/// The ActiveTurnQueue sorts Battlers neatly into a queue as they are ready to act. Time is paused
/// as Battlers act and is resumed once actors have finished acting. The queue ceases once the player
/// or enemy Battlers have been felled, signaling that the combat has finished.
/// Note: the turn queue defers action/target selection to either AI or player input. While
/// time is slowed for player input, it is not stopped completely which may result in an AI Battler
/// acting while the player is taking their turn.
/// </summary>
[GlobalClass]
public partial class ActiveTurnQueue : Node2D
{
    /// <summary>
    /// The slow-motion value of time_scale used when the player is navigating action/target menus.
    /// </summary>
    public const float SlowTimeScale = 0.05f;

    /// <summary>
    /// Emitted once a player has won or lost a battle, indicating whether or not it may be considered a
    /// victory for the player. All combat animations have finished playing.
    /// </summary>
    /// <param name="isPlayerVictory">Indicates whether the player has won (<see langword="true"/>) or lost (<see langword="false"/>) the battle.</param>
    [Signal]
    public delegate void CombatFinishedEventHandler(bool isPlayerVictory);

    private bool isActive = true;
    private float timeScale = 1.0f;
    private BattlerAction? activeAction;
    private bool isPlayerMenuOpen;
    private Dictionary<Battler, Dictionary<string, object>> cachedActions = new();

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
                if (Battlers != null)
                {
                    foreach (var battler in Battlers.GetAllBattlers())
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
    public BattlerList? Battlers { get; private set; }

    /// <summary>
    /// Initializes the ActiveTurnQueue when it enters the scene tree.
    /// Sets up signal connections, creates battler lists, and configures AI.
    /// </summary>
    public override void _Ready()
    {
        // The time scale slows down whenever the user is picking an action. Connect to UI signals here
        // to adjust accordingly to whether or not the play is navigating the target/action menus.
        if (CombatEvents.Instance != null)
        {
            CombatEvents.Instance.PlayerBattlerSelected += OnPlayerBattlerSelected;
            // TODO: Reconnect after Godot regenerates signal bindings
            // CombatEvents.Instance.ActionSelectedEvent += OnActionSelected;
        }

        // Combat participants are children of the ActiveTurnQueue. Create the data structure that will
        // track battlers and be passed across states.
        var players = GetChildren().OfType<Battler>().Where(b => b.IsPlayer).ToArray();
        var enemies = GetChildren().OfType<Battler>().Where(b => !b.IsPlayer).ToArray();

        Battlers = new BattlerList(players, enemies);
        if (Battlers != null)
        {
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
                battler.HealthDepleted += () => cachedActions.Remove(battler);
            }
        }

        // The ActiveTurnQueue uses _process to wait for animations to finish at combat end, so disable
        // _process for now.
        SetProcess(false);

        // Don't begin combat until the state has been setup. I.e. intro animations, UI is ready, etc.
        IsActive = false;
    }

    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        if (Battlers == null)
            return;

        // Only track the animations of the losing team, as the winning team will animate their idle
        // poses indefinitely.
        var trackedBattlers = Battlers.HasPlayerWon ? Battlers.Enemies : Battlers.Players;

        foreach (var battler in trackedBattlers)
        {
            // If there are still playing BattlerAnims, don't finish the battle yet.
            if (battler.Anim?.IsPlaying ?? false)
            {
                return;
            }
        }

        // There are no defeat animations being played. Combat can now finish.
        SetProcess(false);
        EmitSignal(SignalName.CombatFinished, Battlers.HasPlayerWon);
    }

    private void OnPlayerBattlerSelected(Battler battler)
    {
        isPlayerMenuOpen = true;
        UpdateTimeScale();
    }
    private void OnActionSelected(BattlerAction action, Battler source, Battler[] targets)
    {
        // Cache the action for execution whenever the Battler is ready to act.
        cachedActions[source] = new Dictionary<string, object>
        {
            ["action"] = action,
            ["targets"] = targets
        };

        // Note that the battler only emits its ready_to_act signal once upon reaching 100
        // readiness. If the battler is currently ready to act, re-emit the signal now.
        if (source.IsReadyToAct())
        {
            CallDeferred("emit_signal", "ready_to_act");
        }
    }

    private void OnCombatSideDowned()
    {
        // Begin the shutdown sequence for the combat, flagging end of the combat logic.
        // This is called immediately when the player has either won or lost the combat.
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

    private void UpdateTimeScale()
    {
        float newTimeScale;
        if (activeAction != null)
        {
            newTimeScale = 0;
        }
        else if (isPlayerMenuOpen)
        {
            newTimeScale = SlowTimeScale;
        }
        else
        {
            newTimeScale = 1;
        }

        if (Mathf.IsEqualApprox(timeScale, newTimeScale))
            return;

        timeScale = newTimeScale;
        if (Battlers != null)
        {
            foreach (var battler in Battlers.GetAllBattlers())
            {
                battler.TimeScale = timeScale;
            }
        }
    }

    private async void OnBattlerReadyToAct(Battler battler)
    {
        if (activeAction != null)
            return;

        // Check, first of all, to see if there is a cached action registered to this Battler.
        if (!cachedActions.TryGetValue(battler, out var actionData))
            return;

        var action = (BattlerAction) actionData["action"];
        var targets = ((Battler[]) actionData["targets"]).Where(target => action.CanTargetBattler(target)).ToArray();

        if (action.CanExecute(battler, targets))
        {
            cachedActions.Remove(battler);
            activeAction = action;
            await battler.ActAsync(action, targets);
            activeAction = null;
        }
    }
}
