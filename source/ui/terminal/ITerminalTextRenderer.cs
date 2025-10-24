using Godot;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Ui.Terminal;

/// <summary>
/// Interface for rendering and animating text in the terminal.
/// Handles ghost typing effects, text display, and text formatting.
/// </summary>
public interface ITerminalTextRenderer
{
    /// <summary>
    /// Appends text to the terminal display with ghost typing animation.
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <param name="typingSpeed">Characters per second for typing animation. Default is 30.</param>
    /// <param name="delayBeforeStart">Delay in seconds before starting the animation.</param>
    /// <returns>A task representing the async operation.</returns>
    Task AppendTextAsync(string text, float typingSpeed = 30f, float delayBeforeStart = 0f);

    /// <summary>
    /// Clears all text from the terminal display.
    /// </summary>
    void ClearText();

    /// <summary>
    /// Sets the text color for subsequent text rendering.
    /// </summary>
    /// <param name="color">The color to use for text.</param>
    void SetTextColor(Color color);

    /// <summary>
    /// Gets the current text content of the terminal.
    /// </summary>
    /// <returns>The current text content.</returns>
    string GetCurrentText();

    /// <summary>
    /// Scrolls the text display to show the most recent content.
    /// </summary>
    void ScrollToBottom();

    /// <summary>
    /// Checks if text animation is currently in progress.
    /// </summary>
    /// <returns>True if text is being animated, false otherwise.</returns>
    bool IsAnimating();
}
