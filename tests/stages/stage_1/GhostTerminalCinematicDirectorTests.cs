// <copyright file="GhostTerminalCinematicDirectorTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage1;

using System;
using System.Linq;
using GdUnit4;
using OmegaSpiral.Source.Scripts.Stages.Stage1;
using static GdUnit4.Assertions;

/// <summary>
/// Verifies GhostTerminalCinematicDirector is wired to the on-disk narrative data and
/// keeps the beat ordering we expect for the playable Stage 1 sequence.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class GhostTerminalCinematicDirectorTests
{
    /// <summary>
    /// Ensures a clean state before each test invocation.
    /// </summary>
    [Before]
    public void SetUp()
    {
        GhostTerminalCinematicDirector.Reset();
    }

    /// <summary>
    /// Cleans up cached state after a test completes.
    /// </summary>
    [After]
    public void TearDown()
    {
        GhostTerminalCinematicDirector.Reset();
    }

    /// <summary>
    /// Confirms the cinematic plan contains every beat in the scripted order, including
    /// the secret setup/choice pair before the naming beats.
    /// </summary>
    [TestCase]
    public void GetPlan_ReturnsBeatsInExpectedOrder()
    {
        GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.GetPlan();

        var beatKinds = plan.AllBeats.Select(beat => beat.Kind).ToArray();

        AssertThat(beatKinds).IsEqual(new[]
        {
            GhostTerminalBeatKind.BootSequence,
            GhostTerminalBeatKind.Monologue,
            GhostTerminalBeatKind.InitialChoice,
            GhostTerminalBeatKind.StoryIntro,
            GhostTerminalBeatKind.StoryChoice,
            GhostTerminalBeatKind.StoryContinuation,
            GhostTerminalBeatKind.SecretSetup,
            GhostTerminalBeatKind.SecretChoice,
            GhostTerminalBeatKind.NameSetup,
            GhostTerminalBeatKind.NameChoice,
            GhostTerminalBeatKind.Exit,
        });
    }

    /// <summary>
    /// Verifies the director caches the cinematic plan instance until a reset is triggered.
    /// </summary>
    [TestCase]
    public void GetPlan_CachesPlanUntilReset()
    {
        GhostTerminalCinematicPlan firstPlan = GhostTerminalCinematicDirector.GetPlan();
        GhostTerminalCinematicPlan secondPlan = GhostTerminalCinematicDirector.GetPlan();

        AssertThat(object.ReferenceEquals(firstPlan, secondPlan)).IsTrue();

        GhostTerminalCinematicDirector.Reset();
        GhostTerminalCinematicPlan reloadedPlan = GhostTerminalCinematicDirector.GetPlan();

        AssertThat(object.ReferenceEquals(firstPlan, reloadedPlan)).IsFalse();
    }

    /// <summary>
    /// Validates that the secret reveal retains the omega code fragment and is marked
    /// as persistent narrative data.
    /// </summary>
    [TestCase]
    public void SecretReveal_ContainsCodeFragmentAndPersists()
    {
        GhostTerminalCinematicPlan plan = GhostTerminalCinematicDirector.GetPlan();
        GhostTerminalSecretRevealPlan reveal = plan.SecretChoice.Reveal;

        AssertThat(reveal.Persistent).IsTrue();
        AssertThat(reveal.Lines.Any(line => line.Contains("∞ ◊ Ω ≋ ※", StringComparison.Ordinal))).IsTrue();
    }
}
