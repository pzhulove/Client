using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System;
using Protocol;
using Network;

namespace GameClient
{
    public enum eTeamFrameState1 
    {
        /// <summary>
        /// 无
        /// </summary>
        onNone,

        /// <summary>
        /// 查看列表
        /// </summary>
        onList,

        /// <summary>
        /// 查看队伍
        /// </summary>
        onTeam,
    }

    // 我的队伍界面,里面有嵌入了队伍成员界面Pk3v3CrossTeamMyFrame
    class Pk3v3CrossMyTeamFrame : ClientFrame
    {
        private eTeamFrameState mState = eTeamFrameState.onNone;

        private const string kMenuPath = "UIFlatten/Prefabs/Team/TeamMenuGroup";
        private int mCurrentFilterIndex = 0;
        private bool bStartMatch = false;

        private List<int> FliterFirstMenuList = new List<int>();
        private Dictionary<int, List<int>> FliterSecondMenuDict = new Dictionary<int, List<int>>();

        [UIControl("TargetRoot/pkCount")]
        Text txtPkCount;

        [UIControl("TargetRoot/myScore")]
        Text txtmyScore;

        [UIControl("TargetRoot/myrRank")]
        Text txtmyrRank;


        private int CurTeamDungeonTableID 
        {
            get 
            {
                int id = (int)TeamDataManager.GetInstance().TeamDungeonID;
                if (id <= 0)
                {
                    id = (int)TeamUtility.kDefaultTeamDungeonID;
                }

                return id;
            }
        }
       
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3Cross/Pk3v3CrossMyTeam";
        }

        private class TeamDungeonNode
        {
            public int              id;
            public TeamDungeonTable table;
            public ComCommonBind    bind;
        }

        /// <summary>
        /// 所有节点
        ///
        /// 包含菜单节点
        /// 包含子项节点
        /// </summary>
        List<TeamDungeonNode> mAllNodes = new List<TeamDungeonNode>();

        public static void OnOpenLinkFrame(string argv)
        {
            ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossTeamListFrame>(FrameLayer.Middle);
        }

        protected override void _OnOpenFrame()
        {
            int autoID = 1;

            if (userData != null)
            {
                autoID = (int)userData;
                TeamDataManager.GetInstance().TeamDungeonID = (uint)autoID;
            }
            else 
            {
                autoID = CurTeamDungeonTableID;
            }

            //Logger.LogProcessFormat("[组队] 打开组队界面 teamDungeonID {0}", autoID);

            //打开组队界面的时候，停止自动寻路
            GameClient.ClientSystemTown clientSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
            if (clientSystem != null && clientSystem.MainPlayer != null)
            {
                clientSystem.MainPlayer.CommandStopMove();
            }

            _updateState();
            _loadFirstMenuList();
            BindUIEvent();

            Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            Pk3v3CrossDataManager.ScoreListItem myRankInfo = Pk3v3CrossDataManager.GetInstance().GetMyRankInfo();
            if(pkInfo != null && myRankInfo != null)
            {
                if (txtPkCount != null)
                {
                    txtPkCount.text = string.Format("比赛次数:{0}/{1}", pkInfo.nCurPkCount, 5);
                }

                if(txtmyScore != null)
                {
                    txtmyScore.text = string.Format("个人积分:{0}", pkInfo.nScore);
                }

                if(txtmyrRank != null)
                {
                    txtmyrRank.text = string.Format("个人排名:{0}", myRankInfo.nRank);
                }
            }           

            OnChooseFirstMenu(TeamUtility.GetMenuTeamDungeonID(autoID), true);
            OnChooseSecondMenu(autoID, true);

            OnPk3v3RoomInfoUpdate(null);
        }

        private void _updateState()
        {
            mState = Pk3v3CrossDataManager.GetInstance().HasTeam() ? eTeamFrameState.onTeam: eTeamFrameState.onList;
        }

        protected override void _OnCloseFrame()
        {
            Clear();        

            //ClientSystemManager.instance.CloseFrame<TeamListViewFrame>();
            ClientSystemManager.instance.CloseFrame<Pk3v3CrossTeamMyFrame>();
        }

        void Clear()
        {   
            mCurrentFilterIndex = 0;

            _clearAllCacheNode();

            FliterFirstMenuList.Clear();
            FliterSecondMenuDict.Clear();

            UnBindUIEvent();

            //TeamDataManager.GetInstance().SearchInfo.Reset();
        }

        void BindUIEvent()
        {
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamCreateSuccess, OnCreateTeamSuccess);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, OnCreateTeamSuccess);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamJoinSuccess, OnCreateTeamSuccess);
            //             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamInfoUpdateSuccess, _onTeamInfoUpdateSuccess);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3RoomInfoUpdate, OnPk3v3RoomInfoUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3RoomSimpleInfoUpdate, OnPk3v3RoomSimpleInfoUpdate);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3BeginMatch, OnBeginMatch); 
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3CancelMatch, OnCancelMatch);          

            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3CrossRoomSlotInfoUpdate, OnPk3v3RoomSlotInfoUpdate);
        }

        void UnBindUIEvent()
        {
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamCreateSuccess, OnCreateTeamSuccess);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, OnCreateTeamSuccess);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamJoinSuccess, OnCreateTeamSuccess);
            //             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamInfoUpdateSuccess, _onTeamInfoUpdateSuccess);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3RoomInfoUpdate, OnPk3v3RoomInfoUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3RoomSimpleInfoUpdate, OnPk3v3RoomSimpleInfoUpdate);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3CrossRoomSlotInfoUpdate, OnPk3v3RoomSlotInfoUpdate);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3BeginMatch, OnBeginMatch);        
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3CancelMatch, OnCancelMatch);     
        }

        void UpdateDropListBtnState()
        {
            mOnDiffSelect.interactable = Pk3v3CrossDataManager.GetInstance().IsTeamLeader();
            mOnLevelSelect.interactable = Pk3v3CrossDataManager.GetInstance().IsTeamLeader();

            if (mOnDiffSelect.interactable && frameMgr.IsFrameOpen<PkSeekWaiting>())
            {
                mOnDiffSelect.interactable = false;
            }

            if (mOnLevelSelect.interactable && frameMgr.IsFrameOpen<PkSeekWaiting>())
            {
                mOnLevelSelect.interactable = false;
            }
        }

        void OnBeginMatch(UIEvent iEvent)
        {
            UpdateDropListBtnState();
        }    

        void OnCancelMatch(UIEvent iEvent)
        {
            UpdateDropListBtnState();
        }

        // 刷新房间信息,包括房间基础数据和房间成员数据
        void OnPk3v3RoomInfoUpdate(UIEvent iEvent)
        {
            UpdateDropListBtnState();

            // 刷新等级要求 段位要求
            OnPk3v3RoomSimpleInfoUpdate(null);

            // 刷新成员数据
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RoomSlotInfoUpdate);
        }

        // 刷新房间基础数据
        void OnPk3v3RoomSimpleInfoUpdate(UIEvent iEvent)
        {
            UpdateDropListBtnState();

            // 刷新等级要求 段位要求

            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
            if(roomInfo == null)
            {
                return;
            }

            int iIndex = mOnDiffSelect.value;        
            for (int i = 0;i < seasonLvs.Length;i++)
            {
                int nLevel = 0;
                int.TryParse(seasonLvs[i], out nLevel);
                if (roomInfo.roomSimpleInfo.isLimitPlayerSeasonLevel > 0 && roomInfo.roomSimpleInfo.limitPlayerSeasonLevel == (uint)Pk3v3CrossDataManager.GetInstance().GetRankLvByIndex(i))
                {
                    iIndex = i;
                    break;
                }
            }
            
            if(mOnDiffSelect.value != iIndex)
            {
                mOnDiffSelect.value = iIndex;
            }

            iIndex = mOnLevelSelect.value;
            for (int i = 0; i < limitLvs.Length; i++)
            {
                int nLevel = 0;
                int.TryParse(limitLvs[i], out nLevel);
                if (roomInfo.roomSimpleInfo.isLimitPlayerLevel > 0 && roomInfo.roomSimpleInfo.limitPlayerLevel == (uint)nLevel)
                {
                    iIndex = i;
                    break;
                }
            }

            if(mOnLevelSelect.value != iIndex)
            {
                mOnLevelSelect.value = iIndex;
            }

        } 

        void OnCloseBtnClicked()
        {
            if(bStartMatch)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("正在匹配,无法退出");
                return;
            }

            frameMgr.CloseFrame(this);
        }

        private DungeonID mDungeonID = new DungeonID(0);

        private void _updateTeamDungeonNodeByFilter(TeamDungeonNode node)
        {
            if (null != node && null != node.table && null != node.bind)
            {
                if (mCurrentFilterIndex == 0)
                {
                    node.bind.gameObject.SetActive(true);
                }
                else 
                {
                    if (1 == node.table.ShowIndependent)
                    {
                        node.bind.gameObject.SetActive(true);
                    }
                    else if (TeamDungeonTable.eType.DUNGEON == node.table.Type)
                    {
                        int diff = 4 - mCurrentFilterIndex;
                        mDungeonID.dungeonID = node.table.DungeonID;


                        bool isSame = diff == mDungeonID.diffID;
                        node.bind.gameObject.SetActive(isSame);

                        if (isSame)
                        {
                            if (node.id == CurTeamDungeonTableID)
                            {
                                Toggle ongroup = node.bind.GetCom<Toggle>("ongroup");
                                if (null != ongroup)
                                {
                                    ongroup.isOn = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private TeamDungeonNode _getParentTeamDungeonNodeByID(int id)
        {
            TeamDungeonNode cur = _getTeamDungeonNodeByID(id);

            if (null != cur && null != cur.table)
            {
                return _getTeamDungeonNodeByID(cur.table.MenuID);
            }

            return null;
        }

        private TeamDungeonNode _getTeamDungeonNodeByID(int id)
        {
            TeamDungeonNode node = null;

            if (null != mAllNodes)
            {
                for (int i = 0; i < mAllNodes.Count; ++i)
                {
                    if (mAllNodes[i].id == id)
                    {
                        node = mAllNodes[i];
                        break;
                    }
                }
            }

            return node;
        }


        void OnChooseFirstMenu(int teamDungeonID, bool value)
        {
            return;

            if(bStartMatch)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            Logger.LogProcessFormat("[组队] 一级，组队地下城 {0} -> {1}", teamDungeonID, value);

            if (value)
            {
                _createSecondMenuToggleList(teamDungeonID);
                _sendSwitchTeamDungeonID(teamDungeonID);
            }
        }

        private void _sendSwitchTeamDungeonID(int teamDungeonID)
        {
            _updateState();

            switch(mState)
            {
                case eTeamFrameState.onList:
                    TeamDataManager.GetInstance().RequestSearchTeam((uint)teamDungeonID);
                    break;
                case eTeamFrameState.onTeam:
                    if (TeamDataManager.GetInstance().IsTeamLeader())
                    {
                        TeamDungeonTable table = TableManager.instance.GetTableItem<TeamDungeonTable>(teamDungeonID);
                        if (null != table && TeamDungeonTable.eType.DUNGEON == table.Type)
                        {
                            TeamDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.Target, teamDungeonID);
                        }
                    }
                    break;
            }
        }

        void OnChooseSecondMenu(int teamDungeonID, bool value)
        {
            return;

            if (bStartMatch)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            //             Logger.LogProcessFormat("[组队] 二级，组队地下城 {0} -> {1}", teamDungeonID, value);
            // 
            //             if (value /*&& CurTeamDungeonTableID != teamDungeonID */)
            //             {
            //                 TeamDungeonNode node = _getTeamDungeonNodeByID(teamDungeonID);
            // 
            //                 if (null != node)
            //                 {
            //                     _sendSwitchTeamDungeonID(teamDungeonID);
            //                 }
            //             }            
        }

        private void _onTeamInfoUpdateSuccess(UIEvent ui)
        {
            //_selectNodeByID(CurTeamDungeonTableID, true);
        }


        private void _tryLoadRightFrame()
        {
            //ClientSystemManager.instance.CloseFrame<TeamListViewFrame>();
            //ClientSystemManager.instance.CloseFrame<TeamMyFrame>();

            switch(mState)
            {
                case eTeamFrameState.onList:
                    //ClientSystemManager.instance.OpenFrame(typeof(TeamListViewFrame), mRightRoot);
                    break;
                case eTeamFrameState.onTeam:
                    ClientSystemManager.instance.OpenFrame(typeof(Pk3v3CrossTeamMyFrame), mRightRoot);
                    break;
            }

        }

        private void _updateAutoSelect(int teamDungeonID)
        {
            TeamUtility.eType type = TeamUtility.GetTeamDungeonType(teamDungeonID);

            switch (type)
            {
                case TeamUtility.eType.NoTarget:
                case TeamUtility.eType.Menu:
                    _selectNodeByID(teamDungeonID, true);
                    break;
                case TeamUtility.eType.Dungeon:
                    _selectNodeByID(TeamUtility.GetMenuTeamDungeonID(teamDungeonID), true);
                    _selectNodeByID(teamDungeonID, true);
                    break;
            }
        }

        private void _clearAllCacheNode()
        {
            for (int i = 0; i < mAllNodes.Count; i++)
            {
                if (TeamDungeonTable.eType.DUNGEON == mAllNodes[i].table.Type && null != mAllNodes[i])
                {
                    mAllNodes[i].bind.ClearAllCacheBinds();
                    mAllNodes[i].bind = null;
                }
            }

            for (int i = 0; i < mAllNodes.Count; i++)
            {
                if (TeamDungeonTable.eType.DUNGEON != mAllNodes[i].table.Type && null != mAllNodes[i])
                {
                    mAllNodes[i].bind.ClearAllCacheBinds();
                    mAllNodes[i].bind = null;
                }
            }

            mBind.ClearCacheBinds(kMenuPath);
            mAllNodes.Clear();
        }

        private void _loadFirstMenuList()
        {
            _clearAllCacheNode();

            FliterSecondMenuDict.Clear();
            FliterFirstMenuList = Utility.GetTeamDungeonMenuFliterList(ref FliterSecondMenuDict);

            //_createFirstMenuToggleList();
            //_createSecondMenuToggleList(TeamUtility.GetMenuTeamDungeonID(CurTeamDungeonTableID));

            _tryLoadRightFrame();
        }

        private void _selectNodeByID(int teamDungeonID, bool isOn)
        {
            TeamDungeonNode seletednode = _getTeamDungeonNodeByID(CurTeamDungeonTableID);
            if (null != seletednode)
            {
                Toggle toggle = seletednode.bind.GetCom<Toggle>("ongroup");
                toggle.isOn = isOn;
            }
        }

        private bool _isFirstMenuOn(int menuTeamDungeonID)
        {
            return TeamUtility.GetMenuTeamDungeonID(CurTeamDungeonTableID) == menuTeamDungeonID;
        }

        private void _createFirstMenuToggleList()
        {
            for (int i = 0; i < FliterFirstMenuList.Count; i++)
            {
                int teamDungeonID = FliterFirstMenuList[i];

                TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(teamDungeonID);

                if (null != teamDungeonTable)
                {
                    ComCommonBind bind = mBind.LoadExtraBind(kMenuPath);

                    if (null != bind)
                    {
                        TeamDungeonNode node =  new TeamDungeonNode(); 

                        node.bind            = bind;
                        node.id              = teamDungeonID;
                        node.table           = teamDungeonTable;
                        mAllNodes.Add(node);

                        Utility.AttachTo(bind.gameObject, mTargetRoot);

                        Toggle ongroup = bind.GetCom<Toggle>("ongroup");
                        Text desc = bind.GetCom<Text>("desc");
                        ComSwitchNode switchNode = bind.GetCom<ComSwitchNode>("switchNode");

                        switchNode.Reset();

                        desc.text     = teamDungeonTable.Name;
                        ongroup.group = mFirstMenutogglegroup;
                        ongroup.isOn  = _isFirstMenuOn(teamDungeonID);

                        if (1 == teamDungeonTable.ShowIndependent)
                        {
                            ongroup.onValueChanged.AddListener((value) => { OnChooseSecondMenu(teamDungeonID, value); });
                        }
                        else
                        {
                            ongroup.onValueChanged.AddListener((value) => { OnChooseFirstMenu(teamDungeonID, value); });
                        }
                    }
                }
            }
        }

        private void _createSecondMenuToggleList(int teamMenuDungeonID)
        {
            TeamDungeonNode node = _getTeamDungeonNodeByID(teamMenuDungeonID);
            if (null != node)
            {
                if (!TeamUtility.IsNormalTeamDungeonID(teamMenuDungeonID))
                {
                    if (FliterSecondMenuDict.ContainsKey(node.id))
                    {
                        ComSwitchNode switchNode = node.bind.GetCom<ComSwitchNode>("switchNode");
                        GameObject content = node.bind.GetGameObject("content");
                        List<int> secondMenu = FliterSecondMenuDict[node.id];

                        switchNode.ClearSubItem();

                        for (int i = 0; i < secondMenu.Count; i++)
                        {
                            int secteamDungeonID = secondMenu[i];

                            TeamDungeonNode findNode = _getTeamDungeonNodeByID(secteamDungeonID);

                            if (null == findNode)
                            {
                                TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(secteamDungeonID);

                                if (null != teamDungeonTable)
                                {
                                    TeamDungeonNode senode =  new TeamDungeonNode(); 

                                    senode.id              = secteamDungeonID;
                                    senode.table           = teamDungeonTable;

                                    mAllNodes.Add(senode);

                                    findNode = senode;
                                }
                            }

                            if (null != findNode)
                            {
                                ComCommonBind bind = switchNode.AddOneSubItem();
                                if (null != bind)
                                {
                                    Toggle ongroup = bind.GetCom<Toggle>("ongroup");
                                    Text name = bind.GetCom<Text>("name");
                                    Text reclevel = bind.GetCom<Text>("reclevel");

                                    name.text = findNode.table.Name;
                                    reclevel.text = string.Format("推荐等级:{0}级", findNode.table.RecoLevel);

                                    ongroup.group = mSecondGroup;
                                    ongroup.isOn = CurTeamDungeonTableID == secteamDungeonID; 
                                    ongroup.onValueChanged.AddListener((svalue) => { OnChooseSecondMenu(secteamDungeonID, svalue); });

                                    findNode.bind = bind;

                                    _updateTeamDungeonNodeByFilter(findNode);
                                }
                            }
                        }
                    }
                }
            }
        }

        void OnCreateTeamSuccess(UIEvent uiEvent)
        {
            _updateState();
            _tryLoadRightFrame();

            _updateAutoSelect(CurTeamDungeonTableID);
			_sendSwitchTeamDungeonID(CurTeamDungeonTableID);
        }

        void CalScrollViewPos(Toggle MenuToggle)
        {
            GameObject Ele = MenuToggle.transform.parent.gameObject;
            GameObject Pos = Ele.transform.parent.gameObject;
            GameObject group = Pos.transform.parent.gameObject;

            float PosYy = group.GetComponent<RectTransform>().localPosition.y;

            float fMinY = mTargetRoot.GetComponent<RectTransform>().offsetMax.y;
            float fMaxY = mTargetRoot.GetComponent<RectTransform>().offsetMin.y;

            float fHeight = Math.Abs(fMaxY - fMinY);

            if (fHeight != 0)
            {
                float fPercent = 1f - (PosYy / fHeight);

                if (fPercent < 0f)
                {
                    fPercent = 0f;
                }

                if (fPercent > 1f)
                {
                    fPercent = 1f;
                }

                mDungeonScrollList.GetComponent<ScrollRect>().verticalNormalizedPosition = fPercent;
            }
        }

        private void _printVec3(string tag, Vector3 pos)
        {
            //UnityEngine.Debug.LogErrorFormat("{0}:{1},{2},{3}", tag, pos.x, pos.y, pos.z);
        }


        private void _calScrollView(RectTransform obj)
        {

            float viewHeight = mDungeonScrollList.viewport.rect.height;
            float height = mDungeonScrollList.content.sizeDelta.y;

            float full = height - viewHeight;

            //UnityEngine.Debug.LogErrorFormat("full : {0}, viewHeight {1}, height {2}", full, viewHeight, height);

            _printVec3("content", mDungeonScrollList.content.position);
            _printVec3("obj", obj.position);

            if (full > 0)
            {
                float posTop     = mDungeonScrollList.content.position.y;
                float posItemTop = obj.position.y + obj.sizeDelta.y / 2;

                float delta = (posItemTop - posTop);

                //UnityEngine.Debug.LogErrorFormat("posTop : {0}, posItemTop {1}, delta {2}", posTop, posItemTop, delta);

                if (delta <= 0)
                {
                    mDungeonScrollList.verticalNormalizedPosition = 0.0f;
                }
                else if (delta > full)
                {
                    mDungeonScrollList.verticalNormalizedPosition = 1.0f;
                }
                else
                {
                    mDungeonScrollList.verticalNormalizedPosition = delta / full;
                }
            }
        }

#region ExtraUIBind
        private ToggleGroup mSecondGroup = null;
        private Dropdown mOnDiffSelect = null;
        private ScrollRect mDungeonScrollList = null;
        private GameObject mTargetRoot = null;
        private ToggleGroup mFirstMenutogglegroup = null;
        private Button mOnClose = null;
        private GameObject mRightRoot = null;

        private Dropdown mOnLevelSelect = null;

        private string[] seasonLvs = {"青铜","白银","黄金","铂金","钻石","王者"};
        private string[] limitLvs = {"40","45","50","55","60"};

        protected override void _bindExUI()
        {
            mSecondGroup = mBind.GetCom<ToggleGroup>("secondGroup");
            mOnDiffSelect = mBind.GetCom<Dropdown>("onDiffSelect");
            mOnDiffSelect.onValueChanged.AddListener(_onOnDiffSelectDropdownValueChange);
            mDungeonScrollList = mBind.GetCom<ScrollRect>("dungeonScrollList");
            mTargetRoot = mBind.GetGameObject("TargetRoot");
            mFirstMenutogglegroup = mBind.GetCom<ToggleGroup>("firstMenutogglegroup");
            mOnClose = mBind.GetCom<Button>("onClose");
            mOnClose.onClick.AddListener(_onOnCloseButtonClick);
            mRightRoot = mBind.GetGameObject("RightRoot");

            mOnLevelSelect = mBind.GetCom<Dropdown>("onLevelSelect");
            mOnLevelSelect.onValueChanged.AddListener(_onOnLevelSelectDropdownValueChange);

            mOnDiffSelect.options.Clear();
            for(int i = 0;i < seasonLvs.Length;i++)
            {
                mOnDiffSelect.options.Add(new Dropdown.OptionData(seasonLvs[i]));
            }

            mOnLevelSelect.options.Clear();
            for (int i = 0; i < limitLvs.Length; i++)
            {
                mOnLevelSelect.options.Add(new Dropdown.OptionData(limitLvs[i]));
            }
        }

        protected override void _unbindExUI()
        {
            mSecondGroup = null;
            mOnDiffSelect.onValueChanged.RemoveListener(_onOnDiffSelectDropdownValueChange);
            mOnDiffSelect = null;
            mDungeonScrollList = null;
            mTargetRoot = null;
            mFirstMenutogglegroup = null;
            mOnClose.onClick.RemoveListener(_onOnCloseButtonClick);
            mOnClose = null;
            mRightRoot = null;

            mOnLevelSelect.onValueChanged.RemoveListener(_onOnLevelSelectDropdownValueChange);
            mOnLevelSelect = null;
        }
#endregion   

#region Callback
        private void _onOnDiffSelectDropdownValueChange(int index)
        {
            /* put your code in here */
            //             mCurrentFilterIndex = index;
            // 
            //             Logger.LogProcessFormat("[组队] 筛选更新 {0}", mCurrentFilterIndex);
            // 
            //             for (int i = 0; i < mAllNodes.Count; ++i)
            //             {
            //                 _updateTeamDungeonNodeByFilter(mAllNodes[i]);
            //             }
            // 
            //             LayoutRebuilder.ForceRebuildLayoutImmediate(mDungeonScrollList.content);
            //             mDungeonScrollList.verticalNormalizedPosition = 1.0f;

            if(!Pk3v3CrossDataManager.GetInstance().IsTeamLeader())
            {
                return;
            }

            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
            if (roomInfo == null)
            {
                Logger.LogError("roomInfo is null when save room setting data.");
                return;
            }

            if(index >= seasonLvs.Length)
            {
                return;
            }

            WorldUpdateRoomReq req = new WorldUpdateRoomReq();

            req.roomId = roomInfo.roomSimpleInfo.id;
            req.roomType = roomInfo.roomSimpleInfo.roomType;
            req.name = roomInfo.roomSimpleInfo.name;
            req.password = Pk3v3CrossDataManager.GetInstance().PassWord;
            req.isLimitPlayerLevel = roomInfo.roomSimpleInfo.isLimitPlayerLevel;
            req.isLimitPlayerSeasonLevel = (byte)(index > 0 ? 1 : 0);
            req.limitPlayerLevel = roomInfo.roomSimpleInfo.limitPlayerLevel;
            req.limitPlayerSeasonLevel = (uint)Pk3v3CrossDataManager.GetInstance().GetRankLvByIndex(index);

            req.isLimitPlayerLevel = 1;
            req.isLimitPlayerSeasonLevel = 1;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        private void _onOnLevelSelectDropdownValueChange(int index)
        {
            if (!Pk3v3CrossDataManager.GetInstance().IsTeamLeader())
            {
                return;
            }

            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
            if (roomInfo == null)
            {
                Logger.LogError("roomInfo is null when save room setting data.");
                return;
            }
            
            if(index >= limitLvs.Length)
            {
                return;
            }

            WorldUpdateRoomReq req = new WorldUpdateRoomReq();

            req.roomId = roomInfo.roomSimpleInfo.id;
            req.roomType = roomInfo.roomSimpleInfo.roomType;
            req.name = roomInfo.roomSimpleInfo.name;
            req.password = Pk3v3CrossDataManager.GetInstance().PassWord;
            req.isLimitPlayerLevel = (byte)(index > 0 ? 1 : 0);
            req.isLimitPlayerSeasonLevel = roomInfo.roomSimpleInfo.isLimitPlayerSeasonLevel;
            int nLevel = 0;
            int.TryParse(limitLvs[index], out nLevel);
            req.limitPlayerLevel = (ushort)nLevel;
            req.limitPlayerSeasonLevel = roomInfo.roomSimpleInfo.limitPlayerSeasonLevel;

            req.isLimitPlayerLevel = 1;
            req.isLimitPlayerSeasonLevel = 1;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        private void _onOnCloseButtonClick()
        {
            /* put your code in here */
            OnCloseBtnClicked();
        }
#endregion
    }
}
