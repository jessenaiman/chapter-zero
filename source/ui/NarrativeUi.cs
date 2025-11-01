// <copyright file="NarrativeUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Backend.Narrative;
using OmegaSpiral.Source.Ui.Omega;

namespace OmegaSpiral.Source.Narrative;
/// <summary>
/// Narrative UI component for displaying story text and player choices.
/// Minimal extension of OmegaContainer - lets OmegaUI handle all styling.
/// Text color: warm amber (#FEC962), background: handled by OmegaUI frame.
/// </summary>
[GlobalClass]
public partial class NarrativeUi : OmegaContainer
{
    [Signal]
    public delegate void ChoiceSelectedEventHandler(string sceneOwner, string choiceOwner);

    /// <summary>
    /// Gets or sets a value indicating whether to show boot sequence on ready.
    /// </summary>
    [Export]
    public bool EnableBootSequence { get; set; } = true;

    /// <summary>
    /// Gets or sets the boot sequence text with xterm styling.
    /// </summary>
    [Export]
    public string BootSequenceText { get; set; } = "[SYSTEM INITIALIZING...]\n[NARRATIVE ENGINE LOADED]\n[Ωmega SPIRAL READY]\n$> ";

    /// <summary>
    /// Gets or sets the typing speed (characters per second).
    /// </summary>
    [Export]
    public float TypingSpeed { get; set; } = 20f;

    private VBoxContainer? TextContainer { get; set; }

    private VBoxContainer? ChoiceContainer { get; set; }

    private RichTextLabel? TextDisplay { get; set; }

    /// <summary>
    /// Initializes the UI component.
    /// OmegaUI frame provides styling; this just orchestrates narrative flow.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();
        this.TextContainer = this.GetNodeOrNull<VBoxContainer>("TextContainer");
        this.ChoiceContainer = this.GetNodeOrNull<VBoxContainer>("ChoiceContainer");
        this.TextDisplay = this.GetNodeOrNull<RichTextLabel>("TextDisplay");
    }

    /// <summary>
    /// Displays lines of text in the UI.
    /// Emits narrative_display_finished signal when all lines have been displayed.
    /// </summary>
    /// <param name="lines">The lines to display.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task DisplayLinesAsync(IList<string> lines)
    {
        if (this.TextContainer == null)
        {
            return;
        }

        foreach (var line in lines)
        {
            var label = new Label { Text = line };
            this.TextContainer.AddChild(label);
            GD.Print($"[NarrativeUi] {line}");
            await this.ToSignal(this.GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
        }

        // Signal that narrative display is complete
        this.EmitInteractionComplete("display_finished", default);
    }

    /// <summary>
    /// Presents a set of choices to the player.
    /// Creates buttons and waits for selection, then emits interaction_complete signal.
    /// </summary>
    /// <param name="question">The question to display.</param>
    /// <param name="speaker">The character asking the question.</param>
    /// <param name="choices">The available choices.</param>
    /// <returns>A task representing the asynchronous operation, with the selected choice.</returns>
    public virtual async Task<Choice> PresentChoiceAsync(string question, string speaker, IList<Choice> choices)
    {
        if (this.ChoiceContainer == null)
        {
            return choices.Count > 0 ? choices[0] : throw new InvalidOperationException("No choices available");
        }

        // Display question
        var questionLabel = new Label { Text = question };
        this.ChoiceContainer.AddChild(questionLabel);
        GD.Print($"[NarrativeUi] Question: {question}");

        var tcs = new TaskCompletionSource<Choice>();

        // Create buttons for each choice
        foreach (var choice in choices)
        {
            var button = new Button { Text = choice.Text };
            button.Pressed += () =>
            {
                GD.Print($"[NarrativeUi] Selected: {choice.Text}");
                tcs.TrySetResult(choice);
            };
            this.ChoiceContainer.AddChild(button);
        }

        this.ChoiceContainer.Visible = true;
        var result = await tcs.Task;
        this.ChoiceContainer.Visible = false;

        // Clear choices
        foreach (Node child in this.ChoiceContainer.GetChildren())
        {
            child.QueueFree();
        }

        // Signal that choice presentation is complete
        this.EmitInteractionComplete("choice_made", result.Text ?? string.Empty);

        return result;
    }

    /// <summary>
    /// Handles command lines (for future expansion).
    /// </summary>
    /// <param name="line">The command line.</param>
    /// <returns>A task representing whether the command was handled.</returns>
    public virtual Task<bool> HandleCommandLineAsync(string line)
    {
        GD.Print($"[NarrativeUi] Command: {line}");
        return Task.FromResult(false);
    }

    /// <summary>
    /// Applies scene-specific effects like pauses by parsing commands in the lines.
    /// </summary>
    /// <param name="scene">The scene element.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task ApplySceneEffectsAsync(StoryBlock scene)
    {
        await this.ApplyEffectsFromLinesAsync(scene.Lines);
    }

    /// <summary>
    /// Parses and applies effects from the scene lines, such as pauses.
    /// </summary>
    /// <param name="lines">The lines to parse for commands.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ApplyEffectsFromLinesAsync(IList<string> lines)
    {
        foreach (var line in lines)
        {
            if (line.StartsWith("[PAUSE:") && line.EndsWith("]"))
            {
                var pauseText = line.Substring(7, line.Length - 8); // Remove [PAUSE: and ]
                if (double.TryParse(pauseText, out var pauseSeconds) && pauseSeconds > 0)
                {
                    await this.ToSignal(this.GetTree().CreateTimer(pauseSeconds), SceneTreeTimer.SignalName.Timeout);
                }
            }
            // Add other command parsing here as needed
        }
    }

    /// <summary>
    /// Processes a player choice (for game state updates).
    /// </summary>
    /// <param name="selected">The selected choice.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task ProcessChoiceAsync(Choice selected)
    {
        GD.Print($"[NarrativeUi] Processing choice: {selected.Text}");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Plays boot sequence if enabled.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task PlayBootSequenceAsync()
    {
        if (!this.EnableBootSequence)
        {
            return;
        }

        GD.Print("[NarrativeUi] Boot sequence starting...");
        var lines = this.BootSequenceText.Split('\n');
        await this.DisplayLinesAsync(lines);
        GD.Print("[NarrativeUi] Boot sequence complete");
    }

    /// <summary>
    /// Notifies that the narrative sequence is complete.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task NotifySequenceCompleteAsync()
    {
        GD.Print("[NarrativeUi] Narrative sequence complete");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Clears all displayed text and choices.
    /// </summary>
    public void Clear()
    {
        if (this.TextContainer != null)
        {
            foreach (Node child in this.TextContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (this.ChoiceContainer != null)
        {
            foreach (Node child in this.ChoiceContainer.GetChildren())
            {
                child.QueueFree();
            }
        }
    }

    /// <summary>
    /// Types text character by character with xterm effect.
    /// </summary>
    /// <param name="text">The text to type.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task TypeTextAsync(string text)
    {
        float delayPerChar = 1.0f / this.TypingSpeed;
        foreach (char c in text)
        {
            GD.Print(c);
            await this.ToSignal(this.GetTree().CreateTimer(delayPerChar), SceneTreeTimer.SignalName.Timeout);
        }
    }
}
