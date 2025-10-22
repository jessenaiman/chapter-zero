// <copyright file="Stage1Controller.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Ghost;

/// <summary>
/// Controller for Stage 1 (Opening Sequence).
/// Orchestrates the beat-by-beat flow from boot_sequence through exit.
/// Individual beat scenes call back to this controller to advance the flow.
/// This separation keeps scene scripts focused on presentation, not flow logic.
/// </summary>
[GlobalClass]
public partial class Stage1Controller : StageController
{
    private const string Stage1ManifestPath = "res://source/stages/stage_1/stage_manifest.json";

    /// <inheritdoc/>
    protected override int StageId => 1;

    /// <inheritdoc/>
    protected override string StageManifestPath => Stage1ManifestPath;

    /// <summary>
    /// Called by beat_1_boot_sequence when it's complete and ready to transition to opening monologue.
    /// </summary>
    public async Task AdvanceFromBeat1BootSequenceAsync()
    {
        await TransitionToNextSceneAsync("beat_1_boot_sequence");
    }

    /// <summary>
    /// Called by beat_2_opening_monologue when it's complete and ready to transition to the first question.
    /// </summary>
    public async Task AdvanceFromBeat2OpeningMonologueAsync()
    {
        await TransitionToNextSceneAsync("beat_2_opening_monologue");
    }

    /// <summary>
    /// Called by beat_3_question_1_name when the player completes the identity question.
    /// </summary>
    public async Task AdvanceFromBeat3Question1NameAsync()
    {
        await TransitionToNextSceneAsync("beat_3_question_1_name");
    }

    /// <summary>
    /// Called by beat_4_story_fragment when the player completes the bridge parable choice.
    /// </summary>
    public async Task AdvanceFromBeat4StoryFragmentAsync()
    {
        await TransitionToNextSceneAsync("beat_4_story_fragment");
    }

    /// <summary>
    /// Called by beat_5_secret_question when the player answers the secret question.
    /// </summary>
    public async Task AdvanceFromBeat5SecretQuestionAsync()
    {
        await TransitionToNextSceneAsync("beat_5_secret_question");
    }

    /// <summary>
    /// Called by beat_6_secret_reveal when the secret reveal animation completes.
    /// </summary>
    public async Task AdvanceFromBeat6SecretRevealAsync()
    {
        await TransitionToNextSceneAsync("beat_6_secret_reveal");
    }

    /// <summary>
    /// Called by beat_7_name_question when the player completes the naming question.
    /// </summary>
    public async Task AdvanceFromBeat7NameQuestionAsync()
    {
        await TransitionToNextSceneAsync("beat_7_name_question");
    }

    /// <summary>
    /// Called by beat_8_exit when Stage 1 is complete and ready to transition to Stage 2.
    /// </summary>
    public async Task AdvanceFromBeat8ExitAsync()
    {
        await CompleteStage1Async();
    }

    /// <summary>
    /// Called when Stage 1 is complete (all beats finished) and ready to transition to Stage 2.
    /// </summary>
    private async Task CompleteStage1Async()
    {
        const string Stage2Path = "res://source/stages/echo_hub/echo_hub_main.tscn";
        await TransitionToNextStageAsync(Stage2Path);
    }

    /// <inheritdoc/>
    protected override async Task OnStageInitializeAsync()
    {
        GD.Print("[Stage1Controller] Initializing Stage 1 - Opening Sequence");

        // Start with boot sequence (first beat in flow)
        await TransitionToSceneAsync("beat_1_boot_sequence");
    }

    /// <inheritdoc/>
    protected override async Task OnStageCompleteAsync()
    {
        GD.Print("[Stage1Controller] Stage 1 complete, cleaning up");
        await Task.CompletedTask;
    }
}
