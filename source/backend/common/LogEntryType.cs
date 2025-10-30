// <copyright file="LogEntryType.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Backend.Common;
    /// <summary>
    /// Types of log entries for the combat log.
    /// </summary>
    public enum LogEntryType
    {
        /// <summary>
        /// General information entry.
        /// </summary>
        Info = 0,

        /// <summary>
        /// Damage dealt entry.
        /// </summary>
        Damage = 1,

        /// <summary>
        /// Healing received entry.
        /// </summary>
        Heal = 2,

        /// <summary>
        /// Attack missed entry.
        /// </summary>
        Miss = 3,

        /// <summary>
        /// Action performed entry.
        /// </summary>
        Action = 4,

        /// <summary>
        /// Status effect applied entry.
        /// </summary>
        StatusEffect = 5,

        /// <summary>
        /// Battler defeated entry.
        /// </summary>
        Defeated = 6,

        /// <summary>
        /// Milestone achieved entry.
        /// </summary>
        Milestone = 7,

        /// <summary>
        /// Breakthrough achieved entry.
        /// </summary>
        Breakthrough = 8,

        /// <summary>
        /// System message entry.
        /// </summary>
        System = 9,
    }
