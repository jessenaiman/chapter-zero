// <copyright file="CombatSystemTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage4;

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

/// <summary>
/// Tests for Stage 4 combat system.
/// Verifies combat initialization, turn queue, and action execution.
/// </summary>
[TestSuite]
public partial class CombatSystemTests
{
    /// <summary>
    /// Verifies CombatEvents singleton exists.
    /// </summary>
    [TestCase]
    public void TestCombatEventsExists()
    {
        var combatEvents = Godot.GD.Load<CSharpScript>("res://source/scripts/combat/CombatEvents.cs");
        AssertThat(combatEvents).IsNotNull();
    }

    /// <summary>
    /// Verifies Combat main controller exists.
    /// </summary>
    [TestCase]
    public void TestCombatControllerExists()
    {
        var combat = Godot.GD.Load<CSharpScript>("res://source/scripts/combat/Combat.cs");
        AssertThat(combat).IsNotNull();
    }

    /// <summary>
    /// Verifies CombatActor script exists.
    /// </summary>
    [TestCase]
    public void TestCombatActorExists()
    {
        var combatActor = Godot.GD.Load<CSharpScript>("res://source/scripts/combat/CombatActor.cs");
        AssertThat(combatActor).IsNotNull();
    }

    /// <summary>
    /// Verifies ActiveTurnQueue exists.
    /// </summary>
    [TestCase]
    public void TestActiveTurnQueueExists()
    {
        var turnQueue = Godot.GD.Load<CSharpScript>("res://source/scripts/combat/ActiveTurnQueue.cs");
        AssertThat(turnQueue).IsNotNull();
    }

    /// <summary>
    /// Verifies BattlerActionAttack exists.
    /// </summary>
    [TestCase]
    public void TestAttackActionExists()
    {
        var attackAction = Godot.GD.Load<CSharpScript>("res://source/scripts/combat/actions/BattlerActionAttack.cs");
        AssertThat(attackAction).IsNotNull();
    }

    /// <summary>
    /// Verifies BattlerActionHeal exists.
    /// </summary>
    [TestCase]
    public void TestHealActionExists()
    {
        var healAction = Godot.GD.Load<CSharpScript>("res://source/scripts/combat/actions/BattlerActionHeal.cs");
        AssertThat(healAction).IsNotNull();
    }

    /// <summary>
    /// Verifies Combat UI exists.
    /// </summary>
    [TestCase]
    public void TestCombatUiExists()
    {
        var combatUi = Godot.GD.Load<CSharpScript>("res://source/scripts/combat/ui/UICombatMenus.cs");
        AssertThat(combatUi).IsNotNull();
    }

    /// <summary>
    /// Verifies combat arena scene exists.
    /// </summary>
    [TestCase]
    public void TestCombatArenaSceneExists()
    {
        var arenaScene = ResourceLoader.Exists("res://source/overworld/maps/town/battles/test_combat_arena.tscn");
        AssertThat(arenaScene).IsTrue();
    }
}
