// <copyright file="LogEntryType.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Models;

/// <summary>
/// Defines the types of combat log entries.
/// </summary>
/// <remarks>
/// Used to categorize and format combat log messages for display.
/// </remarks>
public enum LogEntryType
{
    /// <summary>
    /// Generic informational message.
    /// </summary>
    Info,

    /// <summary>
    /// Combat action (attack, spell cast, etc.).
    /// </summary>
    Action,

    /// <summary>
    /// Damage dealt to a target.
    /// </summary>
    Damage,

    /// <summary>
    /// Healing applied to a target.
    /// </summary>
    Healing,

    /// <summary>
    /// Status effect applied or removed.
    /// </summary>
    StatusEffect,

    /// <summary>
    /// Critical hit or special event.
    /// </summary>
    Critical,

    /// <summary>
    /// Turn or round information.
    /// </summary>
    Turn,

    /// <summary>
    /// Error or warning message.
    /// </summary>
    Error,

    /// <summary>
    /// Combat result (victory, defeat, etc.).
    /// </summary>
    Result,
}
