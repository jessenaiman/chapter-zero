using Godot;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Placeholder controller for interlude presentation. Will later handle choice rendering and banter playback.
/// </summary>
[GlobalClass]
public partial class EchoInterlude : Control
{
    private Label? promptLabel;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.promptLabel = this.GetNodeOrNull<Label>("%PromptLabel");
    }

    /// <summary>
    /// Binds interlude data to the view. Currently sets prompt text only.
    /// </summary>
    /// <param name="interlude">The interlude configuration.</param>
    public void Bind(EchoChamberInterlude interlude)
    {
        if (this.promptLabel != null)
        {
            this.promptLabel.Text = interlude.Prompt;
        }
    }
}
