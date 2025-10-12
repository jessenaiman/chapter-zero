// <copyright file="Scene1TestHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using Godot;
    using OmegaSpiral.Source.Scripts;
    using OmegaSpiral.Source.Scripts.Common;

    /// <summary>
    /// Helper class for Scene1Narrative integration tests.
    /// Provides common setup, persona data, and validation methods for testing the opening narrative.
    /// </summary>
    public class Scene1TestHelper
    {
        private readonly DialogicTestHelper dialogicHelper;
        private readonly GameState? gameState;
        private readonly DreamweaverSystem? dreamweaverSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scene1TestHelper"/> class.
        /// </summary>
        /// <param name="testScene">The test scene node.</param>
        /// <param name="dialogicHelper">The Dialogic test helper instance.</param>
        public Scene1TestHelper(Node testScene, DialogicTestHelper dialogicHelper)
        {
            if (testScene == null)
            {
                throw new ArgumentNullException(nameof(testScene));
            }

            this.dialogicHelper = dialogicHelper ?? throw new ArgumentNullException(nameof(dialogicHelper));

            // Try to get autoload singletons
            try
            {
                this.gameState = testScene.GetNodeOrNull<GameState>("/root/GameState");
                this.dreamweaverSystem = testScene.GetNodeOrNull<DreamweaverSystem>("/root/DreamweaverSystem");
            }
            catch (InvalidOperationException ex)
            {
                GD.PrintErr($"Failed to get autoload singletons: {ex.Message}");
                throw; // Re-throw as this indicates a serious configuration issue
            }
        }

        /// <summary>
        /// Gets the expected opening lines for Scene1.
        /// </summary>
        public static List<string> ExpectedOpeningLines => new ()
        {
            "Once, there was a name.",
            "Not written in stone or spoken in halls—but remembered in the silence between stars.",
            "I do not know when I heard it. Time does not pass here.",
            "But I have held it.",
            "And now… I hear it again.",
        };

        /// <summary>
        /// Gets the available secret question responses.
        /// </summary>
        public static Dictionary<int, string> SecretQuestionResponses => new ()
        {
            { 1, "yes" },
            { 2, "no" },
            { 3, "only if you keep one for me" },
        };

        /// <summary>
        /// Gets the available persona choices for Scene1.
        /// </summary>
        internal static Dictionary<int, PersonaChoice> PersonaChoices => new ()
        {
            { 1, new PersonaChoice { Index = 1, Id = "hero", Name = "HERO", Description = "A tale where one choice can unmake a world" } },
            { 2, new PersonaChoice { Index = 2, Id = "shadow", Name = "SHADOW", Description = "A tale that hides its truth until you bleed for it" } },
            { 3, new PersonaChoice { Index = 3, Id = "ambition", Name = "AMBITION", Description = "A tale that changes every time you look away" } },
        };

        /// <summary>
        /// Simulates the complete opening sequence up to persona selection.
        /// </summary>
        public void SimulateOpeningSequence()
        {
            this.dialogicHelper.SimulateStartTimeline("res://Source/Data/scenes/scene1_narrative/opening_scene.dtl");

            // Simulate opening lines
            foreach (var line in ExpectedOpeningLines)
            {
                this.dialogicHelper.SimulateTextSignal(line);
            }

            GD.Print("[TEST] Opening sequence simulated");
        }

        /// <summary>
        /// Simulates the player selecting a persona.
        /// </summary>
        /// <param name="personaIndex">The persona index (1=HERO, 2=SHADOW, 3=AMBITION).</param>
        public void SimulatePersonaSelection(int personaIndex)
        {
            if (!PersonaChoices.ContainsKey(personaIndex))
            {
                throw new ArgumentException($"Invalid persona index: {personaIndex}", nameof(personaIndex));
            }

            var persona = PersonaChoices[personaIndex];
            this.dialogicHelper.SimulateChoice(personaIndex - 1, persona.Name);
            this.dialogicHelper.SetVariable("selected_persona", persona.Id);

            // Update game state if available
            if (this.gameState != null && Enum.TryParse<DreamweaverThread>(persona.Name, true, out var thread))
            {
                this.gameState.DreamweaverThread = thread;
            }

            GD.Print($"[TEST] Selected persona: {persona.Name}");
        }

        /// <summary>
        /// Simulates the player progressing through a story block.
        /// </summary>
        /// <param name="blockIndex">The story block index.</param>
        /// <param name="choiceIndex">The choice to make within the block.</param>
        public void SimulateStoryBlock(int blockIndex, int choiceIndex)
        {
            if (blockIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockIndex), "Block index must be non-negative");
            }

            if (choiceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(choiceIndex), "Choice index must be non-negative");
            }

            this.dialogicHelper.SimulateTextSignal($"StoryBlock_{blockIndex}_Display");
            this.dialogicHelper.SimulateChoice(choiceIndex, $"Choice_{choiceIndex}");
            this.dialogicHelper.SetVariable($"story_block_{blockIndex}_choice", choiceIndex);

            GD.Print($"[TEST] Story block {blockIndex} completed with choice {choiceIndex}");
        }

        /// <summary>
        /// Simulates the player entering their name.
        /// </summary>
        /// <param name="playerName">The name to enter.</param>
        public void SimulateNameInput(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                throw new ArgumentException("Player name cannot be null or empty", nameof(playerName));
            }

            this.dialogicHelper.SimulateTextSignal("What is your name?");
            this.dialogicHelper.SimulateTextInput(playerName);
            this.dialogicHelper.SetVariable("player_name", playerName);

            // Update game state if available
            if (this.gameState != null)
            {
                this.gameState.PlayerName = playerName;
            }

            GD.Print($"[TEST] Player name entered: {playerName}");
        }

        /// <summary>
        /// Simulates the player answering the secret question.
        /// </summary>
        /// <param name="responseIndex">The response index (1-3).</param>
        public void SimulateSecretQuestionResponse(int responseIndex)
        {
            if (!SecretQuestionResponses.ContainsKey(responseIndex))
            {
                throw new ArgumentException($"Invalid response index: {responseIndex}", nameof(responseIndex));
            }

            var response = SecretQuestionResponses[responseIndex];
            this.dialogicHelper.SimulateTextSignal("Omega asks: Can you keep a secret?");
            this.dialogicHelper.SimulateChoice(responseIndex - 1, response);
            this.dialogicHelper.SetVariable("secret_response", response);

            // Update game state if available
            if (this.gameState != null)
            {
                this.gameState.PlayerSecret = response;
            }

            GD.Print($"[TEST] Secret question answered: {response}");
        }

        /// <summary>
        /// Simulates the complete Scene1 playthrough with specified choices.
        /// </summary>
        /// <param name="personaIndex">The persona to select (1-3).</param>
        /// <param name="playerName">The player name to enter.</param>
        /// <param name="storyChoices">Dictionary mapping story block index to choice index.</param>
        /// <param name="secretResponseIndex">The secret question response (1-3).</param>
        public void SimulateCompletePlaythrough(
            int personaIndex,
            string playerName,
            Dictionary<int, int> storyChoices,
            int secretResponseIndex)
        {
            if (storyChoices == null)
            {
                throw new ArgumentNullException(nameof(storyChoices));
            }

            // Opening sequence
            this.SimulateOpeningSequence();

            // Persona selection
            this.SimulatePersonaSelection(personaIndex);

            // Story blocks
            foreach (var kvp in storyChoices)
            {
                this.SimulateStoryBlock(kvp.Key, kvp.Value);
            }

            // Name input
            this.SimulateNameInput(playerName);

            // Secret question
            this.SimulateSecretQuestionResponse(secretResponseIndex);

            // End timeline
            this.dialogicHelper.SimulateTimelineEnd();

            GD.Print("[TEST] Complete playthrough simulated");
        }

        /// <summary>
        /// Validates that the game state matches expected values after a playthrough.
        /// </summary>
        /// <param name="expectedPersona">The expected Dreamweaver thread.</param>
        /// <param name="expectedName">The expected player name.</param>
        /// <param name="expectedSecret">The expected secret response.</param>
        /// <returns><see langword="true"/> if all validations pass; otherwise, <see langword="false"/>.</returns>
        public bool ValidateGameState(DreamweaverThread expectedPersona, string expectedName, string expectedSecret)
        {
            if (this.gameState == null)
            {
                GD.PrintErr("[TEST] GameState not available for validation");
                return false;
            }

            bool isValid = true;

            if (this.gameState.DreamweaverThread != expectedPersona)
            {
                GD.PrintErr($"[TEST] Persona mismatch: expected {expectedPersona}, got {this.gameState.DreamweaverThread}");
                isValid = false;
            }

            if (this.gameState.PlayerName != expectedName)
            {
                GD.PrintErr($"[TEST] Name mismatch: expected {expectedName}, got {this.gameState.PlayerName}");
                isValid = false;
            }

            if (this.gameState.PlayerSecret != expectedSecret)
            {
                GD.PrintErr($"[TEST] Secret mismatch: expected {expectedSecret}, got {this.gameState.PlayerSecret}");
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Represents a persona choice option.
        /// </summary>
        internal class PersonaChoice
        {
            /// <summary>
            /// Gets or sets the numeric index of the persona (1-3).
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Gets or sets the persona identifier (hero/shadow/ambition).
            /// </summary>
            public string Id { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the display name of the persona.
            /// </summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the description text for the persona.
            /// </summary>
            public string Description { get; set; } = string.Empty;
        }
    }
}
