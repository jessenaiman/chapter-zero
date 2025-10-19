// <copyright file="ErrorHandlingTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Functional.Narrative;

using GdUnit4;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Tests.Narrative;
using static GdUnit4.Assertions;

/// <summary>
/// Functional test suite for validating error handling and edge cases in the narrative system.
/// Tests cover input validation, error recovery, focus handling, and system robustness.
/// These tests verify system stability under adverse conditions and error scenarios.
/// </summary>
[TestSuite]
public class ErrorHandlingTests
{
    /// <summary>
    /// Tests that system rejects empty string when player name is submitted.
    /// </summary>
    [TestCase]
    public void SubmitName_WithEmptyString_RejectsInput()
    {
        // Arrange
        var validator = new NameValidationHarness(maximumLength: 16);

        // Act
        var result = validator.SubmitName(string.Empty);

        // Assert
        AssertThat(result.IsAccepted).IsFalse();
        AssertThat(result.ErrorMessage).IsEqual("Name cannot be empty.");
    }

    /// <summary>
    /// Tests that system rejects special characters outside allowed set.
    /// </summary>
    [TestCase]
    public void SubmitName_WithDisallowedSpecialCharacters_RejectsInput()
    {
        // Arrange
        var validator = new NameValidationHarness(maximumLength: 16);

        // Act
        var result = validator.SubmitName("Omega!");

        // Assert
        AssertThat(result.IsAccepted).IsFalse();
        AssertThat(result.ErrorMessage).IsEqual("Name contains unsupported characters.");
    }

    /// <summary>
    /// Tests that system rejects names exceeding maximum length.
    /// </summary>
    [TestCase]
    public void SubmitName_WithExcessiveLength_RejectsInput()
    {
        // Arrange
        var validator = new NameValidationHarness(maximumLength: 8);

        // Act
        var result = validator.SubmitName("BeyondLimits");

        // Assert
        AssertThat(result.IsAccepted).IsFalse();
        AssertThat(result.ErrorMessage).IsEqual("Name must be 8 characters or fewer.");
    }

    /// <summary>
    /// Tests that system displays clear error message when validation fails.
    /// </summary>
    [TestCase]
    public void SubmitName_WithInvalidInput_DisplaysClearErrorMessage()
    {
        // Arrange
        var validator = new NameValidationHarness(maximumLength: 12);

        // Act
        var result = validator.SubmitName("???");

        // Assert
        AssertThat(result.IsAccepted).IsFalse();
        AssertThat(result.ErrorMessage).IsEqual("Name contains unsupported characters.");
    }

    /// <summary>
    /// Tests that content completes normally when input buttons are mashed rapidly.
    /// </summary>
    [TestCase]
    public void ProcessInput_WithRapidMashing_CompletesContentNormally()
    {
        // Arrange
        var inputHarness = new TestInputSpamHarness();
        inputHarness.StartContentBlock();

        // Act - simulate 20 rapid key presses
        for (int i = 0; i < 20; i++)
        {
            inputHarness.SimulateInput();
        }

        // Assert
        AssertThat(inputHarness.ContentDisplayed).IsTrue();
        AssertThat(inputHarness.IsInValidState).IsTrue();
    }

    /// <summary>
    /// Tests that consistent state machine is maintained after 50+ rapid inputs.
    /// </summary>
    [TestCase]
    public void ProcessInput_WithFiftyPlusRapidInputs_MaintainsConsistentState()
    {
        // Arrange
        var inputHarness = new TestInputSpamHarness();
        inputHarness.StartContentBlock();

        // Act - simulate 60+ rapid inputs
        for (int i = 0; i < 60; i++)
        {
            inputHarness.SimulateInput();
        }

        // Assert
        AssertThat(inputHarness.IsInValidState).IsTrue();
        AssertThat(inputHarness.ProcessedInputCount).IsGreater(50);
        AssertThat(inputHarness.CrashOccurred).IsFalse();
    }

    /// <summary>
    /// Tests that system avoids crashes when processing input spam.
    /// </summary>
    [TestCase]
    public void ProcessInput_WithInputSpam_AvoidsCrashes()
    {
        // Arrange
        var inputHarness = new TestInputSpamHarness();
        inputHarness.StartContentBlock();

        // Act - attempt to trigger crashes with spam
        try
        {
            for (int i = 0; i < 100; i++)
            {
                inputHarness.SimulateInput();
            }
        }
        catch (Exception)
        {
            inputHarness.MarkCrash();
        }

        // Assert
        AssertThat(inputHarness.CrashOccurred).IsFalse();
        AssertThat(inputHarness.IsInValidState).IsTrue();
    }
}
