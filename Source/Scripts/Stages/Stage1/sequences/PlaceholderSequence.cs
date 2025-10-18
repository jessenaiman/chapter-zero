// <copyright file="PlaceholderSequence.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Godot;

namespace OmegaSpiral.Source.Scripts.Field.Narrative.Sequences;
/// <summary>
/// Placeholder sequence implementation demonstrating the NarrativeSequence pattern.
/// This sequence serves as a template for implementing Phase 2+ sequences (Opening, ThreadBranch, etc.).
///
/// To implement a new sequence:
/// 1. Inherit from NarrativeSequence
/// 2. Override OnSequenceReady() to initialize sequence-specific state
/// 3. Implement PlayAsync() with your sequence logic
/// 4. Call CompleteSequence(nextId) or OnInput(inputId) to progress the narrative
/// 5. Create a corresponding .tscn file in Source/Scenes/GhostTerminal/
/// 6. Register the sequence with GhostTerminalDirector
/// </summary>
[GlobalClass]
public partial class PlaceholderSequence : NarrativeSequence
{
    private Label? placeholderLabel;

    /// <inheritdoc/>
    protected override void OnSequenceReady()
    {
        base.OnSequenceReady();
        this.placeholderLabel = this.GetNode<Label>("PlaceholderLabel");
    }

    /// <inheritdoc/>
    public override async Task PlayAsync()
    {
        if (this.placeholderLabel != null)
        {
            this.placeholderLabel.Text = $"Placeholder Sequence: {this.SequenceId}";
        }

        // Simulate some work
        await Task.Delay(2000).ConfigureAwait(false);

        // Signal completion (empty string = no specific next sequence)
        this.CompleteSequence();
    }
}
