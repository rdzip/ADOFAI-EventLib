/*using System;
using System.Collections.Generic;
using System.Reflection;
using ADOFAI;
using EventLib.Mapping;
using HarmonyLib;
using UnityEngine;

namespace EventLib.Impl;

public static class DisplayPatch {
    [HarmonyPatch(typeof(InspectorPanel), "ShowTabsForFloor")]
    public static class ShowTabsForFloorPatch {
        private static readonly FieldInfo _cacheSelectedEventType = AccessTools.Field(typeof(InspectorPanel), "cacheSelectedEventType");
        
        public static bool Prefix(InspectorPanel __instance, int floorID) {
            var type_list = new List<LevelEventType>();
            var events = scnEditor.instance.events;
            var decorations = scnEditor.instance.decorations;
            foreach (var evnt in events) {
                if (evnt.floor == floorID && (!evnt.IsChildEvent() || Main.showInternalEvents)) type_list.Add(evnt.eventType);
            }

            __instance.titleCanvas.SetActive(type_list.Count > 0);
            ModifyMessageText2(__instance, "", false);
            if (type_list.Count == 0) {
                __instance.ShowPanel(LevelEventType.None);
                ModifyMessageText(__instance, RDString.Get("editor.dialog.noEventsOnTile"), 0.0f, true);
                ADOBase.editor.DeselectAllDecorations();
            }
            else {
                var eventType = LevelEventType.None;
                var flag = false;
                var values = Enum.GetValues(typeof(LevelEventType));
                foreach (var levelEventType in type_list) {
                    if (levelEventType == (LevelEventType) _cacheSelectedEventType.GetValue(__instance)) {
                        eventType = levelEventType;
                        flag = true;
                        break;
                    }
                }

                if (!flag) {
                    foreach (LevelEventType levelEventType1 in values) {
                        foreach (var levelEventType2 in type_list) {
                            if (levelEventType2 == levelEventType1) {
                                eventType = levelEventType2;
                                break;
                            }
                        }

                        if (eventType != LevelEventType.None) break;
                    }
                }

                __instance.selectedEventType = LevelEventType.None;
                if (eventType != LevelEventType.AddDecoration && eventType != LevelEventType.AddText)
                    ADOBase.editor.DeselectAllDecorations();
                __instance.ShowPanel(eventType);
                __instance.ShowInspector(true);
            }

            var list = new List<string>();
            foreach (var type in type_list) {
                var str = type.ToString();
                if (!list.Contains(str)) list.Add(str);
            }

            var count = list.Count;
            var height = __instance.tabs.rect.height;
            var num1 = 68f;
            if (count * 68.0 >= height) {
                var num2 = (height - 68f * count) / (count * count);
                num1 = height / count + num2;
            }

            var num3 = -1;
            foreach (Transform tab in __instance.tabs) {
                var flag = list.Contains(tab.name);
                tab.gameObject.SetActive(flag);
                if (flag) {
                    ++num3;
                    list.Remove(tab.name);
                }

                var y = -num1 * num3;
                tab.GetComponent<RectTransform>().SetAnchorPosY(y);
            }
            
            static void ModifyMessageText(InspectorPanel panel, string text, float yPos, bool enable)
            {
                if (panel.messageCanvas == null) return;
                panel.messageCanvas.SetActive(enable);
                panel.messageText.text = text;
                panel.messageText.rectTransform.anchoredPosition = panel.messageText.rectTransform.anchoredPosition.WithY(yPos);
            }
            
            static void ModifyMessageText2(InspectorPanel panel, string text, bool enable)
            {
                if (panel.messageCanvas == null) return;
                panel.messageCanvas.SetActive(enable);
                panel.messageText.text = text;
            }

            return false;
        }
    }
}*/