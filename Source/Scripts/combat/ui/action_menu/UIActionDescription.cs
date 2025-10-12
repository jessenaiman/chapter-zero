// <copyright file="UIActionDescription.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Godot;

/// <summary>
/// A text bar that displays the <see cref="BattlerAction.Description"/> of a <see cref="BattlerAction"/>.
///
/// This bar is shown to give the player information about actions as they select one from the
/// <see cref="UIActionMenu"/>.
/// </summary>
public partial class UIActionDescription : MarginContainer
{
    private Label? descriptionLabel;

    private string description = string.Empty;

    /// <summary>
    /// Gets or sets the description text to display.
    /// </summary>
    public string Description
    {
        get => this.description;
        set
        {
            this.description = value;

            if (!this.IsInsideTree())
            {
                // In C#, we need to wait for the node to be ready before accessing child nodes
                // We'll call the setup method when the node is ready instead
                this.description = value;
                return;
            }

            this.descriptionLabel?.Text = this.description;
            if (string.IsNullOrEmpty(this.description))
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.descriptionLabel = this.GetNode<Label>("CenterContainer/MarginContainer/Description");

        // If Description was set before the node was ready, apply it now
        if (!string.IsNullOrEmpty(this.description))
        {
            this.descriptionLabel.Text = this.description;
            this.Show();
        }
        else
        {
            this.Hide();
        }

        if (!Engine.IsEditorHint())
        {
            this.Hide();
        }
    }
}
