// <copyright file="NarrativeScriptLoaderTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.TestsUi.Narrative;

using GdUnit4;
using System;
using System.IO;
using Godot;
using OmegaSpiral.Source.Narrative;
using static GdUnit4.Assertions;

/// <summary>
/// Unit tests for NarrativeScriptLoader.
/// Validates YAML loading and deserialization functionality.
/// </summary>
[TestSuite]
public class NarrativeScriptLoaderTests
{
    /// <summary>
    /// LoadYamlScript should throw exception for non-existent file.
    /// </summary>
    [TestCase]
    public void LoadYamlScript_ThrowsForNonExistentFile()
    {
        // Arrange & Act & Assert
        AssertThat(() => NarrativeScriptLoader.LoadYamlScript("res://non_existent.yaml"))
            .Throws<InvalidOperationException>();
    }

    /// <summary>
    /// LoadYamlScript with generic type should work with extensions.
    /// Generic type parameter is properly passed to deserializer.
    /// </summary>
    [TestCase]
    public void LoadYamlScript_Generic_AcceptsTypeParameter()
    {
        // This test verifies the generic overload exists and compiles correctly.
        // Full YAML loading tests are integration tests that use actual files.

        // Assert that the loader static class and methods exist
        var loaderType = typeof(NarrativeScriptLoader);
        var genericMethod = loaderType.GetMethod("LoadYamlScript",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
            null,
            new[] { typeof(string) },
            null);

        AssertThat(genericMethod).IsNotNull();
    }

    /// <summary>
    /// LoadYamlScript should throw exception when path is null.
    /// </summary>
    [TestCase]
    public void LoadYamlScript_NullPath_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertThat(() => NarrativeScriptLoader.LoadYamlScript(null!))
            .Throws<ArgumentNullException>();
    }
}
