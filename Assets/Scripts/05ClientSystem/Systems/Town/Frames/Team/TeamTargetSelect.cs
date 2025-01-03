using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System;
using Protocol;
using Network;

namespace GameClient
{
    

    class TeamTargetSelect : ClientFrame
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

        private eTeamFrameState mState = eTeamFrameState.onNone;

        private const string kMenuPath = "UIFlatten/Prefabs/Team/TeamMenuGroup";
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
            return "UIFlatten/Prefabs/Team/TeamTargetSelect";
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
            //ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>(FrameLayer.Middle);
        }

        public List<GameObject> locks = null;
        protected override void _OnOpenFrame()
        {
            locks = new List<GameObject>();
            locks.Add(Utility.FindChild(togType2.gameObject, "lock"));
            locks.Add(Utility.FindChild(togType3.gameObject, "lock"));
            locks.Add(Utility.FindChild(togType4.gameObject, "lock"));
            locks.Add(Utility.FindChild(togType5.gameObject, "lock"));

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

            iCurDungeonID = TeamDataManager.GetInstance().TeamDungeonID;


            List<Toggle> toggles = new List<Toggle>();
            toggles.Add(togType2);
            toggles.Add(togType3);
            toggles.Add(togType4);
            toggles.Add(togType5);

            int teamDungeonID = CurTeamDungeonTableID;

            TeamDungeonTable table1 = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(teamDungeonID);
            if (table1 != null)
            {
                if (teamDungeonID == 1 || table1.Type == TeamDungeonTable.eType.CityMonster)
                {
                    UIGray gray = btnOK.gameObject.SafeAddComponent<UIGray>(false);
                    if (null != gray)
                    {
                        gray.enabled = false;
                    }
                    btnOK.interactable = true;

                    iTargetID = (uint)teamDungeonID;

                    _updateAutoSelect(teamDungeonID);
                }
                else
                {
                    UIGray gray = btnOK.gameObject.SafeAddComponent<UIGray>(false);
                    if (null != gray)
                    {
                        gray.enabled = true;
                    }
                    btnOK.interactable = false;

                    iTargetID = (uint)teamDungeonID;

                    togType2.isOn = false;
                    togType3.isOn = false;
                    togType4.isOn = false;
                    togType5.isOn = false;

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
                            continue;
                        }

                        DiffInfo diffInfo = new DiffInfo();
                        secteamDungeons.TryGetValue(table.Name, out diffInfo);
                        if (diffInfo == null)
                        {
                            continue;
                        }

                        if(diffInfo.secteamDungeonID.Contains(teamDungeonID))
                        {
                            _updateAutoSelect(diffInfo.secteamDungeonID[0]);
                        }

                        int iHard = (int)table.Hard;
                        if(iHard < toggles.Count)
                        {
                            toggles[iHard].isOn = true;
                        }
                    }
                    while (false);
                }
            }

            if(CurTeamDungeonTableID == 1)
            {
                none.CustomActive(true);
                txtTips.CustomActive(false);
                btns.CustomActive(false);
            }
            //ClientSystemManager.GetInstance().CloseFrame<TeamMyFrame>();
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
//             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamCreateSuccess, OnCreateTeamSuccess);
//             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, OnCreateTeamSuccess);
//             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamJoinSuccess, OnCreateTeamSuccess);
//             UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamInfoUpdateSuccess, _onTeamInfoUpdateSuccess);
        }

        void UnBindUIEvent()
        {
//             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamCreateSuccess, OnCreateTeamSuccess);
//             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamRemoveMemberSuccess, OnCreateTeamSuccess);
//             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamJoinSuccess, OnCreateTeamSuccess);
//             UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamInfoUpdateSuccess, _onTeamInfoUpdateSuccess);
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
                //TeamDataManager.GetInstance().DiffHard = -1;
                //togType1.isOn = true;

                if(btns != null)
                {
                    btns.CustomActive(false);
                }

                if(txtTips != null)
                {
                    txtTips.CustomActive(true);
                }

                _createSecondMenuToggleList(teamDungeonID);
                _sendSwitchTeamDungeonID(teamDungeonID);

                iCurDungeonID = (uint)teamDungeonID;

                TeamDungeonTable table1 = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(teamDungeonID);
                if (table1 != null)
                {
                    bool bCityMonster = false;
                    int iCityMonsterDungeonID = 0;
                    List<int> secondMenu = new List<int>();
                    bool bRet = FliterSecondMenuDict.TryGetValue(teamDungeonID, out secondMenu);
                    if(bRet)
                    {
                    for (int i = 0; i < secondMenu.Count; i++)
                    {
                        TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(secondMenu[i]);
                        if (teamDungeonTable == null)
                        {
                            continue;
                        }

                        if(teamDungeonTable.Type == TeamDungeonTable.eType.CityMonster)
                        {
                            bCityMonster = true;
                            iCityMonsterDungeonID = secondMenu[i];
                            break;
                            }
                        }
                    }

                    if (teamDungeonID == 1 || bCityMonster)
                    {
                        none.CustomActive(true);
                        txtTips.CustomActive(false);
                        btns.CustomActive(false);
                    }
                    else
                    {
                        none.CustomActive(false);
                        txtTips.CustomActive(true);
                        btns.CustomActive(false);
                    }

                    if(teamDungeonID == 1 || bCityMonster)
                    {
                        UIGray gray = btnOK.gameObject.SafeAddComponent<UIGray>(false);
                        if (null != gray)
                        {
                            gray.enabled = false;
                        }
                        btnOK.interactable = true;

                        iTargetID = (uint)teamDungeonID;
                        if(bCityMonster)
                        {
                            iTargetID = (uint)iCityMonsterDungeonID;
                        }
                    }
                    else
                    {
                        UIGray gray = btnOK.gameObject.SafeAddComponent<UIGray>(false);
                        if (null != gray)
                        {
                            gray.enabled = true;
                        }
                        btnOK.interactable = false;

                        iTargetID = 0;
                    }
                }
            }
        }

        private void _sendSwitchTeamDungeonID(int teamDungeonID)
        {
            return;

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
                        if (null != table && (TeamDungeonTable.eType.DUNGEON == table.Type
                                              || TeamDungeonTable.eType.CityMonster == table.Type))
                        {
                            TeamDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.Target, teamDungeonID);
                        }
                    }
                    break;
            }
        }

        void OnChooseSecondMenu(int teamDungeonID, bool value,List<int> dungeonIDs = null)
        {
            if (bStartMatch)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }

            Logger.LogProcessFormat("[组队] 二级，组队地下城 {0} -> {1}", teamDungeonID, value);

            if (value /*&& CurTeamDungeonTableID != teamDungeonID */)
            {
                if (btns != null)
                {
                    btns.CustomActive(true);
                }

                if (txtTips != null)
                {
                    txtTips.CustomActive(false);
                }

                if(teamDungeonID == 1)
                {
                    if (btns != null)
                    {
                        btns.CustomActive(false);
                    }

                    if (txtTips != null)
                    {
                        txtTips.CustomActive(true);
                    }

                    iTargetID = (uint)teamDungeonID;
                }

                TeamDungeonNode node = _getTeamDungeonNodeByID(teamDungeonID);

                if (null != node)
                {
                    _sendSwitchTeamDungeonID(teamDungeonID);
                }

                iCurDungeonID = (uint)teamDungeonID;

                if(iCurDungeonID == 1)
                {
                    UIGray gray = btnOK.gameObject.SafeAddComponent<UIGray>(false);
                    if (null != gray)
                    {
                        gray.enabled = true;
                    }
                    btnOK.interactable = false;
                }

                UpdateAllTypeLockState(teamDungeonID);

                TeamDungeonTable table1 = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(teamDungeonID);
                if(table1 != null)
                {
                    if(teamDungeonID == 1 || table1.Type == TeamDungeonTable.eType.CityMonster)
                    {
                        none.CustomActive(true);
                        txtTips.CustomActive(false);
                        btns.CustomActive(false);
                    }
                    else
                    {
                        none.CustomActive(false);
                        txtTips.CustomActive(false);
                        btns.CustomActive(true);
                    }

                    if (teamDungeonID == 1 || table1.Type == TeamDungeonTable.eType.CityMonster)
                    {
                        UIGray gray = btnOK.gameObject.SafeAddComponent<UIGray>(false);
                        if (null != gray)
                        {
                            gray.enabled = false;
                        }
                        btnOK.interactable = true;

                        iTargetID = (uint)teamDungeonID;
                    }
                    else
                    {
                        UIGray gray = btnOK.gameObject.SafeAddComponent<UIGray>(false);
                        if (null != gray)
                        {
                            gray.enabled = false;
                        }
                        btnOK.interactable = true;

                        iTargetID = 0;

                        togType2.isOn = true;
                        togType3.isOn = false;
                        togType4.isOn = false;
                        togType5.isOn = false;
               
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

                                DungeonTable table2 = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable1.DungeonID);
                                if (table2 == null)
                                {
                                    continue;
                                }

                                if (table2.Hard == DungeonTable.eHard.NORMAL)
                                {
                                    iTargetID = (uint)diffInfo.secteamDungeonID[i];
                                    break;
                                }
                            }
                        }
                        while (false);
                    }
                }
            }
        }

        private void _onTeamInfoUpdateSuccess(UIEvent ui)
        {
            //_selectNodeByID(CurTeamDungeonTableID, true);
        }

        void UpdateAllTypeLockState(int teamDungeonID)
        {
            List<Toggle> toggles = new List<Toggle>();
            toggles.Add(togType2);
            toggles.Add(togType3);
            toggles.Add(togType4);
            toggles.Add(togType5);

            bool bIsDuoLuo = false;
            bool isWeekHell = false;        //周常深渊，只显示王者难度
            bool isGuildDungeonMap = false; // 公会副本只显示普通难度
            bool isEliteDungeon = false;

            for (int i = 0; i < locks.Count; i++)
            {
                GameObject go = locks[i];
                if (go != null)
                {
                    go.CustomActive(false);
                }
            }

            for (int i = 0; i < toggles.Count; i++)
            {
                Toggle tog = toggles[i];
                if (tog != null)
                {
                    tog.CustomActive(true);
                }
            }

            if (teamDungeonID == 1)
            {
                return;
            }

            if (FliterFirstMenuList.Contains(teamDungeonID))
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

                if(TeamDataManager.GetInstance().IsDuoLuoTeamDungeonID(teamDungeonID))
                {
                    bIsDuoLuo = true;
                }
                else if (DungeonUtility.IsWeekHellEntryDungeon(teamDungeonTable.DungeonID) == true)
                {
                    isWeekHell = true;
                }

                isGuildDungeonMap = GuildDataManager.GetInstance().IsGuildDungeonMap(teamDungeonTable.DungeonID);
                isEliteDungeon = TeamUtility.IsEliteDungeonID(teamDungeonTable.DungeonID);

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

            for (int j = (int)maxHard + 1; j < locks.Count; j++)
            {
                GameObject go = locks[j];
                if (go != null)
                {
                    go.CustomActive(true);
                }
            }

            for(int i = 0;i < toggles.Count;i++)
            {
                Toggle tog = toggles[i];
                if(tog != null)
                {
                    tog.interactable = true;                  
                }
            }

            for (int j = (int)maxHard + 1; j < toggles.Count; j++)
            {
                Toggle tog = toggles[j];
                if (tog != null)
                {
                    tog.interactable = false;
                }
            }

            // 这里做一下处理 堕落深渊关卡只显示王者难度按钮(实际是只显示普通难度按钮，并将文字换成王者)
            //周常深渊之显示王者难度按钮
            // 公会副本只显示普通难度按钮
            // 精英地下城也只显示普通难度按钮
            if(bIsDuoLuo || isWeekHell || isGuildDungeonMap || isEliteDungeon)
            {
                // 先把锁都隐藏
                for (int j = 0; j < locks.Count; j++)
                {
                    GameObject go = locks[j];
                    if (go != null)
                    {
                        go.CustomActive(false);
                    }
                }

                // 再难度tab都隐藏掉
                for (int i = 0; i < toggles.Count; i++)
                {
                    Toggle tog = toggles[i];
                    if (tog != null)
                    {
                        tog.CustomActive(false);
                    }
                }

                togType2.CustomActive(true);
                togType2.SafeSetToggleOnState(true);
            }

            if(bIsDuoLuo || isWeekHell)
            {             
                normalDiffText.SafeSetText("王者");
            }
            else
            {         
                normalDiffText.SafeSetText("普通");
            }


            return;
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
                    if(mAllNodes[i].bind!=null)
                    {
                        mAllNodes[i].bind.ClearAllCacheBinds();
                        mAllNodes[i].bind = null;
                    }
                 
                }
            }

            for (int i = 0; i < mAllNodes.Count; i++)
            {
                if (TeamDungeonTable.eType.DUNGEON != mAllNodes[i].table.Type && null != mAllNodes[i])
                {
                    if (mAllNodes[i].bind != null)
                    {
                        mAllNodes[i].bind.ClearAllCacheBinds();
                        mAllNodes[i].bind = null;
                    }
                       
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
          
        }

        private void _selectNodeByID(int teamDungeonID, bool isOn)
        {
            TeamDungeonNode seletednode = _getTeamDungeonNodeByID(teamDungeonID);
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
// 
//                         if(teamDungeonID == 1)
//                         {
//                             desc.text = "全部挑战";
//                         }

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

        Dictionary<string, DiffInfo> secteamDungeons = new Dictionary<string, DiffInfo>();
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

                        List<int> secondMenuTemp = new List<int>();
                        for (int i = 0; i < secondMenu.Count; i++)
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

                                //                                 if (!(table.SubType == DungeonTable.eSubType.S_HELL_ENTRY || table.SubType == DungeonTable.eSubType.S_YUANGU))
                                //                                 {
                                //                                     continue;
                                //                                 }

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
                                }
                                else
                                {
                                    diffInfo.secteamDungeonID.Add(secondMenuTemp[i]);
                                    if (diffInfo.iMaxLv < teamDungeonTable.RecoLevel)
                                    {
                                        diffInfo.iMaxLv = teamDungeonTable.RecoLevel;
                                    }

                                    secondMenuTemp.RemoveAt(i);
                                    i--;
                                    continue;
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
                                    TeamDungeonNode senode = new TeamDungeonNode();

                                    senode.id = secteamDungeonID;
                                    senode.table = teamDungeonTable;

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

                                    DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(findNode.table.DungeonID);
                                    if (table != null)
                                    {
                                        DiffInfo diffInfo = new DiffInfo();
                                        secteamDungeons.TryGetValue(table.Name, out diffInfo);
                                        if (diffInfo != null)
                                        {
                                            name.text = diffInfo.dungeonName;
                                            reclevel.text = string.Format("推荐等级:{0}-{1}级", diffInfo.iMinLv, diffInfo.iMaxLv);

                                            if (diffInfo.iMaxLv == diffInfo.iMinLv)
                                            {
                                                reclevel.text = string.Format("推荐等级:{0}级", diffInfo.iMinLv);
                                            }

                                            ongroup.onValueChanged.RemoveAllListeners();
                                            ongroup.onValueChanged.AddListener((svalue) => { OnChooseSecondMenu(secteamDungeonID, svalue, diffInfo.secteamDungeonID); });
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

        private Toggle togType1 = null;
        private Toggle togType2 = null;
        private Toggle togType3 = null;
        private Toggle togType4 = null;
        private Toggle togType5 = null;

        private GameObject btns = null;
        private Text txtTips = null;

        private Button btnOK = null;
        private GameObject none = null;

        private Text normalDiffText = null;


        protected override void _bindExUI()
        {
            mSecondGroup = mBind.GetCom<ToggleGroup>("secondGroup");         
            mDungeonScrollList = mBind.GetCom<ScrollRect>("dungeonScrollList");
            mTargetRoot = mBind.GetGameObject("TargetRoot");
            mFirstMenutogglegroup = mBind.GetCom<ToggleGroup>("firstMenutogglegroup");
            mOnClose = mBind.GetCom<Button>("onClose");
            mOnClose.onClick.AddListener(_onOnCloseButtonClick);  

            togType1 = mBind.GetCom<Toggle>("Type1");
            togType2 = mBind.GetCom<Toggle>("Type2");
            togType3 = mBind.GetCom<Toggle>("Type3");
            togType4 = mBind.GetCom<Toggle>("Type4");
            togType5 = mBind.GetCom<Toggle>("Type5");
      
            togType1.SafeSetOnValueChangedListener(var => 
            {
                DiffTypeChange(var, -1);
            });
        
            togType2.SafeSetOnValueChangedListener(var =>
            {
                DiffTypeChange(var, (int)DungeonTable.eHard.NORMAL);
            });
          
            togType3.SafeSetOnValueChangedListener(var =>
            {
                DiffTypeChange(var, (int)DungeonTable.eHard.RISK);
            });
     
            togType4.SafeSetOnValueChangedListener(var =>
            {
                DiffTypeChange(var, (int)DungeonTable.eHard.WARRIOR);
            });
    
            togType5.SafeSetOnValueChangedListener(var =>
            {
                DiffTypeChange(var, (int)DungeonTable.eHard.KING);
            });

            btns = mBind.GetGameObject("btns");
            txtTips = mBind.GetCom<Text>("txtTips");
            none = mBind.GetGameObject("none");

            btnOK = mBind.GetCom<Button>("btnOK");
            if(btnOK != null)
            {
                btnOK.onClick.RemoveAllListeners();
                btnOK.onClick.AddListener(() => 
                {
                    if (TeamDataManager.GetInstance().IsTeamLeader())
                    {
                        int teamDungeonID = (int)iTargetID;
                        TeamDungeonTable table = TableManager.instance.GetTableItem<TeamDungeonTable>(teamDungeonID);
                        if (null != table && (TeamDungeonTable.eType.DUNGEON == table.Type
                                              || TeamDungeonTable.eType.CityMonster == table.Type || teamDungeonID == 1))
                        {
                            TeamDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.Target, teamDungeonID);
                        }
                    }

                    frameMgr.CloseFrame(this);
                });
            }

            normalDiffText = mBind.GetCom<Text>("normalDiffText");
        }

        uint iCurDungeonID = 0;
        uint iTargetID = 0;

        private void DiffTypeChange(bool val,int hard)
        {
            if(hard == -1)
            {
                return;
            }

            if(val)
            {
                iTargetID = 0;
                do
                {
                    TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)iCurDungeonID);
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
                        continue;
                    }

                    for(int i = 0;i < diffInfo.secteamDungeonID.Count;i++)
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

                        if(table1.Hard == (DungeonTable.eHard)hard)
                        {
                            iTargetID = (uint)diffInfo.secteamDungeonID[i];
                            break;
                        }
                    }
                }
                while (false);

                if (btnOK != null)
                {
                    UIGray gray = btnOK.gameObject.SafeAddComponent<UIGray>(false);
                    if (null != gray)
                    {
                        gray.enabled = iTargetID == 0;
                    }
                    btnOK.interactable = iTargetID > 0;

                    if(iTargetID == 0)
                    {
                        TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)iCurDungeonID);
                        if (teamDungeonTable != null && teamDungeonTable.Type == TeamDungeonTable.eType.CityMonster && hard == (int)DungeonTable.eHard.NORMAL)
                        {
                            iTargetID = iCurDungeonID;

                            if (null != gray)
                            {
                                gray.enabled = false;
                            }
                            btnOK.interactable = true;
                        }
                    }
                }
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

            mOnClose = mBind.GetCom<Button>("onClose");
            mOnClose.onClick.AddListener(_onOnCloseButtonClick);
            mOnClose = null;

            togType1 = null;
            togType2 = null;
            togType3 = null;
            togType4 = null;
            togType5 = null;

            btnOK = null;
            txtTips = null;
            none = null;
            btns = null;

            normalDiffText = null;
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
