// <copyright file="UIActionButton.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Godot;

/// <summary>
/// A button representing a single <see cref="BattlerAction"/>, shown in the player's <see cref="UIActionMenu"/>.
/// </summary>
public partial class UIActionButton : TextureButton
{
    private TextureRect icon;
    private Label nameLabel;

    private BattlerAction action;

    /// <summary>
    /// Gets or sets setup the button's icon and label to match a given <see cref="BattlerAction"/>.
    /// </summary>
    public BattlerAction Action
    {
        get => this.action;
        set
        {
            this.action = value;

            if (!this.IsInsideTree())
            {
                // In C#, we need to wait for the node to be ready before accessing child nodes
                // We'll call the setup method when the node is ready instead
                this.action = value;
                return;
            }

            this.icon.Texture = this.action.Icon;
            this.nameLabel.Text = this.action.Label;

            // In C# Godot, we can call this directly since it's not async
            this.CustomMinimumSize = GetNode < "MarginContainer" > "MarginContainer".Size;
        }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.icon = this.GetNode<TextureRect>("MarginContainer/Items/Icon");
        this.nameLabel = this.GetNode<Label>("MarginContainer/Items/Name");

        // If Action was set before the node was ready, apply it now
        if (this.action != null)
        {
            this.icon.Texture = this.action.Icon;
            this.nameLabel.Text = this.action.Label;
            this.CustomMinimumSize = GetNode < "MarginContainer" > "MarginContainer".Size;
        }

        this.Pressed += () =>
        {
            this.ReleaseFocus();
        };
    }
}
