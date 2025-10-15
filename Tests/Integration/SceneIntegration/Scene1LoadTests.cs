// <copyright file="Scene1LoadTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Integration.SceneIntegration
{
    using System;
    using Godot;
    using NUnit.Framework;
    using GodotFileAccess = Godot.FileAccess;

    /// <summary>
    /// Integration tests for Scene1Narrative to verify the first scene can load and function correctly.
    /// This validates that we have a working product.
    /// </summary>
    [TestFixture]
    public partial class Scene1LoadTests : GodotObject
    {
        /// <summary>
        /// Verifies that the Scene1Narrative.tscn file exists in the expected location.
        /// </summary>
        /// <remarks>
        /// Ensures the narrative scene file is present for loading and testing.
        /// </remarks>
        [Test]
        public void Scene1_SceneFileExists()
        {
            // Arrange
            string scenePath = "res://Source/Scenes/Scene1Narrative.tscn";

            // Act
            bool exists = ResourceLoader.Exists(scenePath);

            // Assert
            Assert.That(exists, Is.True, $"Scene1Narrative.tscn should exist at {scenePath}");
        }

        /// <summary>
        /// Checks that the hero.json data file exists for Scene1.
        /// </summary>
        /// <remarks>
        /// Validates the presence of hero narrative data for Scene1.
        /// </remarks>
        [Test]
        public void Scene1_HeroDataFileExists()
        {
            // Arrange
            string dataPath = "res://Source/Data/scenes/scene1_narrative/hero.json";

            // Act
            bool exists = GodotFileAccess.FileExists(dataPath);

            // Assert
            Assert.That(exists, Is.True, $"Hero narrative data should exist at {dataPath}");
        }

        /// <summary>
        /// Checks that the shadow.json data file exists for Scene1.
        /// </summary>
        /// <remarks>
        /// Validates the presence of shadow narrative data for Scene1.
        /// </remarks>
        [Test]
        public void Scene1_ShadowDataFileExists()
        {
            // Arrange
            string dataPath = "res://Source/Data/scenes/scene1_narrative/shadow.json";

            // Act
            bool exists = GodotFileAccess.FileExists(dataPath);

            // Assert
            Assert.That(exists, Is.True, $"Shadow narrative data should exist at {dataPath}");
        }

        /// <summary>
        /// Checks that the ambition.json data file exists for Scene1.
        /// </summary>
        /// <remarks>
        /// Validates the presence of ambition narrative data for Scene1.
        /// </remarks>
        [Test]
        public void Scene1_AmbitionDataFileExists()
        {
            // Arrange
            string dataPath = "res://Source/Data/scenes/scene1_narrative/ambition.json";

            // Act
            bool exists = GodotFileAccess.FileExists(dataPath);

            // Assert
            Assert.That(exists, Is.True, $"Ambition narrative data should exist at {dataPath}");
        }

        /// <summary>
        /// Verifies that the hero.json data file for Scene1 can be parsed as valid JSON.
        /// </summary>
        /// <remarks>
        /// Ensures the hero narrative data structure is correct and parseable.
        /// </remarks>
        [Test]
        public void Scene1_HeroDataCanBeParsed()
        {
            // Arrange
            string dataPath = "res://Source/Data/scenes/scene1_narrative/hero.json";

            // Act
            string jsonData = GodotFileAccess.GetFileAsString(dataPath);
            System.Text.Json.JsonDocument? doc = null;
            Exception? parseException = null;

            try
            {
                doc = System.Text.Json.JsonDocument.Parse(jsonData);
            }
            catch (Exception ex)
            {
                parseException = ex;
            }

            // Assert
            Assert.That(parseException, Is.Null, $"Hero JSON should parse without errors: {parseException?.Message}");
            Assert.That(doc, Is.Not.Null, "Parsed JSON document should not be null");

            // Verify required fields exist
            if (doc != null)
            {
                Assert.That(doc.RootElement.TryGetProperty("type", out var typeProperty), Is.True, "JSON should have 'type' field");
                Assert.That(typeProperty.GetString(), Is.EqualTo("narrative_terminal"), "Type should be 'narrative_terminal'");

                Assert.That(doc.RootElement.TryGetProperty("openingLines", out _), Is.True, "JSON should have 'openingLines' array");
                Assert.That(doc.RootElement.TryGetProperty("initialChoice", out _), Is.True, "JSON should have 'initialChoice' object");
            }

            doc?.Dispose();
        }

        /// <summary>
        /// Verifies that the Scene1Narrative.tscn file can be loaded as a PackedScene.
        /// </summary>
        /// <remarks>
        /// Ensures the scene resource loads successfully for further testing.
        /// </remarks>
        [Test]
        public void Scene1_SceneCanBeLoaded()
        {
            // Arrange
            string scenePath = "res://Source/Scenes/Scene1Narrative.tscn";

            // Act
            Resource sceneResource = ResourceLoader.Load(scenePath);
            PackedScene? packedScene = sceneResource as PackedScene;

            // Assert
            Assert.That(sceneResource, Is.Not.Null, "Scene resource should load successfully");
            Assert.That(packedScene, Is.Not.Null, "Scene resource should be a PackedScene");
        }

        /// <summary>
        /// Verifies that the Scene1Narrative.tscn file can be instantiated as a Node.
        /// </summary>
        /// <remarks>
        /// Ensures the scene can be instantiated and its root node is correct.
        /// </remarks>
        [Test]
        public void Scene1_SceneCanBeInstantiated()
        {
            // Arrange
            string scenePath = "res://Source/Scenes/Scene1Narrative.tscn";
            PackedScene? packedScene = ResourceLoader.Load<PackedScene>(scenePath);
            Assert.That(packedScene, Is.Not.Null, "PackedScene should load successfully");

            // Act
            Node? instance = null;
            Exception? instantiateException = null;

            try
            {
                if (packedScene != null)
                {
                    instance = packedScene.Instantiate();
                }
            }
            catch (Exception ex)
            {
                instantiateException = ex;
            }

            // Assert
            Assert.That(instantiateException, Is.Null, $"Scene should instantiate without errors: {instantiateException?.Message}");
            Assert.That(instance, Is.Not.Null, "Scene instance should not be null");

            if (instance != null)
            {
                Assert.That(instance.Name.ToString(), Is.EqualTo("Scene1Narrative"), "Scene root node should be named 'Scene1Narrative'");
            }

            // Cleanup
            instance?.QueueFree();
        }

        /// <summary>
        /// Checks that the NarrativeTerminal.cs script exists for Scene1.
        /// </summary>
        /// <remarks>
        /// Validates the presence of the narrative terminal script for Scene1.
        /// </remarks>
        [Test]
        public void Scene1_NarrativeTerminalScriptExists()
        {
            // Arrange
            string scriptPath = "res://Source/Scripts/NarrativeTerminal.cs";

            // Act
            bool exists = ResourceLoader.Exists(scriptPath);

            // Assert
            Assert.That(exists, Is.True, $"NarrativeTerminal.cs script should exist at {scriptPath}");
        }

        /// <summary>
        /// Verifies that the manifest.json references Scene1 with the correct properties.
        /// </summary>
        /// <remarks>
        /// Ensures Scene1 is present in the manifest and its properties are correct.
        /// </remarks>
        [Test]
        public void Scene1_ManifestReferencesScene1()
        {
            // Arrange
            string manifestPath = "res://Source/Data/manifest.json";

            // Act
            string jsonData = GodotFileAccess.GetFileAsString(manifestPath);
            var doc = System.Text.Json.JsonDocument.Parse(jsonData);

            bool foundScene1 = false;
            if (doc.RootElement.TryGetProperty("scenes", out var scenesArray))
            {
                foreach (var scene in scenesArray.EnumerateArray())
                {
                    if (scene.TryGetProperty("id", out var idProp) && idProp.GetInt32() == 1)
                    {
                        foundScene1 = true;

                        // Verify scene1 properties
                        Assert.That(scene.TryGetProperty("type", out var typeProp), Is.True, "Scene1 should have 'type' property");
                        Assert.That(typeProp.GetString(), Is.EqualTo("narrative_terminal"), "Scene1 type should be 'narrative_terminal'");

                        Assert.That(scene.TryGetProperty("path", out var pathProp), Is.True, "Scene1 should have 'path' property");
                        Assert.That(pathProp.GetString(), Is.EqualTo("scene1_narrative"), "Scene1 path should be 'scene1_narrative'");

                        break;
                    }
                }
            }

            // Assert
            Assert.That(foundScene1, Is.True, "Manifest should contain Scene1 with id=1");

            doc.Dispose();
        }

        /// <summary>
        /// Verifies that all thread data files for Scene1 have valid JSON structure.
        /// </summary>
        /// <remarks>
        /// Ensures hero, shadow, and ambition data files are correctly structured.
        /// </remarks>
        [Test]
        public void Scene1_AllThreadDataFilesHaveValidStructure()
        {
            // Arrange
            string[] threads = { "hero", "shadow", "ambition" };

            foreach (string thread in threads)
            {
                // Act
                string dataPath = $"res://Source/Data/scenes/scene1_narrative/{thread}.json";
                string jsonData = GodotFileAccess.GetFileAsString(dataPath);
                var doc = System.Text.Json.JsonDocument.Parse(jsonData);

                // Assert
                Assert.That(
                    doc.RootElement.TryGetProperty("type", out var typeProperty), Is.True,
                    $"{thread}.json should have 'type' field");
                Assert.That(typeProperty.GetString(), Is.EqualTo("narrative_terminal"),
                    $"{thread}.json type should be 'narrative_terminal'");

                Assert.That(
                    doc.RootElement.TryGetProperty("openingLines", out var openingLines), Is.True,
                    $"{thread}.json should have 'openingLines' array");
                Assert.That(
                    openingLines.GetArrayLength() > 0, Is.True,
                    $"{thread}.json should have at least one opening line");

                Assert.That(
                    doc.RootElement.TryGetProperty("initialChoice", out var initialChoice),
                    $"{thread}.json should have 'initialChoice' object");
                Assert.That(
                    initialChoice.TryGetProperty("prompt", out _),
                    Is.True,
                    $"{thread}.json initialChoice should have 'prompt'");
                Assert.That(
                    initialChoice.TryGetProperty("options", out var options),
                    Is.True,
                    $"{thread}.json initialChoice should have 'options' array");
                Assert.That(
                    options.GetArrayLength() > 0,
                    Is.True,
                    $"{thread}.json should have at least one choice option");

                doc.Dispose();
            }
        }
    }
}
