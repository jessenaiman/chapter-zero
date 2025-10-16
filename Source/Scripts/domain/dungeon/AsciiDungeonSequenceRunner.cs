// <copyright file="AsciiDungeonSequenceRunner.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts.Domain.Dungeon
{
    using System.Linq;
    using OmegaSpiral.Source.Scripts.Common;

    /// <summary>
    /// Runs an ASCII dungeon sequence, publishing events as stages are entered and cleared.
    /// </summary>
    public sealed class AsciiDungeonSequenceRunner
    {
        private readonly AsciiDungeonSequence sequence;
        private readonly IDungeonEventPublisher eventPublisher;
        private readonly IDreamweaverAffinityService affinityService;
        private int currentStageIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsciiDungeonSequenceRunner"/> class.
        /// </summary>
        /// <param name="sequence">The dungeon sequence to run.</param>
        /// <param name="eventPublisher">The event publisher for domain events.</param>
        /// <param name="affinityService">The affinity service for dreamweaver changes.</param>
        public AsciiDungeonSequenceRunner(
            AsciiDungeonSequence sequence,
            IDungeonEventPublisher eventPublisher,
            IDreamweaverAffinityService affinityService)
        {
            this.sequence = sequence ?? throw new System.ArgumentNullException(nameof(sequence));
            this.eventPublisher = eventPublisher ?? throw new System.ArgumentNullException(nameof(eventPublisher));
            this.affinityService = affinityService ?? throw new System.ArgumentNullException(nameof(affinityService));
        }

        /// <summary>
        /// Starts running the dungeon sequence.
        /// </summary>
        public void Start()
        {
            this.currentStageIndex = 0;
            if (this.sequence.Stages.Count > 0)
            {
                this.PublishCurrentStageEntered();
            }
        }

        /// <summary>
        /// Completes the current stage and advances to the next one.
        /// </summary>
        public void CompleteCurrentStage()
        {
            if (this.currentStageIndex >= 0 && this.currentStageIndex < this.sequence.Stages.Count)
            {
                var currentStage = this.sequence.Stages[this.currentStageIndex];
                var clearedEvent = new DungeonStageClearedEvent(currentStage.Id);
                this.eventPublisher.PublishStageCleared(clearedEvent);

                this.currentStageIndex++;
                if (this.currentStageIndex < this.sequence.Stages.Count)
                {
                    this.PublishCurrentStageEntered();
                }
            }
        }

        private void PublishCurrentStageEntered()
        {
            var stage = this.sequence.Stages[this.currentStageIndex];
            var enteredEvent = new DungeonStageEnteredEvent(
                stage.Id,
                this.currentStageIndex,
                stage.Owner,
                stage.Map);
            this.eventPublisher.PublishStageEntered(enteredEvent);
        }
    }
}
