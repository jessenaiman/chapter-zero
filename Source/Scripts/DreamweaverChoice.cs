using System;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts;

public partial class DreamweaverChoice
{
    public string Id { get; set; }
    public string Text { get; set; }
    public string Description { get; set; }
    public DreamweaverThread Thread { get; set; }
    public Dictionary<DreamweaverType, int> AlignmentBonus { get; set; } = new();
}