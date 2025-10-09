using System;
using System.Collections.Generic;

public partial class NarrativeSceneData
{
    public string Type { get; set; } = "narrative_terminal";
    public List<string> OpeningLines { get; set; } = new();
    public NarrativeChoice InitialChoice { get; set; }
    public List<StoryBlock> StoryBlocks { get; set; } = new();
    public string NamePrompt { get; set; }
    public SecretQuestion SecretQuestion { get; set; }
    public string ExitLine { get; set; }
}

public partial class NarrativeChoice
{
    public string Prompt { get; set; }
    public List<DreamweaverChoice> Options { get; set; } = new();
}

public partial class StoryBlock
{
    public List<string> Paragraphs { get; set; } = new();
    public string Question { get; set; }
    public List<ChoiceOption> Choices { get; set; } = new();
}

public partial class ChoiceOption
{
    public string Text { get; set; }
    public int NextBlock { get; set; }
}

public partial class SecretQuestion
{
    public string Prompt { get; set; }
    public List<string> Options { get; set; } = new();
}