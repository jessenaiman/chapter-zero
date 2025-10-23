
// <copyright file="UIBattlerTargetCursor.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;

namespace OmegaSpiral.Source.Scripts.Combat.UI.Cursors;
/// <summary>
/// Allows the player to choose the targets of a <see cref="BattlerAction"/>.
/// </summary>
[GlobalClass]
public partial class UIBattlerTargetingCursor : Node2D
{
    /// <summary>
    /// An empty array of <see cref="Battler"/>s passed to <see cref="TargetsSelected"/> when no target is selected.
    /// </summary>
    private static readonly Godot.Collections.Array<Battler> InvalidTargets = new Godot.Collections.Array<Battler>();

    /// <summary>
    /// The cursor scene that will be used to denote the active target[s].
    /// </summary>
    private readonly PackedScene cursorScene = GD.Load<PackedScene>("res://source/scripts/combat/ui/cursors/ui_menu_cursor.tscn");

    /// <summary>
    /// Emitted when the player has selected targets.
    /// If the player has pressed 'back' instead, <see cref="InvalidTargets"/> will be returned.
    /// In either case, the cursor will call QueueFree() after emitting this signal.
    /// </summary>
    /// <param name="selection">The array of selected <see cref="Battler"/> targets, or <see cref="InvalidTargets"/> if no target is selected.</param>
    [Signal]
    public delegate void TargetsSelectedEventHandler(Godot.Collections.Array selection);

    /// <summary>
    /// Gets or sets a value indicating whether whether the selected action should target all <see cref="Targets"/>, or only one from the array.
    /// Currently, this must be set to true or false before filling the <see cref="Targets"/> array.
    /// </summary>
    [Export]
    public bool TargetsAll { get; set; } = false;

    private Collection<Battler> targets = new Collection<Battler>();

    /// <summary>
    /// Gets or sets all possible targets for a given action. Generates cursor instances if <see cref="TargetsAll"/> is true.
    /// </summary>
    public Collection<Battler> Targets
    {
        get => this.targets;
        set
        {
            this.targets = value;
            if (this.targets.Count > 0)
            {
                if (this.currentTarget != null && !this.targets.Contains(this.currentTarget))
                {
                    this.currentTarget = this.targets[0];
                    if (this.currentTarget != null && this.secondaryCursors.ContainsKey(this.currentTarget))
                    {
                        this.secondaryCursors.Remove(this.currentTarget);
                    }
                }

                // The target list has changed, so "secondary" cursors need to accommodate the new list.
                // Any new Battlers will need a cursor and any removed Battlers should no longer be
                // targeted.
                if (this.TargetsAll)
                {
                    // Remove cursors over targets that are no longer in the target list.
                    var keysToRemove = new List<Battler>();
                    foreach (var battler in this.secondaryCursors.Keys)
                    {
                        if (!this.targets.Contains(battler))
                        {
                            this.secondaryCursors[battler].QueueFree();
                            keysToRemove.Add(battler);
                        }
                    }

                    foreach (var key in keysToRemove)
                    {
                        this.secondaryCursors.Remove(key);
                    }

                    // Add cursors to new targets, syncing the animation time.
                    foreach (var battler in this.targets)
                    {
                        if (!this.secondaryCursors.ContainsKey(battler) && battler != this.currentTarget)
                        {
                            var newCursor = this.CreateCursorOverBattler(battler);
                            if (this.cursor != null && newCursor != null)
                            {
                                var animationPosition = this.cursor.GetAnimationPosition();
                                newCursor.AdvanceAnimation(animationPosition);
                            }

                            if (newCursor != null)
                            {
                                this.secondaryCursors[battler] = newCursor;
                            }
                        }
                    }
                }

                // Due to processing the tween above, there is a single frame where the cursor will be
                // stuck at the origin (before the tween updates).
                // Therefore, defer calling Show() until after the tween will have processed.
                this.CallDeferred("Show");
            }
            else
            {
                this.currentTarget = null;
            }
        }
    }

    /// <summary>
    /// One of the entries specified by <see cref="Targets"/>, at which the cursor is located.
    /// </summary>
    private Battler? currentTarget;

    /// <summary>
    /// Gets or sets the currently selected <see cref="Battler"/> target for the cursor.
    /// </summary>
    /// <remarks>
    /// When set, moves the cursor to the new target's position. If set to <see langword="null"/>, hides the cursor.
    /// </remarks>
    public Battler? CurrentTarget
    {
        get => this.currentTarget;
        set
        {
            this.currentTarget = value;

            if (this.currentTarget == null)
            {
                this.Hide();
            }
            else if (this.cursor != null && this.currentTarget.Anim?.Top != null)
            {
                var topPosition = this.currentTarget.Anim.Top.GlobalPosition;
                this.cursor.MoveTo(topPosition);
            }
        }
    }

    /// <summary>
    /// The primary cursor instance, which is moved from target to target whenever <see cref="TargetsAll"/> is false.
    /// </summary>
    private UIMenuCursor? cursor;

    /// <summary>
    /// Secondary cursors, which are created whenever <see cref="TargetsAll"/> is true.
    /// They are children of the <see cref="UIBattlerTargetingCursor"/>. Dictionary keys are a <see cref="Battler"/> instance that
    /// corresponds with one of the targets. This allows the number of cursors to be updated as <see cref="Battler"/>
    /// state changes. In other words, if targets die or are added while the player is choosing targets, the cursors
    /// highlighting the targets will update accordingly.
    /// </summary>
    private Dictionary<Battler, UIMenuCursor> secondaryCursors = new Dictionary<Battler, UIMenuCursor>();

    /// <inheritdoc/>
    public override void _Ready()
    {
        if (this.Targets.Count == 0)
        {
            GD.PrintErr("The target cursor needs a non-empty target array!");
            return;
        }

        this.Hide();
        if (this.currentTarget != null)
        {
            this.cursor = this.CreateCursorOverBattler(this.currentTarget);
        }

        // If the Battler that is currently selecting targets is downed, close the cursor immediately.
        if (CombatEvents.Instance != null)
        {
            CombatEvents.Instance.PlayerBattlerSelected += (_) =>
            {
                this.SetProcessUnhandledInput(false);
                this.QueueFree();
            };
        }
    }

    /// <inheritdoc/>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionReleased("ui_accept"))
        {
            // Let the UI know which Battler(s) were selected.
            var highlightedTargets = new Godot.Collections.Array<Battler>();
            if (this.TargetsAll)
            {
                foreach (var target in this.Targets)
                {
                    highlightedTargets.Add(target);
                }
            }
            else if (this.currentTarget != null)
            {
                highlightedTargets.Add(this.currentTarget);
            }

            this.EmitSignal(SignalName.TargetsSelected, highlightedTargets);
            this.QueueFree();
        }
        else if (@event.IsActionReleased("back"))
        {
            this.EmitSignal(SignalName.TargetsSelected, InvalidTargets);
            this.QueueFree();
        }

        // Other keypresses may indicate that the player is selecting another target.
        else if (@event is InputEventKey)
        {
            // Don't move anything if ALL targets are currently being targeted.
            if (this.TargetsAll)
            {
                return;
            }

            var direction = Vector2.Zero;
            if (@event.IsActionReleased("ui_left"))
            {
                direction = Vector2.Left;
            }
            else if (@event.IsActionReleased("ui_right"))
            {
                direction = Vector2.Right;
            }
            else if (@event.IsActionReleased("ui_up"))
            {
                direction = Vector2.Up;
            }
            else if (@event.IsActionReleased("ui_down"))
            {
                direction = Vector2.Down;
            }

            if (direction != Vector2.Zero)
            {
                var newTarget = this.FindClosestTarget(direction);
                if (newTarget != null)
                {
                    this.CurrentTarget = newTarget;
                }
            }
        }
    }

    /// <summary>
    /// Creates the actual cursor object over a given battler, or all battlers if targets_all is true.
    /// </summary>
    /// <param name="target">The battler to create the cursor over.</param>
    /// <returns>The created cursor.</returns>
    private UIMenuCursor? CreateCursorOverBattler(Battler? target)
    {
        if (target?.Anim == null)
        {
            return null;
        }

        var newCursor = this.cursorScene.Instantiate() as UIMenuCursor;
        if (newCursor != null && target?.Anim?.Top != null)
        {
            this.AddChild(newCursor);

            newCursor.Rotation = Mathf.Pi / 2;
            var topPosition = target.Anim.Top.GlobalPosition;
            newCursor.GlobalPosition = topPosition;
        }
        return newCursor;
    }

    /// <summary>
    /// Finds the closest battler (that is also in _targets) in a given direction.
    /// Returns null if no battlers may be found in that direction.
    /// </summary>
    /// <param name="direction">The direction to search in.</param>
    /// <returns>The closest battler in the given direction, or null if none found.</returns>
    private Battler? FindClosestTarget(Vector2 direction)
    {
        if (this.currentTarget == null)
        {
            GD.PrintErr("Target cursor cannot find closest target to a null battler! Current" +
                "target must be non-null.");
            return null;
        }

        Battler? newTarget = null;
        float distanceToNewTarget = float.PositiveInfinity;

        // First, we find all targetable battlers in a given direction.
        var candidates = new List<Battler>();
        foreach (var battler in this.targets)
        {
            // Don't select the current target.
            if (battler == this.currentTarget)
            {
                continue;
            }

            // We're going to search within a 90-degree triangle (matching the direction vector +/- 45
            // degrees) for battlers. Anything outside is excluded, as it is found in a different
            // direction.
            var currentTargetPosition = this.currentTarget.GlobalPosition;
            var battlerPosition = battler.GlobalPosition;
            var vectorToBattler = battlerPosition - currentTargetPosition;
            if (Mathf.Abs(direction.AngleTo(vectorToBattler)) <= Mathf.Pi / 2.0f)
            {
                candidates.Add(battler);
            }
        }

        // Secondly, loop over all candidates and find the one closest to the current battler.
        // That is our new target.
        foreach (var battler in candidates)
        {
            var battlerPosition = battler.GlobalPosition;
            var distanceToBattler = this.GlobalPosition.DistanceTo(battlerPosition);
            if (distanceToBattler < distanceToNewTarget)
            {
                distanceToNewTarget = distanceToBattler;
                newTarget = battler;
            }
        }

        return newTarget;
    }
}
