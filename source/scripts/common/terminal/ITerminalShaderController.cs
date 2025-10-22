using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Scripts.Common.Terminal;

/// <summary>
/// Interface for controlling terminal shader effects and visual presets.
/// Handles CRT shader configurations, pixel dissolve effects, and visual transitions.
/// </summary>
public interface ITerminalShaderController
{
    /// <summary>
    /// Applies a visual preset to the terminal display.
    /// </summary>
    /// <param name="presetName">The name of the preset to apply (e.g., "phosphor", "scanlines", "glitch").</param>
    /// <returns>A task representing the async operation.</returns>
    Task ApplyVisualPresetAsync(string presetName);

    /// <summary>
    /// Performs a pixel dissolve effect on the terminal text.
    /// </summary>
    /// <param name="duration">The duration of the dissolve effect in seconds.</param>
    /// <returns>A task representing the async operation.</returns>
    Task PixelDissolveAsync(float duration = 1.0f);

    /// <summary>
    /// Resets all shader effects to default state.
    /// </summary>
    void ResetShaderEffects();

    /// <summary>
    /// Gets the current shader material applied to the terminal.
    /// </summary>
    /// <returns>The current ShaderMaterial, or null if none applied.</returns>
    ShaderMaterial? GetCurrentShaderMaterial();
}
