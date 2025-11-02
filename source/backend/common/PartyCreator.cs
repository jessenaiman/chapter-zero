
// <copyright file="PartyCreator.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Backend.Common;
/// <summary>
/// Handles party creation for the Wizardry-style character creation scene.
/// Provides UI for selecting character classes, races, and managing party composition.
/// Integrates with Maaack GlobalState to persist the created party for subsequent scenes.
/// </summary>
[GlobalClass]
public partial class PartyCreator : Node2D
{
    /// <summary>
    /// UI Elements
    /// </summary>
    private LineEdit? _characterNameInput;
    private OptionButton? _classSelector;
    private OptionButton? _raceSelector;
    private Button? _addCharacterButton;
    private Button? _finishPartyButton;
    private VBoxContainer? _partyList;

    /// <summary>
    /// Initializes the party creator scene and sets up all UI components.
    /// Loads existing party data from GlobalState and populates UI elements.
    /// </summary>
    public override void _Ready()
    {
        // Initialize UI
        this._characterNameInput = this.GetNode<LineEdit>("CharacterNameInput");
        this._classSelector = this.GetNode<OptionButton>("ClassSelector");
        this._raceSelector = this.GetNode<OptionButton>("RaceSelector");
        this._addCharacterButton = this.GetNode<Button>("AddCharacterButton");
        this._finishPartyButton = this.GetNode<Button>("FinishPartyButton");
        this._partyList = this.GetNode<VBoxContainer>("PartyList");

        if (this._characterNameInput == null || this._classSelector == null || this._raceSelector == null ||
            this._addCharacterButton == null || this._finishPartyButton == null || this._partyList == null)
        {
            GD.PrintErr("Failed to find required nodes in PartyCreator");
            return;
        }

        // Populate selectors
        this.PopulateClassSelector();
        this.PopulateRaceSelector();

        // Connect signals
        this._addCharacterButton.Pressed += this.OnAddCharacterPressed;
        this._finishPartyButton.Pressed += this.OnFinishPartyPressed;

        // Load existing party data
        this.UpdatePartyList();
        this.UpdateUi();
    }

    /// <summary>
    /// Populates the character class selection dropdown with all available character classes.
    /// </summary>
    private void PopulateClassSelector()
    {
        if (this._classSelector == null)
        {
            return;
        }

        this._classSelector.Clear();
        this._classSelector.AddItem("Fighter", 0);
        this._classSelector.AddItem("Mage", 1);
        this._classSelector.AddItem("Priest", 2);
        this._classSelector.AddItem("Thief", 3);
        this._classSelector.AddItem("Bard", 4);
        this._classSelector.AddItem("Paladin", 5);
        this._classSelector.AddItem("Ranger", 6);
    }

    /// <summary>
    /// Populates the character race selection dropdown with all available character races.
    /// </summary>
    private void PopulateRaceSelector()
    {
        if (this._raceSelector == null)
        {
            return;
        }

        this._raceSelector.Clear();
        this._raceSelector.AddItem("Human", 0);
        this._raceSelector.AddItem("Elf", 1);
        this._raceSelector.AddItem("Dwarf", 2);
        this._raceSelector.AddItem("Gnome", 3);
        this._raceSelector.AddItem("Halfling", 4);
        this._raceSelector.AddItem("Half-Elf", 5);
    }

    private void OnAddCharacterPressed()
    {
        if (this._characterNameInput == null || this._classSelector == null || this._raceSelector == null)
        {
            return;
        }

        string name = this._characterNameInput.Text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            GD.Print("Character name cannot be empty");
            return;
        }

        // Get current party data from GlobalState
        var partyData = this.GetPartyData();
        int memberCount = partyData.Count;

        if (memberCount >= 3)
        {
            GD.Print("Party is full (maximum 3 members)");
            return;
        }

        // Add new character
        var characterData = new Godot.Collections.Dictionary
        {
            ["name"] = name,
            ["class"] = this._classSelector.Selected,
            ["race"] = this._raceSelector.Selected
        };

        partyData[memberCount.ToString()] = characterData;

        // Save to GlobalState
        this.SavePartyData(partyData);

        // Update UI
        this.UpdatePartyList();
        this._characterNameInput.Text = string.Empty;
        this.UpdateUi();
    }

    private void UpdatePartyList()
    {
        if (this._partyList == null)
        {
            return;
        }

        // Clear existing list
        foreach (Node child in this._partyList.GetChildren())
        {
            child.QueueFree();
        }

        // Load current party data
        var partyData = this.GetPartyData();

        // Add current members
        foreach (var memberKey in partyData.Keys)
        {
            var memberData = (Godot.Collections.Dictionary)partyData[memberKey];
            string name = (string)memberData["name"];
            int classId = (int)memberData["class"];
            int raceId = (int)memberData["race"];

            string className = this.GetClassName(classId);
            string raceName = this.GetRaceName(raceId);

            var label = new Label();
            label.Text = $"{name} - {className} {raceName}";
            this._partyList.AddChild(label);
        }
    }

    private void UpdateUi()
    {
        if (this._addCharacterButton == null || this._finishPartyButton == null)
        {
            return;
        }

        var partyData = this.GetPartyData();
        int memberCount = partyData.Count;

        this._addCharacterButton.Disabled = memberCount >= 3;
        this._finishPartyButton.Disabled = memberCount < 3;
    }

    private void OnFinishPartyPressed()
    {
        var partyData = this.GetPartyData();
        if (partyData.Count >= 3)
        {
            // Party creation complete - could emit signal or transition to next scene
            GD.Print("Party creation complete!");
            // TODO: Transition to next scene (combat, overworld, etc.)
        }
    }

    private Godot.Collections.Dictionary GetPartyData()
    {
        // Access GlobalState through GDScript
        var globalState = this.GetNode("/root/GlobalState");
        if (globalState == null)
        {
            GD.PrintErr("GlobalState not found");
            return new Godot.Collections.Dictionary();
        }

        // Call GDScript method to get GameState
        var gameState = (GodotObject)globalState.Call("get_or_create_state", "GameState", "res://source/autoloads/game_state.gd");
        if (gameState == null)
        {
            GD.PrintErr("GameState not found");
            return new Godot.Collections.Dictionary();
        }

        // Get player_party from GameState
        return (Godot.Collections.Dictionary)gameState.Get("player_party");
    }

    private void SavePartyData(Godot.Collections.Dictionary partyData)
    {
        // Access GlobalState through GDScript
        var globalState = this.GetNode("/root/GlobalState");
        if (globalState == null)
        {
            GD.PrintErr("GlobalState not found");
            return;
        }

        // Call GDScript method to get GameState and save
        var gameState = (GodotObject)globalState.Call("get_or_create_state", "GameState", "res://source/autoloads/game_state.gd");
        if (gameState != null)
        {
            gameState.Set("player_party", partyData);
            globalState.Call("save");
        }
    }

    private string GetClassName(int classId)
    {
        return classId switch
        {
            0 => "Fighter",
            1 => "Mage",
            2 => "Priest",
            3 => "Thief",
            4 => "Bard",
            5 => "Paladin",
            6 => "Ranger",
            _ => "Unknown"
        };
    }

    private string GetRaceName(int raceId)
    {
        return raceId switch
        {
            0 => "Human",
            1 => "Elf",
            2 => "Dwarf",
            3 => "Gnome",
            4 => "Halfling",
            5 => "Half-Elf",
            _ => "Unknown"
        };
    }
}
