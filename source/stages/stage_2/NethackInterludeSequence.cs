// <copyright file="NethackInterludeSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;
using OmegaSpiral.Source.Stages.Stage2;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Stage2.Beats;

/// <summary>
/// Beat: Interlude Sequence scene handler.
/// Loads interlude data from stage_2.json and displays dialogue with player choice options.
/// Player selection updates stage affinity scores before advancing.
/// </summary>
[GlobalClass]
public partial class NethackInterludeSequence : SceneBase
{
    private const string NarrativeJsonPath = "res://source/stages/stage_2/stage_2.json";

#pragma warning disable CA2213 // _contentContainer is managed by Godot's scene tree
    private VBoxContainer? _contentContainer;
#pragma warning restore CA2213
    private Stage2NarrativeData? _narrativeData;
    private int _interludeIndex; // 0, 1, or 2 for light, shadow, ambition interludes

    [Export(PropertyHint.Range, "0,2,1")]
    private int interludeIndex;

    /// <inheritdoc/>
    protected override string CurrentBeatId => $"beat_interlude_{_interludeIndex + 1}";

    /// <inheritdoc/>
    protected override string StageManifestPath => "res://source/stages/stage_2/stage_2_manifest.json";

    /// <summary>
    /// Sets which interlude this beat will display (0=light, 1=shadow, 2=ambition).
    /// </summary>
    public void SetInterludeIndex(int index)
    {
        if (index < 0 || index > 2)
        {
            GD.PrintErr($"[NethackInterludeSequence] Invalid interlude index: {index}");
            return;
        }

        _interludeIndex = index;
        interludeIndex = index;
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        _interludeIndex = Math.Clamp(interludeIndex, 0, 2);
        base._Ready();

        GD.Print($"[NethackInterludeSequence] Interlude {_interludeIndex + 1} started");

        // Load narrative data from JSON
        var loader = new NarrativeDataLoader();
        _narrativeData = loader.LoadNarrativeData<Stage2NarrativeData>(NarrativeJsonPath);

        if (_narrativeData == null)
        {
            GD.PrintErr("[NethackInterludeSequence] Failed to load narrative data");
            return;
        }

        if (_narrativeData.Interludes == null || _narrativeData.Interludes.Count <= _interludeIndex)
        {
            GD.PrintErr($"[NethackInterludeSequence] Interlude index {_interludeIndex} out of range");
            return;
        }

        // Find or create content container
        _contentContainer = GetNodeOrNull<VBoxContainer>("ContentMargin/ContentVBox");

        if (_contentContainer == null)
        {
            GD.PrintErr("[NethackInterludeSequence] ContentVBox not found in scene");
            return;
        }

        // Dynamically create UI from interlude data
        RenderInterlude();
    }

    /// <summary>
    /// Renders the interlude content from JSON data.
    /// </summary>
    private void RenderInterlude()
    {
        if (_narrativeData?.Interludes == null || _contentContainer == null)
        {
            return;
        }

        var interlude = _narrativeData.Interludes[_interludeIndex];

        // Show the Dreamweaver who owns this interlude
        var dreamer = _narrativeData.Dreamweavers.Find(d => d.Id == interlude.Owner);
        if (dreamer != null)
        {
            AddTextLabel(_contentContainer, $"[{dreamer.Name.ToUpper()}]", 16);
        }

        // Show the prompt
        AddTextLabel(_contentContainer, interlude.Prompt, 18);

        // Add choice buttons for each option
        foreach (var option in interlude.Options)
        {
            var optionText = $"[{option.Alignment.ToUpper()}] {option.Text}";
            AddButton(_contentContainer, optionText, () => OnOptionSelected(option, interlude));
        }
    }

    /// <summary>
    /// Handles player selecting an option.
    /// Updates affinity tracking and advances to next beat.
    /// </summary>
    private void OnOptionSelected(InterludeOption option, InterludeData interlude)
    {
        GD.Print($"[NethackInterludeSequence] Player selected: {option.Id} (alignment: {option.Alignment})");

        int points = option.Alignment == interlude.Owner ? 2 : 1;
        AwardAffinity(option.Alignment, points);

        // Show approval banter
        if (option.Banter?.Approve != null)
        {
            GD.Print($"[{option.Banter.Approve.Speaker}] {option.Banter.Approve.Line}");
        }

        // TODO: In a full implementation, we'd display the banter and wait before advancing

        GD.Print("[NethackInterludeSequence] Interlude complete, advancing to next beat");
        TransitionToNextBeat();
    }
}
