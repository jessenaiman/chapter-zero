
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Container for the player equipment display.
/// The UIEquipment manages the display and interaction of player equipment items,
/// allowing players to view, equip, and unequip items. It handles the presentation
/// of equipment slots, current equipment, and equipment management interfaces.
/// It provides a comprehensive view of the player's equipment and allows for easy
/// management of their gear.
/// </summary>
public partial class UIEquipment : Control
{
    /// <summary>
    /// Emitted when the player equips an item.
    /// </summary>
    [Signal]
    public delegate void ItemEquippedEventHandler(Item item, EquipmentSlot slot);

    /// <summary>
    /// Emitted when the player unequips an item.
    /// </summary>
    [Signal]
    public delegate void ItemUnequippedEventHandler(Item item, EquipmentSlot slot);

    /// <summary>
    /// Emitted when the player selects an equipment slot.
    /// </summary>
    [Signal]
    public delegate void EquipmentSlotSelectedEventHandler(EquipmentSlot slot);

    /// <summary>
    /// The player character to manage equipment for.
    /// </summary>
    public Character PlayerCharacter { get; private set; }

    /// <summary>
    /// Whether the equipment UI is currently visible.
    /// </summary>
    public bool EquipmentVisible
    {
        get => Visible;
        set => Visible = value;
    }

    /// <summary>
    /// The container for equipment slots display.
    /// </summary>
    private Control equipmentSlotsContainer;

    /// <summary>
    /// The container for equipment details display.
    /// </summary>
    private Control equipmentDetailsContainer;

    /// <summary>
    /// The container for equipment actions display.
    /// </summary>
    private Control equipmentActionsContainer;

    /// <summary>
    /// The close button.
    /// </summary>
    private Button closeButton;

    /// <summary>
    /// Dictionary mapping equipment slots to their UI displays.
    /// </summary>
    private Dictionary<EquipmentSlot, Control> equipmentSlotDisplays = new Dictionary<EquipmentSlot, Control>();

    /// <summary>
    /// Dictionary mapping equipped items to their UI displays.
    /// </summary>
    private Dictionary<Item, Control> equippedItemDisplays = new Dictionary<Item, Control>();

    /// <summary>
    /// The currently selected equipment slot.
    /// </summary>
    private EquipmentSlot selectedSlot;

    public override void _Ready()
    {
        // Get references to child UI elements
        equipmentSlotsContainer = GetNode<Control>("EquipmentSlotsContainer");
        equipmentDetailsContainer = GetNode<Control>("EquipmentDetailsContainer");
        equipmentActionsContainer = GetNode<Control>("EquipmentActionsContainer");
        closeButton = GetNode<Button>("CloseButton");

        // Initially hide the equipment UI
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

        // Connect to equipment events
        // EquipmentEvents.ItemEquipped += OnItemEquipped;
        // EquipmentEvents.ItemUnequipped += OnItemUnequipped;
        // EquipmentEvents.EquipmentSlotSelected += OnEquipmentSlotSelected;
    }

    /// <summary>
    /// Setup the UI equipment with the given player character.
    /// </summary>
    /// <param name="character">The player character to manage equipment for</param>
    public void Setup(Character character)
    {
        PlayerCharacter = character;

        // Clear any existing displays
        ClearDisplays();

        // Create displays for all equipment slots
        if (PlayerCharacter != null && PlayerCharacter.Equipment != null)
        {
            CreateEquipmentSlotsDisplay(PlayerCharacter.Equipment.Slots, equipmentSlotsContainer);
        }

        // Show the equipment UI
        Visible = true;

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Clear all equipment displays.
    /// </summary>
    private void ClearDisplays()
    {
        // Remove all existing equipment slot displays
        foreach (var display in equipmentSlotDisplays.Values)
        {
            display.QueueFree();
        }

        equipmentSlotDisplays.Clear();

        // Remove all existing equipped item displays
        foreach (var display in equippedItemDisplays.Values)
        {
            display.QueueFree();
        }

        equippedItemDisplays.Clear();

        // Clear containers
        if (equipmentSlotsContainer != null)
        {
            foreach (var child in equipmentSlotsContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (equipmentDetailsContainer != null)
        {
            foreach (var child in equipmentDetailsContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (equipmentActionsContainer != null)
        {
            foreach (var child in equipmentActionsContainer.GetChildren())
            {
                child.QueueFree();
            }
        }
    }

    /// <summary>
    /// Create a display for equipment slots.
    /// </summary>
    /// <param name="slots">The equipment slots to display</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateEquipmentSlotsDisplay(List<EquipmentSlot> slots, Control container)
    {
        if (slots == null || container == null)
        {
            return;
        }

        // Create displays for all equipment slots
        foreach (var slot in slots)
        {
            CreateEquipmentSlotDisplay(slot, container);
        }
    }

    /// <summary>
    /// Create a display for an equipment slot.
    /// </summary>
    /// <param name="slot">The equipment slot to create a display for</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateEquipmentSlotDisplay(EquipmentSlot slot, Control container)
    {
        if (slot == null || container == null)
        {
            return;
        }

        // Create a new equipment slot display control
        var display = new Control();
        display.Name = slot.Name;

        // Add the display to the container
        container.AddChild(display);

        // Store the display in the dictionary
        equipmentSlotDisplays[slot] = display;

        // Set up the display with initial values
        UpdateEquipmentSlotDisplay(slot, display);

        // Connect input events to allow selecting the equipment slot
        display.GuiInput += (inputEvent) => OnEquipmentSlotDisplayInput(slot, inputEvent);
    }

    /// <summary>
    /// Create a display for an equipped item.
    /// </summary>
    /// <param name="item">The equipped item to create a display for</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateEquippedItemDisplay(Item item, Control container)
    {
        if (item == null || container == null)
        {
            return;
        }

        // Create a new equipped item display control
        var display = new Control();
        display.Name = item.Name;

        // Add the display to the container
        container.AddChild(display);

        // Store the display in the dictionary
        equippedItemDisplays[item] = display;

        // Set up the display with initial values
        UpdateEquippedItemDisplay(item, display);

        // Connect input events to allow selecting the equipped item
        display.GuiInput += (inputEvent) => OnEquippedItemDisplayInput(item, inputEvent);
    }

    /// <summary>
    /// Update an equipment slot display with current values.
    /// </summary>
    /// <param name="slot">The equipment slot to update the display for</param>
    /// <param name="display">The display to update</param>
    private void UpdateEquipmentSlotDisplay(EquipmentSlot slot, Control display)
    {
        if (slot == null || display == null)
        {
            return;
        }

        // Update the display with the equipment slot's current properties
        // This would typically involve updating labels, icons, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var nameLabel = display.GetNode<Label>("Name");
        // nameLabel.Text = slot.Name;

        // var itemLabel = display.GetNode<Label>("Item");
        // itemLabel.Text = slot.Item?.Name ?? "Empty";

        // var icon = display.GetNode<TextureRect>("Icon");
        // icon.Texture = slot.Item?.Icon ?? slot.EmptyIcon;
    }

    /// <summary>
    /// Update an equipped item display with current values.
    /// </summary>
    /// <param name="item">The equipped item to update the display for</param>
    /// <param name="display">The display to update</param>
    private void UpdateEquippedItemDisplay(Item item, Control display)
    {
        if (item == null || display == null)
        {
            return;
        }

        // Update the display with the equipped item's current properties
        // This would typically involve updating labels, icons, stats, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var nameLabel = display.GetNode<Label>("Name");
        // nameLabel.Text = item.Name;

        // var descriptionLabel = display.GetNode<Label>("Description");
        // descriptionLabel.Text = item.Description;

        // var statsLabel = display.GetNode<Label>("Stats");
        // statsLabel.Text = string.Join(", ", item.Stats.Select(s => $"{s.Key}: {s.Value}"));

        // var icon = display.GetNode<TextureRect>("Icon");
        // icon.Texture = item.Icon;
    }

    /// <summary>
    /// Update the equipment details panel.
    /// </summary>
    /// <param name="item">The item to display details for</param>
    private void UpdateEquipmentDetailsPanel(Item item)
    {
        if (item == null || equipmentDetailsContainer == null)
        {
            return;
        }

        // Update the details panel with the item's properties
        // This would typically involve updating labels, descriptions, stats, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var nameLabel = equipmentDetailsContainer.GetNode<Label>("Name");
        // nameLabel.Text = item.Name;

        // var descriptionLabel = equipmentDetailsContainer.GetNode<Label>("Description");
        // descriptionLabel.Text = item.Description;

        // var typeLabel = equipmentDetailsContainer.GetNode<Label>("Type");
        // typeLabel.Text = item.Type.ToString();

        // var valueLabel = equipmentDetailsContainer.GetNode<Label>("Value");
        // valueLabel.Text = $"Value: {item.Value}";

        // var statsContainer = equipmentDetailsContainer.GetNode<VBoxContainer>("Stats");
        // foreach (var child in statsContainer.GetChildren())
        // {
        //     child.QueueFree();
        // }

        // foreach (var stat in item.Stats)
        // {
        //     var statLabel = new Label();
        //     statLabel.Text = $"{stat.Key}: {stat.Value}";
        //     statsContainer.AddChild(statLabel);
        // }
    }

    /// <summary>
    /// Update the equipment actions panel.
    /// </summary>
    /// <param name="item">The item to show actions for</param>
    /// <param name="slot">The equipment slot the item is in</param>
    private void UpdateEquipmentActionsPanel(Item item, EquipmentSlot slot)
    {
        if (equipmentActionsContainer == null)
        {
            return;
        }

        // Update the actions panel with available actions for the item
        // This would typically involve showing/hiding buttons based on the item type
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var equipButton = equipmentActionsContainer.GetNode<Button>("Equip");
        // var unequipButton = equipmentActionsContainer.GetNode<Button>("Unequip");
        // var dropButton = equipmentActionsContainer.GetNode<Button>("Drop");

        // equipButton.Visible = item != null && slot != null && item.IsEquipment;
        // unequipButton.Visible = item != null && slot != null && slot.Item == item;
        // dropButton.Visible = item != null;

        // Connect button signals
        // equipButton.Pressed += () => OnEquipButtonPressed(item, slot);
        // unequipButton.Pressed += () => OnUnequipButtonPressed(item, slot);
        // dropButton.Pressed += () => OnDropButtonPressed(item);
    }

    /// <summary>
    /// Update all equipment displays.
    /// </summary>
    public void UpdateAllDisplays()
    {
        if (PlayerCharacter == null || PlayerCharacter.Equipment == null)
        {
            return;
        }

        // Update all equipment slot displays
        foreach (var kvp in equipmentSlotDisplays)
        {
            UpdateEquipmentSlotDisplay(kvp.Key, kvp.Value);
        }

        // Update all equipped item displays
        foreach (var kvp in equippedItemDisplays)
        {
            UpdateEquippedItemDisplay(kvp.Key, kvp.Value);
        }

        // If there's a selected slot, update the details panel
        if (selectedSlot != null)
        {
            UpdateEquipmentDetailsPanel(selectedSlot.Item);
            UpdateEquipmentActionsPanel(selectedSlot.Item, selectedSlot);
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
    /// Get the display for a specific equipped item.
    /// </summary>
    /// <param name="item">The equipped item to get the display for</param>
    /// <returns>The display control for the equipped item, or null if not found</returns>
    public Control GetEquippedItemDisplay(Item item)
    {
        if (item == null)
        {
            return null;
        }

        return equippedItemDisplays.GetValueOrDefault(item, null);
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

        selectedSlot = slot;

        // Update the details panel with the slot's item
        UpdateEquipmentDetailsPanel(slot.Item);

        // Update the actions panel
        UpdateEquipmentActionsPanel(slot.Item, slot);

        // Emit the equipment slot selected signal
        EmitSignal(SignalName.EquipmentSlotSelected, slot);
    }

    /// <summary>
    /// Equip an item to a specific slot.
    /// </summary>
    /// <param name="item">The item to equip</param>
    /// <param name="slot">The slot to equip the item to</param>
    public void EquipItem(Item item, EquipmentSlot slot)
    {
        if (item == null || slot == null || PlayerCharacter == null || PlayerCharacter.Equipment == null)
        {
            return;
        }

        // Equip the item to the slot
        PlayerCharacter.Equipment.EquipItem(item, slot);

        // Update the equipment slot display
        if (equipmentSlotDisplays.ContainsKey(slot))
        {
            UpdateEquipmentSlotDisplay(slot, equipmentSlotDisplays[slot]);
        }

        // Update the equipped item display
        if (!equippedItemDisplays.ContainsKey(item))
        {
            CreateEquippedItemDisplay(item, equipmentSlotsContainer);
        }
        else
        {
            UpdateEquippedItemDisplay(item, equippedItemDisplays[item]);
        }

        // Emit the item equipped signal
        EmitSignal(SignalName.ItemEquipped, item, slot);

        // Show a notification
        ShowMessage($"Equipped {item.Name} to {slot.Name}.");
    }

    /// <summary>
    /// Unequip an item from a specific slot.
    /// </summary>
    /// <param name="slot">The slot to unequip the item from</param>
    public void UnequipItem(EquipmentSlot slot)
    {
        if (slot == null || PlayerCharacter == null || PlayerCharacter.Equipment == null)
        {
            return;
        }

        var item = slot.Item;
        if (item == null)
        {
            return;
        }

        // Unequip the item from the slot
        PlayerCharacter.Equipment.UnequipItem(slot);

        // Update the equipment slot display
        if (equipmentSlotDisplays.ContainsKey(slot))
        {
            UpdateEquipmentSlotDisplay(slot, equipmentSlotDisplays[slot]);
        }

        // Remove the equipped item display
        if (equippedItemDisplays.ContainsKey(item))
        {
            var display = equippedItemDisplays[item];
            display.QueueFree();
            equippedItemDisplays.Remove(item);
        }

        // Emit the item unequipped signal
        EmitSignal(SignalName.ItemUnequipped, item, slot);

        // Show a notification
        ShowMessage($"Unequipped {item.Name} from {slot.Name}.");
    }

    /// <summary>
    /// Drop an item from the equipment.
    /// </summary>
    /// <param name="item">The item to drop</param>
    public void DropItem(Item item)
    {
        if (item == null || PlayerCharacter == null || PlayerCharacter.Equipment == null)
        {
            return;
        }

        // Remove the item from the equipment
        PlayerCharacter.Equipment.RemoveItem(item);

        // Remove the equipped item display
        if (equippedItemDisplays.ContainsKey(item))
        {
            var display = equippedItemDisplays[item];
            display.QueueFree();
            equippedItemDisplays.Remove(item);
        }

        // Show a notification
        ShowMessage($"Dropped {item.Name}.");
    }

    /// <summary>
    /// Show a message in the equipment UI.
    /// </summary>
    /// <param name="message">The message to show</param>
    /// <param name="duration">The duration to show the message for</param>
    public async void ShowMessage(string message, float duration = 2.0f)
    {
        if (string.IsNullOrEmpty(message) || equipmentDetailsContainer == null)
        {
            return;
        }

        // Show a temporary message in the equipment UI
        // This would typically involve showing a label or panel with the message

        // For example:
        // var messageLabel = equipmentDetailsContainer.GetNode<Label>("Message");
        // messageLabel.Text = message;
        // messageLabel.Show();

        // Wait for the specified duration
        await Task.Delay(TimeSpan.FromSeconds(duration));

        // Hide the message
        // messageLabel.Hide();
    }

    /// <summary>
    /// Show an effect label (like equipment changes or stat updates).
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
    /// Hide the equipment UI.
    /// </summary>
    public void HideEquipment()
    {
        Visible = false;
        selectedSlot = null;
        EmitSignal(SignalName.SheetClosed);
    }

    /// <summary>
    /// Show the equipment UI.
    /// </summary>
    public void ShowEquipment()
    {
        Visible = true;
        selectedSlot = null;
    }

    /// <summary>
    /// Toggle the equipment UI visibility.
    /// </summary>
    public void ToggleEquipment()
    {
        if (Visible)
        {
            HideEquipment();
        }
        else
        {
            ShowEquipment();
        }
    }

    /// <summary>
    /// Refresh the equipment UI with current character data.
    /// </summary>
    public void Refresh()
    {
        // Clear and recreate all displays
        ClearDisplays();

        if (PlayerCharacter != null && PlayerCharacter.Equipment != null)
        {
            if (PlayerCharacter.Equipment.Slots != null)
            {
                foreach (var slot in PlayerCharacter.Equipment.Slots)
                {
                    CreateEquipmentSlotDisplay(slot, equipmentSlotsContainer);
                }
            }

            if (PlayerCharacter.Equipment.EquippedItems != null)
            {
                foreach (var item in PlayerCharacter.Equipment.EquippedItems)
                {
                    CreateEquippedItemDisplay(item, equipmentSlotsContainer);
                }
            }
        }

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Callback when the close button is pressed.
    /// </summary>
    private void OnCloseButtonPressed()
    {
        HideEquipment();
    }

    /// <summary>
    /// Callback when input is received on an equipment slot display.
    /// </summary>
    /// <param name="slot">The equipment slot associated with the display</param>
    /// <param name="inputEvent">The input event</param>
    private void OnEquipmentSlotDisplayInput(EquipmentSlot slot, InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            // Select the equipment slot when clicked
            SelectEquipmentSlot(slot);
        }
    }

    /// <summary>
    /// Callback when input is received on an equipped item display.
    /// </summary>
    /// <param name="item">The equipped item associated with the display</param>
    /// <param name="inputEvent">The input event</param>
    private void OnEquippedItemDisplayInput(Item item, InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            // Find the slot that contains this item
            var slot = PlayerCharacter?.Equipment?.Slots?.FirstOrDefault(s => s.Item == item);
            if (slot != null)
            {
                // Select the equipment slot when clicked
                SelectEquipmentSlot(slot);
            }
        }
    }

    /// <summary>
    /// Callback when an item is equipped.
    /// </summary>
    /// <param name="item">The item that was equipped</param>
    /// <param name="slot">The slot the item was equipped to</param>
    private void OnItemEquipped(Item item, EquipmentSlot slot)
    {
        if (item == null || slot == null || PlayerCharacter == null || PlayerCharacter.Equipment == null)
        {
            return;
        }

        // Update the equipment slot display
        if (equipmentSlotDisplays.ContainsKey(slot))
        {
            UpdateEquipmentSlotDisplay(slot, equipmentSlotDisplays[slot]);
        }

        // Create or update the equipped item display
        if (!equippedItemDisplays.ContainsKey(item))
        {
            CreateEquippedItemDisplay(item, equipmentSlotsContainer);
        }
        else
        {
            UpdateEquippedItemDisplay(item, equippedItemDisplays[item]);
        }

        // Update all displays
        UpdateAllDisplays();

        // Show a notification
        ShowMessage($"Equipped {item.Name} to {slot.Name}.");
    }

    /// <summary>
    /// Callback when an item is unequipped.
    /// </summary>
    /// <param name="item">The item that was unequipped</param>
    /// <param name="slot">The slot the item was unequipped from</param>
    private void OnItemUnequipped(Item item, EquipmentSlot slot)
    {
        if (item == null || slot == null || PlayerCharacter == null || PlayerCharacter.Equipment == null)
        {
            return;
        }

        // Update the equipment slot display
        if (equipmentSlotDisplays.ContainsKey(slot))
        {
            UpdateEquipmentSlotDisplay(slot, equipmentSlotDisplays[slot]);
        }

        // Remove the equipped item display
        if (equippedItemDisplays.ContainsKey(item))
        {
            var display = equippedItemDisplays[item];
            display.QueueFree();
            equippedItemDisplays.Remove(item);
        }

        // Update all displays
        UpdateAllDisplays();

        // Show a notification
        ShowMessage($"Unequipped {item.Name} from {slot.Name}.");
    }

    /// <summary>
    /// Callback when an equipment slot is selected.
    /// </summary>
    /// <param name="slot">The selected equipment slot</param>
    private void OnEquipmentSlotSelected(EquipmentSlot slot)
    {
        if (slot == null)
        {
            return;
        }

        selectedSlot = slot;

        // Update the details panel with the slot's item
        UpdateEquipmentDetailsPanel(slot.Item);

        // Update the actions panel
        UpdateEquipmentActionsPanel(slot.Item, slot);
    }

    /// <summary>
    /// Callback when the equip button is pressed.
    /// </summary>
    /// <param name="item">The item to equip</param>
    /// <param name="slot">The slot to equip the item to</param>
    private void OnEquipButtonPressed(Item item, EquipmentSlot slot)
    {
        if (item == null || slot == null)
        {
            return;
        }

        EquipItem(item, slot);
    }

    /// <summary>
    /// Callback when the unequip button is pressed.
    /// </summary>
    /// <param name="item">The item to unequip</param>
    /// <param name="slot">The slot to unequip the item from</param>
    private void OnUnequipButtonPressed(Item item, EquipmentSlot slot)
    {
        if (item == null || slot == null)
        {
            return;
        }

        UnequipItem(slot);
    }

    /// <summary>
    /// Callback when the drop button is pressed.
    /// </summary>
    /// <param name="item">The item to drop</param>
    private void OnDropButtonPressed(Item item)
    {
        if (item == null)
        {
            return;
        }

        DropItem(item);
    }

    /// <summary>
    /// Get all equipment slot displays.
    /// </summary>
    /// <returns>A dictionary mapping equipment slots to their UI displays</returns>
    public Dictionary<EquipmentSlot, Control> GetEquipmentSlotDisplays()
    {
        return new Dictionary<EquipmentSlot, Control>(equipmentSlotDisplays);
    }

    /// <summary>
    /// Get all equipped item displays.
    /// </summary>
    /// <returns>A dictionary mapping equipped items to their UI displays</returns>
    public Dictionary<Item, Control> GetEquippedItemDisplays()
    {
        return new Dictionary<Item, Control>(equippedItemDisplays);
    }

    /// <summary>
    /// Get the currently selected equipment slot.
    /// </summary>
    /// <returns>The currently selected equipment slot, or null if none</returns>
    public EquipmentSlot GetSelectedSlot()
    {
        return selectedSlot;
    }

    /// <summary>
    /// Set the currently selected equipment slot.
    /// </summary>
    /// <param name="slot">The equipment slot to select</param>
    public void SetSelectedSlot(EquipmentSlot slot)
    {
        SelectEquipmentSlot(slot);
    }

    /// <summary>
    /// Get the player character.
    /// </summary>
    /// <returns>The player character, or null if not found</returns>
    public Character GetPlayerCharacter()
    {
        return PlayerCharacter;
    }

    /// <summary>
    /// Set the player character.
    /// </summary>
    /// <param name="character">The player character to set</param>
    public void SetPlayerCharacter(Character character)
    {
        Setup(character);
    }

    /// <summary>
    /// Get the equipment slots container.
    /// </summary>
    /// <returns>The equipment slots container, or null if not found</returns>
    public Control GetEquipmentSlotsContainer()
    {
        return equipmentSlotsContainer;
    }

    /// <summary>
    /// Get the equipment details container.
    /// </summary>
    /// <returns>The equipment details container, or null if not found</returns>
    public Control GetEquipmentDetailsContainer()
    {
        return equipmentDetailsContainer;
    }

    /// <summary>
    /// Get the equipment actions container.
    /// </summary>
    /// <returns>The equipment actions container, or null if not found</returns>
    public Control GetEquipmentActionsContainer()
    {
        return equipmentActionsContainer;
    }

    /// <summary>
    /// Get the close button.
    /// </summary>
    /// <returns>The close button, or null if not found</returns>
    public Button GetCloseButton()
    {
        return closeButton;
    }

    /// <summary>
    /// Get all equipment slots.
    /// </summary>
    /// <returns>A list of all equipment slots</returns>
    public List<EquipmentSlot> GetEquipmentSlots()
    {
        if (PlayerCharacter == null || PlayerCharacter.Equipment == null || PlayerCharacter.Equipment.Slots == null)
        {
            return new List<EquipmentSlot>();
        }

        return new List<EquipmentSlot>(PlayerCharacter.Equipment.Slots);
    }

    /// <summary>
    /// Get all equipped items.
    /// </summary>
    /// <returns>A list of all equipped items</returns>
    public List<Item> GetEquippedItems()
    {
        if (PlayerCharacter == null || PlayerCharacter.Equipment == null || PlayerCharacter.Equipment.EquippedItems == null)
        {
            return new List<Item>();
        }

        return new List<Item>(PlayerCharacter.Equipment.EquippedItems);
    }

    /// <summary>
    /// Check if an item is equipped.
    /// </summary>
    /// <param name="item">The item to check</param>
    /// <returns>True if the item is equipped, false otherwise</returns>
    public bool IsItemEquipped(Item item)
    {
        if (item == null || PlayerCharacter == null || PlayerCharacter.Equipment == null)
        {
            return false;
        }

        return PlayerCharacter.Equipment.EquippedItems.Contains(item);
    }

    /// <summary>
    /// Check if an equipment slot has an item equipped.
    /// </summary>
    /// <param name="slot">The equipment slot to check</param>
    /// <returns>True if the slot has an item equipped, false otherwise</returns>
    public bool IsSlotEquipped(EquipmentSlot slot)
    {
        if (slot == null || PlayerCharacter == null || PlayerCharacter.Equipment == null)
        {
            return false;
        }

        return slot.Item != null;
    }

    /// <summary>
    /// Get the equipment slot for an equipped item.
    /// </summary>
    /// <param name="item">The equipped item to get the slot for</param>
    /// <returns>The equipment slot for the item, or null if not found</returns>
    public EquipmentSlot GetSlotForItem(Item item)
    {
        if (item == null || PlayerCharacter == null || PlayerCharacter.Equipment == null || PlayerCharacter.Equipment.Slots == null)
        {
            return null;
        }

        return PlayerCharacter.Equipment.Slots.FirstOrDefault(slot => slot.Item == item);
    }

    /// <summary>
    /// Get the equipped item for an equipment slot.
    /// </summary>
    /// <param name="slot">The equipment slot to get the item for</param>
    /// <returns>The equipped item for the slot, or null if not found</returns>
    public Item GetItemForSlot(EquipmentSlot slot)
    {
        if (slot == null)
        {
            return null;
        }

        return slot.Item;
    }

    /// <summary>
    /// Get the equipment stats for the player character.
    /// </summary>
    /// <returns>The equipment stats for the player character</returns>
    public EquipmentStats GetEquipmentStats()
    {
        if (PlayerCharacter == null || PlayerCharacter.Equipment == null)
        {
            return null;
        }

        return PlayerCharacter.Equipment.Stats;
    }

    /// <summary>
    /// Update the equipment stats display.
    /// </summary>
    /// <param name="stats">The equipment stats to display</param>
    public void UpdateEquipmentStatsDisplay(EquipmentStats stats)
    {
        if (stats == null || statsContainer == null)
        {
            return;
        }

        // Update the equipment stats display with the current values
        // This would typically involve updating labels, progress bars, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var attackLabel = statsContainer.GetNode<Label>("Attack");
        // attackLabel.Text = $"Attack: {stats.Attack}";

        // var defenseLabel = statsContainer.GetNode<Label>("Defense");
        // defenseLabel.Text = $"Defense: {stats.Defense}";

        // var speedLabel = statsContainer.GetNode<Label>("Speed");
        // speedLabel.Text = $"Speed: {stats.Speed}";

        // var healthLabel = statsContainer.GetNode<Label>("Health");
        // healthLabel.Text = $"Health: {stats.Health}";

        // var manaLabel = statsContainer.GetNode<Label>("Mana");
        // manaLabel.Text = $"Mana: {stats.Mana}";
    }

    /// <summary>
    /// Update the character portrait display.
    /// </summary>
    /// <param name="portrait">The character portrait to display</param>
    public void UpdatePortraitDisplay(Texture2D portrait)
    {
        if (portrait == null || portraitContainer == null)
        {
            return;
        }

        // Update the character portrait display with the current portrait
        // This would typically involve updating a TextureRect or similar control
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var portraitRect = portraitContainer.GetNode<TextureRect>("Portrait");
        // portraitRect.Texture = portrait;
    }

    /// <summary>
    /// Get the character portrait.
    /// </summary>
    /// <returns>The character portrait, or null if not found</returns>
    public Texture2D GetPortrait()
    {
        if (PlayerCharacter == null)
        {
            return null;
        }

        return PlayerCharacter.Portrait;
    }

    /// <summary>
    /// Set the character portrait.
    /// </summary>
    /// <param name="portrait">The character portrait to set</param>
    public void SetPortrait(Texture2D portrait)
    {
        if (portrait == null || PlayerCharacter == null)
        {
            return;
        }

        PlayerCharacter.Portrait = portrait;
        UpdatePortraitDisplay(portrait);
    }

    /// <summary>
    /// Get the character name.
    /// </summary>
    /// <returns>The character name</returns>
    public string GetCharacterName()
    {
        if (PlayerCharacter == null)
        {
            return "";
        }

        return PlayerCharacter.Name;
    }

    /// <summary>
    /// Set the character name.
    /// </summary>
    /// <param name="name">The character name to set</param>
    public void SetCharacterName(string name)
    {
        if (string.IsNullOrEmpty(name) || PlayerCharacter == null)
        {
            return;
        }

        PlayerCharacter.Name = name;

        // Update any displays that show the character name
        // This would typically involve updating labels, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var nameLabel = GetNode<Label>("CharacterName");
        // nameLabel.Text = name;
    }

    /// <summary>
    /// Get the character level.
    /// </summary>
    /// <returns>The character level</returns>
    public int GetCharacterLevel()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return 0;
        }

        return PlayerCharacter.Stats.Level;
    }

    /// <summary>
    /// Set the character level.
    /// </summary>
    /// <param name="level">The character level to set</param>
    public void SetCharacterLevel(int level)
    {
        if (level < 0 || PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return;
        }

        PlayerCharacter.Stats.Level = level;

        // Update any displays that show the character level
        // This would typically involve updating labels, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var levelLabel = GetNode<Label>("CharacterLevel");
        // levelLabel.Text = $"Level: {level}";
    }

    /// <summary>
    /// Get the character experience.
    /// </summary>
    /// <returns>The character experience</returns>
    public int GetCharacterExperience()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return 0;
        }

        return PlayerCharacter.Stats.Experience;
    }

    /// <summary>
    /// Set the character experience.
    /// </summary>
    /// <param name="experience">The character experience to set</param>
    public void SetCharacterExperience(int experience)
    {
        if (experience < 0 || PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return;
        }

        PlayerCharacter.Stats.Experience = experience;

        // Update any displays that show the character experience
        // This would typically involve updating labels, progress bars, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var experienceLabel = GetNode<Label>("CharacterExperience");
        // experienceLabel.Text = $"Experience: {experience}";

        // var experienceBar = GetNode<TextureProgressBar>("ExperienceBar");
        // experienceBar.Value = experience;
        // experienceBar.MaxValue = PlayerCharacter.Stats.NextLevelExperience;
    }

    /// <summary>
    /// Get the character health.
    /// </summary>
    /// <returns>The character health</returns>
    public int GetCharacterHealth()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return 0;
        }

        return PlayerCharacter.Stats.Health;
    }

    /// <summary>
    /// Set the character health.
    /// </summary>
    /// <param name="health">The character health to set</param>
    public void SetCharacterHealth(int health)
    {
        if (health < 0 || PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return;
        }

        PlayerCharacter.Stats.Health = health;

        // Update any displays that show the character health
        // This would typically involve updating labels, progress bars, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var healthLabel = GetNode<Label>("CharacterHealth");
        // healthLabel.Text = $"Health: {health}/{PlayerCharacter.Stats.MaxHealth}";

        // var healthBar = GetNode<TextureProgressBar>("HealthBar");
        // healthBar.Value = health;
        // healthBar.MaxValue = PlayerCharacter.Stats.MaxHealth;
    }

    /// <summary>
    /// Get the character mana.
    /// </summary>
    /// <returns>The character mana</returns>
    public int GetCharacterMana()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return 0;
        }

        return PlayerCharacter.Stats.Mana;
    }

    /// <summary>
    /// Set the character mana.
    /// </summary>
    /// <param name="mana">The character mana to set</param>
    public void SetCharacterMana(int mana)
    {
        if (mana < 0 || PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return;
        }

        PlayerCharacter.Stats.Mana = mana;

        // Update any displays that show the character mana
        // This would typically involve updating labels, progress bars, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var manaLabel = GetNode<Label>("CharacterMana");
        // manaLabel.Text = $"Mana: {mana}/{PlayerCharacter.Stats.MaxMana}";

        // var manaBar = GetNode<TextureProgressBar>("ManaBar");
        // manaBar.Value = mana;
        // manaBar.MaxValue = PlayerCharacter.Stats.MaxMana;
    }

    /// <summary>
    /// Get the character attack stat.
    /// </summary>
    /// <returns>The character attack stat</returns>
    public int GetCharacterAttack()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return 0;
        }

        return PlayerCharacter.Stats.Attack;
    }

    /// <summary>
    /// Set the character attack stat.
    /// </summary>
    /// <param name="attack">The character attack stat to set</param>
    public void SetCharacterAttack(int attack)
    {
        if (attack < 0 || PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return;
        }

        PlayerCharacter.Stats.Attack = attack;

        // Update any displays that show the character attack stat
        // This would typically involve updating labels, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var attackLabel = GetNode<Label>("CharacterAttack");
        // attackLabel.Text = $"Attack: {attack}";
    }

    /// <summary>
    /// Get the character defense stat.
    /// </summary>
    /// <returns>The character defense stat</returns>
    public int GetCharacterDefense()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return 0;
        }

        return PlayerCharacter.Stats.Defense;
    }

    /// <summary>
    /// Set the character defense stat.
    /// </summary>
    /// <param name="defense">The character defense stat to set</param>
    public void SetCharacterDefense(int defense)
    {
        if (defense < 0 || PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return;
        }

        PlayerCharacter.Stats.Defense = defense;

        // Update any displays that show the character defense stat
        // This would typically involve updating labels, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var defenseLabel = GetNode<Label>("CharacterDefense");
        // defenseLabel.Text = $"Defense: {defense}";
    }

    /// <summary>
    /// Get the character speed stat.
    /// </summary>
    /// <returns>The character speed stat</returns>
    public int GetCharacterSpeed()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return 0;
        }

        return PlayerCharacter.Stats.Speed;
    }

    /// <summary>
    /// Set the character speed stat.
    /// </summary>
    /// <param name="speed">The character speed stat to set</param>
    public void SetCharacterSpeed(int speed)
    {
        if (speed < 0 || PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return;
        }

        PlayerCharacter.Stats.Speed = speed;

        // Update any displays that show the character speed stat
        // This would typically involve updating labels, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var speedLabel = GetNode<Label>("CharacterSpeed");
        // speedLabel.Text = $"Speed: {speed}";
    }

    /// <summary>
    /// Get the character agility stat.
    /// </summary>
    /// <returns>The character agility stat</returns>
    public int GetCharacterAgility()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return 0;
        }

        return PlayerCharacter.Stats.Agility;
    }

    /// <summary>
    /// Set the character agility stat.
    /// </summary>
    /// <param name="agility">The character agility stat to set</param>
    public void SetCharacterAgility(int agility)
    {
        if (agility < 0 || PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return;
        }

        PlayerCharacter.Stats.Agility = agility;

        // Update any displays that show the character agility stat
        // This would typically involve updating labels, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var agilityLabel = GetNode<Label>("CharacterAgility");
        // agilityLabel.Text = $"Agility: {agility}";
    }

    /// <summary>
    /// Get the character intelligence stat.
    /// </summary>
    /// <returns>The character intelligence stat</returns>
    public int GetCharacterIntelligence()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return 0;
        }

        return PlayerCharacter.Stats.Intelligence;
    }

    /// <summary>
    /// Set the character intelligence stat.
    /// </summary>
    /// <param name="intelligence">The character intelligence stat to set</param>
    public void SetCharacterIntelligence(int intelligence)
    {
        if (intelligence < 0 || PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return;
        }

        PlayerCharacter.Stats.Intelligence = intelligence;

        // Update any displays that show the character intelligence stat
        // This would typically involve updating labels, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var intelligenceLabel = GetNode<Label>("CharacterIntelligence");
        // intelligenceLabel.Text = $"Intelligence: {intelligence}";
    }

    /// <summary>
    /// Get the character vitality stat.
    /// </summary>
    /// <returns>The character vitality stat</returns>
    public int GetCharacterVitality()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return 0;
        }

        return PlayerCharacter.Stats.Vitality;
    }

    /// <summary>
    /// Set the character vitality stat.
    /// </summary>
    /// <param name="vitality">The character vitality stat to set</param>
    public void SetCharacterVitality(int vitality)
    {
        if (vitality < 0 || PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return;
        }

        PlayerCharacter.Stats.Vitality = vitality;

        // Update any displays that show the character vitality stat
        // This would typically involve updating labels, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var vitalityLabel = GetNode<Label>("CharacterVitality");
        // vitalityLabel.Text = $"Vitality: {vitality}";
    }

    /// <summary>
    /// Get the character luck stat.
    /// </summary>
    /// <returns>The character luck stat</returns>
    public int GetCharacterLuck()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return 0;
        }

        return PlayerCharacter.Stats.Luck;
    }

    /// <summary>
    /// Set the character luck stat.
    /// </summary>
    /// <param name="luck">The character luck stat to set</param>
    public void SetCharacterLuck(int luck)
    {
        if (luck < 0 || PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return;
        }

        PlayerCharacter.Stats.Luck = luck;

        // Update any displays that show the character luck stat
        // This would typically involve updating labels, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var luckLabel = GetNode<Label>("CharacterLuck");
        // luckLabel.Text = $"Luck: {luck}";
    }

    /// <summary>
    /// Get all character stats.
    /// </summary>
    /// <returns>The character stats</returns>
    public CharacterStats GetCharacterStats()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return null;
        }

        return PlayerCharacter.Stats;
    }

    /// <summary>
    /// Set all character stats.
    /// </summary>
    /// <param name="stats">The character stats to set</param>
    public void SetCharacterStats(CharacterStats stats)
    {
        if (stats == null || PlayerCharacter == null)
        {
            return;
        }

        PlayerCharacter.Stats = stats;

        // Update all stat displays
        UpdateAllStatDisplays();
    }

    /// <summary>
    /// Update all character stat displays.
    /// </summary>
    public void UpdateAllStatDisplays()
    {
        if (PlayerCharacter == null || PlayerCharacter.Stats == null)
        {
            return;
        }

        SetCharacterLevel(PlayerCharacter.Stats.Level);
        SetCharacterExperience(PlayerCharacter.Stats.Experience);
        SetCharacterHealth(PlayerCharacter.Stats.Health);
        SetCharacterMana(PlayerCharacter.Stats.Mana);
        SetCharacterAttack(PlayerCharacter.Stats.Attack);
        SetCharacterDefense(PlayerCharacter.Stats.Defense);
        SetCharacterSpeed(PlayerCharacter.Stats.Speed);
        SetCharacterAgility(PlayerCharacter.Stats.Agility);
        SetCharacterIntelligence(PlayerCharacter.Stats.Intelligence);
        SetCharacterVitality(PlayerCharacter.Stats.Vitality);
        SetCharacterLuck(PlayerCharacter.Stats.Luck);
    }

    /// <summary>
    /// Get all character equipment.
    /// </summary>
    /// <returns>The character equipment</returns>
    public Equipment GetCharacterEquipment()
    {
        if (PlayerCharacter == null || PlayerCharacter.Equipment == null)
        {
            return null;
        }

        return PlayerCharacter.Equipment;
    }

    /// <summary>
    /// Set all character equipment.
    /// </summary>
    /// <param name="equipment">The character equipment to set</param>
    public void SetCharacterEquipment(Equipment equipment)
    {
        if (equipment == null || PlayerCharacter == null)
        {
            return;
        }

        PlayerCharacter.Equipment = equipment;

        // Update all equipment displays
        UpdateAllEquipmentDisplays();
    }

    /// <summary>
    /// Update all character equipment displays.
    /// </summary>
    public void UpdateAllEquipmentDisplays()
    {
        if (PlayerCharacter == null || PlayerCharacter.Equipment == null)
        {
            return;
        }

        // Clear and recreate all equipment displays
        foreach (var display in equipmentSlotDisplays.Values)
        {
            display.QueueFree();
        }

        equipmentSlotDisplays.Clear();

        if (PlayerCharacter.Equipment.Slots != null)
        {
            foreach (var slot in PlayerCharacter.Equipment.Slots)
            {
                CreateEquipmentSlotDisplay(slot, equipmentContainer);
            }
        }

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Get all character skills.
    /// </summary>
    /// <returns>The character skills</returns>
    public Skills GetCharacterSkills()
    {
        if (PlayerCharacter == null || PlayerCharacter.Skills == null)
        {
            return null;
        }

        return PlayerCharacter.Skills;
    }

    /// <summary>
    /// Set all character skills.
    /// </summary>
    /// <param name="skills">The character skills to set</param>
    public void SetCharacterSkills(Skills skills)
    {
        if (skills == null || PlayerCharacter == null)
        {
            return;
        }

        PlayerCharacter.Skills = skills;

        // Update all skill displays
        UpdateAllSkillDisplays();
    }

    /// <summary>
    /// Update all character skill displays.
    /// </summary>
    public void UpdateAllSkillDisplays()
    {
        if (PlayerCharacter == null || PlayerCharacter.Skills == null)
        {
            return;
        }

        // Clear and recreate all skill displays
        foreach (var display in skillDisplays.Values)
        {
            display.QueueFree();
        }

        skillDisplays.Clear();

        if (PlayerCharacter.Skills.SkillList != null)
        {
            foreach (var skill in PlayerCharacter.Skills.SkillList)
            {
                CreateSkillDisplay(skill, skillsContainer);
            }
        }

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Get the character portrait.
    /// </summary>
    /// <returns>The character portrait</returns>
    public Texture2D GetCharacterPortrait()
    {
        if (PlayerCharacter == null)
        {
            return null;
        }

        return PlayerCharacter.Portrait;
    }

    /// <summary>
    /// Set the character portrait.
    /// </summary>
    /// <param name="portrait">The character portrait to set</param>
    public void SetCharacterPortrait(Texture2D portrait)
    {
        if (portrait == null || PlayerCharacter == null)
        {
            return;
        }

        PlayerCharacter.Portrait = portrait;

        // Update the portrait display
        UpdatePortraitDisplay(portrait);
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
    /// Refresh the character sheet with current character data.
    /// </summary>
    public void Refresh()
    {
        // Clear and recreate all displays
        ClearDisplays();

        if (PlayerCharacter != null)
        {
            CreateStatsDisplay(PlayerCharacter.Stats, statsContainer);
            CreateEquipmentDisplay(PlayerCharacter.Equipment, equipmentContainer);
            CreateSkillsDisplay(PlayerCharacter.Skills, skillsContainer);
            CreatePortraitDisplay(PlayerCharacter.Portrait, portraitContainer);
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

        // Update all stat displays
        UpdateAllStatDisplays();

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

        // Update all equipment displays
        UpdateAllEquipmentDisplays();

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

        // Update all skill displays
        UpdateAllSkillDisplays();

        // Show a notification
        ShowMessage("Skills updated.");
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

    }
