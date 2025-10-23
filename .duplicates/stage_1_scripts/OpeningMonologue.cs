// <copyright file="Beat2OpeningMonologue.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Stage1.Beats;

/// <summary>
/// Opening Monologue scene - displays narrative text from opening.json.
/// Simple presentation node that signals completion to Stage1Controller.
/// </summary>
[GlobalClass]
public partial class Beat2OpeningMonologue : Control
{
    private const string NarrativeJsonPath = "res://source/stages/stage_1/opening.json";

#pragma warning disable CA2213
    private VBoxContainer? _contentContainer;
#pragma warning restore CA2213
    private Stage1NarrativeData? _narrativeData;

    /// <summary>
    /// Emitted when this scene has finished and is ready to advance.
    /// </summary>
    [Signal]
    public delegate void SceneCompleteEventHandler();

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
        var continueButton = new Button { Text = "Continue..." };
        continueButton.Pressed += OnContinuePressed;
        _contentContainer.AddChild(continueButton);
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

    /// <summary>
    /// Handles Continue button press.
    /// </summary>
    private void OnContinuePressed()
    {
        GD.Print("[Beat2OpeningMonologue] Continue pressed");
        EmitSignal(SignalName.SceneComplete);
    }
}
