using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.UI.Omega;

/// <summary>
/// Interface for shader controller that manages visual effects.
/// Provides methods for applying presets, transitions, and shader management.
/// </summary>
public interface IOmegaShaderController
{
    /// <summary>
    /// Applies a visual preset to the display.
    /// </summary>
    /// <param name="presetName">The name of the preset to apply.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ApplyVisualPresetAsync(string presetName);

    /// <summary>
    /// Performs a pixel dissolve transition effect.
    /// </summary>
    /// <param name="duration">The duration of the dissolve effect in seconds.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PixelDissolveAsync(float duration = 1.0f);

    /// <summary>
    /// Resets all shader effects to default state.
    /// </summary>
    void ResetShaderEffects();

    /// <summary>
    /// Gets the current shader material being used.
    /// </summary>
    /// <returns>The current shader material, or null if none is active.</returns>
    ShaderMaterial? GetCurrentShaderMaterial();
}
