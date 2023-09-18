using System;
using System.Collections.Generic;
using ADOFAI;
using EventLib.Mapping;

namespace EventLib.CustomEvent;

public abstract class CustomEventBase {
    protected internal scnEditor editor { get; internal set; }
    public LevelEvent levelEvent { get; internal set; }

    public virtual void OnAddEvent(int floorId) { }
    public virtual void OnRemoveEvent() { }

    public virtual void OnLoadEvent(Dictionary<string, object> data, Dictionary<string, bool> disabled) {
        var props = MappingInternal.properties[levelEvent.eventType];
        foreach (var (k, v) in data) {
            try {
                var value = v;
                if (!props.TryGetValue(k, out var p)) continue;
                if (p is System.Reflection.FieldInfo f2) f2.SetValue(this, value);
                else if (p is System.Reflection.PropertyInfo p2) p2.SetValue(this, value);
            } catch (Exception e) {
                L.og(e);
            }
        }
        
        foreach (var (k, v) in disabled) {
            OnDisableChange(k, v);
        }
    }
    
    public virtual void DecodeExtra(Dictionary<string, object> data) { }
    public virtual Dictionary<string, object> EncodeExtra() => null;
    
    public virtual void OnValueChange(string key, object value) { }
    public virtual void OnDisableChange(string key, bool enabled) { }
}