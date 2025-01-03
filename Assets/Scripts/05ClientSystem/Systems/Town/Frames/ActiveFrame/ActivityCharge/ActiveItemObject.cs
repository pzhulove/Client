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
    public class ActiveItemObject : CachedObject
    {
        enum ActiveKeyType
        {
            AKT_KEY,//只是替换KEY
            AKT_KEY_VALUE,//替换KEY/VALUE 并判断KEY与VALUE大小，决定文字色
            AKT_KEY_KEY,//替换KEY/VALUE 并判断KEY与VALUE大小，决定文字色
            AKT_COUNT,
        }
        static Regex[] ms_missionkey_regex = new Regex[(int)ActiveKeyType.AKT_COUNT]
            {
            new Regex(@"<key>key=(\w+)</key>"),
            new Regex(@"<key>key=(\w+)/value=(\d+)</key>"),
            new Regex(@"<key>key=(\w+)/key=(\w+)</key>"),
            };
        static Regex s_regex_content = new Regex(@"<keyname=(\w+) valuegroup=(.+)>", RegexOptions.Singleline);
        static Regex s_regex = new Regex(@"<prefabkey=(\w+) type=(\d+) key=(\w+) value=([A-Za-z0-9_/]+)>", RegexOptions.Singleline);
        static Regex s_award = new Regex(@"<prefabkey=(\w+) key=(\d+) value=(.+)>", RegexOptions.Singleline);
        static Regex s_dynamic_award = new Regex(@"<KeyNum=(\w+) KeyId=(\w+) KeySize=(\w+)>", RegexOptions.Singleline);

        enum InitializeKeyType
        {
            IKT_SHOW = 0,
            IKT_VALUE,
            IKT_COUNT,
        }
        static Regex[] ms_initkeys = new Regex[(int)InitializeKeyType.IKT_COUNT]
        {
            new Regex(@"<prefabkey=(\w+) localpath=([A-Za-z0-9/]+)>"),
            new Regex(@"<prefabkey=(\w+) localpath=([A-Za-z0-9/]+) type=(\w+) value=(.+)>"),
        };

        static Regex s_init_key0 = new Regex(@"<localpath=(\w+)>", RegexOptions.Singleline);
        public static StringBuilder ms_kStringBuilder;
        public static void Clear()
        {
            if (ms_kStringBuilder != null)
            {
                StringBuilderCache.Release(ms_kStringBuilder);
                ms_kStringBuilder = null;
            }
        }

        protected GameObject goLocal;
        protected GameObject goParent;
        protected GameObject goPrefab;
        protected GameObject goAwardParent;
        protected string localKey;//预制体名称KEY
        protected LevelFullControl comLevelFull;

        ActiveManager.ActivityData activeItem;
        public ActiveManager.ActivityData ActiveItem
        {
            get { return activeItem; }
        }

        ActiveManager.ActiveData activeData;
        Dictionary<string, ValueObject> m_akKey2ValueGroup = new Dictionary<string, ValueObject>();
        ActiveChargeFrame THIS = null;

        //ActiveBindRecords m_kActiveBindRecords = null;
        void _InitVarBinder()
        {
            //m_kActiveBindRecords = goLocal.GetComponent<ActiveBindRecords>();
        }

        #region keyValuePair
        List<KeyValuePairObject> m_akKeyValuePairObjects = new List<KeyValuePairObject>();
        List<SliderObject> m_akSliders = new List<SliderObject>();
        List<FullLevelObject> m_akFullObjects = new List<FullLevelObject>();

        void _InitKeyValuePair()
        {
            m_akKeyValuePairObjects.Clear();
            m_akSliders.Clear();

            if (!string.IsNullOrEmpty(activeItem.activeItem.UpdateDesc))
            {
                var mulDesc = activeItem.activeItem.UpdateDesc.Split(new char[] { '\r', '\n' });
                for (int i = 0; i < mulDesc.Length; ++i)
                {
                    if (string.IsNullOrEmpty(mulDesc[i]))
                    {
                        continue;
                    }

                    Match matchFirst = KeyValuePairObject.kvRegex.Match(mulDesc[i]);
                    if (!string.IsNullOrEmpty(matchFirst.Groups[0].Value))
                    {
                        Match matchSecond = KeyValuePairObject.kvContent.Match(matchFirst.Groups[2].Value);
                        if (string.IsNullOrEmpty(matchSecond.Groups[0].Value))
                        {
                            continue;
                        }

                        Text text = Utility.FindComponent<Text>(goLocal, matchFirst.Groups[1].Value);
                        if (text == null)
                        {
                            continue;
                        }

                        KeyValuePairObject kvObject = new KeyValuePairObject();
                        kvObject.text = text;
                        kvObject.key = matchSecond.Groups[1].Value;
                        kvObject.v = matchSecond.Groups[2].Value;
                        kvObject.eKVMode = (KeyValuePairObject.KVMode)int.Parse(matchSecond.Groups[3].Value);

                        kvObject.kPreContent = matchFirst.Groups[2].Value.Substring(0, matchSecond.Index);
                        kvObject.kAftContent = matchFirst.Groups[2].Value.Substring(matchSecond.Index + matchSecond.Length, matchFirst.Groups[2].Value.Length - (matchSecond.Index + matchSecond.Length));

                        if (!string.IsNullOrEmpty(matchSecond.Groups[4].Value) && !string.IsNullOrEmpty(matchSecond.Groups[5].Value))
                        {
                            kvObject.enableColor = matchSecond.Groups[4].Value;
                            kvObject.disableColor = matchSecond.Groups[5].Value;
                            kvObject.bHasColor = true;
                        }
                        m_akKeyValuePairObjects.Add(kvObject);
                        continue;
                    }

                    matchFirst = SliderObject.kvRegex.Match(mulDesc[i]);
                    if (!string.IsNullOrEmpty(matchFirst.Groups[0].Value))
                    {
                        Slider slider = Utility.FindComponent<Slider>(goLocal, matchFirst.Groups[1].Value);
                        if (slider == null)
                        {
                            continue;
                        }
                        SliderObject kvObject = new SliderObject();
                        kvObject.slider = slider;
                        kvObject.key = matchFirst.Groups[2].Value;
                        kvObject.v = matchFirst.Groups[3].Value;
                        kvObject.eKVMode = (KeyValuePairObject.KVMode)int.Parse(matchFirst.Groups[4].Value);
                        m_akSliders.Add(kvObject);
                        continue;
                    }

                    matchFirst = FullLevelObject.kvRegex.Match(mulDesc[i]);
                    if (matchFirst.Success)
                    {
                        GameObject gameObject = Utility.FindChild(goLocal, matchFirst.Groups[1].Value);
                        if (null != gameObject)
                        {
                            FullLevelObject fullLevelObject = new FullLevelObject();
                            fullLevelObject.gameObject = gameObject;
                            m_akFullObjects.Add(fullLevelObject);
                        }
                        continue;
                    }
                }
            }
        }

        void _UpdateKeyValuePair()
        {
            for (int i = 0; i < m_akKeyValuePairObjects.Count; ++i)
            {
                var current = m_akKeyValuePairObjects[i];
                if (current == null)
                {
                    continue;
                }
                switch (current.eKVMode)
                {
                    case KeyValuePairObject.KVMode.KVM_KV:
                        {
                            int iK = ActiveManager.GetInstance().GetActiveItemValue(activeItem.activeItem.ID, current.key);
                            int iV = 0;
                            if (int.TryParse(current.v, out iV))
                            {
                                if (current.bHasColor)
                                {
                                    current.text.text = string.Format("{1}<color={0}>{2}/{3}</color>{4}",
                                        iK >= iV ? current.enableColor : current.disableColor, current.kPreContent, iK, iV, current.kAftContent);
                                }
                                else
                                {
                                    current.text.text = current.kPreContent + iK + "/" + iV + current.kAftContent;
                                }
                            }
                        }
                        break;
                    case KeyValuePairObject.KVMode.KVM_KK:
                        {
                            int iK = ActiveManager.GetInstance().GetActiveItemValue(activeItem.activeItem.ID, current.key);
                            int iV = ActiveManager.GetInstance().GetActiveItemValue(activeItem.activeItem.ID, current.v);
                            if (current.bHasColor)
                            {
                                current.text.text = string.Format("{1}<color={0}>{2}/{3}</color>{4}",
                                    iK >= iV ? current.enableColor : current.disableColor, current.kPreContent, iK, iV, current.kAftContent);
                            }
                            else
                            {
                                current.text.text = current.kPreContent + iK + "/" + iV + current.kAftContent;
                            }
                        }
                        break;
                    case KeyValuePairObject.KVMode.KVM_REPLACE:
                        {
                            int iK = ActiveManager.GetInstance().GetActiveItemValue(activeItem.activeItem.ID, current.key);
                            if (current.bHasColor)
                            {
                                current.text.text = string.Format("{0}<color={1}>{2}</color>{3}", current.kPreContent, current.enableColor, iK, current.kAftContent);
                            }
                            else
                            {
                                current.text.text = current.kPreContent + iK + current.kAftContent;
                            }
                        }
                        break;
                }
            }

            for (int i = 0; i < m_akSliders.Count; ++i)
            {
                var current = m_akSliders[i];
                if (current == null)
                {
                    continue;
                }
                switch (current.eKVMode)
                {
                    case KeyValuePairObject.KVMode.KVM_KV:
                        {
                            int iK = ActiveManager.GetInstance().GetActiveItemValue(activeItem.activeItem.ID, current.key);
                            int iV = 0;
                            if (int.TryParse(current.v, out iV))
                            {

                            }

                            if (iV <= 0)
                            {
                                iV = 1;
                            }
                            current.slider.value = Mathf.Clamp01(iK * 1.0f / iV);
                        }
                        break;
                    case KeyValuePairObject.KVMode.KVM_KK:
                        {
                            int iK = ActiveManager.GetInstance().GetActiveItemValue(activeItem.activeItem.ID, current.key);
                            int iV = ActiveManager.GetInstance().GetActiveItemValue(activeItem.activeItem.ID, current.v);
                            if (iV <= 0)
                            {
                                iV = 1;
                            }
                            current.slider.value = Mathf.Clamp01(iK * 1.0f / iV);
                        }
                        break;
                    case KeyValuePairObject.KVMode.KVM_REPLACE:
                        {
                            Logger.LogErrorFormat("MODE ERROR !!");
                        }
                        break;
                }
            }

            for (int i = 0; i < m_akFullObjects.Count; ++i)
            {
                m_akFullObjects[i].Update();
            }
        }
        #endregion

        public override void OnCreate(object[] param)
        {
            goParent = param[0] as GameObject;
            goPrefab = param[1] as GameObject;
            activeItem = param[2] as ActiveManager.ActivityData;
            activeData = param[3] as ActiveManager.ActiveData;
            localKey = param[4] as string;
            THIS = param[5] as ActiveChargeFrame;

            if (goLocal == null && goPrefab == null)
            {
                return;
            }

            if (goLocal == null)
            {
                goLocal = GameObject.Instantiate(goPrefab);
                Utility.AttachTo(goLocal, goParent);
                _BindKeyValuePair();
                _CreateAwards();
                _InitVarBinder();
                _InitKeyValuePair();
                comLevelFull = goLocal.GetComponent<LevelFullControl>();
            }
            goLocal.name = activeItem.activeItem.ID.ToString();

            Enable();

            _Initialize();
            _UpdateItem();
        }

        public static int[] ms_sort_order = new int[(int)Protocol.TaskStatus.TASK_OVER + 1]
            {
                    2,1,0,3,4,5
            };


        public static int Cmp(ActiveManager.ActivityData left, ActiveManager.ActivityData right)
        {
            if (left.activeItem.SortPriority != right.activeItem.SortPriority)
            {
                return left.activeItem.SortPriority - right.activeItem.SortPriority;
            }

            if (left.status != right.status)
            {
                return ms_sort_order[left.status] - ms_sort_order[right.status];
            }

            if (left.activeItem.SortPriority2 != right.activeItem.SortPriority2)
            {
                return left.activeItem.SortPriority2 - right.activeItem.SortPriority2;
            }

            return left.activeItem.ID - right.activeItem.ID;
        }

        public static int Cmp(ActiveItemObject left, ActiveItemObject right)
        {
            if (left.ActiveItem.activeItem.SortPriority != right.activeItem.activeItem.SortPriority)
            {
                return left.activeItem.activeItem.SortPriority - right.activeItem.activeItem.SortPriority;
            }

            if (left.ActiveItem.status != right.ActiveItem.status)
            {
                return ms_sort_order[left.ActiveItem.status] - ms_sort_order[right.ActiveItem.status];
            }

            if (left.ActiveItem.activeItem.SortPriority2 != right.activeItem.activeItem.SortPriority2)
            {
                return left.activeItem.activeItem.SortPriority2 - right.activeItem.activeItem.SortPriority2;
            }

            return left.ActiveItem.activeItem.ID - right.activeItem.activeItem.ID;
        }

        public override void SetAsLastSibling()
        {
            if (goLocal != null)
            {
                goLocal.transform.SetAsLastSibling();
            }
        }

        /// <summary>
        /// 创建奖励内容
        /// </summary>
        void _CreateAwards()
        {
            try
            {
                goAwardParent = null;
                if (!string.IsNullOrEmpty(activeData.mainItem.awardparent))
                {
                    var awardParents = activeData.mainItem.awardparent.Split(new char[] { '\r', '\n' });
                    for (int i = 0; i < awardParents.Length; ++i)
                    {
                        Match match = s_award.Match(awardParents[i]);
                        if (!string.IsNullOrEmpty(match.Groups[0].Value))
                        {
                            if (match.Groups[1].Value == localKey)
                            {
                                goAwardParent = Utility.FindChild(goLocal, match.Groups[3].Value);
                                break;
                            }
                        }
                    }
                }

                if (goAwardParent != null)
                {
                    bool bDynamic = false;
                    if (!string.IsNullOrEmpty(activeItem.activeItem.DanymicAwards))
                    {
                        bDynamic = true;
                        var match = s_dynamic_award.Match(activeItem.activeItem.DanymicAwards);
                        if (match != null && !string.IsNullOrEmpty(match.Groups[0].Value))
                        {
                            int iCount = ActiveManager.GetInstance().GetActiveItemValue(activeItem.activeItem.ID, match.Groups[3].Value);
                            for (int i = 0; i < iCount; ++i)
                            {
                                int iTableID = ActiveManager.GetInstance().GetActiveItemValue(activeItem.activeItem.ID, match.Groups[2].Value + (i + 1).ToString());
                                var itemData = GameClient.ItemDataManager.CreateItemDataFromTable(iTableID);
                                if (itemData == null)
                                {
                                    continue;
                                }
                                int iNum = ActiveManager.GetInstance().GetActiveItemValue(activeItem.activeItem.ID, match.Groups[1].Value + (i + 1).ToString());
                                if (iNum <= 0)
                                {
                                    continue;
                                }
                                itemData.Count = iNum;
                                var comItem = THIS.CreateComItem(goAwardParent);
                                if (comItem != null)
                                {
                                    comItem.Setup(itemData, (GameObject obj, ItemData item) =>
                                    {
                                        ItemTipManager.GetInstance().ShowTip(item);
                                    });
                                }
                            }
                        }
                        else
                        {
                            Logger.LogErrorFormat("MATCH ERROR WITH DanymicAwards ActiveID is {0}", activeItem.activeItem.ID);
                        }
                    }

                    if (!bDynamic && !string.IsNullOrEmpty(activeItem.activeItem.Awards))
                    {
                        var awards = activeItem.activeItem.Awards.Split(new char[] { ',' });
                        for (int i = 0; i < awards.Length; ++i)
                        {
                            if (!string.IsNullOrEmpty(awards[i]))
                            {
                                var substrings = awards[i].Split(new char[] { '_' });
                                if (substrings.Length == 2)
                                {
                                    int id = int.Parse(substrings[0]);
                                    int iCount = int.Parse(substrings[1]);
                                    var itemData = GameClient.ItemDataManager.CreateItemDataFromTable(id);
                                    if (itemData != null)
                                    {
                                        itemData.Count = iCount;
                                        var comItem = THIS.CreateComItem(goAwardParent);
                                        comItem.Setup(itemData, OnItemClicked);
                                    }
                                }
                                else if (substrings.Length == 4)
                                {
                                    int id = int.Parse(substrings[0]);
                                    int iCount = int.Parse(substrings[1]);
                                    int iEquipType = int.Parse(substrings[2]);
                                    int iStrengthenLevel = int.Parse(substrings[3]);
                                    var itemData = GameClient.ItemDataManager.CreateItemDataFromTable(id);
                                    if (itemData != null)
                                    {
                                        itemData.Count = iCount;
                                        itemData.EquipType = (EEquipType)iEquipType;
                                        itemData.StrengthenLevel = iStrengthenLevel;
                                        var comItem = THIS.CreateComItem(goAwardParent);
                                        comItem.Setup(itemData, OnItemClicked);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
        /// <summary>
        /// 绑定KEY-VALUE PAIR
        /// </summary>
        void _BindKeyValuePair()
        {
            try
            {
                m_akKey2ValueGroup.Clear();

                var splits = activeItem.activeItem.Desc.Split(new char[] { '|' });
                for (int i = 0; i < splits.Length; ++i)
                {
                    if (string.IsNullOrEmpty(splits[i]))
                    {
                        continue;
                    }
                    foreach (Match match in s_regex_content.Matches(splits[i]))
                    {
                        ValueObject current = new ValueObject();
                        current.kOrgValue = match.Groups[2].Value;
                        current.kKey = match.Groups[1].Value;
                        if (!m_akKey2ValueGroup.ContainsKey(current.kKey))
                        {
                            m_akKey2ValueGroup.Add(current.kKey, current);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        string _ParseText(string content)
        {
            if (ms_kStringBuilder == null)
            {
                ms_kStringBuilder = StringBuilderCache.Acquire();
            }

            ms_kStringBuilder.Clear();
            int indexText = 0;
            for (int i = 0; i < (int)ms_missionkey_regex.Length; ++i)
            {
                foreach (Match curMatch in ms_missionkey_regex[i].Matches(content))
                {
                    ms_kStringBuilder.Append(content.Substring(indexText, curMatch.Index - indexText));

                    switch ((ActiveKeyType)i)
                    {
                        case ActiveKeyType.AKT_KEY:
                            {
                                var findItem = activeItem.akActivityValues.Find(x => { return x.key == curMatch.Groups[1].Value; });
                                if (findItem != null)
                                {
                                    ms_kStringBuilder.Append(findItem.value);
                                }
                                else
                                {
                                    ms_kStringBuilder.Append("0");
                                }
                            }
                            break;
                        case ActiveKeyType.AKT_KEY_KEY:
                            {
                                var value = activeItem.akActivityValues.Find(x => { return x.key == curMatch.Groups[1].Value; });
                                var key0 = value != null ? value.value : "0";
                                value = activeItem.akActivityValues.Find(x => { return x.key == curMatch.Groups[2].Value; });
                                var key1 = value != null ? value.value : "0";
                                ms_kStringBuilder.AppendFormat("{0}/{1}", key0, key1);
                            }
                            break;
                        case ActiveKeyType.AKT_KEY_VALUE:
                            {
                                var value = activeItem.akActivityValues.Find(x => { return x.key == curMatch.Groups[1].Value; });
                                var key0 = value != null ? value.value : "0";
                                ms_kStringBuilder.AppendFormat("{0}/{1}", key0, curMatch.Groups[2].Value);
                            }
                            break;
                    }

                    indexText = curMatch.Index + curMatch.Length;
                }
            }

            ms_kStringBuilder.Append(content.Substring(indexText, content.Length - indexText));
            return ms_kStringBuilder.ToString();
        }

        List<ActiveManager.ControlData> m_kControlData = null;

        void _StatusFilter()
        {
            if (m_kControlData == null)
            {
                if (activeData == null || activeData.values == null)
                {
                    return;
                }
                if (!activeData.values.TryGetValue(localKey, out m_kControlData))
                {
                    return;
                }
            }

            if (m_kControlData != null)
            {
                bool isFull = PlayerBaseData.GetInstance().IsLevelFull;

                for (int i = 0; i < m_kControlData.Count; ++i)
                {
                    var currentControlData = m_kControlData[i];
                    if (currentControlData != null && currentControlData.Type != ActiveManager.ControlData.ControlDataType.CDT_INVALID)
                    {
                        switch (currentControlData.Type)
                        {
                            case ActiveManager.ControlData.ControlDataType.CDT_IMAGE:
                                {
                                    Image kImage = Utility.FindComponent<Image>(goLocal, currentControlData.Name);
                                    if (kImage != null)
                                    {
                                        bool bNeedShow = currentControlData.NeedShow(activeItem.status);

                                        //特殊处理 活动是疲劳找回 或者是 奖励找回 
                                        if (activeData.iActiveID == 8100 || activeData.iActiveID == 8200)
                                        {
                                            //如果状态是初始值 已找回图标不显示
                                            if (activeItem.status == (int)Protocol.TaskStatus.TASK_INIT)
                                            {
                                                bNeedShow = false;
                                            }
                                        }

                                        if (bNeedShow && isFull &&
                                            0 == activeItem.activeItem.IsWorkWithFullLevel &&
                                            activeItem.status >= (int)Protocol.TaskStatus.TASK_INIT && activeItem.status <= (int)Protocol.TaskStatus.TASK_FINISHED)
                                        {
                                            bNeedShow = false;
                                        }

                                        kImage.gameObject.CustomActive(bNeedShow);
                                        var statusValue = currentControlData.GetStatusValue(activeItem.status);
                                        if (statusValue != null)
                                        {
                                            // kImage.sprite = AssetLoader.instance.LoadRes(statusValue.value,typeof(Sprite)).obj as Sprite;
                                            ETCImageLoader.LoadSprite(ref kImage, statusValue.value);
                                        }
                                    }
                                }
                                break;
                            case ActiveManager.ControlData.ControlDataType.CDT_BUTTON:
                                {
                                    Button kButton = Utility.FindComponent<Button>(goLocal, currentControlData.Name);
                                    if (kButton != null)
                                    {
                                        bool bNeedShow = currentControlData.NeedShow(activeItem.status);
                                        if (bNeedShow && isFull &&
                                            0 == activeItem.activeItem.IsWorkWithFullLevel &&
                                            activeItem.status >= (int)Protocol.TaskStatus.TASK_INIT && activeItem.status <= (int)Protocol.TaskStatus.TASK_FINISHED)
                                        {
                                            bNeedShow = false;
                                        }

                                        kButton.gameObject.CustomActive(bNeedShow);
                                        kButton.onClick.RemoveAllListeners();

                                        if (bNeedShow)
                                        {
                                            OnClickActive comActive = kButton.gameObject.GetComponent<OnClickActive>();
                                            kButton.onClick.AddListener(() =>
                                            {
                                                if (!ActiveManager.GetInstance()._CheckCanJumpGo(activeItem.activeItem.LinkLimit, true))
                                                {
                                                    return;
                                                }
                                                if (comActive.m_eOnClickCloseType == OnClickActive.OnClickCloseType.OCCT_PRE)
                                                {
                                                    var frameBinder = comActive.GetComponentInParent<ClientFrameBinder>();
                                                    if (null != frameBinder)
                                                    {
                                                        frameBinder.CloseFrame(true);
                                                    }
                                                }
                                                ActiveManager.GetInstance().OnClickActivity(activeData, comActive, activeItem);
                                                if (comActive.m_eOnClickCloseType == OnClickActive.OnClickCloseType.OCCT_AFT)
                                                {
                                                    var frameBinder = comActive.GetComponentInParent<ClientFrameBinder>();
                                                    if (null != frameBinder)
                                                    {
                                                        frameBinder.CloseFrame(true);
                                                    }
                                                }
                                            });
                                        }
                                    }
                                }
                                break;
                            case ActiveManager.ControlData.ControlDataType.CDT_TEXT:
                                {
                                    Text kText = Utility.FindComponent<Text>(goLocal, currentControlData.Name);
                                    if (kText != null)
                                    {
                                        bool bNeedShow = currentControlData.NeedShow(activeItem.status);
                                        if (bNeedShow && isFull &&
                                            0 == activeItem.activeItem.IsWorkWithFullLevel &&
                                            activeItem.status >= (int)Protocol.TaskStatus.TASK_INIT && activeItem.status <= (int)Protocol.TaskStatus.TASK_FINISHED)
                                        {
                                            bNeedShow = false;
                                        }

                                        kText.gameObject.CustomActive(bNeedShow);

                                        var statusValue = currentControlData.GetStatusValue(activeItem.status);
                                        if (statusValue != null)
                                        {
                                            kText.text = statusValue.value;
                                        }
                                    }
                                }
                                break;
                            case ActiveManager.ControlData.ControlDataType.CDT_GAMEOBJECT:
                                {
                                    GameObject gameObject = Utility.FindChild(goLocal, currentControlData.Name);
                                    bool bNeedShow = currentControlData.NeedShow(activeItem.status);
                                    if (bNeedShow && isFull &&
                                        0 == activeItem.activeItem.IsWorkWithFullLevel &&
                                        activeItem.status >= (int)Protocol.TaskStatus.TASK_INIT && activeItem.status <= (int)Protocol.TaskStatus.TASK_FINISHED)
                                    {
                                        bNeedShow = false;
                                    }
                                    gameObject.CustomActive(bNeedShow);
                                }
                                break;
                        }
                    }
                }
            }
        }

        void _UpdateText()
        {
            if (activeItem != null && !string.IsNullOrEmpty(activeData.mainItem.Desc))
            {
                foreach (Match match in s_regex.Matches(activeData.mainItem.Desc))
                {
                    var filterKey = match.Groups[1].Value;
                    if (filterKey == localKey)
                    {
                        int iType = int.Parse(match.Groups[2].Value);
                        var key = match.Groups[3].Value;
                        var path = match.Groups[4].Value;

                        Text kText = Utility.FindComponent<Text>(goLocal, path);
                        if (kText != null)
                        {
                            if (m_akKey2ValueGroup.ContainsKey(key))
                            {
                                var valueObject = m_akKey2ValueGroup[key];
                                if (valueObject != null)
                                {
                                    kText.text = _ParseText(valueObject.kOrgValue);
                                }
                            }
                        }
                    }
                }
            }
        }

        void _Initialize()
        {
            if (!string.IsNullOrEmpty(activeItem.activeItem.InitDesc))
            {
                var descs = activeItem.activeItem.InitDesc.Split(new char[] { '\r', '\n' });
                for (int i = 0; i < descs.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(descs[i]))
                    {
                        for (int j = 0; j < ms_initkeys.Length; ++j)
                        {
                            var match = ms_initkeys[j].Match(descs[i]);
                            if (match != null && !string.IsNullOrEmpty(match.Groups[0].Value))
                            {
                                switch ((InitializeKeyType)j)
                                {
                                    case InitializeKeyType.IKT_SHOW:
                                        {
                                            GameObject goCurrent = Utility.FindChild(goLocal, match.Groups[2].Value);
                                            if (goCurrent != null)
                                            {
                                                goCurrent.CustomActive(true);
                                            }
                                        }
                                        break;
                                    case InitializeKeyType.IKT_VALUE:
                                        {
                                            switch (match.Groups[3].Value)
                                            {
                                                case "Text":
                                                    {
                                                        Text kText = Utility.FindComponent<Text>(goLocal, match.Groups[2].Value);
                                                        if (null != kText)
                                                        {
                                                            kText.CustomActive(true);
                                                            kText.enabled = true;
                                                            kText.text = match.Groups[4].Value;
                                                        }
                                                    }
                                                    break;
                                                case "Image":
                                                    {
                                                        Image kImage = Utility.FindComponent<Image>(goLocal, match.Groups[2].Value);
                                                        if (null != kImage)
                                                        {
                                                            kImage.CustomActive(true);
                                                            kImage.enabled = true;
                                                            // kImage.sprite = AssetLoader.instance.LoadRes(match.Groups[4].Value, typeof(Sprite)).obj as Sprite;
                                                            ETCImageLoader.LoadSprite(ref kImage, match.Groups[4].Value);
                                                        }
                                                    }
                                                    break;
                                                case "UINumber":
                                                    {
                                                        UINumber uiNumber = Utility.FindComponent<UINumber>(goLocal, match.Groups[2].Value);
                                                        int iNumber = -1;
                                                        if (null != uiNumber && int.TryParse(match.Groups[4].Value, out iNumber))
                                                        {
                                                            uiNumber.gameObject.CustomActive(true);
                                                            uiNumber.Value = iNumber;
                                                        }
                                                        else
                                                        {
                                                            if (null == uiNumber)
                                                            {
                                                                Logger.LogErrorFormat("can not find component with path = {0}", match.Groups[2].Value);
                                                            }
                                                            if (iNumber == -1)
                                                            {
                                                                Logger.LogErrorFormat("int.TraParse error value = {0} can not be converned to int type!", match.Groups[4].Value);
                                                            }
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        void OnItemClicked(GameObject gameObject, ItemData itemData)
        {
            ItemTipManager.GetInstance().ShowTip(itemData);
        }

        public override void OnRecycle()
        {
            Disable();
        }

        public override void Enable()
        {
            if (goLocal != null)
            {
                goLocal.CustomActive(true);
            }
        }

        public override void Disable()
        {
            if (goLocal != null)
            {
                goLocal.CustomActive(false);
            }
        }
        public override void OnDecycle(object[] param) { OnCreate(param); }
        public override void OnRefresh(object[] param) { _UpdateItem(); }
        public override bool NeedFilter(object[] param) { return false; }

        void _UpdateItem()
        {
            try
            {
                _UpdateText();
                _StatusFilter();
                _UpdateVarBinder();
                _UpdateKeyValuePair();

                if (null != comLevelFull)
                {
                    comLevelFull.BindActiveID = activeItem.activeItem.ID;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        void _UpdateVarBinder()
        {
            //if (m_kActiveBindRecords != null)
            //{
            //    for (int i = 0; i < m_kActiveBindRecords.m_VarBinders.Count; ++i)
            //    {
            //        m_kActiveBindRecords.m_VarBinders[i].RefreshStatus();
            //    }
            //}
        }
    }

}