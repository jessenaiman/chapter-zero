namespace OmegaSpiral.Source.Scripts.Infrastructure.StageManagement;

/// <summary>
/// Stage manager for Stage 6 (System Log Epilogue).
/// </summary>
public sealed class Stage6Manager : StageManagerBase
{
    private const string Stage6EntryScene = "res://source/stages/stage_6/system_log.tscn";

    public Stage6Manager()
        : base(Stage6EntryScene)
    {
    }

    /// <inheritdoc/>
    public override int StageId => 6;
}
