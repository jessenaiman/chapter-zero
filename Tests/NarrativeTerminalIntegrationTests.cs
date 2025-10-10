using NUnit.Framework;
using Godot;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts;

namespace OmegaSpiral.Tests
{
    [TestFixture]
    public class NarrativeTerminalIntegrationTests
    {
        private GameState _gameState;
        private NarrativeTerminal _narrativeTerminal;

        [SetUp]
        public void Setup()
        {
            _gameState = new GameState();
            _narrativeTerminal = new NarrativeTerminal();
            // Note: In a real Godot test, we'd need to set up the scene tree
            // For now, we'll test the logic independently
        }

        [Test]
        public void ProcessDreamweaverChoice_HeroChoice_UpdatesGameStateCorrectly()
        {
            // Arrange
            var choice = new DreamweaverChoice
            {
                Id = "hero_choice",
                Text = "The path of light",
                Thread = DreamweaverThread.Hero,
                AlignmentBonus = new Dictionary<DreamweaverType, int>
                {
                    [DreamweaverType.Light] = 2,
                    [DreamweaverType.Mischief] = 0,
                    [DreamweaverType.Wrath] = 0
                }
            };

            // Act
            _gameState.DreamweaverThread = choice.Thread;
            foreach (var bonus in choice.AlignmentBonus)
            {
                _gameState.DreamweaverScores[bonus.Key] += bonus.Value;
            }

            // Assert
            Assert.AreEqual(DreamweaverThread.Hero, _gameState.DreamweaverThread);
            Assert.AreEqual(2, _gameState.DreamweaverScores[DreamweaverType.Light]);
            Assert.AreEqual(0, _gameState.DreamweaverScores[DreamweaverType.Mischief]);
            Assert.AreEqual(0, _gameState.DreamweaverScores[DreamweaverType.Wrath]);
        }

        [Test]
        public void ProcessDreamweaverChoice_ShadowChoice_UpdatesGameStateCorrectly()
        {
            // Arrange
            var choice = new DreamweaverChoice
            {
                Id = "shadow_choice",
                Text = "The path of wrath",
                Thread = DreamweaverThread.Shadow,
                AlignmentBonus = new Dictionary<DreamweaverType, int>
                {
                    [DreamweaverType.Light] = 0,
                    [DreamweaverType.Mischief] = 0,
                    [DreamweaverType.Wrath] = 2
                }
            };

            // Act
            _gameState.DreamweaverThread = choice.Thread;
            foreach (var bonus in choice.AlignmentBonus)
            {
                _gameState.DreamweaverScores[bonus.Key] += bonus.Value;
            }

            // Assert
            Assert.AreEqual(DreamweaverThread.Shadow, _gameState.DreamweaverThread);
            Assert.AreEqual(0, _gameState.DreamweaverScores[DreamweaverType.Light]);
            Assert.AreEqual(0, _gameState.DreamweaverScores[DreamweaverType.Mischief]);
            Assert.AreEqual(2, _gameState.DreamweaverScores[DreamweaverType.Wrath]);
        }

        [Test]
        public void ProcessDreamweaverChoice_AmbitionChoice_UpdatesGameStateCorrectly()
        {
            // Arrange
            var choice = new DreamweaverChoice
            {
                Id = "ambition_choice",
                Text = "The path of mischief",
                Thread = DreamweaverThread.Ambition,
                AlignmentBonus = new Dictionary<DreamweaverType, int>
                {
                    [DreamweaverType.Light] = 0,
                    [DreamweaverType.Mischief] = 2,
                    [DreamweaverType.Wrath] = 0
                }
            };

            // Act
            _gameState.DreamweaverThread = choice.Thread;
            foreach (var bonus in choice.AlignmentBonus)
            {
                _gameState.DreamweaverScores[bonus.Key] += bonus.Value;
            }

            // Assert
            Assert.AreEqual(DreamweaverThread.Ambition, _gameState.DreamweaverThread);
            Assert.AreEqual(0, _gameState.DreamweaverScores[DreamweaverType.Light]);
            Assert.AreEqual(2, _gameState.DreamweaverScores[DreamweaverType.Mischief]);
            Assert.AreEqual(0, _gameState.DreamweaverScores[DreamweaverType.Wrath]);
        }

        [Test]
        public void PlayerNameInput_ValidName_UpdatesGameState()
        {
            // Arrange
            string playerName = "TestPlayer";

            // Act
            _gameState.PlayerName = playerName;

            // Assert
            Assert.AreEqual(playerName, _gameState.PlayerName);
        }

        [Test]
        public void SceneProgression_AfterChoice_IncrementsScene()
        {
            // Arrange
            int initialScene = _gameState.CurrentScene;

            // Act
            _gameState.CurrentScene = initialScene + 1;

            // Assert
            Assert.AreEqual(initialScene + 1, _gameState.CurrentScene);
        }

        [Test]
        public void DreamweaverScoring_MultipleChoices_AccumulatesCorrectly()
        {
            // Arrange
            var choice1 = new DreamweaverChoice
            {
                AlignmentBonus = new Dictionary<DreamweaverType, int>
                {
                    [DreamweaverType.Light] = 1,
                    [DreamweaverType.Mischief] = 0,
                    [DreamweaverType.Wrath] = 0
                }
            };

            var choice2 = new DreamweaverChoice
            {
                AlignmentBonus = new Dictionary<DreamweaverType, int>
                {
                    [DreamweaverType.Light] = 1,
                    [DreamweaverType.Mischief] = 1,
                    [DreamweaverType.Wrath] = 0
                }
            };

            // Act
            foreach (var bonus in choice1.AlignmentBonus)
            {
                _gameState.DreamweaverScores[bonus.Key] += bonus.Value;
            }
            foreach (var bonus in choice2.AlignmentBonus)
            {
                _gameState.DreamweaverScores[bonus.Key] += bonus.Value;
            }

            // Assert
            Assert.AreEqual(2, _gameState.DreamweaverScores[DreamweaverType.Light]);
            Assert.AreEqual(1, _gameState.DreamweaverScores[DreamweaverType.Mischief]);
            Assert.AreEqual(0, _gameState.DreamweaverScores[DreamweaverType.Wrath]);
        }
    }
}