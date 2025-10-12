using Godot;
using System.Threading.Tasks;

/// <summary>
/// A button representing a single <see cref="BattlerAction"/>, shown in the player's <see cref="UIActionMenu"/>.
/// </summary>
public partial class UIActionButton : TextureButton
{
    private TextureRect _icon;
    private Label _nameLabel;

    private BattlerAction _action;
    /// <summary>
    /// Setup the button's icon and label to match a given <see cref="BattlerAction"/>.
    /// </summary>
    public BattlerAction Action
    {
        get => _action;
        set
        {
            _action = value;

            if (!IsInsideTree())
            {
                // In C#, we need to wait for the node to be ready before accessing child nodes
                // We'll call the setup method when the node is ready instead
                _action = value;
                return;
            }

            _icon.Texture = _action.Icon;
            _nameLabel.Text = _action.Label;

            // In C# Godot, we can call this directly since it's not async
            CustomMinimumSize = GetNode<"MarginContainer">("MarginContainer").Size;
        }
    }

    public override void _Ready()
    {
        _icon = GetNode<TextureRect>("MarginContainer/Items/Icon");
        _nameLabel = GetNode<Label>("MarginContainer/Items/Name");

        // If Action was set before the node was ready, apply it now
        if (_action != null)
        {
            _icon.Texture = _action.Icon;
            _nameLabel.Text = _action.Label;
            CustomMinimumSize = GetNode<"MarginContainer">("MarginContainer").Size;
        }

        Pressed += () =>
        {
            ReleaseFocus();
        };
    }
}
