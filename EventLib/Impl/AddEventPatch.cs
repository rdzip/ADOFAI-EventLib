using System;
using System.Linq;
using EventLib.CustomEvent;
using EventLib.Mapping;
using HarmonyLib;

namespace EventLib.Impl; 

[HarmonyPatch(typeof(scnEditor), "AddEvent")]
public class AddEventPatch {
    public static void Postfix(scnEditor __instance, int floorID) {
        var new_evnt = __instance.events.Last();
        if (!new_evnt.eventType.IsCustomEventType()) return;
        var type = MappingInternal.registeredTypes[new_evnt.eventType];
        var evnt = (CustomEventBase) Activator.CreateInstance(type);
        evnt.editor = __instance;
        evnt.levelEvent = new_evnt;
        MappingInternal.customEvents.Add(new_evnt, evnt);
        evnt.OnAddEvent(floorID);
    }
}
