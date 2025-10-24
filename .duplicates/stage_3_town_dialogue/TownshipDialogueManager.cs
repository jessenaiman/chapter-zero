using System.Linq;
using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

using OmegaSpiral.Source.Scripts.Common.Dialogue;

namespace OmegaSpiral.Source.Scripts.Stages.Stage4.Dialogue;

/// <summary>
/// Manager for township NPC dialogue that extends the base dialogue manager
/// </summary>
public partial class TownshipDialogueManager : BaseDialogueManager
{
    private readonly Dictionary<string, NpcDialogueData> _npcDialogues = new();

    /// <summary>
    /// Loads NPC dialogue data from a JSON resource
    /// </summary>
    /// <param name="npcId">The NPC identifier</param>
    /// <param name="resourcePath">Path to the dialogue JSON file</param>
    /// <returns>The loaded NPC dialogue data</returns>
    public async Task<NpcDialogueData> LoadNpcDialogueAsync(string npcId, string resourcePath)
    {
        GD.Print($"Loading dialogue for NPC: {npcId} from {resourcePath}");

        // In a real implementation, this would:
        // 1. Load the JSON file
        // 2. Parse it into NpcDialogueData structure
        // 3. Validate the data
        // 4. Cache it for future use

        // For now, create a placeholder with some sample data
        var dialogueData = CreateSampleNpcDialogue(npcId);
        _npcDialogues[npcId] = dialogueData;

        return dialogueData;
    }

    /// <summary>
    /// Gets cached NPC dialogue data, loading it if necessary
    /// </summary>
    /// <param name="npcId">The NPC identifier</param>
    /// <returns>The NPC dialogue data</returns>
    public async Task<NpcDialogueData> GetNpcDialogueAsync(string npcId)
    {
        if (_npcDialogues.ContainsKey(npcId))
        {
            return _npcDialogues[npcId];
        }

        // In a real implementation, this would load from file
        // For now, create sample data
        var dialogueData = CreateSampleNpcDialogue(npcId);
        _npcDialogues[npcId] = dialogueData;

        return dialogueData;
    }

    /// <summary>
    /// Creates sample dialogue data for an NPC (placeholder for actual JSON loading)
    /// </summary>
    /// <param name="npcId">The NPC identifier</param>
    /// <returns>Sample NPC dialogue data</returns>
    private NpcDialogueData CreateSampleNpcDialogue(string npcId)
    {
        var name = GetNpcNameFromId(npcId);
        var type = GetNpcTypeFromId(npcId);
        var openingLines = CreateOpeningLines(name, type);
        var dialogueLines = CreateDialogueLines();
        var choices = CreateDialogueChoices();
        var loopReferences = CreateLoopReferences();
        var liminalResponses = CreateLiminalResponses();
        var memoryFragments = CreateMemoryFragments();

        return new NpcDialogueData
        {
            NpcId = npcId,
            Name = name,
            CharacterType = type,
            IsLiminal = npcId.Contains("liminal") || npcId.Contains("strange"),
            OpeningLines = openingLines,
            DialogueLines = dialogueLines,
            Choices = choices,
            LoopReferences = loopReferences,
            PersonalityTraits = new List<string> { "observant", "thoughtful" },
            LiminalResponses = liminalResponses,
            MemoryFragments = memoryFragments
        };
    }

    private List<string> CreateOpeningLines(string name, string type)
    {
        return new List<string>
        {
            $"Hello, traveler. Welcome to our humble town.",
            $"I am {name}, a simple {type} here."
        };
    }

    private List<string> CreateDialogueLines()
    {
        return new List<string>
        {
            "The weather has been quite peculiar lately, hasn't it?",
            "Sometimes I feel like I've seen this day before...",
            "There's something odd about this place, but I can't quite put my finger on it."
        };
    }

    private List<IDialogueChoice> CreateDialogueChoices()
    {
        return new List<IDialogueChoice>
        {
            new TownshipDialogueChoice("ask_about_town", "Tell me about this town", "Ask about the town's history", 0)
            {
                LeadsToLiminalAwareness = true,
                AffinityChanges = new Dictionary<string, int> { { "curiosity", 1 } }
            },
            new TownshipDialogueChoice("ask_about_weather", "Why is the weather so strange?", "Ask about the unusual weather", 1)
            {
                LeadsToLiminalAwareness = true,
                AffinityChanges = new Dictionary<string, int> { { "observation", 1 } }
            },
            new TownshipDialogueChoice("farewell", "I must be going", "Say goodbye", 2)
        };
    }

    private List<string> CreateLoopReferences()
    {
        return new List<string>
        {
            "I swear I've seen someone like you before...",
            "This town feels familiar, yet different somehow."
        };
    }

    private List<string> CreateLiminalResponses()
    {
        return new List<string>
        {
            "Have you noticed how the footprints seem to multiply?",
            "Sometimes I wonder if we're all just echoes..."
        };
    }

    private List<string> CreateMemoryFragments()
    {
        return new List<string>
        {
            "I remember a time when the banners weren't quite so faded.",
            "There was another traveler here once, asking the same questions."
        };
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
            "shopkeeper" => "Merchantsen",
            "innkeeper" => "Restwell",
            "smith" => "Ironforge",
            "townsfolk_elite" => "Highworth",
            "townsfolk_common" => "Simpleton",
            "strange_old_man" => "Whisperton",
            "child" => "Little Timmy",
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
            "smith" => "blacksmith",
            "townsfolk_elite" => "town elite",
            "townsfolk_common" => "common townsfolk",
            "strange_old_man" => "mysterious figure",
            "child" => "local child",
            _ => "town resident"
        };
    }

    /// <summary>
    /// Starts a dialogue sequence with an NPC
    /// </summary>
    /// <param name="npcId">The NPC identifier</param>
    /// <returns>A task that completes when dialogue is finished</returns>
    public async Task StartDialogueAsync(string npcId)
    {
        GD.Print($"Starting dialogue with NPC: {npcId}");

        var dialogueData = await GetNpcDialogueAsync(npcId).ConfigureAwait(false);

        // In a real implementation, this would:
        // 1. Display the dialogue Ui
        // 2. Show the NPC's opening lines
        // 3. Present choices
        // 4. Handle player responses
        // 5. Apply game state changes

        DisplayDialogue(dialogueData);
    }

    /// <summary>
    /// Displays dialogue data to the player
    /// </summary>
    /// <param name="dialogueData">The dialogue data to display</param>
    private void DisplayDialogue(NpcDialogueData dialogueData)
    {
        GD.Print($"[{dialogueData.Name}]: {string.Join("\n[" + dialogueData.Name + "]: ", dialogueData.OpeningLines)}");

        if (dialogueData.IsLiminal)
        {
            var idx = new System.Random().Next(dialogueData.LiminalResponses.Count);
            GD.Print($"[Liminal Hint]: {dialogueData.LiminalResponses.ElementAt(idx)}");
        }

        // Display choices
        GD.Print("What would you like to say?");
        for (int i = 0; i < dialogueData.Choices.Count; i++)
        {
            var choice = dialogueData.Choices[i];
            GD.Print($"{i + 1}. {choice.Text} - {choice.Description}");
        }
    }
}
