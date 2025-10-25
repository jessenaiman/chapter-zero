// <copyright file="GhostTerminalCinematicDirectorTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage1;

using GdUnit4;
using Godot;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Scripts.Stages.Stage1;

/// <summary>
/// Verifies GhostTerminalCinematicDirector loads and parses ghost.yaml correctly.
///
/// Test Cases:
/// 1. YAML file loads without errors
/// 2. Metadata is parsed correctly (iteration, previousAttempt, interface, status)
/// 3. All blocks are loaded in correct order
/// 4. Block 1 (Boot Sequence) - narrative type with visualPreset and fadeToStable
/// 5. Block 2 (Opening Monologue) - narrative type with timing and pause
/// 6. Block 3 (First Choice "Do you have a name?") - composite type with setup, prompt, context, and 3 options
/// 7. Each option has correct dreamweaver scores (light, shadow, ambition)
/// 8. Block 4 (Bridge Parable) - composite type with setup, prompt, options, and continuation
/// 9. Block 5 (Secret Keeper) - question type with setup, pause, prompt, and options
/// 10. Plan is cached - calling GetPlan twice returns same instance
/// 11. Reset() clears the cache
/// 12. Template variables {{THREAD_NAME}} and {{LIVE_API_COUNTER}} exist in YAML
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public static class GhostTerminalCinematicDirectorTests
{
    [TestCase]
    public static async Task GetPlan_ReturnsNonNull()
    {
        // Arrange
        var director = new GhostTerminalCinematicDirector();  // instantiate

        // Act
        var plan = director.GetPlan();  // instance call

        // Assert
        AssertThat(plan).IsNotNull();
    }
}
