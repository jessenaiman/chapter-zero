// <copyright file="IAffinityTracker.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace OmegaSpiral.Source.Scripts.Common
{
    /// <summary>
    /// Interface for affinity/score tracking across stages.
    /// </summary>
    public interface IAffinityTracker
    {
        /// <summary>
        /// Gets all Dreamweaver scores as a read-only dictionary.
        /// </summary>
        /// <returns>Immutable view of current scores.</returns>
        IReadOnlyDictionary<string, int> GetAllScores();

        /// <summary>
        /// Determines the final Dreamweaver claim based on accumulated scores.
        /// </summary>
        /// <returns>The Dreamweaver ID with the highest score.</returns>
        string DetermineClaim();
    }
}
