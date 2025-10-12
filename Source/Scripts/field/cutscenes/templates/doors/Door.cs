using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timer = Godot.Timer; // Resolve ambiguity between Godot.Timer and System.Threading.Timer

/// <summary>
/// A door that can be locked or unlocked, blocking or allowing passage through its cell.
/// When a door is locked, it places a dummy gamepiece on the cell to block movement.
/// When unlocked, the door allows passage and can open when triggered.
/// </summary>
[GlobalClass]
public partial class Door : AreaTransition
{
    /// <summary>
    /// Whether the door is currently locked and blocking movement.
    /// </summary>
    [Export]
    public bool IsLocked
    {
        get => isLocked;
        set
        {
            if (value != isLocked)
            {
                isLocked = value;

                // Wait for the node to be ready if not already inside the tree
                if (!IsInsideTree())
                {
                    CallDeferred(nameof(SetDoorLockState), value);
                    return;
                }

                SetDoorLockState(value);
            }
        }
    }

    // NewMusic is already defined in the parent AreaTransition class, so we don't need to redefine it
    // The Door class will use the NewMusic property from its parent AreaTransition class

    /// <summary>
    /// Keep a reference to the object used to block movement through a locked door.
    /// Note that this gamepiece has no animation, movement, etc. It exists to occupy a board cell.
    /// </summary>
    private Gamepiece _dummyGp = null;

    /// <summary>
    /// Animation player for door animations.
    /// </summary>
    private AnimationPlayer _anim;

    /// <summary>
    /// Sprite for the closed door.
    /// </summary>
    private Sprite2D _closedDoor;

    /// <summary>
    /// Whether the door is currently locked.
    /// </summary>
    private bool isLocked = false;

    public override void _Ready()
    {
        base._Ready();

        // Get references to child nodes
        _anim = GetNode<AnimationPlayer>("AnimationPlayer");
        _closedDoor = GetNode<Sprite2D>("Area2D/ClosedDoor");
    }

    /// <summary>
    /// Set the locked state of the door.
    /// </summary>
    /// <param name="locked">Whether the door should be locked</param>
    private void SetDoorLockState(bool locked)
    {
        if (locked)
        {
            // If locked and no dummy gamepiece exists, create one
            if (_dummyGp == null)
            {
                // In Godot C#, we can't directly preload scenes like in GDScript
                // Instead, we'll need to create a basic gamepiece or load from resource
                _dummyGp = new Gamepiece();
                _dummyGp.Name = "CellBlocker";
                _closedDoor.AddChild(_dummyGp);
            }
        }
        else
        {
            Open();
            // Remove the dummy gamepiece if it exists
            if (_dummyGp != null && _dummyGp.IsInsideTree())
            {
                _dummyGp.QueueFree();
                _dummyGp = null;
            }
        }
    }

    /// <summary>
    /// Open the door if it's closed.
    /// </summary>
    public async void Open()
    {
        // Do not open the door if it is already open.
        if (!_closedDoor.Visible)
        {
            return;
        }
        else if (IsLocked)
        {
            if (_anim != null)
            {
                _anim.Play("locked");
                await ToSignal(_anim, AnimationPlayer.SignalName.AnimationFinished);
            }
        }
        else
        {
            if (_anim != null)
            {
                _anim.Play("open");
                await ToSignal(_anim, AnimationPlayer.SignalName.AnimationFinished);
            }
        }
    }

    /// <summary>
    /// Activate the door's logic.
    /// Opens the door if it's closed before proceeding with the area transition.
    /// </summary>
    /// <param name="triggeringObject">The object that triggered this door</param>
    public override async void Activate(Node2D triggeringObject)
    {
        // Only open the door if it is closed.
        if (_closedDoor.Visible && _anim != null)
        {
            _anim.Play("open");
            await ToSignal(_anim, AnimationPlayer.SignalName.AnimationFinished);
        }

        // Call the parent implementation to handle the area transition
        base.Activate(triggeringObject);
    }

    /// <summary>
    /// Callback when the blackout occurs during area transition.
    /// Plays new music if specified.
    /// </summary>
    protected override async Task _OnBlackout()
    {
        // Play new music if specified
        if (base.NewMusic != null)
        {
            Music.Instance?.Play(base.NewMusic);
        }

        // Call the parent implementation
        await base._OnBlackout();
    }
}
