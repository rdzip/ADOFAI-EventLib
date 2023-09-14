using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Numerics;
using System.Reflection;
using ADOFAI;
using EventLib.EnumLib;
using EventLib.Mapping;
using ADO_PropertyInfo = ADOFAI.PropertyInfo;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace EventLib.CustomEvent; 

public static class CustomEventManager {
    public static void RegisterCustomEvent<T>() where T : CustomEventBase {
        var info = GetLevelEventInfo<T>();
        GCS.levelEventsInfo.Add(info.name, info);
        MappingInternal.registeredTypes[info.type] = typeof(T);
    }

    private static LevelEventInfo GetLevelEventInfo<T>() where T : CustomEventBase {
        var attr = typeof(T).GetCustomAttribute<CustomEventAttribute>();
        var info = new LevelEventInfo();
        info.name = attr.id;
        info.type = attr.levelEventType;
        
        EnumPatcher<LevelEventType>.AddField(info.name, (ulong) info.type);
        GCS.levelEventIcons[info.type] = GCS.levelEventIcons[LevelEventType.SongSettings];
        GCS.levelEventTypeString[info.type] = info.name;
        
        info.allowFirstFloor = attr.allowFirstFloor;
        info.categories = typeof(T).GetCustomAttributes<EventCategoryAttribute>().Select(c => c.category).ToList();
        info.propertiesInfo = new Dictionary<string, ADO_PropertyInfo>();

        MappingInternal.properties[info.type] = new Dictionary<string, MemberInfo>();
        
        foreach (var m in typeof(T).GetMembers()) {
            var f_attr = m.GetCustomAttribute<EventFieldAttribute>();
            if (f_attr == null) continue;
            switch (m) {
                case PropertyInfo p:
                    var dict = new Dictionary<string, object>();
                    var t = p.PropertyType;
                    dict["name"] = p.Name;
                    var p_type = f_attr.propertyType switch {
                        PropertyType.NotAssigned when t == typeof(bool) => "Bool",
                        PropertyType.NotAssigned when t == typeof(int) => "Int",
                        PropertyType.NotAssigned when t == typeof(float) => "Float",
                        PropertyType.NotAssigned when t == typeof(string) => "String",
                        PropertyType.NotAssigned when t == typeof(Color) => "Color",
                        PropertyType.NotAssigned when t == typeof(Vector2) => "Vector2",
                        PropertyType.NotAssigned when t.IsEnum => $"Enum:{t.Name}",
                        _ => f_attr.propertyType.ToString(),
                    };
                    dict["type"] = p_type;

                    if (f_attr.affectsFloors) dict["affectsFloors"] = true;
                    if (p_type == "Int" && f_attr.maxInt != int.MaxValue) dict["max"] = f_attr.maxInt;
                    if (p_type == "Int" && f_attr.minInt != int.MinValue) dict["min"] = f_attr.minInt;
                    if (p_type == "Float" && f_attr.maxFloat < float.PositiveInfinity) dict["max"] = f_attr.maxFloat;
                    if (p_type == "Float" && f_attr.minFloat > float.NegativeInfinity) dict["min"] = f_attr.minFloat;
                    if (p_type == "File") dict["fileType"] = f_attr.fileType.ToString();
                    if (f_attr.unit != null) dict["unit"] = f_attr.unit;
                    if (f_attr.canBeDisabled) dict["canBeDisabled"] = true;
                    
                    var d_attr = m.GetCustomAttribute<DisableIfAttribute>();
                    if (d_attr != null) dict["disableIf"] = d_attr.disableIf.Select(d => (object) d).ToList();
                    
                    var h_attr = m.GetCustomAttribute<HideIfAttribute>();
                    if (h_attr != null) dict["hideIf"] = h_attr.hideIf.Select(d => (object) d).ToList();
                    var p_info = new ADO_PropertyInfo(dict, info);
                    info.propertiesInfo[p.Name] = p_info;
                    
                    break;
                case FieldInfo f:
                    dict = new Dictionary<string, object>();
                    t = f.FieldType;
                    dict["name"] = f.Name;
                    p_type = f_attr.propertyType switch {
                        PropertyType.NotAssigned when t == typeof(bool) => "Bool",
                        PropertyType.NotAssigned when t == typeof(int) => "Int",
                        PropertyType.NotAssigned when t == typeof(float) => "Float",
                        PropertyType.NotAssigned when t == typeof(string) => "String",
                        PropertyType.NotAssigned when t == typeof(Color) => "Color",
                        PropertyType.NotAssigned when t == typeof(Vector2) => "Vector2",
                        PropertyType.NotAssigned when t.IsEnum => $"Enum:{t.Name}",
                        _ => f_attr.propertyType.ToString(),
                    };
                    dict["type"] = p_type;

                    if (f_attr.affectsFloors) dict["affectsFloors"] = true;
                    if (p_type == "Int" && f_attr.maxInt != int.MaxValue) dict["max"] = f_attr.maxInt;
                    if (p_type == "Int" && f_attr.minInt != int.MinValue) dict["min"] = f_attr.minInt;
                    if (p_type == "Float" && f_attr.maxFloat < float.PositiveInfinity) dict["max"] = f_attr.maxFloat;
                    if (p_type == "Float" && f_attr.minFloat > float.NegativeInfinity) dict["min"] = f_attr.minFloat;
                    if (p_type == "File") dict["fileType"] = f_attr.fileType.ToString();
                    if (f_attr.unit != null) dict["unit"] = f_attr.unit;
                    if (f_attr.canBeDisabled) dict["canBeDisabled"] = true;
                    
                    d_attr = m.GetCustomAttribute<DisableIfAttribute>();
                    if (d_attr != null) dict["disableIf"] = d_attr.disableIf.Select(d => (object) d).ToList();
                    
                    h_attr = m.GetCustomAttribute<HideIfAttribute>();
                    if (h_attr != null) dict["hideIf"] = h_attr.hideIf.Select(d => (object) d).ToList();
                    p_info = new ADO_PropertyInfo(dict, info);
                    info.propertiesInfo[f.Name] = p_info;
                    
                    break;
            }
            
            MappingInternal.properties[info.type][m.Name] = m;
        }
        
        return info;
    }
}
