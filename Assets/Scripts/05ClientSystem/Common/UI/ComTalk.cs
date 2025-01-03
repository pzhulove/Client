using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using ProtoTable;
using System;
using ActivityLimitTime;
using Protocol;

namespace GameClient
{
    class ComTalk : MonoBehaviour
    {
        enum ShowMode
        {
            Normal, // 正常模式，界面高度小
            Expand, // 扩展模式，界面高度大
        }

        public static ComTalk ms_comTalk = null;
        private const string prefabPath = "UIFlatten/Prefabs/Common/ComTalk";
        private GameObject m_root;
        private GameObject m_goBtnUp;
        private GameObject m_goBtnDown;
        private GameObject m_goChatParent;
        private GameObject m_goChatPrefab;
        private FastVerticalLayout m_kFastVerticalLayout;
        private Button m_btnChat;
        private Button m_btnSetting;
        private Button m_btnUp;
        private Button m_btnDown;
        private Button mFriend = null;
        private Image mFriendRedPoint = null;
        private GameObject mPrivateChatBubble = null;

        ComCommonBind bind = null;

        List<ComCommonBind> NeedReshowEffectFuncs = new List<ComCommonBind>();
        ComVoiceChat comVoiceChat = null;
        GameObject teamVoiceBtnGo;    
        GameObject guildVoiceBtnGo;
        float fOffset = 125.0f + 25;
        float fOffset2 = 64.0f + 25;
        ComTalkExtraParam extraParam;
        GameObject voiceListRoot = null; // 可以供选择的话筒列表
        bool showVoiceListRoot = false; // 当前是否显示话筒选择列表
        ButtonEx selectVoice = null;    // 弹出选择列表的按钮
        GameObject curSelectVoiceGo = null; // 当前选择的话筒

        Button selectTeamVoice = null; // 选择组队话筒
        Button selectGuildVoice = null; // 选择公会话筒

        ShowMode showMode = ShowMode.Normal; // 聊天显示模式

        List<GameObject> voiceBtnGoList = new List<GameObject>(); // 界面上可以使用的话筒列表(不是那个选择列表里面的，是可以按住说话的那种)

        class ChatObject : CachedObject
        {
            protected GameObject goLocal;
            protected GameObject goParent;
            protected GameObject goPrefab;
            protected ChatBlock chatBlock;
            protected ChatData chatData;
            protected ComTalk THIS;
            protected ChatType eChatType;
            protected int iOrder;

            Image kIcon;
            Text kTime;
            LinkParse kContent;
            LayoutSortOrder kSortOrder;
            public int SortOrder
            {
                get
                {
                    return iOrder;
                }
            }

            public sealed override void SetAsLastSibling()
            {
                if (goLocal != null)
                {
                    goLocal.transform.SetAsLastSibling();
                }
            }

            public sealed override void OnDestroy()
            {
                kContent.RemoveFailedListener(_OnClickFailed);
            }

            void _OnClickFailed()
            {
                ClientSystemManager.GetInstance().OpenFrame<ChatFrame>(FrameLayer.Middle);
            }

            public sealed override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                chatBlock = param[2] as ChatBlock;
                chatData = chatBlock.chatData;
                chatBlock.iPreID = (ulong)chatBlock.chatData.guid;
                chatBlock.eType = ChatBlockType.CBT_KEEP;
                eChatType = chatData.eChatType;
                iOrder = chatBlock.iOrder;
                chatBlock = null;

                if (goPrefab == null)
                {
                    return;
                }

                THIS = param[3] as ComTalk;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    kContent = Utility.FindComponent<LinkParse>(goLocal, "Emotion");
                    kSortOrder = goLocal.GetComponent<LayoutSortOrder>();
                    Utility.AttachTo(goLocal, goParent);
                    kContent.RemoveFailedListener(_OnClickFailed);
                    kContent.AddOnFailedListener(_OnClickFailed);
                }

                _UpdateItem();

                Enable();
                /*
                if (!NeedFilter(new object[] { SystemConfigManager.GetInstance().ChatFilters }))
                {
                    Enable();
                    //InvokeMethod.AddPerFrameCall(THIS, THIS.DelayUpdatePosition);
                }
                else
                {
                    Disable();
                }
                */
            }

            public sealed override void OnRecycle()
            {
                Disable();
            }
            public sealed override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }

            public sealed override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public sealed override void OnDecycle(object[] param) { OnCreate(param); }
            public sealed override void OnRefresh(object[] param) { OnCreate(param); }

            public sealed override bool NeedFilter(object[] param)
            {
                return false;
                var value = param[0] as List<bool>;
                int iIndex = (int)eChatType;
                if (iIndex >= 0 && iIndex < value.Count)
                {
                    return !((bool)value[iIndex]);
                }

                return true;
            }

            void _UpdateItem()
            {
                try
                {
                    if (chatData != null)
                    {
                        var nameLink = chatData.GetNameLink();
                        if(!string.IsNullOrEmpty(nameLink))
                        {
                            kContent.SetText(chatData.GetChannelString() + nameLink + ":" + chatData.GetWords(), true);
                        }
                        else
                        {
                            kContent.SetText(chatData.GetChannelString() + chatData.GetWords(), true);
                        }
                        chatData = null;
                    }
                    if (kSortOrder != null)
                    {
                        kSortOrder.SortID = iOrder;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            }
        }
        CachedObjectDicManager<int, ChatObject> m_akChatObjects = new CachedObjectDicManager<int, ChatObject>();

        public static ComTalk Create(GameObject goParent)
        {
            if(ms_comTalk != null)
            {
                Utility.AttachTo(ms_comTalk.gameObject, goParent);
                ms_comTalk.gameObject.CustomActive(true);
                ms_comTalk.gameObject.transform.localScale = Vector3.one;
                (ms_comTalk.gameObject.transform as RectTransform).anchoredPosition3D = Vector3.zero;
                return ms_comTalk;
            }

            GameObject goTalk = AssetLoader.instance.LoadRes(prefabPath, typeof(GameObject)).obj as GameObject;
            if (goTalk != null)
            {
                Utility.AttachTo(goTalk, goParent);
                goTalk.CustomActive(true);
                ms_comTalk = goTalk.AddComponent<ComTalk>();
                (ms_comTalk.gameObject.transform as RectTransform).anchoredPosition3D = Vector3.zero;
                return ms_comTalk;
            }

            return null;
        }
        public static void Recycle()
        {
            if(null != ms_comTalk)
            {
                ms_comTalk.gameObject.transform.SetParent(null);
            }
        }

        public static void ForceDestroy()
        {
            if (null != ms_comTalk)
            {
                ms_comTalk.gameObject.transform.SetParent(null);
                UnityEngine.Object.Destroy(ms_comTalk.gameObject);
                ms_comTalk = null;
            }
        }

        ComCommonBind   m_newMailNotice;
        Button          m_newMailNoticeButton;
        Image           m_newMailItemTip;

        ComCommonBind   m_ActivityNotice;
        Button          m_ActivityNoticeButton;
        Text            m_ActivityNoticeText;

        ComCommonBind   m_TeamInvite;
        Button          m_TeamInviteButton;
        Text            m_TeamInviteText;

        //团本申请（申请入团）
        private ComCommonBind   m_TeamDuplicationRequest;
        private Button m_TeamDuplicationRequestButton;
        private Text m_teamDuplicationRequestText;

        //团本邀请(团本邀请）
        private ComCommonBind m_TeamDuplicationTeamInvite;
        private Button m_TeamDuplicationTeamInviteButton;
        private Text m_TeamDuplicationTeamInviteText;

        ComCommonBind   m_RoomInvite;
        Button          m_RoomInviteButton;
        Text            m_RoomInviteText;

        ComCommonBind m_CrossPK3v3RoomInvite;
        Button m_CrossPK3v3RoomInviteButton;

        ComCommonBind   m_SkillLvUp;
        Button          m_SkillLvUpButton;

        private ComCommonBind m_FinancialPlan;
        private Button m_FinancialPlanButton;
        private Text m_FinancialPlanText;

        ComCommonBind m_FriendRequest;
        Button          m_FriendRequestButton;
        Text            m_FriendRequestText;

        ComCommonBind   m_GuildInvite;
        Button          m_GuildInviteButton;
        Text            m_GuildInviteText;

        ComCommonBind   m_LimitTimeGift;
        Button          m_LimitTimeGiftButton;
        Text            m_LimitTimeGiftText;

        ComCommonBind   m_ItemTimeLess;
        Button          m_ItemTimeLessButton;

        ComCommonBind m_PrivateOrdering;
        Button m_PrivateOrderingButton;
        Text m_PrivateOrderingDes;

        ComCommonBind m_AuctionFreezeRemind;
        Button m_AuctionFreezeRemindButton;

        ComCommonBind m_CrossPK3V3;
        Button m_CrossPk3V3Button;

        ComCommonBind m_SecurityLockApplyState;
        Button m_SecurityLockApplyStateButton;
        ComCommonBind mGuildMerge;
        Button mGuildMergeBtn;
        UInt64 mDataVersion = UInt64.MaxValue;
        
        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            //UpdateNewMailNotice();
        }

        void Awake()
        {
            voiceBtnGoList = new List<GameObject>();
            showMode = ShowMode.Normal;
            curSelectVoiceGo = null;

            bind = this.gameObject.GetComponent<ComCommonBind>();
            bindUI();

            showVoiceListRoot = false;
            voiceListRoot.CustomActive(showVoiceListRoot);

            m_root = gameObject;//CGameObjectPool.instance.GetGameObject(prefabPath, enResourceType.UIPrefab);
            m_goBtnUp = Utility.FindChild(m_root, "UpButton");
            m_goBtnDown = Utility.FindChild(m_root, "DownButton");
            m_goChatParent = Utility.FindChild(m_root, "Viewport/Content");
            m_goChatPrefab = Utility.FindChild(m_goChatParent, "ChatPrefab");
            m_goChatPrefab.CustomActive(false);
            m_kFastVerticalLayout = m_goChatParent.GetComponent<FastVerticalLayout>();
            m_goBtnDown.CustomActive(false);

            m_newMailNotice = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/NewMailNotice");
            m_ActivityNotice = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/ActivityNotice");
            m_TeamInvite = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/TeamInvite");

            m_TeamDuplicationRequest = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/TeamDuplicationRequest");
            m_TeamDuplicationTeamInvite = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/TeamDuplicationTeamInvite");

            m_RoomInvite = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/RoomInvite");
            m_SkillLvUp = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/SkillLvUpNotice");
            m_FriendRequest = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/FriendRequest");
            m_GuildInvite = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/GuildRequest");
            m_LimitTimeGift = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/NewMallGiftNotice");
            m_ItemTimeLess = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/ItemTimeLess");
            m_PrivateOrdering = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/PrivateOrdering");
            m_FinancialPlan = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/FinancialPlan");
            mGuildMerge = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/GuildMerge");

            m_AuctionFreezeRemind = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/AuctionFreezeRemind");
            m_CrossPK3V3 = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/CrossPK3V3");
            m_CrossPK3v3RoomInvite = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/CrossPK3v3RoomInvite");
            m_SecurityLockApplyState = Utility.FindComponent<ComCommonBind>(m_root, "Vertical/NoticeRoot/SecurityLockApplyState");

            m_newMailNoticeButton = m_newMailNotice.GetCom<Button>("NoticeButton");
            m_newMailNoticeButton.onClick.AddListener(OnClickNewMailNotice);
            m_newMailItemTip = m_newMailNotice.GetCom<Image>("ItemTip");

            m_ActivityNoticeButton = m_ActivityNotice.GetCom<Button>("NoticeButton");
            m_ActivityNoticeButton.onClick.AddListener(OnClickActivityNoticeButton);
            m_ActivityNoticeText = m_ActivityNotice.GetCom<Text>("RedPointText");

            m_TeamInviteButton = m_TeamInvite.GetCom<Button>("NoticeButton");
            m_TeamInviteButton.onClick.AddListener(OnClickTeamInviteButton);
            m_TeamInviteText = m_TeamInvite.GetCom<Text>("RedPointText");

            if (m_TeamDuplicationRequest != null)
            {
                m_teamDuplicationRequestText = m_TeamDuplicationRequest.GetCom<Text>("RedPointText");
                m_TeamDuplicationRequestButton = m_TeamDuplicationRequest.GetCom<Button>("NoticeButton");
                if (m_TeamDuplicationRequestButton != null)
                {
                    m_TeamDuplicationRequestButton.onClick.AddListener(OnClickTeamDuplicationRequestButton);
                }
            }

            if (m_TeamDuplicationTeamInvite != null)
            {
                m_TeamDuplicationTeamInviteText = m_TeamDuplicationTeamInvite.GetCom<Text>("RedPointText");
                m_TeamDuplicationTeamInviteButton = m_TeamDuplicationTeamInvite.GetCom<Button>("NoticeButton");
                if (m_TeamDuplicationTeamInviteButton != null)
                {
                    m_TeamDuplicationTeamInviteButton.onClick.AddListener(OnClickTeamDuplicationTeamInviteButton);
                }
            }

            ComTalkExtraParam param = this.gameObject.GetComponent<ComTalkExtraParam>();
            if (param != null)
            {
                mFriend = param.mFriend;
                mFriendRedPoint = param.mFriendRedPoint;
                mPrivateChatBubble = param.mPrivateChatBubble;
            }

            mFriend.SafeSetOnClickListener(() => 
            {
                RelationFrameNew.CommandOpen();
                GameStatisticManager.GetInstance().DoStartUIButton("Friend");
            });


            m_RoomInviteButton = m_RoomInvite.GetCom<Button>("NoticeButton");
            m_RoomInviteButton.onClick.AddListener(OnClickRoomInviteButton);
            m_RoomInviteText = m_RoomInvite.GetCom<Text>("RedPointText");

            m_CrossPK3v3RoomInviteButton = m_CrossPK3v3RoomInvite.GetCom<Button>("NoticeButton");
            m_CrossPK3v3RoomInviteButton.onClick.AddListener(OnCrossPK3v3RoomInviteButton);

            m_SkillLvUpButton = m_SkillLvUp.GetCom<Button>("NoticeButton");
            m_SkillLvUpButton.onClick.AddListener(OnClickSkillLvUpButton);

            m_FinancialPlanButton = m_FinancialPlan.GetCom<Button>("NoticeButton");
            m_FinancialPlanButton.onClick.AddListener(OnClickFinancialPlan);
            m_FinancialPlanText = m_FinancialPlan.GetCom<Text>("FinancialPlanText");

            m_FriendRequestButton = m_FriendRequest.GetCom<Button>("NoticeButton");
            m_FriendRequestButton.onClick.AddListener(OnClickFriendRequestButton);
            m_FriendRequestText = m_FriendRequest.GetCom<Text>("RedPointText");

            m_GuildInviteButton = m_GuildInvite.GetCom<Button>("NoticeButton");
            m_GuildInviteButton.onClick.AddListener(OnClickGuildInviteButton);
            m_GuildInviteText = m_GuildInvite.GetCom<Text>("RedPointText");

            m_LimitTimeGiftButton = m_LimitTimeGift.GetCom<Button>("NoticeButton");
            m_LimitTimeGiftButton.onClick.AddListener(OnClickLimitTimeGiftButton);
            m_LimitTimeGiftText = m_LimitTimeGift.GetCom<Text>("RedPointText");

            m_ItemTimeLessButton = m_ItemTimeLess.GetCom<Button>("NoticeButton");
            m_ItemTimeLessButton.onClick.AddListener(OnClickItemTimeLessButton);

            m_PrivateOrderingButton = m_PrivateOrdering.GetCom<Button>("NoticeButton");
            m_PrivateOrderingButton.onClick.AddListener(OnClickPrivateOrderingButton);
            m_PrivateOrderingDes = m_PrivateOrdering.GetCom<Text>("Des");

            m_AuctionFreezeRemindButton = m_AuctionFreezeRemind.GetCom<Button>("NoticeButton");
            m_AuctionFreezeRemindButton.onClick.AddListener(OnAuctinFreezeRemindClick);

            m_CrossPk3V3Button = m_CrossPK3V3.GetCom<Button>("NoticeButton");
            m_CrossPk3V3Button.onClick.AddListener(OpenJoinPK3V3CrossFrame);

            m_SecurityLockApplyStateButton = m_SecurityLockApplyState.GetCom<Button>("NoticeButton");
            m_SecurityLockApplyStateButton.onClick.AddListener(OpenSettingFrameWithApplyState);
            mGuildMergeBtn = mGuildMerge.GetCom<Button>("NoticeButton");
            mGuildMergeBtn.onClick.AddListener(OnGuildMergeAskBtnClick);
            m_btnChat = Utility.FindComponent<Button>(m_root, "BtnChat");
            m_btnChat.onClick.AddListener(() =>
            {
                ClientSystemManager.GetInstance().OpenFrame<ChatFrame>(FrameLayer.Middle);
            });

            m_btnSetting = Utility.FindComponent<Button>(m_root, "SettingButton");
            m_btnSetting.onClick.AddListener(() =>
            {
                TalkTabFrame.Open();
            });

            extraParam = m_root.GetComponent<ComTalkExtraParam>();
            if (extraParam)
            {
                fOffset = extraParam.upOffsetHeight;
                fOffset2 = extraParam.normalHeight;
            }

            m_btnUp = Utility.FindComponent<Button>(m_root, "UpButton");
            m_btnUp.onClick.AddListener(() =>
            {
                showMode = ShowMode.Expand;

                m_goBtnUp.CustomActive(false);
                m_goBtnDown.CustomActive(true);
                RectTransform rect = m_root.transform as RectTransform;
                //95 == > 115
                rect.offsetMax = new Vector2(rect.offsetMax.x, fOffset);
                rect.offsetMin = new Vector2(rect.offsetMin.x, 0);
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
                rect.anchoredPosition3D = new Vector3(rect.anchoredPosition3D.x, 0, rect.anchoredPosition3D.z);
               
                var isTeamShow = TeamDataManager.GetInstance().HasTeam() ? true : false;
                var isGuildShow = GuildDataManager.GetInstance().myGuild != null ? true : false;

                // 点击向上的时候把该显示的都显示出来
                teamVoiceBtnGo.CustomActive(isTeamShow);
                guildVoiceBtnGo.CustomActive(isGuildShow);

                // 选择按钮和选择列表是一定要隐藏的
                selectVoice.CustomActive(false);
                showVoiceListRoot = false;
                voiceListRoot.CustomActive(showVoiceListRoot);
            });

            m_btnDown = Utility.FindComponent<Button>(m_root, "DownButton");
            m_btnDown.onClick.AddListener(() =>
            {
                showMode = ShowMode.Normal;

                m_goBtnUp.CustomActive(true);
                m_goBtnDown.CustomActive(false);
                RectTransform rect = m_root.transform as RectTransform;
                //115 == > 95
                rect.offsetMax = new Vector2(rect.offsetMax.x, fOffset2);
                rect.offsetMin = new Vector2(rect.offsetMin.x, 0);
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
                rect.anchoredPosition3D = new Vector3(rect.anchoredPosition3D.x, 0, rect.anchoredPosition3D.z);

                var isTeamShow = TeamDataManager.GetInstance().HasTeam() ? true : false;
                var isGuildShow = GuildDataManager.GetInstance().myGuild != null ? true : false;

                // 点击向下按钮时只显示之前选中的话筒
                if (voiceBtnGoList != null && curSelectVoiceGo != null)
                {
                    for (int i = 0; i < voiceBtnGoList.Count; i++)
                    {
                        if (voiceBtnGoList[i] != curSelectVoiceGo)
                        {
                            voiceBtnGoList[i].CustomActive(false);
                        }
                    }

                    curSelectVoiceGo.CustomActive(true);
                }

                // 选择按钮按照情况显示
                selectVoice.CustomActive(isTeamShow || isGuildShow);

                // 选择列表一定要隐藏
                showVoiceListRoot = false;
                voiceListRoot.CustomActive(showVoiceListRoot);
            });

            _InitVoiceChat();

            InitChat();
            ChatManager.GetInstance().onAddGlobalChatData += this.OnAddChat;
            ChatManager.GetInstance().onRebuildGlobalChatData += this.OnRebuildChatData;
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;

            InitRedPoint();

            BindUIEvent();

            UpdateNewMailNotice();
            UpdateActivityNotice();
            UpdateNewTeamInviteNotice();

            UpdateTeamDuplicationRequest();
            UpdateTeamDuplicationNewTeamInvite();

            UpdateNewRoomInviteNotice();
            UpdateNewRoomInviteNoticePk3v3Cross();
            UpdateSkillLvUpNotice();
            UpdateFriendRequestNotice();
            //UpdateGuildInviteNotice();
            UpdateItemTimeLess();
            InitFinancialPlan();

            UIEvent uiEvent = new UIEvent();
            uiEvent.Param1 = (byte)Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();

            OnPK3V3CrossButtonIsShow(uiEvent);
            OnSecurityLockApplyStateButtonIsShow(null);
        }

        void Start()
        {
        }

        void OnDestroy()
        {
            voiceBtnGoList = null;
            curSelectVoiceGo = null;
            showMode = ShowMode.Normal;
            showVoiceListRoot = false;

            ChatManager.GetInstance().onAddGlobalChatData -= this.OnAddChat;
            ChatManager.GetInstance().onRebuildGlobalChatData -= this.OnRebuildChatData;

            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;

            if (m_btnDown != null)
            {
                m_btnDown.onClick.RemoveAllListeners();
                m_btnDown = null;
            }

            if (m_btnUp != null)
            {
                m_btnUp.onClick.RemoveAllListeners();
                m_btnUp = null;
            }

            if (m_btnChat != null)
            {
                m_btnChat.onClick.RemoveAllListeners();
                m_btnChat = null;
            }

            if (m_btnSetting != null)
            {
                m_btnSetting.onClick.RemoveAllListeners();
                m_btnSetting = null;
            }

            if (m_newMailNoticeButton != null)
            {
                m_newMailNoticeButton.onClick.RemoveListener(OnClickNewMailNotice);
            }

            if (m_ActivityNoticeButton != null)
            {
                m_ActivityNoticeButton.onClick.RemoveListener(OnClickActivityNoticeButton);
            }

            if (m_TeamInviteButton != null)
            {
                m_TeamInviteButton.onClick.RemoveListener(OnClickTeamInviteButton);
            }

            if (m_TeamDuplicationRequestButton != null)
            {
                m_TeamDuplicationRequestButton.onClick.RemoveListener(OnClickTeamDuplicationRequestButton);
            }

            if (m_TeamDuplicationTeamInviteButton != null)
            {
                m_TeamDuplicationTeamInviteButton.onClick.RemoveListener(OnClickTeamDuplicationTeamInviteButton);
            }

            if(m_RoomInviteButton != null)
            {
                m_RoomInviteButton.onClick.RemoveListener(OnClickRoomInviteButton);
            }

            if (m_CrossPK3v3RoomInviteButton != null)
            {
                m_CrossPK3v3RoomInviteButton.onClick.RemoveListener(OnCrossPK3v3RoomInviteButton);
            }

            if (m_SkillLvUpButton != null)
            {
                m_SkillLvUpButton.onClick.RemoveListener(OnClickSkillLvUpButton);
            }

            if (m_FriendRequestButton != null)
            {
                m_FriendRequestButton.onClick.RemoveListener(OnClickFriendRequestButton);
            }

            if (m_GuildInviteButton != null)
            {
                m_GuildInviteButton.onClick.RemoveListener(OnClickGuildInviteButton);
            }

            if (m_ItemTimeLessButton != null)
            {
                m_ItemTimeLessButton.onClick.RemoveListener(OnClickItemTimeLessButton);
            }

            if (m_PrivateOrderingButton != null)
            {
                m_PrivateOrderingButton.onClick.RemoveListener(OnClickPrivateOrderingButton);
            }

            if (m_AuctionFreezeRemindButton != null)
            {
                m_AuctionFreezeRemindButton.onClick.RemoveListener(OnAuctinFreezeRemindClick);
            }

            if (m_FinancialPlanButton != null)
            {
                m_FinancialPlanButton.onClick.RemoveAllListeners();
            }

            if (m_CrossPk3V3Button != null)
            {
                m_CrossPk3V3Button.onClick.RemoveListener(OpenJoinPK3V3CrossFrame);
            }

            if (m_SecurityLockApplyStateButton != null)
            {
                m_SecurityLockApplyStateButton.onClick.RemoveListener(OpenSettingFrameWithApplyState);
            }
            if(mGuildMergeBtn!=null)
            {
                mGuildMergeBtn.onClick.RemoveListener(OnGuildMergeAskBtnClick);
            }

            UnBindUIEvent();

            _UnInitVoiceChat();

            m_akChatObjects.DestroyAllObjects();

            if (ms_comTalk == this)
            {
                ms_comTalk = null;
            }

            NeedReshowEffectFuncs.Clear();

            unBindUI();
            bind = null;
        }

        public GameObject GetTeamInvitedBtn()
        {
            if(m_TeamInvite == null)
            {
                return null;
            }
            return m_TeamInvite.gameObject;
        }
        private void InitChat()
        {
            m_akChatObjects.RecycleAllObject();
            var chatDatas = new List<ChatBlock>();
            chatDatas.AddRange(ChatManager.GetInstance().GlobalChatBlock);
            chatDatas.Sort((x, y) =>
            {
                return x.iOrder - y.iOrder;
            });

            for (int i = 0; i < chatDatas.Count; ++i)
            {
                OnAddChat(chatDatas[i]);
            }
        }

        public GameObject GetRoot()
        {
            return m_root;
        }

        public void OnAddChat(ChatBlock chatBlock)
        {
            if (chatBlock.chatData.eChatType == ChatType.CT_PRIVATE)
            {
                return;
            }
            GameObject goChatParent = m_goChatParent;
            GameObject goChatPrefab = m_goChatPrefab;
            var chatData = chatBlock.chatData;
            if (m_akChatObjects.HasObject(chatData.guid))
            {
                return;
            }
            var current = m_akChatObjects.Create(chatData.guid, new object[] { m_goChatParent, m_goChatPrefab, chatBlock, this });
            if (current == null)
            {
                return;
            }
            //current.SetAsLastSibling();
            m_akChatObjects.FilterObject(chatData.guid, new object[] { SystemConfigManager.GetInstance().ChatFilters });
            //_SortObject();
            if (m_kFastVerticalLayout != null)
            {
                m_kFastVerticalLayout.MarkDirty();
            }
            //OnPopAroundDialog(chatData);
        }

        public void OnChatFilterChanged(List<bool> chatFilters)
        {
            m_akChatObjects.Filter(new object[] { SystemConfigManager.GetInstance().ChatFilters });
        }

        void _OnAddNewItem(List<Item> items)
        {
            UpdateItemTimeLess();
        }

        void _OnUpdateItem(List<Item> items)
        {
            UpdateItemTimeLess();
        }

        void _OnRemoveItem(ItemData data)
        {
            UpdateItemTimeLess();
        }

        public void OnRebuildChatData(ulong preGuid, ChatBlock chatBlock)
        {
            var chatData = chatBlock.chatData;
            if (m_akChatObjects.HasObject(chatData.guid))
            {
                return;
            }
            var current = m_akChatObjects.RebuildOrCreate((int)preGuid, chatData.guid, new object[] { m_goChatParent, m_goChatPrefab, chatBlock, this });
            if (current == null)
            {
                return;
            }
            ///current.SetAsLastSibling();
            m_akChatObjects.FilterObject(chatData.guid, new object[] { SystemConfigManager.GetInstance().ChatFilters });
            //_SortObject();
            if (m_kFastVerticalLayout != null)
            {
                m_kFastVerticalLayout.MarkDirty();
            }
            //OnPopAroundDialog(chatData);
        }

        private void DelayUpdatePosition()
        {
            ScrollRect rect = m_root.GetComponent<ScrollRect>();
            rect.verticalNormalizedPosition = 0.0f;
        }

        void bindUI()
        {
            voiceListRoot = bind.GetGameObject("voiceListRoot");

            selectTeamVoice = bind.GetCom<Button>("selectTeamVoice");
            selectTeamVoice.SafeSetOnClickListener(() => 
            {
                curSelectVoiceGo = teamVoiceBtnGo;
                if(curSelectVoiceGo != null)
                {
                    curSelectVoiceGo.transform.SetAsLastSibling();
                }                

                teamVoiceBtnGo.CustomActive(true);
                guildVoiceBtnGo.CustomActive(false);

                showVoiceListRoot = false;
                voiceListRoot.CustomActive(showVoiceListRoot);

                
            });

            selectGuildVoice = bind.GetCom<Button>("selectGuildVoice");
            selectGuildVoice.SafeSetOnClickListener(() =>
            {
                curSelectVoiceGo = guildVoiceBtnGo;
                if (curSelectVoiceGo != null)
                {
                    curSelectVoiceGo.transform.SetAsLastSibling();
                }

                teamVoiceBtnGo.CustomActive(false);
                guildVoiceBtnGo.CustomActive(true);

                showVoiceListRoot = false;
                voiceListRoot.CustomActive(showVoiceListRoot);

                
            });

            selectVoice = bind.GetCom<ButtonEx>("selectVoice");
            selectVoice.SafeSetOnClickListener(() => 
            {
                showVoiceListRoot = !showVoiceListRoot;
                voiceListRoot.CustomActive(showVoiceListRoot);
            });
            
            comVoiceChat = bind.GetCom<ComVoiceChat>("comVoiceChat");
        }

        void unBindUI()
        {
            voiceListRoot = null;
            selectTeamVoice = null;
            selectGuildVoice = null;
            selectVoice = null;
            comVoiceChat = null;
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NewMailNotify, OnNewMailNoticeUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityNoticeUpdate, OnActivityNoticeUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamNewInviteNoticeUpdate, OnTeamNewInviteNoticeUpdate);

            UIEventSystem.GetInstance()
                .RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationOwnerNewRequesterMessage,
                    OnTeamDuplicationRequestUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationOwnerNewTeamInviteMessage,
                OnTeamDuplicationNewTeamInviteUpdate);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3InviteRoomListUpdate, OnRoomNewInviteNoticeUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SkillLvUpNoticeUpdate, OnSkillLvUpNoticeUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FriendRequestNoticeUpdate, OnFriendRequestNoticeUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildInviteNoticeUpdate, OnGuildInviteNoticeUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GoodsRecommend,OnPrivateOrderingNoticeUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MiddleFrameClose, _OnMiddleFrameClose);

            //理财计划，充值以及购买理财计划
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FinancialPlanBuyRes, OnFinancialPlanUpdate);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerVipLvChanged, OnFinancialPlanUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FinancialPlanButtonUpdateByLevel, OnFinancialPlanUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FinancialPlanButtonUpdateByLogin, OnFinancialPlanUpdateByLogin);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FinancialPlanButtonUpdateBySceneChanged, OnFinancialPlanButtonUpdateBySceneChanged);

            // added by mjx on 170802
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HasLimitTimeGiftToBuy ,OnNewLimitTimeGift);

            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TimeLessItemsChanged, _OnTimeLessItemsChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DeadLineReminderChanged, _OnTimeLessItemsChanged);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AuctionFreezeRemind, _OnAuctionFreezeRemind);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3CrossButton, OnPK3V3CrossButtonIsShow);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, _OnMoneyRewardsStatusChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SecurityLockApplyStateButton, OnSecurityLockApplyStateButtonIsShow);
            //公会兼并
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildReceiveMergerd, OnGuildReceiveMergerRequest);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefuseAllReceive, OnRefuseAllReciveRequest);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshInviteList, OnNotifyFriendInvite);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnNewPupilApplyRecieved, OnNotifyFriendInvite);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRecvPrivateChat, OnRecvPrivateChat);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnApplyPupilListChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnApplyTeacherListChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTAPApplyToggleRedPointUpdate, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleChatDirtyChanged, OnPrivateChat);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRelationChanged, OnRelationChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTapPupilReportRedPoint, OnRelationChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTapTeacherSubmitRedPoint, OnRelationChanged);
        }

       

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NewMailNotify, OnNewMailNoticeUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityNoticeUpdate, OnActivityNoticeUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamNewInviteNoticeUpdate, OnTeamNewInviteNoticeUpdate);

            UIEventSystem.GetInstance()
                .UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationOwnerNewRequesterMessage,
                    OnTeamDuplicationRequestUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationOwnerNewTeamInviteMessage,
                OnTeamDuplicationNewTeamInviteUpdate);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3InviteRoomListUpdate, OnRoomNewInviteNoticeUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SkillLvUpNoticeUpdate, OnSkillLvUpNoticeUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FriendRequestNoticeUpdate, OnFriendRequestNoticeUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildInviteNoticeUpdate, OnGuildInviteNoticeUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GoodsRecommend, OnPrivateOrderingNoticeUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MiddleFrameClose, _OnMiddleFrameClose);

            //理财计划
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FinancialPlanBuyRes, OnFinancialPlanUpdate);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerVipLvChanged, OnFinancialPlanUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FinancialPlanButtonUpdateByLevel, OnFinancialPlanUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FinancialPlanButtonUpdateByLogin, OnFinancialPlanUpdateByLogin);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FinancialPlanButtonUpdateBySceneChanged, OnFinancialPlanButtonUpdateBySceneChanged);

            // added by mjx on 170802
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HasLimitTimeGiftToBuy, OnNewLimitTimeGift);

            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TimeLessItemsChanged, _OnTimeLessItemsChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DeadLineReminderChanged, _OnTimeLessItemsChanged);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AuctionFreezeRemind, _OnAuctionFreezeRemind);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3CrossButton, OnPK3V3CrossButtonIsShow);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, _OnMoneyRewardsStatusChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SecurityLockApplyStateButton, OnSecurityLockApplyStateButtonIsShow);
            //公会兼并
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildReceiveMergerd, OnGuildReceiveMergerRequest);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefuseAllReceive, OnRefuseAllReciveRequest);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshInviteList, OnNotifyFriendInvite);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnNewPupilApplyRecieved, OnNotifyFriendInvite);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRecvPrivateChat, OnRecvPrivateChat);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnApplyPupilListChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnApplyTeacherListChanged, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTAPApplyToggleRedPointUpdate, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleChatDirtyChanged, OnPrivateChat);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRelationChanged, OnRelationChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTapPupilReportRedPoint, OnRelationChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTapTeacherSubmitRedPoint, OnRelationChanged);
        }

        void OnNewMailNoticeUpdate(UIEvent iEvent)
        {
            UpdateNewMailNotice();
        }

        void OnActivityNoticeUpdate(UIEvent iEvent)
        {
            UpdateActivityNotice();
        }

        void OnTeamNewInviteNoticeUpdate(UIEvent iEvent)
        {
            UpdateNewTeamInviteNotice();
        }


        //团本新的申请者消息
        private void OnTeamDuplicationRequestUpdate(UIEvent uiEvent)
        {
            UpdateTeamDuplicationRequest();
        }

        //团本新的邀请者消息
        private void OnTeamDuplicationNewTeamInviteUpdate(UIEvent uiEvent)
        {
            UpdateTeamDuplicationNewTeamInvite();
        }

        void OnRoomNewInviteNoticeUpdate(UIEvent iEvent)
        {
            //byte roomType = (byte)iEvent.Param1;
            //if(roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE || 
            //   roomType == (byte)RoomType.ROOM_TYPE_MELEE ||
            //   roomType == (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
            //{         
            //    UpdateNewRoomInviteNotice();
            //}
            //else if(roomType == (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
            //{
            //    UpdateNewRoomInviteNoticePk3v3Cross();
            //}

            UpdateNewRoomInviteNotice();
        }


        void OnSkillLvUpNoticeUpdate(UIEvent iEvent)
        {
            UpdateSkillLvUpNotice();
        }

        void OnFriendRequestNoticeUpdate(UIEvent iEvent)
        {
            UpdateFriendRequestNotice();
        }

        void OnGuildInviteNoticeUpdate(UIEvent iEvent)
        {
            //UpdateGuildInviteNotice();
            UpdateNewTeamInviteNotice();
        }

        void OnPrivateOrderingNoticeUpdate(UIEvent iEvent)
        {
            return;

            if(HideButtonIn3v3Cross(m_PrivateOrderingButton))
            {
                return;
            }

            if ((int)iEvent.Param1 == 1&& (bool)iEvent.Param3==true && MallDataManager.GetInstance().OnlineTips == false)
            {
#if APPLE_STORE
                //add for ios appstore
                if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_GIFT) == false)
				{
#endif
					if (_IsMiddleFrameOpen() && m_PrivateOrdering.gameObject.activeSelf == false)
                	{
                    	_AddReshowEffectFuncs(m_PrivateOrdering);
                	}
                	else
                	{
                    	m_PrivateOrderingButton.gameObject.CustomActive(true);
                	}
#if APPLE_STORE   
				}
#endif
                
                uint tableId = (uint)iEvent.Param2;

                MallItemTable mallTable = TableManager.GetInstance().GetTableItem<MallItemTable>((int)tableId);
                if (mallTable == null)
                {
                    Logger.LogError("Mall Table ID is null...");
                    return;
                }

                if (mallTable.PersonalTailID == 0)
                {
                    Logger.LogError("ÉÌ³ÇµÀ¾ß±íË½ÈË¶©ÖÆ±íidÎªÁã...");
                    return;
                }

                PersonalTailorTriggerTable ptttable = TableManager.GetInstance().GetTableItem<PersonalTailorTriggerTable>(mallTable.PersonalTailID);

                if (ptttable == null)
                {
                    Logger.LogError("Ë½ÈË¶©ÖÆ±íÎª¿Õ...");
                    return;
                }

                m_PrivateOrderingDes.text = ptttable.Describing;
            }
            else if ((int)iEvent.Param1 == 2 && MallDataManager.GetInstance().OnlineTips == true)
            {
                m_PrivateOrderingButton.gameObject.CustomActive(false);
            }
            
        }

        void _OnMiddleFrameClose(UIEvent iEvent)
        {
            for (int i = 0; i < NeedReshowEffectFuncs.Count; i++)
            {
                ComCommonBind bind = NeedReshowEffectFuncs[i];

                if (bind != null)
                {
                    //GameObject objroot = bind.GetGameObject("EffectRoot");

                    //if (objroot != null)
                    //{
                    //    ComEffectLoader effectloader = this.GetComponent<ComEffectLoader>();

                    //    if (effectloader != null)
                    //    {
                    //        effectloader.ActiveEffect(objroot.name);
                    //    }
                    //}
                    bind.CustomActive(true);
                }
            }

            NeedReshowEffectFuncs.Clear();
        }

        bool _IsMiddleFrameOpen()
        {
            DictionaryView<string, IClientFrame> allFrames = ClientSystemManager.GetInstance().GetAllFrames();

            var allframe = allFrames.GetEnumerator();

            while(allframe.MoveNext())
            {
                var frame = allframe.Current.Value as IClientFrame;

                if(frame == null)
                {
                    continue;
                }

                if(frame.GetLayer() == FrameLayer.Middle && frame.GetFrameType() == eFrameType.FullScreen)
                {
                    return true;
                }          
            }

            return false;
        }

        void _AddReshowEffectFuncs(ComCommonBind combind)
        {
            ComCommonBind bind = NeedReshowEffectFuncs.Find(value => { return value == combind; });

            if (bind == null && combind != null)
            {
                NeedReshowEffectFuncs.Add(combind);
            }
        }

        void UpdateNewMailNotice()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 || 
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3 ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                m_newMailNoticeButton.gameObject.SetActive(false);
                return;
            }

            if (scenedata.SceneType == CitySceneTable.eSceneType.PK_PREPARE)
            {
                m_newMailNoticeButton.gameObject.SetActive(false);
                return;
            }
            if(scenedata.SceneSubType==CitySceneTable.eSceneSubType.FairDuelPrepare)
            {
                m_newMailNoticeButton.gameObject.SetActive(false);
                return;
            }

            if (MailDataManager.UnReadMailNum > 0
                || MailDataManager.OneKeyReceiveNum > 0)
            {
                if (MailDataManager.GetInstance().mRewardMailTitleInfoList.Count > 0 ||
                    MailDataManager.GetInstance().mGuildMailTitleInfoList.Count > 0 ||
                    MailDataManager.GetInstance().mAnnouncementMailTitleInfoList.Count > 0)
                {
                    if (_IsMiddleFrameOpen() && m_newMailNotice.gameObject.activeSelf == false)
                    {
                        _AddReshowEffectFuncs(m_newMailNotice);
                        return;
                    }

                    m_newMailItemTip.gameObject.SetActive(true);
                }
                else
                {
                    m_newMailItemTip.gameObject.SetActive(false);
                }

                if (m_newMailNoticeButton.gameObject.activeSelf)
                {
                    return;
                }
                else
                {
                    if (_IsMiddleFrameOpen() && m_newMailNotice.gameObject.activeSelf == false)
                    {
                        _AddReshowEffectFuncs(m_newMailNotice);
                        return;
                    }

                    m_newMailNoticeButton.gameObject.SetActive(true);
                }
            }
            else
            {
                m_newMailNoticeButton.gameObject.SetActive(false);
            }

            //             var newMsgMgr = NewMessageNoticeManager.GetInstance();
            // 
            //             if (mDataVersion != newMsgMgr.DataVersion)
            //             {
            //                 mDataVersion = newMsgMgr.DataVersion;
            //                 m_newMessageNoticeObject.CustomActive(newMsgMgr.mNewMessageNoticeCount != 0);
            //                 int iCount = newMsgMgr.GetUnReadCount();
            //                 m_newMessageNoticeRedPoint.CustomActive(iCount > 0);
            //                 m_newMessageNoticeText.text = iCount.ToString();
            //             }
        }

        void UpdateActivityNotice()
        {
            return;

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if(scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 || 
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                m_ActivityNoticeButton.gameObject.SetActive(false);
                return;
            }

            if (scenedata.SceneType == CitySceneTable.eSceneType.PK_PREPARE)
            {
                m_ActivityNoticeButton.gameObject.SetActive(false);
                return;
            }

            if (ActivityNoticeDataManager.GetInstance().GetActivityNoticeDataList().Count > 0)
            {
                m_ActivityNoticeText.text = ActivityNoticeDataManager.GetInstance().GetActivityNoticeDataList().Count.ToString();
                if (_IsMiddleFrameOpen() && m_ActivityNotice.gameObject.activeSelf == false)
                {
                    _AddReshowEffectFuncs(m_ActivityNotice);
                }
                else
                {
                    m_ActivityNoticeButton.gameObject.SetActive(true);
                }
               
                
            }
            else
            {
                m_ActivityNoticeButton.gameObject.SetActive(false);
            }
        }

        void UpdateNewTeamInviteNotice()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }
       

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                m_TeamInviteButton.gameObject.SetActive(false);
                return;
            }

            if (scenedata.SceneType == CitySceneTable.eSceneType.PK_PREPARE)
            {
                m_TeamInviteButton.gameObject.SetActive(false);
                return;
            }

            List<NewTeamInviteList> InviteTeamList = TeamDataManager.GetInstance().GetInviteTeamList();
            List<WorldGuildInviteNotify> GuildInviteList = GuildDataManager.GetInstance().GuildInviteList;

            if (InviteTeamList.Count > 0 || GuildInviteList.Count > 0)
            {
                m_TeamInviteText.text = (InviteTeamList.Count + GuildInviteList.Count).ToString();

                if (_IsMiddleFrameOpen() && m_TeamInvite.gameObject.activeSelf == false)
                {
                    _AddReshowEffectFuncs(m_TeamInvite);
                }
                else
                {
                    m_TeamInviteButton.gameObject.SetActive(true);
                }
                
            }
            else
            {
                m_TeamInviteButton.gameObject.SetActive(false);
            }
        }

        //团本申请者更新
        private void UpdateTeamDuplicationRequest()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            //非团本场景，不显示
            if (scenedata.SceneSubType != CitySceneTable.eSceneSubType.TeamDuplicationBuid
                && scenedata.SceneSubType != CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                //m_TeamDuplicationRequestButton.gameObject.SetActive(false);
                CommonUtility.UpdateButtonVisible(m_TeamDuplicationRequestButton, false);
                return;
            }


            //拥有团本并且是团长
            if (TeamDuplicationUtility.IsTeamDuplicationOwnerTeam()
                && TeamDuplicationUtility.IsSelfPlayerIsTeamLeaderInTeamDuplication()
                && TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerNewRequester == true)
            {
                if (m_TeamDuplicationRequest != null)
                {
                    if (_IsMiddleFrameOpen() && m_TeamDuplicationRequest.gameObject.activeSelf == false)
                    {
                        _AddReshowEffectFuncs(m_TeamDuplicationRequest);
                    }
                    else
                    {
                        CommonUtility.UpdateButtonVisible(m_TeamDuplicationRequestButton,
                            true);
                    }
                }
            }
            else
            {
                //其他不显示
                CommonUtility.UpdateButtonVisible(m_TeamDuplicationRequestButton, false);
            }
        }

        //团本新的邀请者更新
        private void UpdateTeamDuplicationNewTeamInvite()
        {
            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            var citySceneTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (citySceneTable == null)
            {
                return;
            }

            //非团本场景，不显示
            if (citySceneTable.SceneSubType != CitySceneTable.eSceneSubType.TeamDuplicationBuid
                && citySceneTable.SceneSubType != CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                CommonUtility.UpdateButtonVisible(m_TeamDuplicationTeamInviteButton, false);
                return;
            }

            if (TeamDuplicationUtility.IsTeamDuplicationOwnerTeam() == true)
            {
                //拥有团本，不显示
                CommonUtility.UpdateButtonVisible(m_TeamDuplicationTeamInviteButton, false);
            }
            else
            {
                //没有团本
                //没有团本邀请的消息
                if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerNewInvite == false)
                {
                    CommonUtility.UpdateButtonVisible(m_TeamDuplicationTeamInviteButton, false);
                }
                else
                {
                    //存在团本邀请消息，展示按钮
                    if (m_TeamDuplicationTeamInvite != null)
                    {
                        if (_IsMiddleFrameOpen() && m_TeamDuplicationTeamInvite.gameObject.activeSelf == false)
                        {
                            _AddReshowEffectFuncs(m_TeamDuplicationTeamInvite);
                        }
                        else
                        {
                            CommonUtility.UpdateButtonVisible(m_TeamDuplicationTeamInviteButton,
                                true);
                        }
                    }
                }
            }
        }

        void UpdateNewRoomInviteNoticePk3v3Cross()
        {
            return;

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                m_CrossPK3v3RoomInvite.gameObject.SetActive(false);
                return;
            }

            List<WorldSyncRoomInviteInfo> InviteRoomList = Pk3v3CrossDataManager.GetInstance().GetInviteRoomList();

            if (InviteRoomList.Count > 0)
            {
                //m_RoomInviteText.text = InviteRoomList.Count.ToString();

                if (_IsMiddleFrameOpen() && m_CrossPK3v3RoomInvite.gameObject.activeSelf == false)
                {
                    _AddReshowEffectFuncs(m_CrossPK3v3RoomInvite);
                }
                else
                {
                    m_CrossPK3v3RoomInviteButton.gameObject.SetActive(true);
                }

            }
            else
            {
                m_CrossPK3v3RoomInviteButton.gameObject.SetActive(false);
            }
        }

        bool HideButtonIn3v3Cross(Button button)
        {
            if(button == null)
            {
                return false;
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return false;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return false;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 || 
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                button.gameObject.SetActive(false);
                return true;
            }

            return false;
        }

        void UpdateNewRoomInviteNotice()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if(scenedata.SceneSubType != CitySceneTable.eSceneSubType.TRADITION)
            {
                m_RoomInviteButton.gameObject.SetActive(false);
                return;
            }

            List<WorldSyncRoomInviteInfo> Pk3v3InviteRoomList = Pk3v3DataManager.GetInstance().GetInviteRoomList();
            List<WorldSyncRoomInviteInfo> Pk3v3CrossInviteRoomList = Pk3v3CrossDataManager.GetInstance().GetInviteRoomList();
            List<SceneSyncRequest> FriendsPlayInviteList = RelationDataManager.GetInstance().FriendsPlayInviteList;

            if (Pk3v3InviteRoomList.Count > 0 || Pk3v3CrossInviteRoomList.Count > 0 || FriendsPlayInviteList.Count > 0)
            {
                m_RoomInviteText.text = (Pk3v3InviteRoomList.Count + Pk3v3CrossInviteRoomList.Count + FriendsPlayInviteList.Count).ToString();

                if (_IsMiddleFrameOpen() && m_RoomInvite.gameObject.activeSelf == false)
                {
                    _AddReshowEffectFuncs(m_RoomInvite);
                }
                else
                {
                    m_RoomInviteButton.gameObject.SetActive(true);
                }

            }
            else
            {
                m_RoomInviteButton.gameObject.SetActive(false);
            }
        }


        private void OnFinancialPlanButtonUpdateBySceneChanged(UIEvent uiEvent)
        {
            UpdateFinancialPlanButtonState();
        }

        private void UpdateFinancialPlanButtonState()
        {
            return;

            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            if (systemTown == null)
                return;

            if (FinancialPlanDataManager.GetInstance().GetFinancialPlanButtonShowState() != FinancialPlanButtonState.IsShowing)
            {
                m_FinancialPlanButton.gameObject.CustomActive(false);
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                m_FinancialPlanButton.gameObject.SetActive(false);
                return;
            }

            if (FinancialPlanDataManager.GetInstance().GetFinancialPlanButtonShowState() == FinancialPlanButtonState.IsShowing)
            {
                m_FinancialPlanButton.gameObject.CustomActive(true);
                return;
            }

        }

        private void OnFinancialPlanUpdateByLogin(UIEvent uiEvent)
        {
            UpdateFinancialPlanByLogin();
        }

        /// <summary>
        /// 角色登录的时候是否现实理财计划的Icon
        /// </summary>
        private void UpdateFinancialPlanByLogin()
        {
            //FinancialPlanDataManager.GetInstance().SetFanancialPlanButtonShowState(false);
            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            if(systemTown == null)
                return;

            if (false == FinancialPlanDataManager.GetInstance().IsShowFinancialPlanButton())
            {
                m_FinancialPlanButton.gameObject.CustomActive(false);
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 || 
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                m_FinancialPlanButton.gameObject.SetActive(false);
                return;
            }

            if (true == FinancialPlanDataManager.GetInstance().IsPlayerAlreadyShowOnceFinancialPlanInLogin())
            {
                m_FinancialPlanButton.CustomActive(false);
                FinancialPlanDataManager.GetInstance().SetFinancialPlanButtonState(FinancialPlanButtonState.IsNotShowing);
                return;
            }

            if (_IsMiddleFrameOpen() && m_FinancialPlan.gameObject.activeSelf == false)
            {
                _AddReshowEffectFuncs(m_FinancialPlan);
            }
            else
            {
                m_FinancialPlanButton.gameObject.CustomActive(true);
                FinancialPlanDataManager.GetInstance().SetFinancialPlanButtonState(FinancialPlanButtonState.IsShowing);
            }
           
            m_FinancialPlanText.text = TR.Value("financial_plan_text");
            FinancialPlanDataManager.GetInstance().SetPlayerAlreadyShowFinancialPlanInLogin();
        }

        private void InitFinancialPlan()
        {
            //FinancialPlanDataManager.GetInstance().SetFanancialPlanButtonShowState(false);
            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            if (systemTown == null)
                return;

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 || 
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                m_FinancialPlanButton.gameObject.SetActive(false);
                return;
            }

            //不显示直接返回
            if (false == FinancialPlanDataManager.GetInstance().IsShowFinancialPlanButton())
            {
                m_FinancialPlanButton.gameObject.CustomActive(false);
                return;
            }

            if (_IsMiddleFrameOpen() && m_FinancialPlan.gameObject.activeSelf == false)
            {
                _AddReshowEffectFuncs(m_FinancialPlan);
            }
            else
            {
                if(FinancialPlanDataManager.GetInstance().GetFinancialPlanButtonShowState() == FinancialPlanButtonState.IsShowing
                    || FinancialPlanDataManager.GetInstance().GetFinancialPlanButtonShowState() == FinancialPlanButtonState.Invalid)
                {
                    m_FinancialPlanButton.gameObject.CustomActive(true);
                    FinancialPlanDataManager.GetInstance().SetFinancialPlanButtonState(FinancialPlanButtonState.IsShowing);
                }
            }

            m_FinancialPlanText.text = TR.Value("financial_plan_text");
        }

        private void OnFinancialPlanUpdate(UIEvent uiEvent)
        {
            UpdateFinancialPlan();
        }

        private void UpdateFinancialPlan()
        {
            //FinancialPlanDataManager.GetInstance().SetFanancialPlanButtonShowState(false);
            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            if(systemTown == null)
                return;

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 || 
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                m_FinancialPlanButton.gameObject.SetActive(false);
                return;
            }

            //不显示直接返回
            if (false == FinancialPlanDataManager.GetInstance().IsShowFinancialPlanButton())
            {
                m_FinancialPlanButton.gameObject.CustomActive(false);
                return;
            }

            if (_IsMiddleFrameOpen() && m_FinancialPlan.gameObject.activeSelf == false)
            {
                _AddReshowEffectFuncs(m_FinancialPlan);
            }
            else
            {
                //if(FinancialPlanDataManager.GetInstance().GetFinancialPlanButtonShowState() != FinancialPlanButtonState.AlreadyClicked)
                //{
                  m_FinancialPlanButton.gameObject.CustomActive(true);
                  FinancialPlanDataManager.GetInstance().SetFinancialPlanButtonState(FinancialPlanButtonState.IsShowing);
                //}
            }
           
            m_FinancialPlanText.text = TR.Value("financial_plan_text");
        }

        void UpdateSkillLvUpNotice()
        {
            return;

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 || scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3)
            {
                m_SkillLvUpButton.gameObject.SetActive(false);
                return;
            }
            if(scenedata.SceneSubType==CitySceneTable.eSceneSubType.FairDuelPrepare)
            {
                m_SkillLvUpButton.gameObject.SetActive(false);
                return;
            }
            ExpTable data = TableManager.GetInstance().GetTableItem<ExpTable>(PlayerBaseData.GetInstance().Level);
            if(data == null)
            {
                m_SkillLvUpButton.gameObject.SetActive(false);
            }
            else
            {
                if (PlayerBaseData.GetInstance().SP >= data.SpLeft)
                {
                    if (_IsMiddleFrameOpen() && m_SkillLvUp.gameObject.activeSelf == false)
                    {
                        _AddReshowEffectFuncs(m_SkillLvUp);
                    }
                    else
                    {
                        m_SkillLvUpButton.gameObject.SetActive(true);
                    }
                }
                else
                {
                    m_SkillLvUpButton.gameObject.SetActive(false);
                }
            }
            
            if(ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CrossWaitingRoom>())
            {
                m_SkillLvUpButton.gameObject.SetActive(false);
                m_CrossPk3V3Button.CustomActive(false);
            }
        }

        void UpdateFriendRequestNotice()
        {
            return;

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 || 
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3 ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                m_FriendRequestButton.gameObject.SetActive(false);
                return;
            }

            if (RelationDataManager.GetInstance().GetInviteFriendData().Count > 0)
            {
                m_FriendRequestText.text = RelationDataManager.GetInstance().GetInviteFriendData().Count.ToString();
                if (_IsMiddleFrameOpen() && m_FriendRequest.gameObject.activeSelf == false)
                {
                    _AddReshowEffectFuncs(m_FriendRequest);
                }
                else
                {
                    m_FriendRequestButton.gameObject.SetActive(true);
                }
               
            }
            else
            {
                m_FriendRequestButton.gameObject.SetActive(false);
            }
        }

        void UpdateGuildInviteNotice()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            if (HideButtonIn3v3Cross(m_GuildInviteButton))
            {
                return;
            }

            List<WorldGuildInviteNotify> GuildInviteList = GuildDataManager.GetInstance().GuildInviteList;
            if(GuildInviteList.Count > 0)
            {
                m_GuildInviteText.text = GuildInviteList.Count.ToString();
                m_GuildInviteButton.gameObject.SetActive(true);
            }
            else
            {
                m_GuildInviteButton.gameObject.SetActive(false);
            }
        }

        void UpdateItemTimeLess()
        {
            //ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            //if (systemTown == null)
            //{
            //    return;
            //}
           
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 || 
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                m_ItemTimeLess.gameObject.SetActive(false);
                return;
            }

            if (DeadLineReminderDataManager.GetInstance().GetDeadLineReminderModelList().Count > 0)
            {
                if (_IsMiddleFrameOpen() && m_ItemTimeLess.gameObject.activeSelf == false)
                {
                    _AddReshowEffectFuncs(m_ItemTimeLess);
                    return;
                }
            }

            m_ItemTimeLess.gameObject.SetActive(
                DeadLineReminderDataManager.GetInstance().GetDeadLineReminderModelList().Count > 0
                );
        }

        void OnClickNewMailNotice()
        {
            ClientSystemManager.instance.OpenFrame<MailNewFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("ComTalk_NewMailNotice");
        }

        void OnClickActivityNoticeButton()
        {
            ClientSystemManager.instance.OpenFrame<ActivityNoticeListFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("ComTalk_ActivityNotice");
        }

        void OnClickTeamInviteButton()
        {
            ClientSystemManager.instance.OpenFrame<GuildBeInvitedListFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("ComTalk_TeamInviteNotice");
        }

        private void OnClickTeamDuplicationRequestButton()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationRequesterListFrame();
        }

        private void OnClickTeamDuplicationTeamInviteButton()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationTeamInviteListFrame();
        }

        void OnClickRoomInviteButton()
        {
            ClientSystemManager.instance.OpenFrame<TeamBeInvitedListFrame>(FrameLayer.Middle, InviteType.Pk3v3Invite);
            GameStatisticManager.GetInstance().DoStartUIButton("ComTalk_RoomInviteNotice");
        }

        void OnCrossPK3v3RoomInviteButton()
        {
            ClientSystemManager.instance.OpenFrame<TeamBeInvitedListFrame>(FrameLayer.Middle, InviteType.Pk3v3Invite, "Pk3v3Cross");
            GameStatisticManager.GetInstance().DoStartUIButton("ComTalk_RoomInvitePk3v3Notice");
        }

        private void OnClickFinancialPlan()
        {
            if (m_FinancialPlanButton != null)
            {
                m_FinancialPlanButton.gameObject.CustomActive(false);
                FinancialPlanDataManager.GetInstance().SetFinancialPlanButtonState(FinancialPlanButtonState.AlreadyClicked);
            }
            FinancialPlanDataManager.GetInstance().ShowFinancialPlanActivity();
        }

        void OnClickSkillLvUpButton()
        {
            ClientSystemManager.instance.OpenFrame<SkillFrame>(FrameLayer.Middle);

            //m_SkillLvUpButton.gameObject.SetActive(false);
            GameStatisticManager.GetInstance().DoStartUIButton("ComTalk_SkillLvUpNotice");
        }

        void OnClickFriendRequestButton()
        {
            RelationFrameNew.CommandOpen(new RelationFrameData
            {
                eRelationOptionType = RelationOptionType.ROT_MY_FRIEND
            });

            m_FriendRequestButton.gameObject.SetActive(false);
            GameStatisticManager.GetInstance().DoStartUIButton("ComTalk_FrendRequestNotice");
        }

        void OnClickGuildInviteButton()
        { 
            ClientSystemManager.instance.OpenFrame<GuildBeInvitedListFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("ComTalk_GuildInviteNotice");
        }
        
        void OnClickItemTimeLessButton()
        {
            //ClientSystemManager.instance.OpenFrame<TimeLessItemsFrame>(FrameLayer.Middle, new TimeLessItemsFrameData
            //{
            //    onCloseFrame = () =>
            //    {
            //        ItemDataManager.GetInstance().SendDeleteTimeLessItemsNotify();
            //    },
            //});
            ClientSystemManager.GetInstance().OpenFrame<DeadLineReminderFrame>(FrameLayer.Middle);
            GameStatisticManager.GetInstance().DoStartUIButton("ComTalk_ItemTimeLessNotice");
        }


        void OnClickPrivateOrderingButton()
        {
            ClientSystemManager.instance.OpenFrame<GoodsRecommendFrame>(FrameLayer.Middle);
            m_PrivateOrderingButton.gameObject.CustomActive(false);
            GameStatisticManager.GetInstance().DoStartUIButton("ComTalk_PrivateOrderingNotice");
        }

        void OnAuctinFreezeRemindClick()
        {
            AuctionNewDataManager.GetInstance().SendSceneAbnormalTransactionRecordReq();
        }

        void OpenJoinPK3V3CrossFrame()
        {
            ClientSystemManager.GetInstance().OpenFrame<JoinPK3v3CrossFrame>(FrameLayer.Middle);
            //m_CrossPk3V3Button.gameObject.CustomActive(false);
        }

        void OpenSettingFrameWithApplyState()
        {
            ClientSystemManager.GetInstance().OpenFrame<SettingFrame>(FrameLayer.Middle, SettingFrame.TabType.ACCOUNT_LOCK);
            m_SecurityLockApplyStateButton.gameObject.CustomActive(false);
            SecurityLockDataManager.GetInstance().BtnClickedCount++;
        }
        void OnClickLimitTimeGiftButton()
        {
            ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.m_LimitTimeGiftIsClick = true;
            var mallNewFrame = ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.LimitTimeMall }) as MallNewFrame;
            if (m_LimitTimeGift)
                m_LimitTimeGift.CustomActive(false);

            GameStatisticManager.GetInstance().DoStartUIButton("ComTalk_LimitTimeNotice");
        }
        /// <summary>
        /// 工会兼并的请求
        /// </summary>
        private void OnGuildMergeAskBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildMergeAskFrame>(FrameLayer.Middle);
        }
        #region LimitTimeGift in Mall  

        void OnNewLimitTimeGift(UIEvent uiEvent)
        {
            UpdateLimitTimeGiftNotice();
        }

        void UpdateLimitTimeGiftNotice()
        {
#if APPLE_STORE
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_GIFT))
            {
                return;
            }
#endif
            return;

            if (ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.m_LimitTimeGiftIsClick)
            {
                return;
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 || 
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                m_LimitTimeGift.CustomActive(false);
                return;
            }

            if (ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.HasNewGiftPackOrToBuy() == true)
            {
                if (_IsMiddleFrameOpen() && m_LimitTimeGift.gameObject.activeSelf == false)
                {
                    _AddReshowEffectFuncs(m_LimitTimeGift);
                    return;
                }
            }

            if (m_LimitTimeGift)
                m_LimitTimeGift.CustomActive(ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.HasNewGiftPackOrToBuy());
        }

        #endregion

        private Vector2 currComTalkOriginalPos;
        private float voiceBtnWidth;

        void ChangeThisScrollRectActive(UIEvent uiEvent)
        {
            if (uiEvent != null && uiEvent.Param1 != null)
            {
                bool isActive = (bool)uiEvent.Param1;
                ScrollRect rect = m_root.GetComponent<ScrollRect>();
                if (rect)
                {
                    rect.enabled = isActive;
                }
            }
        }

        void _OnTimeLessItemsChanged(UIEvent a_event)
        {
            UpdateItemTimeLess();
        }

        void _OnAuctionFreezeRemind(UIEvent a_event)
        {
            if(HideButtonIn3v3Cross(m_AuctionFreezeRemindButton))
            {
                return;
            }

            bool mIsFlag = (bool)a_event.Param1;

            if (mIsFlag == true)
            {
                if (_IsMiddleFrameOpen() && m_AuctionFreezeRemind.gameObject.activeSelf == false)
                {
                    _AddReshowEffectFuncs(m_AuctionFreezeRemind);
                    return;
                }
            }

            m_AuctionFreezeRemindButton.CustomActive(mIsFlag);
        }

        void OnSecurityLockApplyStateButtonIsShow(UIEvent uiEvent)
        {
            return;

            if (HideButtonIn3v3Cross(m_SecurityLockApplyStateButton))
            {
                return;
            }
            m_SecurityLockApplyStateButton.CustomActive(false);
            SecurityLockState lockState = SecurityLockDataManager.GetInstance().GetSecurityLockData().lockState;
            if(lockState == SecurityLockState.SECURITY_STATE_APPLY && SecurityLockDataManager.GetInstance().BtnClickedCount == 0)
            {
                m_SecurityLockApplyStateButton.CustomActive(true);
            }
        }
        void OnPK3V3CrossButtonIsShow(UIEvent uiEvent)
        {
            return;

            string strHideBtn = uiEvent.Param1 as string;
            if(strHideBtn != null)
            {
                m_CrossPk3V3Button.CustomActive(false);
                return;
            }

            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_SCORE_WAR_LEVEL);
            if (SystemValueTableData != null && PlayerBaseData.GetInstance().Level < SystemValueTableData.Value)
            {
                m_CrossPk3V3Button.CustomActive(false);
                return;
            }
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 || 
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid ||
                scenedata.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
            {
                m_CrossPk3V3Button.CustomActive(false);
                return;
            }

            byte status = (byte)uiEvent.Param1;

            bool isFlag = false;
            if (status >= (byte)ScoreWarStatus.SWS_PREPARE && status < (byte)ScoreWarStatus.SWS_WAIT_END)
            {
                isFlag = true;
            }
            else
            {
                isFlag = false;
            }

            m_CrossPk3V3Button.CustomActive(isFlag);

            if(ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CrossWaitingRoom>())
            {
                m_CrossPk3V3Button.CustomActive(false);
            }
        }

        void _OnMoneyRewardsStatusChanged(UIEvent uiEvent)
        {
            var mBind = m_ActivityNoticeButton.gameObject.GetComponent<ComCommonBind>();
            if (mBind != null)
            {
                GameObject mEffectRoot = mBind.GetGameObject("Effect");
                if (mEffectRoot != null)
                {
                    mEffectRoot.CustomActive(false);
                    //var activityList = ActivityNoticeDataManager.GetInstance().GetActivityNoticeDataList();
                    //for (int i = 0; i < activityList.Count; i++)
                    //{
                    //    if (activityList[i].type == (uint)NotifyType.NT_MONEY_REWARDS)
                    //    {
                    //        mEffectRoot.CustomActive(true);
                    //    }
                    //}
                    if (MoneyRewardsDataManager.GetInstance().Status == PremiumLeagueStatus.PLS_ENROLL)
                    {
                        mEffectRoot.CustomActive(true);
                    }
                }
            }
        }

        private void OnGuildReceiveMergerRequest(UIEvent uiEvent)
        {
            mGuildMerge.CustomActive(GuildDataManager.GetInstance().IsHaveMergedRequest);
        }
        private void OnRefuseAllReciveRequest(UIEvent uiEvent)
        {
            if(!GuildDataManager.GetInstance().IsAgreeMergerRequest)
            {
                mGuildMerge.CustomActive(false);
            }
        }
        private void _InitVoiceChat()
        {
            teamVoiceBtnGo = Utility.FindChild (m_root, "voiceBtnListRoot/TeamVoiceBtn");
            guildVoiceBtnGo = Utility.FindChild (m_root, "voiceBtnListRoot/GuildVoiceBtn");

            if(voiceBtnGoList != null)
            {
                voiceBtnGoList.Add(teamVoiceBtnGo);
                voiceBtnGoList.Add(guildVoiceBtnGo);
            }

            RectTransform rect = (transform as RectTransform);
            if (rect == null)
                return;
            currComTalkOriginalPos = rect.anchoredPosition;
            //默认隐藏
            SetVoiceBtnsActive(false,false);


            comVoiceChat.Init(ComVoiceChat.ComVoiceChatType.Global);
        }

        private void _UnInitVoiceChat()
        {
            comVoiceChat.UnInit();
        }

        void InitRedPoint()
        {
            _CheckFriendRedPoint();
        }

        void OnNotifyFriendInvite(UIEvent uiEvent)
        {
            _CheckFriendRedPoint();
        }

        void OnRecvPrivateChat(UIEvent uiEvent)
        {
            _CheckFriendRedPoint();
        }

        void _OnRefreshInviteList(UIEvent uiEvent)
        {
            _CheckFriendRedPoint();
        }

        void OnPrivateChat(UIEvent iEvent)
        {
            _CheckFriendRedPoint();
        }

        void OnRelationChanged(UIEvent mEvent)
        {
            _CheckFriendRedPoint();
        }

        void _CheckFriendRedPoint()
        {
            List<InviteFriendData> inviteFriends = RelationDataManager.GetInstance().GetInviteFriendData();
            bool haveApply = TAPNewDataManager.GetInstance().HaveApplyRedPoint();
            bool haveSubmit = TAPNewDataManager.GetInstance().HaveSubmitRedPoint();
            bool canReport = TAPNewDataManager.GetInstance().HaveReportRedPoint();
            bool canLeave = TAPNewDataManager.GetInstance().HaveLeaveMasterRedPoint();
            if (null != inviteFriends && inviteFriends.Count > 0 || haveApply || haveSubmit || canReport || canLeave)
            {
                mFriendRedPoint.CustomActive(true);
            }
            else
            {
                mFriendRedPoint.CustomActive(false);
            }

            if (RelationDataManager.GetInstance().GetPriDirty() || RelationDataManager.GetInstance().GetFriendPriDirty())
            {
                mPrivateChatBubble.CustomActive(true);
            }
            else
            {
                mPrivateChatBubble.CustomActive(false);
            }
        }

        private void SetVoiceBtnsActive(bool isTeamShow, bool isGuildShow)
        {
            teamVoiceBtnGo.CustomActive(isTeamShow);
            guildVoiceBtnGo.CustomActive(isGuildShow);            

            if (isTeamShow && isGuildShow && showMode == ShowMode.Normal) // normal模式下,如果组队和公会话筒都可以使用，则默认显示公会话筒 
            {
                teamVoiceBtnGo.CustomActive(false);              
            }        

            if (showMode == ShowMode.Normal) // 正常模式
            {
                selectVoice.CustomActive(isTeamShow || isGuildShow);               
            }
            else if(showMode == ShowMode.Expand) // 扩展模式
            {
                selectVoice.CustomActive(false);               
            }

            if (isTeamShow)
            {
                curSelectVoiceGo = teamVoiceBtnGo;
            }

            if (isGuildShow)
            {
                curSelectVoiceGo = guildVoiceBtnGo;
            }

            if(!isTeamShow && !isGuildShow)
            {
                curSelectVoiceGo = null;
            }

            if (curSelectVoiceGo != null)
            {
                curSelectVoiceGo.transform.SetAsLastSibling();
            }

            showVoiceListRoot = false;
            voiceListRoot.CustomActive(showVoiceListRoot);

            selectTeamVoice.CustomActive(isTeamShow);
            selectGuildVoice.CustomActive(isGuildShow);

            bool bMin = isTeamShow || isGuildShow;
            ResetTalkContentWidth(bMin);
        }

        private void  ResetTalkContentWidth(bool bMin)
        {
            ComTalkExtraParam param = this.gameObject.GetComponent<ComTalkExtraParam>();
            if(param == null)
            {
                return;
            }

            float anchorPos = bMin ? param.anchorPos0 : param.anchorPos1; 
            RectTransform rect = param.talkContent;
            if (null != rect)
            {
                rect.offsetMin = new Vector2(anchorPos, rect.offsetMin.y);
            }
        }
    }
}