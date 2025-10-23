// <copyright file="Stage4AffinityTracker.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Stages.Stage4
{
    /// <summary>
    /// Tracks Dreamweaver affinity scores throughout Stage 4.
    /// Implements IAffinityTracker for unified score reporting.
    /// </summary>
    public partial class Stage4AffinityTracker : IAffinityTracker
    {
        private readonly Dictionary<string, int> scores = new()
        {
            { "light", 0 },
            { "shadow", 0 },
            { "ambition", 0 }
        };

        private string? claimedDreamweaver;

        /// <summary>
        /// Records a character selection or combat outcome.
        /// </summary>
        /// <param name="dreamweaverId">The Dreamweaver ID (light, shadow, ambition).</param>
        /// <param name="points">Points to award.</param>
        public void RecordScore(string dreamweaverId, int points)
        {
            if (this.scores.ContainsKey(dreamweaverId))
            {
                this.scores[dreamweaverId] += points;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, int> GetAllScores()
        {
            return new Dictionary<string, int>(this.scores);
        }

        /// <inheritdoc/>
        public string DetermineClaim()
        {
            if (this.claimedDreamweaver != null)
            {
                return this.claimedDreamweaver;
            }

            this.claimedDreamweaver = this.scores
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key)
                .First()
                .Key;

            return this.claimedDreamweaver;
        }
    }
}
