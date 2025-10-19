// <copyright file="TerminalBase.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;
using System.Threading.Tasks;

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

        // TODO: Load shader materials once created
        // _phosphorLayer.Material = ResourceLoader.Load<ShaderMaterial>("res://Source/Shaders/CRT_Phosphor.tres");
        // _scanlineLayer.Material = ResourceLoader.Load<ShaderMaterial>("res://Source/Shaders/CRT_Scanlines.tres");
        // _glitchLayer.Material = ResourceLoader.Load<ShaderMaterial>("res://Source/Shaders/CRT_Glitch.tres");
    }

    /// <summary>
    /// Displays text with optional typewriter effect.
    /// </summary>
    /// <param name="text">The BBCode text to display.</param>
    /// <param name="instant">If <see langword="true"/>, display instantly without animation.</param>
    /// <returns>A task that completes when text is fully displayed.</returns>
    public async Task DisplayTextAsync(string text, bool instant = false)
    {
        if (instant)
        {
            _textDisplay.Text = text;
            return;
        }

        // TODO: Implement typewriter effect with audio
        _textDisplay.Text = string.Empty;
        await ToSignal(GetTree().CreateTimer(0.05f), SceneTreeTimer.SignalName.Timeout);

        for (int i = 0; i <= text.Length; i++)
        {
            _textDisplay.Text = text[..i];

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
        string currentText = _textDisplay.Text;
        string newText = string.IsNullOrEmpty(currentText) ? text : $"{currentText}\n{text}";
        await DisplayTextAsync(newText, instant);
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
