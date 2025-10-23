using Godot;
using System.Collections.Generic;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Narrative.Dialogue;

namespace OmegaSpiral.Source.Scripts.Stages.Stage4;

/// <summary>
/// Controls the appearance and behavior of Dreamweaver presences in Stage 4
/// </summary>
[GlobalClass]
public partial class DreamweaverPresenceController : Node2D
{
    private Dictionary<string, DreamweaverPresence> _presences = new();
    private DreamweaverSystem? _dreamweaverSystem;
    private DreamweaverDialogueManager _dialogueManager = new();

    // Key locations where Dreamweavers appear
    private readonly Dictionary<string, Vector2> _dwLocations = new()
    {
        {"town_center", new Vector2(200, 200)},
        {"inn", new Vector2(300, 150)},
        {"shop", new Vector2(150, 250)},
        {"town_edge", new Vector2(400, 300)}
    };

    public override void _Ready()
    {
        InitializeDreamweaverSystem();
        SpawnDreamweaverPresences();
        StartPresenceMonitoring();
    }

    private void InitializeDreamweaverSystem()
    {
        _dreamweaverSystem = GetNodeOrNull<DreamweaverSystem>("/root/DreamweaverSystem");
        if (_dreamweaverSystem == null)
        {
            GD.PrintErr("DreamweaverSystem not found, Dreamweaver presences may not function correctly");
        }
    }

    private void SpawnDreamweaverPresences()
    {
        // Create translucent Dreamweaver avatars at key locations
        foreach (var location in _dwLocations)
        {
            var presence = CreateDreamweaverPresence(location.Key, location.Value);
            if (presence != null)
            {
                _presences[location.Key] = presence;
                AddChild(presence);
            }
        }
    }

    private DreamweaverPresence? CreateDreamweaverPresence(string locationId, Vector2 position)
    {
        // In a real implementation, this would instantiate actual Dreamweaver avatar scenes
        // For now, we'll create a placeholder that handles the logic
        var presence = new DreamweaverPresence
        {
            Position = position,
            LocationId = locationId
        };

        // Set up the presence based on the location
        ConfigurePresenceForLocation(presence, locationId);

        return presence;
    }

    private void ConfigurePresenceForLocation(DreamweaverPresence presence, string locationId)
    {
        // Different locations trigger different types of Dreamweaver appearances
        presence.AppearanceType = locationId switch
        {
            "town_center" => DreamweaverAppearanceType.Interactive,
            "inn" => DreamweaverAppearanceType.Guidance,
            "shop" => DreamweaverAppearanceType.Persuasion,
            "town_edge" => DreamweaverAppearanceType.Urgent,
            _ => DreamweaverAppearanceType.Interactive
        };

        presence.Visible = false; // Start invisible, will appear when appropriate
        presence.ZIndex = 100; // Ensure they render above other elements
    }

    private void StartPresenceMonitoring()
    {
        // Monitor for conditions that should trigger Dreamweaver appearances:
        // - Player approaching key locations
        // - Time elapsed in area
        // - Player actions (talking to NPCs, reading signs, etc.)

        var checkTimer = GetTree().CreateTimer(3.0f, false); // Check every 3 seconds
        checkTimer.Timeout += CheckForPresenceTriggers;
    }

    private void CheckForPresenceTriggers()
    {
        // In a real implementation, this would check player position
        // and other conditions to determine if any Dreamweaver should appear
        foreach (var presence in _presences.Values)
        {
            if (ShouldAppear(presence))
            {
                AppearDreamweaver(presence);
            }
        }
    }

    private bool ShouldAppear(DreamweaverPresence presence)
    {
        // Complex logic to determine if this Dreamweaver presence should appear
        // based on:
        // - Player proximity to the location
        // - Game state (what choices have been made in previous stages)
        // - Time spent in the area
        // - Player's current alignment/affinity values
        // - Whether they've already encountered this particular presence

        // For now, simple implementation - appear after a period of time
        return presence.AppearanceTimer <= 0;
    }

    private async void AppearDreamweaver(DreamweaverPresence presence)
    {
        // Make the Dreamweaver appear with visual effects
        presence.Visible = true;
        presence.Modulate = new Color(1, 1, 1, 0.7f); // Semi-transparent

        // Play appearance animation
        await ToSignal(GetTree().CreateTimer(0.5), SceneTreeTimer.SignalName.Timeout);

        // Show their message or engage in dialogue
        await ShowDreamweaverMessage(presence);

        // After showing their message, they might linger or fade away
        await HandlePostAppearance(presence);
    }

    private async Task ShowDreamweaverMessage(DreamweaverPresence presence)
    {
        // Get appropriate dialogue based on the presence type and game state
        var dwType = GetAppropriateDreamweaverType(presence);
        var dialogueId = GetDialogueIdForTypeAndLocation(dwType, presence.LocationId);
var dialogue = await _dialogueManager.GetDreamweaverDialogueAsync(dialogueId).ConfigureAwait(false);


        // In a real implementation, this would display the dialogue
        // through the dialogue system
        var openingLine = dialogue.OpeningLines.Count > 0 ? dialogue.OpeningLines[0] : "Hello.";
        GD.Print($"Dreamweaver {dwType} says: {openingLine}");

        // Trigger any special mechanics related to the Dreamweaver's appearance
        TriggerDreamweaverMechanics(presence, dialogue);
    }

    private DreamweaverType GetAppropriateDreamweaverType(DreamweaverPresence presence)
    {
        // Choose which Dreamweaver to show based on:
        // - Game state (which path was taken in Stage 2, etc.)
        // - Which Dreamweavers have already appeared
        // - The player's current alignment

        // For now, rotate through available types
        return presence.AppearanceType switch
        {
            DreamweaverAppearanceType.Urgent => DreamweaverType.Wrath,
            DreamweaverAppearanceType.Persuasion => DreamweaverType.Mischief,
            _ => DreamweaverType.Light
        };
    }

    private string GetDialogueIdForTypeAndLocation(DreamweaverType type, string locationId)
    {
        var typeStr = type switch
        {
            DreamweaverType.Light => "light",
            DreamweaverType.Mischief => "mischief",
            DreamweaverType.Wrath => "wrath",
            _ => "light"
        };

        return $"{typeStr}_{locationId}";
    }

    private void TriggerDreamweaverMechanics(DreamweaverPresence presence, DreamweaverDialogueData dialogue)
    {
        // Apply any affinity adjustments or other game state changes
        if (_dreamweaverSystem != null)
        {
            // In a real implementation, this would affect player affinity values
            GD.Print($"Applied Dreamweaver mechanics for {presence.LocationId}");
        }
    }

    private async Task HandlePostAppearance(DreamweaverPresence presence)
    {
        // Wait for interaction or timeout
        await ToSignal(GetTree().CreateTimer(10.0), SceneTreeTimer.SignalName.Timeout);

        // Fade out gradually
        var fadeTimer = 0.0;
        const double fadeDuration = 2.0;

        while (fadeTimer < fadeDuration)
        {
            fadeTimer += GetProcessDeltaTime();
            var alpha = 0.7f * (1.0f - (float)(fadeTimer / fadeDuration));
            presence.Modulate = new Color(1, 1, 1, alpha);

            await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout);
        }

        presence.Visible = false;
    }
}

/// <summary>
/// Represents a single Dreamweaver presence in the world
/// </summary>
public partial class DreamweaverPresence : Node2D
{
    public string LocationId { get; set; } = string.Empty;
    public DreamweaverAppearanceType AppearanceType { get; set; } = DreamweaverAppearanceType.Interactive;
    public double AppearanceTimer { get; set; } = 10.0; // Seconds until this presence considers appearing

    public override void _Process(double delta)
    {
        // Update appearance timer
        AppearanceTimer -= delta;
    }
}

/// <summary>
/// Types of Dreamweaver appearances based on context
/// </summary>
public enum DreamweaverAppearanceType
{
    Interactive,    // Approaches player for conversation
    Guidance,       // Provides helpful guidance
    Persuasion,     // Attempts to influence player's path
    Urgent          // Appears due to urgent situation
}
