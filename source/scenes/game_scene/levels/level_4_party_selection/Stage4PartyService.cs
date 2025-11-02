// <copyright file="Stage4PartyService.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using OmegaSpiral.Source.Backend;
using OmegaSpiral.Source.Backend.Common;
using OmegaSpiral.Source.Backend.Narrative;

namespace OmegaSpiral.Source.Stages.Stage4
{
    /// <summary>
    /// Manages party persistence for Stage 4.
    /// Handles storing selected characters to GameState.
    /// Separated from Stage4Controller to follow Single Responsibility Principle.
    /// </summary>
    public class Stage4PartyService
    {
        private readonly List<string> _SelectedCharacters = new();

        /// <summary>
        /// Records a character selection.
        /// </summary>
        public void AddSelectedCharacter(string characterId)
        {
            _SelectedCharacters.Add(characterId);
        }

        /// <summary>
        /// Persists all selected characters to GameState.
        /// </summary>
        public void PersistPartyToGameState(GameState gameState)
        {
            if (gameState == null)
            {
                return;
            }

            foreach (var charId in _SelectedCharacters)
            {
                var character = new Character
                {
                    Name = charId, // TODO: Load actual character data
                    Class = CharacterClass.Fighter,
                    Race = CharacterRace.Human,
                    Level = 3
                };
                gameState.PlayerParty.AddMember(character);
            }
        }

        /// <summary>
        /// Gets all selected characters.
        /// </summary>
        public Collection<string> GetSelectedCharacters()
        {
            return new Collection<string>(_SelectedCharacters);
        }

        /// <summary>
        /// Clears all selections.
        /// </summary>
        public void Clear()
        {
            _SelectedCharacters.Clear();
        }
    }
}
