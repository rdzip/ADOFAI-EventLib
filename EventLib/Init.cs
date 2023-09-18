using System;
using System.Collections.Generic;
using ADOFAI;
using EventLib.CustomEvent;
using EventLib.Mapping;
using HarmonyLib;

namespace EventLib; 

[HarmonyPatch(typeof(scnEditor), "Awake")]
public class Init {
    internal static Queue<Type> registrationQueue = new();
    internal static bool init = false;
    public static void Prefix() {
        MappingInternal.internalEvents.Clear();
        MappingInternal.internalDecos.Clear();
        
        if (init) return;
        init = true;
        CustomEventManager.RegisterCustomEvent<TestEventBase>();
        while (registrationQueue.Count > 0) {
            var type = registrationQueue.Dequeue();
            CustomEventManager.RegisterCustomEvent(type);
        }

        registrationQueue = null;
    }
}
