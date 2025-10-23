// <copyright file="Stage2DataDeserializationTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage2;

using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Source.Stages.Stage2;

/// <summary>
/// Unit tests for Stage 2 data deserialization.
/// Verifies that stage_2.json deserializes correctly to Stage2NarrativeData.
/// </summary>
[TestSuite]
public partial class Stage2DataDeserializationTests
{
    private const string Stage2JsonPath = "res://source/stages/stage_2/stage_2.json";

    /// <summary>
    /// Test that stage_2.json deserializes without errors.
    /// </summary>
    [TestCase]
    public void Stage2JsonShouldDeserializeSuccessfully()
    {
        // Arrange
        var loader = new NarrativeDataLoader();

        // Act
        var data = loader.LoadNarrativeData<Stage2NarrativeData>(Stage2JsonPath);

        // Assert
        AssertThat(data).IsNotNull();
        AssertThat(data!.Type).IsEqual("echo_chamber_stage");
    }

    /// <summary>
    /// Test that metadata loads correctly.
    /// </summary>
    [TestCase]
    public void Stage2MetadataShouldLoadCorrectly()
    {
        // Arrange
        var loader = new NarrativeDataLoader();
        var data = loader.LoadNarrativeData<Stage2NarrativeData>(Stage2JsonPath);

        // Assert
        AssertThat(data!.Metadata).IsNotNull();
        AssertThat(data.Metadata.Status).IsEqual("ECHO_SIMULATION_ACTIVE");
        AssertThat(data.Metadata.SystemIntro).IsNotEmpty();
        AssertThat(data.Metadata.SystemIntro.Count).IsGreater(0);
    }

    /// <summary>
    /// Test that three Dreamweavers are loaded correctly.
    /// </summary>
    [TestCase]
    public void Stage2ShouldLoad3Dreamweavers()
    {
        // Arrange
        var loader = new NarrativeDataLoader();
        var data = loader.LoadNarrativeData<Stage2NarrativeData>(Stage2JsonPath);

        // Assert
        AssertThat(data!.Dreamweavers).IsNotEmpty();
        AssertThat(data.Dreamweavers.Count).IsEqual(3);
        AssertThat(data.Dreamweavers[0].Id).IsEqual("light");
        AssertThat(data.Dreamweavers[1].Id).IsEqual("shadow");
        AssertThat(data.Dreamweavers[2].Id).IsEqual("ambition");
    }

    /// <summary>
    /// Test that three interludes are loaded correctly.
    /// </summary>
    [TestCase]
    public void Stage2ShouldLoad3Interludes()
    {
        // Arrange
        var loader = new NarrativeDataLoader();
        var data = loader.LoadNarrativeData<Stage2NarrativeData>(Stage2JsonPath);

        // Assert
        AssertThat(data!.Interludes).IsNotEmpty();
        AssertThat(data.Interludes.Count).IsEqual(3);
        AssertThat(data.Interludes[0].Owner).IsEqual("light");
        AssertThat(data.Interludes[1].Owner).IsEqual("shadow");
        AssertThat(data.Interludes[2].Owner).IsEqual("ambition");
    }

    /// <summary>
    /// Test that each interlude has 3 options.
    /// </summary>
    [TestCase]
    public void EachInterloudeShouldHave3Options()
    {
        // Arrange
        var loader = new NarrativeDataLoader();
        var data = loader.LoadNarrativeData<Stage2NarrativeData>(Stage2JsonPath);

        // Assert
        foreach (var interlude in data!.Interludes)
        {
            AssertThat(interlude.Options.Count).IsEqual(3);
            foreach (var option in interlude.Options)
            {
                AssertThat(option.Alignment).IsNotEmpty();
                AssertThat(option.Banter).IsNotNull();
            }
        }
    }

    /// <summary>
    /// Test that three chambers are loaded correctly.
    /// </summary>
    [TestCase]
    public void Stage2ShouldLoad3Chambers()
    {
        // Arrange
        var loader = new NarrativeDataLoader();
        var data = loader.LoadNarrativeData<Stage2NarrativeData>(Stage2JsonPath);

        // Assert
        AssertThat(data!.Chambers).IsNotEmpty();
        AssertThat(data.Chambers.Count).IsEqual(3);
        AssertThat(data.Chambers[0].Owner).IsEqual("light");
        AssertThat(data.Chambers[1].Owner).IsEqual("shadow");
        AssertThat(data.Chambers[2].Owner).IsEqual("ambition");
    }

    /// <summary>
    /// Test that each chamber has 3 set pieces (door, monster, chest).
    /// </summary>
    [TestCase]
    public void EachChamberShouldHave3SetPieces()
    {
        // Arrange
        var loader = new NarrativeDataLoader();
        var data = loader.LoadNarrativeData<Stage2NarrativeData>(Stage2JsonPath);

        // Assert
        foreach (var chamber in data!.Chambers)
        {
            AssertThat(chamber.Objects.Count).IsEqual(3);

            // Verify slot assignments
            var slots = chamber.Objects.ConvertAll(o => o.Slot);
            AssertThat(slots).Contains("door");
            AssertThat(slots).Contains("monster");
            AssertThat(slots).Contains("chest");

            // Verify each object has banter
            foreach (var obj in chamber.Objects)
            {
                AssertThat(obj.Banter).IsNotNull();
                AssertThat(obj.Banter!.Approve).IsNotNull();
                AssertThat(obj.Banter.Dissent).IsNotEmpty();
            }
        }
    }

    /// <summary>
    /// Test that each chamber has decoys.
    /// </summary>
    [TestCase]
    public void EachChamberShouldHaveDecoys()
    {
        // Arrange
        var loader = new NarrativeDataLoader();
        var data = loader.LoadNarrativeData<Stage2NarrativeData>(Stage2JsonPath);

        // Assert
        foreach (var chamber in data!.Chambers)
        {
            AssertThat(chamber.Decoys).IsNotEmpty();
            AssertThat(chamber.Decoys.Count).IsEqual(2);
            foreach (var decoy in chamber.Decoys)
            {
                AssertThat(decoy.Id).IsNotEmpty();
                AssertThat(decoy.RevealText).IsNotEmpty();
            }
        }
    }

    /// <summary>
    /// Test that finale data loads correctly with all claimants.
    /// </summary>
    [TestCase]
    public void FinaleShouldHaveAllClaimants()
    {
        // Arrange
        var loader = new NarrativeDataLoader();
        var data = loader.LoadNarrativeData<Stage2NarrativeData>(Stage2JsonPath);

        // Assert
        AssertThat(data!.Finale).IsNotNull();
        AssertThat(data.Finale.Claimants).IsNotNull();
        AssertThat(data.Finale.Claimants.Light).IsNotNull();
        AssertThat(data.Finale.Claimants.Shadow).IsNotNull();
        AssertThat(data.Finale.Claimants.Ambition).IsNotNull();
        AssertThat(data.Finale.SystemOutro).IsNotEmpty();
    }
}
