// <copyright file="DungeonStageEnteredEvent.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Collections.Generic;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Domain.Dungeon
{
    /// <summary>
    /// Domain event raised when a dungeon stage is entered.
    /// </summary>
    /// <param name="StageId">The identifier of the entered stage.</param>
    /// <param name="StageIndex">The index of the entered stage in the sequence.</param>
    /// <param name="Owner">The Dreamweaver type that owns this stage.</param>
    /// <param name="MapRows">The ASCII map rows for this stage.</param>
    public sealed record DungeonStageEnteredEvent(
        string StageId,
        int StageIndex,
        DreamweaverType Owner,
        IReadOnlyList<string> MapRows);
}
