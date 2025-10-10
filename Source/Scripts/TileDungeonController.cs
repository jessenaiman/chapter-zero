using Godot;
using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts;

public partial class TileDungeonController : Node2D
{
    private TileMapLayer _tileMapLayer;
    private Node2D _player;
    private Label _infoLabel;
    private TileDungeonData _dungeonData;
    private SceneManager _sceneManager;
    private GameState _gameState;

    private Vector2I _playerTilePosition;

    public override void _Ready()
    {
        _tileMapLayer = GetNode<TileMapLayer>("TileMapLayer");
        _player = GetNode<Node2D>("Player");
        _infoLabel = GetNode<Label>("InfoLabel");
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");
        _gameState = GetNode<GameState>("/root/GameState");

        LoadDungeonData();
        InitializeTileMap();
        SetPlayerStartPosition();
    }

    private void LoadDungeonData()
    {
        try
        {
            string dataPath = "res://Source/Data/scenes/scene4_tile_dungeon/dungeon.json";
            var jsonText = Godot.FileAccess.GetFileAsString(dataPath);
            var jsonNode = Json.ParseString(jsonText).AsGodotDictionary();

            _dungeonData = new TileDungeonData
            {
                Type = jsonNode["type"].ToString(),
                Controls = jsonNode["controls"].ToString(),
                ExitCondition = jsonNode["exitCondition"].ToString(),
                UI = new DungeonUI()
            };

            // Parse tilemap
            foreach (var line in jsonNode["tilemap"].AsGodotArray())
            {
                _dungeonData.Tilemap.Add(line.ToString());
            }

            // Parse legend
            var legendDict = jsonNode["legend"].AsGodotDictionary();
            foreach (var key in legendDict.Keys)
            {
                var entry = legendDict[key].AsGodotDictionary();
                var definition = new TileDefinition
                {
                    Type = Enum.Parse<TileType>(entry["type"].ToString()),
                    Walkable = entry["walkable"].AsBool(),
                    Interactable = entry["interactable"].AsBool(),
                    Description = entry["description"].ToString()
                };
                _dungeonData.Legend[key.ToString()[0]] = definition;
            }

            // Parse UI
            var uiDict = jsonNode["ui"].AsGodotDictionary();
            _dungeonData.UI.ShowInventory = uiDict["showInventory"].AsBool();
            _dungeonData.UI.ShowMap = uiDict["showMap"].AsBool();
            foreach (var stat in uiDict["showStats"].AsGodotArray())
            {
                _dungeonData.UI.ShowStats.Add(stat.ToString());
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to load tile dungeon data: {e.Message}");
            _dungeonData = new TileDungeonData();
        }
    }

    private void InitializeTileMap()
    {
        // This is a placeholder implementation
        // In a full implementation, you would create tiles based on _dungeonData.Tilemap and _dungeonData.Legend
    }

    private void SetPlayerStartPosition()
    {
        // Place player at first walkable tile
        for (int y = 0; y < _dungeonData.Tilemap.Count; y++)
        {
            for (int x = 0; x < _dungeonData.Tilemap[y].Length; x++)
            {
                char symbol = _dungeonData.Tilemap[y][x];
                if (_dungeonData.Legend.TryGetValue(symbol, out var definition) && definition.Walkable)
                {
                    _playerTilePosition = new Vector2I(x, y);
                    UpdatePlayerPosition();
                    return;
                }
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            Vector2I direction = Vector2I.Zero;
            if (keyEvent.Keycode == Key.Up || keyEvent.Keycode == Key.W)
                direction = new Vector2I(0, -1);
            else if (keyEvent.Keycode == Key.Down || keyEvent.Keycode == Key.S)
                direction = new Vector2I(0, 1);
            else if (keyEvent.Keycode == Key.Left || keyEvent.Keycode == Key.A)
                direction = new Vector2I(-1, 0);
            else if (keyEvent.Keycode == Key.Right || keyEvent.Keycode == Key.D)
                direction = new Vector2I(1, 0);

            if (direction != Vector2I.Zero)
            {
                TryMovePlayer(direction);
            }
        }
    }

    private void TryMovePlayer(Vector2I direction)
    {
        var newPosition = _playerTilePosition + direction;
        if (IsWalkable(newPosition))
        {
            _playerTilePosition = newPosition;
            UpdatePlayerPosition();
            CheckTileInteraction();
        }
    }

    private bool IsWalkable(Vector2I position)
    {
        if (position.Y < 0 || position.Y >= _dungeonData.Tilemap.Count) return false;
        if (position.X < 0 || position.X >= _dungeonData.Tilemap[position.Y].Length) return false;

        char symbol = _dungeonData.Tilemap[position.Y][position.X];
        if (_dungeonData.Legend.TryGetValue(symbol, out var definition))
        {
            return definition.Walkable;
        }

        return false;
    }

    private void UpdatePlayerPosition()
    {
        Vector2 tileSize = _tileMapLayer.TileSet.TileSize;
        Vector2 worldPosition = new Vector2(_playerTilePosition.X * tileSize.X, _playerTilePosition.Y * tileSize.Y);
        _player.Position = worldPosition;
    }

    private void CheckTileInteraction()
    {
        char symbol = _dungeonData.Tilemap[_playerTilePosition.Y][_playerTilePosition.X];
        if (_dungeonData.Legend.TryGetValue(symbol, out var definition) && definition.Interactable)
        {
            _infoLabel.Text = definition.Description;

            if (definition.Type == TileType.Exit)
            {
                _sceneManager.TransitionToScene("Scene5PixelCombat");
            }
        }
    }
}
