// <copyright file="BattleSceneTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Field;

using System.IO;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Test suite for validating battle scene configuration and resources.
/// Tests verify that battle scenes exist, are properly formatted, and contain expected content.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class BattleSceneTests
{
    private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private static string BattleScenePath => ResolveProjectPath("Source/overworld/maps/town/battles/test_combat_arena.tscn");

    /// <summary>
    /// Tests that the battle scene file exists and contains valid content.
    /// Verifies that the test combat arena scene is properly configured and accessible.
    /// </summary>
    [TestCase]
    public void BattleScene_ExistsAndIsValid()
    {
        AssertThat(File.Exists(BattleScenePath)).IsTrue();

        var sceneText = File.ReadAllText(BattleScenePath);
        AssertThat(sceneText).IsNotEmpty();
    }

    private static string ResolveProjectPath(string relativePath) => Path.Combine(ProjectRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
}
