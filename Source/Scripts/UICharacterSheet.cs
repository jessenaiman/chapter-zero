using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Container for the player character sheet display.
/// The UICharacterSheet manages the display of detailed character information,
/// including stats, equipment, skills, and other character-specific data.
/// It provides a comprehensive view of the player's character progression and current state.
/// </summary>
public partial class UICharacterSheet : Control
{
    /// <summary>
    /// Emitted when the player closes the character sheet.
    /// </summary>
    [Signal]
    public delegate void SheetClosedEventHandler();

    /// <summary>
    /// Emitted when the player selects an equipment slot.
    /// </summary>
    [Signal]
    public delegate void EquipmentSlotSelectedEventHandler(EquipmentSlot slot);

    /// <summary>
    /// Emitted when the player selects a skill.
    /// </summary>
    [Signal]
    public delegate void SkillSelectedEventHandler(Skill skill);

    /// <summary>
    /// The player character to display information for.
    /// </summary>
    public Character PlayerCharacter { get; private set; }

    /// <summary>
    /// Whether the character sheet is currently visible.
    /// </summary>
    public bool SheetVisible
    {
        get => Visible;
        set => Visible = value;
    }

    /// <summary>
    /// The container for character stats display.
    /// </summary>
    private Control statsContainer;

    /// <summary>
    /// The container for equipment display.
    /// </summary>
    private Control equipmentContainer;

    /// <summary>
    /// The container for skills display.
    /// </summary>
    private Control skillsContainer;

    /// <summary>
    /// The container for character portrait.
    /// </summary>
    private Control portraitContainer;

    /// <summary>
    /// The close button.
    /// </summary>
    private Button closeButton;

    /// <summary>
    /// Dictionary mapping equipment slots to their UI displays.
    /// </summary>
    private Dictionary<EquipmentSlot, Control> equipmentSlotDisplays = new Dictionary<EquipmentSlot, Control>();

    /// <summary>
    /// Dictionary mapping skills to their UI displays.
    /// </summary>
    private Dictionary<Skill, Control> skillDisplays = new Dictionary<Skill, Control>();

    public override void _Ready()
    {
        // Get references to child UI elements
        statsContainer = GetNode<Control>("StatsContainer");
        equipmentContainer = GetNode<Control>("EquipmentContainer");
        skillsContainer = GetNode<Control>("SkillsContainer");
        portraitContainer = GetNode<Control>("PortraitContainer");
        closeButton = GetNode<Button>("CloseButton");

        // Initially hide the character sheet
        Visible = false;

        // Connect to any necessary signals
        ConnectSignals();
    }

    /// <summary>
    /// Connect to necessary signals.
    /// </summary>
    private void ConnectSignals()
    {
        // Connect the close button
        if (closeButton != null)
        {
            closeButton.Pressed += OnCloseButtonPressed;
        }

        // Connect to character events
        // CharacterEvents.CharacterStatsChanged += OnCharacterStatsChanged;
        // CharacterEvents.EquipmentChanged += OnEquipmentChanged;
        // CharacterEvents.SkillsChanged += OnSkillsChanged;
    }

    /// <summary>
    /// Setup the UI character sheet with the given player character.
    /// </summary>
    /// <param name="character">The player character to display</param>
    public void Setup(Character character)
    {
        PlayerCharacter = character;

        // Clear any existing displays
        ClearDisplays();

        // Create displays for all character information
        if (PlayerCharacter != null)
        {
            CreateStatsDisplay(PlayerCharacter.Stats, statsContainer);
            CreateEquipmentDisplay(PlayerCharacter.Equipment, equipmentContainer);
            CreateSkillsDisplay(PlayerCharacter.Skills, skillsContainer);
            CreatePortraitDisplay(PlayerCharacter.Portrait, portraitContainer);
        }

        // Show the character sheet
        Visible = true;

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Clear all character sheet displays.
    /// </summary>
    private void ClearDisplays()
    {
        // Remove all existing equipment slot displays
        foreach (var display in equipmentSlotDisplays.Values)
        {
            display.QueueFree();
        }

        equipmentSlotDisplays.Clear();

        // Remove all existing skill displays
        foreach (var display in skillDisplays.Values)
        {
            display.QueueFree();
        }

        skillDisplays.Clear();

        // Clear containers
        if (statsContainer != null)
        {
            foreach (var child in statsContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (equipmentContainer != null)
        {
            foreach (var child in equipmentContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (skillsContainer != null)
        {
            foreach (var child in skillsContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (portraitContainer != null)
        {
            foreach (var child in portraitContainer.GetChildren())
            {
                child.QueueFree();
            }
        }
    }

    /// <summary>
    /// Create a display for character stats.
    /// </summary>
    /// <param name="stats">The character stats to display</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateStatsDisplay(CharacterStats stats, Control container)
    {
        if (stats == null || container == null)
        {
            return;
        }

        // Create a new stats display control
        var display = new Control();
        display.Name = "CharacterStats";

        // Add the display to the container
        container.AddChild(display);

        // Set up the display with initial values
        UpdateStatsDisplay(stats, display);
    }

    /// <summary>
    /// Create a display for character equipment.
    /// </summary>
    /// <param name="equipment">The character equipment to display</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateEquipmentDisplay(Equipment equipment, Control container)
    {
        if (equipment == null || container == null)
        {
            return;
        }

        // Create a new equipment display control
        var display = new Control();
        display.Name = "CharacterEquipment";

        // Add the display to the container
        container.AddChild(display);

        // Set up the display with initial values
        UpdateEquipmentDisplay(equipment, display);
    }

    /// <summary>
    /// Create a display for character skills.
    /// </summary>
    /// <param name="skills">The character skills to display</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateSkillsDisplay(Skills skills, Control container)
    {
        if (skills == null || container == null)
        {
            return;
        }

        // Create a new skills display control
        var display = new Control();
        display.Name = "CharacterSkills";

        // Add the display to the container
        container.AddChild(display);

        // Set up the display with initial values
        UpdateSkillsDisplay(skills, display);
    }

    /// <summary>
    /// Create a display for character portrait.
    /// </summary>
    /// <param name="portrait">The character portrait to display</param>
    /// <param name="container">The container to add the display to</param>
    private void CreatePortraitDisplay(Texture2D portrait, Control container)
    {
        if (portrait == null || container == null)
        {
            return;
        }

        // Create a new portrait display control
        var display = new TextureRect();
        display.Name = "CharacterPortrait";
        display.Texture = portrait;

        // Add the display to the container
        container.AddChild(display);

        // Set up the display with initial values
        UpdatePortraitDisplay(portrait, display);
    }

    /// <summary>
    /// Update a stats display with current values.
    /// </summary>
    /// <param name="stats">The character stats to update the display for</param>
    /// <param name="display">The display to update</param>
    private void UpdateStatsDisplay(CharacterStats stats, Control display)
    {
        if (stats == null || display == null)
        {
            return;
        }

        // Update the display with the character's current stats
        // This would typically involve updating labels, progress bars, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var nameLabel = display.GetNode<Label>("Name");
        // nameLabel.Text = stats.Name;

        // var levelLabel = display.GetNode<Label>("Level");
        // levelLabel.Text = $"Level: {stats.Level}";

        // var experienceLabel = display.GetNode<Label>("Experience");
        // experienceLabel.Text = $"XP: {stats.Experience}/{stats.NextLevelExperience}";

        // var healthLabel = display.GetNode<Label>("Health");
        // healthLabel.Text = $"Health: {stats.Health}/{stats.MaxHealth}";

        // var manaLabel = display.GetNode<Label>("Mana");
        // manaLabel.Text = $"Mana: {stats.Mana}/{stats.MaxMana}";

        // var strengthLabel = display.GetNode<Label>("Strength");
        // strengthLabel.Text = $"Strength: {stats.Strength}";

        // var agilityLabel = display.GetNode<Label>("Agility");
        // agilityLabel.Text = $"Agility: {stats.Agility}";

        // var intelligenceLabel = display.GetNode<Label>("Intelligence");
        // intelligenceLabel.Text = $"Intelligence: {stats.Intelligence}";

        // var vitalityLabel = display.GetNode<Label>("Vitality");
        // vitalityLabel.Text = $"Vitality: {stats.Vitality}";

        // var luckLabel = display.GetNode<Label>("Luck");
        // luckLabel.Text = $"Luck: {stats.Luck}";
    }

    /// <summary>
    /// Update an equipment display with current values.
    /// </summary>
    /// <param name="equipment">The character equipment to update the display for</param>
    /// <param name="display">The display to update</param>
    private void UpdateEquipmentDisplay(Equipment equipment, Control display)
    {
        if (equipment == null || display == null)
        {
            return;
        }

        // Update the display with the character's current equipment
        // This would typically involve updating labels, icons, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var weaponLabel = display.GetNode<Label>("Weapon");
        // weaponLabel.Text = $"Weapon: {equipment.Weapon?.Name ?? "None"}";

        // var armorLabel = display.GetNode<Label>("Armor");
        // armorLabel.Text = $"Armor: {equipment.Armor?.Name ?? "None"}";

        // var accessoryLabel = display.GetNode<Label>("Accessory");
        // accessoryLabel.Text = $"Accessory: {equipment.Accessory?.Name ?? "None"}";
    }

    /// <summary>
    /// Update a skills display with current values.
    /// </summary>
    /// <param name="skills">The character skills to update the display for</param>
    /// <param name="display">The display to update</param>
    private void UpdateSkillsDisplay(Skills skills, Control display)
    {
        if (skills == null || display == null)
        {
            return;
        }

        // Update the display with the character's current skills
        // This would typically involve updating labels, progress bars, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var skillsList = display.GetNode<VBoxContainer>("SkillsList");
        // foreach (var child in skillsList.GetChildren())
        // {
        //     child.QueueFree();
        // }

        // foreach (var skill in skills.SkillList)
        // {
        //     var skillLabel = new Label();
        //     skillLabel.Text = $"{skill.Name} (Level {skill.Level})";
        //     skillsList.AddChild(skillLabel);
        // }
    }

    /// <summary>
    /// Update a portrait display with current values.
    /// </summary>
    /// <param name="portrait">The character portrait to update the display for</param>
    /// <param name="display">The display to update</param>
    private void UpdatePortraitDisplay(Texture2D portrait, Control display)
    {
        if (portrait == null || display == null)
        {
            return;
        }

        // Update the display with the character's current portrait
        // This would typically involve updating a TextureRect or similar control
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var portraitRect = display.GetNode<TextureRect>("Portrait");
        // portraitRect.Texture = portrait;
    }

    /// <summary>
    /// Update all character sheet displays.
    /// </summary>
    public void UpdateAllDisplays()
    {
        if (PlayerCharacter == null)
        {
            return;
        }

        // Update the stats display
        if (PlayerCharacter.Stats != null && statsContainer != null)
        {
            var statsDisplay = statsContainer.GetNodeOrNull<Control>("CharacterStats");
            if (statsDisplay != null)
            {
                UpdateStatsDisplay(PlayerCharacter.Stats, statsDisplay);
            }
        }

        // Update the equipment display
        if (PlayerCharacter.Equipment != null && equipmentContainer != null)
        {
            var equipmentDisplay = equipmentContainer.GetNodeOrNull<Control>("CharacterEquipment");
            if (equipmentDisplay != null)
            {
                UpdateEquipmentDisplay(PlayerCharacter.Equipment, equipmentDisplay);
            }
        }

        // Update the skills display
        if (PlayerCharacter.Skills != null && skillsContainer != null)
        {
            var skillsDisplay = skillsContainer.GetNodeOrNull<Control>("CharacterSkills");
            if (skillsDisplay != null)
            {
                UpdateSkillsDisplay(PlayerCharacter.Skills, skillsDisplay);
            }
        }

        // Update the portrait display
        if (PlayerCharacter.Portrait != null && portraitContainer != null)
        {
            var portraitDisplay = portraitContainer.GetNodeOrNull<TextureRect>("CharacterPortrait");
            if (portraitDisplay != null)
            {
                UpdatePortraitDisplay(PlayerCharacter.Portrait, portraitDisplay);
            }
        }
    }

    /// <summary>
    /// Get the display for a specific equipment slot.
    /// </summary>
    /// <param name="slot">The equipment slot to get the display for</param>
    /// <returns>The display control for the equipment slot, or null if not found</returns>
    public Control GetEquipmentSlotDisplay(EquipmentSlot slot)
    {
        if (slot == null)
        {
            return null;
        }

        return equipmentSlotDisplays.GetValueOrDefault(slot, null);
    }

    /// <summary>
    /// Get the display for a specific skill.
    /// </summary>
    /// <param name="skill">The skill to get the display for</param>
    /// <returns>The display control for the skill, or null if not found</returns>
    public Control GetSkillDisplay(Skill skill)
    {
        if (skill == null)
        {
            return null;
        }

        return skillDisplays.GetValueOrDefault(skill, null);
    }

    /// <summary>
    /// Refresh the character sheet with current character data.
    /// </summary>
    public void Refresh()
    {
        // Clear and recreate all displays
        ClearDisplays();

        if (PlayerCharacter != null)
        {
            if (PlayerCharacter.Stats != null)
            {
                CreateStatsDisplay(PlayerCharacter.Stats, statsContainer);
            }

            if (PlayerCharacter.Equipment != null)
            {
                CreateEquipmentDisplay(PlayerCharacter.Equipment, equipmentContainer);
            }

            if (PlayerCharacter.Skills != null)
            {
                CreateSkillsDisplay(PlayerCharacter.Skills, skillsContainer);
            }

            if (PlayerCharacter.Portrait != null)
            {
                CreatePortraitDisplay(PlayerCharacter.Portrait, portraitContainer);
            }
        }

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Show a message in the character sheet.
    /// </summary>
    /// <param name="message">The message to show</param>
    /// <param name="duration">The duration to show the message for</param>
    public async void ShowMessage(string message, float duration = 2.0f)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        // Show a temporary message in the character sheet
        // This would typically involve showing a label or panel with the message

        // For example:
        // var messageLabel = GetNode<Label>("MessageLabel");
        // messageLabel.Text = message;
        // messageLabel.Show();

        // Wait for the specified duration
        await Task.Delay(TimeSpan.FromSeconds(duration));

        // Hide the message
        // messageLabel.Hide();
    }

    /// <summary>
    /// Show an effect label (like stat increases or skill upgrades).
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
    /// Hide the character sheet.
    /// </summary>
    public void HideSheet()
    {
        Visible = false;
        EmitSignal(SignalName.SheetClosed);
    }

    /// <summary>
    /// Show the character sheet.
    /// </summary>
    public void ShowSheet()
    {
        Visible = true;
    }

    /// <summary>
    /// Toggle the character sheet visibility.
    /// </summary>
    public void ToggleSheet()
    {
        if (Visible)
        {
            HideSheet();
        }
        else
        {
            ShowSheet();
        }
    }

    /// <summary>
    /// Callback when the close button is pressed.
    /// </summary>
    private void OnCloseButtonPressed()
    {
        HideSheet();
    }

    /// <summary>
    /// Callback when character stats change.
    /// </summary>
    private void OnCharacterStatsChanged(CharacterStats stats)
    {
        if (stats == null || PlayerCharacter == null || PlayerCharacter.Stats != stats)
        {
            return;
        }

        // Update the stats display
        if (statsContainer != null)
        {
            var statsDisplay = statsContainer.GetNodeOrNull<Control>("CharacterStats");
            if (statsDisplay != null)
            {
                UpdateStatsDisplay(stats, statsDisplay);
            }
        }

        // Show a notification
        ShowMessage("Character stats updated.");
    }

    /// <summary>
    /// Callback when equipment changes.
    /// </summary>
    private void OnEquipmentChanged(Equipment equipment)
    {
        if (equipment == null || PlayerCharacter == null || PlayerCharacter.Equipment != equipment)
        {
            return;
        }

        // Update the equipment display
        if (equipmentContainer != null)
        {
            var equipmentDisplay = equipmentContainer.GetNodeOrNull<Control>("CharacterEquipment");
            if (equipmentDisplay != null)
            {
                UpdateEquipmentDisplay(equipment, equipmentDisplay);
            }
        }

        // Show a notification
        ShowMessage("Equipment updated.");
    }

    /// <summary>
    /// Callback when skills change.
    /// </summary>
    private void OnSkillsChanged(Skills skills)
    {
        if (skills == null || PlayerCharacter == null || PlayerCharacter.Skills != skills)
        {
            return;
        }

        // Update the skills display
        if (skillsContainer != null)
        {
            var skillsDisplay = skillsContainer.GetNodeOrNull<Control>("CharacterSkills");
            if (skillsDisplay != null)
            {
                UpdateSkillsDisplay(skills, skillsDisplay);
            }
        }

        // Show a notification
        ShowMessage("Skills updated.");
    }

    /// <summary>
    /// Select an equipment slot.
    /// </summary>
    /// <param name="slot">The equipment slot to select</param>
    public void SelectEquipmentSlot(EquipmentSlot slot)
    {
        if (slot == null)
        {
            return;
        }

        // Emit the equipment slot selected signal
        EmitSignal(SignalName.EquipmentSlotSelected, slot);
    }

    /// <summary>
    /// Select a skill.
    /// </summary>
    /// <param name="skill">The skill to select</param>
    public void SelectSkill(Skill skill)
    {
        if (skill == null)
        {
            return;
        }

        // Emit the skill selected signal
        EmitSignal(SignalName.SkillSelected, skill);
    }
}
