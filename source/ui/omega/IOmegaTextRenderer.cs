using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.UI.Omega;

/// <summary>
/// Interface for text renderer that manages text display and animations.
/// Provides methods for text output, ghost typing effects, and text formatting.
/// </summary>
public interface IOmegaTextRenderer
{
    /// <summary>
    /// Appends text with optional ghost typing animation.
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <param name="typingSpeed">Characters per second for typing animation.</param>
    /// <param name="delayBeforeStart">Delay in seconds before starting the animation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AppendTextAsync(string text, float typingSpeed = 30f, float delayBeforeStart = 0f);

    /// <summary>
    /// Clears all text from the display.
    /// </summary>
    void ClearText();

    /// <summary>
    /// Sets the text color for the display.
    /// </summary>
    /// <param name="color">The color to set.</param>
    void SetTextColor(Color color);

    /// <summary>
    /// Gets the current text being displayed.
    /// </summary>
    /// <returns>The current text content.</returns>
    string GetCurrentText();

    /// <summary>
    /// Scrolls the text display to the bottom.
    /// </summary>
    void ScrollToBottom();

    /// <summary>
    /// Checks if text animation is currently in progress.
    /// </summary>
    /// <returns>True if animating, false otherwise.</returns>
    bool IsAnimating();
}
