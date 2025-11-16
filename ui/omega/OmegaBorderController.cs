using Godot;

namespace OmegaSpiral.Source.Ui.Omega
{
    /// <summary>
    /// Controller that forwards exported C# properties to the border shader.
    /// Attach this script to the BorderPanel node (or any Control that has a child
    /// TextureRect named "BorderTexture").
    /// </summary>
    public partial class OmegaBorderController : Control
    {
        // Exposed shader parameters – designers can edit them in the inspector.
        /// <summary>
        /// Gets or sets the base color of the border.
        /// </summary>
        [Export] public Color BaseColor { get; set; } = new Color(1, 0.666667f, 0); // red (accent_red)
        /// <summary>
        /// Gets or sets the width of the border.
        /// </summary>
        [Export] public float BorderWidth { get; set; } = 0.03f;
        /// <summary>
        /// Gets or sets the speed of the pulse effect.
        /// </summary>
        [Export] public float PulseSpeed { get; set; } = 0.1f;
        /// <summary>
        /// Gets or sets the amplitude of the wave effect.
        /// </summary>
        [Export] public float WaveAmplitude { get; set; } = 0.02f;

        // Nullable because the material may not be present at construction time.
        private ShaderMaterial? _Material;

        /// <summary>
        /// Called when the node enters the scene tree for the first time.
        /// Initializes the shader material reference.
        /// </summary>
        public override void _Ready()
        {
            // Look for a child TextureRect named "BorderTexture" that holds the ShaderMaterial.
            var textureRect = GetNodeOrNull<TextureRect>("BorderTexture");
            if (textureRect != null)
            {
                _Material = textureRect.Material as ShaderMaterial;
            }

            if (_Material == null)
            {
                GD.PrintErr("OmegaBorderController: ShaderMaterial not found on BorderTexture.");
            }

            UpdateShaderUniforms();
        }

        /// <summary>
        /// Called when the node receives a notification.
        /// Updates shader uniforms when editor settings change.
        /// </summary>
        /// <param name="what">The notification type.</param>
        public override void _Notification(int what)
        {
            // Godot.Object defines NotificationEditorSettingsChanged = 2000.
            const int notificationEditorSettingsChanged = 2000;
            if (what == notificationEditorSettingsChanged)
            {
                UpdateShaderUniforms();
            }
        }

        // Push all exported values to the shader.
        private void UpdateShaderUniforms()
        {
            if (_Material == null) return;
            _Material.SetShaderParameter("base_color", BaseColor);
            _Material.SetShaderParameter("border_width", BorderWidth);
            _Material.SetShaderParameter("pulse_speed", PulseSpeed);
            // The shader uses the uniform name "jaggedness" for amplitude.
            _Material.SetShaderParameter("jaggedness", WaveAmplitude);
        }

        // Helper methods that UI controls can call directly.
        /// <summary>
        /// Sets the border width and updates the shader parameter.
        /// </summary>
        /// <param name="value">The new border width value.</param>
        public void SetBorderWidth(float value)
        {
            BorderWidth = value;
            _Material?.SetShaderParameter("border_width", value);
        }

        /// <summary>
        /// Sets the pulse speed and updates the shader parameter.
        /// </summary>
        /// <param name="value">The new pulse speed value.</param>
        public void SetPulseSpeed(float value)
        {
            PulseSpeed = value;
            _Material?.SetShaderParameter("pulse_speed", value);
        }

        /// <summary>
        /// Sets the wave amplitude and updates the shader parameter.
        /// </summary>
        /// <param name="value">The new wave amplitude value.</param>
        public void SetWaveAmplitude(float value)
        {
            WaveAmplitude = value;
            _Material?.SetShaderParameter("jaggedness", value);
        }
    }
}
