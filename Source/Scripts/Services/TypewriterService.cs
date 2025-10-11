namespace OmegaSpiral.Source.Scripts.Services
{
    using System.Threading.Tasks;
    using Godot;
    using OmegaSpiral.Source.Scripts.Interfaces;

    /// <summary>
    /// Service for rendering text with a typewriter effect.
    /// Provides character-by-character animation with configurable speed and skip functionality.
    /// </summary>
    public class TypewriterService : ITypewriterService
    {
        private bool isRunning;
        private bool skipRequested;

        /// <inheritdoc/>
        public float CharactersPerSecond { get; set; } = 40f;

        /// <inheritdoc/>
        public bool ForceInstant { get; set; }

        /// <inheritdoc/>
        public bool IsRunning => isRunning;

        /// <inheritdoc/>
        public async Task RenderTextAsync(string text, RichTextLabel targetLabel)
        {
            if (targetLabel == null || string.IsNullOrEmpty(text))
            {
                return;
            }

            isRunning = true;
            skipRequested = false;

            try
            {
                if (ForceInstant)
                {
                    targetLabel.Text += text;
                    return;
                }

                double delayPerCharacter = 1.0 / CharactersPerSecond;

                foreach (char c in text)
                {
                    if (skipRequested)
                    {
                        targetLabel.Text += text.Substring(text.IndexOf(c));
                        break;
                    }

                    targetLabel.Text += c;
                    await Task.Delay((int)(delayPerCharacter * 1000));
                }
            }
            finally
            {
                isRunning = false;
                skipRequested = false;
            }
        }

        /// <inheritdoc/>
        public void Skip()
        {
            if (isRunning)
            {
                skipRequested = true;
            }
        }

        /// <inheritdoc/>
        public void Stop()
        {
            isRunning = false;
            skipRequested = false;
        }
    }
}
