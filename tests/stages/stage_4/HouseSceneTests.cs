// <copyright file="HouseSceneTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

#pragma warning disable SA1202
#pragma warning disable SA1401
#pragma warning disable SA1403

namespace OmegaSpiral.Tests.Field;

using System.IO;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Test suite for validating house scene configuration and interactions.
/// Tests verify that house scenes exist, are properly formatted, and contain expected interaction components.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class HouseSceneTests
{
    private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private static string HouseScenePath => ResolveProjectPath("Source/overworld/maps/house/wand_pedestal_interaction.tscn");

    private static string ResolveProjectPath(string relativePath) => Path.Combine(ProjectRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

    /// <summary>
    /// Tests that the house scene file exists and contains valid content.
    /// Verifies that the wand pedestal interaction scene is properly configured and includes required components.
    /// </summary>
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
