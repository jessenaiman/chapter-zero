
// <copyright file="PartyCreator.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;

namespace OmegaSpiral.Source.Scripts.Common;
/// <summary>
/// Handles party creation for the Wizardry-style character creation scene.
/// Provides Ui for selecting character classes, races, and managing party composition.
/// Integrates with GameState to persist the created party for subsequent scenes.
/// </summary>
[GlobalClass]
public partial class PartyCreator : Node2D
{
    private PartyData? _partyData;
    private GodotObject? _sceneLoader;

    /// <summary>
    /// Ui Elements
    /// </summary>
    private LineEdit? _characterNameInput;
    private OptionButton? _classSelector;
    private OptionButton? _raceSelector;
    private Label? _statsDisplay;
    private Button? _addCharacterButton;
    private Button? _finishPartyButton;
    private VBoxContainer? _partyList;

    /// <summary>
    /// Initializes the party creator scene and sets up all Ui components.
    /// Loads GameState and SceneManager singletons, initializes Ui elements,
    /// populates dropdown selectors, and connects event handlers.
    /// </summary>
    public override void _Ready()
    {
        this._partyData = new PartyData();
        this._sceneLoader = this.GetNode<GodotObject>("/root/SceneLoader");

        // Initialize Ui
        this._characterNameInput = this.GetNode<LineEdit>("CharacterNameInput");
        this._classSelector = this.GetNode<OptionButton>("ClassSelector");
        this._raceSelector = this.GetNode<OptionButton>("RaceSelector");
        this._statsDisplay = this.GetNode<Label>("StatsDisplay");
        this._addCharacterButton = this.GetNode<Button>("AddCharacterButton");
        this._finishPartyButton = this.GetNode<Button>("FinishPartyButton");
        this._partyList = this.GetNode<VBoxContainer>("PartyList");

        if (this._sceneLoader == null || this._characterNameInput == null ||
            this._classSelector == null || this._raceSelector == null || this._statsDisplay == null ||
            this._addCharacterButton == null || this._finishPartyButton == null || this._partyList == null)
        {
            GD.PrintErr("Failed to find required nodes in PartyCreator");
            return;
        }

        // Populate selectors
        this.PopulateClassSelector();
        this.PopulateRaceSelector();

        // Connect signals
        this._classSelector.ItemSelected += this.OnClassSelected;
        this._raceSelector.ItemSelected += this.OnRaceSelected;
        this._addCharacterButton.Pressed += this.OnAddCharacterPressed;
        this._finishPartyButton.Pressed += this.OnFinishPartyPressed;

        this.UpdateUi();
    }

    /// <summary>
    /// Populates the character class selection dropdown with all available character classes.
    /// Clears existing items and adds each enum value as a selectable option.
    /// </summary>
    private void PopulateClassSelector()
    {
        if (this._classSelector == null)
        {
            return;
        }

        this._classSelector.Clear();
        foreach (CharacterClass cClass in Enum.GetValues<CharacterClass>())
        {
            this._classSelector.AddItem(cClass.ToString());
        }
    }

    /// <summary>
    /// Populates the character race selection dropdown with all available character races.
    /// Clears existing items and adds each enum value as a selectable option.
    /// </summary>
    private void PopulateRaceSelector()
    {
        if (this._raceSelector == null)
        {
            return;
        }

        this._raceSelector.Clear();
        foreach (CharacterRace race in Enum.GetValues<CharacterRace>())
        {
            this._raceSelector.AddItem(race.ToString());
        }
    }

    private void OnClassSelected(long index)
    {
        _ = index;
        this.UpdateStatsPreview();
    }

    private void OnRaceSelected(long index)
    {
        _ = index;
        this.UpdateStatsPreview();
    }

    private void UpdateStatsPreview()
    {
        if (this._classSelector == null || this._raceSelector == null || this._statsDisplay == null)
        {
            return;
        }

        var selectedClass = (CharacterClass) this._classSelector.Selected;
        var selectedRace = (CharacterRace) this._raceSelector.Selected;

        var previewStats = CharacterStats.GenerateRandomStats();
        previewStats.ApplyRacialModifiers(selectedRace);

        this._statsDisplay.Text = $"Class: {selectedClass}\nRace: {selectedRace}\n\nStats:\n" +
                            $"STR: {previewStats.Strength}\n" +
                            $"INT: {previewStats.Intelligence}\n" +
                            $"WIS: {previewStats.Wisdom}\n" +
                            $"DEX: {previewStats.Dexterity}\n" +
                            $"CON: {previewStats.Constitution}\n" +
                            $"CHA: {previewStats.Charisma}\n" +
                            $"LCK: {previewStats.Luck}";
    }

    private void OnAddCharacterPressed()
    {
        if (this._characterNameInput == null || this._classSelector == null || this._raceSelector == null || this._partyData == null)
        {
            return;
        }

        string name = this._characterNameInput.Text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            GD.Print("Character name cannot be empty");
            return;
        }

        var selectedClass = (CharacterClass) this._classSelector.Selected;
        var selectedRace = (CharacterRace) this._raceSelector.Selected;

        var character = new Character(name, selectedClass, selectedRace);
        if (this._partyData.AddMember(character))
        {
            this.UpdatePartyList();
            this._characterNameInput.Text = string.Empty;
        }
    }

    private void UpdatePartyList()
    {
        if (this._partyList == null || this._partyData == null)
        {
            return;
        }

        // Clear existing list
        foreach (Node child in this._partyList.GetChildren())
        {
            child.QueueFree();
        }

        // Add current members
        foreach (var member in this._partyData.Members)
        {
            var label = new Label();
            label.Text = $"{member.Name} - {member.Class} {member.Race}";
            this._partyList.AddChild(label);
            label.Dispose();
        }

        this.UpdateUi();
    }

    private void UpdateUi()
    {
        if (this._addCharacterButton == null || this._finishPartyButton == null || this._partyData == null)
        {
            return;
        }

        this._addCharacterButton.Disabled = this._partyData.Members.Count >= 3;
        this._finishPartyButton.Disabled = this._partyData.Members.Count < 3;
    }

    private void OnFinishPartyPressed()
    {
        if (this._partyData == null || this._sceneLoader == null)
        {
            return;
        }

        if (this._partyData.Members.Count >= 3)
        {
            this._sceneLoader.Call("load_scene", "res://scenes/Scene4TileDungeon.tscn");
        }
    }
}
