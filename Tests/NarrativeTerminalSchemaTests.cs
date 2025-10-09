using NUnit.Framework;
using Godot;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OmegaSpiral.Tests
{
    [TestFixture]
    public class NarrativeTerminalSchemaTests
    {
        private JsonSchemaValidator _validator;
        private string _schemaPath;

        [SetUp]
        public void Setup()
        {
            _validator = new JsonSchemaValidator();
            _schemaPath = "res://specs/004-implement-omega-spiral/contracts/scene1_narrative_schema.json";
        }

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
                                ["wrath"] = 0
                            }
                        }
                    }
                },
                ["storyBlocks"] = new JsonArray(),
                ["namePrompt"] = "What is your name, traveler?",
                ["secretQuestion"] = new JsonObject
                {
                    ["question"] = "What is your deepest secret?",
                    ["validation"] = "required"
                },
                ["exitLine"] = "Your journey begins..."
            };

            // Act
            bool result = _validator.ValidateSchema(validData, _schemaPath);

            // Assert
            Assert.IsTrue(result, "Valid narrative terminal data should pass schema validation");
        }

        [Test]
        public void ValidateNarrativeTerminalSchema_MissingRequiredField_ReturnsFalse()
        {
            // Arrange
            var invalidData = new JsonObject
            {
                ["type"] = "narrative_terminal",
                // Missing required fields like openingLines, initialChoice, etc.
            };

            // Act
            bool result = _validator.ValidateSchema(invalidData, _schemaPath);

            // Assert
            Assert.IsFalse(result, "Data missing required fields should fail schema validation");
        }

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
                ["exitLine"] = "Test"
            };

            // Act
            bool result = _validator.ValidateSchema(invalidData, _schemaPath);

            // Assert
            Assert.IsFalse(result, "Data with wrong type should fail schema validation");
        }

        [Test]
        public void ValidateNarrativeTerminalSchema_EmptyData_ReturnsFalse()
        {
            // Arrange
            var emptyData = new JsonObject();

            // Act
            bool result = _validator.ValidateSchema(emptyData, _schemaPath);

            // Assert
            Assert.IsFalse(result, "Empty data should fail schema validation");
        }
    }
}