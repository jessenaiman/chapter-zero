// <copyright file="TownSceneTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Field;

using System.IO;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class TownSceneTests
{
    private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private static string ResolveProjectPath(string relativePath) => Path.Combine(ProjectRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

    private static string TownMapPath => ResolveProjectPath("Source/overworld/maps/town");

    [TestCase]
    public void TownDirectory_ContainsExpectedFiles()
    {
        AssertThat(Directory.Exists(TownMapPath)).IsTrue();

        var files = Directory.GetFiles(TownMapPath);
        AssertThat(files).IsNotEmpty();
        AssertThat(files).Contains(ResolveProjectPath("Source/overworld/maps/town/ConversationEncounter.cs"));
    }
}
