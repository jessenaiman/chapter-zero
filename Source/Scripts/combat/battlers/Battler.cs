using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// A playable combatant that carries out <see cref="BattlerAction"/>s as its <see cref="Readiness"/> charges.
///
/// Battlers are the playable characters or enemies that show up in battle. They have <see cref="BattlerStats"/>,
/// a list of <see cref="BattlerAction"/>s to choose from, and respond to a variety of stimuli such as status
/// effects and <see cref="BattlerHit"/>s, which typically deal damage or heal the Battler.
///
/// <br/><br/>Battlers have <see cref="BattlerAnim"/>ation children which play out the Battler's actions.
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
    /// Forwarded from the receiving of <see cref="BattlerStats.HealthDepleted"/>.
    /// </summary>
    [Signal]
    public delegate void HealthDepletedEventHandler();

    /// <summary>
    /// Emitted when taking damage or being healed from a <see cref="BattlerHit"/>.
    /// <br/><br/>Note the difference between this and <see cref="BattlerStats.HealthChanged"/>:
    /// 'hit_received' is always the direct result of an action, requiring graphical feedback.
    /// </summary>
    [Signal]
    public delegate void HitReceivedEventHandler(int value);

    /// <summary>
    /// Emitted whenever a hit targeting this battler misses.
    /// </summary>
    [Signal]
    public delegate void HitMissedEventHandler();

    /// <summary>
    /// Emitted when the battler's `_readiness` changes.
    /// </summary>
    [Signal]
    public delegate void ReadinessChangedEventHandler(float newValue);

    /// <summary>
    /// Emitted when the battler is ready to take a turn.
    /// </summary>
    [Signal]
    public delegate void ReadyToActEventHandler();

    /// <summary>
    /// Emitted when modifying <see cref="IsSelected"/>. The user interface will react to this for
    /// player-controlled battlers.
    /// </summary>
    [Signal]
    public delegate void SelectionToggledEventHandler(bool value);

    /// <summary>
    /// The name of the node group that will contain all combat Battlers.
    /// </summary>
    public const string Group = "_COMBAT_BATTLER_GROUP";

    /// <summary>
    /// A Battler must have <see cref="BattlerStats"/> to act and receive actions.
    /// </summary>
    [Export]
    public BattlerStats Stats { get; set; } = null!;

    /// <summary>
    /// Each action's data stored in this array represents an action the battler can perform.
    /// These can be anything: attacks, healing spells, etc.
    /// </summary>
    [Export]
    public BattlerAction[] Actions { get; set; } = new BattlerAction[0];

    private PackedScene _battlerAnimScene;
    /// <summary>
    /// Each Battler is shown on the screen by a <see cref="BattlerAnim"/> object. The object is created dynamically
    /// from a PackedScene, which must yield a <see cref="BattlerAnim"/> object when instantiated.
    /// </summary>
    [Export]
    public PackedScene BattlerAnimScene
    {
        get => _battlerAnimScene;
        set
        {
            _battlerAnimScene = value;

            if (!IsInsideTree())
            {
                // In Godot 4.x, we can't await in a property setter, so we'll defer the initialization
                CallDeferred("_InitializeBattlerAnim");
                return;
            }

            _InitializeBattlerAnim();
        }
    }

    private void _InitializeBattlerAnim()
    {
        // Free an already existing BattlerAnim.
        if (Anim != null)
        {
            Anim.QueueFree();
            Anim = null;
        }

        // Add the new BattlerAnim class as a child and link it to this Battler instance.
        if (BattlerAnimScene != null)
        {
            // Normally we could wrap a check for battler_anim_scene's type (should be BattlerAnim)
            // in a call to assert, but we want the following code to run in the editor and clean up
            // dynamically if the user drops an incorrect PackedScene (i.e. not a BattlerAnim) into
            // the battler_anim_scene slot.
            var newScene = BattlerAnimScene.Instantiate();
            Anim = newScene as BattlerAnim;
            if (Anim == null)
            {
                GD.PushWarning($"Battler '{Name}' cannot accept '{newScene.Name}' as " +
                    $"battler_anim_scene. '{newScene.Name}' is not a BattlerAnim!");
                newScene.Free();
                BattlerAnimScene = null;
                return;
            }

            AddChild(Anim);
            var facing = IsPlayer ? BattlerDirection.Left : BattlerDirection.Right;
            Anim.Setup(this, facing);
        }
    }

    private PackedScene _aiScene;
    /// <summary>
    /// A CombatAI object that will determine the Battler's combat behaviour.
    /// If the battler has an `ai_scene`, we will instantiate it and let the AI make decisions.
    /// If not, the player controls this battler. The system should allow for ally AIs.
    /// </summary>
    [Export]
    public PackedScene AiScene
    {
        get => _aiScene;
        set
        {
            _aiScene = value;

            if (_aiScene != null)
            {
                // In the editor, check to make sure that the value set to ai_scene is actually a
                // CombatAI object.
                var newInstance = _aiScene.Instantiate();
                if (Engine.IsEditorHint())
                {
                    if (newInstance is not CombatAI)
                    {
                        GD.PrintErr($"Cannot assign '{newInstance.Name}' to Battler '{Name}'" +
                            " as ai_scene property. Assigned PackedScene is not a CombatAI type!");
                        _aiScene = null;
                    }
                    newInstance.Free();
                }
                else
                {
                    Ai = newInstance as CombatAI;
                    if (Ai != null)
                    {
                        AddChild(Ai);
                    }
                }
            }
        }
    }

    private PackedScene _actorScene;
    /// <summary>
    /// The <see cref="CombatActor"/> object that will determine the Battler's combat behavior. A Battler without an CombatActor
    /// is just a dummy object that will not take turns or perform actions.
    /// </summary>
    [Export]
    public PackedScene ActorScene
    {
        get => _actorScene;
        set
        {
            if (value == _actorScene)
            {
                return;
            }

            _actorScene = value;

            if (!IsInsideTree())
            {
                // In Godot 4.x, we can't await in a property setter, so we'll defer the initialization
                CallDeferred("_InitializeActor");
                return;
            }

            _InitializeActor();
        }
    }

    private void _InitializeActor()
    {
        if (Actor != null)
        {
            Actor.QueueFree();
            Actor = null;
        }

        if (ActorScene != null)
        {
            var newInstance = ActorScene.Instantiate();
            Actor = newInstance as CombatActor;
            if (Actor != null)
            {
                AddChild(Actor);
            }
        }
    }

    //TODO: Battler.is_player is redundant. Use that defined by CombatActor instead.
    private bool _isPlayer = false;
    /// <summary>
    /// Player battlers are controlled by the player.
    /// </summary>
    [Export]
    public bool IsPlayer
    {
        get => _isPlayer;
        set
        {
            _isPlayer = value;
            if (Anim != null)
            {
                var facing = _isPlayer ? BattlerDirection.Left : BattlerDirection.Right;
                Anim.Direction = facing;
            }
        }
    }

    /// <summary>
    /// Reference to the Battler's child <see cref="CombatActor"/>, which controls it's combat behaviour.
    /// Note that this value is assigned automatically with reference to <see cref="ActorScene"/>.
    /// </summary>
    public CombatActor? Actor { get; private set; } = null;

    /// <summary>
    /// Reference to this Battler's child <see cref="CombatAI"/> node, if applicable.
    /// </summary>
    public CombatAI? Ai { get; private set; } = null;

    /// <summary>
    /// Reference to this Battler's child <see cref="BattlerAnim"/> node.
    /// </summary>
    public BattlerAnim? Anim { get; private set; } = null;

    // TODO: this property belongs with the CombatActor.
    private bool _isActive = true;
    /// <summary>
    /// If `false`, the battler will not be able to act.
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            SetProcess(_isActive);
        }
    }

    private float _timeScale = 1.0f;
    /// <summary>
    /// The turn queue will change this property when another battler is acting.
    /// </summary>
    public float TimeScale
    {
        get => _timeScale;
        set => _timeScale = value;
    }

    private bool _isSelected = false;
    /// <summary>
    /// If `true`, the battler is selected, which makes it move forward.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (value)
            {
                System.Diagnostics.Debug.Assert(IsSelectable);
            }

            _isSelected = value;
            EmitSignal(SignalName.SelectionToggled, _isSelected);
        }
    }

    private bool _isSelectable = true;
    /// <summary>
    /// If `false`, the battler cannot be targeted by any action.
    /// </summary>
    public bool IsSelectable
    {
        get => _isSelectable;
        set
        {
            _isSelectable = value;
            if (!_isSelectable)
            {
                IsSelected = false;
            }
        }
    }

    private float _readiness = 0.0f;
    /// <summary>
    /// When this value reaches `100.0`, the battler is ready to take their turn.
    /// </summary>
    public float Readiness
    {
        get => _readiness;
        set
        {
            _readiness = value;
            EmitSignal(SignalName.ReadinessChanged, _readiness);

            if (_readiness >= 100.0f)
            {
                _readiness = 100.0f;
                Stats.Energy += 1;

                EmitSignal(SignalName.ReadyToAct);
                SetProcess(false);
            }
        }
    }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            SetProcess(false);
        }
        else
        {
            System.Diagnostics.Debug.Assert(Stats != null, $"Battler {Name} does not have stats assigned!");

            AddToGroup(Group);

            // Resources are NOT unique, so treat the currently assigned BattlerStats as a prototype.
            // That is, copy what it is now and use the copy, so that the original remains unaltered.
            Stats = (BattlerStats)Stats.Duplicate();
            Stats.Initialize();
            Stats.HealthDepleted += () =>
            {
                IsActive = false;
                IsSelectable = false;
                EmitSignal(SignalName.HealthDepleted);
            };
        }
    }

    public override void _Process(double delta)
    {
        Readiness += Stats.Speed * (float)delta * TimeScale;
    }

    public async Task ActAsync(BattlerAction action, Battler[] targets = null!)
    {
        if (targets == null) targets = new Battler[0];

        SetProcess(false);

        Stats.Energy -= action.EnergyCost;

        // action.Execute() almost certainly is a coroutine.
        await action.Execute(this, targets);
        if (Stats.Health > 0)
        {
            Readiness = action.ReadinessSaved;

            if (IsActive)
            {
                SetProcess(true);
            }
        }

        CallDeferred("EmitActionFinished");
    }

    private void EmitActionFinished()
    {
        EmitSignal(SignalName.ActionFinished);
    }

    public void TakeHit(BattlerHit hit)
    {
        if (hit.IsSuccessful())
        {
            EmitSignal(SignalName.HitReceived, hit.Damage);
            Stats.Health -= hit.Damage;
        }
        else
        {
            EmitSignal(SignalName.HitMissed);
        }
    }

    public bool IsReadyToAct()
    {
        return Readiness >= 100.0f;
    }
}
