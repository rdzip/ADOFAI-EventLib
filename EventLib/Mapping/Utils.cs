using ADOFAI;
using EventLib.CustomEvent;

namespace EventLib.Mapping; 

public static class Utils {
    public static bool IsCustomEventType(this LevelEventType type) => MappingInternal.registeredTypes.ContainsKey(type);
    public static CustomEventBase GetCustomEvent(this LevelEvent evnt) => MappingInternal.customEvents[evnt];
    public static T GetCustomEvent<T>(this LevelEvent evnt) where T : CustomEventBase => (T) evnt.GetCustomEvent();
}
