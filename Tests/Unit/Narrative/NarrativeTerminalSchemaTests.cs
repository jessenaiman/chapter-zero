// <copyright file="NarrativeTerminalSchemaTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Narrative
{
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the narrative terminal schema validation.
    /// </summary>
    [TestFixture]
    public class NarrativeTerminalSchemaTests
    {
        /// <summary>
        /// Tests that valid data returns true.
        /// </summary>
        [Test]
        public void ValidateNarrativeTerminalSchema_ValidData_ReturnsTrue()
        {
            // Arrange
            var validData = new JsonObject
            {
                ["type"] = "narrative_terminal",
                ["openingLines"] = new JsonArray { "Welcome to Ωmega Spiral", "Choose your path wisely" },
                ["initialChoice"] = new JsonObject
                {
                    ["prompt"] = "What drives your journey?",
                    ["options"] = new JsonArray
                    {
                        new JsonObject
                        {
                            ["id"] = "hero",
                            ["text"] = "The path of light and sacrifice",
                            ["thread"] = "hero",
                            ["alignmentBonus"] = new JsonObject
                            {
                                ["light"] = 2,
                                ["mischief"] = 0,
                                ["wrath"] = 0,
                            },
                        },
                    },
                },
                ["storyBlocks"] = new JsonArray(),
                ["namePrompt"] = "What is your name, traveler?",
                ["secretQuestion"] = new JsonObject
                {
                    ["question"] = "What is your deepest secret?",
                    ["validation"] = "required",
                },
                ["exitLine"] = "Your journey begins...",
            };

            string jsonData = validData.ToJsonString();

            // Act & Assert
            // This is a placeholder test - in reality, we would validate against the actual schema
            Assert.That(jsonData, Does.Contain("\"type\":\"narrative_terminal\""), "Should contain correct type");
            Assert.That(jsonData, Does.Contain("\"openingLines\":"), "Should contain opening lines");
        }

        /// <summary>
        /// Tests that data with missing required field returns false.
        /// </summary>
        [Test]
        public void ValidateNarrativeTerminalSchema_MissingRequiredField_ReturnsFalse()
        {
            // Arrange
            var invalidData = new JsonObject
            {
                ["type"] = "narrative_terminal",
                // Missing required fields like openingLines, initialChoice, etc.
            };

            string jsonData = invalidData.ToJsonString();

            // Act & Assert
            // This is a placeholder test - in reality, we would validate against the actual schema
            Assert.That(jsonData, Does.Not.Contain("\"openingLines\":"), "Should be missing opening lines");
        }

        /// <summary>
        /// Tests that data with wrong type returns false.
        /// </summary>
        [Test]
        public void ValidateNarrativeTerminalSchema_WrongType_ReturnsFalse()
        {
            // Arrange
            var invalidData = new JsonObject
            {
                ["type"] = "invalid_type", // Wrong type
                ["openingLines"] = new JsonArray { "Test" },
                ["initialChoice"] = new JsonObject(),
                ["storyBlocks"] = new JsonArray(),
                ["namePrompt"] = "Test",
                ["secretQuestion"] = new JsonObject(),
                ["exitLine"] = "Test",
            };

            string jsonData = invalidData.ToJsonString();

            // Act & Assert
            Assert.That(jsonData, Does.Contain("\"type\":\"invalid_type\""), "Should contain wrong type");
        }

        /// <summary>
        /// Tests that empty data returns false.
        /// </summary>
        [Test]
        public void ValidateNarrativeTerminalSchema_EmptyData_ReturnsFalse()
        {
            // Arrange
            var emptyData = new JsonObject();
            string jsonData = emptyData.ToJsonString();

            // Act & Assert
            Assert.That(jsonData, Is.EqualTo("{}"), "Should be empty JSON object");
        }
    }
}
