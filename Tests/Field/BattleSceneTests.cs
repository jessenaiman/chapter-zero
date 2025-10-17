// <copyright file="BattleSceneTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Field;

using System.IO;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class BattleSceneTests
{
    private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private static string ResolveProjectPath(string relativePath) => Path.Combine(ProjectRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

    private static string BattleScenePath => ResolveProjectPath("Source/overworld/maps/town/battles/test_combat_arena.tscn");

    [TestCase]
    public void BattleScene_ExistsAndIsValid()
    {
        AssertThat(File.Exists(BattleScenePath)).IsTrue();

        var sceneText = File.ReadAllText(BattleScenePath);
        AssertThat(sceneText).IsNotEmpty();
    }
}
