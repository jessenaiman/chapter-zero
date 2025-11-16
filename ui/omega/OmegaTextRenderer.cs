using Godot;
using System;
using System.Threading.Tasks;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Text renderer for Omega UI system - standard Godot RichTextLabel with text animation.
/// Handles text display, ghost typing animations, and text formatting.
/// Use as a normal Godot node - add to scene tree or instantiate with 'new OmegaTextRenderer()'.
/// </summary>
[GlobalClass]
public partial class OmegaTextRenderer : RichTextLabel
{
    private bool _IsAnimating;

    /// <summary>
    /// Initializes a new instance of the OmegaTextRenderer.
    /// Standard Godot node - no parameters needed.
    /// </summary>
    public OmegaTextRenderer()
    {
        BbcodeEnabled = true; // Enable BBCode for formatting
        // Color will be set in _Ready from theme
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();
        var theme = GetTheme();
        if (theme != null)
        {
            var warmAmber = theme.GetColor("gold", "OmegaSpiral");
            SetTextColor(warmAmber);
        }
        else
        {
            SetTextColor(Colors.Yellow); // Fallback
        }
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
            await Task.Delay((int) (delayBeforeStart * 1000)).ConfigureAwait(false);
        }

        _IsAnimating = true;

        try
        {
            var currentText = this.Text;
            var charDelay = 1.0f / typingSpeed; // seconds per character

            foreach (var character in text)
            {
                currentText += character;
                CallDeferred(nameof(UpdateTextDeferred), currentText);

                // Wait for the character delay, but allow for cancellation
                await Task.Delay((int) (charDelay * 1000)).ConfigureAwait(false);
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
        this.Text = "";
    }

    /// <inheritdoc/>
    public void SetTextColor(Color color)
    {
        // Store color for future text - RichTextLabel handles this via BBCode
        // This is a simplified implementation - full implementation would track color state
        this.AddThemeColorOverride("default_color", color);
    }

    /// <inheritdoc/>
    public string GetCurrentText()
    {
        return this.Text;
    }

    /// <inheritdoc/>
    public void ScrollToBottom()
    {
        // For RichTextLabel, we can scroll to the end by setting scroll following
        this.ScrollFollowing = true;
    }

    /// <summary>
    /// Gets whether the renderer is currently animating text.
    /// </summary>
    /// <returns><see langword="true"/> if text animation is in progress, <see langword="false"/> otherwise.</returns>
    public bool IsAnimating()
    {
        return _IsAnimating;
    }

    /// <summary>
    /// Deferred method to update the text display on the main thread.
    /// </summary>
    /// <param name="text">The text to set on the RichTextLabel.</param>
    private void UpdateTextDeferred(string text)
    {
        this.Text = text;
    }
}
