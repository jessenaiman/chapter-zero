using Godot;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Classic CRPG party creation and management system.
    /// </summary>
    public class PartyData
    {
        public Godot.Collections.Array<Character> Members { get; set; } = new();
        public int Gold { get; set; } = 0;
        public Godot.Collections.Dictionary<string, int> Inventory { get; set; } = new();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PartyData()
        {
        }

        /// <summary>
        /// Add a character to the party if there's space (max 3 members by default).
        /// </summary>
        /// <param name="character">The character to add</param>
        /// <returns>True if character was added, false if party is full</returns>
        public bool AddMember(Character character)
        {
            if (Members.Count >= 3)
            {
                GD.Print("Party is full (maximum 3 members)");
                return false;
            }

            Members.Add(character);
            GD.Print($"Added {character.Name} to party (now {Members.Count}/3 members)");
            return true;
        }

        /// <summary>
        /// Remove a character from the party by index.
        /// </summary>
        /// <param name="index">Index of character to remove</param>
        /// <returns>True if character was removed, false if index is invalid</returns>
        public bool RemoveMember(int index)
        {
            if (index < 0 || index >= Members.Count)
            {
                GD.Print($"Invalid party member index: {index}");
                return false;
            }

            var character = Members[index];
            Members.RemoveAt(index);
            GD.Print($"Removed {character.Name} from party (now {Members.Count}/3 members)");
            return true;
        }

        /// <summary>
        /// Get a character by index.
        /// </summary>
        /// <param name="index">Index of character to get</param>
        /// <returns>Character if index is valid, null otherwise</returns>
        public Character GetMember(int index)
        {
            if (index < 0 || index >= Members.Count)
                return null;

            return Members[index];
        }

        /// <summary>
        /// Check if the party is complete (has 3 members).
        /// </summary>
        public bool IsComplete()
        {
            return Members.Count >= 3;
        }

        /// <summary>
        /// Get the total level of all party members.
        /// </summary>
        public int GetTotalLevel()
        {
            int total = 0;
            foreach (var member in Members)
            {
                total += member.Level;
            }
            return total;
        }

        /// <summary>
        /// Calculate average party level.
        /// </summary>
        public double GetAverageLevel()
        {
            if (Members.Count == 0)
                return 0;

            return (double)GetTotalLevel() / Members.Count;
        }

        /// <summary>
        /// Add gold to the party's treasury.
        /// </summary>
        /// <param name="amount">Amount of gold to add</param>
        public void AddGold(int amount)
        {
            if (amount > 0)
            {
                Gold += amount;
                GD.Print($"Added {amount} gold to party treasury (total: {Gold})");
            }
        }

        /// <summary>
        /// Remove gold from the party's treasury.
        /// </summary>
        /// <param name="amount">Amount of gold to remove</param>
        /// <returns>True if gold was removed, false if insufficient funds</returns>
        public bool RemoveGold(int amount)
        {
            if (amount > Gold)
            {
                GD.Print($"Insufficient gold: need {amount}, have {Gold}");
                return false;
            }

            Gold -= amount;
            GD.Print($"Removed {amount} gold from party treasury (remaining: {Gold})");
            return true;
        }

        /// <summary>
        /// Add an item to the party inventory.
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <param name="quantity">Quantity to add (default 1)</param>
        public void AddItem(string itemName, int quantity = 1)
        {
            if (Inventory.ContainsKey(itemName))
            {
                Inventory[itemName] += quantity;
            }
            else
            {
                Inventory[itemName] = quantity;
            }

            GD.Print($"Added {quantity} x {itemName} to party inventory");
        }

        /// <summary>
        /// Remove an item from the party inventory.
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <param name="quantity">Quantity to remove (default 1)</param>
        /// <returns>True if item was removed, false if insufficient quantity</returns>
        public bool RemoveItem(string itemName, int quantity = 1)
        {
            if (!Inventory.ContainsKey(itemName) || Inventory[itemName] < quantity)
            {
                GD.Print($"Insufficient {itemName}: need {quantity}, have {Inventory.GetValueOrDefault(itemName, 0)}");
                return false;
            }

            Inventory[itemName] -= quantity;

            if (Inventory[itemName] <= 0)
            {
                Inventory.Remove(itemName);
            }

            GD.Print($"Removed {quantity} x {itemName} from party inventory");
            return true;
        }

        /// <summary>
        /// Get the quantity of a specific item in inventory.
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <returns>Quantity of the item, or 0 if not found</returns>
        public int GetItemQuantity(string itemName)
        {
            return Inventory.GetValueOrDefault(itemName, 0);
        }

        /// <summary>
        /// Convert party data to Godot dictionary for serialization.
        /// </summary>
        public Godot.Collections.Dictionary<string, Variant> ToDictionary()
        {
            var membersArray = new Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>>();
            foreach (var member in Members)
            {
                membersArray.Add(member.ToDictionary());
            }

            return new Godot.Collections.Dictionary<string, Variant>
            {
                ["members"] = membersArray,
                ["gold"] = Gold,
                ["inventory"] = new Godot.Collections.Dictionary<string, int>(Inventory)
            };
        }

        /// <summary>
        /// Create party data from Godot dictionary.
        /// </summary>
        public static PartyData FromDictionary(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var party = new PartyData();

            if (dict.ContainsKey("gold"))
                party.Gold = (int)dict["gold"];

            if (dict.ContainsKey("inventory") && dict["inventory"].Obj is Godot.Collections.Dictionary inventoryDict)
            {
                foreach (var key in inventoryDict.Keys)
                {
                    party.Inventory[(string)key] = (int)inventoryDict[key];
                }
            }

            if (dict.ContainsKey("members") && dict["members"].Obj is Godot.Collections.Array membersArray)
            {
                foreach (var memberVariant in membersArray)
                {
                    if (memberVariant.Obj is Godot.Collections.Dictionary memberDict)
                    {
                        var character = Character.FromDictionary(memberDict);
                        party.Members.Add(character);
                    }
                }
            }

            return party;
        }
    }
}