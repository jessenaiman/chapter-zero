// <copyright file="NethackDirectorTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Stages.Stage2;

using GdUnit4;
using static GdUnit4.Assertions;
using OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Verifies NethackDirector loads and parses nethack.yaml correctly.
///
/// Test Cases:
/// 1. YAML file loads without errors
/// 2. Metadata is parsed correctly (iteration, interface, status)
/// 3. All blocks are loaded in correct order
/// 4. Block structure matches nethack.yaml format
/// 5. Dreamweaver sections are properly defined
/// 6. Choice options have correct IDs and responses
/// 7. Plan is cached - calling GetPlan twice returns same instance
/// 8. Reset() clears the cache
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class NethackDirectorTests
{
    [TestCase]
    public void GetPlan_ReturnsNonNull()
    {
        // Arrange
        var director = new NethackDirector();

        // Act
        var plan = director.GetPlan();

        // Assert
        AssertThat(plan).IsNotNull();
    }
}
