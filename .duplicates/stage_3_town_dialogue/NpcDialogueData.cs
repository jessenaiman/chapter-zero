using System.Collections.Generic;
using OmegaSpiral.Source.Scripts.Common.Dialogue;

namespace OmegaSpiral.Source.Scripts.Stages.Stage4.Dialogue;

/// <summary>
/// Dialogue data for township NPCs that extends base dialogue with liminal elements
/// </summary>
public class NpcDialogueData : BaseDialogueData
{
    /// <summary>
    /// Gets or sets the NPC's unique identifier
    /// </summary>
    public string NpcId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the NPC's display name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the NPC's character type (shopkeeper, townsfolk, etc.)
    /// </summary>
    public string CharacterType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this dialogue should appear "liminal" - suggesting the simulation nature
    /// </summary>
    public bool IsLiminal { get; set; }

    /// <summary>
    /// Gets or sets special dialogue references to other players or loops
    /// </summary>
    public IReadOnlyCollection<string> LoopReferences { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the mood or personality traits of this NPC
    /// </summary>
    public IReadOnlyCollection<string> PersonalityTraits { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets special responses that hint at the simulation nature
    /// </summary>
    public IReadOnlyCollection<string> LiminalResponses { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets dialogue that references previous iterations of the town
    /// </summary>
    public IReadOnlyCollection<string> MemoryFragments { get; set; } = new List<string>();

    public NpcDialogueData() : base()
    {
    }

    public NpcDialogueData(string npcId, string name, string characterType,
                          IReadOnlyCollection<string> openingLines, IReadOnlyCollection<string> dialogueLines,
                          IReadOnlyCollection<IDialogueChoice> choices, IReadOnlyCollection<INarrativeBlock> narrativeBlocks)
        : base(new List<string>(openingLines), new List<string>(dialogueLines), new List<IDialogueChoice>(choices), new List<INarrativeBlock>(narrativeBlocks))
    {
        NpcId = npcId;
        Name = name;
        CharacterType = characterType;
    }
}

/// <summary>
/// Represents a township NPC dialogue choice with additional properties
/// </summary>
public class TownshipDialogueChoice : BaseDialogueChoice
{
    /// <summary>
    /// Gets or sets whether this choice leads to liminal awareness dialogue
    /// </summary>
    public bool LeadsToLiminalAwareness { get; set; }

    /// <summary>
    /// Gets or sets the affinity changes that result from this choice
    /// </summary>
    public Dictionary<string, int> AffinityChanges { get; set; } = new();

    /// <summary>
    /// Gets or sets special flags that this choice might set
    /// </summary>
    public IReadOnlyCollection<string> FlagsSet { get; set; } = new List<string>();

    public TownshipDialogueChoice() : base()
    {
    }

    public TownshipDialogueChoice(string id, string text, string description, int nextDialogueIndex)
        : base(id, text, description, nextDialogueIndex)
    {
    }
}

/// <summary>
/// Represents a township narrative block with liminal elements
/// </summary>
public class TownshipNarrativeBlock : BaseNarrativeBlock
{
    /// <summary>
    /// Gets or sets whether this block contains liminal awareness content
    /// </summary>
    public bool ContainsLiminalContent { get; set; }

    /// <summary>
    /// Gets or sets references to previous iterations or loops
    /// </summary>
    public IReadOnlyCollection<string> TemporalReferences { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets special atmospheric effects to play during this block
    /// </summary>
    public string AtmosphericEffect { get; set; } = string.Empty;

    public TownshipNarrativeBlock() : base()
    {
    }

    public TownshipNarrativeBlock(IReadOnlyCollection<string> paragraphs, string question, IReadOnlyCollection<IDialogueChoice> choices, int nextBlock)
        : base(new List<string>(paragraphs), question, new List<IDialogueChoice>(choices), nextBlock)
    {
    }
}
