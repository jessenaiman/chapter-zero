// <copyright file="GhostTerminalUi.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Stages.Ghost;

/// <summary>
/// Visual preset identifier for Ghost Terminal (Stage 1) shader effects.
/// These presets correspond to the different Dreamweaver personas and game states.
/// </summary>
public readonly struct GhostTerminalVisualPreset
{
    public string Value { get; }

    private GhostTerminalVisualPreset(string value) => Value = value;

    public static readonly GhostTerminalVisualPreset BootSequence = new("boot_sequence");
    public static readonly GhostTerminalVisualPreset StableBaseline = new("stable_baseline");
    public static readonly GhostTerminalVisualPreset SecretReveal = new("secret_reveal");
    public static readonly GhostTerminalVisualPreset ThreadLight = new("thread_light");
    public static readonly GhostTerminalVisualPreset ThreadMischief = new("thread_mischief");
    public static readonly GhostTerminalVisualPreset ThreadWrath = new("thread_wrath");
    public static readonly GhostTerminalVisualPreset ThreadBalance = new("thread_balance");

    public override string ToString() => Value;
}

/// <summary>
/// Stage 1 (Ghost Terminal) specific Ui that extends NarrativeUi for sequential story progression.
/// Adds visual presets for Dreamweaver personas and Stage 1-specific transition effects.
/// All Stage 1 scene scripts inherit from this to access Ghost Terminal infrastructure.
/// </summary>
[GlobalClass]
public partial class GhostTerminalUi : NarrativeUi
{
    /// <summary>
    /// Applies a visual preset to the Ghost Terminal shaders.
    /// Stage 1 specific - used for Dreamweaver persona switching and effects.
    /// </summary>
    /// <param name="presetName">The Ghost Terminal preset to apply.</param>
    protected async Task ApplyVisualPresetAsync(GhostTerminalVisualPreset presetName)
    {
        if (ShaderController != null)
        {
            await ShaderController.ApplyVisualPresetAsync(presetName.Value).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Performs a pixel dissolve transition effect specific to Ghost Terminal.
    /// Stage 1 specific visual effect for scene transitions.
    /// </summary>
    /// <param name="duration">The duration in seconds.</param>
    protected new async Task PixelDissolveAsync(float duration)
    {
        if (ShaderController != null)
        {
            await ShaderController.PixelDissolveAsync(duration).ConfigureAwait(false);
        }
    }
}
