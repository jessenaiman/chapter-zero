// <copyright file="PixelCombatController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Godot;
using YamlDotNet.Serialization;
using OmegaSpiral.Source.Scripts;

public partial class PixelCombatController : Node2D
{
    private Sprite2D? playerSprite;
    private Sprite2D? enemySprite;
    private Label? combatLog;
    private VBoxContainer? actionButtons;
    private CombatSceneData? combatData;
    private SceneManager? sceneManager;
    private GameState? gameState;

    private int playerHP;
    private int enemyHP;
    private bool playerTurn = true;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.playerSprite = this.GetNode<Sprite2D>("PlayerSprite");
        this.enemySprite = this.GetNode<Sprite2D>("EnemySprite");
        this.combatLog = this.GetNode<Label>("CombatLog");
        this.actionButtons = this.GetNode<VBoxContainer>("ActionButtons");
        this.sceneManager = this.GetNode<SceneManager>("/root/SceneManager");
        this.gameState = this.GetNode<GameState>("/root/GameState");

        this.LoadCombatData();
        this.InitializeCombat();
        this.UpdateUI();
    }

    private void LoadCombatData()
    {
        try
        {
            string dataPath = "res://Source/Data/scenes/scene5_ff_combat/combat.yaml";
            var yamlText = Godot.FileAccess.GetFileAsString(dataPath);

            var deserializer = new DeserializerBuilder().Build();
            this.combatData = deserializer.Deserialize<CombatSceneData>(yamlText);

            GD.Print($"Loaded combat data with enemy: {this.combatData.Enemy?.Name}");
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"Failed to load combat data: {ex.Message}");
            this.combatData = new CombatSceneData();
        }
        catch (YamlDotNet.Core.YamlException ex)
        {
            GD.PrintErr($"YAML parsing error for combat data: {ex.Message}");
            this.combatData = new CombatSceneData();
        }
    }

    private void InitializeCombat()
    {
        if (this.combatData == null)
        {
            GD.PrintErr("Combat data not loaded");
            return;
        }

        this.playerHP = 100; // TODO: Calculate from party
        this.enemyHP = this.combatData.Enemy?.HP ?? 50;

        // Load sprites
        if (!string.IsNullOrEmpty(this.combatData.PlayerSprite))
        {
            var playerTexture = GD.Load<Texture2D>(this.combatData.PlayerSprite);
            if (this.playerSprite != null)
            {
                this.playerSprite.Texture = playerTexture;
            }
        }

        if (!string.IsNullOrEmpty(this.combatData.Enemy?.Sprite))
        {
            var enemyTexture = GD.Load<Texture2D>(this.combatData.Enemy.Sprite);
            if (this.enemySprite != null)
            {
                this.enemySprite.Texture = enemyTexture;
            }
        }

        // Create action buttons
        foreach (var action in this.combatData.Actions)
        {
            var button = new Button();
            button.Text = action;
            button.Pressed += () => this.OnActionPressed(action);
            this.actionButtons?.AddChild(button);
        }
    }

    private void OnActionPressed(string action)
    {
        if (!this.playerTurn)
        {
            return;
        }

        switch (action)
        {
            case "FIGHT":
                this.PerformAttack(true);
                break;
            case "MAGIC":
                this.PerformMagic();
                break;
            case "ITEM":
                this.PerformItem();
                break;
            case "RUN":
                this.AttemptRun();
                break;
        }

        this.playerTurn = false;
        this.UpdateUI();

        // Enemy turn after a delay
        this.GetTree().CreateTimer(1.0f).Timeout += this.EnemyTurn;
    }

    private void PerformAttack(bool isPlayer)
    {
        if (this.combatData?.Enemy == null)
        {
            GD.PrintErr("Cannot perform attack: combat data or enemy not available");
            return;
        }

        int damage = isPlayer ? CalculatePlayerDamage() : this.combatData.Enemy.Attack;
        string target = isPlayer ? "Enemy" : "Player";

        if (isPlayer)
        {
            this.enemyHP -= damage;
        }
        else
        {
            this.playerHP -= damage;
        }

        this.combatLog?.Text += $"\n{target} takes {damage} damage!";
    }

    private void PerformMagic()
    {
        // Placeholder magic implementation
        this.PerformAttack(true);
        this.combatLog?.Text += "\nYou cast a spell!";
    }

    private void PerformItem()
    {
        // Placeholder item implementation
        this.playerHP += 20;
        this.combatLog?.Text += "\nYou use a healing item!";
    }

    private void AttemptRun()
    {
        // 50% chance to run
        if (GD.Randf() > 0.5f)
        {
            this.combatLog?.Text += "\nYou successfully ran away!";

            // TODO: Return to previous scene
        }
        else
        {
            this.combatLog?.Text += "\nYou couldn't escape!";
            this.playerTurn = false;
            this.EnemyTurn();
        }
    }

    private static int CalculatePlayerDamage()
    {
        // TODO: Calculate based on party stats
        return 10 + (int)(GD.Randf() * 10);
    }

    private void EnemyTurn()
    {
        if (this.enemyHP <= 0)
        {
            return;
        }

        // Simple AI: always attack
        this.PerformAttack(false);
        this.playerTurn = true;
        this.UpdateUI();

        this.CheckCombatEnd();
    }

    private void UpdateUI()
    {
        if (this.actionButtons == null)
        {
            return;
        }

        // Update HP displays, enable/disable buttons, etc.
        foreach (Button button in this.actionButtons.GetChildren())
        {
            button.Disabled = !this.playerTurn;
        }
    }

    private void CheckCombatEnd()
    {
        if (this.combatLog == null)
        {
            return;
        }

        if (this.playerHP <= 0)
        {
            this.combatLog.Text += "\nYou have been defeated!";

            // TODO: Game over
        }
        else if (this.enemyHP <= 0)
        {
            if (this.combatData != null)
            {
                this.combatLog.Text += $"\n{this.combatData.VictoryText}";
            }

            this.SelectDreamweaver();

            // TODO: Victory sequence
        }
    }

    private void SelectDreamweaver()
    {
        if (this.gameState == null)
        {
            return;
        }

        // Find the Dreamweaver with the highest score
        DreamweaverType selected = DreamweaverType.Light;
        int maxScore = 0;

        foreach (var kvp in this.gameState.DreamweaverScores)
        {
            if (kvp.Value > maxScore)
            {
                maxScore = kvp.Value;
                selected = kvp.Key;
            }
        }

        this.gameState.SelectedDreamweaver = selected;

        // Set thread based on selected Dreamweaver
        this.gameState.DreamweaverThread = selected switch
        {
            DreamweaverType.Light => DreamweaverThread.Hero,
            DreamweaverType.Mischief => DreamweaverThread.Ambition,
            DreamweaverType.Wrath => DreamweaverThread.Shadow,
            _ => DreamweaverThread.Hero
        };
    }
}
