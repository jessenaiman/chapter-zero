// <copyright file="NeverGoAloneCombatController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Combat.Battlers;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Stages.Stage4
{
    /// <summary>
    /// Manages combat encounters for the Echo Vault stage (Stage 4).
    /// Handles tutorial-style combats with auto-interrupt based on party size.
    /// </summary>
    [GlobalClass]
    public partial class NeverGoAloneCombatController : Node
{
        /// <summary>
        /// Note: BattlerRoster cannot be exported as it's not a supported Godot type.
        /// </summary>
        private BattlerRoster? partyRoster;
        private BattlerRoster? enemyRoster;

        private Stage4Main? stageController;
        private string currentBeatId = string.Empty;
        private string encounterDescription = string.Empty;
        private int initialPartySize;
        private int requiredPartySize;
        private string outcome = string.Empty;
        private int turnCount;
        private bool combatActive;

        /// <summary>
        /// Emitted when combat starts.
        /// </summary>
        /// <param name="beatId">The beat identifier.</param>
        [Signal]
        public delegate void CombatStartedEventHandler(string beatId);

        /// <summary>
        /// Emitted when combat ends.
        /// </summary>
        /// <param name="beatId">The beat identifier.</param>
        /// <param name="outcome">The combat outcome.</param>
        [Signal]
        public delegate void CombatEndedEventHandler(string beatId, string outcome);

        /// <inheritdoc/>
        public override void _Ready()
        {
            this.stageController = this.GetNode<Stage4Main>("/root/Stage4Main");
        }

        /// <summary>
        /// Initializes and starts a combat encounter.
        /// </summary>
        /// <param name="beatId">The current beat identifier.</param>
        /// <param name="combatData">The combat data from JSON.</param>
        /// <param name="selectedChars">The list of selected character IDs.</param>
        public async Task StartCombatAsync(string beatId, Godot.Collections.Dictionary<string, Variant> combatData, IReadOnlyList<string> selectedChars)
        {
            this.currentBeatId = beatId;
            this.combatActive = true;
            this.turnCount = 0;

            // Parse combat data
            if (combatData.TryGetValue("encounter", out var encounterVar))
            {
                this.encounterDescription = encounterVar.AsString();
            }

            if (combatData.TryGetValue("initial_party_size", out var initialSizeVar))
            {
                this.initialPartySize = initialSizeVar.AsInt32();
            }

            if (combatData.TryGetValue("required_party_size", out var requiredSizeVar))
            {
                this.requiredPartySize = requiredSizeVar.AsInt32();
            }

            if (combatData.TryGetValue("outcome", out var outcomeVar))
            {
                this.outcome = outcomeVar.AsString();
            }

            GD.Print($"Starting combat: {this.encounterDescription}");
            GD.Print($"Party size: {this.initialPartySize}/{this.requiredPartySize}");

            this.EmitSignal(SignalName.CombatStarted, beatId);

            // Setup party
            await SetupPartyAsync(selectedChars.Take(this.initialPartySize).ToList()).ConfigureAwait(false);

            // Setup enemies based on encounter
            await this.SetupEnemiesAsync().ConfigureAwait(false);

            // Monitor for interrupt conditions
            await this.MonitorCombatAsync().ConfigureAwait(false);
        }

        private static Task SetupPartyAsync(IReadOnlyList<string> charIds)
        {
            // For now, just log the setup - actual battler creation would happen in a real combat system
            foreach (var charId in charIds)
            {
                var characterData = CreateCharacterFromId(charId);
                var character = characterData.ToCharacter();
                GD.Print($"Setting up party member: {character.Name} ({character.Class})");
            }

            return Task.CompletedTask;
        }

        private Task SetupEnemiesAsync()
        {
            // Setup enemies based on encounter description
            string[] separator = { " and ", " " };
            var encounterParts = this.encounterDescription.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            foreach (var enemyName in encounterParts)
            {
                if (enemyName.Contains('\''))
                {
                    var cleanName = enemyName.Replace("'", "");
                    var battler = CreateEnemyBattler(cleanName);
                    if (battler != null)
                    {
                        GD.Print($"Setting up enemy: {battler.Name}");
                    }
                }
            }

            return Task.CompletedTask;
        }

        private static Battler? CreateEnemyBattler(string enemyType)
        {
            var battler = new Battler();
            battler.Name = enemyType;

            return enemyType switch
            {
                "Wolf-Claw Hybrid" => SetupWolfClawHybrid(battler),
                "Code Fragment" => SetupCodeFragment(battler),
                "Code Guardian" => SetupCodeGuardian(battler),
                _ => battler,
            };
        }

        private static Battler SetupWolfClawHybrid(Battler battler)
        {
            battler.Name = "Wolf-Claw Hybrid";
            return battler;
        }

        private static Battler SetupCodeFragment(Battler battler)
        {
            battler.Name = "Code Fragment";
            return battler;
        }

        private static Battler SetupCodeGuardian(Battler battler)
        {
            battler.Name = "Code Guardian";
            return battler;
        }

        private async Task MonitorCombatAsync()
        {
            // Check interrupt conditions
            if (this.initialPartySize < this.requiredPartySize)
            {
                // Interrupt after a few turns
                await this.ToSignal(this.GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);
                this.EndCombat("automatic_return_to_mirror");
                return;
            }

            // For final combat, let it run longer or until victory/defeat
            if (this.outcome == "variable_success_or_interrupt")
            {
                await this.ToSignal(this.GetTree().CreateTimer(8.0f), SceneTreeTimer.SignalName.Timeout);
                this.EndCombat("variable_success_or_interrupt");
            }
        }

        // Removed OnTurnEnded as CombatArena was removed

        private void EndCombat(string result)
        {
            this.combatActive = false;
            GD.Print($"Combat ended with outcome: {result}");
            this.EmitSignal(SignalName.CombatEnded, this.currentBeatId, result);
        }

        private static CharacterData CreateCharacterFromId(string charId)
        {
            // Simple mapping - in full implementation, load from data
            return charId switch
            {
                "fighter" => new CharacterData("fighter", "Fighter", "Strong and resilient. The bulwark against the storm.", "light"),
                "wizard" => new CharacterData("wizard", "Wizard", "Master of arcane. The wielder of hidden power.", "mischief"),
                "thief" => new CharacterData("thief", "Thief", "Swift and cunning. The shadow's edge.", "mischief"),
                "scribe" => new CharacterData("scribe", "Scribe", "Keeper of echoes. The bridge between stories.", "light"),
                _ => new CharacterData("fighter", "Fighter", "Default character.", "light"),
            };
        }
    }
}
