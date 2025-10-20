using System.Globalization;
using Godot;

namespace OmegaSpiral.Source.Scripts.Stages.Stage3;

/// <summary>
/// Simple auto-resolve combat presentation for Echo Vault.
/// </summary>
[GlobalClass]
public partial class EchoVaultCombatScene : Control
{
    private RichTextLabel? logLabel;
    private Button? resolveButton;
    private Button? defeatButton;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.logLabel = this.GetNodeOrNull<RichTextLabel>("%EncounterLog");
        this.resolveButton = this.GetNodeOrNull<Button>("%ResolveButton");
        this.defeatButton = this.GetNodeOrNull<Button>("%DefeatButton");

        if (this.resolveButton != null)
        {
            this.resolveButton.Pressed += this.OnResolvePressed;
        }

        if (this.defeatButton != null)
        {
            this.defeatButton.Pressed += this.OnDefeatPressed;
        }

        this.DisplayIntro();
    }

    private void DisplayIntro()
    {
        var beat = EchoVaultSession.Plan.Beats[EchoVaultSession.BeatIndex];
        GD.Print($"EchoVaultCombat: {beat.EncounterId}");
        if (this.logLabel != null)
        {
            this.logLabel.Text = beat.EncounterIntro ?? string.Empty;
        }
    }

    private void OnResolvePressed()
    {
        EchoCombatResult result = this.SimulateCombat();
        EchoVaultSession.ResolveCombat(result.Victory);
        this.ShowLogAndContinue(result.Log, result.Victory);
    }

    private void OnDefeatPressed()
    {
        EchoVaultSession.ResolveCombat(false);
        this.ShowLogAndContinue("The echoes retreat to the archive, scarred.", false);
    }

    private void ShowLogAndContinue(string text, bool victory)
    {
        if (this.logLabel != null)
        {
            this.logLabel.Text = text;
        }

        this.resolveButton?.SetDeferred(BaseButton.PropertyName.Disabled, true);
        this.defeatButton?.SetDeferred(BaseButton.PropertyName.Disabled, true);

        var timer = GetTree().CreateTimer(1.0f);
        timer.Timeout += () =>
        {
            if (EchoVaultSession.IsFinaleBeat())
            {
                GetTree().ChangeSceneToFile("res://source/stages/stage_3/echo_vault_finale.tscn");
            }
            else
            {
                GetTree().ChangeSceneToFile("res://source/stages/stage_3/echo_vault_hub.tscn");
            }
        };
    }

    private EchoCombatResult SimulateCombat()
    {
        var combat = EchoVaultSession.GetCurrentCombat();
        double partyHp = 0;
        var echoes = new List<(string Id, int Attack, int Hp)>();
        foreach (string echoId in EchoVaultSession.SelectedEchoIds)
        {
            var definition = EchoVaultSession.Plan.Echoes.Find(e => e.Id == echoId);
            if (definition != null)
            {
                echoes.Add((definition.Id, definition.Mechanics.Attack, definition.Mechanics.Hp));
                partyHp += definition.Mechanics.Hp;
            }
        }

        double enemyHp = 0;
        foreach (var enemy in combat.EnemyList)
        {
            enemyHp += (enemy.Level * 5) + 10;
        }

        int round = 1;
        System.Text.StringBuilder sb = new();
        sb.AppendLine("[b]AUTO-RESOLVE[/b]");
        while (partyHp > 0 && enemyHp > 0 && round <= 6)
        {
            double roundDamage = 0;
            foreach (var echo in echoes)
            {
                roundDamage += echo.Attack;
            }

            enemyHp -= roundDamage;
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Round {0}: echoes deal {1:0} damage.", round, roundDamage));

            if (enemyHp <= 0)
            {
                sb.AppendLine("Enemies fragment into static.");
                break;
            }

            double enemyDamage = 0;
            foreach (var enemy in combat.EnemyList)
            {
                enemyDamage += (enemy.Level * 2) + 3;
            }

            partyHp -= enemyDamage;
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Round {0}: archive claws back {1:0} damage.", round, enemyDamage));
            round++;
        }

        bool victory = enemyHp <= 0;
        if (!victory)
        {
            sb.AppendLine("The echoes fall. But the loop remembers.");
        }
        else
        {
            sb.AppendLine("Victory! The archive weakens.");
        }

        return new EchoCombatResult(sb.ToString(), victory);
    }
}

public readonly record struct EchoCombatResult(string Log, bool Victory);
