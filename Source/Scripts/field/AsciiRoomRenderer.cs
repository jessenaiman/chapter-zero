// <copyright file="AsciiRoomRenderer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OmegaSpiral.Source.Scripts;
using YamlDotNet.Serialization;

/// <summary>
/// Renders ASCII-based dungeon rooms for the NetHack-style sequence.
/// Handles player movement, object interactions, and dungeon progression.
/// Loads dungeon data from JSON and renders it as ASCII art with collision detection.
/// </summary>
public partial class AsciiRoomRenderer : Node2D
{
    private Label? asciiDisplay;
    private DungeonSequenceData? dungeonData;
    private int currentDungeonIndex;
    private Vector2I playerPosition;
    private SceneManager? sceneManager;
    private GameState? gameState;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.asciiDisplay = this.GetNode<Label>("AsciiDisplay");
        this.sceneManager = this.GetNode<SceneManager>("/root/SceneManager");
        this.gameState = this.GetNode<GameState>("/root/GameState");

        if (this.asciiDisplay == null || this.sceneManager == null || this.gameState == null)
        {
            GD.PrintErr("Failed to initialize required nodes in AsciiRoomRenderer");
            return;
        }

        this.LoadDungeonData();
        this.InitializePlayerPosition();
        this.RenderDungeon();
    }

     /// <inheritdoc/>
    public override void _Input(InputEvent @event)
    {
        if (this.dungeonData == null || this.dungeonData.Dungeons.Count == 0)
        {
            return;
        }

        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            Vector2I newPosition = this.playerPosition;

            if (keyEvent.Keycode == Key.W || keyEvent.Keycode == Key.Up)
            {
                newPosition.Y--;
            }
            else if (keyEvent.Keycode == Key.S || keyEvent.Keycode == Key.Down)
            {
                newPosition.Y++;
            }
            else if (keyEvent.Keycode == Key.A || keyEvent.Keycode == Key.Left)
            {
                newPosition.X--;
            }
            else if (keyEvent.Keycode == Key.D || keyEvent.Keycode == Key.Right)
            {
                newPosition.X++;
            }

            if (this.IsValidMove(newPosition))
            {
                this.playerPosition = newPosition;
                this.CheckObjectInteraction();
                this.RenderDungeon();
            }
        }
    }

    private void LoadDungeonData()
    {
        try
        {
            string dataPath = "res://Source/Data/scenes/scene2_nethack/dungeon_sequence.yaml";
            var yamlText = Godot.FileAccess.GetFileAsString(dataPath);

            var deserializer = new DeserializerBuilder().Build();
            this.dungeonData = deserializer.Deserialize<DungeonSequenceData>(yamlText);

            GD.Print($"Loaded dungeon sequence with {this.dungeonData.Dungeons.Count} rooms");
        }
        catch (InvalidOperationException ex)
        {
            GD.PrintErr($"Failed to load dungeon data: {ex.Message}");
            this.dungeonData = new DungeonSequenceData();
        }
        catch (YamlDotNet.Core.YamlException ex)
        {
            GD.PrintErr($"YAML parsing error for dungeon data: {ex.Message}");
            this.dungeonData = new DungeonSequenceData();
        }
    }

    private void InitializePlayerPosition()
    {
        if (this.dungeonData == null || this.dungeonData.Dungeons.Count == 0)
        {
            return;
        }

        this.playerPosition = this.dungeonData.Dungeons[this.currentDungeonIndex].PlayerStartPosition;
    }

    private void RenderDungeon()
    {
        if (this.dungeonData == null || this.asciiDisplay == null || this.dungeonData.Dungeons.Count == 0)
        {
            return;
        }

        var dungeon = this.dungeonData.Dungeons[this.currentDungeonIndex];
        var map = new List<string>();

        // Copy map
        foreach (var line in dungeon.Map)
        {
            map.Add(line);
        }

        // Place objects
        foreach (var kvp in dungeon.Objects)
        {
            char symbol = kvp.Key;
            var obj = kvp.Value;
            if (obj.Position.Y < map.Count && obj.Position.X < map[obj.Position.Y].Length)
            {
                var row = map[obj.Position.Y].ToCharArray();
                row[obj.Position.X] = symbol;
                map[obj.Position.Y] = new string(row);
            }
        }

        // Place player (overwrites objects if on same position)
        if (this.playerPosition.Y < map.Count && this.playerPosition.X < map[this.playerPosition.Y].Length)
        {
            var row = map[this.playerPosition.Y].ToCharArray();
            row[this.playerPosition.X] = '@';
            map[this.playerPosition.Y] = new string(row);
        }

        this.asciiDisplay.Text = string.Join("\n", map);
    }

    private bool IsValidMove(Vector2I position)
    {
        if (this.dungeonData == null || this.dungeonData.Dungeons.Count == 0)
        {
            return false;
        }

        var dungeon = this.dungeonData.Dungeons[this.currentDungeonIndex];
        if (position.Y < 0 || position.Y >= dungeon.Map.Count)
        {
            return false;
        }

        if (position.X < 0 || position.X >= dungeon.Map[position.Y].Length)
        {
            return false;
        }

        char tile = dungeon.Map[position.Y][position.X];

        // Check if the tile is in the legend and if it's walkable (not a wall)
        if (dungeon.Legend.ContainsKey(tile))
        {
            string tileDescription = dungeon.Legend[tile];

            // If the tile is a wall, it's not valid to move to
            return tileDescription != "wall";
        }

        // If the tile is not in the legend, assume it's not walkable
        return false;
    }

    private void CheckObjectInteraction()
    {
        if (this.dungeonData == null || this.dungeonData.Dungeons.Count == 0)
        {
            return;
        }

        var dungeon = this.dungeonData.Dungeons[this.currentDungeonIndex];

        foreach (var kvp in dungeon.Objects)
        {
            if (kvp.Value.Position == this.playerPosition)
            {
                this.InteractWithObject(kvp.Value);
                break;
            }
        }
    }

    private void InteractWithObject(DungeonObject obj)
    {
        if (this.dungeonData == null || this.gameState == null || this.dungeonData.Dungeons.Count == 0)
        {
            return;
        }

        // Update Dreamweaver scores
        int score = obj.AlignedTo == this.dungeonData.Dungeons[this.currentDungeonIndex].Owner ? 2 : 1;
        this.gameState.DreamweaverScores[obj.AlignedTo] += score;

        // Display interaction text
        GD.Print(obj.Text);
        GD.Print($"Dreamweaver {obj.AlignedTo} score increased by {score} points!");

        // Remove object
        var dungeon = this.dungeonData.Dungeons[this.currentDungeonIndex];
        char symbolToRemove = ' ';
        foreach (var kvp in dungeon.Objects)
        {
            if (kvp.Value == obj)
            {
                symbolToRemove = kvp.Key;
                break;
            }
        }

        dungeon.Objects.Remove(symbolToRemove);

        // Check if all objects collected
        if (dungeon.Objects.Count == 0)
        {
            // Move to next dungeon or scene
            this.currentDungeonIndex++;
            if (this.currentDungeonIndex >= this.dungeonData.Dungeons.Count)
            {
                // Transition to party creation
                this.sceneManager?.TransitionToScene("Scene3WizardryParty");
            }
            else
            {
                this.InitializePlayerPosition();
                this.RenderDungeon();
            }
        }
        else
        {
            this.RenderDungeon();
        }
    }
}
