// <copyright file="FieldEvents.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

using OmegaSpiral.Source.Scripts.Field;
using OmegaSpiral.Source.Scripts.Field.cutscenes;
using OmegaSpiral.Source.Scripts.Field.gamepieces.Controllers.Cursor;

namespace OmegaSpiral.Field;

/// <summary>
/// A signal bus to connect distant scenes to various field-exclusive events.
/// </summary>
[GlobalClass]
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
    /// <param name="cell">The cell position that was highlighted.</param>
    [Signal]
    public delegate void CellHighlightedEventHandler(Vector2I cell);

    /// <summary>
    /// Emitted when the player selects a cell on the field gameboard via the <see cref="FieldCursor"/>.
    /// </summary>
    /// <param name="cell">The cell position that was selected.</param>
    [Signal]
    public delegate void CellSelectedEventHandler(Vector2I cell);

    /// <summary>
    /// Emitted when the player selects a cell that is covered by an <see cref="Interaction"/>.
    /// </summary>
    /// <param name="interaction">The interaction that was selected.</param>
    [Signal]
    public delegate void InteractionSelectedEventHandler(Interaction interaction);

    /// <summary>
    /// Emitted whenever a combat is triggered. This will lead to a transition from the field 'state' to
    /// a combat 'state'.
    /// </summary>
    /// <param name="arena">The packed scene for the combat arena.</param>
    [Signal]
    public delegate void CombatTriggeredEventHandler(PackedScene arena);

    /// <summary>
    /// Emitted when a <see cref="Cutscene"/> begins, signalling that the player should yield control of their
    /// character to the cutscene code.
    /// </summary>
    [Signal]
    public delegate void CutsceneBeganEventHandler();

    /// <summary>
    /// Emitted when a <see cref="Cutscene"/> ends, restoring normal mode of play.
    /// </summary>
    [Signal]
    public delegate void CutsceneEndedEventHandler();

    /// <summary>
    /// Emitted whenever ALL input within the field state is to be paused or resumed.
    /// Typically emitted by combat, dialogues, etc.
    /// </summary>
    /// <param name="isPaused">Whether input is paused (<see langword="true"/>) or resumed (<see langword="false"/>).</param>
    [Signal]
    public delegate void InputPausedEventHandler(bool isPaused);

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Set this object's process priority to a very high number.
        // We want the Field Event manager's _process method to run after all Gamepieces and Controllers.
        base.ProcessPriority = ProcessPriority;
    }
}
