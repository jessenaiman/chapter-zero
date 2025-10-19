using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Base terminal scene for Stage 1 opening sequences.
/// Provides shader layer management, audio playback, and advanced text animation effects.
/// All Stage 1 scenes inherit from or use this as a foundation.
/// </summary>
[GlobalClass]
public partial class TerminalBase : Control
{
    private const string UiSelectAudioPath = "res://Source/Assets/sfx/confirmation_002.ogg";
    private const string TransitionAudioPath = "res://Source/Assets/sfx/doorOpen_2.ogg";

    private static readonly char[] GlitchCharacters = "█▓▒░◊∞Ω≋※▉▐▌".ToCharArray();

    private ColorRect _phosphorLayer = default!;
    private ColorRect _scanlineLayer = default!;
    private ColorRect _glitchLayer = default!;

    private RichTextLabel _textDisplay = default!;
    private VBoxContainer _choiceContainer = default!;
    private VBoxContainer _terminalContent = default!;

    private AudioStreamPlayer _ambientAudio = default!;
    private AudioStreamPlayer _effectsAudio = default!;
    private AudioStreamPlayer _uiAudio = default!;
    private AudioStreamPlayer _musicAudio = default!;

    private AnimationPlayer _animationPlayer = default!;
    private Label _captionLabel = default!;

    private ShaderMaterial? _phosphorMaterial;
    private ShaderMaterial? _scanlineMaterial;
    private ShaderMaterial? _glitchMaterial;

    private readonly StringBuilder _textBuffer = new();
    private readonly RandomNumberGenerator _rng = new();
    private Vector3 _moodTint = new(0.2f, 1.0f, 0.4f);

    /// <summary>Gets or sets whether closed captions are enabled.</summary>
    public bool CaptionsEnabled { get; set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        CacheNodeReferences();
        ConfigureShaderMaterials();

        _rng.Randomize();
        _textBuffer.Clear();

        _textDisplay.Text = string.Empty;
        _textDisplay.Modulate = Colors.White;
        _choiceContainer.Visible = false;
        _captionLabel.Visible = CaptionsEnabled;

        ApplyPressStartMood(GetGameState().PressStartMood);
        ApplyVisualPreset(TerminalVisualPreset.StableBaseline);
    }

    private void CacheNodeReferences()
    {
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
    }

    private void ConfigureShaderMaterials()
    {
        _phosphorMaterial = LoadShaderInstance("res://Source/Shaders/crt_phosphor.tres");
        if (_phosphorMaterial != null)
        {
            _phosphorLayer.Material = _phosphorMaterial;
        }

        _scanlineMaterial = LoadShaderInstance("res://Source/Shaders/crt_scanlines.tres");
        if (_scanlineMaterial != null)
        {
            _scanlineLayer.Material = _scanlineMaterial;
        }

        _glitchMaterial = LoadShaderInstance("res://Source/Shaders/crt_glitch.tres");
        if (_glitchMaterial != null)
        {
            _glitchLayer.Material = _glitchMaterial;
        }
    }

    private static ShaderMaterial? LoadShaderInstance(string path)
    {
        if (ResourceLoader.Load<ShaderMaterial>(path) is ShaderMaterial shaderMaterial)
        {
            return (ShaderMaterial) shaderMaterial.Duplicate();
        }

        GD.PushWarning($"[TerminalBase] Unable to load shader material at '{path}'.");
        return null;
    }

    /// <summary>
    /// Displays text with optional typewriter or ghostwriting effect.
    /// </summary>
    /// <param name="text">The text to render.</param>
    /// <param name="instant">When true, text renders immediately without animation.</param>
    /// <param name="useGhostEffect">When true, applies the ghostwriting glitch effect.</param>
    /// <param name="charDelaySeconds">Delay between characters in seconds.</param>
    public async Task DisplayTextAsync(string text, bool instant = false, bool useGhostEffect = false, double charDelaySeconds = 0.03)
    {
        await WriteTextAsync(text, append: false, instant, useGhostEffect, charDelaySeconds);
    }

    /// <summary>
    /// Appends text below the existing buffer with optional animation.
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <param name="instant">When true, text renders immediately without animation.</param>
    /// <param name="useGhostEffect">When true, applies the ghostwriting glitch effect.</param>
    /// <param name="charDelaySeconds">Delay between characters in seconds.</param>
    public async Task AppendTextAsync(string text, bool instant = false, bool useGhostEffect = false, double charDelaySeconds = 0.03)
    {
        await WriteTextAsync(text, append: true, instant, useGhostEffect, charDelaySeconds);
    }

    private async Task WriteTextAsync(string text, bool append, bool instant, bool useGhostEffect, double charDelaySeconds)
    {
        if (!append)
        {
            _textBuffer.Clear();
        }
        else if (_textBuffer.Length > 0)
        {
            _textBuffer.Append('\n');
        }

        if (instant || string.IsNullOrEmpty(text))
        {
            _textBuffer.Append(text);
            _textDisplay.Text = _textBuffer.ToString();
            return;
        }

        if (useGhostEffect)
        {
            await GhostWriteInternalAsync(text, charDelaySeconds);
        }
        else
        {
            await TypewriteInternalAsync(text, charDelaySeconds);
        }
    }

    private async Task TypewriteInternalAsync(string text, double charDelaySeconds)
    {
        foreach (char glyph in text)
        {
            _textBuffer.Append(glyph);
            _textDisplay.Text = _textBuffer.ToString();

            await ToSignal(GetTree().CreateTimer(charDelaySeconds), SceneTreeTimer.SignalName.Timeout);
        }
    }

    private async Task GhostWriteInternalAsync(string text, double charDelaySeconds)
    {
        foreach (char glyph in text)
        {
            _textBuffer.Append(glyph);
            string canonical = _textBuffer.ToString();

            bool shouldGlitch = !char.IsWhiteSpace(glyph) && _rng.Randf() < 0.22f;
            if (shouldGlitch)
            {
                char glitchGlyph = GlitchCharacters[_rng.RandiRange(0, GlitchCharacters.Length - 1)];
                var glitchBuilder = new StringBuilder(canonical);
                glitchBuilder[glitchBuilder.Length - 1] = glitchGlyph;
                _textDisplay.Text = glitchBuilder.ToString();
                await ToSignal(GetTree().CreateTimer(Math.Max(charDelaySeconds * 0.35, 0.01)), SceneTreeTimer.SignalName.Timeout);
            }

            _textDisplay.Text = canonical;
            await ToSignal(GetTree().CreateTimer(charDelaySeconds), SceneTreeTimer.SignalName.Timeout);
        }
    }

    /// <summary>
    /// Clears all displayed text.
    /// </summary>
    public void ClearText()
    {
        _textBuffer.Clear();
        _textDisplay.Text = string.Empty;
        _textDisplay.Modulate = Colors.White;
    }

    /// <summary>
    /// Displays multiple choice options and handles selection.
    /// </summary>
    public async Task<string> PresentChoicesAsync(string question, string[] choices, Action<string>? onChoiceSelected = null, bool ghostPrompt = true)
    {
        foreach (Node child in _choiceContainer.GetChildren())
        {
            child.QueueFree();
        }

        await DisplayTextAsync(question, useGhostEffect: ghostPrompt);

        var completionSource = new TaskCompletionSource<string>();

        foreach (string choice in choices)
        {
            var button = new Button
            {
                Text = choice,
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                ThemeTypeVariation = "TerminalButton"
            };

            button.Pressed += () =>
            {
                PlayAudio(AudioBus.UI, LoadAudioStream(UiSelectAudioPath));
                onChoiceSelected?.Invoke(choice);
                completionSource.TrySetResult(choice);
            };

            _choiceContainer.AddChild(button);
        }

        _choiceContainer.Visible = true;

        string selectedChoice = await completionSource.Task;

        _choiceContainer.Visible = false;
        foreach (Node child in _choiceContainer.GetChildren())
        {
            child.QueueFree();
        }

        return selectedChoice;
    }

    /// <summary>
    /// Prompts the user for text input.
    /// </summary>
    public async Task<string> GetTextInputAsync(string prompt, string placeholder = "")
    {
        await DisplayTextAsync(prompt, useGhostEffect: true);

        var lineEdit = new LineEdit
        {
            PlaceholderText = placeholder,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            ThemeTypeVariation = "TerminalInput"
        };

        var submitButton = new Button
        {
            Text = "ENTER",
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            ThemeTypeVariation = "TerminalButton"
        };

        _choiceContainer.AddChild(lineEdit);
        _choiceContainer.AddChild(submitButton);
        _choiceContainer.Visible = true;

        var completionSource = new TaskCompletionSource<string>();

        void SubmitInput()
        {
            string input = lineEdit.Text.Trim();
            if (!string.IsNullOrEmpty(input))
            {
                PlayAudio(AudioBus.UI, LoadAudioStream(UiSelectAudioPath));
                completionSource.TrySetResult(input);
            }
        }

        submitButton.Pressed += SubmitInput;
        lineEdit.TextSubmitted += _ => SubmitInput();
        lineEdit.GrabFocus();

        string result = await completionSource.Task;

        _choiceContainer.Visible = false;
        lineEdit.QueueFree();
        submitButton.QueueFree();

        return result;
    }

    /// <summary>
    /// Performs a pixel dissolve effect on the displayed text.
    /// </summary>
    public async Task PixelDissolveAsync(double durationSeconds = 2.5)
    {
        if (_textBuffer.Length == 0)
        {
            return;
        }

        var characters = _textBuffer.ToString().ToCharArray();
        var indices = Enumerable.Range(0, characters.Length)
            .Where(i => characters[i] != '\n' && !char.IsWhiteSpace(characters[i]))
            .OrderBy(_ => _rng.Randf())
            .ToList();

        if (indices.Count == 0)
        {
            ClearText();
            return;
        }

        double interval = durationSeconds / Math.Max(indices.Count, 1);

        for (int i = 0; i < indices.Count; i++)
        {
            int index = indices[i];
            characters[index] = GlitchCharacters[_rng.RandiRange(0, GlitchCharacters.Length - 1)];

            float progress = (float) (i + 1) / indices.Count;
            _textDisplay.Text = new string(characters);
            _textDisplay.Modulate = new Color(1f, 1f, 1f, Math.Max(0.25f, 1f - progress * 0.85f));

            await ToSignal(GetTree().CreateTimer(interval), SceneTreeTimer.SignalName.Timeout);
        }

        ClearText();
    }

    /// <summary>
    /// Plays audio on the specified bus.
    /// </summary>
    public void PlayAudio(AudioBus bus, AudioStream? stream)
    {
        if (stream is null)
        {
            GD.PushWarning($"[TerminalBase] Attempted to play audio on {bus} without a valid stream.");
            return;
        }

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
    /// Transitions to the next scene in the Stage 1 sequence.
    /// </summary>
    /// <param name="nextScenePath">Path to the next scene file.</param>
    public void TransitionToScene(string nextScenePath)
    {
        PlayAudio(AudioBus.Effects, LoadAudioStream(TransitionAudioPath));

        if (ResourceLoader.Load<PackedScene>(nextScenePath) is not { } nextScene)
        {
            GD.PrintErr($"[TerminalBase] Failed to load scene: {nextScenePath}");
            return;
        }

        if (_animationPlayer.HasAnimation("scene_transition"))
        {
            void Handler(StringName animationName)
            {
                if (animationName == "scene_transition")
                {
                    _animationPlayer.AnimationFinished -= Handler;
                    GetTree().ChangeSceneToPacked(nextScene);
                }
            }

            _animationPlayer.AnimationFinished += Handler;
            _animationPlayer.Play("scene_transition");
            return;
        }

        GD.PushWarning("[TerminalBase] Missing 'scene_transition' animation; changing scene immediately.");
        GetTree().ChangeSceneToPacked(nextScene);
    }

    /// <summary>
    /// Applies a preconfigured visual preset to the terminal.
    /// </summary>
    public void ApplyVisualPreset(TerminalVisualPreset preset)
    {
        if (_phosphorMaterial is null || _scanlineMaterial is null || _glitchMaterial is null)
        {
            return;
        }

        switch (preset)
        {
            case TerminalVisualPreset.BootSequence:
                SetPhosphorSettings(glow: 1.6f, curvature: 0.18f, vignetteStrength: 0.35f, vignetteSoftness: 0.45f, spread: 1.15f, chromatic: 2.5f, brightness: 1.05f, contrast: 1.2f, tint: _moodTint);
                SetScanlineSettings(opacity: 0.12f, speed: 9.0f, count: 520f, thickness: 1.4f, tint: Vector3.One);
                SetGlitchSettings(intensity: 0.85f, interferenceSpeed: 18.0f, chromaticOffset: 7.0f, blockSize: 18.0f, blockIntensity: 0.7f, noiseAmount: 0.6f, displacementStrength: 0.08f);
                break;
            case TerminalVisualPreset.StableBaseline:
                SetPhosphorSettings(glow: 1.2f, curvature: 0.15f, vignetteStrength: 0.3f, vignetteSoftness: 0.5f, spread: 1.0f, chromatic: 1.5f, brightness: 1.0f, contrast: 1.1f, tint: _moodTint);
                SetScanlineSettings(opacity: 0.08f, speed: 5.0f, count: 420f, thickness: 1.2f, tint: Vector3.One);
                SetGlitchSettings(intensity: 0.05f, interferenceSpeed: 8.0f, chromaticOffset: 2.0f, blockSize: 22.0f, blockIntensity: 0.2f, noiseAmount: 0.18f, displacementStrength: 0.02f);
                break;
            case TerminalVisualPreset.SecretReveal:
                SetPhosphorSettings(glow: 2.4f, curvature: 0.17f, vignetteStrength: 0.6f, vignetteSoftness: 0.4f, spread: 1.35f, chromatic: 3.2f, brightness: 1.15f, contrast: 1.25f, tint: new Vector3(0.9f, 0.95f, 1.0f));
                SetScanlineSettings(opacity: 0.06f, speed: 0.2f, count: 480f, thickness: 1.0f, tint: new Vector3(0.95f, 1.0f, 1.0f));
                SetGlitchSettings(intensity: 1.0f, interferenceSpeed: 22.0f, chromaticOffset: 8.0f, blockSize: 14.0f, blockIntensity: 0.85f, noiseAmount: 0.45f, displacementStrength: 0.09f);
                break;
            case TerminalVisualPreset.ThreadLight:
                SetPhosphorSettings(glow: 1.35f, curvature: 0.15f, vignetteStrength: 0.18f, vignetteSoftness: 0.55f, spread: 1.1f, chromatic: 1.2f, brightness: 1.05f, contrast: 1.05f, tint: new Vector3(1.0f, 0.92f, 0.56f));
                SetScanlineSettings(opacity: 0.25f, speed: 4.5f, count: 410f, thickness: 1.1f, tint: new Vector3(1.0f, 0.95f, 0.75f));
                SetGlitchSettings(intensity: 0.22f, interferenceSpeed: 6.0f, chromaticOffset: 1.5f, blockSize: 28.0f, blockIntensity: 0.15f, noiseAmount: 0.12f, displacementStrength: 0.02f);
                break;
            case TerminalVisualPreset.ThreadMischief:
                SetPhosphorSettings(glow: 1.4f, curvature: 0.15f, vignetteStrength: 0.28f, vignetteSoftness: 0.48f, spread: 1.2f, chromatic: 2.2f, brightness: 1.0f, contrast: 1.15f, tint: new Vector3(0.4f, 0.9f, 1.0f));
                SetScanlineSettings(opacity: 0.12f, speed: 7.5f, count: 430f, thickness: 1.3f, tint: new Vector3(0.75f, 0.95f, 1.0f));
                SetGlitchSettings(intensity: 0.45f, interferenceSpeed: 12.0f, chromaticOffset: 4.0f, blockSize: 20.0f, blockIntensity: 0.35f, noiseAmount: 0.25f, displacementStrength: 0.05f);
                break;
            case TerminalVisualPreset.ThreadWrath:
                SetPhosphorSettings(glow: 1.5f, curvature: 0.16f, vignetteStrength: 0.4f, vignetteSoftness: 0.45f, spread: 1.3f, chromatic: 2.8f, brightness: 1.05f, contrast: 1.2f, tint: new Vector3(1.0f, 0.45f, 0.45f));
                SetScanlineSettings(opacity: 0.18f, speed: 6.0f, count: 400f, thickness: 1.4f, tint: new Vector3(1.0f, 0.6f, 0.6f));
                SetGlitchSettings(intensity: 0.55f, interferenceSpeed: 14.0f, chromaticOffset: 5.5f, blockSize: 18.0f, blockIntensity: 0.45f, noiseAmount: 0.3f, displacementStrength: 0.06f);
                break;
            case TerminalVisualPreset.ThreadBalance:
                SetPhosphorSettings(glow: 1.3f, curvature: 0.15f, vignetteStrength: 0.24f, vignetteSoftness: 0.52f, spread: 1.2f, chromatic: 1.8f, brightness: 1.0f, contrast: 1.12f, tint: new Vector3(0.8f, 0.85f, 1.0f));
                SetScanlineSettings(opacity: 0.14f, speed: 5.5f, count: 420f, thickness: 1.2f, tint: new Vector3(0.9f, 0.95f, 1.0f));
                SetGlitchSettings(intensity: 0.3f, interferenceSpeed: 10.0f, chromaticOffset: 3.0f, blockSize: 22.0f, blockIntensity: 0.25f, noiseAmount: 0.2f, displacementStrength: 0.04f);
                break;
        }
    }

    /// <summary>
    /// Directly sets a shader parameter on one of the terminal layers.
    /// </summary>
    public void SetShaderParameter(ShaderLayer layer, string parameterName, Variant value)
    {
        ShaderMaterial? targetMaterial = layer switch
        {
            ShaderLayer.Phosphor => _phosphorMaterial,
            ShaderLayer.Scanline => _scanlineMaterial,
            ShaderLayer.Glitch => _glitchMaterial,
            _ => null
        };

        targetMaterial?.SetShaderParameter(parameterName, value);
    }

    private void SetPhosphorSettings(float glow, float curvature, float vignetteStrength, float vignetteSoftness, float spread, float chromatic, float brightness, float contrast, Vector3 tint)
    {
        if (_phosphorMaterial is null)
        {
            return;
        }

        _phosphorMaterial.SetShaderParameter("phosphor_glow", glow);
        _phosphorMaterial.SetShaderParameter("curvature_strength", curvature);
        _phosphorMaterial.SetShaderParameter("vignette_strength", vignetteStrength);
        _phosphorMaterial.SetShaderParameter("vignette_softness", vignetteSoftness);
        _phosphorMaterial.SetShaderParameter("phosphor_spread", spread);
        _phosphorMaterial.SetShaderParameter("chromatic_aberration", chromatic);
        _phosphorMaterial.SetShaderParameter("brightness", brightness);
        _phosphorMaterial.SetShaderParameter("contrast", contrast);
        _phosphorMaterial.SetShaderParameter("phosphor_tint", tint);
    }

    private void SetScanlineSettings(float opacity, float speed, float count, float thickness, Vector3 tint)
    {
        if (_scanlineMaterial is null)
        {
            return;
        }

        _scanlineMaterial.SetShaderParameter("scanline_opacity", opacity);
        _scanlineMaterial.SetShaderParameter("scanline_speed", speed);
        _scanlineMaterial.SetShaderParameter("scanline_count", count);
        _scanlineMaterial.SetShaderParameter("scanline_thickness", thickness);
        _scanlineMaterial.SetShaderParameter("scanline_tint", tint);
    }

    private void SetGlitchSettings(float intensity, float interferenceSpeed, float chromaticOffset, float blockSize, float blockIntensity, float noiseAmount, float displacementStrength)
    {
        if (_glitchMaterial is null)
        {
            return;
        }

        _glitchMaterial.SetShaderParameter("glitch_intensity", intensity);
        _glitchMaterial.SetShaderParameter("interference_speed", interferenceSpeed);
        _glitchMaterial.SetShaderParameter("chromatic_offset", chromaticOffset);
        _glitchMaterial.SetShaderParameter("block_size", blockSize);
        _glitchMaterial.SetShaderParameter("block_intensity", blockIntensity);
        _glitchMaterial.SetShaderParameter("noise_amount", noiseAmount);
        _glitchMaterial.SetShaderParameter("displacement_strength", displacementStrength);
    }

    private void ApplyPressStartMood(PressStartMood mood)
    {
        _moodTint = mood switch
        {
            PressStartMood.Ominous => new Vector3(0.85f, 0.35f, 0.45f),
            _ => new Vector3(0.2f, 1.0f, 0.4f)
        };
    }

    /// <summary>
    /// Gets the global GameState instance for tracking choices and scores.
    /// </summary>
    protected GameState GetGameState()
    {
        return GetNode<GameState>("/root/GameState");
    }

    private static AudioStream? LoadAudioStream(string resourcePath)
    {
        var stream = ResourceLoader.Load<AudioStream>(resourcePath);
        if (stream is null)
        {
            GD.PushWarning($"[TerminalBase] Unable to load audio stream at '{resourcePath}'.");
        }

        return stream;
    }
}

/// <summary>
/// Shader layer identifiers for TerminalBase.
/// </summary>
public enum ShaderLayer
{
    Phosphor,
    Scanline,
    Glitch,
}

/// <summary>
/// Audio bus identifiers for TerminalBase.
/// </summary>
public enum AudioBus
{
    Ambient,
    Effects,
    UI,
    Music,
}

/// <summary>
/// Visual presets applied to the terminal.
/// </summary>
public enum TerminalVisualPreset
{
    BootSequence,
    StableBaseline,
    SecretReveal,
    ThreadLight,
    ThreadMischief,
    ThreadWrath,
    ThreadBalance,
}
