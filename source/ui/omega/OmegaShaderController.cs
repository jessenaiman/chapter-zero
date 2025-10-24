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
    private readonly ColorRect _display;
    private ShaderMaterial? _currentMaterial;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the OmegaShaderController.
    /// </summary>
    /// <param name="display">The ColorRect node to apply shader effects to.</param>
    /// <exception cref="ArgumentNullException">Thrown when display is null.</exception>
    public OmegaShaderController(ColorRect display)
    {
        _display = display ?? throw new ArgumentNullException(nameof(display));
    }

    /// <inheritdoc/>
    public async Task ApplyVisualPresetAsync(string shaderPath)
    {
        if (string.IsNullOrEmpty(shaderPath))
            throw new ArgumentException("Shader path cannot be null or empty", nameof(shaderPath));

        // Remove existing material
        ResetShaderEffects();

        // Load and apply shader
        var shader = GD.Load<Shader>(shaderPath);
        if (shader == null)
            throw new InvalidOperationException($"Failed to load shader: {shaderPath}");

        _currentMaterial = new ShaderMaterial();
        _currentMaterial.Shader = shader;
        _display.Material = _currentMaterial;

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
