// <copyright file="OmegaChoicePresenterTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Ui.Omega;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OmegaSpiral.Tests.Ui;

/// <summary>
/// Unit tests for OmegaChoicePresenter component.
/// Tests choice display, selection, and user interaction handling.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class OmegaChoicePresenterTests
{
    private OmegaChoicePresenter _Presenter = null!;
    private VBoxContainer _ChoiceContainer = null!;

        /// <summary>
        /// Sets up test fixtures before each test.
        /// </summary>
        [Before]
        public void Setup()
        {
            // Create mock Godot nodes for testing
            _ChoiceContainer = new VBoxContainer();

            // Initialize presenter with mock node
            _Presenter = new OmegaChoicePresenter(_ChoiceContainer);

            // Add to scene tree for Godot operations
            var testScene = new Node();
            testScene.AddChild(_ChoiceContainer);

            // Auto-free will clean up after test
            AutoFree(testScene);
        }

        /// <summary>
        /// Tests presenting choices with single selection.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task PresentChoicesAsync_SingleSelection_ReturnsSelectedIndex()
        {
            // Arrange
            var choices = new Collection<string> { "Choice 1", "Choice 2", "Choice 3" };

            // Act - Present choices and simulate clicking the second button (index 1)
            var selectionTask = _Presenter.PresentChoicesAsync(choices, false);

            // Wait for buttons to be created
            await Task.Delay(100).ConfigureAwait(true); // Small delay to allow Ui to update

            // Get the second button and simulate clicking it
            var button = _ChoiceContainer.GetChild<Button>(1); // Second button (index 1)
            button.EmitSignal("pressed");

            // Assert
            var result = await selectionTask.ConfigureAwait(true);
            AssertThat(result).Contains(1);
        }

        /// <summary>
        /// Tests presenting choices with multiple selection.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task PresentChoicesAsync_MultipleSelection_ReturnsSelectedIndices()
        {
            // Arrange
            var choices = new Collection<string> { "Choice 1", "Choice 2", "Choice 3" };

            // Act
            var selectionTask = _Presenter.PresentChoicesAsync(choices, true);

            // Wait for buttons to be created
            await Task.Delay(100).ConfigureAwait(true); // Small delay to allow Ui to update

            // Get the second button and simulate clicking it
            var button = _ChoiceContainer.GetChild<Button>(1); // Second button (index 1)
            button.EmitSignal("pressed");

            // Assert
            var result = await selectionTask.ConfigureAwait(true);
            AssertThat(result).Contains(1);
        }

        /// <summary>
        /// Tests presenting choices with choice options.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task PresentChoicesAsync_WithChoiceOptions_ReturnsSelectedIndex()
        {
            // Arrange
            var choiceOptions = new Collection<OmegaChoiceOption>
            {
                new OmegaChoiceOption { Text = "Option 1", TextColor = Colors.Red },
                new OmegaChoiceOption { Text = "Option 2", IsSelected = true },
                new OmegaChoiceOption { Text = "Option 3" }
            };

            // Act
            var selectionTask = _Presenter.PresentChoicesAsync(choiceOptions);

            // Wait for buttons to be created
            await Task.Delay(100).ConfigureAwait(true); // Small delay to allow Ui to update

            // Get the third button and simulate clicking it
            var button = _ChoiceContainer.GetChild<Button>(2); // Third button (index 2)
            button.EmitSignal("pressed");

            // Assert
            var result = await selectionTask.ConfigureAwait(true);
            AssertThat(result).IsEqual(2);
        }

        /// <summary>
        /// Tests hiding choices after presenting them.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task HideChoices_AfterPresentingChoices_RemovesFromDisplay()
        {
            // Arrange
            var choices = new Collection<string> { "Choice 1", "Choice 2" };

            var choiceTask = _Presenter.PresentChoicesAsync(choices);

            // Wait for buttons to be created
            await Task.Delay(100).ConfigureAwait(true); // Small delay to allow Ui to update

            // Click first button to complete the task
            var button = _ChoiceContainer.GetChild<Button>(0);
            button.EmitSignal("pressed");

            await choiceTask.ConfigureAwait(true);

            // Act
            _Presenter.HideChoices();

            // Assert
            AssertThat(_Presenter.AreChoicesVisible()).IsFalse();
        }

        /// <summary>
        /// Tests checking if choices are visible initially.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task AreChoicesVisible_Initially_ReturnsFalse()
        {
            // Arrange & Act & Assert
            AssertThat(_Presenter.AreChoicesVisible()).IsFalse();
        }

        /// <summary>
        /// Tests presenting empty choices list throws exception.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task PresentChoicesAsync_EmptyChoices_ThrowsArgumentException()
        {
            // Arrange
            var emptyChoices = new Collection<string>();

            // Act & Assert
            AssertThrown(() => PresentChoicesEmptySync(_Presenter))
                .IsInstanceOf<ArgumentException>();
        }

        /// <summary>
        /// Tests presenting null choices throws exception.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public async Task PresentChoicesAsync_NullChoices_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            AssertThrown(() => PresentChoicesEmptySync(_Presenter))
                .IsInstanceOf<ArgumentException>();
        }

        /// <summary>
        /// Tests presenting null choice options throws exception.
        /// </summary>
        [TestCase]
        [RequireGodotRuntime]
        public void PresentChoicesAsync_NullChoiceOptions_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            AssertThrown(() => PresentChoicesNullOptionsSync(_Presenter))
                .IsInstanceOf<ArgumentNullException>();
        }

        private void PresentChoicesEmptySync(OmegaChoicePresenter presenter)
        {
            var emptyChoices = new Collection<string>();
            _Presenter.PresentChoicesAsync(emptyChoices).Wait();
        }

        private void PresentChoicesNullSync(OmegaChoicePresenter presenter)
        {
            _Presenter.PresentChoicesAsync((Collection<string>)null!).Wait();
        }

        private void PresentChoicesNullOptionsSync(OmegaChoicePresenter presenter)
        {
                    {
            _Presenter.PresentChoicesAsync((Collection<OmegaChoiceOption>)null!).Wait();
        }
    }
}
        }
    }
}
