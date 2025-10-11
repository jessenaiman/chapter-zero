namespace OmegaSpiral.Source.Scripts.UI.Dialogue
{
    using System.Threading.Tasks;
    using Godot;

    /// <summary>
    /// Manages visual effects for the narrative terminal including pixel dissolve and ASCII static overlays.
    /// </summary>
    public partial class NarrativeEffectsController : Node
    {
        private ColorRect pixelDissolveOverlay = default!;
        private ColorRect asciiStaticOverlay = default!;

        /// <summary>
        /// Initializes the effects controller with required overlay components.
        /// </summary>
        /// <param name="pixelDissolveOverlay">The overlay used for pixel dissolve effects.</param>
        /// <param name="asciiStaticOverlay">The overlay used for ASCII static effects.</param>
        public void Initialize(ColorRect pixelDissolveOverlay, ColorRect asciiStaticOverlay)
        {
            this.pixelDissolveOverlay = pixelDissolveOverlay;
            this.asciiStaticOverlay = asciiStaticOverlay;

            this.pixelDissolveOverlay.Visible = false;
            this.asciiStaticOverlay.Visible = false;
        }

        /// <summary>
        /// Plays a pixel dissolve effect with shader animation.
        /// </summary>
        /// <param name="durationMs">Duration of the effect in milliseconds.</param>
        /// <returns>A task that completes when the effect finishes.</returns>
        public async Task PlayPixelDissolveAsync(int durationMs)
        {
            if (this.pixelDissolveOverlay.Material is ShaderMaterial material)
            {
                this.pixelDissolveOverlay.Visible = true;
                material.SetShaderParameter("progress", 0f);

                Tween tween = this.CreateTween();
                tween.TweenProperty(material, "shader_parameter/progress", 1f, durationMs / 1000f);
                await this.ToSignal(tween, Tween.SignalName.Finished);

                material.SetShaderParameter("progress", 0f);
                this.pixelDissolveOverlay.Visible = false;
            }
            else
            {
                await Task.Delay(durationMs);
            }
        }

        /// <summary>
        /// Plays an ASCII static effect overlay.
        /// </summary>
        /// <param name="durationMs">Duration of the effect in milliseconds.</param>
        /// <returns>A task that completes when the effect finishes.</returns>
        public async Task PlayAsciiStaticAsync(int durationMs)
        {
            this.asciiStaticOverlay.Visible = true;
            await Task.Delay(durationMs);
            this.asciiStaticOverlay.Visible = false;
        }
    }
}
