// <copyright file="Beat2OpeningMonologue.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Stages.Stage1.Beats;

/// <summary>
/// Beat 2: Opening Monologue scene handler.
/// Loads monologue lines from opening.json, displays them with timing,
/// and waits for player input to advance.
/// </summary>
[GlobalClass]
public partial class Beat2OpeningMonologue : BeatSceneBase
{
    /// <inheritdoc/>
    protected override string CurrentBeatId => "beat_2_opening_monologue";

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        // Load and display opening monologue content from JSON
        if (NarrativeContent?.OpeningMonologue != null)
        {
            var monologueContent = NarrativeContent.OpeningMonologue;
            GD.Print($"[Beat2OpeningMonologue] Loaded {monologueContent.Lines?.Count ?? 0} lines from JSON");
            GD.Print($"[Beat2OpeningMonologue] Cinematic timing: {monologueContent.CinematicTiming}");

            // Display each monologue line (implementation in subclass or dedicated display system)
            // For now, just log that content was loaded
            if (monologueContent.Lines != null)
            {
                foreach (var line in monologueContent.Lines)
                {
                    GD.Print($"  > {line}");
                }
            }
        }
        else
        {
            GD.PrintErr("[Beat2OpeningMonologue] Opening monologue content not found in narrative document");
        }

        // Find and connect the Continue button
        var continueButton = GetNodeOrNull<Button>("ContentMargin/ContentVBox/ContinueButton");
        if (continueButton != null)
        {
            continueButton.Pressed += OnContinuePressed;
            GD.Print("[Beat2OpeningMonologue] Continue button connected");
        }
        else
        {
            GD.PrintErr("[Beat2OpeningMonologue] Continue button not found");
        }
    }

    private void OnContinuePressed()
    {
        GD.Print("[Beat2OpeningMonologue] Continue pressed, advancing to next beat");
        TransitionToNextBeat();
    }
}
