using Godot;
using OmegaSpiral.Source.Backend;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Shader controller for Omega UI system that IS a ColorRect node.
/// Manages shader effects, visual presets, and pixel dissolve transitions.
/// Extends ColorRect to be a proper Godot node following standard architecture.
/// </summary>
/// <remarks>
/// This controller acts as the primary display layer and can manage additional
/// shader layers (phosphor, scanline, glitch) that are child nodes.
/// Relies on Godot's node lifecycle for cleanup - no manual disposal needed.
/// </remarks>
[GlobalClass]
public partial class OmegaShaderController : ColorRect
{
    private enum ShaderLayer
    {
        Primary,
        Phosphor,
        Scanline,
        Glitch
    }

    private static readonly Dictionary<string, ShaderLayer> _ShaderPathToLayer = new(StringComparer.OrdinalIgnoreCase)
    {
        { "res://source/shaders/crt_phosphor.tres", ShaderLayer.Phosphor },
        { "res://source/shaders/crt_scanlines.tres", ShaderLayer.Scanline },
        { "res://source/shaders/crt_glitch.tres", ShaderLayer.Glitch }
    };

    private readonly Dictionary<ShaderLayer, ColorRect> _LayerMap = new();
    private readonly Dictionary<ShaderLayer, ShaderMaterial?> _ActiveMaterials = new();
    private ShaderLayer _PrimaryLayer = ShaderLayer.Primary;
    private ShaderMaterial? _CurrentMaterial;

    /// <summary>
    /// Initializes a new instance of the <see cref="OmegaShaderController"/> class.
    /// This ColorRect acts as the primary shader display layer.
    /// </summary>
    public OmegaShaderController()
    {
        // Register self as primary layer
        _LayerMap[ShaderLayer.Primary] = this;
        _PrimaryLayer = ShaderLayer.Primary;
    }

    /// <summary>
    /// Called when the node enters the scene tree.
    /// Discovers and registers additional shader layers as child nodes.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        // Discover additional layers by name
        DiscoverShaderLayers();
    }

    /// <summary>
    /// Discovers and registers child ColorRect nodes as shader layers.
    /// Looks for nodes named PhosphorLayer, ScanlineLayer, GlitchLayer.
    /// </summary>
    private void DiscoverShaderLayers()
    {
        var phosphor = GetNodeOrNull<ColorRect>("PhosphorLayer");
        if (phosphor != null)
        {
            _LayerMap[ShaderLayer.Phosphor] = phosphor;
        }

        var scanline = GetNodeOrNull<ColorRect>("ScanlineLayer");
        if (scanline != null)
        {
            _LayerMap[ShaderLayer.Scanline] = scanline;
        }

        var glitch = GetNodeOrNull<ColorRect>("GlitchLayer");
        if (glitch != null)
        {
            _LayerMap[ShaderLayer.Glitch] = glitch;
        }
    }

    /// <inheritdoc/>
    public async Task ApplyVisualPresetAsync(string presetName)
    {
        if (string.IsNullOrEmpty(presetName))
            throw new ArgumentException("Preset name cannot be null or empty", nameof(presetName));

        // Get the preset configuration from DesignConfigService
        if (!DesignConfigService.TryGetShaderPreset(presetName, out var preset) || preset == null)
        {
            var availablePresets = DesignConfigService.Configuration.ShaderPresets?.Keys.ToList() ?? new List<string>();
            GD.PrintErr($"[OmegaShaderController] Shader preset '{presetName}' not found. Available presets: {string.Join(", ", availablePresets)}");
            return;
        }

        if (string.IsNullOrEmpty(preset.Shader))
        {
            ResetShaderEffects();
            await Task.CompletedTask.ConfigureAwait(false);
            return;
        }

        var material = GD.Load<ShaderMaterial>(preset.Shader)?.Duplicate() as ShaderMaterial;
        if (material == null)
        {
            throw new InvalidOperationException($"Failed to load shader material: {presetName}");
        }

        if (preset.Parameters != null)
        {
            foreach (var param in preset.Parameters)
            {
                material.SetShaderParameter(param.Key, Variant.From(param.Value));
            }
        }

        var targetLayer = ResolveLayerFromShader(preset.Shader);
        if (!_LayerMap.TryGetValue(targetLayer, out var target))
        {
            target = _LayerMap.GetValueOrDefault(_PrimaryLayer, _LayerMap.Values.First());
        }

        target.Material = material;
        _ActiveMaterials[targetLayer] = material;

        if (targetLayer == _PrimaryLayer || _PrimaryLayer == ShaderLayer.Primary)
        {
            _CurrentMaterial = material;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    /// <summary>
    /// Applies a stack of visual presets in order.
    /// </summary>
    /// <param name="presetNames">Preset names to apply.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ApplyPresetStackAsync(params string[] presetNames)
    {
        if (presetNames == null || presetNames.Length == 0)
        {
            return;
        }

        foreach (var preset in presetNames)
        {
            await ApplyVisualPresetAsync(preset).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task PixelDissolveAsync(float duration = 1.0f)
    {
        if (duration <= 0)
            throw new ArgumentException("Duration must be positive", nameof(duration));

        var targetLayer = _LayerMap.ContainsKey(_PrimaryLayer) ? _PrimaryLayer : _LayerMap.Keys.First();
        var target = _LayerMap[targetLayer];

        if (_ActiveMaterials.TryGetValue(targetLayer, out var activeMaterial) && activeMaterial != null)
        {
            _CurrentMaterial = activeMaterial;
        }

        if (_CurrentMaterial == null)
        {
            _CurrentMaterial = new ShaderMaterial();
            target.Material = _CurrentMaterial;
        }

        // Set dissolve parameters
        _CurrentMaterial.SetShaderParameter("dissolve_progress", 0.0f);
        _CurrentMaterial.SetShaderParameter("dissolve_speed", 1.0f / duration);

        // Animate dissolve with fixed frame rate
        const int frameRate = 60;
        int totalFrames = (int)(duration * frameRate);
        for (int frame = 0; frame < totalFrames; frame++)
        {
            float progress = (float)frame / totalFrames;
            _CurrentMaterial.SetShaderParameter("dissolve_progress", progress);
            await Task.Delay(1000 / frameRate).ConfigureAwait(false); // Delay for one frame
        }

        // Ensure fully dissolved
        _CurrentMaterial.SetShaderParameter("dissolve_progress", 1.0f);
    }

    /// <inheritdoc/>
    public void ResetShaderEffects()
    {
        foreach (var rect in _LayerMap.Values)
        {
            rect.Material = null;
        }

        _ActiveMaterials.Clear();
        _CurrentMaterial = null;
    }

    /// <inheritdoc/>
    public ShaderMaterial? GetCurrentShaderMaterial()
    {
        return _CurrentMaterial;
    }

    /// <summary>
    /// Called when node exits the scene tree.
    /// Cleanup is handled by Godot's lifecycle.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        ResetShaderEffects();
    }

    private static ShaderLayer ResolveLayerFromShader(string shaderPath)
    {
        return _ShaderPathToLayer.TryGetValue(shaderPath, out var layer)
            ? layer
            : ShaderLayer.Primary;
    }
}
