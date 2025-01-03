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
    public class ActiveObject : CachedObject
    {
        protected GameObject goLocal;
        protected GameObject goParent;
        protected GameObject goPrefab;
        public GameObject gameObject
        {
            get { return goLocal; }
        }

        ActiveBindRecords m_kActiveBindRecords = null;
        void _InitVarBinder()
        {
            m_kActiveBindRecords = goLocal.GetComponent<ActiveBindRecords>();
        }

        ActiveManager.ActiveData activeData;
        static Regex s_regex = new Regex(@"<path=(.+) type=(.+) content=(.+)>");
        static Regex s_regex_content = new Regex(@"<key=(\w+) default=(\w*)>");
        static StringBuilder ms_kStringBuilder;

        #region activeMainDesc
        public static Regex s_regex_maininit = new Regex(@"<Name=(.+) Type=(\w+) Value=(.+)>", RegexOptions.Singleline);
        void _InitMainDesc()
        {
            if (!string.IsNullOrEmpty(activeData.mainItem.MainInitDesc))
            {
                var initItems = activeData.mainItem.MainInitDesc.Split(new char[] { '\r', '\n' });
                for (int i = 0; i < initItems.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(initItems[i]))
                    {
                        var match = s_regex_maininit.Match(initItems[i]);
                        if (!string.IsNullOrEmpty(match.Groups[0].Value))
                        {
                            switch (match.Groups[2].Value)
                            {
                                case "Text":
                                    {
                                        Text text = Utility.FindComponent<Text>(goLocal, match.Groups[1].Value);
                                        if (text != null)
                                        {
                                            text.text = match.Groups[3].Value;
                                        }
                                    }
                                    break;
                                case "Image":
                                    {
                                        Image image = Utility.FindComponent<Image>(goLocal, match.Groups[1].Value);
                                        if (image != null)
                                        {
                                            // image.sprite = AssetLoader.instance.LoadRes(match.Groups[3].Value, typeof(Sprite)).obj as Sprite;
                                            ETCImageLoader.LoadSprite(ref image, match.Groups[3].Value);
                                            image.SetNativeSize();
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        public static void Clear()
        {
            if (ms_kStringBuilder != null)
            {
                StringBuilderCache.Release(ms_kStringBuilder);
                ms_kStringBuilder = null;
            }
        }

        class ValueObject
        {
            public int iIndex;
            public string kOrgValue;
            public string kDefault;
            public object kObject;
            public bool bNeedMatch;
        }

        List<ValueObject> m_akValues = new List<ValueObject>();
        LevelFullControl comLevelFullControl;

        List<RedPointObject> m_akGoRedPoints = null;
        void _InitRedPoint()
        {
            m_akGoRedPoints = RedPointObject.Create(activeData.mainItem.RedPointLocalPath, goLocal);
            if (m_akGoRedPoints != null)
            {
                for (int i = 0; i < m_akGoRedPoints.Count; ++i)
                {
                    m_akGoRedPoints[i].Current.CustomActive(false);
                }
                for (int i = 0; i < m_akGoRedPoints.Count; ++i)
                {
                    if (m_akGoRedPoints[i].redBinder != null)
                    {
                        m_akGoRedPoints[i].redBinder.iMainId = activeData.iActiveID;
                    }
                }
            }
        }
        void _UpdateRedPoint()
        {
            if (m_akGoRedPoints != null)
            {
                for (int i = 0; i < m_akGoRedPoints.Count; ++i)
                {
                    bool bShowRedPoint = ActiveManager.GetInstance().CheckHasFinishedChildItem(activeData, m_akGoRedPoints[i].Keys);
                    m_akGoRedPoints[i].Current.CustomActive(bShowRedPoint);
                }
            }
        }

        public override void OnCreate(object[] param)
        {
            goLocal = param[0] as GameObject;
            goPrefab = param[1] as GameObject;
            activeData = param[2] as ActiveManager.ActiveData;
            goParent = goLocal == null ? goPrefab.transform.parent.gameObject : goLocal.transform.parent.gameObject;

            if (goLocal == null)
            {
                goLocal = GameObject.Instantiate(goPrefab);
                Utility.AttachTo(goLocal, goParent);
            }

            _InitMainDesc();
            _InitVarBinder();
            _InitRedPoint();
            m_akValues.Clear();
            if (ms_kStringBuilder == null)
            {
                ms_kStringBuilder = StringBuilderCache.Acquire();
            }

            Enable();
            _UpdateItem();
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
        public override void OnRefresh(object[] param)
        {
            _UpdateItem();
        }
        public override bool NeedFilter(object[] param) { return activeData.iActiveID != (int)param[0]; }

        void _UpdateMainKeyValues()
        {
            if (goLocal != null)
            {
                if (!string.IsNullOrEmpty(activeData.mainItem.ParticularDesc))
                {
                    var tokens = activeData.mainItem.ParticularDesc.Split(new char[] { '\r', '\n' });
                    int iIndex = -1;
                    for (int i = 0; i < tokens.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(tokens[i]))
                        {
                            var match = s_regex.Match(tokens[i]);
                            if (match != null)
                            {
                                ValueObject current = new ValueObject();
                                current.iIndex = ++iIndex;

                                var path = match.Groups[1].Value;
                                var type = match.Groups[2].Value;
                                var content = match.Groups[3].Value;
                                var indexT = 0;
                                ms_kStringBuilder.Clear();

                                foreach (Match curMatch in s_regex_content.Matches(content))
                                {
                                    ms_kStringBuilder.Append(content.Substring(indexT, curMatch.Index - indexT));

                                    bool bAssigned = false;
                                    for (int j = 0; j < activeData.mainKeyValue.Count; ++j)
                                    {
                                        if (activeData.mainKeyValue[j].key == curMatch.Groups[1].Value)
                                        {
                                            ms_kStringBuilder.Append(activeData.mainKeyValue[j].value);
                                            bAssigned = true;
                                            break;
                                        }
                                    }

                                    if (!bAssigned)
                                    {
                                        var countInfo = CountDataManager.GetInstance().GetCountInfo(activeData.mainItem.ID + "_" + curMatch.Groups[1].Value);
                                        if (null != countInfo)
                                        {
                                            bAssigned = true;
                                            ms_kStringBuilder.Append(countInfo.value);
                                        }
                                    }

                                    if (!bAssigned)
                                    {
                                        ms_kStringBuilder.Append(curMatch.Groups[2].Value);
                                    }

                                    indexT = curMatch.Index + curMatch.Length;
                                }
                                ms_kStringBuilder.Append(content.Substring(indexT, content.Length - indexT));

                                var realContent = ms_kStringBuilder.ToString();

                                if (type == "Text")
                                {
                                    var kText = Utility.FindComponent<Text>(goLocal, path);
                                    if (kText != null)
                                    {
                                        kText.text = realContent;
                                        current.kObject = kText;
                                        current.kOrgValue = content;
                                    }
                                }
                                else if (type == "Image")
                                {
                                    var kImage = Utility.FindComponent<Image>(goLocal, path);
                                    if (kImage != null)
                                    {
                                        // kImage.sprite = AssetLoader.instance.LoadRes(realContent, typeof(Sprite)).obj as Sprite;
                                        ETCImageLoader.LoadSprite(ref kImage, realContent);
                                        current.kObject = kImage;
                                    }
                                }
                                else if (type == "Button")
                                {
                                    var button = Utility.FindComponent<Button>(goLocal, path);
                                    if (button != null)
                                    {
                                        var script = Utility.FindComponent<OnClickActive>(goLocal, path);
                                        button.onClick.RemoveAllListeners();
                                        button.onClick.AddListener(() =>
                                        {
                                            if (script.m_eOnClickCloseType == OnClickActive.OnClickCloseType.OCCT_PRE)
                                            {
                                                var frameBinder = script.GetComponentInParent<ClientFrameBinder>();
                                                if (null != frameBinder)
                                                {
                                                    frameBinder.CloseFrame(true);
                                                }
                                            }
                                            ActiveManager.GetInstance().OnClickActivity(activeData, script, null);
                                            if (script.m_eOnClickCloseType == OnClickActive.OnClickCloseType.OCCT_AFT)
                                            {
                                                var frameBinder = script.GetComponentInParent<ClientFrameBinder>();
                                                if (null != frameBinder)
                                                {
                                                    frameBinder.CloseFrame(true);
                                                }
                                            }
                                        });
                                        current.kObject = button;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void _UpdateItem()
        {
            _UpdateMainKeyValues();
            _UpdateVarBinder();
            _UpdateRedPoint();
        }

        void _UpdateVarBinder()
        {
            if (m_kActiveBindRecords != null)
            {
                for (int i = 0; i < m_kActiveBindRecords.m_VarBinders.Count; ++i)
                {
                    m_kActiveBindRecords.m_VarBinders[i].RefreshStatus();
                }
            }
        }
    }
}