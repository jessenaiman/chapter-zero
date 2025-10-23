namespace OmegaSpiral.Source.Scripts.Infrastructure.StageManagement;

/// <summary>
/// Stage manager for Stage 4 (Liminal Township build).
/// </summary>
public sealed class Stage4Manager : StageManagerBase
{
    private const string Stage4EntryScene = "res://source/stages/stage_4/Stage4Main.tscn";

    public Stage4Manager()
        : base(Stage4EntryScene)
    {
    }

    /// <inheritdoc/>
    public override int StageId => 4;
}
