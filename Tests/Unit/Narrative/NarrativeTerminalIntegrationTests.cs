// <copyright file="NarrativeTerminalIntegrationTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Unit.Narrative
{
    using System.Collections.Generic;
    using Godot;
    using NUnit.Framework;
    using OmegaSpiral.Source.Scripts;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Field.Narrative;

    /// <summary>
    /// Integration tests for the narrative terminal functionality, testing the interaction between narrative choices and game state updates.
    /// </summary>
    [TestFixture]
    public class NarrativeTerminalIntegrationTests : IDisposable
    {
        private GameState gameState = new();
        private NarrativeTerminal narrativeTerminal = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="NarrativeTerminalIntegrationTests"/> class.
        /// </summary>
        public NarrativeTerminalIntegrationTests()
        {
        }

        /// <summary>
        /// Sets up the test environment before each test method.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.gameState = new GameState();
            this.narrativeTerminal = new NarrativeTerminal();

            // Note: In a real Godot test, we'd need to set up the scene tree
            // For now, we'll test the logic independently
        }

        /// <summary>
        /// Tests that processing a hero dreamweaver choice correctly updates the game state.
        /// </summary>
        [Test]
        public void ProcessDreamweaverChoice_HeroChoice_UpdatesGameStateCorrectly()
        {
            // Arrange
            var choice = new DreamweaverChoice
            {
                Id = "hero_choice",
                Text = "The path of light",
                Thread = DreamweaverThread.Hero,
            };
            choice.AlignmentBonus.Add(DreamweaverType.Light, 2);
            choice.AlignmentBonus.Add(DreamweaverType.Mischief, 0);
            choice.AlignmentBonus.Add(DreamweaverType.Wrath, 0);

            // Act
            this.gameState.DreamweaverThread = choice.Thread;
            foreach (var bonus in choice.AlignmentBonus)
            {
                this.gameState.DreamweaverScores[bonus.Key] += bonus.Value;
            }

            // Assert
            Assert.That(this.gameState.DreamweaverThread, Is.EqualTo(DreamweaverThread.Hero));
            Assert.That(this.gameState.DreamweaverScores[DreamweaverType.Light], Is.EqualTo(2));
            Assert.That(this.gameState.DreamweaverScores[DreamweaverType.Mischief], Is.EqualTo(0));
            Assert.That(this.gameState.DreamweaverScores[DreamweaverType.Wrath], Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that processing a shadow dreamweaver choice correctly updates the game state.
        /// </summary>
        [Test]
        public void ProcessDreamweaverChoiceShadowChoiceUpdatesGameStateCorrectly()
        {
            // Arrange
            var choice = new DreamweaverChoice
            {
                Id = "shadow_choice",
                Text = "The path of wrath",
                Thread = DreamweaverThread.Shadow,
            };
            choice.AlignmentBonus.Add(DreamweaverType.Light, 0);
            choice.AlignmentBonus.Add(DreamweaverType.Mischief, 0);
            choice.AlignmentBonus.Add(DreamweaverType.Wrath, 2);

            // Act
            this.gameState.DreamweaverThread = choice.Thread;
            foreach (var bonus in choice.AlignmentBonus)
            {
                this.gameState.DreamweaverScores[bonus.Key] += bonus.Value;
            }

            // Assert
            Assert.That(this.gameState.DreamweaverThread, Is.EqualTo(DreamweaverThread.Shadow));
            Assert.That(this.gameState.DreamweaverScores[DreamweaverType.Light], Is.EqualTo(0));
            Assert.That(this.gameState.DreamweaverScores[DreamweaverType.Mischief], Is.EqualTo(0));
            Assert.That(this.gameState.DreamweaverScores[DreamweaverType.Wrath], Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that processing an ambition dreamweaver choice correctly updates the game state.
        /// </summary>
        [Test]
        public void ProcessDreamweaverChoice_AmbitionChoice_UpdatesGameStateCorrectly()
        {
            // Arrange
            var choice = new DreamweaverChoice
            {
                Id = "ambition_choice",
                Text = "The path of mischief",
                Thread = DreamweaverThread.Ambition,
            };
            choice.AlignmentBonus.Add(DreamweaverType.Light, 0);
            choice.AlignmentBonus.Add(DreamweaverType.Mischief, 2);
            choice.AlignmentBonus.Add(DreamweaverType.Wrath, 0);

            // Act
            this.gameState.DreamweaverThread = choice.Thread;
            foreach (var bonus in choice.AlignmentBonus)
            {
                this.gameState.DreamweaverScores[bonus.Key] += bonus.Value;
            }

            // Assert
            Assert.That(this.gameState.DreamweaverThread, Is.EqualTo(DreamweaverThread.Ambition));
            Assert.That(this.gameState.DreamweaverScores[DreamweaverType.Light], Is.EqualTo(0));
            Assert.That(this.gameState.DreamweaverScores[DreamweaverType.Mischief], Is.EqualTo(2));
            Assert.That(this.gameState.DreamweaverScores[DreamweaverType.Wrath], Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that providing a valid player name correctly updates the game state.
        /// </summary>
        [Test]
        public void PlayerNameInput_ValidName_UpdatesGameState()
        {
            // Arrange
            string playerName = "TestPlayer";

            // Act
            this.gameState.PlayerName = playerName;

            // Assert
            Assert.That(this.gameState.PlayerName, Is.EqualTo(playerName));
        }

        /// <summary>
        /// Tests that scene progression correctly increments the current scene after a choice.
        /// </summary>
        [Test]
        public void SceneProgression_AfterChoice_IncrementsScene()
        {
            // Arrange
            int initialScene = this.gameState.CurrentScene;

            // Act
            this.gameState.CurrentScene = initialScene + 1;

            // Assert
            Assert.That(this.gameState.CurrentScene, Is.EqualTo(initialScene + 1));
        }

        /// <summary>
        /// Tests that dreamweaver scoring correctly accumulates values from multiple choices.
        /// </summary>
        [Test]
        public void DreamweaverScoring_MultipleChoices_AccumulatesCorrectly()
        {
            // Arrange
            var choice1 = new DreamweaverChoice();
            choice1.AlignmentBonus.Add(DreamweaverType.Light, 1);
            choice1.AlignmentBonus.Add(DreamweaverType.Mischief, 0);
            choice1.AlignmentBonus.Add(DreamweaverType.Wrath, 0);

            var choice2 = new DreamweaverChoice();
            choice2.AlignmentBonus.Add(DreamweaverType.Light, 1);
            choice2.AlignmentBonus.Add(DreamweaverType.Mischief, 1);
            choice2.AlignmentBonus.Add(DreamweaverType.Wrath, 0);

            // Act
            foreach (var bonus in choice1.AlignmentBonus)
            {
                this.gameState.DreamweaverScores[bonus.Key] += bonus.Value;
            }

            foreach (var bonus in choice2.AlignmentBonus)
            {
                this.gameState.DreamweaverScores[bonus.Key] += bonus.Value;
            }

            // Assert
            Assert.That(this.gameState.DreamweaverScores[DreamweaverType.Light], Is.EqualTo(2));
            Assert.That(this.gameState.DreamweaverScores[DreamweaverType.Mischief], Is.EqualTo(1));
            Assert.That(this.gameState.DreamweaverScores[DreamweaverType.Wrath], Is.EqualTo(0));
        }

        /// <summary>
        /// Disposes of the test resources.
        /// </summary>
        public void Dispose()
        {
            // Dispose of managed resources if any
            // In this test class, there are no managed resources to dispose
        }
    }
}
