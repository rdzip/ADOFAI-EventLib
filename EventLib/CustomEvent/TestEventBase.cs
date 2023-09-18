using System;
using System.Collections.Generic;
using ADOFAI;
using UnityEngine;

namespace EventLib.CustomEvent; 

[CustomEvent("testEvent", 1001, allowMultiple = false, allowFirstFloor = false)]
[EventCategory(LevelEventCategory.Gameplay)]
public class TestEventBase : CustomEventWrapperBase {
    [EventIcon] public static Sprite GetIcon() => GCS.levelEventIcons[LevelEventType.Hide];
    private LevelEvent _event;

    public override void OnAddEvent(int floorId) {
        _event = AddChildEvent(levelEvent.floor, LevelEventType.Flash);
    }

    public override void DecodeExtra(Dictionary<string, object> data) {
        base.DecodeExtra(data);
        //_event = children[0];
    }

    /*[EventField]
    public float testInt {
        get => (float) _event["duration"];
        set => _event["duration"] = value;
    }*/
}
