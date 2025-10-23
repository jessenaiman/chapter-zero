using OmegaSpiral.Source.Scripts.domain.Dungeon;

namespace OmegaSpiral.Source.Narrative
{
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
