using Godot;
using Godot.Collections;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Scripts.Common.Dialogue;
using System.Collections.Generic;

namespace OmegaSpiral.Source.Scripts.Dialogue;

/// <summary>
/// Base implementation of dialogue data structure
/// </summary>
public class BaseDialogueData : IDialogueData
{
    public List<string> OpeningLines { get; set; } = new();
    public List<string> DialogueLines { get; set; } = new();
    public List<IDialogueChoice> Choices { get; set; } = new();
    public List<INarrativeBlock> NarrativeBlocks { get; set; } = new();

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
    public virtual IDialogueData ParseDialogueData(Godot.Collections.Dictionary<string, Variant> jsonData)
    {
        var dialogueData = new BaseDialogueData();

        ParseOpeningLines(jsonData, dialogueData);
        ParseDialogueLines(jsonData, dialogueData);
        ParseChoices(jsonData, dialogueData);
        ParseNarrativeBlocks(jsonData, dialogueData);

        return dialogueData;
    }

    /// <summary>
    /// Parses opening lines from JSON data
    /// </summary>
    protected virtual void ParseOpeningLines(Godot.Collections.Dictionary<string, Variant> jsonData, BaseDialogueData dialogueData)
    {
        if (jsonData.ContainsKey("openingLines") && jsonData["openingLines"].VariantType == Variant.Type.Array)
        {
            var openingLinesArray = jsonData["openingLines"].AsGodotArray();
            foreach (var line in openingLinesArray)
            {
                dialogueData.OpeningLines.Add(line.AsString());
            }
        }
    }

    /// <summary>
    /// Parses dialogue lines from JSON data
    /// </summary>
    protected virtual void ParseDialogueLines(Godot.Collections.Dictionary<string, Variant> jsonData, BaseDialogueData dialogueData)
    {
        if (jsonData.ContainsKey("dialogueLines") && jsonData["dialogueLines"].VariantType == Variant.Type.Array)
        {
            var dialogueLinesArray = jsonData["dialogueLines"].AsGodotArray();
            foreach (var line in dialogueLinesArray)
            {
                dialogueData.DialogueLines.Add(line.AsString());
            }
        }
    }

    /// <summary>
    /// Parses player choices from JSON data
    /// </summary>
    protected virtual void ParseChoices(Godot.Collections.Dictionary<string, Variant> jsonData, BaseDialogueData dialogueData)
    {
        if (jsonData.ContainsKey("choices") && jsonData["choices"].VariantType == Variant.Type.Array)
        {
            var choicesArray = jsonData["choices"].AsGodotArray();
            foreach (var choiceVariant in choicesArray)
            {
                if (choiceVariant.VariantType == Variant.Type.Dictionary)
                {
                    var choice = ParseChoice(choiceVariant.AsGodotDictionary());
                    dialogueData.Choices.Add(choice);
                }
            }
        }
    }

    /// <summary>
    /// Parses a single dialogue choice from a dictionary
    /// </summary>
    protected virtual IDialogueChoice ParseChoice(Godot.Collections.Dictionary choiceDict)
    {
        return new BaseDialogueChoice
        {
            Id = choiceDict.ContainsKey("id") ? choiceDict["id"].AsString() : string.Empty,
            Text = choiceDict.ContainsKey("text") ? choiceDict["text"].AsString() : string.Empty,
            Description = choiceDict.ContainsKey("description") ? choiceDict["description"].AsString() : string.Empty,
            NextDialogueIndex = choiceDict.ContainsKey("nextDialogueIndex") ? choiceDict["nextDialogueIndex"].AsInt32() : 0
        };
    }

    /// <summary>
    /// Parses narrative blocks from JSON data
    /// </summary>
    protected virtual void ParseNarrativeBlocks(Godot.Collections.Dictionary<string, Variant> jsonData, BaseDialogueData dialogueData)
    {
        if (jsonData.ContainsKey("narrativeBlocks") && jsonData["narrativeBlocks"].VariantType == Variant.Type.Array)
        {
            var blocksArray = jsonData["narrativeBlocks"].AsGodotArray();
            foreach (var blockVariant in blocksArray)
            {
                if (blockVariant.VariantType == Variant.Type.Dictionary)
                {
                    var narrativeBlock = ParseNarrativeBlock(blockVariant.AsGodotDictionary());
                    dialogueData.NarrativeBlocks.Add(narrativeBlock);
                }
            }
        }
    }

    /// <summary>
    /// Parses a single narrative block from a dictionary
    /// </summary>
    protected virtual INarrativeBlock ParseNarrativeBlock(Godot.Collections.Dictionary blockDict)
    {
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
                    var choice = ParseChoice(choiceVariant.AsGodotDictionary());
                    narrativeBlock.Choices.Add(choice);
                }
            }
        }

        // Parse next block index
        if (blockDict.ContainsKey("nextBlock"))
        {
            narrativeBlock.NextBlock = blockDict["nextBlock"].AsInt32();
        }

        return narrativeBlock;
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
    private readonly System.Collections.Generic.Dictionary<string, IDialogueData> _dialogueCache = new();
    private readonly System.Collections.Generic.Dictionary<string, string> _dialoguePaths = new();
    protected IDialogueParser _parser = new BaseDialogueParser();

    public async Task<IDialogueData> LoadDialogueAsync(string resourcePath)
    {
        if (!Godot.FileAccess.FileExists(resourcePath))
        {
            return LogErrorAndCreateFallback($"Dialogue resource not found: {resourcePath}");
        }

        try
        {
            var jsonString = ReadJsonFile(resourcePath);
            return string.IsNullOrEmpty(jsonString) ? CreateFallbackDialogue() : ParseJsonToDialogue(resourcePath, jsonString);
        }
        catch (Exception ex)
        {
            return LogErrorAndCreateFallback($"Error loading dialogue from {resourcePath}: {ex.Message}");
        }
    }

    /// <summary>
    /// Logs an error and returns fallback dialogue
    /// </summary>
    private IDialogueData LogErrorAndCreateFallback(string message)
    {
        GD.PrintErr(message);
        return CreateFallbackDialogue();
    }

    /// <summary>
    /// Reads JSON content from a file
    /// </summary>
    private string ReadJsonFile(string resourcePath)
    {
        using var file = Godot.FileAccess.Open(resourcePath, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"Failed to open dialogue file: {resourcePath}");
            return string.Empty;
        }

        return file.GetAsText();
    }

    /// <summary>
    /// Parses JSON string into dialogue data
    /// </summary>
    private IDialogueData ParseJsonToDialogue(string resourcePath, string jsonString)
    {
        var json = new Json();
        var error = json.Parse(jsonString);

        if (error != Error.Ok)
        {
            GD.PrintErr($"Failed to parse JSON from: {resourcePath}");
            return CreateFallbackDialogue();
        }

        if (json.Data.VariantType != Variant.Type.Dictionary)
        {
            GD.PrintErr($"Failed to parse JSON from: {resourcePath}");
            return CreateFallbackDialogue();
        }

        var anyDict = json.Data.AsGodotDictionary();
        var parsedData = new Godot.Collections.Dictionary<string, Variant>();

        foreach (var kvp in anyDict)
        {
            parsedData[kvp.Key.ToString()] = kvp.Value;
        }

        var dialogueData = _parser.ParseDialogueData(parsedData);

        if (!_parser.ValidateDialogueData(dialogueData))
        {
            GD.PrintErr($"Dialogue data validation failed for: {resourcePath}");
            return CreateFallbackDialogue();
        }

        return dialogueData;
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
