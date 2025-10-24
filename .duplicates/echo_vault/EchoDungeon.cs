using Godot;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Minimal dungeon scene stub. Future work will render the map and handle movement.
/// </summary>
[GlobalClass]
public partial class EchoDungeon : Control
{
    private Label? ownerLabel;

    /// <summary>
    /// Emitted when entering a new dungeon stage.
    /// </summary>
    /// <param name="stageId">The identifier of the entered stage.</param>
    /// <param name="stageIndex">The index of the entered stage in the sequence.</param>
    /// <param name="owner">The Dreamweaver type that owns this stage.</param>
    /// <param name="map">The ASCII map for this stage.</param>
    [Signal]
    public delegate void StageEnteredEventHandler(string stageId, int stageIndex, int owner, string[] map);

    /// <summary>
    /// Emitted when a stage is cleared/completed.
    /// </summary>
    /// <param name="stageId">The identifier of the cleared stage.</param>
    [Signal]
    public delegate void StageClearedEventHandler(string stageId);

    /// <summary>
    /// Emitted when a glyph interaction is resolved.
    /// </summary>
    /// <param name="glyph">The glyph that was interacted with.</param>
    /// <param name="alignedTo">The Dreamweaver type the interaction was aligned to.</param>
    /// <param name="change">The affinity change value.</param>
    [Signal]
    public delegate void InteractionResolvedEventHandler(char glyph, int alignedTo, int change);

    /// <summary>
    /// Emitted when Dreamweaver affinity is changed.
    /// </summary>
    /// <param name="dwType">The Dreamweaver type whose affinity changed.</param>
    /// <param name="change">The change value.</param>
    [Signal]
    public delegate void AffinityChangedEventHandler(int dwType, int change);

    /// <summary>
    /// Emitted when the entire dungeon sequence is completed.
    /// </summary>
    /// <param name="finalScore">The final score of the sequence.</param>
    [Signal]
    public delegate void SequenceCompleteEventHandler(int finalScore);

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.ownerLabel = this.GetNodeOrNull<Label>("%OwnerLabel");
    }

    /// <summary>
    /// Binds the chamber data to the Ui.
    /// </summary>
    /// <param name="chamber">The chamber payload.</param>
    public void Bind(EchoChamberChamber chamber)
    {
        if (this.ownerLabel != null)
        {
            this.ownerLabel.Text = $"{chamber.Owner.ToUpperInvariant()} Chamber ({chamber.Style.Template})";
        }
    }

    /// <summary>
    /// Emits the StageEntered signal.
    /// </summary>
    /// <param name="stageId">The identifier of the entered stage.</param>
    /// <param name="stageIndex">The index of the entered stage in the sequence.</param>
    /// <param name="owner">The Dreamweaver type that owns this stage.</param>
    /// <param name="map">The ASCII map for this stage.</param>
    public void EmitStageEntered(string stageId, int stageIndex, DreamweaverType owner, string[] map)
    {
        this.EmitSignal(SignalName.StageEntered, stageId, stageIndex, (int)owner, map);
    }

    /// <summary>
    /// Emits the StageCleared signal.
    /// </summary>
    /// <param name="stageId">The identifier of the cleared stage.</param>
    public void EmitStageCleared(string stageId)
    {
        this.EmitSignal(SignalName.StageCleared, stageId);
    }

    /// <summary>
    /// Emits the InteractionResolved signal.
    /// </summary>
    /// <param name="glyph">The glyph that was interacted with.</param>
    /// <param name="alignedTo">The Dreamweaver type the interaction was aligned to.</param>
    /// <param name="change">The affinity change value.</param>
    public void EmitInteractionResolved(char glyph, DreamweaverType alignedTo, int change)
    {
        this.EmitSignal(SignalName.InteractionResolved, glyph, (int)alignedTo, change);
    }

    /// <summary>
    /// Emits the AffinityChanged signal.
    /// </summary>
    /// <param name="dwType">The Dreamweaver type whose affinity changed.</param>
    /// <param name="change">The change value.</param>
    public void EmitAffinityChanged(DreamweaverType dwType, int change)
    {
        this.EmitSignal(SignalName.AffinityChanged, (int)dwType, change);
    }

    /// <summary>
    /// Emits the SequenceComplete signal.
    /// </summary>
    /// <param name="finalScore">The final score of the sequence.</param>
    public void EmitSequenceComplete(int finalScore)
    {
        this.EmitSignal(SignalName.SequenceComplete, finalScore);
    }
}
