// <copyright file="GhostUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Source.Scripts.Stages.Stage1;

namespace OmegaSpiral.Source.Stages.Stage1;

/// <summary>
/// Stage 1 Ghost Terminal UI - orchestrates the narrative sequence from ghost.yaml.
/// Extends NarrativeUi to inherit all presentation logic (shaders, text, choices).
/// Only handles Ghost-specific concerns: script loading and Dreamweaver score tracking.
/// </summary>
[GlobalClass]
public partial class GhostUi : NarrativeUi
{
    private NarrativeScript? _Script;
    private int _CurrentMomentIndex;

    /// <summary>
    /// Tracks accumulated Dreamweaver affiliation scores throughout the stage.
    /// </summary>
    private Dictionary<string, int> _DreamweaverScores = new()
    {
        { "light", 0 },
        { "shadow", 0 },
        { "ambition", 0 }
    };

    private SceneManager? _SceneManager;
    private GameState? _GameState;
    private GhostAudioManager? _AudioManager;

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready(); // Initialize NarrativeUi (which initializes OmegaThemedContainer + all presentation logic)

        // Initialize audio manager
        _AudioManager = new GhostAudioManager();
        AddChild(_AudioManager);

        // Get singleton references
        _SceneManager = GetNode<SceneManager>("/root/SceneManager");
        _GameState = GetNode<GameState>("/root/GameState");

        // Load ghost.yaml script
        if (!LoadGhostScript())
        {
            GD.PrintErr("[GhostTerminal] Failed to load script - cannot start sequence");
            return;
        }

        // Start the narrative sequence
        CallDeferred(nameof(StartGhostSequence));
    }

    /// <summary>
    /// Loads the ghost.yaml narrative script using the GhostCinematicDirector.
    /// </summary>
    /// <returns><see langword="true"/> if loaded successfully, <see langword="false"/> otherwise.</returns>
    private bool LoadGhostScript()
    {
        try
        {
            var director = new GhostCinematicDirector();
            var plan = director.GetPlan();
            _Script = plan.Script;
            _CurrentMomentIndex = 0;

            GD.Print($"[GhostTerminal] Loaded: {_Script.Title}");
            GD.Print($"[GhostTerminal] Speaker: {_Script.Speaker}");
            GD.Print($"[GhostTerminal] Moments: {_Script.Moments.Count}");

            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[GhostTerminal] Failed to load ghost.yaml: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Starts the Ghost Terminal narrative sequence.
    /// Called deferred to ensure all nodes are ready.
    /// </summary>
    private async void StartGhostSequence()
    {
        await PresentNextMomentAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Presents the next narrative moment from ghost.yaml.
    /// Delegates to base class for all presentation logic (shaders, text, choices).
    /// Only handles Ghost-specific concerns (score tracking, secret reveal ceremony).
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    private async Task PresentNextMomentAsync()
    {
        if (_Script == null || _CurrentMomentIndex >= _Script.Moments.Count)
        {
            await CompleteGhostSequenceAsync().ConfigureAwait(false);
            return;
        }

        var moment = _Script.Moments[_CurrentMomentIndex];
        _CurrentMomentIndex++;

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
    /// Presents a narrative moment (type: "narrative").
    /// Converts ContentBlock to NarrativeBeat and delegates to base class.
    /// Special handling for secret reveal ceremony.
    /// </summary>
    /// <param name="moment">The narrative moment to present.</param>
    /// <returns>A task representing the async operation.</returns>
    private async Task PresentNarrativeMomentAsync(ContentBlock moment)
    {
        // Special handling for secret reveal ceremony (stage-specific visual logic)
        if (string.Equals(moment.VisualPreset, "CODE_FRAGMENT_GLITCH_OVERLAY", StringComparison.OrdinalIgnoreCase))
        {
            await PresentSecretRevealCeremonyAsync(moment).ConfigureAwait(false);
            await PresentNextMomentAsync().ConfigureAwait(false);
            return;
        }

        // Convert ContentBlock to NarrativeBeat for base class
        var beats = ConvertToNarrativeBeats(moment).ToArray();
        await PlayNarrativeSequenceAsync(beats).ConfigureAwait(false);

        // Auto-advance to next moment
        await PresentNextMomentAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Presents a question moment (type: "question").
    /// Delegates to base class PresentChoicesAsync and tracks Dreamweaver scores.
    /// </summary>
    /// <param name="moment">The question moment to present.</param>
    /// <returns>A task representing the async operation.</returns>
    private async Task PresentQuestionMomentAsync(ContentBlock moment)
    {
        // Display setup lines if present
        if (moment.Setup != null && moment.Setup.Count > 0)
        {
            var setupBeats = moment.Setup.Select(line => new NarrativeUi.NarrativeBeat
            {
                Text = line,
                VisualPreset = null,
                DelaySeconds = 0
            }).ToArray();
            await PlayNarrativeSequenceAsync(setupBeats).ConfigureAwait(false);
        }

        // Present choices using base class (handles all choice UI logic)
        if (moment.Options != null && moment.Options.Count > 0)
        {
            var choiceTexts = moment.Options
                .Where(o => !string.IsNullOrEmpty(o.Text))
                .Select(o => o.Text!)
                .ToArray();

            var selectedText = await PresentChoicesAsync(moment.Prompt ?? string.Empty, choiceTexts).ConfigureAwait(false);

            // Find selected option and track Dreamweaver scores (Ghost-specific logic)
            var selectedOption = moment.Options.FirstOrDefault(o => o.Text == selectedText);
            if (selectedOption != null)
            {
                TrackDreamweaverScores(selectedOption);
            }
        }

        // Auto-advance to next moment
        await PresentNextMomentAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Presents a composite moment (type: "composite") - setup + question + continuation.
    /// </summary>
    /// <param name="moment">The composite moment to present.</param>
    /// <returns>A task representing the async operation.</returns>
    private async Task PresentCompositeMomentAsync(ContentBlock moment)
    {
        // Display setup narrative
        if (moment.Setup != null && moment.Setup.Count > 0)
        {
            var setupBeats = moment.Setup.Select(line => new NarrativeUi.NarrativeBeat
            {
                Text = line,
                VisualPreset = null,
                DelaySeconds = 0
            }).ToArray();
            await PlayNarrativeSequenceAsync(setupBeats).ConfigureAwait(false);
        }

        // Present the question part (delegates to base class for choices)
        if (moment.Options != null && moment.Options.Count > 0)
        {
            var choiceTexts = moment.Options
                .Where(o => !string.IsNullOrEmpty(o.Text))
                .Select(o => o.Text!)
                .ToArray();

            var selectedText = await PresentChoicesAsync(moment.Prompt ?? string.Empty, choiceTexts).ConfigureAwait(false);

            // Find selected option and track Dreamweaver scores
            var selectedOption = moment.Options.FirstOrDefault(o => o.Text == selectedText);
            if (selectedOption != null)
            {
                TrackDreamweaverScores(selectedOption);
            }
        }

        // Display continuation narrative after choice
        if (moment.Continuation != null && moment.Continuation.Count > 0)
        {
            var continuationBeats = moment.Continuation.Select(line => new NarrativeUi.NarrativeBeat
            {
                Text = line,
                VisualPreset = null,
                DelaySeconds = 0
            }).ToArray();
            await PlayNarrativeSequenceAsync(continuationBeats).ConfigureAwait(false);
        }

        // Auto-advance to next moment
        await PresentNextMomentAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Converts a ContentBlock narrative moment to NarrativeBeat format for base class.
    /// </summary>
    /// <param name="moment">The content block to convert.</param>
    /// <returns>An enumerable of narrative beats.</returns>
    private IEnumerable<NarrativeUi.NarrativeBeat> ConvertToNarrativeBeats(ContentBlock moment)
    {
        // Apply visual preset first
        if (!string.IsNullOrEmpty(moment.VisualPreset))
        {
            yield return new NarrativeUi.NarrativeBeat
            {
                Text = string.Empty,
                VisualPreset = moment.VisualPreset,
                DelaySeconds = 0.5f // Allow shader to settle
            };
        }
        else
        {
            // Default to terminal preset if none specified
            yield return new NarrativeUi.NarrativeBeat
            {
                Text = string.Empty,
                VisualPreset = "terminal",
                DelaySeconds = 0
            };
        }

        // Add all narrative lines as beats
        if (moment.Lines != null)
        {
            foreach (var line in moment.Lines)
            {
                yield return new NarrativeUi.NarrativeBeat
                {
                    Text = line,
                    VisualPreset = null,
                    DelaySeconds = 0
                };
            }
        }

        // Add pause at end if specified
        if (moment.Pause.HasValue && moment.Pause.Value > 0)
        {
            yield return new NarrativeUi.NarrativeBeat
            {
                Text = string.Empty,
                VisualPreset = null,
                DelaySeconds = moment.Pause.Value
            };
        }
    }

    /// <summary>
    /// Presents the secret reveal ceremony with symbol-by-symbol text reveal and audio sync.
    /// Implements the 4-second orchestrated buildup with Ωmega Spiral symbols.
    /// This is Ghost-specific stage logic that extends base class presentation.
    /// </summary>
    /// <param name="moment">The moment containing the secret reveal content.</param>
    /// <returns>A task representing the async operation.</returns>
    private async Task PresentSecretRevealCeremonyAsync(ContentBlock moment)
    {
        // Start the 4-second secret reveal audio buildup
        if (_AudioManager != null)
        {
            await _AudioManager.EnterSecretRevealAsync().ConfigureAwait(false);
        }

        // 4-second pause for audio buildup (CRT hum → sub-bass → modem fragments → silence → singing bowl)
        await Task.Delay(4000).ConfigureAwait(false);

        // Symbol-by-symbol reveal with individual overtones
        string[] symbols = { "∞", "◊", "Ω", "≋", "※" };
        for (int i = 0; i < symbols.Length; i++)
        {
            var symbol = symbols[i];

            // Play symbol-specific overtone (index 0-4)
            if (_AudioManager != null)
            {
                await _AudioManager.PlaySymbolOvertoneAsync(i).ConfigureAwait(false);
            }

            // Reveal symbol in text
            await AppendTextAsync(symbol).ConfigureAwait(false);

            // Brief pause between symbols
            await Task.Delay(800).ConfigureAwait(false);
        }

        // Add acknowledgment prompt
        await AppendTextAsync("\n\n[Press any key to acknowledge]").ConfigureAwait(false);

        // Wait for user input
        await WaitForAnyKeyAsync().ConfigureAwait(false);

        // Clear the acknowledgment prompt
        ClearText();

        // Auto-advance to next moment
        await PresentNextMomentAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Waits for any key press from the user.
    /// Used for acknowledgment prompts in the secret reveal ceremony.
    /// </summary>
    /// <returns>A task that completes when any key is pressed.</returns>
    protected async Task WaitForAnyKeyAsync()
    {
        // Poll for common acknowledgment keys (space, enter, any mouse click)
        while (true)
        {
            // Check for space or enter key
            if (Input.IsKeyPressed(Key.Space) || Input.IsKeyPressed(Key.Enter) || Input.IsKeyPressed(Key.KpEnter))
            {
                // Wait a bit to avoid registering the key press multiple times
                await Task.Delay(100).ConfigureAwait(false);
                break;
            }

            // Check for mouse click
            if (Input.IsMouseButtonPressed(MouseButton.Left) || Input.IsMouseButtonPressed(MouseButton.Right))
            {
                await Task.Delay(100).ConfigureAwait(false);
                break;
            }

            // Small delay to avoid busy waiting
            await Task.Delay(50).ConfigureAwait(false);
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
            if (_DreamweaverScores.ContainsKey(threadKey))
            {
                _DreamweaverScores[threadKey] += score.Value;
                GD.Print($"[GhostTerminal] {threadKey}: +{score.Value} = {_DreamweaverScores[threadKey]}");
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
        var dominantThread = this.GetDominantDreamweaver();
        GD.Print($"[GhostUi] Dominant thread: {dominantThread}");
        GD.Print($"[GhostUi] Scores - Light: {this._DreamweaverScores["light"]}, Shadow: {this._DreamweaverScores["shadow"]}, Ambition: {this._DreamweaverScores["ambition"]}");

        // Set the Dreamweaver thread in GameState
        if (this._GameState != null)
        {
            // TODO: Add DreamweaverThread property to GameState if it doesn't exist
            // this._GameState.DreamweaverThread = dominantThread;
            GD.Print($"[GhostUi] Set Dreamweaver thread in GameState: {dominantThread}");
        }

        // Transition to next stage
        if (this._SceneManager != null)
        {
            await this.AppendTextAsync("The choice has been made. Your path is set...").ConfigureAwait(false);
            await Task.Delay(2000).ConfigureAwait(false);

            // TODO: Update scene transition to actual next stage name
            // this._SceneManager.TransitionToScene("Scene2NethackSequence");
            GD.Print($"[GhostUi] Would transition to next stage here");
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Determines which Dreamweaver has the highest score.
    /// Returns "balance" if no thread exceeds 60% of total points (hidden fourth path).
    /// </summary>
    /// <returns>The dominant Dreamweaver thread name.</returns>
    protected virtual string GetDominantDreamweaver()
    {
        var totalPoints = _DreamweaverScores["light"] + _DreamweaverScores["shadow"] + _DreamweaverScores["ambition"];

        if (totalPoints == 0)
        {
            return "light"; // Default if no choices made
        }

        // Check for balance (no thread exceeds 60%)
        foreach (var score in _DreamweaverScores)
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
