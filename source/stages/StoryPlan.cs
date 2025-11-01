using OmegaSpiral.Source.Backend.Narrative;

namespace OmegaSpiral.Source.Stages
{
    /// <summary>
    /// Base class for cinematic plans that wrap story scripts.
    /// </summary>
    public abstract class StoryPlan
    {
        /// <summary>
        /// Gets the story script for this plan.
        /// </summary>
        public StoryBlock Script { get; protected set; }
    }
}
