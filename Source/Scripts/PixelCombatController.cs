using Godot;
using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts;

public partial class PixelCombatController : Node2D
{
    private Sprite2D _playerSprite;
    private Sprite2D _enemySprite;
    private Label _combatLog;
    private VBoxContainer _actionButtons;
    private CombatSceneData _combatData;
    private SceneManager _sceneManager;
    private GameState _gameState;

    private int _playerHP;
    private int _enemyHP;
    private bool _playerTurn = true;

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

    private void LoadCombatData()
    {
        try
        {
            string dataPath = "res://Source/Data/scenes/scene5_ff_combat/combat.json";
            var jsonText = Godot.FileAccess.GetFileAsString(dataPath);
            var jsonNode = Json.ParseString(jsonText).AsGodotDictionary();

            _combatData = new CombatSceneData
            {
                Type = jsonNode["type"].ToString(),
                PlayerSprite = jsonNode["playerSprite"].ToString(),
                VictoryText = jsonNode["victoryText"].ToString(),
                Music = jsonNode["music"].ToString()
            };

            // Parse enemy
            var enemyDict = jsonNode["enemy"].AsGodotDictionary();
            _combatData.Enemy = new CombatEnemy
            {
                Name = enemyDict["name"].ToString(),
                HP = enemyDict["hp"].AsInt32(),
                MaxHP = enemyDict["maxHp"].AsInt32(),
                Attack = enemyDict["attack"].AsInt32(),
                Defense = enemyDict["defense"].AsInt32(),
                Sprite = enemyDict["sprite"].ToString()
            };

            // Parse actions
            foreach (var action in jsonNode["actions"].AsGodotArray())
            {
                _combatData.Actions.Add(action.ToString());
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to load combat data: {e.Message}");
            _combatData = new CombatSceneData();
        }
    }

    private void InitializeCombat()
    {
        _playerHP = 100; // TODO: Calculate from party
        _enemyHP = _combatData.Enemy.HP;

        // Load sprites
        if (!string.IsNullOrEmpty(_combatData.PlayerSprite))
        {
            var playerTexture = GD.Load<Texture2D>(_combatData.PlayerSprite);
            _playerSprite.Texture = playerTexture;
        }

        if (!string.IsNullOrEmpty(_combatData.Enemy.Sprite))
        {
            var enemyTexture = GD.Load<Texture2D>(_combatData.Enemy.Sprite);
            _enemySprite.Texture = enemyTexture;
        }

        // Create action buttons
        foreach (var action in _combatData.Actions)
        {
            var button = new Button();
            button.Text = action;
            button.Pressed += () => OnActionPressed(action);
            _actionButtons.AddChild(button);
        }
    }

    private void OnActionPressed(string action)
    {
        if (!_playerTurn) return;

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

    private void PerformAttack(bool isPlayer)
    {
        int damage = isPlayer ? CalculatePlayerDamage() : _combatData.Enemy.Attack;
        string target = isPlayer ? "Enemy" : "Player";

        if (isPlayer)
            _enemyHP -= damage;
        else
            _playerHP -= damage;

        _combatLog.Text += $"\n{target} takes {damage} damage!";
    }

    private void PerformMagic()
    {
        // Placeholder magic implementation
        PerformAttack(true);
        _combatLog.Text += "\nYou cast a spell!";
    }

    private void PerformItem()
    {
        // Placeholder item implementation
        _playerHP += 20;
        _combatLog.Text += "\nYou use a healing item!";
    }

    private void AttemptRun()
    {
        // 50% chance to run
        if (GD.Randf() > 0.5f)
        {
            _combatLog.Text += "\nYou successfully ran away!";
            // TODO: Return to previous scene
        }
        else
        {
            _combatLog.Text += "\nYou couldn't escape!";
            _playerTurn = false;
            EnemyTurn();
        }
    }

    private int CalculatePlayerDamage()
    {
        // TODO: Calculate based on party stats
        return 10 + (int)(GD.Randf() * 10);
    }

    private void EnemyTurn()
    {
        if (_enemyHP <= 0) return;

        // Simple AI: always attack
        PerformAttack(false);
        _playerTurn = true;
        UpdateUI();

        CheckCombatEnd();
    }

    private void UpdateUI()
    {
        // Update HP displays, enable/disable buttons, etc.
        foreach (Button button in _actionButtons.GetChildren())
        {
            button.Disabled = !_playerTurn;
        }
    }

    private void CheckCombatEnd()
    {
        if (_playerHP <= 0)
        {
            _combatLog.Text += "\nYou have been defeated!";
            // TODO: Game over
        }
        else if (_enemyHP <= 0)
        {
            _combatLog.Text += $"\n{_combatData.VictoryText}";
            SelectDreamweaver();
            // TODO: Victory sequence
        }
    }

    private void SelectDreamweaver()
    {
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