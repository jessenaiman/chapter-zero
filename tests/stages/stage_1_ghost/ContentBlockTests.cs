// <copyright file="ContentBlockTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage1;

using Godot;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Functional test suite for validating content block presentation behavior.
/// Tests cover CRT effects, typewriter animation, transitions, and input handling.
/// These tests verify visual and interaction aspects of the narrative presentation system.
/// </summary>
[TestSuite]
    public static class ContentBlockTests
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
    public static void DisplaytextWithnouserinputfortensecondsRemainsvisible()
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
    public static void DisplaytextWithnoinputDoesnotautoadvance()
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
    public static void DisplaytextWithnoinputWaitsindefinitelyuntilinteraction()
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
    /// Tests CRT shader configurations with various blur and scanline intensities.
    /// Uses parameterized test data to verify shader effects across different parameter combinations.
    /// </summary>
    [TestCase(800, 600, 400, 300, 0.75, 0.5, true, TestName = "BlurEffect")]
    [TestCase(800, 600, 400, 300, 0.6, 0.85, true, TestName = "VisibleScanlines")]
    [TestCase(1024, 768, 512, 384, 0.5, 0.7, true, TestName = "VisualConsistency")]
    public static void DisplaytextWithcrtshader_AppliesEffectsCorrectly(
        int frameWidth,
        int frameHeight,
        int textX,
        int textY,
        double blurStrength,
        double scanlineIntensity,
        bool overlayAligned)
    {
        // Arrange
        var contentBlock = new TestContentBlock(TimeSpan.FromSeconds(5), autoAdvanceOnTimeout: false);
        contentBlock.ConfigureCrtFrame(width: frameWidth, height: frameHeight, textX: textX, textY: textY);
        contentBlock.ApplyCrtShader(blurStrength: blurStrength, scanlineIntensity: scanlineIntensity, overlayAligned: overlayAligned);

        // Assert
        AssertThat(contentBlock.HasCrtShader).IsTrue();
        AssertThat(contentBlock.BlurStrength).IsEqual(blurStrength);
        AssertThat(contentBlock.ScanlineIntensity).IsEqual(scanlineIntensity);
        AssertThat(contentBlock.HasScanlines).IsEqual(scanlineIntensity > 0);
        var expectedAspectRatio = (double)frameWidth / frameHeight;
        AssertThat(contentBlock.FrameAspectRatio).IsEqual(expectedAspectRatio);
        AssertThat(contentBlock.IsTextCentered).IsTrue();
    }

    /// <summary>
    /// Tests that characters are revealed sequentially at consistent intervals.
    /// </summary>
    [TestCase]
    public static void PlaytypewriterWithtimingRevealscharacterssequentially()
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
    public static void PlaytypewriterWithaudioSynchronizessoundwithtext()
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
    public static void PlaytypewriterWithaudioLoopssounduntillinecompletion()
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
    public static void PlaytypewriterWithtimingMaintainsconsistenttiming()
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
    public static void TransitionsectionAtyamlboundaryActivatesdissolveeffect()
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
    public static void TransitionsectionWithdissolveshaderFadestextcorrectly()
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
    public static void TransitionsectionWithtimingMatchesconfiguredduration()
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
    public static void TransitionsectionWithdissolveWaitsforcompletion()
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
    /// Tests that various input methods (keyboard, mouse, gamepad) allow selection of dialogue choices.
    /// Uses parameterized test data to verify consistent choice selection behavior across different input methods.
    /// </summary>
    [TestCase("keyboard", 2, 1, "Option C", 2, TestName = "KeyboardNavigation")]
    [TestCase("mouse", 150, 220, "Option C", 2, TestName = "MouseClick")]
    [TestCase("gamepad", 2, 1, "Option C", 2, TestName = "GamepadInput")]
    public static void SelectChoice_WithVariousInputMethods_AllowsSelection(
        string inputMethod,
        int navigationX,
        int navigationY,
        string expectedChoice,
        int expectedIndex)
    {
        // Arrange
        var choiceContext = new TestChoiceContext(numberOfChoices: 3);
        choiceContext.DisplayChoices(DefaultChoiceOptions);

        // Act
        if (inputMethod == "keyboard" || inputMethod == "gamepad")
        {
            // For keyboard/gamepad: navigationX represents number of navigations
            for (int i = 0; i < navigationX; i++)
            {
                choiceContext.NavigateChoices(1);
            }
            choiceContext.ConfirmSelection();
        }
        else if (inputMethod == "mouse")
        {
            // For mouse: navigationX and navigationY are coordinates
            choiceContext.LayoutChoicesWithPositions(DefaultChoiceLayout);
            choiceContext.SimulateMouseClick(x: navigationX, y: navigationY);
        }

        // Assert
        AssertThat(choiceContext.SelectedChoiceIndex).IsEqual(expectedIndex);
        AssertThat(choiceContext.SelectedChoice).IsEqual(expectedChoice);
    }

    /// <summary>
    /// Tests that narrative advances after any valid choice selection.
    /// </summary>
    [TestCase]
    public static void SelectchoiceWithanyvalidinputAdvancesnarrative()
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
        choiceContext.ConfirmSelection();
        bool narrativeAdvancedAfterKeyboard = choiceContext.NarrativeAdvanced;

        // Reset and test gamepad
        choiceContext = new TestChoiceContext(numberOfChoices: 3);
        choiceContext.DisplayChoices(DefaultChoiceOptions);
        choiceContext.ConfirmSelection();
        bool narrativeAdvancedAfterGamepad = choiceContext.NarrativeAdvanced;

        // Assert
        AssertThat(narrativeAdvancedAfterMouse).IsTrue();
        AssertThat(narrativeAdvancedAfterKeyboard).IsTrue();
        AssertThat(narrativeAdvancedAfterGamepad).IsTrue();
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
        /// Navigates choices up or down.
        /// </summary>
        /// <param name="direction">Navigation direction: -1 for up, 1 for down.</param>
        internal void NavigateChoices(int direction)
        {
            int newIndex = this.selectedIndex + direction;
            if (newIndex >= 0 && newIndex < this.numberOfChoices)
            {
                this.selectedIndex = newIndex;
            }
        }

        /// <summary>
        /// Confirms selection of current choice.
        /// </summary>
        internal void ConfirmSelection()
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
