using Godot;
using OmegaSpiral.Source.Audio;
using OmegaSpiral.Source.Backend.Common;

/// <summary>
/// Centralized audio management system for the Omega Spiral game.
/// Handles all audio playback, player pooling, bus management, and asset caching.
/// Serves as the foundation for category-specific audio managers like NarrativeAudioManager.
/// </summary>
[GlobalClass]
public partial class AudioManager : Node, IOmegaAudioDirector
{
    // Signals for observability and testing
    [Signal] public delegate void CueStartedEventHandler(string cueId, int category);
    [Signal] public delegate void CueFinishedEventHandler(string cueId, int category);
    [Signal] public delegate void DuckAppliedEventHandler(int source, int target, float amountDb, int durationMs);
    [Signal] public delegate void PoolExhaustedEventHandler();

    /// <summary>
    /// Maximum number of concurrent audio players to prevent resource exhaustion.
    /// </summary>
    private const int MaxAudioPlayers = 32;

    /// <summary>
    /// Audio bus names used throughout the game.
    /// </summary>
    public const string MasterBus = "Master";
    public const string MusicBus = "Music";
    public const string SfxBus = "SFX";
    public const string VoiceBus = "Voice";
    public const string AmbientBus = "Ambient";

    /// <summary>
    /// Available audio players for one-shot sounds.
    /// </summary>
    private readonly List<AudioStreamPlayer> availablePlayers = new();

    /// <summary>
    /// Currently playing audio players.
    /// </summary>
    private readonly List<AudioStreamPlayer> activePlayers = new();

    /// <summary>
    /// Cached audio streams to avoid repeated loading.
    /// </summary>
    private readonly System.Collections.Generic.Dictionary<string, AudioStream> audioCache = new();

    /// <summary>
    /// Minimal cue catalog mapping cue ids to resource paths and categories.
    /// </summary>
    private readonly System.Collections.Generic.Dictionary<string, (string Path, AudioCategory Category, float Gain)> cueCatalog = new()
    {
        { "ui_hover", ("res://assets/audio/ui/hover.ogg", AudioCategory.Sfx, 0f) },
        { "ui_confirm", ("res://assets/audio/ui/confirm.ogg", AudioCategory.Sfx, 0f) },
        { "reveal_bowl", ("res://assets/audio/secret/singing_bowl_432hz.ogg", AudioCategory.Music, -6f) }
    };

    /// <summary>
    /// Volume levels for different audio categories (in dB).
    /// </summary>
    private readonly System.Collections.Generic.Dictionary<AudioCategory, float> categoryVolumes = new()
    {
        { AudioCategory.Master, 0.0f },
        { AudioCategory.Music, 0.0f },
        { AudioCategory.Sfx, 0.0f },
        { AudioCategory.Voice, 0.0f },
        { AudioCategory.Ambient, -5.0f },
    };

    /// <summary>
    /// Gets a value indicating whether the audio manager is initialized.
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Initializes the audio manager and creates the player pool.
    /// </summary>
    public override void _Ready()
    {
        this.InitializePlayerPool();
        this.EnsureAudioBuses();
        this.IsInitialized = true;
        GD.Print("AudioManager: Initialized successfully");
    }

    /// <summary>
    /// Updates active players and returns finished ones to the pool.
    /// </summary>
    public override void _Process(double delta)
    {
        // Return finished players to the pool
        for (int i = this.activePlayers.Count - 1; i >= 0; i--)
        {
            var player = this.activePlayers[i];
            if (!player.Playing)
            {
                this.ReturnPlayerToPool(player);
            }
        }
    }

    /// <summary>
    /// Initializes the audio player pool.
    /// </summary>
    private void InitializePlayerPool()
    {
        for (int i = 0; i < MaxAudioPlayers; i++)
        {
            var player = new AudioStreamPlayer();
            player.Bus = SfxBus; // Default to SFX bus
            this.availablePlayers.Add(player);
            this.AddChild(player);
        }
    }

    /// <summary>
    /// Ensures all required audio buses exist in the project.
    /// </summary>
    private void EnsureAudioBuses()
    {
        var buses = new[] { MasterBus, MusicBus, SfxBus, VoiceBus, AmbientBus };
        foreach (var bus in buses)
        {
            if (AudioServer.GetBusIndex(bus) == -1)
            {
                AudioServer.AddBus();
                AudioServer.SetBusName(AudioServer.BusCount - 1, bus);
                GD.Print($"AudioManager: Created audio bus '{bus}'");
            }
        }
    }

    /// <summary>
    /// Gets an available audio player from the pool.
    /// </summary>
    /// <returns>An available AudioStreamPlayer, or null if none available.</returns>
    private AudioStreamPlayer? GetAvailablePlayer()
    {
        if (this.availablePlayers.Count > 0)
        {
            var player = this.availablePlayers[0];
            this.availablePlayers.RemoveAt(0);
            this.activePlayers.Add(player);
            return player;
        }

        GD.PrintErr("AudioManager: No available audio players in pool");
        EmitSignal(SignalName.PoolExhausted);
        return null;
    }

    /// <summary>
    /// Returns a player to the pool for reuse.
    /// </summary>
    /// <param name="player">The player to return.</param>
    private void ReturnPlayerToPool(AudioStreamPlayer player)
    {
        if (this.activePlayers.Contains(player))
        {
            this.activePlayers.Remove(player);
            this.availablePlayers.Add(player);
        }
    }

    /// <summary>
    /// Loads and caches an audio stream.
    /// </summary>
    /// <param name="resourcePath">The resource path to load.</param>
    /// <returns>The loaded audio stream, or null if loading failed.</returns>
    private AudioStream? LoadAudioStream(string resourcePath)
    {
        if (this.audioCache.TryGetValue(resourcePath, out var cachedStream))
        {
            return cachedStream;
        }

        try
        {
            var stream = ResourceLoader.Load<AudioStream>(resourcePath);
            if (stream != null)
            {
                this.audioCache[resourcePath] = stream;
                return stream;
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"AudioManager: Failed to load audio stream '{resourcePath}': {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Plays a one-shot audio clip.
    /// </summary>
    /// <param name="resourcePath">Path to the audio resource.</param>
    /// <param name="category">The audio category for bus and volume settings.</param>
    /// <param name="volumeDb">Additional volume adjustment in dB.</param>
    /// <returns>True if the audio was played successfully.</returns>
    public bool PlayOneShot(string resourcePath, AudioCategory category = AudioCategory.Sfx, float volumeDb = 0.0f)
    {
        if (!this.IsInitialized)
        {
            return false;
        }

        var stream = this.LoadAudioStream(resourcePath);
        if (stream == null)
        {
            return false;
        }

        var player = this.GetAvailablePlayer();
        if (player == null)
        {
            return false;
        }

        player.Stream = stream;
        player.Bus = this.GetBusName(category);
        player.VolumeDb = this.categoryVolumes[category] + volumeDb;
        player.Finished += () => EmitSignal(SignalName.CueFinished, resourcePath, (int)category);
        player.Play();
        EmitSignal(SignalName.CueStarted, resourcePath, (int)category);

        return true;
    }

    /// <summary>
    /// Plays a one-shot audio clip with a pre-loaded stream.
    /// </summary>
    /// <param name="stream">The audio stream to play.</param>
    /// <param name="category">The audio category for bus and volume settings.</param>
    /// <param name="volumeDb">Additional volume adjustment in dB.</param>
    /// <returns>True if the audio was played successfully.</returns>
    public bool PlayOneShot(AudioStream stream, AudioCategory category = AudioCategory.Sfx, float volumeDb = 0.0f)
    {
        if (!this.IsInitialized || stream == null)
        {
            return false;
        }

        var player = this.GetAvailablePlayer();
        if (player == null)
        {
            return false;
        }

        player.Stream = stream;
        player.Bus = this.GetBusName(category);
        player.VolumeDb = this.categoryVolumes[category] + volumeDb;
        player.Finished += () => EmitSignal(SignalName.CueFinished, stream.ResourcePath, (int)category);
        player.Play();
        EmitSignal(SignalName.CueStarted, stream.ResourcePath, (int)category);

        return true;
    }

    /// <summary>
    /// Plays a cataloged cue by id.
    /// </summary>
    /// <param name="cueId">The cue identifier.</param>
    /// <returns>True if the cue was scheduled.</returns>
    public bool PlayCue(string cueId)
    {
        if (!cueCatalog.TryGetValue(cueId, out var entry))
        {
            GD.PrintErr($"AudioManager: Unknown cue id '{cueId}'");
            return false;
        }
        return PlayOneShot(entry.Path, entry.Category, entry.Gain);
    }

    /// <summary>
    /// Ducks a target category by amount dB for durationMs then restores.
    /// </summary>
    public async void DuckCategoryFor(AudioCategory target, float amountDb, int durationMs)
    {
        var original = GetCategoryVolume(target);
        SetCategoryVolume(target, original + amountDb);
        EmitSignal(SignalName.DuckApplied, (int)AudioCategory.Sfx, (int)target, amountDb, durationMs);
        var tree = GetTree();
        if (tree != null)
        {
            await ToSignal(tree.CreateTimer(durationMs / 1000.0), SceneTreeTimer.SignalName.Timeout);
        }
        SetCategoryVolume(target, original);
    }

    /// <summary>
    /// Sets the volume for an audio category.
    /// </summary>
    /// <param name="category">The audio category.</param>
    /// <param name="volumeDb">The volume in dB.</param>
    public void SetCategoryVolume(AudioCategory category, float volumeDb)
    {
        this.categoryVolumes[category] = volumeDb;
        var busIndex = AudioServer.GetBusIndex(this.GetBusName(category));
        if (busIndex != -1)
        {
            AudioServer.SetBusVolumeDb(busIndex, volumeDb);
        }
    }

    /// <summary>
    /// Gets the volume for an audio category.
    /// </summary>
    /// <param name="category">The audio category.</param>
    /// <returns>The volume in dB.</returns>
    public float GetCategoryVolume(AudioCategory category)
    {
        return this.categoryVolumes[category];
    }

    /// <summary>
    /// Gets the audio bus name for a category.
    /// </summary>
    /// <param name="category">The audio category.</param>
    /// <returns>The bus name.</returns>
    private string GetBusName(AudioCategory category)
    {
        return category switch
        {
            AudioCategory.Master => MasterBus,
            AudioCategory.Music => MusicBus,
            AudioCategory.Sfx => SfxBus,
            AudioCategory.Voice => VoiceBus,
            AudioCategory.Ambient => AmbientBus,
            _ => SfxBus,
        };
    }

    /// <summary>
    /// Preloads audio assets to reduce loading times during gameplay.
    /// </summary>
    /// <param name="resourcePaths">Array of resource paths to preload.</param>
    public void PreloadAudioAssets(string[] resourcePaths)
    {
        foreach (var path in resourcePaths)
        {
            this.LoadAudioStream(path);
        }
    }

    /// <summary>
    /// Clears the audio cache to free memory.
    /// </summary>
    public void ClearCache()
    {
        this.audioCache.Clear();
    }

    /// <summary>
    /// Gets the number of available players in the pool.
    /// </summary>
    /// <returns>The number of available players.</returns>
    public int GetAvailablePlayerCount()
    {
        return this.availablePlayers.Count;
    }

    /// <summary>
    /// Gets the number of active players.
    /// </summary>
    /// <returns>The number of active players.</returns>
    public int GetActivePlayerCount()
    {
        return this.activePlayers.Count;
    }

    /// <inheritdoc/>
    public void OnBootSequenceStarted()
    {
        this.SetCategoryVolume(AudioCategory.Ambient, -20f);
        this.SetCategoryVolume(AudioCategory.Music, -24f);
    }

    /// <inheritdoc/>
    public void OnStageStabilized()
    {
        this.SetCategoryVolume(AudioCategory.Ambient, -8f);
        this.SetCategoryVolume(AudioCategory.Music, -12f);
    }

    /// <inheritdoc/>
    public void OnSecretRevealStarted(OmegaAudioAccessibilityProfile profile)
    {
        if (profile.EssentialAudioOnly)
        {
            this.SetCategoryVolume(AudioCategory.Ambient, -60f);
            this.SetCategoryVolume(AudioCategory.Music, -80f);
            return;
        }

        var ambientVolume = profile.ReduceAudioIntensity ? -18f : -6f;
        var musicVolume = profile.ReduceAudioIntensity ? -24f : -12f;

        this.SetCategoryVolume(AudioCategory.Ambient, ambientVolume);
        this.SetCategoryVolume(AudioCategory.Music, musicVolume);
    }

    /// <inheritdoc/>
    public void OnSecretRevealCompleted()
    {
        this.SetCategoryVolume(AudioCategory.Ambient, -8f);
        this.SetCategoryVolume(AudioCategory.Music, -12f);
    }

    /// <inheritdoc/>
    public void OnMenuOpened()
    {
        this.SetCategoryVolume(AudioCategory.Ambient, -12f);
        this.SetCategoryVolume(AudioCategory.Music, -18f);
    }

    /// <inheritdoc/>
    public void OnMenuClosed()
    {
        this.SetCategoryVolume(AudioCategory.Ambient, -8f);
        this.SetCategoryVolume(AudioCategory.Music, -12f);
    }

    /// <inheritdoc/>
    public void OnUiHover(DreamweaverType? thread)
    {
        _ = PlayCue("ui_hover");
    }

    /// <inheritdoc/>
    public void OnUiConfirm(DreamweaverType? thread)
    {
        if (PlayCue("ui_confirm"))
        {
            // Briefly duck ambient for clarity
            DuckCategoryFor(AudioCategory.Ambient, -6f, 250);
        }
    }
}

/// <summary>
/// Audio categories for different types of game audio.
/// </summary>
public enum AudioCategory
{
    /// <summary>
    /// Master volume control.
    /// </summary>
    Master,

    /// <summary>
    /// Background music.
    /// </summary>
    Music,

    /// <summary>
    /// Sound effects.
    /// </summary>
    Sfx,

    /// <summary>
    /// Voice audio (dialogue, narration).
    /// </summary>
    Voice,

    /// <summary>
    /// Ambient environmental sounds.
    /// </summary>
    Ambient,
}
