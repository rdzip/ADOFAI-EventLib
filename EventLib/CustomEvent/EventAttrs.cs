using System;
using ADOFAI;

namespace EventLib.CustomEvent; 

[AttributeUsage(AttributeTargets.Class)] public class CustomEventAttribute : Attribute {
    public readonly string id;
    public readonly LevelEventType levelEventType;
    public bool allowMultiple;
    public bool allowFirstFloor;

    public CustomEventAttribute(string id, LevelEventType event_type) {
        this.id = id;
        levelEventType = event_type;
    }
    
    public CustomEventAttribute(string id, int event_type) {
        this.id = id;
        levelEventType = (LevelEventType) event_type;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)] public class RequiredModAttribute : Attribute {
    public readonly string modId;
    public RequiredModAttribute(string modId) {
        this.modId = modId;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)] public class EventCategoryAttribute : Attribute {
    public readonly LevelEventCategory category;
    public EventCategoryAttribute(LevelEventCategory category) {
        this.category = category;
    }
}

[AttributeUsage(AttributeTargets.Method)] public class EventIconAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class EventFieldAttribute : Attribute {
    public bool affectsFloors = false;
    public int minInt = int.MinValue;
    public int maxInt = int.MaxValue;
    public float minFloat = float.NegativeInfinity;
    public float maxFloat = float.PositiveInfinity;
    public FileType fileType;
    
    public string unit = null;
    public readonly PropertyType propertyType;
    public readonly string key = null;
    
    public EventFieldAttribute(string key, PropertyType property_type) {
        this.key = key;
        propertyType = property_type;
    }
    
    public EventFieldAttribute(string key) {
        this.key = key;
    }
    
    public EventFieldAttribute(PropertyType property_type) {
        propertyType = property_type;
    }

    public EventFieldAttribute() {
        propertyType = PropertyType.NotAssigned;
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class CanBeDisabledAttribute : Attribute {
    public readonly bool startEnabled = false;
    public CanBeDisabledAttribute() { }
    public CanBeDisabledAttribute(bool start_enabled) {
        startEnabled = start_enabled;
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DisableIfAttribute : Attribute {
    public readonly string[] disableIf;
    public DisableIfAttribute(params string[] disableIf) {
        this.disableIf = disableIf;
    }
}


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class HideIfAttribute : Attribute {
    public readonly string[] hideIf;
    public HideIfAttribute(params string[] hideIf) {
        this.hideIf = hideIf;
    }
}

