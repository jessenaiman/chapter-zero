namespace OmegaSpiral.Source.Scripts.Infrastructure.StageManagement;

/// <summary>
/// Stage manager for Stage 2 (Echo Chamber).
/// Loads the Echo Hub, which is responsible for orchestrating the stage sequence.
/// </summary>
public sealed class Stage2Manager : StageManagerBase
{
    private const string Stage2EntryScene = "res://source/stages/stage_2/echo_hub.tscn";

    public Stage2Manager()
        : base(Stage2EntryScene)
    {
    }

    /// <inheritdoc/>
    public override int StageId => 2;
}
