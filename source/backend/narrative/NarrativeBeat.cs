// <copyright file="NarrativeBeat.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Narrative;

/// <summary>
/// Represents a narrative beat with text, visual preset, delay, and typing speed.
/// </summary>
public class NarrativeBeat
{
    /// <summary>Gets the display text.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets the optional visual preset.</summary>
    public string? VisualPreset { get; set; }

    /// <summary>Gets the delay in seconds.</summary>
    public float DelaySeconds { get; set; }

    /// <summary>Gets the typing speed (chars/sec); -1 uses default.</summary>
    public float TypingSpeed { get; set; } = 30f;
}
