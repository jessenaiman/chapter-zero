namespace OmegaSpiral.Source.Scripts.Infrastructure.StageManagement;

/// <summary>
/// Stage manager for Stage 3 (Echo Vault).
/// Transitions directly into the Stage 3 main scene.
/// </summary>
public sealed class Stage3Manager : StageManagerBase
{
    private const string Stage3EntryScene = "res://source/stages/stage_3/stage_3_main.tscn";

    public Stage3Manager()
        : base(Stage3EntryScene)
    {
    }

    /// <inheritdoc/>
    public override int StageId => 3;
}
