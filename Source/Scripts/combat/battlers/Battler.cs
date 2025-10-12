// <copyright file="Battler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Godot;

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
    /// Gets or sets a Battler must have <see cref="BattlerStats"/> to act and receive actions.
    /// </summary>
    [Export]
    public BattlerStats Stats { get; set; } = null!;

    /// <summary>
    /// Gets or sets each action's data stored in this array represents an action the battler can perform.
    /// These can be anything: attacks, healing spells, etc.
    /// </summary>
    [Export]
    public BattlerAction[] Actions { get; set; } = Array.Empty<BattlerAction>();

    private PackedScene battlerAnimScene;

    /// <summary>
    /// Gets or sets each Battler is shown on the screen by a <see cref="BattlerAnim"/> object. The object is created dynamically
    /// from a PackedScene, which must yield a <see cref="BattlerAnim"/> object when instantiated.
    /// </summary>
    [Export]
    public PackedScene BattlerAnimScene
    {
        get => this.battlerAnimScene;
        set
        {
            this.battlerAnimScene = value;

            if (!this.IsInsideTree())
            {
                // In Godot 4.x, we can't await in a property setter, so we'll defer the initialization
                this.CallDeferred("_InitializeBattlerAnim");
                return;
            }

            this.InitializeBattlerAnim();
        }
    }

    private void InitializeBattlerAnim()
    {
        // Free an already existing BattlerAnim.
        if (this.Anim != null)
        {
            this.Anim.QueueFree();
            this.Anim = null;
        }

        // Add the new BattlerAnim class as a child and link it to this Battler instance.
        if (this.BattlerAnimScene != null)
        {
            // Normally we could wrap a check for battler_anim_scene's type (should be BattlerAnim)
            // in a call to assert, but we want the following code to run in the editor and clean up
            // dynamically if the user drops an incorrect PackedScene (i.e. not a BattlerAnim) into
            // the battler_anim_scene slot.
            var newScene = this.BattlerAnimScene.Instantiate();
            this.Anim = newScene as BattlerAnim;
            if (this.Anim == null)
            {
                GD.PushWarning($"Battler '{this.Name}' cannot accept '{newScene.Name}' as " +
                    $"battler_anim_scene. '{newScene.Name}' is not a BattlerAnim!");
                newScene.Free();
                this.BattlerAnimScene = null;
                return;
            }

            this.AddChild(this.Anim);
            var facing = this.IsPlayer ? BattlerDirection.Left : BattlerDirection.Right;
            this.Anim.Setup(this, facing);
        }
    }

    private PackedScene aiScene;

    /// <summary>
    /// Gets or sets a CombatAI object that will determine the Battler's combat behaviour.
    /// If the battler has an `ai_scene`, we will instantiate it and let the AI make decisions.
    /// If not, the player controls this battler. The system should allow for ally AIs.
    /// </summary>
    [Export]
    public PackedScene AiScene
    {
        get => this.aiScene;
        set
        {
            this.aiScene = value;

            if (this.aiScene != null)
            {
                // In the editor, check to make sure that the value set to ai_scene is actually a
                // CombatAI object.
                var newInstance = this.aiScene.Instantiate();
                if (Engine.IsEditorHint())
                {
                    if (newInstance is not CombatAI)
                    {
                        GD.PrintErr($"Cannot assign '{newInstance.Name}' to Battler '{this.Name}'" +
                            " as ai_scene property. Assigned PackedScene is not a CombatAI type!");
                        this.aiScene = null;
                    }

                    newInstance.Free();
                }
                else
                {
                    this.Ai = newInstance as CombatAI;
                    if (this.Ai != null)
                    {
                        this.AddChild(this.Ai);
                    }
                }
            }
        }
    }

    private PackedScene actorScene;

    /// <summary>
    /// Gets or sets the <see cref="CombatActor"/> object that will determine the Battler's combat behavior. A Battler without an CombatActor
    /// is just a dummy object that will not take turns or perform actions.
    /// </summary>
    [Export]
    public PackedScene ActorScene
    {
        get => this.actorScene;
        set
        {
            if (value == this.actorScene)
            {
                return;
            }

            this.actorScene = value;

            if (!this.IsInsideTree())
            {
                // In Godot 4.x, we can't await in a property setter, so we'll defer the initialization
                this.CallDeferred("_InitializeActor");
                return;
            }

            this.InitializeActor();
        }
    }

    private void InitializeActor()
    {
        if (this.Actor != null)
        {
            this.Actor.QueueFree();
            this.Actor = null;
        }

        if (this.ActorScene != null)
        {
            var newInstance = this.ActorScene.Instantiate();
            this.Actor = newInstance as CombatActor;
            if (this.Actor != null)
            {
                this.AddChild(this.Actor);
            }
        }
    }

    // TODO: Battler.is_player is redundant. Use that defined by CombatActor instead.
    private bool isPlayer;

    /// <summary>
    /// Gets or sets a value indicating whether player battlers are controlled by the player.
    /// </summary>
    [Export]
    public bool IsPlayer
    {
        get => this.isPlayer;
        set
        {
            this.isPlayer = value;
            if (this.Anim != null)
            {
                var facing = this.isPlayer ? BattlerDirection.Left : BattlerDirection.Right;
                this.Anim.Direction = facing;
            }
        }
    }

    /// <summary>
    /// Gets reference to the Battler's child <see cref="CombatActor"/>, which controls it's combat behaviour.
    /// Note that this value is assigned automatically with reference to <see cref="ActorScene"/>.
    /// </summary>
    public CombatActor? Actor { get; private set; }

    /// <summary>
    /// Gets reference to this Battler's child <see cref="CombatAI"/> node, if applicable.
    /// </summary>
    public CombatAI? Ai { get; private set; }

    /// <summary>
    /// Gets reference to this Battler's child <see cref="BattlerAnim"/> node.
    /// </summary>
    public BattlerAnim? Anim { get; private set; }

    // TODO: this property belongs with the CombatActor.
    private bool isActive = true;

    /// <summary>
    /// Gets or sets a value indicating whether if `false`, the battler will not be able to act.
    /// </summary>
    public bool IsActive
    {
        get => this.isActive;
        set
        {
            this.isActive = value;
            this.SetProcess(this.isActive);
        }
    }

    private float timeScale = 1.0f;

    /// <summary>
    /// Gets or sets the turn queue will change this property when another battler is acting.
    /// </summary>
    public float TimeScale
    {
        get => this.timeScale;
        set => this.timeScale = value;
    }

    private bool isSelected;

    /// <summary>
    /// Gets or sets a value indicating whether if `true`, the battler is selected, which makes it move forward.
    /// </summary>
    public bool IsSelected
    {
        get => this.isSelected;
        set
        {
            if (value)
            {
                System.Diagnostics.Debug.Assert(this.IsSelectable);
            }

            this.isSelected = value;
            this.EmitSignal(SignalName.SelectionToggled, this.isSelected);
        }
    }

    private bool isSelectable = true;

    /// <summary>
    /// Gets or sets a value indicating whether if `false`, the battler cannot be targeted by any action.
    /// </summary>
    public bool IsSelectable
    {
        get => this.isSelectable;
        set
        {
            this.isSelectable = value;
            if (!this.isSelectable)
            {
                this.IsSelected = false;
            }
        }
    }

    private float readiness;

    /// <summary>
    /// Gets or sets when this value reaches `100.0`, the battler is ready to take their turn.
    /// </summary>
    public float Readiness
    {
        get => this.readiness;
        set
        {
            this.readiness = value;
            this.EmitSignal(SignalName.ReadinessChanged, this.readiness);

            if (this.readiness >= 100.0f)
            {
                this.readiness = 100.0f;
                this.Stats.Energy += 1;

                this.EmitSignal(SignalName.ReadyToAct);
                this.SetProcess(false);
            }
        }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            this.SetProcess(false);
        }
        else
        {
            System.Diagnostics.Debug.Assert(this.Stats != null, $"Battler {this.Name} does not have stats assigned!");

            this.AddToGroup(Group);

            // Resources are NOT unique, so treat the currently assigned BattlerStats as a prototype.
            // That is, copy what it is now and use the copy, so that the original remains unaltered.
            this.Stats = (BattlerStats)this.Stats.Duplicate();
            this.Stats.Initialize();
            this.Stats.HealthDepleted += () =>
            {
                this.IsActive = false;
                this.IsSelectable = false;
                this.EmitSignal(SignalName.HealthDepleted);
            };
        }
    }

    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        this.Readiness += this.Stats.Speed * (float)delta * this.TimeScale;
    }

    public async Task ActAsync(BattlerAction action, Battler[] targets = null!)
    {
        if (targets == null)
        {
            targets = Array.Empty<Battler>();
        }

        this.SetProcess(false);

        this.Stats.Energy -= action.EnergyCost;

        // action.Execute() almost certainly is a coroutine.
        await action.Execute(this, targets).ConfigureAwait(false);
        if (this.Stats.Health > 0)
        {
            this.Readiness = action.ReadinessSaved;

            if (this.IsActive)
            {
                this.SetProcess(true);
            }
        }

        this.CallDeferred("EmitActionFinished");
    }

    private void EmitActionFinished()
    {
        this.EmitSignal(SignalName.ActionFinished);
    }

    public void TakeHit(BattlerHit hit)
    {
        if (hit.IsSuccessful())
        {
            this.EmitSignal(SignalName.HitReceived, hit.Damage);
            this.Stats.Health -= hit.Damage;
        }
        else
        {
            this.EmitSignal(SignalName.HitMissed);
        }
    }

    public bool IsReadyToAct()
    {
        return this.Readiness >= 100.0f;
    }
}
