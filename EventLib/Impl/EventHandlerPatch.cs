using System.Collections.Generic;
using System.Linq;
using ADOFAI;
using EventLib.EventHandler;
using EventLib.Mapping;
using HarmonyLib;

namespace EventLib.Impl;

public class EventHandlerPatch {
    [HarmonyPatch(typeof(scnGame), "ApplyEventsToFloors", typeof(List<scrFloor>))]
    public class FloorUpdatePatch {
        public static void Prefix() {
            if (scnEditor.instance == null || scnEditor.instance.playMode) return;
            var events = scnEditor.instance.events.Where(e => e.eventType.IsCustomEventType()).Select(e => e.GetCustomEvent());
            foreach (var evnt in events.OfType<IFloorUpdateHandler>()) {
                evnt.OnFloorUpdate();
            }
        }
    }
}
