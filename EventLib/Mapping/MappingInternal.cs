using System;
using System.Collections.Generic;
using System.Reflection;
using ADOFAI;
using EventLib.CustomEvent;

namespace EventLib.Mapping; 

internal static class MappingInternal {
    public static readonly Dictionary<LevelEventType, Type> registeredTypes = new();
    public static readonly Dictionary<LevelEventType, Dictionary<string, MemberInfo>> properties = new();
    public static Dictionary<LevelEvent, CustomEventBase> customEvents = new();
}
