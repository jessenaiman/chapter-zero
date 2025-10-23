// <copyright file="Stage1AffinityTracker.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Stages.Stage1
{
    /// <summary>
    /// Tracks Dreamweaver affinity scores for Stage 1 opening sequence.
    /// Implements IAffinityTracker for unified score reporting.
    /// </summary>
    public partial class Stage1AffinityTracker : IAffinityTracker
    {
        private readonly Dictionary<string, int> scores = new()
        {
            { "light", 0 },
            { "shadow", 0 },
            { "ambition", 0 }
        };

        private readonly List<ChoiceRecord> choiceHistory = new();
        private string? claimedDreamweaver;

        /// <summary>
        /// Records a choice and updates thread scores.
        /// </summary>
        public void RecordChoice(string questionId, string choiceText, int lightPoints, int shadowPoints, int ambitionPoints)
        {
            scores["light"] += lightPoints;
            scores["shadow"] += shadowPoints;
            scores["ambition"] += ambitionPoints;
            choiceHistory.Add(new ChoiceRecord(questionId, choiceText, lightPoints, shadowPoints, ambitionPoints));
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, int> GetAllScores()
        {
            return new Dictionary<string, int>(scores);
        }

        /// <inheritdoc/>
        public string DetermineClaim()
        {
            if (claimedDreamweaver != null)
            {
                return claimedDreamweaver;
            }
            claimedDreamweaver = scores
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key)
                .First().Key;
            return claimedDreamweaver;
        }
    }
}
