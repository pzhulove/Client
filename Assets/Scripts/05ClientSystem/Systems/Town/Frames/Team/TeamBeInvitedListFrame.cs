using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;
using ProtoTable;
using Scripts.UI;

namespace GameClient
{
    /// <summary>
    /// 邀请界面也签类型
    /// </summary>
    public enum InvitedTabType
    {
        ITT_None = 0,
        /// <summary>
        /// 组队决斗
        /// </summary>
        ITT_DuelTeam,
        /// <summary>
        /// //好友切磋
        /// </summary>
        ITT_FriendsPlay,
        /// <summary>
        /// //组队
        /// </summary>
        ITT_Team,
        /// <summary>
        /// //公会
        /// </summary>
        ITT_Guild,
        ITT_Count,
    }

    /// <summary>
    /// 页签数据
    /// </summary>
    public class InvitedTabData
    {
        public InvitedTabType mInvitedTabType;
        public string mTabName;
    }
    
    class TeamBeInvitedListFrame : ClientFrame
    {
        string BeInvitedListElePath = "UIFlatten/Prefabs/Team/TeamBeInvitedEle";

        InviteType inviteType = InviteType.None;

        List<GameObject> EleObjList = new List<GameObject>();

        private InvitedTabType invitedTabType = InvitedTabType.ITT_None;

        private List<InvitedTabData> mMainTabDataList = new List<InvitedTabData>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamBeInvitedListFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                inviteType = (InviteType)userData;
            }

            InitMainTabUIList();
            InitInterface();
            BindUIEvent();
            UpdateMainTabList();
        }

        protected override void _OnCloseFrame()
        {
            UnBindUIEvent();
            ClearData();
            UnInitMainTabUIList();
        }

        #region MainTab

        private void InitMainTabUIList()
        {
            if (mMainTabs != null)
            {
                mMainTabs.Initialize();
                mMainTabs.onBindItem += OnBindItemDelegate;
                mMainTabs.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitMainTabUIList()
        {
            if (mMainTabs != null)
            {
                mMainTabs.onBindItem -= OnBindItemDelegate;
                mMainTabs.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private TeamInvitedTabItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<TeamInvitedTabItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var teamInvitedTabItem = item.gameObjectBindScript as TeamInvitedTabItem;
            if (teamInvitedTabItem != null && item.m_index >= 0 && item.m_index < mMainTabDataList.Count)
            {
                var mainTabData = mMainTabDataList[item.m_index];
                if (invitedTabType != InvitedTabType.ITT_None)
                {
                    teamInvitedTabItem.InitTab(mainTabData, OnInvitedTabClick, mainTabData.mInvitedTabType == invitedTabType);
                }
                else
                {
                    teamInvitedTabItem.InitTab(mainTabData, OnInvitedTabClick, item.m_index == 0);
                }
            }
        }

        private void OnInvitedTabClick(InvitedTabData invitedTabData)
        {
            if (invitedTabData == null)
            {
                return;
            }

            invitedTabType = invitedTabData.mInvitedTabType;

            UpdateEleObjList();
        }

        #endregion

        void ClearData()
        {
            inviteType = InviteType.None;

            for (int i = 0; i < EleObjList.Count; i++)
            {
                if(EleObjList[i] == null)
                {
                    continue;
                }

                ComCommonBind bind = EleObjList[i].GetComponent<ComCommonBind>();
                if(bind == null)
                {
                    continue;
                }

                GameObject BtReject = bind.GetGameObject("BtReject");
                BtReject.GetComponent<Button>().onClick.RemoveAllListeners();

                GameObject BtAgree = bind.GetGameObject("BtAgree");
                BtAgree.GetComponent<Button>().onClick.RemoveAllListeners();
            }

            EleObjList.Clear();

            invitedTabType = InvitedTabType.ITT_None;

            if (mMainTabDataList != null)
            {
                mMainTabDataList.Clear();
            }
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3InviteRoomListUpdate, OnNewInviteNoticeUpdate);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3InviteRoomListUpdate, OnNewInviteNoticeUpdate);
        }

        void OnNewInviteNoticeUpdate(UIEvent iEvent)
        {
            UpdateMainTabList();
            UpdateEleObjList();
        }

        void OnReject(int index)
        {
            NetManager netMgr = NetManager.Instance();

            byte roomType = 0;

            if (invitedTabType == InvitedTabType.ITT_DuelTeam)
            {
                List<WorldSyncRoomInviteInfo> InviteRoomList = Pk3v3DataManager.GetInstance().GetInviteRoomList();
                InviteRoomList.AddRange(Pk3v3CrossDataManager.GetInstance().GetInviteRoomList());
              
                if (index < 0 || index > InviteRoomList.Count)
                {
                    return;
                }

                WorldBeInviteRoomReq roomInviteReq = new WorldBeInviteRoomReq();

                roomInviteReq.roomId = InviteRoomList[index].roomId;
                roomInviteReq.invitePlayerId = InviteRoomList[index].inviterId;
                roomInviteReq.isAccept = 0;
                roomInviteReq.slotGroup = InviteRoomList[index].slotGroup;

                netMgr.SendCommand(ServerType.GATE_SERVER, roomInviteReq);
                roomType = InviteRoomList[index].roomType;

                InviteRoomList.RemoveAt(index);

                if (InviteRoomList.Count <= 0)
                {
                    invitedTabType = InvitedTabType.ITT_None;
                }
            }
            else
            {
                List<SceneSyncRequest> friendsPlayInviteList = RelationDataManager.GetInstance().FriendsPlayInviteList;
                if (index < 0 || index > friendsPlayInviteList.Count)
                {
                    return;
                }

                RelationDataManager.GetInstance().ReplyRequest(friendsPlayInviteList[index], 0);

                friendsPlayInviteList.RemoveAt(index);

                if (friendsPlayInviteList.Count <= 0)
                {
                    invitedTabType = InvitedTabType.ITT_None;
                }
            }

            UpdateEleObjList();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3InviteRoomListUpdate, roomType);

            List<WorldSyncRoomInviteInfo> InviteRoomList2 = Pk3v3DataManager.GetInstance().GetInviteRoomList();
            InviteRoomList2.AddRange(Pk3v3CrossDataManager.GetInstance().GetInviteRoomList());
            List<SceneSyncRequest> friendsPlayInviteList2 = RelationDataManager.GetInstance().FriendsPlayInviteList;

            if (InviteRoomList2.Count <= 0 &&
                friendsPlayInviteList2.Count <= 0)
            {
                frameMgr.CloseFrame(this);
            }
        }
        
        void OnAgree(int index)
        {
            if (PkWaitingRoom.bBeginSeekPlayer)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            byte roomType = 0;

            NetManager netMgr = NetManager.Instance();

            if (invitedTabType == InvitedTabType.ITT_DuelTeam)
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

                List<WorldSyncRoomInviteInfo> InviteRoomList = Pk3v3DataManager.GetInstance().GetInviteRoomList();
                InviteRoomList.AddRange(Pk3v3CrossDataManager.GetInstance().GetInviteRoomList());
              
                if (index < 0 || index > InviteRoomList.Count)
                {
                    return;
                }

                if (Pk3v3DataManager.HasInPk3v3Room() || Pk3v3CrossDataManager.HasInPk3v3Room())
                {
                    SystemNotifyManager.SystemNotify(3401008);
                    return;
                }

                if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.BUDO)
                {
                    SystemNotifyManager.SystemNotify(9307);
                    return;
                }

                if (InviteRoomList[index].roomType == (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR && Pk3v3DataManager.HasInPk3v3Room())
                {
                    return;
                }

                if (InviteRoomList[index].roomType != (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR && Pk3v3CrossDataManager.GetInstance().CheckPk3v3CrossScence())
                {
                    return;
                }

                if (InviteRoomList[index].roomType == (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
                {
                    var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_SCORE_WAR_LEVEL);
                    if (SystemValueTableData != null && PlayerBaseData.GetInstance().Level < SystemValueTableData.Value)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(string.Format("该活动需要达到{0}级后才能加入", SystemValueTableData.Value));
                        return;
                    }

                    if (TeamDataManager.GetInstance().HasTeam())
                    {
                        SystemNotifyManager.SysNotifyMsgBoxOkCancel("进入积分赛场景会退出当前所在队伍，是否确认进入？", () =>
                        {
                            TeamDataManager.GetInstance().QuitTeam(PlayerBaseData.GetInstance().RoleID);

                            WorldBeInviteRoomReq roomInviteReq1 = new WorldBeInviteRoomReq();

                            roomInviteReq1.roomId = InviteRoomList[index].roomId;
                            roomInviteReq1.invitePlayerId = InviteRoomList[index].inviterId;
                            roomInviteReq1.isAccept = 1;
                            roomInviteReq1.slotGroup = InviteRoomList[index].slotGroup;

                            netMgr.SendCommand(ServerType.GATE_SERVER, roomInviteReq1);

                            roomType = InviteRoomList[index].roomType;
                            InviteRoomList.RemoveAt(index);

                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3InviteRoomListUpdate, roomType);

                            frameMgr.CloseFrame(this);

                        });

                        return;
                    }

                    Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
                    if (pkInfo != null && pkInfo.nCurPkCount >= Pk3v3CrossDataManager.MAX_PK_COUNT)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("挑战次数已达上限，操作失败");
                        return;
                    }

                    if (frameMgr.IsFrameOpen<PkSeekWaiting>())
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("匹配中无法进行该操作，请取消后再试");
                        return;
                    }                   
                }

                WorldBeInviteRoomReq roomInviteReq = new WorldBeInviteRoomReq();

                roomInviteReq.roomId = InviteRoomList[index].roomId;
                roomInviteReq.invitePlayerId = InviteRoomList[index].inviterId;
                roomInviteReq.isAccept = 1;
                roomInviteReq.slotGroup = InviteRoomList[index].slotGroup;

                netMgr.SendCommand(ServerType.GATE_SERVER, roomInviteReq);

                roomType = InviteRoomList[index].roomType;
                InviteRoomList.RemoveAt(index);

                if (InviteRoomList.Count <= 0)
                {
                    invitedTabType = InvitedTabType.ITT_None;
                }
            }
            else
            {
                List<SceneSyncRequest> friendsPlayInviteList = RelationDataManager.GetInstance().FriendsPlayInviteList;
                if (index < 0 || index > friendsPlayInviteList.Count)
                {
                    return;
                }

                RelationDataManager.GetInstance().ReplyRequest(friendsPlayInviteList[index], 1);

                friendsPlayInviteList.RemoveAt(index);
                if (friendsPlayInviteList.Count <= 0)
                {
                    invitedTabType = InvitedTabType.ITT_None;
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3InviteRoomListUpdate, roomType);

            frameMgr.CloseFrame(this);
        }

        void InitInterface()
        {
            if(inviteType == InviteType.Pk3v3Invite)
            {
                mTitle.text = "决斗邀请列表";
            }
        }

        private void UpdateMainTabList()
        {
            if (mMainTabDataList != null)
            {
                mMainTabDataList.Clear();
            }

            for (int i = 0; i < (int)InvitedTabType.ITT_Count; i++)
            {
                int RealNum = 0;
                if (i == (int)InvitedTabType.ITT_DuelTeam)
                {
                    RealNum = Pk3v3DataManager.GetInstance().GetInviteRoomList().Count;
                    RealNum += Pk3v3CrossDataManager.GetInstance().GetInviteRoomList().Count;
                    if (RealNum > 0)
                    {
                        InvitedTabData tabData = new InvitedTabData();
                        tabData.mInvitedTabType = InvitedTabType.ITT_DuelTeam;
                        tabData.mTabName = "组队决斗";

                        mMainTabDataList.Add(tabData);
                    }
                }
                
                if (i == (int)InvitedTabType.ITT_FriendsPlay)
                {
                    RealNum = RelationDataManager.GetInstance().FriendsPlayInviteList.Count;
                    if (RealNum > 0)
                    {
                        InvitedTabData tabData = new InvitedTabData();
                        tabData.mInvitedTabType = InvitedTabType.ITT_FriendsPlay;
                        tabData.mTabName = "好友切磋";

                        mMainTabDataList.Add(tabData);
                    }
                }
            }

            mMainTabs.SetElementAmount(mMainTabDataList.Count);
        }

        void UpdateEleObjList()
        {
            int RealNum = 0;
            if(invitedTabType == InvitedTabType.ITT_DuelTeam)
            {
                RealNum = Pk3v3DataManager.GetInstance().GetInviteRoomList().Count;
                RealNum += Pk3v3CrossDataManager.GetInstance().GetInviteRoomList().Count;
            }
            else
            {
                RealNum = RelationDataManager.GetInstance().FriendsPlayInviteList.Count;
            }

            if(RealNum > EleObjList.Count)
            {
                int iDiff = RealNum - EleObjList.Count;

                for (int i = 0; i < iDiff; i++)
                {
                    GameObject EleObj = AssetLoader.instance.LoadResAsGameObject(BeInvitedListElePath);
                    if (EleObj == null)
                    {
                        continue;
                    }

                    EleObj.transform.SetParent(mEleRoot.transform, false);

                    EleObjList.Add(EleObj);
                }
            }

            for(int i = 0; i < EleObjList.Count; i++)
            {
                if(i < RealNum)
                {
                    ComCommonBind commonbind = EleObjList[i].GetComponent<ComCommonBind>();
                    if(commonbind == null)
                    {
                        EleObjList[i].SetActive(false);
                        continue;
                    }

                    GameObject icon = commonbind.GetGameObject("Icon");
                    GameObject Name = commonbind.GetGameObject("Name");
                    GameObject level = commonbind.GetGameObject("Level");
                    GameObject target = commonbind.GetGameObject("Target");
                    GameObject reject = commonbind.GetGameObject("BtReject");
                    GameObject agree = commonbind.GetGameObject("BtAgree");
                    Text name = Name.GetComponent<Text>();
                    int JobID = 0;
                    if(invitedTabType == InvitedTabType.ITT_DuelTeam)
                    {

                        List<WorldSyncRoomInviteInfo> InviteRoomList = Pk3v3DataManager.GetInstance().GetInviteRoomList();
                        InviteRoomList.AddRange(Pk3v3CrossDataManager.GetInstance().GetInviteRoomList());
                      
                        name.text = string.Format("{0}  ({1}/{2})", InviteRoomList[i].inviterName, InviteRoomList[i].playerSize, InviteRoomList[i].playerMaxSize);
                        RelationData relationData = null;
                        RelationDataManager.GetInstance().FindPlayerIsRelation(InviteRoomList[i].inviterId, ref relationData);
                        if (relationData != null)
                        {
                            if (relationData.remark != null && relationData.remark != "")
                            {
                                name.text = string.Format("{0}  ({1}/{2})", relationData.remark, InviteRoomList[i].playerSize, InviteRoomList[i].playerMaxSize);
                            }
                        }
                        level.GetComponent<Text>().text = string.Format("Lv.{0}", InviteRoomList[i].inviterLevel);                      
                        JobID = InviteRoomList[i].inviterOccu;

                        target.GetComponent<Text>().text = string.Format("邀请你进入{0}号房间", InviteRoomList[i].roomId);

                        if (InviteRoomList[i].roomType == (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
                        {
                            target.GetComponent<Text>().text = "邀请你进入积分赛队伍";
                        }
                    }
                    else
                    {
                        List<SceneSyncRequest> friendsPlayInviteList = RelationDataManager.GetInstance().FriendsPlayInviteList;
                        name.text = friendsPlayInviteList[i].requesterName;
                        RelationData relationData = null;
                        RelationDataManager.GetInstance().FindPlayerIsRelation(friendsPlayInviteList[i].requester, ref relationData);
                        if (relationData != null)
                        {
                            if (relationData.remark != null && relationData.remark != "")
                            {
                                name.text = relationData.remark;
                            }
                        }

                        level.GetComponent<Text>().text = string.Format("Lv.{0}", friendsPlayInviteList[i].requesterLevel);
                        JobID = friendsPlayInviteList[i].requesterOccu;

                        target.GetComponent<Text>().text = "邀请你进行好友切磋";
                    }

                    JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(JobID);
                    if (jobData != null)
                    {
                        ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                        if (resData != null)
                        {         
                            Image image = icon.GetComponent<Image>();
                            ETCImageLoader.LoadSprite(ref image, resData.IconPath);
                        }
                    }
                   
                    Button btReject = reject.GetComponent<Button>();
                    btReject.onClick.RemoveAllListeners();
                    int index = i;
                    btReject.onClick.AddListener(() => { OnReject(index); });

                    Button btAgree = agree.GetComponent<Button>();
                    btAgree.onClick.RemoveAllListeners();
                    int iIndex = i;
                    btAgree.onClick.AddListener(() => { OnAgree(iIndex); });

                    EleObjList[i].SetActive(true);
                }
                else
                {
                    EleObjList[i].SetActive(false);
                }
            }
        }

        #region ExtraUIBind
        private Button mBtRejectAll = null;
        private GameObject mEleRoot = null;
        private Button mBtClose = null;
        private Text mTitle = null;
        private ComUIListScript mMainTabs = null;

        protected override void _bindExUI()
        {
            mBtRejectAll = mBind.GetCom<Button>("BtRejectAll");
            mBtRejectAll.onClick.AddListener(_onBtRejectAllButtonClick);
            mEleRoot = mBind.GetGameObject("EleRoot");
            mBtClose = mBind.GetCom<Button>("BtClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mTitle = mBind.GetCom<Text>("Title");
            mMainTabs = mBind.GetCom<ComUIListScript>("MainTabs");
        }

        protected override void _unbindExUI()
        {
            mBtRejectAll.onClick.RemoveListener(_onBtRejectAllButtonClick);
            mBtRejectAll = null;
            mEleRoot = null;
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mTitle = null;
            mMainTabs = null;
        }
        #endregion

        #region Callback
        private void _onBtRejectAllButtonClick()
        {
            NetManager netMgr = NetManager.Instance();

            byte roomType = 0;

            if(invitedTabType ==  InvitedTabType.ITT_DuelTeam)
            {
                roomType = (byte)RoomType.ROOM_TYPE_THREE_FREE;
                List<WorldSyncRoomInviteInfo> InviteRoomList = Pk3v3DataManager.GetInstance().GetInviteRoomList();
                InviteRoomList.AddRange(Pk3v3CrossDataManager.GetInstance().GetInviteRoomList());
            
                for (int i = 0; i < InviteRoomList.Count; i++)
                {
                    WorldBeInviteRoomReq roomInviteReq = new WorldBeInviteRoomReq();

                    roomInviteReq.roomId = InviteRoomList[i].roomId;
                    roomInviteReq.invitePlayerId = InviteRoomList[i].inviterId;
                    roomInviteReq.isAccept = 0;
                    roomInviteReq.slotGroup = InviteRoomList[i].slotGroup;

                    netMgr.SendCommand(ServerType.GATE_SERVER, roomInviteReq);
                }

                InviteRoomList.Clear();
            }
            else
            {
                List<SceneSyncRequest> friendsPlayInviteList = RelationDataManager.GetInstance().FriendsPlayInviteList;

                for (int i = 0; i < friendsPlayInviteList.Count; i++)
                {
                    RelationDataManager.GetInstance().ReplyRequest(friendsPlayInviteList[i], 0);
                }

                friendsPlayInviteList.Clear();
            }

            invitedTabType = InvitedTabType.ITT_None;

            UpdateEleObjList();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3InviteRoomListUpdate, roomType);

            List<WorldSyncRoomInviteInfo> mInviteRoomList = Pk3v3DataManager.GetInstance().GetInviteRoomList();
            mInviteRoomList.AddRange(Pk3v3CrossDataManager.GetInstance().GetInviteRoomList());
            List<SceneSyncRequest> mfriendsPlayInviteList = RelationDataManager.GetInstance().FriendsPlayInviteList;

            if (mInviteRoomList.Count <= 0 && mfriendsPlayInviteList.Count <= 0)
            {
                frameMgr.CloseFrame(this);
            }
        }

        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
