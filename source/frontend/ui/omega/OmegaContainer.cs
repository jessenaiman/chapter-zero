// <copyright file="OmegaContainer.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Ui.Omega
{
    using Godot;
    using OmegaSpiral.Source.Backend.Narrative;
    /// <summary>
    /// Base container for UI with component lifecycle management.
    /// Pure composition - no visual theming code, just lifecycle orchestration.
    /// Subclasses decide which Omega components to use via composition.
    /// </summary>
    /// <remarks>
    /// Architecture: Template Method Pattern
    /// - _Ready() defines the initialization sequence
    /// - Subclasses override virtual hooks to customize behavior
    /// - No visual styling - that's done via composition with OmegaComponentFactory.
    /// </remarks>
    [GlobalClass]
    public partial class OmegaContainer : Control
    {
        /// <summary>
        /// Called by Godot when the node enters the scene tree.
        /// Sets up layout anchors to fill parent container.
        /// </summary>
        public override void _Ready()
        {
            base._Ready();
            this.AnchorLeft = 0f;
            this.AnchorTop = 0f;
            this.AnchorRight = 1f;
            this.AnchorBottom = 1f;
            this.OffsetLeft = 0;
            this.OffsetTop = 0;
            this.OffsetRight = 0;
            this.OffsetBottom = 0;
        }
    }
}
