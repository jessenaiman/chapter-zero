// <copyright file="OmegaAudioAccessibilityProfile.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Audio;

/// <summary>
/// Describes accessibility preferences that affect audio timing and intensity.
/// </summary>
public readonly struct OmegaAudioAccessibilityProfile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OmegaAudioAccessibilityProfile"/> struct.
    /// </summary>
    /// <param name="reduceAudioIntensity">True to reduce sudden spikes.</param>
    /// <param name="essentialAudioOnly">True to play only essential cues.</param>
    /// <param name="isFirstReveal">True when the secret reveal is being seen for the first time.</param>
    public OmegaAudioAccessibilityProfile(bool reduceAudioIntensity, bool essentialAudioOnly, bool isFirstReveal)
    {
        ReduceAudioIntensity = reduceAudioIntensity;
        EssentialAudioOnly = essentialAudioOnly;
        IsFirstReveal = isFirstReveal;
    }

    /// <summary>
    /// Gets a value indicating whether intensity spikes should be reduced.
    /// </summary>
    public bool ReduceAudioIntensity { get; }

    /// <summary>
    /// Gets a value indicating whether non-essential ambience should be muted.
    /// </summary>
    public bool EssentialAudioOnly { get; }

    /// <summary>
    /// Gets a value indicating whether this is the player's first reveal sequence.
    /// </summary>
    public bool IsFirstReveal { get; }

    /// <summary>
    /// Gets the default profile used when no accessibility overrides are provided.
    /// </summary>
    public static OmegaAudioAccessibilityProfile Default => new(false, false, true);
}
