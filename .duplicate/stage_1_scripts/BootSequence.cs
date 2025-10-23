// <copyright file="Beat1BootSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Stage1.Beats;

/// <summary>
/// Boot Sequence scene - displays glitch lines from opening.json.
/// Simple presentation node that signals completion to Stage1Controller.
/// </summary>
[GlobalClass]
public partial class Beat1BootSequence : Control
{
    private const float AutoAdvanceDelaySeconds = 3.0f;
    private const string NarrativeJsonPath = "res://source/stages/stage_1/opening.json";

    private VBoxContainer? _contentContainer;
    private Stage1NarrativeData? _narrativeData;

    /// <summary>
    /// Emitted when this scene has finished and is ready to advance.
    /// </summary>
    [Signal]
    public delegate void SceneCompleteEventHandler();

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

        // Simulate boot sequence, then signal completion
        await Task.Delay((int)(AutoAdvanceDelaySeconds * 1000)).ConfigureAwait(false);

        GD.Print("[Beat1BootSequence] Boot sequence complete");
        EmitSignal(SignalName.SceneComplete);
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

        // Create and add the text label
        var label = new Label
        {
            Text = combinedText,
            AutowrapMode = TextServer.AutowrapMode.Word,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        label.AddThemeFontSizeOverride("font_size", 20);
        _contentContainer.AddChild(label);
    }
}
