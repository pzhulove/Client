using System;
using System.Collections.Generic;
using UnityEngine;
//Created Time : 2020-3-5
//Created By ; shensi
//Description: 自定义gameobject池管理
//用于不符合通用引擎规则的gameobject获取与释放，以内存换时间获得最快加载时间
//用法和类名的名称一致，手动调用Clear释放，否则容易产生泄漏
//清楚你的需求，并且明白调用Get 和recycle ，clear时机 
public class ManualPoolCollector : MonoSingleton<ManualPoolCollector>
{

    Dictionary<string, List<GameObject>> m_pools = new Dictionary<string, List<GameObject>>();
    class GameObjectDesc
    {
        public  string path;
        public GameObject go;
    };
    Dictionary<int, GameObjectDesc> m_IdToGoMap = new Dictionary<int, GameObjectDesc>();
    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
    }
    public GameObject GetGameObject(string path)
    {
        List<GameObject> pool = null;
        if (!m_pools.ContainsKey(path))
        {
            m_pools.Add(path, new List<GameObject>());
        }
        pool = m_pools[path];
        GameObject go = null;
        if(pool.Count > 0)
        {
            go = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            if (go.IsNull()) return go;
            if (m_IdToGoMap.ContainsKey(go.GetInstanceID()))
            {
                m_IdToGoMap[go.GetInstanceID()].go = null;
            }
            else
            {
                m_IdToGoMap.Add(go.GetInstanceID(), new GameObjectDesc
                {
                    path = path,
                    go = null
                });
            }
           
        }
        else
        {
            go = AssetLoader.instance.LoadResAsGameObject(path);
            if (go.IsNull()) return go;
            if (m_IdToGoMap.ContainsKey(go.GetInstanceID()))
            {
                m_IdToGoMap[go.GetInstanceID()].go = null;
            }
            else
            {
                m_IdToGoMap.Add(go.GetInstanceID(), new GameObjectDesc
                {
                    path = path,
                    go = null
                });
            }
        }
        go.transform.SetParent(null, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.parent = null;
        go.SetActive(true);
        return go;
    }
    public void Recycle(GameObject go)
    {
        if (go.IsNull()) return;
        if (!m_IdToGoMap.ContainsKey(go.GetInstanceID()))
        {
            Logger.LogErrorFormat("has not instanceId record {0}", go.GetInstanceID());
            GameObject.Destroy(go);
            return;
        }
        string path = m_IdToGoMap[go.GetInstanceID()].path;
        m_IdToGoMap[go.GetInstanceID()].go = go;
        if (!m_pools.ContainsKey(path))
        {
            m_pools.Add(path, new List<GameObject>());
               
        }
        m_pools[path].Add(go);
        go.SetActive(false);
        go.transform.SetParent(gameObject.transform, false);
    }
    public void Clear()
    {
        var iter = m_IdToGoMap.GetEnumerator();
        while(iter.MoveNext())
        {
            var goDesc = iter.Current.Value;
            if (goDesc == null || goDesc.go.IsNull()) continue;
            GameObject.Destroy(goDesc.go);
        }
        m_IdToGoMap.Clear();
        m_pools.Clear();
    }
}

