// <copyright file="OmegaContainer.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;

namespace OmegaSpiral.Source.Ui.Omega;

/// <summary>
/// Base container for UI with component lifecycle management.
/// Pure composition - no visual theming code, just lifecycle orchestration.
/// Subclasses decide which Omega components to use via composition.
/// </summary>
/// <remarks>
/// Architecture: Template Method Pattern
/// - _Ready() defines the initialization sequence
/// - Subclasses override virtual hooks to customize behavior
/// - No visual styling - that's done via composition with OmegaComponentFactory
/// </remarks>
[GlobalClass]
public partial class OmegaContainer : Control
{
    /// <summary>
    /// Emitted when UI has completed initialization.
    /// Subclasses can connect to this signal for dependency setup.
    /// </summary>
    [Signal]
    public delegate void InitializationCompletedEventHandler();

    /// <summary>
    /// Gets the shader controller component if composed.
    /// </summary>
    protected IOmegaShaderController? ShaderController { get; private set; }

    /// <summary>
    /// Gets the text renderer component if composed.
    /// </summary>
    protected IOmegaTextRenderer? TextRenderer { get; private set; }

    /// <summary>
    /// Gets the choice presenter component if composed.
    /// </summary>
    protected IOmegaChoicePresenter? ChoicePresenter { get; private set; }

    /// <summary>
    /// Called by Godot when the node enters the scene tree.
    /// Template method that orchestrates initialization sequence.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        try
        {
            CacheRequiredNodes();
            CreateComponents();
            InitializeComponentStates();
            EmitSignal(SignalName.InitializationCompleted);
        }
        catch (Exception ex)
        {
            GD.PushError($"[{GetType().Name}] Initialization failed: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Caches references to child nodes from the scene.
    /// Override to cache scene-specific nodes.
    /// </summary>
    /// <remarks>
    /// Scene structure is authoritative - nodes should exist in .tscn files.
    /// This method just caches references for performance.
    /// </remarks>
    protected virtual void CacheRequiredNodes()
    {
        // Subclasses override to cache their specific nodes
    }

    /// <summary>
    /// Creates and composes component instances.
    /// Override to create Omega components via OmegaComponentFactory.
    /// </summary>
    /// <remarks>
    /// Use OmegaComponentFactory to create themed components.
    /// Use Compose* helper methods to assign to protected properties.
    /// </remarks>
    protected virtual void CreateComponents()
    {
        // Subclasses override to create their components
    }

    /// <summary>
    /// Initializes component states after creation.
    /// Override to set initial values, configure components, etc.
    /// </summary>
    protected virtual void InitializeComponentStates()
    {
        // Subclasses override to initialize their state
    }

    /// <summary>
    /// Helper to compose a shader controller from a ColorRect.
    /// </summary>
    /// <param name="display">The ColorRect to apply shaders to.</param>
    protected void ComposeShaderController(ColorRect display)
    {
        ShaderController = OmegaComponentFactory.CreateShaderController(display);
    }

    /// <summary>
    /// Helper to compose a text renderer from a RichTextLabel.
    /// </summary>
    /// <param name="textDisplay">The RichTextLabel to render text on.</param>
    protected void ComposeTextRenderer(RichTextLabel textDisplay)
    {
        TextRenderer = OmegaComponentFactory.CreateTextRenderer(textDisplay);
    }

    /// <summary>
    /// Helper to compose a choice presenter from a VBoxContainer.
    /// </summary>
    /// <param name="choiceContainer">The VBoxContainer to hold choice buttons.</param>
    protected void ComposeChoicePresenter(VBoxContainer choiceContainer)
    {
        ChoicePresenter = OmegaComponentFactory.CreateChoicePresenter(choiceContainer);
    }

    /// <summary>
    /// Called when node exits the scene tree.
    /// Cleanup is handled by Godot's reference counting and AutoFree in tests.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        // Godot handles cleanup automatically via reference counting
    }
}
