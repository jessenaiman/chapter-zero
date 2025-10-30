// <copyright file="GhostUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage1;

using Godot;
using OmegaSpiral.Source.Backend.Narrative;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// UI for Ghost Terminal stage using godot_xterm for authentic terminal experience.
/// Handles narrative display, choices, and shader effects.
/// </summary>
[GlobalClass]
public partial class GhostUi : NarrativeUi
{
    /// <summary>
    /// Gets or sets a value indicating whether to show boot sequence on ready.
    /// </summary>
    [Export]
    public new bool EnableBootSequence { get; set; } = true;

    /// <summary>
    /// Gets or sets the typing speed for terminal text display.
    /// </summary>
    [Export]
    public float DefaultTypingSpeed { get; set; } = 15f;

    private Node? Terminal { get; set; }

    private VBoxContainer? ChoiceContainer { get; set; }

    private bool BootSequencePlayed { get; set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();
        this.Terminal = this.GetNodeOrNull<Node>("NarrativeStack/Terminal");
        this.ChoiceContainer = this.GetNodeOrNull<VBoxContainer>("NarrativeStack/ChoiceContainer");

        if (this.EnableBootSequence && !this.BootSequencePlayed)
        {
            this.CallDeferred("StartBootSequence");
        }
    }

    /// <summary>
    /// Displays a list of lines to the terminal with delays.
    /// </summary>
    /// <param name="lines">The lines to display.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public new async Task DisplayLinesAsync(IList<string> lines)
    {
        foreach (var line in lines)
        {
            await this.TypeTextAsync(line + "\n");
            await Task.Delay(120);
        }
    }

    /// <summary>
    /// Handles a command line input.
    /// </summary>
    /// <param name="line">The command line.</param>
    /// <returns>Always false.</returns>
    public new Task<bool> HandleCommandLineAsync(string line) => Task.FromResult(false);

    /// <summary>
    /// Applies scene effects based on the script element.
    /// </summary>
    /// <param name="scene">The narrative script element.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public new async Task ApplySceneEffectsAsync(StoryScriptElement scene)
    {
        if (scene.Pause.HasValue && scene.Pause.Value > 0)
        {
            await Task.Delay((int)(scene.Pause.Value * 1000));
        }

        // Apply shader effects based on tags
        if (scene.Lines?.Any(l => l.Contains("[GLITCH]")) == true)
        {
            this.ApplyGlitchEffect();
        }
        else if (scene.Lines?.Any(l => l.Contains("[FADE_TO_STABLE]")) == true)
        {
            this.FadeToStable();
        }
    }

    /// <summary>
    /// Presents a choice to the user.
    /// </summary>
    /// <param name="question">The question to ask.</param>
    /// <param name="speaker">The speaker.</param>
    /// <param name="choices">The available choices.</param>
    /// <returns>The selected choice.</returns>
    public new async Task<ChoiceOption> PresentChoiceAsync(string question, string speaker, IList<ChoiceOption> choices)
    {
        await this.TypeTextAsync(question + "\n");

        if (this.ChoiceContainer == null)
        {
            return choices[0]; // Default
        }

        var tcs = new TaskCompletionSource<ChoiceOption>();

        foreach (var choice in choices)
        {
            var button = new Button { Text = choice.Text };
            button.Pressed += () => tcs.TrySetResult(choice);
            this.ChoiceContainer.AddChild(button);
        }

        this.ChoiceContainer.Visible = true;
        var selected = await tcs.Task;
        this.ChoiceContainer.Visible = false;

        foreach (Node child in this.ChoiceContainer.GetChildren())
        {
            child.QueueFree();
        }

        return selected;
    }

    /// <summary>
    /// Processes the selected choice.
    /// </summary>
    /// <param name="selected">The selected choice.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public new Task ProcessChoiceAsync(ChoiceOption selected)
    {
        // Handle choice processing, e.g., update scores
        GD.Print($"Choice selected: {selected.Text}");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies that the sequence is complete.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public new Task NotifySequenceCompleteAsync()
    {
        GD.Print("Sequence complete");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Starts the boot sequence.
    /// </summary>
    private async void StartBootSequence()
    {
        await this.PlayBootSequenceAsync();
        this.BootSequencePlayed = true;
    }

    /// <summary>
    /// Plays boot sequence with terminal effects.
    /// </summary>
    private new async Task PlayBootSequenceAsync()
    {
        if (this.Terminal == null)
        {
            // Fallback: use TextDisplay if Terminal is not available
            var textDisplay = this.GetNodeOrNull<RichTextLabel>("NarrativeStack/TextDisplay");
            if (textDisplay != null)
            {
                var bootText = "[color=#fdc962][INITIALIZING GHOST TERMINAL...]\n[LOADING ARCHIVES...]\n[SYSTEM READY][/color]";
                textDisplay.Text = bootText;
            }

            return;
        }

        var lines = new[]
        {
            "[INITIALIZING GHOST TERMINAL...]",
            "[LOADING ARCHIVES...]",
            "[SYSTEM READY]",
        };

        foreach (var line in lines)
        {
            await this.TypeTextAsync(line + "\n");
            await Task.Delay(500);
        }
    }

    /// <summary>
    /// Types text to terminal with delay.
    /// </summary>
    private async Task TypeTextAsync(string text)
    {
        if (this.Terminal == null)
        {
            return;
        }

        foreach (char c in text)
        {
            this.Terminal.Call("write", c.ToString());
            await Task.Delay((int)(1000 / this.DefaultTypingSpeed));
        }
    }

    private void ApplyGlitchEffect()
    {
        // TODO: Implement shader effect
        GD.Print("Applying glitch effect");
    }

    private void FadeToStable()
    {
        // TODO: Implement fade effect
        GD.Print("Fading to stable");
    }
}
