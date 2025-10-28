using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Ui.Omega
{
    /// <summary>
    /// Integration tests for OmegaBorderFrame component.
    /// Tests the animated spiral border frame used by all Omega UI instances.
    ///
    /// RESPONSIBILITY: Verify OmegaBorderFrame creates, initializes, and configures correctly.
    /// Shader setup, thread colors, animation parameters, and visibility all tested here.
    ///
    /// Keeps these tests OUT of OmegaUiTests.cs and MainMenuTests.cs to avoid cluttering
    /// those files with frame-specific concerns.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public partial class OmegaBorderFrame_UnitTests
    {
        private ISceneRunner _Runner = null!;
        private OmegaBorderFrame _BorderFrame = null!;

        /// <summary>
        /// Sets up test fixtures before each test.
        /// Loads test fixture scene and adds BorderFrame to it.
        /// This ensures Godot processes layout and sizing correctly.
        /// </summary>
        [Before]
        public async Task Setup()
        {
            // Load test fixture scene with proper Control parent
            _Runner = ISceneRunner.Load("res://tests/fixtures/border_frame_test.tscn");

            // Create BorderFrame and add to fixture scene
            _BorderFrame = new OmegaBorderFrame();
            _Runner.Scene().AddChild(_BorderFrame);

            // Let Godot process the scene tree (layout, sizing, etc.)
            await _Runner.AwaitIdleFrame();
        }

        // ==================== CRITICAL INTEGRATION TESTS ====================

        /// <summary>
        /// CRITICAL: BorderFrame must have non-zero size to render.
        /// This will FAIL until the component is properly integrated into a scene.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void BorderFrame_HasNonZeroSize()
        {
            // This should FAIL because BorderFrame isn't in scene tree yet
            AssertThat(_BorderFrame!.Size.X).IsGreater(0f);
            AssertThat(_BorderFrame.Size.Y).IsGreater(0f);
        }

        // ==================== INITIALIZATION ====================

        /// <summary>
        /// OmegaBorderFrame initializes without errors.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void Constructor_InitializesSuccessfully()
        {
            AssertThat(_BorderFrame).IsNotNull();
            AssertThat(_BorderFrame!.Name).IsEqual("BorderFrame");
        }

        /// <summary>
        /// OmegaBorderFrame has correct layout properties.
        /// Fills parent control and ignores mouse events.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void Layout_PropertiesAreCorrect()
        {
            AssertThat(_BorderFrame!.SizeFlagsHorizontal).IsEqual(Control.SizeFlags.ExpandFill);
            AssertThat(_BorderFrame.SizeFlagsVertical).IsEqual(Control.SizeFlags.ExpandFill);
            AssertThat(_BorderFrame.MouseFilter).IsEqual(Control.MouseFilterEnum.Ignore);
            AssertThat(_BorderFrame.ZIndex).IsEqual(10);
        }

        /// <summary>
        /// OmegaBorderFrame anchors fill parent bounds.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void Anchors_FillParentBounds()
        {
            AssertThat(_BorderFrame!.AnchorLeft).IsEqual(0.0f);
            AssertThat(_BorderFrame.AnchorTop).IsEqual(0.0f);
            AssertThat(_BorderFrame.AnchorRight).IsEqual(1.0f);
            AssertThat(_BorderFrame.AnchorBottom).IsEqual(1.0f);
        }

        // ==================== SHADER SETUP ====================

        /// <summary>
        /// OmegaBorderFrame has ShaderMaterial configured.
        /// Spiral border shader is loaded and attached.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void ShaderMaterial_IsConfigured()
        {
            AssertThat(_BorderFrame!.GetShaderMaterial()).IsNotNull()
                .OverrideFailureMessage("OmegaBorderFrame must have ShaderMaterial");

            AssertThat(_BorderFrame.Material).IsInstanceOf<ShaderMaterial>()
                .OverrideFailureMessage("BorderFrame.Material must be ShaderMaterial");
        }

        /// <summary>
        /// BorderFrame shader uses three thread colors from OmegaSpiralColors.
        /// Verifies Light, Shadow, and Ambition thread colors are applied.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void ThreadColors_AppliedFromDesignSystem()
        {
            var shaderMaterial = _BorderFrame!.GetShaderMaterial();
            AssertThat(shaderMaterial).IsNotNull();

            var lightThread = (Color)shaderMaterial!.GetShaderParameter("light_thread");
            var shadowThread = (Color)shaderMaterial.GetShaderParameter("shadow_thread");
            var ambitionThread = (Color)shaderMaterial.GetShaderParameter("ambition_thread");

            AssertThat(lightThread).IsEqual(OmegaSpiralColors.LightThread)
                .OverrideFailureMessage("Light thread must match design system (Silver/White)");
            AssertThat(shadowThread).IsEqual(OmegaSpiralColors.ShadowThread)
                .OverrideFailureMessage("Shadow thread must match design system (Golden/Amber)");
            AssertThat(ambitionThread).IsEqual(OmegaSpiralColors.AmbitionThread)
                .OverrideFailureMessage("Ambition thread must match design system (Crimson/Red)");
        }

        // ==================== ANIMATION PARAMETERS ====================

        /// <summary>
        /// BorderFrame has default animation parameters set.
        /// Rotation speed, wave speed, and other parameters have sensible defaults.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void AnimationDefaults_AreSet()
        {
            var shaderMaterial = _BorderFrame!.GetShaderMaterial();
            AssertThat(shaderMaterial).IsNotNull();

            // Check default animation parameters
            var rotationSpeed = (float)shaderMaterial!.GetShaderParameter("rotation_speed");
            var waveSpeed = (float)shaderMaterial.GetShaderParameter("wave_speed");
            var waveFrequency = (float)shaderMaterial.GetShaderParameter("wave_frequency");
            var waveAmplitude = (float)shaderMaterial.GetShaderParameter("wave_amplitude");
            var borderWidth = (float)shaderMaterial.GetShaderParameter("border_width");
            var glowIntensity = (float)shaderMaterial.GetShaderParameter("glow_intensity");

            // Verify defaults match intended slow, subtle animation
            AssertThat(rotationSpeed).IsEqual(0.05f)
                .OverrideFailureMessage("Default rotation_speed should be 0.05 (very slow)");
            AssertThat(waveSpeed).IsEqual(0.8f)
                .OverrideFailureMessage("Default wave_speed should be 0.8 (gentle)");
            AssertThat(waveFrequency).IsEqual(8.0f)
                .OverrideFailureMessage("Default wave_frequency should be 8.0");
            AssertThat(waveAmplitude).IsEqual(0.25f)
                .OverrideFailureMessage("Default wave_amplitude should be 0.25 (subtle)");
            AssertThat(borderWidth).IsEqual(0.015f)
                .OverrideFailureMessage("Default border_width should be 0.015");
            AssertThat(glowIntensity).IsEqual(1.2f)
                .OverrideFailureMessage("Default glow_intensity should be 1.2");
        }

        /// <summary>
        /// GetRotationSpeed() returns current rotation speed.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void GetRotationSpeed_ReturnsCurrent()
        {
            AssertThat(_BorderFrame!.GetRotationSpeed()).IsEqual(0.05f);
        }

        /// <summary>
        /// GetWaveSpeed() returns current wave speed.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void GetWaveSpeed_ReturnsCurrent()
        {
            AssertThat(_BorderFrame!.GetWaveSpeed()).IsEqual(0.8f);
        }

        /// <summary>
        /// ConfigureAnimationSpeed() updates rotation and wave speeds.
        /// Allows independent control of both parameters.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void ConfigureAnimationSpeed_UpdatesBothParameters()
        {
            // Set custom speeds
            _BorderFrame!.ConfigureAnimationSpeed(0.5f, 2.0f);

            AssertThat(_BorderFrame.GetRotationSpeed()).IsEqual(0.5f)
                .OverrideFailureMessage("Rotation speed should be updated to 0.5");
            AssertThat(_BorderFrame.GetWaveSpeed()).IsEqual(2.0f)
                .OverrideFailureMessage("Wave speed should be updated to 2.0");
        }

        /// <summary>
        /// ConfigureAnimationSpeed() clamps values to valid ranges.
        /// Rotation: 0.0-2.0, Wave: 0.0-5.0.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void ConfigureAnimationSpeed_ClampsToValidRanges()
        {
            // Try to set values outside valid range
            _BorderFrame!.ConfigureAnimationSpeed(5.0f, 10.0f);

            AssertThat(_BorderFrame.GetRotationSpeed()).IsEqual(2.0f)
                .OverrideFailureMessage("Rotation speed should be clamped to max 2.0");
            AssertThat(_BorderFrame.GetWaveSpeed()).IsEqual(5.0f)
                .OverrideFailureMessage("Wave speed should be clamped to max 5.0");
        }

        /// <summary>
        /// ConfigureAnimationSpeed() accepts zero for static animation.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void ConfigureAnimationSpeed_AcceptsZero()
        {
            _BorderFrame!.ConfigureAnimationSpeed(0.0f, 0.0f);

            AssertThat(_BorderFrame.GetRotationSpeed()).IsEqual(0.0f);
            AssertThat(_BorderFrame.GetWaveSpeed()).IsEqual(0.0f);
        }

        // ==================== THREAD COLOR UPDATES ====================

        /// <summary>
        /// UpdateThreadColors() changes all three thread colors.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void UpdateThreadColors_ChangesAllThreads()
        {
            var newLight = Colors.Red;
            var newShadow = Colors.Green;
            var newAmbition = Colors.Blue;

            _BorderFrame!.UpdateThreadColors(newLight, newShadow, newAmbition);

            var shaderMaterial = _BorderFrame.GetShaderMaterial();
            var light = (Color)shaderMaterial!.GetShaderParameter("light_thread");
            var shadow = (Color)shaderMaterial.GetShaderParameter("shadow_thread");
            var ambition = (Color)shaderMaterial.GetShaderParameter("ambition_thread");

            AssertThat(light).IsEqual(newLight);
            AssertThat(shadow).IsEqual(newShadow);
            AssertThat(ambition).IsEqual(newAmbition);
        }

        /// <summary>
        /// UpdateThreadColors() can revert to design system colors.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void UpdateThreadColors_CanRevertToDesignSystem()
        {
            // Change to custom colors
            _BorderFrame!.UpdateThreadColors(Colors.Red, Colors.Green, Colors.Blue);

            // Revert to design system
            _BorderFrame.UpdateThreadColors(
                OmegaSpiralColors.LightThread,
                OmegaSpiralColors.ShadowThread,
                OmegaSpiralColors.AmbitionThread
            );

            var shaderMaterial = _BorderFrame.GetShaderMaterial();
            var light = (Color)shaderMaterial!.GetShaderParameter("light_thread");
            var shadow = (Color)shaderMaterial.GetShaderParameter("shadow_thread");
            var ambition = (Color)shaderMaterial.GetShaderParameter("ambition_thread");

            AssertThat(light).IsEqual(OmegaSpiralColors.LightThread);
            AssertThat(shadow).IsEqual(OmegaSpiralColors.ShadowThread);
            AssertThat(ambition).IsEqual(OmegaSpiralColors.AmbitionThread);
        }

        // ==================== VISIBILITY ====================

        /// <summary>
        /// BorderFrame is visible by default.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void BorderFrame_IsVisibleByDefault()
        {
            AssertThat(_BorderFrame!.Visible).IsTrue();
        }
    }
}
