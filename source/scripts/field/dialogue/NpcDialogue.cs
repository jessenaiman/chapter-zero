using Godot;
using Godot.Collections;
using OmegaSpiral.Source.Scripts.Common.Dialogue;
using OmegaSpiral.Source.Scripts.Dialogue;

namespace OmegaSpiral.Source.Scripts.Field.Dialogue;

/// <summary>
/// NPC dialogue data that extends base dialogue with NPC-specific properties
/// </summary>
public class NpcDialogueData : BaseDialogueData
{
    /// <summary>
    /// Gets the NPC's character identifier
    /// </summary>
    public string NpcId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets the NPC's name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets the NPC's character type or role
    /// </summary>
    public string CharacterType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets whether this dialogue should appear "liminal" - suggesting the simulation nature
    /// </summary>
    public bool IsLiminal { get; set; }
    
    /// <summary>
    /// Gets special dialogue references to other players or loops
    /// </summary>
    public List<string> LoopReferences { get; set; } = new();

    public NpcDialogueData() : base()
    {
    }
    
    public NpcDialogueData(string npcId, string name, string characterType, 
                          List<string> openingLines, List<string> dialogueLines, 
                          List<IDialogueChoice> choices, List<INarrativeBlock> narrativeBlocks) 
        : base(openingLines, dialogueLines, choices, narrativeBlocks)
    {
        NpcId = npcId;
        Name = name;
        CharacterType = characterType;
    }
}

/// <summary>
/// NPC-specific dialogue parser that handles liminal elements
/// </summary>
public class NpcDialogueParser : BaseDialogueParser
{
    public override IDialogueData ParseDialogueData(Dictionary<string, Variant> jsonData)
    {
        var baseData = (BaseDialogueData)base.ParseDialogueData(jsonData);
        
        var npcData = new NpcDialogueData
        {
            OpeningLines = baseData.OpeningLines,
            DialogueLines = baseData.DialogueLines,
            Choices = baseData.Choices,
            NarrativeBlocks = baseData.NarrativeBlocks
        };
        
        // Parse NPC-specific properties
        if (jsonData.ContainsKey("npcId"))
        {
            npcData.NpcId = jsonData["npcId"].AsString();
        }
        
        if (jsonData.ContainsKey("name"))
        {
            npcData.Name = jsonData["name"].AsString();
        }
        
        if (jsonData.ContainsKey("characterType"))
        {
            npcData.CharacterType = jsonData["characterType"].AsString();
        }
        
        if (jsonData.ContainsKey("isLiminal"))
        {
            npcData.IsLiminal = jsonData["isLiminal"].AsBool();
        }
        
        // Parse loop references - special dialogue elements that hint at the simulation nature
        if (jsonData.ContainsKey("loopReferences") && jsonData["loopReferences"].VariantType == Variant.Type.Array)
        {
            var loopRefsArray = jsonData["loopReferences"].AsGodotArray();
            foreach (var reference in loopRefsArray)
            {
                npcData.LoopReferences.Add(reference.AsString());
            }
        }
        
        return npcData;
    }
    
    public override bool ValidateDialogueData(IDialogueData dialogueData)
    {
        var isValid = base.ValidateDialogueData(dialogueData);
        
        if (dialogueData is NpcDialogueData npcData)
        {
            return isValid && !string.IsNullOrEmpty(npcData.NpcId) && !string.IsNullOrEmpty(npcData.Name);
        }
        
        return isValid;
    }
}

/// <summary>
/// Manager for NPC dialogue in the field scenes
/// </summary>
public partial class NpcDialogueManager : BaseDialogueManager
{
    public NpcDialogueManager()
    {
        // Override the parser to use NPC-specific parsing
        _parser = new NpcDialogueParser();
    }
    
    /// <summary>
    /// Gets NPC-specific dialogue data
    /// </summary>
    public async Task<NpcDialogueData> GetNpcDialogueAsync(string npcId)
    {
        var dialogueData = await GetDialogueAsync(npcId);
        return dialogueData as NpcDialogueData ?? CreateFallbackNpcDialogue(npcId);
    }
    
    /// <summary>
    /// Loads NPC dialogue from a JSON resource
    /// </summary>
    public async Task<NpcDialogueData> LoadNpcDialogueAsync(string resourcePath)
    {
        var dialogueData = await LoadDialogueAsync(resourcePath);
        return dialogueData as NpcDialogueData ?? CreateFallbackNpcDialogue("unknown");
    }
    
    private NpcDialogueData CreateFallbackNpcDialogue(string npcId)
    {
        return new NpcDialogueData
        {
            NpcId = npcId,
            Name = "Unknown NPC",
            CharacterType = "generic",
            OpeningLines = new List<string> { "...", "Hello, traveler." },
            DialogueLines = new List<string> { "I don't have much to say right now." },
            Choices = new List<IDialogueChoice>(),
            NarrativeBlocks = new List<INarrativeBlock>(),
            IsLiminal = false,
            LoopReferences = new List<string>()
        };
    }
}