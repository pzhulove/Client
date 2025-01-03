
using System;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using Network;
using Protocol;
using UnityEngine;
using System.IO;
using System.Net;
using System.Reflection;
using Tenmove.Runtime.Unity;

namespace tm.tool
{
#if ROBOT_TEST
    public class AutoFight : MonoSingleton<AutoFight>
    {
        public override void Init()
        {
            base.Init();
            GameObject.DontDestroyOnLoad(this);
        }

        public class AutoFightInfo
        {
            public string Account {get;set;}
            public string IP      {get;set;}
            public int    Port    {get;set;}
            public string RoleID  {get;set;}
            public bool RecordProcess { get; set; }
            public int LogLevel { get; set; }
            public int WriteLogLevel { get; set; }
            public bool Team { get; set; }
            public bool Leader { get; set; }
            public string TeamName { get; set; }
            public List<int> IgnoreDungeonIds { get; set; }
            public List<int> ChapterIDs { get; set; }
        }
        AutoFightInfo mInfoData;
        public const string skConfigPathName = "autofight.json";
        public  string GetPath()
        {
            string dir = "../";

#if UNITY_EDITOR
            dir = "../";
#elif UNITY_ANDROID || UNITY_IOS
            dir = FileUtil.GetWriteablePath();
#else
            dir = "../";
#endif

            return dir + skConfigPathName;
        }

        public AutoFightInfo InfoData
        {
            get 
            {
                if (null == mInfoData)
                {
                    string json = "";
                    string path = GetPath();
                    try 
                    {
                        json = System.IO.File.ReadAllText(path);
                    }
                    catch (System.Exception e)
                    {
                        Logger.LogErrorFormat("load {0} failed {1}", path, e.ToString());
                    }

                    mInfoData = LitJson.JsonMapper.ToObject<AutoFightInfo>(json);
                }

                return mInfoData;
            }
        }

        public void Gogogo()
        {
            AutoStart();
        }

        bool Main;
        string Account;
        string IP;
        int Port;
        string RoleID;
        bool TeamLeader;
        string TeamName;

        public bool IsRelaySvrNotifyUnsync = false;
        public bool IsServerReconnectFailed = false;

        int deep = 0;
        public AutoFight()
        {
            if (null == InfoData)
            {
                return;
            }
            //string[] args = System.Environment.GetCommandLineArgs();

            //int index = 0;
            //// 假设一定有
            //foreach (var arg in args)
            //{
            //    if (arg.IndexOf("index") < 0)
            //    {
            //        continue;
            //    }
            //    index = int.Parse(arg.Substring("index=".Length));

            //}
            Account = InfoData.Account;//  Data.players[index].account;
            IP = InfoData.IP;
            Port = InfoData.Port;
            RoleID = InfoData.RoleID;
            TeamLeader = InfoData.Leader;
            TeamName = InfoData.TeamName;
            


            Logger.LogProcessFormat("Account: {0}, RoleID:{1}", Account, RoleID);
            Logger.LogProcessFormat("Server: {0}:{1}", IP, Port);
        }

        private IEnumerator mEnumerator = null;
        public void AutoStart()
        {
            StopAllCoroutines();

            //NetProcess.AddMsgHandler(RelaySvrNotifyUnsync.MsgID, OnRelaySvrNotifyUnsync);
            NetProcess.AddMsgHandler(GateNotifyKickoff.MsgID, OnRelaySvrNotifyUnsync);

            //UIEventManager.GetInstance().RegisterUIEvent(EUIEventID.ReconnetFailWithTips, _OnReconnectFailed);
            mEnumerator = Exec();
            StartCoroutine(mEnumerator);
            //Tenmove.Runtime.Client.Unity.Environment.Instance.StartCoroutine(mEnumerator);
        }

        public void Finish()
        {
            //NetProcess.RemoveMsgHandler(RelaySvrNotifyUnsync.MsgID, OnRelaySvrNotifyUnsync);
            NetProcess.RemoveMsgHandler(GateNotifyKickoff.MsgID, OnRelaySvrNotifyUnsync);
            //UIEventManager.GetInstance().RegisterUIEvent(EUIEventID.ReconnetFailWithTips, _OnReconnectFailed);
        }

        private void _OnReconnectFailed(UIEvent e)
        {
            IsServerReconnectFailed = true;
        }

        public enum State
        {
            Login,
            SelectRole,
            Login2Town,
            Town,
            TownTeam,
            WaitTeamate,
            WaitJoinTeam,
            Battle,
        }
        
        public State CurState { get; private set; }

        public string CurStateStr
        {
            get
            {
                switch (CurState)
                {
                    case State.Login:
                        return "登录";
                    case State.SelectRole:
                        return "选角";
                    case State.Login2Town:
                        return "登录到城镇";
                    case State.Town:
                        return "城镇";
                    case State.TownTeam:
                        return "组队";
                    case State.WaitTeamate:
                        return "组队-队长等待队友";
                    case State.WaitJoinTeam:
                        return "组队-等待队伍出现";
                    case State.Battle:
                        return "战斗";
                }

                return "*未知*";
            }
            
        }

        public void OnGUI()
        {
            GUI.Label(new Rect(0, 0, 200, 30), CurStateStr);
            if (skWaitNode.D > 0)
            {
                GUI.Label(new Rect(0, 30, 200, 30), skWaitNode.D.ToString());
            }
        }
        
        

        public IEnumerator Exec()
        {
            if (string.IsNullOrEmpty(Account) ||
                string.IsNullOrEmpty(IP) ||
                0 == (Port))
            {
                yield break;
            }

            yield return skWait5Sec;

            // 登陆
            yield return Login();

            yield return SelectRole();

            yield return JoinMyTeam();

            yield return AutoGameManagerStart();

            while (!IsNeedQuit2Login)
            {
                yield return skWait1Sec;
            }

            yield return skWait5Sec;
            ClientSystemManager.instance._QuitToLoginImpl();
        }

        private bool IsNeedQuit2Login
        {
            get 
            {
                return IsRelaySvrNotifyUnsync || IsServerReconnectFailed;
            }
        }

        IEnumerator Login()
        {
            CurState = State.Login;
            global::ClientApplication.adminServer.ip = IP;
            global::ClientApplication.adminServer.port = (ushort)Port;

            CloseFrame<PublishContentFrame>();
            
            while (!IsFrameOpen<SelectRoleFrame>())
            {
                var login = ClientSystemManager.instance.CurrentSystem as ClientSystemLogin;
                if (null != login)
                {
                    login.LoginWithAccount(Account);
                }
                

                yield return skWait2Sec;
                yield return null;
            } 

            
            yield return skWait1Sec;
        }

        private void CloseFrame<T>() where T : ClientFrame
        {
            ClientSystemManager.instance.CloseFrame<T>();
        }

        private bool IsFrameOpen<T>() where T : ClientFrame
        {
            return ClientSystemManager.instance.IsFrameOpen(typeof(T));
        }

        IEnumerator SelectRole()
        {
            CurState = State.SelectRole;
            yield return new WaitClientFrameOpen(typeof(SelectRoleFrame));
            
            ClientApplication.playerinfo.curSelectedRoleIdx = 0;
            yield return ClientSystemLogin.StartEnterGame();

            yield return skWait1Sec;

            CurState = State.Login2Town;
            Global.Settings.CloseLoginPushFrame = true;
            yield return new WaitClientFrameOpen(typeof(ClientSystemTownFrame));
        }

        private static WaitForSeconds skWait1Sec = new WaitForSeconds(1.0f);
        private static WaitForSeconds skWait2Sec = new WaitForSeconds(2.0f);
        private static WaitForSeconds skWait5Sec = new WaitForSeconds(5.0f);

        IEnumerator JoinMyTeam()
        {
            CurState = State.TownTeam;
            
            if (!this.TeamLeader)
            {
                CurState = State.WaitJoinTeam;
                int tryJoinCnt = 5;
                do
                {
                    TeamDataManager.GetInstance().TeamDungeonID = TeamUtility.kDefaultTeamDungeonID;
                    TeamDataManager.GetInstance().RequestTeamList(0);
                    yield return skWait1Sec;
                    var list = TeamDataManager.GetInstance().GetTeamList();
                    Team curTeam = null;
                    foreach (var cur in list)
                    {
                        if (cur.leaderInfo.name == TeamName)
                        {
                            curTeam = cur;
                            break;
                        }
                    }

                    if (curTeam != null)
                    {
                        while (!TeamDataManager.GetInstance().HasTeam() && --tryJoinCnt > 0)
                        {
                            TeamDataManager.GetInstance().JoinTeam(curTeam.leaderInfo.id);
                            yield return skWait1Sec;
                        }

                        TeamDataManager.GetInstance().IsAutoAgree = true;
                        TeamDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.AutoAgree, 1);
                        break;
                    }
                    
                } while (true);

                
                //TeamDataManager.GetInstance().C();
            }

            if (!TeamDataManager.GetInstance().HasTeam())
            {
                yield return CreateMyTeam();
            }
        }
        

        IEnumerator CreateMyTeam()
        {
            if (TeamDataManager.GetInstance().HasTeam())
            {
                yield break;
            }

            if (TeamDataManager.GetInstance().IsTeamLeader())
            {
                yield break;
            }

            if (TeamLeader)
            {
                CurState = State.WaitTeamate;
            }
            else
            {
                CurState = State.TownTeam;
            }
            
            TeamDataManager.GetInstance().CreateTeam(TeamUtility.kDefaultTeamDungeonID);
            TeamDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.AutoAgree, 1);

            if (TeamLeader)
            {
                yield return Wait(10);
            }
        }
        
        private static WaitNode skWaitNode = new WaitNode(0);

        private IEnumerator Wait(float time)
        {
            skWaitNode.D = time;
            yield return skWaitNode;
        }
        
        

        public class WaitNode : IEnumerator
        {
            public float D { get; set; }
            public WaitNode(float time)
            {
                D = time;
            }
            
            public bool MoveNext()
            {
                D -= Time.deltaTime;
                return D > 0;
            }

            public void Reset()
            {
                D = 0;
            }

            public object Current { get; }
        } 
        
        

        IEnumerator AutoGameManagerStart()
        {
            //UIManager.Instance.OpenFrame<AutoGameFrame>();
            //while (!UIManager.Instance.IsFrameOpen<AutoGameFrame>())
            //{
            //    yield return skWait1Sec;
            //}

            if (TeamDataManager.GetInstance().IsTeamLeader())
            {
                AutoFightTestDataManager.GetInstance().IsStart = false;
                AutoFightTestDataManager.GetInstance().SelectDungeonDic.Clear();
                foreach (var id in InfoData.ChapterIDs)
                {
                    _AddAutoFightTestChapter(id);
                }
                AutoFightRunTime.instance.BuildRunTimeData(AutoFightTestDataManager.GetInstance().SelectDungeonDic);
                AutoFightTestDataManager.GetInstance().IsStart = true;
                yield return skWait1Sec;
                //AutoGameManager.Instance.Start(true, true, true);
            }
        }

        private void _AddAutoFightTestChapter(int id)
        {
            var dict = AutoFightTestDataManager.instance.ChapterDataDic;
            if (!dict.ContainsKey(id))
            {
                return;
            }

            foreach (var it in dict[id])
            {
                AutoFightTestDataManager.instance.SelectDungeonDic.Add(it.DungeonId, it.Name);
            }
        }
        

        bool intown = false;

        // void OnTownMapChanged(UIEvent uiEvent)
        // {
        //     intown = true;
        // }

        IEnumerator GoToTown()
        {
            Logger.LogErrorFormat("[GoToTown] enter deep {0}", ++deep);


           //while (!UIManager.Instance.IsFrameOpen<TownFrame>())
           //{
           //    yield return null;
           //}
           //yield return new WaitForSeconds(1);
           //
           //UIManager.Instance.OpenFrame<TeamFrame>();
           //yield return new WaitForSeconds(1);

            Logger.LogErrorFormat("[GoToTown] leave");
            //if (Main)
            //{
            //    if (mTeamInfo == null)
            //    {
            //        Logger.LogErrorFormat("[GoToTown] CreateTeam");
            //        yield return CreateTeam();
            //    }
            //    else
            //    {
            //        Logger.LogErrorFormat("[GoToTown] WaitingForStart");
            //        yield return Setting();
            //    }
            //}
            //else
            //{
            //    if (mTeamInfo == null)
            //    {
            //        Logger.LogErrorFormat("[GoToTown] JoinTeam");
            //        yield return JoinTeam();
            //    }
            //    else
            //    {
            //        Logger.LogErrorFormat("[GoToTown] GameEnd");
            //        yield return GameEnd();
            //    }
            //}
            yield return null;
        }



        //TeamInfo mTeamInfo;
        IEnumerator CreateTeam()
        {

            //var req = new WorldTeamCreateReq();
            //var msgEvent = new MessageEvents();
            //var msgRet = new WorldTeamCreateRes();
            //
            //yield return Tenmove.Runtime.Game.MessageUtility.Wait<WorldTeamCreateReq, WorldTeamCreateRes>(ServerType.GATE_SERVER, msgEvent, req, msgRet, false, 20);
            //
            //if (msgEvent.IsAllMessageReceived())
            //{
            //    if (msgRet.result != 0)
            //    {
            //        Logger.LogErrorFormat("[WorldTeamCreateRes] msgRet.result == {0}", msgRet.result);
            //        yield break;
            //    }
            //
            //    mTeamInfo = new TeamInfo();
            //    mTeamInfo.baseInfo = msgRet.info.baseInfo;
            //    mTeamInfo.memberInfos = new TeamMember[msgRet.info.baseInfo.maxMemberNum];
            //
            //    for (int j = 0; j < msgRet.info.memberInfos.Length; j++)
            //    {
            //        int iii = msgRet.info.memberInfos[j].index - 1;
            //        mTeamInfo.memberInfos[iii] = msgRet.info.memberInfos[j];
            //
            //    }
            //    Logger.LogErrorFormat("[WorldTeamCreateRes] teamid {0}", mTeamInfo.baseInfo.teamId);
            //}
            //else
            //{
            //    Logger.LogErrorFormat("[WorldTeamCreateRes] not AllMessageReceived", msgRet.result);
            //    yield break;
            //}
            //
            yield return new WaitForSeconds(1);
            yield return Setting();
        }


        IEnumerator JoinTeam()
        {
            Logger.LogErrorFormat("[JoinTeam] enter");

            TeamBaseInfo iteamInfo = null;
            //do
            //{
            //    WorldTeamListReq listReq = new WorldTeamListReq();
            //    listReq.num = 100;
            //    WorldTeamListRes listRes = new WorldTeamListRes();
            //    MessageEvents msgEvent = new MessageEvents();
            //    Logger.LogErrorFormat("[WorldTeamListReq] send");
            //    yield return Tenmove.Runtime.Game.MessageUtility.Wait<WorldTeamListReq, WorldTeamListRes>(ServerType.GATE_SERVER, msgEvent, listReq, listRes, false, 20);
            //    if (msgEvent.IsAllMessageReceived())
            //    {
            //        Logger.LogErrorFormat("[WorldTeamListRes] result == {0}", listRes.result);
            //        if (listRes.result == 0)
            //        {
            //
            //            foreach (var info in listRes.teamList)
            //            {
            //                if (info.isPublish == 0 || info.autoAgree == 0)
            //                {
            //                    continue;
            //                }
            //
            //                if (info.name == "自动战斗")
            //                {
            //                    iteamInfo = info;
            //                    break;
            //                }
            //            }
            //            if (iteamInfo != null)
            //            {
            //                break;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Logger.LogErrorFormat("[WorldTeamListRes] not AllMessageReceived", listRes.result);
            //    }
            //
            //    // 没找到 就继续
            //    yield return new WaitForSeconds(1);
            //} while (true);

            //do
            //{
            //    WorldTeamJoinReq req = new WorldTeamJoinReq();
            //    req.teamId = iteamInfo.teamId;
            //    WorldTeamJoinRes res = new WorldTeamJoinRes();
            //    MessageEvents msgEvent = new MessageEvents();
            //
            //    Logger.LogErrorFormat("[WorldTeamJoinReq] send");
            //    yield return Tenmove.Runtime.Game.MessageUtility.Wait<WorldTeamJoinReq, WorldTeamJoinRes>(ServerType.GATE_SERVER, msgEvent, req, res, false, 20);
            //    if (msgEvent.IsAllMessageReceived())
            //    {
            //        Logger.LogErrorFormat("[WorldTeamJoinRes] result == {0}", res.result);
            //        if (res.result == 0)
            //        {
            //            mTeamInfo = res.info;
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        Logger.LogErrorFormat("[WorldTeamJoinRes] not AllMessageReceived");
            //    }
            //
            //    // 没找到 就继续
            //} while (false);
            //
            //do
            //{
            //
            //    WorldTeamReportVoteReq req = new WorldTeamReportVoteReq();
            //    req.voteType = (byte)TeamVoteType.TEAM_VOTE_ACCEPT;
            //    WorldTeamReportVoteRes res = new WorldTeamReportVoteRes();
            //    MessageEvents msgEvent = new MessageEvents();
            //
            //    Logger.LogErrorFormat("[WorldTeamReportVoteReq] send");
            //    yield return Tenmove.Runtime.Game.MessageUtility.Wait<WorldTeamReportVoteReq, WorldTeamReportVoteRes>(ServerType.GATE_SERVER, msgEvent, req, res, false, 20);
            //    if (msgEvent.IsAllMessageReceived())
            //    {
            //        Logger.LogErrorFormat("[WorldTeamReportVoteReq] result == {0}", res.result);
            //        if (res.result == 0)
            //        {
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        Logger.LogErrorFormat("[WorldTeamReportVoteReq] not AllMessageReceived");
            //    }
            //
            //} while (false);


            Logger.LogErrorFormat("[JoinTeam] leave");
            yield return null;

            yield return GameEnd();
        }

        System.Random rand = new System.Random();
  
        private const int TARGETMIN = 5;
        private const int TARGETMAX = 20;

        IEnumerator Setting()
        {
            //Logger.LogErrorFormat("[Setting] start");
            //var baseInfo = mTeamInfo.baseInfo;
            //
            //WorldTeamSettingReq req = MakeSetting(baseInfo.teamId, (uint)rand.Next(TARGETMIN,TARGETMAX+1) , baseInfo.minLevel, baseInfo.maxLevel, baseInfo.occus);
            //WorldTeamSettingRes res = new WorldTeamSettingRes();
            //MessageEvents msgEvent = new MessageEvents();
            //
            //
            //
            //Logger.LogErrorFormat("[JoinTeam] {0} {1} {2} {3} {4} {5} {6} {7}",
            //req.teamId,
            //req.isPublish,
            //req.name,
            //req.targetId,
            //req.minLevel,
            //req.maxLevel,
            //req.content,
            //req.isAutoAgree);
            //
            //
            //
            //yield return Tenmove.Runtime.Game.MessageUtility.Wait<WorldTeamSettingReq, WorldTeamSettingRes>(ServerType.GATE_SERVER, msgEvent, req, res, false, 20);
            //if (msgEvent.IsAllMessageReceived())
            //{
            //    Logger.LogErrorFormat("[Setting] result == {0}", res.result);
            //    if (res.result != 0)
            //    {
            //        yield break;
            //    }
            //}
            //else
            //{
            //    Logger.LogErrorFormat("[Setting] not AllMessageReceived", res.result);
            //    yield break;
            //}
            //Logger.LogErrorFormat("[Setting] end");
            //yield return null;
            yield return WaitingForStart();
        }




        //Action<MsgDATA> teamRequestSync = (MsgDATA data) =>
        //{
        //    WorldTeamRequesterListSync msgRet = new WorldTeamRequesterListSync();
        //    msgRet.decode(data.bytes);
        //    if (msgRet.infos == null)
        //    {
        //        return;
        //    }
        //
        //    if (msgRet.infos.Length == 0)
        //    {
        //        return;
        //    }
        //
        //    TeamDataManager.Instance.SendAgreeJoinTeamReq(msgRet.infos[0].id, TeamProcessRequesterType.TEAM_PROCESS_ACCEPT);
        //};

        IEnumerator WaitingForStart()
        {
            Logger.LogErrorFormat("[WaitingForStart] start");

            while (true)
            {
                int count = 0;
                //foreach (var info in mTeamInfo.memberInfos)
                //{
                //    if (info == null)
                //    {
                //        continue;
                //    }
                //    count += 1;
                //}

                if (count >= 1)
                {
                    yield return new WaitForSeconds(1);
                    break;
                }
                yield return new WaitForSeconds(1);
            }

            yield return new WaitForSeconds(1);

            Logger.LogErrorFormat("[WaitingForStart] end");
            yield return null;
            yield return StartGame();
        }


        //WorldTeamSettingReq MakeSetting(uint teamID, uint target, ushort minL, ushort maxL, UInt16[] occus)
        //{
        //
        //    Logger.LogErrorFormat("teamid {0}  target {1}", teamID, target);
        //
        //    WorldTeamSettingReq req = new WorldTeamSettingReq();
        //    req.teamId = teamID;
        //    req.targetId = target;
        //    req.minLevel = minL;
        //    req.maxLevel = maxL;
        //    req.name = "自动战斗";
        //    req.isAutoAgree = 1;
        //    req.occus = occus;
        //    req.content = "";
        //    req.isPublish = 1;
        //    return req;
        //}



        IEnumerator StartGame()
        {
            Logger.LogErrorFormat("[StartGame] Start");
            GameStartState = StartState.None;
            //WorldTeamReadyMatchReq req = new WorldTeamReadyMatchReq();
            //WorldTeamReadyMatchRes res = new WorldTeamReadyMatchRes();
            //MessageEvents msgEvent = new MessageEvents();
            //
            //yield return Tenmove.Runtime.Game.MessageUtility.Wait<WorldTeamReadyMatchReq, WorldTeamReadyMatchRes>(ServerType.GATE_SERVER, msgEvent, req, res, false, 20);
            //if (msgEvent.IsAllMessageReceived())
            //{
            //    Logger.LogErrorFormat("[StartGame] result == {0}", res.result);
            //    if (res.result != 0)
            //    {
            //        yield break;
            //    }
            //}
            //else
            //{
            //    Logger.LogErrorFormat("[StartGame] not AllMessageReceived", res.result);
            //    yield break;
            //}



            // 等待游戏开始命令

            while (GameStartState == StartState.None)
            {
                yield return null;
            }

            if (GameStartState == StartState.Fail)
            {
                yield break;
            }

            Logger.LogErrorFormat("[StartGame] end");
            yield return null;
            yield return new WaitForSeconds(1);
            yield return GameEnd();
        }

        enum StartState
        {
            None,
            Success,
            Fail,
        }

        StartState GameStartState = StartState.None;
        bool GameFinish = false;
        IEnumerator GameEnd()
        {
            Logger.LogErrorFormat("[GameEnd] enter");

            // 监听不同步
            // 监听游戏结束

            GameFinish = false;
            while (!GameFinish)
            {
                //BaseBattle battle = Tenmove.Runtime.Client.Unity.Environment.Instance.Battle;
                //if (battle == null)
                //{
                //    yield return null;
                //    continue;
                //}
                //
                //if (battle.IsEnd())
                //{
                //    break;
                //}
                //
                yield return null;
            }
            yield return new WaitForSeconds(2);

            intown = false;
            //DungeonUtility.SendReturn2Town();
            Logger.LogErrorFormat("[GameEnd] leave");
            yield return null;
            yield return GoToTown();
        }


        void OnRelaySvrNotifyUnsync(MsgDATA data)
        {
            IsRelaySvrNotifyUnsync = true;

            Finish();
            
            Logger.LogErrorFormat("[OnRelaySvrNotifyUnsync]");
            
            TryUploadCurrentBattleRecord();
        }

        public void TryUploadCurrentBattleRecord()
        {
            try
            {
                var mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerInfo;
                string who = string.Format("{0}_{1}", mainPlayer.accid, mainPlayer.roleId);
                string sessionID = (BattleMain.instance.GetBattle() as BaseBattle).recordServer.sessionID;
                PushUnsync(who, RecordData.GetRootPath(), sessionID);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        void OnWorldTeamStartRaceSync(MsgDATA data)
        {
            //WorldTeamStartRaceSync raceSync = new WorldTeamStartRaceSync();
            //raceSync.decode(data.bytes);
            //GameStartState = raceSync.result == 0 ? StartState.Success : StartState.Fail;
            //Logger.LogErrorFormat("[OnWorldTeamStartRaceSync] result {0}", raceSync.result);
        }

        public void OnSceneDungeonStartRes(MsgDATA data)
        {
            SceneDungeonStartRes raceSync = new SceneDungeonStartRes();
            raceSync.decode(data.bytes);
            GameStartState = raceSync.result == 0 ? StartState.Success : StartState.Fail;
            Logger.LogErrorFormat("[OnWorldTeamStartRaceSync] result {0}", raceSync.result);
        }


        void OnWorldTeamInfoSync(MsgDATA data)
        {
            //WorldTeamInfoSync infoSync = new WorldTeamInfoSync();
            //infoSync.decode(data.bytes);
            //mTeamInfo.baseInfo = infoSync.teamInfo;
        }


        void OnTeamMemberSync(MsgDATA a_msgData)
        {
            //Logger.LogError("[OnTeamMemberSync] enter");
            //WorldTeamMemberSync msgRet = new WorldTeamMemberSync();
            //msgRet.decode(a_msgData.bytes);
            //
            //if (mTeamInfo == null)
            //{
            //    Logger.LogError("[OnTeamMemberSync] MyTeamInfo is null, I has already quit team, but server still send team message to me...");
            //    return;
            //}
            //
            //if (mTeamInfo.memberInfos == null)
            //{
            //    Logger.LogError("[OnTeamMemberSync] MyTeamInfo.memberInfos is null.");
            //    return;
            //}
            //
            //bool bfind = false;
            //
            //
            //for (int i = 0; i < mTeamInfo.memberInfos.Length; i++)
            //{
            //    if (mTeamInfo.memberInfos[i] == null)
            //    {
            //        continue;
            //    }
            //    // 如果找到了，说明是组员信息更改
            //    if (mTeamInfo.memberInfos[i].playerBaseInfo.id == msgRet.member.playerBaseInfo.id)
            //    {
            //        mTeamInfo.memberInfos[i] = msgRet.member;
            //        bfind = true;
            //        break;
            //    }
            //}
            //// 如果没找到，说明是新加组员
            //if (!bfind)
            //{
            //    for (int i = 0; i < mTeamInfo.memberInfos.Length; i++)
            //    {
            //        // 将新组员放在空位置,客户端UI从0开始算，服务器从1开始的
            //        if (msgRet.member.index == (i + 1))
            //        {
            //            mTeamInfo.memberInfos[i] = msgRet.member;
            //            break;
            //        }
            //    }
            //}
            //Logger.LogError("[OnTeamMemberSync] leave");
        }

        void OnTeamVoteChoiceSync(MsgDATA a_msgData)
        {
            //WorldTeamVoteStartSync msgRet = new WorldTeamVoteStartSync();
            //msgRet.decode(a_msgData.bytes);
            //TeamDataManager.Instance.SendVoteResultReq(TeamVoteType.TEAM_VOTE_ACCEPT);
        }

        void OnWorldTeamSettingRes(MsgDATA a_msgData)
        {
            //WorldTeamSettingRes msgRet = new WorldTeamSettingRes();
            //msgRet.decode(a_msgData.bytes);
            //Logger.LogError("[OnWorldTeamSettingRes] OnWorldTeamSettingRes");
        }



        // void SetAutoFight(){
        //     player.playerActor.RegisterEvent(BeEventType.onBirth, (args) =>
        //         {

        //         });
        // }



        void OnEnterBattle(UIEvent uiEvent)
        {
            //var battle = Tenmove.Runtime.Client.Unity.Environment.Instance.Battle;
            //if (battle == null)
            //{
            //    return;
            //}
            //
            //var players = battle.DungeonPlayers.GetAllPlayers();
            //if (players == null)
            //{
            //    return;
            //}
            //
            //foreach (BattlePlayer player in players)
            //{
            //    player.playerActor.SetAutoFight(AutoFightState.Open);
            //}
        }

        public void Update()
        {
            unsyncChecker.UpdateUnsycnNode(Time.deltaTime);
        }

        public void PushUnsync(string uploader, string root, string sessionID)
        {
            unsyncChecker.PushUnsync(uploader, root, sessionID);
        }
        
        private UnsyncChecker unsyncChecker = new UnsyncChecker();
    }
    
#endif // ROBOT_TEST
    public class UnsyncChecker
    {
        #region Upload Unsync
        private List<UnsyncNode> AllUnSyncNode = new List<UnsyncNode>();

        public void PushUnsync(string uploader, string root, string sessionID)
        {
            AllUnSyncNode.Add(new UnsyncNode(uploader, root, sessionID));
        }

        private float TickTime = 0;
        public void UpdateUnsycnNode(float dt)
        {
            if (TickTime > 0)
            {
                TickTime -= dt;
                return;
            }

            TickTime = 10;

            if (AllUnSyncNode.Count <= 0)
            {
                return;
            }

            bool hasDone = false;
            foreach (var cur in AllUnSyncNode)
            {
                if (cur.IsDone)
                {
                    hasDone = true;
                }

                if (cur.IsReadyToUpload)
                {
                    cur.TryUpload();
                }
                else
                {
                    Logger.LogErrorFormat("{0} with {1} not ready", cur.UploaderID, cur.SessionID);
                }
            }

            if (hasDone)
            {
                AllUnSyncNode.RemoveAll(x => x.IsDone);
            }
        }
        
        
        private class UnsyncNode
        {
            public UnsyncNode(string uploader, string root, string id)
            {
                UploaderID = uploader;
                Root = root;
                SessionID =id;
                IsDone = false;
            }

            public void TryUpload()
            {
                try
                {
                    Logger.LogErrorFormat("TryUpload {0}:{1}/{2}", UploaderID, Root, SessionID);
                    HttpUtilty.UploadFile(UploaderID, Root, SessionID);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
                
                IsDone = true;
            }

            public bool IsReadyToUpload
            {
                get
                {
                    var files =  Directory.GetFiles(Root, string.Format("*{0}*", SessionID));
                    return files.Length > 1;
                }
            }
            
            public bool IsDone { get; private set; }
            public string SessionID { get; private set; }
            public string Root { get; private set; }
            public string UploaderID { get; private set; }
        }
        #endregion
    }
    
    public class HttpUtilty
    {
        private static string skFileServerAddress = "http://192.168.3.198:3000/";

        public static string SetFileServerAddress
        {
            get => skFileServerAddress;
            set => skFileServerAddress = value;
        }


        public static string skLogicServerUploader = "logicserver";
        public static string skClientReplayUploader = "clientreplay";
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploaderID">上报者ID</param>
        /// <param name="root">根目录</param>
        /// <param name="sessionID">比赛ID</param>
        public static void UploadFile(string uploaderID, string root, string sessionID)
        {
            if (string.IsNullOrEmpty(skFileServerAddress))
            {
                Logger.LogErrorFormat("skFileServerAddress is nil");
                return;
            }
            
            string dllversion = VersionManager.instance.DllVersion.ToString();
            string uploadUrl = string.Format("{0}upload?session={1}&version={2}&uploader={3}", skFileServerAddress, sessionID, dllversion, uploaderID);
            
            Logger.LogErrorFormat("start upload file with url {0}", uploadUrl);
            
            UploadTypeFile(root, string.Format("*{0}*.zip", sessionID), uploadUrl,string.Format("{0}.{1}.zip", sessionID, uploaderID) );
            UploadTypeFile(root, sessionID, uploadUrl, string.Format("{0}.{1}", sessionID, uploaderID) );
            UploadTypeFile(root, string.Format("*{0}*process.txt", sessionID), uploadUrl,string.Format("{0}.{1}.process", sessionID, uploaderID) );
            UploadTypeFile(root, string.Format("*{0}*.mark", sessionID), uploadUrl,string.Format("{0}.{1}.mark", sessionID, uploaderID) );
            UploadTypeFile(root, string.Format("*{0}*process_receive_package.txt", sessionID), uploadUrl,string.Format("{0}.{1}.rcvpkg", sessionID, uploaderID) );

            Logger.LogError("[Unsync] end upload log");
        }

        public class UploadObj
        {
            public WebRequest webRequest;
            public byte[] header;
            public byte[] bodyer;
            public byte[] footer;
        }

        public static void UploadTypeFile(string root, string search, string remoteRoot, string remoteFileName)
        {
            string[] files = Directory.GetFiles(root, search);
            if (files.Length > 0)
            {
                Upload(files[0], remoteRoot, remoteFileName);
            }

            if (files.Length > 1)
            {
                Logger.LogErrorFormat("[Unsync] typefile find mutiflie {0}", string.Join(",", files));
            }
        }
        
        public static void Upload(string localFilePath, string remoteRoot, string remoteFileName)
        {
            if (!File.Exists(localFilePath))
            {
                Logger.LogErrorFormat("[Unsync] FileNotExist {0}, with remoteRoot:{1} remoteFileName:{2}", localFilePath, remoteRoot, remoteFileName);
                return;
            }
            
            Logger.LogErrorFormat("[Unsync] new httpupload {0}", localFilePath);
            byte[] replay = File.ReadAllBytes(localFilePath);
            UploadEx(remoteRoot, remoteFileName, replay);
        }
        

        static void UploadEx(string url, string name, byte[] dataBuffer)
        {
            //Logger.LogErrorFormat("[Unsync] new HttpWebRequest");
            Type type = typeof(HttpWebRequest);
            Type[] types = new Type[] { typeof(Uri) };
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, types, null);
            HttpWebRequest webRequest = (HttpWebRequest)constructor.Invoke(new System.Object[] { new Uri(url) });
            webRequest.Method = "POST";
            
            //Logger.LogErrorFormat("[Unsync] new HttpWebRequest end");
            
            string boundary = string.Format("----TengmuFormBoundary{0}", DateTime.Now.Ticks.ToString("x"));
            webRequest.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            string formDataStr = string.Format("\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" + "\r\nContent-Type: application/octet-stream\r\nContent-Transfer-Encoding: binary" + "\r\n\r\n", "file", name);
            byte[] header = System.Text.Encoding.Default.GetBytes(formDataStr);
            byte[] footer = System.Text.Encoding.Default.GetBytes("\r\n--" + boundary + "--\r\n");

            webRequest.ContentLength = dataBuffer.Length + header.Length + footer.Length;

            UploadObj obj = new UploadObj();
            obj.webRequest = webRequest;
            obj.header = header;
            obj.bodyer = dataBuffer;
            obj.footer = footer;
            
            //Logger.LogErrorFormat("[Unsync] start stream");
            
            webRequest.BeginGetRequestStream((IAsyncResult ar) =>
            {
                try
                {
                    //Logger.LogError("[Unsync] end stream");
                    UploadObj uploadobj = ar.AsyncState as UploadObj;
                    if (uploadobj == null)
                    {
                        Logger.LogError("[Unsync] uploadobj null");
                        return;
                    }
                    using (Stream upstream = uploadobj.webRequest.EndGetRequestStream(ar))
                    {
                        if (upstream == null)
                        {
                            Logger.LogError("[Unsync] upstream null");
                            return;
                        }
                        Logger.LogError("[Unsync] write stream");
                        upstream.Write(uploadobj.header, 0, uploadobj.header.Length);
                        upstream.Write(uploadobj.bodyer, 0, uploadobj.bodyer.Length);
                        upstream.Write(uploadobj.footer, 0, uploadobj.footer.Length);
                        upstream.Close();
                    }
                    //Logger.LogError("[Unsync] response");
                    using (WebResponse responce = uploadobj.webRequest.GetResponse())
                    {
                        Logger.LogError("[Unsync] response " + responce.ToString());
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError("[Unsync] " + e.ToString());
                }
            }, obj);
        }
    }
}

