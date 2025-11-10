
// <copyright file="GangOfFourConversation.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Field;

namespace OmegaSpiral.Source.Overworld.Maps.town;

/// <summary>
/// Interaction that drives the dialogue with the "Gang of Four" NPC group.
/// It integrates with the Dialogic plugin via the static signal handler
/// <see cref="OnDialogicSignalEvent(string)"/> and awards a coin to the player
/// when the "coin_received" signal is emitted.
/// </summary>
[GlobalClass]
[Tool]
public partial class GangOfFourConversation : Interaction
{
    /// <summary>
    /// Handles Dialogic signal events for this conversation.
    /// The method is registered as a global callback by the Dialogic integration layer.
    /// When the <c>coin_received</c> argument is received, a coin item is added to the
    /// player's <see cref="Inventory"/>.
    /// </summary>
    /// <param name="argument">The signal argument supplied by Dialogic (e.g., "coin_received").</param>
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
        // and the DialogueWindow component for Ui
    }
}
