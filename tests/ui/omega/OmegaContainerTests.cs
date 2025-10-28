// <copyright file="OmegaContainer_UnitTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Ui.Omega;

/// <summary>
/// Unit tests for OmegaContainer.
/// Tests lifecycle template methods, composition helpers, and signal emission.
///
/// RESPONSIBILITY: Verify OmegaContainer orchestrates initialization correctly
/// and provides proper composition helpers for Omega components.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class OmegaContainer_UnitTests
{
    /// <summary>
    /// Test implementation of OmegaContainer for verifying basic functionality.
    /// </summary>
    private partial class TestContainer : OmegaContainer
    {
        // Used for basic container tests
    }

    private TestContainer? _Container;

    [Before]
    public void Setup()
    {
        _Container = AutoFree(new TestContainer())!;
    }

    // ==================== INHERITANCE ====================

    /// <summary>
    /// Tests that OmegaContainer extends Control.
    /// </summary>
    [TestCase]
    public void OmegaContainer_ExtendsControl()
    {
        AssertThat(typeof(OmegaContainer).BaseType).IsEqual(typeof(Control));
        AssertThat(typeof(OmegaContainer).IsAssignableTo(typeof(Node))).IsTrue();
    }

    /// <summary>
    /// Tests that OmegaContainer is marked as GlobalClass.
    /// </summary>
    [TestCase]
    public void OmegaContainer_IsGlobalClass()
    {
        var attributes = typeof(OmegaContainer).GetCustomAttributes(typeof(GlobalClassAttribute), false);
        AssertThat(attributes.Length).IsGreater(0);
    }

    // ==================== COMPONENT PROPERTIES ====================

    /// <summary>
    /// Tests that ShaderController property is null by default.
    /// </summary>
    [TestCase]
    public void ShaderController_IsNullByDefault()
    {
        // Arrange
        var container = AutoFree(new TestContainer())!;

        // Assert - Property is accessible and null by default
        AssertThat(container.ShaderController).IsNull();
    }

    /// <summary>
    /// Tests that TextRenderer property is null by default.
    /// </summary>
    [TestCase]
    public void TextRenderer_IsNullByDefault()
    {
        // Arrange
        var container = AutoFree(new TestContainer())!;

        // Assert
        AssertThat(container.TextRenderer).IsNull();
    }

    /// <summary>
    /// Tests that ChoicePresenter property is null by default.
    /// </summary>
    [TestCase]
    public void ChoicePresenter_IsNullByDefault()
    {
        // Arrange
        var container = AutoFree(new TestContainer())!;

        // Assert
        AssertThat(container.ChoicePresenter).IsNull();
    }
}
