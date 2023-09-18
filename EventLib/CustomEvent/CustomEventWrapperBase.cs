using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ADOFAI;
using EventLib.Mapping;
using UnityEngine;

namespace EventLib.CustomEvent;

public abstract class CustomEventWrapperBase : CustomEventBase {
    public CustomEventWrapperBase() {
        children = _children.AsReadOnly();
        childDecos = _childDecos.AsReadOnly();
    }
    
    public LevelEvent AddChildEvent(int floorID, LevelEventType eventType) {
        var evnt = new LevelEvent(floorID, eventType);
        AddChildEvent(evnt);
        return evnt;
    }

    public void AddChildEvent(LevelEvent evnt) {
        _children.Add(evnt);
        MappingInternal.internalEvents.Add(evnt);
    }
    
    public void DeleteChildEvent(LevelEvent evnt) {
        _children.Remove(evnt);
        MappingInternal.internalEvents.Remove(evnt);
    }
    
    public LevelEvent AddChildDeco(LevelEventType decoType, bool create = false) {
        var deco = new LevelEvent(-1, decoType) {
            ["position"] = Vector3.zero,
        };
        AddChildDeco(deco, create);
        return deco;
    }
    
    public void AddChildDeco(LevelEvent deco, bool create = false) {
        _childDecos.Add(deco);
        MappingInternal.internalDecos.Add(deco);
        if (create) scrDecorationManager.instance.CreateDecoration(deco, out _);
    }
    
    public void DeleteChildDeco(LevelEvent deco, bool update = false) {
        _childDecos.Remove(deco);
        MappingInternal.internalDecos.Remove(deco);
        if (update) editor.UpdateDecorationObjects();
    }

    public override void OnRemoveEvent() {
        _children.ToList().ForEach(DeleteChildEvent);
        _childDecos.ToList().ForEach(d => DeleteChildDeco(d));
        editor.UpdateDecorationObjects();
    }

    private readonly List<LevelEvent> _children = new();
    public readonly ReadOnlyCollection<LevelEvent> children;

    private readonly List<LevelEvent> _childDecos = new();
    public readonly ReadOnlyCollection<LevelEvent> childDecos;

    public override Dictionary<string, object> EncodeExtra() {
        // Encode 시에는 Internal Event도 함께 Encode
        return new Dictionary<string, object> {
            { "children", _children.Select(c => editor.events.IndexOf(c)).ToList() },
            { "childDecos", _childDecos.Select(c => editor.decorations.IndexOf(c)).ToList() },
        };
    }

    public override void DecodeExtra(Dictionary<string, object> data) {
        if (data.TryGetValue("children", out var ch)) {
            foreach (var child in ((List<object>) ch).Cast<int>()) {
                var evnt = editor.events[child];
                AddChildEvent(evnt);
            }
        }
        
        if (data.TryGetValue("childDecos", out var cd)) {
            foreach (var child in ((List<object>) cd).Cast<int>()) {
                var evnt = editor.decorations[child];
                AddChildDeco(evnt);
            }
        }
    }
}