// <copyright file="PartyData.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Scripts
{
    using Godot;

    /// <summary>
    /// Classic CRPG party creation and management system.
    /// </summary>
    public class PartyData
    {
        /// <summary>
        /// Gets or sets the list of party members.
        /// </summary>
        public List<Character> Members { get; set; } = new();

        /// <summary>
        /// Gets or sets the party's gold amount.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// Gets or sets the party's inventory as a dictionary of item names to quantities.
        /// </summary>
        public Godot.Collections.Dictionary<string, int> Inventory { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyData"/> class.
        /// </summary>
        /// <remarks>Default constructor that creates an empty party with no members, zero gold, and empty inventory.</remarks>
        public PartyData()
        {
        }

        /// <summary>
        /// Adds a character to the party if there is space (maximum 3 members).
        /// </summary>
        /// <param name="character">The character to add.</param>
        /// <returns><see langword="true"/> if character was added, <see langword="false"/> if party is full.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="character"/> is <c>null</c>.</exception>
        public bool AddMember(Character character)
        {
            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }

            if (this.Members.Count >= 3)
            {
                GD.Print("Party is full (maximum 3 members)");
                return false;
            }

            this.Members.Add(character);
            GD.Print($"Added {character.Name} to party (now {this.Members.Count}/3 members)");
            return true;
        }

        /// <summary>
        /// Removes a character from the party by index.
        /// </summary>
        /// <param name="index">Index of character to remove.</param>
        /// <returns><see langword="true"/> if character was removed, <see langword="false"/> if index is invalid.</returns>
        public bool RemoveMember(int index)
        {
            if (index < 0 || index >= this.Members.Count)
            {
                GD.Print($"Invalid party member index: {index}");
                return false;
            }

            var character = this.Members[index];
            this.Members.RemoveAt(index);
            GD.Print($"Removed {character.Name} from party (now {this.Members.Count}/3 members)");
            return true;
        }

        /// <summary>
        /// Gets a character by index.
        /// </summary>
        /// <param name="index">Index of character to get.</param>
        /// <returns>Character if index is valid, <see langword="null"/> otherwise.</returns>
        public Character? GetMember(int index)
        {
            if (index < 0 || index >= this.Members.Count)
            {
                return null;
            }

            return this.Members[index];
        }

        /// <summary>
        /// Checks if the party is complete (has 3 members).
        /// </summary>
        /// <returns><see langword="true"/> if the party has 3 members, <see langword="false"/> otherwise.</returns>
        public bool IsComplete()
        {
            return this.Members.Count >= 3;
        }

        /// <summary>
        /// Gets the total level of all party members.
        /// </summary>
        /// <returns>The sum of all party member levels.</returns>
        public int GetTotalLevel()
        {
            int total = 0;
            foreach (var member in this.Members)
            {
                total += member.Level;
            }

            return total;
        }

        /// <summary>
        /// Calculates the average party level.
        /// </summary>
        /// <returns>The average level of all party members, or 0 if party is empty.</returns>
        public double GetAverageLevel()
        {
            if (this.Members.Count == 0)
            {
                return 0;
            }

            return (double) this.GetTotalLevel() / this.Members.Count;
        }

        /// <summary>
        /// Adds gold to the party's treasury.
        /// </summary>
        /// <param name="amount">Amount of gold to add. Must be positive.</param>
        public void AddGold(int amount)
        {
            if (amount > 0)
            {
                this.Gold += amount;
                GD.Print($"Added {amount} gold to party treasury (total: {this.Gold})");
            }
        }

        /// <summary>
        /// Removes gold from the party's treasury.
        /// </summary>
        /// <param name="amount">Amount of gold to remove.</param>
        /// <returns><see langword="true"/> if gold was removed, <see langword="false"/> if insufficient funds.</returns>
        public bool RemoveGold(int amount)
        {
            if (amount > this.Gold)
            {
                GD.Print($"Insufficient gold: need {amount}, have {this.Gold}");
                return false;
            }

            this.Gold -= amount;
            GD.Print($"Removed {amount} gold from party treasury (remaining: {this.Gold})");
            return true;
        }

        /// <summary>
        /// Adds an item to the party inventory.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="quantity">Quantity to add (default 1). Must be positive.</param>
        public void AddItem(string itemName, int quantity = 1)
        {
            if (this.Inventory.ContainsKey(itemName))
            {
                this.Inventory[itemName] += quantity;
            }
            else
            {
                this.Inventory[itemName] = quantity;
            }

            GD.Print($"Added {quantity} x {itemName} to party inventory");
        }

        /// <summary>
        /// Removes an item from the party inventory.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="quantity">Quantity to remove (default 1).</param>
        /// <returns><see langword="true"/> if item was removed, <see langword="false"/> if insufficient quantity.</returns>
        public bool RemoveItem(string itemName, int quantity = 1)
        {
            if (!this.Inventory.ContainsKey(itemName) || this.Inventory[itemName] < quantity)
            {
                GD.Print($"Insufficient {itemName}: need {quantity}, have {this.Inventory.GetValueOrDefault(itemName, 0)}");
                return false;
            }

            this.Inventory[itemName] -= quantity;

            if (this.Inventory[itemName] <= 0)
            {
                this.Inventory.Remove(itemName);
            }

            GD.Print($"Removed {quantity} x {itemName} from party inventory");
            return true;
        }

        /// <summary>
        /// Gets the quantity of a specific item in the inventory.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <returns>Quantity of the item, or 0 if not found.</returns>
        public int GetItemQuantity(string itemName)
        {
            return this.Inventory.GetValueOrDefault(itemName, 0);
        }

        /// <summary>
        /// Converts party data to a Godot dictionary for serialization.
        /// </summary>
        /// <returns>A dictionary containing members, gold, and inventory data.</returns>
        public Godot.Collections.Dictionary<string, Variant> ToDictionary()
        {
            var membersArray = new Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>>();
            foreach (var member in this.Members)
            {
                membersArray.Add(member.ToDictionary());
            }

            return new Godot.Collections.Dictionary<string, Variant>
            {
                ["members"] = membersArray,
                ["gold"] = this.Gold,
                ["inventory"] = this.Inventory,
            };
        }

        /// <summary>
        /// Creates party data from a Godot dictionary.
        /// </summary>
        /// <param name="dict">The dictionary containing serialized party data.</param>
        /// <returns>A new <see cref="PartyData"/> instance populated from the dictionary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dict"/> is <c>null</c>.</exception>
        public static PartyData FromDictionary(Godot.Collections.Dictionary<string, Variant> dict)
        {
            if (dict == null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            var party = new PartyData();

            if (dict.ContainsKey("gold"))
            {
                party.Gold = (int) dict["gold"];
            }

            if (dict.ContainsKey("inventory"))
            {
                var inventoryVar = dict["inventory"];
                if (inventoryVar.VariantType == Variant.Type.Dictionary)
                {
                    var inventoryDict = inventoryVar.AsGodotDictionary<string, Variant>();
                    foreach (var kvp in inventoryDict)
                    {
                        party.Inventory[kvp.Key] = kvp.Value.AsInt32();
                    }
                }
            }

            if (dict.ContainsKey("members"))
            {
                var membersVar = dict["members"];
                if (membersVar.VariantType == Variant.Type.Array)
                {
                    var membersArray = membersVar.AsGodotArray();
                    foreach (var memberVar in membersArray)
                    {
                        if (memberVar.VariantType == Variant.Type.Dictionary)
                        {
                            var memberDict = memberVar.AsGodotDictionary<string, Variant>();
                            var character = Character.FromDictionary(memberDict);
                            party.Members.Add(character);
                        }
                    }
                }
            }

            return party;
        }
    }
}
