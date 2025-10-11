namespace OmegaSpiral.Source.Scripts.UI.Dialogue
{
    using System.Threading.Tasks;
    using Godot;
    using OmegaSpiral.Source.Scripts.Interfaces;

    /// <summary>
    /// Renders narrative text with typewriter effect and handles text display for the narrative terminal.
    /// </summary>
    public partial class NarrativeRenderer : Node
    {
        private RichTextLabel outputLabel = default!;
        private ScrollContainer outputScroll = default!;
        private ITypewriterService typewriterService = default!;

        private bool isRendering;

        /// <summary>
        /// Initializes the narrative renderer with required components.
        /// </summary>
        /// <param name="outputLabel">The rich text label for displaying narrative content.</param>
        /// <param name="outputScroll">The scroll container wrapping the output label.</param>
        /// <param name="typewriterService">The service handling typewriter text animation.</param>
        public void Initialize(RichTextLabel outputLabel, ScrollContainer outputScroll, ITypewriterService typewriterService)
        {
            this.outputLabel = outputLabel;
            this.outputScroll = outputScroll;
            this.typewriterService = typewriterService;
        }

        /// <summary>
        /// Gets a value indicating whether text rendering is currently in progress.
        /// </summary>
        public bool IsRendering => this.isRendering;

        /// <summary>
        /// Displays text immediately without animation.
        /// </summary>
        /// <param name="text">The text to display, or empty string for blank line.</param>
        public void DisplayImmediate(string text)
        {
            if (this.outputLabel != null)
            {
                this.outputLabel.AppendText(string.IsNullOrEmpty(text) ? "\n" : $"{text}\n");
                this.ScrollToBottom();
            }
            else
            {
                GD.Print(text);
            }
        }

        /// <summary>
        /// Displays text with typewriter effect animation.
        /// </summary>
        /// <param name="text">The text to display with animation.</param>
        /// <returns>A task that completes when the animation finishes.</returns>
        public async Task DisplayTextAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                this.DisplayImmediate(string.Empty);
                return;
            }

            this.isRendering = true;

            await this.typewriterService.RenderTextAsync(
                text,
                character =>
                {
                    this.outputLabel.AppendText(character.ToString());
                    this.ScrollToBottom();
                },
                () => this.GetTree().CreateTimer(0.001f));

            this.outputLabel.AppendText("\n");
            this.ScrollToBottom();

            this.isRendering = false;
        }

        /// <summary>
        /// Requests that the current typewriter animation be skipped.
        /// </summary>
        public void RequestSkip()
        {
            this.typewriterService.Skip();
        }

        /// <summary>
        /// Stops the current typewriter animation.
        /// </summary>
        public void StopRendering()
        {
            this.typewriterService.Stop();
            this.isRendering = false;
        }

        private void ScrollToBottom()
        {
            this.outputLabel.ScrollToLine(this.outputLabel.GetLineCount());
            if (this.outputScroll.GetVScrollBar() is VScrollBar vScroll)
            {
                vScroll.Value = vScroll.MaxValue;
            }
        }
    }
}
