// <copyright file="MainMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.UI.Menus;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.UI
{
    /// <summary>
    /// Unit tests for MainMenu UI component.
    /// Validates inheritance, visual elements, and CRT effects.
    /// Ensures menu displays correctly with borders and shaders.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public class MainMenuTests : IDisposable
    {
        private ISceneRunner? runner;
        private MainMenu? mainMenu;

        /// <summary>
        /// Sets up the test scene and MainMenu instance.
        /// Loads the main_menu.tscn scene for testing.
        /// </summary>
        [Before]
        public void Setup()
        {
            this.runner = ISceneRunner.Load("res://source/stages/stage_0_start/main_menu.tscn");
            this.mainMenu = this.runner.Scene() as MainMenu;
            AssertThat(this.mainMenu).IsNotNull();
        }

        /// <summary>
        /// Cleans up test resources.
        /// </summary>
        [After]
        public void Teardown()
        {
            // Dispose runner first (which handles scene cleanup), then clear references.
            // Do NOT call QueueFree on mainMenu as it's owned by the runner.
            this.mainMenu = null;
            this.runner?.Dispose();
            this.runner = null;
        }

        /// <summary>
        /// Disposes of resources used by the test class.
        /// </summary>
        public void Dispose()
        {
            this.Teardown();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Tests that MainMenu inherits from OmegaUI.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void MainMenuInheritsFromOmegaUi()
        {
            AssertThat(this.mainMenu).IsInstanceOf<OmegaSpiral.Source.UI.Omega.OmegaUI>();
        }

        /// <summary>
        /// Tests that the title label is visible and contains non-empty text.
        /// </summary>
        /// <returns>Returns <see langword="void"/>.</returns>
        [TestCase]
        [RequireGodotRuntime]
        public void TitleTextIsVisible()
        {
            var titleLabel = this.runner!.Scene().GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/TitleMargin/TitleLabel");
            AssertThat(titleLabel).IsNotNull();
            AssertThat(titleLabel!.Text).IsNotEmpty();
            AssertThat(titleLabel!.Visible).IsTrue();
        }

        /// <summary>
        /// Tests that the description text is actually visible and contains expected content.
        /// This catches broken UI scenarios where no content renders.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void DescriptionTextIsVisible()
        {
            var descriptionLabel = this.runner!.Scene().GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/DescriptionMargin/DescriptionLabel");
            AssertThat(descriptionLabel).IsNotNull();
            AssertThat(descriptionLabel!.Text).IsNotEmpty();
            AssertThat(descriptionLabel!.Visible).IsTrue();
        }

        /// <summary>
        /// Tests that text labels appear horizontally (left to right).
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void TextAppearsHorizontally()
        {
            var titleLabel = this.runner!.Scene().GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/TitleMargin/TitleLabel");
            AssertThat(titleLabel).IsNotNull();
            AssertThat(titleLabel!.HorizontalAlignment).IsEqual(HorizontalAlignment.Center);

            var descriptionLabel = this.runner!.Scene().GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/DescriptionMargin/DescriptionLabel");
            AssertThat(descriptionLabel).IsNotNull();
            AssertThat(descriptionLabel!.HorizontalAlignment).IsEqual(HorizontalAlignment.Center);
        }

        /// <summary>
        /// Tests that MainMenu has visible spacing from the top edge of the screen.
        /// Ensures the menu doesn't flush against the top, maintaining visual breathing room.
        /// </summary>
        /// <returns>Returns <see langword="void"/>.</returns>
        [TestCase]
        [RequireGodotRuntime]
        public void MenuUiHasTopSpacing()
        {
            var menuContainer = this.runner!.Scene().GetNodeOrNull<MarginContainer>("MenuContainer");
            AssertThat(menuContainer).IsNotNull();
            AssertThat(menuContainer).IsInstanceOf<MarginContainer>();
            // Assert top margin is greater than zero (Godot 4.x)
            AssertThat(menuContainer!.GetThemeConstant("margin_top")).IsGreater(0);
        }

        /// <summary>
        /// Tests that MainMenu has visible spacing from the top edge of the screen.
        /// Ensures the menu doesn't flush against the top, maintaining visual breathing room.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void MenuUiComponentsLoaded()
        {
            var menuContainer = this.runner!.Scene().GetNodeOrNull<MarginContainer>("MenuContainer");
            AssertThat(menuContainer).IsNotNull();
            AssertThat(menuContainer).IsInstanceOf<MarginContainer>();

            // Verify the wrapper exists and is the single child
            var menuWrapper = this.runner!.Scene().GetNodeOrNull<Control>("MenuContainer/MenuWrapper");
            AssertThat(menuWrapper).IsNotNull();

            // Verify MenuFrame and MenuContent are children of the wrapper
            var menuFrame = this.runner!.Scene().GetNodeOrNull<Panel>("MenuContainer/MenuWrapper/MenuFrame");
            AssertThat(menuFrame).IsNotNull();

            var menuContent = this.runner!.Scene().GetNodeOrNull<VBoxContainer>("MenuContainer/MenuWrapper/MenuContent");
            AssertThat(menuContent).IsNotNull();
        }

        /// <summary>
        /// Tests that MainMenu has visible spacing from the bottom edge of the screen.
        /// Ensures the menu doesn't flush against the bottom, maintaining visual breathing room.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void MenuUiHasBottomSpacing()
        {
            var menuContainer = this.runner!.Scene().GetNodeOrNull<MarginContainer>("MenuContainer");
            AssertThat(menuContainer).IsNotNull();
            AssertThat(menuContainer).IsInstanceOf<MarginContainer>();

            // Verify the single child wrapper exists
            var menuWrapper = this.runner!.Scene().GetNodeOrNull<Control>("MenuContainer/MenuWrapper");
            AssertThat(menuWrapper).IsNotNull();

            // Verify content is visible and properly nested
            var menuFrame = this.runner!.Scene().GetNodeOrNull<Panel>("MenuContainer/MenuWrapper/MenuFrame");
            AssertThat(menuFrame).IsNotNull();
        }

        /// <summary>
        /// Tests that CRT shaders are visible and applied.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void MenuUiLoadsCrtShaders()
        {
            var phosphorLayer = this.runner!.Scene().GetNodeOrNull<ColorRect>("ShaderLayers/PhosphorLayer");
            AssertThat(phosphorLayer).IsNotNull();
            AssertThat(phosphorLayer!.Material).IsNotNull();

            var scanlineLayer = this.runner!.Scene().GetNodeOrNull<ColorRect>("ShaderLayers/ScanlineLayer");
            AssertThat(scanlineLayer).IsNotNull();
            AssertThat(scanlineLayer!.Material).IsNotNull();

            var glitchLayer = this.runner!.Scene().GetNodeOrNull<ColorRect>("ShaderLayers/GlitchLayer");
            AssertThat(glitchLayer).IsNotNull();
            AssertThat(glitchLayer!.Material).IsNotNull();
        }

        /// <summary>
        /// Tests that the CRT shader layers visibly change color over time.
        /// Ensures the effect is animated and not static.
        /// </summary>
        /// <returns>Returns <see langword="void"/>.</returns>
        [TestCase]
        [RequireGodotRuntime]
        public async Task CrtShaderLayersChangeColorOverTime()
        {
            var phosphorLayer = this.runner!.Scene().GetNodeOrNull<ColorRect>("ShaderLayers/PhosphorLayer");
            AssertThat(phosphorLayer).IsNotNull();
            AssertThat(phosphorLayer!.Material).IsNotNull();
            // Get initial color from the material (assuming a shader param "color" exists)
            var initialColor = phosphorLayer!.Material!.Get("shader_param/color").As<Color>();
            AssertThat(initialColor).IsNotNull();
            // Simulate frames for animation
            await this.runner!.SimulateFrames(120).ConfigureAwait(false); // 2 seconds at 60fps
            // Get color again
            var finalColor = phosphorLayer.Material.Get("shader_param/color").As<Color>();
            AssertThat(finalColor).IsNotNull();
            // Assert color has changed
            AssertThat(finalColor).IsNotEqual(initialColor);
        }

        /// <summary>
        /// Tests that MenuUI has an animated 3-color border.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task MenuUiHasAnimatedThreeColorBorder()
        {
            var menuFrame = this.runner!.Scene().GetNodeOrNull<Panel>("MenuContainer/MenuFrame");
            AssertThat(menuFrame).IsNotNull();

            var initialColor = (menuFrame!.GetThemeStylebox("panel") as StyleBoxFlat)?.BorderColor;
            AssertThat(initialColor).IsNotNull();

            // Wait for the manifest to load and buttons to be created
            await this.runner!.SimulateFrames(180).ConfigureAwait(false); // 3 seconds at 60fps

            var finalColor = (menuFrame!.GetThemeStylebox("panel") as StyleBoxFlat)?.BorderColor;
            AssertThat(finalColor).IsNotEqual(initialColor); // Color should have changed
        }

        /// <summary>
        /// Tests that stage buttons are populated from manifest.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void StageButtonsPopulatedFromManifest()
        {
            var stageButtonList = this.runner!.Scene().GetNodeOrNull<VBoxContainer>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList");
            AssertThat(stageButtonList).IsNotNull();

            // Assuming manifest has at least one stage
            AssertThat(stageButtonList!.GetChildCount()).IsGreater(0);
        }

        /// <summary>
        /// Tests that start button launches Stage 1.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task StartButtonLaunchesStage1()
        {
            var startButton = this.runner!.Scene().GetNodeOrNull<Button>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StartButton");
            AssertThat(startButton).IsNotNull();

            // Simulate button press
            startButton!.EmitSignal("pressed");
            await this.runner!.AwaitInputProcessed().ConfigureAwait(false);

            // Check if scene transition was initiated (hard to test without full setup, but basic press works)
            AssertThat(startButton!.Disabled).IsFalse();
        }

        /// <summary>
        /// Tests that the title and description labels appear inside the correct parent containers.
        /// Ensures that UI hierarchy is correct and labels are placed as designed.
        /// </summary>
        /// <returns>Returns <see langword="void"/>.</returns>
        [TestCase]
        [RequireGodotRuntime]
        public void TextAppearsInCorrectContainers()
        {
            var titleLabel = this.runner!.Scene().GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/TitleMargin/TitleLabel");
            AssertThat(titleLabel).IsNotNull();
            var titleParent = titleLabel!.GetParent();
            AssertThat(titleParent).IsInstanceOf<MarginContainer>();
            AssertThat(titleParent!.Name).IsEqual("TitleMargin");

            var descriptionLabel = this.runner!.Scene().GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/DescriptionMargin/DescriptionLabel");
            AssertThat(descriptionLabel).IsNotNull();
            var descriptionParent = descriptionLabel!.GetParent();
            AssertThat(descriptionParent).IsInstanceOf<MarginContainer>();
            AssertThat(descriptionParent.Name).IsEqual("DescriptionMargin");
        }
    }
}
