using System.Collections.Generic;
using ADOFAI;

namespace EventLib.CustomEvent;

public abstract class CustomEventBase {
    protected internal scnEditor editor { get; internal set; }
    public LevelEvent levelEvent { get; internal set; }

    public virtual void OnAddEvent(int floorId) { }
    public virtual void OnRemoveEvent() { }
    
    public virtual void DecodeExtra(Dictionary<string, object> data) { }
    public virtual Dictionary<string, object> EncodeExtra() => null;
}