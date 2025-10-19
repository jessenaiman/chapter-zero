using Godot;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Entry point scene for the Echo Chamber stage. Currently serves as a thin stub that loads the stage plan.
/// Subsequent work will orchestrate interludes and dungeon traversal from here.
/// </summary>
[GlobalClass]
public partial class EchoHub : Control
{
    private Label? statusLabel;

    /// <inheritdoc/>
    public override void _Ready()
    {
        EchoChamberPlan plan = EchoChamberDirector.GetPlan();

        GD.Print($"[EchoHub] Loaded Echo Chamber plan with {plan.Chambers.Count} chambers.");

        this.statusLabel = this.GetNodeOrNull<Label>("%StatusLabel");
        if (this.statusLabel != null)
        {
            this.statusLabel.Text = $"Echo Chamber Ready ({plan.Chambers.Count} chambers)";
        }
    }
}
