using Godot;
using System;

/// <summary>
/// A text bar that displays the <see cref="BattlerAction.Description"/> of a <see cref="BattlerAction"/>.
///
/// This bar is shown to give the player information about actions as they select one from the
/// <see cref="UIActionMenu"/>.
/// </summary>
public partial class UIActionDescription : MarginContainer
{
    private Label _descriptionLabel;

    private string _description = "";
    /// <summary>
    /// The description text to display
    /// </summary>
    public string Description
    {
        get => _description;
        set
        {
            _description = value;

            if (!IsInsideTree())
            {
                // In C#, we need to wait for the node to be ready before accessing child nodes
                // We'll call the setup method when the node is ready instead
                _description = value;
                return;
            }

            _descriptionLabel.Text = _description;
            if (string.IsNullOrEmpty(_description))
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }

    public override void _Ready()
    {
        _descriptionLabel = GetNode<Label>("CenterContainer/MarginContainer/Description");

        // If Description was set before the node was ready, apply it now
        if (!string.IsNullOrEmpty(_description))
        {
            _descriptionLabel.Text = _description;
            Show();
        }
        else
        {
            Hide();
        }

        if (!Engine.IsEditorHint())
        {
            Hide();
        }
    }
}
