// <copyright file="TerminalBase.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Base terminal scene for Stage 1 opening sequences.
/// Provides shader layer management, audio playback, and text display without text input.
/// All Stage 1 scenes inherit from or use this as a foundation.
/// </summary>
[GlobalClass]
public partial class TerminalBase : Control
{
    /// <summary>Shader layer for phosphor glow effects.</summary>
    private ColorRect _phosphorLayer = default!;

    /// <summary>Shader layer for scanline overlay.</summary>
    private ColorRect _scanlineLayer = default!;

    /// <summary>Shader layer for glitch effects.</summary>
    private ColorRect _glitchLayer = default!;

    /// <summary>Rich text label for terminal text display.</summary>
    private RichTextLabel _textDisplay = default!;

    /// <summary>Container for choice buttons.</summary>
    private VBoxContainer _choiceContainer = default!;

    /// <summary>Main terminal content container.</summary>
    private VBoxContainer _terminalContent = default!;

    /// <summary>Audio player for ambient sounds (CRT hum).</summary>
    private AudioStreamPlayer _ambientAudio = default!;

    /// <summary>Audio player for sound effects (glitches, typewriter).</summary>
    private AudioStreamPlayer _effectsAudio = default!;

    /// <summary>Audio player for UI sounds (button clicks).</summary>
    private AudioStreamPlayer _uiAudio = default!;

    /// <summary>Audio player for music and resonance tones.</summary>
    private AudioStreamPlayer _musicAudio = default!;

    /// <summary>Animation player for shader transitions.</summary>
    private AnimationPlayer _animationPlayer = default!;

    /// <summary>Label for closed caption display.</summary>
    private Label _captionLabel = default!;

    /// <summary>
    /// Gets or sets the current phosphor tint color for thread-specific theming.
    /// </summary>
    public Color PhosphorTint { get; set; } = new Color(0.2f, 1.0f, 0.4f); // Green CRT default

    /// <summary>
    /// Gets or sets whether closed captions are enabled.
    /// </summary>
    public bool CaptionsEnabled { get; set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Get unique name references
        _phosphorLayer = GetNode<ColorRect>("%PhosphorLayer");
        _scanlineLayer = GetNode<ColorRect>("%ScanlineLayer");
        _glitchLayer = GetNode<ColorRect>("%GlitchLayer");

        _textDisplay = GetNode<RichTextLabel>("%TextDisplay");
        _choiceContainer = GetNode<VBoxContainer>("%ChoiceContainer");
        _terminalContent = GetNode<VBoxContainer>("%TerminalContent");

        _ambientAudio = GetNode<AudioStreamPlayer>("%AmbientAudio");
        _effectsAudio = GetNode<AudioStreamPlayer>("%EffectsAudio");
        _uiAudio = GetNode<AudioStreamPlayer>("%UIAudio");
        _musicAudio = GetNode<AudioStreamPlayer>("%MusicAudio");

        _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        _captionLabel = GetNode<Label>("%CaptionLabel");

        // Initialize
        _textDisplay.Clear();
        _choiceContainer.Visible = false;
        _captionLabel.Visible = CaptionsEnabled;

        // Set default CRT green text color
        _textDisplay.AddThemeColorOverride("default_color", new Color(0.2f, 1.0f, 0.4f));
        _textDisplay.AddThemeColorOverride("font_shadow_color", new Color(0.0f, 0.5f, 0.2f, 0.3f));

        // Load and apply shader materials (fallback to null if not found)
        var phosphorShader = ResourceLoader.Load<ShaderMaterial>("res://Source/Shaders/crt_phosphor.tres");
        var scanlineShader = ResourceLoader.Load<ShaderMaterial>("res://Source/Shaders/crt_scanlines.tres");
        var glitchShader = ResourceLoader.Load<ShaderMaterial>("res://Source/Shaders/crt_glitch.tres");

        if (phosphorShader != null)
        {
            _phosphorLayer.Material = phosphorShader;
        }

        if (scanlineShader != null)
        {
            _scanlineLayer.Material = scanlineShader;
        }

        if (glitchShader != null)
        {
            _glitchLayer.Material = glitchShader;
        }
    }

    /// <summary>
    /// Displays text with optional typewriter effect.
    /// </summary>
    /// <param name="text">The BBCode text to display.</param>
    /// <param name="instant">If <see langword="true"/>, display instantly without animation.</param>
    /// <returns>A task that completes when text is fully displayed.</returns>
    public async Task DisplayTextAsync(string text, bool instant = false)
    {
        _textDisplay.Clear();

        if (instant)
        {
            _textDisplay.AppendText(text);
            return;
        }

        // Typewriter effect character by character
        foreach (char c in text)
        {
            _textDisplay.AppendText(c.ToString());

            // Play typewriter audio on each character
            // TODO: Integrate AudioSynthesizer for procedural click sounds

            await ToSignal(GetTree().CreateTimer(0.03f), SceneTreeTimer.SignalName.Timeout);
        }
    }

    /// <summary>
    /// Appends a new line of text to the display.
    /// </summary>
    /// <param name="text">The BBCode text to append.</param>
    /// <param name="instant">If <see langword="true"/>, append instantly.</param>
    /// <returns>A task that completes when text is appended.</returns>
    public async Task AppendTextAsync(string text, bool instant = false)
    {
        // Add newline if there's already content
        if (_textDisplay.Text.Length > 0)
        {
            _textDisplay.AppendText("\n");
        }

        if (instant)
        {
            _textDisplay.AppendText(text);
            return;
        }

        // Typewriter effect for appended text
        foreach (char c in text)
        {
            _textDisplay.AppendText(c.ToString());

            // Play typewriter audio on each character
            // TODO: Integrate AudioSynthesizer for procedural click sounds

            await ToSignal(GetTree().CreateTimer(0.03f), SceneTreeTimer.SignalName.Timeout);
        }
    }

    /// <summary>
    /// Clears all displayed text.
    /// </summary>
    public void ClearText()
    {
        _textDisplay.Clear();
    }

    /// <summary>
    /// Shows closed caption text.
    /// </summary>
    /// <param name="caption">The caption text to display.</param>
    /// <param name="duration">How long to display the caption in seconds.</param>
    public async void ShowCaption(string caption, float duration = 3.0f)
    {
        if (!CaptionsEnabled)
        {
            return;
        }

        _captionLabel.Text = caption;
        _captionLabel.Visible = true;

        await ToSignal(GetTree().CreateTimer(duration), SceneTreeTimer.SignalName.Timeout);

        _captionLabel.Visible = false;
    }

    /// <summary>
    /// Sets shader parameter values for visual state changes.
    /// </summary>
    /// <param name="layer">Which shader layer to modify.</param>
    /// <param name="parameterName">The shader uniform parameter name.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentException">Thrown when an unknown shader layer is specified.</exception>
    public void SetShaderParameter(ShaderLayer layer, string parameterName, Variant value)
    {
        ColorRect targetLayer = layer switch
        {
            ShaderLayer.Phosphor => _phosphorLayer,
            ShaderLayer.Scanline => _scanlineLayer,
            ShaderLayer.Glitch => _glitchLayer,
            _ => throw new ArgumentException($"Unknown shader layer: {layer}", nameof(layer))
        };

        if (targetLayer.Material is ShaderMaterial shaderMaterial)
        {
            shaderMaterial.SetShaderParameter(parameterName, value);
        }
    }

    /// <summary>
    /// Plays audio on the specified bus.
    /// </summary>
    /// <param name="bus">Which audio bus to use.</param>
    /// <param name="stream">The audio stream to play.</param>
    /// <exception cref="ArgumentException">Thrown when an unknown audio bus is specified.</exception>
    public void PlayAudio(AudioBus bus, AudioStream stream)
    {
        AudioStreamPlayer player = bus switch
        {
            AudioBus.Ambient => _ambientAudio,
            AudioBus.Effects => _effectsAudio,
            AudioBus.UI => _uiAudio,
            AudioBus.Music => _musicAudio,
            _ => throw new ArgumentException($"Unknown audio bus: {bus}", nameof(bus))
        };

        player.Stream = stream;
        player.Play();
    }

    /// <summary>
    /// Stops audio on the specified bus.
    /// </summary>
    /// <param name="bus">Which audio bus to stop.</param>
    /// <exception cref="ArgumentException">Thrown when an unknown audio bus is specified.</exception>
    public void StopAudio(AudioBus bus)
    {
        AudioStreamPlayer player = bus switch
        {
            AudioBus.Ambient => _ambientAudio,
            AudioBus.Effects => _effectsAudio,
            AudioBus.UI => _uiAudio,
            AudioBus.Music => _musicAudio,
            _ => throw new ArgumentException($"Unknown audio bus: {bus}", nameof(bus))
        };

        player.Stop();
    }

    /// <summary>
    /// Displays multiple choice options and handles selection.
    /// </summary>
    /// <param name="question">The question text to display.</param>
    /// <param name="choices">Array of choice options.</param>
    /// <param name="onChoiceSelected">Callback invoked when a choice is selected.</param>
    /// <returns>A task that completes when a choice is made.</returns>
    public async Task<string> PresentChoicesAsync(string question, string[] choices, Action<string>? onChoiceSelected = null)
    {
        // Clear existing choices
        foreach (Node child in _choiceContainer.GetChildren())
        {
            child.QueueFree();
        }

        // Display question
        await DisplayTextAsync(question);

        // Create choice buttons
        var completionSource = new TaskCompletionSource<string>();

        foreach (string choice in choices)
        {
            var button = new Button
            {
                Text = choice,
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                ThemeTypeVariation = "TerminalButton" // Custom theme for terminal styling
            };

            button.Pressed += () =>
            {
                PlayAudio(AudioBus.UI, ResourceLoader.Load<AudioStream>("res://Source/Audio/ui_select.wav"));
                onChoiceSelected?.Invoke(choice);
                completionSource.SetResult(choice);
            };

            _choiceContainer.AddChild(button);
        }

        _choiceContainer.Visible = true;

        // Wait for choice selection
        string selectedChoice = await completionSource.Task;

        // Hide choices after selection
        _choiceContainer.Visible = false;

        return selectedChoice;
    }

    /// <summary>
    /// Transitions to the next scene in the Stage 1 sequence.
    /// </summary>
    /// <param name="nextScenePath">Path to the next scene file.</param>
    public void TransitionToScene(string nextScenePath)
    {
        // Play transition audio
        PlayAudio(AudioBus.Effects, ResourceLoader.Load<AudioStream>("res://Source/Audio/transition.wav"));

        // Trigger transition animation
        _animationPlayer.Play("scene_transition");

        // Wait for animation, then change scene
        _animationPlayer.AnimationFinished += (animName) =>
        {
            if (animName == "scene_transition")
            {
                var nextScene = ResourceLoader.Load<PackedScene>(nextScenePath);
                if (nextScene != null)
                {
                    GetTree().ChangeSceneToPacked(nextScene);
                }
                else
                {
                    GD.PrintErr($"[TerminalBase] Failed to load scene: {nextScenePath}");
                }
            }
        };
    }

    /// <summary>
    /// Gets the global GameState instance for tracking choices and scores.
    /// </summary>
    /// <returns>The GameState singleton instance.</returns>
    protected GameState GetGameState()
    {
        return GetNode<GameState>("/root/GameState");
    }

    /// <summary>
    /// Prompts the user for text input.
    /// </summary>
    /// <param name="prompt">The prompt text to display.</param>
    /// <param name="placeholder">Placeholder text for the input field.</param>
    /// <returns>A task that completes with the entered text.</returns>
    public async Task<string> GetTextInputAsync(string prompt, string placeholder = "")
    {
        // Display prompt
        await DisplayTextAsync(prompt);

        // Create text input field
        var lineEdit = new LineEdit
        {
            PlaceholderText = placeholder,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            ThemeTypeVariation = "TerminalInput" // Custom theme for terminal styling
        };

        var submitButton = new Button
        {
            Text = "ENTER",
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            ThemeTypeVariation = "TerminalButton"
        };

        // Add to choice container (reusing for input UI)
        _choiceContainer.AddChild(lineEdit);
        _choiceContainer.AddChild(submitButton);
        _choiceContainer.Visible = true;

        var completionSource = new TaskCompletionSource<string>();

        // Handle submission
        void OnSubmit()
        {
            string inputText = lineEdit.Text.Trim();
            if (!string.IsNullOrEmpty(inputText))
            {
                PlayAudio(AudioBus.UI, ResourceLoader.Load<AudioStream>("res://Source/Audio/ui_select.wav"));
                completionSource.SetResult(inputText);
            }
        }

        submitButton.Pressed += OnSubmit;
        lineEdit.TextSubmitted += (_) => OnSubmit();

        // Focus the input field
        lineEdit.GrabFocus();

        // Wait for input
        string result = await completionSource.Task;

        // Clean up
        _choiceContainer.Visible = false;
        lineEdit.QueueFree();
        submitButton.QueueFree();

        return result;
    }
}

/// <summary>
/// Shader layer identifiers for TerminalBase.
/// </summary>
public enum ShaderLayer
{
    /// <summary>Base phosphor glow and color tint layer.</summary>
    Phosphor,

    /// <summary>Scanline overlay layer.</summary>
    Scanline,

    /// <summary>Glitch effects layer.</summary>
    Glitch,
}

/// <summary>
/// Audio bus identifiers for TerminalBase.
/// </summary>
public enum AudioBus
{
    /// <summary>Ambient sounds (CRT hum, background).</summary>
    Ambient,

    /// <summary>Sound effects (glitches, typewriter).</summary>
    Effects,

    /// <summary>UI sounds (button clicks, selection).</summary>
    UI,

    /// <summary>Music and resonance tones.</summary>
    Music,
}
