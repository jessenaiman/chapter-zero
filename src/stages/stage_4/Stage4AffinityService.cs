// <copyright file="Stage4AffinityService.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Stages.Stage4
{
    /// <summary>
    /// Manages Dreamweaver affinity for Stage 4.
    /// Filters character options based on Stage 2 influence.
    /// Separated from Stage4Controller to follow Single Responsibility Principle.
    /// </summary>
    public class Stage4AffinityService
    {
        /// <summary>
        /// Filters available characters based on Stage 2 Dreamweaver influence.
        /// </summary>
        public Collection<string> FilterCharactersByStage2Influence(Collection<string> availableCharacters, GameState gameState)
        {
            if (gameState == null || gameState.DreamweaverScores == null)
            {
                return availableCharacters;
            }

            // Get dominant Dreamweaver from GameState
            var dominantDw = DetermineDominantDreamweaver(gameState.DreamweaverScores);
            if (string.IsNullOrEmpty(dominantDw))
            {
                return availableCharacters;
            }

            // TODO: Filter characters based on dominant Dreamweaver
            // For now, return all characters
            return availableCharacters;
        }

        /// <summary>
        /// Determines the dominant Dreamweaver type from scores.
        /// </summary>
        private string DetermineDominantDreamweaver(Dictionary<DreamweaverType, int> scores)
        {
            var maxScore = 0;
            var dominant = "";

            foreach (var kvp in scores)
            {
                if (kvp.Value > maxScore)
                {
                    maxScore = kvp.Value;
                    dominant = kvp.Key.ToString().ToLower();
                }
            }

            return dominant;
        }
    }
}
