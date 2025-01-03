using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System;
using Protocol;
using Network;

namespace GameClient
{
    class TeamListViewFrame : ClientFrame
    {
        class TeamUI
        {
            public Image teamIcon;
            public Text teamName;   
            public Text targetName;
            public Image Num1;
            public Image Num2;
            public Image Num3;
            public Button join;
            public GameObject returnPlayer;
            public GameObject myFriend;
            public GameObject myGuild;

            private bool[,] mSetNumFlag = new bool[4, 3]
            {
                {false, false, false},
                {true,  false, false},
                {true,  true,  false},
                {true,  true,  true},
            };

            public void SetNumImage(int iNum)
            {
                if (iNum >= mSetNumFlag.GetLength(0) || iNum < 0)
                {
                    return;
                }

                Num1.gameObject.SetActive(mSetNumFlag[iNum, 0]);
                Num2.gameObject.SetActive(mSetNumFlag[iNum, 1]);
                Num3.gameObject.SetActive(mSetNumFlag[iNum, 2]);
            }

            public int GetNumImageCount()
            {
                return mSetNumFlag.GetLength(0);
            }
        }

        private int CurTeamDungeonTableID 
        {
            get 
            {
                return (int)TeamDataManager.GetInstance().TeamDungeonID;
            }
        }

        bool bStartMatch = false;
        float fAddUpTime = 0.0f;

        private List<int> FliterFirstMenuList = new List<int>();
        private Dictionary<int, List<int>> FliterSecondMenuDict = new Dictionary<int, List<int>>();
        List<TeamUI> TeamListUI = new List<TeamUI>();
        List<GameObject> TeamListObj = new List<GameObject>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamListView";
        }

        protected override void _OnOpenFrame()
        {
            InitInterface();
            mRefreshTime = kRefreshGap;
            FliterSecondMenuDict.Clear();
            FliterFirstMenuList = Utility.GetTeamDungeonMenuFliterList(ref FliterSecondMenuDict);
        }

        protected override void _OnCloseFrame()
        {
            Clear();        
        }

        void Clear()
        {   
            mRefreshTime = kRefreshGap;
            bStartMatch = false;
            fAddUpTime = 0.0f;

            TeamListUI.Clear();
            TeamListObj.Clear();

            UnBindUIEvent();
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamListRequestSuccess, OnTeamListRequestSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamMatchStartSuccess, OnTeamMatchStartSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamMatchCancelSuccess, OnTeamMatchCancelSuccess);         
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamListUpdateByHard, OnUdpateTeamListByHard);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamListRequestSuccess, OnTeamListRequestSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamMatchStartSuccess, OnTeamMatchStartSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamMatchCancelSuccess, OnTeamMatchCancelSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamListUpdateByHard, OnUdpateTeamListByHard);
        }

        void OnClickJoinTeam(ulong iTeamID)
        {
            //Debug.LogErrorFormat("OnClickJoinTeam iTeamID = {0}", iTeamID);

            int iIndex = -1;

            List<Team> teamDatas = TeamDataManager.GetInstance().GetTeamList();
            for(int i = 0;i < teamDatas.Count;i++)
            {
                if(iTeamID == teamDatas[i].leaderInfo.id)
                {
                    iIndex = i;
                    break;
                }
            }           
            

            if (iIndex < 0 || teamDatas == null || iIndex >= teamDatas.Count)
            {
                return;
            }

            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_already_has_team"));
                return;
            }

            int iNotifyID = -1;
            DungeonID id = new DungeonID((int)teamDatas[iIndex].leaderInfo.id);
            if (!Utility.CheckJoinTeamCondition((ulong)id.dungeonIDWithOutDiff, ref iNotifyID))
            {
                if (iNotifyID != -1)
                {
                    SystemNotifyManager.SystemNotify(iNotifyID);
                }

                return;
            }

            if (teamDatas[iIndex].currentMemberCount >= teamDatas[iIndex].maxMemberCount)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("队伍人数已满");
                return;
            }

            TeamDataManager.GetInstance().JoinTeam(teamDatas[iIndex].leaderInfo.id);
        }

        //[UIEventHandle("Bottom/btMatch")]
        void OnMatch()
        {
            NetManager netMgr = NetManager.Instance();

            if (!bStartMatch)
            {

                TeamDungeonTable table = TableManager.instance.GetTableItem<TeamDungeonTable>(CurTeamDungeonTableID);

                // TODO 在TeamDungeonNode中添加一个CanMatch
                if (null == table || table.Type == TeamDungeonTable.eType.MENU || CurTeamDungeonTableID == 1)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("请选择一个关卡目标");
                    return;
                }

                SceneTeamMatchStartReq msg = new SceneTeamMatchStartReq();
                msg.dungeonId = (uint)table.DungeonID;

                TeamListFrame frame1 = ClientSystemManager.GetInstance().GetFrame(typeof(TeamListFrame)) as TeamListFrame;
                if (frame1 == null)
                {
                    return;
                }
                int iHard = TeamDataManager.GetInstance().DiffHard;
                if (iHard == -1)
                {
                    return;
                }
                else
                {
                    if (iHard >= frame1.locks.Count)
                    {
                        return;
                    }              
                    if (frame1.locks[iHard].activeInHierarchy)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("关卡未解锁");
                        return;
                    }
                    Dictionary<string, DiffInfo> secteamDungeons = frame1.secteamDungeons;
                    int iTeamDungeonID = CurTeamDungeonTableID;
                    do
                    {
                        TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);
                        if (teamDungeonTable == null)
                        {
                            continue;
                        }
                        DungeonTable table1 = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
                        if (table1 == null)
                        {
                            if (teamDungeonTable.Type == TeamDungeonTable.eType.CityMonster)
                            {
                                iTeamDungeonID = CurTeamDungeonTableID;
                                if(iHard != 0)
                                {
                                    return;
                                }
                            }
                            continue;
                        }
                        DiffInfo diffInfo = new DiffInfo();
                        secteamDungeons.TryGetValue(table1.Name, out diffInfo);
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
                            if (table2.Hard == (DungeonTable.eHard)iHard)
                            {
                                iTeamDungeonID = diffInfo.secteamDungeonID[i];
                                break;
                            }
                        }
                    }
                    while (false);
                    TeamDungeonTable table3 = TableManager.instance.GetTableItem<TeamDungeonTable>(iTeamDungeonID);
                    if(table3 != null)
                    {
                        msg.dungeonId = (uint)table3.DungeonID;
                    }                    
                }                
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
            else
            {
                WorldTeamMatchCancelReq msg = new WorldTeamMatchCancelReq();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            }
        }

        //[UIEventHandle("Bottom/btCreateTeam")]
        void OnCreateTeam()
        {
            TeamCreateInfo createInfo = TeamDataManager.GetInstance().CreateInfo;
            createInfo.Debug();
            createInfo.Reset();
            TeamListFrame frame1 = ClientSystemManager.GetInstance().GetFrame(typeof(TeamListFrame)) as TeamListFrame;
            if (frame1 == null)
            {
                return;
            }
            int iHard = TeamDataManager.GetInstance().DiffHard;
            if(iHard == -1)
            {
                if(TeamDataManager.GetInstance().TeamDungeonID == 1)
                {
                    TeamDataManager.GetInstance().CreateTeam((uint)CurTeamDungeonTableID);
                }
                else
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("请选择挑战难度");
                }
                return;
            }
            else
            {
                if(iHard >= frame1.locks.Count)
                {                    
                    return;
                }
                if(frame1.locks[iHard].activeInHierarchy)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("关卡未解锁");
                    return;
                }
                Dictionary<string, DiffInfo> secteamDungeons = frame1.secteamDungeons;
                int iTeamDungeonID = CurTeamDungeonTableID;
                do
                {
                    TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);
                    if (teamDungeonTable == null)
                    {
                        continue;
                    }
                    DungeonTable table = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
                    if (table == null)
                    {
                        if (teamDungeonTable.Type == TeamDungeonTable.eType.CityMonster)
                        {
                            iTeamDungeonID = CurTeamDungeonTableID;
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
                        if (table1.Hard == (DungeonTable.eHard)iHard)
                        {
                            iTeamDungeonID = diffInfo.secteamDungeonID[i];
                            break;
                        }
                    }
                }
                while (false);
                TeamDataManager.GetInstance().CreateTeam((uint)iTeamDungeonID);
                return;
            }


            TeamDataManager.GetInstance().CreateTeam((uint)CurTeamDungeonTableID);
        }

        void InitInterface()
        {
            BindUIEvent();
            //SendTeamSearchInfo((uint)CurTeamDungeonTableID);
        }


        void UpdateTeamListPrefab()
        {
            TeamDungeonTable tdData = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(CurTeamDungeonTableID);

            if (tdData == null)
            {
                return;
            }


            if (tdData != null)
            {
                if (!bStartMatch)
                {
                    if (tdData.MatchType == TeamDungeonTable.eMatchType.QUICK_MATCH)
                    {
                        mMatchText.text = "快速匹配";
                    }
                    else
                    {
                        mMatchText.text = "快速组队";
                    }
                }
            }           

            List<Team> teamDatas = TeamDataManager.GetInstance().GetTeamList();
            if (teamDatas.Count > 0)
            {
                mTeamListDesc.gameObject.SetActive(false);
            }
            else
            {
                mTeamListDesc.gameObject.SetActive(true);
            }

            if (teamDatas.Count > TeamListObj.Count)
            {
                int iOriNum = TeamListObj.Count;
                int iDifference = teamDatas.Count - TeamListObj.Count;

                for (int i = 0; i < iDifference; i++)
                {
                    GameObject TeamListEleObj = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Team/TeamListEle");
                    if (TeamListEleObj == null)
                    {
                        continue;
                    }

                    Utility.AttachTo(TeamListEleObj, mTeamListRoot);
                    TeamListObj.Add(TeamListEleObj);

                    TeamUI ui = new TeamUI();

                    Image[] imgs = TeamListEleObj.GetComponentsInChildren<Image>();
                    for (int j = 0; j < imgs.Length; j++)
                    {
                        if (imgs[j].name == "Icon")
                        {
                            ui.teamIcon = imgs[j];
                        }
                        else if (imgs[j].name == "Num1")
                        {
                            ui.Num1 = imgs[j];
                        }
                        else if (imgs[j].name == "Num2")
                        {
                            ui.Num2 = imgs[j];
                        }
                        else if (imgs[j].name == "Num3")
                        {
                            ui.Num3 = imgs[j];
                        }
                    }

                    Text[] texts = TeamListEleObj.GetComponentsInChildren<Text>();
                    for (int j = 0; j < texts.Length; j++)
                    {
                        if (texts[j].name == "Name")
                        {
                            ui.teamName = texts[j];
                        }                       
                        else if (texts[j].name == "TargetName")
                        {
                            ui.targetName = texts[j];
                        }
                    }

                    ui.join = TeamListEleObj.GetComponentInChildren<Button>();
                    ComCommonBind bind = TeamListEleObj.GetComponent<ComCommonBind>();
                    if (bind != null)
                    {
                        ui.returnPlayer = bind.GetGameObject("returnPlayer");
                        ui.myFriend = bind.GetGameObject("myFriend");
                        ui.myGuild = bind.GetGameObject("myGuild");
                    }

                    TeamListUI.Add(ui);
                }
            }

            for (int i = 0; i < TeamListObj.Count; i++)
            {
                TeamListObj[i].SetActive(false);
            }
            int iIndex = 0;
            int iTeamIndex = 0;
            for (int i = 0; i < teamDatas.Count && iIndex < TeamListObj.Count; i++)
            {
                iTeamIndex = i;
                TeamDungeonTable table1 = TableManager.instance.GetTableItem<TeamDungeonTable>((int)teamDatas[i].teamDungeonID);
                if (table1 == null)
                {
                    continue;
                }
                DungeonTable table2 = TableManager.GetInstance().GetTableItem<DungeonTable>(table1.DungeonID);
                if (table2 == null && table1.Type != TeamDungeonTable.eType.CityMonster)
                {
                    continue;
                }

                int iHard = TeamDataManager.GetInstance().DiffHard;

                //堕落深渊只在王者和全部的页签下面显示              
                //周常深渊只在王者和全部的页签下面显示
                if (TeamDataManager.GetInstance().IsDuoLuoTeamDungeonID((int)teamDatas[i].teamDungeonID) || DungeonUtility.IsWeekHellEntryDungeon(table1.DungeonID) == true)
                {
                    //全部和王者
                    if (TeamDataManager.GetInstance().DiffHard != -1 &&
                        (DungeonTable.eHard)TeamDataManager.GetInstance().DiffHard != DungeonTable.eHard.KING)
                    {
                        continue;
                    }
                }
                else
                {
                    if (iHard != -1 && table2 != null && table2.Hard != (DungeonTable.eHard)iHard)
                    {
                        continue;
                    }
                }

                JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(teamDatas[i].leaderInfo.occu);
                if (jobData != null)
                {
                    ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                    if (resData != null)
                    {
                        //Sprite spr = AssetLoader.instance.LoadRes(resData.IconPath, typeof(Sprite)).obj as Sprite;
                        //if (spr != null)
                        //{
                        //    TeamListUI[i].teamIcon.sprite = spr;
                        //}
                        ETCImageLoader.LoadSprite(ref TeamListUI[iIndex].teamIcon, resData.IconPath);
                    }
                }

                TeamListUI[iIndex].teamName.text = teamDatas[i].leaderInfo.name;                
                TeamDungeonTable table = TableManager.instance.GetTableItem<TeamDungeonTable>((int)teamDatas[i].teamDungeonID);
                if (null != table)
                {
                    TeamListUI[iIndex].targetName.text = table.Name;
                }

                if (teamDatas[i].currentMemberCount >= TeamListUI[iIndex].GetNumImageCount())
                {
                    string log = string.Format("队伍成员数量<{0}>超过设计的数量<{1}>,检查服务端数据，队伍id{2}, 队伍地下城id{3}, 最大成员数{4}. 队伍成员:\n",
                    teamDatas[i].currentMemberCount, TeamListUI[iIndex].GetNumImageCount(), teamDatas[i].teamID, teamDatas[i].teamDungeonID, teamDatas[i].maxMemberCount);
                    for (int j = 0; j < teamDatas[i].members.Length; ++j)
                    {
                        var member = teamDatas[i].members[j];
                        if (member != null)
                        {
                            log += string.Format("id:{0}, 名字{1}\n", member.id, member.name);
                        }
                        else
                        {
                            log += string.Format("下标为{0}的数组成员为空", j);
                        }
                    }
                    Logger.LogError(log);
                }

                TeamListUI[iIndex].SetNumImage(teamDatas[i].currentMemberCount);

                //Debug.LogErrorFormat("OnClickJoinTeam iTeamID = {0}", teamDatas[i].leaderInfo.id);

                ulong iTeamID = teamDatas[i].leaderInfo.id;

                TeamListUI[iIndex].join.onClick.RemoveAllListeners();
                TeamListUI[iIndex].join.onClick.AddListener(() => { OnClickJoinTeam(iTeamID); });

                TeamListObj[iIndex].SetActive(true);
                TeamListUI[iIndex].returnPlayer.CustomActive(false);
                TeamListUI[iIndex].myFriend.CustomActive(false);
                TeamListUI[iIndex].myGuild.CustomActive(false);
                RelationData relationData = null;
                bool isMyFriend = RelationDataManager.GetInstance().FindPlayerIsRelation(teamDatas[i].leaderInfo.id, ref relationData);
                bool isMyGuild = GuildDataManager.GetInstance().IsSameGuild(teamDatas[i].leaderInfo.playerLabelInfo.guildId);
                if (teamDatas[i].leaderInfo.playerLabelInfo.returnStatus == 1)
                {
                    TeamListUI[iIndex].returnPlayer.CustomActive(true);
                }
                else if (isMyFriend)
                {
                    TeamListUI[iIndex].myFriend.CustomActive(true);
                }
                else if (isMyGuild)
                {
                    TeamListUI[iIndex].myGuild.CustomActive(true);
                }
                iIndex++;
            }
            if (iIndex > 0)
            {
                mTeamListDesc.gameObject.SetActive(false);
            }
            else
            {
                mTeamListDesc.gameObject.SetActive(true);
            }
        }

        void OnTeamListRequestSuccess(UIEvent uiEvent)
        {        
            UpdateTeamListPrefab();
        }

        void OnUdpateTeamListByHard(UIEvent uiEvent)
        {
            UpdateTeamListPrefab();
        }

        void OnTeamMatchStartSuccess(UIEvent uiEvent)
        {
            fAddUpTime = 0.0f;
            bStartMatch = true;

            TeamDungeonTable tdData = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(CurTeamDungeonTableID);

            if (tdData == null)
            {
                return;
            }

            TeamMatchWaitingData data = new TeamMatchWaitingData();

            if (tdData.MatchType == TeamDungeonTable.eMatchType.QUICK_MATCH)
            {
                data.matchState = MatchState.TeamMatch;
            }
            else
            {
                data.matchState = MatchState.TeamJoin;
            }

            data.TeamDungeonTableID = CurTeamDungeonTableID;
            TeamListFrame frame1 = ClientSystemManager.GetInstance().GetFrame(typeof(TeamListFrame)) as TeamListFrame;
            if (frame1 == null)
            {
                return;
            }
            int iHard = TeamDataManager.GetInstance().DiffHard;
            if (iHard == -1)
            {
                return;
            }
            else
            {
                if (iHard >= frame1.locks.Count)
                {
                    return;
                }               
                Dictionary<string, DiffInfo> secteamDungeons = frame1.secteamDungeons;
                int iTeamDungeonID = CurTeamDungeonTableID;
                do
                {
                    TeamDungeonTable teamDungeonTable = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)CurTeamDungeonTableID);
                    if (teamDungeonTable == null)
                    {
                        continue;
                    }
                    DungeonTable table1 = TableManager.GetInstance().GetTableItem<DungeonTable>(teamDungeonTable.DungeonID);
                    if (table1 == null)
                    {
                        if (teamDungeonTable.Type == TeamDungeonTable.eType.CityMonster)
                        {
                            iTeamDungeonID = CurTeamDungeonTableID;
                            if (iHard != 0)
                            {
                                return;
                            }
                        }
                        continue;
                    }
                    DiffInfo diffInfo = new DiffInfo();
                    secteamDungeons.TryGetValue(table1.Name, out diffInfo);
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
                        if (table2.Hard == (DungeonTable.eHard)iHard)
                        {
                            iTeamDungeonID = diffInfo.secteamDungeonID[i];
                            break;
                        }
                    }
                }
                while (false);
                data.TeamDungeonTableID = iTeamDungeonID;
            }

            ClientSystemManager.GetInstance().OpenFrame<TeamMatchWaitingFrame>(FrameLayer.Middle, data);     
        }

        void OnTeamMatchCancelSuccess(UIEvent uiEvent)
        {
            bStartMatch = false;

            TeamDungeonTable tdData = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(CurTeamDungeonTableID);

            if (tdData == null)
            {
                return;
            }

            if (tdData.MatchType == TeamDungeonTable.eMatchType.QUICK_MATCH)
            {
                mMatchText.text = "快速匹配";
            }
            else
            {
                mMatchText.text = "快速组队";
            }
        }

        void OnJoinTeamRequestSuccess(UIEvent uiEvent)
        {

        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        private const float kRefreshGap = 10.0f;
        private float mRefreshTime = 0.0f;

        private void _updateRefresh(float delta)
        {
            if (mRefreshTime > 0)
            {
                mRefreshTime -= delta;
            }
            else
            {
                mRefreshTime = kRefreshGap;
                TeamListFrame frame1 = ClientSystemManager.GetInstance().GetFrame(typeof(TeamListFrame)) as TeamListFrame;
                if (frame1 == null)
                {
                    return;
                }

                
                Dictionary<int, List<int>> secondMenuID2TargetIDs = frame1.secondMenuID2TargetIDs;
                if (secondMenuID2TargetIDs != null && secondMenuID2TargetIDs.ContainsKey((int)CurTeamDungeonTableID))
                {
                    TeamDataManager.GetInstance().RequestSearchTeam((uint)CurTeamDungeonTableID, secondMenuID2TargetIDs[(int)CurTeamDungeonTableID]);
                }
                else
                {
                    TeamDataManager.GetInstance().RequestSearchTeam((uint)CurTeamDungeonTableID);
                }
                //TeamDataManager.GetInstance().RequestSearchTeam((uint)CurTeamDungeonTableID);
            }
        }
        
        protected override void _OnUpdate(float timeElapsed)
        {
//             if(mOnMatch != null)
//             {
//                 bool bShow = true;
//                 if(FliterFirstMenuList.Contains((int)TeamDataManager.GetInstance().TeamDungeonID) || TeamDataManager.GetInstance().DiffHard == -1)
//                 {
//                     bShow = false;
//                 }
//                 mOnMatch.CustomActive(bShow);
//             }
            _updateRefresh(timeElapsed);

            if (bStartMatch)
            {
                TeamDungeonTable tdData = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(CurTeamDungeonTableID);

                if (tdData == null)
                {
                    return;
                }

                fAddUpTime += timeElapsed;
                int iInt = (int)(fAddUpTime);

                if (tdData.MatchType == TeamDungeonTable.eMatchType.QUICK_MATCH)
                {
                    mMatchText.text = string.Format("匹配中({0}秒)", iInt);
                }
                else
                {
                    mMatchText.text = string.Format("组队中({0}秒)", iInt);
                }               
            }
        }

#region ExtraUIBind 
        private GameObject mTeamListRoot = null;
        private Text mTeamListDesc = null;
        private Text mMatchText = null;
        private Button mOnMatch = null;
        private Button mOnCreateTeam = null;

        protected override void _bindExUI()
        {      
            mTeamListRoot = mBind.GetGameObject("TeamListRoot");
            mTeamListDesc = mBind.GetCom<Text>("TeamListDesc");
            mMatchText = mBind.GetCom<Text>("MatchText");
            mOnMatch = mBind.GetCom<Button>("onMatch");
            mOnMatch.onClick.AddListener(_onOnMatchButtonClick);
            mOnCreateTeam = mBind.GetCom<Button>("onCreateTeam");
            mOnCreateTeam.onClick.AddListener(_onOnCreateTeamButtonClick);
        }

        protected override void _unbindExUI()
        {     
            mTeamListRoot = null;
            mTeamListDesc = null;
            mMatchText = null;
            mOnMatch.onClick.RemoveListener(_onOnMatchButtonClick);
            mOnMatch = null;
            mOnCreateTeam.onClick.RemoveListener(_onOnCreateTeamButtonClick);
            mOnCreateTeam = null;
        }
#endregion   

#region Callback
        private void _onOnMatchButtonClick()
        {
            /* put your code in here */
            OnMatch();

        }
        private void _onOnCreateTeamButtonClick()
        {
            /* put your code in here */
            OnCreateTeam();
        }
        #endregion

        


    }
}
