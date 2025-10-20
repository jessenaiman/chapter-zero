// <copyright file="DreamweaverScoreTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage1;

using GdUnit4;

/// <summary>
/// Lightweight checks around the DreamweaverScore helper to guard against ID or scoring regressions.
///
/// TODO: This test is currently disabled due to Stage1 namespace not being available yet.
/// The original tests verified:
/// - RecordChoice with secret IDs marks secret as answered
/// - GetDominantThread returns correct thread (Light, Shadow, Ambition, or Balance)
/// Re-enable once OmegaSpiral.Source.Scripts.Stages.Stage1 namespace is available.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class DreamweaverScoreTests
{
    // TODO: Restore implementation once Stage1 namespace is available
}
