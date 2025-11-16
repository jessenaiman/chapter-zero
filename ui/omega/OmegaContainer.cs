using Godot;

namespace OmegaSpiral.UI.Omega
{
    /// <summary>
    /// Provides a styled container panel with customizable corner radius, padding, and color gradients for the Omega UI theme.
    /// </summary>
    public partial class OmegaContainer : Panel
    {
        [Export] public float CornerRadius = 40f;
        [Export] public float FramePadding = 12f;
        [Export] public Color ColorA = new Color(0.5f, 0.9f, 1f);
        [Export] public Color ColorB = new Color(1f, 0.9f, 0.4f);
        [Export] public Color ColorC = new Color(1f, 0.3f, 0.2f);

        public override void _Ready()
        {
            var border = GetNode<OmegaFrame>("BorderPanel");
            var filaments = GetNode<FilamentField>("FilamentField");

            border.CornerRadius = CornerRadius;
            border.FramePadding = FramePadding;
            border.ColorA = ColorA;
            border.ColorB = ColorB;
            border.ColorC = ColorC;

            filaments.CornerRadius = CornerRadius;
            filaments.FramePadding = FramePadding;
            filaments.ColorA = ColorA;
            filaments.ColorB = ColorB;
            filaments.ColorC = ColorC;
        }
    }
}
