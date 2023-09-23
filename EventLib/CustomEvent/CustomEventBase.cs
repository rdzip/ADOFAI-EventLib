using System;
using System.Collections.Generic;
using ADOFAI;
using EventLib.Mapping;

namespace EventLib.CustomEvent;

public abstract class CustomEventBase {
    protected internal scnEditor editor { get; internal set; }

    public LevelEvent levelEvent {
        get {
            if (editor.events.Contains(_levelEvent) && _levelEvent["@customEvent"] == this) return _levelEvent;
            _levelEvent = editor.events.Find(e => e["@customEvent"] == this);
            return _levelEvent;
        }
        protected internal set {
            _levelEvent = value;
            _levelEvent["@customEvent"] = this;
            _levelEvent.disabled["@customEvent"] = false;
        }
    }
    private LevelEvent _levelEvent;

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
    public virtual CustomEventBase Copy(LevelEvent evnt) {
        var e = (CustomEventBase) Activator.CreateInstance(GetType());
        e.editor = editor;
        e.levelEvent = evnt;
        return e;
    }

    public bool IsCutOrRemoved => levelEvent == null;
}