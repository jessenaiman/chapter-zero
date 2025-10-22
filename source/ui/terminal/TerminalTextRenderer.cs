using Godot;
using System;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.UI.Terminal;

/// <summary>
/// Implementation of terminal text renderer.
/// Handles text display, ghost typing animations, and text formatting.
/// </summary>
public class TerminalTextRenderer : ITerminalTextRenderer, IDisposable
{
    private readonly RichTextLabel _textDisplay;
    private bool _isAnimating;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the TerminalTextRenderer.
    /// </summary>
    /// <param name="textDisplay">The RichTextLabel node to display text on.</param>
    /// <exception cref="ArgumentNullException">Thrown when textDisplay is null.</exception>
    public TerminalTextRenderer(RichTextLabel textDisplay)
    {
        _textDisplay = textDisplay ?? throw new ArgumentNullException(nameof(textDisplay));
        _textDisplay.BbcodeEnabled = true; // Enable BBCode for formatting
    }

    /// <inheritdoc/>
    public async Task AppendTextAsync(string text, float typingSpeed = 30f, float delayBeforeStart = 0f)
    {
        if (string.IsNullOrEmpty(text))
            return;

        if (typingSpeed <= 0)
            throw new ArgumentException("Typing speed must be positive", nameof(typingSpeed));

        if (delayBeforeStart < 0)
            throw new ArgumentException("Delay before start cannot be negative", nameof(delayBeforeStart));

        // Delay before starting animation
        if (delayBeforeStart > 0)
        {
            await Task.Delay((int)(delayBeforeStart * 1000));
        }

        _isAnimating = true;

        try
        {
            var currentText = _textDisplay.Text;
            var charDelay = 1.0f / typingSpeed; // seconds per character

            foreach (var character in text)
            {
                currentText += character;
                _textDisplay.Text = currentText;

                // Wait for the character delay, but allow for cancellation
                await Task.Delay((int)(charDelay * 1000));
            }
        }
        finally
        {
            _isAnimating = false;
        }
    }

    /// <inheritdoc/>
    public void ClearText()
    {
        _textDisplay.Text = "";
    }

    /// <inheritdoc/>
    public void SetTextColor(Color color)
    {
        // Store color for future text - RichTextLabel handles this via BBCode
        // This is a simplified implementation - full implementation would track color state
        _textDisplay.AddThemeColorOverride("default_color", color);
    }

    /// <inheritdoc/>
    public string GetCurrentText()
    {
        return _textDisplay.Text;
    }

    /// <inheritdoc/>
    public void ScrollToBottom()
    {
        // For RichTextLabel, we can scroll to the end by setting scroll following
        _textDisplay.ScrollFollowing = true;
    }

    /// <inheritdoc/>
    public bool IsAnimating()
    {
        return _isAnimating;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the renderer and cleans up resources.
    /// </summary>
    /// <param name="disposing">Whether this is being called from Dispose() or finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Clear any ongoing animation state
                _isAnimating = false;
            }
            _disposed = true;
        }
    }
}
