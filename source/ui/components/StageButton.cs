using Godot;

namespace OmegaSpiral.Source.Ui.Components;

/// <summary>
/// Individual stage button in the stage select menu.
/// Displays stage name, status indicator, and handles selection.
/// </summary>
[GlobalClass]
public partial class StageButton : Button
{
    /// <summary>
    /// Configures the stage button with id, title, and status.
    /// </summary>
    /// <param name="newStageId">The stage id to assign.</param>
    /// <param name="title">The display title for the stage.</param>
    /// <param name="newStatus">The content status for the stage.</param>
    public void Configure(string newStageId, string title, ContentStatus newStatus)
    {
        StageId = newStageId;
        Status = newStatus;
        Text = title;
        if (GetNodeOrNull<Label>("HBox/NameLabel") is { } nameLabel)
        {
            nameLabel.Text = title;
        }
    }
    /// <summary>
    /// Content status enumeration.
    /// </summary>
    public enum ContentStatus
    {
        /// <summary>Stage is ready for play (static or LLM content loaded).</summary>
        Ready,

        /// <summary>Stage has LLM-generated content.</summary>
        LlmGenerated,

        /// <summary>Stage is missing content.</summary>
        Missing,
    }

    /// <summary>
    /// Signal emitted when this stage button is clicked.
    /// </summary>
    [Signal]
    public delegate void ClickedStageEventHandler(string stageId);

    private string _StageId = string.Empty;
    private ContentStatus _Status = ContentStatus.Missing;

    /// <summary>
    /// Gets or sets the stage ID (e.g., "Stage1", "Stage2").
    /// </summary>
    [Export]
    public string StageId
    {
        get => _StageId;
        set => _StageId = value;
    }

    /// <summary>
    /// Gets or sets the content status for this stage.
    /// </summary>
    [Export]
    public ContentStatus Status
    {
        get => _Status;
        set
        {
            _Status = value;
            UpdateDisplay();
        }
    }

    /// <summary>
    /// Called when the node enters the scene tree.
    /// </summary>
    public override void _Ready()
    {
        Pressed += OnPressed;
    }

    /// <summary>
    /// Updates the visual display based on the content status.
    /// </summary>
    private void UpdateDisplay()
    {
        var iconLabel = GetNodeOrNull<Label>("HBox/IconLabel");
        var statusLabel = GetNodeOrNull<Label>("HBox/StatusLabel");

        if (iconLabel == null || statusLabel == null)
        {
            GD.PrintErr("StageButton: Missing HBox/IconLabel or HBox/StatusLabel nodes");
            return;
        }

        // Update icon and status text based on content status
        switch (_Status)
        {
            case ContentStatus.Ready:
                iconLabel.Text = "✓";
                statusLabel.Text = "[Ready]";
                iconLabel.AddThemeColorOverride("font_color", Colors.LimeGreen);
                statusLabel.AddThemeColorOverride("font_color", Colors.LimeGreen);
                Disabled = false;
                break;

            case ContentStatus.LlmGenerated:
                iconLabel.Text = "⚡";
                statusLabel.Text = "[LLM Gen]";
                iconLabel.AddThemeColorOverride("font_color", new Color(1.0f, 0.776f, 0.0f)); // Gold
                statusLabel.AddThemeColorOverride("font_color", new Color(1.0f, 0.776f, 0.0f));
                Disabled = false;
                break;

            case ContentStatus.Missing:
                iconLabel.Text = "○";
                statusLabel.Text = "[Generate]";
                iconLabel.AddThemeColorOverride("font_color", Colors.Gray);
                statusLabel.AddThemeColorOverride("font_color", Colors.Gray);
                Disabled = true;
                break;
        }
    }

    /// <summary>
    /// Handler for button pressed event.
    /// </summary>
    private void OnPressed()
    {
        GD.Print($"StageButton: {StageId} pressed");
        EmitSignal(SignalName.ClickedStage, StageId);
    }
}
