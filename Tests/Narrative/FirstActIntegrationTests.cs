// <copyright file="FirstActIntegrationTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Functional.Narrative
{
    using System.IO;
    using GdUnit4;
    using OmegaSpiral.Source.Scripts.Field.Narrative;
    using OmegaSpiral.Source.Scripts.Infrastructure;
    using static GdUnit4.Assertions;

    /// <summary>
    /// Integration tests that validate authored JSON scenes for the Ghost Terminal first act.
    /// </summary>
    [TestSuite]
    public class FirstActIntegrationTests
    {
        /// <summary>
        /// Ensures each ghost-terminal stage JSON file produces a cinematic plan with expected beats.
        /// </summary>
        [TestCase]
        public void LoadStageConfiguration_WhenJsonAuthoringComplete_GeneratesPlanForEachThread()
        {
            string[] stageIds = { "hero", "shadow", "ambition", "omega" };

            foreach (string stageId in stageIds)
            {
                string relativePath = Path.Combine("Source", "Data", "stages", "ghost-terminal", $"{stageId}.json");
                string jsonContent = File.ReadAllText(relativePath);
                var config = ConfigurationService.LoadConfigurationFromString(jsonContent, relativePath);

                NarrativeSceneData sceneData = NarrativeSceneFactory.Create(config);
                GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.BuildPlan(sceneData);

                AssertThat(sceneData.OpeningLines.Count).IsGreater(0);
                AssertThat(plan.Beats.Count).IsGreater(0);
                AssertThat(plan.Beats[^1].Type).IsEqual(GhostTerminalBeatType.ExitLine);
            }
        }
    }
}
