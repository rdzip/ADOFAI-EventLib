using System;
using System.Collections.Generic;
using System.Linq;
using ADOFAI;
using EventLib.CustomEvent;
using EventLib.EventHandler;
using EventLib.Mapping;
using HarmonyLib;

namespace EventLib.Impl;

public class EventPatches {
    [HarmonyPatch(typeof(scnEditor), "AddEvent")]
    public class AddEventPatch {
        public static void Postfix(scnEditor __instance, int floorID) {
            var new_evnt = __instance.events.Last();
            if (!new_evnt.eventType.IsCustomEventType()) return;
            var type = MappingInternal.registeredTypes[new_evnt.eventType];
            var evnt = (CustomEventBase)Activator.CreateInstance(type);
            evnt.editor = __instance;
            evnt.levelEvent = new_evnt;
            evnt.levelEvent.data["@customEvent"] = evnt;
            evnt.levelEvent.disabled["@customEvent"] = false;
            evnt.OnAddEvent(floorID);
        }
    }
    
    [HarmonyPatch(typeof(scnEditor), "RemoveEvent")]
    public class RemoveEventPatch {
        public static void Prefix(LevelEvent evnt) {
            if (!evnt.eventType.IsCustomEventType()) return;
            var c_evnt = evnt.GetCustomEvent();
            c_evnt.OnRemoveEvent();
        }
    }

    
    [HarmonyPatch(typeof(scnGame), "ApplyEventsToFloors", typeof(List<scrFloor>))]
    public class FloorUpdatePatch {
        public static void Prefix() {
            if (scnEditor.instance == null) return;
            scnEditor.instance.events.AddRange(MappingInternal.internalEvents);
        }
        
        public static void Postfix() {
            if (scnEditor.instance == null) return;
            scnEditor.instance.events.RemoveAll(MappingInternal.internalEvents.Contains);
        }
    }
    
    [HarmonyPatch(typeof(scnGame), "UpdateDecorationObjects")]
    public class DecorationUpdatePatch {
        public static void Prefix() {
            if (scnEditor.instance == null) return;
            scnEditor.instance.decorations.AddRange(MappingInternal.internalDecos);
        }
        
        public static void Postfix() {
            if (scnEditor.instance == null) return;
            scnEditor.instance.decorations.RemoveAll(MappingInternal.internalDecos.Contains);
        }
    }
    
    [HarmonyPatch(typeof(scnEditor), "OffsetFloorIDsInEvents")]
    public class FloorOffsetPatch {
        public static void Postfix(int startFloorID, int offset) {
            if (scnEditor.instance == null) return;
            foreach (var evnt in MappingInternal.internalEvents.Where(evnt => evnt.floor > startFloorID)) {
                evnt.floor += offset;
            }
            
            foreach (var evnt in MappingInternal.internalDecos.Where(evnt => evnt.floor > startFloorID)) {
                evnt.floor += offset;
            }
        }
    }
    
    [HarmonyPatch(typeof(scnEditor), "DeleteFloor")]
    public class DeleteFloorPatch {
        private static SaveStateScope _scope;
        private static List<LevelEvent> _events = new();
        public static void Prefix(int sequenceIndex, scnEditor __instance) {
            if (__instance.lockPathEditing) return;
            _scope = new SaveStateScope(__instance);
            _events.AddRange(__instance.events.Where(e => e.floor == sequenceIndex));
        }
        
        public static void Postfix(int sequenceIndex, scnEditor __instance) {
            if (__instance.lockPathEditing) return;
            bool remake = false;
            foreach (var evnt in _events) {
                if (!evnt.eventType.IsCustomEventType()) continue;
                if (__instance.events.Contains(evnt)) continue;
                var c_evnt = evnt.GetCustomEvent();
                c_evnt.OnRemoveEvent();
                remake = true;
            }
            
            __instance.RemakePath();
            _scope.Dispose();
            _events.Clear();
        }
    }
}