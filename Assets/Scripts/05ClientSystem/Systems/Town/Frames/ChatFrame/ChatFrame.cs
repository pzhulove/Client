using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using ProtoTable;
using Protocol;

namespace GameClient
{
    public class ChatFrameData
    {
        public RelationData curPrivate = null;
        public ChatType eChatType = ChatType.CT_WORLD;
    }
    public class ChatFrame : ClientFrame
    {
        bool m_bFirstEnter = false;
        static string ms_holdText = string.Empty;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chat/ChatFrame";
        }

        ChatFrameData data = new ChatFrameData();
        ComChatList comChatList = new ComChatList();
        
        [UIControl("ScrollviewBg/Scrollview", typeof(ScrollRect))]
        ScrollRect scrollrect;
        [UIControl("", typeof(ComScriptBinder))]
        ComScriptBinder mScriptBinder;

        Button ChangeInputMode = null;

        ComVoiceChat mComVoiceChat = null;

        protected override void _OnOpenFrame()
        {
            if(null != (userData as ChatFrameData))
            {
                data = userData as ChatFrameData;
            }
            m_inputField.text = ms_holdText;
            m_bFirstEnter = true;
            m_akTabObjects.Clear();
            m_akEmotionObjects.Clear();
            m_inputFieldRoot = Utility.FindGameObject(frame, "ScrollviewBg/InputField");
            
            m_closeRoot = Utility.FindGameObject(frame, "ScrollviewBg/close");
            ChatManager.GetInstance().onAddChatdata += OnAddChatData;
            ChatManager.GetInstance().onRebuildChatData += OnRebuildChatData;
            m_inputField.onValueChanged.AddListener(OnValueChanged);
            scrollrect.onValueChanged.AddListener(OnScrollrectBarChanged);

            ChatType[] chatTypes = new ChatType[] {
                ChatType.CT_WORLD ,
                ChatType.CT_NORMAL,
                ChatType.CT_ACOMMPANY,
                ChatType.CT_TEAM_COPY_PREPARE,
                ChatType.CT_TEAM_COPY_TEAM,
                ChatType.CT_TEAM_COPY_SQUAD};
            _RegisterThisComIntervalGroup(chatTypes, Utility.FindComponent<ComFunctionInterval>(frame, "ScrollviewBg/InputField/Send"));
            _RegisterThisComIntervalGroup(chatTypes, Utility.FindComponent<ComFunctionInterval>(frame, "ScrollviewBg/InputField/VoiceInput/VoiceSend"));

            _InitChatObject();

            comChatList.Initialize(this, Utility.FindChild(frame, "ScrollviewBg"), data);

            mComVoiceChat = mBind.GetCom<ComVoiceChat>("VoiceChat");
            if(mComVoiceChat)
            {
                mComVoiceChat.Init(ComVoiceChat.ComVoiceChatType.Frame, true, is_satisfied_with_talk_condition, _IsRecordVoiceLimited);
            }
            StaticUtility.SafeSetVisible(mBind, "VoiceSend", false);

            ChangeInputMode = mBind.GetCom<Button>("ChangeInputMode");
            ChangeInputMode.SafeAddOnClickListener(OnChangeInputMode);

            showVoiceInput = false;

            _InitEmotionBag();
            _InitTabs();

            _RegisterUIEvent();


            var find = m_akTabObjects.Find(x =>
            {
                return x.Value.eChatType == ChatType.CT_PRIVATE_LIST;
            });

            if(find != null)
            {
                bool dirty = RelationDataManager.GetInstance().GetPriDirty();
                find.SetRedPointActive(dirty);
            }

            Input.onEndEdit.AddListener(_OnInputEndEdit);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnChatFrameStatusChanged,true);

            RelationDataManager.GetInstance().QueryPlayerOnlineStatus();
            RefreshFriendIcon();

            #region TeamCopy
            //团本相关
            bool bIsTeamCopyScene = TeamDuplicationUtility.IsInTeamDuplicationScene();
            if (bIsTeamCopyScene || BeUtility.IsRaidBattle()) 
            {
                if(TeamDuplicationUtility.IsTeamDuplicationOwnerTeam())
                {
                    SetTab(ChatType.CT_TEAM_COPY_TEAM);
                }
                else
                {
                    SetTab(ChatType.CT_TEAM_COPY_PREPARE);
                }
            }
            else
            {
                if(data.eChatType == ChatType.CT_TEAM_COPY_PREPARE || data.eChatType == ChatType.CT_TEAM_COPY_TEAM || data.eChatType == ChatType.CT_TEAM_COPY_SQUAD)
                {
                    SetTab(ChatType.CT_WORLD);
                }               
            }

            //团本渠道不显示
            if (bIsTeamCopyScene == true)
            {
                StaticUtility.SafeSetVisible(mBind, "voiceInput", false);
                StaticUtility.SafeSetVisible(mBind, "Laba", false);
            }
            #endregion

        }

        //注册按钮响应
        void _RegisterThisComIntervalGroup(ChatType[] chatTypes, ComFunctionInterval comFuncInterval)
        {
            if (null == chatTypes || chatTypes.Length < 0)
            {
                return;
            }
            if (null == comFuncInterval)
            {
                return;
            }
            for (int i = 0; i < chatTypes.Length; i++)
            {
                ComIntervalGroup.GetInstance().Register(this, (int)chatTypes[i], comFuncInterval);
            }
        }

        bool showVoiceInput = false;
        void OnChangeInputMode()
        {
            showVoiceInput = !showVoiceInput;
            UpdateChangeInputMode();
        }

        void UpdateChangeInputMode()
        {
            StaticUtility.SafeSetVisible(mBind, "VoiceSend", showVoiceInput);
            StaticUtility.SafeSetVisible(mBind, "inputText", !showVoiceInput);
            StaticUtility.SafeSetVisible(mBind, "Emotion", !showVoiceInput);
            StaticUtility.SafeSetVisible(mBind, "Send", !showVoiceInput);
        }

        void _UpdateInputHoldplaceText()
        {
            Text holdText = m_inputField.placeholder as Text;
            if(holdText != null)
            {
                if(data.eChatType == ChatType.CT_WORLD)
                {
                    var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WORLD_CHAT_LV_LIMIT);
                    if(systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                    {
                        holdText.text = string.Format(TR.Value("chat_world_talk_need_level"), systemValue.Value);
                        return;
                    }

                    int iFreeTimes = ChatManager.GetInstance().FreeWorldChatLeftTimes;
                    if(iFreeTimes > 0)
                    {
                        float vipValue = Utility.GetCurVipLevelPrivilegeData((int)ProtoTable.VipPrivilegeTable.eType.WORLD_CHAT_FREE_TIMES);
                        holdText.text = string.Format(TR.Value("chat_world_first_hold_place"),
                            PlayerBaseData.GetInstance().VipLevel,
                            iFreeTimes,
                            vipValue);
                        return;
                    }

                    int iActivityValue = PlayerBaseData.GetInstance().ActivityValue;
                    int iCostValue = ChatManager.WorldChatCostActivityValue;
                    holdText.text = string.Format(TR.Value("chat_world_talk_need_activity_value"), iCostValue, iActivityValue, iCostValue);
                    return;
                }
                else if(data.eChatType == ChatType.CT_ACOMMPANY)
                {
                    var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_TEAM_CHAT_LV_LIMIT);
                    if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                    {
                        holdText.text = string.Format(TR.Value("chat_team_talk_need_level"), systemValue.Value);
                        return;
                    }
                }
                else if(data.eChatType == ChatType.CT_TEAM)
                {
                    bool bHasTeam = TeamDataManager.GetInstance().HasTeam();
                    if(frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
                    {
                        bHasTeam = Pk3v3CrossDataManager.GetInstance().HasTeam();
                    }

                    if (!bHasTeam)
                    {
                        holdText.text = TR.Value("chat_team_talk_need_team");
                        return;
                    }
                }
                else if(data.eChatType == ChatType.CT_TEAM_COPY_TEAM)
                {
                    bool flag = TeamDuplicationUtility.IsTeamDuplicationOwnerTeam() || BeUtility.IsRaidBattle();                    
                    if (!flag)
                    {
                        holdText.text = TR.Value("chat_failed_for_has_no_team_copy_team");
                        return;
                    }
                }
                else if (data.eChatType == ChatType.CT_TEAM_COPY_SQUAD)
                {
                    bool flag = TeamDuplicationUtility.IsTeamDuplicationOwnerTeam() || BeUtility.IsRaidBattle();
                    if (!flag)
                    {
                        holdText.text = TR.Value("chat_failed_for_has_no_team_copy_squad");
                        return;
                    }
                }
                else if (data.eChatType == ChatType.CT_NORMAL)
                {
                    var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_AROUND_CHAT_LV_LIMIT);
                    if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                    {
                        holdText.text = string.Format(TR.Value("chat_normal_talk_need_level"), systemValue.Value);
                        return;
                    }
                }

                holdText.text = TR.Value("chat_generation_hint");
            }
        }

        bool CheckFunction()
        {
            return data != null && data.eChatType == ChatType.CT_WORLD;
        }

        protected override void _OnCloseFrame()
        {
            TabObject.Clear();
            m_akTabObjects.DestroyAllObjects();
            m_akEmotionObjects.Clear();
            m_inputField.onClick -= this.OnClickInputField;
            m_inputField.onValueChanged.RemoveListener(OnValueChanged);

            ChatManager.GetInstance().onAddChatdata -= OnAddChatData;
            ChatManager.GetInstance().onRebuildChatData -= OnRebuildChatData;
            _UnRegisterUIEvent();

            Input.onEndEdit.RemoveListener(_OnInputEndEdit);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnChatFrameStatusChanged, false);

            comChatList.UnInitialize();
            scrollrect.onValueChanged.RemoveListener(OnScrollrectBarChanged);
            newMessageNumber = 0;
            ComIntervalGroup.GetInstance().UnRegister(this);

            ChangeInputMode.SafeRemoveOnClickListener(OnChangeInputMode);
            showVoiceInput = false;

            if(mComVoiceChat)
            {
                mComVoiceChat.UnInit();
                mComVoiceChat = null;
            }
        }

        protected void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnShowFriendSecMenu, _OnShowFrienSecInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCloseMenu, _OnCloseMenu);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPrivateChat, _OnPrivateChat);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdatePrivate, _OnUpdatePrivate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnDelPrivate, _OnDelPrivate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPrivateRdChanged,_OnPrivateRdChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleChatDirtyChanged, _OnRoleChatDirtyChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPlayerOnLineStatusChanged, _OnPlayerOnLineStatusChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCounterChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SetChatTab, _OnSetChatTab);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LockStatuNewMessageNumber, _OnLockStatuNewMessageNumberChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefreshChatData, _RefreshChatData);

            UnityEngine.UI.Button btnJoinTeam = mScriptBinder.GetObject((int)ComScriptLabel.Label_ChatFrame_Button_JoinTeam) as UnityEngine.UI.Button;
            if(null != btnJoinTeam)
            {
                btnJoinTeam.onClick.AddListener(() =>
                {
                    if (ChijiDataManager.GetInstance().CheckCurrentSystemIsClientSystemGameBattle())
                    {
                        SystemNotifyManager.SysNotifyTextAnimation("本场景无法加入队伍");
                        return;
                    }

                    if(frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
                    {
                        ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossTeamListFrame>(FrameLayer.Middle);
                        frameMgr.CloseFrame(this);
                        return;
                    }

                    var data = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.Team);
                    if (data == null)
                    {
                        return;
                    }

                    if (PlayerBaseData.GetInstance().Level < data.FinishLevel)
                    {
                        SystemNotifyManager.SystemNotify(1300031);
                        return;
                    }

                    TeamListFrame.TryOpenTeamListOrTeamMyFrame();
                    //ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>(FrameLayer.Middle);
                    frameMgr.CloseFrame(this);
                });
            }

            UnityEngine.UI.Button btnJoinGuild = mScriptBinder.GetObject((int)ComScriptLabel.Label_ChatFrame_Button_JoinGuild) as UnityEngine.UI.Button;
            if (null != btnJoinGuild)
            {
                btnJoinGuild.onClick.AddListener(() =>
                {
                    var data = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)15);
                    if (data == null)
                    {
                        return;
                    }

                    if (PlayerBaseData.GetInstance().Level < data.FinishLevel)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guild_not_open_need_lv"),CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                        return;
                    }

                    ClientSystemManager.GetInstance().OpenFrame<GuildListFrame>(FrameLayer.Middle);
                    frameMgr.CloseFrame(this);
                });
            }

            UnityEngine.UI.Button btnJoinTeamCopy = mScriptBinder.GetObject((int)ComScriptLabel.Label_ChatFrame_Button_JoinTeamCopy) as UnityEngine.UI.Button;
            if (null != btnJoinTeamCopy)
            {
                btnJoinTeamCopy.SafeSetOnClickListener(() =>
                {
                    ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationTeamListFrame> (FrameLayer.Middle);
                    frameMgr.CloseFrame(this);
                });
            }
        }

        protected void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnShowFriendSecMenu, _OnShowFrienSecInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCloseMenu, _OnCloseMenu);
           //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPrivateChat, _OnPrivateChat);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdatePrivate, _OnUpdatePrivate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDelPrivate, _OnDelPrivate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPrivateRdChanged, _OnPrivateRdChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleChatDirtyChanged, _OnRoleChatDirtyChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPlayerOnLineStatusChanged, _OnPlayerOnLineStatusChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCounterChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SetChatTab, _OnSetChatTab);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LockStatuNewMessageNumber, _OnLockStatuNewMessageNumberChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshChatData, _RefreshChatData);
        }

        IClientFrame m_openMenu;
        protected void _OnShowFrienSecInfo(UIEvent uiEvent)
        {
            UIEventShowFriendSecMenu myEvent = uiEvent as UIEventShowFriendSecMenu;

            bool bCanOpen = true;

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null)
                {
                    if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS && scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossGuildBattle)
                    {
                        bCanOpen = false;
                    }
                }
            }

            if(bCanOpen)
            {
                if(!Pk3v3DataManager.HasInPk3v3Room() && !Pk3v3CrossDataManager.GetInstance().CheckPk3v3CrossScence())
                {
                    //如果好友界面打开了，commenu不打开
                    if (ClientSystemManager.GetInstance().IsFrameOpen<RelationFrameNew>())
                    {
                        return;
                    }

                    m_openMenu = frameMgr.OpenFrame<RelationMenuFram>(FrameLayer.Middle, myEvent.m_data);
                }                
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect("跨服公会战场景下无法查看玩家信息");
            }     
        }

        protected void _OnCloseMenu(UIEvent uiEvent)
        {
            if (m_openMenu != null)
            {
                frameMgr.CloseFrame(m_openMenu);
                m_openMenu = null;
            }
        }


        #region tabs
        GameObject m_goTabParent;
        GameObject m_goTabPrefab;
        List<CommonPlayerInfo> m_chatPlayerList = new List<CommonPlayerInfo>();
        GameObject m_inputFieldRoot;
        GameObject m_closeRoot;

        [UIControl("ScrollviewBg/InputField/Button/Input")]
        OnFocusInputField Input;

        [UIControl("Bottom/Text",typeof(LinkParse))]
        LinkParse friendlyHint;

        class ChatTypeData
        {
            public ChatType eChatType;
            public string content;
        }

        class TabObject : CachedSelectedObject<ChatTypeData,TabObject>
        {
            Text labelText;
            Text labelMarkText;
            Image m_redPt;
            GameObject goCheckMark;

            public override void Initialize()
            {
                labelText = Utility.FindComponent<Text>(goLocal, "Label");
                labelMarkText = Utility.FindComponent<Text>(goLocal, "Checkmark/Label");
                m_redPt = Utility.FindComponent<Image>(goLocal, "RedPt");
                m_redPt.CustomActive(false);
                goCheckMark = Utility.FindChild(goLocal, "Checkmark");
            }

            public override void UnInitialize()
            {

            }

            public override void OnUpdate()
            {
                if(Value != null)
                {
                    if (labelText != null)
                    {
                        labelText.text = Value.content;
                    }
                    if (labelMarkText != null)
                    {
                        labelMarkText.text = Value.content;
                    }
                }
            }

            public override void OnDisplayChanged(bool bShow)
            {
                if(goCheckMark != null)
                {
                    goCheckMark.CustomActive(bShow);
                }
            }

            public void SetRedPointActive(bool active)
            {
                m_redPt.gameObject.SetActive(active);
            }
        }

        CachedObjectListManager<TabObject> m_akTabObjects = new CachedObjectListManager<TabObject>();

        void _InitTabs()
        {
            m_goTabParent = Utility.FindChild(frame, "tabs");
            m_goTabPrefab = Utility.FindChild(m_goTabParent, "tab");
            m_goTabPrefab.CustomActive(false);
            for (int i = (int)ChatType.CT_ALL; i < (int)ChatType.CT_MAX_WORDS; ++i)
            {
                ChatType eCurChatType = (ChatType)ChatManager.GetMapIndex(i);
                if(eCurChatType == ChatType.CT_PK3V3_ROOM)
                {
                    continue;
                }

                string content = string.Empty;
                if (eCurChatType == ChatType.CT_PRIVATE)
                {
                    continue;
                }
                if (eCurChatType == ChatType.CT_PRIVATE_LIST)
                {
                    continue;
                }
                //if (eCurChatType != ChatType.CT_PRIVATE)
                //{
                //    content = Utility.GetEnumDescription(eCurChatType);
                //}
                //else
                //{
                //    content = _GetPrivateChatTabContent();
                //}

                content = Utility.GetEnumDescription(eCurChatType);

                var current = m_akTabObjects.Create(new object[]
                {
                        m_goTabParent,
                        m_goTabPrefab,
                        new ChatTypeData { eChatType = eCurChatType, content = content },
                        Delegate.CreateDelegate(typeof(TabObject.OnSelectedDelegate),this,"OnFilterChanged"),
                        false,
                });

                if (_NeedShowTab(eCurChatType))
                {
                    current.Enable();
                }
                else
                {
                    current.Disable();
                }
            }

            var find = m_akTabObjects.Find(x =>
            {
                return x.Value.eChatType == this.data.eChatType;
            });

            if(find != null)
            {
                find.OnSelected();
            }
        }

        bool _NeedShowTab(ChatType tabType)
        {
            bool bIsTeamCopyScene = TeamDuplicationUtility.IsInTeamDuplicationScene();
            bool bIsRaidBattle = BeUtility.IsRaidBattle();
            if (tabType == ChatType.CT_TEAM_COPY_PREPARE || tabType == ChatType.CT_TEAM_COPY_TEAM || tabType == ChatType.CT_TEAM_COPY_SQUAD)
            {
                return bIsTeamCopyScene || bIsRaidBattle;
            }
            else
            {
                if(bIsTeamCopyScene || bIsRaidBattle)
                {
                    return false;
                }
            }

            if (tabType == ChatType.CT_PRIVATE && data.curPrivate == null)
            {
                return false;
            }

            if(tabType == ChatType.CT_ALL)
            {
                return false;
            }

            return true;
        }

        public void SetPrivateChatTab(RelationData data)
        {
            this.data.eChatType = ChatType.CT_PRIVATE;
            this.data.curPrivate = data;
            SetTab(ChatType.CT_PRIVATE);
        }

        string _GetPrivateChatTabContent()
        {
            string tmpname = "CT_PRIVATE";
            if(data != null && data.curPrivate != null)
            {
                /* 
                int len = 4;
                if (data.curPrivate.name != null && data.curPrivate.name.Length > len)
                {
                    tmpname = string.Format("{0}...", data.curPrivate.name.Substring(0, len));
                }
                else*/
                {
                    tmpname = data.curPrivate.name;
                }
            }
            return tmpname;
        }

        public void SetTab(ChatType eType)
        {
            var find = m_akTabObjects.Find(x => { return x.Value.eChatType == eType; });
            if(find != null)
            {
                find.Enable();
                find.OnSelected();
            }
        }

        void _UpdateFriendlyHint()
        {
            if(null != friendlyHint)
            {
                if(null != mScriptBinder)
                {
                    mScriptBinder.SetAction((int)ComScriptLabel.Label_ChatFrame_State_None);
                }
                if (this.data.eChatType == ChatType.CT_GUILD)
                {
                    if(!GuildDataManager.GetInstance().HasSelfGuild())
                    {
                        friendlyHint.SetText(TR.Value("chat_failed_for_has_no_guild"));
                        if (null != mScriptBinder)
                        {
                            mScriptBinder.SetAction((int)ComScriptLabel.Label_ChatFrame_State_NoGuild);
                        }
                    }
                    else
                    {
                        friendlyHint.SetText(TR.Value("chat_succeed"));
                    }
                }
                else if (this.data.eChatType == ChatType.CT_TEAM)
                {
                    if(!TeamDataManager.GetInstance().HasTeam())
                    {
                        friendlyHint.SetText(TR.Value("chat_failed_for_has_no_team"));
                        if (null != mScriptBinder)
                        {
                            mScriptBinder.SetAction((int)ComScriptLabel.Label_ChatFrame_State_NoTeam);
                        }
                    }
                    else
                    {
                        friendlyHint.SetText(TR.Value("chat_succeed"));
                    }
                }
                else if (this.data.eChatType == ChatType.CT_TEAM_COPY_TEAM)
                {
                    if(!TeamDuplicationUtility.IsTeamDuplicationOwnerTeam())
                    {
                        friendlyHint.SetText(TR.Value("chat_failed_for_has_no_team_copy_team"));
                        if (null != mScriptBinder)
                        {
                            mScriptBinder.SetAction((int)ComScriptLabel.Label_ChatFrame_State_NoTeamCopy);
                        }
                    }
                    else
                    {
                        friendlyHint.SetText(TR.Value("chat_succeed"));
                    }
                }
                else if (this.data.eChatType == ChatType.CT_TEAM_COPY_SQUAD)
                {
                    if (!TeamDuplicationUtility.IsTeamDuplicationOwnerTeam())
                    {
                        friendlyHint.SetText(TR.Value("chat_failed_for_has_no_team_copy_squad"));
                        if (null != mScriptBinder)
                        {
                            mScriptBinder.SetAction((int)ComScriptLabel.Label_ChatFrame_State_NoTeamCopy);
                        }
                    }
                    else
                    {
                        friendlyHint.SetText(TR.Value("chat_succeed"));
                    }
                }
                else if (this.data.eChatType == ChatType.CT_ACOMMPANY)
                {
                    friendlyHint.SetText(TR.Value("chat_failed_for_accompany"));
                }
                else if (this.data.eChatType == ChatType.CT_PRIVATE_LIST)
                {
                    friendlyHint.SetText(TR.Value("chat_failed_for_private"));
                }
                else if (this.data.eChatType == ChatType.CT_PRIVATE)
                {
                    friendlyHint.SetText(TR.Value("chat_succeed"));
                }
                else if (this.data.eChatType == ChatType.CT_SYSTEM)
                {
                    friendlyHint.SetText(TR.Value("chat_failed_for_system"));
                }
                else if(this.data.eChatType == ChatType.CT_NORMAL)
                {
                    friendlyHint.SetText(TR.Value("chat_succeed"));

                    if (!ChatManager.GetInstance().EnableNormalChat())
                    {
                        friendlyHint.SetText(TR.Value("chat_normal_talk_scene_not_allow"));
                    }
                }
                else if (this.data.eChatType == ChatType.CT_WORLD)
                {
                    friendlyHint.SetText(TR.Value("chat_succeed"));
                }
                else
                {
                    friendlyHint.SetText(TR.Value("chat_failed"));
                }
            }
        }

        void OnFilterChanged(ChatTypeData chatTypeData)
        {
            this.data.eChatType = chatTypeData.eChatType;

            _SwitchContent(true);
            _UpdateWorldChatCoolDown();
            _UpdateNormalChatCoolDown();
            _UpdateTeamCopyPrepareChatCoolDown();
            _UpdateTeamCopyTeamChatCoolDown();
            _UpdateTeamCopySquadChatCoolDown();
            _UpdateAccompanyChatCoolDown();
            _UpdateDefaultCoolDown();

            _ShowInputFiled();
            _UpdateInputHoldplaceText();
            _UpdateFriendlyHint();
            RelationDataManager.GetInstance().SetCurPriChatUid(0);

            if (this.data.eChatType == ChatType.CT_GUILD)
            {
                if (GuildDataManager.GetInstance().HasSelfGuild() == false)
                {
                    m_kTextChatHint.CustomActive(true);
                    m_kTextChatHint.text = TR.Value("enchant_no_gang");
                }
                else
                {
                    m_kTextChatHint.CustomActive(false);
                }

                m_inputFieldRoot.SetActive(GuildDataManager.GetInstance().HasSelfGuild());
            }
            else if (this.data.eChatType == ChatType.CT_TEAM)
            {
                if(frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
                {
                    m_kTextChatHint.CustomActive(!Pk3v3CrossDataManager.GetInstance().HasTeam());
                    m_kTextChatHint.text = TR.Value("enchant_no_team");

                    m_inputFieldRoot.SetActive(Pk3v3CrossDataManager.GetInstance().HasTeam());
                }
                else
                {
                    m_kTextChatHint.CustomActive(!TeamDataManager.GetInstance().HasTeam());
                    m_kTextChatHint.text = TR.Value("enchant_no_team");

                    m_inputFieldRoot.SetActive(TeamDataManager.GetInstance().HasTeam());
                }
            }
            else if (this.data.eChatType == ChatType.CT_TEAM_COPY_TEAM)
            {
                m_kTextChatHint.CustomActive(!(TeamDuplicationUtility.IsTeamDuplicationOwnerTeam()||BeUtility.IsRaidBattle()));
                m_kTextChatHint.text = TR.Value("chat_failed_for_has_no_team_copy_team");

                m_inputFieldRoot.SetActive(TeamDuplicationUtility.IsTeamDuplicationOwnerTeam() || BeUtility.IsRaidBattle());
            }
            else if (this.data.eChatType == ChatType.CT_TEAM_COPY_SQUAD)
            {
                m_kTextChatHint.CustomActive(!(TeamDuplicationUtility.IsTeamDuplicationOwnerTeam() || BeUtility.IsRaidBattle()));
                m_kTextChatHint.text = TR.Value("chat_failed_for_has_no_team_copy_squad");

                m_inputFieldRoot.SetActive(TeamDuplicationUtility.IsTeamDuplicationOwnerTeam() || BeUtility.IsRaidBattle());
            }
            else if(this.data.eChatType == ChatType.CT_ACOMMPANY)
            {
                m_inputFieldRoot.SetActive(true);
                m_kTextChatHint.CustomActive(false);
                m_inputFieldRoot.SetActive(false);
            }
            else if (this.data.eChatType == ChatType.CT_PRIVATE_LIST)
            {
                m_kTextChatHint.CustomActive(false);
                _AddPrivateChatPlayerList();
            }
            else if (this.data.eChatType == ChatType.CT_PRIVATE)
            {
                m_kTextChatHint.CustomActive(false);
                if(this.data.curPrivate != null)
                {
                    RelationDataManager.GetInstance().ClearPriChatDirty(this.data.curPrivate.uid);
                }

                var find = m_akTabObjects.Find(x =>
                {
                    return x.Value.eChatType == ChatType.CT_PRIVATE;
                });

                if (find != null)
                {
                    find.OnRefresh(new object[] { new ChatTypeData { eChatType = data.eChatType, content = _GetPrivateChatTabContent() } });
                }
            }
            else if (this.data.eChatType == ChatType.CT_NORMAL)
            {
                if (!ChatManager.GetInstance().EnableNormalChat())
                {
                    m_inputFieldRoot.SetActive(false);
                    m_kTextChatHint.CustomActive(true);
                    m_kTextChatHint.text = TR.Value("chat_normal_talk_scene_not_allow");
                }
            }
            else
            {
                m_kTextChatHint.CustomActive(false);
            }

            friendlyHint.CustomActive(!m_inputFieldRoot.activeSelf);
            screenLocked = false;
            MarkChatDataDirty();
        }

        void _ShowInputFiled()
        {
            if(this.data.eChatType == ChatType.CT_ALL || this.data.eChatType == ChatType.CT_SYSTEM ||
                this.data.eChatType == ChatType.CT_PRIVATE_LIST)
            {
                m_inputFieldRoot.SetActive(false);
            }
            else
            {
                m_inputFieldRoot.SetActive(true);
            }
        }

        public void SetTalkContent(string content)
        {
            if(m_inputFieldRoot.activeSelf)
            {
                Input.text = content;
            }
        }

        void _AddPrivateChatPlayerList()
        {
            _ClearChatPlayerList();

            List<PrivateChatPlayerData> list = new List<PrivateChatPlayerData>();
            var srclist = RelationDataManager.GetInstance().GetPriChatList();
            list.AddRange(srclist);
            list.Sort((x, y) =>
            {
                if((x.chatNum > 0) != (y.chatNum > 0))
                {
                    return x.chatNum > 0 ? -1 : 1;
                }

                if(x.relationData.isOnline != y.relationData.isOnline)
                {
                    return x.relationData.isOnline == 1 ? -1 : 1;
                }

                if(x.iOrder != y.iOrder)
                {
                    return y.iOrder - x.iOrder;
                }

                return x.relationData.uid < y.relationData.uid ? -1 : (x.relationData.uid == y.relationData.uid ? 0 : 1);
            });

            UnityEngine.GameObject content = mScriptBinder.GetObject((int)ComScriptLabel.Label_ChatFrame_chatListContent) as UnityEngine.GameObject;
            for (int i = 0; i < list.Count; ++i)
            {
                CommonPlayerInfo chatPlayer = new CommonPlayerInfo(list[i].relationData.uid, list[i].relationData.name,
                    list[i].relationData.level, list[i].relationData.occu, CommonPlayerInfo.CommonPlayerType.CPT_PRIVAT_CHAT, 
                    list[i].chatNum > 0 ? true : false, list[i].relationData.isOnline, list[i].relationData.type, list[i].relationData.vipLv);

                m_chatPlayerList.Add(chatPlayer);

                chatPlayer.m_friendPrefab.transform.SetParent(content.transform, false);
            }
            _SwitchContent(false);
        }

        void _SwitchContent(bool isChatContent)
        {
            UnityEngine.GameObject chatListContent = mScriptBinder.GetObject((int)ComScriptLabel.Label_ChatFrame_chatListContent) as UnityEngine.GameObject;

            if (isChatContent)
            {
                chatListContent.CustomActive(false);
            }
            else
            {
                chatListContent.CustomActive(true);
            }
        }

        void _UpdateWorldChatCoolDown()
        {
            if (data.eChatType == ChatType.CT_WORLD)
            {
                if (ChatManager.world_cool_time > 0)
                {
                    ComIntervalGroup.GetInstance().BeginInvoke(this, (int)ChatType.CT_WORLD, ChatManager.world_cool_time);
                    ComIntervalGroup.GetInstance().DisableFunction(this, (int)ChatType.CT_WORLD);
                }
                else
                {
                    ComIntervalGroup.GetInstance().EnableFunction(this, (int)ChatType.CT_WORLD);
                }
            }
        }

        void _UpdateNormalChatCoolDown()
        {
            if (data.eChatType == ChatType.CT_NORMAL)
            {
                if (ChatManager.arround_cool_time > 0)
                {
                    ComIntervalGroup.GetInstance().BeginInvoke(this, (int)ChatType.CT_NORMAL, ChatManager.arround_cool_time);
                    ComIntervalGroup.GetInstance().DisableFunction(this, (int)ChatType.CT_NORMAL);
                }
                else
                {
                    ComIntervalGroup.GetInstance().EnableFunction(this, (int)ChatType.CT_NORMAL);
                }
            }
        }

        void _UpdateTeamCopyPrepareChatCoolDown()
        {
            if (data.eChatType == ChatType.CT_TEAM_COPY_PREPARE)
            {
                if (ChatManager.teamcopy_prepare_cool_time > 0)
                {
                    ComIntervalGroup.GetInstance().BeginInvoke(this, (int)ChatType.CT_TEAM_COPY_PREPARE, ChatManager.teamcopy_prepare_cool_time);
                    ComIntervalGroup.GetInstance().DisableFunction(this, (int)ChatType.CT_TEAM_COPY_PREPARE);
                }
                else
                {
                    ComIntervalGroup.GetInstance().EnableFunction(this, (int)ChatType.CT_TEAM_COPY_PREPARE);
                }
            }
        }

        void _UpdateTeamCopyTeamChatCoolDown()
        {
            if (data.eChatType == ChatType.CT_TEAM_COPY_TEAM)
            {
                if (ChatManager.teamcopy_team_cool_time > 0)
                {
                    ComIntervalGroup.GetInstance().BeginInvoke(this, (int)ChatType.CT_TEAM_COPY_TEAM, ChatManager.teamcopy_team_cool_time);
                    ComIntervalGroup.GetInstance().DisableFunction(this, (int)ChatType.CT_TEAM_COPY_TEAM);
                }
                else
                {
                    ComIntervalGroup.GetInstance().EnableFunction(this, (int)ChatType.CT_TEAM_COPY_TEAM);
                }
            }
        }

        void _UpdateTeamCopySquadChatCoolDown()
        {
            if (data.eChatType == ChatType.CT_TEAM_COPY_SQUAD)
            {
                if (ChatManager.teamcopy_squad_cool_time > 0)
                {
                    ComIntervalGroup.GetInstance().BeginInvoke(this, (int)ChatType.CT_TEAM_COPY_SQUAD, ChatManager.teamcopy_squad_cool_time);
                    ComIntervalGroup.GetInstance().DisableFunction(this, (int)ChatType.CT_TEAM_COPY_SQUAD);
                }
                else
                {
                    ComIntervalGroup.GetInstance().EnableFunction(this, (int)ChatType.CT_TEAM_COPY_SQUAD);
                }
            }
        }

        void _UpdateAccompanyChatCoolDown()
        {
            if (data.eChatType == ChatType.CT_ACOMMPANY)
            {
                if (ChatManager.accompany_cool_time > 0)
                {
                    ComIntervalGroup.GetInstance().BeginInvoke(this, (int)ChatType.CT_ACOMMPANY, ChatManager.accompany_cool_time);
                    ComIntervalGroup.GetInstance().DisableFunction(this, (int)ChatType.CT_ACOMMPANY);
                }
                else
                {
                    ComIntervalGroup.GetInstance().EnableFunction(this, (int)ChatType.CT_ACOMMPANY);
                }
            }
        }

        void _UpdateDefaultCoolDown()
        {
            if (this.data.eChatType != ChatType.CT_ACOMMPANY &&
                this.data.eChatType != ChatType.CT_NORMAL &&
                this.data.eChatType != ChatType.CT_WORLD &&
                this.data.eChatType != ChatType.CT_TEAM_COPY_PREPARE &&
                this.data.eChatType != ChatType.CT_TEAM_COPY_TEAM && 
                this.data.eChatType != ChatType.CT_TEAM_COPY_SQUAD)
            {
                ComIntervalGroup.GetInstance().EnableFunction(this, (int)ChatType.CT_ACOMMPANY);
                ComIntervalGroup.GetInstance().EnableFunction(this, (int)ChatType.CT_NORMAL);
                ComIntervalGroup.GetInstance().EnableFunction(this, (int)ChatType.CT_WORLD);
                ComIntervalGroup.GetInstance().EnableFunction(this, (int)ChatType.CT_TEAM_COPY_PREPARE);
                ComIntervalGroup.GetInstance().EnableFunction(this, (int)ChatType.CT_TEAM_COPY_TEAM);
                ComIntervalGroup.GetInstance().EnableFunction(this, (int)ChatType.CT_TEAM_COPY_SQUAD);
            }
        }

        void _ClearChatPlayerList()
        {
            for (int i = 0; i < m_chatPlayerList.Count; ++i)
            {
                CGameObjectPool.instance.RecycleGameObject(m_chatPlayerList[i].m_friendPrefab);
            }

            m_chatPlayerList.Clear();
        }

        void _RefreshPrivateChatList(ref List<PrivateChatPlayerData> list)
        {

        }

        #endregion

        #region chatobjects
        [UIControl("ScrollviewBg/Scrollview/TipText", typeof(Text))]
        Text m_kTextChatHint;
        
        void _InitChatObject()
        {
            m_inputField.onClick += this.OnClickInputField;
        }

        void OnClickInputField()
        {
            m_goEmotionTab.CustomActive(false);
            m_closeRoot.SetActive(false);
        }

        void OnValueChanged(string value)
        {
            ms_holdText = value;
        }

        bool screenLocked = false;
        public bool ScreenLocked
        {
            get
            {
                return screenLocked;
            }
            set
            {
                if(screenLocked != value)
                {
                    screenLocked = value;
                    if (!value)
                    {
                        MarkChatDataDirty();
                    }
                }
            }
        }

        void OnScrollrectBarChanged(Vector2 vector)
        {
            if (vector.y <= 0.001f)
            {
                ScreenLocked = false;
                goNewMessageBumber.CustomActive(false);
                newMessageNumber = 0;
                //Logger.LogErrorFormat("ScreenLocked = {0} vector.y = {1}", ScreenLocked, vector.y);
            }
            else
            {
                ScreenLocked = true;
                //Logger.LogErrorFormat("ScreenLocked = {0} vector.y = {1}", ScreenLocked, vector.y);
            }
        }
        #endregion

        #region sendNetMsg
        [UIControl("ScrollviewBg/InputField/Button/Input", typeof(OnFocusInputField))]
        OnFocusInputField m_inputField;

        public bool world_chat_try_enter_cool_down()
        {
            if (data.eChatType == ChatType.CT_WORLD)
            {
                if (!ChatManager.world_chat_try_enter_cool_down())
                {
                    return false;
                }
            }

            ComIntervalGroup.GetInstance().BeginInvoke(this, (int)ChatType.CT_WORLD, ChatManager.ms_world_cool_down);
            ComIntervalGroup.GetInstance().DisableFunction(this, (int)ChatType.CT_WORLD);

            return true;
        }

        private bool arround_chat_try_enter_cool_down()
        {
            if(data.eChatType == ChatType.CT_NORMAL)
            {
                if(!ChatManager.arround_chat_try_enter_cool_down())
                {
                    return false;
                }
            }

            ComIntervalGroup.GetInstance().BeginInvoke(this, (int)ChatType.CT_NORMAL, ChatManager.ms_arround_cool_down);
            ComIntervalGroup.GetInstance().DisableFunction(this, (int)ChatType.CT_NORMAL);
            return true;
        }

        private bool teamcopy_prepare_chat_try_enter_cool_down()
        {
            if (data.eChatType == ChatType.CT_TEAM_COPY_PREPARE)
            {
                if (!ChatManager.teamcopy_prepare_chat_try_enter_cool_down())
                {
                    return false;
                }
            }

            ComIntervalGroup.GetInstance().BeginInvoke(this, (int)ChatType.CT_TEAM_COPY_PREPARE, ChatManager.ms_teamcopy_prepare_cool_down);
            ComIntervalGroup.GetInstance().DisableFunction(this, (int)ChatType.CT_TEAM_COPY_PREPARE);
            return true;
        }

        private bool teamcopy_team_chat_try_enter_cool_down()
        {
            if (data.eChatType == ChatType.CT_TEAM_COPY_TEAM)
            {
                if (!ChatManager.teamcopy_team_chat_try_enter_cool_down())
                {
                    return false;
                }
            }

            ComIntervalGroup.GetInstance().BeginInvoke(this, (int)ChatType.CT_TEAM_COPY_TEAM, ChatManager.ms_teamcopy_team_cool_down);
            ComIntervalGroup.GetInstance().DisableFunction(this, (int)ChatType.CT_TEAM_COPY_TEAM);
            return true;
        }

        private bool teamcopy_squad_chat_try_enter_cool_down()
        {
            if (data.eChatType == ChatType.CT_TEAM_COPY_SQUAD)
            {
                if (!ChatManager.teamcopy_squad_chat_try_enter_cool_down())
                {
                    return false;
                }
            }

            ComIntervalGroup.GetInstance().BeginInvoke(this, (int)ChatType.CT_TEAM_COPY_SQUAD, ChatManager.ms_teamcopy_squad_cool_down);
            ComIntervalGroup.GetInstance().DisableFunction(this, (int)ChatType.CT_TEAM_COPY_SQUAD);
            return true;
        }

        private bool accompany_chat_try_enter_cool_down()
        {
            if(data.eChatType == ChatType.CT_ACOMMPANY)
            {
                if(!ChatManager.accompany_chat_try_enter_cool_down())
                {
                    return false;
                }
            }

            ComIntervalGroup.GetInstance().BeginInvoke(this, (int)ChatType.CT_ACOMMPANY, ChatManager.ms_accompany_cool_down);
            ComIntervalGroup.GetInstance().DisableFunction(this, (int)ChatType.CT_ACOMMPANY);
            return true;
        }

        private bool is_satisfied_with_talk_condition()
        {
            if(data.eChatType == ChatType.CT_WORLD)
            {
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WORLD_CHAT_LV_LIMIT);
                if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_world_talk_need_level"), systemValue.Value));
                    return false;
                }

                int iFreeTimes = ChatManager.GetInstance().FreeWorldChatLeftTimes;
                if (iFreeTimes <= 0)
                {
                    if (!ChatManager.GetInstance().CheckWorldActivityValueEnough())
                    {
                        SystemNotifyManager.SystemNotify(7006, () =>
                        {
                            ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle, VipTabType.VIP);
                        });
                        return false;
                    }
                }

                if (!world_chat_try_enter_cool_down())
                {
                    return false;
                }
            }
            else if (data.eChatType == ChatType.CT_ACOMMPANY)
            {
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_TEAM_CHAT_LV_LIMIT);
                if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_team_talk_need_level"), systemValue.Value));
                    return false;
                }

                if(!accompany_chat_try_enter_cool_down())
                {
                    return false;
                }
            }
            else if (data.eChatType == ChatType.CT_NORMAL)
            {
                //var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_AROUND_CHAT_LV_LIMIT);
                //if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                //{
                //    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_normal_talk_need_level"), systemValue.Value));
                //    return false;
                //}

                if(!arround_chat_try_enter_cool_down())
                {
                    return false;
                }
            }
            else if(data.eChatType == ChatType.CT_TEAM)
            {
                if(frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
                {
                    if (!Pk3v3CrossDataManager.GetInstance().HasTeam())
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("chat_team_talk_need_team"));
                        return false;
                    }
                }
                else
                {
                    if (!TeamDataManager.GetInstance().HasTeam())
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("chat_team_talk_need_team"));
                        return false;
                    }
                }               
            }
            else if(data.eChatType == ChatType.CT_TEAM_COPY_PREPARE)
            {
                if (!teamcopy_prepare_chat_try_enter_cool_down())
                {
                    return false;
                }
            }
            else if (data.eChatType == ChatType.CT_TEAM_COPY_TEAM)
            {
                if (!teamcopy_team_chat_try_enter_cool_down())
                {
                    return false;
                }
            }
            else if (data.eChatType == ChatType.CT_TEAM_COPY_SQUAD)
            {
                if (!teamcopy_squad_chat_try_enter_cool_down())
                {
                    return false;
                }
            }

            return true;
        }

        [UIEventHandle("ScrollviewBg/InputField/Send")]
        void OnClickSendChatContent()
        {
            if (!string.IsNullOrEmpty(m_inputField.text) && m_inputField.text.Length < ChatData.CD_MAX_WORDS)
            {
                UInt64 uid = this.data.curPrivate == null ? 0 : this.data.curPrivate.uid;

#if UNITY_STANDALONE_WIN
                if (!is_satisfied_with_talk_condition() && !m_inputField.text.StartsWith("!!"))
                {
                    return;
                }
#else
                if(!is_satisfied_with_talk_condition())
                {
                    return;
                }
#endif

                if(frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>() && this.data.eChatType == ChatType.CT_TEAM)
                {
                    ChatManager.GetInstance().SendChat(ChatType.CT_PK3V3_ROOM, GetFliterSizeString(m_inputField.text), uid);
                }
                else
                {
                    ChatManager.GetInstance().SendChat(this.data.eChatType, GetFliterSizeString(m_inputField.text), uid);
                }
                
                m_inputField.text = "";
            }
            else
            {
                if(m_inputField.text != null && m_inputField.text.Length >= ChatData.CD_MAX_WORDS)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("chat_too_many_words"));
                }                
            }
        }

        public static string GetFliterSizeString(string str)
        {
            //〈color=red〉red〈/color〉
            str = str.Replace('<', '〈');
            str = str.Replace('>', '〉');

            return str;
        }


        [UIEventHandle("ScrollviewBg/InputField/VoiceInput/Emotion")]
        void OnClickEmotionBag()
        {
            m_goEmotionTab.SetActive(!m_goEmotionTab.activeSelf);
        }
#endregion

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
            protected ChatFrame THIS;

            Image emotion;
            Button button;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                spriteAssetInfo = param[2] as SpriteAssetInfor;
                THIS = param[3] as ChatFrame;

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
                THIS.AddChatText("{F " + string.Format("{0}",spriteAssetInfo.ID) + "}");
            }
        }

        CachedObjectDicManager<int, EmotionObject> m_akEmotionObjects = new CachedObjectDicManager<int, EmotionObject>();

        void _InitEmotionBag()
        {
            m_goEmotionTab = Utility.FindChild(frame, "ScrollviewBg/EmotionTab");
            m_goEmotionTab.CustomActive(false);
            m_goEmotionPrefab = Utility.FindChild(m_goEmotionTab, "Emotion");
            m_goEmotionPrefab.CustomActive(false);

            m_spriteAsset = AssetLoader.instance.LoadRes("UI/Image/Emotion/emotion.asset", typeof(SpriteAsset)).obj as SpriteAsset;
            if(m_spriteAsset != null && m_spriteAsset.listSpriteAssetInfor != null)
            {
                for (int i = 0; i < m_spriteAsset.listSpriteAssetInfor.Count; ++i)
                {
                    var spriteAssetInfo = m_spriteAsset.listSpriteAssetInfor[i];
                    if(spriteAssetInfo != null)
                    {
                        m_akEmotionObjects.Create(i, new object[] { m_goEmotionTab , m_goEmotionPrefab , spriteAssetInfo ,this});
                    }
                }
            }
        }

#endregion

        void AddChatText(string content)
        {
            if(!string.IsNullOrEmpty(content))
            {
                m_inputField.text += content;
            }
        }

        void OnRebuildChatData(ulong preGuid,ChatBlock chatBlock)
        {
            if(chatBlock.chatData.eChatType == data.eChatType)
            {
                MarkChatDataDirty();
            }
        }

        void OnAddChatData(ChatBlock chatBlock)
        {
            if (chatBlock.chatData.eChatType == ChatType.CT_PRIVATE)
            {
                return;
            }
            if(chatBlock.chatData.eChatType == data.eChatType)
            {
                MarkChatDataDirty();
            }
        }

        bool m_bDirty = false;
        void RefreshChatData()
        {
            comChatList.RefreshChatData(data);
            scrollrect.verticalNormalizedPosition = 0;
        }

        void MarkChatDataDirty()
        {
            if(screenLocked)
            {
                return;
            }
            m_bDirty = true;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if(m_bDirty)
            {
                RefreshChatData();
                m_bDirty = false;
            }
        }

        void _InitCurrentChatData()
        {
            for (int i = (int)ChatType.CT_ALL; i < (int)ChatType.CT_MAX_WORDS; ++i)
            {
                List<ChatBlock> chatDatas = null;
                if (i == (int)ChatType.CT_PRIVATE && this.data.curPrivate != null)
                {

                    chatDatas = ChatManager.GetInstance().GetPrivateChat(this.data.curPrivate.uid);
                        
                }
                else if(i == (int)ChatType.CT_PRIVATE)
                {
                    _AddPrivateChatPlayerList();
                }
                else
                {
                    chatDatas = ChatManager.GetInstance().GetChatDataByChanelType(ChatManager.GetInstance()._TransChatType((ChatType)i));
                    
                }

                if (chatDatas != null)
                {
                    var values = new List<ChatBlock>();
                    values.AddRange(chatDatas);
                    values.Sort((x, y) =>
                    {
                        return x.iOrder - y.iOrder;
                    });
                    for (int j = 0; j < values.Count; ++j)
                    {
                        OnAddChatData(values[j]);
                    }
                }
                
            }
        }

        [UIEventHandle("ScrollviewBg/close")]
        void OnClickClose()
        {
            m_goEmotionTab.SetActive(false);
            frameMgr.CloseFrame(this);
        
        }

        private void _OnPrivateChat(UIEvent uiEvent)
        {
            UIEventPrivateChat myEvent = uiEvent as UIEventPrivateChat;
            if(true == myEvent.m_recvChat)
            {
                bool dirty = RelationDataManager.GetInstance().GetPriDirty();
                var find = m_akTabObjects.Find(x =>
                {
                    return ChatType.CT_PRIVATE_LIST == x.Value.eChatType;
                });

                if(find != null && find.isOn)
                {
                    _AddPrivateChatPlayerList();
                }
            }
            else
            {
                SetPrivateChatTab(myEvent.m_data);
            }
            
        }

        private void _OnUpdatePrivate(UIEvent uiEvent)
        {
            var find = m_akTabObjects.Find(x =>
            {
                return x.Value.eChatType == ChatType.CT_PRIVATE_LIST;
            });
            if(find != null && find.isOn)
            {
                _AddPrivateChatPlayerList();
            }
        }

        private void _OnDelPrivate(UIEvent uiEvent)
        {
            UIEventDelPrivate myEvent = uiEvent as UIEventDelPrivate;

            var find = m_akTabObjects.Find(x =>
            {
                return x.Value.eChatType == ChatType.CT_PRIVATE_LIST;
            });

            if(find != null && find.isOn)
            {
                _AddPrivateChatPlayerList();
            }

            if (null != this.data && null != this.data.curPrivate && myEvent.m_uid == this.data.curPrivate.uid)
            {
                var tabPrivate = m_akTabObjects.Find(x => { return x.Value.eChatType == ChatType.CT_PRIVATE; });
                if (tabPrivate != null)
                {
                    if (tabPrivate.isOn)
                    {
                        TabObject.Clear();
                    }
                    tabPrivate.Disable();
                    data.curPrivate = null;
                }
            }

            var tab = m_akTabObjects.Find(x => { return x.Value.eChatType == ChatType.CT_PRIVATE_LIST; });
            if(null != tab)
            {
                tab.SetRedPointActive(RelationDataManager.GetInstance().GetPriDirty());
            }
        }

        private void _OnPrivateRdChanged(UIEvent uiEvent)
        {
            data = uiEvent.Param1 as ChatFrameData;
            if(data != null)
            {
                SetTab(data.eChatType);
            }
        }

        private void _OnRoleChatDirtyChanged(UIEvent uiEvent)
        {
            //ulong uid = (ulong)uiEvent.Param1;
            //bool bDirty = (bool)uiEvent.Param2;
            //var tab = m_akTabObjects.ActiveObjects.Find(x => { return x.Value.eChatType == ChatType.CT_PRIVATE_LIST; });
            //if(tab != null)
            //{
            //    var privateTab = m_akTabObjects.ActiveObjects.Find(x => { return x.Value.eChatType == ChatType.CT_PRIVATE; });
            //    if(privateTab != null && privateTab.isOn && data.curPrivate != null && data.curPrivate.uid == uid && bDirty)
            //    {
            //        RelationDataManager.GetInstance().ClearPriChatDirty(uid);
            //    }
            //    tab.SetRedPointActive(RelationDataManager.GetInstance().GetPriDirty());
            //}
            RefreshFriendIcon();
        }

        void _OnPlayerOnLineStatusChanged(UIEvent uiEvent)
        {
            var find = m_akTabObjects.Find(x =>
            {
                return x.Value.eChatType == ChatType.CT_PRIVATE_LIST;
            });

            if (find != null && find.isOn)
            {
                _AddPrivateChatPlayerList();
            }
        }

        void _OnCounterChanged(UIEvent uiEvent)
        {
            if((uiEvent.Param1 as string) == CounterKeys.COUNTER_ACTIVITY_VALUE ||
                (uiEvent.Param1 as string) == CounterKeys.COUNTER_WORLD_FREE_CHAT_TIMES)
            {
                _UpdateInputHoldplaceText();
            }
        }

        void _OnSetChatTab(UIEvent uiEvent)
        {
            ChatType eTab = (ChatType)uiEvent.Param1;
            SetTab(eTab);
        }

        void _OnLockStatuNewMessageNumberChanged(UIEvent uiEvent)
        {
            var data = uiEvent.Param1 as ChatBlock;
            if (data.chatData.channel == (int)ChanelType.CHAT_CHANNEL_WORLD)
            {
                RefreshNewMessageBumber();
            }
        }

        void _RefreshChatData(UIEvent uiEvent)
        {
            //MarkChatDataDirty();
            m_bDirty = true;
        }
        

        private void _OnInputEndEdit(string str)
        {
            m_closeRoot.SetActive(true);
        }

        private bool _IsRecordVoiceLimited()
        {
            if (data.eChatType == ChatType.CT_WORLD)
            {
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WORLD_CHAT_LV_LIMIT);
                if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_world_talk_need_level"), systemValue.Value));
                    return true;
                }

                int iFreeTimes = ChatManager.GetInstance().FreeWorldChatLeftTimes;
                if (iFreeTimes <= 0)
                {
                    if (!ChatManager.GetInstance().CheckWorldActivityValueEnough())
                    {
                        SystemNotifyManager.SystemNotify(7006, () =>
                            {
                                ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle, VipTabType.VIP);
                            });
                        return true;
                    }
                }
            }
            else if (data.eChatType == ChatType.CT_ACOMMPANY)
            {
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_TEAM_CHAT_LV_LIMIT);
                if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_team_talk_need_level"), systemValue.Value));
                    return true;
                }
            }
            else if (data.eChatType == ChatType.CT_NORMAL)
            {
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_AROUND_CHAT_LV_LIMIT);
                if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_normal_talk_need_level"), systemValue.Value));
                    return true;
                }
            }
            return false;
        }

        private bool IsSendChatCoolDown()
        {
            if (data.eChatType == ChatType.CT_WORLD)
            {
                if (!world_chat_try_enter_cool_down())
                {
                    return true;
                }
            }
            else if (data.eChatType == ChatType.CT_ACOMMPANY)
            {
                if (!accompany_chat_try_enter_cool_down())
                {
                    return true;
                }
            }
            else if (data.eChatType == ChatType.CT_NORMAL)
            {
                if (!arround_chat_try_enter_cool_down())
                {
                    return true;
                }
            }
            else if (data.eChatType == ChatType.CT_TEAM_COPY_PREPARE)
            {
                if (!teamcopy_prepare_chat_try_enter_cool_down())
                {
                    return true;
                }
            }
            else if (data.eChatType == ChatType.CT_TEAM_COPY_PREPARE)
            {
                if (!teamcopy_team_chat_try_enter_cool_down())
                {
                    return true;
                }
            }

            return false;
        }

        private void HideEmotionBar()
        {
            if (m_goEmotionTab != null)
                m_goEmotionTab.CustomActive(false);
        }

        public void PlayVoice(string voiceKey)
		{
			if (mComVoiceChat != null)
			{
                mComVoiceChat.PlayVoice(voiceKey);
			}
		}

        [UIEventHandle("ScrollviewBg/InputField/Button/Laba")]
        void _OnOpenHornFrame()
        {
            if (frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
            {
                return;
            }

            if(Pk3v3DataManager.HasInPk3v3Room())
            {
                return;
            }

            HornFrame.Open();
            Close();
        }
        [UIObject("tabs/Friend")]
        GameObject goFriend;
        [UIObject("ScrollviewBg/newmessagenumber")]
        GameObject goNewMessageBumber;
        [UIControl("ScrollviewBg/newmessagenumber/Text")]
        Text mNewMessageText;
        int newMessageNumber = 0;
        void RefreshFriendIcon()
        {
            if (RelationDataManager.GetInstance().GetPriDirty())
            {
                goFriend.CustomActive(true);
            }
            else
            {
                goFriend.CustomActive(false);
            }
        }
        [UIEventHandle("tabs/Friend")]
        void OnFriendButtonClick()
        {
            RelationFrameNew.CommandOpen();
            frameMgr.CloseFrame(this);
        }

        void RefreshNewMessageBumber()
        {
            if (screenLocked)
            {
                newMessageNumber += 1;
                goNewMessageBumber.CustomActive(true);
                if (mNewMessageText != null)
                {
                    mNewMessageText.text = string.Format("有{0}条新消息",newMessageNumber);
                }
            }
            else
            {
                goNewMessageBumber.CustomActive(false);
            }
            
        }

        [UIEventHandle("ScrollviewBg/newmessagenumber")]
        void OnNewMessageNumberClick()
        {
            screenLocked = false;
            MarkChatDataDirty();
            goNewMessageBumber.CustomActive(false);
            newMessageNumber = 0;
        }
    }
}