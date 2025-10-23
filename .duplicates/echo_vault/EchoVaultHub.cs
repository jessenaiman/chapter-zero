using Godot;
using System.Linq;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.UI.Terminal;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Handles echo scene selection in the Echo Vault stage.
/// </summary>
[GlobalClass]
public partial class EchoVaultHub : TerminalUI
{
    private Label? tierLabel;
    private RichTextLabel? promptLabel;
    private OptionButton? optionSelector;
    private RichTextLabel? descriptionLabel;
    private RichTextLabel? reactionsLabel;
    private Button? confirmButton;

    private EchoSelectionFeedback lastFeedback;

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Set terminal mode - for Stage 3 we may want full functionality for narrative prompts
        Mode = TerminalMode.Full; // Or TerminalMode.Minimal if we only need basic text

        // Initialize base TerminalBase functionality
        base._Ready();

        EchoVaultSession.Initialize(EchoVaultDirector.GetPlan());

        this.tierLabel = this.GetNodeOrNull<Label>("%TierLabel");
        this.promptLabel = this.GetNodeOrNull<RichTextLabel>("%PromptLabel");
        this.optionSelector = this.GetNodeOrNull<OptionButton>("%EchoOptions");
        this.descriptionLabel = this.GetNodeOrNull<RichTextLabel>("%EchoDescription");
        this.reactionsLabel = this.GetNodeOrNull<RichTextLabel>("%ReactionsLabel");
        this.confirmButton = this.GetNodeOrNull<Button>("%ConfirmButton");

        if (this.optionSelector != null)
        {
            this.optionSelector.ItemSelected += this.OnOptionSelected;
        }

        if (this.confirmButton != null)
        {
            this.confirmButton.Pressed += this.OnConfirmPressed;
        }

        this.RefreshUI();
    }

    /// <inheritdoc/>
    public override void _Notification(int what)
    {
        const int NotificationPredelete = 1;
        if (what == NotificationPredelete)
        {
            tierLabel?.Dispose();
            promptLabel?.Dispose();
            optionSelector?.Dispose();
            descriptionLabel?.Dispose();
            reactionsLabel?.Dispose();
            confirmButton?.Dispose();
        }
        base._Notification(what);
    }

    private void RefreshUI()
    {
        EchoVaultBeat beat = EchoVaultSession.CurrentBeat;
        if (this.tierLabel != null)
        {
            this.tierLabel.Text = $"Presentation Tier {beat.Tier}";
        }

        if (this.promptLabel != null)
        {
            this.promptLabel.Text = beat.Prompt ?? string.Empty;
        }

        if (this.optionSelector != null)
        {
            this.optionSelector.Clear();
            var options = EchoVaultSession.GetCurrentEchoOptions();
            if (options.Count == 0)
            {
                this.optionSelector.AddItem("No echoes available");
                this.optionSelector.Disabled = true;
            }
            else
            {
                this.optionSelector.Disabled = false;
                foreach (EchoVaultEchoDefinition echo in options)
                {
                    this.optionSelector.AddItem(echo.Name);
                }

                this.optionSelector.Selected = 0;
                this.DisplayOptionDetails(options[0]);
            }
        }

        this.lastFeedback = EchoSelectionFeedback.Empty;
    }

    private void OnOptionSelected(long index)
    {
        var options = EchoVaultSession.GetCurrentEchoOptions();
        if (index < 0 || index >= options.Count)
        {
            return;
        }

        this.DisplayOptionDetails(options[(int) index]);
    }

    private void DisplayOptionDetails(EchoVaultEchoDefinition echo)
    {
        if (this.descriptionLabel != null)
        {
            this.descriptionLabel.Text = $"[b]{echo.Name}[/b]\n[font=monospace]{echo.Description ?? string.Empty}\nHP {echo.Mechanics.Hp}  ATK {echo.Mechanics.Attack}\n{echo.Mechanics.Signature}[/font]\n[color=orange]{echo.MemoryCost ?? string.Empty}[/color]";
        }

        if (this.reactionsLabel != null)
        {
            string reactions = string.Join("\n\n", echo.DreamweaverResponses.Select(kvp =>
            {
                string supportive = string.IsNullOrEmpty(kvp.Value.Supportive) ? string.Empty : kvp.Value.Supportive;
                string caution = string.IsNullOrEmpty(kvp.Value.Caution) ? string.Empty : kvp.Value.Caution;
                return $"[color=yellow]{kvp.Key.ToUpperInvariant()}[/color]: {supportive}\n[cadetblue]{caution}[/cadetblue]";
            }));

            this.reactionsLabel.Text = reactions;
        }
    }

    private void OnConfirmPressed()
    {
        var options = EchoVaultSession.GetCurrentEchoOptions();
        if (this.optionSelector == null || this.optionSelector.Selected < 0 || this.optionSelector.Selected >= options.Count)
        {
            return;
        }

        EchoVaultEchoDefinition echo = options[this.optionSelector.Selected];
        if (!EchoVaultSession.TrySelectEcho(echo.Id, out this.lastFeedback))
        {
            GD.PrintErr($"Failed to select echo {echo.Id}");
            return;
        }

        // Continue to next beat or scene based on selection feedback
        GD.Print($"[EchoVaultHub] Selected echo {echo.Name}, awarded to: {lastFeedback.AwardedOwner}");
    }
}
