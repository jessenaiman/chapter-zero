
// <copyright file="AsciiRoomRenderer.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

#nullable enable

using Godot;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Infrastructure;

namespace OmegaSpiral.Source.Scripts.Field;
/// <summary>
/// Enhanced ASCII-based dungeon room renderer with godot-xterm integration.
/// Provides immersive NetHack-style dungeon exploration with 3D terminal effects,
/// animated rendering, and atmospheric audio feedback.
/// </summary>
[GlobalClass]
public partial class AsciiRoomRenderer : Node2D
{
    /// <summary>
    /// Terminal components from godot-xterm addon.
    /// </summary>
    private Node? terminalNode;
    private Node? ptyNode;

    /// <summary>
    /// 3D display components for immersive presentation.
    /// </summary>
    private Node3D? terminalDisplay3D;
    // TODO: Stage2 - TerminalDisplayBox was removed during Stage1 cleanup, needs reimplementation for ASCII dungeon
    // private TerminalDisplayBox? terminalDisplayBox;
    private MeshInstance3D? screenMesh;
    private Camera3D? camera3D;
    private DirectionalLight3D? screenLight;

    /// <summary>
    /// Enhanced 2D overlay components.
    /// </summary>
    private RichTextLabel? overlayText;
    private CanvasLayer? overlayCanvas;
    private TextureRect? screenGlow;

    /// <summary>
    /// Animation and effect components.
    /// </summary>
    private Godot.Timer? renderTimer;
    private Godot.Timer? movementTimer;
    private Godot.Timer? glowPulseTimer;
    private AudioStreamPlayer? dungeonAudio;
    private AudioStreamPlayer? movementAudio;

    /// <summary>
    /// Dungeon state.
    /// </summary>
    private DungeonSequenceData dungeonData = new();
    private int currentDungeonIndex;
    private Vector2I playerPosition;
    private SceneManager? sceneManager;
    private GameState? gameState;

    /// <summary>
    /// Rendering state.
    /// </summary>
    private string currentDungeonText = string.Empty;
    private string targetDungeonText = string.Empty;
    private int currentCharIndex;
    private bool isRendering;
    private bool isMoving;

    /// <summary>
    /// Configuration properties for rendering and effects.
    /// </summary>
    [Export] public float RenderSpeed { get; set; } = 0.02f;
    [Export] public float MovementDelay { get; set; } = 0.15f;
    [Export] public bool Enable3DEffects { get; set; } = true;
    [Export] public bool EnableGlowEffects { get; set; } = true;
    [Export] public bool EnableAudio { get; set; } = true;
    [Export] public Color ScreenGlowColor { get; set; } = new Color(0.2f, 1.0f, 0.2f);
    [Export] public float GlowIntensity { get; set; } = 0.8f;

    /// <inheritdoc/>
    public override void _Ready()
    {
        this.InitializeTerminal();
        this.Initialize3DComponents();
        this.Initialize2DOverlay();
        this.InitializeEffects();
        this.InitializeAudio();

        this.sceneManager = this.GetNode<SceneManager>("/root/SceneManager");
        this.gameState = this.GetNode<GameState>("/root/GameState");

        if (this.sceneManager == null || this.gameState == null)
        {
            GD.PrintErr("Failed to initialize required nodes in AsciiRoomRenderer");
            return;
        }

        this.LoadDungeonData();
        this.InitializePlayerPosition();
        this.CallDeferred(nameof(this.StartDungeonRendering));
    }

    private void InitializeTerminal()
    {
        // Get the Terminal node from godot-xterm addon
        this.terminalNode = this.GetNode<Node>("Terminal");
        if (this.terminalNode == null)
        {
            GD.PrintErr("Terminal node not found - ensure godot-xterm addon is properly configured");
            return;
        }

        // Get PTY node for terminal functionality
        this.ptyNode = this.GetNode<Node>("PTY");
        if (this.ptyNode == null)
        {
            GD.PrintErr("PTY node not found - ensure godot-xterm addon is properly configured");
            return;
        }

        // Connect terminal signals for enhanced interaction
        this.terminalNode.Connect("data_sent", Callable.From<string>(this.OnTerminalDataSent));
        this.ptyNode.Connect("data_received", Callable.From<string>(this.OnTerminalDataReceived));
        this.terminalNode.Connect("size_changed", Callable.From<Vector2I>(this.OnTerminalSizeChanged));

        // Configure terminal for dungeon display
        this.ConfigureTerminalForDungeon();
    }

    private void ConfigureTerminalForDungeon()
    {
        if (this.terminalNode == null)
            return;

        // Set terminal properties for optimal dungeon display
        // These would be configured based on godot-xterm API
        GD.Print("Configuring terminal for ASCII dungeon display");
    }

    private void Initialize3DComponents()
    {
        if (!this.Enable3DEffects)
            return;

        // TODO: Stage2 - Temporarily disabled 3D effects pending TerminalDisplayBox reimplementation
        /*
        // Create the terminal display box with dungeon-appropriate geometry
        this.terminalDisplayBox = new TerminalDisplayBox
        {
            Name = "TerminalDisplayBox",
            ScreenWidth = 10.0f,  // Wider for dungeon display
            ScreenHeight = 6.0f,  // Taller for dungeon display
            BezelThickness = 0.2f,
            CaseThickness = 0.5f,
            ScreenGlowColor = this.ScreenGlowColor,
            ScreenGlowIntensity = this.GlowIntensity,
            EnableScreenGlow = this.EnableGlowEffects,
            EnableAmbientLight = true
        };

        // Get components from display box
        this.screenMesh = this.terminalDisplayBox.GetScreenMesh();
        this.camera3D = this.terminalDisplayBox.GetViewingCamera();

        // Create 3D container
        this.terminalDisplay3D = new Node3D { Name = "TerminalDisplay3D" };
        this.terminalDisplay3D.AddChild(this.terminalDisplayBox);
        this.AddChild(this.terminalDisplay3D);
        */

        // TODO: Stage2 - Lighting also disabled until TerminalDisplayBox is reimplemented
        /*
        // Add atmospheric lighting for dungeon ambiance
        this.screenLight = new DirectionalLight3D
        {
            Name = "DungeonLight",
            Position = new Vector3(0, -1, -3),
            Rotation = new Vector3(Mathf.Pi * 0.25f, 0, 0),
            LightColor = new Color(0.1f, 0.8f, 0.1f), // Green tint for retro feel
            LightEnergy = 0.4f,
            LightIndirectEnergy = 0.6f
        };
        this.terminalDisplay3D.AddChild(this.screenLight);
        */
    }

    private void Initialize2DOverlay()
    {
        // Create overlay canvas for additional visual effects
        this.overlayCanvas = new CanvasLayer { Name = "OverlayCanvas" };
        this.AddChild(this.overlayCanvas);

        // Create overlay text for status messages and effects
        this.overlayText = new RichTextLabel
        {
            Name = "OverlayText",
            Position = new Vector2(50, 50),
            Size = new Vector2(700, 100),
            BbcodeEnabled = true,
            Visible = false
        };
        this.overlayCanvas.AddChild(this.overlayText);

        // Create screen glow effect
        if (this.EnableGlowEffects)
        {
            this.screenGlow = new TextureRect
            {
                Name = "ScreenGlow",
                Position = new Vector2(0, 0),
                Size = new Vector2(800, 600),
                Modulate = new Color(0.2f, 1.0f, 0.2f, 0.1f),
                Visible = false
            };
            this.overlayCanvas.AddChild(this.screenGlow);
        }
    }

    private void InitializeEffects()
    {
        // Create render timer for typewriter-style dungeon display
        this.renderTimer = new Godot.Timer
        {
            Name = "RenderTimer",
            WaitTime = this.RenderSpeed,
            OneShot = false
        };
        this.renderTimer.Timeout += this.OnRenderTimerTimeout;
        this.AddChild(this.renderTimer);

        // Create movement timer for smooth movement feedback
        this.movementTimer = new Godot.Timer
        {
            Name = "MovementTimer",
            WaitTime = this.MovementDelay,
            OneShot = true
        };
        this.movementTimer.Timeout += this.OnMovementTimerTimeout;
        this.AddChild(this.movementTimer);

        // Create glow pulse timer for atmospheric effect
        if (this.EnableGlowEffects)
        {
            this.glowPulseTimer = new Godot.Timer
            {
                Name = "GlowPulseTimer",
                WaitTime = 2.0f,
                OneShot = false
            };
            this.glowPulseTimer.Timeout += this.OnGlowPulseTimeout;
            this.AddChild(this.glowPulseTimer);
            this.glowPulseTimer.Start();
        }
    }

    private void InitializeAudio()
    {
        if (!this.EnableAudio)
            return;

        // Create dungeon ambiance audio
        this.dungeonAudio = new AudioStreamPlayer
        {
            Name = "DungeonAudio",
            VolumeDb = -15.0f,
            Bus = "SFX"
        };
        this.AddChild(this.dungeonAudio);

        // Create movement audio feedback
        this.movementAudio = new AudioStreamPlayer
        {
            Name = "MovementAudio",
            VolumeDb = -10.0f,
            Bus = "SFX"
        };
        this.AddChild(this.movementAudio);
    }

    /// <inheritdoc/>
    public override void _Input(InputEvent @event)
    {
        if (this.dungeonData.Dungeons.Count == 0 || this.isMoving || this.isRendering)
        {
            return;
        }

        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
        {
            this.HandleKeyInput(keyEvent);
            GetTree().Root.SetInputAsHandled();
        }
    }

    private void HandleKeyInput(InputEventKey keyEvent)
    {
        Vector2I newPosition = AsciiRoomRenderer.GetMovementDirection(keyEvent);
        if (newPosition == Vector2I.Zero)
            return;

        newPosition += this.playerPosition;

        if (!this.IsValidMove(newPosition))
            return;

        this.playerPosition = newPosition;
        this.isMoving = true;
        this.movementTimer?.Start();
        this.PlayMovementSound();
        this.CheckObjectInteraction();
        this.StartDungeonRendering();
    }

    private static Vector2I GetMovementDirection(InputEventKey keyEvent)
    {
        return keyEvent.Keycode switch
        {
            Key.W or Key.Up => Vector2I.Up,
            Key.S or Key.Down => Vector2I.Down,
            Key.A or Key.Left => Vector2I.Left,
            Key.D or Key.Right => Vector2I.Right,
            _ => Vector2I.Zero,
        };
    }

    private void StartDungeonRendering()
    {
        if (this.dungeonData.Dungeons.Count == 0)
            return;

        // Generate the full dungeon text
        this.targetDungeonText = this.GenerateDungeonText();

        // Start typewriter-style rendering
        this.currentCharIndex = 0;
        this.currentDungeonText = string.Empty;
        this.isRendering = true;
        this.renderTimer?.Start();

        // Update terminal display
        this.UpdateTerminalDisplay();
    }

    private string GenerateDungeonText()
    {
        var dungeon = this.dungeonData.Dungeons[this.currentDungeonIndex];
        var lines = new List<string>();

        // Add dungeon header with atmospheric styling
        lines.Add($"[color=#00ff00]═══ {dungeon.Owner} Domain ═══[/color]");
        lines.Add(string.Empty);

        // Generate ASCII map with player and objects
        for (int y = 0; y < dungeon.Map.Count; y++)
        {
            var row = dungeon.Map[y].ToCharArray();

            // Place objects
            foreach (var kvp in dungeon.Objects)
            {
                var obj = kvp.Value;
                if (obj.Position.Y == y && obj.Position.X < row.Length)
                {
                    row[obj.Position.X] = kvp.Key;
                }
            }

            // Place player (overwrites objects if on same position)
            if (this.playerPosition.Y == y && this.playerPosition.X < row.Length)
            {
                row[this.playerPosition.X] = '@';
            }

            lines.Add(new string(row));
        }

        lines.Add(string.Empty);
        lines.Add("[color=#888888]Use WASD to move • @ = You • Find all treasures to proceed[/color]");

        return string.Join("\n", lines);
    }

    private void UpdateTerminalDisplay()
    {
        if (this.terminalNode == null || this.ptyNode == null)
            return;

        // Send the current text to the terminal
        // This would use godot-xterm API to write to the terminal
        GD.Print($"Updating terminal with dungeon display (chars: {this.currentDungeonText.Length})");
    }

    private void PlayMovementSound()
    {
        if (!this.EnableAudio || this.movementAudio == null)
            return;

        // Play subtle movement sound
        // This would load and play appropriate audio
        GD.Print("Playing movement sound");
    }

    private void LoadDungeonData()
    {
        try
        {
            Godot.Collections.Dictionary<string, Variant>? configData = null;

            // Use file system path for better test compatibility, fall back to Godot resource path
            string dataPath = "Source/Data/stages/nethack/dungeon_sequence.json";
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), dataPath);

            if (File.Exists(fullPath))
            {
                string jsonContent = File.ReadAllText(fullPath);
                configData = ConfigurationService.LoadConfigurationFromString(jsonContent, fullPath);
            }
            else
            {
                // Fall back to Godot resource path for runtime
                string godotPath = "res://source/Data/stages/nethack/dungeon_sequence.json";
                configData = ConfigurationService.LoadConfiguration(godotPath);
            }

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

    private void OnRenderTimerTimeout()
    {
        if (!this.isRendering || this.currentCharIndex >= this.targetDungeonText.Length)
        {
            this.isRendering = false;
            this.renderTimer?.Stop();
            return;
        }

        // Add next character with typewriter effect
        this.currentDungeonText += this.targetDungeonText[this.currentCharIndex];
        this.currentCharIndex++;

        // Update terminal display incrementally
        this.UpdateTerminalDisplay();
    }

    private void OnMovementTimerTimeout()
    {
        this.isMoving = false;
    }

    private void OnGlowPulseTimeout()
    {
        if (this.screenGlow == null || !this.EnableGlowEffects)
            return;

        // Create subtle glow pulsing effect
        var pulse = (Mathf.Sin((float) Time.GetTicksMsec() * 0.001f) * 0.1f) + 0.9f;
        this.screenGlow.Modulate = new Color(
            this.ScreenGlowColor.R,
            this.ScreenGlowColor.G,
            this.ScreenGlowColor.B,
            this.ScreenGlowColor.A * pulse
        );
    }

    private void OnTerminalDataSent(string data)
    {
        // Handle any terminal input if needed
        GD.Print($"Terminal data sent: {data}");
    }

    private void OnTerminalDataReceived(string data)
    {
        // Handle terminal output
        GD.Print($"Terminal data received: {data}");
    }

    private void OnTerminalSizeChanged(Vector2I size)
    {
        // Handle terminal resize if needed
        GD.Print($"Terminal size changed: {size}");
    }

    private bool IsValidMove(Vector2I position)
    {
        if (this.dungeonData.Dungeons.Count == 0)
        {
            GD.Print("IsValidMove: No dungeons loaded");
            return false;
        }

        var dungeon = this.dungeonData.Dungeons[this.currentDungeonIndex];

        if (!AsciiRoomRenderer.IsWithinBounds(position, dungeon))
            return false;

        return AsciiRoomRenderer.IsTileWalkable(position, dungeon);
    }

    private static bool IsWithinBounds(Vector2I position, DungeonRoom dungeon)
    {
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

        return true;
    }

    private static bool IsTileWalkable(Vector2I position, DungeonRoom dungeon)
    {
        char tile = dungeon.Map[position.Y][position.X];

        if (dungeon.Legend.TryGetValue(tile, out string? tileDescription))
        {
            if (tileDescription == "wall")
            {
                GD.Print($"IsValidMove: Position {position} contains a wall ('{tile}')");
                return false;
            }

            return true;
        }

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

        this.UpdateDreamweaverScore(obj);
        AsciiRoomRenderer.DisplayInteractionText(obj);
        this.RemoveObject(obj);
        this.CheckDungeonProgression();
    }

    private void UpdateDreamweaverScore(DungeonObject obj)
    {
        if (this.gameState?.DreamweaverScores == null)
            return;

        int score = obj.AlignedTo == this.dungeonData.Dungeons[this.currentDungeonIndex].Owner ? 2 : 1;
        this.gameState.DreamweaverScores[obj.AlignedTo] += score;
        GD.Print($"Dreamweaver {obj.AlignedTo} score increased by {score} points!");
    }

    private static void DisplayInteractionText(DungeonObject obj)
    {
        GD.Print(obj.Text);
    }

    private void RemoveObject(DungeonObject obj)
    {
        var dungeon = this.dungeonData.Dungeons[this.currentDungeonIndex];
        foreach (var kvp in dungeon.Objects)
        {
            if (kvp.Value == obj)
            {
                dungeon.Objects.Remove(kvp.Key);
                break;
            }
        }
    }

    private void CheckDungeonProgression()
    {
        var dungeon = this.dungeonData.Dungeons[this.currentDungeonIndex];

        if (dungeon.Objects.Count == 0)
        {
            this.ProgressToNextDungeon();
        }
        else
        {
            this.RenderDungeon();
        }
    }

    private void ProgressToNextDungeon()
    {
        this.currentDungeonIndex++;
        if (this.currentDungeonIndex >= this.dungeonData.Dungeons.Count)
        {
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

            var owner = ExtractOwner(dungeonDict);
            if (owner == null)
                return null;

            var map = ExtractMap(dungeonDict);
            if (map == null)
                return null;

            var legend = ExtractLegend(dungeonDict);
            if (legend == null)
                return null;

            var mapList = new List<string>();
            foreach (var line in map)
            {
                mapList.Add(line);
            }

            var playerStart = FindPlayerStart(mapList);
            var objects = ExtractObjects(dungeonDict);

            var room = new DungeonRoom
            {
                Owner = owner.Value,
                Map = map,
                YamlLegend = legend,
                YamlObjects = objects,
            };

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

    private static DreamweaverType? ExtractOwner(Godot.Collections.Dictionary dungeonDict)
    {
        if (!dungeonDict.TryGetValue("owner", out var ownerVar))
        {
            GD.PrintErr("DeserializeDungeonRoom: Missing 'owner' field");
            return null;
        }

        return Enum.Parse<DreamweaverType>(ownerVar.ToString() ?? "Light");
    }

    private static Godot.Collections.Array<string>? ExtractMap(Godot.Collections.Dictionary dungeonDict)
    {
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

        return map;
    }

    private static Godot.Collections.Dictionary<string, string>? ExtractLegend(Godot.Collections.Dictionary dungeonDict)
    {
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

        return yamlLegend;
    }

    private static Godot.Collections.Dictionary<string, DungeonObject> ExtractObjects(Godot.Collections.Dictionary dungeonDict)
    {
        var yamlObjects = new Godot.Collections.Dictionary<string, DungeonObject>();
        if (!dungeonDict.TryGetValue("objects", out var objectsVar))
        {
            return yamlObjects;
        }

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

        return yamlObjects;
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
                    obj.YamlPosition = new Godot.Collections.Array<int> { (int) posArray[0].AsInt64(), (int) posArray[1].AsInt64() };
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

    private void RenderDungeon()
    {
        this.StartDungeonRendering();
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
