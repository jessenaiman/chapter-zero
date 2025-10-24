// <copyright file="TerminalTextRendererTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.UI.Terminal;

namespace OmegaSpiral.Tests.Unit.Common.Terminal
{
    /// <summary>
    /// Unit tests for TerminalTextRenderer component.
    /// Tests text display, animation, and formatting functionality.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public class TerminalTextRendererTests
    {
        private TerminalTextRenderer _renderer = null!;
        private RichTextLabel _mockTextDisplay = null!;

        /// <summary>
        /// Test constants for consistent values across tests.
        /// </summary>
        private const string TestText = "Hello World";
        private const string FastText = "Fast Text";
        private const string DelayedText = "Delayed Text";
        private const string AnimatedText = "Animated Text";
        private const string LongText = "This is a long text that should cause scrolling behavior to be tested.";
        private const string RedText = "Red Text";
        private const string EmptyText = "";
        private const float TypingSpeed = 50f;
        private const float Delay = 0.1f;
        private const float ToleranceMs = 10f;
        private const int AnimationDelay = 50;

        /// <summary>
        /// Sets up test fixtures before each test.
        /// </summary>
        [Before]
        public void Setup()
        {
            _mockTextDisplay = AutoFree(new RichTextLabel())!;
            _renderer = new TerminalTextRenderer(_mockTextDisplay);
            _renderer.ClearText(); // Ensure clean state
        }

        /// <summary>
        /// Cleans up test fixtures after each test.
        /// </summary>
        [After]
        public void Cleanup()
        {
            _renderer?.Dispose();
        }

        /// <summary>
        /// Tests that constructor initializes with valid text display.
        /// </summary>
        [TestCase]
        public void Constructor_WithValidTextDisplay_InitializesCorrectly()
        {
            // Arrange & Act
            var renderer = new TerminalTextRenderer(_mockTextDisplay);

            // Assert
            AssertThat(renderer).IsNotNull();
            AssertThat(renderer.GetCurrentText()).IsEqual("");
        }

        /// <summary>
        /// Tests that constructor throws with null text display.
        /// </summary>
        [TestCase]
        public void Constructor_WithNullTextDisplay_ThrowsArgumentNullException()
        {
            // Act & Assert
            AssertThrown(() => new TerminalTextRenderer(null!))
                .IsInstanceOf<ArgumentNullException>();
        }

        /// <summary>
        /// Tests appending text with default typing speed.
        /// </summary>
        [TestCase]
        public async Task AppendTextAsync_DefaultParameters_AppendsTextCorrectly()
        {
            // Arrange
            var testText = "Hello World";

            // Act
            await _renderer.AppendTextAsync(testText).ConfigureAwait(false);

            // Assert
            AssertThat(_renderer.GetCurrentText()).IsEqual(testText);
        }

        /// <summary>
        /// Tests appending text with custom typing speed.
        /// </summary>
        [TestCase]
        public async Task AppendTextAsync_CustomTypingSpeed_AppendsTextCorrectly()
        {
            // Arrange
            var testText = "Fast Text";
            var typingSpeed = 50f; // characters per second

            // Act
            await _renderer.AppendTextAsync(testText, typingSpeed).ConfigureAwait(false);

            // Assert
            AssertThat(_renderer.GetCurrentText()).IsEqual(testText);
        }

        /// <summary>
        /// Tests appending text with delay before start.
        /// </summary>
        [TestCase]
        public async Task AppendTextAsync_WithDelay_DelaysBeforeStarting()
        {
            // Arrange
            var testText = "Delayed Text";
            var delay = 0.1f;
            var startTime = Time.GetTicksMsec();

            // Act
            await _renderer.AppendTextAsync(testText, delayBeforeStart: delay).ConfigureAwait(false);

            // Assert
            var elapsed = Time.GetTicksMsec() - startTime;
            AssertThat(elapsed).IsGreater(90); // Should take at least 90ms
            AssertThat(_renderer.GetCurrentText()).IsEqual(testText);
        }

        /// <summary>
        /// Tests clearing text removes all content.
        /// </summary>
        [TestCase]
        public async Task ClearText_AfterAppendingText_RemovesAllContent()
        {
            // Arrange
            await _renderer.AppendTextAsync("Some text").ConfigureAwait(false);

            // Act
            _renderer.ClearText();

            // Assert
            AssertThat(_renderer.GetCurrentText()).IsEqual("");
        }

        /// <summary>
        /// Tests setting text color applies to subsequent text.
        /// </summary>
        [TestCase]
        public async Task SetTextColor_ChangesColorOfSubsequentText()
        {
            // Arrange
            var color = new Color(1.0f, 0.0f, 0.0f); // Red

            // Act
            _renderer.SetTextColor(color);
            await _renderer.AppendTextAsync("Red Text").ConfigureAwait(false);

            // Assert
            // Note: This test verifies the method doesn't throw and text is appended
            // Full color verification would require inspecting RichTextLabel BBCode
            AssertThat(_renderer.GetCurrentText()).Contains("Red Text");
        }

        /// <summary>
        /// Tests getting current text when empty returns empty string.
        /// </summary>
        [TestCase]
        public void GetCurrentText_WhenEmpty_ReturnsEmptyString()
        {
            // Act & Assert
            AssertThat(_renderer.GetCurrentText()).IsEqual("");
        }

        /// <summary>
        /// Tests scroll to bottom when text is added.
        /// </summary>
        [TestCase]
        public async Task ScrollToBottom_AfterAddingText_ScrollsCorrectly()
        {
            // Arrange
            var longText = "This is a long text that should cause scrolling behavior to be tested.";

            // Act
            await _renderer.AppendTextAsync(longText).ConfigureAwait(false);
            _renderer.ScrollToBottom();

            // Assert
            // Note: Full scroll verification would require mocking scroll properties
            AssertThat(_renderer.GetCurrentText()).IsEqual(longText);
        }

        /// <summary>
        /// Tests animation state when not animating.
        /// </summary>
        [TestCase]
        public void IsAnimating_WhenNotAnimating_ReturnsFalse()
        {
            // Act & Assert
            AssertThat(_renderer.IsAnimating()).IsFalse();
        }

        /// <summary>
        /// Tests animation state during text appending.
        /// </summary>
        [TestCase]
        public async Task IsAnimating_DuringTextAppending_ReturnsTrue()
        {
            // Arrange
            var testText = "Animated Text";

            // Act - Start animation
            var animationTask = _renderer.AppendTextAsync(testText, typingSpeed: 10f); // Slow typing

            // Give it a moment to start animating
            await Task.Delay(50).ConfigureAwait(false);

            // Assert - Should be animating
            AssertThat(_renderer.IsAnimating()).IsTrue();

            // Wait for completion
            await animationTask.ConfigureAwait(false);
            AssertThat(_renderer.IsAnimating()).IsFalse();
        }

        /// <summary>
        /// Tests appending empty text does nothing.
        /// </summary>
        [TestCase]
        public async Task AppendTextAsync_EmptyText_DoesNothing()
        {
            // Arrange
            var initialText = _renderer.GetCurrentText();

            // Act
            await _renderer.AppendTextAsync("").ConfigureAwait(false);

            // Assert
            AssertThat(_renderer.GetCurrentText()).IsEqual(initialText);
        }
    }
}
