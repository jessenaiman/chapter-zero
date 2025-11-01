using System;
using OmegaSpiral.Source.Backend.Narrative;
using OmegaSpiral.Source.Backend;

namespace OmegaSpiral.Source.Stages
{
    public class StageConfiguration
    {
        public required string DataPath { get; set; }
        public string? ScenePath { get; set; }
        public required Func<StoryBlock, StoryPlan> PlanFactory { get; set; }
        public required Func<Scene, object, SceneManager> ManagerFactory { get; set; }
    }
}
