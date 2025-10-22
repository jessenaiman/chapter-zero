// <copyright file="Beat1BootSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Stage1.Beats;

/// <summary>
/// Beat 1: Boot Sequence scene handler.
/// Loads glitch lines from opening.json and dynamically creates UI,
/// then auto-advances after delay.
/// </summary>
[GlobalClass]
public partial class Beat1BootSequence : BeatSceneBase
{
    private const float AutoAdvanceDelaySeconds = 3.0f;
    private const string NarrativeJsonPath = "res://source/stages/stage_1/opening.json";

    private VBoxContainer? _contentContainer;
    private Stage1NarrativeData? _narrativeData;

    /// <inheritdoc/>
    protected override string CurrentBeatId => "beat_1_boot_sequence";

    /// <inheritdoc/>
    protected override string StageManifestPath => "res://source/stages/stage_1/stage_manifest.json";

    /// <inheritdoc/>
    public override async void _Ready()
    {
        base._Ready();

        GD.Print("[Beat1BootSequence] Boot sequence started");

        // Load narrative data from JSON
        var loader = new NarrativeDataLoader();
        _narrativeData = loader.LoadNarrativeData<Stage1NarrativeData>(NarrativeJsonPath);

        if (_narrativeData == null)
        {
            GD.PrintErr("[Beat1BootSequence] Failed to load narrative data");
            return;
        }

        // Find or create content container
        _contentContainer = GetNodeOrNull<VBoxContainer>("ContentMargin/ContentVBox");

        if (_contentContainer == null)
        {
            GD.PrintErr("[Beat1BootSequence] ContentVBox not found in scene");
            return;
        }

        // Dynamically create UI from boot sequence data
        RenderBootSequence();

        // Simulate boot sequence, then auto-advance
        await Task.Delay((int)(AutoAdvanceDelaySeconds * 1000)).ConfigureAwait(false);

        GD.Print("[Beat1BootSequence] Boot sequence complete, advancing to next beat");
        TransitionToNextBeat();
    }

    /// <summary>
    /// Renders the boot sequence content from JSON data.
    /// </summary>
    private void RenderBootSequence()
    {
        if (_narrativeData?.BootSequence?.GlitchLines == null || _contentContainer == null)
        {
            return;
        }

        GD.Print($"[Beat1BootSequence] Rendering {_narrativeData.BootSequence.GlitchLines.Count} glitch lines");

        // Combine all glitch lines into a single text block
        var combinedText = string.Join("\n", _narrativeData.BootSequence.GlitchLines);

        // Add the text label
        AddTextLabel(_contentContainer, combinedText, 20);
    }
}
