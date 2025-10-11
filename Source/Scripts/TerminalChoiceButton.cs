using Godot;
using System;
using OmegaSpiral.Source.Scripts;

namespace OmegaSpiral;

/// <summary>
/// Styled terminal choice button with primary label and optional description.
/// Provides hover/focus feedback matching the narrative terminal aesthetic.
/// </summary>
public partial class TerminalChoiceButton : Button
{
    private Label mainLabel = default!;
    private Label descriptionLabel = default!;

    /// <summary>
    /// Gets the underlying choice option represented by this button.
    /// </summary>
    public ChoiceOption? Option { get; private set; }

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.mainLabel = this.GetNode<Label>("%ChoiceLabel");
        this.descriptionLabel = this.GetNode<Label>("%DescriptionLabel");

        this.FocusMode = FocusModeEnum.All;
    }

    /// <summary>
    /// Configure the button content with a choice option.
    /// </summary>
    /// <param name="option">The narrative choice option.</param>
    /// <param name="displayIndex">Zero-based index used for numbering.</param>
    public void Configure(ChoiceOption option, int displayIndex)
    {
        this.Option = option ?? throw new ArgumentNullException(nameof(option));

        string label = option.Text ?? option.Id ?? $"Choice {displayIndex + 1}";
        this.mainLabel.Text = $"[{displayIndex + 1}] {label}";

        if (!string.IsNullOrWhiteSpace(option.Description))
        {
            this.descriptionLabel.Visible = true;
            this.descriptionLabel.Text = option.Description;
        }
        else
        {
            this.descriptionLabel.Visible = false;
            this.descriptionLabel.Text = string.Empty;
        }
    }
}
