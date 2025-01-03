using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Scripts.UI;
using Protocol;
using Network;
using System;

namespace GameClient
{
    class RelationFriendFrameData
    {
        public RelationTabType eRelationTabType = RelationTabType.RTT_COUNT;
        public RelationData eCurrentRelationData = null;
        public string mTalk = "";
    }

    class RelationFriendFrame : ClientFrame
    {
        RelationFriendFrameData data = null;
        List<RelationData> relationList = null;
        List<InviteFriendData> inviteFriends = null;
        ComChatListNew comChatList = new ComChatListNew();
        RelationData relationData = null;
        ComRelationInfoList comRecentlyRelationInfoList = new ComRelationInfoList();//最近页签
        ComRelationInfoList comFriendRelationInfoList = new ComRelationInfoList();//好友页签
        ComRelationInfoList comBlackRelationInfoList = new ComRelationInfoList();//黑名单页签
        // Dictionary<RelationTabType, RelationNode> mRelationNodeDic = new Dictionary<RelationTabType, RelationNode>();//保存所有页签ComCommonBind
        Dictionary<RelationTabType, ComCommonBind> mRelationSecondMenuDic = new Dictionary<RelationTabType, ComCommonBind>();
        
        ChatFrameData chatFrameData = new ChatFrameData();
        ComVoiceChat mComVoiceChat = null;
        private RelationTabType curToggleType = RelationTabType.RTT_COUNT;

        
        #region ExtraUIBind
        private GameObject mEmotionTab = null;
        private Button mEmotion = null;
        private GameObject mEmotionHightlight = null;
        private Button mSend = null;
        private OnFocusInputField mInput = null;
        private ComUIListScript mComUIList = null;
        private Button mBtnDelete = null;
        private Button mBtnAdd = null;
        private Text mDegreeFriend = null;
        private Text mName = null;
        private GameObject mDegreeFriendsGO = null;
        private GameObject mBtnAddGo = null;
        private GameObject mChatFrameNewRoot = null;
        private ToggleGroup mToggleGroup = null;
        private RectTransform mChatScrollViewRect = null;
        private GameObject mHintRoot = null;
        private GameObject mBlackRoot = null;
        private Button mBlackBtnAdd = null;
        private Button mBalckBtnRemove = null;
        private GameObject mBlackJianbianRoot = null;
        private Text mRelation = null;
        private Button mDegreeFriendBtn = null;
        private ScrollRect mScrollRect = null;
        private GameObject mRecentylList = null;
        private GameObject mFriendList = null;
        private GameObject mBlackList = null;
        private Text mOnLineNum = null;
        private GameObject mRecommended = null;
        private RectTransform mInputFieldRoot = null;

        protected sealed override void _bindExUI()
        {
            mEmotionTab = mBind.GetGameObject("EmotionTab");
            mEmotion = mBind.GetCom<Button>("Emotion");
            if (null != mEmotion)
            {
                mEmotion.onClick.AddListener(_onEmotionButtonClick);
            }
            mEmotionHightlight = mBind.GetGameObject("EmotionHightlight");
            mSend = mBind.GetCom<Button>("Send");
            if (null != mSend)
            {
                mSend.onClick.AddListener(_onSendButtonClick);
            }
            mInput = mBind.GetCom<OnFocusInputField>("Input");
            mComUIList = mBind.GetCom<ComUIListScript>("ComUIList");
            mBtnDelete = mBind.GetCom<Button>("BtnDelete");
            if (null != mBtnDelete)
            {
                mBtnDelete.onClick.AddListener(_onBtnDeleteButtonClick);
            }
            mBtnAdd = mBind.GetCom<Button>("BtnAdd");
            if (null != mBtnAdd)
            {
                mBtnAdd.onClick.AddListener(_onBtnAddButtonClick);
            }
            mDegreeFriend = mBind.GetCom<Text>("DegreeFriend");
            mName = mBind.GetCom<Text>("Name");
            mDegreeFriendsGO = mBind.GetGameObject("DegreeFriendsGO");
            mBtnAddGo = mBind.GetGameObject("BtnAddGo");
            mChatFrameNewRoot = mBind.GetGameObject("ChatFrameNew");
            mToggleGroup = mBind.GetCom<ToggleGroup>("toggleGroup");
            mChatScrollViewRect = mBind.GetCom<RectTransform>("ChatScrollViewRect");
            mHintRoot = mBind.GetGameObject("HintRoot");
            mBlackRoot = mBind.GetGameObject("BlackRoot");
            mBlackBtnAdd = mBind.GetCom<Button>("BlackBtnAdd");
            if (mBlackBtnAdd != null)
            {
                mBlackBtnAdd.onClick.AddListener(_onBlackButtonClick);
            }
            mBalckBtnRemove = mBind.GetCom<Button>("Remove");
            if (mBalckBtnRemove != null)
            {
                mBalckBtnRemove.onClick.AddListener(_onRemoveButtonClick);
            }
            mBlackJianbianRoot = mBind.GetGameObject("JianbianRoot");
            mRelation = mBind.GetCom<Text>("Relation");
            mDegreeFriendBtn = mBind.GetCom<Button>("DegreeFriendBtn");
            if (mDegreeFriendBtn != null)
            {
                mDegreeFriendBtn.onClick.AddListener(_onDegreeFriendClick);
            }
            mScrollRect = mBind.GetCom<ScrollRect>("ScrollView");
            mRecentylList = mBind.GetGameObject("RecentylList");
            mFriendList = mBind.GetGameObject("FriendList");
            mBlackList = mBind.GetGameObject("BlackList");
            mOnLineNum = mBind.GetCom<Text>("OnLineNum");
            mRecommended = mBind.GetGameObject("Recommended");
            mInputFieldRoot = mBind.GetCom<RectTransform>("InputFieldRoot");
        }

        protected sealed override void _unbindExUI()
        {
            mEmotionTab = null;
            if (null != mEmotion)
            {
                mEmotion.onClick.RemoveListener(_onEmotionButtonClick);
            }
            mEmotion = null;
            if (null != mSend)
            {
                mSend.onClick.RemoveListener(_onSendButtonClick);
            }

            mEmotionHightlight = null;
            mSend = null;
            mInput = null;
            mComUIList = null;
            if (null != mBtnDelete)
            {
                mBtnDelete.onClick.RemoveListener(_onBtnDeleteButtonClick);
            }
            mBtnDelete = null;
            if (null != mBtnAdd)
            {
                mBtnAdd.onClick.RemoveListener(_onBtnAddButtonClick);
            }
            mBtnAdd = null;
            mDegreeFriend = null;
            mName = null;
            mDegreeFriendsGO = null;
            mBtnAddGo = null;
            mChatFrameNewRoot = null;
            mToggleGroup = null;
            mChatScrollViewRect = null;
            mHintRoot = null;
            mBlackRoot = null;
            if (mBlackBtnAdd != null)
            {
                mBlackBtnAdd.onClick.RemoveListener(_onBlackButtonClick);
            }
            mBlackBtnAdd = null;
            if (mBalckBtnRemove != null)
            {
                mBalckBtnRemove.onClick.RemoveListener(_onRemoveButtonClick);
            }
            mBalckBtnRemove = null;
            mBlackJianbianRoot = null;
            mRelation = null;
            if (mDegreeFriendBtn != null)
            {
                mDegreeFriendBtn.onClick.RemoveListener(_onDegreeFriendClick);
            }
            mDegreeFriendBtn = null;
            mScrollRect = null;
            mRecentylList = null;
            mFriendList = null;
            mBlackList = null;
            mOnLineNum = null;
            mRecommended = null;
            mInputFieldRoot = null;
        }
        #endregion

        #region Callback
        private void _onEmotionButtonClick()
        {
            mEmotionTab.CustomActive(!mEmotionTab.activeSelf);
            mEmotionHightlight.CustomActive(mEmotionTab.activeSelf);
        }
        private void _onSendButtonClick()
        {
            if (!string.IsNullOrEmpty(mInput.text) && mInput.text.Length < ChatData.CD_MAX_WORDS)
            {
                UInt64 uid = this.relationData == null ? 0 : this.relationData.uid;

                ChatManager.GetInstance().SendChat(ChatType.CT_PRIVATE, GetFliterSizeString(mInput.text), uid);
                mInput.text = "";
            }
            else if (string.IsNullOrEmpty(mInput.text))
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_relation_inputnull"));
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_relation_inputceil"));
            }

            if (mEmotionTab.activeSelf)
            {
                mEmotionTab.CustomActive(false);
            }
        }
        private void _onBtnDeleteButtonClick()
        {
            string content = TR.Value("relation_relation_removechat");
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(content, () => {
                RelationDataManager.GetInstance().DelPriChat(this.relationData.uid);
            }, () => { return; } );
        }
        private void _onBtnAddButtonClick()
        {
            OnAddRelation();
        }
        private void _onBlackButtonClick()
        {
            if (relationData == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_black_selectplayer"));
                return;
            }
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("relation_black_addrelation"), () =>
            {
                OnAddRelation();
            });
        }
        private void _onRemoveButtonClick()
        {
            OnRemoveBlack();
        }
        private void _onDegreeFriendClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<FriendLinessSpecificationFrame>(FrameLayer.Middle);
        }

       
        #endregion

        public static string GetFliterSizeString(string str)
        {
            //〈color=red〉red〈/color〉
            str = str.Replace('<', '〈');
            str = str.Replace('>', '〉');

            return str;
        }

        private void OnAddRelation()
        {
            SceneRequest req = new SceneRequest();
            req.type = (byte)RequestType.RequestFriend;
            req.target = this.relationData.uid;
            req.targetName = "";
            req.param = 0;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        private void OnRemoveBlack()
        {
            if (null != relationData)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("relation_confirm_to_del_black"), () =>
                {
                    RelationDataManager.GetInstance().DelBlack(relationData.uid);
                });
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_black_selectplayer"));
            }
        }

        private class RelationNode
        {
           public ComCommonBind bind;
        }


        protected sealed override void _OnOpenFrame()
        {
            data = userData as RelationFriendFrameData;
            if (data == null)
            {
                return;
            }
            this.relationData = data.eCurrentRelationData;

            _RegisterUIEvent();

            comChatList.Initialize(this, Utility.FindChild(frame, "ChatRoot/ChatFrameNew/Middle"), this.relationData);

            _CreateFirstMenuToggleList();

            m_akEmotionObjects.Clear();
            _InitEmotionBag();
            
            RelationDataManager.GetInstance().SendUpdateRelation();

            RelationDataManager.GetInstance().QueryPlayerOnlineStatus();

            chatFrameData.eChatType = ChatType.CT_PRIVATE;
            chatFrameData.curPrivate = this.relationData;

            //TODO

            GameObject mBtnAddFriend = mBtnAdd.gameObject;

            UIGray gray = mBtnAddFriend.SafeAddComponent<UIGray>();
            if (gray != null)
            {
                gray.enabled = false;
            }

            Button btnAddFriend = mBtnAddFriend.GetComponent<Button>();
            if (btnAddFriend != null)
            {
                btnAddFriend.interactable = true;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CrossWaitingRoom>())
            {
                if (gray != null)
                {
                    gray.enabled = true;
                }

                if (btnAddFriend != null)
                {
                    btnAddFriend.interactable = false;
                }
            }

            //特殊处理 如果传过来mTalk不为空 把对话信息发给要聊天的人
            if (data.mTalk != "")
            {
                mInput.text = data.mTalk;

                _onSendButtonClick();
            }
        }
        protected sealed override void _OnCloseFrame()
        {
            InvokeMethod.RemoveInvokeCall(this);

            RelationDataManager.GetInstance().ClearQueryInfo();

            _UnRegisterUIEvent();

            if (m_openMenu != null)
            {
                m_openMenu.Close(true);
                m_openMenu = null;
            }
            relationList = null;
            data = null;
            
            relationData = null;
            if (m_akEmotionObjects != null)
                m_akEmotionObjects.Clear();
            if (comChatList != null)
                comChatList.UnInitialize();
            // if (mRelationNodeDic != null)
            //     mRelationNodeDic.Clear();
            if (mRelationSecondMenuDic != null)
                 mRelationSecondMenuDic.Clear();
            if (comRecentlyRelationInfoList != null)
                comRecentlyRelationInfoList.UnInitialize();
            if (comFriendRelationInfoList != null)
                comFriendRelationInfoList.UnInitialize();
            if (comBlackRelationInfoList != null)
                comBlackRelationInfoList.UnInitialize();
            curToggleType = RelationTabType.RTT_COUNT;

            if(mComVoiceChat != null)
            {
                mComVoiceChat.UnInit();
            }
        }
        private void HideEmotionBar()
        {
            if (m_goEmotionTab != null)
                m_goEmotionTab.CustomActive(false);
        }
        
        
        void _OnItemSelected(RelationData relationData)
        {
            this.relationData = relationData;
            chatFrameData.curPrivate = this.relationData;
            data.eCurrentRelationData = null;

            if (relationData == null)
            {
                mBlackRoot.CustomActive(false);
                mChatFrameNewRoot.CustomActive(false);
                mHintRoot.CustomActive(true);
                return;
            }
            //chatFrameData.curPrivate = this.relationData;
            mChatFrameNewRoot.CustomActive(true);
            mHintRoot.CustomActive(false);
            mBlackRoot.CustomActive(false);
            mChatScrollViewRect.offsetMax = new Vector2(mChatScrollViewRect.offsetMax.x, 0);

            if (this.relationData.type == (byte)RelationType.RELATION_STRANGER || this.relationData.type == (byte)RelationType.RELATION_NONE)
            {
                mChatScrollViewRect.offsetMax = new Vector2(mChatScrollViewRect.offsetMax.x, -62.0f);
            }

            if (this.relationData != null)
            {
                RelationDataManager.GetInstance().ClearPriChatDirty(this.relationData.uid);
            }
            UIEventSystem.GetInstance().SendUIEvent(new UIEventPrivateChat(this.relationData, false));
        }
        #region filters

       
        protected void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRelationChanged, _OnRelationChanged);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshInviteList, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnShowFriendSecMenu, _OnShowFrienSecInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCloseMenu, _OnCloseMenu);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPrivateChat, _OnPrivateChat);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnDelPrivate, _OnDelPrivate);
            ChatManager.GetInstance().onAddChatdata += OnAddChatData;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPlayerOnLineStatusChanged, _OnPlayerOnLineStatusChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FriendComMenuRemoveList, _OnFriendComMenuRemoveList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SetNoteNameSuccess, OnSetNoteNameSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSendFriendGift, OnSendGiftSuccess);
        }
        protected void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRelationChanged, _OnRelationChanged);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshInviteList, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnShowFriendSecMenu, _OnShowFrienSecInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCloseMenu, _OnCloseMenu);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPrivateChat, _OnPrivateChat);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDelPrivate, _OnDelPrivate);
            ChatManager.GetInstance().onAddChatdata -= OnAddChatData;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPlayerOnLineStatusChanged, _OnPlayerOnLineStatusChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FriendComMenuRemoveList, _OnFriendComMenuRemoveList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SetNoteNameSuccess, OnSetNoteNameSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSendFriendGift, OnSendGiftSuccess);
        }

        void SetBottomStuffs()
        {
            if (relationData == null)
            {
                return;
            }
            if (this.relationData.type == (byte)RelationType.RELATION_STRANGER || this.relationData.type == (byte)RelationType.RELATION_NONE)
            {
                mDegreeFriendsGO.CustomActive(false);
                mBtnAdd.CustomActive(true);
                mBtnDelete.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2.7f, -2.3f);
            }
            else
            {
                if (relationData.type != (byte)RelationType.RELATION_BLACKLIST)
                {
                    mDegreeFriendsGO.CustomActive(true);
                }
                else
                {
                    mDegreeFriendsGO.CustomActive(false);
                }
                mBtnAdd.CustomActive(false);
                mBtnDelete.GetComponent<RectTransform>().anchoredPosition = new Vector2(341.0f, -2.3f);
            }
        }

        void SetTopStuff()
        {
            if (relationData == null)
            {
                return;
            }
            if (relationData.type == (byte)RelationType.RELATION_BLACKLIST)
            {
                mBlackJianbianRoot.CustomActive(true);
                mInputFieldRoot.gameObject.CustomActive(false);
            }
            else
            {
                mBlackJianbianRoot.CustomActive(false);
                mInputFieldRoot.gameObject.CustomActive(true);
            }
        }

        void OnSendGiftSuccess(UIEvent iEvent)
        {
            RefreshChatInfo(this.relationData);
        }
        void OnSetNoteNameSuccess(UIEvent iEvent)
        {
            RefreshChatData();
        }
        private void _OnFriendComMenuRemoveList(UIEvent uiEvent)
        {
            RefreshRelationList();
        }
        void _OnPlayerOnLineStatusChanged(UIEvent uiEvent)
        {
            RefreshRelationList();
        }
        void OnAddChatData(ChatBlock chatBlock)
        {
            if (chatBlock.chatData.eChatType == ChatType.CT_PRIVATE)
            {
                if (relationData != null)
                {
                    if (this.relationData.uid == chatBlock.chatData.objid)
                    {
                        RelationDataManager.GetInstance().ClearPriChatDirty(this.relationData.uid);
                    }
                    else
                    {
                        RelationDataManager.GetInstance().OnAddPriChatList(this.relationData, false);
                        RefreshTabTitleCount();
                    }
                }
               
                RefreshChatData();
            }
               
        }
        private void _OnDelPrivate(UIEvent uiEvent)
        {
            UIEventDelPrivate myEvent = uiEvent as UIEventDelPrivate;
            if (null != this.relationData && myEvent.m_uid == this.relationData.uid)
            {
                RefreshChatData();
            }
        }
       
        private void RefreshRelationList()
        {
            if (data.eRelationTabType == RelationTabType.RTT_RECENTLY)
            {
                if (comRecentlyRelationInfoList.Initilized)
                {
                    comRecentlyRelationInfoList.RefreshAllRelations(data.eRelationTabType);
                }
            }
            else if (data.eRelationTabType == RelationTabType.RTT_FRIEND)
            {
                if (comFriendRelationInfoList.Initilized)
                {
                    comFriendRelationInfoList.RefreshAllRelations(data.eRelationTabType);
                }
            }
            else if (data.eRelationTabType == RelationTabType.RTT_BLACK)
            {
                if (comBlackRelationInfoList.Initilized)
                {
                    comBlackRelationInfoList.RefreshAllRelations(data.eRelationTabType);
                }
            }

            RefreshTabTitleCount();
            RefreshRelationListHide(data.eRelationTabType);
            RefreshTabNoRelationShowContent(data.eRelationTabType);
            RefreshRelationNodeRePoint();
            RefreshChatData();
        }
        /// <summary>
        /// 刷新页签上面好友数量
        /// </summary>
        void RefreshTabTitleCount()
        {
            // RelationNode node = null;
            // for (int i = 0; i < (int)RelationTabType.RTT_COUNT; i++)
            // {
            //     RelationTabType type = (RelationTabType)i;
            //     if (mRelationNodeDic.TryGetValue(type, out node))
            //     {
            //         if (null != node)
            //         {
            //             ComCommonBind bind = node.bind;
            //             Text title = bind.GetCom<Text>("desc");
            //             Text imagetitle = bind.GetCom<Text>("Text");
            //             title.text = imagetitle.text = InitMenuGroupTitle(type);
            //         }
            //     }
            // }
            if (mOnLineNum == null)
            {
                return;
            }
            int count = RelationCount(curToggleType);
            if (count <= 0)
            {
                mOnLineNum.CustomActive(false);
            }
            else
            {
                mOnLineNum.CustomActive(true);
                mOnLineNum.SafeSetText(InitMenuGroupTitle(curToggleType));    
            }
            
           
        }

        /// <summary>
        /// 刷新好友列表Content的高度
        /// </summary>
        void RefreshRelationListHide(RelationTabType type)
        {
            // ComCommonBind mSecendMenuBind = null;
            //
            // if (mRelationSecondMenuDic.TryGetValue(data.eRelationTabType, out mSecendMenuBind))
            // {
            //     GameObject content = mSecendMenuBind.GetGameObject("Content");
            //     if (content == null)
            //     {
            //         return;
            //     }
            //     LayoutElement layoutElement = mSecendMenuBind.GetCom<LayoutElement>("LayoutElement");
            //     if (layoutElement == null)
            //     {
            //         return;
            //     }
            //     RectTransform transform = content.GetComponent<RectTransform>();
            //     float x = transform.sizeDelta.x;
            //     float y = 0;
            //     int count = RelationCount(type);
            //     if (count == 2)
            //     {
            //         y = 310;
            //     }
            //     else if (count == 0)
            //     {
            //         y = 462;
            //     }
            //     else if (count == 1)
            //     {
            //         y = 160;
            //     }
            //     else
            //     {
            //         y = 462;
            //     }
            //     layoutElement.preferredWidth = x;
            //     layoutElement.preferredHeight = y;
            // }
        }

        void RefreshRelationNodeRePoint()
        {
            GameObject redPoint = relationRedPoints[(int) RelationTabType.RTT_RECENTLY].gameObject;
            if (redPoint != null)
            {
                redPoint.CustomActive(RelationDataManager.GetInstance().GetPriDirty());
            }
        }
        
        [UIControl("LeftRoot/TopMenu/Toggle{0}/target", typeof(Toggle), 0)]
        Toggle[] relationToggle = new Toggle[(int)RelationTabType.RTT_COUNT];
        
        [UIControl("LeftRoot/TopMenu/Toggle{0}/redPoint", typeof(RectTransform), 0)]
        RectTransform[] relationRedPoints = new RectTransform[(int)RelationTabType.RTT_COUNT];
        
        [UIObject("LeftRoot/Content")]
        GameObject mTargetRoot;
        void _CreateFirstMenuToggleList()
        {
            for (int i = 0; i < (int)RelationTabType.RTT_COUNT; i++)
            {
                Toggle onGroup = relationToggle[i];
                GameObject redPoint = relationRedPoints[i].gameObject;
                if ((RelationTabType)i == RelationTabType.RTT_RECENTLY)
                {
                    redPoint.CustomActive(RelationDataManager.GetInstance().GetPriDirty());
                }
                else
                {
                    redPoint.CustomActive(false);
                }

                int index = i;
                    
                if (onGroup != null)
                {
                    onGroup.group = mToggleGroup;

                    onGroup.onValueChanged.AddListener((value) => { OnChooseFirstMenu((RelationTabType)index, value); });

                    if (RelationTabType.RTT_RECENTLY == (RelationTabType)index)
                    {
                        onGroup.isOn = true;
                    }    
                }

            }
        }

        string InitMenuGroupTitle(RelationTabType type)
        {
            string str = "";
            int onLineNum = 0;
            if (type == RelationTabType.RTT_RECENTLY)
            {
                var srclist = RelationDataManager.GetInstance().GetPriChatList();
                for (int i = 0; i < srclist.Count; i++)
                {
                    var privateChatData = srclist[i];
                    if (privateChatData.relationData.status == (byte)FriendMatchStatus.Offlie)
                    {
                        continue;
                    }
                    if (privateChatData.relationData.isOnline < 1)
                    {
                        continue;
                    }
                    onLineNum++;
                }

                str = TR.Value("relation_online", onLineNum, srclist.Count);
            }
            else if (type == RelationTabType.RTT_FRIEND)
            {
                List<RelationData> relationDatas = new List<RelationData>();
                var datas_friends = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_FRIEND);
                var datas_teachers = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_MASTER);
                var datas_pupils = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
                relationDatas.AddRange(datas_friends);
                relationDatas.AddRange(datas_teachers);
                relationDatas.AddRange(datas_pupils);
                for (int i = 0; i < relationDatas.Count; i++)
                {
                    var friendData = relationDatas[i];
                    if (friendData.status == (byte)FriendMatchStatus.Offlie)
                    {
                        continue;
                    }
                    onLineNum++;
                }

                str = TR.Value("relation_online", onLineNum, relationDatas.Count);
            }
            else if (type == RelationTabType.RTT_BLACK)
            {
                var datas_blacks = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_BLACKLIST);
                for (int i = 0; i < datas_blacks.Count; i++)
                {
                    var blackDatas = datas_blacks[i];
                    if (blackDatas.status == (byte)FriendMatchStatus.Offlie)
                    {
                        continue;
                    }
                    onLineNum++;
                }
                str = TR.Value("relation_online", onLineNum, datas_blacks.Count);
            }
            return str;
        }

       int RelationCount(RelationTabType type)
        {
            List<RelationData> relationDatas = new List<RelationData>();
            if (type == RelationTabType.RTT_RECENTLY)
            {
                var srclist = RelationDataManager.GetInstance().GetPriChatList();
                for (int i = 0; i < srclist.Count; i++)
                {
                    var privateChatData = srclist[i];
                    relationDatas.Add(privateChatData.relationData);
                }
            }
            else if (type == RelationTabType.RTT_FRIEND)
            {
                var datas_friends = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_FRIEND);
                var datas_teachers = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_MASTER);
                var datas_pupils = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_DISCIPLE);
                relationDatas.AddRange(datas_friends);
                relationDatas.AddRange(datas_teachers);
                relationDatas.AddRange(datas_pupils);
            }
            else if (type == RelationTabType.RTT_BLACK)
            {
                var datas_blacks = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_BLACKLIST);
                relationDatas.AddRange(datas_blacks);
            }

            return relationDatas.Count;
        }

        void RefreshTabNoRelationShowContent(RelationTabType type)
        {
            if (relationData != null)
            {
                mChatFrameNewRoot.CustomActive(true);
                mHintRoot.CustomActive(false);
            }
            else
            {
                mChatFrameNewRoot.CustomActive(false);
                if (type == RelationTabType.RTT_BLACK)
                {
                    mHintRoot.CustomActive(false);
                }
                else
                {
                    mHintRoot.CustomActive(true); 
                }
            }
            
            comStateFriend.Key = "None";
            int relationCount = RelationCount(type);
            if (relationCount <= 0)
            {
                if (type == RelationTabType.RTT_RECENTLY)
                {
                    mHintRoot.CustomActive(true);
                    comStateFriend.Key = "Recently";
                }
                else if (type == RelationTabType.RTT_FRIEND)
                {
                    mHintRoot.CustomActive(true);
                    comStateFriend.Key = "Friend";
                }
            }

            if (type == RelationTabType.RTT_BLACK)
            {
                if (relationCount <= 0)
                {
                    comStateFriend.Key = "Black";
                }
                if (relationData == null || relationCount <= 0)
                {
                    mBlackRoot.CustomActive(true);
                }
                else
                    mBlackRoot.CustomActive(false);
            }
            else
            {
                mBlackRoot.CustomActive(false);
            }
        }
        void OnChooseFirstMenu(RelationTabType type,bool value)
        {
            if (data.eRelationTabType == type)
            {
                return;
            }

            if (value)
            {
                ComRelationInfo.ms_selected = null;
                if (data.eCurrentRelationData != null)
                {
                    relationData = data.eCurrentRelationData;
                }
                else
                {
                    relationData = null;
                }
                mChatFrameNewRoot.CustomActive(true);
                mHintRoot.CustomActive(false);
                mBlackRoot.CustomActive(false);
                mBlackJianbianRoot.CustomActive(false);
                mInputFieldRoot.gameObject.CustomActive(true);
                data.eRelationTabType = type;
                // RelationNode relationNode = null;
                ComCommonBind secondMenuComBind = null;
                GameObject go = null;
                List<RelationData> relationDatas = new List<RelationData>();
                // if (mRelationNodeDic.TryGetValue(type, out relationNode))
                // {
                //     if (mRelationSecondMenuDic.ContainsKey(type) == false)
                //     {
                //         ComCommonBind bind = relationNode.bind;
                //         ComSwitchNode switchNode = bind.GetCom<ComSwitchNode>("switchNode");
                //         secondMenuComBind = switchNode.AddOneSubItem();
                //         go = secondMenuComBind.GetGameObject("go");
                //         mRelationSecondMenuDic.Add(type, secondMenuComBind);
                //     }
                //     else
                //     {
                //         if (mRelationSecondMenuDic.TryGetValue(type, out secondMenuComBind))
                //         {
                //             go = secondMenuComBind.GetGameObject("go");
                //         }
                //     }
                // }
                curToggleType = type;
                if (type == RelationTabType.RTT_RECENTLY)
                {
                    go = mRecentylList;   
                    if (go != null)
                    {
                        if (!comRecentlyRelationInfoList.Initilized)
                        {
                            comRecentlyRelationInfoList.Initialize(this, go, _OnItemSelected, type,this.relationData);
                        }
                        else
                        {
                            comRecentlyRelationInfoList.RefreshAllRelations(type);
                        }
                    }
                    mRecommended.CustomActive(true);
                }
                else if (type == RelationTabType.RTT_FRIEND)
                {
                    go = mFriendList;
                    if (go != null)
                    {
                        if (!comFriendRelationInfoList.Initilized)
                        {
                            comFriendRelationInfoList.Initialize(this, go, _OnItemSelected, type,this.relationData);
                        }
                        else
                        {
                            comFriendRelationInfoList.RefreshAllRelations(type);
                        }
                    }
                    mRecommended.CustomActive(true);
                }
                else if (type == RelationTabType.RTT_BLACK)
                {
                    go = mBlackList;
                    if (go != null)
                    {
                        if (!comBlackRelationInfoList.Initilized)
                        {
                            comBlackRelationInfoList.Initialize(this, go, _OnItemSelected, type,this.relationData);
                        }
                        else
                        {
                            comBlackRelationInfoList.RefreshAllRelations(type);
                        }
                    }
                    mRecommended.CustomActive(false);
                }

                if (mRelationSecondMenuDic.ContainsKey(type) == false)
                {
                    if (go != null)
                    {
                        secondMenuComBind = go.GetComponent<ComCommonBind>();
                        mRelationSecondMenuDic.Add(type, secondMenuComBind);
                    
                    }
                }
                else
                {
                    if (go != null)
                    {
                        secondMenuComBind = go.GetComponent<ComCommonBind>();
                    }
                    
                }
                
                //show current type second menu
                foreach (var secondMenu in mRelationSecondMenuDic)
                {
                    if (secondMenu.Value != null)
                    {
                        secondMenu.Value.gameObject.CustomActive(secondMenu.Key == type);
                    }
                }
                

                RefreshTabNoRelationShowContent(type);
                RefreshTabTitleCount();
                // GameObject content = secondMenuComBind.GetGameObject("Content");
                // if (content == null)
                // {
                //     return;
                // }
                // LayoutElement layoutElement = secondMenuComBind.GetCom<LayoutElement>("LayoutElement");
                // if (layoutElement == null)
                // {
                //     return;
                // }
                // RectTransform transform = content.GetComponent<RectTransform>();
                // float x = transform.sizeDelta.x;
                // float y = 0;
                // int count = RelationCount(type);
                // if (count == 2)
                // {
                //     y = 310;
                // }
                // else if (count == 0)
                // {
                //     y = 462;
                // }
                // else if (count == 1)
                // {
                //     y = 160;
                // }
                // else
                // {
                //     y = 462;
                // }
                // layoutElement.preferredWidth = x;
                // layoutElement.preferredHeight = y;
            }
            else
            {
                comStateFriend.Key = "None";
            }
        }

        #endregion

        void _OnRelationChanged(UIEvent uiEvent)
        {
            //这个信息获取，只是为了更新当前选中好友的最新信息，不加这个判断会导致左右信息对不上。
            if (uiEvent != null)
            {
                if (uiEvent.Param1 != null)
                {
                    var newData = uiEvent.Param1 as RelationData;
                    if (relationData != null)
                    {
                        if (newData != null && newData.uid == relationData.uid)
                        {
                            this.relationData = newData;
                        }    
                    }
                }
            }
            RefreshRelationList();
        }
        
        void _OnRefreshInviteList(UIEvent uiEvent)
        {
            _OnRelationChanged(null);
        }
        

        [UIEventHandle("LeftRoot/buttom/Recommended")]
        void OnReconmmendClick()
        {
            OpenFriendRecommendFrame();
        }

        void OpenFriendRecommendFrame()
        {
           ClientSystemManager.GetInstance().OpenFrame<FriendRecommendedFrame>(FrameLayer.Middle);
        }
        
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RelationFrame/RelationFriendFrame";
        }
        
        [UIControl("", typeof(StateController))]
        StateController comStateFriend;

        #region Menu
        IClientFrame m_openMenu = null;
        protected void _OnShowFrienSecInfo(UIEvent uiEvent)
        {
            UIEventShowFriendSecMenu myEvent = uiEvent as UIEventShowFriendSecMenu;

            if(null != m_openMenu)
            {
                m_openMenu.Close(true);
                m_openMenu = null;
            }
            m_openMenu = frameMgr.OpenFrame<FriendComMenuFrame>(FrameLayer.Middle, myEvent.m_data);
        }

        protected void _OnCloseMenu(UIEvent uiEvent)
        {
            if (m_openMenu != null)
            {
                frameMgr.CloseFrame(m_openMenu);
                m_openMenu = null;
            }
        }
        #endregion

        #region Chat

        [UIObject("ChatRoot")]
        GameObject chatRoot;
        
        private void _OnPrivateChat(UIEvent uiEvent)
        {
            UIEventPrivateChat myEvent = uiEvent as UIEventPrivateChat;

            if (this.relationData != null)
            {
                RelationDataManager.GetInstance().ClearPriChatDirty(this.relationData.uid);
            }

            if (myEvent.m_recvChat == true)
            {
               if (data.eRelationTabType == RelationTabType.RTT_RECENTLY)
               {
                    comRecentlyRelationInfoList.RefreshAllRelations(data.eRelationTabType);
               }
            }
            else
            {
                // if (data.eRelationTabType != RelationTabType.RTT_BLACK)
                // {
                RefreshChatData();
                // }
            }

            RefreshRelationNodeRePoint();
            RefreshRelationListHide(data.eRelationTabType);
            RefreshTabTitleCount();
            RefreshTabNoRelationShowContent(data.eRelationTabType);
        }

        void RefreshChatData()
        {
            RefreshChatInfo(this.relationData);
            comChatList.RefreshChatData(this.relationData);
            mScrollRect.verticalNormalizedPosition = 0;
        }

        void RefreshChatInfo(RelationData data)
        {
            if (data == null )
            {
                return;
            }
            if (mName != null)
            {
                if (data.remark != null && relationData.remark != "")
                {
                    mName.text = data.remark;
                }
                else
                {
                    mName.text = data.name;
                }
            }
            

            if (data.type == (int)RelationType.RELATION_FRIEND||
                data.type == (int)RelationType.RELATION_MASTER||
                data.type == (int)RelationType.RELATION_DISCIPLE||
                data.type == (int)RelationType.RELATION_BLACKLIST)
            {
                mBtnAddGo.CustomActive(false);
            }
            else
            {
                mBtnAddGo.CustomActive(true);
            }

            if (mDegreeFriend != null)
            {
                int percent = RelationDataManager.GetInstance().GetFriendlyDegreesAddPercent((int)data.intimacy);
                mDegreeFriend.text = data.intimacy + "（" + percent + "%）";
            }

            SetBottomStuffs();
            SetTopStuff();

            string str = "";
            if (data.type == (byte) RelationType.RELATION_STRANGER ||
                data.type == (int) RelationType.RELATION_NONE)
            {
                //todo  read from table later
                str = "陌生人";
                mRelation.text = str;
                mRelation.SafeSetColor(Color.red);
                return;
            }

            if (data.type == (byte) RelationType.RELATION_BLACKLIST)
            {
                str = "黑名单";
                mRelation.text = str;
                mRelation.SafeSetColor(Color.red);
                return;
            }

            str = RelationDataManager.GetInstance().GetFriendlyDegreesIntervalName((int)data.intimacy);
            if (str != "")
            {
                mRelation.text = str;
                mRelation.SafeSetColor(new Color(198/255f,193/255f,179/255f,1.0f));
            }
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
            protected RelationFriendFrame THIS;

            Image emotion;
            Button button;

            public sealed override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                spriteAssetInfo = param[2] as SpriteAssetInfor;
                THIS = param[3] as RelationFriendFrame;

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
            public sealed override bool NeedFilter(object[] param) { return false; }

            void _UpdateItem()
            {
                emotion.sprite = spriteAssetInfo.sprite;
            }

            void OnClickEmotion()
            {
                THIS.AddChatText("{F " + string.Format("{0}", spriteAssetInfo.ID) + "}");
            }
        }

        CachedObjectDicManager<int, EmotionObject> m_akEmotionObjects = new CachedObjectDicManager<int, EmotionObject>();

        void _InitEmotionBag()
        {
            m_goEmotionTab = Utility.FindChild(frame, "ChatRoot/ChatFrameNew/Bottom/EmotionTab");
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

        #endregion

      
        void AddChatText(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                mInput.text += content;
            }
        }

        public void PlayVoice(string voiceKey)
        {
            if (mComVoiceChat != null)
            {
                mComVoiceChat.PlayVoice(voiceKey);
            }
        }
        public void OnPopupMenu()
        {
            if (this.relationData != null)
            {
                RelationMenuData menuData = new RelationMenuData();
                menuData.m_data = this.relationData;

                if (data.eRelationTabType == RelationTabType.RTT_RECENTLY)
                {
                    menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_PRIVAT_CHAT;
                }
                else if (data.eRelationTabType == RelationTabType.RTT_FRIEND)
                {
                    menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_COMMON;
                }
                else if (data.eRelationTabType == RelationTabType.RTT_BLACK)
                {
                    menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_BLACK;
                }

                UIEventSystem.GetInstance().SendUIEvent(new UIEventShowFriendSecMenu(menuData));
            }

        }
        #endregion
    }
}