using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectLeakDitector
{

    class ObjRefDesc
    {
        public int m_AllocTotal = 0;
        public int m_DestroyTotal = 0;
    }
    static private Dictionary<System.Type, ObjRefDesc> sm_ObjectRefMap = new Dictionary<System.Type, ObjRefDesc>();

    static public void DumpObjectRef()
    {
#if UNITY_EDITOR
        FileStream stream = new FileStream("ObjRefDescDump.csv", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        StreamWriter sw = new StreamWriter(stream);
        stream.Seek(0, SeekOrigin.Begin);
        Dictionary<string, float> resSizeMap = new Dictionary<string, float>();

        Dictionary<System.Type, ObjRefDesc>.Enumerator it = sm_ObjectRefMap.GetEnumerator();
        while(it.MoveNext())
        {
            KeyValuePair<System.Type, ObjRefDesc> cur = it.Current;
            string line = string.Format("{0},{1},{2}", cur.Key, cur.Value.m_AllocTotal, cur.Value.m_DestroyTotal);
            sw.WriteLine(line);
        }

        stream.Flush();
        sw.Close();
        stream.Close();
#endif
    }

    static void AddObjectRef(System.Type type)
    {
        ObjRefDesc objRefDesc = null;
        if (!sm_ObjectRefMap.TryGetValue(type, out objRefDesc))
        {
            objRefDesc = new ObjRefDesc();
            sm_ObjectRefMap.Add(type, objRefDesc);
        }

        if (null != objRefDesc)
            ++objRefDesc.m_AllocTotal;
        else
            Logger.LogError("### 严重错误，添加类型记录失败！！");
    }

    static void RemoveObjectRef(System.Type type)
    {
        ObjRefDesc objRefDesc = null;
        if (sm_ObjectRefMap.TryGetValue(type, out objRefDesc))
            ++objRefDesc.m_DestroyTotal;
        else
            Logger.LogError("### 严重错误，没有找到匹配类型的添加记录！！");
    }

    public ObjectLeakDitector()
    {
#if UNITY_EDITOR
        AddObjectRef(GetType());
#endif
    }

    ~ObjectLeakDitector()
    {
#if UNITY_EDITOR
        RemoveObjectRef(GetType());
#endif
    }
}
