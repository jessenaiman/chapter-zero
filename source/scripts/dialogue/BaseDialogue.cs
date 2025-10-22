using Godot;
using Godot.Collections;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Common.Dialogue;

namespace OmegaSpiral.Source.Scripts.Dialogue;

/// <summary>
/// Base implementation of dialogue data structure
/// </summary>
public class BaseDialogueData : IDialogueData
{
    public List<string> OpeningLines { get; protected set; } = new();
    public List<string> DialogueLines { get; protected set; } = new();
    public List<IDialogueChoice> Choices { get; protected set; } = new();
    public List<INarrativeBlock> NarrativeBlocks { get; protected set; } = new();

    /// <summary>
    /// Initializes a new instance of BaseDialogueData with provided values
    /// </summary>
    public BaseDialogueData(List<string> openingLines, List<string> dialogueLines, List<IDialogueChoice> choices, List<INarrativeBlock> narrativeBlocks)
    {
        OpeningLines = openingLines;
        DialogueLines = dialogueLines;
        Choices = choices;
        NarrativeBlocks = narrativeBlocks;
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public BaseDialogueData()
    {
    }
}

/// <summary>
/// Base implementation of dialogue choice
/// </summary>
public class BaseDialogueChoice : IDialogueChoice
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int NextDialogueIndex { get; set; }

    /// <summary>
    /// Initializes a new instance of BaseDialogueChoice with provided values
    /// </summary>
    public BaseDialogueChoice(string id, string text, string description, int nextDialogueIndex)
    {
        Id = id;
        Text = text;
        Description = description;
        NextDialogueIndex = nextDialogueIndex;
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public BaseDialogueChoice()
    {
    }
}

/// <summary>
/// Base implementation of narrative block
/// </summary>
public class BaseNarrativeBlock : INarrativeBlock
{
    public List<string> Paragraphs { get; set; } = new();
    public string Question { get; set; } = string.Empty;
    public List<IDialogueChoice> Choices { get; set; } = new();
    public int NextBlock { get; set; }

    /// <summary>
    /// Initializes a new instance of BaseNarrativeBlock with provided values
    /// </summary>
    public BaseNarrativeBlock(List<string> paragraphs, string question, List<IDialogueChoice> choices, int nextBlock)
    {
        Paragraphs = paragraphs;
        Question = question;
        Choices = choices;
        NextBlock = nextBlock;
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public BaseNarrativeBlock()
    {
    }
}

/// <summary>
/// Base dialogue parser implementation
/// </summary>
public class BaseDialogueParser : IDialogueParser
{
    public virtual IDialogueData ParseDialogueData(Dictionary<string, Variant> jsonData)
    {
        var dialogueData = new BaseDialogueData();
        
        // Parse opening lines
        if (jsonData.ContainsKey("openingLines") && jsonData["openingLines"].VariantType == Variant.Type.Array)
        {
            var openingLinesArray = jsonData["openingLines"].AsGodotArray();
            foreach (var line in openingLinesArray)
            {
                dialogueData.OpeningLines.Add(line.AsString());
            }
        }
        
        // Parse dialogue lines
        if (jsonData.ContainsKey("dialogueLines") && jsonData["dialogueLines"].VariantType == Variant.Type.Array)
        {
            var dialogueLinesArray = jsonData["dialogueLines"].AsGodotArray();
            foreach (var line in dialogueLinesArray)
            {
                dialogueData.DialogueLines.Add(line.AsString());
            }
        }
        
        // Parse choices
        if (jsonData.ContainsKey("choices") && jsonData["choices"].VariantType == Variant.Type.Array)
        {
            var choicesArray = jsonData["choices"].AsGodotArray();
            foreach (var choiceVariant in choicesArray)
            {
                if (choiceVariant.VariantType == Variant.Type.Dictionary)
                {
                    var choiceDict = choiceVariant.AsGodotDictionary();
                    var choice = new BaseDialogueChoice
                    {
                        Id = choiceDict.ContainsKey("id") ? choiceDict["id"].AsString() : string.Empty,
                        Text = choiceDict.ContainsKey("text") ? choiceDict["text"].AsString() : string.Empty,
                        Description = choiceDict.ContainsKey("description") ? choiceDict["description"].AsString() : string.Empty,
                        NextDialogueIndex = choiceDict.ContainsKey("nextDialogueIndex") ? choiceDict["nextDialogueIndex"].AsInt32() : 0
                    };
                    dialogueData.Choices.Add(choice);
                }
            }
        }
        
        // Parse narrative blocks
        if (jsonData.ContainsKey("narrativeBlocks") && jsonData["narrativeBlocks"].VariantType == Variant.Type.Array)
        {
            var blocksArray = jsonData["narrativeBlocks"].AsGodotArray();
            foreach (var blockVariant in blocksArray)
            {
                if (blockVariant.VariantType == Variant.Type.Dictionary)
                {
                    var blockDict = blockVariant.AsGodotDictionary();
                    var narrativeBlock = new BaseNarrativeBlock();
                    
                    // Parse paragraphs
                    if (blockDict.ContainsKey("paragraphs") && blockDict["paragraphs"].VariantType == Variant.Type.Array)
                    {
                        var paragraphsArray = blockDict["paragraphs"].AsGodotArray();
                        foreach (var para in paragraphsArray)
                        {
                            narrativeBlock.Paragraphs.Add(para.AsString());
                        }
                    }
                    
                    // Parse question
                    if (blockDict.ContainsKey("question"))
                    {
                        narrativeBlock.Question = blockDict["question"].AsString();
                    }
                    
                    // Parse choices for this block
                    if (blockDict.ContainsKey("choices") && blockDict["choices"].VariantType == Variant.Type.Array)
                    {
                        var blockChoicesArray = blockDict["choices"].AsGodotArray();
                        foreach (var choiceVariant in blockChoicesArray)
                        {
                            if (choiceVariant.VariantType == Variant.Type.Dictionary)
                            {
                                var choiceDict = choiceVariant.AsGodotDictionary();
                                var choice = new BaseDialogueChoice
                                {
                                    Id = choiceDict.ContainsKey("id") ? choiceDict["id"].AsString() : string.Empty,
                                    Text = choiceDict.ContainsKey("text") ? choiceDict["text"].AsString() : string.Empty,
                                    Description = choiceDict.ContainsKey("description") ? choiceDict["description"].AsString() : string.Empty,
                                    NextDialogueIndex = choiceDict.ContainsKey("nextDialogueIndex") ? choiceDict["nextDialogueIndex"].AsInt32() : 0
                                };
                                narrativeBlock.Choices.Add(choice);
                            }
                        }
                    }
                    
                    // Parse next block index
                    if (blockDict.ContainsKey("nextBlock"))
                    {
                        narrativeBlock.NextBlock = blockDict["nextBlock"].AsInt32();
                    }
                    
                    dialogueData.NarrativeBlocks.Add(narrativeBlock);
                }
            }
        }
        
        return dialogueData;
    }
    
    public virtual bool ValidateDialogueData(IDialogueData dialogueData)
    {
        // Basic validation - all collections should have valid content
        return dialogueData.OpeningLines != null && 
               dialogueData.DialogueLines != null && 
               dialogueData.Choices != null && 
               dialogueData.NarrativeBlocks != null;
    }
}

/// <summary>
/// Base dialogue manager implementation
/// </summary>
public partial class BaseDialogueManager : Node, IDialogueManager
{
    private readonly Dictionary<string, IDialogueData> _dialogueCache = new();
    private readonly Dictionary<string, string> _dialoguePaths = new();
    private IDialogueParser _parser = new BaseDialogueParser();

    public async Task<IDialogueData> LoadDialogueAsync(string resourcePath)
    {
        if (!Godot.FileAccess.FileExists(resourcePath))
        {
            GD.PrintErr($"Dialogue resource not found: {resourcePath}");
            return CreateFallbackDialogue();
        }

        try
        {
            using var file = Godot.FileAccess.Open(resourcePath, Godot.FileAccess.ModeFlags.Read);
            if (file == null)
            {
                GD.PrintErr($"Failed to open dialogue file: {resourcePath}");
                return CreateFallbackDialogue();
            }

            var jsonString = file.GetAsText();
            
            // Use Godot's JSON class to parse the string
            var jsonResult = Json.ParseString(jsonString);
            if (jsonResult is Godot.Collections.Dictionary<string, Variant> parsedData)
            {
                var dialogueData = _parser.ParseDialogueData(parsedData);
                
                if (_parser.ValidateDialogueData(dialogueData))
                {
                    return dialogueData;
                }
                else
                {
                    GD.PrintErr($"Dialogue data validation failed for: {resourcePath}");
                    return CreateFallbackDialogue();
                }
            }
            else
            {
                GD.PrintErr($"Failed to parse JSON from: {resourcePath}");
                return CreateFallbackDialogue();
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error loading dialogue from {resourcePath}: {ex.Message}");
            return CreateFallbackDialogue();
        }
    }

    public async Task<IDialogueData> GetDialogueAsync(string dialogueId)
    {
        if (_dialogueCache.ContainsKey(dialogueId))
        {
            return _dialogueCache[dialogueId];
        }

        if (_dialoguePaths.ContainsKey(dialogueId))
        {
            var dialogueData = await LoadDialogueAsync(_dialoguePaths[dialogueId]);
            _dialogueCache[dialogueId] = dialogueData;
            return dialogueData;
        }
        
        GD.PrintErr($"Dialogue with ID '{dialogueId}' not found and no path registered");
        return CreateFallbackDialogue();
    }

    public void PreloadDialogue(string dialogueId, string resourcePath)
    {
        _dialoguePaths[dialogueId] = resourcePath;
    }

    public void ClearCache(string? dialogueId = null)
    {
        if (string.IsNullOrEmpty(dialogueId))
        {
            _dialogueCache.Clear();
        }
        else
        {
            _dialogueCache.Remove(dialogueId);
        }
    }
    
    private IDialogueData CreateFallbackDialogue()
    {
        return new BaseDialogueData(
            new List<string> { "Dialogue not available" },
            new List<string> { "Default dialogue line" },
            new List<IDialogueChoice>(),
            new List<INarrativeBlock>()
        );
    }
}