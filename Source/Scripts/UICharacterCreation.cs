using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Models;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Interfaces;

/// <summary>
/// Container for the player character creation display.
/// The UICharacterCreation manages the display and interaction of character creation options,
/// allowing players to customize their character's appearance, name, class, and other attributes.
/// It provides a comprehensive interface for character customization with real-time previews
/// and validation of player choices.
/// </summary>
public partial class UICharacterCreation : Control
{
    /// <summary>
    /// Emitted when the player confirms their character creation.
    /// </summary>
    [Signal]
    public delegate void CharacterConfirmedEventHandler();

    /// <summary>
    /// Emitted when the player cancels character creation.
    /// </summary>
    [Signal]
    public delegate void CharacterCreationCancelledEventHandler();

    /// <summary>
    /// Emitted when a character attribute changes.
    /// </summary>
    [Signal]
    public delegate void CharacterAttributeChangedEventHandler(string attributeName, Variant newValue);

    /// <summary>
    /// The character being created.
    /// </summary>
    public Character CreatedCharacter { get; private set; }

    /// <summary>
    /// Whether the character creation interface is currently visible.
    /// </summary>
    public bool CreationVisible
    {
        get => Visible;
        set => Visible = value;
    }

    /// <summary>
    /// The container for character appearance options.
    /// </summary>
    private Control appearanceContainer;

    /// <summary>
    /// The container for character name input.
    /// </summary>
    private Control nameContainer;

    /// <summary>
    /// The container for character class selection.
    /// </summary>
    private Control classContainer;

    /// <summary>
    /// The container for character stats display.
    /// </summary>
    private Control statsContainer;

    /// <summary>
    /// The container for character preview.
    /// </summary>
    private Control previewContainer;

    /// <summary>
    /// The confirm button.
    /// </summary>
    private Button confirmButton;

    /// <summary>
    /// The cancel button.
    /// </summary>
    private Button cancelButton;

    /// <summary>
    /// The list of available character classes.
    /// </summary>
    private List<CharacterClass> availableClasses = new List<CharacterClass>();

    /// <summary>
    /// The list of available character appearances.
    /// </summary>
    private List<CharacterAppearance> availableAppearances = new List<CharacterAppearance>();

    /// <summary>
    /// The currently selected character class.
    /// </summary>
    private CharacterClass selectedClass;

    /// <summary>
    /// The currently selected character appearance.
    /// </summary>
    private CharacterAppearance selectedAppearance;

    /// <summary>
    /// The character's name.
    /// </summary>
    private string characterName = "";

    /// <summary>
    /// The character's stats.
    /// </summary>
    private CharacterStats characterStats;

    public override void _Ready()
    {
        // Get references to child UI elements
        appearanceContainer = GetNode<Control>("AppearanceContainer");
        nameContainer = GetNode<Control>("NameContainer");
        classContainer = GetNode<Control>("ClassContainer");
        statsContainer = GetNode<Control>("StatsContainer");
        previewContainer = GetNode<Control>("PreviewContainer");
        confirmButton = GetNode<Button>("ConfirmButton");
        cancelButton = GetNode<Button>("CancelButton");

        // Initially hide the character creation interface
        Visible = false;

        // Connect to any necessary signals
        ConnectSignals();
    }

    /// <summary>
    /// Connect to necessary signals.
    /// </summary>
    private void ConnectSignals()
    {
        // Connect the confirm and cancel buttons
        if (confirmButton != null)
        {
            confirmButton.Pressed += OnConfirmButtonPressed;
        }

        if (cancelButton != null)
        {
            cancelButton.Pressed += OnCancelButtonPressed;
        }

        // Connect to character creation events
        // CharacterCreationEvents.AppearanceSelected += OnAppearanceSelected;
        // CharacterCreationEvents.ClassSelected += OnClassSelected;
        // CharacterCreationEvents.NameChanged += OnNameChanged;
    }

    /// <summary>
    /// Setup the UI character creation with available options.
    /// </summary>
    /// <param name="classes">The available character classes</param>
    /// <param name="appearances">The available character appearances</param>
    public void Setup(List<CharacterClass> classes, List<CharacterAppearance> appearances)
    {
        availableClasses = new List<CharacterClass>(classes ?? new List<CharacterClass>());
        availableAppearances = new List<CharacterAppearance>(appearances ?? new List<CharacterAppearance>());

        // Clear any existing displays
        ClearDisplays();

        // Create displays for all available options
        CreateClassDisplay(availableClasses, classContainer);
        CreateAppearanceDisplay(availableAppearances, appearanceContainer);
        CreateNameDisplay(nameContainer);
        CreateStatsDisplay(statsContainer);
        CreatePreviewDisplay(previewContainer);

        // Show the character creation interface
        Visible = true;

        // Update all displays
        UpdateAllDisplays();

        // Create a new character to be customized
        CreatedCharacter = new Character();
        characterStats = new CharacterStats();
        CreatedCharacter.Stats = characterStats;

        // Select default options
        if (availableClasses.Count > 0)
        {
            SelectClass(availableClasses[0]);
        }

        if (availableAppearances.Count > 0)
        {
            SelectAppearance(availableAppearances[0]);
        }
    }

    /// <summary>
    /// Clear all character creation displays.
    /// </summary>
    private void ClearDisplays()
    {
        // Clear containers
        if (appearanceContainer != null)
        {
            foreach (var child in appearanceContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (nameContainer != null)
        {
            foreach (var child in nameContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (classContainer != null)
        {
            foreach (var child in classContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (statsContainer != null)
        {
            foreach (var child in statsContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (previewContainer != null)
        {
            foreach (var child in previewContainer.GetChildren())
            {
                child.QueueFree();
            }
        }
    }

    /// <summary>
    /// Create a display for character classes.
    /// </summary>
    /// <param name="classes">The character classes to display</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateClassDisplay(List<CharacterClass> classes, Control container)
    {
        if (classes == null || container == null)
        {
            return;
        }

        // Create displays for all classes
        foreach (var characterClass in classes)
        {
            CreateClassOption(characterClass, container);
        }
    }

    /// <summary>
    /// Create a display for character appearances.
    /// </summary>
    /// <param name="appearances">The character appearances to display</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateAppearanceDisplay(List<CharacterAppearance> appearances, Control container)
    {
        if (appearances == null || container == null)
        {
            return;
        }

        // Create displays for all appearances
        foreach (var appearance in appearances)
        {
            CreateAppearanceOption(appearance, container);
        }
    }

    /// <summary>
    /// Create a display for character name input.
    /// </summary>
    /// <param name="container">The container to add the display to</param>
    private void CreateNameDisplay(Control container)
    {
        if (container == null)
        {
            return;
        }

        // Create a text input field for the character name
        var nameInput = new LineEdit();
        nameInput.PlaceholderText = "Enter character name...";
        nameInput.MaxLength = 20;
        nameInput.TextChanged += OnNameTextChanged;
        container.AddChild(nameInput);
    }

    /// <summary>
    /// Create a display for character stats.
    /// </summary>
    /// <param name="container">The container to add the display to</param>
    private void CreateStatsDisplay(Control container)
    {
        if (container == null || characterStats == null)
        {
            return;
        }

        // Create displays for all stats
        CreateStatDisplay("Strength", characterStats.Strength, container);
        CreateStatDisplay("Agility", characterStats.Agility, container);
        CreateStatDisplay("Intelligence", characterStats.Intelligence, container);
        CreateStatDisplay("Vitality", characterStats.Vitality, container);
        CreateStatDisplay("Luck", characterStats.Luck, container);
    }

    /// <summary>
    /// Create a display for a character stat.
    /// </summary>
    /// <param name="statName">The name of the stat</param>
    /// <param name="statValue">The value of the stat</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateStatDisplay(string statName, int statValue, Control container)
    {
        if (string.IsNullOrEmpty(statName) || container == null)
        {
            return;
        }

        // Create a label for the stat name and value
        var statLabel = new Label();
        statLabel.Text = $"{statName}: {statValue}";
        statLabel.Name = statName;
        container.AddChild(statLabel);
    }

    /// <summary>
    /// Create a display for the character preview.
    /// </summary>
    /// <param name="container">The container to add the display to</param>
    private void CreatePreviewDisplay(Control container)
    {
        if (container == null)
        {
            return;
        }

        // Create a sprite for the character preview
        var previewSprite = new Sprite2D();
        previewSprite.Name = "CharacterPreview";
        container.AddChild(previewSprite);
    }

    /// <summary>
    /// Create a display option for a character class.
    /// </summary>
    /// <param name="characterClass">The character class to create an option for</param>
    /// <param name="container">The container to add the option to</param>
    private void CreateClassOption(CharacterClass characterClass, Control container)
    {
        if (characterClass == null || container == null)
        {
            return;
        }

        // Create a button for the class option
        var classButton = new Button();
        classButton.Text = characterClass.Name;
        classButton.Name = characterClass.Name;
        classButton.Pressed += () => OnClassSelected(characterClass);
        container.AddChild(classButton);
    }

    /// <summary>
    /// Create a display option for a character appearance.
    /// </summary>
    /// <param name="appearance">The character appearance to create an option for</param>
    /// <param name="container">The container to add the option to</param>
    private void CreateAppearanceOption(CharacterAppearance appearance, Control container)
    {
        if (appearance == null || container == null)
        {
            return;
        }

        // Create a button for the appearance option
        var appearanceButton = new TextureButton();
        appearanceButton.TextureNormal = appearance.PreviewTexture;
        appearanceButton.Name = appearance.Name;
        appearanceButton.Pressed += () => OnAppearanceSelected(appearance);
        container.AddChild(appearanceButton);
    }

    /// <summary>
    /// Update all character creation displays.
    /// </summary>
    public void UpdateAllDisplays()
    {
        // Update the stats display
        UpdateStatsDisplay();

        // Update the preview display
        UpdatePreviewDisplay();

        // Update the confirm button state
        UpdateConfirmButton();
    }

    /// <summary>
    /// Update the character stats display.
    /// </summary>
    private void UpdateStatsDisplay()
    {
        if (statsContainer == null || characterStats == null)
        {
            return;
        }

        // Update all stat displays
        UpdateStatDisplay("Strength", characterStats.Strength);
        UpdateStatDisplay("Agility", characterStats.Agility);
        UpdateStatDisplay("Intelligence", characterStats.Intelligence);
        UpdateStatDisplay("Vitality", characterStats.Vitality);
        UpdateStatDisplay("Luck", characterStats.Luck);
    }

    /// <summary>
    /// Update a specific character stat display.
    /// </summary>
    /// <param name="statName">The name of the stat to update</param>
    /// <param name="statValue">The new value of the stat</param>
    private void UpdateStatDisplay(string statName, int statValue)
    {
        if (string.IsNullOrEmpty(statName) || statsContainer == null)
        {
            return;
        }

        // Find the stat label and update its text
        var statLabel = statsContainer.GetNodeOrNull<Label>(statName);
        if (statLabel != null)
        {
            statLabel.Text = $"{statName}: {statValue}";
        }
    }

    /// <summary>
    /// Update the character preview display.
    /// </summary>
    private void UpdatePreviewDisplay()
    {
        if (previewContainer == null || CreatedCharacter == null)
        {
            return;
        }

        // Update the preview sprite with the current appearance
        var previewSprite = previewContainer.GetNodeOrNull<Sprite2D>("CharacterPreview");
        if (previewSprite != null && selectedAppearance != null)
        {
            previewSprite.Texture = selectedAppearance.PreviewTexture;
        }
    }

    /// <summary>
    /// Update the confirm button state.
    /// </summary>
    private void UpdateConfirmButton()
    {
        if (confirmButton == null)
        {
            return;
        }

        // Enable the confirm button only if all required fields are filled
        confirmButton.Disabled = string.IsNullOrEmpty(characterName) ||
                                selectedClass == null ||
                                selectedAppearance == null;
    }

    /// <summary>
    /// Select a character class.
    /// </summary>
    /// <param name="characterClass">The character class to select</param>
    public void SelectClass(CharacterClass characterClass)
    {
        if (characterClass == null || !availableClasses.Contains(characterClass))
        {
            return;
        }

        selectedClass = characterClass;
        CreatedCharacter.CharacterClass = characterClass;

        // Apply class bonuses to stats
        if (characterStats != null && selectedClass.StatBonuses != null)
        {
            characterStats.ApplyBonuses(selectedClass.StatBonuses);
        }

        // Update the stats display
        UpdateStatsDisplay();

        // Emit the class selected signal
        EmitSignal(SignalName.ClassSelected, characterClass);
    }

    /// <summary>
    /// Select a character appearance.
    /// </summary>
    /// <param name="appearance">The character appearance to select</param>
    public void SelectAppearance(CharacterAppearance appearance)
    {
        if (appearance == null || !availableAppearances.Contains(appearance))
        {
            return;
        }

        selectedAppearance = appearance;
        CreatedCharacter.Appearance = appearance;

        // Update the preview display
        UpdatePreviewDisplay();

        // Emit the appearance selected signal
        EmitSignal(SignalName.AppearanceSelected, appearance);
    }

    /// <summary>
    /// Set the character's name.
    /// </summary>
    /// <param name="name">The character's name</param>
    public void SetCharacterName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        characterName = name;
        if (CreatedCharacter != null)
        {
            CreatedCharacter.Name = characterName;
        }

        // Update the confirm button state
        UpdateConfirmButton();

        // Emit the name changed signal
        EmitSignal(SignalName.NameChanged, name);
    }

    /// <summary>
    /// Callback when a character class is selected.
    /// </summary>
    /// <param name="characterClass">The selected character class</param>
    private void OnClassSelected(CharacterClass characterClass)
    {
        SelectClass(characterClass);
    }

    /// <summary>
    /// Callback when a character appearance is selected.
    /// </summary>
    /// <param name="appearance">The selected character appearance</param>
    private void OnAppearanceSelected(CharacterAppearance appearance)
    {
        SelectAppearance(appearance);
    }

    /// <summary>
    /// Callback when the character name text changes.
    /// </summary>
    /// <param name="newText">The new character name text</param>
    private void OnNameTextChanged(string newText)
    {
        SetCharacterName(newText);
    }

    /// <summary>
    /// Callback when the confirm button is pressed.
    /// </summary>
    private void OnConfirmButtonPressed()
    {
        // Validate the character creation
        if (string.IsNullOrEmpty(characterName) || selectedClass == null || selectedAppearance == null)
        {
            // Show an error message
            ShowErrorMessage("Please fill in all required fields.");
            return;
        }

        // Finalize the character creation
        FinalizeCharacterCreation();
    }

    /// <summary>
    /// Callback when the cancel button is pressed.
    /// </summary>
    private void OnCancelButtonPressed()
    {
        // Cancel character creation
        CancelCharacterCreation();
    }

    /// <summary>
    /// Finalize the character creation process.
    /// </summary>
    private void FinalizeCharacterCreation()
    {
        if (CreatedCharacter == null)
        {
            return;
        }

        // Set the final character properties
        CreatedCharacter.Name = characterName;
        CreatedCharacter.CharacterClass = selectedClass;
        CreatedCharacter.Appearance = selectedAppearance;

        // Emit the character confirmed signal
        EmitSignal(SignalName.CharacterConfirmed, CreatedCharacter);

        // Hide the character creation interface
        HideCreation();
    }

    /// <summary>
    /// Cancel the character creation process.
    /// </summary>
    public void CancelCharacterCreation()
    {
        // Emit the character creation cancelled signal
        EmitSignal(SignalName.CharacterCreationCancelled);

        // Hide the character creation interface
        HideCreation();
    }

    /// <summary>
    /// Hide the character creation interface.
    /// </summary>
    public void HideCreation()
    {
        Visible = false;
        CreatedCharacter = null;
        characterStats = null;
        selectedClass = null;
        selectedAppearance = null;
        characterName = "";
    }

    /// <summary>
    /// Show the character creation interface.
    /// </summary>
    public void ShowCreation()
    {
        Visible = true;
    }

    /// <summary>
    /// Toggle the character creation interface visibility.
    /// </summary>
    public void ToggleCreation()
    {
        if (Visible)
        {
            HideCreation();
        }
        else
        {
            ShowCreation();
        }
    }

    /// <summary>
    /// Show an error message in the character creation interface.
    /// </summary>
    /// <param name="message">The error message to show</param>
    public async void ShowErrorMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        // Show a temporary error message in the character creation interface
        // This would typically involve showing a label or panel with the error message

        // For example:
        // var errorLabel = GetNode<Label>("ErrorLabel");
        // errorLabel.Text = message;
        // errorLabel.Show();

        // Wait for a few seconds
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Hide the error message
        // errorLabel.Hide();
    }

    /// <summary>
    /// Show a success message in the character creation interface.
    /// </summary>
    /// <param name="message">The success message to show</param>
    public async void ShowSuccessMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        // Show a temporary success message in the character creation interface
        // This would typically involve showing a label or panel with the success message

        // For example:
        // var successLabel = GetNode<Label>("SuccessLabel");
        // successLabel.Text = message;
        // successLabel.AddThemeColorOverride("font_color", Colors.Green);
        // successLabel.Show();

        // Wait for a few seconds
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Hide the success message
        // successLabel.Hide();
    }

    /// <summary>
    /// Show an effect label (like stat increases or class bonuses).
    /// </summary>
    /// <param name="text">The text to show</param>
    /// <param name="position">The position to show the text at</param>
    /// <param name="color">The color of the text</param>
    public void ShowEffectLabel(string text, Vector2 position, Color color)
    {
        // Show a floating label at the specified position
        // This would typically involve creating a temporary label that floats upward and fades out

        // For example:
        // var label = new Label();
        // label.Text = text;
        // label.AddThemeColorOverride("font_color", color);
        // label.Position = position;
        // AddChild(label);

        // Create a tween to animate the label
        // var tween = CreateTween();
        // tween.TweenProperty(label, "position:y", position.Y - 50, 1.0f);
        // tween.Parallel().TweenProperty(label, "modulate:a", 0.0f, 1.0f);
        // tween.TweenCallback(new Callable(label, "queue_free"));
    }

    /// <summary>
    /// Randomize the character's appearance.
    /// </summary>
    public void RandomizeAppearance()
    {
        if (availableAppearances == null || availableAppearances.Count == 0)
        {
            return;
        }

        // Select a random appearance
        var randomIndex = GD.Randi() % availableAppearances.Count;
        var randomAppearance = availableAppearances[randomIndex];

        // Select the random appearance
        SelectAppearance(randomAppearance);
    }

    /// <summary>
    /// Randomize the character's class.
    /// </summary>
    public void RandomizeClass()
    {
        if (availableClasses == null || availableClasses.Count == 0)
        {
            return;
        }

        // Select a random class
        var randomIndex = GD.Randi() % availableClasses.Count;
        var randomClass = availableClasses[randomIndex];

        // Select the random class
        SelectClass(randomClass);
    }

    /// <summary>
    /// Randomize the character's name.
    /// </summary>
    public void RandomizeName()
    {
        // Generate a random name
        var randomName = GenerateRandomName();

        // Set the random name
        SetCharacterName(randomName);
    }

    /// <summary>
    /// Generate a random character name.
    /// </summary>
    /// <returns>A randomly generated character name</returns>
    private string GenerateRandomName()
    {
        // Simple random name generator
        var prefixes = new string[] { "Ael", "Bran", "Cor", "Dae", "Eld", "Fen", "Gor", "Hel", "Ith", "Jar" };
        var suffixes = new string[] { "drin", "mir", "nan", "orn", "ros", "sar", "tor", "vin", "wyn", "zar" };

        var randomPrefix = prefixes[GD.Randi() % prefixes.Length];
        var randomSuffix = suffixes[GD.Randi() % suffixes.Length];

        return randomPrefix + randomSuffix;
    }

    /// <summary>
    /// Fully randomize the character.
    /// </summary>
    public void RandomizeCharacter()
    {
        RandomizeAppearance();
        RandomizeClass();
        RandomizeName();
    }

    /// <summary>
    /// Reset the character creation to default values.
    /// </summary>
    public void ResetCharacter()
    {
        // Reset all character properties
        characterName = "";
        selectedClass = null;
        selectedAppearance = null;

        // Reset the character object
        if (CreatedCharacter != null)
        {
            CreatedCharacter.Name = "";
            CreatedCharacter.CharacterClass = null;
            CreatedCharacter.Appearance = null;
        }

        // Reset the stats
        if (characterStats != null)
        {
            characterStats.Reset();
        }

        // Update all displays
        UpdateAllDisplays();

        // Clear the name input field
        var nameInput = nameContainer?.GetNodeOrNull<LineEdit>("NameInput");
        if (nameInput != null)
        {
            nameInput.Text = "";
        }
    }

    /// <summary>
    /// Validate the character creation.
    /// </summary>
    /// <returns>True if the character creation is valid, false otherwise</returns>
    public bool ValidateCharacter()
    {
        // Check if all required fields are filled
        if (string.IsNullOrEmpty(characterName))
        {
            ShowErrorMessage("Please enter a character name.");
            return false;
        }

        if (selectedClass == null)
        {
            ShowErrorMessage("Please select a character class.");
            return false;
        }

        if (selectedAppearance == null)
        {
            ShowErrorMessage("Please select a character appearance.");
            return false;
        }

        // Check if the character name is valid
        if (characterName.Length < 3)
        {
            ShowErrorMessage("Character name must be at least 3 characters long.");
            return false;
        }

        if (characterName.Length > 20)
        {
            ShowErrorMessage("Character name must be no more than 20 characters long.");
            return false;
        }

        // Check if the character name contains only valid characters
        if (!IsValidCharacterName(characterName))
        {
            ShowErrorMessage("Character name contains invalid characters.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Check if a character name is valid.
    /// </summary>
    /// <param name="name">The character name to validate</param>
    /// <returns>True if the name is valid, false otherwise</returns>
    private bool IsValidCharacterName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        // Check if the name contains only letters, numbers, spaces, hyphens, and underscores
        foreach (char c in name)
        {
            if (!char.IsLetterOrDigit(c) && c != ' ' && c != '-' && c != '_')
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Get the available character classes.
    /// </summary>
    /// <returns>A list of available character classes</returns>
    public List<CharacterClass> GetAvailableClasses()
    {
        return new List<CharacterClass>(availableClasses);
    }

    /// <summary>
    /// Get the available character appearances.
    /// </summary>
    /// <returns>A list of available character appearances</returns>
    public List<CharacterAppearance> GetAvailableAppearances()
    {
        return new List<CharacterAppearance>(availableAppearances);
    }

    /// <summary>
    /// Get the currently selected character class.
    /// </summary>
    /// <returns>The currently selected character class, or null if none</returns>
    public CharacterClass GetSelectedClass()
    {
        return selectedClass;
    }

    /// <summary>
    /// Get the currently selected character appearance.
    /// </summary>
    /// <returns>The currently selected character appearance, or null if none</returns>
    public CharacterAppearance GetSelectedAppearance()
    {
        return selectedAppearance;
    }

    /// <summary>
    /// Get the character's name.
    /// </summary>
    /// <returns>The character's name</returns>
    public string GetCharacterName()
    {
        return characterName;
    }

    /// <summary>
    /// Get the character being created.
    /// </summary>
    /// <returns>The character being created</returns>
    public Character GetCreatedCharacter()
    {
        return CreatedCharacter;
    }

    /// <summary>
    /// Get the character's stats.
    /// </summary>
    /// <returns>The character's stats</returns>
    public CharacterStats GetCharacterStats()
    {
        return characterStats;
    }

    /// <summary>
    /// Apply a preset to the character.
    /// </summary>
    /// <param name="preset">The preset to apply</param>
    public void ApplyPreset(CharacterPreset preset)
    {
        if (preset == null)
        {
            return;
        }

        // Apply the preset to the character
        if (preset.Name != null)
        {
            SetCharacterName(preset.Name);
        }

        if (preset.CharacterClass != null && availableClasses.Contains(preset.CharacterClass))
        {
            SelectClass(preset.CharacterClass);
        }

        if (preset.Appearance != null && availableAppearances.Contains(preset.Appearance))
        {
            SelectAppearance(preset.Appearance);
        }

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Save the current character as a preset.
    /// </summary>
    /// <param name="presetName">The name of the preset to save</param>
    /// <returns>The saved preset</returns>
    public CharacterPreset SavePreset(string presetName)
    {
        if (string.IsNullOrEmpty(presetName) || CreatedCharacter == null)
        {
            return null;
        }

        // Create a new preset with the current character's properties
        var preset = new CharacterPreset();
        preset.Name = presetName;
        preset.CharacterClass = selectedClass;
        preset.Appearance = selectedAppearance;

        // Save the preset to a file or database
        // This would typically involve serializing the preset and saving it

        // For example:
        // var presetPath = $"user://presets/{presetName}.json";
        // var file = FileAccess.Open(presetPath, FileAccess.ModeFlags.Write);
        // file.StoreString(JsonSerializer.Serialize(preset));
        // file.Close();

        // Show a success message
        ShowSuccessMessage($"Preset '{presetName}' saved successfully.");

        return preset;
    }

    /// <summary>
    /// Load a preset and apply it to the character.
    /// </summary>
    /// <param name="presetName">The name of the preset to load</param>
    public void LoadPreset(string presetName)
    {
        if (string.IsNullOrEmpty(presetName))
        {
            return;
        }

        // Load the preset from a file or database
        // This would typically involve deserializing the preset

        // For example:
        // var presetPath = $"user://presets/{presetName}.json";
        // if (FileAccess.FileExists(presetPath))
        // {
        //     var file = FileAccess.Open(presetPath, FileAccess.ModeFlags.Read);
        //     var presetJson = file.GetAsText();
        //     file.Close();
        //
        //     var preset = JsonSerializer.Deserialize<CharacterPreset>(presetJson);
        //     ApplyPreset(preset);
        // }
        // else
        // {
        //     ShowErrorMessage($"Preset '{presetName}' not found.");
        // }
    }

    /// <summary>
    /// Delete a preset.
    /// </summary>
    /// <param name="presetName">The name of the preset to delete</param>
    public void DeletePreset(string presetName)
    {
        if (string.IsNullOrEmpty(presetName))
        {
            return;
        }

        // Delete the preset file or database entry
        // This would typically involve deleting the file

        // For example:
        // var presetPath = $"user://presets/{presetName}.json";
        // if (FileAccess.FileExists(presetPath))
        // {
        //     DirAccess.RemoveAbsolute(presetPath);
        //     ShowSuccessMessage($"Preset '{presetName}' deleted successfully.");
        // }
        // else
        // {
        //     ShowErrorMessage($"Preset '{presetName}' not found.");
        // }
    }

    /// <summary>
    /// Get a list of available presets.
    /// </summary>
    /// <returns>A list of available preset names</returns>
    public List<string> GetAvailablePresets()
    {
        // Get a list of available presets from files or database
        // This would typically involve listing files in a directory

        // For example:
        // var presets = new List<string>();
        // var dir = DirAccess.Open("user://presets");
        // if (dir != null)
        // {
        //     dir.ListDirBegin();
        //     var fileName = dir.GetNext();
        //     while (fileName != "")
        //     {
        //         if (!dir.CurrentIsDir() && fileName.EndsWith(".json"))
        //         {
        //             presets.Add(fileName.Replace(".json", ""));
        //         }
        //         fileName = dir.GetNext();
        //     }
        //     dir.ListDirEnd();
        // }
        // return presets;

        return new List<string>();
    }

    /// <summary>
    /// Import a character from another source.
    /// </summary>
    /// <param name="importData">The character data to import</param>
    public void ImportCharacter(CharacterImportData importData)
    {
        if (importData == null)
        {
            return;
        }

        // Import the character data
        if (importData.Name != null)
        {
            SetCharacterName(importData.Name);
        }

        // Find and select the matching class
        if (importData.ClassName != null)
        {
            var matchingClass = availableClasses.FirstOrDefault(c => c.Name == importData.ClassName);
            if (matchingClass != null)
            {
                SelectClass(matchingClass);
            }
        }

        // Find and select the matching appearance
        if (importData.AppearanceName != null)
        {
            var matchingAppearance = availableAppearances.FirstOrDefault(a => a.Name == importData.AppearanceName);
            if (matchingAppearance != null)
            {
                SelectAppearance(matchingAppearance);
            }
        }

        // Update all displays
        UpdateAllDisplays();

        // Show a success message
        ShowSuccessMessage("Character imported successfully.");
    }

    /// <summary>
    /// Export the current character to another format.
    /// </summary>
    /// <returns>The exported character data</returns>
    public CharacterExportData ExportCharacter()
    {
        if (CreatedCharacter == null)
        {
            return null;
        }

        // Create export data with the current character's properties
        var exportData = new CharacterExportData();
        exportData.Name = CreatedCharacter.Name;
        exportData.ClassName = CreatedCharacter.CharacterClass?.Name;
        exportData.AppearanceName = CreatedCharacter.Appearance?.Name;

        // Export the character data to a file or other format
        // This would typically involve serializing the data

        // For example:
        // var exportPath = $"user://exports/{CreatedCharacter.Name}.json";
        // var file = FileAccess.Open(exportPath, FileAccess.ModeFlags.Write);
        // file.StoreString(JsonSerializer.Serialize(exportData));
        // file.Close();

        // Show a success message
        ShowSuccessMessage($"Character '{CreatedCharacter.Name}' exported successfully.");

        return exportData;
    }
}
