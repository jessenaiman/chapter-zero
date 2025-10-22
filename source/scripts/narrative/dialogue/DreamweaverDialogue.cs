using Godot;
using Godot.Collections;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Common.Dialogue;
using OmegaSpiral.Source.Scripts.Dialogue;
using System.Collections.Generic;

namespace OmegaSpiral.Source.Scripts.Narrative.Dialogue;

/// <summary>
/// Dreamweaver dialogue data that extends base dialogue with Dreamweaver-specific properties
/// </summary>
public class DreamweaverDialogueData : BaseDialogueData
{
    /// <summary>
    /// Gets the Dreamweaver's identifier (light, mischief, wrath)
    /// </summary>
    public string DreamweaverId { get; set; } = string.Empty;

    /// <summary>
    /// Gets the Dreamweaver's type
    /// </summary>
    public DreamweaverType Type { get; set; }

    /// <summary>
    /// Gets whether this is a major narrative beat (affects player alignment)
    /// </summary>
    public bool IsMajorBeat { get; set; }

    /// <summary>
    /// Gets affinity adjustments based on player choices
    /// </summary>
    public System.Collections.Generic.Dictionary<string, int> AffinityAdjustments { get; set; } = new();

    /// <summary>
    /// Gets the priority level of this dialogue (higher priority takes precedence)
    /// </summary>
    public int Priority { get; set; }

    public DreamweaverDialogueData() : base()
    {
    }

    public DreamweaverDialogueData(string dreamweaverId, DreamweaverType type,
                          List<string> openingLines, List<string> dialogueLines,
                          List<IDialogueChoice> choices, List<INarrativeBlock> narrativeBlocks)
        : base(openingLines, dialogueLines, choices, narrativeBlocks)
    {
        DreamweaverId = dreamweaverId;
        Type = type;
    }
}

/// <summary>
/// Dreamweaver-specific dialogue parser that handles persona-specific properties
/// </summary>
public class DreamweaverDialogueParser : BaseDialogueParser
{
    public override IDialogueData ParseDialogueData(Godot.Collections.Dictionary<string, Variant> jsonData)
    {
        var baseData = (BaseDialogueData)base.ParseDialogueData(jsonData);

        var dwData = new DreamweaverDialogueData
        {
            OpeningLines = baseData.OpeningLines,
            DialogueLines = baseData.DialogueLines,
            Choices = baseData.Choices,
            NarrativeBlocks = baseData.NarrativeBlocks
        };

        // Parse Dreamweaver-specific properties
        if (jsonData.ContainsKey("dreamweaverId"))
        {
            dwData.DreamweaverId = jsonData["dreamweaverId"].AsString();
            // Map string to enum
            dwData.Type = dwData.DreamweaverId.ToLower() switch
            {
                "light" or "hero" => DreamweaverType.Light,
                "mischief" or "shadow" => DreamweaverType.Mischief,
                "wrath" or "ambition" => DreamweaverType.Wrath,
                _ => DreamweaverType.Light // Default fallback
            };
        }

        if (jsonData.ContainsKey("isMajorBeat"))
        {
            dwData.IsMajorBeat = jsonData["isMajorBeat"].AsBool();
        }

        if (jsonData.ContainsKey("priority"))
        {
            dwData.Priority = jsonData["priority"].AsInt32();
        }

        // Parse affinity adjustments
        if (jsonData.ContainsKey("affinityAdjustments") && jsonData["affinityAdjustments"].VariantType == Variant.Type.Dictionary)
        {
            var adjustmentsDict = jsonData["affinityAdjustments"].AsGodotDictionary();
            foreach (var kvp in adjustmentsDict)
            {
                dwData.AffinityAdjustments[kvp.Key.ToString()] = kvp.Value.AsInt32();
            }
        }

        return dwData;
    }

    public override bool ValidateDialogueData(IDialogueData dialogueData)
    {
        var isValid = base.ValidateDialogueData(dialogueData);

        if (dialogueData is DreamweaverDialogueData dwData)
        {
            return isValid && !string.IsNullOrEmpty(dwData.DreamweaverId);
        }

        return isValid;
    }
}

/// <summary>
/// Manager for Dreamweaver dialogue that integrates with existing Dreamweaver system
/// </summary>
public partial class DreamweaverDialogueManager : BaseDialogueManager
{
    public DreamweaverDialogueManager()
    {
        // Override the parser to use Dreamweaver-specific parsing
        _parser = new DreamweaverDialogueParser();
    }

    /// <summary>
    /// Gets Dreamweaver-specific dialogue data
    /// </summary>
    public async Task<DreamweaverDialogueData> GetDreamweaverDialogueAsync(string dwId)
    {
        var dialogueData = await GetDialogueAsync(dwId);
        return dialogueData as DreamweaverDialogueData ?? CreateFallbackDreamweaverDialogue(dwId);
    }

    /// <summary>
    /// Loads Dreamweaver dialogue from a JSON resource
    /// </summary>
    public async Task<DreamweaverDialogueData> LoadDreamweaverDialogueAsync(string resourcePath)
    {
        var dialogueData = await LoadDialogueAsync(resourcePath);
        return dialogueData as DreamweaverDialogueData ?? CreateFallbackDreamweaverDialogue("unknown");
    }

    private DreamweaverDialogueData CreateFallbackDreamweaverDialogue(string dwId)
    {
        return new DreamweaverDialogueData
        {
            DreamweaverId = dwId,
            Type = dwId.ToLower() switch
            {
                "light" or "hero" => DreamweaverType.Light,
                "mischief" or "shadow" => DreamweaverType.Mischief,
                "wrath" or "ambition" => DreamweaverType.Wrath,
                _ => DreamweaverType.Light
            },
            OpeningLines = new List<string> { "A familiar voice whispers...", "The echo calls to you." },
            DialogueLines = new List<string> { "You feel a presence near you." },
            Choices = new List<IDialogueChoice>(),
            NarrativeBlocks = new List<INarrativeBlock>(),
            IsMajorBeat = false,
            Priority = 0,
            AffinityAdjustments = new System.Collections.Generic.Dictionary<string, int>()
        };
    }
}
