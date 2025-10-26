using Godot;
using System;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Implementation of shader controller for Omega Ui system.
/// Manages shader effects, visual presets, and pixel dissolve transitions.
/// </summary>
public class OmegaShaderController : IOmegaShaderController, IDisposable
{
    private readonly ColorRect _Display;
    private ShaderMaterial? _CurrentMaterial;
    private bool _Disposed;

    /// <summary>
    /// Initializes a new instance of the OmegaShaderController.
    /// </summary>
    /// <param name="display">The ColorRect node to apply shader effects to.</param>
    /// <exception cref="ArgumentNullException">Thrown when display is null.</exception>
    public OmegaShaderController(ColorRect display)
    {
        _Display = display ?? throw new ArgumentNullException(nameof(display));
    }

    /// <inheritdoc/>
    public async Task ApplyVisualPresetAsync(string presetName)
    {
        if (string.IsNullOrEmpty(presetName))
            throw new ArgumentException("Preset name cannot be null or empty", nameof(presetName));

        // Get the preset configuration
        var preset = OmegaShaderPresets.GetPreset(presetName);
        if (preset == null)
            throw new ArgumentException($"Unknown shader preset: {presetName}", nameof(presetName));

        // Remove existing material
        ResetShaderEffects();

        // Load and apply shader material if path is provided
        if (!string.IsNullOrEmpty(preset.ShaderPath))
        {
            _CurrentMaterial = GD.Load<ShaderMaterial>(preset.ShaderPath);
            if (_CurrentMaterial == null)
                throw new InvalidOperationException($"Failed to load shader material: {presetName}");

            // Apply parameters if any (override defaults)
            if (preset.Parameters != null)
            {
                foreach (var param in preset.Parameters)
                {
                    _CurrentMaterial.SetShaderParameter(param.Key, param.Value);
                }
            }

            _Display.Material = _CurrentMaterial;
        }
        // If ShaderPath is null (like terminal preset), material stays null

        // Small delay to ensure shader is applied
        await Task.Delay(10).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task PixelDissolveAsync(float duration = 1.0f)
    {
        if (duration <= 0)
            throw new ArgumentException("Duration must be positive", nameof(duration));

        // Create dissolve material if not exists
        if (_CurrentMaterial == null)
        {
            _CurrentMaterial = new ShaderMaterial();
            _Display.Material = _CurrentMaterial;
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
        if (_CurrentMaterial != null)
        {
            _Display.Material = null;
            _CurrentMaterial = null;
        }
    }

    /// <inheritdoc/>
    public ShaderMaterial? GetCurrentShaderMaterial()
    {
        return _CurrentMaterial;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the controller and cleans up resources.
    /// </summary>
    /// <param name="disposing">Whether this is being called from Dispose() or finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_Disposed)
        {
            if (disposing)
            {
                ResetShaderEffects();
            }
            _Disposed = true;
        }
    }
}
