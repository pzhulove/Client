using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;

public class UIObjectAttribute : Attribute
{
    public string objectName;
    public UIObjectAttribute(string objectName)
    {
        this.objectName = objectName;
    }

}
public class UIControlAttribute : Attribute
{
    public string controlName;
    public System.Type componentType;
    public int baseNum;
    public UIControlAttribute(string controlName,System.Type componentType = null,int baseNum = 0)
    {
        this.controlName = controlName;
        this.componentType = componentType;
        this.baseNum = baseNum;
    }
}

public class UIControlArrayAttribute : Attribute
{
    public string controlName;
    public int    baseNum;
    public UIControlArrayAttribute(string controlName,int baseNum = 0)
    {
        this.controlName = controlName;
        this.baseNum = baseNum;
    }
}

[AttributeUsageAttribute(AttributeTargets.Method, Inherited = false,
    AllowMultiple = false)]
public class UIEventHandleAttribute : Attribute
{
    public string   controlName;
    public Type     eventType;
    public Type     controlType;
    public int      start;
    public int      end;

    public UIEventHandleAttribute(string controlName)
    {
        this.controlName    = controlName;
        this.eventType      = typeof(UnityEngine.Events.UnityAction);
        this.controlType    = typeof(UnityEngine.UI.Button);
        this.start          = 0;
        this.end            = 0;
    }

    public UIEventHandleAttribute(string controlName, Type controlType,Type eventType, int start = 0,int end = 0)
    {
        this.controlName    = controlName;
        this.eventType      = eventType;
        this.controlType    = controlType;
        this.start          = start;
        this.end            = end;
    }
}



public class MessageHandleAttribute : Attribute
{
    public uint id;
    public bool bNeedCache;
    public int order;
    public MessageHandleAttribute(uint id,bool bNeedCache = false, int order = 0)
    {
        this.id = id;
        this.bNeedCache = bNeedCache;
        this.order = order;
    }
}

public class ProtocolHandleAttribute : Attribute
{
    public Type binderType
    {
        get;
        private set;
    }
    public ProtocolHandleAttribute(Type type)
    {
        binderType = type;
    }

    public object GetBinder()
    {
        return Activator.CreateInstance(binderType);
    }
}

public class ClientSystemAttribute : Attribute
{
    public string name;
    public string stage;
    public ClientSystemAttribute(string name,string stage = "")
    {
        this.name =   name;
        this.stage =  stage;
    }
}

public class EnumCommonAttribute : Attribute
{
    public string[] contents;
    public EnumCommonAttribute(string content)
    {
        contents = content.Split('|');
    }
    public string GetValueByIndex(int iIndex)
    {
        if (iIndex >= 0 && iIndex < contents.Length)
        {
            return contents[iIndex];
        }
        return "";
    }
}

public class UIPropertyAttribute : Attribute
{
    public string name;
    public string formatString;
    public bool bPostive;

    public UIPropertyAttribute(string name, string formatString, bool bPostive = true)
    {
        this.name = name;
        this.formatString = formatString;
        this.bPostive = bPostive;
    }
}

public class UIFrameSound : Attribute
{
    public string name;
    public string sound;
    public bool bNeed;
    public UIFrameSound(string name,string sound = "Sound/SE/click1")
    {
        this.name = name;
        this.sound = sound;
        this.bNeed = true;
    }

    public UIFrameSound(string name,bool bNeed)
    {
        this.name = name;
        this.sound = "Sound/SE/click1";
        this.bNeed = bNeed;
    }

    public void OnPlaySound()
    {
        if(bNeed)
        {
            //AudioManager.instance.PlaySound(sound, AudioType.AudioEffect);
        }
    }
}

//用于标记此属性需要注入事件响应函数
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class UIEventSetterAttribute : Attribute
{

}

public class ClientSystemCreateAttribute : Attribute
{
}
