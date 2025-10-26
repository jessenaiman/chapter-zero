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
/// Stage 1 Ghost Ui - extends NarrativeUi with Stage 1-specific behavior.
/// Loads ghost.yaml via GhostTerminalCinematicDirector and presents the opening sequence.
/// Implements Dreamweaver selection through philosophical questions with score tracking.
/// </summary>
[GlobalClass]
public partial class GhostUi : NarrativeUi
{
    private GhostCinematicDirector? _Director;
    private NarrativeScript? _Script;
    private int _CurrentMomentIndex;
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
        base._Ready(); // Initialize OmegaUi components first

        // Initialize audio manager
        _AudioManager = new GhostAudioManager();
        AddChild(_AudioManager);

        // Get singleton references
        this._SceneManager = this.GetNode<SceneManager>("/root/SceneManager");
        this._GameState = this.GetNode<GameState>("/root/GameState");

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
    /// Starts the Ghost Terminal narrative sequence.
    /// Called deferred to ensure all nodes are ready.
    /// </summary>
    private async void StartGhostSequence()
    {
        await PresentNextMomentAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Loads the ghost.yaml narrative script using the GhostTerminalCinematicDirector.
    /// </summary>
    /// <returns><see langword="true"/> if loaded successfully, <see langword="false"/> otherwise.</returns>
    protected virtual bool LoadGhostScript()
    {
        try
        {
            _Director = new GhostCinematicDirector();
            var plan = _Director.GetPlan();
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
    /// Presents the next narrative moment from ghost.yaml.
    /// Handles different moment types: narrative, question, composite.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    protected virtual async Task PresentNextMomentAsync()
    {
        if (_Script == null || _CurrentMomentIndex >= _Script.Moments.Count)
        {
            // Finished all moments - determine dominant Dreamweaver and transition
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
    /// Presents a narrative moment (type: "narrative") - displays lines with optional visual preset.
    /// Special handling for CODE_FRAGMENT_GLITCH_OVERLAY preset implements secret reveal ceremony.
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

        // Special handling for secret reveal ceremony
        if (string.Equals(moment.VisualPreset, "CODE_FRAGMENT_GLITCH_OVERLAY", StringComparison.OrdinalIgnoreCase))
        {
            await PresentSecretRevealCeremonyAsync(moment).ConfigureAwait(false);
            return;
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
    /// Presents the secret reveal ceremony with symbol-by-symbol text reveal and audio sync.
    /// Implements the 4-second orchestrated buildup with Ωmega Spiral symbols.
    /// </summary>
    /// <param name="moment">The moment containing the secret reveal content.</param>
    /// <returns>A task representing the async operation.</returns>
    protected virtual async Task PresentSecretRevealCeremonyAsync(ContentBlock moment)
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
                await AppendTextAsync(line).ConfigureAwait(false);
            }
        }

        // Display prompt
        if (!string.IsNullOrEmpty(moment.Prompt))
        {
            await AppendTextAsync(moment.Prompt).ConfigureAwait(false);
        }

        // Display context
        if (!string.IsNullOrEmpty(moment.Context))
        {
            await AppendTextAsync(moment.Context).ConfigureAwait(false);
        }

        // Present choices and wait for selection
        if (moment.Options != null && moment.Options.Count > 0)
        {
            // Build choice array for base class presentation (filter out nulls)
            var choiceTexts = moment.Options
                .Where(o => !string.IsNullOrEmpty(o.Text))
                .Select(o => o.Text!)
                .ToArray();

            var selectedText = await PresentChoicesAsync(moment.Prompt ?? string.Empty, choiceTexts).ConfigureAwait(false);

            // Find the selected option and track scores
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
    protected virtual async Task PresentCompositeMomentAsync(ContentBlock moment)
    {
        // Display setup narrative
        if (moment.Setup != null && moment.Setup.Count > 0)
        {
            foreach (var line in moment.Setup)
            {
                await AppendTextAsync(line).ConfigureAwait(false);
            }
        }

        // Present the question part
        await PresentQuestionMomentAsync(moment).ConfigureAwait(false);

        // Display continuation narrative after choice
        if (moment.Continuation != null && moment.Continuation.Count > 0)
        {
            foreach (var line in moment.Continuation)
            {
                await AppendTextAsync(line).ConfigureAwait(false);
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
        }        await Task.CompletedTask;
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
