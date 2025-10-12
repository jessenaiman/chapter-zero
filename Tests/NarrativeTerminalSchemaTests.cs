// <copyright file="NarrativeTerminalSchemaTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests
{
    using System.IO;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using Godot;
    using NUnit.Framework;

    [TestFixture]
    public class NarrativeTerminalSchemaTests : IDisposable
    {
        private JsonSchemaValidator? validator;
        private string? schemaPath;

        [SetUp]
        public void Setup()
        {
            this.validator = new JsonSchemaValidator();
            this.schemaPath = "res://specs/004-implement-omega-spiral/contracts/scene1_narrative_schema.json";
        }

        [Test]
        public void ValidateNarrativeTerminalSchema_ValidData_ReturnsTrue()
        {
            Assert.That(this.schemaPath, Is.Not.Null, "Schema path should be initialized");

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

            // Act
            bool result = JsonSchemaValidator.ValidateSchema(validData, this.schemaPath!);

            // Assert
            Assert.That(result, Is.True, "Valid narrative terminal data should pass schema validation");
        }

        [Test]
        public void ValidateNarrativeTerminalSchema_MissingRequiredField_ReturnsFalse()
        {
            Assert.That(this.schemaPath, Is.Not.Null, "Schema path should be initialized");

            // Arrange
            var invalidData = new JsonObject
            {
                ["type"] = "narrative_terminal",

                // Missing required fields like openingLines, initialChoice, etc.
            };

            // Act
            bool result = JsonSchemaValidator.ValidateSchema(invalidData, this.schemaPath!);

            // Assert
            Assert.That(result, Is.False, "Data missing required fields should fail schema validation");
        }

        [Test]
        public void ValidateNarrativeTerminalSchema_WrongType_ReturnsFalse()
        {
            Assert.That(this.schemaPath, Is.Not.Null, "Schema path should be initialized");

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

            // Act
            bool result = JsonSchemaValidator.ValidateSchema(invalidData, this.schemaPath!);

            // Assert
            Assert.That(result, Is.False, "Data with wrong type should fail schema validation");
        }

        [Test]
        public void ValidateNarrativeTerminalSchema_EmptyData_ReturnsFalse()
        {
            Assert.That(this.schemaPath, Is.Not.Null, "Schema path should be initialized");

            // Arrange
            var emptyData = new JsonObject();

            // Act
            bool result = JsonSchemaValidator.ValidateSchema(emptyData, this.schemaPath!);

            // Assert
            Assert.That(result, Is.False, "Empty data should fail schema validation");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the resources used by this test class.
        /// </summary>
        /// <param name="disposing">True if called from Dispose, false if called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.validator?.Dispose();
            }
        }
    }
}
