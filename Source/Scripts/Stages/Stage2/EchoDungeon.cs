using Godot;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Minimal dungeon scene stub. Future work will render the map and handle movement.
/// </summary>
[GlobalClass]
public partial class EchoDungeon : Control
{
    private Label? ownerLabel;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.ownerLabel = this.GetNodeOrNull<Label>("%OwnerLabel");
    }

    /// <summary>
    /// Binds the chamber data to the UI.
    /// </summary>
    /// <param name="chamber">The chamber payload.</param>
    public void Bind(EchoChamberChamber chamber)
    {
        if (this.ownerLabel != null)
        {
            this.ownerLabel.Text = $"{chamber.Owner.ToUpperInvariant()} Chamber ({chamber.Style.Template})";
        }
    }
}
