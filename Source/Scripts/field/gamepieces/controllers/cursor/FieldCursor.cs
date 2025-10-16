namespace OmegaSpiral.Source.Scripts.Field.Gamepieces.Controllers.Cursor;

// <copyright file="FieldCursor.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Field.Gameboard;

/// <summary>
/// Handles mouse/touch events for the field gamestate.
///
/// The field cursor's role is to determine whether or not the input event occurs over a particular
/// cell and how that cell should be highlighted.
/// </summary>
[GlobalClass]
public partial class FieldCursor : TileMapLayer
{
    private Vector2I focus = Gameboard.InvalidCell;

    /// <summary>
    /// Emitted when the highlighted cell changes to a new value. An invalid cell is indicated by a value
    /// of <see cref="Gameboard.InvalidCell"/>.
    /// </summary>
    /// <param name="oldFocus">The previous focused cell.</param>
    /// <param name="newFocus">The new focused cell.</param>
    [Signal]
    public delegate void FocusChangedEventHandler(Vector2I oldFocus, Vector2I newFocus);

    /// <summary>
    /// Emitted when a cell is selected via input event.
    /// </summary>
    /// <param name="selectedCell">The selected cell.</param>
    [Signal]
    public delegate void SelectedEventHandler(Vector2I selectedCell);

    /// <summary>
    /// Gets or sets the cell currently highlighted by the cursor.
    ///
    /// A focus of <see cref="Gameboard.InvalidCell"/> indicates that there is no highlight.
    /// </summary>
    [Export]
    public Vector2I Focus
    {
        get => this.focus;
        set => this.SetFocus(value);
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Connect to field events for input pause handling
        // FieldEvents.InputPaused += OnInputPaused;
    }

    /// <inheritdoc/>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event == null)
        {
            return;
        }

        if (@event is InputEventMouseMotion)
        {
            this.GetViewport().SetInputAsHandled();
            this.SetFocus(this.GetCellUnderMouse());
        }
        else if (@event.IsActionReleased("select"))
        {
            this.GetViewport().SetInputAsHandled();

            var cellUnderMouse = this.GetCellUnderMouse();
            this.EmitSignal(SignalName.Selected, cellUnderMouse);

            // FieldEvents.EmitCellSelected(cellUnderMouse);
        }
    }

    /// <summary>
    /// Change the highlighted cell to a new value. A value of <see cref="Gameboard.InvalidCell"/> will
    /// indicate that there is no highlighted cell.
    /// </summary>
    /// <param name="value">The new focus cell.</param>
    private void SetFocus(Vector2I value)
    {
        if (value == this.focus)
        {
            return;
        }

        var oldFocus = this.focus;
        this.focus = value;

        this.Clear();

        if (this.focus != Gameboard.InvalidCell)
        {
            this.SetCell(this.focus, 0, new Vector2I(1, 5), 0);
        }

        this.EmitSignal(SignalName.FocusChanged, oldFocus, this.focus);

        // FieldEvents.EmitCellHighlighted(_focus);
    }

    /// <summary>
    /// Convert mouse/touch coordinates to a gameboard cell.
    /// </summary>
    /// <returns>The cell under the mouse cursor, or InvalidCell if none.</returns>
    private Vector2I GetCellUnderMouse()
    {
        // The mouse coordinates need to be corrected for any scale or position changes in the scene.
        var mousePosition = (this.GetGlobalMousePosition() - this.GlobalPosition) / this.GlobalScale;

        // Get the Gameboard instance from the scene tree
        var gameboard = this.GetTree().Root.GetNode<Gameboard>("Gameboard");
        if (gameboard == null)
        {
            return Gameboard.InvalidCell;
        }

        var cellUnderMouse = gameboard.PixelToCell(mousePosition);

        if (gameboard.PathFinder == null || !gameboard.PathFinder.HasCell(cellUnderMouse))
        {
            cellUnderMouse = Gameboard.InvalidCell;
        }

        return cellUnderMouse;
    }

    /// <summary>
    /// Handle input pause events.
    /// </summary>
    /// <param name="isPaused">Whether input is paused.</param>
    private void OnInputPaused(bool isPaused)
    {
        this.SetProcessUnhandledInput(!isPaused);

        if (isPaused)
        {
            this.SetFocus(Gameboard.InvalidCell);
        }
    }
}
