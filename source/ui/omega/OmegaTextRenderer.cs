using Godot;
using System;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Implementation of text renderer for Omega Ui system.
/// Handles text display, ghost typing animations, and text formatting.
/// </summary>
public class OmegaTextRenderer : IOmegaTextRenderer, IDisposable
{
    private readonly RichTextLabel _TextDisplay;
    private bool _IsAnimating;
    private bool _Disposed;

    /// <summary>
    /// Initializes a new instance of the OmegaTextRenderer.
    /// </summary>
    /// <param name="textDisplay">The RichTextLabel node to display text on.</param>
    /// <exception cref="ArgumentNullException">Thrown when textDisplay is null.</exception>
    public OmegaTextRenderer(RichTextLabel textDisplay)
    {
        _TextDisplay = textDisplay ?? throw new ArgumentNullException(nameof(textDisplay));
        _TextDisplay.BbcodeEnabled = true; // Enable BBCode for formatting
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

        _IsAnimating = true;

        try
        {
            var currentText = _TextDisplay.Text;
            var charDelay = 1.0f / typingSpeed; // seconds per character

            foreach (var character in text)
            {
                currentText += character;
                _TextDisplay.Text = currentText;

                // Wait for the character delay, but allow for cancellation
                await Task.Delay((int)(charDelay * 1000));
            }
        }
        finally
        {
            _IsAnimating = false;
        }
    }

    /// <inheritdoc/>
    public void ClearText()
    {
        _TextDisplay.Text = "";
    }

    /// <inheritdoc/>
    public void SetTextColor(Color color)
    {
        // Store color for future text - RichTextLabel handles this via BBCode
        // This is a simplified implementation - full implementation would track color state
        _TextDisplay.AddThemeColorOverride("default_color", color);
    }

    /// <inheritdoc/>
    public string GetCurrentText()
    {
        return _TextDisplay.Text;
    }

    /// <inheritdoc/>
    public void ScrollToBottom()
    {
        // For RichTextLabel, we can scroll to the end by setting scroll following
        _TextDisplay.ScrollFollowing = true;
    }

    /// <inheritdoc/>
    public bool IsAnimating()
    {
        return _IsAnimating;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_Disposed)
        {
            if (disposing)
            {
                _IsAnimating = false;
            }
            _Disposed = true;
        }
    }
}
