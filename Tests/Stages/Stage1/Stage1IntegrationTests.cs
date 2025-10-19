// <copyright file="Stage1IntegrationTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System.Threading.Tasks;
using Xunit;

namespace OmegaSpiral.Tests.Stages.Stage1;

/// <summary>
/// Integration tests for Stage 1 opening sequence.
/// </summary>
[Collection("Sequential")]
public class Stage1IntegrationTests
{
    /// <summary>
    /// Tests that BootSequence scene loads without errors.
    /// </summary>
    [Fact]
    public void BootSequence_Loads_Successfully()
    {
        var scene = ResourceLoader.Load<PackedScene>("res://Source/Stages/Stage1/BootSequence.tscn");
        Assert.NotNull(scene);
    }

    /// <summary>
    /// Tests that all 8 Stage 1 scenes exist and are loadable.
    /// </summary>
    [Theory]
    [InlineData("res://Source/Stages/Stage1/BootSequence.tscn")]
    [InlineData("res://Source/Stages/Stage1/OpeningMonologue.tscn")]
    [InlineData("res://Source/Stages/Stage1/Question1_Name.tscn")]
    [InlineData("res://Source/Stages/Stage1/Question2_Bridge.tscn")]
    [InlineData("res://Source/Stages/Stage1/Question3_Voice.tscn")]
    [InlineData("res://Source/Stages/Stage1/Question4_Name.tscn")]
    [InlineData("res://Source/Stages/Stage1/Question5_Secret.tscn")]
    [InlineData("res://Source/Stages/Stage1/Question6_Continue.tscn")]
    public void Stage1Scene_Loads_Successfully(string scenePath)
    {
        var scene = ResourceLoader.Load<PackedScene>(scenePath);
        Assert.NotNull(scene);
    }

    /// <summary>
    /// Tests that TerminalBase scene structure is correct.
    /// </summary>
    [Fact]
    public void TerminalBase_HasRequiredNodes()
    {
        var scene = ResourceLoader.Load<PackedScene>("res://Source/Stages/Stage1/TerminalBase.tscn");
        Assert.NotNull(scene);
        // TODO: Instantiate and verify node hierarchy
    }
}

/// <summary>
/// Unit tests for DreamweaverScore scoring logic.
/// </summary>
public class DreamweaverScoreTests
{
    /// <summary>
    /// Tests initial score state.
    /// </summary>
    [Fact]
    public void New_Score_HasZeroPoints()
    {
        var score = new OmegaSpiral.Source.Scripts.Stages.Stage1.DreamweaverScore();
        Assert.Equal(0, score.TotalPoints);
        Assert.Equal(0, score.LightPoints);
        Assert.Equal(0, score.ShadowPoints);
        Assert.Equal(0, score.AmbitionPoints);
    }

    /// <summary>
    /// Tests recording a choice.
    /// </summary>
    [Fact]
    public void RecordChoice_Updates_Scores()
    {
        var score = new OmegaSpiral.Source.Scripts.Stages.Stage1.DreamweaverScore();
        score.RecordChoice("test_q1", "test_choice", 2, 0, 0);

        Assert.Equal(2, score.LightPoints);
        Assert.Equal(0, score.ShadowPoints);
        Assert.Equal(0, score.AmbitionPoints);
        Assert.Equal(2, score.TotalPoints);
    }

    /// <summary>
    /// Tests multiple choice recording accumulates properly.
    /// </summary>
    [Fact]
    public void Multiple_Choices_Accumulate()
    {
        var score = new OmegaSpiral.Source.Scripts.Stages.Stage1.DreamweaverScore();
        score.RecordChoice("q1", "choice1", 2, 0, 0);
        score.RecordChoice("q2", "choice2", 0, 3, 0);
        score.RecordChoice("q3", "choice3", 0, 0, 3);

        Assert.Equal(2, score.LightPoints);
        Assert.Equal(3, score.ShadowPoints);
        Assert.Equal(3, score.AmbitionPoints);
        Assert.Equal(8, score.TotalPoints);
    }

    /// <summary>
    /// Tests Light thread dominance determination.
    /// </summary>
    [Fact]
    public void DominantThread_Light_When_Highest()
    {
        var score = new OmegaSpiral.Source.Scripts.Stages.Stage1.DreamweaverScore();
        score.RecordChoice("q1", "choice", 5, 1, 1);

        Assert.Equal("Light", score.GetDominantThread());
    }

    /// <summary>
    /// Tests Shadow thread dominance determination.
    /// </summary>
    [Fact]
    public void DominantThread_Shadow_When_Highest()
    {
        var score = new OmegaSpiral.Source.Scripts.Stages.Stage1.DreamweaverScore();
        score.RecordChoice("q1", "choice", 1, 5, 1);

        Assert.Equal("Shadow", score.GetDominantThread());
    }

    /// <summary>
    /// Tests Ambition thread dominance determination.
    /// </summary>
    [Fact]
    public void DominantThread_Ambition_When_Highest()
    {
        var score = new OmegaSpiral.Source.Scripts.Stages.Stage1.DreamweaverScore();
        score.RecordChoice("q1", "choice", 1, 1, 5);

        Assert.Equal("Ambition", score.GetDominantThread());
    }

    /// <summary>
    /// Tests Balance ending when no thread reaches 60% dominance.
    /// </summary>
    [Fact]
    public void DominantThread_Balance_When_No_Clear_Winner()
    {
        var score = new OmegaSpiral.Source.Scripts.Stages.Stage1.DreamweaverScore();
        score.RecordChoice("q1", "choice", 3, 3, 2); // 50%, 50%, 33% - no 60% threshold

        Assert.Equal("Balance", score.GetDominantThread());
    }

    /// <summary>
    /// Tests light tiebreaker when all scores equal.
    /// </summary>
    [Fact]
    public void DominantThread_Light_Wins_Tiebreaker()
    {
        var score = new OmegaSpiral.Source.Scripts.Stages.Stage1.DreamweaverScore();
        score.RecordChoice("q1", "choice", 2, 2, 2);

        Assert.Equal("Light", score.GetDominantThread());
    }

    /// <summary>
    /// Tests choice history tracking.
    /// </summary>
    [Fact]
    public void ChoiceHistory_Tracks_All_Decisions()
    {
        var score = new OmegaSpiral.Source.Scripts.Stages.Stage1.DreamweaverScore();
        score.RecordChoice("q1", "hero", 2, 0, 0);
        score.RecordChoice("q2", "bridge", 0, 3, 0);

        Assert.Equal(2, score.ChoiceHistory.Count);
        Assert.Equal("q1", score.ChoiceHistory[0].QuestionId);
        Assert.Equal("q2", score.ChoiceHistory[1].QuestionId);
    }

    /// <summary>
    /// Tests reset clears all scores.
    /// </summary>
    [Fact]
    public void Reset_Clears_All_Scores()
    {
        var score = new OmegaSpiral.Source.Scripts.Stages.Stage1.DreamweaverScore();
        score.RecordChoice("q1", "choice", 5, 3, 2);

        score.Reset();

        Assert.Equal(0, score.TotalPoints);
        Assert.Equal(0, score.ChoiceHistory.Count);
    }

    /// <summary>
    /// Tests score summary generation.
    /// </summary>
    [Fact]
    public void ScoreSummary_Formats_Correctly()
    {
        var score = new OmegaSpiral.Source.Scripts.Stages.Stage1.DreamweaverScore();
        score.RecordChoice("q1", "choice", 2, 2, 2);

        string summary = score.GetScoreSummary();

        Assert.Contains("Light: 2", summary);
        Assert.Contains("Shadow: 2", summary);
        Assert.Contains("Ambition: 2", summary);
        Assert.Contains("Total: 6", summary);
    }
}
