// <copyright file="NarrativeSystem.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Common.ScreenTransitions;
using OmegaSpiral.Source.Scripts.Field.Narrative.Audio;
using OmegaSpiral.Source.Scripts.Field.Narrative.AssetManagement;
using OmegaSpiral.Source.Scripts.Field.Narrative.IntroMechanics;
using OmegaSpiral.Source.Scripts.Field.Narrative.Sequences;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.Field.Narrative.System;
/// <summary>
/// Unified narrative system that consolidates all narrative components into a single, cohesive architecture.
/// Implements DRY principles by centralizing common functionality and ensuring SOLID compliance through proper separation of concerns.
/// </summary>
[GlobalClass]
public partial class NarrativeSystem : Node
{
    /// <summary>
    /// Audio manager for centralized audio control.
    /// </summary>
    private NarrativeAudioManager? audioManager;

    /// <summary>
    /// Asset mapper for centralized asset management.
    /// </summary>
    private NarrativeAssetMapper? assetMapper;

    /// <summary>
    /// Asset preloader for optimized performance.
    /// </summary>
    private NarrativeAssetPreloader? assetPreloader;

    /// <summary>
    /// Intro mechanics for engaging opening sequences.
    /// </summary>
    private OmegaSpiralIntroMechanics? introMechanics;

    /// <summary>
    /// Screen transition manager for visual effects.
    /// </summary>
    private ScreenTransition? screenTransition;

    /// <summary>
    /// Currently active narrative sequence.
    /// </summary>
    private NarrativeSequence? activeSequence;

    /// <summary>
    /// Registry of available narrative sequences.
    /// </summary>
    private readonly Dictionary<string, Type> sequenceRegistry = new();

    /// <summary>
    /// Gets a value indicating whether the narrative system is initialized.
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets a value indicating whether a narrative sequence is currently playing.
    /// </summary>
    public bool IsPlaying => this.activeSequence != null;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.InitializeNarrativeSystem();
    }

    /// <summary>
    /// Initializes the unified narrative system with all required components.
    /// </summary>
    private void InitializeNarrativeSystem()
    {
        GD.Print("NarrativeSystem: Initializing unified narrative system");

        // Initialize audio manager
        this.audioManager = new NarrativeAudioManager();
        this.AddChild(this.audioManager);

        // Initialize asset mapper
        this.assetMapper = new NarrativeAssetMapper();
        this.AddChild(this.assetMapper);

        // Initialize asset preloader
        this.assetPreloader = new NarrativeAssetPreloader();
        this.AddChild(this.assetPreloader);

        // Initialize intro mechanics
        this.introMechanics = new OmegaSpiralIntroMechanics();
        this.AddChild(this.introMechanics);

        // Find screen transition manager
        this.screenTransition = this.GetNodeOrNull<ScreenTransition>("/root/ScreenTransition");

        // Register available sequences
        this.RegisterNarrativeSequences();

        this.IsInitialized = true;
        GD.Print("NarrativeSystem: Initialization complete");
    }

    /// <summary>
    /// Registers available narrative sequences with the system.
    /// </summary>
    private void RegisterNarrativeSequences()
    {
        // Register core narrative sequences
        this.sequenceRegistry["opening"] = typeof(OpeningSequence);
        this.sequenceRegistry["enhanced_opening"] = typeof(EnhancedOpeningSequence);
        this.sequenceRegistry["thread_hero"] = typeof(ThreadBranchHero);
        this.sequenceRegistry["thread_shadow"] = typeof(ThreadBranchShadow);
        this.sequenceRegistry["thread_ambition"] = typeof(ThreadBranchAmbition);
        this.sequenceRegistry["name_input"] = typeof(NameInputSequence);
        this.sequenceRegistry["secret_question"] = typeof(SecretQuestionSequence);
    }

    /// <summary>
    /// Starts the narrative system with an optional intro sequence.
    /// </summary>
    /// <param name="playIntro">Whether to play the intro sequence before starting.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartAsync(bool playIntro = true)
    {
        if (!this.IsInitialized)
        {
            GD.PrintErr("NarrativeSystem: Cannot start uninitialized system");
            return;
        }

        GD.Print("NarrativeSystem: Starting narrative system");

        // Preload essential assets
        await this.PreloadEssentialAssetsAsync().ConfigureAwait(false);

        if (playIntro && this.introMechanics != null)
        {
            // Play intro sequence
            GD.Print("NarrativeSystem: Playing intro sequence");
            await this.introMechanics.StartIntroSequenceAsync().ConfigureAwait(false);
        }

        // Start main narrative flow
        await this.StartMainNarrativeFlowAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Preloads essential assets for optimal performance.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task PreloadEssentialAssetsAsync()
    {
        if (this.assetPreloader != null)
        {
            GD.Print("NarrativeSystem: Preloading essential assets");
            await this.assetPreloader.PreloadAssetsAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Starts the main narrative flow with the opening sequence.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task StartMainNarrativeFlowAsync()
    {
        GD.Print("NarrativeSystem: Starting main narrative flow");

        // Create and start opening sequence
        var openingSequence = this.CreateNarrativeSequence("enhanced_opening");
        if (openingSequence != null)
        {
            await this.PlaySequenceAsync(openingSequence).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates a narrative sequence by its registered name.
    /// </summary>
    /// <param name="sequenceName">The name of the sequence to create.</param>
    /// <returns>The created narrative sequence, or null if creation failed.</returns>
    public NarrativeSequence? CreateNarrativeSequence(string sequenceName)
    {
        if (!this.sequenceRegistry.TryGetValue(sequenceName, out Type? sequenceType))
        {
            GD.PrintErr($"NarrativeSystem: Unknown sequence type '{sequenceName}'");
            return null;
        }

        try
        {
            var sequence = (NarrativeSequence?) Activator.CreateInstance(sequenceType);
            if (sequence != null)
            {
                // Set common properties
                sequence.SequenceId = sequenceName;
                this.AddChild(sequence);
                GD.Print($"NarrativeSystem: Created sequence '{sequenceName}'");
            }

            return sequence;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"NarrativeSystem: Failed to create sequence '{sequenceName}': {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Plays a narrative sequence asynchronously.
    /// </summary>
    /// <param name="sequence">The sequence to play.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task PlaySequenceAsync(NarrativeSequence sequence)
    {
        if (sequence == null)
        {
            GD.PrintErr("NarrativeSystem: Cannot play null sequence");
            return;
        }

        this.activeSequence = sequence;

        try
        {
            GD.Print($"NarrativeSystem: Playing sequence '{sequence.SequenceId}'");
            await sequence.PlayAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"NarrativeSystem: Error playing sequence '{sequence.SequenceId}': {ex.Message}");
        }
        finally
        {
            this.activeSequence = null;
        }
    }

    /// <summary>
    /// Transitions to a new narrative sequence.
    /// </summary>
    /// <param name="nextSequenceName">The name of the next sequence to play.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task TransitionToSequenceAsync(string nextSequenceName)
    {
        // Fade out current sequence
        if (this.screenTransition != null)
        {
            await this.screenTransition.Cover(0.5f).ConfigureAwait(false);
        }

        // Clean up current sequence
        if (this.activeSequence != null)
        {
            this.activeSequence.QueueFree();
            this.activeSequence = null;
        }

        // Create and play next sequence
        var nextSequence = this.CreateNarrativeSequence(nextSequenceName);
        if (nextSequence != null)
        {
            // Fade in next sequence
            if (this.screenTransition != null)
            {
                await this.screenTransition.Reveal(0.5f).ConfigureAwait(false);
            }

            await this.PlaySequenceAsync(nextSequence).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the audio manager component.
    /// </summary>
    /// <returns>The audio manager, or null if not initialized.</returns>
    public NarrativeAudioManager? GetAudioManager()
    {
        return this.audioManager;
    }

    /// <summary>
    /// Gets the asset mapper component.
    /// </summary>
    /// <returns>The asset mapper, or null if not initialized.</returns>
    public NarrativeAssetMapper? GetAssetMapper()
    {
        return this.assetMapper;
    }

    /// <summary>
    /// Gets the asset preloader component.
    /// </summary>
    /// <returns>The asset preloader, or null if not initialized.</returns>
    public NarrativeAssetPreloader? GetAssetPreloader()
    {
        return this.assetPreloader;
    }

    /// <summary>
    /// Gets the intro mechanics component.
    /// </summary>
    /// <returns>The intro mechanics, or null if not initialized.</returns>
    public OmegaSpiralIntroMechanics? GetIntroMechanics()
    {
        return this.introMechanics;
    }

    /// <summary>
    /// Gets the screen transition component.
    /// </summary>
    /// <returns>The screen transition, or null if not found.</returns>
    public ScreenTransition? GetScreenTransition()
    {
        return this.screenTransition;
    }

    /// <summary>
    /// Gets a preloaded asset by its path.
    /// </summary>
    /// <param name="path">The path of the asset to retrieve.</param>
    /// <typeparam name="T">The expected type of the asset.</typeparam>
    /// <returns>The preloaded asset, or null if not found.</returns>
    public T? GetPreloadedAsset<T>(string path) where T : Resource
    {
        return this.assetPreloader?.GetPreloadedAsset<T>(path);
    }

    /// <summary>
    /// Plays a sound effect using the audio manager.
    /// </summary>
    /// <param name="soundType">The type of sound effect to play.</param>
    public void PlaySoundEffect(AudioPlayerType soundType)
    {
        switch (soundType)
        {
            case AudioPlayerType.Selection:
                this.audioManager?.PlaySelectionSound();
                break;
            case AudioPlayerType.Transition:
                this.audioManager?.PlayTransitionSound();
                break;
            default:
                this.audioManager?.PlayTypewriterSound();
                break;
        }
    }

    /// <summary>
    /// Shuts down the narrative system and cleans up resources.
    /// </summary>
    public void Shutdown()
    {
        GD.Print("NarrativeSystem: Shutting down");

        // Clean up active sequence
        if (this.activeSequence != null)
        {
            this.activeSequence.QueueFree();
            this.activeSequence = null;
        }

        // Clean up components
        this.audioManager?.QueueFree();
        this.assetMapper?.QueueFree();
        this.assetPreloader?.QueueFree();
        this.introMechanics?.QueueFree();

        this.IsInitialized = false;
        GD.Print("NarrativeSystem: Shutdown complete");
    }
}
