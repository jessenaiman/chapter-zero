using System;
using System.Collections.Generic;
using Godot;

namespace OmegaSpiral.Source.Narrative
{
    internal static partial class NarrativeSceneFactory
    {
        private static void AssignString(Godot.Collections.Dictionary dict, string key, Action<string> setter)
        {
            if (dict.TryGetValue(key, out Variant value) && value.VariantType == Variant.Type.String)
            {
                setter(value.AsString());
            }
        }

        private static void AssignInt(Godot.Collections.Dictionary dict, string key, Action<int> setter)
        {
            if (dict.TryGetValue(key, out Variant value) && value.VariantType == Variant.Type.Int)
            {
                setter(value.AsInt32());
            }
        }

        private static void AssignBool(Godot.Collections.Dictionary dict, string key, Action<bool> setter)
        {
            if (dict.TryGetValue(key, out Variant value) && value.VariantType == Variant.Type.Bool)
            {
                setter(value.AsBool());
            }
        }

        private static void AssignDictionary(
            Godot.Collections.Dictionary dict,
            string key,
            Action<Godot.Collections.Dictionary> setter)
        {
            if (dict.TryGetValue(key, out Variant value) && value.VariantType == Variant.Type.Dictionary)
            {
                setter(value.AsGodotDictionary());
            }
        }

        private static void AssignArray(
            Godot.Collections.Dictionary dict,
            string key,
            Action<Godot.Collections.Array> setter)
        {
            if (dict.TryGetValue(key, out Variant value) && value.VariantType == Variant.Type.Array)
            {
                setter(value.AsGodotArray());
            }
        }

        private static void AssignDictionary(
            Godot.Collections.Dictionary<string, Variant> dict,
            string key,
            Action<Godot.Collections.Dictionary<string, Variant>> setter)
        {
            if (dict.TryGetValue(key, out Variant value) && value.VariantType == Variant.Type.Dictionary)
            {
                setter(value.AsGodotDictionary<string, Variant>());
            }
        }

        private static void AssignArray(
            Godot.Collections.Dictionary<string, Variant> dict,
            string key,
            Action<Godot.Collections.Array> setter)
        {
            if (dict.TryGetValue(key, out Variant value) && value.VariantType == Variant.Type.Array)
            {
                setter(value.AsGodotArray());
            }
        }

        private static void AssignString(Godot.Collections.Dictionary<string, Variant> dict, string key, Action<string> setter)
        {
            if (dict.TryGetValue(key, out Variant value) && value.VariantType == Variant.Type.String)
            {
                setter(value.AsString());
            }
        }

        private static void AssignInt(Godot.Collections.Dictionary<string, Variant> dict, string key, Action<int> setter)
        {
            if (dict.TryGetValue(key, out Variant value) && value.VariantType == Variant.Type.Int)
            {
                setter(value.AsInt32());
            }
        }

        private static void AssignBool(Godot.Collections.Dictionary<string, Variant> dict, string key, Action<bool> setter)
        {
            if (dict.TryGetValue(key, out Variant value) && value.VariantType == Variant.Type.Bool)
            {
                setter(value.AsBool());
            }
        }
    }
}
