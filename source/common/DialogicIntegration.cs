// <copyright file="DialogicIntegration.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Common
{
    /// <summary>
    /// Provides integration between C# narrative logic and Dialogic GDScript addon.
    /// This allows leveraging Dialogic's battle-tested dialogue system while maintaining
    /// C# control over narrative flow and game state.
    /// </summary>
    public partial class DialogicIntegration : Node
    {
        /// <summary>
        /// Reference to the Dialogic autoload singleton.
        /// </summary>
        private Node dialogic = default!;

        /// <summary>
        /// Called when the node enters the scene tree.
        /// Initializes the Dialogic integration.
        /// </summary>
        public override void _Ready()
        {
            // Get reference to Dialogic autoload
            this.dialogic = this.GetNode("/root/Dialogic");

            // Connect to Dialogic signals
            this.dialogic.Connect("timeline_started", new Callable(this, nameof(this.OnTimelineStarted)));
            this.dialogic.Connect("timeline_ended", new Callable(this, nameof(this.OnTimelineEnded)));
            this.dialogic.Connect("event_handled", new Callable(this, nameof(this.OnEventHandled)));
        }

        /// <summary>
        /// Starts a Dialogic timeline from C#.
        /// </summary>
        /// <param name="timelinePath">Path to the Dialogic timeline resource.</param>
        /// <param name="labelOrIndex">Optional label or index to start from.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartTimelineAsync(string timelinePath, Variant labelOrIndex = default)
        {
            GD.Print($"Starting Dialogic timeline: {timelinePath}");

            // Call Dialogic's start_timeline method
            this.dialogic.Call("start_timeline", timelinePath, labelOrIndex);

            // Wait for timeline to complete
            await this.ToSignal(this.dialogic, "timeline_ended");
        }

        /// <summary>
        /// Starts a Dialogic timeline with custom variables.
        /// </summary>
        /// <param name="timelinePath">Path to the Dialogic timeline resource.</param>
        /// <param name="variables">Dictionary of variables to set before starting.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="variables"/> is <see langword="null"/>.</exception>
        public async Task StartTimelineWithVariablesAsync(string timelinePath, Godot.Collections.Dictionary variables)
        {
            ArgumentNullException.ThrowIfNull(variables);

            // Set variables in Dialogic before starting timeline
            foreach (var kvp in variables)
            {
                this.SetDialogicVariable(kvp.Key.ToString(), kvp.Value);
            }

            await this.StartTimelineAsync(timelinePath).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets a Dialogic variable from C#.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">Value to set.</param>
        public void SetDialogicVariable(string variableName, Variant value)
        {
            var variablesSubsystem = (GodotObject) this.dialogic.Call("get_subsystem", "VAR");
            if (variablesSubsystem != null)
            {
                variablesSubsystem.Call("set_variable", variableName, value);
            }
        }

        /// <summary>
        /// Gets a Dialogic variable from C#.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <returns>The variable value.</returns>
        public Variant GetDialogicVariable(string variableName)
        {
            var variablesSubsystem = (GodotObject) this.dialogic.Call("get_subsystem", "VAR");
            if (variablesSubsystem != null)
            {
                return variablesSubsystem.Call("get_variable", variableName).As<Variant>();
            }

            return default;
        }

        /// <summary>
        /// Pauses the current Dialogic timeline.
        /// </summary>
        public void PauseTimeline()
        {
            this.dialogic.Call("pause");
        }

        /// <summary>
        /// Resumes the current Dialogic timeline.
        /// </summary>
        public void ResumeTimeline()
        {
            this.dialogic.Call("resume");
        }

        /// <summary>
        /// Example method showing how to integrate Dialogic with existing C# narrative.
        /// This could replace or augment parts of NarrativeTerminal.cs.
        /// </summary>
        /// <param name="choices">Array of choice options to present to the player.</param>
        /// <param name="timelinePath">Path to the Dialogic timeline resource.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="choices"/> is <see langword="null"/>.</exception>
        public async Task PresentChoiceWithDialogicAsync(string[] choices, string timelinePath)
        {
            ArgumentNullException.ThrowIfNull(choices);

            // Set choices as Dialogic variables
            using var choiceDict = new Godot.Collections.Dictionary();
            for (int i = 0; i < choices.Length; i++)
            {
                choiceDict[$"choice_{i}"] = choices[i];
            }

            await this.StartTimelineWithVariablesAsync(timelinePath, choiceDict).ConfigureAwait(false);

            // Get the selected choice from Dialogic variables
            var selectedChoice = (int) this.GetDialogicVariable("selected_choice");
            GD.Print($"Player selected choice: {selectedChoice}");

            // Continue with C# narrative logic based on choice
            await HandleChoiceResultAsync(selectedChoice).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles timeline started signal from Dialogic.
        /// </summary>
        private static void OnTimelineStarted()
        {
            GD.Print("Dialogic timeline started");

            // Add any C# logic that should run when a timeline starts
        }

        /// <summary>
        /// Handles timeline ended signal from Dialogic.
        /// </summary>
        private static void OnTimelineEnded()
        {
            GD.Print("Dialogic timeline ended");

            // Add any C# logic that should run when a timeline ends
        }

        /// <summary>
        /// Handles event handled signal from Dialogic.
        /// </summary>
        /// <param name="eventData">Data about the handled event.</param>
        private static void OnEventHandled(Variant eventData)
        {
            // Handle specific Dialogic events in C#
            // This could be used to trigger C# game logic based on dialogue events
            GD.Print($"Dialogic event handled: {eventData}");
        }

        /// <summary>
        /// Handles the result of a Dialogic choice in C#.
        /// </summary>
        /// <param name="choiceIndex">The index of the selected choice.</param>
        private static async Task HandleChoiceResultAsync(int choiceIndex)
        {
            // Your existing C# narrative logic here
            switch (choiceIndex)
            {
                case 0:
                    GD.Print("Player chose option 1");
                    break;
                case 1:
                    GD.Print("Player chose option 2");
                    break;
                default:
                    GD.Print("Invalid choice");
                    break;
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
