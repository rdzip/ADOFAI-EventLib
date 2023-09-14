using ADOFAI;
using EventLib.CustomEvent;
using EventLib.Mapping;
using HarmonyLib;

namespace EventLib; 

[HarmonyPatch(typeof(scnEditor), "Awake")]
public class Init {
    private static bool _init = false;
    public static void Prefix() {
        if (_init) return;
        _init = true;
        CustomEventManager.RegisterCustomEvent<TestEventBase>();
        MappingInternal.customEvents.Clear();
    }
}
