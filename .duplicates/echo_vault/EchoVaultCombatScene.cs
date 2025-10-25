using System.Globalization;
using Godot;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Ui.Terminal;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Scripts.Stages.Stage2;

/// <summary>
/// Simple auto-resolve combat presentation for Echo Vault.
/// </summary>
[GlobalClass]
public partial class EchoVaultCombatScene : OmegaUi
{
    private RichTextLabel? logLabel;
    private Button? resolveButton;
    private Button? defeatButton;

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Set terminal mode for combat log display
        Mode = TerminalMode.Full; // For full text effects on combat logs

        // Initialize base TerminalBase functionality
        base._Ready();

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
        EchoCombatResult result = EchoVaultCombatScene.SimulateCombat();
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

    private static EchoCombatResult SimulateCombat()
    {
        var combat = EchoVaultSession.GetCurrentCombat();
        var partyStats = CalculatePartyStats();
        var enemyStats = CalculateEnemyStats(combat);

        var log = SimulateCombatRounds(partyStats, enemyStats, combat);
        bool victory = enemyStats.hp <= 0;

        return new EchoCombatResult(log.ToString(), victory);
    }

    private static (double hp, List<(string Id, int Attack, int Hp)> echoes) CalculatePartyStats()
    {
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
        return (partyHp, echoes);
    }

    private static (double hp, List<EchoVaultEnemyEntry> enemies) CalculateEnemyStats(EchoVaultCombat combat)
    {
        double enemyHp = 0;
        foreach (var enemy in combat.EnemyList)
        {
            enemyHp += (enemy.Level * 5) + 10;
        }
        return (enemyHp, combat.EnemyList);
    }

    private static System.Text.StringBuilder SimulateCombatRounds(
        (double hp, List<(string Id, int Attack, int Hp)> echoes) party,
        (double hp, List<EchoVaultEnemyEntry> enemies) enemy,
        EchoVaultCombat combat)
    {
        double partyHp = party.hp;
        var echoes = party.echoes;
        double enemyHp = enemy.hp;
        var enemies = enemy.enemies;

        int round = 1;
        System.Text.StringBuilder sb = new();
        sb.AppendLine("[b]AUTO-RESOLVE[/b]");
        while (partyHp > 0 && enemyHp > 0 && round <= 6)
        {
            double roundDamage = echoes.Sum(e => e.Attack);
            enemyHp -= roundDamage;
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Round {0}: echoes deal {1:0} damage.", round, roundDamage));

            if (enemyHp <= 0)
            {
                sb.AppendLine("Enemies fragment into static.");
                break;
            }

            double enemyDamage = enemies.Sum(e => (e.Level * 2) + 3);
            partyHp -= enemyDamage;
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Round {0}: archive claws back {1:0} damage.", round, enemyDamage));
            round++;
        }

        if (enemyHp <= 0)
        {
            sb.AppendLine("Victory! The archive weakens.");
        }
        else
        {
            sb.AppendLine("The echoes fall. But the loop remembers.");
        }

        return sb;
    }
}

public readonly record struct EchoCombatResult(string Log, bool Victory);
