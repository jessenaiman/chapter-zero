// <copyright file="IDungeonEventPublisher.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.domain.Dungeon
{
    /// <summary>
    /// Publishes domain events related to dungeon progression.
    /// </summary>
    public interface IDungeonEventPublisher
    {
        /// <summary>
        /// Publishes a stage cleared event.
        /// </summary>
        /// <param name="domainEvent">The stage cleared event.</param>
        void PublishStageCleared(DungeonStageClearedEvent domainEvent);

        /// <summary>
        /// Publishes a stage entered event.
        /// </summary>
        /// <param name="domainEvent">The stage entered event.</param>
        void PublishStageEntered(DungeonStageEnteredEvent domainEvent);
    }
}
