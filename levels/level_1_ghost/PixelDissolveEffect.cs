#pragma warning disable SA1636 // File header copyright text should match
// <copyright file="PixelDissolveEffect.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Stages.Stage1;
#pragma warning restore SA1636 // File header copyright text should match

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Implements pixel dissolve transition effect similar to the React prototype.
/// Creates character-by-character glitch dissolution using random ASCII characters.
/// </summary>
public sealed partial class PixelDissolveEffect : Node
{
    private const string _GlitchChars = "█▓▒░.:·¯`-_¸,ø¤º°`°º¤ø,¸¸,ø¤º°`";

    private RichTextLabel? _TextDisplay;
    private Timer? _DissolveTimer;
    private string[] _OriginalContent;
    private int _CurrentCharIndex;
    private float _Duration;
    private bool _IsDissolving;

    /// <summary>
    /// Signal emitted when the dissolve effect completes.
    /// </summary>
    [Signal]
    public delegate void DissolveCompleteEventHandler();

    public override void _Ready()
    {
        _TextDisplay = GetNode<RichTextLabel>("../NarrativeStack/TextDisplay");
        _DissolveTimer = new Timer
        {
            WaitTime = 0.05f, // Character delay
            OneShot = false
        };
        _DissolveTimer.Timeout += OnDissolveTick;
        AddChild(_DissolveTimer);
    }

    /// <summary>
    /// Starts the pixel dissolve effect on the current terminal content.
    /// </summary>
    /// <param name="duration">Total duration of the dissolve effect in milliseconds.</param>
    public async Task StartDissolveAsync(float duration = 2500f)
    {
        if (_IsDissolving || _TextDisplay == null)
            return;

        _Duration = duration;
        _IsDissolving = true;
        _CurrentCharIndex = 0;

        // Capture current content
        var currentText = _TextDisplay.Text;
        _OriginalContent = currentText.Split('\n');

        // Calculate character delay
        var totalChars = currentText.Length;
        var charDelay = duration / totalChars / 1000f; // Convert to seconds
        _DissolveTimer.WaitTime = charDelay;

        // Start the dissolve
        _DissolveTimer.Start();

        // Wait for completion
        await ToSignal(this, SignalName.DissolveComplete);
    }

    private void OnDissolveTick()
    {
        if (_TextDisplay == null || _OriginalContent == null)
            return;

        _CurrentCharIndex++;
        var totalChars = string.Join("\n", _OriginalContent).Length;
        var dissolveProgress = (float)_CurrentCharIndex / totalChars;

        // Generate glitched content
        var newLines = new List<string>();
        var charCounter = 0;

        foreach (var line in _OriginalContent)
        {
            var newLine = string.Empty;
            foreach (var character in line)
            {
                if (charCounter < _CurrentCharIndex)
                {
                    // This character should be glitched
                    if (character != ' ' && character != '\n' && !char.IsWhiteSpace(character))
                    {
                        // Multiple levels of glitch based on how long ago it was revealed
                        var glitchAge = _CurrentCharIndex - charCounter;
                        var glitchIntensity = Math.Min(glitchAge / 10f, 1f);

                        if (GD.Randf() < glitchIntensity * 0.7f)
                        {
                            var randomIndex = GD.RandRange(0, _GlitchChars.Length - 1);
                            newLine += _GlitchChars[randomIndex];
                        }
                        else
                        {
                            newLine += character;
                        }
                    }
                    else
                    {
                        newLine += character;
                    }
                }
                else
                {
                    // Not yet revealed
                    newLine += character;
                }
                charCounter++;
            }
            newLines.Add(newLine);
        }

        // Update display with glitched content
        _TextDisplay.Text = string.Join("\n", newLines);

        // Apply visual effects
        var opacity = 1f - dissolveProgress * 0.8f;
        var blur = dissolveProgress * 3f;

        _TextDisplay.Modulate = new Color(1f, 1f, 1f, opacity);

        // Apply blur effect through shader parameters if available
        if (_TextDisplay.Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter("blur_amount", blur);
        }

        // Check if complete
        if (_CurrentCharIndex >= totalChars)
        {
            _DissolveTimer.Stop();
            _IsDissolving = false;

            // Wait a bit before completing
            GetTree().CreateTimer(0.3f).Timeout += () =>
            {
                EmitSignal(SignalName.DissolveComplete);
            };
        }
    }

    /// <summary>
    /// Resets the dissolve effect to its initial state.
    /// </summary>
    public void Reset()
    {
        _IsDissolving = false;
        _CurrentCharIndex = 0;
        _DissolveTimer?.Stop();

        if (_TextDisplay != null)
        {
            _TextDisplay.Modulate = Colors.White;
            if (_TextDisplay.Material is ShaderMaterial shaderMaterial)
            {
                shaderMaterial.SetShaderParameter("blur_amount", 0f);
            }
        }
    }
}
