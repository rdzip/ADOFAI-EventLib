using System.Reflection;
using ADOFAI;
using EventLib.Mapping;
using HarmonyLib;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace EventLib.Impl; 

public static class PropertyPatches {
    [HarmonyPatch(typeof(PropertyControl_Toggle), "SelectVar")]
    public static class PropertySetPatchToggle {
        public static void Postfix(PropertyControl_Toggle __instance, ref string var) {
            var evnt = __instance.propertiesPanel.inspectorPanel.selectedEvent;
            if (evnt == null) return;
            if (!MappingInternal.properties.TryGetValue(evnt.eventType, out var dict)) return;
            if (!dict.TryGetValue(__instance.propertyInfo.name, out var member)) return;
            
            if (member is FieldInfo f) f.SetValue(evnt, var);
            else if (member is PropertyInfo p) p.SetValue(evnt, var);
        }
    }
    
    [HarmonyPatch(typeof(PropertyControl_Text), "Setup")]
    public static class PropertySetPatchText {
        public static void Postfix(PropertyControl_Text __instance) {
            __instance.inputField.onEndEdit.AddListener(_ => {
                var evnt = __instance.propertiesPanel.inspectorPanel.selectedEvent;
                var name = __instance.propertyInfo.name;
                if (evnt == null) return;
                if (!MappingInternal.properties.TryGetValue(evnt.eventType, out var dict)) return;
                if (!dict.TryGetValue(name, out var member)) return;
                
                var val = evnt[name];
                if (member is FieldInfo f) f.SetValue(evnt.GetCustomEvent(), val);
                else if (member is PropertyInfo p) p.SetValue(evnt.GetCustomEvent(), val);
            });
        }
    }
}
