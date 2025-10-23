// <copyright file="DungeonStageClearedEvent.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.domain.Dungeon
{
    /// <summary>
    /// Domain event raised when a dungeon stage is cleared.
    /// </summary>
    /// <param name="StageId">The identifier of the cleared stage.</param>
    public sealed record DungeonStageClearedEvent(string StageId);
}
