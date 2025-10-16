// <copyright file="IDreamweaverAffinityService.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Domain.Dungeon
{
    using OmegaSpiral.Source.Scripts.Common;
    using Models;

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
