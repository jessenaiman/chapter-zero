using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts.domain.Dungeon;
using OmegaSpiral.Source.Scripts.domain.Dungeon.Models;

namespace OmegaSpiral.Source.Stages.Stage2
{
    /// <summary>
    /// Coordinates progression through the Nethack-inspired ASCII dungeon sequence.
    /// Logs interactions for debugging/analytics. Scoring is handled by NethackHub (StageController).
    /// </summary>
    public sealed class NethackSequenceRunner
    {
        private readonly AsciiDungeonSequence sequence;
        private readonly IDungeonEventPublisher publisher;
        private readonly List<DungeonInteractionResult> interactionLog;
        private int currentStageIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="NethackSequenceRunner"/> class.
        /// </summary>
        /// <param name="sequence">The aggregate containing validated stages.</param>
        /// <param name="publisher">The event publisher for stage notifications.</param>
        public NethackSequenceRunner(
            AsciiDungeonSequence sequence,
            IDungeonEventPublisher publisher)
        {
            this.sequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
            this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            this.interactionLog = new List<DungeonInteractionResult>();
        }

        /// <summary>
        /// Gets the interaction log for debugging/analytics.
        /// </summary>
        public IReadOnlyList<DungeonInteractionResult> InteractionLog => this.interactionLog;

        /// <summary>
        /// Starts the sequence at the first stage and publishes the corresponding event.
        /// </summary>
        public void Start()
        {
            this.currentStageIndex = 0;
            this.PublishStageEntered();
        }

        /// <summary>
        /// Resolves a glyph interaction within the current stage and logs it.
        /// Scoring is handled by the chamber scene calling NethackHub.AwardAffinityScore().
        /// </summary>
        /// <param name="glyph">The glyph interacted with.</param>
        /// <returns>The interaction result for display/logging.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the sequence has not been started.</exception>
        public DungeonInteractionResult ResolveInteraction(char glyph)
        {
            if (this.currentStageIndex < 0)
            {
                throw new InvalidOperationException("The dungeon sequence has not been started.");
            }

            var stage = this.sequence.Stages[this.currentStageIndex];
            var result = stage.ResolveInteraction(glyph);

            // Log the interaction for debugging/analytics
            this.interactionLog.Add(result);

            return result;
        }

        /// <summary>
        /// Signals completion of the current stage and moves to the next one, if available.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the sequence has not been started.</exception>
        public void CompleteCurrentStage()
        {
            if (this.currentStageIndex < 0)
            {
                throw new InvalidOperationException("The dungeon sequence has not been started.");
            }

            var stage = this.sequence.Stages[this.currentStageIndex];
            this.publisher.PublishStageCleared(
                new DungeonStageClearedEvent(stage.Id));

            this.currentStageIndex++;
            if (this.currentStageIndex < this.sequence.Stages.Count)
            {
                this.PublishStageEntered();
            }
        }

        private void PublishStageEntered()
        {
            var stage = this.sequence.Stages[this.currentStageIndex];
            this.publisher.PublishStageEntered(
                new DungeonStageEnteredEvent(stage.Id, this.currentStageIndex, stage.Owner, stage.Map));
        }
    }
}
