// <copyright file="Stage4Controller.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Stage4
{
    /// <summary>
    /// Stage 4 controller managing the Echo Vault stage flow.
    /// Inherits from StageController for unified stage orchestration.
    /// Delegates specific beat execution to Stage4BeatExecutor.
    /// </summary>
    [GlobalClass]
    public partial class Stage4Controller : StageController
    {
        private const string Stage4ManifestPath = "res://source/stages/stage_4/stage4_manifest.json";
        private Stage4BeatExecutor? beatExecutor;

        /// <inheritdoc/>
        protected override int StageId => 4;

        /// <inheritdoc/>
        protected override string StageManifestPath => Stage4ManifestPath;

        /// <inheritdoc/>
        protected override async Task OnStageInitializeAsync()
        {
            GD.Print("[Stage4Controller] Initializing Stage 4 - Echo Vault");
            beatExecutor = new Stage4BeatExecutor();

            // Load the first beat from manifest
            await TransitionToSceneAsync(StageManifest!.GetFirstScene()!.Id).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override async Task OnStageCompleteAsync()
        {
            GD.Print("[Stage4Controller] Stage 4 complete");

            // TODO: Report affinity score to GameState here

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
