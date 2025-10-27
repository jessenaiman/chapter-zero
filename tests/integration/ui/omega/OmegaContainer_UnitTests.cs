// <copyright file="OmegaContainer_UnitTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui.Omega;

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
    /// Test implementation of OmegaContainer for verifying lifecycle hooks.
    /// </summary>
    private partial class TestContainer : OmegaContainer
    {
        public bool CacheRequiredNodesCalled { get; private set; }
        public bool CreateComponentsCalled { get; private set; }
        public bool InitializeComponentStatesCalled { get; private set; }
        public int CallSequence { get; private set; }

        protected override void CacheRequiredNodes()
        {
            base.CacheRequiredNodes();
            CacheRequiredNodesCalled = true;
            CallSequence = 1;
        }

        protected override void CreateComponents()
        {
            base.CreateComponents();
            CreateComponentsCalled = true;
            if (CallSequence == 1) CallSequence = 2;
        }

        protected override void InitializeComponentStates()
        {
            base.InitializeComponentStates();
            InitializeComponentStatesCalled = true;
            if (CallSequence == 2) CallSequence = 3;
        }
    }

    /// <summary>
    /// Test implementation that uses composition helpers.
    /// </summary>
    private partial class TestContainerWithComposition : OmegaContainer
    {
        private ColorRect? _ShaderDisplay;
        private RichTextLabel? _TextDisplay;
        private VBoxContainer? _ChoiceContainer;

        public TestContainerWithComposition()
        {
            // Create mock nodes
            _ShaderDisplay = new ColorRect();
            _TextDisplay = new RichTextLabel();
            _ChoiceContainer = new VBoxContainer();

            AddChild(_ShaderDisplay);
            AddChild(_TextDisplay);
            AddChild(_ChoiceContainer);
        }

        protected override void CreateComponents()
        {
            base.CreateComponents();

            // Use composition helpers
            if (_ShaderDisplay != null)
                ComposeShaderController(_ShaderDisplay);

            if (_TextDisplay != null)
                ComposeTextRenderer(_TextDisplay);

            if (_ChoiceContainer != null)
                ComposeChoicePresenter(_ChoiceContainer);
        }

        public IOmegaShaderController? GetShaderController() => ShaderController;
        public IOmegaTextRenderer? GetTextRenderer() => TextRenderer;
        public IOmegaChoicePresenter? GetChoicePresenter() => ChoicePresenter;
    }

    private TestContainer? _Container;

    [Before]
    public void Setup()
    {
        _Container = AutoFree(new TestContainer())!;
    }

    [After]
    public void Cleanup()
    {
        // AutoFree handles cleanup
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

    // ==================== INITIALIZATION LIFECYCLE ====================

    /// <summary>
    /// Tests that _Ready calls CacheRequiredNodes hook.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Ready_CallsCacheRequiredNodes()
    {
        // Arrange - Container created in Setup, not added to tree yet
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act - Adding to tree triggers _Ready()
        sceneTree.Root.AddChild(_Container);

        // Assert
        AssertThat(_Container!.CacheRequiredNodesCalled).IsTrue()
            .OverrideFailureMessage("_Ready should call CacheRequiredNodes");
    }

    /// <summary>
    /// Tests that _Ready calls CreateComponents hook.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Ready_CallsCreateComponents()
    {
        // Arrange
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(_Container);

        // Assert
        AssertThat(_Container!.CreateComponentsCalled).IsTrue()
            .OverrideFailureMessage("_Ready should call CreateComponents");
    }

    /// <summary>
    /// Tests that _Ready calls InitializeComponentStates hook.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Ready_CallsInitializeComponentStates()
    {
        // Arrange
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(_Container);

        // Assert
        AssertThat(_Container!.InitializeComponentStatesCalled).IsTrue()
            .OverrideFailureMessage("_Ready should call InitializeComponentStates");
    }

    /// <summary>
    /// Tests that lifecycle hooks are called in correct order.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void Ready_CallsHooksInCorrectOrder()
    {
        // Arrange
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(_Container);

        // Assert
        AssertThat(_Container!.CallSequence).IsEqual(3)
            .OverrideFailureMessage("Lifecycle hooks should be called in order: CacheRequiredNodes(1) -> CreateComponents(2) -> InitializeComponentStates(3)");
    }

    /// <summary>
    /// Tests that _Ready emits InitializationCompleted signal.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public async void Ready_EmitsInitializationCompletedSignal()
    {
        // Arrange
        var sceneTree = (SceneTree)Engine.GetMainLoop();
        var signalEmitted = false;
        _Container!.InitializationCompleted += () => signalEmitted = true;

        // Act
        sceneTree.Root.AddChild(_Container);
        await AssertSignal(_Container).IsEmitted(OmegaContainer.SignalName.InitializationCompleted).WithTimeout(100);

        // Assert
        AssertThat(signalEmitted).IsTrue()
            .OverrideFailureMessage("_Ready should emit InitializationCompleted signal");
    }

    // ==================== COMPOSITION HELPERS ====================

    /// <summary>
    /// Tests that ComposeShaderController creates shader controller.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void ComposeShaderController_CreatesShaderController()
    {
        // Arrange
        var container = AutoFree(new TestContainerWithComposition())!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert
        var controller = container.GetShaderController();
        AssertThat(controller).IsNotNull()
            .OverrideFailureMessage("ComposeShaderController should create shader controller");
        AssertThat(controller).IsInstanceOf<IOmegaShaderController>();
    }

    /// <summary>
    /// Tests that ComposeTextRenderer creates text renderer.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void ComposeTextRenderer_CreatesTextRenderer()
    {
        // Arrange
        var container = AutoFree(new TestContainerWithComposition())!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert
        var renderer = container.GetTextRenderer();
        AssertThat(renderer).IsNotNull()
            .OverrideFailureMessage("ComposeTextRenderer should create text renderer");
        AssertThat(renderer).IsInstanceOf<IOmegaTextRenderer>();
    }

    /// <summary>
    /// Tests that ComposeChoicePresenter creates choice presenter.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void ComposeChoicePresenter_CreatesChoicePresenter()
    {
        // Arrange
        var container = AutoFree(new TestContainerWithComposition())!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert
        var presenter = container.GetChoicePresenter();
        AssertThat(presenter).IsNotNull()
            .OverrideFailureMessage("ComposeChoicePresenter should create choice presenter");
        AssertThat(presenter).IsInstanceOf<IOmegaChoicePresenter>();
    }

    /// <summary>
    /// Tests that all composition helpers can be used together.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CompositionHelpers_CanBeUsedTogether()
    {
        // Arrange
        var container = AutoFree(new TestContainerWithComposition())!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act
        sceneTree.Root.AddChild(container);

        // Assert - All components should be created
        AssertThat(container.GetShaderController()).IsNotNull();
        AssertThat(container.GetTextRenderer()).IsNotNull();
        AssertThat(container.GetChoicePresenter()).IsNotNull();
    }

    // ==================== VIRTUAL METHOD DEFAULTS ====================

    /// <summary>
    /// Tests that CacheRequiredNodes has safe default implementation.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CacheRequiredNodes_DefaultImplementation_DoesNotThrow()
    {
        // Arrange
        var container = AutoFree(new OmegaContainer())!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act & Assert - No exception thrown
        sceneTree.Root.AddChild(container);
        AssertThat(container).IsNotNull();
    }

    /// <summary>
    /// Tests that CreateComponents has safe default implementation.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateComponents_DefaultImplementation_DoesNotThrow()
    {
        // Arrange
        var container = AutoFree(new OmegaContainer())!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act & Assert - No exception thrown
        sceneTree.Root.AddChild(container);
        AssertThat(container).IsNotNull();
    }

    /// <summary>
    /// Tests that InitializeComponentStates has safe default implementation.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void InitializeComponentStates_DefaultImplementation_DoesNotThrow()
    {
        // Arrange
        var container = AutoFree(new OmegaContainer())!;
        var sceneTree = (SceneTree)Engine.GetMainLoop();

        // Act & Assert - No exception thrown
        sceneTree.Root.AddChild(container);
        AssertThat(container).IsNotNull();
    }

    // ==================== COMPONENT PROPERTIES ====================

    /// <summary>
    /// Tests that ShaderController property is null by default.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void ShaderController_IsNullByDefault()
    {
        // Arrange
        var container = AutoFree(new TestContainerWithComposition())!;

        // Act - Don't add to tree, so CreateComponents isn't called

        // Assert - Property is accessible via subclass but null initially
        AssertThat(container.GetShaderController()).IsNull();
    }

    /// <summary>
    /// Tests that TextRenderer property is null by default.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void TextRenderer_IsNullByDefault()
    {
        // Arrange
        var container = AutoFree(new TestContainerWithComposition())!;

        // Act - Don't add to tree

        // Assert
        AssertThat(container.GetTextRenderer()).IsNull();
    }

    /// <summary>
    /// Tests that ChoicePresenter property is null by default.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void ChoicePresenter_IsNullByDefault()
    {
        // Arrange
        var container = AutoFree(new TestContainerWithComposition())!;

        // Act - Don't add to tree

        // Assert
        AssertThat(container.GetChoicePresenter()).IsNull();
    }
}
