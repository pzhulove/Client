using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class ComCommonBind : MonoBehaviour 
{

    public enum eBindUnitType
    {
        Component,
        GameObject,
    }

    [Serializable]
    public struct BindUnit
    {
        public string        tag;
        public Component     com;
        public eBindUnitType type;
    }

    [Serializable]
    public struct AttachPrefabPath
    {
        public string    tag;
        public string    path;
    }


    [Serializable]
    public struct AttachResource 
    {
        public string  tag;
        public Sprite  sprite;
        public Material material;
    }

    public AttachResource[]   reses     = new AttachResource[0];
    public BindUnit[]         units     = new BindUnit[0];
    public AttachPrefabPath[] prefabs   = new AttachPrefabPath[0];

    public static ComCommonBind LoadBind(string path)
    {
        GameObject go = AssetLoader.instance.LoadResAsGameObject(path);

        if (null != go)
        {
            ComCommonBind bind = go.GetComponent<ComCommonBind>();

            if (null != bind)
            {
                Logger.LogProcessFormat("[loadbind] 加载 {0} 成功", path);
                return bind;
            }
        }

        return null;
    }

    private Dictionary<string, List<ComCommonBind>> mAllCacheBinds = new Dictionary<string, List<ComCommonBind> >();

    public ComCommonBind LoadExtraBind(string path)
    {
        ComCommonBind bind = LoadBind(path);

        if (null != bind)
        {
            if (!mAllCacheBinds.ContainsKey(path))
            {
                mAllCacheBinds.Add(path, new List<ComCommonBind>());
            }

            mAllCacheBinds[path].Add(bind);
        }

        return bind;
    }

    public ComCommonBind GetExistBind(string path, int idx)
    {
        if (mAllCacheBinds.ContainsKey(path))
        {
            if (idx > 0 && idx < mAllCacheBinds[path].Count)
            {
                return mAllCacheBinds[path][idx];
            }
        }

        return null;
    }

    public void ClearAllCacheBinds()
    {
        var iter = mAllCacheBinds.GetEnumerator();

        while (iter.MoveNext())
        {
            ClearCacheBinds(iter.Current.Key);
        }

        mAllCacheBinds.Clear();
    }

    public void OnDestroy()
    {
        ClearAllCacheBinds();
        Array.Clear(reses,0,reses.Length);
        Array.Clear(units,0,units.Length);
        Array.Clear(prefabs,0,prefabs.Length);
        
        reses = null;
        units = null;
        prefabs = null;
        mAllCacheBinds = null;
    }

    private void _clearAllCallback(Component com)
    {
        if (com is Button)
        {
            Button bt = com as Button;
            if (null != bt)
            {
                bt.onClick.RemoveAllListeners();
            }
        }
        else if (com is Toggle)
        {
            Toggle tg = com as Toggle;
            if (null != tg)
            {
                tg.onValueChanged.RemoveAllListeners();
            }
        }
        else if (com is Dropdown)
        {
            Dropdown tg = com as Dropdown;
            if (null != tg)
            {
                tg.onValueChanged.RemoveAllListeners();
            }
        }
    }

    public void ClearCacheBinds(string path)
    {
        if (mAllCacheBinds.ContainsKey(path))
        {
            List<ComCommonBind> list = mAllCacheBinds[path];

            for (int i = 0; i < list.Count; ++i)
            {
                for (int j = 0; j < list[i].units.Length; ++j)
                {
                    _clearAllCallback(list[i].units[j].com);
                }

                list[i].ClearAllCacheBinds();
                GameObject.Destroy(list[i].gameObject);
                list[i] = null;
            }

            list.Clear();
        }
    }

    //public Sprite GetSprite(string name)
    //{
    //    for (int i = 0; i < reses.Length; ++i)
    //    {
    //        if (reses[i].tag.Equals(name))
    //        {
    //            return reses[i].sprite;
    //        }
    //    }

    //    return null;
    //}

    public void GetSprite(string name, ref Image image)
    {
        for (int i = 0; i < reses.Length; ++i)
        {
            if (reses[i].tag.Equals(name))
            {
                // return reses[i].sprite;
                image.sprite = reses[i].sprite;
                image.material = reses[i].material;
            }
        }
    }


    public string GetPrefabPath(string name)
    {
        for (int i = 0; i < prefabs.Length; ++i)
        {
            if (prefabs[i].tag.Equals(name))
            {
                return prefabs[i].path;
            }
        }

        return null;
    }

    public GameObject GetPrefabInstance(string name)
    {
        string path = GetPrefabPath(name);
        return AssetLoader.instance.LoadResAsGameObject(path);
    }

    public GameObject GetGameObject(string name)
    {
        for (int i = 0; i < units.Length; ++i)
        {
            if (units[i].tag.Equals(name))
            {
                if (null != units[i].com)
                {
                    return units[i].com.gameObject;
                }

            }
        }

        return null;
    }

    public Component GetCom(Type type, string name)
    {
        for (int i = 0; i < units.Length; ++i)
        {
            if (units[i].com.GetType() == type &&
                (units[i].tag.Equals(name)))
            {
                return units[i].com as Component;
            }
        }

        return null;
    }

    public T GetCom<T>(string name) where T : Component
    {
        for (int i = 0; i < units.Length; ++i)
        {
            if ((units[i].com is T) &&
                (units[i].tag.Equals(name)))
            {
                return units[i].com as T;
            }
        }

        return null;
    }
}
