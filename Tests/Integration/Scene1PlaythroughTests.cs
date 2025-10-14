// <copyright file="Scene1PlaythroughTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Integration
{
    using System.Collections.Generic;
    using Godot;
    using NUnit.Framework;
    using OmegaSpiral.Source.Scripts;
    using OmegaSpiral.Source.Scripts.Common;

    /// <summary>
    /// Integration tests for Scene1Narrative that simulate complete playthrough scenarios.
    /// These tests validate the entire narrative flow from opening sequence to scene transition,
    /// with simulated user choices for each persona path (HERO, SHADOW, AMBITION).
    /// </summary>
    [TestFixture]
    public partial class Scene1PlaythroughTests : GodotObject
    {
        private DialogicTestHelper? dialogicHelper;
        private Scene1TestHelper? sceneHelper;
        private Node? testScene;

        /// <summary>
        /// Sets up the test environment before each test.
        /// </summary>
        [SetUp]
        public void SetupCreateTestHelpers()
        {
            // Create a test scene node for signal connections
            this.testScene = new Node();
            this.testScene.Name = "TestScene";

            // Initialize helpers
            this.dialogicHelper = new DialogicTestHelper(this.testScene);
            this.sceneHelper = new Scene1TestHelper(this.testScene, this.dialogicHelper);

            GD.Print("[TEST] Scene1PlaythroughTests setup complete");
        }

        /// <summary>
        /// Cleans up the test environment after each test.
        /// </summary>
        [TearDown]
        public void TeardownCleanupTestHelpers()
        {
            this.dialogicHelper?.Reset();
            this.testScene?.QueueFree();
            this.testScene = null;
            this.dialogicHelper = null;
            this.sceneHelper = null;

            GD.Print("[TEST] Scene1PlaythroughTests teardown complete");
        }

        /// <summary>
        /// Test: Complete playthrough with HERO persona path.
        /// Simulates a player choosing the HERO path and making all story choices.
        /// </summary>
        [Test]
        public void CompletePlaythroughHeroPersonaCompletesSuccessfully()
        {
            // Arrange
            Assert.That(this.sceneHelper, Is.Not.Null, "Scene helper should be initialized");
            Assert.That(this.dialogicHelper, Is.Not.Null, "Dialogic helper should be initialized");

            const int heroPersonaIndex = 1;
            const string playerName = "Lumina";
            const int secretResponseIndex = 1; // "yes"

            var storyChoices = new Dictionary<int, int>
            {
                { 0, 0 }, // First story block, first choice
                { 1, 1 }, // Second story block, second choice
            };

            // Act
            this.sceneHelper!.SimulateCompletePlaythrough(
                heroPersonaIndex,
                playerName,
                storyChoices,
                secretResponseIndex);

            // Assert
            Assert.That(this.dialogicHelper!.CapturedSignals.Count, Is.GreaterThan(0), "Should have captured signals during playthrough");

            // Verify opening sequence was triggered
            Assert.That(
                this.dialogicHelper.CapturedSignals[0],
                Does.Contain("timeline_started"),
                "Should start with timeline_started signal");

            // Verify persona selection
            Assert.That(
                this.dialogicHelper.CapturedSignals,
                Has.Some.Matches<string>(s => s.Contains("choice_selected") && s.Contains("HERO")),
                "Should have HERO persona selection signal");

            // Verify name input
            Assert.That(
                this.dialogicHelper.CapturedSignals,
                Has.Some.Matches<string>(s => s.Contains("text_input") && s.Contains(playerName)),
                "Should have player name input signal");

            // Verify timeline completion
            Assert.That(
                this.dialogicHelper.CapturedSignals[^1],
                Does.Contain("timeline_ended"),
                "Should end with timeline_ended signal");

            // Verify game state
            var expectedPersona = DreamweaverThread.Hero;
            var expectedSecret = Scene1TestHelper.SecretQuestionResponses[secretResponseIndex];
            bool stateValid = this.sceneHelper.ValidateGameState(expectedPersona, playerName, expectedSecret);
            Assert.That(stateValid, Is.True, "Game state should match expected values");
        }

        /// <summary>
        /// Test: Complete playthrough with SHADOW persona path.
        /// Simulates a player choosing the SHADOW path with different story choices.
        /// </summary>
        [Test]
        public void CompletePlaythroughShadowPersonaCompletesSuccessfully()
        {
            // Arrange
            Assert.That(this.sceneHelper, Is.Not.Null, "Scene helper should be initialized");
            Assert.That(this.dialogicHelper, Is.Not.Null, "Dialogic helper should be initialized");

            const int shadowPersonaIndex = 2;
            const string playerName = "Umbra";
            const int secretResponseIndex = 2; // "no"

            var storyChoices = new Dictionary<int, int>
            {
                { 0, 1 }, // First story block, second choice
                { 1, 0 }, // Second story block, first choice
            };

            // Act
            this.sceneHelper!.SimulateCompletePlaythrough(
                shadowPersonaIndex,
                playerName,
                storyChoices,
                secretResponseIndex);

            // Assert
            Assert.That(this.dialogicHelper!.CapturedSignals.Count, Is.GreaterThan(0), "Should have captured signals during playthrough");

            // Verify persona selection
            Assert.That(
                this.dialogicHelper.CapturedSignals,
                Has.Some.Matches<string>(s => s.Contains("choice_selected") && s.Contains("SHADOW")),
                "Should have SHADOW persona selection signal");

            // Verify name input
            Assert.That(
                this.dialogicHelper.CapturedSignals,
                Has.Some.Matches<string>(s => s.Contains("text_input") && s.Contains(playerName)),
                "Should have player name input signal");

            // Verify timeline completion
            Assert.That(
                this.dialogicHelper.CapturedSignals,
                Has.Some.Matches<string>(s => s.Contains("timeline_ended")),
                "Should have timeline_ended signal");

            // Verify game state
            var expectedPersona = DreamweaverThread.Shadow;
            var expectedSecret = Scene1TestHelper.SecretQuestionResponses[secretResponseIndex];
            bool stateValid = this.sceneHelper.ValidateGameState(expectedPersona, playerName, expectedSecret);
            Assert.That(stateValid, Is.True, "Game state should match expected values");
        }

        /// <summary>
        /// Test: Complete playthrough with AMBITION persona path.
        /// Simulates a player choosing the AMBITION path with varied choices.
        /// </summary>
        [Test]
        public void CompletePlaythroughAmbitionPersonaCompletesSuccessfully()
        {
            // Arrange
            Assert.That(this.sceneHelper, Is.Not.Null, "Scene helper should be initialized");
            Assert.That(this.dialogicHelper, Is.Not.Null, "Dialogic helper should be initialized");

            const int ambitionPersonaIndex = 3;
            const string playerName = "Caelus";
            const int secretResponseIndex = 3; // "only if you keep one for me"

            var storyChoices = new Dictionary<int, int>
            {
                { 0, 0 }, // First story block, first choice
                { 1, 0 }, // Second story block, first choice
            };

            // Act
            this.sceneHelper!.SimulateCompletePlaythrough(
                ambitionPersonaIndex,
                playerName,
                storyChoices,
                secretResponseIndex);

            // Assert
            Assert.That(this.dialogicHelper!.CapturedSignals.Count, Is.GreaterThan(0), "Should have captured signals during playthrough");

            // Verify persona selection
            Assert.That(
                this.dialogicHelper.CapturedSignals,
                Has.Some.Matches<string>(s => s.Contains("choice_selected") && s.Contains("AMBITION")),
                "Should have AMBITION persona selection signal");

            // Verify name input
            Assert.That(
                this.dialogicHelper.CapturedSignals,
                Has.Some.Matches<string>(s => s.Contains("text_input") && s.Contains(playerName)),
                "Should have player name input signal");

            // Verify timeline completion
            Assert.That(
                this.dialogicHelper.CapturedSignals,
                Has.Some.Matches<string>(s => s.Contains("timeline_ended")),
                "Should have timeline_ended signal");

            // Verify game state
            var expectedPersona = DreamweaverThread.Ambition;
            var expectedSecret = Scene1TestHelper.SecretQuestionResponses[secretResponseIndex];
            bool stateValid = this.sceneHelper.ValidateGameState(expectedPersona, playerName, expectedSecret);
            Assert.That(stateValid, Is.True, "Game state should match expected values");
        }

        /// <summary>
        /// Test: Opening sequence displays all expected lines.
        /// </summary>
        [Test]
        public void OpeningSequenceDisplaysAllLinesInCorrectOrder()
        {
            // Arrange
            Assert.That(this.sceneHelper, Is.Not.Null, "Scene helper should be initialized");
            Assert.That(this.dialogicHelper, Is.Not.Null, "Dialogic helper should be initialized");

            // Act
            this.sceneHelper!.SimulateOpeningSequence();

            // Assert
            var expectedLines = Scene1TestHelper.ExpectedOpeningLines;
            foreach (var line in expectedLines)
            {
                Assert.That(
                    this.dialogicHelper!.CapturedSignals,
                    Has.Some.Matches<string>(s => s.Contains("text") && s.Contains(line)),
                    $"Should display opening line: {line}");
            }
        }

        /// <summary>
        /// Test: Persona selection updates game state correctly.
        /// </summary>
        [Test]
        public void PersonaSelectionUpdatesGameStateForAllPersonas()
        {
            // Arrange
            Assert.That(this.sceneHelper, Is.Not.Null, "Scene helper should be initialized");
            Assert.That(this.dialogicHelper, Is.Not.Null, "Dialogic helper should be initialized");

            // Act & Assert for each persona
            for (int personaIndex = 1; personaIndex <= 3; personaIndex++)
            {
                this.dialogicHelper!.Reset();
                this.sceneHelper!.SimulateOpeningSequence();
                this.sceneHelper.SimulatePersonaSelection(personaIndex);

                var persona = Scene1TestHelper.PersonaChoices[personaIndex];
                Assert.That(
                    this.dialogicHelper.CapturedSignals,
                    Has.Some.Matches<string>(s => s.Contains("choice_selected") && s.Contains(persona.Name)),
                    $"Should have {persona.Name} selection signal");

                var variable = this.dialogicHelper.GetVariable("selected_persona");
                Assert.That(variable, Is.EqualTo(persona.Id), $"Should set persona variable to {persona.Id}");
            }
        }

        /// <summary>
        /// Test: Story block progression captures all choices in sequence.
        /// </summary>
        [Test]
        public void StoryBlockProgressionCapturesAllChoicesInSequence()
        {
            // Arrange
            Assert.That(this.sceneHelper, Is.Not.Null, "Scene helper should be initialized");
            Assert.That(this.dialogicHelper, Is.Not.Null, "Dialogic helper should be initialized");

            this.sceneHelper!.SimulateOpeningSequence();
            this.sceneHelper.SimulatePersonaSelection(1);

            // Act
            this.sceneHelper.SimulateStoryBlock(0, 0);
            this.sceneHelper.SimulateStoryBlock(1, 1);

            // Assert
            Assert.That(
                this.dialogicHelper!.CapturedSignals,
                Has.Some.Matches<string>(s => s.Contains("StoryBlock_0_Display")),
                "Should display story block 0");

            Assert.That(
                this.dialogicHelper.CapturedSignals,
                Has.Some.Matches<string>(s => s.Contains("StoryBlock_1_Display")),
                "Should display story block 1");

            var block0Choice = this.dialogicHelper.GetVariable("story_block_0_choice");
            Assert.That(block0Choice, Is.EqualTo(0), "Should record choice for block 0");

            var block1Choice = this.dialogicHelper.GetVariable("story_block_1_choice");
            Assert.That(block1Choice, Is.EqualTo(1), "Should record choice for block 1");
        }

        /// <summary>
        /// Test: Name input validation and storage.
        /// </summary>
        [Test]
        public void NameInputStoresPlayerNameInGameState()
        {
            // Arrange
            Assert.That(this.sceneHelper, Is.Not.Null, "Scene helper should be initialized");
            Assert.That(this.dialogicHelper, Is.Not.Null, "Dialogic helper should be initialized");

            const string testName = "TestPlayer";

            this.sceneHelper!.SimulateOpeningSequence();
            this.sceneHelper.SimulatePersonaSelection(1);

            // Act
            this.sceneHelper.SimulateNameInput(testName);

            // Assert
            Assert.That(
                this.dialogicHelper!.CapturedSignals,
                Has.Some.Matches<string>(s => s.Contains("text_input") && s.Contains(testName)),
                "Should have name input signal");

            var storedName = this.dialogicHelper.GetVariable("player_name");
            Assert.That(storedName, Is.EqualTo(testName), "Should store player name in variables");
        }

        /// <summary>
        /// Test: Secret question responses are recorded correctly.
        /// </summary>
        [Test]
        public void SecretQuestionRecordsResponseForAllOptions()
        {
            // Arrange
            Assert.That(this.sceneHelper, Is.Not.Null, "Scene helper should be initialized");
            Assert.That(this.dialogicHelper, Is.Not.Null, "Dialogic helper should be initialized");

            this.sceneHelper!.SimulateOpeningSequence();
            this.sceneHelper.SimulatePersonaSelection(1);

            // Act & Assert for each response
            for (int responseIndex = 1; responseIndex <= 3; responseIndex++)
            {
                this.dialogicHelper!.Reset();
                this.sceneHelper.SimulateOpeningSequence();
                this.sceneHelper.SimulatePersonaSelection(1);
                this.sceneHelper.SimulateSecretQuestionResponse(responseIndex);

                var expectedResponse = Scene1TestHelper.SecretQuestionResponses[responseIndex];
                Assert.That(
                    this.dialogicHelper.CapturedSignals,
                    Has.Some.Matches<string>(s => s.Contains("choice_selected") && s.Contains(expectedResponse)),
                    $"Should have secret response signal for: {expectedResponse}");

                var storedResponse = this.dialogicHelper.GetVariable("secret_response");
                Assert.That(storedResponse, Is.EqualTo(expectedResponse), $"Should store secret response: {expectedResponse}");
            }
        }

        /// <summary>
        /// Test: Timeline properly ends after complete playthrough.
        /// </summary>
        [Test]
        public void TimelineEndsSuccessfullyAfterCompletePlaythrough()
        {
            // Arrange
            Assert.That(this.sceneHelper, Is.Not.Null, "Scene helper should be initialized");
            Assert.That(this.dialogicHelper, Is.Not.Null, "Dialogic helper should be initialized");

            var storyChoices = new Dictionary<int, int> { { 0, 0 }, { 1, 1 } };

            // Act
            this.sceneHelper!.SimulateCompletePlaythrough(1, "TestPlayer", storyChoices, 1);

            // Assert
            Assert.That(this.dialogicHelper!.IsTimelineActive, Is.False, "Timeline should be inactive after completion");
            Assert.That(this.dialogicHelper.CurrentTimeline, Is.Null, "Current timeline should be null after completion");

            Assert.That(
                this.dialogicHelper.CapturedSignals,
                Has.Some.Matches<string>(s => s.Contains("timeline_ended")),
                "Should have timeline_ended signal");
        }

        /// <summary>
        /// Test: Multiple playthroughs can be run sequentially.
        /// </summary>
        [Test]
        public void MultiplePlaythroughsCanBeRunSequentiallyWithDifferentChoices()
        {
            // Arrange
            Assert.That(this.sceneHelper, Is.Not.Null, "Scene helper should be initialized");
            Assert.That(this.dialogicHelper, Is.Not.Null, "Dialogic helper should be initialized");

            var storyChoices = new Dictionary<int, int> { { 0, 0 }, { 1, 1 } };

            // Act - First playthrough
            this.sceneHelper!.SimulateCompletePlaythrough(1, "Player1", storyChoices, 1);
            int firstPlaythroughSignals = this.dialogicHelper!.CapturedSignals.Count;

            // Reset and second playthrough
            this.dialogicHelper.Reset();
            this.sceneHelper.SimulateCompletePlaythrough(2, "Player2", storyChoices, 2);
            int secondPlaythroughSignals = this.dialogicHelper.CapturedSignals.Count;

            // Assert
            Assert.That(firstPlaythroughSignals, Is.GreaterThan(0), "First playthrough should capture signals");
            Assert.That(secondPlaythroughSignals, Is.GreaterThan(0), "Second playthrough should capture signals");
            Assert.That(secondPlaythroughSignals, Is.EqualTo(firstPlaythroughSignals), "Both playthroughs should capture same number of signals");
        }
    }
}
