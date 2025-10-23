// Copyright (c) Ωmega Spiral. All rights reserved.

using Godot;

namespace OmegaSpiral.Domain.Models
{
    /// <summary>
    /// Represents the data structure for exporting inventory data.
    /// </summary>
    public class InventoryExportData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryExportData"/> class.
        /// </summary>
        public InventoryExportData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryExportData"/> class from an inventory instance.
        /// </summary>
        /// <param name="inventory">The inventory to export data from.</param>
        public InventoryExportData(Inventory inventory)
        {
            if (inventory != null)
            {
                this.Id = inventory.Id;
                this.MaxSize = inventory.MaxSize;
                this.MaxWeight = inventory.MaxWeight;

                // Export inventory items
                // Note: This assumes the Inventory class has a way to access its items
                // The actual implementation would depend on how the Inventory class stores items
                this.ItemIds = new List<string>();
                this.ItemQuantities = new Dictionary<string, int>();

                // This would need to be adapted based on the actual Inventory class implementation
                // For now, assuming there's a way to iterate through items
                // foreach (var item in inventory.Items)
                // {
                //     ItemIds.Add(item.Id);
                //     ItemQuantities[item.Id] = item.Quantity;
                // }
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier for the inventory.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum size of the inventory.
        /// </summary>
        public int MaxSize { get; set; } = 30;

        /// <summary>
        /// Gets or sets the maximum weight capacity of the inventory.
        /// </summary>
        public float MaxWeight { get; set; } = 100.0f;

        /// <summary>
        /// Gets the list of item IDs in the inventory.
        /// </summary>
        public List<string> ItemIds { get; } = new List<string>();

        /// <summary>
        /// Gets the quantities for each item in the inventory.
        /// </summary>
        public Dictionary<string, int> ItemQuantities { get; } = new Dictionary<string, int>();

        /// <summary>
        /// Gets the equipped item IDs for this inventory.
        /// </summary>
        public Dictionary<string, string> EquippedItems { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the custom item data for items with special properties.
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> CustomItemData { get; } = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// Gets the inventory categories and their item assignments.
        /// </summary>
        public Dictionary<string, List<string>> CategoryAssignments { get; } = new Dictionary<string, List<string>>();

        /// <summary>
        /// Gets the sorting preferences for the inventory.
        /// </summary>
        public Dictionary<string, object> SortingPreferences { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the timestamp of when this data was exported.
        /// </summary>
        public string ExportTimestamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets or sets the version of the export format.
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Gets or sets the source game or application.
        /// </summary>
        public string Source { get; set; } = "OmegaSpiral";

        /// <summary>
        /// Gets additional metadata for the export.
        /// </summary>
        public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Creates export data from a JSON string representation.
        /// </summary>
        /// <summary>
        /// Creates export data from a JSON string representation.
        /// </summary>
        /// <param name="json">The JSON string to parse.</param>
        /// <returns>A new instance of <see cref="InventoryExportData"/> parsed from the JSON.</returns>
        public static InventoryExportData FromJson(string json)
        {
            var exportData = new InventoryExportData();

            if (string.IsNullOrEmpty(json))
            {
                return exportData;
            }

            var jsonData = Json.ParseString(json);
            if (jsonData.VariantType != Variant.Type.Dictionary)
            {
                return exportData;
            }

            var jsonDict = jsonData.AsGodotDictionary();

            // Parse simple fields
            exportData.Id = GetStringValue(jsonDict, "Id", string.Empty);
            exportData.MaxSize = GetIntValue(jsonDict, "MaxSize", 0);
            exportData.MaxWeight = GetFloatValue(jsonDict, "MaxWeight", 0.0f);
            exportData.ExportTimestamp = GetStringValue(jsonDict, "ExportTimestamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture));
            exportData.Version = GetStringValue(jsonDict, "Version", "1.0");
            exportData.Source = GetStringValue(jsonDict, "Source", "OmegaSpiral");

            // Parse array fields
            CopyArrayData(jsonDict, "ItemIds", exportData.ItemIds);

            // Parse dictionary fields
            CopyIntDictionaryData(jsonDict, "ItemQuantities", exportData.ItemQuantities);
            CopyStringDictionaryData(jsonDict, "EquippedItems", exportData.EquippedItems);
            CopyNestedDictionaryData(jsonDict, "CustomItemData", exportData.CustomItemData);
            CopyArrayDictionaryData(jsonDict, "CategoryAssignments", exportData.CategoryAssignments);
            CopyDictionaryData(jsonDict, "SortingPreferences", exportData.SortingPreferences);
            CopyDictionaryData(jsonDict, "Metadata", exportData.Metadata);

            return exportData;
        }

        /// <summary>
        /// Gets a string value from the JSON data with a default fallback.
        /// </summary>
        private static string GetStringValue(Godot.Collections.Dictionary jsonData, string key, string defaultValue)
        {
            if (jsonData.ContainsKey(key))
            {
                return jsonData[key].ToString() ?? defaultValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets an integer value from the JSON data with a default fallback.
        /// </summary>
        private static int GetIntValue(Godot.Collections.Dictionary jsonData, string key, int defaultValue)
        {
            if (jsonData.ContainsKey(key))
            {
                return Convert.ToInt32(jsonData[key], System.Globalization.CultureInfo.InvariantCulture);
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a float value from the JSON data with a default fallback.
        /// </summary>
        private static float GetFloatValue(Godot.Collections.Dictionary jsonData, string key, float defaultValue)
        {
            if (jsonData.ContainsKey(key))
            {
                return Convert.ToSingle(jsonData[key], System.Globalization.CultureInfo.InvariantCulture);
            }
            return defaultValue;
        }

        /// <summary>
        /// Copies array data from JSON to a list.
        /// </summary>
        private static void CopyArrayData(Godot.Collections.Dictionary jsonData, string key, List<string> targetList)
        {
            if (jsonData.ContainsKey(key))
            {
                var arrayVariant = jsonData[key];
                if (arrayVariant.VariantType == Variant.Type.Array)
                {
                    var array = arrayVariant.AsGodotArray();
                    foreach (var item in array)
                    {
                        targetList.Add(item.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Copies dictionary data from JSON to the target dictionary.
        /// </summary>
        private static void CopyDictionaryData(Godot.Collections.Dictionary jsonData, string key, Dictionary<string, object> targetDictionary)
        {
            if (jsonData.ContainsKey(key))
            {
                var dataVariant = jsonData[key];
                if (dataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = dataVariant.AsGodotDictionary();
                    targetDictionary.Clear();
                    foreach (var dictKey in godotDict.Keys)
                    {
                        targetDictionary[dictKey.ToString()] = godotDict[dictKey];
                    }
                }
            }
        }

        /// <summary>
        /// Copies integer dictionary data from JSON to the target dictionary.
        /// </summary>
        private static void CopyIntDictionaryData(Godot.Collections.Dictionary jsonData, string key, Dictionary<string, int> targetDictionary)
        {
            if (jsonData.ContainsKey(key))
            {
                var dataVariant = jsonData[key];
                if (dataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = dataVariant.AsGodotDictionary();
                    targetDictionary.Clear();
                    foreach (var dictKey in godotDict.Keys)
                    {
                        var keyStr = dictKey.ToString();
                        var valueVariant = godotDict[dictKey];
                        if (valueVariant.VariantType == Variant.Type.Int)
                        {
                            targetDictionary[keyStr] = valueVariant.AsInt32();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Copies string dictionary data from JSON to the target dictionary.
        /// </summary>
        private static void CopyStringDictionaryData(Godot.Collections.Dictionary jsonData, string key, Dictionary<string, string> targetDictionary)
        {
            if (jsonData.ContainsKey(key))
            {
                var dataVariant = jsonData[key];
                if (dataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = dataVariant.AsGodotDictionary();
                    targetDictionary.Clear();
                    foreach (var dictKey in godotDict.Keys)
                    {
                        var keyStr = dictKey.ToString();
                        var valueVariant = godotDict[dictKey];
                        targetDictionary[keyStr] = valueVariant.ToString() ?? string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Copies nested dictionary data from JSON to the target dictionary.
        /// </summary>
        private static void CopyNestedDictionaryData(Godot.Collections.Dictionary jsonData, string key, Dictionary<string, Dictionary<string, object>> targetDictionary)
        {
            if (jsonData.ContainsKey(key))
            {
                var dataVariant = jsonData[key];
                if (dataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = dataVariant.AsGodotDictionary();
                    targetDictionary.Clear();
                    foreach (var dictKey in godotDict.Keys)
                    {
                        var keyStr = dictKey.ToString();
                        var innerVariant = godotDict[dictKey];
                        if (innerVariant.VariantType == Variant.Type.Dictionary)
                        {
                            var innerDict = innerVariant.AsGodotDictionary();
                            var itemData = new Dictionary<string, object>();
                            foreach (var innerKey in innerDict.Keys)
                            {
                                itemData[innerKey.ToString()] = innerDict[innerKey];
                            }
                            targetDictionary[keyStr] = itemData;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Copies array dictionary data from JSON to the target dictionary.
        /// </summary>
        private static void CopyArrayDictionaryData(Godot.Collections.Dictionary jsonData, string key, Dictionary<string, List<string>> targetDictionary)
        {
            if (jsonData.ContainsKey(key))
            {
                var dataVariant = jsonData[key];
                if (dataVariant.VariantType == Variant.Type.Dictionary)
                {
                    var godotDict = dataVariant.AsGodotDictionary();
                    targetDictionary.Clear();
                    foreach (var dictKey in godotDict.Keys)
                    {
                        var keyStr = dictKey.ToString();
                        var listVariant = godotDict[dictKey];
                        if (listVariant.VariantType == Variant.Type.Array)
                        {
                            var list = listVariant.AsGodotArray();
                            var stringList = new List<string>();
                            foreach (var item in list)
                            {
                                stringList.Add(item.ToString());
                            }
                            targetDictionary[keyStr] = stringList;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates whether this export data contains all required information.
        /// </summary>
        /// <returns><see langword="true"/> if the data is valid for export, <see langword="false"/> otherwise.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Id) &&
                   this.ItemIds != null &&
                   this.ItemQuantities != null &&
                   this.EquippedItems != null;
        }

        /// <summary>
        /// Creates an inventory instance from this export data.
        /// </summary>
        /// <returns>A new inventory instance populated from the export data.</returns>
        public Inventory CreateInventory()
        {
            var inventory = new Inventory
            {
                Id = this.Id,
                MaxSize = this.MaxSize,
                MaxWeight = this.MaxWeight,
            };

            // Add items from the export data
            foreach (var itemId in this.ItemIds)
            {
                var quantity = this.ItemQuantities.TryGetValue(itemId, out int value) ? value : 1;
                inventory.AddItem(itemId, quantity);
            }

            return inventory;
        }

        /// <summary>
        /// Converts this export data to a JSON string representation.
        /// </summary>
        /// <returns>A JSON string representation of the export data.</returns>
        public string ToJson()
        {
            var disposables = new List<System.IDisposable>();
            try
            {
                var itemIdsArray = new Godot.Collections.Array();
                disposables.Add(itemIdsArray);
                foreach (var itemId in this.ItemIds)
                {
                    itemIdsArray.Add(itemId);
                }

                var itemQuantitiesDict = ConvertToGodotDict(this.ItemQuantities);
                disposables.Add(itemQuantitiesDict);

                var equippedItemsDict = ConvertToGodotDict(this.EquippedItems);
                disposables.Add(equippedItemsDict);

                var customItemDataDict = new Godot.Collections.Dictionary();
                disposables.Add(customItemDataDict);
                foreach (var kvp in this.CustomItemData)
                {
                    var innerDict = ConvertToGodotDict(kvp.Value);
                    disposables.Add(innerDict);
                    customItemDataDict[kvp.Key] = innerDict;
                }

                var categoryAssignmentsDict = new Godot.Collections.Dictionary();
                disposables.Add(categoryAssignmentsDict);
                foreach (var kvp in this.CategoryAssignments)
                {
                    var categoryArray = new Godot.Collections.Array();
                    disposables.Add(categoryArray);
                    foreach (var item in kvp.Value)
                    {
                        categoryArray.Add(item);
                    }

                    categoryAssignmentsDict[kvp.Key] = categoryArray;
                }

                var sortingPreferencesDict = ConvertToGodotDict(this.SortingPreferences);
                disposables.Add(sortingPreferencesDict);

                var metadataDict = ConvertToGodotDict(this.Metadata);
                disposables.Add(metadataDict);

                var exportDict = new Godot.Collections.Dictionary
                {
                    ["Id"] = this.Id,
                    ["MaxSize"] = this.MaxSize,
                    ["MaxWeight"] = this.MaxWeight,
                    ["ItemIds"] = itemIdsArray,
                    ["ItemQuantities"] = itemQuantitiesDict,
                    ["EquippedItems"] = equippedItemsDict,
                    ["CustomItemData"] = customItemDataDict,
                    ["CategoryAssignments"] = categoryAssignmentsDict,
                    ["SortingPreferences"] = sortingPreferencesDict,
                    ["ExportTimestamp"] = this.ExportTimestamp,
                    ["Version"] = this.Version,
                    ["Source"] = this.Source,
                    ["Metadata"] = metadataDict,
                };
                disposables.Add(exportDict);

                return Json.Stringify(exportDict);
            }
            finally
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// Adds an item to this export data configuration.
        /// </summary>
        /// <param name="itemId">The ID of the item to add.</param>
        /// <param name="quantity">The quantity of the item to add.</param>
        public void AddItem(string itemId, int quantity = 1)
        {
            if (!string.IsNullOrEmpty(itemId) && quantity > 0)
            {
                if (!this.ItemIds.Contains(itemId))
                {
                    this.ItemIds.Add(itemId);
                }

                this.ItemQuantities[itemId] = quantity;
            }
        }

        /// <summary>
        /// Removes an item from this export data configuration.
        /// </summary>
        /// <param name="itemId">The ID of the item to remove.</param>
        public void RemoveItem(string itemId)
        {
            if (!string.IsNullOrEmpty(itemId))
            {
                this.ItemIds.Remove(itemId);
                this.ItemQuantities.Remove(itemId);

                // Remove from equipped items if present
                var equippedSlots = new List<string>();
                foreach (var kvp in this.EquippedItems)
                {
                    if (kvp.Value == itemId)
                    {
                        equippedSlots.Add(kvp.Key);
                    }
                }

                foreach (var slot in equippedSlots)
                {
                    this.EquippedItems.Remove(slot);
                }

                // Remove from custom data if present
                this.CustomItemData.Remove(itemId);

                // Remove from categories if present
                foreach (var kvp in this.CategoryAssignments)
                {
                    kvp.Value.Remove(itemId);
                }
            }
        }

        /// <summary>
        /// Sets custom data for a specific item.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="customData">The custom data dictionary for the item.</param>
        public void SetItemCustomData(string itemId, Dictionary<string, object> customData)
        {
            if (!string.IsNullOrEmpty(itemId) && customData != null)
            {
                this.CustomItemData[itemId] = customData;
            }
        }

        /// <summary>
        /// Gets custom data for a specific item.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <returns>The custom data dictionary for the item, or null if not found.</returns>
        public Dictionary<string, object>? GetItemCustomData(string itemId)
        {
            return this.CustomItemData.TryGetValue(itemId, out Dictionary<string, object>? value) ? value : null;
        }

        /// <summary>
        /// Gets the total weight of all items in this export data.
        /// </summary>
        /// <param name="itemWeightProvider">A function that provides the weight of an item by its ID.</param>
        /// <returns>The total weight of all items in the export data.</returns>
        public float GetTotalWeight(System.Func<string, float> itemWeightProvider)
        {
            float totalWeight = 0f;

            foreach (var itemId in this.ItemIds)
            {
                var quantity = this.ItemQuantities.TryGetValue(itemId, out int value) ? value : 1;
                var itemWeight = itemWeightProvider?.Invoke(itemId) ?? 0f;
                totalWeight += itemWeight * quantity;
            }

            return totalWeight;
        }

        /// <summary>
        /// Gets the total number of unique items in this export data.
        /// </summary>
        /// <returns>The count of unique items in the export data.</returns>
        public int GetUniqueItemCount()
        {
            return this.ItemIds.Count;
        }

        /// <summary>
        /// Gets the total quantity of all items in this export data.
        /// </summary>
        /// <returns>The total quantity of all items in the export data.</returns>
        public int GetTotalItemCount()
        {
            int totalCount = 0;
            foreach (var kvp in this.ItemQuantities)
            {
                totalCount += kvp.Value;
            }

            return totalCount;
        }

        /// <summary>
        /// Clears all items from this export data.
        /// </summary>
        public void ClearAllItems()
        {
            this.ItemIds.Clear();
            this.ItemQuantities.Clear();
            this.EquippedItems.Clear();
            this.CustomItemData.Clear();
            this.CategoryAssignments.Clear();
        }

        /// <summary>
        /// Creates a deep copy of this export data.
        /// </summary>
        /// <returns>A new instance of <see cref="InventoryExportData"/> with the same values.</returns>
        public InventoryExportData Clone()
        {
            var clone = new InventoryExportData
            {
                Id = this.Id,
                MaxSize = this.MaxSize,
                MaxWeight = this.MaxWeight,
                ExportTimestamp = this.ExportTimestamp,
                Version = this.Version,
                Source = this.Source,
            };

            foreach (var itemId in this.ItemIds)
            {
                clone.ItemIds.Add(itemId);
            }

            foreach (var kvp in this.ItemQuantities)
            {
                clone.ItemQuantities[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.EquippedItems)
            {
                clone.EquippedItems[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.CustomItemData)
            {
                clone.CustomItemData[kvp.Key] = new Dictionary<string, object>(kvp.Value);
            }

            foreach (var kvp in this.CategoryAssignments)
            {
                clone.CategoryAssignments[kvp.Key] = new List<string>(kvp.Value);
            }

            foreach (var kvp in this.SortingPreferences)
            {
                clone.SortingPreferences[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in this.Metadata)
            {
                clone.Metadata[kvp.Key] = kvp.Value;
            }

            return clone;
        }

        private static Godot.Collections.Dictionary ConvertToGodotDict(Dictionary<string, object> dict)
        {
            var godotDict = new Godot.Collections.Dictionary();
            foreach (var kvp in dict)
            {
                godotDict[kvp.Key] = (Variant) kvp.Value;
            }

            return godotDict;
        }

        private static Godot.Collections.Dictionary ConvertToGodotDict(Dictionary<string, int> dict)
        {
            var godotDict = new Godot.Collections.Dictionary();
            foreach (var kvp in dict)
            {
                godotDict[kvp.Key] = kvp.Value;
            }

            return godotDict;
        }

        private static Godot.Collections.Dictionary ConvertToGodotDict(Dictionary<string, string> dict)
        {
            var godotDict = new Godot.Collections.Dictionary();
            foreach (var kvp in dict)
            {
                godotDict[kvp.Key] = kvp.Value;
            }

            return godotDict;
        }
    }
}
