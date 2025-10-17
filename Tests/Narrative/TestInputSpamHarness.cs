// <copyright file="TestInputSpamHarness.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Narrative
{
    using GdUnit4;
    using static GdUnit4.Assertions;

    /// <summary>
    /// Test harness for simulating rapid input scenarios in narrative processing.
    /// Used to test error handling and state consistency under input spam conditions.
    /// </summary>
    internal sealed class TestInputSpamHarness
    {
        /// <summary>
        /// Gets a value indicating whether the content has been fully displayed.
        /// </summary>
        public bool ContentDisplayed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the system is in a valid state.
        /// </summary>
        public bool IsInValidState { get; private set; } = true;

        /// <summary>
        /// Gets the count of processed inputs.
        /// </summary>
        public int ProcessedInputCount { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a crash occurred during processing.
        /// </summary>
        public bool CrashOccurred { get; private set; }

        /// <summary>
        /// Starts a content block for testing.
        /// </summary>
        public void StartContentBlock()
        {
            this.ContentDisplayed = false;
            this.IsInValidState = true;
            this.ProcessedInputCount = 0;
            this.CrashOccurred = false;
        }

        /// <summary>
        /// Simulates a single input event.
        /// </summary>
        public void SimulateInput()
        {
            this.ProcessedInputCount++;

            // Simulate content completion after some inputs
            if (this.ProcessedInputCount >= 10)
            {
                this.ContentDisplayed = true;
            }

            // Simulate potential crash on high input count (for testing)
            if (this.ProcessedInputCount > 80)
            {
                this.CrashOccurred = true;
                this.IsInValidState = false;
            }
        }

        /// <summary>
        /// Marks that a crash occurred during testing. Called from test catch blocks.
        /// </summary>
        public void MarkCrash()
        {
            this.CrashOccurred = true;
            this.IsInValidState = false;
        }
    }
}
