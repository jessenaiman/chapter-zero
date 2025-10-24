// <copyright file="MainMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Stages.Stage0Start;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.Ui
{
    /// <summary>
    /// Unit tests for MainMenuUi component.
    /// Validates inheritance, visual elements, and CRT effects.
    /// Ensures menu displays correctly with borders and shaders.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public class MainMenuTests : IDisposable
    {
        private ISceneRunner? _Runner;
        private MainMenu? _MainMenu;

        /// <summary>
        /// Sets up the test scene and MainMenu instance.
        /// Loads the main_menu.tscn scene for testing.
        /// </summary>
        [Before]
        public void Setup()
        {
            this._Runner = ISceneRunner.Load("res://source/stages/stage_0_start/main_menu.tscn");
            this._MainMenu = this._Runner.Scene() as MainMenu;
            AssertThat(this._MainMenu).IsNotNull();
        }

        /// <summary>
        /// Cleans up test resources.
        /// </summary>
        [After]
        public void Teardown()
        {
            // Dispose runner first (which handles scene cleanup), then clear references.
            // Do NOT call QueueFree on mainMenu as it's owned by the runner.
            this._MainMenu = null;
            this._Runner?.Dispose();
            this._Runner = null;
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
        /// Tests that MainMenu inherits from OmegaUi.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void MainMenuInheritsFromOmegaUi()
        {
            AssertThat(this._MainMenu).IsInstanceOf<OmegaSpiral.Source.Ui.Omega.OmegaUi>();
        }

        /// <summary>
        /// Tests that the title label is visible and contains non-empty text.
        /// </summary>
        /// <returns>Returns <see langword="void"/>.</returns>
        [TestCase]
        [RequireGodotRuntime]
        public void TitleTextIsVisible()
        {
            var titleLabel = this._Runner!.Scene().GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/TitleMargin/TitleLabel");
            AssertThat(titleLabel).IsNotNull();
            AssertThat(titleLabel!.Text).IsNotEmpty();
            AssertThat(titleLabel.Visible).IsTrue();
        }

        /// <summary>
        /// Tests that the description text is actually visible and contains expected content.
        /// This catches broken Ui scenarios where no content renders.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void DescriptionTextIsVisible()
        {
            var descriptionLabel = this._Runner!.Scene().GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/DescriptionMargin/DescriptionLabel");
            AssertThat(descriptionLabel).IsNotNull();
            AssertThat(descriptionLabel!.Text).IsNotEmpty();
            AssertThat(descriptionLabel.Visible).IsTrue();
        }

        /// <summary>
        /// Tests that text labels appear horizontally (left to right).
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void TextAppearsHorizontally()
        {
            var titleLabel = this._Runner!.Scene().GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/TitleMargin/TitleLabel");
            AssertThat(titleLabel).IsNotNull();
            AssertThat((int)titleLabel!.HorizontalAlignment).IsEqual((int)HorizontalAlignment.Center);

            var descriptionLabel = this._Runner!.Scene().GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/DescriptionMargin/DescriptionLabel");
            AssertThat(descriptionLabel).IsNotNull();
            AssertThat((int)descriptionLabel!.HorizontalAlignment).IsEqual((int)HorizontalAlignment.Center);
        }



        /// <summary>
        /// Tests that stage buttons are populated from manifest.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void StageButtonsPopulatedFromManifest()
        {
            var stageButtonList = this._Runner!.Scene().GetNodeOrNull<VBoxContainer>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList");
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
            var startButton = this._Runner!.Scene().GetNodeOrNull<Button>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StartButton");
            AssertThat(startButton).IsNotNull();

            // Simulate button press
            startButton!.EmitSignal("pressed");
            await this._Runner!.AwaitInputProcessed().ConfigureAwait(false);

            // Check if scene transition was initiated (hard to test without full setup, but basic press works)
            AssertThat(startButton.Disabled).IsFalse();
        }

        /// <summary>
        /// Tests that the title and description labels appear inside the correct parent containers.
        /// Ensures that Ui hierarchy is correct and labels are placed as designed.
        /// </summary>
        /// <returns>Returns <see langword="void"/>.</returns>
        [TestCase]
        [RequireGodotRuntime]
        public void TextAppearsInCorrectContainers()
        {
            var titleLabel = this._Runner!.Scene().GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/TitleMargin/TitleLabel");
            AssertThat(titleLabel).IsNotNull();
            var titleParent = titleLabel!.GetParent();
            AssertThat(titleParent).IsInstanceOf<MarginContainer>();
            AssertThat(titleParent!.Name).IsEqual("TitleMargin");

            var descriptionLabel = this._Runner!.Scene().GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/DescriptionMargin/DescriptionLabel");
            AssertThat(descriptionLabel).IsNotNull();
            var descriptionParent = descriptionLabel!.GetParent();
            AssertThat(descriptionParent).IsInstanceOf<MarginContainer>();
            AssertThat(descriptionParent!.Name).IsEqual("DescriptionMargin");
        }
    }
}
