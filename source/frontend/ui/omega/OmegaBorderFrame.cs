using Godot;
using OmegaSpiral.Source.Frontend.Design;

namespace OmegaSpiral.Source.Ui.Omega
{
    /// <summary>
    /// Animated spiral border frame with three intermingling energy streams.
    /// Creates a decorative border using shader-based rendering with wavelike particle trails
    /// in LightThread (Silver), ShadowThread (Golden), and AmbitionThread (Crimson) colors.
    ///
    /// RESPONSIBILITY: Pure border frame presentation - shader setup, animation parameters, thread colors.
    /// Does NOT manage parent control or layout - assumes parent will add it to scene tree.
    /// </summary>
    [GlobalClass]
    public partial class OmegaBorderFrame : ColorRect
    {
        private ShaderMaterial? _ShaderMaterial;

        /// <summary>
        /// Initializes the spiral border frame with shader and default animation parameters.
        /// Loads spiral_border.gdshader and applies design system colors and animation settings.
        /// If shader file is missing, logs error but frame remains functional (transparent fallback).
        /// </summary>
        public OmegaBorderFrame()
        {
            // Configure basic layout properties in constructor
            Name = "BorderFrame";
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            ZIndex = 10;
            MouseFilter = MouseFilterEnum.Ignore;

            // Anchor to fill entire parent control
            AnchorLeft = 0;
            AnchorTop = 0;
            AnchorRight = 1;
            AnchorBottom = 1;
        }

        /// <summary>
        /// Called when the node enters the scene tree.
        /// Initializes the shader and animation parameters here (not in constructor).
        /// </summary>
        public override void _Ready()
        {
            base._Ready();
            try
            {
                InitializeShader();
            }
            catch (Exception ex)
            {
                GD.PushError($"[OmegaBorderFrame] Shader initialization failed: {ex.Message}");
                // Don't rethrow - allow the border to function without shader
            }
        }

        /// <summary>
        /// Sets up the shader and animation parameters.
        /// Called from _Ready() to initialize after the node enters the scene tree.
        /// </summary>
        private void InitializeShader()
        {
            // Load spiral border shader
            var shader = GD.Load<Shader>("res://source/shaders/spiral_border.gdshader");
            if (shader == null)
            {
                GD.PushError("[OmegaBorderFrame] spiral_border.gdshader not found at res://source/shaders/spiral_border.gdshader");
                return;
            }

            _ShaderMaterial = new ShaderMaterial { Shader = shader };
            Material = _ShaderMaterial;

            // Apply design system colors and shader defaults
            ApplyDesignSystemColors();
            ApplyShaderDefaults();
        }

        /// <summary>
        /// Applies thread colors from the design system to the shader.
        /// Updates LightThread (Silver), ShadowThread (Golden), and AmbitionThread (Crimson).
        /// </summary>
        private void ApplyDesignSystemColors()
        {
            if (_ShaderMaterial == null)
                return;

            _ShaderMaterial.SetShaderParameter("light_thread", DesignService.GetColor("light_thread"));
            _ShaderMaterial.SetShaderParameter("shadow_thread", DesignService.GetColor("shadow_thread"));
            _ShaderMaterial.SetShaderParameter("ambition_thread", DesignService.GetColor("ambition_thread"));
        }

        /// <summary>
        /// Applies default animation parameters from the design configuration.
        /// Falls back to legacy values if configuration is missing.
        /// </summary>
        private void ApplyShaderDefaults()
        {
            if (_ShaderMaterial == null)
                return;

            if (DesignService.TryGetShaderDefaults("spiral_border", out var defaults) && defaults.Count > 0)
            {
                foreach (var entry in defaults)
                {
                    _ShaderMaterial.SetShaderParameter(entry.Key, Variant.From(entry.Value));
                }
                return;
            }

            ApplyFallbackDefaults();
        }

        /// <summary>
        /// Applies fallback constants to preserve legacy behavior when configuration is missing.
        /// </summary>
        private void ApplyFallbackDefaults()
        {
            if (_ShaderMaterial == null)
            {
                return;
            }

            _ShaderMaterial.SetShaderParameter("rotation_speed", 0.05f);
            _ShaderMaterial.SetShaderParameter("wave_speed", 0.8f);
            _ShaderMaterial.SetShaderParameter("wave_frequency", 8.0f);
            _ShaderMaterial.SetShaderParameter("wave_amplitude", 0.25f);
            _ShaderMaterial.SetShaderParameter("border_width", 0.015f);
            _ShaderMaterial.SetShaderParameter("glow_intensity", 1.2f);
            _ShaderMaterial.SetShaderParameter("flow_speed", 2.0f);
            _ShaderMaterial.SetShaderParameter("particle_density", 20.0f);
            _ShaderMaterial.SetShaderParameter("trail_length", 1.5f);
            _ShaderMaterial.SetShaderParameter("line_width", 0.003f);
        }

        /// <summary>
        /// Gets the current rotation speed of the spiral animation.
        /// Range: 0.0 (static) to 2.0 (fast). Default: 0.05 (very slow).
        /// </summary>
        /// <returns>Current rotation speed value.</returns>
        public float GetRotationSpeed()
        {
            if (_ShaderMaterial == null)
                return 0.05f;

            return (float)_ShaderMaterial.GetShaderParameter("rotation_speed");
        }

        /// <summary>
        /// Gets the current wave flow speed of the border animation.
        /// Range: 0.0 (static) to 5.0 (fast). Default: 0.8 (gentle flow).
        /// </summary>
        /// <returns>Current wave speed value.</returns>
        public float GetWaveSpeed()
        {
            if (_ShaderMaterial == null)
                return 0.8f;

            return (float)_ShaderMaterial.GetShaderParameter("wave_speed");
        }

        /// <summary>
        /// Configures the animation speed of the spiral border.
        /// Allows independent control of rotation and wave flow speeds.
        /// </summary>
        /// <param name="rotationSpeed">Rotation speed of spiral. Range: 0.0 (static) to 2.0 (fast).</param>
        /// <param name="waveSpeed">Wave flow speed. Range: 0.0 (static) to 5.0 (fast).</param>
        public void ConfigureAnimationSpeed(float rotationSpeed, float waveSpeed)
        {
            if (_ShaderMaterial == null)
            {
                GD.PushWarning("[OmegaBorderFrame] Cannot configure animation - ShaderMaterial not initialized");
                return;
            }

            _ShaderMaterial.SetShaderParameter("rotation_speed", Mathf.Clamp(rotationSpeed, 0.0f, 2.0f));
            _ShaderMaterial.SetShaderParameter("wave_speed", Mathf.Clamp(waveSpeed, 0.0f, 5.0f));
        }

        /// <summary>
        /// Updates the three thread colors used by the spiral border animation.
        /// </summary>
        /// <param name="lightThread">Silver/light thread color from the design palette.</param>
        /// <param name="shadowThread">Golden/shadow thread color from the design palette.</param>
        /// <param name="ambitionThread">Crimson/ambition thread color from the design palette.</param>
        public void UpdateThreadColors(Color lightThread, Color shadowThread, Color ambitionThread)
        {
            if (_ShaderMaterial == null)
            {
                GD.PushWarning("[OmegaBorderFrame] Cannot update thread colors - ShaderMaterial not initialized");
                return;
            }

            _ShaderMaterial.SetShaderParameter("light_thread", lightThread);
            _ShaderMaterial.SetShaderParameter("shadow_thread", shadowThread);
            _ShaderMaterial.SetShaderParameter("ambition_thread", ambitionThread);
        }

        /// <summary>
        /// Gets the current ShaderMaterial used by the border frame.
        /// Exposed for testing and advanced shader parameter access.
        /// </summary>
        /// <returns>The ShaderMaterial, or null if shader failed to load.</returns>
        public ShaderMaterial? GetShaderMaterial() => _ShaderMaterial;
    }
}
