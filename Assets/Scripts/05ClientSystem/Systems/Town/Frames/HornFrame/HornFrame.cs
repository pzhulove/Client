using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class HornFrame : ClientFrame
    {
        public static string ms_lastContent = string.Empty;
        public static void Open()
        {
            if(Pk3v3CrossDataManager.GetInstance().CheckPk3v3CrossScence())
            {
                return;
            }

            if(Pk3v3DataManager.HasInPk3v3Room())
            {
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<HornFrame>(FrameLayer.Middle);
        }

        public static void OpenLinkFrame(string strParam)
        {
            Open();
        }

        #region EmotionBag
        GameObject m_goEmotionTab;
        GameObject m_goEmotionPrefab;
        SpriteAsset m_spriteAsset;

        public class EmotionObject : CachedObject
        {
            protected GameObject goLocal;
            protected GameObject goParent;
            protected GameObject goPrefab;
            protected SpriteAssetInfor spriteAssetInfo;
            protected HornFrame THIS;

            Image emotion;
            Button button;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                spriteAssetInfo = param[2] as SpriteAssetInfor;
                THIS = param[3] as HornFrame;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    emotion = goLocal.GetComponent<Image>();
                    button = goLocal.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(OnClickEmotion);
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
            public override void OnRefresh(object[] param) { OnCreate(param); }
            public override bool NeedFilter(object[] param) { return false; }

            void _UpdateItem()
            {
                emotion.sprite = spriteAssetInfo.sprite;
            }

            void OnClickEmotion()
            {
                THIS.AddChatText("{F " + string.Format("{0}", spriteAssetInfo.ID) + "}");
            }
        }

        void AddChatText(string content)
        { 
            if (!string.IsNullOrEmpty(content))
            { 
                if(content.Length + inputField.text.Length > _GetMaxContentLength())
                {
                    return;
                }

                inputField.text += content;
            }
        }

        CachedObjectDicManager<int, EmotionObject> m_akEmotionObjects = new CachedObjectDicManager<int, EmotionObject>();

        void _InitEmotionBag()
        {
            m_goEmotionTab = Utility.FindChild(frame, "EmotionTab");
            m_goEmotionTab.CustomActive(false);
            m_goEmotionPrefab = Utility.FindChild(m_goEmotionTab, "Emotion");
            m_goEmotionPrefab.CustomActive(false);

            m_spriteAsset = AssetLoader.instance.LoadRes("UI/Image/Emotion/emotion.asset", typeof(SpriteAsset)).obj as SpriteAsset;
            if (m_spriteAsset != null && m_spriteAsset.listSpriteAssetInfor != null)
            {
                for (int i = 0; i < m_spriteAsset.listSpriteAssetInfor.Count; ++i)
                {
                    var spriteAssetInfo = m_spriteAsset.listSpriteAssetInfor[i];
                    if (spriteAssetInfo != null)
                    {
                        m_akEmotionObjects.Create(i, new object[] { m_goEmotionTab, m_goEmotionPrefab, spriteAssetInfo, this });
                    }
                }
            }
        }
        void _UnInitEmotionBag()
        {
            m_akEmotionObjects.DestroyAllObjects();
            m_spriteAsset = null;
            m_goEmotionPrefab = null;
            m_goEmotionTab = null;
        }
        #endregion

        protected override void _OnOpenFrame()
        {
            _InitEmotionBag();
            inputField.onValueChanged.AddListener(OnValueChanged);
            toggle.onValueChanged.AddListener(OnToggleChanged);
            OnValueChanged(ms_lastContent);
            inputField.text = ms_lastContent;
            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_HORN_10_MAX_SEC);
            hornCombHint.text = TR.Value("horn_comb_time", systemValue.Value);
            _UpdateUIData();
        }

        protected override void _OnCloseFrame()
        {
            inputField.onValueChanged.RemoveListener(OnValueChanged);
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
            _UnInitEmotionBag();
            OnCloseWaitData();
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/HornFrame/HornFrame";
        }

        void _UpdateUIData()
        {
            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_HORN_ID);
            if(null != systemValue)
            {
                int iID = systemValue.Value;
                int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(iID);
                int iCostCount = toggle.isOn ? 10 : 1;
                hornCount.text = iHasCount >= iCostCount ? string.Format("<color=#ffffff>{0}/{1}</color>", iHasCount, iCostCount) : string.Format("<color=#ff0000>{0}/{1}</color>", iHasCount,iCostCount);

                var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iID);
                if(null != item)
                {
                    // hornIcon.sprite = AssetLoader.instance.LoadRes(item.Icon, typeof(Sprite)).obj as Sprite;
                    //ETCImageLoader.LoadSprite(ref hornIcon, item.Icon);
                }
            }
        }

        public void OnValueChanged(string content)
        {
            int iMax = _GetMaxContentLength();
            content = content.Replace("\r\n", "");
            if (iMax < content.Length)
            {
                content = content.Substring(0, iMax);
            }
            if(!string.Equals(content,inputField.text))
            {
                inputField.text = content;
                return;
            }

            int iCurCount = content.Length;
            conentCount.text = iCurCount <= iMax ? string.Format("<color=#ffffff>{0}/{1}</color>", iCurCount, iMax) : string.Format("<color=#ff0000>{0}/{1}</color>", iCurCount, iMax);

            if(ms_lastContent != content)
            {
                ms_lastContent = content;
            }
        }

        public void OnToggleChanged(bool bValue)
        {
            _UpdateUIData();
        }

        int _GetMaxContentLength()
        {
            int iMax = 60;
            var systemMaxContent = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_HORN_CONTENT_MAX_WIDTH);
            if (null != systemMaxContent)
            {
                iMax = systemMaxContent.Value;
            }
            return iMax/2;
        }

        private WaitNetMessageManager.IWaitData _mCurWaitData = null;

        [UIControl("Toggle", typeof(Toggle))]
        Toggle toggle;
        [UIControl("Icon/HorCount", typeof(Text))]
        Text hornCount;
        [UIControl("Icon", typeof(Image))]
        Image hornIcon;
        [UIControl("Emotion/Process", typeof(Text))]
        Text conentCount;
        [UIControl("InputField", typeof(InputField))]
        InputField inputField;
        [UIControl("Toggle/Hint",typeof(Text))]
        Text hornCombHint;

        [UIEventHandle("close")]
        void _OnClickClose()
        {
            Close();
        }

        [UIEventHandle("Send")]
        void _OnClickSendMsg()
        {
            if (Pk3v3CrossDataManager.GetInstance().CheckPk3v3CrossScence())
            {
                return;
            }

            if(Pk3v3DataManager.HasInPk3v3Room())
            {
                return;
            }

            if (string.IsNullOrEmpty(inputField.text))
            {
                SystemNotifyManager.SystemNotify(7015);
                return;
            }

            string sendContent = inputField.text;
           
            int iMaxLength = _GetMaxContentLength();
            if(iMaxLength < sendContent.Length)
            {
                //sendContent = sendContent.Substring(0,iMaxLength);
                SystemNotifyManager.SystemNotify(7017);
                return;
            }

            sendContent = ChatFrame.GetFliterSizeString(sendContent);

            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_HORN_ID);
            if(null == systemValue)
            {
                Logger.LogErrorFormat("can not find horn id from systemValue table!");
                return;
            }

            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(systemValue.Value);
            int iCostCount = toggle.isOn ? 10 : 1;

            if(iHasCount >= iCostCount)
            {
                _OnSend(sendContent);
                return;
            }

            ItemComeLink.OnLink(systemValue.Value, iCostCount - iHasCount,true,()=>
            {
                _OnSend(sendContent);
            });
        }

        void _OnSend(string content)
        {
            if(content == null)
            {
                return;
            }

            if (null != _mCurWaitData)
            {
                Logger.LogErrorFormat("IWaitData is not null!!!");
                return;
            }

            SceneChatHornReq kSend = new SceneChatHornReq();
            if(kSend == null)
            {
                return;
            }

            kSend.content = content;
            if(toggle!=null)
            {
                kSend.num = toggle.isOn ? (byte)10 : (byte)1;
            }
            Network.NetManager.Instance().SendCommand(Network.ServerType.GATE_SERVER, kSend);

            _mCurWaitData = WaitNetMessageManager.GetInstance().Wait<SceneChatHornRes>(msgRet =>
            {
                _mCurWaitData = null;
                if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    if(msgRet.result != (uint)ProtoErrorCode.HORN_FORBID_TALK)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.result);
                    }
                }
                else
                {
                    SystemNotifyManager.SystemNotify(7016);
                }

                _UpdateUIData();
                Close();
            }, true, 15.0f, OnCloseWaitData);
        }

        private void OnCloseWaitData()
        {
            if (null != _mCurWaitData)
            {
                WaitNetMessageManager.GetInstance().CancelWait(_mCurWaitData);
                _mCurWaitData = null;
            }
        }

        [UIEventHandle("Emotion")]
        void OnClickEmotionBag()
        {
            m_goEmotionTab.SetActive(!m_goEmotionTab.activeSelf);
        }
    }
}