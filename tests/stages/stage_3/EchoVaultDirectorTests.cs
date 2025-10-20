using GdUnit4;
using Godot;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Stages.Stage3;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Stages.Stage3;

/// <summary>
/// Regression tests for Echo Vault data loading.
/// </summary>
[TestSuite]
public partial class EchoVaultDirectorTests : Node
{
    [After]
    public void TearDown()
    {
        EchoVaultDirector.Reset();
    }

    [TestCase]
    public void PlanLoadsFromJson()
    {
        EchoVaultPlan plan = EchoVaultDirector.GetPlan();

        Assertions.AssertThat(plan.Metadata.Palette).IsEqual("monochrome_to_glitch");
        Assertions.AssertThat(plan.PointsLedger.Count).IsGreater(2);
        Assertions.AssertThat(plan.Beats.Count).IsGreater(5);
        Assertions.AssertThat(plan.Echoes.Count).IsGreater(5);
    }

    [TestCase]
    public void FactoryMapsEchoVaultData()
    {
        var config = OmegaSpiral.Source.Scripts.Infrastructure.ConfigurationService.LoadConfiguration("res://source/stages/stage_3/stage3.json");
        NarrativeSceneData data = NarrativeSceneFactory.Create(config);

        Assertions.AssertThat(data.EchoVault).IsNotNull();
        Assertions.AssertThat(data.EchoVault!.Beats.Count).IsGreater(5);
        Assertions.AssertThat(data.EchoVault.EchoDefinitions[0].Mechanics.Signature)
            .Contains("Shield");
    }
}
