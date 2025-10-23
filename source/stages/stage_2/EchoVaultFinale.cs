using System.Globalization;
using Godot;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.UI.Terminal;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Displays finale summary and persists party state.
/// </summary>
[GlobalClass]
public partial class EchoVaultFinale : TerminalUI
{
    private RichTextLabel? summaryLabel;
    private Button? continueButton;
    private bool persisted;

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Set terminal mode appropriately for finale display
        Mode = TerminalMode.Full; // For full text effects on summary

        // Initialize base TerminalBase functionality
        base._Ready();

        this.summaryLabel = this.GetNodeOrNull<RichTextLabel>("%SummaryLabel");
        this.continueButton = this.GetNodeOrNull<Button>("%ContinueButton");

        if (this.continueButton != null)
        {
            this.continueButton.Pressed += this.OnContinuePressed;
        }

        this.RenderSummary();
        this.PersistParty();
    }

    private void RenderSummary()
    {
        var finale = EchoVaultSession.GetFinaleData();
        if (this.summaryLabel == null)
        {
            return;
        }

        System.Text.StringBuilder sb = new();
        sb.AppendLine("[center][b]Echo Vault // Extraction Complete[/b][/center]\n");

        foreach (var kv in finale.Summary)
        {
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "[color=yellow]{0}[/color]", kv.Key.ToUpperInvariant()));
            foreach (string line in kv.Value)
            {
                sb.AppendLine(line);
            }
            sb.AppendLine();
        }

        sb.AppendLine("[b]Points Ledger[/b]");
        foreach (var kv in finale.Points)
        {
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", kv.Key, kv.Value));
        }
        sb.AppendLine();

        sb.AppendLine("[b]Memory Loss[/b]");
        foreach (string loss in finale.MemoryLossEntries)
        {
            sb.AppendLine(loss);
        }

        this.summaryLabel.Text = sb.ToString();
    }

    private void PersistParty()
    {
        if (this.persisted)
        {
            return;
        }

        GameState? gameState = this.GetNodeOrNull<GameState>("/root/GameState");
        if (gameState == null)
        {
            GD.PrintErr("GameState not found; unable to persist Echo Vault party.");
            return;
        }

        foreach (string echoId in EchoVaultSession.SelectedEchoIds)
        {
            var definition = EchoVaultSession.Plan.Echoes.FirstOrDefault(e => e.Id == echoId);
            if (definition == null)
            {
                continue;
            }

            var character = new Character
            {
                Name = definition.Name,
                Class = CharacterClass.Fighter,
                Race = CharacterRace.Human,
                Level = 3
            };

            character.Stats.Strength = definition.Mechanics.Attack;
            character.Stats.Constitution = definition.Mechanics.Hp / 2;

            gameState.PlayerParty.AddMember(character);
        }

        this.persisted = true;
    }

    private void OnContinuePressed()
    {
        EchoVaultSession.Reset();
        SceneManager? sceneManager = this.GetNodeOrNull<SceneManager>("/root/SceneManager");
        if (sceneManager != null)
        {
            sceneManager.UpdateCurrentScene(4);
            sceneManager.TransitionToScene("Stage4TileDungeon", showLoadingScreen: false);
        }
        else
        {
            this.GetTree().ChangeSceneToFile("res://source/stages/stage_1/opening.tscn");
        }
    }
}
