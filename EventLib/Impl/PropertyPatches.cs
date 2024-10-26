using System;
using System.Linq;
using System.Reflection;
using ADOFAI;
using ADOFAI.LevelEditor.Controls;
using EventLib.CustomEvent;
using EventLib.Mapping;
using HarmonyLib;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace EventLib.Impl; 

public static class PropertyPatches {
    [HarmonyPatch(typeof(PropertiesPanel), "SetProperties")]
    public static class PropertySetPatch {
        public static void Prefix(PropertiesPanel __instance) {
            var evnt = __instance.inspectorPanel.selectedEvent;
            if (evnt == null) return;
            if (!MappingInternal.properties.TryGetValue(evnt.eventType, out var dict)) return;
            var props = evnt.data.Keys.ToArray();
            foreach (var name in props) {
                if (!dict.TryGetValue(name, out var member)) continue;
                evnt.data[name] = member switch {
                    FieldInfo f => f.GetValue(evnt.GetCustomEvent()),
                    PropertyInfo p => p.GetValue(evnt.GetCustomEvent()),
                    _ => throw new NotSupportedException(),
                };
            }
        }
    }
    
    [HarmonyPatch(typeof(PropertyControl_Toggle), "SelectVar")]
    public static class PropertySetPatchToggle {
        public static void Postfix(PropertyControl_Toggle __instance, ref string var) {
            if (__instance.propertyInfo.type != PropertyType.Bool) return;
            var evnt = __instance.propertiesPanel.inspectorPanel.selectedEvent;
            if (evnt == null) return;
            if (!MappingInternal.properties.TryGetValue(evnt.eventType, out var dict)) return;
            if (!dict.TryGetValue(__instance.propertyInfo.name, out var member)) return;
            
            if (member is FieldInfo f) f.SetValue(evnt, var);
            else if (member is PropertyInfo p) p.SetValue(evnt, var);
            
            evnt.GetCustomEvent().OnValueChange(__instance.propertyInfo.name, var);
        }
    }

    private static void UpdateValues(PropertyControl control) {
        var evnt = control.propertiesPanel.inspectorPanel.selectedEvent;
        var name = control.propertyInfo.name;
        if (evnt == null) return;
        if (!MappingInternal.properties.TryGetValue(evnt.eventType, out var dict)) return;
        if (!dict.TryGetValue(name, out var member)) return;
                
        var val = evnt[name];
        if (member is FieldInfo f) f.SetValue(evnt.GetCustomEvent(), val);
        else if (member is PropertyInfo p) p.SetValue(evnt.GetCustomEvent(), val);
        
        evnt.GetCustomEvent().OnValueChange(name, val);
    }
    
    [HarmonyPatch(typeof(PropertyControl_Text), "Setup")]
    public static class PropertySetPatchText {
        public static void Postfix(PropertyControl_Text __instance) {
            __instance.inputField.onEndEdit.AddListener(_ => {
                UpdateValues(__instance);
            });
        }
    }
    
    [HarmonyPatch(typeof(PropertyControl_Color), "Setup")]
    public static class PropertySetPatchColor {
        public static void Postfix(PropertyControl_Color __instance) {
            __instance.colorField.onChange.AddListener(_ => {
                UpdateValues(__instance);
            });
        }
    }
    
    [HarmonyPatch(typeof(PropertyControl_Vector2), "Setup")]
    public static class PropertySetPatchVector2 {
        public static void Postfix(PropertyControl_Vector2 __instance) {
            __instance.inputX.onEndEdit.AddListener(_ => {
                UpdateValues(__instance);
            });
            
            __instance.inputY.onEndEdit.AddListener(_ => {
                UpdateValues(__instance);
            });
        }
    }
    
    [HarmonyPatch(typeof(PropertyControl_File), "ProcessFile")]
    public static class PropertySetPatchFile {
        public static void Postfix(PropertyControl_File __instance) {
            UpdateValues(__instance);
        }
    }
    
    [HarmonyPatch(typeof(PropertyControl_Tile), "Setup")]
    public static class PropertySetPatchTile {
        public static void Postfix(PropertyControl_Tile __instance) {
            __instance.inputField.onEndEdit.AddListener(_ => {
                UpdateValuesTile();
            });
            
            __instance.buttonFirstTile.onClick.AddListener(UpdateValuesTile);
            __instance.buttonLastTile.onClick.AddListener(UpdateValuesTile);
            __instance.buttonThisTile.onClick.AddListener(UpdateValuesTile);

            void UpdateValuesTile() {
                var evnt = __instance.propertiesPanel.inspectorPanel.selectedEvent;
                var name = __instance.propertyInfo.name;
                if (evnt == null) return;

                if (!MappingInternal.properties.TryGetValue(evnt.eventType, out var dict)) return;
                if (!dict.TryGetValue(name, out var member)) return;
                
                var val = __instance.tileValue;
                if (member is FieldInfo f) f.SetValue(evnt.GetCustomEvent(), val);
                else if (member is PropertyInfo p) p.SetValue(evnt.GetCustomEvent(), val);
        
                evnt.GetCustomEvent().OnValueChange(name, val);
            }
        }
    }
    
    [HarmonyPatch(typeof(PropertiesPanel), "Init")]
    public static class PropertySetPatchInit {
        public static void Postfix(PropertiesPanel __instance) {
            foreach (var (s, p) in __instance.properties) {
                p.enabledButton.onClick.AddListener(() => {
                    var evnt = __instance.inspectorPanel.selectedEvent;
                    var name = p.info.name;
                    if (evnt == null || !evnt.eventType.IsCustomEventType()) return;
                    evnt.GetCustomEvent().OnDisableChange(s, evnt.disabled[name]);
                });
            }
        }
    }
}
