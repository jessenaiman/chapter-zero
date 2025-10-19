// <copyright file="IDreamweaverAffinityService.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.domain.Dungeon.Models;

namespace OmegaSpiral.Source.Scripts.domain.Dungeon
{
    /// <summary>
    /// Service for managing Dreamweaver affinity changes.
    /// </summary>
    public interface IDreamweaverAffinityService
    {
        /// <summary>
        /// Applies an affinity change to the specified Dreamweaver type.
        /// </summary>
        /// <param name="owner">The Dreamweaver type to modify.</param>
        /// <param name="change">The affinity change to apply.</param>
        void ApplyChange(DreamweaverType owner, DreamweaverAffinityChange change);
    }
}
