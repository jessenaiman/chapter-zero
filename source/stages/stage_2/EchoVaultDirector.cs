using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Infrastructure;

#nullable enable

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Loads and caches the Stage 3 Echo Vault data.
/// </summary>
public static class EchoVaultDirector
{
    private const string DataPath = "res://source/stages/stage_3/stage3.json";
    private const string SchemaPath = "res://source/data/schemas/echo_vault_stage.json";

    private static readonly object Sync = new();
    private static EchoVaultPlan? cachedPlan;

    /// <summary>
    /// Gets the cached plan, loading from disk if necessary.
    /// </summary>
    public static EchoVaultPlan GetPlan()
    {
        if (cachedPlan != null)
        {
            return cachedPlan;
        }

        lock (Sync)
        {
            cachedPlan ??= LoadPlan();
        }

        return cachedPlan!;
    }

    /// <summary>
    /// Clears cached data (useful for tests).
    /// </summary>
    public static void Reset()
    {
        lock (Sync)
        {
            cachedPlan = null;
        }
    }

    private static EchoVaultPlan LoadPlan()
    {
        var config = ConfigurationService.LoadConfiguration(DataPath);

        if (!ConfigurationService.ValidateConfiguration(config, SchemaPath))
        {
            throw new InvalidOperationException("Echo Vault data failed schema validation.");
        }

        NarrativeSceneData sceneData = NarrativeSceneFactory.Create(config);

        if (sceneData.EchoVault == null)
        {
            throw new InvalidOperationException("Echo Vault data missing in narrative scene payload.");
        }

        EchoVaultData vault = sceneData.EchoVault;

        return new EchoVaultPlan(
            vault.Metadata,
            new List<EchoVaultPointsLedgerEntry>(vault.PointsLedger),
            new List<EchoVaultBeat>(vault.Beats),
            new List<EchoVaultEchoDefinition>(vault.EchoDefinitions),
            new List<EchoVaultSpecialOption>(vault.SpecialOptions),
            new List<EchoVaultCombat>(vault.Combats),
            new List<string>(vault.OmegaLogs),
            vault.PartyPersistence);
    }
}

/// <summary>
/// Immutable runtime view of Stage 3 data.
/// </summary>
/// <param name="Metadata">Presentation metadata.</param>
/// <param name="PointsLedger">Decision ownership ledger.</param>
/// <param name="Beats">Ordered beats (selection/combat/finale).</param>
/// <param name="Echoes">Echo definitions catalog.</param>
/// <param name="SpecialOptions">Special options available.</param>
/// <param name="Combats">Combat encounter definitions.</param>
/// <param name="OmegaLogs">Global Omega log strings.</param>
/// <param name="PartyPersistence">Persistence configuration.</param>
public sealed record EchoVaultPlan(
    EchoVaultMetadata Metadata,
    List<EchoVaultPointsLedgerEntry> PointsLedger,
    List<EchoVaultBeat> Beats,
    List<EchoVaultEchoDefinition> Echoes,
    List<EchoVaultSpecialOption> SpecialOptions,
    List<EchoVaultCombat> Combats,
    List<string> OmegaLogs,
    EchoVaultPartyPersistence PartyPersistence);
