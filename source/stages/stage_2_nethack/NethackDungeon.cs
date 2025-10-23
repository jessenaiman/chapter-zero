using Godot;
using OmegaSpiral.Source.Stages.Stage2;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Stages.Stage2.Beats;

/// <summary>
/// Minimal dungeon scene stub for the Nethack-inspired sequence. Handles signal emission for integration tests and future UI wiring.
/// </summary>
[GlobalClass]
public partial class NethackDungeon : Control
{
    private Label? ownerLabel;

    [Signal]
    public delegate void StageEnteredEventHandler(string stageId, int stageIndex, int owner, string[] map);

    [Signal]
    public delegate void StageClearedEventHandler(string stageId);

    [Signal]
    public delegate void InteractionResolvedEventHandler(char glyph, int alignedTo, int change);

    [Signal]
    public delegate void AffinityChangedEventHandler(int dwType, int change);

    [Signal]
    public delegate void SequenceCompleteEventHandler(int finalScore);

    /// <inheritdoc/>
    public override void _Ready()
    {
        ownerLabel = GetNodeOrNull<Label>("%OwnerLabel");
    }

    /// <summary>
    /// Binds basic chamber information to the view.
    /// </summary>
    public void Bind(ChamberData chamber)
    {
        if (ownerLabel != null)
        {
            ownerLabel.Text = $"{chamber.Owner.ToUpperInvariant()} Chamber ({chamber.Style.Template})";
        }
    }

    public void EmitStageEntered(string stageId, int stageIndex, DreamweaverType owner, string[] map)
    {
        EmitSignal(SignalName.StageEntered, stageId, stageIndex, (int)owner, map);
    }

    public void EmitStageCleared(string stageId)
    {
        EmitSignal(SignalName.StageCleared, stageId);
    }

    public void EmitInteractionResolved(char glyph, DreamweaverType alignedTo, int change)
    {
        EmitSignal(SignalName.InteractionResolved, glyph, (int)alignedTo, change);
    }

    public void EmitAffinityChanged(DreamweaverType dwType, int change)
    {
        EmitSignal(SignalName.AffinityChanged, (int)dwType, change);
    }

    public void EmitSequenceComplete(int finalScore)
    {
        EmitSignal(SignalName.SequenceComplete, finalScore);
    }
}
