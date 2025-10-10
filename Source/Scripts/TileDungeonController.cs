// <copyright file="TileDungeonController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Scripts;

public partial class TileDungeonController : Node2D
{
    private TileMapLayer? tileMapLayer;
    private Node2D? player;
    private Label? infoLabel;
    private TileDungeonData? dungeonData;
    private SceneManager? sceneManager;
    private GameState? gameState;

    private Vector2I playerTilePosition;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.tileMapLayer = this.GetNode<TileMapLayer>("TileMapLayer");
        this.player = this.GetNode<Node2D>("Player");
        this.infoLabel = this.GetNode<Label>("InfoLabel");
        this.sceneManager = this.GetNode<SceneManager>("/root/SceneManager");
        this.gameState = this.GetNode<GameState>("/root/GameState");

        this.LoadDungeonData();
        InitializeTileMap();
        this.SetPlayerStartPosition();
    }

    private void LoadDungeonData()
    {
        try
        {
            string dataPath = "res://Source/Data/scenes/scene4_tile_dungeon/dungeon.json";
            var jsonText = Godot.FileAccess.GetFileAsString(dataPath);
            var jsonNode = Json.ParseString(jsonText).AsGodotDictionary();

            this.dungeonData = new TileDungeonData
            {
                Type = jsonNode["type"].ToString(),
                Controls = jsonNode["controls"].ToString(),
                ExitCondition = jsonNode["exitCondition"].ToString(),
                UI = new DungeonUI(),
            };

            // Parse tilemap
            foreach (var line in jsonNode["tilemap"].AsGodotArray())
            {
                this.dungeonData.Tilemap.Add(line.ToString());
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
                    Description = entry["description"].ToString(),
                };
                this.dungeonData.Legend[key.ToString()[0]] = definition;
            }

            // Parse UI
            var uiDict = jsonNode["ui"].AsGodotDictionary();
            this.dungeonData.UI.ShowInventory = uiDict["showInventory"].AsBool();
            this.dungeonData.UI.ShowMap = uiDict["showMap"].AsBool();
            foreach (var stat in uiDict["showStats"].AsGodotArray())
            {
                this.dungeonData.UI.ShowStats.Add(stat.ToString());
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to load tile dungeon data: {e.Message}");
            this.dungeonData = new TileDungeonData();
        }
    }

    private static void InitializeTileMap()
    {
        // This is a placeholder implementation
        // In a full implementation, you would create tiles based on _dungeonData.Tilemap and _dungeonData.Legend
    }

    private void SetPlayerStartPosition()
    {
        if (this.dungeonData == null)
        {
            return;
        }

        // Place player at first walkable tile
        for (int y = 0; y < this.dungeonData.Tilemap.Count; y++)
        {
            for (int x = 0; x < this.dungeonData.Tilemap[y].Length; x++)
            {
                char symbol = this.dungeonData.Tilemap[y][x];
                if (this.dungeonData.Legend.TryGetValue(symbol, out var definition) && definition.Walkable)
                {
                    this.playerTilePosition = new Vector2I(x, y);
                    this.UpdatePlayerPosition();
                    return;
                }
            }
        }
    }

    /// <inheritdoc/>
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            Vector2I direction = Vector2I.Zero;
            if (keyEvent.Keycode == Key.Up || keyEvent.Keycode == Key.W)
            {
                direction = new Vector2I(0, -1);
            }
            else if (keyEvent.Keycode == Key.Down || keyEvent.Keycode == Key.S)
            {
                direction = new Vector2I(0, 1);
            }
            else if (keyEvent.Keycode == Key.Left || keyEvent.Keycode == Key.A)
            {
                direction = new Vector2I(-1, 0);
            }
            else if (keyEvent.Keycode == Key.Right || keyEvent.Keycode == Key.D)
            {
                direction = new Vector2I(1, 0);
            }

            if (direction != Vector2I.Zero)
            {
                this.TryMovePlayer(direction);
            }
        }
    }

    private void TryMovePlayer(Vector2I direction)
    {
        var newPosition = this.playerTilePosition + direction;
        if (this.IsWalkable(newPosition))
        {
            this.playerTilePosition = newPosition;
            this.UpdatePlayerPosition();
            this.CheckTileInteraction();
        }
    }

    private bool IsWalkable(Vector2I position)
    {
        if (this.dungeonData == null)
        {
            return false;
        }

        if (position.Y < 0 || position.Y >= this.dungeonData.Tilemap.Count)
        {
            return false;
        }

        if (position.X < 0 || position.X >= this.dungeonData.Tilemap[position.Y].Length)
        {
            return false;
        }

        char symbol = this.dungeonData.Tilemap[position.Y][position.X];
        if (this.dungeonData.Legend.TryGetValue(symbol, out var definition))
        {
            return definition.Walkable;
        }

        return false;
    }

    private void UpdatePlayerPosition()
    {
        if (this.tileMapLayer?.TileSet == null || this.player == null)
        {
            return;
        }

        Vector2 tileSize = this.tileMapLayer.TileSet.TileSize;
        Vector2 worldPosition = new Vector2(this.playerTilePosition.X * tileSize.X, this.playerTilePosition.Y * tileSize.Y);
        this.player.Position = worldPosition;
    }

    private void CheckTileInteraction()
    {
        if (this.dungeonData == null || this.infoLabel == null || this.sceneManager == null)
        {
            return;
        }

        char symbol = this.dungeonData.Tilemap[this.playerTilePosition.Y][this.playerTilePosition.X];
        if (this.dungeonData.Legend.TryGetValue(symbol, out var definition) && definition.Interactable)
        {
            this.infoLabel.Text = definition.Description;

            if (definition.Type == TileType.Exit)
            {
                this.sceneManager.TransitionToScene("Scene5PixelCombat");
            }
        }
    }
}
