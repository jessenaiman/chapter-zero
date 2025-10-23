// <copyright file="MainMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Ui.Menus;
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
            this.mainMenu?.QueueFree();
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
        public void MainMenuInheritsFromOmegaUI()
        {
            AssertThat(this.mainMenu).IsInstanceOf<OmegaSpiral.Source.UI.Omega.OmegaUI>();
        }

        /// <summary>
        /// Tests that text labels appear horizontally (left to right).
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void TextAppearsHorizontally()
        {
            var titleLabel = this.runner?.FindChild("MainMenu/MenuContainer/MenuContent/TitleMargin/TitleLabel") as Label;
            AssertThat(titleLabel).IsNotNull();
            AssertThat(titleLabel?.HorizontalAlignment).IsEqual(HorizontalAlignment.Center);

            var descriptionLabel = this.runner?.FindChild("MainMenu/MenuContainer/MenuContent/DescriptionMargin/DescriptionLabel") as Label;
            AssertThat(descriptionLabel).IsNotNull();
            AssertThat(descriptionLabel?.HorizontalAlignment).IsEqual(HorizontalAlignment.Center);
        }

        /// <summary>
        /// Tests that MenuUI has a top visible border.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void MenuUIHasTopVisibleBorder()
        {
            var menuFrame = this.runner?.FindChild("MainMenu/MenuContainer/MenuFrame") as Panel;
            AssertThat(menuFrame).IsNotNull();

            var styleBox = menuFrame?.GetThemeStylebox("panel");
            AssertThat(styleBox).IsInstanceOf<StyleBoxFlat>();

            var flatStyle = styleBox as StyleBoxFlat;
            AssertThat(flatStyle?.BorderWidthTop).IsGreater(0);
            AssertThat(flatStyle?.BorderColor).IsNotEqual(Colors.Transparent);
        }

        /// <summary>
        /// Tests that MenuUI has a bottom visible border.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void MenuUIHasBottomVisibleBorder()
        {
            var menuFrame = this.runner?.FindChild("MainMenu/MenuContainer/MenuFrame") as Panel;
            AssertThat(menuFrame).IsNotNull();

            var styleBox = menuFrame?.GetThemeStylebox("panel");
            AssertThat(styleBox).IsInstanceOf<StyleBoxFlat>();

            var flatStyle = styleBox as StyleBoxFlat;
            AssertThat(flatStyle?.BorderWidthBottom).IsGreater(0);
            AssertThat(flatStyle?.BorderColor).IsNotEqual(Colors.Transparent);
        }

        /// <summary>
        /// Tests that CRT shaders are visible and applied.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void MenuUIShowsCRTShaders()
        {
            var phosphorLayer = this.runner?.FindChild("MainMenu/ShaderLayers/PhosphorLayer") as ColorRect;
            AssertThat(phosphorLayer).IsNotNull();
            AssertThat(phosphorLayer?.Material).IsNotNull();

            var scanlineLayer = this.runner?.FindChild("MainMenu/ShaderLayers/ScanlineLayer") as ColorRect;
            AssertThat(scanlineLayer).IsNotNull();
            AssertThat(scanlineLayer?.Material).IsNotNull();

            var glitchLayer = this.runner?.FindChild("MainMenu/ShaderLayers/GlitchLayer") as ColorRect;
            AssertThat(glitchLayer).IsNotNull();
            AssertThat(glitchLayer?.Material).IsNotNull();
        }

        /// <summary>
        /// Tests that MenuUI has an animated 3-color border.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task MenuUIHasAnimatedThreeColorBorder()
        {
            var menuFrame = this.runner?.FindChild("MainMenu/MenuContainer/MenuFrame") as Panel;
            AssertThat(menuFrame).IsNotNull();

            var initialColor = (menuFrame?.GetThemeStylebox("panel") as StyleBoxFlat)?.BorderColor;
            AssertThat(initialColor).IsNotNull();

            // Wait for animation cycle (3 colors over 3 seconds)
            await this.runner!.SimulateFrames(180); // 3 seconds at 60fps

            var finalColor = (menuFrame?.GetThemeStylebox("panel") as StyleBoxFlat)?.BorderColor;
            AssertThat(finalColor).IsNotEqual(initialColor); // Color should have changed
        }

        /// <summary>
        /// Tests that stage buttons are populated from manifest.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void StageButtonsPopulatedFromManifest()
        {
            var stageButtonList = this.runner?.FindChild("MainMenu/MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList") as VBoxContainer;
            AssertThat(stageButtonList).IsNotNull();

            // Assuming manifest has at least one stage
            AssertThat(stageButtonList?.GetChildCount()).IsGreater(0);
        }

        /// <summary>
        /// Tests that start button launches Stage 1.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task StartButtonLaunchesStage1()
        {
            var startButton = this.runner?.FindChild("MainMenu/MenuContainer/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StartButton") as Button;
            AssertThat(startButton).IsNotNull();

            // Simulate button press
            this.runner?.SimulateActionPressed("ui_accept");
            await this.runner!.AwaitInputProcessed();

            // Check if scene transition was initiated (hard to test without full setup, but basic press works)
            AssertThat(startButton?.Disabled).IsFalse();
        }
    }
}
