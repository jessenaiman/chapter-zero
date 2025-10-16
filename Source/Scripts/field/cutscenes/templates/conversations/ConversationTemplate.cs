namespace OmegaSpiral.Source.Scripts.Field.Cutscenes.Templates.Conversations;

// <copyright file="ConversationTemplate.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// Template for conversation interactions that execute Dialogic timelines.
/// This class provides a reusable template for creating dialogue interactions
/// that start a specific timeline and handle Dialogic signals.
/// </summary>
[GlobalClass]
[Tool]
public partial class ConversationTemplate : Interaction
{
    /// <summary>
    /// Gets or sets the Dialogic timeline to play when this interaction is triggered.
    /// </summary>
    [Export]
    public Resource Timeline { get; set; } = null!;

    /// <summary>
    /// Execute the conversation interaction.
    /// Starts the Dialogic timeline and waits for it to complete.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async System.Threading.Tasks.Task Execute()
    {
        if (this.Timeline == null)
        {
            GD.Print("No timeline assigned to conversation template");
            return;
        }

        try
        {
            // Start the timeline using DialogicIntegration
            var dialogic = this.GetNode("/root/Dialogic");
            if (dialogic != null)
            {
                // Connect to signal events
                dialogic.Connect("signal_event", new Callable(this, nameof(OnDialogicSignalEvent)));

                // Start the timeline (this would need to be adapted based on your DialogicIntegration)
                dialogic.Call("start_timeline", this.Timeline.ResourcePath);

                // Wait for the timeline to end
                await this.ToSignal(dialogic, "timeline_ended");

                // Disconnect from signal events
                dialogic.Disconnect("signal_event", new Callable(this, nameof(OnDialogicSignalEvent)));
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error executing conversation template: {e.Message}");
        }
    }

    /// <summary>
    /// Override the Run method to execute the conversation.
    /// </summary>
    public override async void Run()
    {
        await this.Execute();
        base.Run();
    }

    /// <summary>
    /// Handle Dialogic signal events.
    /// Override this method to respond to specific Dialogic signals.
    /// </summary>
    /// <param name="argument">The signal argument from Dialogic.</param>
    private static void OnDialogicSignalEvent(string argument)
    {
        // Default implementation does nothing
        // Override in subclasses to handle specific signals
        GD.Print($"Received Dialogic signal: {argument}");
    }
}
