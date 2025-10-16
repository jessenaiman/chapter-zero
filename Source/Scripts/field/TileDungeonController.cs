namespace OmegaSpiral.Source.Scripts.Field;

// <copyright file="TileDungeonController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using Godot;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;
using YamlDotNet.Serialization;

/// <summary>
/// Controls the tile-based dungeon functionality, handling player movement, tile interactions, and dungeon state.
/// </summary>
[GlobalClass]
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
            string dataPath = "res://Source/Data/scenes/scene4_tile_dungeon/dungeon.yaml";
            var yamlText = Godot.FileAccess.GetFileAsString(dataPath);

            var deserializer = new DeserializerBuilder().Build();
            this.dungeonData = deserializer.Deserialize<TileDungeonData>(yamlText);

            GD.Print($"Loaded dungeon data with {this.dungeonData?.Tilemap?.Count ?? 0} tilemap rows");
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"Failed to load dungeon data: {ex.Message}");
            this.dungeonData = new TileDungeonData();
        }
        catch (YamlDotNet.Core.YamlException ex)
        {
            GD.PrintErr($"YAML parsing error for dungeon data: {ex.Message}");
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
