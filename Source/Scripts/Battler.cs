using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// A playable combatant that carries out BattlerActions as its readiness charges.
/// Battlers are the playable characters or enemies that show up in battle. They have BattlerStats,
/// a list of BattlerActions to choose from, and respond to a variety of stimuli such as status
/// effects and BattlerHits, which typically deal damage or heal the Battler.
/// </summary>
[Tool]
public partial class Battler : Node2D
{
    /// <summary>
    /// Emitted when the battler finished their action and arrived back at their rest position.
    /// </summary>
    [Signal]
    public delegate void ActionFinishedEventHandler();

    /// <summary>
    /// Emitted when the battler's readiness changes.
    /// </summary>
    [Signal]
    public delegate void ReadinessChangedEventHandler(float newValue);

    /// <summary>
    /// Emitted when the battler is ready to take a turn.
    /// </summary>
    [Signal]
    public delegate void ReadyToActEventHandler();

    /// <summary>
    /// Emitted when modifying IsSelected. The user interface will react to this for
    /// player-controlled battlers.
    /// </summary>
    [Signal]
    public delegate void SelectionToggledEventHandler(bool value);

    /// <summary>
    /// The name of the node group that will contain all combat Battlers.
    /// </summary>
    public const string Group = "_COMBAT_BATTLER_GROUP";

    /// <summary>
    /// A Battler must have BattlerStats to act and receive actions.
    /// </summary>
    [Export]
    public BattlerStats Stats { get; set; } = null;

    /// <summary>
    /// Each action's data stored in this array represents an action the battler can perform.
    /// These can be anything: attacks, healing spells, etc.
    /// </summary>
    [Export]
    public BattlerAction[] Actions { get; set; } = new BattlerAction[0];

    /// <summary>
    /// A CombatAI object that will determine the Battler's combat behaviour.
    /// If the battler has an AiScene, we will instantiate it and let the AI make decisions.
    /// If not, the player controls this battler. The system should allow for ally AIs.
    /// </summary>
    [Export]
    public PackedScene AiScene { get; set; }

    /// <summary>
    /// Player battlers are controlled by the player.
    /// </summary>
    [Export]
    public bool IsPlayer { get; set; } = false;

    /// <summary>
    /// If false, the battler will not be able to act.
    /// </summary>
    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;
            SetProcess(isActive);
        }
    }

    /// <summary>
    /// The turn queue will change this property when another battler is acting.
    /// </summary>
    public float TimeScale { get; set; } = 1.0f;

    /// <summary>
    /// If true, the battler is selected, which makes it move forward.
    /// </summary>
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (value)
            {
                // assert(is_selectable)
            }

            isSelected = value;
            EmitSignal(SignalName.SelectionToggled, isSelected);
        }
    }

    /// <summary>
    /// If false, the battler cannot be targeted by any action.
    /// </summary>
    public bool IsSelectable
    {
        get => isSelectable;
        set
        {
            isSelectable = value;
            if (!isSelectable)
            {
                IsSelected = false;
            }
        }
    }

    /// <summary>
    /// When this value reaches 100.0, the battler is ready to take their turn.
    /// </summary>
    public float Readiness
    {
        get => readiness;
        set
        {
            readiness = value;
            EmitSignal(SignalName.ReadinessChanged, readiness);

            if (readiness >= 10.0f)
            {
                readiness = 100.0f;
                Stats.Energy += 1;

                EmitSignal(SignalName.ReadyToAct);
                SetProcess(false);
            }
        }
    }

    // Private fields to maintain the actual values
    private bool isActive = true;
    private bool isSelected = false;
    private bool isSelectable = true;
    private float readiness = 0.0f;
    private bool wasActive = true;

    // Reference to this Battler's child CombatAI node, if applicable.
    public CombatAI Ai { get; set; } = null;

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            SetProcess(false);
        }
        else
        {
            if (Stats == null)
            {
                GD.PrintErr($"Battler {Name} does not have stats assigned!");
            }

            AddToGroup(Group);

            // Resources are NOT unique, so treat the currently assigned BattlerStats as a prototype.
            // That is, copy what it is now and use the copy, so that the original remains unaltered.
            // Note: In C#, we would typically handle this differently, but we'll duplicate the stats here
            Stats.Initialize();

            Stats.HealthDepleted += OnStatsHealthDepleted;
        }
    }

    public override void _Process(double delta)
    {
        Readiness += Stats.Speed * (float)delta * TimeScale;
    }

    /// <summary>
    /// Performs an action with the specified targets.
    /// </summary>
    public async Task ActAsync(BattlerAction action, List<Battler>? targets = null)
    {
        if (targets == null) targets = new List<Battler>();

        SetProcess(false);

        Stats.Energy -= action.EnergyCost;

        // Execute the action
        action.Execute(this, targets);

        if (Stats.Health > 0)
        {
            Readiness = action.ReadinessSaved;

            if (IsActive)
            {
                SetProcess(true);
            }
        }

        // Emit action finished signal after a delay to allow for animations
        await Task.Delay(10);
        EmitSignal(SignalName.ActionFinished);
    }

    /// <summary>
    /// Applies a hit to this battler, dealing damage or healing.
    /// </summary>
    public void TakeHit(BattlerHit hit)
    {
        if (hit.IsSuccessful(this))
        {
            Stats.Health -= hit.Damage;
        }
        else
        {
            // Hit missed - you might want to emit a signal or handle this case
        }
    }

    /// <summary>
    /// Checks if the battler is ready to act.
    /// </summary>
    public bool IsReadyToAct()
    {
        return Readiness >= 100.0f;
    }

    /// <summary>
    /// Callback when the battler's stats health is depleted.
    /// </summary>
    private void OnStatsHealthDepleted()
    {
        IsActive = false;
        IsSelectable = false;
    }
}
