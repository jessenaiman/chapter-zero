// <copyright file="NeverGoAloneController.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Common;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Narrative
{
    /// <summary>
    /// Orchestrates the Never Go Alone stage flow (Act 3), managing character selection
    /// and combat sequences.
    /// </summary>
    [GlobalClass]
    public partial class NeverGoAloneController : Node
    {
        public const string StageConfigPath = "res://source/Data/stages/act3/never_go_alone.json";

        private Godot.Collections.Dictionary<string, Variant> stageData = new();
        private List<string> selectedCharacters = new();
        private bool stageComplete;

        private MirrorSelectionController? mirrorSelectionNode;

        private NeverGoAloneCombatController? combatController;

        /// <summary>
        /// Emitted when the stage begins.
        /// </summary>
        [Signal]
        public delegate void StageStartedEventHandler();

        /// <summary>
        /// Emitted when a beat (mirror or combat) completes.
        /// </summary>
        /// <param name="beatId">The identifier of the completed beat.</param>
        /// <param name="beatType">The type of beat ("mirror" or "combat").</param>
        [Signal]
        public delegate void BeatCompletedEventHandler(string beatId, string beatType);

        /// <summary>
        /// Emitted when the entire stage completes.
        /// </summary>
        [Signal]
        public delegate void StageCompletedEventHandler();

        /// <summary>
        /// Emitted when a character is selected from the mirror.
        /// </summary>
        /// <param name="characterId">The identifier of the selected character.</param>
        [Signal]
        public delegate void CharacterSelectedEventHandler(string characterId);

        /// <summary>
        /// Emitted when a combat encounter starts.
        /// </summary>
        /// <param name="beatId">The identifier of the combat beat.</param>
        [Signal]
        public delegate void CombatStartedEventHandler(string beatId);

        /// <summary>
        /// Emitted when a combat encounter completes.
        /// </summary>
        /// <param name="beatId">The identifier of the combat beat.</param>
        /// <param name="outcome">The combat outcome string.</param>
        [Signal]
        public delegate void CombatCompletedEventHandler(string beatId, string outcome);

        /// <inheritdoc/>
        public override void _Ready()
        {
            this.LoadStageData();

            this.mirrorSelectionNode = this.GetNode<MirrorSelectionController>("MirrorSelection");
            this.combatController = this.GetNode<NeverGoAloneCombatController>("CombatArea");
        }

        /// <summary>
        /// Loads the stage configuration from JSON.
        /// </summary>
        public void LoadStageData()
        {
            var file = Godot.FileAccess.Open(StageConfigPath, Godot.FileAccess.ModeFlags.Read);
            if (file != null)
            {
                var jsonString = file.GetAsText();
                file.Close();

                var jsonResult = Json.ParseString(jsonString);
                if (jsonResult.VariantType == Variant.Type.Dictionary)
                {
                    this.stageData = jsonResult.AsGodotDictionary<string, Variant>();
                    GD.Print($"Loaded stage data for: {this.stageData["stage_name"]}");
                }
                else
                {
                    GD.PrintErr("Failed to parse stage data JSON");
                }
            }
            else
            {
                GD.PrintErr($"Failed to load stage data from: {StageConfigPath}");
            }
        }

        /// <summary>
        /// Starts the stage sequence asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartStageAsync()
        {
            this.EmitSignal(SignalName.StageStarted);

            await this.PlayIntroAsync();
            await this.ExecuteBeatSequenceAsync();
            await this.PlayOutroAsync();

            this.EmitSignal(SignalName.StageCompleted);
            this.stageComplete = true;
        }

        /// <summary>
        /// Plays the intro narrative asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task PlayIntroAsync()
        {
            if (this.stageData.TryGetValue("narrative_context", out var narrativeContextVar))
            {
                var narrativeContext = narrativeContextVar.AsGodotDictionary<string, Variant>();
                if (narrativeContext.TryGetValue("intro", out var introVar))
                {
                    var intro = introVar.AsGodotDictionary<string, Variant>();
                    if (intro.TryGetValue("text", out var textVar))
                    {
                        var introText = textVar.AsString();
                        GD.Print($"Intro: {introText}");
                    }
                }
            }

            await this.ToSignal(this.GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
        }

        /// <summary>
        /// Plays the outro narrative asynchronously based on stage outcome.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task PlayOutroAsync()
        {
            if (this.stageData.TryGetValue("narrative_context", out var narrativeContextVar))
            {
                var narrativeContext = narrativeContextVar.AsGodotDictionary<string, Variant>();
                if (narrativeContext.TryGetValue("outro", out var outroVar))
                {
                    var outro = outroVar.AsGodotDictionary<string, Variant>();
                    if (outro.TryGetValue("text", out var textVar))
                    {
                        var textDict = textVar.AsGodotDictionary<string, Variant>();
                        var outcome = this.stageComplete ? "success" : "failure";
                        if (textDict.TryGetValue(outcome, out var outcomeTextVar))
                        {
                            var outroText = outcomeTextVar.AsString();
                            GD.Print($"Outro: {outroText}");
                        }
                    }
                }
            }

            await this.ToSignal(this.GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
        }

        /// <summary>
        /// Executes the sequence of stage beats (mirrors and combat) asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task ExecuteBeatSequenceAsync()
        {
            if (this.stageData.TryGetValue("stage_beats", out var beatsVar))
            {
                var beats = beatsVar.AsGodotArray<Godot.Collections.Dictionary<string, Variant>>();

                for (int i = 0; i < beats.Count; i++)
                {
                    var beat = beats[i];
                    if (beat.TryGetValue("type", out var typeVar) && beat.TryGetValue("id", out var idVar))
                    {
                        var beatType = typeVar.AsString();
                        var beatId = idVar.AsString();

                        GD.Print($"Executing beat: {beatId} ({beatType})");

                        switch (beatType)
                        {
                            case "mirror":
                                await this.ExecuteMirrorBeatAsync(beat);
                                break;
                            case "combat":
                                await this.ExecuteCombatBeatAsync(beat);
                                break;
                        }

                        this.EmitSignal(SignalName.BeatCompleted, beatId, beatType);

                        if (beatId == "beat_5")
                        {
                            this.PersistPartyToGameState();
                        }

                        if (this.stageComplete)
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Executes a mirror beat (character selection) asynchronously.
        /// </summary>
        /// <param name="beat">The beat data dictionary.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task ExecuteMirrorBeatAsync(Godot.Collections.Dictionary<string, Variant> beat)
        {
            var beatId = string.Empty;
            if (beat.TryGetValue("id", out var idVar))
            {
                beatId = idVar.AsString();
            }

            Godot.Collections.Dictionary<string, Variant>? narrative = null;

            // Display dw_intro
            if (beat.TryGetValue("narrative", out var narrativeVar))
            {
                narrative = narrativeVar.AsGodotDictionary<string, Variant>();
                if (narrative.TryGetValue("dw_intro", out var dwIntroVar))
                {
                    var dwIntro = dwIntroVar.AsGodotDictionary<string, Variant>();
                    GD.Print("DW Intro - Light: " + dwIntro.GetValueOrDefault("light", "").AsString());
                    GD.Print("DW Intro - Mischief: " + dwIntro.GetValueOrDefault("mischief", "").AsString());
                    GD.Print("DW Intro - Wrath: " + dwIntro.GetValueOrDefault("wrath", "").AsString());
                    await this.ToSignal(this.GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
                }
            }

            if (beat.TryGetValue("mirror_data", out var mirrorDataVar))
            {
                var mirrorData = mirrorDataVar.AsGodotDictionary<string, Variant>();
                var prompt = mirrorData["prompt"].AsString();
                var availableChars = mirrorData["available_characters"].AsGodotArray<Godot.Collections.Dictionary<string, Variant>>();

                var filteredChars = NeverGoAloneController.ApplyStage2Influence(availableChars);

                if (this.mirrorSelectionNode != null)
                {
                    this.mirrorSelectionNode.InitializeSelection(beatId, filteredChars, prompt, new Godot.Collections.Dictionary<string, Variant>());
                    var selectedVar = await this.ToSignal(this.mirrorSelectionNode, MirrorSelectionController.SignalName.CharacterSelected);
                    var selected = selectedVar[0].AsString();
                    this.selectedCharacters.Add(selected);

                    // Display dw_comment_after
                    if (narrative != null && narrative.TryGetValue("dw_comment_after", out var afterVar))
                    {
                        var after = afterVar.AsGodotDictionary<string, Variant>();
                        GD.Print("DW After - Light: " + after.GetValueOrDefault("light", "").AsString());
                        GD.Print("DW After - Mischief: " + after.GetValueOrDefault("mischief", "").AsString());
                        GD.Print("DW After - Wrath: " + after.GetValueOrDefault("wrath", "").AsString());
                        await this.ToSignal(this.GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
                    }
                }
            }
        }

        /// <summary>
        /// Applies Stage 2 Dreamweaver influence to filter available characters.
        /// </summary>
        /// <param name="availableCharacters">The array of available character data.</param>
        /// <returns>The filtered array of character data.</returns>
        private static Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> ApplyStage2Influence(Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> availableCharacters)
        {
            // Get dominant Dreamweaver from GameState
            var dominantDw = string.Empty; // Placeholder - implement GameState.DominantDreamweaver
            if (string.IsNullOrEmpty(dominantDw))
            {
                return availableCharacters; // No influence if no dominant
            }

            var filtered = new Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>>();
            foreach (var charData in availableCharacters)
            {
                if (charData.TryGetValue("dw_reflection", out var reflectionVar))
                {
                    var reflection = reflectionVar.AsString();
                    if (reflection == dominantDw || reflection == "neutral")
                    {
                        filtered.Add(charData);
                    }
                }
            }

            GD.Print($"Filtered characters for {dominantDw}: {filtered.Count}/{availableCharacters.Count}");
            return filtered.Count > 0 ? filtered : availableCharacters; // Fallback to all if none match
        }

        /// <summary>
        /// Persists the selected party to GameState.
        /// </summary>
        private void PersistPartyToGameState()
        {
            foreach (var charId in this.selectedCharacters)
            {
                var charData = new CharacterData(charId, "Placeholder", "Placeholder description", "neutral");
                var character = charData.ToCharacter();
                // Placeholder - implement GameState.PlayerParty.AddMember(character)
                GD.Print($"Persisted character to GameState: {character.Name}");
            }
        }

        /// <summary>
        /// Executes a combat beat asynchronously.
        /// </summary>
        /// <param name="beat">The beat data dictionary.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task ExecuteCombatBeatAsync(Godot.Collections.Dictionary<string, Variant> beat)
        {
            var beatId = string.Empty;
            if (beat.TryGetValue("id", out var idVar))
            {
                beatId = idVar.AsString();
            }

            if (beat.TryGetValue("combat_data", out var combatDataVar))
            {
                var combatData = combatDataVar.AsGodotDictionary<string, Variant>();

                var encounter = string.Empty;
                var initialPartySize = 0;
                var requiredPartySize = 0;
                var outcome = string.Empty;

                if (combatData.TryGetValue("encounter", out var encounterVar))
                {
                    encounter = encounterVar.AsString();
                }

                if (combatData.TryGetValue("initial_party_size", out var initialSizeVar))
                {
                    initialPartySize = initialSizeVar.AsInt32();
                }

                if (combatData.TryGetValue("required_party_size", out var requiredSizeVar))
                {
                    requiredPartySize = requiredSizeVar.AsInt32();
                }

                if (combatData.TryGetValue("outcome", out var outcomeVar))
                {
                    outcome = outcomeVar.AsString();
                }

                this.EmitSignal(SignalName.CombatStarted, beatId);

                GD.Print($"Combat encounter: {encounter}");
                GD.Print($"Party size: {initialPartySize}, required: {requiredPartySize}");

                // TODO: Replace with actual combat system integration
                await this.ToSignal(this.GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);

                this.EmitSignal(SignalName.CombatCompleted, beatId, outcome);

                if (outcome == "automatic_return_to_mirror")
                {
                    GD.Print("Combat ended, returning to mirror selection");
                }
                else if (outcome == "variable_success_or_interrupt")
                {
                    this.stageComplete = true;
                }
            }

            await this.ToSignal(this.GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
        }

        /// <summary>
        /// Gets a copy of the selected character IDs.
        /// </summary>
        /// <returns>A list of selected character IDs.</returns>
        public List<string> GetSelectedCharacters()
        {
            return new List<string>(this.selectedCharacters);
        }

        /// <summary>
        /// Gets whether the stage is complete.
        /// </summary>
        /// <returns><see langword="true"/> if the stage is complete; otherwise, <see langword="false"/>.</returns>
        public bool IsStageComplete()
        {
            return this.stageComplete;
        }

        /// <summary>
        /// Gets a copy of the loaded stage data.
        /// </summary>
        /// <returns>A dictionary containing the stage configuration data.</returns>
        public Godot.Collections.Dictionary<string, Variant> GetStageData()
        {
            return new Godot.Collections.Dictionary<string, Variant>(this.stageData);
        }

        // Duplicate methods removed - keeping the first implementation

        // Duplicate method removed - keeping the first implementation
    }
}
