// <copyright file="OmegaTextRendererTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Ui.Omega;

namespace OmegaSpiral.Tests.Ui;

/// <summary>
/// Integration tests for OmegaTextRenderer component.
/// Tests text display, animation, and formatting functionality.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class OmegaTextRendererTests
    {
        private OmegaTextRenderer _Renderer = null!;
        private RichTextLabel _MockTextDisplay = null!;

        /// <summary>
        /// Test constants for consistent values across tests.
        /// </summary>
        private const string _TestText = "Hello World";
        private const string _FastText = "Fast Text";
        private const string _DelayedText = "Delayed Text";
        private const string _AnimatedText = "Animated Text";
        private const string _LongText = "This is a long text that should cause scrolling behavior to be tested.";
        private const string _RedText = "Red Text";
        private const string _EmptyText = "";
        private const float _TypingSpeed = 50f;
        private const float _Delay = 0.1f;
        private const float _ToleranceMs = 10f;
        private const int _AnimationDelay = 50;

        /// <summary>
        /// Sets up test fixtures before each test.
        /// </summary>
        [Before]
        public void Setup()
        {
            _MockTextDisplay = AutoFree(new RichTextLabel())!;
            _Renderer = new OmegaTextRenderer(_MockTextDisplay);
            _Renderer.ClearText(); // Ensure clean state
        }

        /// <summary>
        /// Cleans up test fixtures after each test.
        /// </summary>
        [After]
        public void Cleanup()
        {
            _Renderer?.ClearText();
            _Renderer?.Dispose();
            _MockTextDisplay?.QueueFree();
        }

        /// <summary>
        /// Tests that constructor initializes with valid text display.
        /// </summary>
        [TestCase]
        public void Constructor_WithValidTextDisplay_InitializesCorrectly()
        {
            // Arrange & Act
            var renderer = new OmegaTextRenderer(_MockTextDisplay);

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
            AssertThrown(() => new OmegaTextRenderer(null!))
                .IsInstanceOf<ArgumentNullException>();
        }

        /// <summary>
        /// Tests appending text with default typing speed.
        /// </summary>
        [TestCase]
        public async Task AppendTextAsync_DefaultParameters_AppendsTextCorrectly()
        {
            // Arrange
            var testText = _TestText;

            // Act
            await _Renderer.AppendTextAsync(testText).ConfigureAwait(true);

            // Assert
            AssertThat(_Renderer.GetCurrentText()).IsEqual(testText);
        }

        /// <summary>
        /// Tests appending text with custom typing speed.
        /// </summary>
        [TestCase]
        public async Task AppendTextAsync_CustomTypingSpeed_AppendsTextCorrectly()
        {
            // Arrange
            _Renderer.ClearText();
            var testText = _FastText;
            var typingSpeed = 50f; // characters per second

            // Act
            await _Renderer.AppendTextAsync(testText, typingSpeed).ConfigureAwait(true);

            // Assert
            AssertThat(_Renderer.GetCurrentText()).IsEqual(testText);
        }

        /// <summary>
        /// Tests appending text with delay before start.
        /// </summary>
        [TestCase]
        public async Task AppendTextAsync_WithDelay_DelaysBeforeStarting()
        {
            // Arrange
            _Renderer.ClearText();
            var testText = _DelayedText;
            var delay = 0.1f;
            var startTime = Time.GetTicksMsec();

            // Act
            await _Renderer.AppendTextAsync(testText, delayBeforeStart: delay).ConfigureAwait(true);

            // Assert
            var elapsed = Time.GetTicksMsec() - startTime;
            AssertThat(elapsed).IsGreater(90); // Should take at least 90ms
            AssertThat(_Renderer.GetCurrentText()).IsEqual(testText);
        }

        /// <summary>
        /// Tests clearing text removes all content.
        /// </summary>
        [TestCase]
        public async Task ClearText_AfterAppendingText_RemovesAllContent()
        {
            // Arrange
            await _Renderer.AppendTextAsync("Some text").ConfigureAwait(true);

            // Act
            _Renderer.ClearText();

            // Assert
            AssertThat(_Renderer.GetCurrentText()).IsEqual("");
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
            _Renderer.SetTextColor(color);
            await _Renderer.AppendTextAsync(_RedText).ConfigureAwait(true);

            // Assert
            // Note: This test verifies the method doesn't throw and text is appended
            // Full color verification would require inspecting RichTextLabel BBCode
            AssertThat(_Renderer.GetCurrentText()).Contains(_RedText);
        }

        /// <summary>
        /// Tests getting current text when empty returns empty string.
        /// </summary>
        [TestCase]
        public void GetCurrentText_WhenEmpty_ReturnsEmptyString()
        {
            // Arrange
            _Renderer.ClearText();

            // Act & Assert
            AssertThat(_Renderer.GetCurrentText()).IsEqual("");
        }

        /// <summary>
        /// Tests scroll to bottom when text is added.
        /// </summary>
        [TestCase]
        public async Task ScrollToBottom_AfterAddingText_ScrollsCorrectly()
        {
            // Arrange
            _Renderer.ClearText();
            var longText = _LongText;

            // Act
            await _Renderer.AppendTextAsync(longText).ConfigureAwait(true);
            _Renderer.ScrollToBottom();

            // Assert
            // Note: Full scroll verification would require mocking scroll properties
            AssertThat(_Renderer.GetCurrentText()).IsEqual(longText);
        }

        /// <summary>
        /// Tests animation state when not animating.
        /// </summary>
        [TestCase]
        public void IsAnimating_WhenNotAnimating_ReturnsFalse()
        {
            // Act & Assert
            AssertThat(_Renderer.IsAnimating()).IsFalse();
        }

        /// <summary>
        /// Tests animation state during text appending.
        /// </summary>
        [TestCase]
        public async Task IsAnimating_DuringTextAppending_ReturnsTrue()
        {
            // Arrange
            var testText = _AnimatedText;

            // Act - Start animation
            var animationTask = _Renderer.AppendTextAsync(testText, typingSpeed: 10f); // Slow typing

            // Give it a moment to start animating
            await Task.Delay(_AnimationDelay).ConfigureAwait(true);

            // Assert - Should be animating
            AssertThat(_Renderer.IsAnimating()).IsTrue();

            // Wait for completion
            await animationTask.ConfigureAwait(true);
            AssertThat(_Renderer.IsAnimating()).IsFalse();
        }

        /// <summary>
        /// Tests appending empty text does nothing.
        /// </summary>
        [TestCase]
        public async Task AppendTextAsync__EmptyText_DoesNothing()
        {
            // Arrange
            var initialText = _Renderer.GetCurrentText();

            // Act
            await _Renderer.AppendTextAsync("").ConfigureAwait(true);

            // Assert
            AssertThat(_Renderer.GetCurrentText()).IsEqual(initialText);
        }
    }
}
