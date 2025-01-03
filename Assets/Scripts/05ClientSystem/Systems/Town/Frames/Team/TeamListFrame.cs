using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System;
using Protocol;
using Network;

namespace GameClient
{
    public enum eTeamFrameState 
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

    class TeamListFrame : ClientFrame
    {
        private eTeamFrameState mState = eTeamFrameState.onNone;

        private const string kMenuPath = "UIFlatten/Prefabs/Team/TeamListMenuGroup";
        private int mCurrentFilterIndex = 0;
        private bool bStartMatch = false;

        private List<int> FliterFirstMenuList = new List<int>();
        private Dictionary<int, List<int>> FliterSecondMenuDict = new Dictionary<int, List<int>>();

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
            return "UIFlatten/Prefabs/Team/TeamList";
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
            TryOpenTeamListOrTeamMyFrame();
            //ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>(FrameLayer.Middle);
        }

        public static void TryOpenTeamListOrTeamMyFrame(int iTeamDungeonTableID = 0)
        {
            if(TeamDataManager.GetInstance().HasTeam())
            {
                ClientSystemManager.GetInstance().OpenFrame<TeamMyFrame>();
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>(FrameLayer.Middle, iTeamDungeonTableID);
            }
        }

        public List<GameObject> locks = null;
        protected override void _OnOpenFrame()
        {
            TeamDataManager.GetInstance().NotPopUpTeamList = false;

//             // 队员打开这个界面时候先关闭列表界面，然后直接打开队伍界面
//             // add by qxy 2019-02-11
//             if (TeamDataManager.GetInstance().HasTeam() && !TeamDataManager.GetInstance().IsTeamLeader())
//             {
//                 frameMgr.CloseFrame(this);
//                 ClientSystemManager.GetInstance().OpenFrame<TeamMyFrame>();
//                 return;
//             }
            locks = new List<GameObject>();
            locks.Add(Utility.FindChild(togType2.gameObject, "lock"));
            locks.Add(Utility.FindChild(togType3.gameObject, "lock"));
            locks.Add(Utility.FindChild(togType4.gameObject, "lock"));
            locks.Add(Utility.FindChild(togType5.gameObject, "lock"));
            _updateState();         
            int autoID = 1;            

            if (userData != null)
            {
                FliterSecondMenuDict.Clear();
                FliterFirstMenuList = Utility.GetTeamDungeonMenuFliterList(ref FliterSecondMenuDict);
                GuildDataManager.FliterTeamDungeonID(ref FliterFirstMenuList, ref FliterSecondMenuDict);

                autoID = (int)userData;
                string name = "";
                TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)autoID);
                if (teamDungeonTable != null)
                {
                    DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
                    if (table != null)
                    {
                        name = table.Name;
                    }
                }
                foreach (var data in FliterSecondMenuDict)
                {
                    List<int> ids = data.Value;
                    if(ids != null && ids.Contains(autoID))
                    {
                        for(int i = 0;i < ids.Count;i++)
                        {
                            TeamDungeonTable teamDungeonTable1 = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(ids[i]);
                            if (teamDungeonTable1 != null)
                            {
                                DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable1.DungeonID);
                                if (table != null)
                                {
                                    if(table.Name == name)
                                    {
                                        autoID = ids[i];
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }                
                if(mState == eTeamFrameState.onList)
                {
                    TeamDataManager.GetInstance().TeamDungeonID = (uint)(int)autoID;
                }
                else if(mState == eTeamFrameState.onTeam)
                {
                    TeamDataManager.GetInstance().TeamDungeonID = (uint)(int)userData;
                }
            }
            else 
            {
                autoID = CurTeamDungeonTableID;

                // 默认界面开启的时候是请求所有的队伍
                // 公会场景和公会地图场景不应该请求所有的队伍，应该要请求公会副本队伍
                // add by qxy 2018-12-11
                if (GuildDataManager.IsInGuildAreanScence() || GuildDataManager.IsGuildDungeonMapScence())
                {
                    autoID = GetGuildTeamDungeonID();
                }

                FliterSecondMenuDict.Clear();
                FliterFirstMenuList = Utility.GetTeamDungeonMenuFliterList(ref FliterSecondMenuDict);
                GuildDataManager.FliterTeamDungeonID(ref FliterFirstMenuList, ref FliterSecondMenuDict);

                string name = "";
                TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)autoID);
                if (teamDungeonTable != null)
                {
                    DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
                    if (table != null)
                    {
                        name = table.Name;
                    }
                }
                foreach (var data in FliterSecondMenuDict)
                {
                    List<int> ids = data.Value;
                    if (ids != null && ids.Contains(autoID))
                    {
                        for (int i = 0; i < ids.Count; i++)
                        {
                            TeamDungeonTable teamDungeonTable1 = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(ids[i]);
                            if (teamDungeonTable1 != null)
                            {
                                DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable1.DungeonID);
                                if (table != null)
                                {
                                    if (table.Name == name)
                                    {
                                        autoID = ids[i];
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
                if(mState == eTeamFrameState.onList)
                {
                    TeamDataManager.GetInstance().TeamDungeonID = (uint)autoID;
                }
            }

            Logger.LogProcessFormat("[组队] 打开组队界面 teamDungeonID {0}", autoID);

            //打开组队界面的时候，停止自动寻路
            GameClient.ClientSystemTown clientSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
            if (clientSystem != null && clientSystem.MainPlayer != null)
            {
                clientSystem.MainPlayer.CommandStopMove();
            }

            _updateState();
            _loadFirstMenuList();
            BindUIEvent();

            OnChooseFirstMenu(TeamUtility.GetMenuTeamDungeonID(autoID), true);
            OnChooseSecondMenu(autoID, true);
            if (mState == eTeamFrameState.onList)
            {
                _updateAutoSelect(CurTeamDungeonTableID);
            }
            if(userData != null)
            {
                int teamDungeonID = (int)userData;
                TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)teamDungeonID);
                if (teamDungeonTable != null)
                {
                    DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
                    if (table != null)
                    {
                        int iHard = (int)table.Hard;

                        // 堕落深渊和周常深渊的配表难度是普通，但是要显示成王者
                        if(TeamDataManager.GetInstance().IsDuoLuoTeamDungeonID(teamDungeonID) || DungeonUtility.IsWeekHellTeamDungeon(teamDungeonID))
                        {
                            iHard = (int)DungeonTable.eHard.KING;
                        }

                        List<Toggle> toggles = new List<Toggle>();
                        toggles.Add(togType2);
                        toggles.Add(togType3);
                        toggles.Add(togType4);
                        toggles.Add(togType5);
                        if(iHard < toggles.Count)
                        {
                            toggles[iHard].isOn = true;
                        }
                        UpdateToggleTextColor(togType1);
                        UpdateToggleTextColor(togType2);
                        UpdateToggleTextColor(togType3);
                        UpdateToggleTextColor(togType4);
                        UpdateToggleTextColor(togType5);
                    }
                }
            }
// 
//             if (mState == eTeamFrameState.onTeam)
//             {
//                 ClientSystemManager.GetInstance().CloseFrame<TeamListFrame>();
//             }
        }

        int GetGuildTeamDungeonID()
        {
            FliterSecondMenuDict.Clear();
            FliterFirstMenuList = Utility.GetTeamDungeonMenuFliterList(ref FliterSecondMenuDict);
            GuildDataManager.FliterTeamDungeonID(ref FliterFirstMenuList, ref FliterSecondMenuDict);

            if (FliterFirstMenuList.Count > 0)
            {
                return FliterFirstMenuList[0];
            }

            return 0;
        }

        private void _updateState()
        {
            mState = TeamDataManager.GetInstance().HasTeam() ? eTeamFrameState.onTeam : eTeamFrameState.onList;
        }

        protected override void _OnCloseFrame()
        {
            Clear();        

            ClientSystemManager.instance.CloseFrame<TeamListViewFrame>();
            //ClientSystemManager.instance.CloseFrame<TeamMyFrame>();
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
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamCreateSuccess, OnCreateTeamSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, OnCreateTeamSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamJoinSuccess, OnCreateTeamSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamInfoUpdateSuccess, _onTeamInfoUpdateSuccess);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamCreateSuccess, OnCreateTeamSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, OnCreateTeamSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamJoinSuccess, OnCreateTeamSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamInfoUpdateSuccess, _onTeamInfoUpdateSuccess);
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
            if(bStartMatch)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            Logger.LogProcessFormat("[组队] 一级，组队地下城 {0} -> {1}", teamDungeonID, value);

            if (value)
            {
                TeamDataManager.GetInstance().DiffHard = -1;
                _createSecondMenuToggleList(teamDungeonID);
                _sendSwitchTeamDungeonID(teamDungeonID,null);
                DungeonTable.eHard eHard = DungeonTable.eHard.NORMAL;
                UpdateAllTypeLockState(teamDungeonID,ref eHard);
                togType1.CustomActive(true);
                togType1.isOn = true;
                togType2.isOn = false;
                togType3.isOn = false;
                togType4.isOn = false;
                togType5.isOn = false;


                UpdateToggleTextColor(togType1);
                UpdateToggleTextColor(togType2);
                UpdateToggleTextColor(togType3);
                UpdateToggleTextColor(togType4);
                UpdateToggleTextColor(togType5);
                if (teamDungeonID == GetGuildTeamDungeonID())
                {
                    togType1.CustomActive(true);
                    togType2.CustomActive(true);
                    togType3.CustomActive(false);
                    togType4.CustomActive(false);
                    togType5.CustomActive(false);               
                }
                else
                {
                    togType1.CustomActive(true);
                    togType2.CustomActive(true);
                    togType3.CustomActive(true);
                    togType4.CustomActive(true);
                    togType5.CustomActive(true);
                }
            }
        }

        private void _sendSwitchTeamDungeonID(int teamDungeonID,List<int> IDs = null)
        {
            _updateState();

            
            

            switch(mState)
            {
                case eTeamFrameState.onList:
                    TeamDataManager.GetInstance().RequestSearchTeam((uint)teamDungeonID,IDs);
                    break;
                case eTeamFrameState.onTeam:
                    if (TeamDataManager.GetInstance().IsTeamLeader())
                    {
                        TeamDungeonTable table = TableManager.instance.GetTableItem<TeamDungeonTable>(teamDungeonID);
                        if (null != table && (TeamDungeonTable.eType.DUNGEON == table.Type
                                              || TeamDungeonTable.eType.CityMonster == table.Type))
                        {
                            //TeamDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.Target, teamDungeonID);
                        }
                    }
                    break;
            }
        }

        void UpdateToggleTextColor(Toggle toggle)
        {
            if(toggle != null)
            {
                ComChangeColor changeColor = toggle.GetComponent<ComChangeColor>();
                if(changeColor != null)
                {
                    changeColor.SetColor(toggle.isOn);
                }
            }
        }

        void OnChooseSecondMenu(int teamDungeonID, bool value, List<int> dungeonIDs = null)
        {
            if (bStartMatch)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            // 二级tab的类型不可能为 MENU
            if (value && teamDungeonID != 1)
            {
                TeamDungeonNode node = _getTeamDungeonNodeByID(teamDungeonID);
                if (node != null && node.table.Type == TeamDungeonTable.eType.MENU)
                {
                    return;
                }
            }
            togType1.CustomActive(true);
            togType2.CustomActive(true);
            togType3.CustomActive(true);
            togType4.CustomActive(true);
            togType5.CustomActive(true);

            Logger.LogProcessFormat("[组队] 二级，组队地下城 {0} -> {1}", teamDungeonID, value);

            if (value /*&& CurTeamDungeonTableID != teamDungeonID */)
            {
                togType1.isOn = true;
                togType2.isOn = false;
                togType3.isOn = false;
                togType4.isOn = false;
                togType5.isOn = false;
                TeamDungeonNode node = _getTeamDungeonNodeByID(teamDungeonID);

                if(node == null)
                {
                    return;
                }

                if(node.table == null)
                {
                    return;
                }
                _sendSwitchTeamDungeonID(teamDungeonID, dungeonIDs);
                DungeonTable.eHard eHard = DungeonTable.eHard.NORMAL;
                UpdateAllTypeLockState(teamDungeonID,ref eHard);

                List<Toggle> toggles = new List<Toggle>();
                if(toggles == null)
                {
                    return;
                }
                toggles.Add(togType2);
                toggles.Add(togType3);
                toggles.Add(togType4);
                toggles.Add(togType5);

                if (teamDungeonID != 1)
                {
                    int iIndex = (int)eHard;
                    for(int i = 0;i < toggles.Count;i++)
                    {
                        if(i == iIndex)
                        {
                            toggles[i].isOn = true;
                        }
                        else
                    {
                            toggles[i].isOn = false;
                    }
                    }              
                }
                UpdateToggleTextColor(togType1);
                UpdateToggleTextColor(togType2);
                UpdateToggleTextColor(togType3);
                UpdateToggleTextColor(togType4);
                UpdateToggleTextColor(togType5);
                if (teamDungeonID == 1)
                {
                    togType1.CustomActive(true);

                    togType1.isOn = true;
                    togType2.isOn = false;
                    togType3.isOn = false;
                    togType4.isOn = false;
                    togType5.isOn = false;
                    UpdateToggleTextColor(togType1);
                    UpdateToggleTextColor(togType2);
                    UpdateToggleTextColor(togType3);
                    UpdateToggleTextColor(togType4);
                    UpdateToggleTextColor(togType5);

                    TeamDataManager.GetInstance().DiffHard = -1;
                }
                else
                {
                    togType1.CustomActive(false);
                }

                for (int i = 0; i < toggles.Count; i++)
                {
                    Toggle tog = toggles[i];
                    if (tog != null)
                    {
                        tog.CustomActive(true);
                    }
                }
                bool bIsGuildTeamDungoen = false;

                // 公会副本只有普通难度 这里做下处理
                // 隐藏除普通难度外的其他难度按钮
                    bIsGuildTeamDungoen = (node.table.MenuID == GetGuildTeamDungeonID());                  

                // 精英地下城也只有普通难度
                bool isEliteDungeon = TeamUtility.IsEliteDungeonID(node.table.DungeonID);
                if (bIsGuildTeamDungoen || isEliteDungeon)
                {
                    for (int i = 0; i < toggles.Count; i++)
                    {
                        Toggle tog = toggles[i];
                        if (tog == null)
                        {
                            continue;
                        }
                        if (i == 0)
                        {
                            tog.CustomActive(true);
                        }
                        else
                        {
                            tog.CustomActive(false);
                        }
                    }
                    if (toggles.Count > 0)
                    {
                        toggles[0].SafeSetToggleOnState(true);
                    }
                }
                else if(TeamDataManager.GetInstance().IsDuoLuoTeamDungeonID(teamDungeonID)
                        || DungeonUtility.IsWeekHellTeamDungeon(teamDungeonID) == true) // 堕落深渊
                {
                    // 堕落深渊只有最高难度
                    for (int i = 0; i < toggles.Count; i++)
                    {
                        if (i == toggles.Count - 1)
                        {
                            toggles[i].CustomActive(true);
                        }
                        else
                        {
                            if (TeamDataManager.GetInstance().IsDuoLuoTeamDungeonID(teamDungeonID)
                                || DungeonUtility.IsWeekHellTeamDungeon(teamDungeonID) == true)        //周常深渊，只有最高难度
                            {
                                toggles[i].CustomActive(false);
                            }
                            else
                            {
                                toggles[i].CustomActive(true);
                            }
                        }
                    }
                }                                
            }
        }

        private void _onTeamInfoUpdateSuccess(UIEvent ui)
        {
            //_selectNodeByID(CurTeamDungeonTableID, true);
        }


        private void _tryLoadRightFrame()
        {
            ClientSystemManager.instance.CloseFrame<TeamListViewFrame>();
            //ClientSystemManager.instance.CloseFrame<TeamMyFrame>();

            switch(mState)
            {
                case eTeamFrameState.onList:
                    ClientSystemManager.instance.OpenFrame(typeof(TeamListViewFrame), mRightRoot);
                    break;
                case eTeamFrameState.onTeam:
                    //ClientSystemManager.instance.OpenFrame(typeof(TeamMyFrame), mRightRoot);
                    //ClientSystemManager.GetInstance().OpenFrame<TeamMyFrame>();
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
                case TeamUtility.eType.AttackCityMonster:
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
            GuildDataManager.FliterTeamDungeonID(ref FliterFirstMenuList, ref FliterSecondMenuDict);

            _createFirstMenuToggleList();
            _createSecondMenuToggleList(TeamUtility.GetMenuTeamDungeonID(CurTeamDungeonTableID));

            _tryLoadRightFrame();
        }

        void UpdateAllTypeLockState(int teamDungeonID,ref DungeonTable.eHard eMaxHard)
        {
            for(int i = 0;i < locks.Count;i++)
            {
                GameObject go = locks[i];
                if(go != null)
                {
                    go.CustomActive(false);
                }
            }
            if(teamDungeonID == 1)
            {
                return;
            }
            if(FliterFirstMenuList.Contains(teamDungeonID))
            {
                return;
            }
            DungeonTable.eHard maxHard = (int)DungeonTable.eHard.NORMAL;
            do
            {
                TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)teamDungeonID);
                if (teamDungeonTable == null)
                {
                    continue;
                }
                DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
                if (table == null)
                {
                    if (teamDungeonTable.Type == TeamDungeonTable.eType.CityMonster)
                    {
                        maxHard = DungeonTable.eHard.NORMAL;          
                    }
                    continue;
                }
                DiffInfo diffInfo = new DiffInfo();
                secteamDungeons.TryGetValue(table.Name, out diffInfo);
                if (diffInfo == null)
                {
                    continue;
                }
                for (int i = 0; i < diffInfo.secteamDungeonID.Count; i++)
                {
                    TeamDungeonTable teamDungeonTable1 = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(diffInfo.secteamDungeonID[i]);
                    if (teamDungeonTable1 == null)
                    {
                        continue;
                    }
                    DungeonTable table1 = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable1.DungeonID);
                    if (table1 == null)
                    {
                        continue;
                    }
                    if (table1.Hard > maxHard)
                    {
                        maxHard = table1.Hard;
                    }
                }               
            }
            while (false);

            // 堕落深渊只有最高难度
            //周常深渊的入口，只有最高难度
            if(TeamDataManager.GetInstance().IsDuoLuoTeamDungeonID(teamDungeonID)
               || DungeonUtility.IsWeekHellTeamDungeon(teamDungeonID) == true)
            {
                maxHard = DungeonTable.eHard.KING;
            }

            for (int j = (int)maxHard + 1; j < locks.Count; j++)
            {
                GameObject go = locks[j];
                if (go != null)
                {
                    go.CustomActive(true);
                }
            }
            eMaxHard = maxHard;
            return;
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

                        if(teamDungeonID == 1)
                        {
                            desc.text = "全部挑战";
                        }
                        if (1 == teamDungeonTable.ShowIndependent)
                        {
                            ongroup.onValueChanged.AddListener((value) => { OnChooseSecondMenu(teamDungeonID, value); });
                        }
                        else
                        {
                            ongroup.onValueChanged.AddListener((value) => { OnChooseFirstMenu(teamDungeonID, value); });
                        }

                        if(desc != null)
                        {
                            StaticUtility.SafeSetText(bind, "desc2", desc.text);
                        }                        
                    }
                }
            }
        }

        public Dictionary<string, DiffInfo> secteamDungeons = TeamDataManager.GetInstance().GetDiffInfo();
        public Dictionary<int, List<int>> secondMenuID2TargetIDs = new Dictionary<int, List<int>>();
        public Dictionary<int, int> targetID2secondMenuID = new Dictionary<int, int>();
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

                        Dictionary<int, List<int>> OriSecDict = TableManager.GetInstance().GetTeamDungeonSecondMenuDict();
                        Dictionary<string, int> maxLevels = new Dictionary<string, int>();
                        foreach(var data in OriSecDict)
                        {
                            List<int> ids = data.Value;                        
                            for(int i = 0;i < ids.Count;i++)
                            {
                                TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(ids[i]);
                                if (teamDungeonTable == null)
                                {
                                    continue;
                                }
                                DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
                                if (table == null)
                                {
                                    continue;
                                }

                                if (GuildDataManager.GetInstance().IsGuildDungeonMap(teamDungeonTable.DungeonID))
                                {
                                    if (GuildDataManager.GetInstance().HasSelfGuild())
                                    {
                                        // 根据设置的难度进行过滤
                                        if(GuildDataManager.GetInstance().GetGuildDungeonDiffType() != GuildDataManager.GetDungeonTypeByDungeonID(teamDungeonTable.DungeonID))
                                        {
                                            continue;
                                        }
                                    }
                                }

                                int iMaxLv = 0;
                                bool bRet = maxLevels.TryGetValue(table.Name, out iMaxLv);
                                if(!bRet)
                                {
                                    maxLevels.Add(table.Name, teamDungeonTable.RecoLevel);
                                }
                                else
                                {
                                    if(teamDungeonTable.RecoLevel > iMaxLv)
                                    {
                                        maxLevels[table.Name] = teamDungeonTable.RecoLevel;
                                    }
                                }
                            }
                        }
                        List<int> secondMenuTemp = new List<int>();
                        for(int i = 0;i < secondMenu.Count;i++)
                        {
                            secondMenuTemp.Add(secondMenu[i]);
                        }
                        secteamDungeons.Clear();
                        if (true)
                        {
                            for (int i = 0; i < secondMenuTemp.Count; i++)
                            {
                                TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(secondMenuTemp[i]);
                                if (teamDungeonTable == null)
                                {
                                    continue;
                                }
                                DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
                                if (table == null)
                                {
                                    continue;
                                }
                                DiffInfo diffInfo = new DiffInfo();
                                secteamDungeons.TryGetValue(table.Name, out diffInfo);
                                if (diffInfo == null)
                                {
                                    diffInfo = new DiffInfo();
                                    diffInfo.secteamDungeonID.Add(secondMenuTemp[i]);
                                    diffInfo.dungeonName = table.Name;
                                    diffInfo.iMinLv = teamDungeonTable.RecoLevel;
                                    diffInfo.iMaxLv = teamDungeonTable.RecoLevel;
                                    secteamDungeons.Add(table.Name, diffInfo);
                                    diffInfo.iMaxLv = maxLevels[table.Name];
                                }
                                else
                                {
                                    diffInfo.secteamDungeonID.Add(secondMenuTemp[i]);
                                    if (diffInfo.iMaxLv < teamDungeonTable.RecoLevel)
                                    {
                                    }
                                    secondMenuTemp.RemoveAt(i);
                                    i--;
                                    continue;
                                }
                            }
                        }
                        secondMenuID2TargetIDs.Clear();
                        targetID2secondMenuID.Clear();
                        foreach(var data in secteamDungeons)
                        {
                            DiffInfo info = data.Value;
                            if(info.secteamDungeonID.Count > 0)
                            {
                                int secondMenuID = info.secteamDungeonID[0];
                                List<int> ids = new List<int>();
                                ids = info.secteamDungeonID;
                                secondMenuID2TargetIDs.Add(secondMenuID, ids);
                                for(int i = 0;i < info.secteamDungeonID.Count;i++)
                                {
                                    targetID2secondMenuID.Add(info.secteamDungeonID[i], secondMenuID);
                                }
                            }
                        }
                        switchNode.ClearSubItem();

                        for (int i = 0; i < secondMenuTemp.Count; i++)
                        {
                            int secteamDungeonID = secondMenuTemp[i];

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
                                    //ongroup.isOn = CurTeamDungeonTableID == secteamDungeonID; 
                                    ongroup.onValueChanged.AddListener((svalue) => { OnChooseSecondMenu(secteamDungeonID, svalue); });
                                    DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(findNode.table.DungeonID);
                                    if (table != null)
                                    {
                                        DiffInfo diffInfo = new DiffInfo();
                                        secteamDungeons.TryGetValue(table.Name, out diffInfo);
                                        if (diffInfo != null)
                                        {
                                            name.text = diffInfo.dungeonName;
                                            reclevel.text = string.Format("推荐等级:{0}-{1}级", diffInfo.iMinLv,diffInfo.iMaxLv);
                                            if(diffInfo.iMaxLv == diffInfo.iMinLv)
                                            {
                                                reclevel.text = string.Format("推荐等级:{0}级", diffInfo.iMinLv);
                                            }
                                            ongroup.onValueChanged.RemoveAllListeners();
                                            ongroup.onValueChanged.AddListener((svalue) => { OnChooseSecondMenu(secteamDungeonID, svalue,diffInfo.secteamDungeonID); });
                                        }
                                    }

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

            frameMgr.CloseFrame(this);
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
            UnityEngine.Debug.LogErrorFormat("{0}:{1},{2},{3}", tag, pos.x, pos.y, pos.z);
        }


        private void _calScrollView(RectTransform obj)
        {

            float viewHeight = mDungeonScrollList.viewport.rect.height;
            float height = mDungeonScrollList.content.sizeDelta.y;

            float full = height - viewHeight;

            UnityEngine.Debug.LogErrorFormat("full : {0}, viewHeight {1}, height {2}", full, viewHeight, height);

            _printVec3("content", mDungeonScrollList.content.position);
            _printVec3("obj", obj.position);

            if (full > 0)
            {
                float posTop     = mDungeonScrollList.content.position.y;
                float posItemTop = obj.position.y + obj.sizeDelta.y / 2;

                float delta = (posItemTop - posTop);

                UnityEngine.Debug.LogErrorFormat("posTop : {0}, posItemTop {1}, delta {2}", posTop, posItemTop, delta);

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
        private ScrollRect mDungeonScrollList = null;
        private GameObject mTargetRoot = null;
        private ToggleGroup mFirstMenutogglegroup = null;
        private Button mOnClose = null;
        private GameObject mRightRoot = null;

        private Toggle togType1 = null;
        private Toggle togType2 = null;
        private Toggle togType3 = null;
        private Toggle togType4 = null;
        private Toggle togType5 = null;
        protected override void _bindExUI()
        {
            mSecondGroup = mBind.GetCom<ToggleGroup>("secondGroup");       
            mDungeonScrollList = mBind.GetCom<ScrollRect>("dungeonScrollList");
            mTargetRoot = mBind.GetGameObject("TargetRoot");
            mFirstMenutogglegroup = mBind.GetCom<ToggleGroup>("firstMenutogglegroup");
            mOnClose = mBind.GetCom<Button>("onClose");
            mOnClose.onClick.AddListener(_onOnCloseButtonClick);
            mRightRoot = mBind.GetGameObject("RightRoot");
            togType1 = mBind.GetCom<Toggle>("Type1");
            togType2 = mBind.GetCom<Toggle>("Type2");
            togType3 = mBind.GetCom<Toggle>("Type3");
            togType4 = mBind.GetCom<Toggle>("Type4");
            togType5 = mBind.GetCom<Toggle>("Type5");
            togType1.onValueChanged.RemoveAllListeners();
            togType1.onValueChanged.AddListener(var => 
            {
                DiffTypeChange(var, -1);
            });
            togType2.onValueChanged.RemoveAllListeners();
            togType2.onValueChanged.AddListener(var =>
            {
                DiffTypeChange(var, (int)DungeonTable.eHard.NORMAL);
            });
            togType3.onValueChanged.RemoveAllListeners();
            togType3.onValueChanged.AddListener(var =>
            {
                DiffTypeChange(var, (int)DungeonTable.eHard.RISK);
            });
            togType4.onValueChanged.RemoveAllListeners();
            togType4.onValueChanged.AddListener(var =>
            {
                DiffTypeChange(var, (int)DungeonTable.eHard.WARRIOR);
            });
            togType5.onValueChanged.RemoveAllListeners();
            togType5.onValueChanged.AddListener(var =>
            {
                DiffTypeChange(var, (int)DungeonTable.eHard.KING);
            });
        }
        private void DiffTypeChange(bool val,int hard)
        {
            if(val)
            {
                TeamDataManager.GetInstance().DiffHard = hard;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamListUpdateByHard);
            }
            return;
        }

        protected override void _unbindExUI()
        {
            mSecondGroup = null;    
            mDungeonScrollList = null;
            mTargetRoot = null;
            mFirstMenutogglegroup = null;
            mOnClose.onClick.RemoveListener(_onOnCloseButtonClick);
            mOnClose = null;
            mRightRoot = null;
            mOnClose = mBind.GetCom<Button>("onClose");
            mOnClose.onClick.AddListener(_onOnCloseButtonClick);
            mOnClose = null;
            togType1 = null;
            togType2 = null;
            togType3 = null;
            togType4 = null;
            togType5 = null;
        }
#endregion   

#region Callback 

        private void _onOnCloseButtonClick()
        {
            /* put your code in here */
            OnCloseBtnClicked();
        }
#endregion
    }
}
