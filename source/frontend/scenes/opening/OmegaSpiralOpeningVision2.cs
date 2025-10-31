// <copyright file="OmegaSpiralOpeningVision2.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Frontend.Scenes.Opening;

using Godot;
using OmegaSpiral.Source.Frontend.Design;

/// <summary>
/// Vision 2: "The Spiral Descent" – Custom opening scene for Ωmega Spiral.
/// Orchestrates a 15-second opening sequence featuring:
/// - Perlin noise layers creating a spiraling descent effect (0-3s)
/// - Starfield warping and spiral distortion (3-9s)
/// - Chromatic aberration with grid overlay (6-12s)
/// - Ωmega Spiral logo emergence at center with pulsing effect (9-12s)
/// - UI buttons fade-in with matching glitch aesthetic (12-15s)
///
/// RESPONSIBILITY: Orchestrate the Vision 2 timeline, manage shader animations,
/// and coordinate the transition to the main menu or game scene.
/// </summary>
[GlobalClass]
public partial class OmegaSpiralOpeningVision2 : Control
{
    /// <summary>
    /// Duration of the entire opening sequence in seconds.
    /// </summary>
    private const float SequenceDuration = 15.0f;

    /// <summary>
    /// Timeline breakpoints for Vision 2 visual effects.
    /// </summary>
    private const float PerlinNoiseStart = 0.0f;
    private const float PerlinNoiseEnd = 3.0f;
    private const float StarfieldStart = 3.0f;
    private const float StarfieldEnd = 9.0f;
    private const float ChromaticAberrationStart = 6.0f;
    private const float LogoStart = 9.0f;
    private const float LogoEnd = 12.0f;
    private const float UIFadeStart = 12.0f;
    private const float UIFadeEnd = 15.0f;

    private ShaderMaterial? _perlinNoiseMaterial;
    private ShaderMaterial? _starfieldMaterial;
    private ShaderMaterial? _chromaticAberrationMaterial;
    private Tween? _sequenceTween;
    private float _elapsedTime;

    public override void _Ready()
    {
        base._Ready();
        try
        {
            InitializeShaderMaterials();
            StartOpeningSequence();
        }
        catch (Exception ex)
        {
            GD.PushError($"[OmegaSpiralOpeningVision2] Initialization failed: {ex.Message}");
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        _elapsedTime += (float) delta;
    }

    /// <summary>
    /// Initializes all shader materials using the DesignService.GetShader() accessor.
    /// Each shader is loaded once and cached in a ShaderMaterial for efficient animation.
    /// </summary>
    private void InitializeShaderMaterials()
    {
        try
        {
            // Load perlin noise shader for initial descent effect
            var perlinShader = DesignService.GetShader("perlin_noise");
            if (perlinShader != null)
            {
                _perlinNoiseMaterial = new ShaderMaterial { Shader = perlinShader };
                GD.Print("[OmegaSpiralOpeningVision2] Perlin noise shader loaded.");
            }
        }
        catch (Exception ex)
        {
            GD.PushWarning($"[OmegaSpiralOpeningVision2] Failed to load perlin_noise shader: {ex.Message}");
        }

        try
        {
            // Load starfield shader for cosmic spiral
            var starfieldShader = DesignService.GetShader("star");
            if (starfieldShader != null)
            {
                _starfieldMaterial = new ShaderMaterial { Shader = starfieldShader };
                GD.Print("[OmegaSpiralOpeningVision2] Starfield shader loaded.");
            }
        }
        catch (Exception ex)
        {
            GD.PushWarning($"[OmegaSpiralOpeningVision2] Failed to load star shader: {ex.Message}");
        }

        try
        {
            // Load chromatic aberration shader for glitch aesthetic
            var chromaticShader = DesignService.GetShader("chromatic_aberration");
            if (chromaticShader != null)
            {
                _chromaticAberrationMaterial = new ShaderMaterial { Shader = chromaticShader };
                GD.Print("[OmegaSpiralOpeningVision2] Chromatic aberration shader loaded.");
            }
        }
        catch (Exception ex)
        {
            GD.PushWarning($"[OmegaSpiralOpeningVision2] Failed to load chromatic_aberration shader: {ex.Message}");
        }

        GD.Print("[OmegaSpiralOpeningVision2] Shader material initialization complete.");
    }

    /// <summary>
    /// Orchestrates the 15-second Vision 2 opening sequence with precise timing.
    /// Uses tweens to animate shader parameters and UI elements in sync.
    /// </summary>
    private void StartOpeningSequence()
    {
        if (_sequenceTween != null)
        {
            _sequenceTween.Kill();
        }

        _sequenceTween = CreateTween();
        _sequenceTween.SetTrans(Tween.TransitionType.Linear);

        try
        {
            // Phase 1: Perlin noise descent (0-3s)
            _sequenceTween.TweenCallback(AnimatePerlinNoiseDescent);
            _sequenceTween.TweenInterval(PerlinNoiseEnd - PerlinNoiseStart);

            // Phase 2: Starfield spiral and distortion (3-9s)
            _sequenceTween.TweenCallback(AnimateStarfieldSpiral);
            _sequenceTween.TweenInterval(StarfieldEnd - StarfieldStart);

            // Phase 3: Chromatic aberration overlay (6-12s, overlaps with starfield)
            _sequenceTween.TweenCallback(AnimateChromaticAberration);
            _sequenceTween.TweenInterval(LogoEnd - ChromaticAberrationStart);

            // Phase 4: Logo emergence and pulsing (9-12s)
            _sequenceTween.TweenCallback(AnimateLogoEmergence);
            _sequenceTween.TweenInterval(LogoEnd - LogoStart);

            // Phase 5: UI fade-in (12-15s)
            _sequenceTween.TweenCallback(AnimateUIFadeIn);
            _sequenceTween.TweenInterval(UIFadeEnd - UIFadeStart);

            // Transition to main menu
            _sequenceTween.TweenCallback(TransitionToMainMenu);

            GD.Print("[OmegaSpiralOpeningVision2] Opening sequence timeline initialized.");
        }
        catch (Exception ex)
        {
            GD.PushError($"[OmegaSpiralOpeningVision2] Failed to start opening sequence: {ex.Message}");
            // Fallback: skip directly to main menu
            TransitionToMainMenu();
        }
    }

    /// <summary>
    /// Animates the initial Perlin noise descent effect (0-3s).
    /// Creates a cellular division pattern suggesting chaos organizing into order.
    /// </summary>
    private void AnimatePerlinNoiseDescent()
    {
        if (_perlinNoiseMaterial == null)
            return;

        GD.Print("[OmegaSpiralOpeningVision2] Starting Perlin noise descent animation...");
        _perlinNoiseMaterial.SetShaderParameter("time_offset", 0.0f);
        _perlinNoiseMaterial.SetShaderParameter("scale", 2.0f);
    }

    /// <summary>
    /// Animates the starfield spiral effect (3-9s).
    /// Warps stars into a spiral pattern and adds vertex distortion.
    /// </summary>
    private void AnimateStarfieldSpiral()
    {
        if (_starfieldMaterial == null)
            return;

        GD.Print("[OmegaSpiralOpeningVision2] Starting starfield spiral animation...");
        _starfieldMaterial.SetShaderParameter("spiral_intensity", 0.5f);
        _starfieldMaterial.SetShaderParameter("rotation_speed", 0.3f);
    }

    /// <summary>
    /// Animates the chromatic aberration overlay (6-12s).
    /// Creates RGB channel separation with glitch interference patterns.
    /// </summary>
    private void AnimateChromaticAberration()
    {
        if (_chromaticAberrationMaterial == null)
            return;

        GD.Print("[OmegaSpiralOpeningVision2] Starting chromatic aberration animation...");
        _chromaticAberrationMaterial.SetShaderParameter("aberration_strength", 0.02f);
        _chromaticAberrationMaterial.SetShaderParameter("animation_speed", 1.0f);
    }

    /// <summary>
    /// Animates the Ωmega Spiral logo emergence at the center (9-12s).
    /// Logo scales up, glows, and pulses with a heartbeat effect.
    /// </summary>
    private void AnimateLogoEmergence()
    {
        GD.Print("[OmegaSpiralOpeningVision2] Starting logo emergence animation...");
        // TODO: Get the logo node from the scene tree and animate its scale, alpha, and glow.
        // var logo = GetNode<Control>("LogoNode");
        // var tween = CreateTween();
        // tween.Parallel().TweenProperty(logo, "scale", Vector2.One, 1.5f);
        // tween.Parallel().TweenProperty(logo, "modulate:a", 1.0f, 1.5f);
    }

    /// <summary>
    /// Animates the UI button fade-in (12-15s).
    /// Buttons appear with the matching glitch aesthetic applied.
    /// </summary>
    private void AnimateUIFadeIn()
    {
        GD.Print("[OmegaSpiralOpeningVision2] Starting UI fade-in animation...");
        // TODO: Get the button container and fade in buttons with staggered delay.
        // var buttonContainer = GetNode<Control>("ButtonContainer");
        // var tween = CreateTween();
        // tween.TweenProperty(buttonContainer, "modulate:a", 1.0f, 1.0f);
    }

    /// <summary>
    /// Transitions from the opening sequence to the main menu scene.
    /// Called after all visual effects complete.
    /// </summary>
    private void TransitionToMainMenu()
    {
        try
        {
            GD.Print("[OmegaSpiralOpeningVision2] Opening sequence complete. Transitioning to main menu...");

            // Check if SceneLoader autoload exists
            if (IsNodeReady() && GetTree().Root.HasNode("/root/SceneLoader"))
            {
                var sceneLoader = GetTree().Root.GetNode("SceneLoader");
                if (sceneLoader != null && sceneLoader.HasMethod("change_scene_to_resource"))
                {
                    sceneLoader.Call("change_scene_to_resource");
                    return;
                }
            }

            // Fallback: load main menu directly
            GD.PrintErr("[OmegaSpiralOpeningVision2] SceneLoader not available. Loading main menu directly.");
            GetTree().ChangeSceneToFile("res://source/frontend/scenes/menus/main_menu/main_menu.tscn");
        }
        catch (Exception ex)
        {
            GD.PushError($"[OmegaSpiralOpeningVision2] Failed to transition to main menu: {ex.Message}");
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_sequenceTween != null)
        {
            _sequenceTween.Kill();
            _sequenceTween = null;
        }
    }
}
