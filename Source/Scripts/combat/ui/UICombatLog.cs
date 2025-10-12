// <copyright file="UICombatLog.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>
/// Container for the combat log display.
/// The UICombatLog displays a record of combat events, actions, and outcomes during battles.
/// It provides players with a history of what has happened in combat, helping them understand
/// the flow of battle and make informed decisions.
/// </summary>
namespace OmegaSpiral.Source.Scripts
{
    using System.Collections.Generic;
    using Godot;

    public partial class UICombatLog : Control
    {
        /// <summary>
        /// Gets or sets the maximum number of log entries to keep.
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
        /// Gets or sets a value indicating whether whether the log is currently visible.
        /// </summary>
        public bool LogVisible
        {
            get => this.Visible;
            set => this.Visible = value;
        }

        /// <inheritdoc/>
        public override void _Ready()
        {
            // Get references to child UI elements
            this.logContainer = this.GetNode<VBoxContainer>("LogContainer");

            // Initially hide the log
            this.Visible = false;
        }

        /// <summary>
        /// Add a log entry.
        /// </summary>
        /// <param name="text">The text of the log entry.</param>
        /// <param name="color">The color of the log entry text.</param>
        public void AddLogEntry(string text, Color? color = null)
        {
            if (string.IsNullOrEmpty(text) || this.logContainer == null)
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
            this.logContainer.AddChild(logEntry);

            // Store the log entry in the list
            this.logEntries.Add(logEntry);

            // If we've exceeded the maximum number of entries, remove the oldest one
            if (this.logEntries.Count > this.MaxLogEntries)
            {
                var oldestEntry = this.logEntries[0];
                this.logContainer.RemoveChild(oldestEntry);
                this.logEntries.RemoveAt(0);
                oldestEntry.QueueFree();
            }

            // Scroll to the bottom to show the latest entry
            this.CallDeferred("scroll_to_bottom");
        }

        /// <summary>
        /// Scroll to the bottom of the log.
        /// </summary>
        private void ScrollToBottom()
        {
            // This method is called deferred to ensure the UI has updated
            if (this.logContainer != null && this.logContainer.GetChildCount() > 0)
            {
                var lastChild = this.logContainer.GetChild(this.logContainer.GetChildCount() - 1);
                if (lastChild is Control control)
                {
                    // Scroll to make the last entry visible
                    // Note: This is a simplified implementation. In a real implementation,
                    // you might want to use a ScrollContainer and set its scroll position.
                }
            }
        }

        /// <summary>
        /// Clear all log entries.
        /// </summary>
        public void ClearLog()
        {
            foreach (var entry in this.logEntries)
            {
                this.logContainer.RemoveChild(entry);
                entry.QueueFree();
            }

            this.logEntries.Clear();
        }

        /// <summary>
        /// Add a damage log entry.
        /// </summary>
        /// <param name="battler">The battler that took damage.</param>
        /// <param name="damageAmount">The amount of damage taken.</param>
        public void AddDamageEntry(Battler battler, int damageAmount)
        {
            var text = $"{battler.Name} takes {damageAmount} damage!";
            this.AddLogEntry(text, Colors.Red);
        }

        /// <summary>
        /// Add a heal log entry.
        /// </summary>
        /// <param name="battler">The battler that was healed.</param>
        /// <param name="healAmount">The amount of healing received.</param>
        public void AddHealEntry(Battler battler, int healAmount)
        {
            var text = $"{battler.Name} recovers {healAmount} HP!";
            this.AddLogEntry(text, Colors.Green);
        }

        /// <summary>
        /// Add a miss log entry.
        /// </summary>
        /// <param name="battler">The battler that missed.</param>
        public void AddMissEntry(Battler battler)
        {
            var text = $"{battler.Name}'s attack missed!";
            this.AddLogEntry(text, Colors.Yellow);
        }

        /// <summary>
        /// Add an action log entry.
        /// </summary>
        /// <param name="battler">The battler performing the action.</param>
        /// <param name="action">The action being performed.</param>
        /// <param name="targets">The targets of the action.</param>
        public void AddActionEntry(Battler battler, BattlerAction action, List<Battler> targets)
        {
            var targetNames = string.Join(", ", targets.ConvertAll(t => t.Name));
            var text = $"{battler.Name} uses {action.Name} on {targetNames}!";
            this.AddLogEntry(text, Colors.White);
        }

        /// <summary>
        /// Add a status effect log entry.
        /// </summary>
        /// <param name="battler">The battler affected by the status effect.</param>
        /// <param name="statusEffect">The status effect applied.</param>
        public void AddStatusEffectEntry(Battler battler, string statusEffect)
        {
            var text = $"{battler.Name} is affected by {statusEffect}!";
            this.AddLogEntry(text, Colors.Purple);
        }

        /// <summary>
        /// Add a battler defeated log entry.
        /// </summary>
        /// <param name="battler">The battler that was defeated.</param>
        public void AddBattlerDefeatedEntry(Battler battler)
        {
            var text = $"{battler.Name} has been defeated!";
            this.AddLogEntry(text, Colors.DarkRed);
        }

        /// <summary>
        /// Add a critical hit log entry.
        /// </summary>
        /// <param name="battler">The battler that landed the critical hit.</param>
        /// <param name="damageAmount">The damage amount of the critical hit.</param>
        public void AddCriticalHitEntry(Battler battler, int damageAmount)
        {
            var text = $"{battler.Name} lands a critical hit for {damageAmount} damage!";
            this.AddLogEntry(text, Colors.Orange);
        }

        /// <summary>
        /// Add a buff log entry.
        /// </summary>
        /// <param name="battler">The battler that received the buff.</param>
        /// <param name="buffName">The name of the buff.</param>
        public void AddBuffEntry(Battler battler, string buffName)
        {
            var text = $"{battler.Name} gains {buffName}!";
            this.AddLogEntry(text, Colors.LightBlue);
        }

        /// <summary>
        /// Add a debuff log entry.
        /// </summary>
        /// <param name="battler">The battler that received the debuff.</param>
        /// <param name="debuffName">The name of the debuff.</param>
        public void AddDebuffEntry(Battler battler, string debuffName)
        {
            var text = $"{battler.Name} suffers {debuffName}!";
            this.AddLogEntry(text, Colors.DarkBlue);
        }

        /// <summary>
        /// Add a turn start log entry.
        /// </summary>
        /// <param name="battler">The battler whose turn is starting.</param>
        public void AddTurnStartEntry(Battler battler)
        {
            var text = $"{battler.Name}'s turn begins.";
            this.AddLogEntry(text, Colors.LightGray);
        }

        /// <summary>
        /// Add a turn end log entry.
        /// </summary>
        /// <param name="battler">The battler whose turn is ending.</param>
        public void AddTurnEndEntry(Battler battler)
        {
            var text = $"{battler.Name}'s turn ends.";
            this.AddLogEntry(text, Colors.Gray);
        }

        /// <summary>
        /// Add a combat start log entry.
        /// </summary>
        public void AddCombatStartEntry()
        {
            var text = "Combat begins!";
            this.AddLogEntry(text, Colors.White);
        }

        /// <summary>
        /// Add a combat end log entry.
        /// </summary>
        /// <param name="playerWon">Whether the player won the combat.</param>
        public void AddCombatEndEntry(bool playerWon)
        {
            var text = playerWon ? "Victory! Combat ends." : "Defeat! Combat ends.";
            this.AddLogEntry(text, playerWon ? Colors.Gold : Colors.DarkRed);
        }

        /// <summary>
        /// Add a BBCode log entry.
        /// </summary>
        /// <param name="bbcodeText">The BBCode formatted text.</param>
        public void AddBBCodeEntry(string bbcodeText)
        {
            if (string.IsNullOrEmpty(bbcodeText) || this.logContainer == null)
            {
                return;
            }

            var logEntry = new RichTextLabel();
            logEntry.BbcodeEnabled = true;
            logEntry.FitContent = true;
            logEntry.ScrollFollowing = true;
            logEntry.AppendText(bbcodeText);

            this.logContainer.AddChild(logEntry);
            this.logEntries.Add(logEntry);

            if (this.logEntries.Count > this.MaxLogEntries)
            {
                var oldestEntry = this.logEntries[0];
                this.logContainer.RemoveChild(oldestEntry);
                this.logEntries.RemoveAt(0);
                oldestEntry.QueueFree();
            }

            this.CallDeferred("scroll_to_bottom");
        }

        /// <summary>
        /// Add an error log entry.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public void AddErrorEntry(string errorMessage)
        {
            var text = $"[ERROR] {errorMessage}";
            this.AddLogEntry(text, Colors.Red);
        }

        /// <summary>
        /// Add a warning log entry.
        /// </summary>
        /// <param name="warningMessage">The warning message.</param>
        public void AddWarningEntry(string warningMessage)
        {
            var text = $"[WARNING] {warningMessage}";
            this.AddLogEntry(text, Colors.Yellow);
        }

        /// <summary>
        /// Add an info log entry.
        /// </summary>
        /// <param name="infoMessage">The info message.</param>
        public void AddInfoEntry(string infoMessage)
        {
            var text = $"[INFO] {infoMessage}";
            this.AddLogEntry(text, Colors.LightBlue);
        }

        /// <summary>
        /// Add a special ability log entry.
        /// </summary>
        /// <param name="battler">The battler using the special ability.</param>
        /// <param name="abilityName">The name of the ability.</param>
        /// <param name="targets">The targets of the ability.</param>
        public void AddSpecialAbilityEntry(Battler battler, string abilityName, List<Battler> targets)
        {
            var targetNames = string.Join(", ", targets.ConvertAll(t => t.Name));
            var text = $"{battler.Name} uses {abilityName} on {targetNames}!";
            this.AddLogEntry(text, Colors.Cyan);
        }

        /// <summary>
        /// Add an item usage log entry.
        /// </summary>
        /// <param name="battler">The battler using the item.</param>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="targets">The targets of the item.</param>
        public void AddItemUsageEntry(Battler battler, string itemName, List<Battler> targets)
        {
            var targetNames = string.Join(", ", targets.ConvertAll(t => t.Name));
            var text = $"{battler.Name} uses {itemName} on {targetNames}!";
            this.AddLogEntry(text, Colors.LightGreen);
        }

        /// <summary>
        /// Add a flee attempt log entry.
        /// </summary>
        /// <param name="battler">The battler attempting to flee.</param>
        /// <param name="success">Whether the flee attempt was successful.</param>
        public void AddFleeAttemptEntry(Battler battler, bool success)
        {
            var text = success
                ? $"{battler.Name} successfully flees from battle!"
                : $"{battler.Name} failed to flee!";
            this.AddLogEntry(text, success ? Colors.LightGreen : Colors.Orange);
        }
    }
}
