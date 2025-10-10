using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts;

public partial class DreamweaverChoice : ChoiceOption
{
    public new string Id { get; set; }
    public new string Text { get; set; }
    public string Description { get; set; }
    public DreamweaverThread Thread { get; set; }
    public Dictionary<DreamweaverType, int> AlignmentBonus { get; set; } = new();
}