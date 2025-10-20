// <copyright file="DreamweaverScoreTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage1;

using GdUnit4;
using OmegaSpiral.Source.Scripts.Stages.Stage1;
using static GdUnit4.Assertions;

/// <summary>
/// Lightweight checks around the DreamweaverScore helper to guard against ID or scoring regressions.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class DreamweaverScoreTests
{
    /// <summary>
    /// Recording the secret question should mark the secret as answered and expand the max scoring pool.
    /// </summary>
    [TestCase]
    public void RecordChoice_WithSecretId_SetsSecretAnswered()
    {
        var score = new DreamweaverScore();

        score.RecordChoice("secret_question", "Yes.", lightPoints: 2, shadowPoints: 2, ambitionPoints: 0);

        AssertThat(score.SecretAnswered).IsTrue();
        AssertThat(score.MaximumPossiblePoints).IsEqual(12);
    }

    /// <summary>
    /// Ensures balance ending triggers when no thread reaches the dominance threshold.
    /// </summary>
    [TestCase]
    public void GetDominantThread_WithBalancedScores_ReturnsBalance()
    {
        var score = new DreamweaverScore();

        score.RecordChoice("question1_name", "Light leaning", lightPoints: 1, shadowPoints: 1, ambitionPoints: 1);
        score.RecordChoice("question2_bridge", "Shadow leaning", lightPoints: 1, shadowPoints: 2, ambitionPoints: 0);
        score.RecordChoice("secret_question", "Ambition leaning", lightPoints: 0, shadowPoints: 1, ambitionPoints: 2);

        AssertThat(score.GetDominantThread()).IsEqual("Balance");
    }
}
