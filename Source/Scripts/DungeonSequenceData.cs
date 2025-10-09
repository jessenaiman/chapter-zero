using Godot;
using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts;

public partial class DungeonSequenceData
{
    public string Type { get; set; } = "ascii_dungeon_sequence";
    public List<DungeonRoom> Dungeons { get; set; } = new();
}

public partial class DungeonRoom
{
    public DreamweaverType Owner { get; set; }
    public List<string> Map { get; set; } = new();
    public Dictionary<char, string> Legend { get; set; } = new();
    public Dictionary<char, DungeonObject> Objects { get; set; } = new();
    public Vector2I PlayerStartPosition { get; set; } = new(2, 2);
}

public partial class DungeonObject
{
    public ObjectType Type { get; set; }
    public string Text { get; set; }
    public DreamweaverType AlignedTo { get; set; }
    public Vector2I Position { get; set; }
}