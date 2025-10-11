using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Models;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Interfaces;

/// <summary>
/// Container for the player inventory display.
/// The UIInventory manages the display and interaction of player inventory items,
/// allowing players to view, organize, and use their collected items during gameplay.
/// It handles the presentation of items, equipment management, and item usage interfaces.
/// </summary>
public partial class UIInventory : Control
{
    /// <summary>
    /// Emitted when the player selects an item.
    /// </summary>
    [Signal]
    public delegate void ItemSelectedEventHandler(Item item);

    /// <summary>
    /// Emitted when the player uses an item.
    /// </summary>
    [Signal]
    public delegate void ItemUsedEventHandler(Item item, Battler target);

    /// <summary>
    /// Emitted when the player equips an item.
    /// </summary>
    [Signal]
    public delegate void ItemEquippedEventHandler(Item item, Battler target);

    /// <summary>
    /// Emitted when the player unequips an item.
    /// </summary>
    [Signal]
    public delegate void ItemUnequippedEventHandler(Item item, Battler target);

    /// <summary>
    /// The player's inventory.
    /// </summary>
    public Inventory PlayerInventory { get; private set; }

    /// <summary>
    /// The currently selected item.
    /// </summary>
    public Item SelectedItem { get; private set; }

    /// <summary>
    /// Whether the inventory is currently visible.
    /// </summary>
    public bool InventoryVisible
    {
        get => Visible;
        set => Visible = value;
    }

    /// <summary>
    /// The container for inventory items.
    /// </summary>
    private Control itemContainer;

    /// <summary>
    /// The container for equipped items.
    /// </summary>
    private Control equippedContainer;

    /// <summary>
    /// The item details panel.
    /// </summary>
    private Control itemDetailsPanel;

    /// <summary>
    /// The item action buttons.
    /// </summary>
    private Control itemActions;

    /// <summary>
    /// Dictionary mapping items to their UI displays.
    /// </summary>
    private Dictionary<Item, Control> itemDisplays = new Dictionary<Item, Control>();

    /// <summary>
    /// Dictionary mapping equipped items to their UI displays.
    /// </summary>
    private Dictionary<Item, Control> equippedDisplays = new Dictionary<Item, Control>();

    public override void _Ready()
    {
        // Get references to child UI elements
        itemContainer = GetNode<Control>("ItemsContainer");
        equippedContainer = GetNode<Control>("EquippedContainer");
        itemDetailsPanel = GetNode<Control>("ItemDetails");
        itemActions = GetNode<Control>("ItemActions");

        // Initially hide the inventory
        Visible = false;

        // Connect to any necessary signals
        ConnectSignals();
    }

    /// <summary>
    /// Connect to necessary signals.
    /// </summary>
    private void ConnectSignals()
    {
        // Connect to inventory events
        // InventoryEvents.ItemAdded += OnItemAdded;
        // InventoryEvents.ItemRemoved += OnItemRemoved;
        // InventoryEvents.ItemEquipped += OnItemEquipped;
        // InventoryEvents.ItemUnequipped += OnItemUnequipped;
    }

    /// <summary>
    /// Setup the UI inventory with the given player inventory.
    /// </summary>
    /// <param name="inventory">The player's inventory</param>
    public void Setup(Inventory inventory)
    {
        PlayerInventory = inventory;

        // Clear any existing item displays
        ClearItemDisplays();

        // Create displays for all items in the inventory
        if (PlayerInventory.Items != null)
        {
            foreach (var item in PlayerInventory.Items)
            {
                CreateItemDisplay(item, itemContainer);
            }
        }

        // Create displays for all equipped items
        if (PlayerInventory.EquippedItems != null)
        {
            foreach (var item in PlayerInventory.EquippedItems)
            {
                CreateEquippedDisplay(item, equippedContainer);
            }
        }

        // Show the inventory
        Visible = true;

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Clear all item displays.
    /// </summary>
    private void ClearItemDisplays()
    {
        // Remove all existing item displays
        foreach (var display in itemDisplays.Values)
        {
            display.QueueFree();
        }

        itemDisplays.Clear();

        // Remove all existing equipped displays
        foreach (var display in equippedDisplays.Values)
        {
            display.QueueFree();
        }

        equippedDisplays.Clear();

        // Clear containers
        if (itemContainer != null)
        {
            foreach (var child in itemContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (equippedContainer != null)
        {
            foreach (var child in equippedContainer.GetChildren())
            {
                child.QueueFree();
            }
        }
    }

    /// <summary>
    /// Create a display for an item.
    /// </summary>
    /// <param name="item">The item to create a display for</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateItemDisplay(Item item, Control container)
    {
        if (item == null || container == null)
        {
            return;
        }

        // Create a new item display control
        var display = new Control();
        display.Name = item.Name;

        // Add the display to the container
        container.AddChild(display);

        // Store the display in the dictionary
        itemDisplays[item] = display;

        // Set up the display with initial values
        UpdateItemDisplay(item, display);

        // Connect input events to allow selecting the item
        display.GuiInput += (inputEvent) => OnItemDisplayInput(item, inputEvent);
    }

    /// <summary>
    /// Create a display for an equipped item.
    /// </summary>
    /// <param name="item">The equipped item to create a display for</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateEquippedDisplay(Item item, Control container)
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
        equippedDisplays[item] = display;

        // Set up the display with initial values
        UpdateEquippedDisplay(item, display);

        // Connect input events to allow selecting the item
        display.GuiInput += (inputEvent) => OnEquippedDisplayInput(item, inputEvent);
    }

    /// <summary>
    /// Update an item display with current values.
    /// </summary>
    /// <param name="item">The item to update the display for</param>
    /// <param name="display">The display to update</param>
    private void UpdateItemDisplay(Item item, Control display)
    {
        if (item == null || display == null)
        {
            return;
        }

        // Update the display with the item's current properties
        // This would typically involve updating labels, icons, quantities, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var nameLabel = display.GetNode<Label>("Name");
        // nameLabel.Text = item.Name;

        // var quantityLabel = display.GetNode<Label>("Quantity");
        // quantityLabel.Text = $"x{item.Quantity}";

        // var icon = display.GetNode<TextureRect>("Icon");
        // icon.Texture = item.Icon;
    }

    /// <summary>
    /// Update an equipped item display with current values.
    /// </summary>
    /// <param name="item">The equipped item to update the display for</param>
    /// <param name="display">The display to update</param>
    private void UpdateEquippedDisplay(Item item, Control display)
    {
        if (item == null || display == null)
        {
            return;
        }

        // Update the display with the equipped item's current properties
        // This would typically involve updating labels, icons, equipment slots, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var nameLabel = display.GetNode<Label>("Name");
        // nameLabel.Text = item.Name;

        // var slotLabel = display.GetNode<Label>("Slot");
        // slotLabel.Text = item.EquipmentSlot.ToString();

        // var icon = display.GetNode<TextureRect>("Icon");
        // icon.Texture = item.Icon;
    }

    /// <summary>
    /// Update the item details panel.
    /// </summary>
    /// <param name="item">The item to display details for</param>
    private void UpdateItemDetailsPanel(Item item)
    {
        if (item == null || itemDetailsPanel == null)
        {
            return;
        }

        // Update the details panel with the item's properties
        // This would typically involve updating labels, descriptions, stats, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var nameLabel = itemDetailsPanel.GetNode<Label>("Name");
        // nameLabel.Text = item.Name;

        // var descriptionLabel = itemDetailsPanel.GetNode<Label>("Description");
        // descriptionLabel.Text = item.Description;

        // var typeLabel = itemDetailsPanel.GetNode<Label>("Type");
        // typeLabel.Text = item.Type.ToString();

        // var valueLabel = itemDetailsPanel.GetNode<Label>("Value");
        // valueLabel.Text = $"Value: {item.Value}";
    }

    /// <summary>
    /// Show the available actions for an item.
    /// </summary>
    /// <param name="item">The item to show actions for</param>
    private void ShowItemActions(Item item)
    {
        if (item == null || itemActions == null)
        {
            return;
        }

        // Show the appropriate actions for the item
        // This would typically involve showing/hiding buttons based on the item type
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var useButton = itemActions.GetNode<Button>("Use");
        // var equipButton = itemActions.GetNode<Button>("Equip");
        // var dropButton = itemActions.GetNode<Button>("Drop");

        // useButton.Visible = item.IsUsable;
        // equipButton.Visible = item.IsEquipment;
        // dropButton.Visible = true;

        // Connect button signals
        // useButton.Pressed += () => OnUseItem(item);
        // equipButton.Pressed += () => OnEquipItem(item);
        // dropButton.Pressed += () => OnDropItem(item);
    }

    /// <summary>
    /// Hide the item actions.
    /// </summary>
    private void HideItemActions()
    {
        if (itemActions == null)
        {
            return;
        }

        // Hide all action buttons
        foreach (var child in itemActions.GetChildren())
        {
            if (child is Button button)
            {
                button.Visible = false;
            }
        }
    }

    /// <summary>
    /// Callback when an item is selected.
    /// </summary>
    /// <param name="item">The selected item</param>
    private void OnItemSelected(Item item)
    {
        if (item == null)
        {
            return;
        }

        SelectedItem = item;
        UpdateItemDetailsPanel(item);
        ShowItemActions(item);
        EmitSignal(SignalName.ItemSelected, item);
    }

    /// <summary>
    /// Callback when input is received on an item display.
    /// </summary>
    /// <param name="item">The item associated with the display</param>
    /// <param name="inputEvent">The input event</param>
    private void OnItemDisplayInput(Item item, InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            // Select the item when clicked
            OnItemSelected(item);
        }
    }

    /// <summary>
    /// Callback when input is received on an equipped display.
    /// </summary>
    /// <param name="item">The equipped item associated with the display</param>
    /// <param name="inputEvent">The input event</param>
    private void OnEquippedDisplayInput(Item item, InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            // Select the equipped item when clicked
            OnItemSelected(item);
        }
    }

    /// <summary>
    /// Callback when the use item button is pressed.
    /// </summary>
    /// <param name="item">The item to use</param>
    private void OnUseItem(Item item)
    {
        if (item == null || !item.IsUsable)
        {
            return;
        }

        // Determine the target for the item use
        // This might be the player, a party member, or all party members
        Battler target = null;
        if (item.TargetScope == Item.TargetScope.Single)
        {
            // For single target items, use on the player
            target = GetPlayerBattler();
        }
        else if (item.TargetScope == Item.TargetScope.All)
        {
            // For all target items, use on all party members
            // This would typically involve selecting all party members as targets
        }
        else if (item.TargetScope == Item.TargetScope.Self)
        {
            // For self target items, use on the player
            target = GetPlayerBattler();
        }

        // Emit the item used signal
        if (target != null)
        {
            EmitSignal(SignalName.ItemUsed, item, target);
        }

        // Hide the inventory after using an item
        HideInventory();
    }

    /// <summary>
    /// Callback when the equip item button is pressed.
    /// </summary>
    /// <param name="item">The item to equip</param>
    private void OnEquipItem(Item item)
    {
        if (item == null || !item.IsEquipment)
        {
            return;
        }

        // Determine the target for the item equip
        // This might be the player or a party member
        Battler target = GetPlayerBattler();

        // Emit the item equipped signal
        if (target != null)
        {
            EmitSignal(SignalName.ItemEquipped, item, target);
        }

        // Hide the inventory after equipping an item
        HideInventory();
    }

    /// <summary>
    /// Callback when the unequip item button is pressed.
    /// </summary>
    /// <param name="item">The item to unequip</param>
    private void OnUnequipItem(Item item)
    {
        if (item == null)
        {
            return;
        }

        // Determine the target for the item unequip
        // This might be the player or a party member
        Battler target = GetPlayerBattler();

        // Emit the item unequipped signal
        if (target != null)
        {
            EmitSignal(SignalName.ItemUnequipped, item, target);
        }

        // Hide the inventory after unequipping an item
        HideInventory();
    }

    /// <summary>
    /// Callback when the drop item button is pressed.
    /// </summary>
    /// <param name="item">The item to drop</param>
    private void OnDropItem(Item item)
    {
        if (item == null)
        {
            return;
        }

        // Emit the item dropped signal
        // EmitSignal(SignalName.ItemDropped, item);

        // Remove the item from the inventory
        if (PlayerInventory != null)
        {
            PlayerInventory.RemoveItem(item);
        }

        // Hide the inventory after dropping an item
        HideInventory();
    }

    /// <summary>
    /// Get the player battler.
    /// </summary>
    /// <returns>The player battler, or null if not found</returns>
    private Battler GetPlayerBattler()
    {
        // This would typically return the player's battler object
        // The exact implementation depends on how the player battler is stored/retrieved

        // For example:
        // return GameState.Instance?.PlayerBattler;

        return null;
    }

    /// <summary>
    /// Hide the inventory.
    /// </summary>
    public void HideInventory()
    {
        Visible = false;
        SelectedItem = null;
        HideItemActions();
    }

    /// <summary>
    /// Show the inventory.
    /// </summary>
    public void ShowInventory()
    {
        Visible = true;
        SelectedItem = null;
        HideItemActions();
    }

    /// <summary>
    /// Toggle the inventory visibility.
    /// </summary>
    public void ToggleInventory()
    {
        if (Visible)
        {
            HideInventory();
        }
        else
        {
            ShowInventory();
        }
    }

    /// <summary>
    /// Update all item displays.
    /// </summary>
    public void UpdateAllDisplays()
    {
        foreach (var kvp in itemDisplays)
        {
            UpdateItemDisplay(kvp.Key, kvp.Value);
        }

        foreach (var kvp in equippedDisplays)
        {
            UpdateEquippedDisplay(kvp.Key, kvp.Value);
        }

        // If there's a selected item, update the details panel
        if (SelectedItem != null)
        {
            UpdateItemDetailsPanel(SelectedItem);
        }
    }

    /// <summary>
    /// Get the display for a specific item.
    /// </summary>
    /// <param name="item">The item to get the display for</param>
    /// <returns>The display control for the item, or null if not found</returns>
    public Control GetItemDisplay(Item item)
    {
        if (item == null)
        {
            return null;
        }

        return itemDisplays.GetValueOrDefault(item, null);
    }

    /// <summary>
    /// Get the display for a specific equipped item.
    /// </summary>
    /// <param name="item">The equipped item to get the display for</param>
    /// <returns>The display control for the equipped item, or null if not found</returns>
    public Control GetEquippedDisplay(Item item)
    {
        if (item == null)
        {
            return null;
        }

        return equippedDisplays.GetValueOrDefault(item, null);
    }

    /// <summary>
    /// Refresh the inventory with current item data.
    /// </summary>
    public void Refresh()
    {
        // Clear and recreate all item displays
        ClearItemDisplays();

        if (PlayerInventory != null)
        {
            if (PlayerInventory.Items != null)
            {
                foreach (var item in PlayerInventory.Items)
                {
                    CreateItemDisplay(item, itemContainer);
                }
            }

            if (PlayerInventory.EquippedItems != null)
            {
                foreach (var item in PlayerInventory.EquippedItems)
                {
                    CreateEquippedDisplay(item, equippedContainer);
                }
            }
        }

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Show a message in the inventory.
    /// </summary>
    /// <param name="message">The message to show</param>
    /// <param name="duration">The duration to show the message for</param>
    public async void ShowMessage(string message, float duration = 2.0f)
    {
        // Show a temporary message in the inventory
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
    /// Show an effect label (like item usage feedback).
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
    /// Apply a preset to the inventory.
    /// </summary>
    /// <param name="preset">The preset to apply</param>
    public void ApplyPreset(InventoryPreset preset)
    {
        if (preset == null)
        {
            return;
        }

        // Apply the preset to the inventory
        if (PlayerInventory != null)
        {
            PlayerInventory.ApplyPreset(preset);
        }

        // Update all displays
        UpdateAllDisplays();

        // Show a success message
        ShowMessage($"Preset '{preset.Name}' applied successfully.");
    }

    /// <summary>
    /// Save the current inventory as a preset.
    /// </summary>
    /// <param name="presetName">The name of the preset to save</param>
    /// <returns>The saved preset</returns>
    public InventoryPreset SavePreset(string presetName)
    {
        if (string.IsNullOrEmpty(presetName) || PlayerInventory == null)
        {
            return null;
        }

        // Create a new preset with the current inventory's properties
        var preset = new InventoryPreset();
        preset.Name = presetName;

        // Save the preset to a file or database
        // This would typically involve serializing the preset and saving it

        // For example:
        // var presetPath = $"user://presets/{presetName}.json";
        // var file = FileAccess.Open(presetPath, FileAccess.ModeFlags.Write);
        // file.StoreString(JsonSerializer.Serialize(preset));
        // file.Close();

        // Show a success message
        ShowMessage($"Preset '{presetName}' saved successfully.");

        return preset;
    }

    /// <summary>
    /// Load a preset and apply it to the inventory.
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
        //     var preset = JsonSerializer.Deserialize<InventoryPreset>(presetJson);
        //     ApplyPreset(preset);
        // }
        // else
        // {
        //     ShowMessage($"Preset '{presetName}' not found.");
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
        //     ShowMessage($"Preset '{presetName}' deleted successfully.");
        // }
        // else
        // {
        //     ShowMessage($"Preset '{presetName}' not found.");
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
    /// Import an inventory from another source.
    /// </summary>
    /// <param name="importData">The inventory data to import</param>
    public void ImportInventory(InventoryImportData importData)
    {
        if (importData == null)
        {
            return;
        }

        // Import the inventory data
        if (PlayerInventory != null)
        {
            PlayerInventory.Import(importData);
        }

        // Update all displays
        UpdateAllDisplays();

        // Show a success message
        ShowMessage("Inventory imported successfully.");
    }

    /// <summary>
    /// Export the current inventory to another format.
    /// </summary>
    /// <returns>The exported inventory data</returns>
    public InventoryExportData ExportInventory()
    {
        if (PlayerInventory == null)
        {
            return null;
        }

        // Create export data with the current inventory's properties
        var exportData = new InventoryExportData();
        exportData.Items = new List<Item>(PlayerInventory.Items);
        exportData.EquippedItems = new List<Item>(PlayerInventory.EquippedItems);

        // Export the inventory data to a file or other format
        // This would typically involve serializing the data

        // For example:
        // var exportPath = $"user://exports/inventory.json";
        // var file = FileAccess.Open(exportPath, FileAccess.ModeFlags.Write);
        // file.StoreString(JsonSerializer.Serialize(exportData));
        // file.Close();

        // Show a success message
        ShowMessage("Inventory exported successfully.");

        return exportData;
    }

    /// <summary>
    /// Sort the inventory items.
    /// </summary>
    /// <param name="sortType">The type of sorting to apply</param>
    public void SortItems(Inventory.SortType sortType)
    {
        if (PlayerInventory == null)
        {
            return;
        }

        // Sort the inventory items
        PlayerInventory.SortItems(sortType);

        // Refresh the inventory display
        Refresh();
    }

    /// <summary>
    /// Filter the inventory items.
    /// </summary>
    /// <param name="filterType">The type of filtering to apply</param>
    public void FilterItems(Inventory.FilterType filterType)
    {
        if (PlayerInventory == null)
        {
            return;
        }

        // Filter the inventory items
        PlayerInventory.FilterItems(filterType);

        // Refresh the inventory display
        Refresh();
    }

    /// <summary>
    /// Search for items in the inventory.
    /// </summary>
    /// <param name="searchTerm">The term to search for</param>
    public void SearchItems(string searchTerm)
    {
        if (PlayerInventory == null || string.IsNullOrEmpty(searchTerm))
        {
            return;
        }

        // Search for items in the inventory
        PlayerInventory.SearchItems(searchTerm);

        // Refresh the inventory display
        Refresh();
    }

    /// <summary>
    /// Get the total value of all items in the inventory.
    /// </summary>
    /// <returns>The total value of all items</returns>
    public int GetTotalValue()
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return 0;
        }

        return PlayerInventory.Items.Sum(item => item.Value);
    }

    /// <summary>
    /// Get the total weight of all items in the inventory.
    /// </summary>
    /// <returns>The total weight of all items</returns>
    public float GetTotalWeight()
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return 0.0f;
        }

        return PlayerInventory.Items.Sum(item => item.Weight);
    }

    /// <summary>
    /// Get the number of items in the inventory.
    /// </summary>
    /// <returns>The number of items in the inventory</returns>
    public int GetItemCount()
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return 0;
        }

        return PlayerInventory.Items.Count;
    }

    /// <summary>
    /// Get the number of equipped items.
    /// </summary>
    /// <returns>The number of equipped items</returns>
    public int GetEquippedCount()
    {
        if (PlayerInventory == null || PlayerInventory.EquippedItems == null)
        {
            return 0;
        }

        return PlayerInventory.EquippedItems.Count;
    }

    /// <summary>
    /// Check if the inventory is full.
    /// </summary>
    /// <returns>True if the inventory is full, false otherwise</returns>
    public bool IsInventoryFull()
    {
        if (PlayerInventory == null)
        {
            return false;
        }

        return PlayerInventory.IsFull();
    }

    /// <summary>
    /// Check if an item is in the inventory.
    /// </summary>
    /// <param name="item">The item to check for</param>
    /// <returns>True if the item is in the inventory, false otherwise</returns>
    public bool HasItem(Item item)
    {
        if (item == null || PlayerInventory == null || PlayerInventory.Items == null)
        {
            return false;
        }

        return PlayerInventory.Items.Contains(item);
    }

    /// <summary>
    /// Check if an item is equipped.
    /// </summary>
    /// <param name="item">The item to check for</param>
    /// <returns>True if the item is equipped, false otherwise</returns>
    public bool IsItemEquipped(Item item)
    {
        if (item == null || PlayerInventory == null || PlayerInventory.EquippedItems == null)
        {
            return false;
        }

        return PlayerInventory.EquippedItems.Contains(item);
    }

    /// <summary>
    /// Get all items of a specific type.
    /// </summary>
    /// <param name="itemType">The type of items to get</param>
    /// <returns>A list of items of the specified type</returns>
    public List<Item> GetItemsByType(Item.ItemType itemType)
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return new List<Item>();
        }

        return PlayerInventory.Items.Where(item => item.Type == itemType).ToList();
    }

    /// <summary>
    /// Get all usable items.
    /// </summary>
    /// <returns>A list of usable items</returns>
    public List<Item> GetUsableItems()
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return new List<Item>();
        }

        return PlayerInventory.Items.Where(item => item.IsUsable).ToList();
    }

    /// <summary>
    /// Get all equipment items.
    /// </summary>
    /// <returns>A list of equipment items</returns>
    public List<Item> GetEquipmentItems()
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return new List<Item>();
        }

        return PlayerInventory.Items.Where(item => item.IsEquipment).ToList();
    }

    /// <summary>
    /// Get all consumable items.
    /// </summary>
    /// <returns>A list of consumable items</returns>
    public List<Item> GetConsumableItems()
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return new List<Item>();
        }

        return PlayerInventory.Items.Where(item => item.IsConsumable).ToList();
    }

    /// <summary>
    /// Get all quest items.
    /// </summary>
    /// <returns>A list of quest items</returns>
    public List<Item> GetQuestItems()
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return new List<Item>();
        }

        return PlayerInventory.Items.Where(item => item.IsQuestItem).ToList();
    }

    /// <summary>
    /// Get all items sorted by value.
    /// </summary>
    /// <param name="ascending">Whether to sort in ascending order</param>
    /// <returns>A list of items sorted by value</returns>
    public List<Item> GetItemsSortedByValue(bool ascending = false)
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return new List<Item>();
        }

        if (ascending)
        {
            return PlayerInventory.Items.OrderBy(item => item.Value).ToList();
        }
        else
        {
            return PlayerInventory.Items.OrderByDescending(item => item.Value).ToList();
        }
    }

    /// <summary>
    /// Get all items sorted by weight.
    /// </summary>
    /// <param name="ascending">Whether to sort in ascending order</param>
    /// <returns>A list of items sorted by weight</returns>
    public List<Item> GetItemsSortedByWeight(bool ascending = false)
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return new List<Item>();
        }

        if (ascending)
        {
            return PlayerInventory.Items.OrderBy(item => item.Weight).ToList();
        }
        else
        {
            return PlayerInventory.Items.OrderByDescending(item => item.Weight).ToList();
        }
    }

    /// <summary>
    /// Get all items sorted by name.
    /// </summary>
    /// <param name="ascending">Whether to sort in ascending order</param>
    /// <returns>A list of items sorted by name</returns>
    public List<Item> GetItemsSortedByName(bool ascending = true)
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return new List<Item>();
        }

        if (ascending)
        {
            return PlayerInventory.Items.OrderBy(item => item.Name).ToList();
        }
        else
        {
            return PlayerInventory.Items.OrderByDescending(item => item.Name).ToList();
        }
    }

    /// <summary>
    /// Get all items sorted by type.
    /// </summary>
    /// <param name="ascending">Whether to sort in ascending order</param>
    /// <returns>A list of items sorted by type</returns>
    public List<Item> GetItemsSortedByType(bool ascending = true)
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return new List<Item>();
        }

        if (ascending)
        {
            return PlayerInventory.Items.OrderBy(item => item.Type).ToList();
        }
        else
        {
            return PlayerInventory.Items.OrderByDescending(item => item.Type).ToList();
        }
    }

    /// <summary>
    /// Get all items sorted by rarity.
    /// </summary>
    /// <param name="ascending">Whether to sort in ascending order</param>
    /// <returns>A list of items sorted by rarity</returns>
    public List<Item> GetItemsSortedByRarity(bool ascending = false)
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return new List<Item>();
        }

        if (ascending)
        {
            return PlayerInventory.Items.OrderBy(item => item.Rarity).ToList();
        }
        else
        {
            return PlayerInventory.Items.OrderByDescending(item => item.Rarity).ToList();
        }
    }

    /// <summary>
    /// Get all items sorted by quantity.
    /// </summary>
    /// <param name="ascending">Whether to sort in ascending order</param>
    /// <returns>A list of items sorted by quantity</returns>
    public List<Item> GetItemsSortedByQuantity(bool ascending = false)
    {
        if (PlayerInventory == null || PlayerInventory.Items == null)
        {
            return new List<Item>();
        }

        if (ascending)
        {
            return PlayerInventory.Items.OrderBy(item => item.Quantity).ToList();
        }
        else
        {
            return PlayerInventory.Items.OrderByDescending(item => item.Quantity).ToList();
        }
    }

    /// <summary>
    /// Callback when the close button is pressed.
    /// </summary>
    private void OnCloseButtonPressed()
    {
        HideInventory();
        EmitSignal(SignalName.SheetClosed);
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
        UpdateStatsDisplay(stats, statsContainer);

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
        UpdateEquipmentDisplay(equipment, equipmentContainer);

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
        UpdateSkillsDisplay(skills, skillsContainer);

        // Show a notification
        ShowMessage("Skills updated.");
    }

    /// <summary>
    /// Update the stats display with current values.
    /// </summary>
    /// <param name="stats">The character stats to update the display for</param>
    /// <param name="container">The container to update</param>
    private void UpdateStatsDisplay(CharacterStats stats, Control container)
    {
        if (stats == null || container == null)
        {
            return;
        }

        // Update the display with the character's current stats
        // This would typically involve updating labels, progress bars, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var strengthLabel = container.GetNode<Label>("Strength");
        // strengthLabel.Text = $"Strength: {stats.Strength}";

        // var agilityLabel = container.GetNode<Label>("Agility");
        // agilityLabel.Text = $"Agility: {stats.Agility}";

        // var intelligenceLabel = container.GetNode<Label>("Intelligence");
        // intelligenceLabel.Text = $"Intelligence: {stats.Intelligence}";

        // var vitalityLabel = container.GetNode<Label>("Vitality");
        // vitalityLabel.Text = $"Vitality: {stats.Vitality}";

        // var luckLabel = container.GetNode<Label>("Luck");
        // luckLabel.Text = $"Luck: {stats.Luck}";
    }

    /// <summary>
    /// Update the equipment display with current values.
    /// </summary>
    /// <param name="equipment">The character equipment to update the display for</param>
    /// <param name="container">The container to update</param>
    private void UpdateEquipmentDisplay(Equipment equipment, Control container)
    {
        if (equipment == null || container == null)
        {
            return;
        }

        // Update the display with the character's current equipment
        // This would typically involve updating labels, icons, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var weaponLabel = container.GetNode<Label>("Weapon");
        // weaponLabel.Text = $"Weapon: {equipment.Weapon?.Name ?? "None"}";

        // var armorLabel = container.GetNode<Label>("Armor");
        // armorLabel.Text = $"Armor: {equipment.Armor?.Name ?? "None"}";

        // var accessoryLabel = container.GetNode<Label>("Accessory");
        // accessoryLabel.Text = $"Accessory: {equipment.Accessory?.Name ?? "None"}";
    }

    /// <summary>
    /// Update the skills display with current values.
    /// </summary>
    /// <param name="skills">The character skills to update the display for</param>
    /// <param name="container">The container to update</param>
    private void UpdateSkillsDisplay(Skills skills, Control container)
    {
        if (skills == null || container == null)
        {
            return;
        }

        // Update the display with the character's current skills
        // This would typically involve updating labels, progress bars, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var skillsList = container.GetNode<VBoxContainer>("SkillsList");
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
    /// Update the portrait display with current values.
    /// </summary>
    /// <param name="portrait">The character portrait to update the display for</param>
    /// <param name="container">The container to update</param>
    private void UpdatePortraitDisplay(Texture2D portrait, Control container)
    {
        if (portrait == null || container == null)
        {
            return;
        }

        // Update the display with the character's current portrait
        // This would typically involve updating a TextureRect or similar control
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var portraitRect = container.GetNode<TextureRect>("Portrait");
        // portraitRect.Texture = portrait;
    }

    /// <summary>
    /// Create a display for character stats.
    /// </summary>
    /// <param name="stats">The character stats to create a display for</param>
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
    /// <param name="equipment">The character equipment to create a display for</param>
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
    /// <param name="skills">The character skills to create a display for</param>
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
    /// <param name="portrait">The character portrait to create a display for</param>
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
}
