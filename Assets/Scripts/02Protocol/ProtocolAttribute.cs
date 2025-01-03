using System;
using System.Collections.Generic;
#if UNITY_EDITOR 
using System.Linq;
#endif

[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
sealed class ProtocolAttribute : System.Attribute
{
    public ProtocolAttribute ()
    {
        
    }
}


[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
sealed class ProtocolIDAttribute : System.Attribute
{
 
    readonly string _IDString;
    
    // This is a positional argument
    public ProtocolIDAttribute (string IDString)
    {
        this._IDString = IDString;
    }
     
    // This is a named argument
    public string IDString 
    { 
        get
        {
            return _IDString;
        }
        
    }
}

class ProtocolHelper : Singleton<ProtocolHelper>
{
    Dictionary<UInt32, string> m_id2name = new Dictionary<uint, string>();

    static public string ID2Name(UInt32 id)
    {
        return ProtocolHelper.instance.GetName(id);
    }
    
    public string GetName(UInt32 id)
    {
        string name;
        if (m_id2name.TryGetValue(id, out name))
        {
            return name;
        }

        return "unknown(" + id.ToString() + ")";
    }

    public override void Init()
    {
    //只在编辑器模式下，开启消息调试
#if UNITY_EDITOR  && !LOGIC_SERVER
        var protoClasses = (from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.GetTypes()
                            where t.IsClass && t.IsDefined(typeof(ProtocolAttribute), false)
                            select t).ToArray();
        foreach (var proto in protoClasses)
        {
            var field = proto.GetField("MsgID");
            if (field != null)
            {
                UInt32 msgId = UInt32.Parse(field.GetValue(proto).ToString());
                string name = proto.ToString();
                Register(msgId, name);
            }
        }
#endif
    }

    void Register(UInt32 id, string name)
    {
        if(m_id2name.ContainsKey(id))
        {
            Logger.LogErrorFormat("duplicate protocol id:" + id);
            return;
        }
        m_id2name.Add(id, name);
    }
}