// <copyright file="Beat2OpeningMonologue.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Stage1.Beats;

/// <summary>
/// Beat 2: Opening Monologue scene handler.
/// Loads monologue lines from opening.json and dynamically creates UI,
/// then waits for player input to advance.
/// </summary>
[GlobalClass]
public partial class Beat2OpeningMonologue : BeatSceneBase
{
    private const string NarrativeJsonPath = "res://source/stages/stage_1/opening.json";

#pragma warning disable CA2213
    private VBoxContainer? _contentContainer;
#pragma warning restore CA2213
    private Stage1NarrativeData? _narrativeData;

    /// <inheritdoc/>
    protected override string CurrentBeatId => "beat_2_opening_monologue";

    /// <inheritdoc/>
    protected override string StageManifestPath => "res://source/stages/stage_1/stage_manifest.json";

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        GD.Print("[Beat2OpeningMonologue] Opening monologue started");

        // Load narrative data from JSON
        var loader = new NarrativeDataLoader();
        _narrativeData = loader.LoadNarrativeData<Stage1NarrativeData>(NarrativeJsonPath);

        if (_narrativeData == null)
        {
            GD.PrintErr("[Beat2OpeningMonologue] Failed to load narrative data");
            return;
        }

        // Find or create content container
        _contentContainer = GetNodeOrNull<VBoxContainer>("ContentMargin/ContentVBox");

        if (_contentContainer == null)
        {
            GD.PrintErr("[Beat2OpeningMonologue] ContentVBox not found in scene");
            return;
        }

        // Dynamically create UI from opening monologue data
        RenderOpeningMonologue();

        // Add a Continue button
        AddButton(_contentContainer, "Continue...", OnContinuePressed);
    }

    /// <summary>
    /// Renders the opening monologue content from JSON data.
    /// </summary>
    private void RenderOpeningMonologue()
    {
        if (_narrativeData?.OpeningMonologue?.Lines == null || _contentContainer == null)
        {
            return;
        }

        GD.Print($"[Beat2OpeningMonologue] Rendering {_narrativeData.OpeningMonologue.Lines.Count} monologue lines");

        // Combine all lines into a single text block
        var combinedText = string.Join("\n", _narrativeData.OpeningMonologue.Lines);

        // Add the text label
        AddTextLabel(_contentContainer, combinedText, 20);
    }

    /// <summary>
    /// Handles Continue button press.
    /// </summary>
    private void OnContinuePressed()
    {
        GD.Print("[Beat2OpeningMonologue] Continue pressed, advancing to next beat");
        TransitionToNextBeat();
    }
}
