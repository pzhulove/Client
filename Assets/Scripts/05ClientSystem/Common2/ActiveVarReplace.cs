using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Text;

namespace GameClient
{
    [System.Serializable]
    public class KeyValueInfo
    {
        public string Content;
        public Text text;
        public Image img;
        public bool bFromCount = false;
    }

    public enum ActiveVarRelpaceType
    {
        AVRT_NORMAL = 0,
        AVRT_ITEM_NAME = 1,
        AVRT_ITEM_ICON = 2,
        AVRT_SYSTEM_VALUE_ID = 3,
        AVRT_SYSTEM_VALUE_ITEM_NAME = 4,
    }

    public enum ZoneType
    {
        ZT_ENABLE = (1 << 31),
        ZT_IMMEDIATELY = (1 << 0),
        ZT_FROM_ACTIVE_KEY = (1 << 1),
        ZT_FROM_COUNT = (1 << 2),
        ZT_FROM_SYSTEM_VALUE = (1 << 3),
        ZF_MASK = 0xFF,
    }

    public class ActiveVarReplace : MonoBehaviour
    {
        public KeyValueInfo[] keyValues = new KeyValueInfo[0];
        public int iActiveID;
        static Regex ms_regex_key_replace = new Regex(@"{key=([a-zA-Z0-9]+) type=([0-9]+) z=([0-9-]+) l=([a-zA-Z0-9]+) h=([a-zA-Z0-9]+)}", RegexOptions.Singleline);

        // Use this for initialization
        void Start()
        {
            ActiveManager.GetInstance().onAddMainActivity += _OnAddMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity += _OnRemoveMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity += _OnUpdateMainActivity;
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;

            _UpdateValue();
        }

        int _GetActiveKeyValue(string key,int iDef = 0)
        {
            int iQueryValue = iDef;
            var activityData = ActiveManager.GetInstance().GetChildActiveData(iActiveID);
            if (null != activityData)
            {
                var find = activityData.akActivityValues.Find(x => { return x.key == key; });
                if (null != find && int.TryParse(find.value, out iQueryValue))
                {

                }
            }
            return iQueryValue;
        }

        void _UpdateValue()
        {
            StringBuilder kBuilder = StringBuilderCache.Acquire();
            for(int i = 0; i < keyValues.Length; ++i)
            {
                var keyValue = keyValues[i];
                if(null == keyValue)
                {
                    continue;
                }

                kBuilder.Clear();

                var matches = ms_regex_key_replace.Matches(keyValue.Content);
                int indexT = 0;
                for(int j = 0; j < matches.Count; ++j)
                {
                    if(matches[j].Success)
                    {
                        kBuilder.Append(keyValue.Content.Substring(indexT, matches[j].Index - indexT));

                        ActiveVarRelpaceType eActiveVarRelpaceType = (ActiveVarRelpaceType)int.Parse(matches[j].Groups[2].Value);
                        ActiveVarRelpaceType eZoneFrom = (ActiveVarRelpaceType)int.Parse(matches[j].Groups[3].Value);
                        int iZone = int.Parse(matches[j].Groups[3].Value);
                        bool bZone = (iZone & (int)ZoneType.ZT_ENABLE) != 0;

                        int iLow = 0;
                        int iHigh = 0;
                        if (bZone)
                        {
                            iLow = _QueryValueByMask(matches[j].Groups[4].Value, (iZone & (int)ZoneType.ZF_MASK), 0);
                            iHigh = _QueryValueByMask(matches[j].Groups[5].Value, ((iZone >> 8) & (int)ZoneType.ZF_MASK), 0);
                            //Logger.LogErrorFormat("key = {0} low = {1} high = {2}", matches[j].Groups[1].Value, iLow, iHigh);
                        }

                        int iQueryValue = _QueryValue(matches[j].Groups[1].Value, keyValue.bFromCount, eActiveVarRelpaceType);
                        if(bZone)
                        {
                            iQueryValue = IntMath.Max(iQueryValue, iLow);
                            iQueryValue = IntMath.Min(iQueryValue, iHigh);
                        }

                        switch (eActiveVarRelpaceType)
                        {
                            case ActiveVarRelpaceType.AVRT_ITEM_NAME:
                                {
                                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iQueryValue);
                                    if (null != item)
                                    {
                                        kBuilder.Append(item.Name);
                                    }
                                }
                                break;
                            case ActiveVarRelpaceType.AVRT_NORMAL:
                                kBuilder.Append(iQueryValue.ToString());
                                    break;
                            case ActiveVarRelpaceType.AVRT_ITEM_ICON:
                                {
                                    var imgItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iQueryValue);
                                    if (null != imgItem)
                                    {
                                        kBuilder.Append(imgItem.Icon);
                                    }
                                }
                                break;
                            case ActiveVarRelpaceType.AVRT_SYSTEM_VALUE_ID:
                                {
                                    var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>(iQueryValue);
                                    if (null != systemValue)
                                    {
                                        kBuilder.Append(systemValue.Value);
                                    }
                                    else
                                    {
                                        kBuilder.Append(0);
                                    }
                                }
                                break;
                            case ActiveVarRelpaceType.AVRT_SYSTEM_VALUE_ITEM_NAME:
                                {
                                    var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>(iQueryValue);
                                    if (null != systemValue)
                                    {
                                        var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(systemValue.Value);
                                        if (null != item)
                                        {
                                            kBuilder.Append(item.Name);
                                        }
                                    }
                                }
                                break;
                        }

                        indexT = matches[j].Index + matches[j].Length;
                    }
                }
                kBuilder.Append(keyValue.Content.Substring(indexT, keyValue.Content.Length - indexT));

                if(keyValue.text != null)
                {
                    keyValue.text.text = kBuilder.ToString();
                }
                if(keyValue.img != null)
                {
                    // keyValue.img.sprite = AssetLoader.instance.LoadRes(kBuilder.ToString(), typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref keyValue.img, kBuilder.ToString());
                }
            }
            StringBuilderCache.Release(kBuilder);
        }

        int _QueryValueByMask(string key,int zone,int iDef)
        {
            int iQueryValue = iDef;
            if((zone & (int)ZoneType.ZT_IMMEDIATELY) != 0)
            {
                int.TryParse(key, out iQueryValue);
            }
            else if ((zone & (int)ZoneType.ZT_FROM_ACTIVE_KEY) != 0)
            {
                iQueryValue = _GetActiveKeyValue(key, iQueryValue);
            }
            else if ((zone & (int)ZoneType.ZT_FROM_COUNT) != 0)
            {
                var countInfo = CountDataManager.GetInstance().GetCountInfo(key);
                if (null != countInfo)
                {
                    iQueryValue = (int)countInfo.value;
                }
            }
            else if ((zone & (int)ZoneType.ZT_FROM_SYSTEM_VALUE) != 0)
            {
                int iSystemValueID = 0;
                if(int.TryParse(key, out iSystemValueID))
                {
                    var systemValueItem = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>(iSystemValueID);
                    if(null != systemValueItem)
                    {
                        iQueryValue = systemValueItem.Value;
                    }
                }
            }
            return iQueryValue;
        }

        int _QueryValue(string key,bool bFromCount, ActiveVarRelpaceType eActiveVarRelpaceType)
        {
            int iQueryValue = 0;
            switch (eActiveVarRelpaceType)
            {
                case ActiveVarRelpaceType.AVRT_ITEM_ICON:
                case ActiveVarRelpaceType.AVRT_ITEM_NAME:
                case ActiveVarRelpaceType.AVRT_NORMAL:
                    {
                        if (bFromCount)
                        {
                            var countInfo = CountDataManager.GetInstance().GetCountInfo(key);
                            if (null != countInfo)
                            {
                                iQueryValue = (int)countInfo.value;
                            }
                        }
                        else
                        {
                            iQueryValue = _GetActiveKeyValue(key, iQueryValue);
                        }
                    }
                    break;
                case ActiveVarRelpaceType.AVRT_SYSTEM_VALUE_ID:
                    int.TryParse(key, out iQueryValue);
                    break;
                case ActiveVarRelpaceType.AVRT_SYSTEM_VALUE_ITEM_NAME:
                    {
                        int.TryParse(key, out iQueryValue);
                    }
                    break;
            }
            return iQueryValue;
        }

        void _OnAddMainActivity(ActiveManager.ActiveData data)
        {
            _UpdateValue();
        }

        void _OnRemoveMainActivity(ActiveManager.ActiveData data)
        {
            _UpdateValue();
        }

        void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            if(data.activeItem.ID == iActiveID)
            {
                _UpdateValue();
            }
        }

        void _OnUpdateMainActivity(ActiveManager.ActiveData data)
        {
            _UpdateValue();
        }

        void OnDestroy()
        {
            ActiveManager.GetInstance().onAddMainActivity -= _OnAddMainActivity;
            ActiveManager.GetInstance().onUpdateMainActivity -= _OnUpdateMainActivity;
            ActiveManager.GetInstance().onRemoveMainActivity -= _OnRemoveMainActivity;
            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;
        }
    }
}