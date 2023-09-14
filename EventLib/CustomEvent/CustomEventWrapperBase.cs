using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ADOFAI;

namespace EventLib.CustomEvent;

public abstract class CustomEventWrapperBase : CustomEventBase {
    public LevelEvent AddChildEvent(int floorID, LevelEventType eventType) {
        var evnt = new LevelEvent(floorID, eventType);
        editor.events.Add(evnt);
        children.Add(evnt);
        return evnt;
    }
    public void DeleteChildEvent(LevelEvent evnt) {
        editor.events.Remove(evnt);
        children.Remove(evnt);
    }

    public override void OnRemoveEvent() {
        children.ForEach(c => editor.events.Remove(c));
    }

    public readonly List<LevelEvent> children = new();

    public override Dictionary<string, object> EncodeExtra() {
        return new Dictionary<string, object> {
            { "children", children.Select(c => editor.events.IndexOf(c)).ToList() },
        };
    }

    public override void DecodeExtra(Dictionary<string, object> data) {
        if (data.TryGetValue("children", out var ch)) {
            foreach (var child in ((List<object>) ch).Cast<int>()) {
                children.Add(editor.events[child]);
            }
        }
    }
}