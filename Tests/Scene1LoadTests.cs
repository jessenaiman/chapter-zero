// <copyright file="Scene1LoadTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests
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
        [Test]
        public void Scene1_SceneFileExists()
        {
            // Arrange
            string scenePath = "res://Source/Scenes/Scene1Narrative.tscn";

            // Act
            bool exists = ResourceLoader.Exists(scenePath);

            // Assert
            Assert.IsTrue(exists, $"Scene1Narrative.tscn should exist at {scenePath}");
        }

        [Test]
        public void Scene1_HeroDataFileExists()
        {
            // Arrange
            string dataPath = "res://Source/Data/scenes/scene1_narrative/hero.json";

            // Act
            bool exists = GodotFileAccess.FileExists(dataPath);

            // Assert
            Assert.IsTrue(exists, $"Hero narrative data should exist at {dataPath}");
        }

        [Test]
        public void Scene1_ShadowDataFileExists()
        {
            // Arrange
            string dataPath = "res://Source/Data/scenes/scene1_narrative/shadow.json";

            // Act
            bool exists = GodotFileAccess.FileExists(dataPath);

            // Assert
            Assert.IsTrue(exists, $"Shadow narrative data should exist at {dataPath}");
        }

        [Test]
        public void Scene1_AmbitionDataFileExists()
        {
            // Arrange
            string dataPath = "res://Source/Data/scenes/scene1_narrative/ambition.json";

            // Act
            bool exists = GodotFileAccess.FileExists(dataPath);

            // Assert
            Assert.IsTrue(exists, $"Ambition narrative data should exist at {dataPath}");
        }

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
            Assert.IsNull(parseException, $"Hero JSON should parse without errors: {parseException?.Message}");
            Assert.IsNotNull(doc, "Parsed JSON document should not be null");

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

        [Test]
        public void Scene1_SceneCanBeLoaded()
        {
            // Arrange
            string scenePath = "res://Source/Scenes/Scene1Narrative.tscn";

            // Act
            Resource sceneResource = ResourceLoader.Load(scenePath);
            PackedScene? packedScene = sceneResource as PackedScene;

            // Assert
            Assert.IsNotNull(sceneResource, "Scene resource should load successfully");
            Assert.IsNotNull(packedScene, "Scene resource should be a PackedScene");
        }

        [Test]
        public void Scene1_SceneCanBeInstantiated()
        {
            // Arrange
            string scenePath = "res://Source/Scenes/Scene1Narrative.tscn";
            PackedScene? packedScene = ResourceLoader.Load<PackedScene>(scenePath);
            Assert.IsNotNull(packedScene, "PackedScene should load successfully");

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
            Assert.IsNull(instantiateException, $"Scene should instantiate without errors: {instantiateException?.Message}");
            Assert.IsNotNull(instance, "Scene instance should not be null");

            if (instance != null)
            {
                Assert.AreEqual("Scene1Narrative", instance.Name, "Scene root node should be named 'Scene1Narrative'");
            }

            // Cleanup
            instance?.QueueFree();
        }

        [Test]
        public void Scene1_NarrativeTerminalScriptExists()
        {
            // Arrange
            string scriptPath = "res://Source/Scripts/NarrativeTerminal.cs";

            // Act
            bool exists = ResourceLoader.Exists(scriptPath);

            // Assert
            Assert.IsTrue(exists, $"NarrativeTerminal.cs script should exist at {scriptPath}");
        }

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
            Assert.IsTrue(foundScene1, "Manifest should contain Scene1 with id=1");

            doc.Dispose();
        }

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

                Assert.IsTrue(
                    doc.RootElement.TryGetProperty("initialChoice", out var initialChoice),
                    $"{thread}.json should have 'initialChoice' object");
                Assert.IsTrue(
                    initialChoice.TryGetProperty("prompt", out _),
                    $"{thread}.json initialChoice should have 'prompt'");
                Assert.IsTrue(
                    initialChoice.TryGetProperty("options", out var options),
                    $"{thread}.json initialChoice should have 'options' array");
                Assert.IsTrue(
                    options.GetArrayLength() > 0,
                    $"{thread}.json should have at least one choice option");

                doc.Dispose();
            }
        }
    }
}
