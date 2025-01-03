using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;

namespace GameClient
{
    public class RedPointObject
    {
        public static Regex ms_red_point = new Regex(@"<path=([A-Za-z/]+) key=([A-Za-z\|]+)");
        public enum RedPointType
        {
            RPT_LOCAL = 0,
            RPT_GLOBAL,
            RPT_COUNT,
        }
        public ActiveSpecialRedBinder redBinder = null;
        RedPointType eRedPointType = RedPointType.RPT_COUNT;
        int iMark = 0;
        string path = null;
        List<string> keys = new List<string>();
        public List<string> Keys
        {
            get { return keys; }
        }

        GameObject goCurrent = null;
        public GameObject Current
        {
            get { return goCurrent; }
        }

        public static List<RedPointObject> Create(string value, GameObject goLocal)
        {
            List<RedPointObject> ret = null;
            foreach (Match match in ms_red_point.Matches(value))
            {
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    RedPointObject current = new RedPointObject();
                    current.path = match.Groups[1].Value;
                    current.goCurrent = Utility.FindChild(goLocal, current.path);
                    current.redBinder = current.goCurrent.GetComponent<ActiveSpecialRedBinder>();

                    if (current.goCurrent != null)
                    {
                        var keys = match.Groups[2].Value.Split('|');
                        for (int i = 0; i < keys.Length; ++i)
                        {
                            if (!string.IsNullOrEmpty(keys[i]))
                            {
                                current.keys.Add(keys[i]);
                            }
                        }
                        if (ret == null)
                        {
                            ret = new List<RedPointObject>();
                        }
                        ret.Add(current);
                    }
                }
            }
            return ret;
        }
    }

}
