// <copyright file="Cutscene.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// A cutscene stops field gameplay to run a scripted event.
///
/// A cutscene may be thought of as the videogame equivalent of a short scene in a film. For example,
/// dialogue may be displayed, the scene may switch to show key NPCs performing an event, or the
/// inventory may be altered. Gameplay on the field is <b>stopped</b> until the cutscene concludes,
/// though this may span a combat scenario (e.g. epic bossfight).
///
/// Cutscenes may or may not have duration, and only one cutscene may be active at a time. Field
/// gameplay is stopped for the entire duration of the active cutscene.
/// Gameplay is stopped by emitting the global <see cref="FieldEvents.InputPaused"/> signal.
/// AI and player objects respond to this signal. For examples or responses to this signal, see
/// <see cref="GamepieceController.IsPaused"/> or <see cref="FieldCursor.OnInputPaused(bool)"/>.
///
/// Cutscenes are inherently custom and must be derived to do anything useful. They may be run via
/// the <see cref="Run"/> method and derived cutscenes will override the <see cref="Execute"/> method to
/// provide custom functionality.
///
/// Cutscenes are easily extensible, taking advantage of Godot's scene architecture. A variety of
/// cutscene templates are included out-of-the-box. See <see cref="Trigger"/> for a type of cutscene that is
/// triggered by contact with a gamepeiece. See <see cref="Interaction"/> for cutscene's that are triggered by
/// the player interaction with them via a keypress or touch. Several derived temlpates (for example,
/// open-able doors) are included in res://field/cutscenes/templates.
/// </summary>
[Tool]
public partial class Cutscene : Node2D
{
    // Indicates if a cutscene is currently running. <b>This member should not be set externally</b>.
    private static bool isCutsceneInProgress;

    protected static bool Is_cutscene_in_progress
    {
        get => isCutsceneInProgress;
        set
        {
            if (isCutsceneInProgress != value)
            {
                isCutsceneInProgress = value;

                // FieldEvents.input_paused.emit(value) - we'll need to implement this when FieldEvents is available
            }
        }
    }

    /// <summary>
    /// Returns true if a cutscene is currently running.
    /// </summary>
    /// <returns></returns>
    public static bool IsCutsceneInProgress()
    {
        return isCutsceneInProgress;
    }

    /// <summary>
    /// Execute the cutscene, if possible. Everything happening on the field gamestate will be
    /// paused and unpaused as the cutscene starts and finishes, respectively.
    /// </summary>
    public async void Run()
    {
        Is_cutscene_in_progress = true;

        // The _execute method may or may not be asynchronous, depending on the particular cutscene.
        await this.Execute();

        Is_cutscene_in_progress = false;
    }

    /// <summary>
    /// Play out the specific events of the cutscene.
    /// This method is intended to be overridden by derived cutscene types.
    /// May or may not be asynchronous.
    /// </summary>
    protected virtual async void Execute()
    {
        // Default implementation does nothing
        await this.ToSignal(this.GetTree(), SceneTree.SignalName.ProcessFrame);
    }
}
