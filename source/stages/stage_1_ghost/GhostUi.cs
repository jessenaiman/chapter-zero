// <copyright file="GhostTerminal.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Stages.Stage1;

#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable IDE1006 // Naming rule violation

namespace OmegaSpiral.Source.Stages.Stage1;

/// <summary>
/// Stage 1 Ghost Terminal - extends NarrativeUi with Stage 1-specific behavior.
/// Loads ghost.yaml via GhostTerminalCinematicDirector and presents the opening sequence.
/// Implements Dreamweaver selection through philosophical questions with score tracking.
/// </summary>
[GlobalClass]
public partial class GhostTerminal : NarrativeUi
{
    private GhostTerminalCinematicDirector? _director;
    private NarrativeScript? _script;
    private int _currentMomentIndex;
    private Dictionary<string, int> _dreamweaverScores = new()
    {
        { "light", 0 },
        { "shadow", 0 },
        { "ambition", 0 }
    };

    /// <summary>
    /// Loads the ghost.yaml narrative script using the GhostTerminalCinematicDirector.
    /// </summary>
    /// <returns><see langword="true"/> if loaded successfully, <see langword="false"/> otherwise.</returns>
    protected virtual bool LoadGhostScript()
    {
        try
        {
            _director = new GhostTerminalCinematicDirector();
            var plan = _director.GetPlan();
            _script = plan.Script;
            _currentMomentIndex = 0;

            GD.Print($"[GhostTerminal] Loaded: {_script.Title}");
            GD.Print($"[GhostTerminal] Speaker: {_script.Speaker}");
            GD.Print($"[GhostTerminal] Moments: {_script.Moments.Count}");

            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[GhostTerminal] Failed to load ghost.yaml: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Presents the next narrative moment from ghost.yaml.
    /// Handles different moment types: narrative, question, composite.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    protected virtual async Task PresentNextMomentAsync()
    {
        if (_script == null || _currentMomentIndex >= _script.Moments.Count)
        {
            // Finished all moments - determine dominant Dreamweaver and transition
            await CompleteGhostSequenceAsync().ConfigureAwait(false);
            return;
        }

        var moment = _script.Moments[_currentMomentIndex];
        _currentMomentIndex++;

        switch (moment.Type.ToLowerInvariant())
        {
            case "narrative":
                await PresentNarrativeMomentAsync(moment).ConfigureAwait(false);
                break;

            case "question":
                await PresentQuestionMomentAsync(moment).ConfigureAwait(false);
                break;

            case "composite":
                await PresentCompositeMomentAsync(moment).ConfigureAwait(false);
                break;

            default:
                GD.PrintErr($"[GhostTerminal] Unknown moment type: {moment.Type}");
                await PresentNextMomentAsync().ConfigureAwait(false);
                break;
        }
    }

    /// <summary>
    /// Presents a narrative moment (type: "narrative") - displays lines with optional visual preset.
    /// </summary>
    /// <param name="moment">The narrative moment to present.</param>
    /// <returns>A task representing the async operation.</returns>
    protected virtual async Task PresentNarrativeMomentAsync(ContentBlock moment)
    {
        // Apply visual preset if specified (e.g., "boot_sequence", "secret_reveal")
        if (!string.IsNullOrEmpty(moment.VisualPreset) && ShaderController != null)
        {
            await ShaderController.ApplyVisualPresetAsync(moment.VisualPreset).ConfigureAwait(false);
        }

        // Display narrative lines using base class text renderer
        if (moment.Lines != null && moment.Lines.Count > 0)
        {
            foreach (var line in moment.Lines)
            {
                await AppendTextAsync(line).ConfigureAwait(false);
            }
        }

        // Handle pause if specified
        if (moment.Pause.HasValue && moment.Pause.Value > 0)
        {
            await Task.Delay((int)(moment.Pause.Value * 1000)).ConfigureAwait(false);
        }

        // Auto-advance to next moment
        await PresentNextMomentAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Presents a question moment (type: "question") - shows prompt and waits for player choice.
    /// </summary>
    /// <param name="moment">The question moment to present.</param>
    /// <returns>A task representing the async operation.</returns>
    protected virtual async Task PresentQuestionMomentAsync(ContentBlock moment)
    {
        // Display setup lines if present
        if (moment.Setup != null && moment.Setup.Count > 0)
        {
            foreach (var line in moment.Setup)
            {
                // TODO: Use NarrativeTerminal's display method
                GD.Print($"[GhostTerminal] {line}");
            }
        }

        // Display prompt
        if (!string.IsNullOrEmpty(moment.Prompt))
        {
            GD.Print($"[GhostTerminal] {moment.Prompt}");
        }

        // Display context
        if (!string.IsNullOrEmpty(moment.Context))
        {
            GD.Print($"[GhostTerminal] {moment.Context}");
        }

        // Present choices and wait for selection
        if (moment.Options != null && moment.Options.Count > 0)
        {
            // TODO: Use NarrativeTerminal's choice presentation system
            // For now, just track scores and auto-advance
            var selectedOption = moment.Options[0]; // Placeholder
            TrackDreamweaverScores(selectedOption);
        }

        // Auto-advance to next moment (TODO: wait for actual player input)
        await PresentNextMomentAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Presents a composite moment (type: "composite") - setup + question + continuation.
    /// </summary>
    /// <param name="moment">The composite moment to present.</param>
    /// <returns>A task representing the async operation.</returns>
    protected virtual async Task PresentCompositeMomentAsync(ContentBlock moment)
    {
        // Display setup narrative
        if (moment.Setup != null && moment.Setup.Count > 0)
        {
            foreach (var line in moment.Setup)
            {
                GD.Print($"[GhostTerminal] {line}");
            }
        }

        // Present the question part
        await PresentQuestionMomentAsync(moment).ConfigureAwait(false);

        // Display continuation narrative after choice
        if (moment.Continuation != null && moment.Continuation.Count > 0)
        {
            foreach (var line in moment.Continuation)
            {
                GD.Print($"[GhostTerminal] {line}");
            }
        }
    }

    /// <summary>
    /// Tracks Dreamweaver scores from a player's choice.
    /// Updates light, shadow, and ambition scores based on the choice's scoring.
    /// </summary>
    /// <param name="choice">The choice option that was selected.</param>
    protected virtual void TrackDreamweaverScores(ChoiceOption choice)
    {
        if (choice.Scores == null)
        {
            return;
        }

        foreach (var score in choice.Scores)
        {
            var threadKey = score.Key.ToLowerInvariant();
            if (_dreamweaverScores.ContainsKey(threadKey))
            {
                _dreamweaverScores[threadKey] += score.Value;
                GD.Print($"[GhostTerminal] {threadKey}: +{score.Value} = {_dreamweaverScores[threadKey]}");
            }
        }
    }

    /// <summary>
    /// Completes the Ghost Terminal sequence and determines the dominant Dreamweaver.
    /// Transitions to the next stage based on the player's accumulated scores.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    protected virtual async Task CompleteGhostSequenceAsync()
    {
        // Determine dominant Dreamweaver
        var dominantThread = GetDominantDreamweaver();
        GD.Print($"[GhostTerminal] Dominant thread: {dominantThread}");
        GD.Print($"[GhostTerminal] Scores - Light: {_dreamweaverScores["light"]}, Shadow: {_dreamweaverScores["shadow"]}, Ambition: {_dreamweaverScores["ambition"]}");

        // TODO: Set the Dreamweaver thread in GameState
        // TODO: Transition to next stage

        await Task.CompletedTask;
    }

    /// <summary>
    /// Determines which Dreamweaver has the highest score.
    /// Returns "balance" if no thread exceeds 60% of total points (hidden fourth path).
    /// </summary>
    /// <returns>The dominant Dreamweaver thread name.</returns>
    protected virtual string GetDominantDreamweaver()
    {
        var totalPoints = _dreamweaverScores["light"] + _dreamweaverScores["shadow"] + _dreamweaverScores["ambition"];

        if (totalPoints == 0)
        {
            return "light"; // Default if no choices made
        }

        // Check for balance (no thread exceeds 60%)
        foreach (var score in _dreamweaverScores)
        {
            float percentage = (float)score.Value / totalPoints;
            if (percentage >= 0.6f)
            {
                return score.Key; // This thread is dominant
            }
        }

        // All threads are balanced - hidden fourth path
        return "balance";
    }
}
