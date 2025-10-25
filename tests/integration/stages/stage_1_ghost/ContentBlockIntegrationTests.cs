// <copyright file="ContentBlockIntegrationTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage1;

using Godot;
using GdUnit4;
using System.Threading.Tasks;
using static GdUnit4.Assertions;

/// <summary>
/// Integration test suite for Stage 1 content block scene loading and runtime behavior.
/// These tests require the Godot runtime and verify scene initialization with the full engine.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public static class ContentBlockIntegrationTests
{

    /// <summary>
    /// Tests that the stage's opening/boot scene loads successfully in the Godot runtime.
    /// </summary>
    [TestCase]
    public static async Task BootSequenceSceneLoadsSuccessfully()
    {
        using var runner = ISceneRunner.Load("res://source/stages/ghost/scenes/boot_sequence.tscn");
        await runner.SimulateFrames(1).ConfigureAwait(false);

        AssertThat(runner).IsNotNull();
    }

    /// <summary>
    /// Tests that the opening monologue scene loads successfully in the Godot runtime.
    /// </summary>
    [TestCase]
    public static async Task OpeningMonologueSceneLoadsSuccessfully()
    {
        using var runner = ISceneRunner.Load("res://source/stages/ghost/scenes/opening_monologue.tscn");
        await runner.SimulateFrames(1).ConfigureAwait(false);

        AssertThat(runner).IsNotNull();
    }

    /// <summary>
    /// Tests that the first question scene loads successfully in the Godot runtime.
    /// </summary>
    [TestCase]
    public static async Task QuestionSceneLoadsSuccessfully()
    {
        using var runner = ISceneRunner.Load("res://source/stages/ghost/scenes/question_1_name.tscn");
        await runner.SimulateFrames(1).ConfigureAwait(false);

        AssertThat(runner).IsNotNull();
    }

    /// <summary>
    /// Tests that all Stage 1 scenes can be loaded simultaneously without conflicts.
    /// Verifies that the scene runner handles multiple scene instances correctly.
    /// </summary>
    [TestCase]
    public static async Task AllStage1ScenesLoadConcurrently()
    {
        using var runnerBoot = ISceneRunner.Load("res://source/stages/ghost/scenes/boot_sequence.tscn");
        await runnerBoot.SimulateFrames(1).ConfigureAwait(false);

        using var runnerOpening = ISceneRunner.Load("res://source/stages/ghost/scenes/opening_monologue.tscn");
        await runnerOpening.SimulateFrames(1).ConfigureAwait(false);

        using var runnerQuestion = ISceneRunner.Load("res://source/stages/ghost/scenes/question_1_name.tscn");
        await runnerQuestion.SimulateFrames(1).ConfigureAwait(false);

        AssertThat(runnerBoot).IsNotNull();
        AssertThat(runnerOpening).IsNotNull();
        AssertThat(runnerQuestion).IsNotNull();
    }
}
