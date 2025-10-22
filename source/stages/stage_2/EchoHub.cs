using System.Threading.Tasks;
using Godot;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Orchestrator for the Echo Chamber stage (Stage 2).
/// Manages the flow of interludes and chambers, tracks Dreamweaver affinity,
/// and determines the final claim. Mirrors the GhostTerminalDirector pattern from Stage 1.
/// </summary>
[GlobalClass]
public partial class EchoHub : TerminalBase
{
    private EchoChamberPlan? plan;
    private EchoOrchestratorBeat? orchestrator;
    private EchoAffinityTracker? affinityTracker;
    private int currentBeatIndex;

    private Label? statusLabel;
    private Control? contentContainer;

    /// <summary>
    /// Emitted when the entire Echo Chamber stage completes.
    /// </summary>
    /// <param name="claimedDreamweaver">The Dreamweaver that claimed the player.</param>
    /// <param name="scores">Final affinity scores for all Dreamweavers.</param>
    [Signal]
    public delegate void StageCompleteEventHandler(string claimedDreamweaver, Godot.Collections.Dictionary scores);

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Set terminal mode to minimal for Stage 2 - only basic text functionality
        terminalMode = TerminalMode.Minimal;

        // Initialize base TerminalBase functionality
        base._Ready();

        this.statusLabel = this.GetNodeOrNull<Label>("%StatusLabel");
        this.contentContainer = this.GetNodeOrNull<Control>("%ContentContainer");

        this.plan = EchoChamberDirector.GetPlan();
        this.orchestrator = new EchoOrchestratorBeat(this.plan);
        this.affinityTracker = new EchoAffinityTracker();

        GD.Print($"[EchoHub] Loaded Echo Chamber with {this.orchestrator.BeatCount} beats");

        // Start orchestration after scene is fully loaded
        this.CallDeferred(nameof(this.StartOrchestrationAsync));
    }

    /// <summary>
    /// Begins the orchestrated playthrough of all beats.
    /// </summary>
    private async void StartOrchestrationAsync()
    {
        await this.PlayAllBeatsAsync();
    }

    /// <summary>
    /// Plays all beats in sequence from start to finish.
    /// </summary>
    /// <returns>A task that completes when all beats finish.</returns>
    private async Task PlayAllBeatsAsync()
    {
        if (this.orchestrator == null || this.affinityTracker == null)
        {
            GD.PrintErr("[EchoHub] Cannot start orchestration: missing orchestrator or tracker");
            return;
        }

        this.currentBeatIndex = 0;

        while (this.currentBeatIndex < this.orchestrator.BeatCount)
        {
            IEchoBeat beat = this.orchestrator.GetBeat(this.currentBeatIndex);

            GD.Print($"[EchoHub] Playing beat {this.currentBeatIndex + 1}/{this.orchestrator.BeatCount}: {beat.BeatId}");

            await this.PlayBeatAsync(beat);

            this.currentBeatIndex++;
        }

        // All beats complete - emit stage completion
        string claimedDreamweaver = this.affinityTracker.DetermineClaim();
        var scores = new Godot.Collections.Dictionary
        {
            { "light", this.affinityTracker.GetScore("light") },
            { "shadow", this.affinityTracker.GetScore("shadow") },
            { "ambition", this.affinityTracker.GetScore("ambition") }
        };

        GD.Print($"[EchoHub] Stage complete - {claimedDreamweaver} claims the player (scores: {scores})");
        this.EmitSignal(SignalName.StageComplete, claimedDreamweaver, scores);

        // Record to GameState for Stage 3+ (retrieved via autoload)
        var gameState = GetNode<GameState>("/root/GameState");
        if (gameState != null)
        {
            // Map string ID to DreamweaverType enum
            gameState.SelectedDreamweaver = claimedDreamweaver.ToLowerInvariant() switch
            {
                "light" => DreamweaverType.Light,
                "shadow" => DreamweaverType.Mischief,
                "ambition" => DreamweaverType.Wrath,
                _ => null
            };
        }
    }

    /// <summary>
    /// Plays a single beat based on its type.
    /// </summary>
    /// <param name="beat">The beat to play.</param>
    /// <returns>A task that completes when the beat finishes.</returns>
    private async Task PlayBeatAsync(IEchoBeat beat)
    {
        switch (beat.Kind)
        {
            case EchoBeatKind.SystemIntro:
                await this.PlaySystemIntroAsync((EchoIntroBeat)beat);
                break;

            case EchoBeatKind.Interlude:
                await this.PlayInterludeAsync((EchoInterludeBeat)beat);
                break;

            case EchoBeatKind.Chamber:
                await this.PlayChamberAsync((EchoChamberBeat)beat);
                break;

            case EchoBeatKind.Finale:
                await this.PlayFinaleAsync((EchoFinaleBeat)beat);
                break;

            default:
                GD.PrintErr($"[EchoHub] Unknown beat kind: {beat.Kind}");
                break;
        }
    }

    /// <summary>
    /// Plays the system intro beat.
    /// </summary>
    /// <param name="beat">The intro beat.</param>
    /// <returns>A task that completes when intro finishes.</returns>
    private async Task PlaySystemIntroAsync(EchoIntroBeat beat)
    {
        // Display status information
        if (this.statusLabel != null)
        {
            this.statusLabel.Text = $"{beat.Metadata.Status}\nIteration #{beat.Metadata.IterationFallback}";
        }

        // Use TerminalBase functionality to display system intro lines with typewriter effect
        foreach (string line in beat.Metadata.SystemIntro)
        {
            GD.Print($"[Intro] {line}");
            await AppendTextAsync(line + "\n", useGhostEffect: true, charDelaySeconds: 0.03);
            await ToSignal(GetTree().CreateTimer(0.5), SceneTreeTimer.SignalName.Timeout);
        }

        await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);
    }

    /// <summary>
    /// Plays an interlude beat by instantiating the interlude scene and binding data.
    /// </summary>
    /// <param name="beat">The interlude beat.</param>
    /// <returns>A task that completes when interlude finishes.</returns>
    private async Task PlayInterludeAsync(EchoInterludeBeat beat)
    {
        // TODO: Load and instance echo_interlude.tscn
        // TODO: Bind beat.Interlude data to the scene
        // TODO: Wait for user choice
        // TODO: Record choice to affinityTracker

        GD.Print($"[Interlude] Owner: {beat.Interlude.Owner}");
        GD.Print($"[Interlude] Prompt: {beat.Interlude.Prompt}");
        GD.Print($"[Interlude] Options: {beat.Interlude.Options.Count}");

        // Placeholder: simulate choice
        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);

        // Simulate first choice
        if (beat.Interlude.Options.Count > 0 && this.affinityTracker != null)
        {
            var firstOption = beat.Interlude.Options[0];
            this.affinityTracker.RecordInterludeChoice(
                beat.Interlude.Owner,
                firstOption.Id,
                firstOption.Alignment);

            GD.Print($"[Interlude] Recorded choice: {firstOption.Id} -> {firstOption.Alignment}");
        }
    }

    /// <summary>
    /// Plays a chamber beat by instantiating the dungeon scene.
    /// </summary>
    /// <param name="beat">The chamber beat.</param>
    /// <returns>A task that completes when chamber finishes.</returns>
    private async Task PlayChamberAsync(EchoChamberBeat beat)
    {
        // TODO: Load and instance echo_dungeon.tscn
        // TODO: Bind beat.Chamber data to EchoDungeon controller
        // TODO: Wait for object interaction
        // TODO: Record interaction to affinityTracker

        GD.Print($"[Chamber] Owner: {beat.Chamber.Owner}");
        GD.Print($"[Chamber] Objects: {beat.Chamber.Objects.Count}");

        // Placeholder: simulate interaction
        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);

        // Simulate first object interaction
        if (beat.Chamber.Objects.Count > 0 && this.affinityTracker != null)
        {
            var firstObject = beat.Chamber.Objects[0];
            this.affinityTracker.RecordChamberChoice(
                beat.Chamber.Owner,
                firstObject.Slot,
                firstObject.Alignment);

            GD.Print($"[Chamber] Recorded interaction: {firstObject.Slot} -> {firstObject.Alignment}");
        }
    }

    /// <summary>
    /// Plays the finale beat with the Dreamweaver claim.
    /// </summary>
    /// <param name="beat">The finale beat.</param>
    /// <returns>A task that completes when finale finishes.</returns>
    private async Task PlayFinaleAsync(EchoFinaleBeat beat)
    {
        if (this.affinityTracker == null)
        {
            return;
        }

        string claimedDreamweaver = this.affinityTracker.DetermineClaim();

        GD.Print($"[Finale] {claimedDreamweaver} claims the player");

        // TODO: Display claim dialogue from beat.Finale.Claimants[claimedDreamweaver]
        // TODO: Display responses from other Dreamweavers
        // TODO: Display system outro

        await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);
    }
}
