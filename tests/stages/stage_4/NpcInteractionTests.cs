// <copyright file="NpcInteractionTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage4;

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

/// <summary>
/// Tests for Stage 4 NPC interactions.
/// Verifies dialogue system, conversation templates, and interaction popups.
/// </summary>
[TestSuite]
public partial class NpcInteractionTests
{
    /// <summary>
    /// Verifies DialogueWindow script exists.
    /// </summary>
    [TestCase]
    public void TestDialogueWindowExists()
    {
        var dialogueWindow = Godot.GD.Load<CSharpScript>("res://source/scripts/field/ui/DialogueWindow.cs");
        AssertThat(dialogueWindow).IsNotNull();
    }

    /// <summary>
    /// Verifies ConversationTemplate exists.
    /// </summary>
    [TestCase]
    public void TestConversationTemplateExists()
    {
        var conversationTemplate = Godot.GD.Load<CSharpScript>("res://source/scripts/field/cutscenes/templates/conversations/ConversationTemplate.cs");
        AssertThat(conversationTemplate).IsNotNull();
    }

    /// <summary>
    /// Verifies Interaction base class exists.
    /// </summary>
    [TestCase]
    public void TestInteractionExists()
    {
        var interaction = Godot.GD.Load<CSharpScript>("res://source/scripts/field/cutscenes/Interaction.cs");
        AssertThat(interaction).IsNotNull();
    }

    /// <summary>
    /// Verifies InteractionPopup scene exists.
    /// </summary>
    [TestCase]
    public void TestInteractionPopupSceneExists()
    {
        var popupScene = ResourceLoader.Exists("res://source/scripts/field/cutscenes/popups/InteractionPopup.tscn");
        AssertThat(popupScene).IsTrue();
    }

    /// <summary>
    /// Verifies MovingInteractionPopup script exists.
    /// </summary>
    [TestCase]
    public void TestMovingInteractionPopupExists()
    {
        var movingPopup = Godot.GD.Load<CSharpScript>("res://source/scripts/field/cutscenes/popups/MovingInteractionPopup.cs");
        AssertThat(movingPopup).IsNotNull();
    }

    /// <summary>
    /// Verifies Dialogic timeline files exist for NPCs.
    /// </summary>
    [TestCase]
    public void TestDialogicTimelinesExist()
    {
        var warriorTimeline = ResourceLoader.Exists("res://source/overworld/maps/town/warrior.dtl");
        var thiefTimeline = ResourceLoader.Exists("res://source/overworld/maps/town/thief.dtl");
        var monkTimeline = ResourceLoader.Exists("res://source/overworld/maps/town/monk.dtl");

        AssertThat(warriorTimeline && thiefTimeline && monkTimeline).IsTrue();
    }

    /// <summary>
    /// Verifies Gamepiece scene exists for NPCs.
    /// </summary>
    [TestCase]
    public void TestGamepieceSceneExists()
    {
        var gamepieceScene = ResourceLoader.Exists("res://source/scripts/field/gamepieces/Gamepiece.tscn");
        AssertThat(gamepieceScene).IsTrue();
    }

    /// <summary>
    /// Verifies GamepieceController exists.
    /// </summary>
    [TestCase]
    public void TestGamepieceControllerExists()
    {
        var controller = Godot.GD.Load<CSharpScript>("res://source/scripts/field/gamepieces/controllers/GamepieceController.cs");
        AssertThat(controller).IsNotNull();
    }
}
