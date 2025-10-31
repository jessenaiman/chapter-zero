// <copyright file="StageSelector.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Frontend.Scenes.Menus;

using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Stage selector popup menu for development purposes.
/// Allows developers to quickly jump to any game stage without playing through all preceding stages.
///
/// RESPONSIBILITY: Display available stages in a popup dialog and handle stage selection.
/// Does NOT handle scene loading – delegates to SceneLoader.
/// </summary>
[GlobalClass]
public partial class StageSelector : ConfirmationDialog
{
    /// <summary>
    /// Maps stage names to their scene paths.
    /// Can be configured externally or updated from a configuration file.
    /// </summary>
    private readonly Dictionary<string, string> _StageMap = new()
    {
        { "Stage 1: Ghost Terminal", "res://source/frontend/stages/stage_1_ghost/ghost_main.tscn" },
        { "Stage 2: NetHack", "res://source/frontend/stages/stage_2_nethack/nethack_main.tscn" },
        { "Stage 3: Town", "res://source/frontend/stages/stage_3_town/town_main_start.tscn" },
        { "Stage 4: Party Selection", "res://source/frontend/stages/stage_4_party_selection/stage4_main.tscn" },
        { "Stage 5: Escape", "res://source/frontend/stages/stage_5_escape/escape_main.tscn" },
    };

    private ItemList? _stageList;
    private string _selectedStage = string.Empty;

    [Signal]
    public delegate void StageSelectedEventHandler(string scenePath);

    public override void _Ready()
    {
        base._Ready();
        try
        {
            InitializeDialog();
            PopulateStageList();
            ConnectSignals();
        }
        catch (Exception ex)
        {
            GD.PushError($"[StageSelector] Initialization failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Initializes the dialog appearance and layout.
    /// </summary>
    private void InitializeDialog()
    {
        Title = "Select Development Stage";
        DialogText = "Choose a stage to load:";
        Size = new Vector2I(600, 400);
        Exclusive = true;
        OkButtonText = "Load";
        CancelButtonText = "Cancel";
    }

    /// <summary>
    /// Creates and populates the ItemList with available stages.
    /// </summary>
    private void PopulateStageList()
    {
        _stageList = new ItemList();
        _stageList.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        _stageList.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

        int index = 0;
        foreach (var stage in _StageMap)
        {
            _stageList.AddItem(stage.Key, null);
            index++;
        }

        AddChild(_stageList);
        _stageList.ItemSelected += OnStageItemSelected;
    }

    /// <summary>
    /// Connects dialog signals to appropriate handlers.
    /// </summary>
    private void ConnectSignals()
    {
        Confirmed += OnConfirmed;
        Canceled += OnCanceled;
    }

    /// <summary>
    /// Handles stage item selection in the list.
    /// </summary>
    private void OnStageItemSelected(long index)
    {
        if (_stageList == null || index < 0 || index >= _stageList.ItemCount)
            return;

        var stageName = _stageList.GetItemText((int) index);
        if (_StageMap.TryGetValue(stageName, out var scenePath))
        {
            _selectedStage = scenePath;
            GD.Print($"[StageSelector] Selected stage: {stageName} -> {scenePath}");
        }
    }

    /// <summary>
    /// Handles confirmation – emits the stage selected signal with the scene path.
    /// </summary>
    private void OnConfirmed()
    {
        if (string.IsNullOrEmpty(_selectedStage))
        {
            GD.PushWarning("[StageSelector] No stage selected. Please choose a stage and try again.");
            return;
        }

        GD.Print($"[StageSelector] Loading scene: {_selectedStage}");
        EmitSignal(SignalName.StageSelected, _selectedStage);
        QueueFree();
    }

    /// <summary>
    /// Handles cancellation – simply closes the dialog.
    /// </summary>
    private void OnCanceled()
    {
        GD.Print("[StageSelector] Stage selection cancelled.");
        QueueFree();
    }

    /// <summary>
    /// Gets the internal stage map for testing or external updates.
    /// </summary>
    /// <returns>Dictionary mapping stage names to scene paths.</returns>
    public Dictionary<string, string> GetStageMap() => _StageMap;

    /// <summary>
    /// Updates the stage map with new mappings.
    /// Useful for testing or if stage paths change at runtime.
    /// </summary>
    /// <param name="newMap">New stage map to use.</param>
    public void SetStageMap(Dictionary<string, string> newMap)
    {
        _StageMap.Clear();
        foreach (var entry in newMap)
        {
            _StageMap[entry.Key] = entry.Value;
        }
    }
}
