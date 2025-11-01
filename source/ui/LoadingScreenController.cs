// <copyright file="LoadingScreenController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Ui
{
    /// <summary>
    /// Controls the loading screen display during scene transitions.
    /// Shows progress indication and loading status.
    /// </summary>
    [GlobalClass]
    public partial class LoadingScreenController : CanvasLayer
    {
        /// <summary>
        /// Gets or sets the duration for fading the loading screen in (in seconds).
        /// </summary>
        [Export]
        public float FadeInDuration { get; set; } = 0.3f;

        /// <summary>
        /// Gets or sets the duration for fading the loading screen out (in seconds).
        /// </summary>
        [Export]
        public float FadeOutDuration { get; set; } = 0.5f;

        private ColorRect? overlay;
        private Label? loadingLabel;
        private ProgressBar? progressBar;
        private Tween? fadeTween;

        /// <inheritdoc/>
        public override void _Ready()
        {
            this.CacheNodeReferences();
            this.SetupLoadingScreen();
        }

        /// <summary>
        /// Caches references to Ui nodes using unique names.
        /// </summary>
        private void CacheNodeReferences()
        {
            this.overlay = this.GetNode<ColorRect>("%Overlay");
            this.loadingLabel = this.GetNode<Label>("%LoadingLabel");
            this.progressBar = this.GetNode<ProgressBar>("%ProgressBar");
        }

        /// <summary>
        /// Sets up the initial loading screen state.
        /// </summary>
        private void SetupLoadingScreen()
        {
            if (this.overlay is not null)
            {
                var color = this.overlay.Color;
                color.A = 0.0f;
                this.overlay.Color = color;
            }
        }

        /// <summary>
        /// Shows the loading screen with fade-in animation.
        /// </summary>
        public void ShowLoadingScreen()
        {
            if (this.overlay is null)
            {
                return;
            }

            this.fadeTween?.Kill();
            this.fadeTween = this.CreateTween();
            this.fadeTween.SetTrans(Tween.TransitionType.Quad);
            this.fadeTween.SetEase(Tween.EaseType.Out);

            var color = this.overlay.Color;
            var targetColor = new Color(color.R, color.G, color.B, 0.8f);
            this.fadeTween.TweenProperty(this.overlay, "color", targetColor, this.FadeInDuration);

            GD.Print("Loading screen shown");
        }

        /// <summary>
        /// Hides the loading screen with fade-out animation.
        /// </summary>
        public void HideLoadingScreen()
        {
            if (this.overlay is null)
            {
                return;
            }

            this.fadeTween?.Kill();
            this.fadeTween = this.CreateTween();
            this.fadeTween.SetTrans(Tween.TransitionType.Quad);
            this.fadeTween.SetEase(Tween.EaseType.Out);

            var color = this.overlay.Color;
            var targetColor = new Color(color.R, color.G, color.B, 0.0f);
            this.fadeTween.TweenProperty(this.overlay, "color", targetColor, this.FadeOutDuration);

            GD.Print("Loading screen hidden");
        }

        /// <summary>
        /// Updates the loading progress.
        /// </summary>
        /// <param name="progress">Progress value from 0.0 to 1.0.</param>
        public void UpdateProgress(float progress)
        {
            if (this.progressBar is not null)
            {
                this.progressBar.Value = Mathf.Clamp(progress, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// Sets the loading status text.
        /// </summary>
        /// <param name="statusText">The status message to display.</param>
        public void SetLoadingText(string statusText)
        {
            if (this.loadingLabel is not null)
            {
                this.loadingLabel.Text = statusText;
            }
        }
    }
}
