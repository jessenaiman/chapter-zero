using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Common.Terminal;

namespace OmegaSpiral.Source.Scripts.Common;

/// <summary>
/// Terminal interface for narrative scenes with CRT effects and text animations.
/// Can be enabled/disabled based on terminalMode setting.
/// </summary>
[GlobalClass]
public partial class TerminalBase : BaseNarrativeScene
{
    public enum TerminalMode
    {
        Disabled,     // No terminal functionality (for minimal UI stages)
        Minimal,      // Basic text display only (for dungeon overlays)
        Full          // Full terminal with shaders and effects (for Stage 1)
    }

    [Export] public TerminalMode terminalMode = TerminalMode.Full;

    private struct PhosphorSettings {
        public float Glow;
        public float Curvature;
        public float VignetteStrength;
        public float VignetteSoftness;
        public float Spread;
        public float Chromatic;
        public float Brightness;
        public float Contrast;
        public Vector3 Tint;
    }
    private const string UiSelectAudioPath = "res://source/assets/sfx/confirmation_002.ogg";
    private const string TransitionAudioPath = "res://source/assets/sfx/doorOpen_2.ogg";

    private static readonly char[] GlitchCharacters = "█▓▒░◊∞Ω≋※▉▐▌".ToCharArray();

    private ColorRect? _phosphorLayer;
    private ColorRect? _scanlineLayer;
    private ColorRect? _glitchLayer;

    protected RichTextLabel? _textDisplay;
    private VBoxContainer? _choiceContainer;
    private VBoxContainer? _terminalContent;

    private AnimationPlayer? _animationPlayer;
    private Label? _captionLabel;

    private ShaderMaterial? _phosphorMaterial;
    private ShaderMaterial? _scanlineMaterial;
    private ShaderMaterial? _glitchMaterial;

    private readonly StringBuilder _textBuffer = new();
    private readonly RandomNumberGenerator _rng = new();
    private Vector3 _moodTint = new(0.2f, 1.0f, 0.4f);

    // Component composition fields
    private ITerminalShaderController? _shaderController;
    private ITerminalTextRenderer? _textRenderer;
    private ITerminalChoicePresenter? _choicePresenter;

    /// <summary>Gets or sets whether closed captions are enabled.</summary>
    public bool CaptionsEnabled { get; set; }

    // Protected accessors for component customization by derived classes
    protected ITerminalShaderController? ShaderController => _shaderController;
    protected ITerminalTextRenderer? TextRenderer => _textRenderer;
    protected ITerminalChoicePresenter? ChoicePresenter => _choicePresenter;

    private void CacheNodeReferences()
    {
        if (terminalMode == TerminalMode.Disabled) return;

        _phosphorLayer = GetNode<ColorRect>("PhosphorLayer");
        _scanlineLayer = GetNode<ColorRect>("ScanlineLayer");
        _glitchLayer = GetNode<ColorRect>("GlitchLayer");
        _textDisplay = GetNode<RichTextLabel>("TextDisplay");
        _choiceContainer = GetNode<VBoxContainer>("ChoiceContainer");
        _terminalContent = GetNode<VBoxContainer>("TerminalContent");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _captionLabel = GetNode<Label>("CaptionLabel");
    }

    private void ConfigureShaderMaterials()
    {
        if (terminalMode == TerminalMode.Disabled) return;

        if (_phosphorLayer?.Material is ShaderMaterial phosphorMat)
            _phosphorMaterial = phosphorMat;
        if (_scanlineLayer?.Material is ShaderMaterial scanlineMat)
            _scanlineMaterial = scanlineMat;
        if (_glitchLayer?.Material is ShaderMaterial glitchMat)
            _glitchMaterial = glitchMat;
    }

    private void InitializeComponents()
    {
        if (terminalMode == TerminalMode.Disabled) return;

        // Initialize shader controller with phosphor layer (primary shader layer)
        if (_phosphorLayer != null)
        {
            _shaderController = new TerminalShaderController(_phosphorLayer);
        }

        // Initialize text renderer with text display
        if (_textDisplay != null)
        {
            _textRenderer = new TerminalTextRenderer(_textDisplay);
        }

        // Initialize choice presenter with choice container
        if (_choiceContainer != null)
        {
            _choicePresenter = new TerminalChoicePresenter(_choiceContainer);
        }
    }

    private void ClearText()
    {
        if (terminalMode == TerminalMode.Disabled) return;

        _textRenderer?.ClearText();
    }

    public async Task AppendTextAsync(string text, bool useGhostEffect = false, double charDelaySeconds = 0.05)
    {
        if (terminalMode == TerminalMode.Disabled || _textRenderer == null)
        {
            // If no display available, just return or log
            return;
        }

        if (useGhostEffect)
        {
            await GhostWriteInternalAsync(text, charDelaySeconds);
        }
        else
        {
            await _textRenderer.AppendTextAsync(text, (float)charDelaySeconds * 20, 0f); // Convert to characters per second
        }
    }

    public async Task<string> PresentChoicesAsync(string prompt, string[] optionTexts, bool ghostPrompt = false)
    {
        if (terminalMode == TerminalMode.Disabled || _textRenderer == null || _choicePresenter == null)
        {
            return optionTexts.Length > 0 ? optionTexts[0] : string.Empty;
        }

        await AppendTextAsync(prompt, ghostPrompt);

        var choices = optionTexts.Select(text => new ChoiceOption { Text = text }).ToList();
        int selectedIndex = await _choicePresenter.PresentChoicesAsync(choices);

        return selectedIndex >= 0 && selectedIndex < optionTexts.Length ? optionTexts[selectedIndex] : string.Empty;
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        if (terminalMode != TerminalMode.Disabled)
        {
            CacheNodeReferences();
            ConfigureShaderMaterials();
            InitializeComponents();
        }

        _rng.Randomize();
        _textBuffer.Clear();

        if (_textDisplay != null)
        {
            _textDisplay.Text = string.Empty;
            _textDisplay.Modulate = Colors.White;
        }
        if (_choiceContainer != null)
            _choiceContainer.Visible = false;
        if (_captionLabel != null)
            _captionLabel.Visible = CaptionsEnabled;

        // Safe retrieval of GameState - may not exist in test environments
        var gameState = GetNodeOrNull<GameState>("/root/GameState");
        if (gameState != null)
        {
            ApplyPressStartMood(gameState.PressStartMood);
        }
    }

    private async Task GhostWriteInternalAsync(string text, double charDelaySeconds)
    {
        if (terminalMode == TerminalMode.Disabled || _textDisplay == null) return;

        foreach (char glyph in text)
        {
            _textBuffer.Append(glyph);
            string canonical = _textBuffer.ToString();

            bool shouldGlitch = !char.IsWhiteSpace(glyph) && _rng.Randf() < 0.22f;
            if (shouldGlitch)
            {
                char glitchGlyph = GlitchCharacters[_rng.RandiRange(0, GlitchCharacters.Length - 1)];
                _textBuffer[^1] = glitchGlyph;
            }
            _textDisplay.Text = canonical;
            await ToSignal(GetTree().CreateTimer(charDelaySeconds), SceneTreeTimer.SignalName.Timeout);
        }
    }

    /// <summary>
    /// Performs a pixel dissolve effect on the displayed text.
    /// </summary>
    public async Task PixelDissolveAsync(double durationSeconds = 2.5)
    {
        if (terminalMode == TerminalMode.Disabled || _shaderController == null)
        {
            return;
        }

        await _shaderController.PixelDissolveAsync((float)durationSeconds);
    }

    private AudioManager? _audioManager;

    /// <summary>
    /// Plays a terminal audio cue via the centralized <see cref="AudioManager"/>.
    /// </summary>
    /// <param name="bus">The logical terminal audio bus.</param>
    /// <param name="resourcePath">The resource path to the audio stream.</param>
    /// <param name="volumeDb">Optional volume adjustment in decibels.</param>
    /// <returns><see langword="true"/> if the cue was dispatched successfully.</returns>
    protected bool TryPlayTerminalAudio(AudioBus bus, string resourcePath, float volumeDb = 0.0f)
    {
        if (string.IsNullOrEmpty(resourcePath))
        {
            GD.PushWarning("[TerminalBase] Ignoring audio request with empty resource path.");
            return false;
        }

        var manager = ResolveAudioManager();
        if (manager == null)
        {
            return false;
        }

        return manager.PlayOneShot(resourcePath, MapAudioCategory(bus), volumeDb);
    }

    /// <summary>
    /// Plays a terminal audio cue with a pre-loaded <see cref="AudioStream"/>.
    /// </summary>
    /// <param name="bus">The logical terminal audio bus.</param>
    /// <param name="stream">The audio stream to play.</param>
    /// <param name="volumeDb">Optional volume adjustment in decibels.</param>
    /// <returns><see langword="true"/> if the cue was dispatched successfully.</returns>
    protected bool TryPlayTerminalAudio(AudioBus bus, AudioStream? stream, float volumeDb = 0.0f)
    {
        if (stream == null)
        {
            GD.PushWarning($"[TerminalBase] Attempted to play audio on {bus} without a valid stream.");
            return false;
        }

        var manager = ResolveAudioManager();
        if (manager == null)
        {
            return false;
        }

        return manager.PlayOneShot(stream, MapAudioCategory(bus), volumeDb);
    }

    private AudioManager? ResolveAudioManager()
    {
        if (_audioManager != null && GodotObject.IsInstanceValid(_audioManager))
        {
            return _audioManager.IsInitialized ? _audioManager : null;
        }

        var manager = GetNodeOrNull<AudioManager>("/root/AudioManager");
        if (manager == null)
        {
            GD.PrintErr("[TerminalBase] AudioManager not found in scene tree.");
            return null;
        }

        if (!manager.IsInitialized)
        {
            GD.PushWarning("[TerminalBase] AudioManager exists but is not initialized.");
            return null;
        }

        _audioManager = manager;
        return _audioManager;
    }

    private static AudioCategory MapAudioCategory(AudioBus bus)
    {
        return bus switch
        {
            AudioBus.Ambient => AudioCategory.Ambient,
            AudioBus.Effects => AudioCategory.Sfx,
            AudioBus.UI => AudioCategory.Sfx,
            AudioBus.Music => AudioCategory.Music,
            _ => AudioCategory.Sfx,
        };
    }

    /// <summary>
    /// Applies a preconfigured visual preset to the terminal.
    /// </summary>
    public void ApplyVisualPreset(TerminalVisualPreset preset)
    {
        if (terminalMode == TerminalMode.Disabled || _shaderController == null) return;

        string presetName = preset.ToString().ToLower();
        // For backward compatibility, call async method synchronously
        // This is acceptable since the operation is actually synchronous
        _shaderController.ApplyVisualPresetAsync(presetName).Wait();
    }

    /// <summary>
    /// Directly sets a shader parameter on one of the terminal layers.
    /// </summary>
    public void SetShaderParameter(ShaderLayer layer, string parameterName, Variant value)
    {
        if (terminalMode == TerminalMode.Disabled) return;

        ShaderMaterial? targetMaterial = layer switch
        {
            ShaderLayer.Phosphor => _phosphorMaterial,
            ShaderLayer.Scanline => _scanlineMaterial,
            ShaderLayer.Glitch => _glitchMaterial,
            _ => null
        };

        targetMaterial?.SetShaderParameter(parameterName, value);
    }

    private void SetPhosphorSettings(PhosphorSettings settings)
    {
        if (terminalMode == TerminalMode.Disabled || _phosphorMaterial is null)
        {
            return;
        }
        _phosphorMaterial.SetShaderParameter("phosphor_glow", settings.Glow);
        _phosphorMaterial.SetShaderParameter("curvature_strength", settings.Curvature);
        _phosphorMaterial.SetShaderParameter("vignette_strength", settings.VignetteStrength);
        _phosphorMaterial.SetShaderParameter("vignette_softness", settings.VignetteSoftness);
        _phosphorMaterial.SetShaderParameter("phosphor_spread", settings.Spread);
        _phosphorMaterial.SetShaderParameter("chromatic_aberration", settings.Chromatic);
        _phosphorMaterial.SetShaderParameter("brightness", settings.Brightness);
        _phosphorMaterial.SetShaderParameter("contrast", settings.Contrast);
        _phosphorMaterial.SetShaderParameter("phosphor_tint", settings.Tint);
    }

    private void SetScanlineSettings(float opacity, float speed, float count, float thickness, Vector3 tint)
    {
        if (terminalMode == TerminalMode.Disabled || _scanlineMaterial is null)
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
        if (terminalMode == TerminalMode.Disabled || _glitchMaterial is null)
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

}

/// <summary>
/// Shader layer identifiers for TerminalBase.
/// </summary>
public enum ShaderLayer
{
    /// <summary>
    /// Phosphor shader layer for CRT glow effects.
    /// </summary>
    Phosphor,
    /// <summary>
    /// Scanline shader layer for CRT scanline effects.
    /// </summary>
    Scanline,
    /// <summary>
    /// Glitch shader layer for CRT glitch effects.
    /// </summary>
    Glitch,
}

/// <summary>
/// Audio bus identifiers for TerminalBase.
/// </summary>
public enum AudioBus
{
    /// <summary>
    /// Ambient audio bus for background sounds.
    /// </summary>
    Ambient,
    /// <summary>
    /// Effects audio bus for sound effects.
    /// </summary>
    /// <summary>
    /// Effects audio bus for sound effects.
    /// </summary>
    Effects,
    /// <summary>
    /// UI audio bus for user interface sounds.
    /// </summary>
    UI,
    /// <summary>
    /// Music audio bus for background music.
    /// </summary>
    Music,
}

/// <summary>
/// Visual presets applied to the terminal.
/// </summary>
public enum TerminalVisualPreset
{
    /// <summary>
    /// Boot sequence visual preset for terminal startup.
    /// </summary>
    BootSequence,
    /// <summary>
    /// Stable baseline visual preset for normal terminal operation.
    /// </summary>
    StableBaseline,
    /// <summary>
    /// Secret reveal visual preset for special events.
    /// </summary>
    SecretReveal,
    /// <summary>
    /// Thread light visual preset for light narrative thread.
    /// </summary>
    ThreadLight,
    /// <summary>
    /// Thread mischief visual preset for mischief narrative thread.
    /// </summary>
    ThreadMischief,
    /// <summary>
    /// Thread wrath visual preset for wrath narrative thread.
    /// </summary>
    ThreadWrath,
    /// <summary>
    /// Thread balance visual preset for balance narrative thread.
    /// </summary>
    ThreadBalance,
}
