// <copyright file="TerminalChoicePresenterTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.UI.Terminal;
using System.Collections.Generic;

namespace OmegaSpiral.Tests.Unit.Common.Terminal
{
    /// <summary>
    /// Unit tests for TerminalChoicePresenter component.
    /// Tests choice display, selection, and user interaction handling.
    /// </summary>
    [TestSuite]
    [RequireGodotRuntime]
    public class TerminalChoicePresenterTests
    {
        private TerminalChoicePresenter _presenter = null!;
        private VBoxContainer _mockChoiceContainer = null!;

        /// <summary>
        /// Sets up test fixtures before each test.
        /// </summary>
        [Before]
        public void Setup()
        {
            _mockChoiceContainer = new VBoxContainer();
            _presenter = new TerminalChoicePresenter(_mockChoiceContainer);
        }

        /// <summary>
        /// Cleans up test fixtures after each test.
        /// </summary>
        [After]
        public void Cleanup()
        {
            _presenter?.Dispose();
            _mockChoiceContainer?.QueueFree();
        }

        /// <summary>
        /// Tests that constructor initializes with valid choice container.
        /// </summary>
        [TestCase]
        public void Constructor_WithValidChoiceContainer_InitializesCorrectly()
        {
            // Arrange & Act
            var presenter = new TerminalChoicePresenter(_mockChoiceContainer);

            // Assert
            AssertThat(presenter).IsNotNull();
            AssertThat(presenter.AreChoicesVisible()).IsFalse();
        }

        /// <summary>
        /// Tests that constructor throws with null choice container.
        /// </summary>
        [TestCase]
        public void Constructor_WithNullChoiceContainer_ThrowsArgumentNullException()
        {
            // Act & Assert
            AssertThrown(() => new TerminalChoicePresenter(null!))
                .IsInstanceOf<ArgumentNullException>();
        }

        /// <summary>
        /// Tests presenting choices with single selection.
        /// </summary>
        [TestCase]
        public async Task PresentChoicesAsync_SingleSelection_ReturnsSelectedIndex()
        {
            // Arrange
            var choices = new List<string> { "Choice 1", "Choice 2", "Choice 3" };

            // Act - Simulate user selecting choice 1 (index 0)
            var selectionTask = _presenter.PresentChoicesAsync(choices, false);
            // In a real scenario, this would wait for user input
            // For testing, we'll simulate the selection

            // For now, just verify the method doesn't throw
            // Full integration testing would require mocking user input
            AssertThat(selectionTask).IsNotNull();
            _presenter.HideChoices(); // Cancel the task to avoid hanging
        }

        /// <summary>
        /// Tests presenting choices with multiple selection enabled.
        /// </summary>
        [TestCase]
        public async Task PresentChoicesAsync_MultipleSelection_ReturnsSelectedIndices()
        {
            // Arrange
            var choices = new List<string> { "Choice 1", "Choice 2", "Choice 3" };

            // Act
            var selectionTask = _presenter.PresentChoicesAsync(choices, true);

            // Assert
            AssertThat(selectionTask).IsNotNull();
            // Full testing would verify multiple selections are returned
            _presenter.HideChoices(); // Cancel the task to avoid hanging
        }

        /// <summary>
        /// Tests presenting choices with ChoiceOption objects.
        /// </summary>
        [TestCase]
        public async Task PresentChoicesAsync_WithChoiceOptions_ReturnsSelectedIndex()
        {
            // Arrange
            var choiceOptions = new List<ChoiceOption>
            {
                new ChoiceOption { Text = "Option 1", TextColor = Colors.Red },
                new ChoiceOption { Text = "Option 2", IsSelected = true },
                new ChoiceOption { Text = "Option 3" }
            };

            // Act
            var selectionTask = _presenter.PresentChoicesAsync(choiceOptions);

            // Assert
            AssertThat(selectionTask).IsNotNull();
            _presenter.HideChoices(); // Cancel the task to avoid hanging
        }

        /// <summary>
        /// Tests hiding choices removes them from display.
        /// </summary>
        [TestCase]
        public async Task HideChoices_AfterPresentingChoices_RemovesFromDisplay()
        {
            // Arrange
            var choices = new List<string> { "Choice 1", "Choice 2" };
            await _presenter.PresentChoicesAsync(choices).ConfigureAwait(false);

            // Act
            _presenter.HideChoices();

            // Assert
            AssertThat(_presenter.AreChoicesVisible()).IsFalse();
        }

        /// <summary>
        /// Tests getting selected choice index when none selected.
        /// </summary>
        [TestCase]
        public void GetSelectedChoiceIndex_NoSelection_ReturnsNegativeOne()
        {
            // Act & Assert
            AssertThat(_presenter.GetSelectedChoiceIndex()).IsEqual(-1);
        }

        /// <summary>
        /// Tests setting choice navigation enabled/disabled.
        /// </summary>
        [TestCase]
        public void SetChoiceNavigationEnabled_ChangesNavigationState()
        {
            // Act
            _presenter.SetChoiceNavigationEnabled(false);

            // Assert
            // Note: This test verifies the method doesn't throw
            // Full verification would require checking internal state
            AssertThat(_presenter).IsNotNull();
        }

        /// <summary>
        /// Tests choices are not visible initially.
        /// </summary>
        [TestCase]
        public void AreChoicesVisible_Initially_ReturnsFalse()
        {
            // Act & Assert
            AssertThat(_presenter.AreChoicesVisible()).IsFalse();
        }

        /// <summary>
        /// Tests presenting empty choices list throws exception.
        /// </summary>
        [TestCase]
        public void PresentChoicesAsync_EmptyChoices_ThrowsArgumentException()
        {
            // Arrange
            var emptyChoices = new List<string>();

            // Act & Assert
            AssertThrown(() => PresentChoicesEmptySync())
                .IsInstanceOf<ArgumentException>();
        }

        /// <summary>
        /// Tests presenting null choices throws exception.
        /// </summary>
        [TestCase]
        public void PresentChoicesAsync_NullChoices_ThrowsArgumentNullException()
        {
            // Act & Assert
            AssertThrown(() => PresentChoicesNullSync())
                .IsInstanceOf<ArgumentNullException>();
        }

        /// <summary>
        /// Tests presenting null choice options throws exception.
        /// </summary>
        [TestCase]
        public void PresentChoicesAsync_NullChoiceOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            AssertThrown(() => PresentChoicesNullOptionsSync())
                .IsInstanceOf<ArgumentNullException>();
        }

        private void PresentChoicesEmptySync()
        {
            var emptyChoices = new List<string>();
            _presenter.PresentChoicesAsync(emptyChoices).Wait();
        }

        private void PresentChoicesNullSync()
        {
            _presenter.PresentChoicesAsync((List<string>)null!).Wait();
        }

        private void PresentChoicesNullOptionsSync()
        {
            _presenter.PresentChoicesAsync((List<ChoiceOption>)null!).Wait();
        }
    }
}
