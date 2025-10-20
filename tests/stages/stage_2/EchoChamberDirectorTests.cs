using GdUnit4;
using Godot;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Stages.Stage2;

namespace OmegaSpiral.Tests.Stages.Stage2;

/// <summary>
/// Lightweight regression tests ensuring the Stage 2 data asset parses correctly.
/// </summary>
[TestSuite]
public partial class EchoChamberDirectorTests : Node
{
    [After]
    public void TearDown()
    {
        EchoChamberDirector.Reset();
    }

    [TestCase]
    public void LoadsPlanFromJson()
    {
        EchoChamberPlan plan = EchoChamberDirector.GetPlan();

        Assertions.AssertThat(plan.Metadata.Status)
            .IsEqual("ECHO_SIMULATION_ACTIVE");

        Assertions.AssertThat(plan.Dreamweavers.Count).IsEqual(3);
        Assertions.AssertThat(plan.Interludes.Count).IsEqual(3);
        Assertions.AssertThat(plan.Chambers.Count).IsEqual(3);

        Assertions.AssertThat(plan.Finale.Claimants.ContainsKey("light")).IsTrue();
    }

    [TestCase]
    public void FactoryMapsEchoChamberData()
    {
        var config = Source.Scripts.Infrastructure.ConfigurationService.LoadConfiguration("res://source/stages/stage_2/stage2.json");
        NarrativeSceneData data = NarrativeSceneFactory.Create(config);

        Assertions.AssertThat(data.EchoChamber).IsNotNull();
        Assertions.AssertThat(data.EchoChamber!.Interludes.Count).IsEqual(3);
        Assertions.AssertThat(data.EchoChamber.Chambers[0].Objects.Count).IsGreater(2);
    }
}
