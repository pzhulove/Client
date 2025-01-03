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
    public class KeyValuePairObject
    {
        //<path=Title value=累计充值replace=<k= v= mode=0 ec= dc=>元>
        public static Regex kvRegex = new Regex(@"<path=([A-Za-z0-9/]+) value=(.+)>", RegexOptions.Singleline);
        public static Regex kvContent = new Regex(@"replace=<k=(\w+) v=([A-Za-z0-9]*) mode=(\d+) ec=(#[A-Fa-f0-9]*) dc=(#[A-Fa-f0-9]*)>", RegexOptions.Singleline);
        public enum KVMode
        {
            KVM_KV = 0,
            KVM_KK,
            KVM_REPLACE,
        }

        public KeyValuePairObject()
        {
            kPreContent = null;
            kAftContent = null;
            text = null;
            key = null;
            v = null;
            eKVMode = KVMode.KVM_KV;
            bHasColor = false;
        }

        public string kPreContent;
        public string kAftContent;
        public Text text;
        public string key;
        public string v;
        public KVMode eKVMode;
        public string enableColor;
        public string disableColor;
        public bool bHasColor;
    }


}
