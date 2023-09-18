using ADOFAI;
using EventLib.CustomEvent;

namespace EventLib.Mapping; 

public static class Utils {
    public static bool IsCustomEventType(this LevelEventType type) => MappingInternal.registeredTypes.ContainsKey(type);
    public static CustomEventBase GetCustomEvent(this LevelEvent evnt) => (CustomEventBase) evnt["@customEvent"];
    public static T GetCustomEvent<T>(this LevelEvent evnt) where T : CustomEventBase => (T) evnt.GetCustomEvent();
    public static bool IsChildEvent(this LevelEvent evnt) => MappingInternal.internalEvents.Contains(evnt) || MappingInternal.internalDecos.Contains(evnt);
}
