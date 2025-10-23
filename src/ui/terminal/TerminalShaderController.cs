using Godot;
using System;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.UI.Terminal;

/// <summary>
/// Implementation of terminal shader controller.
/// Manages CRT shader effects, visual presets, and pixel dissolve transitions.
/// </summary>
public class TerminalShaderController : ITerminalShaderController, IDisposable
{
    private readonly ColorRect _display;
    private ShaderMaterial? _currentMaterial;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the TerminalShaderController.
    /// </summary>
    /// <param name="display">The ColorRect node to apply shader effects to.</param>
    /// <exception cref="ArgumentNullException">Thrown when display is null.</exception>
    public TerminalShaderController(ColorRect display)
    {
        _display = display ?? throw new ArgumentNullException(nameof(display));
    }

    /// <inheritdoc/>
    public async Task ApplyVisualPresetAsync(string presetName)
    {
        if (string.IsNullOrEmpty(presetName))
            throw new ArgumentException("Preset name cannot be null or empty", nameof(presetName));

        var preset = TerminalPresetProvider.GetPreset(presetName);
        if (preset == null)
            throw new ArgumentException($"Unknown preset: {presetName}", nameof(presetName));

        // Remove existing material
        ResetShaderEffects();

        // Apply new preset if it has a shader
        if (!string.IsNullOrEmpty(preset.ShaderPath))
        {
            var shader = GD.Load<Shader>(preset.ShaderPath);
            if (shader == null)
                throw new InvalidOperationException($"Failed to load shader: {preset.ShaderPath}");

            _currentMaterial = new ShaderMaterial();
            _currentMaterial.Shader = shader;

            // Apply preset parameters
            foreach (var param in preset.Parameters)
            {
                _currentMaterial.SetShaderParameter(param.Key, param.Value);
            }

            _display.Material = _currentMaterial;
        }

        // Small delay to ensure shader is applied
        await Task.Delay(10);
    }

    /// <inheritdoc/>
    public async Task PixelDissolveAsync(float duration = 1.0f)
    {
        if (duration <= 0)
            throw new ArgumentException("Duration must be positive", nameof(duration));

        // Create dissolve material if not exists
        if (_currentMaterial == null)
        {
            _currentMaterial = new ShaderMaterial();
            _display.Material = _currentMaterial;
        }

        // Set dissolve parameters
        _currentMaterial.SetShaderParameter("dissolve_progress", 0.0f);
        _currentMaterial.SetShaderParameter("dissolve_speed", 1.0f / duration);

        // Animate dissolve with fixed frame rate
        const int frameRate = 60;
        int totalFrames = (int)(duration * frameRate);
        for (int frame = 0; frame < totalFrames; frame++)
        {
            float progress = (float)frame / totalFrames;
            _currentMaterial.SetShaderParameter("dissolve_progress", progress);
            await Task.Delay(1000 / frameRate); // Delay for one frame
        }

        // Ensure fully dissolved
        _currentMaterial.SetShaderParameter("dissolve_progress", 1.0f);
    }

    /// <inheritdoc/>
    public void ResetShaderEffects()
    {
        if (_currentMaterial != null)
        {
            _display.Material = null;
            _currentMaterial = null;
        }
    }

    /// <inheritdoc/>
    public ShaderMaterial? GetCurrentShaderMaterial()
    {
        return _currentMaterial;
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
        if (!_disposed)
        {
            if (disposing)
            {
                ResetShaderEffects();
            }
            _disposed = true;
        }
    }
}
