using System;
using System.Collections.Generic;
using ADOFAI;
using UnityEngine;

namespace EventLib.CustomEvent; 

[CustomEvent("testEvent", 1001, allowMultiple = false, allowFirstFloor = false)]
[EventCategory(LevelEventCategory.Gameplay)]
public class TestEventBase : CustomEventWrapperBase {
    [EventIcon] public static Sprite icon => throw new NotImplementedException();
    private LevelEvent _setSpeedEvnt;

    public override void OnAddEvent(int floorId) {
        _setSpeedEvnt = AddChildEvent(levelEvent.floor, LevelEventType.SetSpeed);
    }

    public override void DecodeExtra(Dictionary<string, object> data) {
        base.DecodeExtra(data);
        _setSpeedEvnt = children[0];
    }

    [EventField]
    public float testInt {
        get => (float) _setSpeedEvnt["beatsPerMinute"];
        set => _setSpeedEvnt["beatsPerMinute"] = value;
    }
}
