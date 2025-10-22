using Godot;

namespace OmegaSpiral.Source.UI.Terminal;

/// <summary>
/// Interface for managing terminal shader effects and visual presets.
/// </summary>
public interface ITerminalShaderController
{
    /// <summary>
    /// Applies a visual preset asynchronously to the terminal shaders.
    /// </summary>
    /// <param name="presetName">The name of the preset to apply.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ApplyVisualPresetAsync(string presetName);

    /// <summary>
    /// Performs a pixel dissolve effect on the terminal.
    /// </summary>
    /// <param name="durationSeconds">The duration of the dissolve effect.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PixelDissolveAsync(float durationSeconds);

    /// <summary>
    /// Resets all shader effects and materials.
    /// </summary>
    void ResetShaderEffects();

    /// <summary>
    /// Gets the current shader material for a specific layer.
    /// </summary>
    /// <param name="layer">The shader layer to get.</param>
    /// <returns>The current shader material, or null if not available.</returns>
    ShaderMaterial? GetCurrentShaderMaterial();
}
