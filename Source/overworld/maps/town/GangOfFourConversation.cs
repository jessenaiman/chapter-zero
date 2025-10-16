namespace OmegaSpiral.Source.Overworld.Maps.Town;

// <copyright file="GangOfFourConversation.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral;
using OmegaSpiral.Source.Scripts.Field;

/// <summary>
/// Conversation interaction with the Gang of Four NPCs.
/// Handles dialogue interactions and coin rewards.
/// </summary>
[GlobalClass]
[Tool]
public partial class GangOfFourConversation : Interaction
{
    /// <summary>
    /// Handle Dialogic signal events for this conversation.
    /// </summary>
    /// <param name="argument">The signal argument from Dialogic.</param>
    public static void OnDialogicSignalEvent(string argument)
    {
        if (argument == "coin_received")
        {
            Inventory.Restore()?.Add(Inventory.ItemType.Coin);
        }
    }

    /// <summary>
    /// Override the interaction to start the appropriate dialogue.
    /// </summary>
    public override void Run()
    {
        // Start the Gang of Four conversation dialogue
        // This would typically use DialogicIntegration to start the appropriate timeline
        GD.Print("Starting Gang of Four conversation");

        // Call parent interaction logic
        base.Run();

        // The dialogue handling would be managed by DialogicIntegration
        // and the DialogueWindow component for UI
    }
}
