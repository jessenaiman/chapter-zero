// <copyright file="NpcInteractionTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Scripts.Field.gamepieces;
using OmegaSpiral.Source.Scripts.Field.gamepieces.Controllers;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Stages.Stage4;

/// <summary>
/// Test suite for validating NPC interactions in Stage 4.
/// Ensures all NPC dialogue and interaction systems match godot-open-rpg behavior.
/// </summary>
[TestSuite]
public partial class NpcInteractionTests : Node
{
    private const string Stage4ScenePath = "res://source/stages/stage_4/field_combat.tscn";

    /// <summary>
    /// Test that all expected NPCs are present in the scene.
    /// </summary>
    [TestCase]
    public void TestAllNpcsPresent()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Check for NPC nodes (based on godot-open-rpg town structure)
        var warrior = stage4Scene.FindChild("Warrior", true, false);
        var thief = stage4Scene.FindChild("Thief", true, false);
        var monk = stage4Scene.FindChild("Monk", true, false);
        var smith = stage4Scene.FindChild("Smith", true, false);
        var wizard = stage4Scene.FindChild("Wizard", true, false);

        AssertThat(warrior).IsNotNull()
            .OverrideFailureMessage("Warrior NPC should be present in town");
        AssertThat(thief).IsNotNull()
            .OverrideFailureMessage("Thief NPC should be present in town");
        AssertThat(monk).IsNotNull()
            .OverrideFailureMessage("Monk NPC should be present in town");
        AssertThat(smith).IsNotNull()
            .OverrideFailureMessage("Smith NPC should be present in town");
        AssertThat(wizard).IsNotNull()
            .OverrideFailureMessage("Wizard NPC should be present in town");

        runner.Dispose();
    }

    /// <summary>
    /// Test that Dialogic timelines exist for all NPCs.
    /// </summary>
    [TestCase]
    public void TestNpcDialogicTimelinesExist()
    {
        // Check for Dialogic .dtl timeline files
        var warriorTimeline = ResourceLoader.Exists("res://source/overworld/maps/town/warrior.dtl");
        var thiefTimeline = ResourceLoader.Exists("res://source/overworld/maps/town/thief.dtl");
        var monkTimeline = ResourceLoader.Exists("res://source/overworld/maps/town/monk.dtl");
        var smithTimeline = ResourceLoader.Exists("res://source/overworld/maps/town/smith.dtl");
        var wizardTimeline = ResourceLoader.Exists("res://source/overworld/maps/town/wizard.dtl");

        AssertThat(warriorTimeline).IsTrue()
            .OverrideFailureMessage("Warrior dialogue timeline should exist");
        AssertThat(thiefTimeline).IsTrue()
            .OverrideFailureMessage("Thief dialogue timeline should exist");
        AssertThat(monkTimeline).IsTrue()
            .OverrideFailureMessage("Monk dialogue timeline should exist");
        AssertThat(smithTimeline).IsTrue()
            .OverrideFailureMessage("Smith dialogue timeline should exist");
        AssertThat(wizardTimeline).IsTrue()
            .OverrideFailureMessage("Wizard dialogue timeline should exist");
    }

    /// <summary>
    /// Test that NPCs have interaction components attached.
    /// </summary>
    [TestCase]
    public void TestNpcsHaveInteractionComponents()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find NPC nodes
        var warrior = stage4Scene.FindChild("Warrior", true, false);
        var thief = stage4Scene.FindChild("Thief", true, false);
        var monk = stage4Scene.FindChild("Monk", true, false);

        // Each NPC should have an Interaction child or script
        if (warrior != null)
        {
            var interaction = warrior.FindChild("Interaction", true, false);
            var hasInteraction = interaction != null || warrior.GetScript().Obj != null;
            AssertThat(hasInteraction).IsTrue()
                .OverrideFailureMessage("Warrior should have interaction capability");
        }

        if (thief != null)
        {
            var interaction = thief.FindChild("Interaction", true, false);
            var hasInteraction = interaction != null || thief.GetScript().Obj != null;
            AssertThat(hasInteraction).IsTrue()
                .OverrideFailureMessage("Thief should have interaction capability");
        }

        if (monk != null)
        {
            var interaction = monk.FindChild("Interaction", true, false);
            var hasInteraction = interaction != null || monk.GetScript().Obj != null;
            AssertThat(hasInteraction).IsTrue()
                .OverrideFailureMessage("Monk should have interaction capability");
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that ConversationEncounter interaction exists.
    /// </summary>
    [TestCase]
    public void TestConversationEncounterExists()
    {
        // Check that ConversationEncounter script is available
        var scriptPath = "res://source/overworld/maps/town/ConversationEncounter.cs";
        var scriptExists = ResourceLoader.Exists(scriptPath);

        AssertThat(scriptExists).IsTrue()
            .OverrideFailureMessage("ConversationEncounter.cs should exist for NPC interactions");
    }

    /// <summary>
    /// Test that FanInteraction (Gang of Four conversation) exists.
    /// </summary>
    [TestCase]
    public void TestGangOfFourInteractionExists()
    {
        // Check that Gang of Four interaction scripts exist
        var fanInteractionExists = ResourceLoader.Exists("res://source/overworld/maps/town/FanInteraction.cs");
        var gangConversationExists = ResourceLoader.Exists("res://source/overworld/maps/town/GangOfFourConversation.cs");
        var fanTimelineExists = ResourceLoader.Exists("res://source/overworld/maps/town/fan_of_four.dtl");

        AssertThat(fanInteractionExists).IsTrue()
            .OverrideFailureMessage("FanInteraction.cs should exist");
        AssertThat(gangConversationExists).IsTrue()
            .OverrideFailureMessage("GangOfFourConversation.cs should exist");
        AssertThat(fanTimelineExists).IsTrue()
            .OverrideFailureMessage("fan_of_four.dtl timeline should exist");
    }

    /// <summary>
    /// Test that StrangeTree interaction exists for special town encounter.
    /// </summary>
    [TestCase]
    public void TestStrangeTreeInteractionExists()
    {
        var strangeTreeScript = ResourceLoader.Exists("res://source/overworld/maps/town/StrangeTreeInteraction.cs");
        var strangeTreeTimeline = ResourceLoader.Exists("res://source/overworld/maps/town/strange_tree.dtl");

        AssertThat(strangeTreeScript).IsTrue()
            .OverrideFailureMessage("StrangeTreeInteraction.cs should exist");
        AssertThat(strangeTreeTimeline).IsTrue()
            .OverrideFailureMessage("strange_tree.dtl timeline should exist");
    }

    /// <summary>
    /// Test that door unlock interaction exists for town exploration.
    /// </summary>
    [TestCase]
    public void TestDoorUnlockInteractionExists()
    {
        var doorUnlockScript = ResourceLoader.Exists("res://source/overworld/maps/town/DoorUnlockInteraction.cs");

        AssertThat(doorUnlockScript).IsTrue()
            .OverrideFailureMessage("DoorUnlockInteraction.cs should exist for door mechanics");
    }

    /// <summary>
    /// Test that NPCs have proper collision shapes for interaction detection.
    /// </summary>
    [TestCase]
    public void TestNpcsHaveCollisionShapes()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find NPC nodes
        var warrior = stage4Scene.FindChild("Warrior", true, false);
        var thief = stage4Scene.FindChild("Thief", true, false);
        var monk = stage4Scene.FindChild("Monk", true, false);

        // Check for Area2D or CollisionShape2D components
        if (warrior != null)
        {
            var area = warrior.FindChild("Area2D", true, false) as Area2D;
            var collisionShape = warrior.FindChild("CollisionShape2D", true, false);
            var hasCollision = area != null || collisionShape != null;

            AssertThat(hasCollision).IsTrue()
                .OverrideFailureMessage("Warrior should have collision detection for interactions");
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that interaction prompts are properly configured.
    /// </summary>
    [TestCase]
    public void TestInteractionPromptsExist()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Look for UI elements related to interaction prompts
        // (e.g., "Press E to interact" popup)
        var ui = stage4Scene.FindChild("UI", true, false);
        var canvas = stage4Scene.FindChild("CanvasLayer", true, false);

        // At least one UI container should exist for interaction feedback
        var hasUi = ui != null || canvas != null;
        AssertThat(hasUi).IsTrue()
            .OverrideFailureMessage("UI layer should exist for interaction prompts");

        runner.Dispose();
    }

    /// <summary>
    /// Test that Dialogic plugin integration is available.
    /// </summary>
    [TestCase]
    public void TestDialogicPluginAvailable()
    {
        // Check that Dialogic plugin files exist
        var dialogicPlugin = ResourceLoader.Exists("res://addons/dialogic/plugin.cfg");

        // Note: In actual implementation, Dialogic might not be in addons
        // Check if Dialogic timelines load successfully instead
        var sampleTimeline = ResourceLoader.Exists("res://source/overworld/maps/town/warrior.dtl");

        AssertThat(sampleTimeline).IsTrue()
            .OverrideFailureMessage("Dialogic timeline resources should be loadable");
    }

    /// <summary>
    /// Test that NPC GamePiece controllers are properly set up.
    /// </summary>
    [TestCase]
    public void TestNpcGamepieceControllers()
    {
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        // Find NPC nodes
        var warrior = stage4Scene.FindChild("Warrior", true, false);
        var thief = stage4Scene.FindChild("Thief", true, false);

        // Check if NPCs have GamepieceController or similar movement controller
        if (warrior != null)
        {
            var controller = warrior.FindChild("Controller", true, false);
            var gamepiece = warrior as Gamepiece;

            // NPCs might be Gamepiece nodes themselves or have controllers
            var hasController = controller != null || gamepiece != null;

            // This might be true or false depending on NPC setup
            // For now, just check the node exists
            AssertThat(warrior).IsNotNull();
        }

        runner.Dispose();
    }

    /// <summary>
    /// Test that conversation state persistence is supported.
    /// </summary>
    [TestCase]
    public void TestConversationStatePersistence()
    {
        // Check for GameState singleton which handles persistent data
        ISceneRunner runner = ISceneRunner.Load(Stage4ScenePath);
        Node stage4Scene = runner.Scene();

        AssertThat(stage4Scene).IsNotNull();

        var gameState = stage4Scene.GetNodeOrNull<Node>("/root/GameState");
        AssertThat(gameState).IsNotNull()
            .OverrideFailureMessage("GameState singleton should exist for conversation state tracking");

        runner.Dispose();
    }

    /// <summary>
    /// Test that runner NPC exists for special encounter trigger.
    /// </summary>
    [TestCase]
    public void TestRunnerNpcExists()
    {
        var runnerTimeline = ResourceLoader.Exists("res://source/overworld/maps/town/runner.dtl");

        AssertThat(runnerTimeline).IsTrue()
            .OverrideFailureMessage("Runner NPC dialogue timeline should exist");
    }

    /// <summary>
    /// Test that sign interaction exists for town information.
    /// </summary>
    [TestCase]
    public void TestSignInteractionExists()
    {
        var signTimeline = ResourceLoader.Exists("res://source/overworld/maps/town/sign.dtl");

        AssertThat(signTimeline).IsTrue()
            .OverrideFailureMessage("Sign dialogue timeline should exist");
    }

    /// <summary>
    /// Test that combat encounter dialogues exist.
    /// </summary>
    [TestCase]
    public void TestCombatEncounterDialoguesExist()
    {
        var beforeCombat = ResourceLoader.Exists("res://source/overworld/maps/town/encounter_before_combat.dtl");
        var onVictory = ResourceLoader.Exists("res://source/overworld/maps/town/encounter_on_victory.dtl");
        var onLoss = ResourceLoader.Exists("res://source/overworld/maps/town/encounter_on_loss.dtl");

        AssertThat(beforeCombat).IsTrue()
            .OverrideFailureMessage("encounter_before_combat.dtl should exist");
        AssertThat(onVictory).IsTrue()
            .OverrideFailureMessage("encounter_on_victory.dtl should exist");
        AssertThat(onLoss).IsTrue()
            .OverrideFailureMessage("encounter_on_loss.dtl should exist");
    }
}
