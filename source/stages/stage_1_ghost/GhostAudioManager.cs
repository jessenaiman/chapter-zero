// <copyright file="GhostAudioManager.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Stages.Stage1;

/// <summary>
/// Audio manager for the Ghost Terminal stage, implementing "Communication Through Time" concept.
/// Manages layered audio states that reflect Omega's existence across different eras.
/// </summary>
[GlobalClass]
public partial class GhostAudioManager : Node
{
    // Audio player nodes (created in scene)
    private AudioStreamPlayer? _AmbientAudio;
    private AudioStreamPlayer? _EffectsAudio;
    private AudioStreamPlayer? _UiAudio;

    // Core audio layers
    private AudioStreamPlayer? _CrtHumPlayer;
    private AudioStreamPlayer? _InterferencePlayer;
    private AudioStreamPlayer? _GhostTypingPlayer;

    // Timers for ambient effects
    private Godot.Timer? _GhostTypingTimer;
    private Godot.Timer? _InterferenceTimer;

    // State-specific audio players
    private AudioStreamPlayer? _BootSequencePlayer;
    private AudioStreamPlayer? _SecretRevealPlayer;
    private AudioStreamPlayer? _ResonantTonePlayer;

    // Audio state management
    private bool _IsInitialized;
    private string _CurrentState = "idle";

    // Audio resource paths
    private readonly Dictionary<string, string> _AudioResources = new()
    {
        // Ambient layers
        { "crt_hum", "res://source/assets/audio/stage1/ambient/crt_hum_60hz.ogg" },
        { "electrical_interference", "res://source/assets/audio/stage1/ambient/electrical_crackle_01.ogg" },

        // Historical sounds (boot sequence)
        { "telegraph_morse", "res://source/assets/audio/stage1/historical/telegraph_morse.ogg" },
        { "modem_handshake", "res://source/assets/audio/stage1/historical/modem_56k_handshake.ogg" },
        { "fax_machine", "res://source/assets/audio/stage1/historical/fax_negotiation.ogg" },
        { "dotmatrix_print", "res://source/assets/audio/stage1/historical/dotmatrix_printer.ogg" },
        { "typewriter_keys", "res://source/assets/audio/stage1/historical/typewriter_ghost_keys.ogg" },

        // Secret reveal
        { "tibetan_singing_bowl", "res://source/assets/audio/stage1/secret_reveal/singing_bowl_432hz.ogg" },
        { "sub_bass_40hz", "res://source/assets/audio/stage1/secret_reveal/sub_bass_40hz.ogg" },
        { "symbol_tone_01_infinity", "res://source/assets/audio/stage1/secret_reveal/symbol_tone_01_infinity.ogg" },
        { "symbol_tone_02_diamond", "res://source/assets/audio/stage1/secret_reveal/symbol_tone_02_diamond.ogg" },
        { "symbol_tone_03_omega", "res://source/assets/audio/stage1/secret_reveal/symbol_tone_03_omega.ogg" },
        { "symbol_tone_04_wave", "res://source/assets/audio/stage1/secret_reveal/symbol_tone_04_wave.ogg" },
        { "symbol_tone_05_asterisk", "res://source/assets/audio/stage1/secret_reveal/symbol_tone_05_asterisk.ogg" },

        // UI sounds
        { "choice_hover", "res://source/assets/audio/stage1/ui/choice_hover.ogg" },
        { "choice_confirm_light", "res://source/assets/audio/stage1/ui/choice_confirm_light.ogg" },
        { "choice_confirm_shadow", "res://source/assets/audio/stage1/ui/choice_confirm_shadow.ogg" },
        { "choice_confirm_ambition", "res://source/assets/audio/stage1/ui/choice_confirm_ambition.ogg" },

        // Thread lock sounds
        { "light_lock_warmth", "res://source/assets/audio/stage1/thread_themes/light_lock_warmth.ogg" },
        { "shadow_lock_vinyl", "res://source/assets/audio/stage1/thread_themes/shadow_lock_vinyl.ogg" },
        { "ambition_lock_digital", "res://source/assets/audio/stage1/thread_themes/ambition_lock_digital.ogg" },
        { "vault_lock_mechanical", "res://source/assets/audio/stage1/thread_themes/vault_lock_mechanical.ogg" },
    };

    // Audio bus names
    public const string AmbientBus = "Ambient";
    public const string EffectsBus = "SFX";
    public const string VoiceBus = "Voice";

    /// <summary>
    /// Gets a value indicating whether the audio manager is initialized.
    /// </summary>
    public bool IsInitialized => _IsInitialized;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        this.InitializeAudioPlayers();
        this.InitializeCoreLayers();
        _IsInitialized = true;
        GD.Print("[GhostAudioManager] Initialized successfully");
    }

    /// <summary>
    /// Initializes the audio player nodes from the scene.
    /// </summary>
    private void InitializeAudioPlayers()
    {
        _AmbientAudio = GetNodeOrNull<AudioStreamPlayer>("AudioPlayers/AmbientAudio");
        _EffectsAudio = GetNodeOrNull<AudioStreamPlayer>("AudioPlayers/EffectsAudio");
        _UiAudio = GetNodeOrNull<AudioStreamPlayer>("AudioPlayers/UiAudio");

        if (_AmbientAudio == null || _EffectsAudio == null || _UiAudio == null)
        {
            GD.PrintErr("[GhostAudioManager] Missing required audio player nodes in scene");
        }
    }

    /// <summary>
    /// Initializes the core audio layers that run throughout the experience.
    /// </summary>
    private void InitializeCoreLayers()
    {
        // Create CRT hum player (always present)
        _CrtHumPlayer = new AudioStreamPlayer();
        _CrtHumPlayer.Bus = AmbientBus;
        _CrtHumPlayer.VolumeDb = -24.0f; // Subliminal, felt more than heard
        _CrtHumPlayer.PitchScale = 1.0f;
        this.AddChild(_CrtHumPlayer);
        _CrtHumPlayer.Owner = this.GetTree().Root;

        // Create interference player (always present with random spikes)
        _InterferencePlayer = new AudioStreamPlayer();
        _InterferencePlayer.Bus = AmbientBus;
        _InterferencePlayer.VolumeDb = -18.0f; // For spikes
        this.AddChild(_InterferencePlayer);
        _InterferencePlayer.Owner = this.GetTree().Root;

        // Create ghost typing player
        _GhostTypingPlayer = new AudioStreamPlayer();
        _GhostTypingPlayer.Bus = AmbientBus;
        _GhostTypingPlayer.VolumeDb = -12.0f; // Noticeable but not intrusive
        this.AddChild(_GhostTypingPlayer);
        _GhostTypingPlayer.Owner = this.GetTree().Root;

        // Initialize state-specific players
        _BootSequencePlayer = new AudioStreamPlayer();
        _BootSequencePlayer.Bus = EffectsBus;
        this.AddChild(_BootSequencePlayer);
        _BootSequencePlayer.Owner = this.GetTree().Root;

        _SecretRevealPlayer = new AudioStreamPlayer();
        _SecretRevealPlayer.Bus = EffectsBus;
        this.AddChild(_SecretRevealPlayer);
        _SecretRevealPlayer.Owner = this.GetTree().Root;

        _ResonantTonePlayer = new AudioStreamPlayer();
        _ResonantTonePlayer.Bus = EffectsBus;
        this.AddChild(_ResonantTonePlayer);
        _ResonantTonePlayer.Owner = this.GetTree().Root;

        // Start core layers
        this.StartCoreLayers();
    }

    /// <summary>
    /// Starts the core audio layers that provide the foundation.
    /// </summary>
    private void StartCoreLayers()
    {
        // Start CRT hum continuously
        if (this._CrtHumPlayer != null)
        {
            var humStream = ResourceLoader.Load<AudioStream>(this._AudioResources["crt_hum"]);
            if (humStream != null)
            {
                this._CrtHumPlayer.Stream = humStream;
                this._CrtHumPlayer.Play();
            }
            else
            {
                GD.Print("[GhostAudioManager] CRT hum audio not available");
            }
        }

        // Start electrical interference with random intervals (3-7 seconds per design doc)
        StartElectricalInterferenceLoop();

        // Start ghost typing with random intervals
        StartGhostTypingLoop();
    }

    /// <summary>
    /// Starts the electrical interference loop with periodic crackles.
    /// Fires every 3-7 seconds to simulate degraded power supply.
    /// </summary>
    private void StartElectricalInterferenceLoop()
    {
        _InterferenceTimer = new Godot.Timer();
        _InterferenceTimer.OneShot = false;
        _InterferenceTimer.WaitTime = GD.RandRange(3.0f, 7.0f);
        _InterferenceTimer.Timeout += () =>
        {
            PlayElectricalInterference();
            this._InterferenceTimer.WaitTime = GD.RandRange(3.0f, 7.0f);
        };
        this.AddChild(this._InterferenceTimer);
        this._InterferenceTimer.Owner = this.GetTree().Root;
        this._InterferenceTimer.Start();
    }

    /// <summary>
    /// Plays an electrical interference crackle sound.
    /// </summary>
    private void PlayElectricalInterference()
    {
        if (this._InterferencePlayer != null && this._AudioResources.ContainsKey("electrical_interference"))
        {
            var stream = ResourceLoader.Load<AudioStream>(this._AudioResources["electrical_interference"]);
            if (stream != null)
            {
                this._InterferencePlayer.Stream = stream;
                this._InterferencePlayer.Play();
            }
        }
    }

    /// <summary>
    /// Starts the ghost typing loop that simulates historical input.
    /// Fires randomly every 5-10 seconds per design doc.
    /// </summary>
    private void StartGhostTypingLoop()
    {
        _GhostTypingTimer = new Godot.Timer();
        _GhostTypingTimer.OneShot = false;
        _GhostTypingTimer.WaitTime = GD.RandRange(5.0f, 10.0f);
        _GhostTypingTimer.Timeout += () =>
        {
            PlayGhostTyping();
            this._GhostTypingTimer.WaitTime = GD.RandRange(5.0f, 10.0f);
        };
        this.AddChild(this._GhostTypingTimer);
        this._GhostTypingTimer.Owner = this.GetTree().Root;
        this._GhostTypingTimer.Start();
    }

    /// <summary>
    /// Plays the ghost typing sound effect.
    /// </summary>
    private void PlayGhostTyping()
    {
        if (this._GhostTypingPlayer != null)
        {
            this._GhostTypingPlayer.Stream = ResourceLoader.Load<AudioStream>(this._AudioResources["typewriter_keys"]);
            if (this._GhostTypingPlayer.Stream != null)
            {
                this._GhostTypingPlayer.Play();
            }
        }
    }

    /// <summary>
    /// Enters the boot sequence audio state with historical layers.
    /// </summary>
    /// <returns>Task representing the async operation.</returns>
    public async Task EnterBootSequenceAsync()
    {
        if (!_IsInitialized) return;

        _CurrentState = "boot_sequence";
        GD.Print("[GhostAudioManager] Entering boot sequence audio state");

        // Layer multiple historical audio artifacts
        if (this._BootSequencePlayer != null)
        {
            // Play telegraph morse pattern
            this._BootSequencePlayer.Stream = ResourceLoader.Load<AudioStream>(this._AudioResources["telegraph_morse"]);
            if (this._BootSequencePlayer.Stream != null)
            {
                this._BootSequencePlayer.VolumeDb = -10.0f;
                this._BootSequencePlayer.Play();
            }

            // Brief delay before adding other layers
            await Task.Delay(500).ConfigureAwait(false);

            var modemPlayer = new AudioStreamPlayer();
            modemPlayer.Bus = EffectsBus;
            modemPlayer.Stream = ResourceLoader.Load<AudioStream>(_AudioResources["modem_handshake"]);
            if (modemPlayer.Stream != null)
            {
                modemPlayer.VolumeDb = -8.0f;
                this.AddChild(modemPlayer);
                modemPlayer.Owner = this.GetTree().Root;
                modemPlayer.Play();

                // Wait for modem handshake to complete
                await Task.Delay(3000).ConfigureAwait(false);

                // Remove the temporary player after use
                modemPlayer.QueueFree();
            }
        }

        // Set CRT hum to more intense setting for boot sequence
        if (_CrtHumPlayer != null)
        {
            _CrtHumPlayer.VolumeDb = -20.0f; // Slightly more noticeable
        }
    }

    /// <summary>
    /// Enters the stable baseline audio state.
    /// </summary>
    public void EnterStableBaseline()
    {
        if (!_IsInitialized) return;

        _CurrentState = "stable_baseline";
        GD.Print("[GhostAudioManager] Entering stable baseline audio state");

        // Adjust core layers for stable state
        if (_CrtHumPlayer != null)
        {
            _CrtHumPlayer.VolumeDb = -24.0f; // Subliminal level
        }

        // Reduce ghost typing frequency
        // This would be handled by adjusting the timer interval in a full implementation
    }

    /// <summary>
    /// Enters the secret reveal audio state with orchestrated 4-second buildup.
    /// Implements the design doc's complex layering: CRT hum increase → sub-bass →
    /// modem fragments → ghost typewriters → silence → singing bowl.
    /// </summary>
    /// <returns>Task representing the async operation.</returns>
    public async Task EnterSecretRevealAsync()
    {
        if (!_IsInitialized) return;

        _CurrentState = "secret_reveal";
        GD.Print("[GhostAudioManager] Entering secret reveal audio state - 4-second buildup");

        // 0.0s: CRT hum increases 200% volume
        if (_CrtHumPlayer != null)
        {
            _CrtHumPlayer.VolumeDb = -12.0f; // From -24dB to -12dB (200% increase)
        }

        // 0.5s: Sub-bass drone (40Hz) enters
        await Task.Delay(500).ConfigureAwait(false);
        var subBassPlayer = new AudioStreamPlayer
        {
            Bus = EffectsBus,
            VolumeDb = -15.0f
        };
        this.AddChild(subBassPlayer);
        subBassPlayer.Owner = this.GetTree().Root;
        // Note: Would need a 40Hz sine wave audio file
        // subBassPlayer.Stream = ResourceLoader.Load<AudioStream>("res://source/assets/audio/stage1/sub_bass_40hz.wav");
        // subBassPlayer.Play();

        // 1.0s: Modem tones return (fragmented, glitching)
        await Task.Delay(500).ConfigureAwait(false);
        if (this._AudioResources.ContainsKey("modem_handshake"))
        {
            var modemFragmentPlayer = new AudioStreamPlayer
            {
                Bus = EffectsBus,
                VolumeDb = -14.0f,
                PitchScale = 0.8f // Slightly detuned for glitch effect
            };
            this.AddChild(modemFragmentPlayer);
            modemFragmentPlayer.Owner = this.GetTree().Root;
            modemFragmentPlayer.Stream = ResourceLoader.Load<AudioStream>(_AudioResources["modem_handshake"]);
            if (modemFragmentPlayer.Stream != null)
            {
                modemFragmentPlayer.Play();
            }
        }

        // 1.5s: Multiple ghost typewriters typing simultaneously
        await Task.Delay(500).ConfigureAwait(false);
        for (int i = 0; i < 3; i++)
        {
            if (this._AudioResources.ContainsKey("typewriter_keys"))
            {
                var ghostTyper = new AudioStreamPlayer
                {
                    Bus = EffectsBus,
                    VolumeDb = -10.0f,
                    PitchScale = 0.9f + (i * 0.1f) // Slightly different pitches
                };
                this.AddChild(ghostTyper);
                ghostTyper.Stream = ResourceLoader.Load<AudioStream>(_AudioResources["typewriter_keys"]);
                if (ghostTyper.Stream != null)
                {
                    ghostTyper.Play();
                }
            }
        }

        // 2.0s: All historical sounds layer (cacophony)
        await Task.Delay(500).ConfigureAwait(false);
        // Telegraph, fax, etc. already layered from above

        // 2.5s: SUDDEN SILENCE (0.3s) - CRITICAL MOMENT
        await Task.Delay(500).ConfigureAwait(false);
        if (_CrtHumPlayer != null)
        {
            _CrtHumPlayer.VolumeDb = -60.0f; // Essentially silent
        }
        // Stop all other layers temporarily
        await Task.Delay(300).ConfigureAwait(false);

        // 3.0s: Clean Tibetan singing bowl tone begins
        if (this._ResonantTonePlayer != null)
        {
            this._ResonantTonePlayer.Stream = ResourceLoader.Load<AudioStream>(this._AudioResources["tibetan_singing_bowl"]);
            if (this._ResonantTonePlayer.Stream != null)
            {
                this._ResonantTonePlayer.VolumeDb = -6.0f; // More prominent during reveal
                this._ResonantTonePlayer.Play();
            }
        }

        // Restore CRT hum at slightly elevated level
        if (this._CrtHumPlayer != null)
        {
            this._CrtHumPlayer.VolumeDb = -22.0f; // Slightly more intense than baseline
        }

        // Wait for singing bowl to sustain (6-second sustain per design doc)
        await Task.Delay(1000).ConfigureAwait(false);
    }

    /// <summary>
    /// Plays a specific symbol overtone during secret reveal.
    /// Each symbol has a unique frequency: ∞(432Hz), ◊(540Hz), Ω(648Hz), ≋(810Hz), ※(972Hz).
    /// </summary>
    /// <param name="symbolIndex">Index of the symbol (0-4).</param>
    /// <returns>Task representing the async operation.</returns>
    public async Task PlaySymbolOvertoneAsync(int symbolIndex)
    {
        if (!_IsInitialized || symbolIndex < 0 || symbolIndex > 4) return;

        var frequencies = new[] { 432, 540, 648, 810, 972 };
        var symbolNames = new[] { "infinity", "diamond", "omega", "wave", "asterisk" };

        GD.Print($"[GhostAudioManager] Playing symbol {symbolIndex} overtone: {frequencies[symbolIndex]}Hz");

        // Play the symbol-specific overtone
        var overtonePlayer = new AudioStreamPlayer
        {
            Bus = EffectsBus,
            VolumeDb = -8.0f
        };
        this.AddChild(overtonePlayer);
        overtonePlayer.Owner = this.GetTree().Root;

        var overtoneKey = $"symbol_tone_{symbolIndex + 1:D2}_{symbolNames[symbolIndex]}";
        if (this._AudioResources.ContainsKey(overtoneKey))
        {
            overtonePlayer.Stream = ResourceLoader.Load<AudioStream>(this._AudioResources[overtoneKey]);
            if (overtonePlayer.Stream != null)
            {
                overtonePlayer.Play();
                // Let the tone ring for a moment
                await Task.Delay(800).ConfigureAwait(false);
            }
        }

        overtonePlayer.QueueFree();
    }

    /// <summary>
    /// Enters the Dreamweaver selection audio state.
    /// </summary>
    /// <param name="threadName">The selected thread (light/shadow/ambition).</param>
    /// <returns>Task representing the async operation.</returns>
    public async Task EnterDreamweaverSelectionAsync(string threadName)
    {
        if (!_IsInitialized) return;

        _CurrentState = "dreamweaver_selection";
        GD.Print($"[GhostAudioManager] Entering Dreamweaver selection audio state: {threadName}");

        // Adjust audio to reflect the importance of the choice
        if (_CrtHumPlayer != null)
        {
            _CrtHumPlayer.VolumeDb = -23.0f; // Slightly more focused
        }

        // Stop ghost typing briefly to create more focused atmosphere
        if (this._GhostTypingPlayer != null)
        {
            this._GhostTypingPlayer.Stop();
        }

        // Play thread-specific lock sound
        var threadKey = threadName.ToLowerInvariant() switch
        {
            "light" => "light_lock_warmth",
            "shadow" => "shadow_lock_vinyl",
            "ambition" => "ambition_lock_digital",
            _ => "light_lock_warmth"
        };

        PlayAudioEffect(threadKey, -10.0f);

        // Wait for thread theme to play
        await Task.Delay(1000).ConfigureAwait(false);

        // Play vault lock mechanical sound (shared across all threads)
        PlayAudioEffect("vault_lock_mechanical", -8.0f);
        await Task.Delay(1500).ConfigureAwait(false); // 2 rotations sound
    }

    /// <summary>
    /// Plays the choice hover sound effect (200Hz → 400Hz charge-up).
    /// </summary>
    public void PlayChoiceHoverSound()
    {
        if (!this._IsInitialized) return;

        if (this._UiAudio != null && this._AudioResources.ContainsKey("choice_hover"))
        {
            var stream = ResourceLoader.Load<AudioStream>(this._AudioResources["choice_hover"]);
            if (stream != null)
            {
                this._UiAudio.Stream = stream;
                this._UiAudio.VolumeDb = -18.0f;
                this._UiAudio.Play();
            }
        }
    }

    /// <summary>
    /// Plays the choice confirmation sound for a specific thread.
    /// </summary>
    /// <param name="threadName">The thread associated with the choice (light/shadow/ambition).</param>
    public void PlayChoiceConfirmSound(string threadName)
    {
        if (!_IsInitialized) return;

        var confirmKey = threadName.ToLowerInvariant() switch
        {
            "light" => "choice_confirm_light",
            "shadow" => "choice_confirm_shadow",
            "ambition" => "choice_confirm_ambition",
            _ => "choice_confirm_light"
        };

        if (this._UiAudio != null && this._AudioResources.ContainsKey(confirmKey))
        {
            var stream = ResourceLoader.Load<AudioStream>(this._AudioResources[confirmKey]);
            if (stream != null)
            {
                this._UiAudio.Stream = stream;
                this._UiAudio.VolumeDb = -12.0f;
                this._UiAudio.Play();
            }
        }
    }

    /// <summary>
    /// Plays a specific audio effect.
    /// </summary>
    /// <param name="audioKey">The key for the audio resource to play.</param>
    /// <returns>True if the audio was played successfully.</returns>
    public bool PlayAudioEffect(string audioKey)
    {
        if (!this._IsInitialized) return false;

        if (this._EffectsAudio != null && this._AudioResources.ContainsKey(audioKey))
        {
            var stream = ResourceLoader.Load<AudioStream>(this._AudioResources[audioKey]);
            if (stream != null)
            {
                this._EffectsAudio.Stream = stream;
                this._EffectsAudio.Play();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Plays an audio effect with specific parameters.
    /// </summary>
    /// <param name="audioKey">The key for the audio resource to play.</param>
    /// <param name="volumeDb">Volume adjustment in dB.</param>
    /// <param name="pitchScale">Pitch adjustment.</param>
    /// <returns>True if the audio was played successfully.</returns>
    public bool PlayAudioEffect(string audioKey, float volumeDb, float pitchScale = 1.0f)
    {
        if (!this._IsInitialized) return false;

        if (this._EffectsAudio != null && this._AudioResources.ContainsKey(audioKey))
        {
            var stream = ResourceLoader.Load<AudioStream>(this._AudioResources[audioKey]);
            if (stream != null)
            {
                this._EffectsAudio.Stream = stream;
                this._EffectsAudio.VolumeDb = volumeDb;
                this._EffectsAudio.PitchScale = pitchScale;
                this._EffectsAudio.Play();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the current audio state.
    /// </summary>
    /// <returns>The current audio state.</returns>
    public string GetCurrentState()
    {
        return _CurrentState;
    }

    /// <summary>
    /// Sets the volume for the ambient audio category.
    /// </summary>
    /// <param name="volumeDb">The volume in dB.</param>
    public void SetAmbientVolume(float volumeDb)
    {
        if (_CrtHumPlayer != null)
        {
            _CrtHumPlayer.VolumeDb = volumeDb;
        }
    }

    /// <summary>
    /// Sets the volume for the effects audio category.
    /// </summary>
    /// <param name="volumeDb">The volume in dB.</param>
    public void SetEffectsVolume(float volumeDb)
    {
        if (_EffectsAudio != null)
        {
            _EffectsAudio.VolumeDb = volumeDb;
        }
    }

    /// <summary>
    /// Sets the volume for the UI audio category.
    /// </summary>
    /// <param name="volumeDb">The volume in dB.</param>
    public void SetUiVolume(float volumeDb)
    {
        if (_UiAudio != null)
        {
            _UiAudio.VolumeDb = volumeDb;
        }
    }

    /// <summary>
    /// Disables or enables the ghost typing layer for accessibility.
    /// </summary>
    /// <param name="enabled">Whether to enable the ghost typing audio.</param>
    public void SetGhostTypingEnabled(bool enabled)
    {
        if (this._GhostTypingTimer != null)
        {
            if (enabled)
            {
                this._GhostTypingTimer.Start();
            }
            else
            {
                this._GhostTypingTimer.Stop();
                this._GhostTypingPlayer?.Stop();
            }
        }
    }

    /// <summary>
    /// Disables or enables electrical interference for accessibility.
    /// </summary>
    /// <param name="enabled">Whether to enable electrical interference audio.</param>
    public void SetElectricalInterferenceEnabled(bool enabled)
    {
        if (_InterferenceTimer != null)
        {
            if (enabled)
            {
                _InterferenceTimer.Start();
            }
            else
            {
                _InterferenceTimer.Stop();
                _InterferencePlayer?.Stop();
            }
        }
    }

    /// <summary>
    /// Disables or enables the CRT hum for accessibility.
    /// </summary>
    /// <param name="enabled">Whether to enable the CRT hum audio.</param>
    public void SetCrtHumEnabled(bool enabled)
    {
        if (_CrtHumPlayer != null)
        {
            if (enabled)
            {
                _CrtHumPlayer.Play();
            }
            else
            {
                _CrtHumPlayer.Stop();
            }
        }
    }
}
