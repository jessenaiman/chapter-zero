using Godot;
using System;

[Tool]
/// <summary>
/// An <see cref="InteractionPopup"/> that follows a moving <see cref="Gamepiece"/>.
///
/// This Popup must be a child of a <see cref="Gamepiece"/> to function.
///
/// Note that other popup types will jump to the occupied cell of the ancestor <see cref="Gamepiece"/>, whereas
/// MovingInteractionPopups sync their position to that of the gamepiece's graphical representation.
/// </summary>
public partial class MovingInteractionPopup : InteractionPopup
{
    private Gamepiece _gp;

    public override void _Ready()
    {
        base._Ready();

        // Do not follow anything in editor or if this object's parent is not of the correct type.
        if (Engine.IsEditorHint() || _gp == null)
        {
            SetProcess(false);
        }
    }

    public string[] GetConfigurationWarnings()
    {
        if (_gp == null)
        {
            return new string[] { "This popup must be a child of a Gamepiece node!" };
        }
        return new string[0];
    }

    public override void _Notification(int what)
    {
        base._Notification(what);

        if (what == NotificationParented)
        {
            _gp = GetParent() as Gamepiece;
            // UpdateConfigurationWarnings() - Godot doesn't have a direct equivalent in C#
        }
    }

    // Every process frame the popup sets its position to that of the graphical representation of the
    // gamepiece, appearing to follow the gamepiece around the field while still playing nicely with the
    // physics/interaction system.
    public override void _Process(double delta)
    {
        if (_gp != null)
        {
            Position = _gp.Follower.Position;
        }
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        _gp = GetParent() as Gamepiece;
    }
}
