using System;
using System.Collections.Generic;
using System.Text;
using ADOFAI;
using EventLib.CustomEvent;
using EventLib.Mapping;
using GDMiniJSON;
using HarmonyLib;
using UnityEngine;

namespace EventLib.Impl; 

public static class SaveLoadPatches {
    public static Dictionary<string, object> metadata;
    public const int MAGIC_NUMBER = int.MaxValue;
    [HarmonyPatch(typeof(LevelData), "Encode")]
    public static class EncodeLevelPatch {
        public static void Prefix(LevelData __instance) {
            metadata = new Dictionary<string, object>();
            __instance.levelEvents.Add(new LevelEvent(MAGIC_NUMBER, LevelEventType.EditorComment));
        }
        
        public static void Postfix(LevelData __instance) {
            __instance.levelEvents.RemoveAt(__instance.levelEvents.Count - 1);
        }
    }
    
    [HarmonyPatch(typeof(LevelEvent), "Encode")]
    public static class EncodePatch {
        public static bool Prefix(LevelEvent __instance, ref string __result) {
            if ( __instance.floor != MAGIC_NUMBER) return true;
            var builder = new StringBuilder();
            builder.Append(RDEditorUtils.EncodeInt("floor", MAGIC_NUMBER));
            builder.Append(RDEditorUtils.EncodeString("eventType", LevelEventType.EditorComment.ToString()));
            var data = Json.Serialize(metadata)[2..^2];
            builder.Append(RDEditorUtils.EncodeString("comment", LevelEvent.EscapeTextForJSON($"@Metadata\n{data}"), true));
            __result = builder.ToString();
            return false;
        }
        
        public static void Postfix(LevelEvent __instance, ref string __result) {
            if (!__instance.eventType.IsCustomEventType()) return;
            var builder = new StringBuilder();
            builder.Append(RDEditorUtils.EncodeInt("floor", __instance.floor));
            builder.Append(RDEditorUtils.EncodeString("eventType", LevelEventType.EditorComment.ToString()));
            builder.Append(RDEditorUtils.EncodeString("comment", LevelEvent.EscapeTextForJSON($"@CustomEvent\n{__result}"), true));
            var idx = scnEditor.instance.events.IndexOf(__instance);
            var meta = __instance.GetCustomEvent().EncodeExtra();
            metadata[$"customEvent_{idx}"] = meta;
            __result = builder.ToString();
        }
    }
    
    [HarmonyPatch(typeof(LevelData), "Decode")]
    public static class DecodeLevelPatch {
        public static void Postfix(LevelData __instance, Dictionary<string, object> dict) {
        }
    }
    
    [HarmonyPatch(typeof(LevelEvent), "Decode")]
    public static class DecodePatch {
        public static bool Prefix(LevelEvent __instance, Dictionary<string, object> dict, string explicitEventType, bool isGlobal, ref LevelEvent.DecodeResult __result) {
            if (explicitEventType != null) return true;
            if (isGlobal) return true;
            var evnt_type = RDUtils.ParseEnum<LevelEventType>(dict["eventType"] as string);
            if (evnt_type != LevelEventType.EditorComment) return true;
            if (dict["comment"] is not string comment) return true;
            if (comment.StartsWith("@CustomEvent")) {
                var data = Json.Deserialize($"{{{comment[13..]}}}") as Dictionary<string, object>;
                __result = __instance.Decode(data);

                var type = MappingInternal.registeredTypes[__instance.eventType];
                var evnt = (CustomEventBase)Activator.CreateInstance(type);
                evnt.editor = scnEditor.instance;
                evnt.levelEvent = __instance;
                MappingInternal.customEvents.Add(__instance, evnt);

                return false;
            } else if (comment.StartsWith("@Metadata")) {
                var data = Json.Deserialize($"{{{comment[10..]}}}") as Dictionary<string, object>;
                foreach (var (key, value) in data) {
                    var k = int.Parse(key[12..]);
                    var evnt = scnEditor.instance.events[k];
                    var c_evnt = evnt.GetCustomEvent();
                    c_evnt.DecodeExtra(value as Dictionary<string, object>);
                }

                return true;
            }

            return true;
        }
    }
}
