
// <copyright file="PixelCombatController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.Field;
/// <summary>
/// Controls the turn-based pixel combat scene, managing player and enemy actions, UI updates, and combat flow.
/// </summary>
/// <remarks>
/// Handles loading combat data, initializing sprites and UI, processing player and enemy turns, and determining combat outcomes.
/// </remarks>
[GlobalClass]
public partial class PixelCombatController : Node2D
{
    private Sprite2D? _playerSprite;
    private Sprite2D? _enemySprite;
    private Label? _combatLog;
    private VBoxContainer? _actionButtons;
    private CombatSceneData? _combatData;
    private SceneManager? _sceneManager;
    private GameState? _gameState;

    private int _playerHP;
    private int _enemyHP;
    private bool _playerTurn = true;

    /// <summary>
    /// Called when the node is added to the scene. Initializes references, loads combat data, and sets up the UI.
    /// </summary>
    public override void _Ready()
    {
        _playerSprite = GetNode<Sprite2D>("PlayerSprite");
        _enemySprite = GetNode<Sprite2D>("EnemySprite");
        _combatLog = GetNode<Label>("CombatLog");
        _actionButtons = GetNode<VBoxContainer>("ActionButtons");
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");
        _gameState = GetNode<GameState>("/root/GameState");

        LoadCombatData();
        InitializeCombat();
        UpdateUI();
    }

    /// <summary>
    /// Loads combat data from the configuration file and maps it to <see cref="CombatSceneData"/>.
    /// </summary>
    private void LoadCombatData()
    {
        try
        {
            string dataPath = "res://Source/Data/stages/combat-dialog/combat.json";
            var configData = ConfigurationService.LoadConfiguration(dataPath);

            // Map the dictionary to CombatSceneData
            if (configData != null)
            {
                _combatData = new CombatSceneData();

                if (configData.TryGetValue("type", out var typeVar))
                {
                    _combatData.Type = typeVar.ToString() ?? "pixel_combat";
                }

                if (configData.TryGetValue("playerSprite", out var playerSpriteVar))
                {
                    _combatData.PlayerSprite = playerSpriteVar.ToString();
                }

                // Map other fields as needed
            }

            GD.Print($"Loaded combat data with enemy: {_combatData?.Enemy?.Name}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to load combat data: {ex.Message}");
            _combatData = new CombatSceneData();
        }
    }

    /// <summary>
    /// Initializes combat state, loads sprites, and creates action buttons.
    /// </summary>
    private void InitializeCombat()
    {
        if (_combatData == null)
        {
            GD.PrintErr("Combat data not loaded");
            return;
        }

        _playerHP = 100; // TODO: Calculate from party
        _enemyHP = _combatData.Enemy?.HP ?? 50;

        // Load sprites
        if (!string.IsNullOrEmpty(_combatData.PlayerSprite))
        {
            var playerTexture = GD.Load<Texture2D>(_combatData.PlayerSprite);
            if (_playerSprite != null)
            {
                _playerSprite.Texture = playerTexture;
            }
        }

        if (!string.IsNullOrEmpty(_combatData.Enemy?.Sprite))
        {
            var enemyTexture = GD.Load<Texture2D>(_combatData.Enemy.Sprite);
            if (_enemySprite != null)
            {
                _enemySprite.Texture = enemyTexture;
            }
        }

        // Create action buttons
        if (_combatData.Actions != null)
        {
            foreach (var action in _combatData.Actions)
            {
                var button = new Button();
                button.Text = action;
                button.Pressed += () => OnActionPressed(action);
                _actionButtons?.AddChild(button);
            }
        }
    }

    /// <summary>
    /// Handles player action button presses and dispatches the selected action.
    /// </summary>
    /// <param name="action">The action selected by the player.</param>
    private void OnActionPressed(string action)
    {
        if (!_playerTurn)
        {
            return;
        }

        switch (action)
        {
            case "FIGHT":
                PerformAttack(true);
                break;
            case "MAGIC":
                PerformMagic();
                break;
            case "ITEM":
                PerformItem();
                break;
            case "RUN":
                AttemptRun();
                break;
        }

        _playerTurn = false;
        UpdateUI();

        // Enemy turn after a delay
        GetTree().CreateTimer(1.0f).Timeout += EnemyTurn;
    }

    /// <summary>
    /// Performs an attack action for player or enemy.
    /// </summary>
    /// <param name="isPlayer">If <see langword="true"/>, player attacks; otherwise, enemy attacks.</param>
    private void PerformAttack(bool isPlayer)
    {
        if (_combatData?.Enemy == null)
        {
            GD.PrintErr("Cannot perform attack: combat data or enemy not available");
            return;
        }

        int damage = isPlayer ? CalculatePlayerDamage() : _combatData.Enemy.Attack;
        string target = isPlayer ? "Enemy" : "Player";

        if (isPlayer)
        {
            _enemyHP -= damage;
        }
        else
        {
            _playerHP -= damage;
        }

        if (_combatLog != null)
        {
            _combatLog.Text += $"\n{target} takes {damage} damage!";
        }
    }

    /// <summary>
    /// Performs a magic action. Placeholder implementation.
    /// </summary>
    private void PerformMagic()
    {
        // Placeholder magic implementation
        PerformAttack(true);
        if (_combatLog != null)
        {
            _combatLog.Text += "\nYou cast a spell!";
        }
    }

    /// <summary>
    /// Performs an item use action. Placeholder implementation.
    /// </summary>
    private void PerformItem()
    {
        // Placeholder item implementation
        _playerHP += 20;
        if (_combatLog != null)
        {
            _combatLog.Text += "\nYou use a healing item!";
        }
    }

    /// <summary>
    /// Attempts to run from combat. 50% chance to escape.
    /// </summary>
    private void AttemptRun()
    {
        // 50% chance to run
        if (GD.Randf() > 0.5f)
        {
            if (_combatLog != null)
            {
                _combatLog.Text += "\nYou successfully ran away!";
            }
            // TODO: Return to previous scene
        }
        else
        {
            if (_combatLog != null)
            {
                _combatLog.Text += "\nYou couldn't escape!";
            }
            _playerTurn = false;
            EnemyTurn();
        }
    }

    /// <summary>
    /// Calculates the player's attack damage. Placeholder implementation.
    /// </summary>
    /// <returns>The calculated damage value.</returns>
    private static int CalculatePlayerDamage()
    {
        // TODO: Calculate based on party stats
        return 10 + (int) (GD.Randf() * 10);
    }

    /// <summary>
    /// Handles the enemy's turn, performs attack, and updates UI.
    /// </summary>
    private void EnemyTurn()
    {
        if (_enemyHP <= 0)
        {
            return;
        }

        // Simple AI: always attack
        PerformAttack(false);
        _playerTurn = true;
        UpdateUI();

        CheckCombatEnd();
    }

    /// <summary>
    /// Updates the UI elements for combat, including enabling/disabling action buttons.
    /// </summary>
    private void UpdateUI()
    {
        if (_actionButtons == null)
        {
            return;
        }

        // Update HP displays, enable/disable buttons, etc.
        foreach (Button button in _actionButtons.GetChildren())
        {
            button.Disabled = !_playerTurn;
        }
    }

    /// <summary>
    /// Checks if combat has ended and handles victory or defeat logic.
    /// </summary>
    private void CheckCombatEnd()
    {
        if (_combatLog == null)
        {
            return;
        }

        if (_playerHP <= 0)
        {
            _combatLog.Text += "\nYou have been defeated!";
            // TODO: Game over
        }
        else if (_enemyHP <= 0)
        {
            if (_combatData != null)
            {
                _combatLog.Text += $"\n{_combatData.VictoryText}";
            }

            SelectDreamweaver();
            // TODO: Victory sequence
        }
    }

    /// <summary>
    /// Selects the Dreamweaver with the highest score and sets the thread in <see cref="GameState"/>.
    /// </summary>
    private void SelectDreamweaver()
    {
        if (_gameState == null)
        {
            return;
        }

        // Find the Dreamweaver with the highest score
        DreamweaverType selected = DreamweaverType.Light;
        int maxScore = 0;

        foreach (var kvp in _gameState.DreamweaverScores)
        {
            if (kvp.Value > maxScore)
            {
                maxScore = kvp.Value;
                selected = kvp.Key;
            }
        }

        _gameState.SelectedDreamweaver = selected;

        // Set thread based on selected Dreamweaver
        _gameState.DreamweaverThread = selected switch
        {
            DreamweaverType.Light => DreamweaverThread.Hero,
            DreamweaverType.Mischief => DreamweaverThread.Ambition,
            DreamweaverType.Wrath => DreamweaverThread.Shadow,
            _ => DreamweaverThread.Hero
        };
    }
}
