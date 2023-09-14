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

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)] public class EventIconAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class EventFieldAttribute : Attribute {
    public readonly PropertyType propertyType;
    public readonly bool affectsFloors = false;
    public readonly int minInt = int.MinValue;
    public readonly int maxInt = int.MaxValue;
    public readonly float minFloat = float.NegativeInfinity;
    public readonly float maxFloat = float.PositiveInfinity;
    public FileType fileType;
    public bool canBeDisabled;

    public string unit = null;
    public EventFieldAttribute(PropertyType property_type) {
        propertyType = property_type;
    }

    public EventFieldAttribute() {
        propertyType = PropertyType.NotAssigned;
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

