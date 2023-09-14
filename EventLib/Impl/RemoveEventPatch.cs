using System;
using System.Linq;
using ADOFAI;
using EventLib.CustomEvent;
using EventLib.Mapping;
using HarmonyLib;

namespace EventLib.Impl; 

[HarmonyPatch(typeof(scnEditor), "RemoveEvent")]
public class RemoveEventPatch {
    public static void Prefix(LevelEvent evnt) {
        if (!evnt.eventType.IsCustomEventType()) return;
        var c_evnt = evnt.GetCustomEvent();
        c_evnt.OnRemoveEvent();
    }
}
