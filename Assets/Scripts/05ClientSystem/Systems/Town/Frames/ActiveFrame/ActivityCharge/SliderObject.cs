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
    public class SliderObject
    {
        public static Regex kvRegex = new Regex(@"<slider=([A-Za-z0-9/]+) k=(\w+) v=([A-Za-z0-9]*) mode=(\d+)>", RegexOptions.Singleline);
        public Slider slider;
        public KeyValuePairObject.KVMode eKVMode;
        public string key;
        public string v;
    }
}
