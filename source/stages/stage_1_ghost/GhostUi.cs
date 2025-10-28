// <copyright file="GhostUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Infrastructure;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Source.Scripts.Stages.Stage1;

namespace OmegaSpiral.Source.Stages.Stage1;

/// <summary>
/// Stage 1 Ghost Terminal UI - orchestrates the narrative sequence from ghost.yaml.
/// Extends both NarrativeUi (for presentation) and StageBase (for stage lifecycle).
/// Handles Ghost-specific concerns: script loading, Dreamweaver score tracking, and stage completion.
/// </summary>
[GlobalClass]
public partial class GhostUi : NarrativeUi
{
    /// <summary>
    /// Emitted when the Ghost Terminal narrative sequence completes.
    /// This signals to GhostCinematicDirector that the stage is finished.
    /// </summary>
    [Signal]
    public delegate void SequenceCompleteEventHandler();

    private NarrativeScript? _Script;
    private int _CurrentMomentIndex;
    private StringBuilder _Transcript = new();
    private List<string> _PlayerChoices = new();
    private GameManager? _GameManager;

    /// <summary>
    /// Gets the stage ID for this UI (Stage 1 = Ghost Terminal).
    /// </summary>
    public int StageId => 1;

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
        _GameManager = GetNode<GameManager>("/root/GameManager");

        // Load ghost.yaml script
        if (!LoadGhostScript())
        {
            GD.PrintErr("[GhostTerminal] Failed to load script - cannot start sequence");
            return;
        }

        // Start the narrative sequence (boot + story)
        CallDeferred(nameof(StartGhostSequence));
    }

    /// <summary>
    /// Loads the ghost.yaml narrative script using the GhostDataLoader.
    /// </summary>
    /// <returns><see langword="true"/> if loaded successfully, <see langword="false"/> otherwise.</returns>
    private bool LoadGhostScript()
    {
        try
        {
            var dataLoader = new GhostDataLoader();
            var plan = dataLoader.GetPlan();
            _Script = plan.Script;
            _CurrentMomentIndex = 0;

            GD.Print($"[GhostTerminal] Loaded: {_Script.Title}");
            GD.Print($"[GhostTerminal] Speaker: {_Script.Speaker}");
            GD.Print($"[GhostTerminal] Scenes: {_Script.Scenes.Count}");

            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[GhostTerminal] Failed to load ghost.yaml: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Override boot sequence to do nothing - we handle boot manually in StartGhostSequence.
    /// </summary>
    protected override async Task PlayBootSequenceAsync()
    {
        // Boot sequence is handled manually in StartGhostSequence after script loading
        await Task.CompletedTask;
    }

    /// <summary>
    /// Starts the Ghost Terminal narrative sequence.
    /// Called deferred to ensure all nodes are ready.
    /// Handles boot sequence manually since we override the automatic one.
    /// </summary>
    private async void StartGhostSequence()
    {
        // Start presenting from the first scene (Scene 0 is the boot/opening)
        _CurrentMomentIndex = 0;
        await PresentNextMomentAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Plays the boot sequence manually using the first scene from the script.
    /// DEPRECATED: No longer used - we present Scene 0 through normal flow.
    /// </summary>
    private async Task PlayBootSequenceManuallyAsync()
    {
        // No longer needed - Scene 0 is presented via PresentNextMomentAsync
        await Task.CompletedTask;
    }    /// <summary>
    /// Presents the next narrative scene from ghost.yaml.
    /// Delegates to base class for all presentation logic (shaders, text, choices).
    /// Only handles Ghost-specific concerns (score tracking, secret reveal ceremony).
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    private async Task PresentNextMomentAsync()
    {
        if (_Script == null || _CurrentMomentIndex >= _Script.Scenes.Count)
        {
            await CompleteGhostSequenceAsync().ConfigureAwait(false);
            return;
        }

        var moment = _Script.Scenes[_CurrentMomentIndex];
        _CurrentMomentIndex++;

        // Scenes can be narrative-only or question scenes
        // Check for question field to determine type
        if (!string.IsNullOrEmpty(moment.Question) && moment.Answers != null && moment.Answers.Count > 0)
        {
            await PresentQuestionSceneAsync(moment).ConfigureAwait(false);
        }
        else
        {
            await PresentNarrativeSceneAsync(moment).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Presents a narrative scene (lines only, no question).
    /// Converts ContentBlock to NarrativeBeat and delegates to base class.
    /// </summary>
    /// <param name="scene">The narrative scene to present.</param>
    /// <returns>A task representing the async operation.</returns>
    private async Task PresentNarrativeSceneAsync(ContentBlock scene)
    {
        // Record narrative lines to transcript
        if (scene.Lines != null)
        {
            foreach (var line in scene.Lines)
            {
                RecordNarrativeLine(line);
            }
        }

        // Convert ContentBlock to NarrativeBeat for base class
        var beats = ConvertToNarrativeBeats(scene).ToArray();
        await PlayNarrativeSequenceAsync(beats).ConfigureAwait(false);

        // Auto-advance to next scene
        await PresentNextMomentAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Presents a question scene (lines + question + answers).
    /// Delegates to base class PresentChoicesAsync and tracks Dreamweaver scores.
    /// Scoring: if answer.owner == currentThread → +2, else → +1 to answer.owner.
    /// </summary>
    /// <param name="scene">The question scene to present.</param>
    /// <returns>A task representing the async operation.</returns>
    private async Task PresentQuestionSceneAsync(ContentBlock scene)
    {
        // Display setup lines if present
        if (scene.Lines != null && scene.Lines.Count > 0)
        {
            foreach (var line in scene.Lines)
            {
                RecordNarrativeLine(line);
            }

            var setupBeats = scene.Lines.Select(line => new NarrativeUi.NarrativeBeat
            {
                Text = line,
                VisualPreset = null,
                DelaySeconds = 0
            }).ToArray();
            await PlayNarrativeSequenceAsync(setupBeats).ConfigureAwait(false);
        }

        // Record question to transcript
        if (!string.IsNullOrEmpty(scene.Question))
        {
            RecordNarrativeLine(scene.Question);
        }

        // Present choices using base class (handles all choice UI logic)
        if (scene.Answers != null && scene.Answers.Count > 0)
        {
            var choiceTexts = scene.Answers
                .Where(a => !string.IsNullOrEmpty(a.Text))
                .Select(a => a.Text!)
                .ToArray();

            var selectedText = await PresentChoicesAsync(scene.Question ?? string.Empty, choiceTexts).ConfigureAwait(false);

            // Record player's choice
            RecordPlayerChoice(selectedText);

            // Find selected answer and track Dreamweaver scores with automatic calculation
            var selectedAnswer = scene.Answers.FirstOrDefault(a => a.Text == selectedText);
            if (selectedAnswer != null)
            {
                TrackDreamweaverScoresAutomatic(selectedAnswer);
            }
        }

        // Auto-advance to next scene
        await PresentNextMomentAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// LEGACY: Presents a narrative moment (type: "narrative").
    /// Kept for backwards compatibility with old YAML format.
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

        // Record narrative lines to transcript
        if (moment.Lines != null)
        {
            foreach (var line in moment.Lines)
            {
                RecordNarrativeLine(line);
            }
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
        // Record setup lines to transcript
        if (moment.Setup != null && moment.Setup.Count > 0)
        {
            foreach (var line in moment.Setup)
            {
                RecordNarrativeLine(line);
            }
        }

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

        // Record prompt to transcript
        if (!string.IsNullOrEmpty(moment.Prompt))
        {
            RecordNarrativeLine(moment.Prompt);
        }

        // Present choices using base class (handles all choice UI logic)
        if (moment.Options != null && moment.Options.Count > 0)
        {
            var choiceTexts = moment.Options
                .Where(o => !string.IsNullOrEmpty(o.Text))
                .Select(o => o.Text!)
                .ToArray();

            var selectedText = await PresentChoicesAsync(moment.Prompt ?? string.Empty, choiceTexts).ConfigureAwait(false);

            // Record player's choice
            RecordPlayerChoice(selectedText);

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
        // Record setup lines to transcript
        if (moment.Setup != null && moment.Setup.Count > 0)
        {
            foreach (var line in moment.Setup)
            {
                RecordNarrativeLine(line);
            }
        }

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

        // Record prompt to transcript
        if (!string.IsNullOrEmpty(moment.Prompt))
        {
            RecordNarrativeLine(moment.Prompt);
        }

        // Present the question part (delegates to base class for choices)
        if (moment.Options != null && moment.Options.Count > 0)
        {
            var choiceTexts = moment.Options
                .Where(o => !string.IsNullOrEmpty(o.Text))
                .Select(o => o.Text!)
                .ToArray();

            var selectedText = await PresentChoicesAsync(moment.Prompt ?? string.Empty, choiceTexts).ConfigureAwait(false);

            // Record player's choice
            RecordPlayerChoice(selectedText);

            // Find selected option and track Dreamweaver scores
            var selectedOption = moment.Options.FirstOrDefault(o => o.Text == selectedText);
            if (selectedOption != null)
            {
                TrackDreamweaverScores(selectedOption);
            }
        }

        // Record continuation lines to transcript
        if (moment.Continuation != null && moment.Continuation.Count > 0)
        {
            foreach (var line in moment.Continuation)
            {
                RecordNarrativeLine(line);
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
    /// Parses inline tags like [GLITCH] and [FADE_TO_STABLE] from lines.
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

        // Add all narrative lines as beats, parsing inline tags
        if (moment.Lines != null)
        {
            foreach (var line in moment.Lines)
            {
                // Parse inline tags and apply visual effects
                var (displayText, visualPreset) = ParseLineForInlineTags(line);

                // If line has a visual tag, emit it as a beat first
                if (!string.IsNullOrEmpty(visualPreset))
                {
                    yield return new NarrativeUi.NarrativeBeat
                    {
                        Text = string.Empty,
                        VisualPreset = visualPreset,
                        DelaySeconds = 0.2f
                    };
                }

                // Emit the cleaned text (without tags)
                if (!string.IsNullOrEmpty(displayText))
                {
                    yield return new NarrativeUi.NarrativeBeat
                    {
                        Text = displayText,
                        VisualPreset = null,
                        DelaySeconds = 0
                    };
                }
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
    /// Parses a line for inline tags like [GLITCH] and [FADE_TO_STABLE].
    /// Removes tags from display text and returns the visual preset to apply.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    /// <returns>Tuple of (cleanedText, visualPreset).</returns>
    private (string displayText, string? visualPreset) ParseLineForInlineTags(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return (line, null);
        }

        string? visualPreset = null;
        string displayText = line;

        // Check for visual effect tags
        if (line.Contains("[GLITCH]", StringComparison.OrdinalIgnoreCase))
        {
            visualPreset = "glitch";
            displayText = System.Text.RegularExpressions.Regex.Replace(
                displayText, @"\[GLITCH\]", "",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
        }
        else if (line.Contains("[FADE_TO_STABLE]", StringComparison.OrdinalIgnoreCase))
        {
            visualPreset = "fade_to_stable";
            displayText = System.Text.RegularExpressions.Regex.Replace(
                displayText, @"\[FADE_TO_STABLE\]", "",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
        }

        return (displayText, visualPreset);
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
    /// Tracks Dreamweaver scores automatically based on answer owner.
    /// Scoring logic: if answer.owner == currentThread → +2, else → +1 to answer.owner.
    /// Only applies to Dreamweaver owners (light, shadow, ambition).
    /// </summary>
    /// <param name="answer">The answer option that was selected.</param>
    protected virtual void TrackDreamweaverScoresAutomatic(ChoiceOption answer)
    {
        if (string.IsNullOrEmpty(answer.Owner))
        {
            return;
        }

        var answerOwner = answer.Owner.ToLowerInvariant();

        // Only score for Dreamweaver threads (not omega, system, etc.)
        if (!_DreamweaverScores.ContainsKey(answerOwner))
        {
            return;
        }

        // Determine current dominant thread for bonus scoring
        var currentThread = GetCurrentDominantThread();

        // Scoring: match current thread = +2, otherwise = +1
        int points = (answerOwner == currentThread) ? 2 : 1;

        _DreamweaverScores[answerOwner] += points;
        GD.Print($"[GhostTerminal] {answerOwner}: +{points} = {_DreamweaverScores[answerOwner]}" +
                 (points == 2 ? " (thread match bonus)" : ""));
    }

    /// <summary>
    /// Gets the current dominant Dreamweaver thread based on accumulated scores.
    /// </summary>
    /// <returns>The key of the dominant thread (light, shadow, or ambition).</returns>
    private string GetCurrentDominantThread()
    {
        return _DreamweaverScores
            .OrderByDescending(kvp => kvp.Value)
            .ThenBy(kvp => kvp.Key) // Tie-breaker: alphabetical
            .First()
            .Key;
    }

    /// <summary>
    /// Completes the Ghost Terminal sequence and determines the dominant Dreamweaver.
    /// Transitions to the next stage based on the player's accumulated scores.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    /// <summary>
    /// Records a line of narrative text to the transcript.
    /// </summary>
    private void RecordNarrativeLine(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            _Transcript.AppendLine(text);
        }
    }

    /// <summary>
    /// Records a player choice to the transcript and choice history.
    /// </summary>
    private void RecordPlayerChoice(string choiceText)
    {
        _PlayerChoices.Add(choiceText);
        _Transcript.AppendLine($"\n[Player Choice: {choiceText}]\n");
    }

    /// <summary>
    /// Completes the Ghost Terminal sequence, records transcript, and transitions to next stage.
    /// </summary>
    protected virtual async Task CompleteGhostSequenceAsync()
    {
        // Determine dominant Dreamweaver
        var dominantThread = this.GetDominantDreamweaver();
        GD.Print($"[GhostUi] Dominant thread: {dominantThread}");
        GD.Print($"[GhostUi] Scores - Light: {this._DreamweaverScores["light"]}, Shadow: {this._DreamweaverScores["shadow"]}, Ambition: {this._DreamweaverScores["ambition"]}");

        // Record stage to journal
        if (_GameManager != null)
        {
            _GameManager.Journal.RecordStage(
                stageId: "ghost",
                transcript: _Transcript.ToString(),
                finalScores: new Dictionary<string, int>(_DreamweaverScores),
                playerChoices: _PlayerChoices
            );
            GD.Print($"[GhostUi] Stage recorded to journal");
        }

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

        GD.Print("[GhostUi] Sequence complete - emitting SequenceComplete signal");
        EmitSignal(SignalName.SequenceComplete);

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
