namespace OmegaSpiral.Source.Scripts.Infrastructure.StageManagement;

/// <summary>
/// Stage manager for Stage 5 (Fractured Escape).
/// </summary>
public sealed class Stage5Manager : StageManagerBase
{
    private const string Stage5EntryScene = "res://source/stages/stage_5/stage_5_main.tscn";

    public Stage5Manager()
        : base(Stage5EntryScene)
    {
    }

    /// <inheritdoc/>
    public override int StageId => 5;
}
