using Godot;
using System.Collections.Generic;
using OmegaSpiral.Source.Scripts;

public partial class PartyCreator : Node2D
{
    private PartyData _partyData;
    private GameState _gameState;
    private SceneManager _sceneManager;

    // UI Elements
    private LineEdit _characterNameInput;
    private OptionButton _classSelector;
    private OptionButton _raceSelector;
    private Label _statsDisplay;
    private Button _addCharacterButton;
    private Button _finishPartyButton;
    private VBoxContainer _partyList;

    public override void _Ready()
    {
        _partyData = new PartyData();
        _gameState = GetNode<GameState>("/root/GameState");
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");

        // Initialize UI
        _characterNameInput = GetNode<LineEdit>("CharacterNameInput");
        _classSelector = GetNode<OptionButton>("ClassSelector");
        _raceSelector = GetNode<OptionButton>("RaceSelector");
        _statsDisplay = GetNode<Label>("StatsDisplay");
        _addCharacterButton = GetNode<Button>("AddCharacterButton");
        _finishPartyButton = GetNode<Button>("FinishPartyButton");
        _partyList = GetNode<VBoxContainer>("PartyList");

        // Populate selectors
        PopulateClassSelector();
        PopulateRaceSelector();

        // Connect signals
        _classSelector.ItemSelected += OnClassSelected;
        _raceSelector.ItemSelected += OnRaceSelected;
        _addCharacterButton.Pressed += OnAddCharacterPressed;
        _finishPartyButton.Pressed += OnFinishPartyPressed;

        UpdateUI();
    }

    private void PopulateClassSelector()
    {
        _classSelector.Clear();
        foreach (CharacterClass cClass in System.Enum.GetValues(typeof(CharacterClass)))
        {
            _classSelector.AddItem(cClass.ToString());
        }
    }

    private void PopulateRaceSelector()
    {
        _raceSelector.Clear();
        foreach (CharacterRace race in System.Enum.GetValues(typeof(CharacterRace)))
        {
            _raceSelector.AddItem(race.ToString());
        }
    }

    private void OnClassSelected(long index)
    {
        UpdateStatsPreview();
    }

    private void OnRaceSelected(long index)
    {
        UpdateStatsPreview();
    }

    private void UpdateStatsPreview()
    {
        var selectedClass = (CharacterClass)_classSelector.Selected;
        var selectedRace = (CharacterRace)_raceSelector.Selected;

        var previewStats = CharacterStats.GenerateRandomStats();
        previewStats.ApplyRacialModifiers(selectedRace);

        _statsDisplay.Text = $"Class: {selectedClass}\nRace: {selectedRace}\n\nStats:\n" +
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
        string name = _characterNameInput.Text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            GD.Print("Character name cannot be empty");
            return;
        }

        var selectedClass = (CharacterClass)_classSelector.Selected;
        var selectedRace = (CharacterRace)_raceSelector.Selected;

        var character = new Character(name, selectedClass, selectedRace);
        if (_partyData.AddMember(character))
        {
            UpdatePartyList();
            _characterNameInput.Text = "";
        }
    }

    private void UpdatePartyList()
    {
        // Clear existing list
        foreach (Node child in _partyList.GetChildren())
        {
            child.QueueFree();
        }

        // Add current members
        foreach (var member in _partyData.Members)
        {
            var label = new Label();
            label.Text = $"{member.Name} - {member.Class} {member.Race}";
            _partyList.AddChild(label);
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        _addCharacterButton.Disabled = _partyData.Members.Count >= 3;
        _finishPartyButton.Disabled = _partyData.Members.Count < 3;
    }

    private void OnFinishPartyPressed()
    {
        if (_partyData.Members.Count >= 3)
        {
            _gameState.PlayerParty = _partyData;
            _sceneManager.TransitionToScene("Scene4TileDungeon");
        }
    }
}