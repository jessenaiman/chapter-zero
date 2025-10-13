// <copyright file="DialogicTestHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using Godot;

    /// <summary>
    /// Helper class to simulate Dialogic signals and timeline execution for testing purposes.
    /// This allows automated tests to simulate user choices and validate narrative flow.
    /// </summary>
    public class DialogicTestHelper
    {
        private readonly Node testScene;
        private readonly List<string> capturedSignals = new ();
        private readonly Dictionary<string, object> dialogicState = new ();
        private bool timelineActive;
        private string? currentTimeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogicTestHelper"/> class.
        /// </summary>
        /// <param name="testScene">The test scene node to use for signal connections.</param>
        public DialogicTestHelper(Node testScene)
        {
            this.testScene = testScene ?? throw new ArgumentNullException(nameof(testScene));
            this.timelineActive = false;
        }

        /// <summary>
        /// Gets the list of captured Dialogic signals during test execution.
        /// </summary>
        public List<string> CapturedSignals => this.capturedSignals;

        /// <summary>
        /// Gets a value indicating whether a Dialogic timeline is currently active.
        /// </summary>
        public bool IsTimelineActive => this.timelineActive;

        /// <summary>
        /// Gets the name of the currently running timeline, or <see langword="null"/> if none is active.
        /// </summary>
        public string? CurrentTimeline => this.currentTimeline;

        /// <summary>
        /// Simulates starting a Dialogic timeline.
        /// </summary>
        /// <param name="timelinePath">The path to the timeline resource.</param>
        public void SimulateStartTimeline(string timelinePath)
        {
            if (string.IsNullOrEmpty(timelinePath))
            {
                throw new ArgumentException("Timeline path cannot be null or empty", nameof(timelinePath));
            }

            this.currentTimeline = timelinePath;
            this.timelineActive = true;
            this.CaptureSignal($"timeline_started: {timelinePath}");
            GD.Print($"[TEST] Simulating timeline start: {timelinePath}");
        }

        /// <summary>
        /// Simulates a Dialogic text signal being emitted.
        /// </summary>
        /// <param name="text">The text content of the signal.</param>
        public void SimulateTextSignal(string text)
        {
            if (!this.timelineActive)
            {
                throw new InvalidOperationException("Cannot emit text signal when no timeline is active");
            }

            this.CaptureSignal($"text: {text}");
            GD.Print($"[TEST] Simulating text signal: {text}");
        }

        /// <summary>
        /// Simulates a player making a choice in the narrative.
        /// </summary>
        /// <param name="choiceIndex">The index of the choice (0-based).</param>
        /// <param name="choiceText">The text of the selected choice.</param>
        public void SimulateChoice(int choiceIndex, string choiceText)
        {
            if (!this.timelineActive)
            {
                throw new InvalidOperationException("Cannot make choice when no timeline is active");
            }

            if (choiceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(choiceIndex), "Choice index must be non-negative");
            }

            this.CaptureSignal($"choice_selected: {choiceIndex} - {choiceText}");
            GD.Print($"[TEST] Simulating choice: {choiceIndex} - {choiceText}");
        }

        /// <summary>
        /// Simulates the player entering text input (e.g., name entry).
        /// </summary>
        /// <param name="input">The text entered by the player.</param>
        public void SimulateTextInput(string input)
        {
            if (!this.timelineActive)
            {
                throw new InvalidOperationException("Cannot enter text when no timeline is active");
            }

            this.CaptureSignal($"text_input: {input}");
            this.dialogicState["last_input"] = input;
            GD.Print($"[TEST] Simulating text input: {input}");
        }

        /// <summary>
        /// Simulates a custom Dialogic signal being emitted.
        /// </summary>
        /// <param name="signalName">The name of the signal.</param>
        /// <param name="parameters">Optional parameters for the signal.</param>
        public void SimulateCustomSignal(string signalName, Dictionary<string, object>? parameters = null)
        {
            if (string.IsNullOrEmpty(signalName))
            {
                throw new ArgumentException("Signal name cannot be null or empty", nameof(signalName));
            }

            string signalData = signalName;
            if (parameters != null && parameters.Count > 0)
            {
                signalData += ": " + System.Text.Json.JsonSerializer.Serialize(parameters);
            }

            this.CaptureSignal($"custom_signal: {signalData}");
            GD.Print($"[TEST] Simulating custom signal: {signalData}");
        }

        /// <summary>
        /// Simulates the end of a Dialogic timeline.
        /// </summary>
        public void SimulateTimelineEnd()
        {
            if (!this.timelineActive)
            {
                throw new InvalidOperationException("Cannot end timeline when none is active");
            }

            this.CaptureSignal($"timeline_ended: {this.currentTimeline}");
            this.timelineActive = false;
            this.currentTimeline = null;
            GD.Print("[TEST] Simulating timeline end");
        }

        /// <summary>
        /// Gets the value of a Dialogic variable from the test state.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <returns>The variable value, or <see langword="null"/> if not found.</returns>
        public object? GetVariable(string variableName)
        {
            return this.dialogicState.TryGetValue(variableName, out var value) ? value : null;
        }

        /// <summary>
        /// Sets a Dialogic variable in the test state.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="value">The value to set.</param>
        public void SetVariable(string variableName, object value)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                throw new ArgumentException("Variable name cannot be null or empty", nameof(variableName));
            }

            this.dialogicState[variableName] = value;
            GD.Print($"[TEST] Set variable {variableName} = {value}");
        }

        /// <summary>
        /// Clears all captured signals and resets the test helper state.
        /// </summary>
        public void Reset()
        {
            this.capturedSignals.Clear();
            this.dialogicState.Clear();
            this.timelineActive = false;
            this.currentTimeline = null;
            GD.Print("[TEST] DialogicTestHelper reset");
        }

        /// <summary>
        /// Captures a signal for later validation in tests.
        /// </summary>
        /// <param name="signalData">The signal data to capture.</param>
        private void CaptureSignal(string signalData)
        {
            this.capturedSignals.Add(signalData);
        }
    }
}
