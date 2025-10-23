using System.Collections.Generic;
using System.Linq;

namespace OmegaSpiral.Source.Scripts.Infrastructure.StageManagement;

/// <summary>
/// Provides lookup and enumeration of stage managers used by the main menu and tooling.
/// </summary>
public static class StageManagerRegistry
{
    private static readonly IReadOnlyDictionary<int, IStageManager> StageManagers = new Dictionary<int, IStageManager>
    {
        { 1, new Stage1Manager() },
        { 2, new Stage2Manager() },
        { 3, new Stage3Manager() },
        { 4, new Stage4Manager() },
        { 5, new Stage5Manager() },
        { 6, new Stage6Manager() },
    };

    /// <summary>
    /// Retrieves the stage manager for the specified stage identifier.
    /// </summary>
    /// <param name="stageId">Numeric stage identifier.</param>
    /// <returns>The matching stage manager or <c>null</c> if none is registered.</returns>
    public static IStageManager? GetStageManager(int stageId)
    {
        return StageManagers.TryGetValue(stageId, out IStageManager? manager) ? manager : null;
    }

    /// <summary>
    /// Gets the registered stage managers.
    /// </summary>
    public static IReadOnlyCollection<IStageManager> GetAllStageManagers() => StageManagers.Values.ToArray();
}
