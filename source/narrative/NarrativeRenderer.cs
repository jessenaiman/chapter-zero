// <copyright file="NarrativeRenderer.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace OmegaSpiral.Source.Narrative;

/// <summary>
/// Simple narrative renderer - displays text and choices without any game logic.
/// Stage managers use this to render narrative scripts.
/// </summary>
[GlobalClass]
public partial class NarrativeRenderer : Control
{
    private RichTextLabel? _OutputLabel;
    private LineEdit? _InputField;
    private Button? _SubmitButton;
    private Label? _PromptLabel;

    private TaskCompletionSource<string>? _ChoiceWaiter;
    private List<NarrativeChoiceOption>? _CurrentOptions;

    /// <inheritdoc/>
    public override void _Ready()
    {
        _OutputLabel = GetNodeOrNull<RichTextLabel>("%OutputLabel");
        _InputField = GetNodeOrNull<LineEdit>("%InputField");
        _SubmitButton = GetNodeOrNull<Button>("%SubmitButton");
        _PromptLabel = GetNodeOrNull<Label>("%PromptLabel");

        if (_SubmitButton != null)
        {
            _SubmitButton.Pressed += OnSubmitPressed;
        }

        if (_InputField != null)
        {
            _InputField.TextSubmitted += _ => OnSubmitPressed();
        }
    }

    /// <summary>
    /// Displays lines of narrative text with typewriter effect.
    /// </summary>
    /// <param name="lines">The lines to display.</param>
    /// <param name="timing">Optional timing hint (e.g., "slow_burn", "rapid").</param>
    /// <param name="visualPreset">Optional visual preset to apply.</param>
    public async Task DisplayLinesAsync(List<string> lines, string? timing = null, string? visualPreset = null)
    {
        if (_OutputLabel == null)
        {
            GD.PrintErr("[NarrativeRenderer] OutputLabel not found!");
            return;
        }

        // Clear input while displaying
        if (_InputField != null)
        {
            _InputField.Editable = false;
        }

        foreach (var line in lines)
        {
            // Check for embedded pause commands like "[PAUSE: 2.5s]"
            if (line.StartsWith("[PAUSE:"))
            {
                var pauseText = line.Replace("[PAUSE:", "").Replace("s]", "").Replace("]", "").Trim();
                if (float.TryParse(pauseText, out var pauseSeconds))
                {
                    await Task.Delay((int)(pauseSeconds * 1000));
                }
                continue;
            }

            // Check for visual effect hints like "[CURSOR BLINKS]", "[TEXT GLITCHES: ...]"
            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                // For now, just print as-is (stages can handle these differently)
                _OutputLabel.Text += line + "\n";
                continue;
            }

            // Display the line with typewriter effect
            await TypewriterEffectAsync(line);
            _OutputLabel.Text += "\n";
        }
    }

    /// <summary>
    /// Shows a question with choice options and waits for player selection.
    /// Returns the selected choice ID.
    /// See <see cref="NarrativeChoiceOption"/> for choice structure.
    /// </summary>
    /// <param name="prompt">The question prompt.</param>
    /// <param name="context">Optional context/clarification text.</param>
    /// <param name="options">The choice options.</param>
    public async Task<string> ShowChoicesAndWaitAsync(string? prompt, string? context, List<NarrativeChoiceOption> options)
    {
        if (_OutputLabel == null || _InputField == null || _PromptLabel == null)
        {
            GD.PrintErr("[NarrativeRenderer] Required UI nodes not found!");
            return options.Count > 0 ? options[0].Id : string.Empty;
        }

        // Display prompt
        if (!string.IsNullOrEmpty(prompt))
        {
            _OutputLabel.Text += "\n" + prompt + "\n";
        }

        // Display context
        if (!string.IsNullOrEmpty(context))
        {
            _OutputLabel.Text += context + "\n";
        }

        _OutputLabel.Text += "\n";

        // Display options
        for (int i = 0; i < options.Count; i++)
        {
            _OutputLabel.Text += $"{i + 1}. {options[i].Text}\n";
        }

        _OutputLabel.Text += "\n";
        _PromptLabel.Text = "Enter your choice (1-" + options.Count + "):";

        // Enable input
        _InputField.Editable = true;
        _InputField.Text = "";
        _InputField.GrabFocus();

        // Store options and wait for selection
        _CurrentOptions = options;
        _ChoiceWaiter = new TaskCompletionSource<string>();

        var selectedId = await _ChoiceWaiter.Task;

        // Clear state
        _CurrentOptions = null;
        _ChoiceWaiter = null;
        _PromptLabel.Text = "";

        return selectedId;
    }

    /// <summary>
    /// Typewriter effect for a single line of text.
    /// </summary>
    private async Task TypewriterEffectAsync(string line)
    {
        if (_OutputLabel == null || string.IsNullOrEmpty(line))
        {
            return;
        }

        // For now, simple character-by-character display
        // TODO: Add configurable speed, sound effects, etc.
        foreach (var c in line)
        {
            _OutputLabel.Text += c;
            await Task.Delay(30); // ~33 chars/second
        }
    }

    /// <summary>
    /// Called when submit button is pressed or Enter key is hit.
    /// </summary>
    private void OnSubmitPressed()
    {
        if (_InputField == null || _CurrentOptions == null || _ChoiceWaiter == null)
        {
            return;
        }

        var input = _InputField.Text.Trim();

        // Try to parse as number (1-based)
        if (int.TryParse(input, out var choiceNum) && choiceNum >= 1 && choiceNum <= _CurrentOptions.Count)
        {
            var selectedOption = _CurrentOptions[choiceNum - 1];

            // Display the choice text in output
            if (_OutputLabel != null)
            {
                _OutputLabel.Text += $"> {selectedOption.Text}\n\n";
            }

            // Complete the wait task
            _ChoiceWaiter.SetResult(selectedOption.Id);
        }
        else
        {
            // Invalid input
            if (_OutputLabel != null)
            {
                _OutputLabel.Text += $"[Invalid choice: {input}]\n";
            }

            _InputField.Text = "";
            _InputField.GrabFocus();
        }
    }
}
