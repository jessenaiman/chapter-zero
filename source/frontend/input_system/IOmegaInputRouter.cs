using System;

namespace OmegaSpiral.Source.InputSystem;

/// <summary>
/// Exposes semantic input events for the Omega UI system.
/// Implementations translate raw input into the high-level actions the GameManager expects.
/// </summary>
public interface IOmegaInputRouter
{
    /// <summary>
    /// Raised when the player requests the menu to toggle visibility.
    /// </summary>
    event Action MenuToggleRequested;

    /// <summary>
    /// Raised when the player requests to navigate upward through focusable UI.
    /// </summary>
    event Action NavigateUpRequested;

    /// <summary>
    /// Raised when the player requests to navigate downward through focusable UI.
    /// </summary>
    event Action NavigateDownRequested;

    /// <summary>
    /// Raised when the player confirms their current selection.
    /// </summary>
    event Action ConfirmRequested;

    /// <summary>
    /// Raised when the player cancels or backs out of the current UI.
    /// </summary>
    event Action BackRequested;
}
