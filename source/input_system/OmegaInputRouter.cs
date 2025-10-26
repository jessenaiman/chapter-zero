using Godot;
using System;

namespace OmegaSpiral.Source.InputSystem;

/// <summary>
/// Default implementation that maps Godot input actions to semantic UI events.
/// </summary>
[GlobalClass]
public partial class OmegaInputRouter : Node, IOmegaInputRouter
{
    /// <summary>
    /// Gets the default singleton node name used when attached to the scene tree.
    /// </summary>
    public const string DefaultNodeName = "OmegaInputRouter";

    /// <inheritdoc/>
    public event Action MenuToggleRequested = delegate { };

    /// <inheritdoc/>
    public event Action NavigateUpRequested = delegate { };

    /// <inheritdoc/>
    public event Action NavigateDownRequested = delegate { };

    /// <inheritdoc/>
    public event Action ConfirmRequested = delegate { };

    /// <inheritdoc/>
    public event Action BackRequested = delegate { };

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();
        if (string.IsNullOrEmpty(Name))
        {
            Name = DefaultNodeName;
        }
        SetProcessUnhandledInput(true);
    }

    /// <summary>
    /// Processes unhandled input events and raises semantic events.
    /// </summary>
    /// <param name="event">Input event received by Godot.</param>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            MenuToggleRequested.Invoke();
            return;
        }

        if (@event.IsActionPressed("ui_cancel"))
        {
            BackRequested.Invoke();
            return;
        }

        if (@event.IsActionPressed("ui_accept"))
        {
            ConfirmRequested.Invoke();
            return;
        }

        if (@event.IsActionPressed("ui_up"))
        {
            NavigateUpRequested.Invoke();
            return;
        }

        if (@event.IsActionPressed("ui_down"))
        {
            NavigateDownRequested.Invoke();
        }
    }
}
