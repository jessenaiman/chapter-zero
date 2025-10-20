// <copyright file="Stage4InitializationTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage4;

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

/// <summary>
/// Tests for Stage 4 initialization and scene loading.
/// </summary>
[TestSuite]
public partial class Stage4InitializationTests
{
    private const string Stage4ScenePath = "res://source/stages/stage_4/field_combat.tscn";

    /// <summary>
    /// Verifies that the Stage 4 scene file exists.
    /// </summary>
    [TestCase]
    public void TestStage4SceneExists()
    {
        AssertThat(ResourceLoader.Exists(Stage4ScenePath)).IsTrue();
    }

    /// <summary>
    /// Verifies that Stage 4 scene loads without errors.
    /// </summary>
    [TestCase]
    public void TestStage4SceneLoads()
    {
        var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);
        AssertThat(scene).IsNotNull();
    }

    /// <summary>
    /// Verifies that Stage 4 scene can be instantiated.
    /// </summary>
    [TestCase]
    public void TestStage4SceneInstantiates()
    {
        var scene = ResourceLoader.Load<PackedScene>(Stage4ScenePath);
        var instance = scene.Instantiate();
        AssertThat(instance).IsNotNull();
        if (instance is Node node)
        {
            node.QueueFree();
        }
    }
}
