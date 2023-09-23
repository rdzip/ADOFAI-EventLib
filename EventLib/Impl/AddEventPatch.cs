using System;
using System.Collections.Generic;
using System.Linq;
using ADOFAI;
using EventLib.CustomEvent;
using EventLib.EventHandler;
using EventLib.Mapping;
using HarmonyLib;

namespace EventLib.Impl;

public static class EventPatches {
    [HarmonyPatch(typeof(scnEditor), "AddEvent")]
    public static class AddEventPatch {
        public static void Postfix(scnEditor __instance, int floorID) {
            var new_evnt = __instance.events.Last();
            if (!new_evnt.eventType.IsCustomEventType()) return;
            var type = MappingInternal.registeredTypes[new_evnt.eventType];
            var evnt = (CustomEventBase)Activator.CreateInstance(type);
            evnt.editor = __instance;
            evnt.levelEvent = new_evnt;
            evnt.OnAddEvent(floorID);
        }
    }

    [HarmonyPatch(typeof(scnGame), "ApplyEventsToFloors", typeof(List<scrFloor>))]
    public static class FloorUpdatePatch {
        public static void Prefix() {
            if (!scnEditor.instance) return;
            scnEditor.instance.events.AddRange(MappingInternal.internalEvents);
        }

        public static void Postfix() {
            if (!scnEditor.instance) return;
            scnEditor.instance.events.RemoveAll(MappingInternal.internalEvents.Contains);
        }
    }

    [HarmonyPatch(typeof(scnGame), "UpdateDecorationObjects")]
    public static class DecorationUpdatePatch {
        public static void Prefix() {
            if (!scnEditor.instance) return;
            scnEditor.instance.decorations.AddRange(MappingInternal.internalDecos);
        }

        public static void Postfix() {
            if (!scnEditor.instance) return;
            scnEditor.instance.decorations.RemoveAll(MappingInternal.internalDecos.Contains);
        }
    }

    [HarmonyPatch(typeof(scnEditor), "CopyOfFloor")]
    public static class CopyFloorPatch {
        public static void Prefix() {
            CopyEventPatch.disableChild = true;
        }
        public static void Postfix(ref scnEditor.FloorData __result) {
            CopyEventPatch.disableChild = false;
            __result.levelEventData.RemoveAll(e => e == null);
            __result.attachedDecorations.RemoveAll(e => e == null);
        }
    }

    [HarmonyPatch(typeof(scnEditor), "OffsetFloorIDsInEvents")]
    public static class FloorOffsetPatch {
        public static void Postfix(int startFloorID, int offset) {
            if (!scnEditor.instance) return;
            foreach (var evnt in MappingInternal.internalEvents.Where(evnt => evnt.floor > startFloorID)) {
                evnt.floor += offset;
            }

            foreach (var evnt in MappingInternal.internalDecos.Where(evnt => evnt.floor > startFloorID)) {
                evnt.floor += offset;
            }
        }
    }

    [HarmonyPatch(typeof(List<LevelEvent>), "RemoveAt")]
    public static class RemoveAtPatch {
        private static CustomEventBase _target;

        public static void Prefix(object __instance, int index) {
            if (__instance is not List<LevelEvent> list) return; // for HarmonyLib
            if (!scnEditor.instance) return;
            if (list == scnEditor.instance.events) {
                var t = list[index];
                if (t?.eventType.IsCustomEventType() == true) _target = t.GetCustomEvent();
            }
        }

        public static void Postfix(object __instance) {
            if (__instance is not List<LevelEvent> list) return; // for HarmonyLib
            if (!scnEditor.instance) return;
            if (list == scnEditor.instance.events) {
                if (_target is null) return;
                _target.OnRemoveEvent();
                _target = null;
            }
        }
    }

    [HarmonyPatch(typeof(List<LevelEvent>), "RemoveRange")]
    public static class RemoveRangePatch {
        private static List<CustomEventBase> _targets = new();

        public static void Prefix(object __instance, int index, int count) {
            if (__instance is not List<LevelEvent> list) return; // for HarmonyLib
            
            if (!scnEditor.instance) return;
            if (list == scnEditor.instance.events) {
                for (var i = index; i < index + count; i++) {
                    var t = list[i];
                    if (t?.eventType.IsCustomEventType() == true) _targets.Add(t.GetCustomEvent());
                }
            }
        }

        public static void Postfix(object __instance) {
            if (__instance is not List<LevelEvent> list) return; // for HarmonyLib
            
            if (!scnEditor.instance) return;
            if (list == scnEditor.instance.events) {
                foreach (var target in _targets) {
                    target.OnRemoveEvent();
                }

                _targets.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(List<LevelEvent>), "RemoveAll")]
    public static class RemoveAllPatch {
        private static List<CustomEventBase> _targets = new();

        public static void Prefix(object __instance, Predicate<LevelEvent> match) {
            if (__instance is not List<LevelEvent> list) return; // for HarmonyLib
            
            if (!scnEditor.instance) return;
            if (__instance == scnEditor.instance.events) {
                foreach (var t in list) {
                    L.og(t);
                    if (t?.eventType.IsCustomEventType() == true && match(t)) _targets.Add(t.GetCustomEvent());
                }
            }
        }

        public static void Postfix(object __instance) {
            if (__instance is not List<LevelEvent> list) return; // for HarmonyLib
            
            if (!scnEditor.instance) return;
            if (list == scnEditor.instance.events) {
                foreach (var target in _targets) {
                    target.OnRemoveEvent();
                }

                _targets.Clear();
            }
        }
    }
    // ReSharper enable Unity.NoNullPropagation

    [HarmonyPatch(typeof(scnEditor), "CopyEvent")]
    public static class CopyEventPatch {
        public static bool disableChild;
        
        public static void Postfix(LevelEvent eventToCopy, ref LevelEvent __result) {
            if (disableChild && eventToCopy.IsChildEvent()) {
                __result = null;
                return;
            }
            
            if (__result.eventType.IsCustomEventType()) {
                __result["@customEvent"] = __result.GetCustomEvent().Copy(__result);
            }
        }
    }
}