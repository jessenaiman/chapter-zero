
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Container for the combat log display.
/// The UICombatLog displays a record of combat events, actions, and outcomes during battles.
/// It provides players with a history of what has happened in combat, helping them understand
/// the flow of battle and make informed decisions.
/// </summary>
public partial class UICombatLog : Control
{
    /// <summary>
    /// The maximum number of log entries to keep.
    /// </summary>
    [Export]
    public int MaxLogEntries { get; set; } = 50;

    /// <summary>
    /// The container for log entries.
    /// </summary>
    private VBoxContainer logContainer;

    /// <summary>
    /// The list of log entries.
    /// </summary>
    private List<RichTextLabel> logEntries = new List<RichTextLabel>();

    /// <summary>
    /// Whether the log is currently visible.
    /// </summary>
    public bool LogVisible
    {
        get => Visible;
        set => Visible = value;
    }

    public override void _Ready()
    {
        // Get references to child UI elements
        logContainer = GetNode<VBoxContainer>("LogContainer");

        // Initially hide the log
        Visible = false;

        // Connect to any necessary signals
        ConnectSignals();
    }

    /// <summary>
    /// Connect to necessary signals.
    /// </summary>
    private void ConnectSignals()
    {
        // Connect to combat events
        // CombatEvents.BattlerActionTaken += OnBattlerActionTaken;
        // CombatEvents.BattlerDamaged += OnBattlerDamaged;
        // CombatEvents.BattlerHealed += OnBattlerHealed;
        // CombatEvents.BattlerMissed += OnBattlerMissed;
        // CombatEvents.StatusEffectApplied += OnStatusEffectApplied;
        // CombatEvents.BattlerDefeated += OnBattlerDefeated;
    }

    /// <summary>
    /// Add a log entry.
    /// </summary>
    /// <param name="text">The text of the log entry</param>
    /// <param name="color">The color of the log entry text</param>
    public void AddLogEntry(string text, Color? color = null)
    {
        if (string.IsNullOrEmpty(text) || logContainer == null)
        {
            return;
        }

        // Create a new log entry
        var logEntry = new RichTextLabel();
        logEntry.BbcodeEnabled = true;
        logEntry.FitContent = true;
        logEntry.ScrollFollowing = true;

        // Set the text with the specified color
        if (color.HasValue)
        {
            logEntry.AppendText($"[color=#{color.Value.ToHtml()}]{text}[/color]");
        }
        else
        {
            logEntry.AppendText(text);
        }

        // Add the log entry to the container
        logContainer.AddChild(logEntry);

        // Store the log entry in the list
        logEntries.Add(logEntry);

        // If we've exceeded the maximum number of entries, remove the oldest one
        if (logEntries.Count > MaxLogEntries)
        {
            var oldestEntry = logEntries[0];
            logEntries.RemoveAt(0);
            oldestEntry.QueueFree();
        }

        // Scroll to the bottom to show the new entry
        CallDeferred(MethodName.ScrollToBottom);
    }

    /// <summary>
    /// Add a damage log entry.
    /// </summary>
    /// <param name="battler">The battler that took damage</param>
    /// <param name="damageAmount">The amount of damage taken</param>
    public void AddDamageEntry(Battler battler, int damageAmount)
    {
        if (battler == null)
        {
            return;
        }

        var text = $"{battler.Name} takes {damageAmount} damage!";
        AddLogEntry(text, Colors.Red);
    }

    /// <summary>
    /// Add a healing log entry.
    /// </summary>
    /// <param name="battler">The battler that was healed</param>
    /// <param name="healAmount">The amount of healing received</param>
    public void AddHealEntry(Battler battler, int healAmount)
    {
        if (battler == null)
        {
            return;
        }

        var text = $"{battler.Name} heals for {healAmount} HP!";
        AddLogEntry(text, Colors.Green);
    }

    /// <summary>
    /// Add a miss log entry.
    /// </summary>
    /// <param name="battler">The battler that was missed</param>
    public void AddMissEntry(Battler battler)
    {
        if (battler == null)
        {
            return;
        }

        var text = $"{battler.Name} dodges the attack!";
        AddLogEntry(text, Colors.Gray);
    }

    /// <summary>
    /// Add an action log entry.
    /// </summary>
    /// <param name="battler">The battler that took the action</param>
    /// <param name="action">The action taken</param>
    /// <param name="targets">The targets of the action</param>
    public void AddActionEntry(Battler battler, BattlerAction action, List<Battler> targets)
    {
        if (battler == null || action == null || targets == null)
        {
            return;
        }

        var targetNames = string.Join(", ", targets.Select(t => t.Name));
        var text = $"{battler.Name} uses {action.Label} on {targetNames}!";
        AddLogEntry(text, Colors.Blue);
    }

    /// <summary>
    /// Add a status effect log entry.
    /// </summary>
    /// <param name="battler">The battler that received the status effect</param>
    /// <param name="statusEffect">The status effect applied</param>
    public void AddStatusEffectEntry(Battler battler, string statusEffect)
    {
        if (battler == null || string.IsNullOrEmpty(statusEffect))
        {
            return;
        }

        var text = $"{battler.Name} is afflicted with {statusEffect}!";
        AddLogEntry(text, Colors.Purple);
    }

    /// <summary>
    /// Add a battler defeated log entry.
    /// </summary>
    /// <param name="battler">The battler that was defeated</param>
    public void AddBattlerDefeatedEntry(Battler battler)
    {
        if (battler == null)
        {
            return;
        }

        var text = $"{battler.Name} has been defeated!";
        AddLogEntry(text, Colors.DarkRed);
    }

    /// <summary>
    /// Add a critical hit log entry.
    /// </summary>
    /// <param name="battler">The battler that took critical damage</param>
    /// <param name="damageAmount">The critical damage amount</param>
    public void AddCriticalHitEntry(Battler battler, int damageAmount)
    {
        if (battler == null)
        {
            return;
        }

        var text = $"CRITICAL HIT! {battler.Name} takes {damageAmount} critical damage!";
        AddLogEntry(text, Colors.Orange);
    }

    /// <summary>
    /// Add a buff log entry.
    /// </summary>
    /// <param name="battler">The battler that received the buff</param>
    /// <param name="buffName">The name of the buff</param>
    public void AddBuffEntry(Battler battler, string buffName)
    {
        if (battler == null || string.IsNullOrEmpty(buffName))
        {
            return;
        }

        var text = $"{battler.Name} gains {buffName}!";
        AddLogEntry(text, Colors.LightBlue);
    }

    /// <summary>
    /// Add a debuff log entry.
    /// </summary>
    /// <param name="battler">The battler that received the debuff</param>
    /// <param name="debuffName">The name of the debuff</param>
    public void AddDebuffEntry(Battler battler, string debuffName)
    {
        if (battler == null || string.IsNullOrEmpty(debuffName))
        {
            return;
        }

        var text = $"{battler.Name} suffers from {debuffName}!";
        AddLogEntry(text, Colors.Magenta);
    }

    /// <summary>
    /// Add a turn start log entry.
    /// </summary>
    /// <param name="battler">The battler whose turn is starting</param>
    public void AddTurnStartEntry(Battler battler)
    {
        if (battler == null)
        {
            return;
        }

        var text = $"{battler.Name}'s turn begins.";
        AddLogEntry(text, Colors.Yellow);
    }

    /// <summary>
    /// Add a turn end log entry.
    /// </summary>
    /// <param name="battler">The battler whose turn is ending</param>
    public void AddTurnEndEntry(Battler battler)
    {
        if (battler == null)
        {
            return;
        }

        var text = $"{battler.Name}'s turn ends.";
        AddLogEntry(text, Colors.Yellow);
    }

    /// <summary>
    /// Add a combat start log entry.
    /// </summary>
    public void AddCombatStartEntry()
    {
        var text = "Combat begins!";
        AddLogEntry(text, Colors.White);
    }

    /// <summary>
    /// Add a combat end log entry.
    /// </summary>
    /// <param name="playerWon">Whether the player won the combat</param>
    public void AddCombatEndEntry(bool playerWon)
    {
        var text = playerWon ? "Victory! The party wins the battle!" : "Defeat! The party has been vanquished!";
        var color = playerWon ? Colors.Gold : Colors.DarkRed;
        AddLogEntry(text, color);
    }

    /// <summary>
    /// Clear all log entries.
    /// </summary>
    public void ClearLog()
    {
        // Remove all log entries
        foreach (var entry in logEntries)
        {
            entry.QueueFree();
        }

        logEntries.Clear();
    }

    /// <summary>
    /// Scroll to the bottom of the log.
    /// </summary>
    private void ScrollToBottom()
    {
        if (logContainer != null)
        {
            // Scroll to the bottom of the container
            logContainer.CallDeferred("set_scroll_vertical", logContainer.GetVScrollBar().MaxValue);
        }
    }

    /// <summary>
    /// Callback when a battler takes an action.
    /// </summary>
    private void OnBattlerActionTaken(Battler battler, BattlerAction action, List<Battler> targets)
    {
        AddActionEntry(battler, action, targets);
    }

    /// <summary>
    /// Callback when a battler takes damage.
    /// </summary>
    private void OnBattlerDamaged(Battler battler, int damage)
    {
        AddDamageEntry(battler, damage);
    }

    /// <summary>
    /// Callback when a battler is healed.
    /// </summary>
    private void OnBattlerHealed(Battler battler, int healAmount)
    {
        AddHealEntry(battler, healAmount);
    }

    /// <summary>
    /// Callback when an attack misses a battler.
    /// </summary>
    private void OnBattlerMissed(Battler battler)
    {
        AddMissEntry(battler);
    }

    /// <summary>
    /// Callback when a status effect is applied to a battler.
    /// </summary>
    private void OnStatusEffectApplied(Battler battler, string statusEffect)
    {
        AddStatusEffectEntry(battler, statusEffect);
    }

    /// <summary>
    /// Callback when a battler is defeated.
    /// </summary>
    private void OnBattlerDefeated(Battler battler)
    {
        AddBattlerDefeatedEntry(battler);
    }

    /// <summary>
    /// Show the combat log.
    /// </summary>
    public void ShowLog()
    {
        Visible = true;
    }

    /// <summary>
    /// Hide the combat log.
    /// </summary>
    public void HideLog()
    {
        Visible = false;
    }

    /// <summary>
    /// Fade out the combat log.
    /// </summary>
    public void FadeOut()
    {
        // Create a tween to fade out the log
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0.0f, 0.5f);
        tween.TweenCallback(new Callable(this, MethodName.QueueFree)).SetDelay(0.5f);
    }

    /// <summary>
    /// Fade in the combat log.
    /// </summary>
    public void FadeIn()
    {
        // Make sure the log is visible
        Visible = true;
        Modulate = new Color(1, 1, 1, 0); // Start transparent

        // Create a tween to fade in the log
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 1.0f, 0.5f);
    }

    /// <summary>
    /// Get the last few log entries.
    /// </summary>
    /// <param name="count">The number of entries to get</param>
    /// <returns>A list of the last few log entries</returns>
    public List<string> GetLastEntries(int count = 5)
    {
        var entries = new List<string>();

        // Get the last few entries from the log
        for (int i = Math.Max(0, logEntries.Count - count); i < logEntries.Count; i++)
        {
            // Extract the text from the rich text label
            // This would typically involve parsing the BBCode or getting the plain text
            // For now, we'll just add a placeholder
            entries.Add($"Log entry {i}");
        }

        return entries;
    }

    /// <summary>
    /// Search the log for entries containing specific text.
    /// </summary>
    /// <param name="searchText">The text to search for</param>
    /// <returns>A list of matching log entries</returns>
    public List<string> SearchLog(string searchText)
    {
        var matchingEntries = new List<string>();

        if (string.IsNullOrEmpty(searchText))
        {
            return matchingEntries;
        }

        // Search through all log entries for the specified text
        foreach (var entry in logEntries)
        {
            // Check if the entry contains the search text (case-insensitive)
            // This would typically involve parsing the BBCode or getting the plain text
            // For now, we'll just check a placeholder
            if (entry.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                matchingEntries.Add(entry.Text);
            }
        }

        return matchingEntries;
    }

    /// <summary>
    /// Filter the log to show only entries of a specific type.
    /// </summary>
    /// <param name="entryType">The type of entries to show</param>
    public void FilterLog(LogEntryType entryType)
    {
        // Hide all log entries
        foreach (var entry in logEntries)
        {
            entry.Hide();
        }

        // Show only entries of the specified type
        switch (entryType)
        {
            case LogEntryType.All:
                // Show all entries
                foreach (var entry in logEntries)
                {
                    entry.Show();
                }
                break;

            case LogEntryType.Damage:
                // Show only damage entries (those with red color)
                foreach (var entry in logEntries)
                {
                    // This would typically involve checking the entry's color or tags
                    // For now, we'll just show all entries
                    entry.Show();
                }
                break;

            case LogEntryType.Healing:
                // Show only healing entries (those with green color)
                foreach (var entry in logEntries)
                {
                    // This would typically involve checking the entry's color or tags
                    // For now, we'll just show all entries
                    entry.Show();
                }
                break;

            case LogEntryType.StatusEffects:
                // Show only status effect entries (those with purple color)
                foreach (var entry in logEntries)
                {
                    // This would typically involve checking the entry's color or tags
                    // For now, we'll just show all entries
                    entry.Show();
                }
                break;

            case LogEntryType.Miscellaneous:
                // Show only miscellaneous entries (those with white, yellow, or blue colors)
                foreach (var entry in logEntries)
                {
                    // This would typically involve checking the entry's color or tags
                    // For now, we'll just show all entries
                    entry.Show();
                }
                break;
        }
    }

    /// <summary>
    /// Add a custom log entry with BBCode formatting.
    /// </summary>
    /// <param name="bbcodeText">The BBCode formatted text</param>
    public void AddBBCodeEntry(string bbcodeText)
    {
        if (string.IsNullOrEmpty(bbcodeText) || logContainer == null)
        {
            return;
        }

        // Create a new log entry
        var logEntry = new RichTextLabel();
        logEntry.BbcodeEnabled = true;
        logEntry.FitContent = true;
        logEntry.ScrollFollowing = true;

        // Set the BBCode text
        logEntry.ParseBbcode(bbcodeText);

        // Add the log entry to the container
        logContainer.AddChild(logEntry);

        // Store the log entry in the list
        logEntries.Add(logEntry);

        // If we've exceeded the maximum number of entries, remove the oldest one
        if (logEntries.Count > MaxLogEntries)
        {
            var oldestEntry = logEntries[0];
            logEntries.RemoveAt(0);
            oldestEntry.QueueFree();
        }

        // Scroll to the bottom to show the new entry
        CallDeferred(MethodName.ScrollToBottom);
    }

    /// <summary>
    /// Add an error log entry.
    /// </summary>
    /// <param name="errorMessage">The error message</param>
    public void AddErrorEntry(string errorMessage)
    {
        if (string.IsNullOrEmpty(errorMessage))
        {
            return;
        }

        var text = $"ERROR: {errorMessage}";
        AddLogEntry(text, Colors.DarkRed);
    }

    /// <summary>
    /// Add a warning log entry.
    /// </summary>
    /// <param name="warningMessage">The warning message</param>
    public void AddWarningEntry(string warningMessage)
    {
        if (string.IsNullOrEmpty(warningMessage))
        {
            return;
        }

        var text = $"WARNING: {warningMessage}";
        AddLogEntry(text, Colors.Orange);
    }

    /// <summary>
    /// Add an info log entry.
    /// </summary>
    /// <param name="infoMessage">The info message</param>
    public void AddInfoEntry(string infoMessage)
    {
        if (string.IsNullOrEmpty(infoMessage))
        {
            return;
        }

        var text = $"INFO: {infoMessage}";
        AddLogEntry(text, Colors.LightBlue);
    }

    /// <summary>
    /// Export the combat log to a file.
    /// </summary>
    /// <param name="filePath">The file path to export to</param>
    public void ExportLog(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        // Create a string containing all log entries
        var logText = string.Join("\n", logEntries.Select(entry => entry.Text));

        // Write the log text to the specified file
        // In Godot, you would typically use FileAccess to write to a file
        // For example:
        // using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
        // file.StoreString(logText);

        // For now, we'll just print a message
        GD.Print($"Exported combat log to {filePath}");
    }

    /// <summary>
    /// Import a combat log from a file.
    /// </summary>
    /// <param name="filePath">The file path to import from</param>
    public void ImportLog(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        // Read the log text from the specified file
        // In Godot, you would typically use FileAccess to read from a file
        // For example:
        // if (FileAccess.FileExists(filePath))
        // {
        //     using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        //     var logText = file.GetAsText();
        //
        //     // Clear the current log
        //     ClearLog();
        //
        //     // Add each line as a log entry
        //     var lines = logText.Split('\n');
        //     foreach (var line in lines)
        //     {
        //         AddLogEntry(line);
        //     }
        // }

        // For now, we'll just print a message
        GD.Print($"Imported combat log from {filePath}");
    }

    /// <summary>
    /// Add a special ability log entry.
    /// </summary>
    /// <param name="battler">The battler that used the special ability</param>
    /// <param name="abilityName">The name of the special ability</param>
    /// <param name="targets">The targets of the special ability</param>
    public void AddSpecialAbilityEntry(Battler battler, string abilityName, List<Battler> targets)
    {
        if (battler == null || string.IsNullOrEmpty(abilityName) || targets == null)
        {
            return;
        }

        var targetNames = string.Join(", ", targets.Select(t => t.Name));
        var text = $"{battler.Name} unleashes {abilityName} on {targetNames}!";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add an item usage log entry.
    /// </summary>
    /// <param name="battler">The battler that used the item</param>
    /// <param name="itemName">The name of the item used</param>
    /// <param name="targets">The targets of the item usage</param>
    public void AddItemUsageEntry(Battler battler, string itemName, List<Battler> targets)
    {
        if (battler == null || string.IsNullOrEmpty(itemName) || targets == null)
        {
            return;
        }

        var targetNames = string.Join(", ", targets.Select(t => t.Name));
        var text = $"{battler.Name} uses {itemName} on {targetNames}!";
        AddLogEntry(text, Colors.Cyan);
    }

    /// <summary>
    /// Add a flee attempt log entry.
    /// </summary>
    /// <param name="battler">The battler that attempted to flee</param>
    /// <param name="success">Whether the flee attempt was successful</param>
    public void AddFleeAttemptEntry(Battler battler, bool success)
    {
        if (battler == null)
        {
            return;
        }

        var text = success
            ? $"{battler.Name} successfully flees from battle!"
            : $"{battler.Name} tries to flee but fails!";

        var color = success ? Colors.Green : Colors.Red;
        AddLogEntry(text, color);
    }

    /// <summary>
    /// Add an experience gained log entry.
    /// </summary>
    /// <param name="battler">The battler that gained experience</param>
    /// <param name="expAmount">The amount of experience gained</param>
    public void AddExperienceEntry(Battler battler, int expAmount)
    {
        if (battler == null)
        {
            return;
        }

        var text = $"{battler.Name} gains {expAmount} experience points!";
        AddLogEntry(text, Colors.LightGreen);
    }

    /// <summary>
    /// Add a level up log entry.
    /// </summary>
    /// <param name="battler">The battler that leveled up</param>
    /// <param name="newLevel">The new level</param>
    public void AddLevelUpEntry(Battler battler, int newLevel)
    {
        if (battler == null)
        {
            return;
        }

        var text = $"{battler.Name} levels up to level {newLevel}!";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add a loot obtained log entry.
    /// </summary>
    /// <param name="itemName">The name of the item obtained</param>
    /// <param name="quantity">The quantity of the item obtained</param>
    public void AddLootEntry(string itemName, int quantity = 1)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            return;
        }

        var text = $"Obtained {quantity}x {itemName}!";
        AddLogEntry(text, Colors.Yellow);
    }

    /// <summary>
    /// Add a gold obtained log entry.
    /// </summary>
    /// <param name="amount">The amount of gold obtained</param>
    public void AddGoldEntry(int amount)
    {
        var text = $"Received {amount} gold!";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add a dialogue log entry.
    /// </summary>
    /// <param name="speaker">The speaker</param>
    /// <param name="dialogue">The dialogue text</param>
    public void AddDialogueEntry(string speaker, string dialogue)
    {
        if (string.IsNullOrEmpty(speaker) || string.IsNullOrEmpty(dialogue))
        {
            return;
        }

        var text = $"{speaker}: \"{dialogue}\"";
        AddLogEntry(text, Colors.White);
    }

    /// <summary>
    /// Add an ambient log entry.
    /// </summary>
    /// <param name="ambientText">The ambient text</param>
    public void AddAmbientEntry(string ambientText)
    {
        if (string.IsNullOrEmpty(ambientText))
        {
            return;
        }

        var text = $"*{ambientText}*";
        AddLogEntry(text, Colors.Gray);
    }

    /// <summary>
    /// Add a system log entry.
    /// </summary>
    /// <param name="systemText">The system text</param>
    public void AddSystemEntry(string systemText)
    {
        if (string.IsNullOrEmpty(systemText))
        {
            return;
        }

        var text = $"[SYSTEM] {systemText}";
        AddLogEntry(text, Colors.LightGray);
    }

    /// <summary>
    /// Add a debug log entry.
    /// </summary>
    /// <param name="debugText">The debug text</param>
    public void AddDebugEntry(string debugText)
    {
        if (string.IsNullOrEmpty(debugText))
        {
            return;
        }

        var text = $"[DEBUG] {debugText}";
        AddLogEntry(text, Colors.Magenta);
    }

    /// <summary>
    /// Add a notification log entry.
    /// </summary>
    /// <param name="notificationText">The notification text</param>
    public void AddNotificationEntry(string notificationText)
    {
        if (string.IsNullOrEmpty(notificationText))
        {
            return;
        }

        var text = $"[NOTIFICATION] {notificationText}";
        AddLogEntry(text, Colors.Cyan);
    }

    /// <summary>
    /// Add a quest log entry.
    /// </summary>
    /// <param name="questText">The quest text</param>
    public void AddQuestEntry(string questText)
    {
        if (string.IsNullOrEmpty(questText))
        {
            return;
        }

        var text = $"[QUEST] {questText}";
        AddLogEntry(text, Colors.Blue);
    }

    /// <summary>
    /// Add a journal log entry.
    /// </summary>
    /// <param name="journalText">The journal text</param>
    public void AddJournalEntry(string journalText)
    {
        if (string.IsNullOrEmpty(journalText))
        {
            return;
        }

        var text = $"[JOURNAL] {journalText}";
        AddLogEntry(text, Colors.Green);
    }

    /// <summary>
    /// Add a tutorial log entry.
    /// </summary>
    /// <param name="tutorialText">The tutorial text</param>
    public void AddTutorialEntry(string tutorialText)
    {
        if (string.IsNullOrEmpty(tutorialText))
        {
            return;
        }

        var text = $"[TUTORIAL] {tutorialText}";
        AddLogEntry(text, Colors.Yellow);
    }

    /// <summary>
    /// Add a hint log entry.
    /// </summary>
    /// <param name="hintText">The hint text</param>
    public void AddHintEntry(string hintText)
    {
        if (string.IsNullOrEmpty(hintText))
        {
            return;
        }

        var text = $"[HINT] {hintText}";
        AddLogEntry(text, Colors.LightBlue);
    }

    /// <summary>
    /// Add a tip log entry.
    /// </summary>
    /// <param name="tipText">The tip text</param>
    public void AddTipEntry(string tipText)
    {
        if (string.IsNullOrEmpty(tipText))
        {
            return;
        }

        var text = $"[TIP] {tipText}";
        AddLogEntry(text, Colors.LightGreen);
    }

    /// <summary>
    /// Add a fact log entry.
    /// </summary>
    /// <param name="factText">The fact text</param>
    public void AddFactEntry(string factText)
    {
        if (string.IsNullOrEmpty(factText))
        {
            return;
        }

        var text = $"[FACT] {factText}";
        AddLogEntry(text, Colors.Orange);
    }

    /// <summary>
    /// Add a lore log entry.
    /// </summary>
    /// <param name="loreText">The lore text</param>
    public void AddLoreEntry(string loreText)
    {
        if (string.IsNullOrEmpty(loreText))
        {
            return;
        }

        var text = $"[LORE] {loreText}";
        AddLogEntry(text, Colors.Purple);
    }

    /// <summary>
    /// Add a secret log entry.
    /// </summary>
    /// <param name="secretText">The secret text</param>
    public void AddSecretEntry(string secretText)
    {
        if (string.IsNullOrEmpty(secretText))
        {
            return;
        }

        var text = $"[SECRET] {secretText}";
        AddLogEntry(text, Colors.DarkPurple);
    }

    /// <summary>
    /// Add a mystery log entry.
    /// </summary>
    /// <param name="mysteryText">The mystery text</param>
    public void AddMysteryEntry(string mysteryText)
    {
        if (string.IsNullOrEmpty(mysteryText))
        {
            return;
        }

        var text = $"[MYSTERY] {mysteryText}";
        AddLogEntry(text, Colors.DarkBlue);
    }

    /// <summary>
    /// Add a discovery log entry.
    /// </summary>
    /// <param name="discoveryText">The discovery text</param>
    public void AddDiscoveryEntry(string discoveryText)
    {
        if (string.IsNullOrEmpty(discoveryText))
        {
            return;
        }

        var text = $"[DISCOVERY] {discoveryText}";
        AddLogEntry(text, Colors.Teal);
    }

    /// <summary>
    /// Add a revelation log entry.
    /// </summary>
    /// <param name="revelationText">The revelation text</param>
    public void AddRevelationEntry(string revelationText)
    {
        if (string.IsNullOrEmpty(revelationText))
        {
            return;
        }

        var text = $"[REVELATION] {revelationText}";
        AddLogEntry(text, Colors.Turquoise);
    }

    /// <summary>
    /// Add a breakthrough log entry.
    /// </summary>
    /// <param name="breakthroughText">The breakthrough text</param>
    public void AddBreakthroughEntry(string breakthroughText)
    {
        if (string.IsNullOrEmpty(breakthroughText))
        {
            return;
        }

        var text = $"[BREAKTHROUGH] {breakthroughText}";
        AddLogEntry(text, Colors.Lime);
    }

    /// <summary>
    /// Add a milestone log entry.
    /// </summary>
    /// <param name="milestoneText">The milestone text</param>
    public void AddMilestoneEntry(string milestoneText)
    {
        if (string.IsNullOrEmpty(milestoneText))
        {
            return;
        }

        var text = $"[MILESTONE] {milestoneText}";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add a victory log entry.
    /// </summary>
    /// <param name="victoryText">The victory text</param>
    public void AddVictoryEntry(string victoryText)
    {
        if (string.IsNullOrEmpty(victoryText))
        {
            return;
        }

        var text = $"[VICTORY] {victoryText}";
        AddLogEntry(text, Colors.Violet);
    }

    /// <summary>
    /// Add a defeat log entry.
    /// </summary>
    /// <param name="defeatText">The defeat text</param>
    public void AddDefeatEntry(string defeatText)
    {
        if (string.IsNullOrEmpty(defeatText))
        {
            return;
        }

        var text = $"[DEFEAT] {defeatText}";
        AddLogEntry(text, Colors.DarkRed);
    }

    /// <summary>
    /// Add a draw log entry.
    /// </summary>
    /// <param name="drawText">The draw text</param>
    public void AddDrawEntry(string drawText)
    {
        if (string.IsNullOrEmpty(drawText))
        {
            return;
        }

        var text = $"[DRAW] {drawText}";
        AddLogEntry(text, Colors.Gray);
    }

    /// <summary>
    /// Add a surrender log entry.
    /// </summary>
    /// <param name="surrenderText">The surrender text</param>
    public void AddSurrenderEntry(string surrenderText)
    {
        if (string.IsNullOrEmpty(surrenderText))
        {
            return;
        }

        var text = $"[SURRENDER] {surrenderText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a retreat log entry.
    /// </summary>
    /// <param name="retreatText">The retreat text</param>
    public void AddRetreatEntry(string retreatText)
    {
        if (string.IsNullOrEmpty(retreatText))
        {
            return;
        }

        var text = $"[RETREAT] {retreatText}";
        AddLogEntry(text, Colors.DarkOrange);
    }

    /// <summary>
    /// Add a negotiation log entry.
    /// </summary>
    /// <param name="negotiationText">The negotiation text</param>
    public void AddNegotiationEntry(string negotiationText)
    {
        if (string.IsNullOrEmpty(negotiationText))
        {
            return;
        }

        var text = $"[NEGOTIATION] {negotiationText}";
        AddLogEntry(text, Colors.Pink);
    }

    /// <summary>
    /// Add a diplomacy log entry.
    /// </summary>
    /// <param name="diplomacyText">The diplomacy text</param>
    public void AddDiplomacyEntry(string diplomacyText)
    {
        if (string.IsNullOrEmpty(diplomacyText))
        {
            return;
        }

        var text = $"[DIPLOMACY] {diplomacyText}";
        AddLogEntry(text, Colors.LightPink);
    }

    /// <summary>
    /// Add a trade log entry.
    /// </summary>
    /// <param name="tradeText">The trade text</param>
    public void AddTradeEntry(string tradeText)
    {
        if (string.IsNullOrEmpty(tradeText))
        {
            return;
        }

        var text = $"[TRADE] {tradeText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a crafting log entry.
    /// </summary>
    /// <param name="craftingText">The crafting text</param>
    public void AddCraftingEntry(string craftingText)
    {
        if (string.IsNullOrEmpty(craftingText))
        {
            return;
        }

        var text = $"[CRAFTING] {craftingText}";
        AddLogEntry(text, Colors.DarkGreen);
    }

    /// <summary>
    /// Add a gathering log entry.
    /// </summary>
    /// <param name="gatheringText">The gathering text</param>
    public void AddGatheringEntry(string gatheringText)
    {
        if (string.IsNullOrEmpty(gatheringText))
        {
            return;
        }

        var text = $"[GATHERING] {gatheringText}";
        AddLogEntry(text, Colors.ForestGreen);
    }

    /// <summary>
    /// Add a farming log entry.
    /// </summary>
    /// <param name="farmingText">The farming text</param>
    public void AddFarmingEntry(string farmingText)
    {
        if (string.IsNullOrEmpty(farmingText))
        {
            return;
        }

        var text = $"[FARMING] {farmingText}";
        AddLogEntry(text, Colors.Olive);
    }

    /// <summary>
    /// Add a mining log entry.
    /// </summary>
    /// <param name="miningText">The mining text</param>
    public void AddMiningEntry(string miningText)
    {
        if (string.IsNullOrEmpty(miningText))
        {
            return;
        }

        var text = $"[MINING] {miningText}";
        AddLogEntry(text, Colors.Silver);
    }

    /// <summary>
    /// Add a fishing log entry.
    /// </summary>
    /// <param name="fishingText">The fishing text</param>
    public void AddFishingEntry(string fishingText)
    {
        if (string.IsNullOrEmpty(fishingText))
        {
            return;
        }

        var text = $"[FISHING] {fishingText}";
        AddLogEntry(text, Colors.Aqua);
    }

    /// <summary>
    /// Add a hunting log entry.
    /// </summary>
    /// <param name="huntingText">The hunting text</param>
    public void AddHuntingEntry(string huntingText)
    {
        if (string.IsNullOrEmpty(huntingText))
        {
            return;
        }

        var text = $"[HUNTING] {huntingText}";
        AddLogEntry(text, Colors.Bronze);
    }

    /// <summary>
    /// Add a exploration log entry.
    /// </summary>
    /// <param name="explorationText">The exploration text</param>
    public void AddExplorationEntry(string explorationText)
    {
        if (string.IsNullOrEmpty(explorationText))
        {
            return;
        }

        var text = $"[EXPLORATION] {explorationText}";
        AddLogEntry(text, Colors.Tan);
    }

    /// <summary>
    /// Add a discovery log entry.
    /// </summary>
    /// <param name="discoveryText">The discovery text</param>
    public void AddDiscoveryEntry(string discoveryText)
    {
        if (string.IsNullOrEmpty(discoveryText))
        {
            return;
        }

        var text = $"[DISCOVERY] {discoveryText}";
        AddLogEntry(text, Colors.Teal);
    }

    /// <summary>
    /// Add a revelation log entry.
    /// </summary>
    /// <param name="revelationText">The revelation text</param>
    public void AddRevelationEntry(string revelationText)
    {
        if (string.IsNullOrEmpty(revelationText))
        {
            return;
        }

        var text = $"[REVELATION] {revelationText}";
        AddLogEntry(text, Colors.Turquoise);
    }

    /// <summary>
    /// Add a breakthrough log entry.
    /// </summary>
    /// <param name="breakthroughText">The breakthrough text</param>
    public void AddBreakthroughEntry(string breakthroughText)
    {
        if (string.IsNullOrEmpty(breakthroughText))
        {
            return;
        }

        var text = $"[BREAKTHROUGH] {breakthroughText}";
        AddLogEntry(text, Colors.Lime);
    }

    /// <summary>
    /// Add a milestone log entry.
    /// </summary>
    /// <param name="milestoneText">The milestone text</param>
    public void AddMilestoneEntry(string milestoneText)
    {
        if (string.IsNullOrEmpty(milestoneText))
        {
            return;
        }

        var text = $"[MILESTONE] {milestoneText}";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add a victory log entry.
    /// </summary>
    /// <param name="victoryText">The victory text</param>
    public void AddVictoryEntry(string victoryText)
    {
        if (string.IsNullOrEmpty(victoryText))
        {
            return;
        }

        var text = $"[VICTORY] {victoryText}";
        AddLogEntry(text, Colors.Violet);
    }

    /// <summary>
    /// Add a defeat log entry.
    /// </summary>
    /// <param name="defeatText">The defeat text</param>
    public void AddDefeatEntry(string defeatText)
    {
        if (string.IsNullOrEmpty(defeatText))
        {
            return;
        }

        var text = $"[DEFEAT] {defeatText}";
        AddLogEntry(text, Colors.DarkRed);
    }

    /// <summary>
    /// Add a draw log entry.
    /// </summary>
    /// <param name="drawText">The draw text</param>
    public void AddDrawEntry(string drawText)
    {
        if (string.IsNullOrEmpty(drawText))
        {
            return;
        }

        var text = $"[DRAW] {drawText}";
        AddLogEntry(text, Colors.Gray);
    }

    /// <summary>
    /// Add a surrender log entry.
    /// </summary>
    /// <param name="surrenderText">The surrender text</param>
    public void AddSurrenderEntry(string surrenderText)
    {
        if (string.IsNullOrEmpty(surrenderText))
        {
            return;
        }

        var text = $"[SURRENDER] {surrenderText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a retreat log entry.
    /// </summary>
    /// <param name="retreatText">The retreat text</param>
    public void AddRetreatEntry(string retreatText)
    {
        if (string.IsNullOrEmpty(retreatText))
        {
            return;
        }

        var text = $"[RETREAT] {retreatText}";
        AddLogEntry(text, Colors.DarkOrange);
    }

    /// <summary>
    /// Add a negotiation log entry.
    /// </summary>
    /// <param name="negotiationText">The negotiation text</param>
    public void AddNegotiationEntry(string negotiationText)
    {
        if (string.IsNullOrEmpty(negotiationText))
        {
            return;
        }

        var text = $"[NEGOTIATION] {negotiationText}";
        AddLogEntry(text, Colors.Pink);
    }

    /// <summary>
    /// Add a diplomacy log entry.
    /// </summary>
    /// <param name="diplomacyText">The diplomacy text</param>
    public void AddDiplomacyEntry(string diplomacyText)
    {
        if (string.IsNullOrEmpty(diplomacyText))
        {
            return;
        }

        var text = $"[DIPLOMACY] {diplomacyText}";
        AddLogEntry(text, Colors.LightPink);
    }

    /// <summary>
    /// Add a trade log entry.
    /// </summary>
    /// <param name="tradeText">The trade text</param>
    public void AddTradeEntry(string tradeText)
    {
        if (string.IsNullOrEmpty(tradeText))
        {
            return;
        }

        var text = $"[TRADE] {tradeText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a crafting log entry.
    /// </summary>
    /// <param name="craftingText">The crafting text</param>
    public void AddCraftingEntry(string craftingText)
    {
        if (string.IsNullOrEmpty(craftingText))
        {
            return;
        }

        var text = $"[CRAFTING] {craftingText}";
        AddLogEntry(text, Colors.DarkGreen);
    }

    /// <summary>
    /// Add a gathering log entry.
    /// </summary>
    /// <param name="gatheringText">The gathering text</param>
    public void AddGatheringEntry(string gatheringText)
    {
        if (string.IsNullOrEmpty(gatheringText))
        {
            return;
        }

        var text = $"[GATHERING] {gatheringText}";
        AddLogEntry(text, Colors.ForestGreen);
    }

    /// <summary>
    /// Add a farming log entry.
    /// </summary>
    /// <param name="farmingText">The farming text</param>
    public void AddFarmingEntry(string farmingText)
    {
        if (string.IsNullOrEmpty(farmingText))
        {
            return;
        }

        var text = $"[FARMING] {farmingText}";
        AddLogEntry(text, Colors.Olive);
    }

    /// <summary>
    /// Add a mining log entry.
    /// </summary>
    /// <param name="miningText">The mining text</param>
    public void AddMiningEntry(string miningText)
    {
        if (string.IsNullOrEmpty(miningText))
        {
            return;
        }

        var text = $"[MINING] {miningText}";
        AddLogEntry(text, Colors.Silver);
    }

    /// <summary>
    /// Add a fishing log entry.
    /// </summary>
    /// <param name="fishingText">The fishing text</param>
    public void AddFishingEntry(string fishingText)
    {
        if (string.IsNullOrEmpty(fishingText))
        {
            return;
        }

        var text = $"[FISHING] {fishingText}";
        AddLogEntry(text, Colors.Aqua);
    }

    /// <summary>
    /// Add a hunting log entry.
    /// </summary>
    /// <param name="huntingText">The hunting text</param>
    public void AddHuntingEntry(string huntingText)
    {
        if (string.IsNullOrEmpty(huntingText))
        {
            return;
        }

        var text = $"[HUNTING] {huntingText}";
        AddLogEntry(text, Colors.Bronze);
    }

    /// <summary>
    /// Add a exploration log entry.
    /// </summary>
    /// <param name="explorationText">The exploration text</param>
    public void AddExplorationEntry(string explorationText)
    {
        if (string.IsNullOrEmpty(explorationText))
        {
            return;
        }

        var text = $"[EXPLORATION] {explorationText}";
        AddLogEntry(text, Colors.Tan);
    }

    /// <summary>
    /// Add a discovery log entry.
    /// </summary>
    /// <param name="discoveryText">The discovery text</param>
    public void AddDiscoveryEntry(string discoveryText)
    {
        if (string.IsNullOrEmpty(discoveryText))
        {
            return;
        }

        var text = $"[DISCOVERY] {discoveryText}";
        AddLogEntry(text, Colors.Teal);
    }

    /// <summary>
    /// Add a revelation log entry.
    /// </summary>
    /// <param name="revelationText">The revelation text</param>
    public void AddRevelationEntry(string revelationText)
    {
        if (string.IsNullOrEmpty(revelationText))
        {
            return;
        }

        var text = $"[REVELATION] {revelationText}";
        AddLogEntry(text, Colors.Turquoise);
    }

    /// <summary>
    /// Add a breakthrough log entry.
    /// </summary>
    /// <param name="breakthroughText">The breakthrough text</param>
    public void AddBreakthroughEntry(string breakthroughText)
    {
        if (string.IsNullOrEmpty(breakthroughText))
        {
            return;
        }

        var text = $"[BREAKTHROUGH] {breakthroughText}";
        AddLogEntry(text, Colors.Lime);
    }

    /// <summary>
    /// Add a milestone log entry.
    /// </summary>
    /// <param name="milestoneText">The milestone text</param>
    public void AddMilestoneEntry(string milestoneText)
    {
        if (string.IsNullOrEmpty(milestoneText))
        {
            return;
        }

        var text = $"[MILESTONE] {milestoneText}";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add a victory log entry.
    /// </summary>
    /// <param name="victoryText">The victory text</param>
    public void AddVictoryEntry(string victoryText)
    {
        if (string.IsNullOrEmpty(victoryText))
        {
            return;
        }

        var text = $"[VICTORY] {victoryText}";
        AddLogEntry(text, Colors.Violet);
    }

    /// <summary>
    /// Add a defeat log entry.
    /// </summary>
    /// <param name="defeatText">The defeat text</param>
    public void AddDefeatEntry(string defeatText)
    {
        if (string.IsNullOrEmpty(defeatText))
        {
            return;
        }

        var text = $"[DEFEAT] {defeatText}";
        AddLogEntry(text, Colors.DarkRed);
    }

    /// <summary>
    /// Add a draw log entry.
    /// </summary>
    /// <param name="drawText">The draw text</param>
    public void AddDrawEntry(string drawText)
    {
        if (string.IsNullOrEmpty(drawText))
        {
            return;
        }

        var text = $"[DRAW] {drawText}";
        AddLogEntry(text, Colors.Gray);
    }

    /// <summary>
    /// Add a surrender log entry.
    /// </summary>
    /// <param name="surrenderText">The surrender text</param>
    public void AddSurrenderEntry(string surrenderText)
    {
        if (string.IsNullOrEmpty(surrenderText))
        {
            return;
        }

        var text = $"[SURRENDER] {surrenderText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a retreat log entry.
    /// </summary>
    /// <param name="retreatText">The retreat text</param>
    public void AddRetreatEntry(string retreatText)
    {
        if (string.IsNullOrEmpty(retreatText))
        {
            return;
        }

        var text = $"[RETREAT] {retreatText}";
        AddLogEntry(text, Colors.DarkOrange);
    }

    /// <summary>
    /// Add a negotiation log entry.
    /// </summary>
    /// <param name="negotiationText">The negotiation text</param>
    public void AddNegotiationEntry(string negotiationText)
    {
        if (string.IsNullOrEmpty(negotiationText))
        {
            return;
        }

        var text = $"[NEGOTIATION] {negotiationText}";
        AddLogEntry(text, Colors.Pink);
    }

    /// <summary>
    /// Add a diplomacy log entry.
    /// </summary>
    /// <param name="diplomacyText">The diplomacy text</param>
    public void AddDiplomacyEntry(string diplomacyText)
    {
        if (string.IsNullOrEmpty(diplomacyText))
        {
            return;
        }

        var text = $"[DIPLOMACY] {diplomacyText}";
        AddLogEntry(text, Colors.LightPink);
    }

    /// <summary>
    /// Add a trade log entry.
    /// </summary>
    /// <param name="tradeText">The trade text</param>
    public void AddTradeEntry(string tradeText)
    {
        if (string.IsNullOrEmpty(tradeText))
        {
            return;
        }

        var text = $"[TRADE] {tradeText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a crafting log entry.
    /// </summary>
    /// <param name="craftingText">The crafting text</param>
    public void AddCraftingEntry(string craftingText)
    {
        if (string.IsNullOrEmpty(craftingText))
        {
            return;
        }

        var text = $"[CRAFTING] {craftingText}";
        AddLogEntry(text, Colors.DarkGreen);
    }

    /// <summary>
    /// Add a gathering log entry.
    /// </summary>
    /// <param name="gatheringText">The gathering text</param>
    public void AddGatheringEntry(string gatheringText)
    {
        if (string.IsNullOrEmpty(gatheringText))
        {
            return;
        }

        var text = $"[GATHERING] {gatheringText}";
        AddLogEntry(text, Colors.ForestGreen);
    }

    /// <summary>
    /// Add a farming log entry.
    /// </summary>
    /// <param name="farmingText">The farming text</param>
    public void AddFarmingEntry(string farmingText)
    {
        if (string.IsNullOrEmpty(farmingText))
        {
            return;
        }

        var text = $"[FARMING] {farmingText}";
        AddLogEntry(text, Colors.Olive);
    }

    /// <summary>
    /// Add a mining log entry.
    /// </summary>
    /// <param name="miningText">The mining text</param>
    public void AddMiningEntry(string miningText)
    {
        if (string.IsNullOrEmpty(miningText))
        {
            return;
        }

        var text = $"[MINING] {miningText}";
        AddLogEntry(text, Colors.Silver);
    }

    /// <summary>
    /// Add a fishing log entry.
    /// </summary>
    /// <param name="fishingText">The fishing text</param>
    public void AddFishingEntry(string fishingText)
    {
        if (string.IsNullOrEmpty(fishingText))
        {
            return;
        }

        var text = $"[FISHING] {fishingText}";
        AddLogEntry(text, Colors.Aqua);
    }

    /// <summary>
    /// Add a hunting log entry.
    /// </summary>
    /// <param name="huntingText">The hunting text</param>
    public void AddHuntingEntry(string huntingText)
    {
        if (string.IsNullOrEmpty(huntingText))
        {
            return;
        }

        var text = $"[HUNTING] {huntingText}";
        AddLogEntry(text, Colors.Bronze);
    }

    /// <summary>
    /// Add a exploration log entry.
    /// </summary>
    /// <param name="explorationText">The exploration text</param>
    public void AddExplorationEntry(string explorationText)
    {
        if (string.IsNullOrEmpty(explorationText))
        {
            return;
        }

        var text = $"[EXPLORATION] {explorationText}";
        AddLogEntry(text, Colors.Tan);
    }

    /// <summary>
    /// Add a discovery log entry.
    /// </summary>
    /// <param name="discoveryText">The discovery text</param>
    public void AddDiscoveryEntry(string discoveryText)
    {
        if (string.IsNullOrEmpty(discoveryText))
        {
            return;
        }

        var text = $"[DISCOVERY] {discoveryText}";
        AddLogEntry(text, Colors.Teal);
    }

    /// <summary>
    /// Add a revelation log entry.
    /// </summary>
    /// <param name="revelationText">The revelation text</param>
    public void AddRevelationEntry(string revelationText)
    {
        if (string.IsNullOrEmpty(revelationText))
        {
            return;
        }

        var text = $"[REVELATION] {revelationText}";
        AddLogEntry(text, Colors.Turquoise);
    }

    /// <summary>
    /// Add a breakthrough log entry.
    /// </summary>
    /// <param name="breakthroughText">The breakthrough text</param>
    public void AddBreakthroughEntry(string breakthroughText)
    {
        if (string.IsNullOrEmpty(breakthroughText))
        {
            return;
        }

        var text = $"[BREAKTHROUGH] {breakthroughText}";
        AddLogEntry(text, Colors.Lime);
    }

    /// <summary>
    /// Add a milestone log entry.
    /// </summary>
    /// <param name="milestoneText">The milestone text</param>
    public void AddMilestoneEntry(string milestoneText)
    {
        if (string.IsNullOrEmpty(milestoneText))
        {
            return;
        }

        var text = $"[MILESTONE] {milestoneText}";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add a victory log entry.
    /// </summary>
    /// <param name="victoryText">The victory text</param>
    public void AddVictoryEntry(string victoryText)
    {
        if (string.IsNullOrEmpty(victoryText))
        {
            return;
        }

        var text = $"[VICTORY] {victoryText}";
        AddLogEntry(text, Colors.Violet);
    }

    /// <summary>
    /// Add a defeat log entry.
    /// </summary>
    /// <param name="defeatText">The defeat text</param>
    public void AddDefeatEntry(string defeatText)
    {
        if (string.IsNullOrEmpty(defeatText))
        {
            return;
        }

        var text = $"[DEFEAT] {defeatText}";
        AddLogEntry(text, Colors.DarkRed);
    }

    /// <summary>
    /// Add a draw log entry.
    /// </summary>
    /// <param name="drawText">The draw text</param>
    public void AddDrawEntry(string drawText)
    {
        if (string.IsNullOrEmpty(drawText))
        {
            return;
        }

        var text = $"[DRAW] {drawText}";
        AddLogEntry(text, Colors.Gray);
    }

    /// <summary>
    /// Add a surrender log entry.
    /// </summary>
    /// <param name="surrenderText">The surrender text</param>
    public void AddSurrenderEntry(string surrenderText)
    {
        if (string.IsNullOrEmpty(surrenderText))
        {
            return;
        }

        var text = $"[SURRENDER] {surrenderText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a retreat log entry.
    /// </summary>
    /// <param name="retreatText">The retreat text</param>
    public void AddRetreatEntry(string retreatText)
    {
        if (string.IsNullOrEmpty(retreatText))
        {
            return;
        }

        var text = $"[RETREAT] {retreatText}";
        AddLogEntry(text, Colors.DarkOrange);
    }

    /// <summary>
    /// Add a negotiation log entry.
    /// </summary>
    /// <param name="negotiationText">The negotiation text</param>
    public void AddNegotiationEntry(string negotiationText)
    {
        if (string.IsNullOrEmpty(negotiationText))
        {
            return;
        }

        var text = $"[NEGOTIATION] {negotiationText}";
        AddLogEntry(text, Colors.Pink);
    }

    /// <summary>
    /// Add a diplomacy log entry.
    /// </summary>
    /// <param name="diplomacyText">The diplomacy text</param>
    public void AddDiplomacyEntry(string diplomacyText)
    {
        if (string.IsNullOrEmpty(diplomacyText))
        {
            return;
        }

        var text = $"[DIPLOMACY] {diplomacyText}";
        AddLogEntry(text, Colors.LightPink);
    }

    /// <summary>
    /// Add a trade log entry.
    /// </summary>
    /// <param name="tradeText">The trade text</param>
    public void AddTradeEntry(string tradeText)
    {
        if (string.IsNullOrEmpty(tradeText))
        {
            return;
        }

        var text = $"[TRADE] {tradeText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a crafting log entry.
    /// </summary>
    /// <param name="craftingText">The crafting text</param>
    public void AddCraftingEntry(string craftingText)
    {
        if (string.IsNullOrEmpty(craftingText))
        {
            return;
        }

        var text = $"[CRAFTING] {craftingText}";
        AddLogEntry(text, Colors.DarkGreen);
    }

    /// <summary>
    /// Add a gathering log entry.
    /// </summary>
    /// <param name="gatheringText">The gathering text</param>
    public void AddGatheringEntry(string gatheringText)
    {
        if (string.IsNullOrEmpty(gatheringText))
        {
            return;
        }

        var text = $"[GATHERING] {gatheringText}";
        AddLogEntry(text, Colors.ForestGreen);
    }

    /// <summary>
    /// Add a farming log entry.
    /// </summary>
    /// <param name="farmingText">The farming text</param>
    public void AddFarmingEntry(string farmingText)
    {
        if (string.IsNullOrEmpty(farmingText))
        {
            return;
        }

        var text = $"[FARMING] {farmingText}";
        AddLogEntry(text, Colors.Olive);
    }

    /// <summary>
    /// Add a mining log entry.
    /// </summary>
    /// <param name="miningText">The mining text</param>
    public void AddMiningEntry(string miningText)
    {
        if (string.IsNullOrEmpty(miningText))
        {
            return;
        }

        var text = $"[MINING] {miningText}";
        AddLogEntry(text, Colors.Silver);
    }

    /// <summary>
    /// Add a fishing log entry.
    /// </summary>
    /// <param name="fishingText">The fishing text</param>
    public void AddFishingEntry(string fishingText)
    {
        if (string.IsNullOrEmpty(fishingText))
        {
            return;
        }

        var text = $"[FISHING] {fishingText}";
        AddLogEntry(text, Colors.Aqua);
    }

    /// <summary>
    /// Add a hunting log entry.
    /// </summary>
    /// <param name="huntingText">The hunting text</param>
    public void AddHuntingEntry(string huntingText)
    {
        if (string.IsNullOrEmpty(huntingText))
        {
            return;
        }

        var text = $"[HUNTING] {huntingText}";
        AddLogEntry(text, Colors.Bronze);
    }

    /// <summary>
    /// Add a exploration log entry.
    /// </summary>
    /// <param name="explorationText">The exploration text</param>
    public void AddExplorationEntry(string explorationText)
    {
        if (string.IsNullOrEmpty(explorationText))
        {
            return;
        }

        var text = $"[EXPLORATION] {explorationText}";
        AddLogEntry(text, Colors.Tan);
    }

    /// <summary>
    /// Add a discovery log entry.
    /// </summary>
    /// <param name="discoveryText">The discovery text</param>
    public void AddDiscoveryEntry(string discoveryText)
    {
        if (string.IsNullOrEmpty(discoveryText))
        {
            return;
        }

        var text = $"[DISCOVERY] {discoveryText}";
        AddLogEntry(text, Colors.Teal);
    }

    /// <summary>
    /// Add a revelation log entry.
    /// </summary>
    /// <param name="revelationText">The revelation text</param>
    public void AddRevelationEntry(string revelationText)
    {
        if (string.IsNullOrEmpty(revelationText))
        {
            return;
        }

        var text = $"[REVELATION] {revelationText}";
        AddLogEntry(text, Colors.Turquoise);
    }

    /// <summary>
    /// Add a breakthrough log entry.
    /// </summary>
    /// <param name="breakthroughText">The breakthrough text</param>
    public void AddBreakthroughEntry(string breakthroughText)
    {
        if (string.IsNullOrEmpty(breakthroughText))
        {
            return;
        }

        var text = $"[BREAKTHROUGH] {breakthroughText}";
        AddLogEntry(text, Colors.Lime);
    }

    /// <summary>
    /// Add a milestone log entry.
    /// </summary>
    /// <param name="milestoneText">The milestone text</param>
    public void AddMilestoneEntry(string milestoneText)
    {
        if (string.IsNullOrEmpty(milestoneText))
        {
            return;
        }

        var text = $"[MILESTONE] {milestoneText}";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add a victory log entry.
    /// </summary>
    /// <param name="victoryText">The victory text</param>
    public void AddVictoryEntry(string victoryText)
    {
        if (string.IsNullOrEmpty(victoryText))
        {
            return;
        }

        var text = $"[VICTORY] {victoryText}";
        AddLogEntry(text, Colors.Violet);
    }

    /// <summary>
    /// Add a defeat log entry.
    /// </summary>
    /// <param name="defeatText">The defeat text</param>
    public void AddDefeatEntry(string defeatText)
    {
        if (string.IsNullOrEmpty(defeatText))
        {
            return;
        }

        var text = $"[DEFEAT] {defeatText}";
        AddLogEntry(text, Colors.DarkRed);
    }

    /// <summary>
    /// Add a draw log entry.
    /// </summary>
    /// <param name="drawText">The draw text</param>
    public void AddDrawEntry(string drawText)
    {
        if (string.IsNullOrEmpty(drawText))
        {
            return;
        }

        var text = $"[DRAW] {drawText}";
        AddLogEntry(text, Colors.Gray);
    }

    /// <summary>
    /// Add a surrender log entry.
    /// </summary>
    /// <param name="surrenderText">The surrender text</param>
    public void AddSurrenderEntry(string surrenderText)
    {
        if (string.IsNullOrEmpty(surrenderText))
        {
            return;
        }

        var text = $"[SURRENDER] {surrenderText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a retreat log entry.
    /// </summary>
    /// <param name="retreatText">The retreat text</param>
    public void AddRetreatEntry(string retreatText)
    {
        if (string.IsNullOrEmpty(retreatText))
        {
            return;
        }

        var text = $"[RETREAT] {retreatText}";
        AddLogEntry(text, Colors.DarkOrange);
    }

    /// <summary>
    /// Add a negotiation log entry.
    /// </summary>
    /// <param name="negotiationText">The negotiation text</param>
    public void AddNegotiationEntry(string negotiationText)
    {
        if (string.IsNullOrEmpty(negotiationText))
        {
            return;
        }

        var text = $"[NEGOTIATION] {negotiationText}";
        AddLogEntry(text, Colors.Pink);
    }

    /// <summary>
    /// Add a diplomacy log entry.
    /// </summary>
    /// <param name="diplomacyText">The diplomacy text</param>
    public void AddDiplomacyEntry(string diplomacyText)
    {
        if (string.IsNullOrEmpty(diplomacyText))
        {
            return;
        }

        var text = $"[DIPLOMACY] {diplomacyText}";
        AddLogEntry(text, Colors.LightPink);
    }

    /// <summary>
    /// Add a trade log entry.
    /// </summary>
    /// <param name="tradeText">The trade text</param>
    public void AddTradeEntry(string tradeText)
    {
        if (string.IsNullOrEmpty(tradeText))
        {
            return;
        }

        var text = $"[TRADE] {tradeText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a crafting log entry.
    /// </summary>
    /// <param name="craftingText">The crafting text</param>
    public void AddCraftingEntry(string craftingText)
    {
        if (string.IsNullOrEmpty(craftingText))
        {
            return;
        }

        var text = $"[CRAFTING] {craftingText}";
        AddLogEntry(text, Colors.DarkGreen);
    }

    /// <summary>
    /// Add a gathering log entry.
    /// </summary>
    /// <param name="gatheringText">The gathering text</param>
    public void AddGatheringEntry(string gatheringText)
    {
        if (string.IsNullOrEmpty(gatheringText))
        {
            return;
        }

        var text = $"[GATHERING] {gatheringText}";
        AddLogEntry(text, Colors.ForestGreen);
    }

    /// <summary>
    /// Add a farming log entry.
    /// </summary>
    /// <param name="farmingText">The farming text</param>
    public void AddFarmingEntry(string farmingText)
    {
        if (string.IsNullOrEmpty(farmingText))
        {
            return;
        }

        var text = $"[FARMING] {farmingText}";
        AddLogEntry(text, Colors.Olive);
    }

    /// <summary>
    /// Add a mining log entry.
    /// </summary>
    /// <param name="miningText">The mining text</param>
    public void AddMiningEntry(string miningText)
    {
        if (string.IsNullOrEmpty(miningText))
        {
            return;
        }

        var text = $"[MINING] {miningText}";
        AddLogEntry(text, Colors.Silver);
    }

    /// <summary>
    /// Add a fishing log entry.
    /// </summary>
    /// <param name="fishingText">The fishing text</param>
    public void AddFishingEntry(string fishingText)
    {
        if (string.IsNullOrEmpty(fishingText))
        {
            return;
        }

        var text = $"[FISHING] {fishingText}";
        AddLogEntry(text, Colors.Aqua);
    }

    /// <summary>
    /// Add a hunting log entry.
    /// </summary>
    /// <param name="huntingText">The hunting text</param>
    public void AddHuntingEntry(string huntingText)
    {
        if (string.IsNullOrEmpty(huntingText))
        {
            return;
        }

        var text = $"[HUNTING] {huntingText}";
        AddLogEntry(text, Colors.Bronze);
    }

    /// <summary>
    /// Add a exploration log entry.
    /// </summary>
    /// <param name="explorationText">The exploration text</param>
    public void AddExplorationEntry(string explorationText)
    {
        if (string.IsNullOrEmpty(explorationText))
        {
            return;
        }

        var text = $"[EXPLORATION] {explorationText}";
        AddLogEntry(text, Colors.Tan);
    }

    /// <summary>
    /// Add a discovery log entry.
    /// </summary>
    /// <param name="discoveryText">The discovery text</param>
    public void AddDiscoveryEntry(string discoveryText)
    {
        if (string.IsNullOrEmpty(discoveryText))
        {
            return;
        }

        var text = $"[DISCOVERY] {discoveryText}";
        AddLogEntry(text, Colors.Teal);
    }

    /// <summary>
    /// Add a revelation log entry.
    /// </summary>
    /// <param name="revelationText">The revelation text</param>
    public void AddRevelationEntry(string revelationText)
    {
        if (string.IsNullOrEmpty(revelationText))
        {
            return;
        }

        var text = $"[REVELATION] {revelationText}";
        AddLogEntry(text, Colors.Turquoise);
    }

    /// <summary>
    /// Add a breakthrough log entry.
    /// </summary>
    /// <param name="breakthroughText">The breakthrough text</param>
    public void AddBreakthroughEntry(string breakthroughText)
    {
        if (string.IsNullOrEmpty(breakthroughText))
        {
            return;
        }

        var text = $"[BREAKTHROUGH] {breakthroughText}";
        AddLogEntry(text, Colors.Lime);
    }

    /// <summary>
    /// Add a milestone log entry.
    /// </summary>
    /// <param name="milestoneText">The milestone text</param>
    public void AddMilestoneEntry(string milestoneText)
    {
        if (string.IsNullOrEmpty(milestoneText))
        {
            return;
        }

        var text = $"[MILESTONE] {milestoneText}";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add a victory log entry.
    /// </summary>
    /// <param name="victoryText">The victory text</param>
    public void AddVictoryEntry(string victoryText)
    {
        if (string.IsNullOrEmpty(victoryText))
        {
            return;
        }

        var text = $"[VICTORY] {victoryText}";
        AddLogEntry(text, Colors.Violet);
    }

    /// <summary>
    /// Add a defeat log entry.
    /// </summary>
    /// <param name="defeatText">The defeat text</param>
    public void AddDefeatEntry(string defeatText)
    {
        if (string.IsNullOrEmpty(defeatText))
        {
            return;
        }

        var text = $"[DEFEAT] {defeatText}";
        AddLogEntry(text, Colors.DarkRed);
    }

    /// <summary>
    /// Add a draw log entry.
    /// </summary>
    /// <param name="drawText">The draw text</param>
    public void AddDrawEntry(string drawText)
    {
        if (string.IsNullOrEmpty(drawText))
        {
            return;
        }

        var text = $"[DRAW] {drawText}";
        AddLogEntry(text, Colors.Gray);
    }

    /// <summary>
    /// Add a surrender log entry.
    /// </summary>
    /// <param name="surrenderText">The surrender text</param>
    public void AddSurrenderEntry(string surrenderText)
    {
        if (string.IsNullOrEmpty(surrenderText))
        {
            return;
        }

        var text = $"[SURRENDER] {surrenderText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a retreat log entry.
    /// </summary>
    /// <param name="retreatText">The retreat text</param>
    public void AddRetreatEntry(string retreatText)
    {
        if (string.IsNullOrEmpty(retreatText))
        {
            return;
        }

        var text = $"[RETREAT] {retreatText}";
        AddLogEntry(text, Colors.DarkOrange);
    }

    /// <summary>
    /// Add a negotiation log entry.
    /// </summary>
    /// <param name="negotiationText">The negotiation text</param>
    public void AddNegotiationEntry(string negotiationText)
    {
        if (string.IsNullOrEmpty(negotiationText))
        {
            return;
        }

        var text = $"[NEGOTIATION] {negotiationText}";
        AddLogEntry(text, Colors.Pink);
    }

    /// <summary>
    /// Add a diplomacy log entry.
    /// </summary>
    /// <param name="diplomacyText">The diplomacy text</param>
    public void AddDiplomacyEntry(string diplomacyText)
    {
        if (string.IsNullOrEmpty(diplomacyText))
        {
            return;
        }

        var text = $"[DIPLOMACY] {diplomacyText}";
        AddLogEntry(text, Colors.LightPink);
    }

    /// <summary>
    /// Add a trade log entry.
    /// </summary>
    /// <param name="tradeText">The trade text</param>
    public void AddTradeEntry(string tradeText)
    {
        if (string.IsNullOrEmpty(tradeText))
        {
            return;
        }

        var text = $"[TRADE] {tradeText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a crafting log entry.
    /// </summary>
    /// <param name="craftingText">The crafting text</param>
    public void AddCraftingEntry(string craftingText)
    {
        if (string.IsNullOrEmpty(craftingText))
        {
            return;
        }

        var text = $"[CRAFTING] {craftingText}";
        AddLogEntry(text, Colors.DarkGreen);
    }

    /// <summary>
    /// Add a gathering log entry.
    /// </summary>
    /// <param name="gatheringText">The gathering text</param>
    public void AddGatheringEntry(string gatheringText)
    {
        if (string.IsNullOrEmpty(gatheringText))
        {
            return;
        }

        var text = $"[GATHERING] {gatheringText}";
        AddLogEntry(text, Colors.ForestGreen);
    }

    /// <summary>
    /// Add a farming log entry.
    /// </summary>
    /// <param name="farmingText">The farming text</param>
    public void AddFarmingEntry(string farmingText)
    {
        if (string.IsNullOrEmpty(farmingText))
        {
            return;
        }

        var text = $"[FARMING] {farmingText}";
        AddLogEntry(text, Colors.Olive);
    }

    /// <summary>
    /// Add a mining log entry.
    /// </summary>
    /// <param name="miningText">The mining text</param>
    public void AddMiningEntry(string miningText)
    {
        if (string.IsNullOrEmpty(miningText))
        {
            return;
        }

        var text = $"[MINING] {miningText}";
        AddLogEntry(text, Colors.Silver);
    }

    /// <summary>
    /// Add a fishing log entry.
    /// </summary>
    /// <param name="fishingText">The fishing text</param>
    public void AddFishingEntry(string fishingText)
    {
        if (string.IsNullOrEmpty(fishingText))
        {
            return;
        }

        var text = $"[FISHING] {fishingText}";
        AddLogEntry(text, Colors.Aqua);
    }

    /// <summary>
    /// Add a hunting log entry.
    /// </summary>
    /// <param name="huntingText">The hunting text</param>
    public void AddHuntingEntry(string huntingText)
    {
        if (string.IsNullOrEmpty(huntingText))
        {
            return;
        }

        var text = $"[HUNTING] {huntingText}";
        AddLogEntry(text, Colors.Bronze);
    }

    /// <summary>
    /// Add a exploration log entry.
    /// </summary>
    /// <param name="explorationText">The exploration text</param>
    public void AddExplorationEntry(string explorationText)
    {
        if (string.IsNullOrEmpty(explorationText))
        {
            return;
        }

        var text = $"[EXPLORATION] {explorationText}";
        AddLogEntry(text, Colors.Tan);
    }

    /// <summary>
    /// Add a discovery log entry.
    /// </summary>
    /// <param name="discoveryText">The discovery text</param>
    public void AddDiscoveryEntry(string discoveryText)
    {
        if (string.IsNullOrEmpty(discoveryText))
        {
            return;
        }

        var text = $"[DISCOVERY] {discoveryText}";
        AddLogEntry(text, Colors.Teal);
    }

    /// <summary>
    /// Add a revelation log entry.
    /// </summary>
    /// <param name="revelationText">The revelation text</param>
    public void AddRevelationEntry(string revelationText)
    {
        if (string.IsNullOrEmpty(revelationText))
        {
            return;
        }

        var text = $"[REVELATION] {revelationText}";
        AddLogEntry(text, Colors.Turquoise);
    }

    /// <summary>
    /// Add a breakthrough log entry.
    /// </summary>
    /// <param name="breakthroughText">The breakthrough text</param>
    public void AddBreakthroughEntry(string breakthroughText)
    {
        if (string.IsNullOrEmpty(breakthroughText))
        {
            return;
        }

        var text = $"[BREAKTHROUGH] {breakthroughText}";
        AddLogEntry(text, Colors.Lime);
    }

    /// <summary>
    /// Add a milestone log entry.
    /// </summary>
    /// <param name="milestoneText">The milestone text</param>
    public void AddMilestoneEntry(string milestoneText)
    {
        if (string.IsNullOrEmpty(milestoneText))
        {
            return;
        }

        var text = $"[MILESTONE] {milestoneText}";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add a victory log entry.
    /// </summary>
    /// <param name="victoryText">The victory text</param>
    public void AddVictoryEntry(string victoryText)
    {
        if (string.IsNullOrEmpty(victoryText))
        {
            return;
        }

        var text = $"[VICTORY] {victoryText}";
        AddLogEntry(text, Colors.Violet);
    }

    /// <summary>
    /// Add a defeat log entry.
    /// </summary>
    /// <param name="defeatText">The defeat text</param>
    public void AddDefeatEntry(string defeatText)
    {
        if (string.IsNullOrEmpty(defeatText))
        {
            return;
        }

        var text = $"[DEFEAT] {defeatText}";
        AddLogEntry(text, Colors.DarkRed);
    }

    /// <summary>
    /// Add a draw log entry.
    /// </summary>
    /// <param name="drawText">The draw text</param>
    public void AddDrawEntry(string drawText)
    {
        if (string.IsNullOrEmpty(drawText))
        {
            return;
        }

        var text = $"[DRAW] {drawText}";
        AddLogEntry(text, Colors.Gray);
    }

    /// <summary>
    /// Add a surrender log entry.
    /// </summary>
    /// <param name="surrenderText">The surrender text</param>
    public void AddSurrenderEntry(string surrenderText)
    {
        if (string.IsNullOrEmpty(surrenderText))
        {
            return;
        }

        var text = $"[SURRENDER] {surrenderText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a retreat log entry.
    /// </summary>
    /// <param name="retreatText">The retreat text</param>
    public void AddRetreatEntry(string retreatText)
    {
        if (string.IsNullOrEmpty(retreatText))
        {
            return;
        }

        var text = $"[RETREAT] {retreatText}";
        AddLogEntry(text, Colors.DarkOrange);
    }

    /// <summary>
    /// Add a negotiation log entry.
    /// </summary>
    /// <param name="negotiationText">The negotiation text</param>
    public void AddNegotiationEntry(string negotiationText)
    {
        if (string.IsNullOrEmpty(negotiationText))
        {
            return;
        }

        var text = $"[NEGOTIATION] {negotiationText}";
        AddLogEntry(text, Colors.Pink);
    }

    /// <summary>
    /// Add a diplomacy log entry.
    /// </summary>
    /// <param name="diplomacyText">The diplomacy text</param>
    public void AddDiplomacyEntry(string diplomacyText)
    {
        if (string.IsNullOrEmpty(diplomacyText))
        {
            return;
        }

        var text = $"[DIPLOMACY] {diplomacyText}";
        AddLogEntry(text, Colors.LightPink);
    }

    /// <summary>
    /// Add a trade log entry.
    /// </summary>
    /// <param name="tradeText">The trade text</param>
    public void AddTradeEntry(string tradeText)
    {
        if (string.IsNullOrEmpty(tradeText))
        {
            return;
        }

        var text = $"[TRADE] {tradeText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a crafting log entry.
    /// </summary>
    /// <param name="craftingText">The crafting text</param>
    public void AddCraftingEntry(string craftingText)
    {
        if (string.IsNullOrEmpty(craftingText))
        {
            return;
        }

        var text = $"[CRAFTING] {craftingText}";
        AddLogEntry(text, Colors.DarkGreen);
    }

    /// <summary>
    /// Add a gathering log entry.
    /// </summary>
    /// <param name="gatheringText">The gathering text</param>
    public void AddGatheringEntry(string gatheringText)
    {
        if (string.IsNullOrEmpty(gatheringText))
        {
            return;
        }

        var text = $"[GATHERING] {gatheringText}";
        AddLogEntry(text, Colors.ForestGreen);
    }

    /// <summary>
    /// Add a farming log entry.
    /// </summary>
    /// <param name="farmingText">The farming text</param>
    public void AddFarmingEntry(string farmingText)
    {
        if (string.IsNullOrEmpty(farmingText))
        {
            return;
        }

        var text = $"[FARMING] {farmingText}";
        AddLogEntry(text, Colors.Olive);
    }

    /// <summary>
    /// Add a mining log entry.
    /// </summary>
    /// <param name="miningText">The mining text</param>
    public void AddMiningEntry(string miningText)
    {
        if (string.IsNullOrEmpty(miningText))
        {
            return;
        }

        var text = $"[MINING] {miningText}";
        AddLogEntry(text, Colors.Silver);
    }

    /// <summary>
    /// Add a fishing log entry.
    /// </summary>
    /// <param name="fishingText">The fishing text</param>
    public void AddFishingEntry(string fishingText)
    {
        if (string.IsNullOrEmpty(fishingText))
        {
            return;
        }

        var text = $"[FISHING] {fishingText}";
        AddLogEntry(text, Colors.Aqua);
    }

    /// <summary>
    /// Add a hunting log entry.
    /// </summary>
    /// <param name="huntingText">The hunting text</param>
    public void AddHuntingEntry(string huntingText)
    {
        if (string.IsNullOrEmpty(huntingText))
        {
            return;
        }

        var text = $"[HUNTING] {huntingText}";
        AddLogEntry(text, Colors.Bronze);
    }

    /// <summary>
    /// Add a exploration log entry.
    /// </summary>
    /// <param name="explorationText">The exploration text</param>
    public void AddExplorationEntry(string explorationText)
    {
        if (string.IsNullOrEmpty(explorationText))
        {
            return;
        }

        var text = $"[EXPLORATION] {explorationText}";
        AddLogEntry(text, Colors.Tan);
    }

    /// <summary>
    /// Add a discovery log entry.
    /// </summary>
    /// <param name="discoveryText">The discovery text</param>
    public void AddDiscoveryEntry(string discoveryText)
    {
        if (string.IsNullOrEmpty(discoveryText))
        {
            return;
        }

        var text = $"[DISCOVERY] {discoveryText}";
        AddLogEntry(text, Colors.Teal);
    }

    /// <summary>
    /// Add a revelation log entry.
    /// </summary>
    /// <param name="revelationText">The revelation text</param>
    public void AddRevelationEntry(string revelationText)
    {
        if (string.IsNullOrEmpty(revelationText))
        {
            return;
        }

        var text = $"[REVELATION] {revelationText}";
        AddLogEntry(text, Colors.Turquoise);
    }

    /// <summary>
    /// Add a breakthrough log entry.
    /// </summary>
    /// <param name="breakthroughText">The breakthrough text</param>
    public void AddBreakthroughEntry(string breakthroughText)
    {
        if (string.IsNullOrEmpty(breakthroughText))
        {
            return;
        }

        var text = $"[BREAKTHROUGH] {breakthroughText}";
        AddLogEntry(text, Colors.Lime);
    }

    /// <summary>
    /// Add a milestone log entry.
    /// </summary>
    /// <param name="milestoneText">The milestone text</param>
    public void AddMilestoneEntry(string milestoneText)
    {
        if (string.IsNullOrEmpty(milestoneText))
        {
            return;
        }

        var text = $"[MILESTONE] {milestoneText}";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add a victory log entry.
    /// </summary>
    /// <param name="victoryText">The victory text</param>
    public void AddVictoryEntry(string victoryText)
    {
        if (string.IsNullOrEmpty(victoryText))
        {
            return;
        }

        var text = $"[VICTORY] {victoryText}";
        AddLogEntry(text, Colors.Violet);
    }

    /// <summary>
    /// Add a defeat log entry.
    /// </summary>
    /// <param name="defeatText">The defeat text</param>
    public void AddDefeatEntry(string defeatText)
    {
        if (string.IsNullOrEmpty(defeatText))
        {
            return;
        }

        var text = $"[DEFEAT] {defeatText}";
        AddLogEntry(text, Colors.DarkRed);
    }

    /// <summary>
    /// Add a draw log entry.
    /// </summary>
    /// <param name="drawText">The draw text</param>
    public void AddDrawEntry(string drawText)
    {
        if (string.IsNullOrEmpty(drawText))
        {
            return;
        }

        var text = $"[DRAW] {drawText}";
        AddLogEntry(text, Colors.Gray);
    }

    /// <summary>
    /// Add a surrender log entry.
    /// </summary>
    /// <param name="surrenderText">The surrender text</param>
    public void AddSurrenderEntry(string surrenderText)
    {
        if (string.IsNullOrEmpty(surrenderText))
        {
            return;
        }

        var text = $"[SURRENDER] {surrenderText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a retreat log entry.
    /// </summary>
    /// <param name="retreatText">The retreat text</param>
    public void AddRetreatEntry(string retreatText)
    {
        if (string.IsNullOrEmpty(retreatText))
        {
            return;
        }

        var text = $"[RETREAT] {retreatText}";
        AddLogEntry(text, Colors.DarkOrange);
    }

    /// <summary>
    /// Add a negotiation log entry.
    /// </summary>
    /// <param name="negotiationText">The negotiation text</param>
    public void AddNegotiationEntry(string negotiationText)
    {
        if (string.IsNullOrEmpty(negotiationText))
        {
            return;
        }

        var text = $"[NEGOTIATION] {negotiationText}";
        AddLogEntry(text, Colors.Pink);
    }

    /// <summary>
    /// Add a diplomacy log entry.
    /// </summary>
    /// <param name="diplomacyText">The diplomacy text</param>
    public void AddDiplomacyEntry(string diplomacyText)
    {
        if (string.IsNullOrEmpty(diplomacyText))
        {
            return;
        }

        var text = $"[DIPLOMACY] {diplomacyText}";
        AddLogEntry(text, Colors.LightPink);
    }

    /// <summary>
    /// Add a trade log entry.
    /// </summary>
    /// <param name="tradeText">The trade text</param>
    public void AddTradeEntry(string tradeText)
    {
        if (string.IsNullOrEmpty(tradeText))
        {
            return;
        }

        var text = $"[TRADE] {tradeText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a crafting log entry.
    /// </summary>
    /// <param name="craftingText">The crafting text</param>
    public void AddCraftingEntry(string craftingText)
    {
        if (string.IsNullOrEmpty(craftingText))
        {
            return;
        }

        var text = $"[CRAFTING] {craftingText}";
        AddLogEntry(text, Colors.DarkGreen);
    }

    /// <summary>
    /// Add a gathering log entry.
    /// </summary>
    /// <param name="gatheringText">The gathering text</param>
    public void AddGatheringEntry(string gatheringText)
    {
        if (string.IsNullOrEmpty(gatheringText))
        {
            return;
        }

        var text = $"[GATHERING] {gatheringText}";
        AddLogEntry(text, Colors.ForestGreen);
    }

    /// <summary>
    /// Add a farming log entry.
    /// </summary>
    /// <param name="farmingText">The farming text</param>
    public void AddFarmingEntry(string farmingText)
    {
        if (string.IsNullOrEmpty(farmingText))
        {
            return;
        }

        var text = $"[FARMING] {farmingText}";
        AddLogEntry(text, Colors.Olive);
    }

    /// <summary>
    /// Add a mining log entry.
    /// </summary>
    /// <param name="miningText">The mining text</param>
    public void AddMiningEntry(string miningText)
    {
        if (string.IsNullOrEmpty(miningText))
        {
            return;
        }

        var text = $"[MINING] {miningText}";
        AddLogEntry(text, Colors.Silver);
    }

    /// <summary>
    /// Add a fishing log entry.
    /// </summary>
    /// <param name="fishingText">The fishing text</param>
    public void AddFishingEntry(string fishingText)
    {
        if (string.IsNullOrEmpty(fishingText))
        {
            return;
        }

        var text = $"[FISHING] {fishingText}";
        AddLogEntry(text, Colors.Aqua);
    }

    /// <summary>
    /// Add a hunting log entry.
    /// </summary>
    /// <param name="huntingText">The hunting text</param>
    public void AddHuntingEntry(string huntingText)
    {
        if (string.IsNullOrEmpty(huntingText))
        {
            return;
        }

        var text = $"[HUNTING] {huntingText}";
        AddLogEntry(text, Colors.Bronze);
    }

    /// <summary>
    /// Add a exploration log entry.
    /// </summary>
    /// <param name="explorationText">The exploration text</param>
    public void AddExplorationEntry(string explorationText)
    {
        if (string.IsNullOrEmpty(explorationText))
        {
            return;
        }

        var text = $"[EXPLORATION] {explorationText}";
        AddLogEntry(text, Colors.Tan);
    }

    /// <summary>
    /// Add a discovery log entry.
    /// </summary>
    /// <param name="discoveryText">The discovery text</param>
    public void AddDiscoveryEntry(string discoveryText)
    {
        if (string.IsNullOrEmpty(discoveryText))
        {
            return;
        }

        var text = $"[DISCOVERY] {discoveryText}";
        AddLogEntry(text, Colors.Teal);
    }

    /// <summary>
    /// Add a revelation log entry.
    /// </summary>
    /// <param name="revelationText">The revelation text</param>
    public void AddRevelationEntry(string revelationText)
    {
        if (string.IsNullOrEmpty(revelationText))
        {
            return;
        }

        var text = $"[REVELATION] {revelationText}";
        AddLogEntry(text, Colors.Turquoise);
    }

    /// <summary>
    /// Add a breakthrough log entry.
    /// </summary>
    /// <param name="breakthroughText">The breakthrough text</param>
    public void AddBreakthroughEntry(string breakthroughText)
    {
        if (string.IsNullOrEmpty(breakthroughText))
        {
            return;
        }

        var text = $"[BREAKTHROUGH] {breakthroughText}";
        AddLogEntry(text, Colors.Lime);
    }

    /// <summary>
    /// Add a milestone log entry.
    /// </summary>
    /// <param name="milestoneText">The milestone text</param>
    public void AddMilestoneEntry(string milestoneText)
    {
        if (string.IsNullOrEmpty(milestoneText))
        {
            return;
        }

        var text = $"[MILESTONE] {milestoneText}";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add a victory log entry.
    /// </summary>
    /// <param name="victoryText">The victory text</param>
    public void AddVictoryEntry(string victoryText)
    {
        if (string.IsNullOrEmpty(victoryText))
        {
            return;
        }

        var text = $"[VICTORY] {victoryText}";
        AddLogEntry(text, Colors.Violet);
    }

    /// <summary>
    /// Add a defeat log entry.
    /// </summary>
    /// <param name="defeatText">The defeat text</param>
    public void AddDefeatEntry(string defeatText)
    {
        if (string.IsNullOrEmpty(defeatText))
        {
            return;
        }

        var text = $"[DEFEAT] {defeatText}";
        AddLogEntry(text, Colors.DarkRed);
    }

    /// <summary>
    /// Add a draw log entry.
    /// </summary>
    /// <param name="drawText">The draw text</param>
    public void AddDrawEntry(string drawText)
    {
        if (string.IsNullOrEmpty(drawText))
        {
            return;
        }

        var text = $"[DRAW] {drawText}";
        AddLogEntry(text, Colors.Gray);
    }

    /// <summary>
    /// Add a surrender log entry.
    /// </summary>
    /// <param name="surrenderText">The surrender text</param>
    public void AddSurrenderEntry(string surrenderText)
    {
        if (string.IsNullOrEmpty(surrenderText))
        {
            return;
        }

        var text = $"[SURRENDER] {surrenderText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a retreat log entry.
    /// </summary>
    /// <param name="retreatText">The retreat text</param>
    public void AddRetreatEntry(string retreatText)
    {
        if (string.IsNullOrEmpty(retreatText))
        {
            return;
        }

        var text = $"[RETREAT] {retreatText}";
        AddLogEntry(text, Colors.DarkOrange);
    }

    /// <summary>
    /// Add a negotiation log entry.
    /// </summary>
    /// <param name="negotiationText">The negotiation text</param>
    public void AddNegotiationEntry(string negotiationText)
    {
        if (string.IsNullOrEmpty(negotiationText))
        {
            return;
        }

        var text = $"[NEGOTIATION] {negotiationText}";
        AddLogEntry(text, Colors.Pink);
    }

    /// <summary>
    /// Add a diplomacy log entry.
    /// </summary>
    /// <param name="diplomacyText">The diplomacy text</param>
    public void AddDiplomacyEntry(string diplomacyText)
    {
        if (string.IsNullOrEmpty(diplomacyText))
        {
            return;
        }

        var text = $"[DIPLOMACY] {diplomacyText}";
        AddLogEntry(text, Colors.LightPink);
    }

    /// <summary>
    /// Add a trade log entry.
    /// </summary>
    /// <param name="tradeText">The trade text</param>
    public void AddTradeEntry(string tradeText)
    {
        if (string.IsNullOrEmpty(tradeText))
        {
            return;
        }

        var text = $"[TRADE] {tradeText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a crafting log entry.
    /// </summary>
    /// <param name="craftingText">The crafting text</param>
    public void AddCraftingEntry(string craftingText)
    {
        if (string.IsNullOrEmpty(craftingText))
        {
            return;
        }

        var text = $"[CRAFTING] {craftingText}";
        AddLogEntry(text, Colors.DarkGreen);
    }

    /// <summary>
    /// Add a gathering log entry.
    /// </summary>
    /// <param name="gatheringText">The gathering text</param>
    public void AddGatheringEntry(string gatheringText)
    {
        if (string.IsNullOrEmpty(gatheringText))
        {
            return;
        }

        var text = $"[GATHERING] {gatheringText}";
        AddLogEntry(text, Colors.ForestGreen);
    }

    /// <summary>
    /// Add a farming log entry.
    /// </summary>
    /// <param name="farmingText">The farming text</param>
    public void AddFarmingEntry(string farmingText)
    {
        if (string.IsNullOrEmpty(farmingText))
        {
            return;
        }

        var text = $"[FARMING] {farmingText}";
        AddLogEntry(text, Colors.Olive);
    }

    /// <summary>
    /// Add a mining log entry.
    /// </summary>
    /// <param name="miningText">The mining text</param>
    public void AddMiningEntry(string miningText)
    {
        if (string.IsNullOrEmpty(miningText))
        {
            return;
        }

        var text = $"[MINING] {miningText}";
        AddLogEntry(text, Colors.Silver);
    }

    /// <summary>
    /// Add a fishing log entry.
    /// </summary>
    /// <param name="fishingText">The fishing text</param>
    public void AddFishingEntry(string fishingText)
    {
        if (string.IsNullOrEmpty(fishingText))
        {
            return;
        }

        var text = $"[FISHING] {fishingText}";
        AddLogEntry(text, Colors.Aqua);
    }

    /// <summary>
    /// Add a hunting log entry.
    /// </summary>
    /// <param name="huntingText">The hunting text</param>
    public void AddHuntingEntry(string huntingText)
    {
        if (string.IsNullOrEmpty(huntingText))
        {
            return;
        }

        var text = $"[HUNTING] {huntingText}";
        AddLogEntry(text, Colors.Bronze);
    }

    /// <summary>
    /// Add a exploration log entry.
    /// </summary>
    /// <param name="explorationText">The exploration text</param>
    public void AddExplorationEntry(string explorationText)
    {
        if (string.IsNullOrEmpty(explorationText))
        {
            return;
        }

        var text = $"[EXPLORATION] {explorationText}";
        AddLogEntry(text, Colors.Tan);
    }

    /// <summary>
    /// Add a discovery log entry.
    /// </summary>
    /// <param name="discoveryText">The discovery text</param>
    public void AddDiscoveryEntry(string discoveryText)
    {
        if (string.IsNullOrEmpty(discoveryText))
        {
            return;
        }

        var text = $"[DISCOVERY] {discoveryText}";
        AddLogEntry(text, Colors.Teal);
    }

    /// <summary>
    /// Add a revelation log entry.
    /// </summary>
    /// <param name="revelationText">The revelation text</param>
    public void AddRevelationEntry(string revelationText)
    {
        if (string.IsNullOrEmpty(revelationText))
        {
            return;
        }

        var text = $"[REVELATION] {revelationText}";
        AddLogEntry(text, Colors.Turquoise);
    }

    /// <summary>
    /// Add a breakthrough log entry.
    /// </summary>
    /// <param name="breakthroughText">The breakthrough text</param>
    public void AddBreakthroughEntry(string breakthroughText)
    {
        if (string.IsNullOrEmpty(breakthroughText))
        {
            return;
        }

        var text = $"[BREAKTHROUGH] {breakthroughText}";
        AddLogEntry(text, Colors.Lime);
    }

    /// <summary>
    /// Add a milestone log entry.
    /// </summary>
    /// <param name="milestoneText">The milestone text</param>
    public void AddMilestoneEntry(string milestoneText)
    {
        if (string.IsNullOrEmpty(milestoneText))
        {
            return;
        }

        var text = $"[MILESTONE] {milestoneText}";
        AddLogEntry(text, Colors.Gold);
    }

    /// <summary>
    /// Add a victory log entry.
    /// </summary>
    /// <param name="victoryText">The victory text</param>
    public void AddVictoryEntry(string victoryText)
    {
        if (string.IsNullOrEmpty(victoryText))
        {
            return;
        }

        var text = $"[VICTORY] {victoryText}";
        AddLogEntry(text, Colors.Violet);
    }

    /// <summary>
    /// Add a defeat log entry.
    /// </summary>
    /// <param name="defeatText">The defeat text</param>
    public void AddDefeatEntry(string defeatText)
    {
        if (string.IsNullOrEmpty(defeatText))
        {
            return;
        }

        var text = $"[DEFEAT] {defeatText}";
        AddLogEntry(text, Colors.DarkRed);
    }

    /// <summary>
    /// Add a draw log entry.
    /// </summary>
    /// <param name="drawText">The draw text</param>
    public void AddDrawEntry(string drawText)
    {
        if (string.IsNullOrEmpty(drawText))
        {
            return;
        }

        var text = $"[DRAW] {drawText}";
        AddLogEntry(text, Colors.Gray);
    }

    /// <summary>
    /// Add a surrender log entry.
    /// </summary>
    /// <param name="surrenderText">The surrender text</param>
    public void AddSurrenderEntry(string surrenderText)
    {
        if (string.IsNullOrEmpty(surrenderText))
        {
            return;
        }

        var text = $"[SURRENDER] {surrenderText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a retreat log entry.
    /// </summary>
    /// <param name="retreatText">The retreat text</param>
    public void AddRetreatEntry(string retreatText)
    {
        if (string.IsNullOrEmpty(retreatText))
        {
            return;
        }

        var text = $"[RETREAT] {retreatText}";
        AddLogEntry(text, Colors.DarkOrange);
    }

    /// <summary>
    /// Add a negotiation log entry.
    /// </summary>
    /// <param name="negotiationText">The negotiation text</param>
    public void AddNegotiationEntry(string negotiationText)
    {
        if (string.IsNullOrEmpty(negotiationText))
        {
            return;
        }

        var text = $"[NEGOTIATION] {negotiationText}";
        AddLogEntry(text, Colors.Pink);
    }

    /// <summary>
    /// Add a diplomacy log entry.
    /// </summary>
    /// <param name="diplomacyText">The diplomacy text</param>
    public void AddDiplomacyEntry(string diplomacyText)
    {
        if (string.IsNullOrEmpty(diplomacyText))
        {
            return;
        }

        var text = $"[DIPLOMACY] {diplomacyText}";
        AddLogEntry(text, Colors.LightPink);
    }

    /// <summary>
    /// Add a trade log entry.
    /// </summary>
    /// <param name="tradeText">The trade text</param>
    public void AddTradeEntry(string tradeText)
    {
        if (string.IsNullOrEmpty(tradeText))
        {
            return;
        }

        var text = $"[TRADE] {tradeText}";
        AddLogEntry(text, Colors.Brown);
    }

    /// <summary>
    /// Add a crafting log entry.
    /// </summary>
    /// <param name="craftingText">The crafting text</param>
    public void AddCraftingEntry(string craftingText)
    {
        if (string.IsNullOrEmpty(craftingText))
        {
            return;
        }

        var text = $"[CRAFTING] {craftingText}";
        AddLogEntry(text, Colors.DarkGreen);
    }

    /// <summary>
    /// Add a gathering log entry.
    /// </summary>
    /// <param name="gatheringText">The gathering text</param>
    public void AddGatheringEntry(string gatheringText)
    {
        if (string.IsNullOrEmpty(gatheringText))
        {
            return;
        }

        var text = $"[GATHERING] {gatheringText}";
        AddLogEntry(text, Colors.ForestGreen);
    }

    /// <summary>
    /// Add a farming log entry.
    /// </summary>
    /// <param name="farmingText">The farming text</param>
    public void AddFarmingEntry(string farmingText)
    {
        if (string.IsNullOrEmpty(farmingText))
        {
            return;
        }

        var text = $"[FARMING] {farmingText}";
        AddLogEntry(text, Colors.Olive);
    }

    /// <summary>
    /// Add a mining log entry.
    /// </summary>
    /// <param name="miningText">The mining text</param>
    public void AddMiningEntry(string miningText)
    {
        if (string.IsNullOrEmpty(miningText))
        {
            return;
        }

        var text = $"[MINING] {miningText}";
        AddLogEntry(text, Colors.Silver);
    }

    /// <summary>
    /// Add a fishing log entry.
    /// </summary>
    /// <param name="fishingText">The fishing text</param>
    public void AddFishingEntry(string fishingText)
    {
        if (string.IsNullOrEmpty(fishingText))
        {
            return;
        }

        var text = $"[FISHING] {fishingText}";
        AddLogEntry(text, Colors.Aqua);
    }

    /// <summary>
    /// Add a hunting log entry.
    /// </summary>
    /// <param name="huntingText">The hunting text</param>
    public void AddHuntingEntry(string huntingText)
    {
        if (string.IsNullOrEmpty(huntingText))
        {
            return;
        }

        var text = $"[HUNTING] {huntingText}";
        AddLogEntry(text, Colors.Bronze);
    }

    /// <summary>
    /// Add a exploration log entry.
    /// </summary>
    /// <param name="explorationText">The exploration text</param>
    public void AddExplorationEntry(string explorationText)
    {
        if (string.IsNullOrEmpty(explorationText))
        {
            return;
        }

        var text = $"[EXPLORATION] {explorationText}";
        AddLogEntry(text, Colors.Tan);
    }

    /// <summary>
    /// Add a discovery log entry.
    /// </summary>
    /// <param name="discoveryText">The discovery text</param>
    public void AddDiscoveryEntry(string discoveryText)
    {
        if (string.IsNullOrEmpty(discoveryText))
        {
            return;
        }

        var text = $"[DISCOVERY] {discoveryText}";
        AddLogEntry(text, Colors.Teal);
    }

    /// <summary>
    /// Add a revelation log entry.
    /// </summary>
    /// <param name="revelationText">The revelation text</param>
    public void AddRevelationEntry(string revelationText)
    {
        if (string.IsNullOrEmpty(revelationText))
        {
            return;
        }

        var text = $"[REVELATION] {revelationText}";
        AddLogEntry(text, Colors.Turquoise);
    }

    /// <summary>
    /// Add a breakthrough log entry.
    /// </summary>
    /// <param name="breakthroughText">The breakthrough text</param>
    public void AddBreakthroughEntry(string breakthroughText)
    {
        if (string.IsNullOrEmpty(breakthroughText))
        {
            return;
        }

        var text = $"[BREAKTHROUGH] {breakthroughText}";
        AddLogEntry(text, Colors.Lime);
    }

    /// <summary>
    /// Add a milestone log entry.
    /// </summary>
    /// <param name="milestoneText">The milestone text</param>
    public void AddMilestoneEntry(string milestoneText)
    {
        if (string.IsNullOrEmpty(milestoneText))
        {
