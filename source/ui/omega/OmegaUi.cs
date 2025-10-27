using Godot;
using System;
using System.Threading.Tasks;
using OmegaSpiral.Source.Ui.Omega;

namespace OmegaSpiral.Source.Ui.Omega
{

/// <summary>
/// DEPRECATED: Use OmegaThemedContainer directly instead.
/// This class remains for backward compatibility with existing scenes.
///
/// Base Omega UI orchestrator for scene-based composition.
/// Pure presentation layer - manages shader and text rendering components.
/// Single Responsibility: Component lifecycle and composition.
///
/// ARCHITECTURE:
/// - Scene-based UI defined in omega_ui.tscn
/// - Inherits OmegaThemedContainer for Omega visual theme
/// - _Ready() initializes components (minimal setup, no debug spam)
/// - _ExitTree() handles cleanup
/// - Signals for state communication with external systems
/// </summary>
[GlobalClass]
[Obsolete("Use OmegaThemedContainer directly. OmegaUi is kept for backward compatibility only.")]
public partial class OmegaUi : OmegaThemedContainer
{
    // Backward compatibility - all functionality is inherited from OmegaThemedContainer

    /// <summary>
    /// Appends text to the main display with optional typing animation.
    /// </summary>
    public async Task AppendTextAsync(string text, float typingSpeed = 30f, float delayBeforeStart = 0f)
    {
        if (TextRenderer == null)
        {
            GD.PushWarning("[OmegaUi] Cannot append text - TextRenderer not initialized.");
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
    public async Task ApplyVisualPresetAsync(string presetName)
    {
        if (ShaderController == null)
        {
            GD.PushWarning("[OmegaUi] Cannot apply visual preset - ShaderController not initialized.");
            return;
        }

        await ShaderController.ApplyVisualPresetAsync(presetName).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs a pixel dissolve effect.
    /// </summary>
    public async Task PixelDissolveAsync(float durationSeconds = 2.5f)
    {
        if (ShaderController == null)
        {
            GD.PushWarning("[OmegaUi] Cannot perform dissolve - ShaderController not initialized.");
            return;
        }

        await ShaderController.PixelDissolveAsync(durationSeconds).ConfigureAwait(false);
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
}
