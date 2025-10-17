
// <copyright file="AsciiRoomRenderer.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Godot;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.Field;
/// <summary>
/// Renders ASCII-based dungeon rooms for the NetHack-style sequence.
/// Handles player movement, object interactions, and dungeon progression.
/// Loads dungeon data from JSON and renders it as ASCII art with collision detection.
/// </summary>
[GlobalClass]
public partial class AsciiRoomRenderer : Node2D
{
    private Label? asciiDisplay;
    private DungeonSequenceData dungeonData = new();
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
        if (this.dungeonData.Dungeons.Count == 0)
        {
            return;
        }

        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            Vector2I newPosition = this.playerPosition;

            if (keyEvent.Keycode == Key.W || keyEvent.Keycode == Key.Up)
            {
                newPosition.Y--;
                GetTree().Root.SetInputAsHandled();
            }
            else if (keyEvent.Keycode == Key.S || keyEvent.Keycode == Key.Down)
            {
                newPosition.Y++;
                GetTree().Root.SetInputAsHandled();
            }
            else if (keyEvent.Keycode == Key.A || keyEvent.Keycode == Key.Left)
            {
                newPosition.X--;
                GetTree().Root.SetInputAsHandled();
            }
            else if (keyEvent.Keycode == Key.D || keyEvent.Keycode == Key.Right)
            {
                newPosition.X++;
                GetTree().Root.SetInputAsHandled();
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
            string dataPath = "res://Source/Data/stages/nethack/dungeon_sequence.json";
            var configData = ConfigurationService.LoadConfiguration(dataPath);

            // Map the dictionary to DungeonSequenceData
            if (configData != null && configData.TryGetValue("dungeons", out var dungeonsVar))
            {
                var dungeonsArray = dungeonsVar.AsGodotArray();
                this.dungeonData = new DungeonSequenceData();

                foreach (var dungeonVar in dungeonsArray)
                {
                    var dungeonDict = dungeonVar.AsGodotDictionary();
                    var room = DeserializeDungeonRoom(dungeonDict);
                    if (room != null)
                    {
                        this.dungeonData.Dungeons.Add(room);
                    }
                }
            }

            GD.Print($"Loaded dungeon sequence with {this.dungeonData.Dungeons.Count} rooms");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to load dungeon data: {ex.Message}");
            this.dungeonData = new DungeonSequenceData();
        }
    }

    private void InitializePlayerPosition()
    {
        if (this.dungeonData.Dungeons.Count == 0)
        {
            return;
        }

        this.playerPosition = this.dungeonData.Dungeons[this.currentDungeonIndex].PlayerStartPosition;
    }

    private void RenderDungeon()
    {
        if (this.asciiDisplay == null || this.dungeonData.Dungeons.Count == 0)
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
        if (this.dungeonData.Dungeons.Count == 0)
        {
            GD.Print("IsValidMove: No dungeons loaded");
            return false;
        }

        var dungeon = this.dungeonData.Dungeons[this.currentDungeonIndex];
        if (position.Y < 0 || position.Y >= dungeon.Map.Count)
        {
            GD.Print($"IsValidMove: Position {position} is out of bounds vertically (map height: {dungeon.Map.Count})");
            return false;
        }

        if (position.X < 0 || position.X >= dungeon.Map[position.Y].Length)
        {
            GD.Print($"IsValidMove: Position {position} is out of bounds horizontally (map width: {dungeon.Map[position.Y].Length})");
            return false;
        }

        char tile = dungeon.Map[position.Y][position.X];

        // Check if the tile is in the legend and if it's walkable (not a wall)
        if (dungeon.Legend.TryGetValue(tile, out string? tileDescription))
        {
            // If the tile is a wall, it's not valid to move to
            if (tileDescription == "wall")
            {
                GD.Print($"IsValidMove: Position {position} contains a wall ('{tile}')");
                return false;
            }

            return true;
        }

        // If the tile is not in the legend, assume it's not walkable
        GD.Print($"IsValidMove: Position {position} contains unknown tile '{tile}' not in legend");
        return false;
    }

    private void CheckObjectInteraction()
    {
        if (this.dungeonData.Dungeons.Count == 0)
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
        if (this.gameState == null || this.dungeonData.Dungeons.Count == 0)
        {
            return;
        }

        // Update Dreamweaver scores
        int score = obj.AlignedTo == this.dungeonData.Dungeons[this.currentDungeonIndex].Owner ? 2 : 1;
        if (this.gameState.DreamweaverScores != null)
        {
            this.gameState.DreamweaverScores[obj.AlignedTo] += score;
        }

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
                if (this.sceneManager != null)
                {
                    this.sceneManager.TransitionToScene("Scene3NeverGoAlone");
                }
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

    /// <summary>
    /// Deserializes a Godot dictionary into a <see cref="DungeonRoom"/> object.
    /// </summary>
    /// <param name="dungeonDict">The Godot dictionary containing dungeon data.</param>
    /// <returns>A <see cref="DungeonRoom"/> instance, or <see langword="null"/> if deserialization fails.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required dictionary keys are missing.</exception>
    private static DungeonRoom? DeserializeDungeonRoom(Godot.Collections.Dictionary dungeonDict)
    {
        try
        {
            if (dungeonDict == null)
            {
                GD.PrintErr("DeserializeDungeonRoom: Received null dictionary");
                return null;
            }

            // Extract required fields
            if (!dungeonDict.TryGetValue("owner", out var ownerVar))
            {
                GD.PrintErr("DeserializeDungeonRoom: Missing 'owner' field");
                return null;
            }

            var owner = Enum.Parse<DreamweaverType>(ownerVar.ToString() ?? "Light");

            if (!dungeonDict.TryGetValue("map", out var mapVar))
            {
                GD.PrintErr("DeserializeDungeonRoom: Missing 'map' field");
                return null;
            }

            var mapArray = mapVar.AsGodotArray();
            var map = new Godot.Collections.Array<string>();
            foreach (var line in mapArray)
            {
                map.Add(line.AsString());
            }

            if (!dungeonDict.TryGetValue("legend", out var legendVar))
            {
                GD.PrintErr("DeserializeDungeonRoom: Missing 'legend' field");
                return null;
            }

            var legendDict = legendVar.AsGodotDictionary();
            var yamlLegend = new Godot.Collections.Dictionary<string, string>();
            foreach (var kvp in legendDict)
            {
                yamlLegend[kvp.Key.AsString()] = kvp.Value.AsString();
            }

            // Find player start position by scanning map
            var mapList = new List<string>();
            foreach (var line in map)
            {
                mapList.Add(line);
            }

            var playerStart = FindPlayerStart(mapList);

            // Deserialize objects
            var yamlObjects = new Godot.Collections.Dictionary<string, DungeonObject>();
            if (dungeonDict.TryGetValue("objects", out var objectsVar))
            {
                var objectsDict = objectsVar.AsGodotDictionary();
                foreach (var kvp in objectsDict)
                {
                    string symbol = kvp.Key.AsString();
                    var obj = DeserializeDungeonObject(kvp.Value.AsGodotDictionary());
                    if (obj != null)
                    {
                        yamlObjects[symbol] = obj;
                    }
                }
            }

            var room = new DungeonRoom
            {
                Owner = owner,
                Map = map,
                YamlLegend = yamlLegend,
                YamlObjects = yamlObjects,
            };

            // Set player start position
            var yamlPlayerStart = new Godot.Collections.Array<int> { playerStart.X, playerStart.Y };
            room.YamlPlayerStartPosition = yamlPlayerStart;

            GD.Print($"Deserialized dungeon room for owner {owner}");
            return room;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"DeserializeDungeonRoom: Error deserializing dungeon: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Deserializes a Godot dictionary into a <see cref="DungeonObject"/> instance.
    /// </summary>
    /// <param name="objectDict">The Godot dictionary containing object data.</param>
    /// <returns>A <see cref="DungeonObject"/> instance, or <see langword="null"/> if deserialization fails.</returns>
    private static DungeonObject? DeserializeDungeonObject(Godot.Collections.Dictionary objectDict)
    {
        try
        {
            if (objectDict == null)
            {
                GD.PrintErr("DeserializeDungeonObject: Received null dictionary");
                return null;
            }

            string type = objectDict.TryGetValue("type", out var typeVar) ? typeVar.AsString() : "Door";
            string text = objectDict.TryGetValue("text", out var textVar) ? textVar.AsString() : string.Empty;
            string alignedToStr = objectDict.TryGetValue("aligned_to", out var alignedVar) ? alignedVar.AsString() : "Light";

            var obj = new DungeonObject
            {
                YamlType = type,
                Text = text,
                YamlAlignedTo = alignedToStr,
            };

            // Set position from dictionary
            if (objectDict.TryGetValue("position", out var posVar))
            {
                var posArray = posVar.AsGodotArray();
                if (posArray.Count >= 2)
                {
                    obj.YamlPosition = new Godot.Collections.Array<int> { (int)posArray[0].AsInt64(), (int)posArray[1].AsInt64() };
                }
            }

            GD.Print($"Deserialized dungeon object at {obj.Position}: {text}");
            return obj;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"DeserializeDungeonObject: Error deserializing object: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Finds the player start position by scanning the map for the '@' character.
    /// </summary>
    /// <param name="map">The dungeon map as a list of strings.</param>
    /// <returns>A <see cref="Vector2I"/> representing the player start position, or <see cref="Vector2I.Zero"/> if not found.</returns>
    private static Vector2I FindPlayerStart(List<string> map)
    {
        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] == '@')
                {
                    return new Vector2I(x, y);
                }
            }
        }

        GD.PrintErr("FindPlayerStart: '@' character not found in map, using default position (0, 0)");
        return Vector2I.Zero;
    }
}
