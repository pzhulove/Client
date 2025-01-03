using Network;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using GoodsType = ProtoTable.ShopTable.eSubType;
using ProtoTable;

namespace GameClient
{
    enum FriendListType
    {
        FLT_NONE = 0,
        FLT_FRIEND = 1,
        FLT_NOTIFY = 2,
        FLT_BLACK = 3,
        FLT_QUERY = 4,
        FLT_CHATLIST = 5,
    }

    class FriendSecondMenu
    {
        public GameObject m_friendSecPrefab;        // 二级菜单
        public ulong m_uid;
        public string m_name;
        public UInt16 m_level;
        public byte m_occu;

        public FriendSecondMenu(ulong uid, string name, UInt16 lv, byte occu)
        {
            m_friendSecPrefab = AssetLoader.instance.LoadResAsGameObject("UI/Prefabs/FriendSecondInfo");
            m_friendSecPrefab.SetActive(true);
        }
    }

    class FriendInfo
    {
        public GameObject m_friendPrefab;        // 好友显示对象

        Button m_giveBtn;
        Button m_accBtn;
        Button m_refBtn;
        Button m_delBtn;
        Button m_chatBtn;
        Image m_redPt;

        Toggle m_secondInfoBtn;
        GameObject friendRoot;
        GameObject inviteRoot;
        GameObject blackRoot;

        public ulong m_uid;
        public string m_name;
        public UInt16 m_level;
        public UInt16 m_vipLv;
        public byte m_occu;
        public byte m_giveNum;
        public byte m_isOnline;
        public UInt32 m_seasonLv;
        RelationData data;
        RelationData m_relationData
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }
        FriendListType m_type;

        public FriendInfo(ulong uid, string name, UInt16 lv, byte occu, 
            byte giveNum, FriendListType type, byte isOnline, byte vipLv, UInt32 seasonLv)
        {
           
            m_uid = uid;
            m_name = name;
            m_level = lv;
            m_occu = occu;
            m_giveNum = giveNum;
            m_isOnline = isOnline;
            m_vipLv = vipLv;
            m_seasonLv = seasonLv;

            //m_friendPrefab = AssetLoader.instance.LoadRes("UIFlatten/Prefabs/Friends/FriendInfo").obj as GameObject;
            m_friendPrefab = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/Friends/FriendInfo", enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);
            
            m_friendPrefab.SetActive(true);

            m_giveBtn = Utility.FindComponent<Button>(m_friendPrefab, "Friend/Give");
            m_accBtn = Utility.FindComponent<Button>(m_friendPrefab, "Invite/Accept");
            m_refBtn = Utility.FindComponent<Button>(m_friendPrefab, "Invite/Refuse");
            m_delBtn = Utility.FindComponent<Button>(m_friendPrefab, "Black/Del");
            m_chatBtn = Utility.FindComponent<Button>(m_friendPrefab, "Chat");
         
            m_redPt = Utility.FindComponent<Image>(m_friendPrefab, "Chat/RedPt");
            m_redPt.gameObject.SetActive(false);

            m_secondInfoBtn = Utility.FindComponent<Toggle>(m_friendPrefab, "FriendBG");

            friendRoot = Utility.FindGameObject(m_friendPrefab, "Friend");
            inviteRoot = Utility.FindGameObject(m_friendPrefab, "Invite");
            blackRoot = Utility.FindGameObject(m_friendPrefab, "Black");

            m_giveBtn.onClick.AddListener(OnClickGive);
            m_accBtn.onClick.AddListener(OnClickAccept);
            m_refBtn.onClick.AddListener(OnClickRefuse);
            m_delBtn.onClick.AddListener(OnClickDel);
            m_chatBtn.onClick.AddListener(OnClickChat);

            if (type == FriendListType.FLT_FRIEND ||
                type == FriendListType.FLT_BLACK)
            {
                m_secondInfoBtn.onValueChanged.AddListener(OnSecondInfo);
            }

            m_type = type;
            SetupInfo(type);
        }
        ~FriendInfo()
        {
            Finatial();
        }

        public void SetRelationData(RelationData data)
        {
            m_relationData = data;
            if (m_relationData == null)
            {
                return;
            }
        }

        public void SetCustomParent(Transform tf)
        {
            m_friendPrefab.transform.SetParent(tf, false);
            m_secondInfoBtn.group = m_friendPrefab.GetComponentInParent<ToggleGroup>();

        }


        public void Finatial()
        {
            m_giveBtn.onClick.RemoveListener(OnClickGive);
            m_accBtn.onClick.RemoveListener(OnClickAccept);
            m_refBtn.onClick.RemoveListener(OnClickRefuse);
            m_delBtn.onClick.RemoveListener(OnClickDel);
            m_chatBtn.onClick.RemoveListener(OnClickChat);

            if (m_type == FriendListType.FLT_FRIEND ||
                m_type == FriendListType.FLT_BLACK)
            {
                m_secondInfoBtn.onValueChanged.RemoveListener(OnSecondInfo);
            }

            CGameObjectPool.instance.RecycleGameObject(m_friendPrefab);
            m_giveBtn = null;
            m_accBtn = null;
            m_refBtn = null;
            m_delBtn = null;
            m_secondInfoBtn = null;
            m_relationData = null;

            friendRoot = null;
            inviteRoot = null;
            blackRoot = null;
        }

        //type: 1.好友列表 2.通知邀请列表 3.查找好友列表 4.黑名单列表
        private void SetupInfo(FriendListType type)
        {
            ProtoTable.JobTable job = TableManager.instance.GetTableItem<ProtoTable.JobTable>(m_occu);
            if (job != null)
            {
                ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(job.Mode);
                if (resData != null)
                {
                    Image headIcon = Utility.FindComponent<Image>(m_friendPrefab, "Info/Mask/OccuHead");
                    // headIcon.sprite = Utility.createSprite(job.JobHalfBody);
                    Utility.createSprite(job.JobHalfBody, ref headIcon);
                    headIcon.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                    headIcon.gameObject.transform.localPosition += new Vector3(job.OffSetXFriendInfo, 0, 0);
                    

                    UIGray gray = Utility.FindComponent<UIGray>(m_friendPrefab, "Info/Mask/OccuHead");
                    gray.enabled = m_isOnline > 0 ? false : true;
                }

                Text OccuDesc = Utility.FindComponent<Text>(m_friendPrefab, "Info/Title/Occu");
                OccuDesc.text = job.Name;
            }

            Text name = Utility.FindComponent<Text>(m_friendPrefab, "Info/Title/Name");
            name.text = string.Format("{0}", m_name);

            Text Lv = Utility.FindComponent<Text>(m_friendPrefab, "Info/Lv");
            Lv.text = string.Format("Lv.{0}", m_level);

            Text VipLv = Utility.FindComponent<Text>(m_friendPrefab, "Info/VipLv");
            VipLv.text = string.Format("贵{0}", m_vipLv);
            
            Text SeasonLv = Utility.FindComponent<Text>(m_friendPrefab, "Info/PkLv");
            SeasonLv.text = SeasonDataManager.GetInstance().GetRankName((int)m_seasonLv);

            m_redPt.gameObject.SetActive(RelationDataManager.GetInstance().GetPriDirtyByUid(m_uid));
            
            SwitchType(type);
        }

        public void ShowRedPoint()
        {
            m_redPt.gameObject.SetActive(true);
        }

        private bool CanGive()
        {
            if (m_giveNum > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SwitchType(FriendListType type)
        {
            m_chatBtn.gameObject.SetActive(false);

            if (FriendListType.FLT_FRIEND == type)
            {
                friendRoot.SetActive(true);
                inviteRoot.SetActive(false);
                blackRoot.SetActive(false);

                UIGray gray = Utility.FindComponent<UIGray>(m_friendPrefab, "Friend/Give");
                gray.enabled = !CanGive();

                Text btnName = Utility.FindComponent<Text>(m_friendPrefab, "Friend/Give/Text");
                if (CanGive())
                {
                    btnName.text = "赠送礼物";
                    m_giveBtn.enabled = true;
                }
                else
                {
                    btnName.text = "已经赠送";
                    m_giveBtn.enabled = false;
                }

                m_chatBtn.gameObject.SetActive(true);
            }
            else if (FriendListType.FLT_NOTIFY == type)
            {
                friendRoot.SetActive(false);
                inviteRoot.SetActive(true);
                blackRoot.SetActive(false);
               
            }
            else if (FriendListType.FLT_BLACK == type)
            {
                friendRoot.SetActive(false);
                inviteRoot.SetActive(false);
                blackRoot.SetActive(true);
            }
            else
            {
                friendRoot.SetActive(false);
                inviteRoot.SetActive(false);
                blackRoot.SetActive(false);
            }
        }

        //接受好友邀请
        public void OnClickAccept()
        {
            SceneReply sendMsg = new SceneReply();
            sendMsg.type = (byte)RequestType.RequestFriend;
            sendMsg.requester = m_uid;
            sendMsg.result = 1;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);

            RelationDataManager.GetInstance().DelInviter(m_uid);
        }

        //拒绝好友邀请
        public void OnClickRefuse()
        {
            SceneReply sendMsg = new SceneReply();
            sendMsg.type = (byte)RequestType.RequestFriend;
            sendMsg.requester = m_uid;
            sendMsg.result = 0;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);

            RelationDataManager.GetInstance().DelInviter(m_uid);
        }

        //赠送
        private void OnClickGive()
        {
            WorldRelationPresentGiveReq sendMsg = new WorldRelationPresentGiveReq();
            sendMsg.friendUID = m_uid;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);

            UIGray gray = Utility.FindComponent<UIGray>(m_friendPrefab, "Friend/Give");
            gray.enabled = true;
            Text btnName = Utility.FindComponent<Text>(m_friendPrefab, "Friend/Give/Text");
            btnName.text = "已经赠送";
            m_giveBtn.enabled = false;

            RelationFrame rf = ClientSystemManager.instance.GetFrame(typeof(RelationFrame)) as RelationFrame;
            if(null != rf)
            {
                rf.GiveFriend = true;
            }
            
        }

        // 删除关系
        private void OnClickDel()
        {
            string msgCtx = String.Format("是否删除黑名单玩家?");
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(msgCtx, () =>
            {
                RelationDataManager.GetInstance().DelBlack(m_uid);
            }, () => { return; });
        }

        //加好友
        //         private void OnAddFriend()
        //         {
        //             SceneRequest req = new SceneRequest();
        //             req.type = (byte)RequestType.RequestFriendByName;
        //             req.targetName = m_name;
        //             NetManager netMgr = NetManager.Instance();
        //             netMgr.SendCommand(ServerType.GATE_SERVER, req);
        // 
        //             GameObject addFriend = Utility.FindGameObject(m_friendPrefab, "Right/Add");
        //             addFriend.SetActive(false);
        //             m_alreadyAdd.SetActive(true);
        //         }

        //显示二级菜单
        private void OnSecondInfo(bool bValue)
        {
            if (bValue)
            {
                RelationMenuData menuData = new RelationMenuData();
                if (m_relationData == null)
                {
                    return;
                }
                menuData.m_data = m_relationData;
                menuData.type = (FriendListType.FLT_BLACK == m_type) ? CommonPlayerInfo.CommonPlayerType.CPT_BLACK : CommonPlayerInfo.CommonPlayerType.CPT_COMMON;
                UIEventSystem.GetInstance().SendUIEvent(new UIEventShowFriendSecMenu(menuData));
            }

        }

        private void OnClickChat()
        {
            m_redPt.gameObject.SetActive(false);
            RelationDataManager.GetInstance().OnAddPriChatList(m_relationData, false);
            ChatManager.GetInstance().OpenPrivateChatFrame(m_relationData);
            ClientSystemManager.instance.CloseFrame<RelationFrame>();

            UIEventSystem.GetInstance().SendUIEvent(new UIEventPrivateChat(m_relationData, true));
        }
    }

    class RelationFrame : WndFrame
    {
        #region var

        const int m_TabNum = 3;
        const string WndRoot = "Content/RelationFrame(Clone)/";

        [UIControl(WndRoot + "Tip", typeof(RectTransform))]
        protected RectTransform mEmptyTip;

        [UIControl(WndRoot + "BottomBG/AllAcceptBtn", typeof(RectTransform))]
        protected RectTransform mAllAccept;

        [UIControl(WndRoot + "BottomBG/AllRefuseBtn", typeof(RectTransform))]
        protected RectTransform mAllRefuse;

        [UIControl(WndRoot + "FriendListView/Viewport/Content", typeof(RectTransform))]
        protected RectTransform mListContent;

        [UIControl(WndRoot + "BottomBG/ImgFriendNum/TextNum", typeof(Text))]
        protected Text mFriendNum;

        [UIControl(WndRoot + "UpBG/ToggleGroup", typeof(ToggleGroup))]
        protected ToggleGroup mTabGroup;

        [UIControl(WndRoot + "UpBG/ToggleGroup/Toggle{0}", typeof(Toggle), 1)]
        protected Toggle[] m_Tab = new Toggle[m_TabNum];

        [UIControl(WndRoot + "UpBG/ToggleGroup/Toggle2/RedPt", typeof(Image))]
        protected Image m_redPt;

        [UIControl(WndRoot + "UpBG/Input/Text", typeof(Text))]
        protected Text m_queryName;

        [UIControl(WndRoot + "BottomBG/FriendListBtn", typeof(RectTransform))]
        protected RectTransform m_addFriend;
        
        [UIControl(WndRoot + "BottomBG/ImgFriendNum", typeof(RectTransform))]
        protected RectTransform m_frendNum;

        protected List<FriendInfo> m_friendInfoList = new List<FriendInfo>();       //好友列表
        protected List<FriendInfo> m_recommendList = new List<FriendInfo>();        //推荐列表 
        protected List<FriendInfo> m_inviteInfoList = new List<FriendInfo>();       //通知列表       
        FriendListType m_curState;
        IClientFrame m_openMenu;
        bool m_bQuerying;
        bool m_bGiveFriend;

        public bool GiveFriend
        {
            set
            {
                m_bGiveFriend = value;
            }
        }
        #endregion

        //public override string GetPrefabPath()
        public override string GetContentPath()
        {
            return "UIFlatten/Prefabs/Friends/RelationFrame.prefab";
        }

        public override string GetTitle()
        {
            return "好友";
        }

        protected override void _OnOpenFrame()
        {

            _RegisterUIEvent();
            //_SendUpdateRelation();
            Init();

        }

        protected override void _OnCloseFrame()
        {
            _UnRegisterUIEvent();
            _ClearFriendList();
            // _ClearNotifyList();

        }

        HelpFrameContentTable.eHelpType _GetHelpType()
        {
            return HelpFrameContentTable.eHelpType.HT_RELATION;
        }

        #region ui event
        protected void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRecievRecommendFriend, _OnRecievRecommendFriend);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshFriendList, _OnRefreshFriendList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshInviteList, _OnRefreshInviteList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnShowFriendSecMenu, _OnShowFrienSecInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCloseMenu, _OnCloseMenu);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnQueryEnd, _OnQueryEnd);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPrivateChat, _OnPrivateChat);
                
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnShowFriendChat, _OnPrivateChat);

        }

        protected void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRecievRecommendFriend, _OnRecievRecommendFriend);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshFriendList, _OnRefreshFriendList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshInviteList, _OnRefreshInviteList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnShowFriendSecMenu, _OnShowFrienSecInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCloseMenu, _OnCloseMenu);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnQueryEnd, _OnQueryEnd);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPrivateChat, _OnPrivateChat);

                //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnShowFriendChat, _OnPrivateChat);

            }

        #endregion

        #region ui fuc callback

        [UIEventHandle(WndRoot + "BottomBG/FriendListBtn")]
        protected void _OnQuerryFriend()
        {
            WorldRelationFindPlayersReq req = new WorldRelationFindPlayersReq();
            req.type = (byte)RelationFindType.Friend;
            req.name = "";
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        [UIEventHandle(WndRoot + "UpBG/ToggleGroup/Toggle{0}", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<int, bool>), 1, 3)]
        protected void _OnTabChanged(int index, bool isChecked)
        {
            if (isChecked)
            {
                if (0 == index)
                {
                    _ChangeFriendTab();

                }
                else if (1 == index)
                {
                    _ChangeInviteTab();

                }
                else if (2 == index)
                {
                    _ChangeBlackTab();
                }
            }

        }

        public void ChangeTabChange(int index)
        {
            m_Tab[index].isOn = true;
            if (0 == index)
            {
                _ChangeFriendTab();

            } 
            else if (1 == index)
            {
                _ChangeInviteTab();

            }
            else if (2 == index)
            {
                _ChangeBlackTab();
            }
        }

        [UIEventHandle(WndRoot + "BottomBG/AllAcceptBtn")]
        protected void _OnAllAcceptBtn()
        {
            List<InviteFriendData> inviteFriends = RelationDataManager.GetInstance().GetInviteFriendData();
            for (int i = 0; i < inviteFriends.Count; ++i)
            {

                SceneReply sendMsg = new SceneReply();
                sendMsg.type = (byte)RequestType.RequestFriend;
                sendMsg.requester = inviteFriends[i].requester;
                sendMsg.result = 1;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);


            }

            RelationDataManager.GetInstance().DelAllInviter();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshInviteList);
        }

        [UIEventHandle(WndRoot + "BottomBG/AllRefuseBtn")]
        protected void _OnAllRefuseBtn()
        {
            List<InviteFriendData> inviteFriends = RelationDataManager.GetInstance().GetInviteFriendData();
            for (int i = 0; i < inviteFriends.Count; ++i)
            {
                SceneReply sendMsg = new SceneReply();
                sendMsg.type = (byte)RequestType.RequestFriend;
                sendMsg.requester = inviteFriends[i].requester;
                sendMsg.result = 0;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, sendMsg);

            }

            RelationDataManager.GetInstance().DelAllInviter();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnRefreshInviteList);
        }

        [UIEventHandle(WndRoot + "UpBG/QueryBtn")]
        protected void _OnQueryFriend()
        {
            if (m_queryName.text == "")
            {
                return;
            }
            m_bQuerying = true;

            WorldQueryPlayerReq kCmd = new WorldQueryPlayerReq();
            kCmd.roleId = 0;
            OtherPlayerInfoManager.GetInstance().QueryPlayerType = WorldQueryPlayerType.WQPT_FRIEND;
            kCmd.name = m_queryName.text;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, kCmd);
        }

        protected void _ChangeFriendTab()
        {
            m_curState = FriendListType.FLT_FRIEND;
            AddFriendToList();

            mAllAccept.gameObject.SetActive(false);
            mAllRefuse.gameObject.SetActive(false);

            m_addFriend.gameObject.SetActive(true);
            m_frendNum.gameObject.SetActive(true);
        }

        protected void _ChangeInviteTab()
        {
            m_curState = FriendListType.FLT_NOTIFY;
            AddInviteFriend();

            mAllAccept.gameObject.SetActive(true);
            mAllRefuse.gameObject.SetActive(true);
            mEmptyTip.gameObject.SetActive(false);
            m_addFriend.gameObject.SetActive(true);
            m_frendNum.gameObject.SetActive(true);
        }

        protected void _ChangeBlackTab()
        {
            m_curState = FriendListType.FLT_BLACK;

            mAllAccept.gameObject.SetActive(false);
            mAllRefuse.gameObject.SetActive(false);
            mEmptyTip.gameObject.SetActive(false);
            m_addFriend.gameObject.SetActive(false);
            m_frendNum.gameObject.SetActive(false);

            
            _AddBlackList();
        }

        [UIEventHandle("Title/Close")]
        void OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Title/Help")]
        void OnClickHelp()
        {
            frameMgr.OpenFrame<HelpFrame>(GameClient.FrameLayer.Middle, _GetHelpType());
        }

        #endregion

        public bool IsQuerying()
        {
            return m_bQuerying;
        }

        public void _OnQueryEnd(UIEvent uiEvent)
        {
            m_bQuerying = false;
        }

        protected void _OnPrivateChat(UIEvent uiEvent)
        {
            UIEventPrivateChat evt = uiEvent as UIEventPrivateChat;
            
            for(int i = 0; i < m_friendInfoList.Count; ++i)
            {
                if (m_friendInfoList[i].m_uid == evt.m_data.uid)
                {
                     m_friendInfoList[i].ShowRedPoint();
                     return;
                }
            }
        
        }

        protected void AddFriendToList()
        {
            //Logger.LogErrorFormat("AddFriendToList");

            _ClearFriendList();
            List<RelationData> relationList = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_FRIEND);

            m_addList = GameFrameWork.instance.StartCoroutine(_AddRelationDataToList(relationList, FriendListType.FLT_FRIEND));
            
            mEmptyTip.gameObject.SetActive(relationList.Count > 0 ? false : true);
            mFriendNum.text = String.Format("{0}", relationList.Count);
        }

        protected void AddInviteFriend()
        {
            _ClearFriendList();
            List<InviteFriendData> inviteFriends = RelationDataManager.GetInstance().GetInviteFriendData();

            if (inviteFriends.Count > 0)
            {
                m_redPt.gameObject.SetActive(true);

                m_addList = GameFrameWork.instance.StartCoroutine(_AddInviteDataToList(inviteFriends, FriendListType.FLT_NOTIFY));
            }
            else
            {
                m_redPt.gameObject.SetActive(false);
            }
        }

        protected void Init()
        {
            m_bGiveFriend = false;
            _InitRedPoint();
            m_Tab[0].isOn = true;
            for (int i = 1; i < m_TabNum; ++i)
            {
                m_Tab[i].isOn = false;
            }
            //_ChangeFriendTab();
        }

        #region friend info op
        protected void ShowFriendInfo(GameObject friend, RelationData data)
        {

            Text name = Utility.GetComponetInChild<Text>(friend, "Left/Name");
            name.text = string.Format("Lv:     {0}", data.name);

            Text Lv = Utility.GetComponetInChild<Text>(friend, "Left/Name/Lv");
            Lv.text = string.Format("{0}", data.level);

            GameObject headIcon_sword = Utility.FindGameObject(friend, "Left/HeadBoard/OccuHead_1");
            GameObject headIcon_gun = Utility.FindGameObject(friend, "Left/HeadBoard/OccuHead_2");
            GameObject headIcon_magic = Utility.FindGameObject(friend, "Left/HeadBoard/OccuHead_3");
            headIcon_sword.SetActive(false);
            headIcon_gun.SetActive(false);
            headIcon_magic.SetActive(false);
            if (data.occu / 10 == 1) headIcon_sword.SetActive(true);
            else if (data.occu / 10 == 2) headIcon_gun.SetActive(true);
            else if (data.occu / 10 == 3) headIcon_magic.SetActive(true);

            GameObject occu_sword = Utility.FindGameObject(friend, "Left/Occu_1");
            GameObject occu_gun = Utility.FindGameObject(friend, "Left/Occu_2");
            GameObject occu_magic = Utility.FindGameObject(friend, "Left/Occu_3");
            occu_sword.SetActive(false);
            occu_gun.SetActive(false);
            occu_magic.SetActive(false);
            if (data.occu / 10 == 1) occu_sword.SetActive(true);
            else if (data.occu / 10 == 2) occu_gun.SetActive(true);
            else if (data.occu / 10 == 3) occu_magic.SetActive(true);

            GameObject talk = Utility.FindGameObject(friend, "Right/Talk");
            GameObject give = Utility.FindGameObject(friend, "Right/Give");
            GameObject accept = Utility.FindGameObject(friend, "Right/Accept");
            GameObject refuse = Utility.FindGameObject(friend, "Right/Refuse");
            GameObject alreadyGive = Utility.FindGameObject(friend, "Right/AlreadyGive");

            talk.SetActive(true);
            give.SetActive(true);
            accept.SetActive(false);
            refuse.SetActive(false);
            alreadyGive.SetActive(false);
        }
        #endregion

        protected void _OnRecievRecommendFriend(UIEvent uiEvent)
        {
            UIEventRecievRecommendFriend recvList = uiEvent as UIEventRecievRecommendFriend;

            _AddRecommendFriend(recvList.m_friendList);
        }

        protected void _OnRefreshFriendList(UIEvent uiEvent)
        {
            if (true == m_bGiveFriend)
            {
                return;
            }

            _OnCloseMenu(new UIEvent());
            if (m_curState == FriendListType.FLT_FRIEND)
            {
                AddFriendToList();
                
            }
            else if (m_curState == FriendListType.FLT_BLACK)
            {
                _AddBlackList();
                
            }

        }

        protected void _OnRefreshInviteList(UIEvent uiEvent)
        {
            _InitRedPoint();
            if (m_curState == FriendListType.FLT_NOTIFY)
            {
                AddInviteFriend();
                _OnCloseMenu(new UIEvent());
            }
        }

        protected void _OnNotifyInvite(UIEvent uiEvent)
        {
            _InitRedPoint();
        }

        protected void _InitRedPoint()
        {
            List<InviteFriendData> inviteFriends = RelationDataManager.GetInstance().GetInviteFriendData();
            if (inviteFriends.Count > 0)
            {
                m_redPt.gameObject.SetActive(true);
            }
            else
            {
                m_redPt.gameObject.SetActive(false);
            }
        }

        protected void _OnShowFrienSecInfo(UIEvent uiEvent)
        {
            
            UIEventShowFriendSecMenu myEvent = uiEvent as UIEventShowFriendSecMenu;

            m_openMenu = frameMgr.OpenFrame<RelationMenuFram>(FrameLayer.Middle, myEvent.m_data);

        }

        protected void _OnCloseMenu(UIEvent uiEvent)
        {
            if (m_openMenu != null)
            {
                frameMgr.CloseFrame(m_openMenu);
                m_openMenu = null;
            }
        }


        //添加推荐好友
        protected void _AddRecommendFriend(RelationData[] list)
        {
            if (frameMgr.IsFrameOpen<RelationPopupFram>())
            {
                return;
            }

            RelationPopupFram popUpFrame = (RelationPopupFram)frameMgr.OpenFrame<RelationPopupFram>(frame, list);
            popUpFrame.SetData(list);
        }

        protected void _ClearNotifyList()
        {
            _ClearRecommendFriend();
            _ClearInviteList();
        }

        protected void _ClearRecommendFriend()
        {
            for (int i = 0; i < m_recommendList.Count; ++i)
            {
                //CGameObjectPool.instance.RecycleGameObject(m_recommendList[i].m_friendPrefab);
                m_recommendList[i].Finatial();
            }

            m_recommendList.Clear();
        }

        protected void _ClearFriendList()
        {
            if (m_addList != null)
            {
                GameFrameWork.instance.StopCoroutine(m_addList);
            }

            for (int i = 0; i < m_friendInfoList.Count; ++i)
            {
                //CGameObjectPool.instance.RecycleGameObject(m_friendInfoList[i].m_friendPrefab);
                m_friendInfoList[i].Finatial();
            }

            m_friendInfoList.Clear();

            int a = 0;
        }

        private UnityEngine.Coroutine m_addList = null;
        protected void _AddBlackList()
        {
            _ClearFriendList();
            List<RelationData> relationList = RelationDataManager.GetInstance().GetRelation((byte)RelationType.RELATION_BLACKLIST);

            m_addList = GameFrameWork.instance.StartCoroutine(_AddRelationDataToList(relationList, FriendListType.FLT_BLACK));
        
        }

        public IEnumerator _AddRelationDataToList(List<RelationData> relationList, FriendListType type)
        {
            
            for (int i = 0; i < relationList.Count; ++i)
            {
                FriendInfo friend = new FriendInfo(relationList[i].uid, relationList[i].name, relationList[i].level,
                    relationList[i].occu, relationList[i].dayGiftNum, type, relationList[i].isOnline, relationList[i].vipLv,
                    relationList[i].seasonLv);
                friend.SetRelationData(relationList[i]);

                m_friendInfoList.Add(friend);

                friend.SetCustomParent(mListContent);

                yield return null;
            }

           
        }

        public IEnumerator _AddInviteDataToList(List<InviteFriendData> inviteFriends, FriendListType type)
        {
            
            for (int i = 0; i < inviteFriends.Count; ++i)
            {
                FriendInfo friend = new FriendInfo(inviteFriends[i].requester, inviteFriends[i].requesterName,
                       inviteFriends[i].requesterLevel, inviteFriends[i].requesterOccu, 0, type, 1, inviteFriends[i].vipLv,
                       0);

                m_friendInfoList.Add(friend);

                friend.SetCustomParent(mListContent);

                yield return null;
            }

            
        }


        protected void _ClearInviteList()
        {
            for (int i = 0; i < m_inviteInfoList.Count; ++i)
            {
                CGameObjectPool.instance.RecycleGameObject(m_inviteInfoList[i].m_friendPrefab);
                m_inviteInfoList[i] = null;
            }

            m_inviteInfoList.Clear();
        }

        protected void _SendUpdateRelation()
        {
            WorldUpdateRelation req = new WorldUpdateRelation();
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

    }
}
