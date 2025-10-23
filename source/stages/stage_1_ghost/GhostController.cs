// <copyright file="Stage1Controller.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Stages.Stage1;

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
    public override string ManifestPath => Stage1ManifestPath;

    /// <summary>
    /// Called by boot_sequence when it's complete and ready to transition to opening monologue.
    /// </summary>
    public async Task AdvanceFromBootSequenceAsync()
    {
        await TransitionToNextSceneAsync("boot_sequence").ConfigureAwait(false);
    }

    /// <summary>
    /// Called by opening_monologue when it's complete and ready to transition to the first question.
    /// </summary>
    public async Task AdvanceFromOpeningMonologueAsync()
    {
        await TransitionToNextSceneAsync("opening_monologue").ConfigureAwait(false);
    }

    /// <summary>
    /// Called by question_1_name when the player completes the identity question.
    /// </summary>
    public async Task AdvanceFromQuestion1NameAsync()
    {
        await TransitionToNextSceneAsync("question_1_name").ConfigureAwait(false);
    }

    /// <summary>
    /// Called by story_fragment when the player completes the bridge parable choice.
    /// </summary>
    public async Task AdvanceFromStoryFragmentAsync()
    {
        await TransitionToNextSceneAsync("story_fragment").ConfigureAwait(false);
    }

    /// <summary>
    /// Called by secret_question when the player answers the secret question.
    /// </summary>
    public async Task AdvanceFromSecretQuestionAsync()
    {
        await TransitionToNextSceneAsync("secret_question").ConfigureAwait(false);
    }

    /// <summary>
    /// Called by secret_reveal when the secret reveal animation completes.
    /// </summary>
    public async Task AdvanceFromSecretRevealAsync()
    {
        await TransitionToNextSceneAsync("secret_reveal").ConfigureAwait(false);
    }

    /// <summary>
    /// Called by name_question when the player completes the naming question.
    /// </summary>
    public async Task AdvanceFromNameQuestionAsync()
    {
        await TransitionToNextSceneAsync("name_question").ConfigureAwait(false);
    }

    /// <summary>
    /// Called by exit when Stage 1 is complete and ready to transition to Stage 2.
    /// </summary>
    public async Task AdvanceFromExitAsync()
    {
        await CompleteStage1Async().ConfigureAwait(false);
    }

    /// <summary>
    /// Called when Stage 1 is complete (all beats finished) and ready to transition to Stage 2.
    /// </summary>
    private async Task CompleteStage1Async()
    {
        const string Stage2Path = "res://source/stages/echo_hub/echo_hub_main.tscn";
        await TransitionToNextStageAsync(Stage2Path).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task OnStageInitializeAsync()
    {
        GD.Print("[Stage1Controller] Initializing Stage 1 - Opening Sequence");

        // Start with boot sequence (first beat in flow)
        await TransitionToSceneAsync("boot_sequence").ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task OnStageCompleteAsync()
    {
        GD.Print("[Stage1Controller] Stage 1 complete, cleaning up");
        await Task.CompletedTask.ConfigureAwait(false);
    }
}
