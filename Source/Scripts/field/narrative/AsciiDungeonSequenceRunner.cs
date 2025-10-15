namespace OmegaSpiral.Source.Scripts.Field.Narrative
{
    using System;
    using System.Collections.Generic;
    using OmegaSpiral.Source.Scripts.Common;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon;
    using OmegaSpiral.Source.Scripts.Domain.Dungeon.Models;

    /// <summary>
    /// Coordinates progression through the ASCII dungeon sequence, publishing stage events and applying affinity changes.
    /// </summary>
    public sealed class AsciiDungeonSequenceRunner
    {
        private readonly AsciiDungeonSequence sequence;
        private readonly IDungeonEventPublisher publisher;
        private readonly IDreamweaverAffinityService affinityService;
        private int currentStageIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsciiDungeonSequenceRunner"/> class.
        /// </summary>
        /// <param name="sequence">The aggregate containing validated stages.</param>
        /// <param name="publisher">The event publisher for stage notifications.</param>
        /// <param name="affinityService">The service that applies Dreamweaver affinity changes.</param>
        public AsciiDungeonSequenceRunner(
            AsciiDungeonSequence sequence,
            IDungeonEventPublisher publisher,
            IDreamweaverAffinityService affinityService)
        {
            this.sequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
            this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            this.affinityService = affinityService ?? throw new ArgumentNullException(nameof(affinityService));
        }

        /// <summary>
        /// Starts the sequence at the first stage and publishes the corresponding event.
        /// </summary>
        public void Start()
        {
            this.currentStageIndex = 0;
            this.PublishStageEntered();
        }

        /// <summary>
        /// Resolves a glyph interaction within the current stage and applies affinity changes.
        /// </summary>
        /// <param name="glyph">The glyph interacted with.</param>
        /// <exception cref="InvalidOperationException">Thrown when the sequence has not been started.</exception>
        public void ResolveInteraction(char glyph)
        {
            if (this.currentStageIndex < 0)
            {
                throw new InvalidOperationException("The dungeon sequence has not been started.");
            }

            var stage = this.sequence.Stages[this.currentStageIndex];
            var result = stage.ResolveInteraction(glyph);
            this.affinityService.ApplyChange(result.AlignedTo, result.Change);
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
                new DungeonStageClearedEvent(this.currentStageIndex, stage.Owner, stage.Map));

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
                new DungeonStageEnteredEvent(this.currentStageIndex, stage.Owner, stage.Map));
        }
    }

    /// <summary>
    /// Contract for publishing dungeon stage lifecycle events.
    /// </summary>
    public interface IDungeonEventPublisher
    {
        /// <summary>
        /// Publishes a notification that a stage has been entered.
        /// </summary>
        /// <param name="domainEvent">The event payload.</param>
        void PublishStageEntered(DungeonStageEnteredEvent domainEvent);

        /// <summary>
        /// Publishes a notification that a stage has been cleared.
        /// </summary>
        /// <param name="domainEvent">The event payload.</param>
        void PublishStageCleared(DungeonStageClearedEvent domainEvent);
    }

    /// <summary>
    /// Contract for applying Dreamweaver affinity changes.
    /// </summary>
    public interface IDreamweaverAffinityService
    {
        /// <summary>
        /// Applies an affinity change for the specified Dreamweaver.
        /// </summary>
        /// <param name="owner">The Dreamweaver receiving the change.</param>
        /// <param name="change">The change to apply.</param>
        void ApplyChange(DreamweaverType owner, DreamweaverAffinityChange change);
    }

    /// <summary>
    /// Event payload emitted when a stage is entered.
    /// </summary>
    /// <param name="StageIndex">The zero-based stage index.</param>
    /// <param name="Owner">The owning Dreamweaver.</param>
    /// <param name="MapRows">The stage's ASCII map rows.</param>
    public sealed record DungeonStageEnteredEvent(int StageIndex, DreamweaverType Owner, IReadOnlyList<string> MapRows);

    /// <summary>
    /// Event payload emitted when a stage is cleared.
    /// </summary>
    /// <param name="StageIndex">The zero-based stage index.</param>
    /// <param name="Owner">The owning Dreamweaver.</param>
    /// <param name="MapRows">The stage's ASCII map rows.</param>
    public sealed record DungeonStageClearedEvent(int StageIndex, DreamweaverType Owner, IReadOnlyList<string> MapRows);
}
