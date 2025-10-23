using Godot;

namespace OmegaSpiral.Source.Narrative
{
    internal static partial class NarrativeSceneFactory
    {
        private static EchoChamberData MapToEchoChamber(Godot.Collections.Dictionary<string, Variant> data)
        {
            var chamberData = new EchoChamberData();

            AssignDictionary(data, "metadata", dict => chamberData.Metadata = MapToEchoMetadata(dict));

            AssignArray(data, "dreamweavers", array =>
            {
                foreach (var dreamVar in array)
                {
                    if (dreamVar.VariantType == Variant.Type.Dictionary)
                    {
                        chamberData.Dreamweavers.Add(MapToEchoDreamweaver(dreamVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });

            AssignArray(data, "interludes", array =>
            {
                foreach (var interludeVar in array)
                {
                    if (interludeVar.VariantType == Variant.Type.Dictionary)
                    {
                        chamberData.Interludes.Add(MapToEchoInterlude(interludeVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });

            AssignArray(data, "chambers", array =>
            {
                foreach (var chamberVar in array)
                {
                    if (chamberVar.VariantType == Variant.Type.Dictionary)
                    {
                        chamberData.Chambers.Add(MapToEchoChamberDefinition(chamberVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });

            AssignDictionary(data, "finale", dict => chamberData.Finale = MapToEchoFinale(dict));

            return chamberData;
        }

        private static EchoChamberMetadata MapToEchoMetadata(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var metadata = new EchoChamberMetadata();

            AssignString(dict, "iteration", value => metadata.Iteration = value);
            AssignInt(dict, "iterationFallback", value => metadata.IterationFallback = value);
            AssignString(dict, "status", value => metadata.Status = value);
            AssignArray(dict, "system_intro", array => ((List<string>)metadata.SystemIntro).AddRange(ExtractStringList(array)));

            return metadata;
        }

        private static EchoChamberDreamweaver MapToEchoDreamweaver(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var dreamweaver = new EchoChamberDreamweaver();

            AssignString(dict, "id", value => dreamweaver.Id = value);
            AssignString(dict, "name", value => dreamweaver.Name = value);
            AssignString(dict, "accentColor", value => dreamweaver.AccentColor = value);
            AssignString(dict, "textTheme", value => dreamweaver.TextTheme = value);

            return dreamweaver;
        }

        private static EchoChamberInterlude MapToEchoInterlude(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var interlude = new EchoChamberInterlude();

            AssignString(dict, "id", value => interlude.Id = value);
            AssignString(dict, "owner", value => interlude.Owner = value);
            AssignString(dict, "prompt", value => interlude.Prompt = value);
            AssignArray(dict, "options", array =>
            {
                foreach (var optionVar in array)
                {
                    if (optionVar.VariantType == Variant.Type.Dictionary)
                    {
                        interlude.Options.Add(MapToEchoOption(optionVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });

            return interlude;
        }

        private static EchoChamberOption MapToEchoOption(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var option = new EchoChamberOption();

            AssignString(dict, "id", value => option.Id = value);
            AssignString(dict, "text", value => option.Text = value);
            AssignString(dict, "alignment", value => option.Alignment = value);
            AssignString(dict, "prompt", value => option.Prompt = value);
            AssignArray(dict, "interactionLog", array => ((List<string>)option.InteractionLog).AddRange(ExtractStringList(array)));
            AssignDictionary(dict, "banter", banterDict => option.Banter = MapToEchoBanter(banterDict));

            return option;
        }

        private static EchoChamberBanter MapToEchoBanter(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var banter = new EchoChamberBanter();

            AssignDictionary(dict, "approve", approveDict => banter.Approve = MapToEchoLine(approveDict));
            AssignArray(dict, "dissent", array =>
            {
                foreach (var lineVar in array)
                {
                    if (lineVar.VariantType == Variant.Type.Dictionary)
                    {
                        banter.Dissent.Add(MapToEchoLine(lineVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });

            return banter;
        }

        private static EchoChamberLine MapToEchoLine(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var line = new EchoChamberLine();

            AssignString(dict, "speaker", value => line.Speaker = value);
            AssignString(dict, "line", value => line.Line = value);

            return line;
        }

        private static EchoChamberChamber MapToEchoChamberDefinition(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var chamber = new EchoChamberChamber();

            AssignString(dict, "id", value => chamber.Id = value);
            AssignString(dict, "owner", value => chamber.Owner = value);
            AssignDictionary(dict, "style", styleDict => chamber.Style = MapToEchoStyle(styleDict));
            AssignArray(dict, "objects", array =>
            {
                foreach (var objectVar in array)
                {
                    if (objectVar.VariantType == Variant.Type.Dictionary)
                    {
                        chamber.Objects.Add(MapToEchoObject(objectVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });
            AssignArray(dict, "decoys", array =>
            {
                foreach (var decoyVar in array)
                {
                    if (decoyVar.VariantType == Variant.Type.Dictionary)
                    {
                        chamber.Decoys.Add(MapToEchoDecoy(decoyVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });

            return chamber;
        }

        private static EchoChamberStyle MapToEchoStyle(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var style = new EchoChamberStyle();

            AssignString(dict, "template", value => style.Template = value);
            AssignString(dict, "ambient", value => style.Ambient = value);
            AssignInt(dict, "decoyCount", value => style.DecoyCount = value);
            AssignString(dict, "glitchProfile", value => style.GlitchProfile = value);

            return style;
        }

        private static EchoChamberObject MapToEchoObject(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var chamberObject = new EchoChamberObject();

            AssignString(dict, "slot", value => chamberObject.Slot = value);
            AssignString(dict, "alignment", value => chamberObject.Alignment = value);
            AssignString(dict, "prompt", value => chamberObject.Prompt = value);
            AssignArray(dict, "interactionLog", array => ((List<string>)chamberObject.InteractionLog).AddRange(ExtractStringList(array)));
            AssignDictionary(dict, "banter", banterDict => chamberObject.Banter = MapToEchoBanter(banterDict));

            return chamberObject;
        }

        private static EchoChamberDecoy MapToEchoDecoy(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var decoy = new EchoChamberDecoy();

            AssignString(dict, "id", value => decoy.Id = value);
            AssignString(dict, "revealText", value => decoy.RevealText = value);

            return decoy;
        }

        private static EchoChamberFinale MapToEchoFinale(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var finale = new EchoChamberFinale();

            AssignString(dict, "systemOutro", value => finale.SystemOutro = value);
            AssignDictionary(dict, "claimants", claimantsDict =>
            {
                foreach (var kv in claimantsDict)
                {
                    if (kv.Value.VariantType == Variant.Type.Dictionary)
                    {
                        finale.Claimants[kv.Key] = MapToEchoClaimant(kv.Value.AsGodotDictionary<string, Variant>());
                    }
                }
            });

            return finale;
        }

        private static EchoChamberFinaleClaimant MapToEchoClaimant(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var claimant = new EchoChamberFinaleClaimant();

            AssignString(dict, "claim", value => claimant.Claim = value);
            AssignArray(dict, "responses", array =>
            {
                foreach (var responseVar in array)
                {
                    if (responseVar.VariantType == Variant.Type.Dictionary)
                    {
                        claimant.Responses.Add(MapToEchoLine(responseVar.AsGodotDictionary<string, Variant>()));
                    }
                }
            });

            return claimant;
        }
    }
}
