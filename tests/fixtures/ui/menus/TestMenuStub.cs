// <copyright file="TestMenuStub.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Ui.Menus;

namespace OmegaSpiral.Tests.Fixtures.Ui.Menus;

/// <summary>
/// Minimal stub for testing MenuUi behavior without triggering MainMenu initialization.
/// This exists solely to provide a testable MenuUi instance for unit tests.
/// It does NOT populate buttons or load manifests like the real MainMenu does.
/// </summary>
[GlobalClass]
public partial class TestMenuStub : MenuUi
{
    /// <summary>
    /// Override PopulateMenuButtons to do nothing in test context.
    /// This prevents hanging on manifest loading during test setup.
    /// </summary>
    protected override void PopulateMenuButtons()
    {
        // Do nothing - tests will manually add buttons as needed
    }
}
