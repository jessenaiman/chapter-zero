// <copyright file="HouseSceneTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Field;

using System.IO;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class HouseSceneTests
{
    private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private static string ResolveProjectPath(string relativePath) => Path.Combine(ProjectRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

    private static string HouseScenePath => ResolveProjectPath("Source/overworld/maps/house/wand_pedestal_interaction.tscn");

    [TestCase]
    public void HouseScene_ExistsAndIsValid()
    {
        AssertThat(File.Exists(HouseScenePath)).IsTrue();

        var sceneText = File.ReadAllText(HouseScenePath);
        AssertThat(sceneText).IsNotEmpty();
        AssertThat(sceneText.Contains("res://Source/Scripts/field/cutscenes/Interaction.tscn")).IsTrue();
        AssertThat(sceneText.Contains("res://Source/overworld/maps/house/WandPedestalInteraction.cs")).IsTrue();
    }
}
