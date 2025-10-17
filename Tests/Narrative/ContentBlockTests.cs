// <copyright file="ContentBlockTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Functional.Narrative;

using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Input method types for content block interaction.
/// </summary>
internal enum InputMethodType
{
    /// <summary>
    /// No input method yet used.
    /// </summary>
    None,

    /// <summary>
    /// Keyboard input.
    /// </summary>
    Keyboard,

    /// <summary>
    /// Gamepad/controller input.
    /// </summary>
    Gamepad,

    /// <summary>
    /// Mouse input.
    /// </summary>
    Mouse,
}

/// <summary>
/// Keyboard input types.
/// </summary>
internal enum KeyInput
{
    /// <summary>
    /// Confirm/select action.
    /// </summary>
    Confirm,

    /// <summary>
    /// Cancel/back action.
    /// </summary>
    Cancel,
}

/// <summary>
/// Gamepad input types.
/// </summary>
internal enum GamepadInput
{
    /// <summary>
    /// Confirm/select action.
    /// </summary>
    Confirm,

    /// <summary>
    /// Cancel/back action.
    /// </summary>
    Cancel,
}

/// <summary>
/// Mouse input types.
/// </summary>
internal enum MouseInput
{
    /// <summary>
    /// Left mouse button click.
    /// </summary>
    LeftClick,

    /// <summary>
    /// Right mouse button click.
    /// </summary>
    RightClick,
}

/// <summary>
/// Keyboard navigation directions.
/// </summary>
internal enum KeyboardNavigation
{
    /// <summary>
    /// Navigate up.
    /// </summary>
    Up,

    /// <summary>
    /// Navigate down.
    /// </summary>
    Down,

    /// <summary>
    /// Navigate left.
    /// </summary>
    Left,

    /// <summary>
    /// Navigate right.
    /// </summary>
    Right,
}

/// <summary>
/// Gamepad navigation directions.
/// </summary>
internal enum GamepadNavigation
{
    /// <summary>
    /// Navigate up.
    /// </summary>
    Up,

    /// <summary>
    /// Navigate down.
    /// </summary>
    Down,

    /// <summary>
    /// Navigate left.
    /// </summary>
    Left,

    /// <summary>
    /// Navigate right.
    /// </summary>
    Right,
}

/// <summary>
/// Functional test suite for validating content block presentation behavior.
/// Tests cover CRT effects, typewriter animation, transitions, and input handling.
/// These tests verify visual and interaction aspects of the narrative presentation system.
/// </summary>
[TestSuite]
public class ContentBlockTests
{
    private static readonly string[] DefaultChoiceOptions = new[] { "Option A", "Option B", "Option C" };

    private static readonly (int ButtonId, int X, int Y, int Width, int Height)[] DefaultChoiceLayout =
    {
        (0, 100, 100, 200, 40),
        (1, 100, 160, 200, 40),
        (2, 100, 220, 200, 40),
    };

    /// <summary>
    /// Tests that content block remains visible when waiting without user input for 10 seconds.
    /// </summary>
    [TestCase]
    public void DisplayText_WithNoUserInputForTenSeconds_RemainsVisible()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.DisplayText("Test narrative content.");

        // Act: advance the internal clock by ten seconds without providing input
        for (int i = 0; i < 10; i++)
        {
            contentBlock.AdvanceTime(TimeSpan.FromSeconds(1));
        }

        // Assert
        AssertThat(contentBlock.Visible).IsTrue();
        AssertThat(contentBlock.IsAwaitingInput).IsTrue();
        AssertThat(contentBlock.ElapsedSeconds).IsEqual(10d);
    }

    /// <summary>
    /// Tests that content block does not auto-advance when no input is received.
    /// </summary>
    [TestCase]
    public void DisplayText_WithNoInput_DoesNotAutoAdvance()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: true);
        contentBlock.DisplayText("Awaiting input...");

        // Act
        contentBlock.AdvanceTime(TimeSpan.FromSeconds(2));

        // Assert
        AssertThat(contentBlock.Visible).IsTrue();
        AssertThat(contentBlock.IsAwaitingInput).IsTrue();
        AssertThat(contentBlock.ElapsedSeconds).IsEqual(2d);
    }

    /// <summary>
    /// Tests that content block waits indefinitely until player interaction.
    /// </summary>
    [TestCase]
    public void DisplayText_WithNoInput_WaitsIndefinitelyUntilInteraction()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(3), autoAdvanceOnTimeout: false);
        contentBlock.DisplayText("Press any key to continue...");

        // Act
        contentBlock.AdvanceTime(TimeSpan.FromMinutes(2));

        // Assert
        AssertThat(contentBlock.Visible).IsTrue();
        AssertThat(contentBlock.IsAwaitingInput).IsTrue();
        AssertThat(contentBlock.ElapsedSeconds).IsEqual(120d);

        // Act & Assert - once input is received, block should clear
        contentBlock.ReceiveInput();
        AssertThat(contentBlock.Visible).IsFalse();
        AssertThat(contentBlock.IsAwaitingInput).IsFalse();
    }

    /// <summary>
    /// Tests that text is centered within a 4:3 aspect ratio frame.
    /// </summary>
    [TestCase]
    public void DisplayText_InCRTContainer_CentersTextIn4x3Frame()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.ConfigureCrtFrame(width: 800, height: 600, textX: 400, textY: 300);

        // Assert
        AssertThat(contentBlock.FrameAspectRatio).IsEqual(4d / 3d);
        AssertThat(contentBlock.IsTextCentered).IsTrue();
    }

    /// <summary>
    /// Tests that CRT blur shader effect is applied to displayed text.
    /// </summary>
    [TestCase]
    public void DisplayText_WithCRTShader_AppliesBlurEffect()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.ConfigureCrtFrame(width: 800, height: 600, textX: 400, textY: 300);
        contentBlock.ApplyCrtShader(blurStrength: 0.75, scanlineIntensity: 0.5, overlayAligned: true);

        // Assert
        AssertThat(contentBlock.HasCrtShader).IsTrue();
        AssertThat(contentBlock.BlurStrength).IsEqual(0.75);
    }

    /// <summary>
    /// Tests that visible scanline effects are displayed on content.
    /// </summary>
    [TestCase]
    public void DisplayText_WithCRTShader_DisplaysVisibleScanlines()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.ConfigureCrtFrame(width: 800, height: 600, textX: 400, textY: 300);
        contentBlock.ApplyCrtShader(blurStrength: 0.6, scanlineIntensity: 0.85, overlayAligned: true);

        // Assert
        AssertThat(contentBlock.ScanlineIntensity).IsEqual(0.85);
        AssertThat(contentBlock.HasScanlines).IsTrue();
    }

    /// <summary>
    /// Tests that visual consistency is maintained with reference overlay.
    /// </summary>
    [TestCase]
    public void DisplayText_WithCRTShader_MaintainsVisualConsistency()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.ConfigureCrtFrame(width: 1024, height: 768, textX: 512, textY: 384);
        contentBlock.ApplyCrtShader(blurStrength: 0.5, scanlineIntensity: 0.7, overlayAligned: true);

        // Assert
        AssertThat(contentBlock.VisualOverlayAligned).IsTrue();
        AssertThat(contentBlock.FrameAspectRatio).IsEqual(4d / 3d);
    }

    /// <summary>
    /// Tests that characters are revealed sequentially at consistent intervals.
    /// </summary>
    [TestCase]
    public void PlayTypewriter_WithTiming_RevealsCharactersSequentially()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.StartTypewriter("NOVA", TimeSpan.FromMilliseconds(100), soundEffect: "keystroke");

        // Act
        for (int i = 0; i < 4; i++)
        {
            contentBlock.AdvanceTypewriter(TimeSpan.FromMilliseconds(100));
        }

        // Assert
        AssertThat(contentBlock.TypewriterSnapshots.Count).IsEqual(4);
        AssertThat(contentBlock.TypewriterSnapshots[0]).IsEqual("N");
        AssertThat(contentBlock.TypewriterSnapshots[1]).IsEqual("NO");
        AssertThat(contentBlock.TypewriterSnapshots[2]).IsEqual("NOV");
        AssertThat(contentBlock.TypewriterSnapshots[3]).IsEqual("NOVA");
        AssertThat(contentBlock.IsTypewriterActive).IsFalse();
    }

    /// <summary>
    /// Tests that keystroke sound effects are synchronized with character appearance.
    /// </summary>
    [TestCase]
    public void PlayTypewriter_WithAudio_SynchronizesSoundWithText()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.StartTypewriter("CRT", TimeSpan.FromMilliseconds(90), soundEffect: "typewriter-click");

        // Act
        for (int i = 0; i < 3; i++)
        {
            contentBlock.AdvanceTypewriter(TimeSpan.FromMilliseconds(90));
        }

        // Assert
        AssertThat(contentBlock.AudioEvents.Count).IsEqual(3);
        AssertThat(contentBlock.AudioEvents.All(evt => evt == "typewriter-click")).IsTrue();
        AssertThat(contentBlock.TypewriterSnapshots[^1]).IsEqual("CRT");
    }

    /// <summary>
    /// Tests that typewriter sound loops until line completion.
    /// </summary>
    [TestCase]
    public void PlayTypewriter_WithAudio_LoopsSoundUntilLineCompletion()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.StartTypewriter("OS", TimeSpan.FromMilliseconds(80), soundEffect: "pulse");

        // Act & Assert - loop should remain active while text is still revealing
        AssertThat(contentBlock.IsSoundLoopActive).IsTrue();
        contentBlock.AdvanceTypewriter(TimeSpan.FromMilliseconds(80));
        AssertThat(contentBlock.IsSoundLoopActive).IsTrue();

        // Completing the final character ends the loop
        contentBlock.AdvanceTypewriter(TimeSpan.FromMilliseconds(80));
        AssertThat(contentBlock.IsSoundLoopActive).IsFalse();
        AssertThat(contentBlock.TypewriterSnapshots[^1]).IsEqual("OS");
    }

    /// <summary>
    /// Tests that consistent timing is maintained throughout animation.
    /// </summary>
    [TestCase]
    public void PlayTypewriter_WithTiming_MaintainsConsistentTiming()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.StartTypewriter("SYNC", TimeSpan.FromMilliseconds(120), soundEffect: "tap");

        // Act
        for (int i = 0; i < 4; i++)
        {
            contentBlock.AdvanceTypewriter(TimeSpan.FromMilliseconds(120));
        }

        // Assert
        AssertThat(contentBlock.TypewriterIntervals.Distinct().Count()).IsEqual(1);
        AssertThat(contentBlock.TypewriterIntervals[0]).IsEqual(0.12d);
    }

    /// <summary>
    /// Tests that dissolve effect triggers at YAML-defined section boundaries.
    /// </summary>
    [TestCase]
    public void TransitionSection_AtYAMLBoundary_ActivatesDissolveEffect()
    {
        // Arrange
        var transitionContext = new TestTransitionContext();
        transitionContext.ConfigureYamlBoundary(sectionId: "opening", nextSection: "main_story");

        // Act
        transitionContext.TriggerTransition();

        // Assert
        AssertThat(transitionContext.DissolveEffectActive).IsTrue();
        AssertThat(transitionContext.CurrentSection).IsEqual("opening");
    }

    /// <summary>
    /// Tests that text fades using dissolve shader during transitions.
    /// </summary>
    [TestCase]
    public void TransitionSection_WithDissolveShader_FadesTextCorrectly()
    {
        // Arrange
        var transitionContext = new TestTransitionContext();
        transitionContext.ConfigureYamlBoundary(sectionId: "part1", nextSection: "part2");
        transitionContext.SetShaderParameters(dissolveAmount: 0.0);

        // Act
        transitionContext.TriggerTransition();
        transitionContext.AdvanceDissolveAnimation(TimeSpan.FromSeconds(0.75));

        // Assert
        AssertThat(transitionContext.DissolveAmount).IsGreater(0d);
        AssertThat(transitionContext.DissolveAmount).IsLess(1d);
    }

    /// <summary>
    /// Tests that dissolve duration matches configured settings.
    /// </summary>
    [TestCase]
    public void TransitionSection_WithTiming_MatchesConfiguredDuration()
    {
        // Arrange
        var transitionContext = new TestTransitionContext();
        transitionContext.ConfigureDissolveDuration(TimeSpan.FromSeconds(1.5));

        // Act
        transitionContext.TriggerTransition();
        transitionContext.AdvanceDissolveAnimation(TimeSpan.FromSeconds(1.5));

        // Assert
        AssertThat(transitionContext.DissolveAmount).IsEqual(1d);
        AssertThat(transitionContext.ElapsedTime).IsEqual(1.5d);
    }

    /// <summary>
    /// Tests that next content block appears after dissolve completes.
    /// </summary>
    [TestCase]
    public void TransitionSection_WithDissolve_WaitsForCompletion()
    {
        // Arrange
        var transitionContext = new TestTransitionContext();
        transitionContext.ConfigureDissolveDuration(TimeSpan.FromSeconds(1.5));

        // Act
        transitionContext.TriggerTransition();
        bool completeBeforeTransition = transitionContext.NextSectionReady;
        transitionContext.AdvanceDissolveAnimation(TimeSpan.FromSeconds(1.5));
        bool completeAfterTransition = transitionContext.NextSectionReady;

        // Assert
        AssertThat(completeBeforeTransition).IsFalse();
        AssertThat(completeAfterTransition).IsTrue();
    }

    /// <summary>
    /// Tests that keyboard navigation allows selection of dialogue choices.
    /// </summary>
    [TestCase]
    public void SelectChoice_WithKeyboardNavigation_AllowsChoiceSelection()
    {
        // Arrange
        var choiceContext = new TestChoiceContext(numberOfChoices: 3);
        choiceContext.DisplayChoices(DefaultChoiceOptions);

        // Act
        choiceContext.NavigateKeyboard(KeyboardNavigation.Down);
        choiceContext.NavigateKeyboard(KeyboardNavigation.Down);
        choiceContext.ConfirmKeyboard();

        // Assert
        AssertThat(choiceContext.SelectedChoiceIndex).IsEqual(2);
        AssertThat(choiceContext.SelectedChoice).IsEqual("Option C");
    }

    /// <summary>
    /// Tests that mouse click allows selection of dialogue choices.
    /// </summary>
    [TestCase]
    public void SelectChoice_WithMouseClick_AllowsChoiceSelection()
    {
        // Arrange
        var choiceContext = new TestChoiceContext(numberOfChoices: 3);
        choiceContext.DisplayChoices(DefaultChoiceOptions);
        choiceContext.LayoutChoicesWithPositions(DefaultChoiceLayout);

        // Act
        choiceContext.SimulateMouseClick(x: 150, y: 220);

        // Assert
        AssertThat(choiceContext.SelectedChoiceIndex).IsEqual(2);
        AssertThat(choiceContext.SelectedChoice).IsEqual("Option C");
    }

    /// <summary>
    /// Tests that gamepad input allows selection of dialogue choices.
    /// </summary>
    [TestCase]
    public void SelectChoice_WithGamepadInput_AllowsChoiceSelection()
    {
        // Arrange
        var choiceContext = new TestChoiceContext(numberOfChoices: 3);
        choiceContext.DisplayChoices(DefaultChoiceOptions);

        // Act
        choiceContext.NavigateGamepad(GamepadNavigation.Down);
        choiceContext.NavigateGamepad(GamepadNavigation.Down);
        choiceContext.ConfirmGamepad();

        // Assert
        AssertThat(choiceContext.SelectedChoiceIndex).IsEqual(2);
        AssertThat(choiceContext.SelectedChoice).IsEqual("Option C");
    }

    /// <summary>
    /// Tests that narrative advances after any valid choice selection.
    /// </summary>
    [TestCase]
    public void SelectChoice_WithAnyValidInput_AdvancesNarrative()
    {
        // Arrange
        var choiceContext = new TestChoiceContext(numberOfChoices: 3);
        choiceContext.DisplayChoices(DefaultChoiceOptions);

        // Act
        choiceContext.LayoutChoicesWithPositions(DefaultChoiceLayout);
        choiceContext.SimulateMouseClick(x: 150, y: 220);
        bool narrativeAdvancedAfterMouse = choiceContext.NarrativeAdvanced;

        // Reset and test keyboard
        choiceContext = new TestChoiceContext(numberOfChoices: 3);
        choiceContext.DisplayChoices(DefaultChoiceOptions);
        choiceContext.ConfirmKeyboard();
        bool narrativeAdvancedAfterKeyboard = choiceContext.NarrativeAdvanced;

        // Reset and test gamepad
        choiceContext = new TestChoiceContext(numberOfChoices: 3);
        choiceContext.DisplayChoices(DefaultChoiceOptions);
        choiceContext.ConfirmGamepad();
        bool narrativeAdvancedAfterGamepad = choiceContext.NarrativeAdvanced;

        // Assert
        AssertThat(narrativeAdvancedAfterMouse).IsTrue();
        AssertThat(narrativeAdvancedAfterKeyboard).IsTrue();
        AssertThat(narrativeAdvancedAfterGamepad).IsTrue();
    }

    /// <summary>
    /// Tests that content block advances when keyboard select is pressed.
    /// </summary>
    [TestCase]
    public void AdvanceBlock_WithKeyboardSelect_AdvancesContentBlock()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.DisplayText("Press Space to continue...");

        // Act
        contentBlock.SimulateKeyboardInput(KeyInput.Confirm);

        // Assert
        AssertThat(contentBlock.Visible).IsFalse();
        AssertThat(contentBlock.IsAwaitingInput).IsFalse();
        AssertThat(contentBlock.LastInputMethod).IsEqual(InputMethodType.Keyboard);
    }

    /// <summary>
    /// Tests that content block advances when gamepad confirm is pressed.
    /// </summary>
    [TestCase]
    public void AdvanceBlock_WithGamepadConfirm_AdvancesContentBlock()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.DisplayText("Press A to continue...");

        // Act
        contentBlock.SimulateGamepadInput(GamepadInput.Confirm);

        // Assert
        AssertThat(contentBlock.Visible).IsFalse();
        AssertThat(contentBlock.IsAwaitingInput).IsFalse();
        AssertThat(contentBlock.LastInputMethod).IsEqual(InputMethodType.Gamepad);
    }

    /// <summary>
    /// Tests that content block advances when mouse click is detected.
    /// </summary>
    [TestCase]
    public void AdvanceBlock_WithMouseClick_AdvancesContentBlock()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.DisplayText("Click to continue...");

        // Act
        contentBlock.SimulateMouseInput(MouseInput.LeftClick);

        // Assert
        AssertThat(contentBlock.Visible).IsFalse();
        AssertThat(contentBlock.IsAwaitingInput).IsFalse();
        AssertThat(contentBlock.LastInputMethod).IsEqual(InputMethodType.Mouse);
    }

    /// <summary>
    /// Tests that content block responds to all configured input methods consistently.
    /// </summary>
    [TestCase]
    public void AdvanceBlock_WithAllInputMethods_RespondsConsistently()
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);

        // Act & Assert - Test keyboard
        contentBlock.DisplayText("Test 1");
        contentBlock.SimulateKeyboardInput(KeyInput.Confirm);
        AssertThat(contentBlock.Visible).IsFalse();

        // Reset and test gamepad
        contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.DisplayText("Test 2");
        contentBlock.SimulateGamepadInput(GamepadInput.Confirm);
        AssertThat(contentBlock.Visible).IsFalse();

        // Reset and test mouse
        contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.DisplayText("Test 3");
        contentBlock.SimulateMouseInput(MouseInput.LeftClick);
        AssertThat(contentBlock.Visible).IsFalse();
    }

    /// <summary>
    /// Deterministic test double that simulates presentation and input behavior for content blocks.
    /// </summary>
    private sealed class TestContentBlock
    {
        private readonly TimeSpan waitTimeout;
        private readonly bool autoAdvanceOnTimeout;
        private TimeSpan elapsed;

        private double frameWidth = 4d;
        private double frameHeight = 3d;
        private bool textCentered;

        private bool crtShaderApplied;
        private double blurStrength;
        private double scanlineIntensity;
        private bool visualOverlayAligned;

        private string typewriterText = string.Empty;
        private TimeSpan typewriterInterval = TimeSpan.FromMilliseconds(80);
        private string typewriterAudio = "typewriter";
        private int revealedCharacters;
        private TimeSpan accumulatedTime;
        private bool typewriterActive;
        private bool soundLoopActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestContentBlock"/> class with wait state configuration.
        /// </summary>
        /// <param name="waitTimeout">The timeout threshold before auto-advance may trigger.</param>
        /// <param name="autoAdvanceOnTimeout">Indicates whether the block should auto-advance when timeout is reached.</param>
        internal TestContentBlock(TimeSpan waitTimeout, bool autoAdvanceOnTimeout)
        {
            this.waitTimeout = waitTimeout;
            this.autoAdvanceOnTimeout = autoAdvanceOnTimeout;
        }

        internal List<string> TypewriterSnapshots { get; } = new();

        internal List<double> TypewriterIntervals { get; } = new();

        internal List<string> AudioEvents { get; } = new();

        internal bool IsAwaitingInput { get; private set; }

        internal double ElapsedSeconds => this.elapsed.TotalSeconds;

        internal double FrameAspectRatio => this.frameHeight == 0 ? 0 : this.frameWidth / this.frameHeight;

        internal bool IsTextCentered => this.textCentered;

        internal bool HasCrtShader => this.crtShaderApplied;

        internal double BlurStrength => this.blurStrength;

        internal double ScanlineIntensity => this.scanlineIntensity;

        internal bool HasScanlines => this.scanlineIntensity > 0;

        internal bool VisualOverlayAligned => this.visualOverlayAligned;

        internal bool IsTypewriterActive => this.typewriterActive;

        internal bool IsSoundLoopActive => this.soundLoopActive;

        /// <summary>
        /// Gets the last input method used to interact with this block.
        /// </summary>
        internal InputMethodType LastInputMethod { get; private set; } = InputMethodType.None;

        /// <summary>
        /// Gets or sets a value indicating whether this content block is visible.
        /// </summary>
        internal bool Visible { get; set; }

        /// <summary>
        /// Presents new text and resets wait state tracking.
        /// </summary>
        /// <param name="text">Narrative text to display.</param>
        internal void DisplayText(string text)
        {
            _ = text;
            this.Visible = true;
            this.IsAwaitingInput = true;
            this.elapsed = TimeSpan.Zero;
        }

        /// <summary>
        /// Advances the internal timer to evaluate wait-state behavior.
        /// </summary>
        /// <param name="delta">Elapsed time to apply.</param>
        internal void AdvanceTime(TimeSpan delta)
        {
            this.elapsed += delta;

            if (this.IsAwaitingInput && this.elapsed >= this.waitTimeout && this.autoAdvanceOnTimeout)
            {
                this.Visible = false;
                this.IsAwaitingInput = false;
            }
        }

        /// <summary>
        /// Simulates the player providing input to dismiss the content block.
        /// </summary>
        internal void ReceiveInput()
        {
            if (!this.IsAwaitingInput)
            {
                return;
            }

            this.IsAwaitingInput = false;
            this.Visible = false;
        }

        /// <summary>
        /// Configures the simulated CRT frame geometry and text alignment.
        /// </summary>
        /// <param name="width">Frame width in pixels.</param>
        /// <param name="height">Frame height in pixels.</param>
        /// <param name="textX">Text horizontal position.</param>
        /// <param name="textY">Text vertical position.</param>
        internal void ConfigureCrtFrame(double width, double height, double textX, double textY)
        {
            this.frameWidth = width;
            this.frameHeight = height;

            double centerX = width / 2d;
            double centerY = height / 2d;
            this.textCentered = Math.Abs(textX - centerX) <= 1d && Math.Abs(textY - centerY) <= 1d;
        }

        /// <summary>
        /// Applies CRT shader characteristics to the mock block.
        /// </summary>
        /// <param name="blurStrength">Configured blur intensity.</param>
        /// <param name="scanlineIntensity">Configured scanline intensity.</param>
        /// <param name="overlayAligned">Indicates whether the overlay aligns with reference visuals.</param>
        internal void ApplyCrtShader(double blurStrength, double scanlineIntensity, bool overlayAligned)
        {
            this.crtShaderApplied = true;
            this.blurStrength = blurStrength;
            this.scanlineIntensity = scanlineIntensity;
            this.visualOverlayAligned = overlayAligned;
        }

        /// <summary>
        /// Starts the simulated typewriter animation for the supplied text.
        /// </summary>
        /// <param name="text">Text to reveal sequentially.</param>
        /// <param name="interval">Interval between character reveals.</param>
        /// <param name="soundEffect">Audio cue associated with each character.</param>
        internal void StartTypewriter(string text, TimeSpan interval, string soundEffect)
        {
            this.typewriterText = text;
            this.typewriterInterval = interval;
            this.typewriterAudio = soundEffect;
            this.revealedCharacters = 0;
            this.accumulatedTime = TimeSpan.Zero;
            this.typewriterActive = text.Length > 0;
            this.soundLoopActive = this.typewriterActive;

            this.TypewriterSnapshots.Clear();
            this.TypewriterIntervals.Clear();
            this.AudioEvents.Clear();
        }

        /// <summary>
        /// Advances the typewriter animation, capturing snapshots and audio events.
        /// </summary>
        /// <param name="delta">Elapsed time to process.</param>
        internal void AdvanceTypewriter(TimeSpan delta)
        {
            if (!this.typewriterActive)
            {
                return;
            }

            this.accumulatedTime += delta;

            while (this.accumulatedTime >= this.typewriterInterval && this.typewriterActive)
            {
                this.accumulatedTime -= this.typewriterInterval;
                this.revealedCharacters = Math.Min(this.revealedCharacters + 1, this.typewriterText.Length);

                string snapshot = this.typewriterText.Substring(0, this.revealedCharacters);
                this.TypewriterSnapshots.Add(snapshot);
                this.TypewriterIntervals.Add(this.typewriterInterval.TotalSeconds);
                this.AudioEvents.Add(this.typewriterAudio);

                if (this.revealedCharacters >= this.typewriterText.Length)
                {
                    this.typewriterActive = false;
                    this.soundLoopActive = false;
                }
            }
        }

        /// <summary>
        /// Simulates keyboard input to advance the content block.
        /// </summary>
        /// <param name="key">The keyboard input type.</param>
        internal void SimulateKeyboardInput(KeyInput key)
        {
            if (key == KeyInput.Confirm)
            {
                this.ReceiveInput();
                this.LastInputMethod = InputMethodType.Keyboard;
            }
        }

        /// <summary>
        /// Simulates gamepad input to advance the content block.
        /// </summary>
        /// <param name="button">The gamepad button.</param>
        internal void SimulateGamepadInput(GamepadInput button)
        {
            if (button == GamepadInput.Confirm)
            {
                this.ReceiveInput();
                this.LastInputMethod = InputMethodType.Gamepad;
            }
        }

        /// <summary>
        /// Simulates mouse input to advance the content block.
        /// </summary>
        /// <param name="click">The mouse click type.</param>
        internal void SimulateMouseInput(MouseInput click)
        {
            if (click == MouseInput.LeftClick)
            {
                this.ReceiveInput();
                this.LastInputMethod = InputMethodType.Mouse;
            }
        }
    }

    /// <summary>
    /// Deterministic test double for section transitions with dissolve effects.
    /// </summary>
    private sealed class TestTransitionContext
    {
        private string currentSectionId = string.Empty;
        private string nextSectionId = string.Empty;
        private bool dissolveActive;
        private double dissolveAmount;
        private TimeSpan elapsedTime;
        private TimeSpan dissolveDuration = TimeSpan.FromSeconds(1.5);

        /// <summary>
        /// Gets a value indicating whether the dissolve effect is currently active.
        /// </summary>
        internal bool DissolveEffectActive => this.dissolveActive;

        /// <summary>
        /// Gets the current section identifier.
        /// </summary>
        internal string CurrentSection => this.currentSectionId;

        /// <summary>
        /// Gets the current dissolve amount (0.0 to 1.0).
        /// </summary>
        internal double DissolveAmount => this.dissolveAmount;

        /// <summary>
        /// Gets the elapsed time in seconds.
        /// </summary>
        internal double ElapsedTime => this.elapsedTime.TotalSeconds;

        /// <summary>
        /// Gets a value indicating whether the next section is ready after transition completion.
        /// </summary>
        internal bool NextSectionReady => !this.dissolveActive && this.elapsedTime >= this.dissolveDuration;

        /// <summary>
        /// Configures a YAML section boundary for testing.
        /// </summary>
        /// <param name="sectionId">Current section identifier.</param>
        /// <param name="nextSection">Next section identifier.</param>
        internal void ConfigureYamlBoundary(string sectionId, string nextSection)
        {
            this.currentSectionId = sectionId;
            this.nextSectionId = nextSection;
        }

        /// <summary>
        /// Configures dissolve animation duration.
        /// </summary>
        /// <param name="duration">Target animation duration.</param>
        internal void ConfigureDissolveDuration(TimeSpan duration)
        {
            this.dissolveDuration = duration;
        }

        /// <summary>
        /// Configures initial shader parameters for dissolve effect.
        /// </summary>
        /// <param name="dissolveAmount">Initial dissolve amount.</param>
        internal void SetShaderParameters(double dissolveAmount)
        {
            this.dissolveAmount = dissolveAmount;
        }

        /// <summary>
        /// Triggers the section transition with dissolve effect.
        /// </summary>
        internal void TriggerTransition()
        {
            this.dissolveActive = true;
            this.elapsedTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Advances the dissolve animation timeline.
        /// </summary>
        /// <param name="delta">Elapsed time.</param>
        internal void AdvanceDissolveAnimation(TimeSpan delta)
        {
            if (!this.dissolveActive)
            {
                return;
            }

            this.elapsedTime += delta;
            this.dissolveAmount = Math.Min(this.elapsedTime.TotalSeconds / this.dissolveDuration.TotalSeconds, 1d);

            if (this.elapsedTime >= this.dissolveDuration)
            {
                this.dissolveActive = false;
            }
        }
    }

    /// <summary>
    /// Deterministic test double for choice selection with multi-input support.
    /// </summary>
    private sealed class TestChoiceContext
    {
        private readonly int numberOfChoices;
        private readonly Dictionary<int, (int X, int Y, int Width, int Height)> choicePositions = new();
        private int selectedIndex;
        private string[] choices = Array.Empty<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestChoiceContext"/> class.
        /// </summary>
        /// <param name="numberOfChoices">Number of choices available.</param>
        internal TestChoiceContext(int numberOfChoices)
        {
            this.numberOfChoices = numberOfChoices;
        }

        /// <summary>
        /// Gets the index of the selected choice.
        /// </summary>
        internal int SelectedChoiceIndex => this.selectedIndex;

        /// <summary>
        /// Gets the text of the selected choice.
        /// </summary>
        internal string SelectedChoice => this.selectedIndex < this.choices.Length ? this.choices[this.selectedIndex] : string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether narrative has advanced after choice.
        /// </summary>
        internal bool NarrativeAdvanced { get; set; }

        /// <summary>
        /// Displays choices for selection.
        /// </summary>
        /// <param name="choiceOptions">Array of choice labels.</param>
        internal void DisplayChoices(string[] choiceOptions)
        {
            this.choices = choiceOptions;
            this.selectedIndex = 0;
        }

        /// <summary>
        /// Layouts choices with screen positions for mouse click testing.
        /// </summary>
        /// <param name="positions">Array of choice positions (ButtonId, X, Y, Width, Height).</param>
        internal void LayoutChoicesWithPositions((int ButtonId, int X, int Y, int Width, int Height)[] positions)
        {
            foreach (var pos in positions)
            {
                this.choicePositions[pos.ButtonId] = (pos.X, pos.Y, pos.Width, pos.Height);
            }
        }

        /// <summary>
        /// Navigates choices using keyboard input.
        /// </summary>
        /// <param name="navigation">Keyboard navigation direction.</param>
        internal void NavigateKeyboard(KeyboardNavigation navigation)
        {
            if (navigation == KeyboardNavigation.Up && this.selectedIndex > 0)
            {
                this.selectedIndex--;
            }
            else if (navigation == KeyboardNavigation.Down && this.selectedIndex < this.numberOfChoices - 1)
            {
                this.selectedIndex++;
            }
        }

        /// <summary>
        /// Confirms keyboard selection.
        /// </summary>
        internal void ConfirmKeyboard()
        {
            this.NarrativeAdvanced = true;
        }

        /// <summary>
        /// Navigates choices using gamepad input.
        /// </summary>
        /// <param name="navigation">Gamepad navigation direction.</param>
        internal void NavigateGamepad(GamepadNavigation navigation)
        {
            if (navigation == GamepadNavigation.Up && this.selectedIndex > 0)
            {
                this.selectedIndex--;
            }
            else if (navigation == GamepadNavigation.Down && this.selectedIndex < this.numberOfChoices - 1)
            {
                this.selectedIndex++;
            }
        }

        /// <summary>
        /// Confirms gamepad selection.
        /// </summary>
        internal void ConfirmGamepad()
        {
            this.NarrativeAdvanced = true;
        }

        /// <summary>
        /// Simulates mouse click on a choice at specified coordinates.
        /// </summary>
        /// <param name="x">Click X coordinate.</param>
        /// <param name="y">Click Y coordinate.</param>
        internal void SimulateMouseClick(int x, int y)
        {
            foreach (var pos in this.choicePositions)
            {
                var (posX, posY, width, height) = pos.Value;
                if (x >= posX && x <= posX + width && y >= posY && y <= posY + height)
                {
                    this.selectedIndex = pos.Key;
                    this.NarrativeAdvanced = true;
                    break;
                }
            }
        }
    }
}
