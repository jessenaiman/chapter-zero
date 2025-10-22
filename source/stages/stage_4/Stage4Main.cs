using Godot;
using System.Collections.Generic;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Field.Dialogue;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Stages.Stage4.Dialogue;

namespace OmegaSpiral.Source.Scripts.Stages.Stage4;

/// <summary>
/// Main controller for Stage 4 - Liminal Township
/// Handles the initial wake-up experience and liminal elements
/// </summary>
[GlobalClass]
public partial class Stage4Main : Node2D
{
    private TownshipDialogueManager _dialogueManager = new();
    private DreamweaverSystem? _dreamweaverSystem;
    private Dictionary<string, Node2D> _dreamweaverPresences = new();
    
    // References to important nodes
    private Node? _gamepieces;
    private Node? _interactions;
    private Node2D? _dreamweaverPresencesNode;
    private Label? _debugInfo;

    public override void _Ready()
    {
        InitializeScene();
        SetupLiminalElements();
        SpawnPlayer();
        ShowWakeUpSequence();
        InitializeNpcs();
    }

    private void InitializeScene()
    {
        _gamepieces = GetNodeOrNull<Node>("Gameboard/Gamepieces");
        _interactions = GetNodeOrNull<Node>("Interactions");
        _dreamweaverPresencesNode = GetNodeOrNull<Node2D>("DreamweaverPresences");
        _debugInfo = GetNodeOrNull<Label>("UI/DebugInfo");
        
        // Try to get the Dreamweaver system
        _dreamweaverSystem = GetNodeOrNull<DreamweaverSystem>("/root/DreamweaverSystem");
        
        // Preload common dialogue resources
        _ = _dialogueManager.LoadNpcDialogueAsync("townspeople", "res://source/data/stages/stage_4/town_dialogue.json");
        _ = _dialogueManager.LoadNpcDialogueAsync("innkeeper", "res://source/data/stages/stage_4/innkeeper_dialogue.json");
        _ = _dialogueManager.LoadNpcDialogueAsync("shopkeeper", "res://source/data/stages/stage_4/shopkeeper_dialogue.json");
        _ = _dialogueManager.LoadNpcDialogueAsync("strange_old_man", "res://source/data/stages/stage_4/strange_old_man_dialogue.json");
    }

    private void SetupLiminalElements()
    {
        if (_debugInfo != null)
        {
            _debugInfo.Text = "Liminal Township - The spiral remembers...";
        }
        
        // Add subtle visual effects to suggest the liminal nature
        AddDuplicateFootprints();
        AddFadingBanners();
        AddEchoingSounds();
    }

    private void AddDuplicateFootprints()
    {
        // In a real implementation, this would place visual indicators
        // that suggest multiple players have been through this area
        GD.Print("Added duplicate footprints for liminal effect");
    }

    private void AddFadingBanners()
    {
        // In a real implementation, this would place banners or decorations
        // that appear to fade in and out, suggesting the simulation nature
        GD.Print("Added fading banners for liminal effect");
    }

    private void AddEchoingSounds()
    {
        // In a real implementation, this would add subtle audio echoes
        // that suggest the area has been experienced many times before
        GD.Print("Added echoing sounds for liminal effect");
    }

    private void SpawnPlayer()
    {
        // For now, just ensure the player character exists at the starting position
        // In a full implementation, this would handle the player's initial state
        // based on their arrival from Stage 3
        GD.Print("Player spawned in liminal township");
    }

    private async void ShowWakeUpSequence()
    {
        // Show the initial wake-up experience
        await ShowInitialNarrative();
        await ShowEnvironmentalClues();
        await ActivateLiminalAwareness();
    }

    private async Task ShowInitialNarrative()
    {
        // This would show the initial scene where the player wakes up
        // Perhaps with dialogue like "Another loop... but this time feels different"
        var dialogue = await _dialogueManager.GetNpcDialogueAsync("narrator");
        GD.Print("Showed initial wake-up narrative");
        
        // Simulate the wake-up text display
        await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);
    }

    private async Task ShowEnvironmentalClues()
    {
        // This would highlight environmental elements that suggest
        // the liminal nature of the space
        GD.Print("Highlighted environmental clues (duplicate footprints, fading banners)");
        
        await ToSignal(GetTree().CreateTimer(1.5), SceneTreeTimer.SignalName.Timeout);
    }

    private async Task ActivateLiminalAwareness()
    {
        // This would gradually make the player aware that something
        // is "off" about this town - it remembers too many eras at once
        GD.Print("Activated liminal awareness - the player starts noticing simulation seams");
        
        // Start monitoring for player exploration
        StartExplorationMonitoring();
    }

    private void StartExplorationMonitoring()
    {
        // Set up timers or event handlers to trigger special dialogue
        // when the player approaches certain areas or interacts with specific NPCs
        var explorationTimer = GetTree().CreateTimer(10.0f); // Check every 10 seconds
        explorationTimer.Timeout += OnExplorationCheck;
    }

    private void OnExplorationCheck()
    {
        // Check if player has visited significant locations or talked to NPCs
        // If so, maybe trigger liminal dialogue or Dreamweaver appearances
        GD.Print("Exploration check - player may have discovered more simulation elements");
    }

    public override void _Process(double delta)
    {
        // Handle any ongoing liminal effects or checks
        UpdateLiminalIndicators();
    }

    private void UpdateLiminalIndicators()
    {
        // Update any visual or audio effects that reinforce the liminal nature
        // of the environment
    }

    public void TriggerLiminalEvent(string eventId)
    {
        // Handle specific liminal events triggered by player actions
        GD.Print($"Liminal event triggered: {eventId}");
    }
    
    /// <summary>
    /// Initialize NPC characters in the township
    /// </summary>
    private void InitializeNpcs()
    {
        // In a real implementation, this would:
        // 1. Instantiate NPC game objects at predetermined positions
        // 2. Load their dialogue data
        // 3. Set up interaction triggers
        // 4. Configure their visual representations
        
        GD.Print("Initializing township NPCs...");
        
        // Create sample NPCs for testing
        CreateSampleNpc("shopkeeper", new Vector2(100, 150));
        CreateSampleNpc("innkeeper", new Vector2(200, 100));
        CreateSampleNpc("strange_old_man", new Vector2(300, 200));
    }
    
    /// <summary>
    /// Create a sample NPC for testing purposes
    /// </summary>
    /// <param name="npcId">The NPC identifier</param>
    /// <param name="position">The position to place the NPC</param>
    private void CreateSampleNpc(string npcId, Vector2 position)
    {
        var npc = new TownshipNpc
        {
            NpcId = npcId,
            Name = GetNpcNameFromId(npcId),
            CharacterType = GetNpcTypeFromId(npcId),
            HasLiminalAwareness = npcId.Contains("strange") || npcId.Contains("old"),
            Position = position
        };
        
        // Load dialogue data for this NPC
        _ = LoadNpcDialogue(npcId, npc);
        
        // Add to scene (in a real implementation, this would be added to the proper node)
        GD.Print($"Created NPC: {npc.Name} at position {position}");
    }
    
    /// <summary>
    /// Load dialogue data for an NPC
    /// </summary>
    /// <param name="npcId">The NPC identifier</param>
    /// <param name="npc">The NPC object</param>
    private async Task LoadNpcDialogue(string npcId, TownshipNpc npc)
    {
        var dialogueData = await _dialogueManager.GetNpcDialogueAsync(npcId);
        npc.DialogueData = dialogueData;
    }
    
    /// <summary>
    /// Derives an NPC name from their ID
    /// </summary>
    /// <param name="npcId">The NPC identifier</param>
    /// <returns>The NPC's display name</returns>
    private string GetNpcNameFromId(string npcId)
    {
        return npcId switch
        {
            "shopkeeper" => "Merchantsen the Shopkeep",
            "innkeeper" => "Restwell the Innkeeper",
            "strange_old_man" => "Whisperton the Mysterious",
            _ => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(npcId.Replace("_", " "))
        };
    }
    
    /// <summary>
    /// Derives an NPC type from their ID
    /// </summary>
    /// <param name="npcId">The NPC identifier</param>
    /// <returns>The NPC's character type</returns>
    private string GetNpcTypeFromId(string npcId)
    {
        return npcId switch
        {
            "shopkeeper" => "merchant",
            "innkeeper" => "hospitality worker",
            "strange_old_man" => "mysterious figure",
            _ => "town resident"
        };
    }
    
    /// <summary>
    /// Handle player interaction with an NPC
    /// </summary>
    /// <param name="npcId">The NPC identifier</param>
    public async void InteractWithNpc(string npcId)
    {
        GD.Print($"Player interacting with NPC: {npcId}");
        await _dialogueManager.StartDialogueAsync(npcId);
    }
}