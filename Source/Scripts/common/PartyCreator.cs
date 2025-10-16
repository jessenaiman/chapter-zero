namespace OmegaSpiral.Source.Scripts.Common;

// <copyright file="PartyCreator.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts;

/// <summary>
/// Handles party creation for the Wizardry-style character creation scene.
/// Provides UI for selecting character classes, races, and managing party composition.
/// Integrates with GameState to persist the created party for subsequent scenes.
/// </summary>
[GlobalClass]
public partial class PartyCreator : Node2D
{
    private PartyData? partyData;
    private GameState? gameState;
    private SceneManager? sceneManager;

    /// <summary>
    /// UI Elements
    /// </summary>
    private LineEdit? characterNameInput;
    private OptionButton? classSelector;
    private OptionButton? raceSelector;
    private Label? statsDisplay;
    private Button? addCharacterButton;
    private Button? finishPartyButton;
    private VBoxContainer? partyList;

    /// <summary>
    /// Initializes the party creator scene and sets up all UI components.
    /// Loads GameState and SceneManager singletons, initializes UI elements,
    /// populates dropdown selectors, and connects event handlers.
    /// </summary>
    public override void _Ready()
    {
        this.partyData = new PartyData();
        this.gameState = this.GetNode<GameState>("/root/GameState");
        this.sceneManager = this.GetNode<SceneManager>("/root/SceneManager");

        // Initialize UI
        this.characterNameInput = this.GetNode<LineEdit>("CharacterNameInput");
        this.classSelector = this.GetNode<OptionButton>("ClassSelector");
        this.raceSelector = this.GetNode<OptionButton>("RaceSelector");
        this.statsDisplay = this.GetNode<Label>("StatsDisplay");
        this.addCharacterButton = this.GetNode<Button>("AddCharacterButton");
        this.finishPartyButton = this.GetNode<Button>("FinishPartyButton");
        this.partyList = this.GetNode<VBoxContainer>("PartyList");

        if (this.gameState == null || this.sceneManager == null || this.characterNameInput == null ||
            this.classSelector == null || this.raceSelector == null || this.statsDisplay == null ||
            this.addCharacterButton == null || this.finishPartyButton == null || this.partyList == null)
        {
            GD.PrintErr("Failed to find required nodes in PartyCreator");
            return;
        }

        // Populate selectors
        this.PopulateClassSelector();
        this.PopulateRaceSelector();

        // Connect signals
        this.classSelector.ItemSelected += this.OnClassSelected;
        this.raceSelector.ItemSelected += this.OnRaceSelected;
        this.addCharacterButton.Pressed += this.OnAddCharacterPressed;
        this.finishPartyButton.Pressed += this.OnFinishPartyPressed;

        this.UpdateUI();
    }

    /// <summary>
    /// Populates the character class selection dropdown with all available character classes.
    /// Clears existing items and adds each enum value as a selectable option.
    /// </summary>
    private void PopulateClassSelector()
    {
        if (this.classSelector == null)
        {
            return;
        }

        this.classSelector.Clear();
        foreach (CharacterClass cClass in System.Enum.GetValues(typeof(CharacterClass)))
        {
            this.classSelector.AddItem(cClass.ToString());
        }
    }

    /// <summary>
    /// Populates the character race selection dropdown with all available character races.
    /// Clears existing items and adds each enum value as a selectable option.
    /// </summary>
    private void PopulateRaceSelector()
    {
        if (this.raceSelector == null)
        {
            return;
        }

        this.raceSelector.Clear();
        foreach (CharacterRace race in System.Enum.GetValues(typeof(CharacterRace)))
        {
            this.raceSelector.AddItem(race.ToString());
        }
    }

    private void OnClassSelected(long index)
    {
        this.UpdateStatsPreview();
    }

    private void OnRaceSelected(long index)
    {
        this.UpdateStatsPreview();
    }

    private void UpdateStatsPreview()
    {
        if (this.classSelector == null || this.raceSelector == null || this.statsDisplay == null)
        {
            return;
        }

        var selectedClass = (CharacterClass) this.classSelector.Selected;
        var selectedRace = (CharacterRace) this.raceSelector.Selected;

        var previewStats = CharacterStats.GenerateRandomStats();
        previewStats.ApplyRacialModifiers(selectedRace);

        this.statsDisplay.Text = $"Class: {selectedClass}\nRace: {selectedRace}\n\nStats:\n" +
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
        if (this.characterNameInput == null || this.classSelector == null || this.raceSelector == null || this.partyData == null)
        {
            return;
        }

        string name = this.characterNameInput.Text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            GD.Print("Character name cannot be empty");
            return;
        }

        var selectedClass = (CharacterClass) this.classSelector.Selected;
        var selectedRace = (CharacterRace) this.raceSelector.Selected;

        var character = new Character(name, selectedClass, selectedRace);
        if (this.partyData.AddMember(character))
        {
            this.UpdatePartyList();
            this.characterNameInput.Text = string.Empty;
        }
    }

    private void UpdatePartyList()
    {
        if (this.partyList == null || this.partyData == null)
        {
            return;
        }

        // Clear existing list
        foreach (Node child in this.partyList.GetChildren())
        {
            child.QueueFree();
        }

        // Add current members
        foreach (var member in this.partyData.Members)
        {
            var label = new Label();
            label.Text = $"{member.Name} - {member.Class} {member.Race}";
            this.partyList.AddChild(label);
            label.Dispose();
        }

        this.UpdateUI();
    }

    private void UpdateUI()
    {
        if (this.addCharacterButton == null || this.finishPartyButton == null || this.partyData == null)
        {
            return;
        }

        this.addCharacterButton.Disabled = this.partyData.Members.Count >= 3;
        this.finishPartyButton.Disabled = this.partyData.Members.Count < 3;
    }

    private void OnFinishPartyPressed()
    {
        if (this.partyData == null || this.gameState == null || this.sceneManager == null)
        {
            return;
        }

        if (this.partyData.Members.Count >= 3)
        {
            this.gameState.PlayerParty = this.partyData;
            this.sceneManager.TransitionToScene("Scene4TileDungeon");
        }
    }
}
