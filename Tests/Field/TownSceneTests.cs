// <copyright file="TownSceneTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

#pragma warning disable SA1636

namespace OmegaSpiral.Tests.Field;

using System.IO;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Test suite for validating town scene configuration and directory structure.
/// Tests verify that town directories exist, are properly organized, and contain expected files.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class TownSceneTests
{
    private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private static string TownMapPath => ResolveProjectPath("Source/overworld/maps/town");

    /// <summary>
    /// Tests that the town directory exists and contains expected files.
    /// Verifies that the town map directory is properly configured and includes required conversation encounter scripts.
    /// </summary>
    [TestCase]
    public void TownDirectory_ContainsExpectedFiles()
    {
        AssertThat(Directory.Exists(TownMapPath)).IsTrue();

        var files = Directory.GetFiles(TownMapPath);
        AssertThat(files).IsNotEmpty();
        AssertThat(files).Contains(ResolveProjectPath("Source/overworld/maps/town/ConversationEncounter.cs"));
    }

    private static string ResolveProjectPath(string relativePath) => Path.Combine(ProjectRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
}
