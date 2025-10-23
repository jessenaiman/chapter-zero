// <copyright file="BeatChamberExploration.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Infrastructure;
using OmegaSpiral.Source.Scripts.Stages.Stage2;

namespace OmegaSpiral.Source.Stages.Stage2.Beats;

/// <summary>
/// Beat: Chamber Exploration base scene handler.
/// Loads chamber data from stage_2.json and displays the dungeon layout with set pieces.
/// Player can interact with door, monster, and chest objects.
/// Subclasses (BeatChamberLight, BeatChamberShadow, BeatChamberAmbition) specify which chamber to display.
/// </summary>
[GlobalClass]
public partial class BeatChamberExploration : BeatSceneBase
{
    private const string NarrativeJsonPath = "res://source/stages/stage_2/stage_2.json";

#pragma warning disable CA2213
    private VBoxContainer? _contentContainer;
#pragma warning restore CA2213
    private Stage2NarrativeData? _narrativeData;
    private string _chamberOwner = "light"; // Override in subclasses

    /// <inheritdoc/>
    protected override string CurrentBeatId => $"beat_chamber_{_chamberOwner}";

    /// <inheritdoc/>
    protected override string StageManifestPath => "res://source/stages/stage_2/stage_2_manifest.json";

    /// <summary>
    /// Sets which chamber to display (by owner: "light", "shadow", or "ambition").
    /// </summary>
    public void SetChamberOwner(string owner)
    {
        if (owner != "light" && owner != "shadow" && owner != "ambition")
        {
            GD.PrintErr($"[BeatChamberExploration] Invalid chamber owner: {owner}");
            return;
        }

        _chamberOwner = owner;
    }

    /// <inheritdoc/>
    public override void _Ready()
    {
        base._Ready();

        GD.Print($"[BeatChamberExploration] Chamber exploration started for {_chamberOwner}");

        // Load narrative data from JSON
        var loader = new NarrativeDataLoader();
        _narrativeData = loader.LoadNarrativeData<Stage2NarrativeData>(NarrativeJsonPath);

        if (_narrativeData == null)
        {
            GD.PrintErr("[BeatChamberExploration] Failed to load narrative data");
            return;
        }

        // Find the chamber owned by the specified Dreamweaver
        var chamber = _narrativeData.Chambers.Find(c => c.Owner == _chamberOwner);
        if (chamber == null)
        {
            GD.PrintErr($"[BeatChamberExploration] Chamber owned by '{_chamberOwner}' not found");
            return;
        }

        // Find or create content container
        _contentContainer = GetNodeOrNull<VBoxContainer>("ContentMargin/ContentVBox");

        if (_contentContainer == null)
        {
            GD.PrintErr("[BeatChamberExploration] ContentVBox not found in scene");
            return;
        }

        // Dynamically create UI from chamber data
        RenderChamber(chamber);
    }

    /// <summary>
    /// Renders the chamber content from JSON data.
    /// </summary>
    private void RenderChamber(ChamberData chamber)
    {
        if (_contentContainer == null || _narrativeData == null)
        {
            return;
        }

        // Show the Dreamweaver who owns this chamber
        var dreamer = _narrativeData.Dreamweavers.Find(d => d.Id == chamber.Owner);
        if (dreamer != null)
        {
            AddTextLabel(_contentContainer, $"[{dreamer.Name.ToUpper()} CHAMBER]", 16);
        }

        // Show chamber style/theme
        AddTextLabel(_contentContainer, $"Template: {chamber.Style.Template}", 12);

        // Display each set piece (door, monster, chest)
        AddTextLabel(_contentContainer, "=== SET PIECES ===", 14);

        foreach (var obj in chamber.Objects)
        {
            AddTextLabel(_contentContainer, $"\n[{obj.Slot.ToUpper()}] ({obj.Alignment})", 13);
            AddTextLabel(_contentContainer, obj.Prompt, 12);

            var buttonText = $"Interact with {obj.Slot}";
            AddButton(_contentContainer, buttonText, () => OnObjectInteracted(obj));
        }

        // Display decoys
        AddTextLabel(_contentContainer, "=== DECOYS ===", 14);
        foreach (var decoy in chamber.Decoys)
        {
            var buttonText = $"Investigate {decoy.Id}";
            AddButton(_contentContainer, buttonText, () => OnDecoyInvestigated(decoy));
        }

        // Add Complete Chamber button
        AddButton(_contentContainer, "Complete Chamber", OnChamberComplete);
    }

    /// <summary>
    /// Handles player interacting with a set piece object.
    /// </summary>
    private void OnObjectInteracted(ChamberObject obj)
    {
        GD.Print($"[BeatChamberExploration] Player interacted with {obj.Slot}");

        // Show interaction log
        if (obj.InteractionLog != null && obj.InteractionLog.Count > 0)
        {
            var log = string.Join("\n", obj.InteractionLog);
            GD.Print($"Interaction Log:\n{log}");
        }

        // Show approval banter
        if (obj.Banter?.Approve != null)
        {
            GD.Print($"[{obj.Banter.Approve.Speaker}] {obj.Banter.Approve.Line}");
        }

        // TODO: Update EchoAffinityTracker with object alignment
        // This will be wired in when tracker is available as a shared service
    }

    /// <summary>
    /// Handles player investigating a decoy.
    /// </summary>
    private void OnDecoyInvestigated(DecoyData decoy)
    {
        GD.Print($"[BeatChamberExploration] Player investigated decoy: {decoy.Id}");
        GD.Print($"Reveal: {decoy.RevealText}");

        // TODO: Show visual feedback (glitch effect, etc)
    }

    /// <summary>
    /// Handles chamber completion - player is ready to leave.
    /// </summary>
    private void OnChamberComplete()
    {
        GD.Print("[BeatChamberExploration] Chamber exploration complete, advancing to next beat");
        TransitionToNextBeat();
    }
}
