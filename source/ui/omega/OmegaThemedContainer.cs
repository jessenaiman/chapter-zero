// <copyright file="OmegaThemedContainer.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Container with optional Omega visual theme (border, shaders, CRT effects).
/// Composes Omega components via factory - enable/disable via exports.
/// Used by narrative scenes that want the full Omega aesthetic.
/// </summary>
/// <remarks>
/// This class provides opt-in Omega theming:
/// - EnableOmegaBorder: Animated spiral border frame
/// - EnableCrtShaders: Phosphor/scanline/glitch effects
/// - EnableOmegaText: Themed text rendering
/// All components are optional and configurable via Godot inspector.
/// </remarks>
[GlobalClass]
public partial class OmegaThemedContainer : OmegaContainer
{
    /// <summary>
    /// Gets or sets a value indicating whether to enable the animated Omega border frame.
    /// </summary>
    [Export]
    public bool EnableOmegaBorder { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to enable CRT shader effects.
    /// </summary>
    [Export]
    public bool EnableCrtShaders { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to enable Omega-themed text rendering.
    /// </summary>
    [Export]
    public bool EnableOmegaText { get; set; } = true;

    private OmegaBorderFrame? _BorderFrame;
    private ColorRect? _Background;
    private ColorRect? _PhosphorLayer; // Phosphor shader layer
    private ColorRect? _ScanlineLayer;
    private ColorRect? _GlitchLayer;
    private OmegaTextRenderer? _TextDisplay;

    /// <inheritdoc/>
    public override bool _Set(StringName property, Variant value)
    {
        switch ((string)property)
        {
            case "enable_omega_border":
                EnableOmegaBorder = value.AsBool();
                return true;
            case "enable_crt_shaders":
                EnableCrtShaders = value.AsBool();
                return true;
            case "enable_omega_text":
                EnableOmegaText = value.AsBool();
                return true;
        }

        return base._Set(property, value);
    }

    /// <summary>
    /// Standard Godot lifecycle - discovers Omega visual frame nodes.
    /// The frame is built in the scene file, not in code.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        // Discover Omega frame nodes (built in scene, not code)
        // Look for them at standard locations
        _Background = GetNodeOrNull<ColorRect>("Background") ?? GetNodeOrNull<ColorRect>("OmegaFrame/Background");
        _PhosphorLayer = GetNodeOrNull<ColorRect>("CrtFrame/PhosphorLayer") ?? GetNodeOrNull<ColorRect>("OmegaFrame/CrtFrame/PhosphorLayer");
        _ScanlineLayer = GetNodeOrNull<ColorRect>("CrtFrame/ScanlineLayer") ?? GetNodeOrNull<ColorRect>("OmegaFrame/CrtFrame/ScanlineLayer");
        _GlitchLayer = GetNodeOrNull<ColorRect>("CrtFrame/GlitchLayer") ?? GetNodeOrNull<ColorRect>("OmegaFrame/CrtFrame/GlitchLayer");
        _TextDisplay = FindTextDisplayRecursive(this);

        GD.Print($"[OmegaThemedContainer._Ready] Discovered frame nodes - Background:{_Background != null}, Phosphor:{_PhosphorLayer != null}, Scanline:{_ScanlineLayer != null}, Glitch:{_GlitchLayer != null}");

        // Assign to base property if found
        if (_TextDisplay != null)
        {
            TextRenderer = _TextDisplay;
        }
    }

    /// <summary>
    /// Recursively searches for an OmegaTextRenderer named "TextDisplay" in the node tree.
    /// </summary>
    private OmegaTextRenderer? FindTextDisplayRecursive(Node node)
    {
        if (node is OmegaTextRenderer renderer && node.Name == "TextDisplay")
        {
            return renderer;
        }

        for (int i = 0; i < node.GetChildCount(); i++)
        {
            var found = FindTextDisplayRecursive(node.GetChild(i));
            if (found != null) return found;
        }

        return null;
    }

    /// <summary>
    /// Builds the complete Omega visual frame: Background → CRT layers → Border.
    /// Creates a 4:3 aspect ratio CRT-style container centered in the viewport.
    /// </summary>
    private void BuildOmegaFrame()
    {
        // Ensure root control fills viewport to center the 4:3 container
        AnchorLeft = 0;
        AnchorTop = 0;
        AnchorRight = 1;
        AnchorBottom = 1;
        OffsetLeft = 0;
        OffsetTop = 0;
        OffsetRight = 0;
        OffsetBottom = 0;
        GD.Print($"[OmegaThemedContainer] Root control fills viewport");

        // Create fullscreen background (letterbox bars for 4:3 content)
        _Background = new ColorRect
        {
            Name = "Background",
            Color = OmegaSpiralColors.DeepSpace,
            MouseFilter = MouseFilterEnum.Ignore,
            AnchorLeft = 0,
            AnchorTop = 0,
            AnchorRight = 1,
            AnchorBottom = 1,
            ZIndex = -10 // Behind everything
        };
        AddChild(_Background);
        GD.Print($"[OmegaThemedContainer] Created fullscreen background");

        // Get bezel margin from config (defaults to 5% if not available)
        var appConfig = GetNodeOrNull<OmegaSpiral.Source.Scripts.Infrastructure.GameAppConfig>("/root/AppConfig");
        float bezelMargin = appConfig?.BezelMargin ?? 0.05f;

        GD.Print($"[OmegaThemedContainer] Using bezel margin: {bezelMargin * 100}%");

        // Create 4:3 aspect ratio container for CRT effect
        // Uses anchor-based positioning with bezel margins
        var crtFrame = new AspectRatioContainer
        {
            Name = "CrtFrame",
            Ratio = 4.0f / 3.0f,
            StretchMode = AspectRatioContainer.StretchModeEnum.Fit,
            AnchorLeft = bezelMargin,
            AnchorTop = bezelMargin,
            AnchorRight = 1.0f - bezelMargin,
            AnchorBottom = 1.0f - bezelMargin,
            GrowHorizontal = Control.GrowDirection.Both,
            GrowVertical = Control.GrowDirection.Both
        };
        AddChild(crtFrame);
        GD.Print($"[OmegaThemedContainer] Created 4:3 CRT frame with {bezelMargin * 100}% margins");

        // Create CRT shader layers inside the 4:3 frame
        if (EnableCrtShaders)
        {
            // PhosphorLayer will be created as OmegaShaderController below

            _ScanlineLayer = new ColorRect
            {
                Name = "ScanlineLayer",
                Color = OmegaSpiralColors.ScanlineOverlay,
                MouseFilter = MouseFilterEnum.Ignore,
                AnchorLeft = 0,
                AnchorTop = 0,
                AnchorRight = 1,
                AnchorBottom = 1,
                ZIndex = -8 // Behind content
            };
            crtFrame.AddChild(_ScanlineLayer);

            _GlitchLayer = new ColorRect
            {
                Name = "GlitchLayer",
                Color = OmegaSpiralColors.GlitchDistortion,
                MouseFilter = MouseFilterEnum.Ignore,
                AnchorLeft = 0,
                AnchorTop = 0,
                AnchorRight = 1,
                AnchorBottom = 1,
                ZIndex = -7 // Behind content
            };
            crtFrame.AddChild(_GlitchLayer);

            GD.Print($"[OmegaThemedContainer] Created CRT shader layers inside 4:3 frame");

            // Create shader controller as the primary layer (replaces PhosphorLayer)
            // It will automatically discover and manage the child shader layers
            var shaderController = new OmegaShaderController
            {
                Name = "ShaderController",
                Color = OmegaSpiralColors.PhosphorGlow,
                MouseFilter = MouseFilterEnum.Ignore,
                AnchorLeft = 0,
                AnchorTop = 0,
                AnchorRight = 1,
                AnchorBottom = 1,
                ZIndex = -10 // Behind everything
            };
            crtFrame.AddChild(shaderController);

            // Move shader layers to be children of the controller
            if (_ScanlineLayer != null)
            {
                _ScanlineLayer.GetParent()?.RemoveChild(_ScanlineLayer);
                shaderController.AddChild(_ScanlineLayer);
            }

            if (_GlitchLayer != null)
            {
                _GlitchLayer.GetParent()?.RemoveChild(_GlitchLayer);
                shaderController.AddChild(_GlitchLayer);
            }

            // Replace PhosphorLayer with the controller
            if (_PhosphorLayer != null)
            {
                _PhosphorLayer.QueueFree();
            }
            _PhosphorLayer = shaderController; // Cache it

            // Assign to base property
            ShaderController = shaderController;
            CallDeferred(nameof(ApplyDefaultShaderStack));
        }

        // Create border frame on top of the 4:3 container
        if (EnableOmegaBorder)
        {
            _BorderFrame = new OmegaBorderFrame();
            _BorderFrame.ZIndex = 100; // On top of everything
            crtFrame.AddChild(_BorderFrame);
            GD.Print($"[OmegaThemedContainer] Created BorderFrame on top of 4:3 frame");
        }
    }

    /// <summary>
    /// Moves ContentContainer inside the 4:3 CRT frame.
    /// The scene file defines ContentContainer's content, but we reparent it into the aspect ratio container.
    /// </summary>
    private void SetupContentContainer()
    {
        var existingContent = GetNodeOrNull<Control>("ContentContainer");
        var crtFrame = GetNodeOrNull<AspectRatioContainer>("CrtFrame");

        if (crtFrame == null)
        {
            GD.PrintErr("[OmegaThemedContainer] CrtFrame not found - cannot setup content!");
            return;
        }

        if (existingContent != null)
        {
            GD.Print($"[OmegaThemedContainer] Found existing ContentContainer - reparenting into CRT frame");

            // Reparent ContentContainer into the 4:3 CRT frame
            RemoveChild(existingContent);
            crtFrame.AddChild(existingContent);

            // Ensure it fills the CRT frame
            existingContent.AnchorLeft = 0;
            existingContent.AnchorTop = 0;
            existingContent.AnchorRight = 1;
            existingContent.AnchorBottom = 1;
            existingContent.OffsetLeft = 0;
            existingContent.OffsetTop = 0;
            existingContent.OffsetRight = 0;
            existingContent.OffsetBottom = 0;
        }
        else
        {
            GD.PrintErr("[OmegaThemedContainer] No ContentContainer found in scene - this is required!");
        }

        GD.Print($"[OmegaThemedContainer] Final child order:");
        for (int i = 0; i < GetChildCount(); i++)
        {
            var child = GetChild(i);
            var zIndex = child is CanvasItem ci ? ci.ZIndex : 0;
            GD.Print($"  [{i}] {child.Name} (ZIndex={zIndex})");
        }

        if (crtFrame != null)
        {
            GD.Print($"[OmegaThemedContainer] CrtFrame children:");
            for (int i = 0; i < crtFrame.GetChildCount(); i++)
            {
                var child = crtFrame.GetChild(i);
                GD.Print($"  [{i}] {child.Name}");
            }
        }
    }

    /// <summary>
    /// Gets the border frame if created.
    /// </summary>
    /// <returns>The OmegaBorderFrame instance, or null if not enabled/created.</returns>
    public OmegaBorderFrame? GetBorderFrame() => _BorderFrame;

    /// <summary>
    /// Gets the phosphor shader layer if available.
    /// </summary>
    /// <returns>The phosphor ColorRect, or null if not in scene.</returns>
    protected ColorRect? GetPhosphorLayer() => _PhosphorLayer;

    /// <summary>
    /// Gets the scanline shader layer if available.
    /// </summary>
    /// <returns>The scanline ColorRect, or null if not in scene.</returns>
    protected ColorRect? GetScanlineLayer() => _ScanlineLayer;

    /// <summary>
    /// Gets the glitch shader layer if available.
    /// </summary>
    /// <returns>The glitch ColorRect, or null if not in scene.</returns>
    protected ColorRect? GetGlitchLayer() => _GlitchLayer;

    // ==================== PUBLIC CONVENIENCE METHODS ====================

    /// <summary>
    /// Appends text to the main display with optional typing animation.
    /// </summary>
    /// <param name="text">Text to append.</param>
    /// <param name="typingSpeed">Characters per second for typing effect.</param>
    /// <param name="delayBeforeStart">Delay in seconds before starting to type.</param>
    public async Task AppendTextAsync(string text, float typingSpeed = 30f, float delayBeforeStart = 0f)
    {
        if (TextRenderer == null)
        {
            GD.PushWarning("[OmegaThemedContainer] Cannot append text - TextRenderer not initialized.");
            return;
        }

        await TextRenderer.AppendTextAsync(text, typingSpeed, delayBeforeStart).ConfigureAwait(false);
    }

    /// <summary>
    /// Clears all text from the main display.
    /// </summary>
    public void ClearText()
    {
        TextRenderer?.ClearText();
    }

    /// <summary>
    /// Applies a visual preset to the shader layers.
    /// </summary>
    /// <param name="presetName">Name of the preset to apply (e.g., "phosphor", "scanlines", "glitch").</param>
    public async Task ApplyVisualPresetAsync(string presetName)
    {
        if (ShaderController == null)
        {
            GD.PushWarning("[OmegaThemedContainer] Cannot apply visual preset - ShaderController not initialized.");
            return;
        }

        await ShaderController.ApplyVisualPresetAsync(presetName).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs a pixel dissolve effect.
    /// </summary>
    /// <param name="durationSeconds">Duration of the dissolve effect in seconds.</param>
    public async Task PixelDissolveAsync(float durationSeconds = 2.5f)
    {
        if (ShaderController == null)
        {
            GD.PushWarning("[OmegaThemedContainer] Cannot perform dissolve - ShaderController not initialized.");
            return;
        }

        await ShaderController.PixelDissolveAsync(durationSeconds).ConfigureAwait(false);
    }

    /// <summary>
    /// Applies the default CRT shader stack (phosphor → scanlines → glitch).
    /// Deferred call to ensure all nodes are ready.
    /// </summary>
    private async void ApplyDefaultShaderStack()
    {
        if (!EnableCrtShaders || ShaderController == null)
        {
            return;
        }

        try
        {
            await ShaderController.ApplyPresetStackAsync(
                "crt_phosphor_base",
                "crt_scanlines_base",
                "crt_glitch_base").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[OmegaThemedContainer] Failed to apply default shader stack: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets or sets the rotation speed of the spiral border animation.
    /// </summary>
    public float BorderRotationSpeed
    {
        get
        {
            var borderFrame = GetBorderFrame();
            return borderFrame?.GetRotationSpeed() ?? 0.05f;
        }
        set
        {
            var borderFrame = GetBorderFrame();
            if (borderFrame != null)
            {
                borderFrame.ConfigureAnimationSpeed(Mathf.Clamp(value, 0.0f, 2.0f), borderFrame.GetWaveSpeed());
            }
        }
    }

    /// <summary>
    /// Gets or sets the wave flow speed of the border animation.
    /// </summary>
    public float BorderWaveSpeed
    {
        get
        {
            var borderFrame = GetBorderFrame();
            return borderFrame?.GetWaveSpeed() ?? 0.8f;
        }
        set
        {
            var borderFrame = GetBorderFrame();
            if (borderFrame != null)
            {
                borderFrame.ConfigureAnimationSpeed(borderFrame.GetRotationSpeed(), Mathf.Clamp(value, 0.0f, 5.0f));
            }
        }
    }
}
