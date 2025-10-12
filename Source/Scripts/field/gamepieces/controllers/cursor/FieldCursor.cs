using Godot;

/// <summary>
/// Handles mouse/touch events for the field gamestate.
///
/// The field cursor's role is to determine whether or not the input event occurs over a particular
/// cell and how that cell should be highlighted.
/// </summary>
public partial class FieldCursor : TileMapLayer
{
    /// <summary>
    /// Emitted when the highlighted cell changes to a new value. An invalid cell is indicated by a value
    /// of <see cref="Gameboard.InvalidCell"/>.
    /// </summary>
    [Signal]
    public delegate void FocusChangedEventHandler(Vector2I oldFocus, Vector2I newFocus);

    /// <summary>
    /// Emitted when a cell is selected via input event.
    /// </summary>
    [Signal]
    public delegate void SelectedEventHandler(Vector2I selectedCell);

    private Vector2I _focus = Gameboard.InvalidCell;

    /// <summary>
    /// The cell currently highlighted by the cursor.
    ///
    /// A focus of <see cref="Gameboard.InvalidCell"/> indicates that there is no highlight.
    /// </summary>
    [Export]
    public Vector2I Focus
    {
        get => _focus;
        set => SetFocus(value);
    }

    public override void _Ready()
    {
        // Connect to field events for input pause handling
        // FieldEvents.InputPaused += OnInputPaused;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            GetViewport().SetInputAsHandled();
            SetFocus(GetCellUnderMouse());
        }
        else if (@event.IsActionReleased("select"))
        {
            GetViewport().SetInputAsHandled();

            var cellUnderMouse = GetCellUnderMouse();
            EmitSignal(SignalName.Selected, cellUnderMouse);
            // FieldEvents.EmitCellSelected(cellUnderMouse);
        }
    }

    /// <summary>
    /// Change the highlighted cell to a new value. A value of <see cref="Gameboard.InvalidCell"/> will
    /// indicate that there is no highlighted cell.
    /// </summary>
    /// <param name="value">The new focus cell</param>
    private void SetFocus(Vector2I value)
    {
        if (value == _focus)
        {
            return;
        }

        var oldFocus = _focus;
        _focus = value;

        Clear();

        if (_focus != Gameboard.InvalidCell)
        {
            SetCell(_focus, 0, new Vector2I(1, 5), 0);
        }

        EmitSignal(SignalName.FocusChanged, oldFocus, _focus);
        // FieldEvents.EmitCellHighlighted(_focus);
    }

    /// <summary>
    /// Convert mouse/touch coordinates to a gameboard cell.
    /// </summary>
    /// <returns>The cell under the mouse cursor, or InvalidCell if none</returns>
    private Vector2I GetCellUnderMouse()
    {
        // The mouse coordinates need to be corrected for any scale or position changes in the scene.
        var mousePosition = (GetGlobalMousePosition() - GlobalPosition) / GlobalScale;
        var cellUnderMouse = Gameboard.PixelToCell(mousePosition);

        if (!Gameboard.Pathfinder.HasCell(cellUnderMouse))
        {
            cellUnderMouse = Gameboard.InvalidCell;
        }

        return cellUnderMouse;
    }

    /// <summary>
    /// Handle input pause events.
    /// </summary>
    /// <param name="isPaused">Whether input is paused</param>
    private void OnInputPaused(bool isPaused)
    {
        SetProcessUnhandledInput(!isPaused);

        if (isPaused)
        {
            SetFocus(Gameboard.InvalidCell);
        }
    }
}
