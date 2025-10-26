using System;
using System.Globalization;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Utility helpers for rendering Ghost Terminal narrative lines.
/// Handles stage directions such as pauses without synthesizing new dialogue.
/// </summary>
internal static class GhostNarrator
{
    /// <summary>
    /// Detects a pause directive of the form <c>[PAUSE: 2.5s]</c>.
    /// </summary>
    /// <param name="line">The narrative line.</param>
    /// <param name="seconds">The duration in seconds, if parsed.</param>
    /// <returns><see langword="true"/> when the line represents a pause directive.</returns>
    public static bool TryParsePause(string line, out double seconds)
    {
        seconds = 0;

        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        string trimmed = line.Trim();

        if (!trimmed.StartsWith("[PAUSE", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        int colonIndex = trimmed.IndexOf(':');
        int suffixIndex = trimmed.IndexOf('s', StringComparison.OrdinalIgnoreCase);

        if (colonIndex <= 0 || suffixIndex <= colonIndex)
        {
            return false;
        }

        string slice = trimmed[(colonIndex + 1)..suffixIndex].Trim();

        if (double.TryParse(slice, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
        {
            seconds = parsed;
            return true;
        }

        return false;
    }
}
