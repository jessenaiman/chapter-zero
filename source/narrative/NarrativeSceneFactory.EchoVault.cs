using Godot;

namespace OmegaSpiral.Source.Narrative
{
    internal static partial class NarrativeSceneFactory
    {
        private static EchoVaultData MapToEchoVault(Godot.Collections.Dictionary<string, Variant> data)
        {
            var vault = new EchoVaultData();

            AssignDictionary(data, "metadata", dict => vault.Metadata = MapToEchoVaultMetadata(dict));
            AssignArray(data, "points_ledger", array =>
            {
                foreach (var entryVar in array)
                {
                    if (entryVar.VariantType == Variant.Type.Dictionary)
                    {
                        vault.PointsLedger.Add(MapToPointsLedgerEntry(entryVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });
            AssignArray(data, "beats", array =>
            {
                foreach (var beatVar in array)
                {
                    if (beatVar.VariantType == Variant.Type.Dictionary)
                    {
                        vault.Beats.Add(MapToEchoVaultBeat(beatVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });
            AssignArray(data, "echo_definitions", array =>
            {
                foreach (var echoVar in array)
                {
                    if (echoVar.VariantType == Variant.Type.Dictionary)
                    {
                        vault.EchoDefinitions.Add(MapToEchoDefinition(echoVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });
            AssignArray(data, "special_options", array =>
            {
                foreach (var optionVar in array)
                {
                    if (optionVar.VariantType == Variant.Type.Dictionary)
                    {
                        vault.SpecialOptions.Add(MapToSpecialOption(optionVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });
            AssignArray(data, "combats", array =>
            {
                foreach (var combatVar in array)
                {
                    if (combatVar.VariantType == Variant.Type.Dictionary)
                    {
                        vault.Combats.Add(MapToEchoVaultCombat(combatVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });
            AssignArray(data, "omega_logs", array => vault.OmegaLogs.AddRange(ExtractStringList(array)));
            AssignDictionary(data, "party_persistence", dict => vault.PartyPersistence = MapToPartyPersistence(dict));

            return vault;
        }

        private static EchoVaultMetadata MapToEchoVaultMetadata(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var metadata = new EchoVaultMetadata();

            AssignString(dict, "palette", value => metadata.Palette = value);
            AssignArray(dict, "system_intro", array => metadata.SystemIntro.AddRange(ExtractStringList(array)));
            AssignArray(dict, "presentation_tiers", array =>
            {
                foreach (var tierVar in array)
                {
                    if (tierVar.VariantType == Variant.Type.Dictionary)
                    {
                        metadata.PresentationTiers.Add(MapToPresentationTier(tierVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });

            return metadata;
        }

        private static EchoVaultPointsLedgerEntry MapToPointsLedgerEntry(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var entry = new EchoVaultPointsLedgerEntry();

            AssignString(dict, "beat_id", value => entry.BeatId = value);
            AssignString(dict, "owner", value => entry.Owner = value);

            return entry;
        }

        private static EchoVaultBeat MapToEchoVaultBeat(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var beat = new EchoVaultBeat();

            AssignString(dict, "id", value => beat.Id = value);
            AssignString(dict, "type", value => beat.Type = value);
            AssignInt(dict, "tier", value => beat.Tier = value);
            AssignString(dict, "owner", value => beat.Owner = value);
            AssignString(dict, "prompt", value => beat.Prompt = value);
            AssignDictionary(dict, "dreamweaver_intro", introDict =>
            {
                beat.DreamweaverIntro.Clear();
                foreach (var kv in introDict)
                {
                    if (kv.Value.VariantType == Variant.Type.String)
                    {
                        beat.DreamweaverIntro[kv.Key] = kv.Value.AsString();
                    }
                }
            });
            AssignDictionary(dict, "champion_whispers", whisperDict =>
            {
                beat.ChampionWhispers.Clear();
                foreach (var kv in whisperDict)
                {
                    if (kv.Value.VariantType == Variant.Type.String)
                    {
                        beat.ChampionWhispers[kv.Key] = kv.Value.AsString();
                    }
                }
            });
            AssignArray(dict, "options", array => beat.Options.AddRange(ExtractStringList(array)));
            AssignString(dict, "encounter_id", value => beat.EncounterId = value);
            AssignString(dict, "encounter_intro", value => beat.EncounterIntro = value);
            AssignString(dict, "tutorial_hint", value => beat.TutorialHint = value);
            AssignArray(dict, "omega_interrupts", array => beat.OmegaInterrupts.AddRange(ExtractStringList(array)));
            AssignDictionary(dict, "summary", summaryDict =>
            {
                beat.Summary.Clear();
                foreach (var kv in summaryDict)
                {
                    if (kv.Value.VariantType == Variant.Type.Array)
                    {
                        beat.Summary[kv.Key] = ExtractStringList(kv.Value.AsGodotArray());
                    }
                }
            });

            return beat;
        }

        private static EchoVaultEchoDefinition MapToEchoDefinition(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var definition = new EchoVaultEchoDefinition();

            AssignString(dict, "id", value => definition.Id = value);
            AssignString(dict, "name", value => definition.Name = value);
            AssignString(dict, "owner", value => definition.Owner = value);
            AssignString(dict, "archetype", value => definition.Archetype = value);
            AssignString(dict, "description", value => definition.Description = value);
            AssignDictionary(dict, "mechanics", mechDict => definition.Mechanics = MapToMechanics(mechDict));
            AssignString(dict, "memory_cost", value => definition.MemoryCost = value);
            AssignDictionary(dict, "dreamweaver_responses", responsesDict =>
            {
                definition.DreamweaverResponses.Clear();
                foreach (var kv in responsesDict)
                {
                    if (kv.Value.VariantType == Variant.Type.Dictionary)
                    {
                        definition.DreamweaverResponses[kv.Key] = MapToResponsePair(kv.Value.AsGodotDictionary<string, Variant>());
                    }
                }
            });

            return definition;
        }

        private static EchoVaultMechanics MapToMechanics(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var mechanics = new EchoVaultMechanics();

            AssignInt(dict, "hp", value => mechanics.Hp = value);
            AssignInt(dict, "attack", value => mechanics.Attack = value);
            AssignString(dict, "signature", value => mechanics.Signature = value);

            return mechanics;
        }

        private static EchoVaultResponsePair MapToResponsePair(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var pair = new EchoVaultResponsePair();

            AssignString(dict, "supportive", value => pair.Supportive = value);
            AssignString(dict, "caution", value => pair.Caution = value);

            return pair;
        }

        private static EchoVaultSpecialOption MapToSpecialOption(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var option = new EchoVaultSpecialOption();

            AssignString(dict, "id", value => option.Id = value);
            AssignString(dict, "stage", value => option.Stage = value);
            AssignString(dict, "description", value => option.Description = value);
            AssignString(dict, "effect", value => option.Effect = value);
            AssignString(dict, "owner", value => option.Owner = value);
            AssignDictionary(dict, "dreamweaver_reactions", reactDict =>
            {
                option.DreamweaverReactions.Clear();
                foreach (var kv in reactDict)
                {
                    if (kv.Value.VariantType == Variant.Type.String)
                    {
                        option.DreamweaverReactions[kv.Key] = kv.Value.AsString();
                    }
                }
            });

            return option;
        }

        private static EchoVaultCombat MapToEchoVaultCombat(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var combat = new EchoVaultCombat();

            AssignString(dict, "encounter_id", value => combat.EncounterId = value);
            AssignArray(dict, "enemy_list", array =>
            {
                foreach (var enemyVar in array)
                {
                    if (enemyVar.VariantType == Variant.Type.Dictionary)
                    {
                        combat.EnemyList.Add(MapToEnemyEntry(enemyVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });
            AssignArray(dict, "log_style", array => combat.LogStyle.AddRange(ExtractStringList(array)));

            return combat;
        }

        private static EchoVaultEnemyEntry MapToEnemyEntry(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var entry = new EchoVaultEnemyEntry();

            AssignString(dict, "enemy_id", value => entry.EnemyId = value);
            AssignInt(dict, "count", value => entry.Count = value);
            AssignInt(dict, "level", value => entry.Level = value);
            AssignString(dict, "conditional", value => entry.Conditional = value);

            return entry;
        }

        private static EchoVaultPartyPersistence MapToPartyPersistence(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var persistence = new EchoVaultPartyPersistence();

            AssignString(dict, "trigger_beat", value => persistence.TriggerBeat = value);
            AssignArray(dict, "fields_saved", array => persistence.FieldsSaved.AddRange(ExtractStringList(array)));
            AssignString(dict, "save_method", value => persistence.SaveMethod = value);
            AssignString(dict, "notes", value => persistence.Notes = value);

            return persistence;
        }
    }
}
