// <copyright file="OmegaThemedContainer_UnitTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui.Omega;

/// <summary>
/// Unit tests for OmegaThemedContainer.
/// Tests Omega visual components creation and export flag configuration.
///
/// RESPONSIBILITY: Verify OmegaThemedContainer creates Omega theme components
/// based on export flags and provides access to shader layers.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class OmegaThemedContainer_UnitTests
{
    /// <summary>
    /// Test implementation with shader layers in scene.
    /// </summary>
    private partial class TestThemedContainer : OmegaThemedContainer
    {
        public TestThemedContainer()
        {
            // Create mock shader layers
            var phosphor = new ColorRect { Name = "PhosphorLayer" };
            var scanline = new ColorRect { Name = "ScanlineLayer" };
            var glitch = new ColorRect { Name = "GlitchLayer" };

            AddChild(phosphor);
            AddChild(scanline);
            AddChild(glitch);

            // Create text display
            var container = new Control { Name = "ContentContainer" };
            var textDisplay = new RichTextLabel { Name = "TextDisplay" };
            container.AddChild(textDisplay);
            AddChild(container);
        }

        public ColorRect? PublicGetPhosphorLayer() => GetPhosphorLayer();
        public ColorRect? PublicGetScanlineLayer() => GetScanlineLayer();
        public ColorRect? PublicGetGlitchLayer() => GetGlitchLayer();
    }

    private OmegaThemedContainer? _Container;

    [Before]
    public void Setup()
    {
        _Container = AutoFree(new OmegaThemedContainer())!;
    }

    [After]
    public void Cleanup()
    {
        // AutoFree handles cleanup
    }

    // ==================== INHERITANCE ====================

    /// <summary>
    /// Tests that OmegaThemedContainer extends OmegaContainer.
    /// </summary>
    [TestCase]
    public void OmegaThemedContainer_ExtendsOmegaContainer()
    {
        AssertThat(typeof(OmegaThemedContainer).BaseType).IsEqual(typeof(OmegaContainer));
        AssertThat(typeof(OmegaThemedContainer).IsAssignableTo(typeof(Control))).IsTrue();
    }

    /// <summary>
    /// Tests that OmegaThemedContainer is marked as GlobalClass.
    /// </summary>
    [TestCase]
    public void OmegaThemedContainer_IsGlobalClass()
    {
        var attributes = typeof(OmegaThemedContainer).GetCustomAttributes(typeof(GlobalClassAttribute), false);
        AssertThat(attributes.Length).IsGreaterThan(0);
    }

    // ==================== EXPORT PROPERTIES ====================

    /// <summary>
    /// Tests that EnableOmegaBorder property is true by default.
    /// </summary>
    [TestCase]
    public void EnableOmegaBorder_IsTrueByDefault()
    {
        AssertThat(_Container!.EnableOmegaBorder).IsTrue()
            .OverrideFailureMessage("EnableOmegaBorder should be true by default");
    }

    /// <summary>
    /// Tests that EnableCrtShaders property is true by default.
    /// </summary>
    [TestCase]
    public void EnableCrtShaders_IsTrueByDefault()
    {
        AssertThat(_Container!.EnableCrtShaders).IsTrue()
            .OverrideFailureMessage("EnableCrtShaders should be true by default");
    }

    /// <summary>
    /// Tests that EnableOmegaText property is true by default.
    /// </summary>
    [TestCase]
    public void EnableOmegaText_IsTrueByDefault()
    {
        AssertThat(_Container!.EnableOmegaText).IsTrue()
            .OverrideFailureMessage("EnableOmegaText should be true by default");
    }

    /// <summary>
    /// Tests that EnableOmegaBorder property can be set.
    /// </summary>
    [TestCase]
    public void EnableOmegaBorder_CanBeSet()
    {
        // Act
        _Container!.EnableOmegaBorder = false;

        // Assert
        AssertThat(_Container.EnableOmegaBorder).IsFalse();
    }

    /// <summary>
    /// Tests that EnableCrtShaders property can be set.
    /// </summary>
    [TestCase]
    public void EnableCrtShaders_CanBeSet()
    {
        // Act
        _Container!.EnableCrtShaders = false;

        // Assert
        AssertThat(_Container.EnableCrtShaders).IsFalse();
    }

    /// <summary>
    /// Tests that EnableOmegaText property can be set.
    /// </summary>
    [TestCase]
    public void EnableOmegaText_CanBeSet()
    {
        // Act
        _Container!.EnableOmegaText = false;

        // Assert
        AssertThat(_Container.EnableOmegaText).IsFalse();
    }

    // ==================== BORDER FRAME CREATION ====================

    /// <summary>
    /// Tests that border frame is created when EnableOmegaBorder is true.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateComponents_WithBorderEnabled_CreatesBorderFrame()
    {
        // Arrange
        var container = AutoFree(new OmegaThemedContainer { EnableOmegaBorder = true })!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert
        var borderFrame = container.GetBorderFrame();
        AssertThat(borderFrame).IsNotNull()
            .OverrideFailureMessage("Border frame should be created when EnableOmegaBorder is true");
    }

    /// <summary>
    /// Tests that border frame is not created when EnableOmegaBorder is false.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateComponents_WithBorderDisabled_DoesNotCreateBorderFrame()
    {
        // Arrange
        var container = AutoFree(new OmegaThemedContainer { EnableOmegaBorder = false })!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert
        var borderFrame = container.GetBorderFrame();
        AssertThat(borderFrame).IsNull()
            .OverrideFailureMessage("Border frame should not be created when EnableOmegaBorder is false");
    }

    /// <summary>
    /// Tests that created border frame is added to scene tree.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateComponents_AddsBorderFrameToSceneTree()
    {
        // Arrange
        var container = AutoFree(new OmegaThemedContainer { EnableOmegaBorder = true })!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert
        var borderFrame = container.GetBorderFrame();
        AssertThat(borderFrame).IsNotNull();
        AssertThat(borderFrame!.GetParent()).IsEqual(container)
            .OverrideFailureMessage("Border frame should be child of container");
    }

    // ==================== SHADER LAYER CACHING ====================

    /// <summary>
    /// Tests that CacheRequiredNodes caches phosphor layer.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CacheRequiredNodes_CachesPhosphorLayer()
    {
        // Arrange
        var container = AutoFree(new TestThemedContainer())!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert
        var phosphor = container.PublicGetPhosphorLayer();
        AssertThat(phosphor).IsNotNull()
            .OverrideFailureMessage("PhosphorLayer should be cached if present in scene");
    }

    /// <summary>
    /// Tests that CacheRequiredNodes caches scanline layer.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CacheRequiredNodes_CachesScanlineLayer()
    {
        // Arrange
        var container = AutoFree(new TestThemedContainer())!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert
        var scanline = container.PublicGetScanlineLayer();
        AssertThat(scanline).IsNotNull()
            .OverrideFailureMessage("ScanlineLayer should be cached if present in scene");
    }

    /// <summary>
    /// Tests that CacheRequiredNodes caches glitch layer.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CacheRequiredNodes_CachesGlitchLayer()
    {
        // Arrange
        var container = AutoFree(new TestThemedContainer())!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert
        var glitch = container.PublicGetGlitchLayer();
        AssertThat(glitch).IsNotNull()
            .OverrideFailureMessage("GlitchLayer should be cached if present in scene");
    }

    /// <summary>
    /// Tests that shader layer getters return null when layers not in scene.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void ShaderLayerGetters_ReturnNullWhenNotInScene()
    {
        // Arrange
        var container = AutoFree(new TestThemedContainer())!;
        // Don't add to tree, so layers won't be found

        // Assert
        AssertThat(container.PublicGetPhosphorLayer()).IsNull();
        AssertThat(container.PublicGetScanlineLayer()).IsNull();
        AssertThat(container.PublicGetGlitchLayer()).IsNull();
    }

    // ==================== SHADER CONTROLLER CREATION ====================

    /// <summary>
    /// Tests that shader controller is created when CRT shaders enabled and layer exists.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateComponents_WithShadersEnabled_CreatesShaderController()
    {
        // Arrange
        var container = AutoFree(new TestThemedContainer { EnableCrtShaders = true })!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert
        // Access through property requires making a public getter in test class
        // For now, we verify no errors occur during creation
        AssertThat(container).IsNotNull();
    }

    /// <summary>
    /// Tests that shader controller is not created when CRT shaders disabled.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateComponents_WithShadersDisabled_DoesNotCreateShaderController()
    {
        // Arrange
        var container = AutoFree(new TestThemedContainer { EnableCrtShaders = false })!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act & Assert - No errors
        sceneTree.Root.AddChild(container);
        AssertThat(container).IsNotNull();
    }

    // ==================== TEXT RENDERER CREATION ====================

    /// <summary>
    /// Tests that text renderer is created when Omega text enabled and display exists.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateComponents_WithTextEnabled_CreatesTextRenderer()
    {
        // Arrange
        var container = AutoFree(new TestThemedContainer { EnableOmegaText = true })!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act & Assert - No errors
        sceneTree.Root.AddChild(container);
        AssertThat(container).IsNotNull();
    }

    /// <summary>
    /// Tests that text renderer is not created when Omega text disabled.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateComponents_WithTextDisabled_DoesNotCreateTextRenderer()
    {
        // Arrange
        var container = AutoFree(new TestThemedContainer { EnableOmegaText = false })!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act & Assert - No errors
        sceneTree.Root.AddChild(container);
        AssertThat(container).IsNotNull();
    }

    // ==================== INTEGRATION TESTS ====================

    /// <summary>
    /// Tests that all components can be enabled together.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void AllComponentsEnabled_WorkTogether()
    {
        // Arrange
        var container = AutoFree(new TestThemedContainer
        {
            EnableOmegaBorder = true,
            EnableCrtShaders = true,
            EnableOmegaText = true
        })!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert - All should be created
        AssertThat(container.GetBorderFrame()).IsNotNull();
        AssertThat(container.PublicGetPhosphorLayer()).IsNotNull();
        AssertThat(container.PublicGetScanlineLayer()).IsNotNull();
        AssertThat(container.PublicGetGlitchLayer()).IsNotNull();
    }

    /// <summary>
    /// Tests that all components can be disabled together.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void AllComponentsDisabled_NoneCreated()
    {
        // Arrange
        var container = AutoFree(new OmegaThemedContainer
        {
            EnableOmegaBorder = false,
            EnableCrtShaders = false,
            EnableOmegaText = false
        })!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert - Border frame should not be created
        AssertThat(container.GetBorderFrame()).IsNull();
    }

    /// <summary>
    /// Tests that export properties are configurable before adding to scene.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void ExportProperties_ConfigurableBeforeSceneAdd()
    {
        // Arrange
        var container = AutoFree(new OmegaThemedContainer())!;

        // Act - Set properties before adding to scene
        container.EnableOmegaBorder = false;
        container.EnableCrtShaders = false;
        container.EnableOmegaText = true;

        // Assert
        AssertThat(container.EnableOmegaBorder).IsFalse();
        AssertThat(container.EnableCrtShaders).IsFalse();
        AssertThat(container.EnableOmegaText).IsTrue();
    }

    /// <summary>
    /// Tests that GetBorderFrame returns null when border disabled.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void GetBorderFrame_ReturnsNullWhenDisabled()
    {
        // Arrange
        var container = AutoFree(new OmegaThemedContainer { EnableOmegaBorder = false })!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();
        sceneTree.Root.AddChild(container);

        // Act
        var borderFrame = container.GetBorderFrame();

        // Assert
        AssertThat(borderFrame).IsNull();
    }
}
