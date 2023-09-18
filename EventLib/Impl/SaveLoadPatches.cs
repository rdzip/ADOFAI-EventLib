using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADOFAI;
using EventLib.CustomEvent;
using EventLib.Mapping;
using GDMiniJSON;
using HarmonyLib;
using UnityEngine;

namespace EventLib.Impl; 

public static class SaveLoadPatches {
    private static Dictionary<string, object> _metadata;
    private const int MAGIC_NUMBER = int.MaxValue;
    [HarmonyPatch(typeof(LevelData), "Encode")]
    public static class EncodeLevelPatch {
        public static void Prefix(LevelData __instance) {
            _metadata = new Dictionary<string, object>();
            __instance.levelEvents.AddRange(MappingInternal.internalEvents);
            __instance.levelEvents.Sort((e1, e2) => e1.floor - e2.floor);
            __instance.levelEvents.Add(new LevelEvent(MAGIC_NUMBER, LevelEventType.EditorComment));
            
            __instance.decorations.AddRange(MappingInternal.internalDecos);
        }
        
        public static void Postfix(LevelData __instance) {
            __instance.levelEvents.RemoveAt(__instance.levelEvents.Count - 1);
            __instance.levelEvents.RemoveAll(MappingInternal.internalEvents.Contains);
            scnEditor.instance.decorations.RemoveAll(MappingInternal.internalDecos.Contains);
        }
    }
    
    [HarmonyPatch(typeof(LevelEvent), "Encode")]
    public static class EncodePatch {
        public static bool Prefix(LevelEvent __instance, ref string __result) {
            if ( __instance.floor != MAGIC_NUMBER) return true;
            var builder = new StringBuilder();
            builder.Append(RDEditorUtils.EncodeInt("floor", MAGIC_NUMBER));
            builder.Append(RDEditorUtils.EncodeString("eventType", LevelEventType.EditorComment.ToString()));
            var data = Json.Serialize(_metadata)[2..^2];
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
            _metadata[$"customEvent_{idx}"] = meta;
            __result = builder.ToString();
        }
    }
    
    [HarmonyPatch(typeof(LevelData), "Decode")]
    public static class DecodeLevelPatch {
        public static string metaData = null;
        
        public static void Prefix() {
            metaData = null;
            MappingInternal.internalEvents.Clear();
            MappingInternal.internalDecos.Clear();
        }
        public static void Postfix() {
            if (metaData == null) return;
            var data = (Dictionary<string, object>) Json.Deserialize(metaData);
            L.og(data);
            foreach (var (key, value) in data) {
                var k = int.Parse(key[12..]);
                var evnt = scnEditor.instance.events[k];
                var c_evnt = evnt.GetCustomEvent();
                c_evnt.DecodeExtra(value as Dictionary<string, object>);
            }

            scnEditor.instance.events.RemoveAll(MappingInternal.internalEvents.Contains);
            scnEditor.instance.decorations.RemoveAll(MappingInternal.internalDecos.Contains);
            foreach (var evnt in scnEditor.instance.events.Where(e => e.eventType.IsCustomEventType())) {
                evnt.GetCustomEvent().OnLoadEvent(evnt.data, evnt.disabled);
            }
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
                evnt.levelEvent.data["@customEvent"] = evnt;
                evnt.levelEvent.disabled["@customEvent"] = false;

                return false;
            }

            if (comment.StartsWith("@Metadata")) {
                DecodeLevelPatch.metaData = $"{{{comment[10..]}}}";
                return true;
            }

            return true;
        }
    }
}
