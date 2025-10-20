
// <copyright file="UIActionButton.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>


using Godot;
using OmegaSpiral.Source.Combat.Actions;

namespace OmegaSpiral.Source.Scripts.Combat.UI.ActionMenu;
/// <summary>
/// A button representing a single <see cref="OmegaSpiral.Source.Combat.Actions.BattlerAction"/>, shown in the player's <see cref="UiActionMenu"/>.
/// </summary>
[GlobalClass]
public partial class UiActionButton : TextureButton
{
    private TextureRect? icon;
    private Label? nameLabel;

    private BattlerAction? action;

    /// <summary>
    /// Gets or sets the action associated with this button, which determines the button's icon and label.
    /// </summary>
    public BattlerAction? Action
    {
        get => this.action;
        set
        {
            this.action = value;

            if (!this.IsInsideTree() || this.action is null)
            {
                // In C#, we need to wait for the node to be ready before accessing child nodes
                // We'll call the setup method when the node is ready instead
                this.action = value;
                return;
            }

            if (this.icon != null && this.action is not null)
            {
                this.icon.Texture = this.action.Icon;
            }

            if (this.nameLabel != null && this.action is not null)
            {
                this.nameLabel.Text = this.action.Label;
            }

            // In C# Godot, we can call this directly since it's not async
            var marginContainer = this.GetNode<Control>("MarginContainer");
            this.CustomMinimumSize = marginContainer?.Size ?? Vector2.Zero;
        }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.icon = this.GetNode<TextureRect>("MarginContainer/Items/Icon");
        this.nameLabel = this.GetNode<Label>("MarginContainer/Items/Name");

        // If Action was set before the node was ready, apply it now
        if (this.action is not null)
        {
            if (this.icon != null)
                this.icon.Texture = this.action.Icon;
            if (this.nameLabel != null)
                this.nameLabel.Text = this.action.Label;
            var marginContainer = this.GetNode<Control>("MarginContainer");
            this.CustomMinimumSize = marginContainer?.Size ?? Vector2.Zero;
        }

        this.Pressed += () => this.ReleaseFocus();
    }
}
