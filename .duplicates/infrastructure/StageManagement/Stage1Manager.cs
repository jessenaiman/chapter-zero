namespace OmegaSpiral.Source.Scripts.Infrastructure.StageManagement;

/// <summary>
/// Stage manager for Stage 1 (Ghost Terminal).
/// Prefers the first beat from the stage manifest before falling back to the root scene.
/// </summary>
public sealed class Stage1Manager : StageManagerBase
{
    private const string Stage1EntryScene = "res://source/stages/stage_1/ghost.tscn";
    private const string Stage1ManifestPath = "res://source/stages/stage_1/stage_manifest.json";

    public Stage1Manager()
        : base(Stage1EntryScene)
    {
    }

    /// <inheritdoc/>
    public override int StageId => 1;

    /// <inheritdoc/>
    protected override string? StageManifestPath => Stage1ManifestPath;
}
