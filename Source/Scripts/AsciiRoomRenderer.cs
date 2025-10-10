using Godot;
using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts;

public partial class AsciiRoomRenderer : Node2D
{
    private Label _asciiDisplay;
    private DungeonSequenceData _dungeonData;
    private int _currentDungeonIndex = 0;
    private Vector2I _playerPosition;
    private SceneManager _sceneManager;
    private GameState _gameState;

    public override void _Ready()
    {
        _asciiDisplay = GetNode<Label>("AsciiDisplay");
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");
        _gameState = GetNode<GameState>("/root/GameState");

        LoadDungeonData();
        InitializePlayerPosition();
        RenderDungeon();
    }

    private void LoadDungeonData()
    {
        try
        {
            string dataPath = "res://Source/Data/scenes/scene2_nethack/dungeon_sequence.json";
            var jsonText = Godot.FileAccess.GetFileAsString(dataPath);
            var jsonNode = Json.ParseString(jsonText).AsGodotDictionary();

            _dungeonData = new DungeonSequenceData();
            _dungeonData.Type = jsonNode["type"].ToString();

            if (jsonNode.ContainsKey("dungeons"))
            {
                foreach (var dungeonNode in jsonNode["dungeons"].AsGodotArray())
                {
                    var dungeonDict = dungeonNode.AsGodotDictionary();
                    var dungeon = new DungeonRoom();

                    // Parse owner
                    if (Enum.TryParse<DreamweaverType>(dungeonDict["owner"].ToString(), out var owner))
                    {
                        dungeon.Owner = owner;
                    }

                    // Parse map
                    if (dungeonDict.ContainsKey("map"))
                    {
                        foreach (var line in dungeonDict["map"].AsGodotArray())
                        {
                            dungeon.Map.Add(line.ToString());
                        }
                    }

                    // Parse legend
                    if (dungeonDict.ContainsKey("legend"))
                    {
                        var legendDict = dungeonDict["legend"].AsGodotDictionary();
                        foreach (var key in legendDict.Keys)
                        {
                            dungeon.Legend[key.ToString()[0]] = legendDict[key].ToString();
                        }
                    }

                    // Parse objects
                    if (dungeonDict.ContainsKey("objects"))
                    {
                        var objectsDict = dungeonDict["objects"].AsGodotDictionary();
                        foreach (var key in objectsDict.Keys)
                        {
                            var objDict = objectsDict[key].AsGodotDictionary();
                            var obj = new DungeonObject();

                            if (Enum.TryParse<ObjectType>(objDict["type"].ToString(), out var type))
                            {
                                obj.Type = type;
                            }

                            obj.Text = objDict["text"].ToString();

                            if (Enum.TryParse<DreamweaverType>(objDict["alignedTo"].ToString(), out var aligned))
                            {
                                obj.AlignedTo = aligned;
                            }

                            var posArray = objDict["position"].AsGodotArray();
                            obj.Position = new Vector2I(posArray[0].AsInt32(), posArray[1].AsInt32());

                            dungeon.Objects[key.ToString()[0]] = obj;
                        }
                    }

                    // Parse player start
                    if (dungeonDict.ContainsKey("playerStartPosition"))
                    {
                        var posArray = dungeonDict["playerStartPosition"].AsGodotArray();
                        dungeon.PlayerStartPosition = new Vector2I(posArray[0].AsInt32(), posArray[1].AsInt32());
                    }

                    _dungeonData.Dungeons.Add(dungeon);
                }
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to load dungeon data: {e.Message}");
            // Create fallback data
            _dungeonData = new DungeonSequenceData();
        }
    }

    private void InitializePlayerPosition()
    {
        if (_dungeonData.Dungeons.Count > 0)
        {
            _playerPosition = _dungeonData.Dungeons[_currentDungeonIndex].PlayerStartPosition;
        }
    }

    private void RenderDungeon()
    {
        if (_dungeonData.Dungeons.Count == 0) return;

        var dungeon = _dungeonData.Dungeons[_currentDungeonIndex];
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
        if (_playerPosition.Y < map.Count && _playerPosition.X < map[_playerPosition.Y].Length)
        {
            var row = map[_playerPosition.Y].ToCharArray();
            row[_playerPosition.X] = '@';
            map[_playerPosition.Y] = new string(row);
        }

        _asciiDisplay.Text = string.Join("\n", map);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            Vector2I newPosition = _playerPosition;

            if (keyEvent.Keycode == Key.W || keyEvent.Keycode == Key.Up)
                newPosition.Y--;
            else if (keyEvent.Keycode == Key.S || keyEvent.Keycode == Key.Down)
                newPosition.Y++;
            else if (keyEvent.Keycode == Key.A || keyEvent.Keycode == Key.Left)
                newPosition.X--;
            else if (keyEvent.Keycode == Key.D || keyEvent.Keycode == Key.Right)
                newPosition.X++;

            if (IsValidMove(newPosition))
            {
                _playerPosition = newPosition;
                CheckObjectInteraction();
                RenderDungeon();
            }
        }
    }

    private bool IsValidMove(Vector2I position)
    {
        var dungeon = _dungeonData.Dungeons[_currentDungeonIndex];
        if (position.Y < 0 || position.Y >= dungeon.Map.Count) return false;
        if (position.X < 0 || position.X >= dungeon.Map[position.Y].Length) return false;
        
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
        var dungeon = _dungeonData.Dungeons[_currentDungeonIndex];

        foreach (var kvp in dungeon.Objects)
        {
            if (kvp.Value.Position == _playerPosition)
            {
                InteractWithObject(kvp.Value);
                break;
            }
        }
    }

    private void InteractWithObject(DungeonObject obj)
    {
        // Update Dreamweaver scores
        int score = obj.AlignedTo == _dungeonData.Dungeons[_currentDungeonIndex].Owner ? 2 : 1;
        _gameState.DreamweaverScores[obj.AlignedTo] += score;

        // Display interaction text
        GD.Print(obj.Text);
        GD.Print($"Dreamweaver {obj.AlignedTo} score increased by {score} points!");

        // Remove object
        var dungeon = _dungeonData.Dungeons[_currentDungeonIndex];
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
            _currentDungeonIndex++;
            if (_currentDungeonIndex >= _dungeonData.Dungeons.Count)
            {
                // Transition to party creation
                _sceneManager.TransitionToScene("Scene3WizardryParty");
            }
            else
            {
                InitializePlayerPosition();
                RenderDungeon();
            }
        }
        else
        {
            RenderDungeon();
        }
    }
}