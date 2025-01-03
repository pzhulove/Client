using System;

/// <summary>
/// 事件信息可访问接口标签
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class BTEventTypeAttribute : Attribute
{
    public behaviac.BTM_EventType eventType;
    public BTEventTypeAttribute(behaviac.BTM_EventType type)
    {
        eventType = type;
    }
}

[System.AttributeUsage(System.AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class BTEventDataAttribute : Attribute
{
    public string agentFuncName;
    
    public BTEventDataAttribute(string name)
    {
        agentFuncName = name;
    }
}


[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class BTSceneEventTypeAttribute : Attribute
{
    public behaviac.BTM_LevelEventType eventType;
    public BTSceneEventTypeAttribute(behaviac.BTM_LevelEventType type)
    {
        eventType = type;
    }
}