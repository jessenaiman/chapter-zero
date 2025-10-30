using Godot;

namespace OmegaSpiral.Domain
{
    /// <summary>
    /// Manages signals and events for the domain layer using proper Godot C# API conventions.
    /// </summary>
    public partial class SignalManager : Node
    {
        /// <summary>
        /// Emitted when a character's stats change.
        /// </summary>
        /// <param name="characterId">The ID of the character whose stats changed.</param>
        /// <param name="statChanges">The stat changes as an array of integers.</param>
        [Signal]
        public delegate void CharacterStatsChangedEventHandler(string characterId, Godot.Collections.Array<int> statChanges);

        /// <summary>
        /// Emitted when a character levels up.
        /// </summary>
        /// <param name="characterId">The ID of the character that leveled up.</param>
        /// <param name="newLevel">The new level of the character.</param>
        [Signal]
        public delegate void CharacterLeveledUpEventHandler(string characterId, int newLevel);

        /// <summary>
        /// Emitted when an item is added to inventory.
        /// </summary>
        /// <param name="itemId">The ID of the item that was added.</param>
        /// <param name="quantity">The quantity of the item that was added.</param>
        /// <param name="inventoryId">The ID of the inventory.</param>
        [Signal]
        public delegate void ItemAddedEventHandler(string itemId, int quantity, string inventoryId);

        /// <summary>
        /// Emitted when an item is removed from inventory.
        /// </summary>
        /// <param name="itemId">The ID of the item that was removed.</param>
        /// <param name="quantity">The quantity of the item that was removed.</param>
        /// <param name="inventoryId">The ID of the inventory.</param>
        [Signal]
        public delegate void ItemRemovedEventHandler(string itemId, int quantity, string inventoryId);

        /// <summary>
        /// Emitted when equipment is equipped.
        /// </summary>
        /// <param name="characterId">The ID of the character.</param>
        /// <param name="equipmentId">The ID of the equipment.</param>
        /// <param name="slot">The equipment slot.</param>
        [Signal]
        public delegate void EquipmentEquippedEventHandler(string characterId, string equipmentId, string slot);

        /// <summary>
        /// Emitted when equipment is unequipped.
        /// </summary>
        /// <param name="characterId">The ID of the character.</param>
        /// <param name="equipmentId">The ID of the equipment.</param>
        /// <param name="slot">The equipment slot.</param>
        [Signal]
        public delegate void EquipmentUnequippedEventHandler(string characterId, string equipmentId, string slot);

        /// <summary>
        /// Emits the CharacterStatsChanged signal.
        /// </summary>
        /// <param name="characterId">The ID of the character whose stats changed.</param>
        /// <param name="statChanges">The stat changes as an array of integers.</param>
        public void EmitCharacterStatsChanged(string characterId, Godot.Collections.Array<int> statChanges)
        {
            this.EmitSignal(SignalName.CharacterStatsChanged, characterId, statChanges);
        }

        /// <summary>
        /// Emits the CharacterLeveledUp signal.
        /// </summary>
        /// <param name="characterId">The ID of the character that leveled up.</param>
        /// <param name="newLevel">The new level of the character.</param>
        public void EmitCharacterLeveledUp(string characterId, int newLevel)
        {
            this.EmitSignal(SignalName.CharacterLeveledUp, characterId, newLevel);
        }

        /// <summary>
        /// Emits the ItemAdded signal.
        /// </summary>
        /// <param name="itemId">The ID of the item that was added.</param>
        /// <param name="quantity">The quantity of the item that was added.</param>
        /// <param name="inventoryId">The ID of the inventory.</param>
        public void EmitItemAdded(string itemId, int quantity, string inventoryId)
        {
            this.EmitSignal(SignalName.ItemAdded, itemId, quantity, inventoryId);
        }

        /// <summary>
        /// Emits the ItemRemoved signal.
        /// </summary>
        /// <param name="itemId">The ID of the item that was removed.</param>
        /// <param name="quantity">The quantity of the item that was removed.</param>
        /// <param name="inventoryId">The ID of the inventory.</param>
        public void EmitItemRemoved(string itemId, int quantity, string inventoryId)
        {
            this.EmitSignal(SignalName.ItemRemoved, itemId, quantity, inventoryId);
        }

        /// <summary>
        /// Emits the EquipmentEquipped signal.
        /// </summary>
        /// <param name="characterId">The ID of the character.</param>
        /// <param name="equipmentId">The ID of the equipment.</param>
        /// <param name="slot">The equipment slot.</param>
        public void EmitEquipmentEquipped(string characterId, string equipmentId, string slot)
        {
            this.EmitSignal(SignalName.EquipmentEquipped, characterId, equipmentId, slot);
        }

        /// <summary>
        /// Emits the EquipmentUnequipped signal.
        /// </summary>
        /// <param name="characterId">The ID of the character.</param>
        /// <param name="equipmentId">The ID of the equipment.</param>
        /// <param name="slot">The equipment slot.</param>
        public void EmitEquipmentUnequipped(string characterId, string equipmentId, string slot)
        {
            this.EmitSignal(SignalName.EquipmentUnequipped, characterId, equipmentId, slot);
        }

        /// <summary>
        /// Connects to the CharacterStatsChanged signal.
        /// </summary>
        /// <param name="callable">The callable to connect.</param>
        public void ConnectCharacterStatsChanged(Callable callable)
        {
            this.Connect(SignalName.CharacterStatsChanged, callable);
        }

        /// <summary>
        /// Connects to the CharacterLeveledUp signal.
        /// </summary>
        /// <param name="callable">The callable to connect.</param>
        public void ConnectCharacterLeveledUp(Callable callable)
        {
            this.Connect(SignalName.CharacterLeveledUp, callable);
        }

        /// <summary>
        /// Connects to the ItemAdded signal.
        /// </summary>
        /// <param name="callable">The callable to connect.</param>
        public void ConnectItemAdded(Callable callable)
        {
            this.Connect(SignalName.ItemAdded, callable);
        }

        /// <summary>
        /// Connects to the ItemRemoved signal.
        /// </summary>
        /// <param name="callable">The callable to connect.</param>
        public void ConnectItemRemoved(Callable callable)
        {
            this.Connect(SignalName.ItemRemoved, callable);
        }

        /// <summary>
        /// Connects to the EquipmentEquipped signal.
        /// </summary>
        /// <param name="callable">The callable to connect.</param>
        public void ConnectEquipmentEquipped(Callable callable)
        {
            this.Connect(SignalName.EquipmentEquipped, callable);
        }

        /// <summary>
        /// Connects to the EquipmentUnequipped signal.
        /// </summary>
        /// <param name="callable">The callable to connect.</param>
        public void ConnectEquipmentUnequipped(Callable callable)
        {
            this.Connect(SignalName.EquipmentUnequipped, callable);
        }

        /// <summary>
        /// Disconnects from the CharacterStatsChanged signal.
        /// </summary>
        /// <param name="callable">The callable to disconnect.</param>
        public void DisconnectCharacterStatsChanged(Callable callable)
        {
            this.Disconnect(SignalName.CharacterStatsChanged, callable);
        }

        /// <summary>
        /// Disconnects from the CharacterLeveledUp signal.
        /// </summary>
        /// <param name="callable">The callable to disconnect.</param>
        public void DisconnectCharacterLeveledUp(Callable callable)
        {
            this.Disconnect(SignalName.CharacterLeveledUp, callable);
        }

        /// <summary>
        /// Disconnects from the ItemAdded signal.
        /// </summary>
        /// <param name="callable">The callable to disconnect.</param>
        public void DisconnectItemAdded(Callable callable)
        {
            this.Disconnect(SignalName.ItemAdded, callable);
        }

        /// <summary>
        /// Disconnects from the ItemRemoved signal.
        /// </summary>
        /// <param name="callable">The callable to disconnect.</param>
        public void DisconnectItemRemoved(Callable callable)
        {
            this.Disconnect(SignalName.ItemRemoved, callable);
        }

        /// <summary>
        /// Disconnects from the EquipmentEquipped signal.
        /// </summary>
        /// <param name="callable">The callable to disconnect.</param>
        public void DisconnectEquipmentEquipped(Callable callable)
        {
            this.Disconnect(SignalName.EquipmentEquipped, callable);
        }

        /// <summary>
        /// Disconnects from the EquipmentUnequipped signal.
        /// </summary>
        /// <param name="callable">The callable to disconnect.</param>
        public void DisconnectEquipmentUnequipped(Callable callable)
        {
            this.Disconnect(SignalName.EquipmentUnequipped, callable);
        }
    }
}
