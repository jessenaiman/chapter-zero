
// <copyright file="UIActionButton.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>


using Godot;
using OmegaSpiral.Source.Combat.Actions;

namespace OmegaSpiral.Source.Scripts.Combat.Ui.ActionMenu;
/// <summary>
/// A button representing a single <see cref="OmegaSpiral.Source.Combat.Actions.BattlerAction"/>, shown in the player's <see cref="UiActionMenu"/>.
/// </summary>
[GlobalClass]
public partial class UIActionButton : TextureButton
{
    private TextureRect? _Icon;
    private Label? _NameLabel;

    private BattlerAction? _Action;

    /// <summary>
    /// Gets or sets the action associated with this button, which determines the button's icon and label.
    /// </summary>
    public BattlerAction? Action
    {
        get => this._Action;
        set
        {
            this._Action = value;

            if (!this.IsInsideTree() || this._Action is null)
            {
                // In C#, we need to wait for the node to be ready before accessing child nodes
                // We'll call the setup method when the node is ready instead
                this._Action = value;
                return;
            }

            if (this._Icon != null && this._Action is not null)
            {
                this._Icon.Texture = this._Action.Icon;
            }

            if (this._NameLabel != null && this._Action is not null)
            {
                this._NameLabel.Text = this._Action.Label;
            }

            // In C# Godot, we can call this directly since it's not async
            var marginContainer = this.GetNode<Control>("MarginContainer");
            this.CustomMinimumSize = marginContainer?.Size ?? Vector2.Zero;
        }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this._Icon = this.GetNode<TextureRect>("MarginContainer/Items/Icon");
        this._NameLabel = this.GetNode<Label>("MarginContainer/Items/Name");

        // If Action was set before the node was ready, apply it now
        if (this._Action is not null)
        {
            if (this._Icon != null)
                this._Icon.Texture = this._Action.Icon;
            if (this._NameLabel != null)
                this._NameLabel.Text = this._Action.Label;
            var marginContainer = this.GetNode<Control>("MarginContainer");
            this.CustomMinimumSize = marginContainer?.Size ?? Vector2.Zero;
        }

        this.Pressed += () => this.ReleaseFocus();
    }
}
