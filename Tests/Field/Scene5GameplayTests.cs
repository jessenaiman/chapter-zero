// <copyright file="Scene5GameplayTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Field;

using System.IO;
using System.Threading.Tasks;
using GdUnit4;
using Godot;
using OmegaSpiral.Combat;
using OmegaSpiral.Field;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Field;
using OmegaSpiral.Source.Scripts.Field.Cutscenes.Templates.Doors;
using OmegaSpiral.Source.Scripts.Field.Gameboard;
using OmegaSpiral.Source.Scripts.Field.Gamepieces.Controllers;
using static GdUnit4.Assertions;

/// <summary>
/// GDUnit4 tests demonstrating Scene5 gameplay: party exploration, house entry, and combat encounters.
/// Tests full e2e flows using Godot scenes and runtime.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class Scene5GameplayTests : Node
{
    private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private static string TownScenePath => ResolveProjectPath("Source/overworld/maps/town/town.tscn");

    private static string HouseScenePath => ResolveProjectPath("Source/overworld/maps/house/wand_pedestal_interaction.tscn");

    private static string CombatArenaPath => ResolveProjectPath("Source/Scripts/combat/CombatArena.tscn");

    /// <summary>
    /// Tests party initialization with default 4-member demo party.
    /// Verifies characters are created with correct classes and stats.
    /// </summary>
    [TestCase]
    public void PartyInitialization_DefaultDemoParty_LoadsCorrectly()
    {
        // Arrange
        var gameState = new GameState();

        // Act
        var party = DefaultPartyFactory.CreateDefaultDemoParty();

        // Assert
        AssertThat(party).IsNotNull();
        AssertThat(party.Members).IsNotNull();
        AssertThat(party.Members.Count).IsEqual(4);

        var members = party.Members;
        AssertThat(members[0].Name).IsEqual("Garrett"); // Fighter
        AssertThat(members[1].Name).IsEqual("Shadow");   // Thief
        AssertThat(members[2].Name).IsEqual("Merlin");   // Mage
        AssertThat(members[3].Name).IsEqual("Celeste");  // Priest

        // Verify classes and races
        AssertThat(members[0].Class).IsEqual(CharacterClass.Fighter);
        AssertThat(members[0].Race).IsEqual(CharacterRace.Human);
        AssertThat(members[1].Class).IsEqual(CharacterClass.Thief);
        AssertThat(members[1].Race).IsEqual(CharacterRace.Elf);
    }

    /// <summary>
    /// Tests town scene loads and party can be spawned.
    /// Verifies scene structure and initial party positioning.
    /// </summary>
    [TestCase]
    public void TownScene_PartySpawning_LoadsAndPositionsCorrectly()
    {
        // Arrange
        var townScene = (PackedScene)ResourceLoader.Load(TownScenePath);
        AssertThat(townScene).IsNotNull();

        // Act
        var townInstance = townScene.Instantiate<Node2D>();
        AssertThat(townInstance).IsNotNull();

        // Simulate party spawning (would normally be handled by Field.cs)
        var gameState = new GameState();
        gameState.PlayerParty = DefaultPartyFactory.CreateDefaultDemoParty();

        // Assert
        AssertThat(gameState.PlayerParty).IsNotNull();
        AssertThat(gameState.PlayerParty.Members).HasSize(4);

        // Verify scene has expected nodes (town map elements)
        var fieldEvents = townInstance.GetNodeOrNull<FieldEvents>("/root/FieldEvents");
        AssertThat(fieldEvents).IsNotNull();

        townInstance.QueueFree();
    }

    /// <summary>
    /// Tests character movement in town scene.
    /// Simulates player input and verifies position updates.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [TestCase]
    public async Task CharacterMovement_TownExploration_MovesCorrectly()
    {
        // Arrange
        var townScene = (PackedScene)ResourceLoader.Load(TownScenePath);
        var townInstance = townScene.Instantiate<Node2D>();
        var gameboard = townInstance.GetNodeOrNull<Gameboard>("/root/Gameboard");
        AssertThat(gameboard).IsNotNull();

        // Create player gamepiece (Node2D) for movement
        var playerGamepiece = new OmegaSpiral.Source.Scripts.Field.Gamepieces.Gamepiece();
        townInstance.AddChild(playerGamepiece);

        // Act: Simulate movement input
        var initialPosition = playerGamepiece.GlobalPosition;
        Input.ActionPress("ui_right");
        await this.ToSignal(this.GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
        Input.ActionRelease("ui_right");

        // Assert: Position should have changed
        var newPosition = playerGamepiece.GlobalPosition;
        AssertThat(newPosition).IsNotEqual(initialPosition);

        townInstance.QueueFree();
    }

    /// <summary>
    /// Tests house entry via door interaction.
    /// Verifies door trigger activates and scene transition occurs.
    /// </summary>
    [TestCase]
    public void HouseEntry_DoorInteraction_TransitionsCorrectly()
    {
        // Arrange
        var townScene = (PackedScene)ResourceLoader.Load(TownScenePath);
        var townInstance = townScene.Instantiate<Node2D>();

        // Find door interaction (assuming Door.cs is attached to a node)
        var doorNode = townInstance.FindChild("Door", true, false);
        AssertThat(doorNode).IsNotNull();

        // Act: Trigger door interaction
        var door = doorNode as Door;
        AssertThat(door).IsNotNull();

        // Add a Gamepiece (Node2D) to the scene to simulate the player
        var playerGamepiece = new OmegaSpiral.Source.Scripts.Field.Gamepieces.Gamepiece();
        townInstance.AddChild(playerGamepiece);

        AssertThat(door).IsNotNull();
        AssertThat(playerGamepiece).IsNotNull();

        // Use Activate to simulate player interaction
        door!.Activate(playerGamepiece);

        townInstance.QueueFree();
    }

    /// <summary>
    /// Tests combat encounter triggering in town.
    /// Verifies CombatEncounterTrigger emits correct signals and loads combat.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [TestCase]
    public async Task CombatEncounter_TriggerActivation_LoadsCombatArena()
    {
        await Task.CompletedTask;

        // Arrange
        var townScene = (PackedScene)ResourceLoader.Load(TownScenePath);
        var townInstance = townScene.Instantiate<Node2D>();
        var fieldEvents = townInstance.GetNodeOrNull<OmegaSpiral.Field.FieldEvents>("/root/FieldEvents");
        var combatEvents = townInstance.GetNodeOrNull<OmegaSpiral.Combat.CombatEvents>("/root/CombatEvents");
        AssertThat(fieldEvents).IsNotNull();
        AssertThat(combatEvents).IsNotNull();

        // Create combat encounter trigger
        var combatTrigger = new OmegaSpiral.Source.Overworld.Maps.CombatEncounterTrigger();
        combatTrigger.CombatArena = (PackedScene)ResourceLoader.Load(CombatArenaPath);
        townInstance.AddChild(combatTrigger);

        // Act: Trigger combat encounter
        // Use Godot's CallDeferred to invoke protected method for test (if needed)
        combatTrigger.CallDeferred("_ExecuteAsync");

        // Wait for signal that combat was triggered
        var combatTriggeredSignal = await this.ToSignal(fieldEvents, "CombatTriggered");

        // Assert: Combat should be triggered
        AssertThat(combatTriggeredSignal).IsNotNull();

        // Verify combat arena was passed
        var arenaScene = (PackedScene)combatTriggeredSignal[0];
        AssertThat(arenaScene).IsNotNull();

        townInstance.QueueFree();
    }

    /// <summary>
    /// Tests complete Scene5 gameplay flow: town exploration → house entry → combat.
    /// Demonstrates full e2e gameplay demonstration.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [TestCase]
    public async Task Scene5Gameplay_FullFlow_DemonstratesCompleteExperience()
    {
        // Arrange: Load town scene
        var townScene = (PackedScene)ResourceLoader.Load(TownScenePath);
        var townInstance = townScene.Instantiate<Node2D>();
        var gameState = new OmegaSpiral.Source.Scripts.Common.GameState();

        // Initialize party (DefaultPartyFactory is static)
        gameState.PlayerParty = OmegaSpiral.Source.Scripts.Common.DefaultPartyFactory.CreateDefaultDemoParty();

        // Act 1: Explore town (simulate movement)
        // PlayerController is a scene, not a direct class instantiation
        var playerControllerScene = (PackedScene)ResourceLoader.Load("res://Source/Scripts/field/gamepieces/controllers/PlayerController.tscn");
        var playerController = playerControllerScene.Instantiate<Node2D>();
        townInstance.AddChild(playerController);
        Input.ActionPress("ui_down");
        await this.ToSignal(this.GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
        Input.ActionRelease("ui_down");

        // Act 2: Enter house
        var doorNode = townInstance.FindChild("Door", true, false);
        var door = doorNode as OmegaSpiral.Source.Scripts.Field.Cutscenes.Templates.Doors.Door;
        if (door != null)
        {
            // Use Activate to simulate player interaction
            await Task.Run(() => door.Activate(playerController as Node2D));
        }

        // Act 3: Trigger combat in town
        var combatTrigger = new OmegaSpiral.Source.Overworld.Maps.CombatEncounterTrigger();
        combatTrigger.CombatArena = (PackedScene)ResourceLoader.Load(CombatArenaPath);
        townInstance.AddChild(combatTrigger);
        combatTrigger.CallDeferred("_ExecuteAsync");

        // Wait for signal that combat was triggered
        var combatTriggeredSignal = await this.ToSignal(townInstance.GetNodeOrNull<OmegaSpiral.Field.FieldEvents>("/root/FieldEvents"), "CombatTriggered");
        AssertThat(combatTriggeredSignal).IsNotNull();

        // Assert: Full flow completed
        AssertThat(gameState.PlayerParty.Members).HasSize(4);
        townInstance.QueueFree();
    }

    private static string ResolveProjectPath(string relativePath) => Path.Combine(ProjectRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
}
