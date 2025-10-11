namespace OmegaSpiral.Source.Scripts.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Service interface for rendering text with a typewriter effect.
    /// Handles character-by-character display animations and skip functionality.
    /// </summary>
    public interface ITypewriterService
    {
        /// <summary>
        /// Gets or sets the number of characters rendered per second.
        /// </summary>
        float CharactersPerSecond { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether rendering should be instant (no animation).
        /// </summary>
        bool ForceInstant { get; set; }

        /// <summary>
        /// Gets a value indicating whether a typewriter animation is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Renders the specified text with a typewriter effect.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="targetLabel">The label control to render into.</param>
        /// <returns>A task that completes when rendering finishes.</returns>
        Task RenderTextAsync(string text, Godot.RichTextLabel targetLabel);

        /// <summary>
        /// Skips the current typewriter animation and displays all remaining text immediately.
        /// </summary>
        void Skip();

        /// <summary>
        /// Stops the current typewriter animation without completing the text.
        /// </summary>
        void Stop();
    }
}
