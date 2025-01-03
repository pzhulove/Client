using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Xml;
using UnityEngine;

static class TR
{
    public enum EType
    {
        CN,
        EN,
    }

    static Dictionary<string, string> ms_table = new Dictionary<string, string>();

    static string m_internationalRes = "Data/Language/International.xml";

    static public bool Initialize(EType type)
    {
        Clear();

        string xml = (AssetLoader.instance.LoadRes(m_internationalRes).obj as TextAsset).text;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        string path = "International/" + Enum.GetName(typeof(TR.EType), type);

        XmlNode root = xmlDoc.SelectSingleNode(path);
        if (root != null)
        {
            for (int i = 0; i < root.ChildNodes.Count; ++i)
            {
                XmlNode node = root.ChildNodes[i];
                if (node.LocalName == "T")
                {
                    XmlElement element = node as XmlElement;
                    string key = element.GetAttribute("Key");
                    if (ms_table.ContainsKey(key) == false)
                    {
                        ms_table.Add(key, element.GetAttribute("Value"));
                    }
                    else
                    {
                        Logger.LogErrorFormat("initialize tr data error!! key:{0} repeated!!", key);
                    }
                }
            }
            Logger.LogProcessFormat("initialize tr data success!! file path:{0}", m_internationalRes);
            return true;
        }
        Logger.LogErrorFormat("initialize tr data error!! file path:{0}", m_internationalRes);
        return false;
    }

    static public void Clear()
    {
        ms_table.Clear();
    }

    static public string Value(string key)
    {
        string value;
        if (ms_table.TryGetValue(key, out value))
        {
            return value.Replace("\\n", "\n");
        }
        else
        {
            Logger.LogErrorFormat("TR can not find key:{0}", key);
            return key;
        }
    }

    static public string Value(string key, object arg0)
    {
        string value;
        if (ms_table.TryGetValue(key, out value))
        {
            return string.Format(value, arg0).Replace("\\n", "\n");
        }
        else
        {
            Logger.LogErrorFormat("TR can not find key:{0}", key);
            return key;
        }
    }

    static public string Value(string key, object arg0, object arg1)
    {
        string value;
        if (ms_table.TryGetValue(key, out value))
        {
            return string.Format(value, arg0, arg1);
        }
        else
        {
            Logger.LogErrorFormat("TR can not find key:{0}", key);
            return key;
        }
    }

    static public string Value(string key, object arg0, object arg1, object arg2)
    {
        string value;
        if (ms_table.TryGetValue(key, out value))
        {
            return string.Format(value, arg0, arg1, arg2);
        }
        else
        {
            Logger.LogErrorFormat("TR can not find key:{0}", key);
            return key;
        }
    }

    static public string Value(string key, object arg0, object arg1, object arg2, object arg3)
    {
        string value;
        if (ms_table.TryGetValue(key, out value))
        {
            return string.Format(value, arg0, arg1, arg2, arg3);
        }
        else
        {
            Logger.LogErrorFormat("TR can not find key:{0}", key);
            return key;
        }
    }

    static public string Value(string key, object arg0, object arg1, object arg2, object arg3, object arg4)
    {
        string value;
        if (ms_table.TryGetValue(key, out value))
        {
            return string.Format(value, arg0, arg1, arg2, arg3, arg4);
        }
        else
        {
            Logger.LogErrorFormat("TR can not find key:{0}", key);
            return key;
        }
    }

    static public string Value(string key, object[] args)
    {
        string value;
        if (ms_table.TryGetValue(key, out value))
        {
            return string.Format(value, args);
        }
        else
        {
            Logger.LogErrorFormat("TR can not find key:{0}", key);
            return key;
        }
    }
}

