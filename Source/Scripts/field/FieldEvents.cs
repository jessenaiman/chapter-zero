using Godot;
using System;

/// <summary>
/// A signal bus to connect distant scenes to various field-exclusive events.
/// </summary>
public partial class FieldEvents : Node
{
    /// <summary>
    /// Set this object's process priority to a very high number.
    /// We want the Field Event manager's _process method to run after all Gamepieces and Controllers.
    /// </summary>
    public new const int ProcessPriority = 99999999;

    /// <summary>
    /// Emitted when the cursor moves to a new position on the field gameboard.
    /// </summary>
    [Signal]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1822:Mark members as static")]
    public delegate void CellHighlightedEventHandler(Vector2I cell);

    /// <summary>
    /// Emitted when the player selects a cell on the field gameboard via the <see cref="FieldCursor"/>.
    /// </summary>
    [Signal]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1822:Mark members as static")]
    public delegate void CellSelectedEventHandler(Vector2I cell);

    /// <summary>
    /// Emitted when the player selects a cell that is covered by an <see cref="Interaction"/>.
    /// </summary>
    [Signal]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1822:Mark members as static")]
    public delegate void InteractionSelectedEventHandler(Interaction interaction);

    /// <summary>
    /// Emitted whenever a combat is triggered. This will lead to a transition from the field 'state' to
    /// a combat 'state'.
    /// </summary>
    [Signal]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1822:Mark members as static")]
    public delegate void CombatTriggeredEventHandler(PackedScene arena);

    /// <summary>
    /// Emitted when a <see cref="Cutscene"/> begins, signalling that the player should yield control of their
    /// character to the cutscene code.
    /// </summary>
    [Signal]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1822:Mark members as static")]
    public delegate void CutsceneBeganEventHandler();

    /// <summary>
    /// Emitted when a <see cref="Cutscene"/> ends, restoring normal mode of play.
    /// </summary>
    [Signal]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1822:Mark members as static")]
    public delegate void CutsceneEndedEventHandler();

    /// <summary>
    /// Emitted whenever ALL input within the field state is to be paused or resumed.
    /// Typically emitted by combat, dialogues, etc.
    /// </summary>
    [Signal]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1822:Mark members as static")]
    public delegate void InputPausedEventHandler(bool isPaused);

    public override void _Ready()
    {
        // Set this object's process priority to a very high number.
        // We want the Field Event manager's _process method to run after all Gamepieces and Controllers.
        base.ProcessPriority = ProcessPriority;
    }
}
