using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
///////删除linq
using ProtoTable;

namespace GameClient
{

    public enum eSceneChangeType
    {
        eNone,
        eDungeonChapterSelect,
    }

    public enum ESceneActorType
    {
        Invalid = -1,
        Npc,
        AttackCityMonster,
        BlackMarketMerchanNpc,//黑市商人
    }

    enum EEnterGameState
    {
        Invalid,
        TimeOut,
        LoginInError,
        Success,
    }

    public class SceneParams
    {
        public delegate void OnSceneLoadFinish();
        public int currSceneID = 0;
        public int currDoorID = 0;
        public int targetSceneID = 0;
        public int targetDoorID = 0;
        public OnSceneLoadFinish onSceneLoadFinish = null;
        public eSceneChangeType type = eSceneChangeType.eNone;
        public Vector3 birthPostion;
        public int iParam0 = 0;
        public int iParam1 = 0;

        public SceneParams()
        {

        }

        public SceneParams(int currentSceneId, int currentDoorId, int targetSceneId, int targetDoorId)
        {
            this.currSceneID = currentSceneId;
            this.currDoorID = currentDoorId;
            this.targetSceneID = targetSceneId;
            this.targetDoorID = targetDoorId;
        }

    }

    //场景中门数据
    public class DoorNode
    {
        public int TargetSceneIndex;
        public ISceneTownDoorData Door;
    }

    //场景数据
    public class SceneNode
    {
        public int SceneID;
        public List<DoorNode> DoorNodes;
        public bool HasVisited;
    }


    class ClientSystemTown : ClientSystem
    {
        public static int ChangeJobLevel = 15;
        public static int Awakelevel = 45;

        public static bool ChangeJobBegin = false;

        public static bool AwakeBegin = false;
        public static bool AwakeEnd = false;


        public ClientSystemTown()
        {
            _InitSceneNodeGraph();
            _InitDungeonMap();
        }

        protected sealed override string _GetLevelName()
        {
            return "Town";
        }

        //protected override void _OnDisconnect(ServerType type)
        //{
        //    switch (type)
        //    {
        //        case ServerType.GATE_SERVER:
        //            GameFrameWork.instance.StartCoroutine(ClientSystemManager.instance.ReconnectGateServer());
        //            break;
        //        default:
        //            OnReconnect();
        //            break;
        //    }
        //}

        protected void _BindUIEvent()
        {
            
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayChangeJobEffect, _OnPlayChangeJobEffect);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JobIDChanged, _OnJobIDChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PopChatMsg, _OnOpopChatMsg);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PKRankChanged, _OnPkRankChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetChanged, _OnPetChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NameChanged, _OnNameChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildTownStatueUpdate, _OnGuildTownStatueUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildGuardStatueUpdate, _OnGuildGuardStatueUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonUpdateActivityData, _OnGuildDungeonUpdateActivityData);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonSyncActivityState, _OnGuildDungeonUpdateActivityData);
        }

        protected void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayChangeJobEffect, _OnPlayChangeJobEffect);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JobIDChanged, _OnJobIDChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PopChatMsg, _OnOpopChatMsg);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PKRankChanged, _OnPkRankChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetChanged, _OnPetChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NameChanged, _OnNameChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildTownStatueUpdate, _OnGuildTownStatueUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildGuardStatueUpdate, _OnGuildGuardStatueUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonUpdateActivityData, _OnGuildDungeonUpdateActivityData);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonSyncActivityState, _OnGuildDungeonUpdateActivityData);
        }

        void _OnJobIDChanged(UIEvent iEvent)
        {
            DestroyMainPlayer();
            CreateMainPlayer();
            _InitializeCameraController();
        }

        //显示转职特效
        void _OnPlayChangeJobEffect(UIEvent iEvent)
        {
            int effectInfoId = 1039;
            var pos = _beTownPlayerMain.GeActor.GetPosition();
            _beTownPlayerMain.GeActor.CreateEffect(effectInfoId, new Vec3(pos.x, pos.z, pos.y), true);
        }

        void _OnOpopChatMsg(UIEvent uiEvent)
        {
            if (null == (uiEvent.Param1 as ChatBlock))
            {
                return;
            }

            ChatData data = (uiEvent.Param1 as ChatBlock).chatData;
            if (data != null && !string.IsNullOrEmpty(data.word))
            {
                if (_beTownPlayerMain != null && (_beTownPlayerMain.ActorData as BeTownPlayerData).GUID == data.objid)
                {
                    _ExecutePopChatMsg(_beTownPlayerMain, data.word, data.bLink == 1);
                    return;
                }

                if (_beTownPlayers.ContainsKey(data.objid))
                {
                    _ExecutePopChatMsg(_beTownPlayers[data.objid], data.word, data.bLink == 1);
                }
            }
        }

        void _OnPkRankChanged(UIEvent a_event)
        {
            if (MainPlayer != null)
            {
                MainPlayer.SetPlayerPKRank(SeasonDataManager.GetInstance().seasonLevel, SeasonDataManager.GetInstance().seasonStar);
            }
        }

        void _OnNameChanged(UIEvent a_event)
        {
            if (MainPlayer != null)
            {
                MainPlayer.SetPlayerName(PlayerBaseData.GetInstance().Name);
            }
        }

        void _OnGuildTownStatueUpdate(UIEvent a_event)
        {
            if (_levelData == null)
            {
                return;
            }

            for (int i = 0; i < _levelData.GetNpcInfoLength(); ++i)
            {
                ISceneNPCInfoData info = _levelData.GetNpcInfo(i);

                if (info == null)
                {
                    continue;
                }

                NpcTable npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(info.GetEntityInfo().GetResid());
                if (npcItem == null)
                {
                    continue;
                }

                if (npcItem.Function != NpcTable.eFunction.Townstatue)
                {
                    continue;
                }

                BeTownNPCData data = new BeTownNPCData
                {
                    NpcID = info.GetEntityInfo().GetResid(),
                    ResourceID = 0
                };

                int RotationY = 0;

                List<FigureStatueInfo> TownStatueInfoList = GuildDataManager.GetInstance().GetTownStatueInfo();

                bool bFind = false;
                for (int j = 0; j < TownStatueInfoList.Count; j++)
                {
                    FigureStatueInfo TownStatueInfo = TownStatueInfoList[j];

                    if (TownStatueInfo.statueType != (byte)npcItem.SubType)
                    {
                        continue;
                    }

                    JobTable job = TableManager.instance.GetTableItem<JobTable>(TownStatueInfo.occu);
                    if (job == null)
                    {
                        Logger.LogErrorFormat("can not find JobTable with TownStatue NPC occu ID:{0} when InitTown", TownStatueInfo.occu);
                        continue;
                    }
                    else
                    {
                        ResTable res = TableManager.instance.GetTableItem<ResTable>(job.Mode);

                        if (res == null)
                        {
                            Logger.LogErrorFormat("can not find ResTable with TownStatue NPC mod id:{0} when InitTown", job.Mode);
                            continue;
                        }
                        else
                        {
                            data.ResourceID = res.ID;
                            data.JobID = TownStatueInfo.occu;
                            data.avatorInfo = TownStatueInfo.avatar;
                            data.StatueName = TownStatueInfo.name;

                            RotationY = job.TownStatueFace;
                        }
                    }

                    bFind = true;
                    break;
                }

                if (!bFind)
                {
                    data.ResourceID = npcItem.ResID;
                }

                data.MoveData.PosLogicToGraph = _logicToGraph;
                data.MoveData.PosServerToClient = _serverToClient;
                data.MoveData.Position = info.GetEntityInfo().GetPosition();
                data.Name = npcItem.NpcName;
                data.NameColor = PlayerInfoColor.TOWN_NPC;

                BeTownNPC beTownNPC = new BeTownNPC(data, this);

                beTownNPC.InitGeActor(_geScene);
                if (beTownNPC.GraphicActor != null)
                {
                    beTownNPC.GraphicActor.SetScale(info.GetEntityInfo().GetScale());

                    Quaternion newRotation = Quaternion.Euler(0, RotationY, 0);
                    beTownNPC.GraphicActor.SetRotation(newRotation);
                }

                beTownNPC.AddActorPostLoadCommand(() =>
                {
                    OnNPCLoadFinished(data.NpcID, beTownNPC.GraphicActor);
                });

                for (int j = 0; j < _beTownNPCs.Count; j++)
                {
                    BeTownNPC npc = _beTownNPCs[j];
                    if (npc == null)
                    {
                        continue;
                    }

                    BeTownNPCData Npcdata = npc.ActorData as BeTownNPCData;
                    if (Npcdata == null)
                    {
                        continue;
                    }

                    if (Npcdata.NpcID == info.GetEntityInfo().GetResid())
                    {
                        npc.Dispose();
                        _beTownNPCs.RemoveAt(j);

                        break;
                    }
                }

                _beTownNPCs.Add(beTownNPC);
            }
        }

        void _OnGuildGuardStatueUpdate(UIEvent a_event)
        {
            if (_levelData == null)
            {
                return;
            }
            for (int i = 0; i < _levelData.GetNpcInfoLength(); ++i)
            {
                ISceneNPCInfoData info = _levelData.GetNpcInfo(i);
                if (info == null)
                {
                    continue;
                }
                NpcTable npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(info.GetEntityInfo().GetResid());
                if (npcItem == null)
                {
                    continue;
                }
                if (npcItem.Function != NpcTable.eFunction.guildGuardStatue)
                {
                    continue;
                }
                BeTownNPCData data = new BeTownNPCData
                {
                    NpcID = info.GetEntityInfo().GetResid(),
                    ResourceID = 0
                };
                int RotationY = 0;
                List<FigureStatueInfo> GuildGuardStatueInfoList = GuildDataManager.GetInstance().GetGuildGuardStatueInfo();
                bool bFind = false;
                for (int j = 0; j < GuildGuardStatueInfoList.Count; j++)
                {
                    FigureStatueInfo TownStatueInfo = GuildGuardStatueInfoList[j];
                    if (TownStatueInfo.statueType != (byte)npcItem.SubType)
                    {
                        continue;
                    }
                    JobTable job = TableManager.instance.GetTableItem<JobTable>(TownStatueInfo.occu);
                    if (job == null)
                    {
                        Logger.LogErrorFormat("can not find JobTable with TownStatue NPC occu ID:{0} when InitTown", TownStatueInfo.occu);
                        continue;
                    }
                    else
                    {
                        ResTable res = TableManager.instance.GetTableItem<ResTable>(job.Mode);
                        if (res == null)
                        {
                            Logger.LogErrorFormat("can not find ResTable with TownStatue NPC mod id:{0} when InitTown", job.Mode);
                            continue;
                        }
                        else
                        {
                            data.ResourceID = res.ID;
                            data.JobID = TownStatueInfo.occu;
                            data.avatorInfo = TownStatueInfo.avatar;
                            data.StatueName = TownStatueInfo.name;
                            RotationY = job.TownStatueFace;
                        }
                    }
                    bFind = true;
                    break;
                }
                if (!bFind)
                {
                    data.ResourceID = npcItem.ResID;
                }
                data.MoveData.PosLogicToGraph = _logicToGraph;
                data.MoveData.PosServerToClient = _serverToClient;
                data.MoveData.Position = info.GetEntityInfo().GetPosition();
                data.Name = npcItem.NpcName;
                data.NameColor = PlayerInfoColor.TOWN_NPC;
                BeTownNPC beTownNPC = new BeTownNPC(data, this);
                beTownNPC.InitGeActor(_geScene);
                if (beTownNPC.GraphicActor != null)
                {
                    beTownNPC.GraphicActor.SetScale(info.GetEntityInfo().GetScale());
                    Quaternion newRotation = Quaternion.Euler(0, RotationY, 0);
                    beTownNPC.GraphicActor.SetRotation(newRotation);
                }
                beTownNPC.AddActorPostLoadCommand(() =>
                {
                    OnNPCLoadFinished(data.NpcID, beTownNPC.GraphicActor);
                });
                for (int j = 0; j < _beTownNPCs.Count; j++)
                {
                    BeTownNPC npc = _beTownNPCs[j];
                    if (npc == null)
                    {
                        continue;
                    }
                    BeTownNPCData Npcdata = npc.ActorData as BeTownNPCData;
                    if (Npcdata == null)
                    {
                        continue;
                    }
                    if (Npcdata.NpcID == info.GetEntityInfo().GetResid())
                    {
                        npc.Dispose();
                        _beTownNPCs.RemoveAt(j);
                        break;
                    }
                }
                _beTownNPCs.Add(beTownNPC);
            }
        }
        void _OnGuildDungeonUpdateActivityData(UIEvent a_event)
        {
            bool bShowChest = GuildDataManager.GetInstance().IsShowChestModel();      
            if (_levelData == null)
            {
                return;
            }
            for (int i = 0; i < _levelData.GetNpcInfoLength(); ++i)
            {
                ISceneNPCInfoData info = _levelData.GetNpcInfo(i);
                if (info == null)
                {
                    continue;
                }
                NpcTable npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(info.GetEntityInfo().GetResid());
                if (npcItem == null)
                {
                    continue;
                }
                if (npcItem.Function != NpcTable.eFunction.guildDungeonActivityChest)
                {
                    continue;
                }
                if(!bShowChest)
                {
                    for (int j = 0; j < _beTownNPCs.Count; j++)
                    {
                        BeTownNPC npc = _beTownNPCs[j];
                        if (npc == null)
                        {
                            continue;
                        }
                        BeTownNPCData Npcdata = npc.ActorData as BeTownNPCData;
                        if (Npcdata == null)
                        {
                            continue;
                        }
                        if (Npcdata.NpcID == info.GetEntityInfo().GetResid())
                        {
                            npc.Dispose();
                            _beTownNPCs.RemoveAt(j);
                            break;
                        }
                    }
                    break;
                }
                else
                {
                    for (int j = 0; j < _beTownNPCs.Count; j++)
                    {
                        BeTownNPC npc = _beTownNPCs[j];
                        if (npc == null)
                        {
                            continue;
                        }
                        BeTownNPCData Npcdata = npc.ActorData as BeTownNPCData;
                        if (Npcdata == null)
                        {
                            continue;
                        }
                        if (Npcdata.NpcID == info.GetEntityInfo().GetResid())
                        {
                            return;
                        }
                    }
                }                
                BeTownNPCData data = new BeTownNPCData
                {
                    NpcID = info.GetEntityInfo().GetResid(),
                    ResourceID = 0
                };
                int RotationY = 0;
                bool bFind = false;
                if (!bFind)
                {
                    data.ResourceID = npcItem.ResID;
                }
                data.MoveData.PosLogicToGraph = _logicToGraph;
                data.MoveData.PosServerToClient = _serverToClient;
                data.MoveData.Position = info.GetEntityInfo().GetPosition();
                data.Name = npcItem.NpcName;
                data.NameColor = PlayerInfoColor.TOWN_NPC;
                BeTownNPC beTownNPC = new BeTownNPC(data, this);
                beTownNPC.InitGeActor(_geScene);
                if (beTownNPC.GraphicActor != null)
                {
                    beTownNPC.GraphicActor.SetScale(info.GetEntityInfo().GetScale());
                    Quaternion newRotation = Quaternion.Euler(0, RotationY, 0);
                    beTownNPC.GraphicActor.SetRotation(newRotation);
                }
                beTownNPC.AddActorPostLoadCommand(() =>
                {
                    OnNPCLoadFinished(data.NpcID, beTownNPC.GraphicActor);
                });               
                _beTownNPCs.Add(beTownNPC);
            }
        }
        void _OnPetChanged(UIEvent a_event)
        {
            if (MainPlayer != null)
            {
                uint id = (uint)a_event.Param1;
                MainPlayer.CreatePet((int)id);
            }
        }

        void _ExecutePopChatMsg(BeTownPlayer actor, string words, bool bLink)
        {
            if (actor != null)
            {
                if (null != actor.GraphicActor)
                {
                    actor.GraphicActor.ShowHeadDialog(words, false, bLink, true, false, 5.0f);
                }
            }
        }

        #region SystemInit

        public sealed override void GetEnterCoroutine(AddCoroutine enter)
        {
            enter(_baseSystemLoadingCoroutine);
            enter(TownLoadingCoroutine);
        }

        public sealed override void OnBeforeEnter()
        {
            ClientReconnectManager.instance.canReconnectGate = true;
            //分包判定顺序调整
            GeAvatarFallback.EnableGlobalAvatarPartFallback = true;
        }

        public sealed override void OnEnter()
        {
            UWAProfilerUtility.Mark("[tm]SysTown_OnEnter");

            if (Global.Settings.displayHUD)
                HUDInfoViewer.instance.Init();
            base.OnEnter();
#if !MG_TEST_EXTENT
            NetManager.instance.ClearReSendData();
#endif

#if MG_TEST_EXTENT
            NetManager.instance.ResetResend();
#endif
            AssetGabageCollectorHelper.instance.SetGCPurgeEnable(AssetGCTickType.Asset, true);
            AssetGabageCollectorHelper.instance.SetGCPurgeEnable(AssetGCTickType.SceneActor, true);
            AssetGabageCollectorHelper.instance.SetGCPurgeEnable(AssetGCTickType.UIFrame, true);

            TryOpenTeamPanel();
            CameraAspectAdjust.MarkDirty();

            AssetLoader.instance.SetAsyncLoadStep(1);
            

            _resetIsSwiching();

#if STAT_EXTRA_INFO 
            SDKCallback.instance.SetNeedCheckTempure(true);
#endif

            UWAProfilerUtility.DoDump();
        }

        void TryOpenTeamPanel()
        {
            if (ClientSystemBattle.bNeedOpenTeamFrame)
            {
                if (NewbieGuideManager.instance.IsGuiding() == false)
                {
                    if (TeamDataManager.GetInstance().HasTeam())
                    {
                        TeamListFrame.TryOpenTeamListOrTeamMyFrame();
                        //ClientSystemManager.instance.OpenFrame<TeamListFrame>(FrameLayer.Middle);
                    }
                }
            }

            ClientSystemBattle.bNeedOpenTeamFrame = false;
        }

        protected void StopBGM()
        {
            if (uint.MaxValue != m_BgmHandle)
            {
                AudioManager.instance.Stop(m_BgmHandle);
                m_BgmHandle = uint.MaxValue;
            }
                
            if (uint.MaxValue != m_EnvGgmHandle)
            {
                AudioManager.instance.Stop(m_EnvGgmHandle);
                m_EnvGgmHandle = uint.MaxValue;
            }
                   
        }

        public sealed override void OnExit()
        {
            UWAProfilerUtility.Mark("[tm]SysTown_OnExit");

            OnUnBindExtraSceneNetMessages();
            _ClearLoadPetList();
            _resetIsSwiching();

            StopBGM();


            //_unLockFrames();

            AssetGabageCollectorHelper.instance.SetGCPurgeEnable(AssetGCTickType.Asset, false);
            AssetGabageCollectorHelper.instance.SetGCPurgeEnable(AssetGCTickType.SceneActor, false);
            AssetGabageCollectorHelper.instance.SetGCPurgeEnable(AssetGCTickType.UIFrame, false);
            ClearScene();
            BeTownPlayerMain.CommandStopAutoMove();

            MissionManager.GetInstance().onAddNewMission -= _OnAddTask;
            _UnBindUIEvent();
            _ClearData();
            ClientSystemManager.instance.CloseFrame<ClientSystemTownFrame>();

            //town状态退出，清空飘字
            SystemNotifyManager.Clear();

            GeAvatarFallback.EnableGlobalAvatarPartFallback = false;

            AssetLoader.instance.SetAsyncLoadStep(4);
            base.OnExit();
        }

        void _ClearData()
        {
            ChangeJobBegin = false;
            AwakeBegin = false;
            AwakeEnd = false;
        }

        protected IEnumerator TownLoadingCoroutine(IASyncOperation systemOperation)
        {
            Logger.LogProcessFormat("return to town Loading...this is not error, just a log of checking process");

            Time.timeScale = 1.0f;

            StopBGM();

            ClientSystemManager.instance.delayCaller.Clear();

            ClientSystemManager.instance.OpenFrame<ClientSystemTownFrame>(FrameLayer.Bottom);

            if (SystemManager.CurrentSystem == null)
            {
                yield return _SystemInitWithoutNet(systemOperation);
                //GameFrameWork.instance.StartCoroutine(_SystemInitWithoutNet(systemOperation));
            }
            else
            {
                ClientSystemManager.GetInstance().OnSwitchSystemFinished.RemoveAllListeners();
                //ClientSystemManager.GetInstance().OnSwitchSystemFinished.AddListener(_OnMailListReq);
                //ClientSystemManager.GetInstance().OnSwitchSystemFinished.AddListener(_OpenChangeJobTip);
                ClientSystemManager.GetInstance().OnSwitchSystemFinished.AddListener(_NextFuncOpen);
                ClientSystemManager.GetInstance().OnSwitchSystemFinished.AddListener(() =>
                {
                    DevelopGuidanceManager.GetInstance().TryOpenGuidanceEntranceFrame();
                });

                _BindUIEvent();

                MissionManager.GetInstance().onAddNewMission += _OnAddTask;

                MissionManager.GetInstance().AddSystemInvoke();
                ClientSystemManager.GetInstance().OnSwitchSystemFinished.AddListener(OnSceneLoadEnd);
                ItemDataManager.GetInstance().AddSystemInvoke();
                TAPNewDataManager.GetInstance().AddSystemInvoke();

                Type systemType = SystemManager.CurrentSystem.GetType();
                if (systemType == typeof(ClientSystemLogin))
                {
                    yield return _SystemInitEnterGame(systemOperation);
                }
                else if (systemType == typeof(ClientSystemBattle))
                {
                    yield return _SystemInitReturnToTown(systemOperation);
                }
                else if(systemType == typeof(ClientSystemGameBattle))
                {
                    ChijiDataManager.GetInstance().ClearPrepareSceneData();
                    if (ManualPoolCollector.instance != null)
                    {
                        ManualPoolCollector.instance.Clear();
                    }

#if UNITY_EDITOR
                    Logger.LogErrorFormat("吃鸡时序测试----吃鸡准备场景[开始]切换返回城镇，清理吃鸡准备场景数据");
#endif

                    yield return _SystemInitReturnToTown(systemOperation);
                }
            }

            //城镇预加载
            yield return ClientSystemManager._PreloadRes(systemOperation);

            /// 使用预加载界面
            //if (EngineConfig.usePrewarmFrame)
            //{
            //    yield return _PrewarmFrames(systemOperation);
            //}


#if !LOGIC_SERVER
            RecordServer.GetInstance().Clear();
#endif

            ReplayServer.GetInstance().Clear();
            //最后播背景音乐
            CitySceneTable targetCityTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(CurrentSceneID);
            if (targetCityTable != null)
            {
                _InitializeBGM(targetCityTable.BGMPath, targetCityTable.EnvironmenBGMtPath);
            }
        }

        public void OnSceneLoadEnd()
        {
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SceneLoadFinish);

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame townFrame = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                townFrame.SceneLoadFinish();
            }

            //GameStatisticManager.GetInstance().DoStatClientData();
        }

        public static void _OpenChangeJobTip()
        {
            if (!Utility.CheckCanChangeJob())
            {
                return;
            }

            if (ChangeJobBegin)
            {
                return;
            }

            /*
            if (ClientSystemManager.GetInstance().IsFrameOpen<ChangeJobBegin>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ChangeJobBegin>();
            }

            ChangeJobBegin ChangeJobBeginFrame = ClientSystemManager.GetInstance().OpenFrame<ChangeJobBegin>(FrameLayer.Middle) as ChangeJobBegin;
            ChangeJobBeginFrame.AddListener(_BeginChangeJobDialog);
            */

            _BeginChangeJobDialog();
            ChangeJobBegin = true;
        }

        public static void _OpenAwakeTip()
        {
            if (!Utility.CheckCanAwake())
            {
                return;
            }

            if (AwakeBegin)
            {
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<OpenAwakeFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<OpenAwakeFrame>();
            }

            OpenAwakeFrame awakeFrame = ClientSystemManager.GetInstance().OpenFrame<OpenAwakeFrame>(FrameLayer.Middle) as OpenAwakeFrame;
            awakeFrame.AddListener(_BeginAwakeDialog);

            AwakeBegin = true;
        }

        public static void _OnAddTask(UInt32 iMissionId)
        {
            //             if (Utility.IsAwakeTask(iMissionID))
            //             {
            //                 _OpenAwakeTip();
            //             }
        }

        public static void _NextFuncOpen()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NextFuncOpen);
        }

        protected IEnumerator _SystemInitWithoutNet(IASyncOperation systemOperation)
        {
#region process data
            {
                PlayerBaseData.GetInstance().RoleID = 0;
                PlayerBaseData.GetInstance().Name = "PlayerMain";
                PlayerBaseData.GetInstance().JobTableID = (int)(ActorOccupation.SwordMan);

                IsNet = false;
            }

            systemOperation.SetProgress(0.7f);
            yield return Yielders.EndOfFrame;
#endregion

            InitializeScene(2233);
            //_InitializeMainUI(); TODO

            systemOperation.SetProgress(1.0f);

            yield return Yielders.EndOfFrame;
        }

#region InitEnterGame
        private IEnumerator _systemInitEnterGameProcess(IASyncOperation systemOperation, bool initwithNewbieGuideBattle = false)
        {
#region net
            //登录前初始化所有系统
            PlayerDataManager.GetInstance().InitiallizeSystem();
            GateEnterGameReq enterGame = new GateEnterGameReq
            {
                roleId = ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin().roleId,
                city = "",
                inviter = 0,
                option = 0
            };
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, enterGame);

            // 0:非法 1:超时 2:登录结果错误 3:正常
            EEnterGameState eEnterState = EEnterGameState.Invalid;
            WaitNetMessageManager.NetMessages netMsgs = null;

            List<uint> arrWaitMsgIDs = new List<uint>
            {
                SceneNotifyEnterScene.MsgID,
                SceneSyncSceneObject.MsgID,
                GateEnterGameRet.MsgID
            };
            arrWaitMsgIDs.AddRange(EEnterGameWaitMsg.msgs);

            PlayerDataManager.GetInstance().BindEnterGameMsg(arrWaitMsgIDs);
            WaitNetMessageManager.WaitMulti wait = WaitNetMessageManager.GetInstance().Wait(arrWaitMsgIDs.ToArray(), msgRets =>
            {
                GateEnterGameRet ret = new GateEnterGameRet();
                ret.decode(msgRets.GetMessageData(GateEnterGameRet.MsgID).bytes);
                if (ret.result != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)ret.result);
                    eEnterState = EEnterGameState.LoginInError;
                    return;
                }
                //登录后，处理登录消息
                PlayerDataManager.GetInstance().ProcessInitNetMessage(msgRets);
                ChapterChange.Init();
                netMsgs = msgRets;
                eEnterState = EEnterGameState.Success;
                PlayerBaseData.GetInstance().IsRoleEnterGame = true;
            },
            true,
            120,
            () =>
            {
                eEnterState = EEnterGameState.TimeOut;
            }
            );

            while (eEnterState == EEnterGameState.Invalid)
            {
                yield return Yielders.EndOfFrame;
            }

            if (!initwithNewbieGuideBattle)
                systemOperation.SetProgress(0.6f);
#endregion

#region process data
            if (eEnterState == EEnterGameState.TimeOut)
            {
                systemOperation.SetError("[TownEnterGame] 预先消息列表各种消息超时");

                Logger.LogErrorFormat(wait.m_netMessage.GetUnReceivedMessageDesc());

                yield return new NormalCustomEnumError("[TownEnterGame] 预先消息列表各种消息超时", eEnumError.NetworkErrorDisconnect);
                yield break;
            }

            if (eEnterState == EEnterGameState.LoginInError)
            {
                systemOperation.SetError("");
                yield return new NormalCustomEnumError("[TownEnterGame] 进入游戏等待消息结果错误");
                yield break;
            }

            IsNet = true;

            if (!initwithNewbieGuideBattle)
                systemOperation.SetProgress(0.7f);
            yield return Yielders.EndOfFrame;

#endregion


            //if (

            // 初始化场景      
            SceneNotifyEnterScene msgEnterScene = new SceneNotifyEnterScene();

            msgEnterScene.decode(netMsgs.GetMessageData(SceneNotifyEnterScene.MsgID).bytes);

            if (msgEnterScene.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgEnterScene.result);
                yield break;
            }

            if (msgEnterScene.mapid == 0)
            {
                yield return new NormalCustomEnumError("[TownEnterGame] mapID无效");
                yield break;
            }

            PlayerBaseData.GetInstance().Pos = new Vector3(
                 msgEnterScene.pos.x / PlayerBaseData.GetInstance().AxisScale,
                 0.0f,
                 msgEnterScene.pos.y / PlayerBaseData.GetInstance().AxisScale
                 );
            _InitOtherPlayerData(netMsgs.GetMessageData(SceneSyncSceneObject.MsgID), msgEnterScene.mapid);
            InitializeScene((int)msgEnterScene.mapid);

            //登陆
            SDKInterface.Instance.UpdateRoleInfo(
                1, ClientApplication.adminServer.id, ClientApplication.adminServer.name,
                PlayerBaseData.GetInstance().RoleID.ToString(),
                PlayerBaseData.GetInstance().Name,
                PlayerBaseData.GetInstance().JobTableID, PlayerBaseData.GetInstance().Level, PlayerBaseData.GetInstance().VipLevel,
                (int)PlayerBaseData.GetInstance().Ticket);

            yield return Yielders.EndOfFrame;
        }

        protected IEnumerator _errorProcess(eEnumError type, string msg)
        {
            Logger.LogErrorFormat("城镇错误 {0}, {1}", type, msg);
            ClientSystemManager.instance.QuitToLoginSystem(8501);
            yield break;
        }

        protected IEnumerator _SystemInitEnterGame(IASyncOperation systemOperation, bool initwithNewbieGuideBattle = false)
        {
            IEnumerator process = _systemInitEnterGameProcess(systemOperation, initwithNewbieGuideBattle);
            ThreeStepProcess threeStepprocess = new ThreeStepProcess(
                    "SystemInitEnterGame",
                    ClientSystemManager.instance.enumeratorManager,
                    process);

            threeStepprocess.SetErrorProcessHandle(_errorProcess);

            yield return threeStepprocess;
        }
#endregion

        protected IEnumerator _SystemInitReturnToTown(IASyncOperation systemOperation)
        {
            Logger.LogProcessFormat("begin to return to town...this is not error");
            {
                NetManager.instance.Disconnect(ServerType.RELAY_SERVER);

                // 重置战斗的一些数据
                BattleDataManager.GetInstance().PkRaceType = RaceType.Dungeon;

                if (Global.Settings.isGuide && BattleMain.IsLastNewbieGuideBattle())
                {
                    Logger.LogProcessFormat("由新手引导第一场战斗返回");
                    yield return _SystemInitEnterGame(systemOperation, true);
                }
                else if (ReplayServer.GetInstance().IsLastPlaying())
                {
                    Logger.LogProcessFormat("由战斗录像返回");
                    InitializeScene(_lastSceneId);
                }
                else if (BattleMain.CheckLastBattleMode(BattleType.Training) ||
                         BattleMain.CheckLastBattleMode(BattleType.TrainingPVE) ||
                         BattleMain.CheckLastBattleMode(BattleType.InputSetting)||
                         BattleMain.CheckLastBattleMode(BattleType.ChangeOccu))
                {
                    Logger.LogProcessFormat("由训练模式返回");
                    InitializeScene(_lastSceneId);
                    BattleMain.battleType = BattleType.Single;
                }
                else
                {
                    Logger.LogProcessFormat("由普通战斗返回");
#region net
                    Logger.LogProcessFormat("发送返回城镇的消息");
                    MessageEvents msgEvents = new MessageEvents();
                    msgEvents.AddMessage(SceneSyncSceneObject.MsgID);
                    SceneNotifyEnterScene msgEnterScene = new SceneNotifyEnterScene();
                    SceneReturnToTown returnScene = new SceneReturnToTown();
                    yield return MessageUtility.WaitWithResend<SceneReturnToTown, SceneNotifyEnterScene>(ServerType.GATE_SERVER, msgEvents, returnScene, msgEnterScene, true, 8.0f);

#endregion

                    systemOperation.SetProgress(0.5f);

#region process data

                    if (msgEvents.IsAllMessageReceived() == false)
                    {
                        Logger.LogErrorFormat("[SystemInitReturnToTown] 错误，没有收到返回城镇相关的消息 {0}", msgEnterScene.mapid);

                        systemOperation.SetError("");

                        ClientSystemManager.instance.QuitToLoginSystem(8501);

                        // dd: 这里强制等待点击确认返回登录
                        while (true) yield return null;
                        //yield break;
                    }

                    if (msgEnterScene.result != (uint)ProtoErrorCode.SUCCESS)
                    {
                        Logger.LogErrorFormat("[SystemInitReturnToTown] 错误，没有收到返回城镇相关的消息 {0}", msgEnterScene.result);
                        //ClientSystemManager.instance.QuitToLoginSystem((int)msgEnterScene.result);
                        ClientSystemManager.instance.QuitToLoginSystem(8501);
                        while (true) yield return null;
                    }

                    if (msgEnterScene.mapid <= 0)
                    {
                        Logger.LogErrorFormat("[SystemInitReturnToTown] 错误，没有收到返回城镇相关的消息 {0}", msgEnterScene.mapid);
                        systemOperation.SetError("");
                        ClientSystemManager.instance.QuitToLoginSystem(8501);
                        while (true) yield return null;
                    }

                    //if (msgEnterScene.mapid <= 0)
                    //{
                    //    Logger.LogErrorFormat("[SystemInitReturnToTown] 错误，收到的消息不对");
                    //    ClientSystemManager.instance.QuitToLoginSystem(8501);
                    //    yield break;
                    //}

                    Logger.LogProcessFormat("收到返回城镇的所有消息");
                    IsNet = true;

                    int currentSceneID = 0;
                    Dictionary<ulong, SceneObject> sceneObjs = null;

                    // 初始化场景信息，其他玩家数据
                    Logger.LogProcessFormat("初始化场景信息，其他玩家数据");
                    {
                        //MsgDATA data = msgEvents.GetMessageData(SceneNotifyEnterScene.MsgID);
                        //SceneNotifyEnterScene msgEnterScene = new SceneNotifyEnterScene();
                        //msgEnterScene.decode(data.bytes);
#if UNITY_EDITOR
                        Logger.LogErrorFormat("吃鸡时序测试----收到服务器消息，吃鸡准备场景切回城镇成功");
#endif
                        currentSceneID = (int)msgEnterScene.mapid;
                        PlayerBaseData.GetInstance().Pos = new Vector3(msgEnterScene.pos.x / PlayerBaseData.GetInstance().AxisScale, 0.0f, msgEnterScene.pos.y / PlayerBaseData.GetInstance().AxisScale);

                        Logger.LogProcessFormat("返回城镇ID：{0}，位置（{1},{2},{3}）", currentSceneID, PlayerBaseData.GetInstance().Pos.x, PlayerBaseData.GetInstance().Pos.y, PlayerBaseData.GetInstance().Pos.z);
                        _InitOtherPlayerData(msgEvents.GetMessageData(SceneSyncSceneObject.MsgID), msgEnterScene.mapid);
                    }

                    systemOperation.SetProgress(0.7f);
                    yield return Yielders.EndOfFrame;
#endregion

                    Logger.LogProcessFormat("初始化场景");
                    InitializeScene(currentSceneID);
                }
                yield return Yielders.EndOfFrame;
            }
            Logger.LogProcessFormat("返回城镇成功");
        }

#endregion

#region Update
        //SystemTownUpdate
        protected sealed override void _OnUpdate(float timeElapsed)
        {
            //_UpdateAsyncLoadPetList();
            UpdateScene(timeElapsed);
            _UpdateData(timeElapsed);
            _UpdateNpcDialog(timeElapsed);
            _OnUpdateDelayCreateCache();
            //_UpdateMainUI(timeElapsed);
            //UpdateInputManager(timeElapsed);
#if !LOGIC_SERVER
            _UpdatePlayers();
#endif
        }

#if !LOGIC_SERVER
        private float mPlayerSimpleModeDistance = 0f;
        private bool mIsEnableSimpleMode;
        private void _UpdatePlayers()
        {
            if (mIsEnableSimpleMode)
            {
                if (MainPlayer != null && MainPlayer.ActorData != null && MainPlayer.ActorData.MoveData != null)
                {
                    var mainPos = MainPlayer.ActorData.MoveData.Position;

                    if (mPlayerSimpleModeDistance == 0f)
                    {
                        mPlayerSimpleModeDistance = 2f;
                        var tableData = TableManager.GetInstance().GetTableItem<ClientConstValueTable>((int)ClientConstValueTable.eKey.TOWN_PLAYER_SIMPLE_MODE_DISTANCE);
                        if (tableData != null && tableData.IntParamsLength > 0)
                        {
                            mPlayerSimpleModeDistance = tableData.IntParams[0] / 1000f;
                        }
                    }

                    foreach (var player in _beTownPlayers.Values)
                    {
                        if (player != null && player.ActorData != null && player.ActorData.MoveData != null)
                        {
                            //Debug.LogFormat("main x {0} {2} player x {1}", mainPos.x, player.ActorData.MoveData.Position.x, player.ActorData.Name);
                            if (Mathf.Abs(mainPos.x - player.ActorData.MoveData.Position.x) >= mPlayerSimpleModeDistance)
                            {
                                player.GeActor.SetDisplayMode(GeActorEx.DisplayMode.Simple);
                            }
                            else
                            {
                                player.GeActor.SetDisplayMode(GeActorEx.DisplayMode.Normal);
                            }
                        }
                    }
                }
            }
        }

        public void SetActorSimpleModeEnable(bool value)
        {
            mIsEnableSimpleMode = value;
            if (!mIsEnableSimpleMode)
            {
                foreach (var player in _beTownPlayers.Values)
                {
                    if (player != null && player.GeActor != null)
                    {
                        player.GeActor.SetDisplayMode(GeActorEx.DisplayMode.Normal);
                    }
                }
            }
        }
#endif
        private void _UpdateData(float delta)
        {
            // buff left time
            var bufflist = PlayerBaseData.GetInstance().buffList;
            bool isMoved = false;
            for (int i = 0; i < bufflist.Count; ++i)
            {
                var buffUnit = bufflist[i];

                if (buffUnit.type == Battle.DungeonBuff.eBuffDurationType.Town
                    || buffUnit.type == Battle.DungeonBuff.eBuffDurationType.SpecialTown)
                {
                    buffUnit.lefttime -= delta;

                    if (buffUnit.lefttime < 0)
                    {
                        buffUnit.readymove = true;
                        isMoved = true;
                    }
                }
            }

            if (isMoved)
            {
                bufflist.RemoveAll(CheckCanRemoveBuff);
            }
        }

        private bool CheckCanRemoveBuff(Battle.DungeonBuff x)
        {
            if (x.readymove)
            {
                Logger.LogProcessFormat("[buffdrug] 本地删除超时buff {0} id {1}", x.uid, x.id);
                PlayerBaseData.GetInstance().removedBuffList.Add(x);
            }

            return x.readymove;
        }
#endregion

#region TownScene

        // 外部传进来的数据
        private int _currentSceneId;
        public int CurrentSceneID
        {
            get
            {
                return _currentSceneId;
            }
            private set
            {
                if (_fromSceneId != _currentSceneId && -1 != _currentSceneId)
                {
                    _fromSceneId = _currentSceneId;
                }

                _currentSceneId = value;
            }
        }

        private int _fromSceneId;
        public int FromSceneID
        {
            get
            {
                return _fromSceneId;
            }
        }

        private int _lastSceneId;

        public Dictionary<ulong, SceneObject> PlayerOthersData = new Dictionary<ulong, SceneObject>();
        public bool IsNet = true;

        // NPC
        protected List<BeTownNPC> _beTownNPCs = new List<BeTownNPC>();

        //攻城怪物
        private List<BeTownNPC> _beTownAttackCityMonsters = new List<BeTownNPC>();

        //黑市商人NPC
        private BeTownNPC _beTownBlackMarketMerchantNpc;

        // 主角avatar的更改，通过PlayerBaseData里服务器下发SceneObjectAttr.SOA_AVATAR决定
        protected BeTownPlayerMain _beTownPlayerMain;

        protected List<BeTownDoor> _beTownDoors = new List<BeTownDoor>();
        protected List<Vector3> _akFollowPetsPos = new List<Vector3>();

        public BeTownPlayerMain MainPlayer
        {
            get
            {
                return _beTownPlayerMain;
            }
        }
        // 其他玩家
        protected Dictionary<ulong, BeTownPlayer> _beTownPlayers = new Dictionary<ulong, BeTownPlayer>();

        protected List<ulong> _beDisplayPlayerList = new List<ulong>();
        protected int _beDisplayNum = 0;

        protected ISceneData _levelData;
        public ISceneData LevelData
        {
            get { return _levelData; }
        }

        public PathFinding.GridInfo GridInfo { get; private set; }

        public GeSceneEx Scene
        {
            get {return _geScene;}
        }
        protected GeSceneEx _geScene;
        protected Vector3 _logicToGraph;
        protected Vector3 _serverToClient;
        //protected ScrollCamera _scrollCamera;

        protected float _axisScale = 10000.0f;
        protected float _inputTimeElapsed = 0.0f;

        protected Dictionary<int, int> m_mapDungeonSceneID = new Dictionary<int, int>();

        protected uint m_BgmHandle = uint.MaxValue;
        protected uint m_EnvGgmHandle = uint.MaxValue;

        //主城中场景（城镇）信息，场景初始化，场景切换相关内容
        public static bool GetCurrentSceneSubType(out CitySceneTable.eSceneSubType subType)
        {
            subType = CitySceneTable.eSceneSubType.NULL;

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return false;
            }

            ProtoTable.CitySceneTable townTableData = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(systemTown.CurrentSceneID);
            if (townTableData == null)
            {
                return false;
            }

            subType = townTableData.SceneSubType;
            return true;
        }

        public void OnGraphicSettingChange(int displayNum)
        {
            _beDisplayNum = displayNum;
            if (_beDisplayNum < _beDisplayPlayerList.Count)
            {
                List<ulong> displayID = GamePool.ListPool<ulong>.Get();
                for (int i = _beDisplayNum, icnt = _beDisplayPlayerList.Count; i < icnt; ++i)
                    displayID.Add(_beDisplayPlayerList[i]);

                for (int i = 0, icnt = displayID.Count; i < icnt; ++i)
                    _RemoveDisplayActor(displayID[i]);
                GamePool.ListPool<ulong>.Release(displayID);
            }
            else if (_beDisplayNum > _beDisplayPlayerList.Count)
            {
                Dictionary<ulong, BeTownPlayer>.Enumerator it = _beTownPlayers.GetEnumerator();

                while (it.MoveNext())
                {
                    BeTownPlayer curPlayer = it.Current.Value;
                    if (null != curPlayer)
                        _AddDisplayActor(it.Current.Key);

                    if (_beDisplayPlayerList.Count == _beDisplayNum)
                        break;
                }
            }
        }

        int _Locate(List<SceneNode> nodes, int sceneID)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (nodes[i].SceneID == sceneID)
                {
                    return i;
                }
            }
            return -1;
        }

        public List<SceneNode> m_sceneNodes = new List<SceneNode>();
        

        //建立场景图的邻接表结构
        void _InitSceneNodeGraph()
        {
            m_sceneNodes.Clear();

            // TODO 离线生成数据

            var tableItem = TableManager.instance.GetTable<ProtoTable.CitySceneTable>();
            var iter = tableItem.GetEnumerator();
            while (iter.MoveNext())
            {
                var table = iter.Current.Value as ProtoTable.CitySceneTable;
                if (table != null)
                {
                    var data = DungeonUtility.LoadSceneData(table.ResPath);// AssetLoader.instance.LoadRes(table.ResPath, typeof(DSceneData)).obj as DSceneData;
                    if (data != null)
                    {
                        SceneNode node = new SceneNode
                        {
                            SceneID = table.ID,
                            DoorNodes = new List<DoorNode>()
                        };
                        for (int i = 0; i < data.GetTownDoorLength(); ++i)
                        {
                            DoorNode doorNode = new DoorNode
                            {
                                TargetSceneIndex = -1,
                                Door = data.GetTownDoor(i)
                            };
                            node.DoorNodes.Add(doorNode);
                        }
                        m_sceneNodes.Add(node);
                    }
                }
            }

            for (int i = 0; i < m_sceneNodes.Count; ++i)
            {
                SceneNode node = m_sceneNodes[i];
                for (int j = 0; j < node.DoorNodes.Count; ++j)
                {
                    DoorNode doorNode = node.DoorNodes[j];
                    doorNode.TargetSceneIndex =
                        SceneMapUtility.GetSceneNodeIndex(m_sceneNodes, doorNode.Door.GetTargetSceneID());
                    if (doorNode.TargetSceneIndex < 0)
                    {
                        Logger.LogErrorFormat(
                            "Scene:{0} door-{1} targetScene {2} is not exist!",
                            node.SceneID, doorNode.Door.GetDoorID(), doorNode.Door.GetTargetSceneID()
                            );
                    }
                }
            }
        }

        bool _DFS(List<SceneNode> nodes, int current, int target, 
            ref List<Vector3> path, 
            DoorNode door, 
            ref List<int> SceneIDList, 
            ref List<int> DoorIDList)
        {
            SceneNode sceneNode = nodes[current];
            sceneNode.HasVisited = true;


            if (current == target)
            {
                if (door != null)
                {
                    path.Add(door.Door.GetRegionInfo().GetEntityInfo().GetPosition());
                    SceneIDList.Add(sceneNode.SceneID);
                    DoorIDList.Add(door.Door.GetDoorID());
                }
                return true;
            }

            if (door != null)
            {
                path.Add(door.Door.GetRegionInfo().GetEntityInfo().GetPosition());
                SceneIDList.Add(sceneNode.SceneID);
                DoorIDList.Add(door.Door.GetDoorID());
            }

            for (int i = 0; i < sceneNode.DoorNodes.Count; ++i)
            {
                DoorNode doorNode = sceneNode.DoorNodes[i];

                SceneNode targetNode = nodes[doorNode.TargetSceneIndex];
                if (targetNode.HasVisited == false)
                {
                    if (_DFS(nodes, doorNode.TargetSceneIndex, target, ref path, doorNode, ref SceneIDList, ref DoorIDList))
                    {
                        return true;
                    }
                }
            }

            if (door != null)
            {
                path.RemoveAt(path.Count - 1);
                SceneIDList.RemoveAt(SceneIDList.Count - 1);
                DoorIDList.RemoveAt(DoorIDList.Count - 1);
            }
            return false;
        }


        public List<Vector3> GetMovePath(int targetSceneID)
        {
            List<Vector3> path = new List<Vector3>();

            var targetIndex = SceneMapUtility.GetSceneNodeIndex(m_sceneNodes, targetSceneID);

            if (targetIndex >= 0)
            {
                for (int i = 0; i < m_sceneNodes.Count; ++i)
                {
                    m_sceneNodes[i].HasVisited = false;
                }

                List<int> sceneIDList = new List<int>();
                List<int> doorIDList = new List<int>();

                var currentIndex = SceneMapUtility.GetSceneNodeIndex(m_sceneNodes, CurrentSceneID);
                _DFS(m_sceneNodes, currentIndex, targetIndex, ref path, null, ref sceneIDList, ref doorIDList);

                Logger.LogProcessFormat("获得移动到场景{0}的路径 >>> 当前场景{1}", targetSceneID, CurrentSceneID);
                for (int i = 0; i < sceneIDList.Count; ++i)
                {
                    Logger.LogProcessFormat("获得移动到场景{0}的路径 >>> 场景ID:{1} 传送门ID:{2}", targetSceneID, sceneIDList[i], doorIDList[i]);
                }
            }
            else
            {
                Logger.LogProcessFormat("获得移动到场景{0}的路径：找不到索引", targetSceneID);
            }

            return path;
        }

        public Vector3 GetNpcPostion(int NpcID)
        {
            Vector3 position = Vector3.zero;
            for (int i = 0; i < LevelData.GetNpcInfoLength(); ++i)
            {
                ISceneNPCInfoData info = LevelData.GetNpcInfo(i);
                if (info.GetEntityInfo().GetResid() == NpcID)
                {
                    position = info.GetEntityInfo().GetPosition();
                    break;
                }
            }
            return position;
        }

        public ISceneNPCInfoData GetNpcInfo(int NpcID)
        {
            ISceneNPCInfoData result = null;
            if (LevelData != null && LevelData.GetNpcInfoLength() >= 0)
            {
                for (int i = 0; i < LevelData.GetNpcInfoLength(); ++i)
                {
                    ISceneNPCInfoData info = LevelData.GetNpcInfo(i);
                    if (info.GetEntityInfo().GetResid() == NpcID)
                    {
                        result = info;
                        break;
                    }
                }
            }
            return result;
        }

        public Vector3 GetValidTargetPosition(Vector3 a_currentPos, Vector3 a_targetPos, Vector2 a_minRange, Vector2 a_maxRange)
        {
            Logger.LogWarningFormat("获取有效格子 >>> a_targetPos:({0},{1},{2}) a_minRange:({3},{4}) a_maxRange:({5},{6})",
                a_targetPos.x, a_targetPos.y, a_targetPos.z,
                a_minRange.x, a_minRange.y,
                a_maxRange.x, a_maxRange.y
            );

            PathFinding.Grid targetGrid = new PathFinding.Grid(GridInfo, a_targetPos);
            int validTargetGridX = targetGrid.X > GridInfo.GridMaxX ?
                GridInfo.GridMaxX : (targetGrid.X < GridInfo.GridMinX ? GridInfo.GridMinX : targetGrid.X);
            int validTargetGridY = targetGrid.Y > GridInfo.GridMaxY ?
                GridInfo.GridMaxX : (targetGrid.Y < GridInfo.GridMinY ? GridInfo.GridMinY : targetGrid.Y);

            PathFinding.Grid validTargetGrid = new PathFinding.Grid(GridInfo, validTargetGridX, validTargetGridY);

            Vector3 vecMinHalf = new Vector3(a_minRange.x * 0.5f, 0.0f, a_minRange.y * 0.5f);
            Vector3 vecMaxHalf = new Vector3(a_maxRange.x * 0.5f, 0.0f, a_maxRange.y * 0.5f);

            PathFinding.Grid minGridA = new PathFinding.Grid(GridInfo, validTargetGrid.RealPos + vecMinHalf);
            PathFinding.Grid minGridB = new PathFinding.Grid(GridInfo, validTargetGrid.RealPos - vecMinHalf);
            PathFinding.Grid maxGridA = new PathFinding.Grid(GridInfo, validTargetGrid.RealPos + vecMaxHalf);
            PathFinding.Grid maxGridB = new PathFinding.Grid(GridInfo, validTargetGrid.RealPos - vecMaxHalf);

            List<PathFinding.Grid> validGrids = new List<PathFinding.Grid>();

            int outMinX = maxGridA.X < maxGridB.X ? maxGridA.X : maxGridB.X;
            int outMaxX = maxGridA.X < maxGridB.X ? maxGridB.X : maxGridA.X;
            int outMinY = maxGridA.Y < maxGridB.Y ? maxGridA.Y : maxGridB.Y;
            int outMaxY = maxGridA.Y < maxGridB.Y ? maxGridB.Y : maxGridA.Y;

            int inMinX = minGridA.X < minGridB.X ? minGridA.X : minGridB.X;
            int inMaxX = minGridA.X < minGridB.X ? minGridB.X : minGridA.X;
            int inMinY = minGridA.Y < minGridB.Y ? minGridA.Y : minGridB.Y;
            int inMaxY = minGridA.Y < minGridB.Y ? minGridB.Y : minGridA.Y;

            for (int i = outMinX; i <= outMaxX; ++i)
            {
                for (int j = outMinY; j <= outMaxY; ++j)
                {
                    if (
                        (i >= inMinX && i <= inMaxX && j >= inMinY && j <= inMaxY) ||
                        i < GridInfo.GridMinX || i >= GridInfo.GridMaxX ||
                        j < GridInfo.GridMinY || j >= GridInfo.GridMaxY
                        )
                    {
                        continue;
                    }

                    int x = i - GridInfo.GridMinX;
                    int y = j - GridInfo.GridMinY;
                    int index = (GridInfo.GridMaxX - GridInfo.GridMinX) * y + x;
                    if (index >= 0 && index < GridInfo.GridBlockLayer.Length)
                    {
                        if (GridInfo.GridBlockLayer[index] != 0)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    validGrids.Add(new PathFinding.Grid(GridInfo, i, j));
                }
            }

            if (validGrids.Count > 0)
            {
                PathFinding.Grid currentGrid = new PathFinding.Grid(GridInfo, a_currentPos);
                for (int i = 0; i < validGrids.Count; ++i)
                {
                    if (currentGrid.X == validGrids[i].X && currentGrid.Y == validGrids[i].Y)
                    {
                        return validGrids[i].RealPos;
                    }
                }

                Logger.LogProcessFormat("获取随机位置 >>> 有效格子数{0}", validGrids.Count);
                int index = UnityEngine.Random.Range(0, validGrids.Count - 1);
                return validGrids[index].RealPos;
            }
            else
            {
                Logger.LogWarningFormat("获取随机位置 >>> 错误!");
                return a_targetPos;
            }
        }

        void _InitDungeonMap()
        {
            Logger.LogProcessFormat("init dungeon map...");
            m_mapDungeonSceneID.Clear();

            var tableItem = TableManager.instance.GetTable<ProtoTable.CitySceneTable>();
            var iter = tableItem.GetEnumerator();

            DungeonID id = new DungeonID(0);

            while (iter.MoveNext())
            {
                var table = iter.Current.Value as ProtoTable.CitySceneTable;
                if (table != null && table.SceneType == ProtoTable.CitySceneTable.eSceneType.DUNGEON_ENTRY)
                {
                    for (int i = 0; i < table.ChapterData.Count; ++i)
                    {
                        var data = AssetLoader.instance.LoadRes(table.ChapterData[i]).obj as DChapterData;
                        if (data != null)
                        {
                            if (data.chapterList.Length > 0)
                            {
                                for (int j = 0; j < data.chapterList.Length; ++j)
                                {
                                    int dungeonID = data.chapterList[j].dungeonID;

                                    id.dungeonID = dungeonID;

                                    if (!m_mapDungeonSceneID.ContainsKey(id.dungeonIDWithOutDiff))
                                    {
                                        m_mapDungeonSceneID.Add(id.dungeonIDWithOutDiff, table.ID);
                                    }
                                    else
                                    {
                                        Logger.LogErrorFormat("地下城ID {0} 在表 城镇副本表 {1} 中重复 ", id.dungeonID, table.ID);
                                    }
                                    Logger.LogProcessFormat("key:{0} Value:{1}", id, table.ID);
                                }
                            }
                        }
                    }
                }
            }

            Logger.LogProcessFormat("init dungeon map finished!!");
        }

        public int GetDungenSceneID(int dungenID)
        {
            int sceneID;
            DungeonID id = new DungeonID(dungenID);
            if (m_mapDungeonSceneID.TryGetValue(id.dungeonIDWithOutPrestory, out sceneID))
            {
                return sceneID;
            }
            else
            {
                return -1;
            }
        }

        public Vector3 GetPathFindingDirection(Vector3 src, Vector3 target)
        {
            Vector3 dir = Vector3.zero;

            dir = target - src;
            if (Mathf.Abs(dir.x) >= 1.0f)
            {
                dir.z = 0.0f;
            }
            dir.y = 0.0f;
            dir.Normalize();
            //var pathFindingList = PathFinding.FindPath(src, target,GridInfo);
            //if (pathFindingList.Count >= 2)
            //{
            //    dir = pathFindingList[pathFindingList.Count - 1] - pathFindingList[0];
            //    if(Mathf.Abs(dir.x) >= 1.0f)
            //    {
            //        dir.z = 0.0f;
            //    }
            //    dir.y = 0.0f;
            //    dir.Normalize();
            //}

            return dir;
        }

        public void OnNPCLoadFinished(int npcID, GeActorEx actor)
        {
            if (actor != null)
            {
                actor.AddVoiceListener(NpcVoiceComponent.SoundEffectType.SET_Random);
            }
        }


#region TownSceneInfo
        // 主城中初始化场景（城镇）
        protected void InitializeScene(int currentSceneId)
        {
#region SceneConfig
            // 加载场景文件
            CitySceneTable targetCityTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(currentSceneId);
            if (targetCityTable == null)
            {
                Logger.LogErrorFormat("scene id {0} is not exist!", currentSceneId);
                return;
            }

            bool bNeedGC = true;
            if (CurrentSceneID > 0)
            {
                CitySceneTable currCityTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(CurrentSceneID);
                if (targetCityTable != null)
                {
                    bNeedGC = targetCityTable.AreaID != currCityTable.AreaID;
                }
            }

            ClearScene(bNeedGC);
            _RegisterEvent();

            CurrentSceneID = currentSceneId;
            _lastSceneId = CurrentSceneID;

            _levelData = DungeonUtility.LoadSceneData(targetCityTable.ResPath); //AssetLoader.instance.LoadRes(targetCityTable.ResPath, typeof(DSceneData)).obj as DSceneData;
            if (_levelData == null)
            {
                Logger.LogErrorFormat("_level data is nil, currentSceneId = {0}, CurrentSceneID = {1}, targetCityTable.ResPath = {2}", currentSceneId, CurrentSceneID, targetCityTable.ResPath);
                return;
            }

            //Physics.queriesHitTriggers = true;

            _logicToGraph = _levelData.GetLogicPos();
            _serverToClient = new Vector3(_levelData.GetLogicXSize().x, 0.0f, _levelData.GetLogicZSize().x);

            var currentGridInfo = new PathFinding.GridInfo
            {
                GridSize = _levelData.GetGridSize(),
                GridMinX = _levelData.GetLogicXmin(),
                GridMaxX = _levelData.GetLogicXmax(),
                GridMinY = _levelData.GetLogicZmin(),
                GridMaxY = _levelData.GetLogicZmax(),
                GridBlockLayer = _levelData.GetRawBlockLayer()
            };
            currentGridInfo.GridDiagonalLength = currentGridInfo.GridSize.magnitude;

            GridInfo = currentGridInfo;

            SceneMapDataManager.GetInstance().SetSceneDataBySceneData(currentSceneId, _levelData);
            SceneMapDataManager.GetInstance().SetGridInfoByGridInfo(currentSceneId, currentGridInfo);

            // 创建场景
            _geScene = new GeSceneEx();

            Logger.LogAsset("Load scene!");
            _geScene.LoadScene(_levelData);
            GeWeatherManager.instance.ChangeWeather(_levelData.GetWeatherMode());

#endregion

            GeGraphicSetting.instance.GetSetting("PlayerDisplayNum", ref _beDisplayNum);
#if !LOGIC_SERVER
            mIsEnableSimpleMode = !GeGraphicSetting.instance.IsHighLevel();
#endif
            #region CreateSceneObject

#if MG_TEST
            Logger.LogError("[城镇] 初始化场景, 开始 CreateSceneObjects()");
#endif
            CreateSceneObjects();
            
#endregion

#region LowConfig
            //低配手机设置注释
            /*
                    //1G内存设置
                    if (PluginManager.IsTargetDevice() && _beDisplayNum > 10)
                    {
                        bool forceModified = false;
                        if (!GeGraphicSetting.instance.GetSetting("PlayerDisplayNum_ForceModify", ref forceModified))
                        {
                            GeGraphicSetting.instance.AddSetting("PlayerDisplayNum_ForceModify", true);

                            _beDisplayNum = 10;
                            GeGraphicSetting.instance.SetSetting("PlayerDisplayNum", _beDisplayNum);
                            GeGraphicSetting.instance.SaveSetting();
                        }
                    }*/
#endregion

            // 初始化相机控制器
            _InitializeCameraController();

            AddExtraOtherPlayerData();
            _BindSceneNetMsgs();

            if (targetCityTable.SceneType == CitySceneTable.eSceneType.PK_PREPARE)
            {
                ClientSystemManager.GetInstance().bIsInPkWaitingRoom = true;
            }
            else
            {
                ClientSystemManager.GetInstance().bIsInPkWaitingRoom = false;
            }

            // EventSystem被拆成 UIEventSystem 和 GlobalEventSystem
            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.SceneJumpFinished, CurrentSceneID);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TownSceneInited);
        }

        public void ChangeScene(int targetSceneId,
            int targetDoorId,
            int currentSceneId = 0,
            int currentDoorId = 0,
            SceneParams.OnSceneLoadFinish sceneLoadFinish = null)
        {
            var bReturnToTown = targetSceneId <= 0;
            var sceneParams = new SceneParams(currentSceneId, currentDoorId, targetSceneId, targetDoorId)
            {
                onSceneLoadFinish = sceneLoadFinish,
            };

            GameFrameWork.instance.StartCoroutine(
                _NetSyncChangeScene(
                    sceneParams,
                    bReturnToTown
                ));
        }

        DTownDoor _backTownDoor;
        public void ReturnToTown()
        {
            if (_backTownDoor != null)
            {
                var sceneParams = new SceneParams(_backTownDoor.SceneID, _backTownDoor.DoorID,
                    _backTownDoor.TargetSceneID, _backTownDoor.TargetDoorID);
                GameFrameWork.instance.StartCoroutine(_NetSyncChangeScene(sceneParams));
                _backTownDoor = null;
            }
        }

        DTownDoor _enterTownDoor;
        public void QuickEnter()
        {
            if (_enterTownDoor != null)
            {
                var sceneParams = new SceneParams(_enterTownDoor.SceneID, _enterTownDoor.DoorID,
                    _enterTownDoor.TargetSceneID, _enterTownDoor.TargetDoorID);
                GameFrameWork.instance.StartCoroutine(_NetSyncChangeScene(sceneParams));
                _enterTownDoor = null;
            }
        }

        private bool _isSwithScene = false;
        public void SwitchToTargetScene(int targetSceneId, int targetDoorId, SceneParams.OnSceneLoadFinish onOk)
        {
            if (_isSwithScene)
            {
                return;
            }

            if (targetSceneId == CurrentSceneID)
            {
                if (onOk != null)
                {
                    onOk.Invoke();
                }
                return;
            }

            _isSwithScene = true;
            var sceneParams = new SceneParams(CurrentSceneID, 0, targetSceneId, targetDoorId);
            GameFrameWork.instance.StartCoroutine(_NetSyncChangeScene(sceneParams, false));
        }

        private bool mIsTownSceneSwitching = false;
        private bool isTownSceneSwitching
        {
            get
            {
                return mIsTownSceneSwitching;
            }

            set
            {
                Logger.LogProcessFormat("[TownIsSceneSwitching] {0} -> {1}", mIsTownSceneSwitching, value);
                mIsTownSceneSwitching = value;
            }
        }

        private void _resetIsSwiching()
        {
            isTownSceneSwitching = false;
            _isSwithScene = false;
        }

        private void SetTownSceneSwitchState(bool flag)
        {
            isTownSceneSwitching = flag;
            ClientSystemManager.GetInstance().SendSceneNotifyLoadingInfoByTownSwitchScene(flag);
            //场景切换完成
            if (isTownSceneSwitching == false)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SceneChangedLoadingFinish);
            }

            if (!flag)
            {
                UWAProfilerUtility.DoDump();
            }
        }

        public bool GetTownSceneSwitchState()
        {
            return isTownSceneSwitching;
        }

        //主城中切换场景（城镇）
        public IEnumerator _NetSyncChangeScene(SceneParams data, bool bReturnScene = false/*这个参数不要随便填，“从其他场景返回主城和利亚房间才填true，其他场景之间的相互切换都填false”*/)
        {
#if MG_TEST
            //Logger.LogErrorFormat("测试服----开始切换场景,不是报错，只是流程日志");
#endif
            //停止播放背景音乐
            if (data != null)
            {
                StopBGM();
            }


            UInt32 startTime = Utility.GetCurrentTimeUnix();			

            if (isTownSceneSwitching)
            {
#if MG_TEST
                //Logger.LogErrorFormat("开始切换场景---isTownSceneSwitching 为 true");
#endif
                yield break;
            }

#if MG_TEST
            //Logger.LogErrorFormat("切换场景---SetTownSceneSwitchState() 为 true");
#endif

            SetTownSceneSwitchState(true);

            //AsyncLoadTaskManager.instance.ClearAllAsyncTasks();

            var isUseLoadingFrame = false;

#region JudgeSceneCanReturn
            //判断场景是否可以切换
            if (bReturnScene == false)
            {
                var currTableScene = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(data.currSceneID);
                var targetTableScene = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(data.targetSceneID);
                if (currTableScene != null && targetTableScene != null)
                {
                    /// 城镇之间切换加Loading图 城镇和对应旅店切换不加Loading图 使用AreaID能够满足需求
                    isUseLoadingFrame = currTableScene.AreaID != targetTableScene.AreaID;

                    if (
                        currTableScene.SceneType == CitySceneTable.eSceneType.NORMAL
                        && (targetTableScene.SceneType == CitySceneTable.eSceneType.NORMAL || targetTableScene.SceneType == CitySceneTable.eSceneType.DUNGEON_ENTRY) 
                        && PlayerBaseData.GetInstance().Level < targetTableScene.LevelLimit
                        )
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("town_lock_desc", targetTableScene.Name, targetTableScene.LevelLimit));
                        _isSwithScene = false;
                        SetTownSceneSwitchState(false);

#if MG_TEST
                        //Logger.LogErrorFormat("切换场景失败---SetTownSceneSwitchState() 为 false");
#endif

                        yield break;
                    }
                }
            }
#endregion

            if (MainPlayer != null)
            {
                MainPlayer.SetEnable(false);
            }

            bool bNeedHideBottomLayer = (data.targetSceneID == GuildDataManager.nGuildAreanScenceID);         
            if(bNeedHideBottomLayer)
            {
                if (ClientSystemManager.GetInstance().BottomLayer != null)
                {
                    ClientSystemManager.GetInstance().BottomLayer.CustomActive(false);
                }
            }
            

            _UnBindSceneNetMsgs();
            OnBindExtraSceneNetMessages();

            //Logger.LogProcessFormat("[城镇] 开始一切换场景 当前场景ID : {0}, 当前门 : {1}, 目标场景: {2}, 目标门ID {3}", data.currSceneID, data.currDoorID, data.targetSceneID, data.targetDoorID);
#if MG_TEST
            Logger.LogErrorFormat("[城镇] 开始一切换场景 当前场景ID : {0}, 当前门 : {1}, 目标场景: {2}, 目标门ID {3}", data.currSceneID, data.currDoorID, data.targetSceneID, data.targetDoorID);
#endif
            
#region SendNetMessage
            //发送切换场景的消息到服务器
            MessageEvents msgEvents = new MessageEvents();
            msgEvents.AddMessage(SceneSyncSceneObject.MsgID);
            SceneNotifyEnterScene msgEnterScene = new SceneNotifyEnterScene();
            if (bReturnScene)
            {
                SceneReturnToTown returnScene = new SceneReturnToTown();

                yield return MessageUtility.WaitWithResend<SceneReturnToTown, SceneNotifyEnterScene>(ServerType.GATE_SERVER, msgEvents, returnScene, msgEnterScene, true, 8.0f);
            }
            else
            {
                ScenePlayerChangeSceneReq changeScene = new ScenePlayerChangeSceneReq
                {
                    curDoorId = (uint)data.currDoorID,
                    dstSceneId = (uint)data.targetSceneID,
                    dstDoorId = (uint)data.targetDoorID
                };
                yield return MessageUtility.WaitWithResend<ScenePlayerChangeSceneReq, SceneNotifyEnterScene>(ServerType.GATE_SERVER, msgEvents, changeScene, msgEnterScene, true, 8.0f);
            }
#endregion

#region DealWithReceivedMessage
            //处理接收到的消息
            if (msgEvents.IsAllMessageReceived() == false)
            {
                if (MainPlayer != null)
                {
                    MainPlayer.SetEnable(true);

                    if (data.type == eSceneChangeType.eDungeonChapterSelect)
                    {
                        MainPlayer.CommandMoveTo(data.birthPostion);
                    }
                }
                _BindSceneNetMsgs();
                _isSwithScene = false;
                SetTownSceneSwitchState(false);

                if (ClientSystemManager.GetInstance().BottomLayer != null)
                {
                    ClientSystemManager.GetInstance().BottomLayer.CustomActive(true);
                }

#if MG_TEST
                Logger.LogErrorFormat("[城镇] 切换场景失败(消息未全收到) 待在原地. 出现该报错不一定代表流程真的有问题,具体问题具体分析，当前场景ID : {0}, 当前门 : {1}, 目标场景: {2}, 目标门ID {3}", data.currSceneID, data.currDoorID, data.targetSceneID, data.targetDoorID);
#endif
                //string str = string.Format("[城镇]--切换场景失败(消息未全收到),待在原地,未收服务器消息id:{0},bReturnScene:{1},当前场景id:{2},目标场景id:{3}.看到该弹框请截图保存，烦请发给客服，谢谢!", msgEvents.GetNotReceivedMessageID(), bReturnScene, data.currSceneID, data.targetSceneID);
                //SystemNotifyManager.SysNotifyMsgBoxOK(str);

                yield break;
            }

            if (msgEnterScene.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgEnterScene.result);

                if (MainPlayer != null)
                {
                    MainPlayer.SetEnable(true);
                }

                if (ClientSystemManager.GetInstance().BottomLayer != null)
                {
                    ClientSystemManager.GetInstance().BottomLayer.CustomActive(true);
                }

                _BindSceneNetMsgs();
                _isSwithScene = false;
                SetTownSceneSwitchState(false);

                Logger.LogErrorFormat("[城镇] 切换场景失败(返回错误码) 待在原地. 当前场景ID : {0}, 当前门 : {1}, 目标场景: {2}, 目标门ID {3},msgEnterScene.result = {4}", data.currSceneID, data.currDoorID, data.targetSceneID, data.targetDoorID, msgEnterScene.result);

                yield break;
            }

            if (msgEnterScene.mapid <= 0)
            {
                if (MainPlayer != null)
                {
                    MainPlayer.SetEnable(true);
                    if (data.type == eSceneChangeType.eDungeonChapterSelect)
                    {
                        MainPlayer.CommandMoveTo(data.birthPostion);
                    }
                }
                _BindSceneNetMsgs();
                _isSwithScene = false;
                SetTownSceneSwitchState(false);
                Logger.LogErrorFormat("[城镇] 切换场景失败(场景id无效) 待在原地. 当前场景ID : {0}, 当前门 : {1}, 目标场景: {2}, 目标门ID {3}", data.currSceneID, data.currDoorID, data.targetSceneID, data.targetDoorID);

                //string str = string.Format("[城镇]--切换场景失败(场景id无效)待在原地,mapid:{0},name:{1},当前场景ID:{2},目标场景ID:{3},bReturnScene:{4},看到该弹框请截图保存信息,联系客服,然后点击确定即可,谢谢", msgEnterScene.mapid, msgEnterScene.name, data.currSceneID, data.targetSceneID, bReturnScene);
                //SystemNotifyManager.SysNotifyMsgBoxOK(str);

                yield break;
            }
#endregion

#region LoadingFrame
            //加载loading页面
#if MG_TEST
            //Logger.LogErrorFormat("[城镇] 切换场景, 开始打开 loadingFrame，isUseLoadingFrame = {0}", isUseLoadingFrame);
#endif
            var loadingFrame = OpenTownFadingFrame(isUseLoadingFrame);

            if(loadingFrame != null)
            {
#if MG_TEST
            	//Logger.LogErrorFormat("[城镇] 切换场景,loadingFrame 开始 FadingIn，isUseLoadingFrame = {0}", isUseLoadingFrame);
#endif
                loadingFrame.FadingIn(0.4f);
            }
            
            //if (isUseLoadingFrame)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
                {
                    ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                    if (null != Townframe)
                    {
                        Townframe.StopMainPlayerMoveAndStopFizzyCheck();
                    }
                }
            }

            // 消息返回
            msgEnterScene.decode(msgEvents.GetMessageData(SceneNotifyEnterScene.MsgID).bytes);
            //初始化网络人物信息
            _InitOtherPlayerData(msgEvents.GetMessageData(SceneSyncSceneObject.MsgID), msgEnterScene.mapid);

            if(loadingFrame != null)
            {
                while (loadingFrame.IsOpened() == false)
                {
#if MG_TEST
                    //Logger.LogErrorFormat("[城镇] 切换场景,loading界面已关闭. 当前场景ID : {0}, 当前门 : {1}, 目标场景: {2}, 目标门ID {3}", data.currSceneID, data.currDoorID, data.targetSceneID, data.targetDoorID);
#endif

                    yield return Yielders.EndOfFrame;
                }
            }
            else
            {
                Logger.LogErrorFormat("[城镇] 切换场景, loadingFrame is null. 当前场景ID : {0}, 当前门 : {1}, 目标场景: {2}, 目标门ID {3}", data.currSceneID, data.currDoorID, data.targetSceneID, data.targetDoorID);
            }
#endregion

            // 先处理是否关闭非常驻界面
            ClientSystemManager.GetInstance().CloseFrames();
            yield return null;

            var tableData = TableManager.GetInstance().GetTableItem<CitySceneTable>((int)msgEnterScene.mapid);
#region switchScene
            // 根据场景类型做不同处理
            if (tableData != null)
            {
                ClientSystemManager.GetInstance().bIsInPkWaitingRoom = false;

                // 再根据特有需求做特殊处理
                if (tableData.SceneType == CitySceneTable.eSceneType.DUNGEON_ENTRY)
                {
#region Enter Dungeon
                    ClearScene(false);
                    _BindSceneNetMsgs();

                    ShowDugeonEntryFrame(data, (int)msgEnterScene.mapid);
#endregion
                }
                else
                {

#region LoadSceneInfo
                    //服务器返回位置
                    PlayerBaseData.GetInstance().Pos = new Vector3(msgEnterScene.pos.x / PlayerBaseData.GetInstance().AxisScale,
                        0.0f,
                        msgEnterScene.pos.y / PlayerBaseData.GetInstance().AxisScale);

                    Logger.LogFormat("change scene rece pos:({0},{1})", PlayerBaseData.GetInstance().Pos.x, PlayerBaseData.GetInstance().Pos.z);

#if MG_TEST
                    Logger.LogError("[城镇] 切换场景, 开始初始化场景.");
#endif
                    //初始化场景
                    InitializeScene((int)msgEnterScene.mapid);
                    //关闭其他Frame
                    ClearBaseMainFrame();
#endregion

                    if (tableData.SceneType == CitySceneTable.eSceneType.PK_PREPARE)
                    {
                        //关闭主城Frame
                        //if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
                        //{
                        //    ClientSystemManager.GetInstance().CloseFrame<ClientSystemTownFrame>(null, true);
                        //}
                        
#region pk_PrePare
                        //使用Switch，打开PrePareFrame
                        ShowPkPrePareFrame(tableData);
#endregion
                    }
                    else if (tableData.SceneType == CitySceneTable.eSceneType.TEAMDUPLICATION)
                    {
                        //团本的界面
                        ShowTeamDuplicationFrame(tableData);
                    }
                    else if (tableData.SceneType == CitySceneTable.eSceneType.NORMAL && tableData.SceneSubType == CitySceneTable.eSceneSubType.Guild)
                    {
                        var guildAeenaData = new GuildArenaData
                        {
                            SceneSubType = tableData.SceneSubType,
                            CurrentSceneID = tableData.ID,
                            TargetTownSceneID = tableData.TraditionSceneID,
                        };
                        ClientSystemManager.GetInstance().OpenFrame<GuildArenaFrame>(FrameLayer.Bottom, guildAeenaData);
                    }

                    var uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                    uiEvent.EventParams.CurrentSceneID = (int)msgEnterScene.mapid;
                    uiEvent.EventID = EUIEventID.SceneChangedFinish;
                    UIEventSystem.GetInstance().SendUIEvent(uiEvent);
                }
            }
#endregion

#if MG_TEST
            //Logger.LogErrorFormat("[城镇] 切换场景,loadingFrame 开始 FadingOut). isUseLoadingFrame = {0}", isUseLoadingFrame);
#endif
            if(loadingFrame != null)
            {
                loadingFrame.FadingOut(0.2f);
            }
       
            if(loadingFrame != null)
            {
                while (loadingFrame.IsClosed() == false)
                {
                    yield return Yielders.EndOfFrame;
                }
            }
#if MG_TEST
            //Logger.LogErrorFormat("[城镇] 切换场景,loading结束. 当前场景ID : {0}, 当前门 : {1}, 目标场景: {2}, 目标门ID {3}，isUseLoadingFrame = {4}", data.currSceneID, data.currDoorID, data.targetSceneID, data.targetDoorID,isUseLoadingFrame);
#endif

            DealWithExtraShowFrame();

            if (tableData != null && tableData.SceneType != CitySceneTable.eSceneType.DUNGEON_ENTRY)
            {
                if (tableData.SceneType != CitySceneTable.eSceneType.PK_PREPARE)
                {
                    //if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>() == false)
                    //{
                    //    ClientSystemManager.GetInstance().OpenFrame<ClientSystemTownFrame>();
                    //}
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SwitchToMainScene);
                }

                GameFrameWork.instance.TownNameShow(tableData.Name);
                GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.SceneChangedFinish);
            }

            if(bNeedHideBottomLayer)
            {
                if (ClientSystemManager.GetInstance().BottomLayer != null)
                {
                    ClientSystemManager.GetInstance().BottomLayer.CustomActive(true);
                }
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                if (null != Townframe)
                {
                    Townframe.StartFizzyCheckAndResumeJoystickMove();
                }
            }

            if (data.onSceneLoadFinish != null)
            {
                data.onSceneLoadFinish.Invoke();
            }

            Logger.LogProcessFormat("[城镇] 切换场景结束 当前场景ID : {0}, 当前门 : {1}, 目标场景: {2}, 目标门ID {3}", data.currSceneID, data.currDoorID, data.targetSceneID, data.targetDoorID);
            _isSwithScene = false;
            SetTownSceneSwitchState(false);

            if(data.targetSceneID == 5007)
            {
                Pk3v3CrossDataManager.GetInstance()._BindNetMsg();
                Pk3v3DataManager.GetInstance().UnBindNetMsg();
            }
            else
            {
                //Pk3v3CrossDataManager.GetInstance()._UnBindNetMsg();
                Pk3v3DataManager.GetInstance().BindNetMsg();
            }

#if STAT_EXTRA_INFO
            var endTime = Utility.GetCurrentTimeUnix();
            var duration = endTime - startTime;
            //Logger.LogErrorFormat("net scene sync:{0}s", duration);
            GameStatisticManager.GetInstance().DoStatSmallPackageInfo(GameStatisticManager.ExtraInfo.PASSDOOR_TOWN, duration.ToString());
#endif
            //如果从关卡选择界面返回 重新播放背景音乐
            if (data != null)
            {
                var targetTableScene = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(data.targetSceneID);
                if (targetTableScene != null)
                {
                    _InitializeBGM(targetTableScene.BGMPath, targetTableScene.EnvironmenBGMtPath);
                }
            }
        }

        /// <summary>
        /// DealWithExtraShowFrame
        /// </summary>
        private void DealWithExtraShowFrame()
        {
            if (PlayerBaseData.GetInstance().bPkClickVipCharge != true) return;

            PlayerBaseData.GetInstance().bPkClickVipCharge = false;
            var vipFrame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>() as VipFrame;
            if (vipFrame != null)
                vipFrame.SwitchPage(VipTabType.PAY);
        }

        /// <summary>
        /// DugenonEntry
        /// </summary>
        /// <param name="data"></param>
        /// <param name="enterSceneId"></param>
        private void ShowDugeonEntryFrame(SceneParams data, int enterSceneId)
        {
            _enterTownDoor = new DTownDoor
            {
                SceneID = data.currSceneID,
                DoorID = data.currDoorID,
                TargetSceneID = data.targetSceneID,
                TargetDoorID = data.targetDoorID,
            };

            _backTownDoor = new DTownDoor
            {
                SceneID = data.targetSceneID,
                DoorID = data.targetDoorID,
                TargetSceneID = data.currSceneID,
                TargetDoorID = data.currDoorID,
            };

            ChapterSelectFrame.SetSceneID(enterSceneId);
            if(enterSceneId == GuildDataManager.nGuildDungeonMapScenceID) // 公会副本有专门的关卡选择界面
            {
                CurrentSceneID = data.currSceneID;
                ClientSystemManager.GetInstance().CloseFrame<GuildArenaFrame>();
                ClientSystemManager.GetInstance().OpenFrame<GuildDungeonMapFrame>();
            }
            else
            {
            ClientSystemManager.instance.OpenFrame<ChapterSelectFrame>();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EnterDungeon);
            }
        }

        /// <summary>
        /// PK Frame
        /// </summary>
        /// <param name="tableData"></param>
        private void ShowPkPrePareFrame(CitySceneTable tableData)
        {
            switch (tableData.SceneSubType)
            {
                case CitySceneTable.eSceneSubType.TRADITION:
                    ClientSystemManager.GetInstance().bIsInPkWaitingRoom = true;
                    if (NewbieGuideManager.GetInstance().IsGuiding())
                    {
                        if (!NewbieGuideManager.GetInstance()
                            .IsGuidingTask(NewbieGuideTable.eNewbieGuideTask.DuelGuide))
                        {
                            NewbieGuideManager.GetInstance().ManagerFinishGuide(NewbieGuideManager.GetInstance().mGuideControl.GuideTaskID);
                        }
                    }
                    var pkWaitingRoomData = new PkWaitingRoomData
                    {
                        SceneSubType = tableData.SceneSubType,
                        CurrentSceneID = tableData.ID,
                        TargetTownSceneID = tableData.BirthCity,
                    };
                    ClientSystemManager.GetInstance().OpenFrame<PkWaitingRoom>(FrameLayer.Bottom, pkWaitingRoomData);
                    break;
                case CitySceneTable.eSceneSubType.BUDO:
                    var budoData = new BudoArenaFrameData
                    {
                        CurrentSceneID = tableData.ID,
                        TargetTownSceneID = tableData.BirthCity,
                    };
                    BudoArenaFrame.Open(budoData);
                    break;
                case CitySceneTable.eSceneSubType.MoneyRewards:
                    var moneyRewardsMainFrameData = new MoneyRewardsMainFrameData
                    {
                        citySceneItem = tableData,
                        CurrentSceneID = tableData.ID,
                        TargetTownSceneID = tableData.BirthCity,
                    };
                    MoneyRewardsMainFrame.CommandOpen(moneyRewardsMainFrameData);
                    break;
                case CitySceneTable.eSceneSubType.GuildBattle:
                    ClientSystemManager.GetInstance().OpenFrame<GuildBattleFrame>(FrameLayer.Bottom);
                    break;
                case CitySceneTable.eSceneSubType.CrossGuildBattle:
                    ClientSystemManager.GetInstance().OpenFrame<GuildBattleFrame>(FrameLayer.Bottom);
                    break;
                case CitySceneTable.eSceneSubType.Pk3v3:
                    var pk3V3WaittingRoomData = new PkWaitingRoomData
                    {
                        SceneSubType = tableData.SceneSubType,
                        CurrentSceneID = tableData.ID,
                        TargetTownSceneID = tableData.TraditionSceneID,
                    };
                    ClientSystemManager.GetInstance().OpenFrame<Pk3v3WaitingRoom>(FrameLayer.Bottom, pk3V3WaittingRoomData);
                    break;
                case CitySceneTable.eSceneSubType.CrossPk3v3:
                    var pk3V3WaittingRoomData1 = new PkWaitingRoomData
                    {
                        SceneSubType = tableData.SceneSubType,
                        CurrentSceneID = tableData.ID,
                        TargetTownSceneID = tableData.BirthCity,
                    };
                    ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossWaitingRoom>(FrameLayer.Bottom, pk3V3WaittingRoomData1);
                    {
                        ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossTeamMainFrame>(FrameLayer.Bottom);
                        ClientSystemManager.GetInstance().GetFrame(typeof(Pk3v3CrossTeamMainFrame)).GetFrame().CustomActive(true);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossUpdateMyTeamFrame);
                    }

                    ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossTeamMainMenuFrame>(FrameLayer.Bottom);
                    ClientSystemManager.GetInstance().GetFrame(typeof(Pk3v3CrossTeamMainMenuFrame)).GetFrame().CustomActive(true);

                    if(Pk3v3CrossDataManager.GetInstance().HasTeam())
                    {
                        ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossMyTeamFrame>();
                        ClientSystemManager.GetInstance().GetFrame(typeof(Pk3v3CrossMyTeamFrame)).GetFrame().CustomActive(true);
                    }

                    //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3CrossButton, "HideJoin3v3CrossBtn");
                    break;
                case CitySceneTable.eSceneSubType.Melee2v2Cross:
                    var RoomData = new PkWaitingRoomData
                    {
                        SceneSubType = tableData.SceneSubType,
                        CurrentSceneID = tableData.ID,
                        TargetTownSceneID = tableData.BirthCity,
                    };
                    ClientSystemManager.GetInstance().OpenFrame<Pk2v2CrossWaitingRoomFrame>(FrameLayer.Bottom, RoomData);
                    break;
                case CitySceneTable.eSceneSubType.FairDuelPrepare:
                    FairDueliRoomData roomData = new FairDueliRoomData
                    {
                        SceneSubType = tableData.SceneSubType,
                        CurrentSceneID = tableData.ID,
                        TargetTownSceneID = tableData.TraditionSceneID,
                    };
                    ClientSystemManager.GetInstance().OpenFrame<FairDuelWaitingRoomFrame>(FrameLayer.Bottom, roomData);
                    break;
                default:
                    break;
            }
        }

        //展示团本的界面
        private void ShowTeamDuplicationFrame(CitySceneTable tableData)
        {
            switch (tableData.SceneSubType)
            {
                case CitySceneTable.eSceneSubType.TeamDuplicationBuid:
                    TeamDuplicationUtility.OnOpenTeamDuplicationMainBuildFrame();
                    break;
                case CitySceneTable.eSceneSubType.TeamDuplicationFight:
                    TeamDuplicationUtility.OnOpenTeamDuplicationMainFightFrame();
                    break;
                default:
                    break;
            }
        }


        private ITownFadingFrame OpenTownFadingFrame(bool isUseLoadingFrame)
        {
            if (!isUseLoadingFrame)
            {
                return ClientSystemManager.GetInstance().OpenGlobalFrame<FadingFrame>(FrameLayer.Top) as ITownFadingFrame;
            }
            else
            {
                return ClientSystemManager.GetInstance().OpenGlobalFrame<TownLoadingFrame>(FrameLayer.Top) as ITownFadingFrame;
            }
        }

        void ClearBaseMainFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<PkWaitingRoom>();
            ClientSystemManager.GetInstance().CloseFrame<BudoArenaFrame>();
            ClientSystemManager.GetInstance().CloseFrame<GuildBattleFrame>();
            ClientSystemManager.GetInstance().CloseFrame<MoneyRewardsMainFrame>();
            ClientSystemManager.GetInstance().CloseFrame<Pk3v3WaitingRoom>();
            ClientSystemManager.GetInstance().CloseFrame<GuildArenaFrame>();
            ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationMainBuildFrame>();
            ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationMainFightFrame>();
        }

        // 清理场景
        protected void ClearScene(bool bNeedGC = true)
        {
            // 清空角色
            List<BeBaseActor> actors = BeBaseActor.Actors;
            while (actors.Count > 0)
            {
                actors[0].Dispose();
            }
            _beTownNPCs.Clear();
            _beTownPlayerMain = null;
            _beTownDoors.Clear();
            _beTownPlayers.Clear();
            _beTownAttackCityMonsters.Clear();

            _beDisplayPlayerList.Clear();
            _beDisplayNum = 0;

            // 清空场景
            _levelData = null;
            GridInfo = null;
            if (_geScene != null)
            {
                _geScene.UnloadScene(bNeedGC);
                _geScene = null;
            }

            // 清空相机滚动脚本
            //_scrollCamera = null;

            _inputTimeElapsed = 0.0f;
            CurrentSceneID = -1;

            _UnBindSceneNetMsgs();

            _UnRegisterEvent();

            _OnClearDelayCreateCache();
        }

        // 更新
        protected void UpdateScene(float timeElapsed)
        {
            /// 缓冲加载列表
            List<BeBaseActor> actors = BeBaseActor.Actors;
            for (int i = 0; i < actors.Count; ++i)
            {
                var baseActor = actors[i];
                if (baseActor != null)
                {
                    baseActor.Update(timeElapsed);
                }
            }

            if (_geScene != null)
            {
                _geScene.Update((int)(timeElapsed * (float)GlobalLogic.VALUE_1000));
            }

            if (_beTownPlayerMain != null)
            {
                PlayerBaseData.GetInstance().Pos = _beTownPlayerMain.ActorData.MoveData.ServerPosition;
                PlayerBaseData.GetInstance().FaceRight = _beTownPlayerMain.ActorData.MoveData.FaceRight;
            }
        }
#endregion

        public void _InitializeCameraController()
        {
            if (_beTownPlayerMain == null)
            {
                Logger.LogError("_InitializeCameraController ==> _beTownPlayerMain == null");
                return;
            }

            if (_levelData == null)
            {
                Logger.LogError("_InitializeCameraController ==> _levelData == null");
                return;
            }

            _geScene.GetCamera().GetController().AttachTo(_beTownPlayerMain.GraphicActor, _levelData.GetCameraLookHeight(), _levelData.GetCameraAngle(), _levelData.GetCameraDistance());
            _geScene.initScrollScript();
        }

        protected void _InitializeBGM(string path, string environmentBGMPath=null)
        {
            Logger.LogProcessFormat("player bgm with path {0}", path);

            if (!string.IsNullOrEmpty(path))
                m_BgmHandle = AudioManager.instance.PlaySound(path, AudioType.AudioStream, 1, true);

            if (!string.IsNullOrEmpty(environmentBGMPath))
                m_EnvGgmHandle = AudioManager.instance.PlaySound(environmentBGMPath, AudioType.AudioEnvironment, 1, true);
        }

        private void CreateSceneObjects()
        {
            //创建场景NPC
            CreateSceneNpcs();

            //创建主角
            CreateMainPlayer();

            // 创建其他玩家
            CreateSceneOtherPlayers();

            // 创建传送门
            CreateSceneDoors();

            //创建宠物，注释掉
            //CreateScenePets();

            //创建攻城怪物
            CreateAttackCityMonsters();

            //创建黑市商人
            CreatBlackMarketMerchant();
        }

        private void CreateMainPlayer()
        {
            Vec3 speed = Global.Settings.townPlayerRun ? Utility.IRepeate2Vector(TableManager.instance.gst.townRunSpeed) : Utility.IRepeate2Vector(TableManager.instance.gst.townWalkSpeed);
            PetInfo petInfo = PetDataManager.GetInstance().GetFollowPet();
            BeTownPlayerData data = new BeTownPlayerData
            {
                GUID = PlayerBaseData.GetInstance().RoleID,
                Name = PlayerBaseData.GetInstance().Name,
                JobID = PlayerBaseData.GetInstance().JobTableID,
                RoleLv = PlayerBaseData.GetInstance().Level,
                pkRank = SeasonDataManager.GetInstance().seasonLevel,
                NameColor = PlayerInfoColor.TOWN_PLAYER,
                tittle = PlayerBaseData.GetInstance().iTittle,
                GuildPost = PlayerBaseData.GetInstance().guildDuty,
                GuildName = PlayerBaseData.GetInstance().guildName,
                petID = petInfo != null ? (int)petInfo.dataId : 0,
                ZoneID = PlayerBaseData.GetInstance().ZoneID,
                AdventureTeamName = AdventureTeamDataManager.GetInstance().GetAdventureTeamName(),
                WearedTitleInfo=PlayerBaseData.GetInstance().WearedTitleInfo,
                GuildEmblemLv = (int)PlayerBaseData.GetInstance().GuildEmblemLv
            };
            data.MoveData.PosLogicToGraph = _logicToGraph;
            data.MoveData.PosServerToClient = _serverToClient;
            data.MoveData.ServerPosition = PlayerBaseData.GetInstance().Pos;
            data.MoveData.FaceRight = PlayerBaseData.GetInstance().FaceRight;
            data.MoveData.MoveSpeed = new Vector3(speed.x, speed.z, speed.y) * PlayerBaseData.GetInstance().MoveSpeedRate;
            //data.iFollowTableID = PlayerBaseData.GetInstance().iFollowTableID;
            //data.iFollowPetLv = PlayerBaseData.GetInstance().iFollowPetLv;


            _beTownPlayerMain = new BeTownPlayerMain(data, this);
            _beTownPlayerMain.InitGeActor(_geScene);
            //_beTownPlayerMain.EquipWeapon(0);

            /*
            int wid = 0;
            int strengthenLevel = 0;
            ulong uwid = ItemDataManager.GetInstance().GetWearEquipBySlotType(EEquipWearSlotType.EquipWeapon);
            if (uwid > 0)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(uwid);
                if (itemData != null)
                {
                    wid = (int)itemData.TableID;
                    strengthenLevel = (int)itemData.StrengthenLevel;
                }
            }

            _beTownPlayerMain.EquipWeapon(wid, strengthenLevel);
            */

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FunctionFrameUpdate);

            //显示正在寻路提示
            BeTownPlayerMain.OnMoveing.RemoveListener(_onMainPlayerMoveing);
            BeTownPlayerMain.OnMoveing.AddListener(_onMainPlayerMoveing);

            BeTownPlayerMain.OnAutoMoving.RemoveListener(_onMainPlayerAutoMoving);
            BeTownPlayerMain.OnAutoMoving.AddListener(_onMainPlayerAutoMoving);

            BeTownPlayerMain.OnAutoMoveFail.RemoveListener(_onMainPlayerAutoMoveFail);
            BeTownPlayerMain.OnAutoMoveFail.AddListener(_onMainPlayerAutoMoveFail);

            BeTownPlayerMain.OnAutoMoveSuccess.RemoveListener(_onMainPlayerAutoMoveSucc);
            BeTownPlayerMain.OnAutoMoveSuccess.AddListener(_onMainPlayerAutoMoveSucc);

            //创建已经出战的随从
            //FollowPetManager.Instance().AddFollowPetForPlayer(data.GUID);
        }

        private void _SetMainPlayerShowFindPath(bool isShow)
        {
            if (_beTownPlayerMain != null && _beTownPlayerMain.GraphicActor != null)
                _beTownPlayerMain.GraphicActor.ShowFindPath(isShow);
        }
        private void _onMainPlayerAutoMoveSucc()
        {
            _SetMainPlayerShowFindPath(false);
            BeTownPlayerMain.ResetOnAutoMoveTargetData();
        }
        private void _onMainPlayerAutoMoveFail()
        {
            _SetMainPlayerShowFindPath(false);

            BeTownPlayerMain.ResetOnAutoMoveTargetData();
        }
        private void _onMainPlayerAutoMoving(Vector3 pos)
        {
            _SetMainPlayerShowFindPath(true);
        }
        private void _onMainPlayerMoveing(Vector3 pos)
        {
            _SetMainPlayerShowFindPath(false);
        }

        private void TriggerMainPlayerAutoMoveEvent()
        {
            //自动寻路关掉 我要变强 和 每日任务界面
            if(ClientSystemManager.GetInstance().IsFrameOpen<DevelopGuidanceMainFrame>())
                ClientSystemManager.GetInstance().CloseFrame<DevelopGuidanceMainFrame>();
            if(ClientSystemManager.GetInstance().IsFrameOpen<MissionFrameNew>())
                ClientSystemManager.GetInstance().CloseFrame<MissionFrameNew>();
        }

        private void DestroyMainPlayer()
        {
            if (_beTownPlayerMain != null)
            {
                _beTownPlayerMain.Dispose();
                _beTownPlayerMain = null;
            }
        }

        private void CreateSceneNpcs()
        {
            Logger.LogAsset("Create NPC!");
            for (var i = 0; i < _levelData.GetNpcInfoLength(); ++i)
            {
                var info = _levelData.GetNpcInfo(i);

                var npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(info.GetEntityInfo().GetResid());
                if (npcItem == null)
                {
                    Logger.LogErrorFormat("[npc]data.NpcID = {0} is not existed! to 国庆!", info.GetEntityInfo().GetResid());
                    continue;
                }

                if(npcItem.Function == NpcTable.eFunction.guildDungeonActivityChest && !GuildDataManager.GetInstance().IsShowChestModel())
                {
                    continue;
                }
                var data = new BeTownNPCData
                {
                    NpcID = info.GetEntityInfo().GetResid(),
                    ResourceID = 0,
                    Name = npcItem.NpcName,
                    NameColor = PlayerInfoColor.TOWN_NPC
                };

                bool bNeedSetRotation = false;
                int RotationY = 0;

                if (npcItem.Function == NpcTable.eFunction.Townstatue || npcItem.Function == NpcTable.eFunction.guildGuardStatue)
                {
                    List<FigureStatueInfo> StatueInfoList = new List<FigureStatueInfo>();
                if (npcItem.Function == NpcTable.eFunction.Townstatue)
                {
                        StatueInfoList = GuildDataManager.GetInstance().GetTownStatueInfo();
                    }
                    else if(npcItem.Function == NpcTable.eFunction.guildGuardStatue)
                    {
                        StatueInfoList = GuildDataManager.GetInstance().GetGuildGuardStatueInfo();
                    }                   

                    bool bFind = false;
                    for (int j = 0; j < StatueInfoList.Count; j++)
                    {
                        FigureStatueInfo townStatueInfo = StatueInfoList[j];

                        if (townStatueInfo.statueType != (byte)npcItem.SubType)
                        {
                            continue;
                        }

                        JobTable job = TableManager.instance.GetTableItem<JobTable>(townStatueInfo.occu);
                        if (job == null)
                        {
                            Logger.LogErrorFormat("can not find JobTable with TownStatue NPC occu ID:{0} when InitTown", townStatueInfo.occu);
                            continue;
                        }
                        else
                        {
                            ResTable res = TableManager.instance.GetTableItem<ResTable>(job.Mode);

                            if (res == null)
                            {
                                Logger.LogErrorFormat("can not find ResTable with TownStatue NPC mod id:{0} when InitTown", job.Mode);
                                continue;
                            }
                            else
                            {
                                data.ResourceID = res.ID;
                                data.JobID = townStatueInfo.occu;
                                data.avatorInfo = townStatueInfo.avatar;
                                data.StatueName = townStatueInfo.name;

                                bNeedSetRotation = true;
                                RotationY = job.TownStatueFace;
                            }
                        }
                       
                        bFind = true;
                        break;
                    }

                    if (!bFind)
                    {
                        data.ResourceID = npcItem.ResID;
                    }
                }
                else
                {
                    data.ResourceID = npcItem.ResID;
                }

                data.MoveData.PosLogicToGraph = _logicToGraph;
                data.MoveData.PosServerToClient = _serverToClient;
                data.MoveData.Position = info.GetEntityInfo().GetPosition();

                var beTownNpc = new BeTownNPC(data, this);

                beTownNpc.InitGeActor(_geScene);
                if (beTownNpc.GraphicActor != null)
                {
                    beTownNpc.GraphicActor.SetScale(info.GetEntityInfo().GetScale());
                    if (bNeedSetRotation)
                    {
                        Quaternion newRotation = Quaternion.Euler(0, RotationY, 0);
                        beTownNpc.GraphicActor.SetRotation(newRotation);
                    }
                }

                beTownNpc.AddActorPostLoadCommand(() =>
                {
                    OnNPCLoadFinished(data.NpcID, beTownNpc.GraphicActor);
                });

                _beTownNPCs.Add(beTownNpc);
            }
            //如果周年派对活动结束了 删除该NPC
            OpActivityData opActivityData= ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_ANNIBERPARTY);
            uint curTime= TimeManager.GetInstance().GetServerTime();
            if(opActivityData!=null)
            {
                uint startTime= opActivityData.startTime;
                uint endTime = opActivityData.endTime;
                if(curTime<startTime||curTime>endTime)
                {
                    DisposeNPC(NpcTable.eFunction.AnniersaryParty);
                }
            }
            else
            {
                DisposeNPC(NpcTable.eFunction.AnniersaryParty);
            }
           
         

            /// //为所有NPC加入对话监听
            /// AddDialogListenerForAllNpc();
            /// //为所有功能Npc加入功能监听
            /// AddFunctionListenerForAllNpc();
            /// //对于任务类Npc监听由MissionManager 根据服务器消息加入
            /// 
            /// //为所有NPC打开随机声音播放
            /// AddVoiceListenerForAllNpc();
        }

        
        /// <summary>
        /// 创建周年庆派对活动NPC
        /// </summary>
        private void CreateNPC(NpcTable.eFunction function)
        { 
            for (var i = 0; i < _levelData.GetNpcInfoLength(); ++i)
            {
                var info = _levelData.GetNpcInfo(i);

                var npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(info.GetEntityInfo().GetResid());
                if (npcItem == null)
                {
                    continue;
                }

                if (npcItem.Function != function)
                {
                    continue;
                }

                var data = new BeTownNPCData
                {
                    NpcID = info.GetEntityInfo().GetResid(),
                    ResourceID = 0,
                    Name = npcItem.NpcName,
                    NameColor = PlayerInfoColor.TOWN_NPC
                };

                bool bNeedSetRotation = false;
                int RotationY = 0;
                data.ResourceID = npcItem.ResID;

                data.MoveData.PosLogicToGraph = _logicToGraph;
                data.MoveData.PosServerToClient = _serverToClient;
                data.MoveData.Position = info.GetEntityInfo().GetPosition();

                var beTownNpc = new BeTownNPC(data, this);

                beTownNpc.InitGeActor(_geScene);
                if (beTownNpc.GraphicActor != null)
                {
                    beTownNpc.GraphicActor.SetScale(info.GetEntityInfo().GetScale());
                    if (bNeedSetRotation)
                    {
                        Quaternion newRotation = Quaternion.Euler(0, RotationY, 0);
                        beTownNpc.GraphicActor.SetRotation(newRotation);
                    }
                }

                beTownNpc.AddActorPostLoadCommand(() =>
                {
                    OnNPCLoadFinished(data.NpcID, beTownNpc.GraphicActor);
                });
                if(!_beTownNPCs.Contains(beTownNpc))
                {
                    _beTownNPCs.Add(beTownNpc);
                }
              

            }
        }
        /// <summary>
        /// 释放NPC
        /// </summary>
        /// <param name="eFunction"></param>
        private void DisposeNPC(NpcTable.eFunction eFunction)
        {
            BeTownNPC beTownNPC = null;
            for (int i = 0; i < _beTownNPCs.Count; i++)
            {
                BeTownNPCData beTownNPCData = _beTownNPCs[i].ActorData as BeTownNPCData;
                var npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(beTownNPCData.NpcID);
                if (npcItem.Function == eFunction)
                {
                    beTownNPC = _beTownNPCs[i];
                    break;

                }
            }
            if(beTownNPC!=null&& _beTownNPCs.Contains(beTownNPC))
            {
                _beTownNPCs.Remove(beTownNPC);
                beTownNPC.Dispose();
            }
        }
        private void CreateSceneOtherPlayers()
        {
            if (PlayerOthersData == null)
                return;

            // ReSharper disable once GenericEnumeratorNotDisposed
            var enumerator = PlayerOthersData.GetEnumerator();

            while (enumerator.MoveNext())
            {
                SceneObject sceneObj = enumerator.Current.Value;
                BeTownPlayer beTownPlayer = _CreateTownPlayer(sceneObj, CurrentSceneID);
                if (beTownPlayer != null)
                {
                    _beTownPlayers.Add(beTownPlayer.ActorData.GUID, beTownPlayer);
                    _AddDisplayActor(beTownPlayer.ActorData.GUID);

                    beTownPlayer.Init3v3RoomPlayerJiaoDiGuangQuan();
                }
            }
        }

        private void CreateSceneDoors()
        {
            for (var i = 0; i < _levelData.GetTownDoorLength(); ++i)
            {
                ISceneTownDoorData info = _levelData.GetTownDoor(i);

                BeTownDoorData data = new BeTownDoorData();
                data.MoveData.Position = info.GetRegionInfo().GetEntityInfo().GetPosition();
                data.Door = info;

                BeTownDoor door = new BeTownDoor(data, this);
                door.InitGeActor(_geScene);
                door.OnTrigger.AddListener((ISceneTownDoorData doorData) =>
                {
                    // 组队时，队员不需要进关卡选择界面
                    //                             ProtoTable.CitySceneTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.CitySceneTable>(doorData.TargetSceneID);
                    //                             if (tableData != null)
                    //                             {
                    //                                 if (tableData.SceneType == ProtoTable.CitySceneTable.eSceneType.DUNGEON_ENTRY)
                    //                                 {
                    //                                     if (TeamDataManager.GetInstance().HasTeam() && TeamDataManager.GetInstance().IsTeamLeader() == false)
                    //                                     {
                    //                                         SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_cannot_enter_single"));
                    //                                         return;
                    //                                     }
                    //                                 }
                    //                             }
                    //doorData.GetSce
                    if(doorData.GetDoorTargetType() == DoorTargetType.Normal)
                    {
                        var sceneParams = new SceneParams(doorData.GetSceneID(),
                        doorData.GetDoorID(),
                        doorData.GetTargetSceneID(),
                        doorData.GetTargetDoorID())
                        {
                            type = eSceneChangeType.eDungeonChapterSelect,
                            birthPostion = doorData.GetBirthPos(),
                        };

                        GuildDataManager.GuildDungeonActivityData activityData = GuildDataManager.GetInstance().GetGuildDungeonActivityData();

                        bool bChangeScene = true;
                        if (doorData != null && doorData.GetTargetSceneID() == GuildDataManager.nGuildDungeonMapScenceID)
                        {
                            if (!GuildDataManager.CheckActivityLimit())
                            {
                                bChangeScene = false;
                            }
                            else if (activityData != null && activityData.nActivityState != (int)GuildDungeonStatus.GUILD_DUNGEON_START)
                            {
                                bChangeScene = false;
                            }
                        }

                        if (bChangeScene)
                        {
                            GameFrameWork.instance.StartCoroutine(_NetSyncChangeScene(sceneParams));
                        }
                    }
                    else if(doorData.GetDoorTargetType() == DoorTargetType.PVEPracticeCourt)
                    {
                        if(TeamDataManager.GetInstance().HasTeam())
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_cannot_enter_pve_training"));
                        }
                        else
                        {
                            //GameFrameWork.instance.StartCoroutine(_sendSceneDungeonStart(60));
                            BattleMain.OpenBattle(BattleType.TrainingPVE, eDungeonMode.LocalFrame, 0, "1000");
                            ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();
                        }
                    }
                });
                _beTownDoors.Add(door);
            }
        }

        protected BeTownPlayer _CreateTownPlayer(SceneObject sceneObj, int a_nSceneID)
        {
            BeTownPlayer beTownPlayer = null;
            try
            {
                if (sceneObj.sceneId != a_nSceneID)
                {
                    Logger.LogWarningFormat("创建其他玩家：当前场景ID->{0},目标场景ID->{1}", a_nSceneID, sceneObj.sceneId);
                    return null;
                }

                if (sceneObj.dir == null)
                {
                    Logger.LogWarning("_CreateTownPlayer ==>> sceneObj.dir is nil");
                    return null;
                }
                if (sceneObj.pos == null)
                {
                    Logger.LogWarning("_CreateTownPlayer ==>> sceneObj.pos is nil");
                    return null;
                }

                // 准备数据
                BeTownPlayerData data = new BeTownPlayerData
                {
                    GUID = sceneObj.id,
                    Name = sceneObj.name,
                    NameColor = PlayerInfoColor.TOWN_OTHER_PLAYER,
                    JobID = sceneObj.occu,
                    RoleLv = sceneObj.level,
                    pkRank = (int)sceneObj.seasonLevel,
                    pkStar = (int)sceneObj.seasonStar,
                    tittle = sceneObj.title,
                    State = (int)sceneObj.state,
                    petID = (int)sceneObj.followPetDataId,
                    avatorInfo = sceneObj.avatar,
                    GuildName = sceneObj.guildName,
                    GuildPost = sceneObj.guildPost,
                    ZoneID = (int)sceneObj.zoneId,
                    AdventureTeamName = sceneObj.adventureTeamName,
                    WearedTitleInfo = sceneObj.wearedTitleInfo,
                    GuildEmblemLv = (int)sceneObj.guildEmblemLvl
            };
                if (sceneObj.vip != null)
                    data.vip = sceneObj.vip.level;
                data.MoveData.PosLogicToGraph = _logicToGraph;
                data.MoveData.PosServerToClient = _serverToClient;
                data.MoveData.ServerPosition = new Vector3(sceneObj.pos.x / _axisScale, 0.0f, sceneObj.pos.y / _axisScale);
                data.MoveData.FaceRight = sceneObj.dir.faceRight >= 1;
                //data.iFollowTableID = sceneObj.sceneRetinue.id;
                //data.iFollowPetLv = sceneObj.sceneRetinue.level;
                Vec3 speed = Global.Settings.townPlayerRun ? Utility.IRepeate2Vector(TableManager.instance.gst.townRunSpeed) : Utility.IRepeate2Vector(TableManager.instance.gst.townWalkSpeed);
                data.MoveData.MoveSpeed = new Vector3(speed.x, speed.z, speed.y) * (sceneObj.moveSpeed / (float)GlobalLogic.VALUE_1000);
                data.MoveData.TargetDirection = new Vector3(sceneObj.dir.x / _axisScale, 0.0f, sceneObj.dir.y / _axisScale);
                // 创建其他玩家
                beTownPlayer = new BeTownPlayer(data, this);

            }
            catch (Exception e)
            {
                Logger.LogError("TownPlayer Create Error!");
            }


            /// beTownPlayer.InitGeActor(_geScene);
            //beTownPlayer.EquipWeapon();

            return beTownPlayer;
        }

        public BeTownNPC GetTownNpcByNpcId(Int32 iNpcId)
        {
            for (var i = 0; i < _beTownNPCs.Count; ++i)
            {
                if (_beTownNPCs[i] == null)
                {
                    continue;
                }

                var npcData = _beTownNPCs[i].ActorData as BeTownNPCData;
                if (npcData == null)
                {
                    continue;
                }

                if (npcData.NpcID == iNpcId)
                {
                    return _beTownNPCs[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 为闲蛋类NPC加入闲谈声音
        /// </summary>
        public void AddVoiceListenerForAllNpc()
        {
            for (int i = 0; i < _beTownNPCs.Count; ++i)
            {
                if (_beTownNPCs[i] != null && _beTownNPCs[i].GraphicActor != null)
                {
                    var npcData = _beTownNPCs[i].ActorData as BeTownNPCData;
                    if (npcData == null)
                    {
                        continue;
                    }

                    if (_beTownNPCs[i].GraphicActor != null)
                    {
                        _beTownNPCs[i].GraphicActor.AddVoiceListener(NpcVoiceComponent.SoundEffectType.SET_Random);
                    }
                }
            }
        }

        public void PlayNpcSound(Int32 iNpcID, NpcVoiceComponent.SoundEffectType eSoundEffect)
        {
            for (int i = 0; i < _beTownNPCs.Count; ++i)
            {
                if (_beTownNPCs[i] != null && _beTownNPCs[i].GraphicActor != null)
                {
                    var npcData = _beTownNPCs[i].ActorData as BeTownNPCData;
                    if (npcData == null)
                    {
                        continue;
                    }

                    if (npcData.NpcID != iNpcID)
                    {
                        continue;
                    }

                    GameObject goActor = _beTownNPCs[i].GraphicActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
                    if (goActor == null)
                    {
                        continue;
                    }

                    Transform voiceParent = goActor.transform.Find("VoiceParent");
                    if (voiceParent == null)
                    {
                        continue;
                    }

                    var voiceComponent = voiceParent.GetComponent<NpcVoiceComponent>();
                    if (voiceComponent != null)
                    {
                        voiceComponent.PlaySound(eSoundEffect);
                    }
                }
            }
        }

        /// <summary>
        /// 为闲蛋类NPC加入闲谈监听器
        /// </summary>
        public void AddDialogListenerForAllNpc()
        {
            for (int i = 0; i < _beTownNPCs.Count; ++i)
            {
                if (_beTownNPCs[i] != null && _beTownNPCs[i].GraphicActor != null)
                {
                    var npcData = _beTownNPCs[i].ActorData as BeTownNPCData;
                    if (npcData == null)
                    {
                        continue;
                    }

                    AddDialogListener(npcData.NpcID);
                }
            }
        }

        public void AddDialogListener(Int32 iNpcID)
        {
            //var townNpc = GetTownNpcByNpcId(iNpcID);
            //if (townNpc == null)
            //{
            //    return;
            //}

            //var npcData = townNpc.ActorData as BeTownNPCData;
            //if (npcData == null)
            //{
            //    return;
            //}

            //ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iNpcID);
            //if (npcItem != null && npcItem.Function == ProtoTable.NpcTable.eFunction.none)
            //{
            //    if (string.IsNullOrEmpty(npcItem.NpcTalk) == false)
            //    {
            //        Int32 dialogID = Int32.Parse(npcItem.NpcTalk);
            //        ProtoTable.TalkTable talkItem = TableManager.GetInstance().GetTableItem<ProtoTable.TalkTable>(dialogID);
            //        if (talkItem != null)
            //        {
            //            GameObject goRoot = townNpc.GraphicActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
            //            NpcInteraction scriptNpcInteraction = goRoot.transform.FindChild("PlayerInfo_Head").gameObject.GetComponent<NpcInteraction>();
            //            if (scriptNpcInteraction != null)
            //            {
            //                scriptNpcInteraction.AddDialogListener(dialogID);
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 为任务类NPC加入任务监听
        /// </summary>

        public void AddMissionListenerForNpc(Int32 iNpcID, Int32 iTaskID)
        {
            //var townNpc = GetTownNpcByNpcId(iNpcID);
            //if (townNpc != null && townNpc.GraphicActor != null)
            //{
            //    ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iNpcID);
            //    if (npcItem != null && npcItem.Function == ProtoTable.NpcTable.eFunction.none)
            //    {
            //        townNpc.GraphicActor.PushPostLoadCommand(() =>
            //        {
            //            GameObject goRoot = townNpc.GraphicActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
            //            NpcInteraction scriptNpcInteraction = goRoot.transform.FindChild("PlayerInfo_Head").gameObject.GetComponent<NpcInteraction>();
            //            scriptNpcInteraction.AddMissionListener(iTaskID);
            //        });

            //    }
            //    return;
            //}
        }

        /// <summary>
        /// 为功能类NPC加入任务监听
        /// </summary>
        public void AddFunctionListenerForAllNpc()
        {
            for (int i = 0; i < _beTownNPCs.Count; ++i)
            {
                if (_beTownNPCs[i] != null && _beTownNPCs[i].GraphicActor != null)
                {
                    var npcData = _beTownNPCs[i].ActorData as BeTownNPCData;
                    if (npcData == null)
                    {
                        continue;
                    }

                    ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(npcData.NpcID);
                    if (npcItem != null && npcItem.Function != ProtoTable.NpcTable.eFunction.none && npcItem.Function < ProtoTable.NpcTable.eFunction.clicknpc)
                    {
                        GameObject goRoot = _beTownNPCs[i].GraphicActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);

                        if (goRoot == null)
                            continue;

                        Transform NPCInteractionTM = goRoot.transform.Find("PlayerInfo_Head");
                        if (null == NPCInteractionTM)
                            continue;

                        NpcInteraction scriptNpcInteraction = NPCInteractionTM.gameObject.GetComponent<NpcInteraction>();
                        //scriptNpcInteraction.AddFunctionListener(npcItem.Function);
                    }
                }
            }
        }

        /// <summary>
        /// 更改某个玩家的称号
        /// parameter iPlayerID ,value=0 means mainplayer
        /// </summary>
        public void OnNotifyTownPlayerLvChanged(UInt32 iPlayerID, Int32 iLevel)
        {
            if (MainPlayer != null)
            {
                BeTownPlayerData curMainData = MainPlayer.ActorData as BeTownPlayerData;
                if (curMainData != null && (curMainData.GUID == iPlayerID || iPlayerID == 0))
                {
                    if (MainPlayer.GraphicActor != null)
                    {
                        MainPlayer.GraphicActor.OnLevelChanged(iLevel);
                        Logger.LogProcessFormat("[Tittle] MainRole TittleChanged!");
                    }
                    return;
                }
            }

            BeTownPlayer curPlayer = null;
            if (_beTownPlayers.TryGetValue(iPlayerID, out curPlayer))
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    curPlayer.GraphicActor.OnLevelChanged(PlayerBaseData.GetInstance().Level);
                    Logger.LogProcessFormat("[Tittle] OtherPlayer TittleChanged!");
                }
            }
        }

        public void OnNotifyTownPlayerTittleChanged(UInt32 iPlayerID, Int32 iTittle)
        {
            if (MainPlayer != null)
            {
                BeTownPlayerData curMainData = MainPlayer.ActorData as BeTownPlayerData;
                if (curMainData != null && (curMainData.GUID == iPlayerID || iPlayerID == 0))
                {
                    if (MainPlayer.GraphicActor != null)
                    {
                        MainPlayer.GraphicActor.OnTittleChanged(iTittle);
                        Logger.LogProcessFormat("[Tittle] MainRole TittleChanged!");
                    }
                    return;
                }
            }

            BeTownPlayer curPlayer = null;
            if (_beTownPlayers.TryGetValue(iPlayerID, out curPlayer))
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    curPlayer.GraphicActor.OnTittleChanged(iTittle);
                    Logger.LogProcessFormat("[Tittle] OtherPlayer TittleChanged!");
                }
            }
        }

        //         public void OnNotifyTownPlayerPkPointsChanged(UInt32 iPlayerID, Int32 iPkPoints)
        //         {
        //             if (MainPlayer != null)
        //             {
        //                 BeTownPlayerData curMainData = MainPlayer.ActorData as BeTownPlayerData;
        //                 if (curMainData != null && (curMainData.GUID == iPlayerID || iPlayerID == 0))
        //                 {
        //                     if (MainPlayer.GraphicActor != null)
        //                     {
        //                         MainPlayer.GraphicActor.UpdatePkRank(iPkPoints);
        //                         Logger.LogProcessFormat("[Tittle] MainRole TittleChanged!");
        //                     }
        //                     return;
        //                 }
        //             }
        // 
        //             BeTownPlayer curPlayer = null;
        //             if (_beTownPlayers.TryGetValue(iPlayerID, out curPlayer))
        //             {
        //                 if (curPlayer != null && curPlayer.GraphicActor != null)
        //                 {
        //                     curPlayer.GraphicActor.UpdatePkRank(iPkPoints);
        //                     Logger.LogProcessFormat("[Tittle] OtherPlayer TittleChanged!");
        //                 }
        //             }
        //         }

        public void OnNotifyTownPlayerGuildNameChanged(UInt32 iPlayerID, string name)
        {
            if (MainPlayer != null)
            {
                BeTownPlayerData curMainData = MainPlayer.ActorData as BeTownPlayerData;
                if (curMainData != null && (curMainData.GUID == iPlayerID || iPlayerID == 0))
                {
                    if (MainPlayer.GraphicActor != null)
                    {
                        MainPlayer.GraphicActor.OnGuildNameChanged(name);
                        Logger.LogProcessFormat("[Tittle] MainRole GuildName Changed!");
                    }
                    return;
                }
            }

            BeTownPlayer curPlayer = null;
            if (_beTownPlayers.TryGetValue(iPlayerID, out curPlayer))
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    curPlayer.GraphicActor.OnGuildNameChanged(name);
                    Logger.LogProcessFormat("[Tittle] OtherPlayer GuildName TittleChanged!");
                }
            }
        }

        public void OnNotifyTownPlayerAdventTeamNameChanged(UInt32 iPlayerID, string name)
        {
            if (MainPlayer != null)
            {
                BeTownPlayerData curMainData = MainPlayer.ActorData as BeTownPlayerData;
                if (curMainData != null && (curMainData.GUID == iPlayerID || iPlayerID == 0))
                {
                    if (MainPlayer.GraphicActor != null)
                    {
                        MainPlayer.GraphicActor.UpdateAdventTeamName(name);
                        Logger.LogProcessFormat("[Tittle] MainRole AdventTeam Changed!");
                    }
                    return;
                }
            }

            BeTownPlayer curPlayer = null;
            if (_beTownPlayers.TryGetValue(iPlayerID, out curPlayer))
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    curPlayer.GraphicActor.UpdateAdventTeamName(name);
                    Logger.LogProcessFormat("[Tittle] OtherPlayer AdventTeam TittleChanged!");
                }
            }
        }

        public void OnNotifyTownPlayerTitleNameChanged(UInt32 iPlayerID, Protocol.PlayerWearedTitleInfo playerWearedTitleInfoe)
        {
            if (MainPlayer != null)
            {
                BeTownPlayerData curMainData = MainPlayer.ActorData as BeTownPlayerData;
                if (curMainData != null && (curMainData.GUID == iPlayerID || iPlayerID == 0))
                {
                    if (MainPlayer.GraphicActor != null)
                    {
                        MainPlayer.GraphicActor.UpdateTitleName(playerWearedTitleInfoe);
                        Logger.LogProcessFormat("[Tittle] MainRole Title Changed!");
                    }
                    return;
                }
            }

            BeTownPlayer curPlayer = null;
            if (_beTownPlayers.TryGetValue(iPlayerID, out curPlayer))
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    curPlayer.GraphicActor.UpdateTitleName(playerWearedTitleInfoe);
                    Logger.LogProcessFormat("[Tittle] MainRole Title Changed!");
                }
            }
        }

        public void OnNotifyTownPlayerNameChanged(UInt32 iPlayerID, string name)
        {
            if (MainPlayer != null)
            {
                BeTownPlayerData curMainData = MainPlayer.ActorData as BeTownPlayerData;
                if (curMainData != null && (curMainData.GUID == iPlayerID || iPlayerID == 0))
                {
                    if (MainPlayer.GraphicActor != null)
                    {
                        MainPlayer.GraphicActor.UpdateName(name);
                        Logger.LogProcessFormat("[Tittle] MainRole Name Changed!");
                    }
                    return;
                }
            }

            BeTownPlayer curPlayer = null;
            if (_beTownPlayers.TryGetValue(iPlayerID, out curPlayer))
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    curPlayer.GraphicActor.UpdateName(name);
                    Logger.LogProcessFormat("[Tittle] MainRole Name Changed!");
                }
            }
        }

        public void OnNotifyTownPlayerGuildLvChanged(UInt32 iPlayerID, uint guildEmblemLv)
        {
            if (MainPlayer != null)
            {
                BeTownPlayerData curMainData = MainPlayer.ActorData as BeTownPlayerData;
                if (curMainData != null && (curMainData.GUID == iPlayerID || iPlayerID == 0))
                {
                    if (MainPlayer.GraphicActor != null)
                    {
                        MainPlayer.GraphicActor.UpdateGuidLv((int)guildEmblemLv);
                        Logger.LogProcessFormat("[Tittle] MainRole guildEmblemLv Changed!");
                    }
                    return;
                }
            }

            BeTownPlayer curPlayer = null;
            if (_beTownPlayers.TryGetValue(iPlayerID, out curPlayer))
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    curPlayer.GraphicActor.UpdateGuidLv((int)guildEmblemLv);
                    Logger.LogProcessFormat("[Tittle] MainRole guildEmblemLv Changed!");
                }
            }
        }

        public void OnNotifyTownPlayerGuildDutyChanged(UInt32 iPlayerID, byte duty)
        {
            if (MainPlayer != null)
            {
                BeTownPlayerData curMainData = MainPlayer.ActorData as BeTownPlayerData;
                if (curMainData != null && (curMainData.GUID == iPlayerID || iPlayerID == 0))
                {
                    if (MainPlayer.GraphicActor != null)
                    {
                        MainPlayer.GraphicActor.OnGuildPostChanged(duty);
                        Logger.LogProcessFormat("[Tittle] MainRole Duty Changed!");
                    }
                    return;
                }
            }

            BeTownPlayer curPlayer = null;
            if (_beTownPlayers.TryGetValue(iPlayerID, out curPlayer))
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    curPlayer.GraphicActor.OnGuildPostChanged(duty);
                    Logger.LogProcessFormat("[Tittle] OtherPlayer Duty Changed!");
                }
            }
        }

        void _RegisterEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MoveSpeedChanged, _OnPlayerMainSpeedChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3ChangePosition, _OnPk3v3ChangePosition);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SyncAttackCityMonsterDel, OnDeleteAttackCityMonsterByUiEvent);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SyncAttackCityMonsterAdd, OnAddAttackCityMonsterByUiEvent);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SyncAttackCityMonsterList, OnResetAttackCityMonsterByUiEvent);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SyncBlackMarketMerchantNPCType, OnSyncBlackMarketMerchantNPCType);
            NetProcess.AddMsgHandler(SyncOpActivityStateChange.MsgID, _OnSyncActivityStateChange);
        }

        void _UnRegisterEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MoveSpeedChanged, _OnPlayerMainSpeedChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3ChangePosition, _OnPk3v3ChangePosition);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SyncAttackCityMonsterDel, OnDeleteAttackCityMonsterByUiEvent);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SyncAttackCityMonsterAdd, OnAddAttackCityMonsterByUiEvent);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SyncAttackCityMonsterList, OnResetAttackCityMonsterByUiEvent);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SyncBlackMarketMerchantNPCType, OnSyncBlackMarketMerchantNPCType);
            NetProcess.RemoveMsgHandler(SyncOpActivityStateChange.MsgID, _OnSyncActivityStateChange);
        }

        private void _OnSyncActivityStateChange(MsgDATA data)
        {
            var resp = new SyncOpActivityStateChange();
            resp.decode(data.bytes);
            OpActivityData actData = resp.data;
            if (actData == null)
            {
                return;
            }
            if (actData.tmpType == (int)OpActivityTmpType.OAT_ZHOUNIAN_PAIDUI)//周年派对已经结束
            {
                OpActivityData opActivityData = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_ANNIBERPARTY);
                uint curTime = TimeManager.GetInstance().GetServerTime();
                if (opActivityData != null)
                {
                    uint startTime = opActivityData.startTime;
                    uint endTime = opActivityData.endTime;
                    if (curTime < startTime || curTime > endTime)
                    {
                        DisposeNPC(NpcTable.eFunction.AnniersaryParty);
                    }
                    else
                    {
                        CreateNPC(NpcTable.eFunction.AnniersaryParty);
                    }
                }
                else
                {
                    DisposeNPC(NpcTable.eFunction.AnniersaryParty);
                }
            }
          
        }
        void _OnPlayerMainSpeedChanged(UIEvent a_event)
        {
            if (MainPlayer != null)
            {
                Vec3 speed = Global.Settings.townPlayerRun ? Utility.IRepeate2Vector(TableManager.instance.gst.townRunSpeed) : Utility.IRepeate2Vector(TableManager.instance.gst.townWalkSpeed);
                MainPlayer.ActorData.MoveData.MoveSpeed = new Vector3(speed.x, speed.z, speed.y) * PlayerBaseData.GetInstance().MoveSpeedRate;
            }
        }

        void _OnPk3v3ChangePosition(UIEvent a_event)
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

            if (scenedata.SceneSubType != CitySceneTable.eSceneSubType.Pk3v3)
            {
                return;
            }

            ulong playerid = (ulong)a_event.Param1;
            byte group = (byte)a_event.Param2;

            if (playerid == _beTownPlayerMain.ActorData.GUID)
            {
                _beTownPlayerMain.Update3v3RoomPlayerJiaoDiGuangQuan(group);
            }
            else
            {
                var OtherPlayerData = _beTownPlayers.GetEnumerator();

                while (OtherPlayerData.MoveNext())
                {
                    var player = OtherPlayerData.Current.Value as BeTownPlayer;

                    if (player == null)
                    {
                        continue;
                    }

                    if (player.ActorData.GUID != playerid)
                    {
                        continue;
                    }

                    player.Update3v3RoomPlayerJiaoDiGuangQuan(group);

                    break;
                }
            }
        }

#region TownScene Net
        protected Dictionary<uint, Action<MsgDATA>> _mapBindMsgHandle = new Dictionary<uint, Action<MsgDATA>>();

        protected void _BindSceneNetMsgs()
        {
            OnUnBindExtraSceneNetMessages();
            //Logger.LogErrorFormat("吃鸡时序测试----城镇初始化协议绑定");

            if (IsNet == true)
            {
                _UnBindSceneNetMsgs();

                _mapBindMsgHandle.Add(SceneSyncObjectProperty.MsgID, new Action<MsgDATA>(this._OnSyncPlayerOthers));
                _mapBindMsgHandle.Add(SceneSyncSceneObject.MsgID, new Action<MsgDATA>(this._OnSyncAddPlayerOthers));
                _mapBindMsgHandle.Add(SceneDeleteSceneObject.MsgID, new Action<MsgDATA>(this._OnSyncRemovePlayerOthers));
                _mapBindMsgHandle.Add(SceneSyncPlayerMove.MsgID, new Action<MsgDATA>(this._OnSyncMovePlayerOthers));

                Dictionary<uint, Action<MsgDATA>>.KeyCollection keys = _mapBindMsgHandle.Keys;
                IEnumerator enumerator = keys.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    uint msgID = (uint)enumerator.Current;
                    NetProcess.AddMsgHandler(msgID, _mapBindMsgHandle[msgID]);
                }
            }
        }

        protected void _UnBindSceneNetMsgs()
        {
            if (IsNet == true)
            {
                Dictionary<uint, Action<MsgDATA>>.KeyCollection keys = _mapBindMsgHandle.Keys;
                IEnumerator enumerator = keys.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    uint msgID = (uint)enumerator.Current;
                    NetProcess.RemoveMsgHandler(msgID, _mapBindMsgHandle[msgID]);
                }
                _mapBindMsgHandle.Clear();
            }
        }

        // 同步视野范围内其他玩家信息
        protected void _OnSyncPlayerOthers(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnSyncPlayerOthers ==>> msg is nil");
                return;
            }

            int pos = 0;
            UInt64 objId = 0;
            BaseDLL.decode_uint64(msg.bytes, ref pos, ref objId);

            SceneObject sceneObj = null;
            if (!PlayerOthersData.TryGetValue(objId, out sceneObj))
            {
                Logger.LogWarningFormat("_OnSyncPlayerOthers ==>> can't find scene object {0}", objId);
                return;
            }

            if (sceneObj == null)
            {
                return;
            }

            SceneObjectDecoder.DecodePropertys(ref sceneObj, msg.bytes, ref pos, msg.bytes.Length);

            BeTownPlayer townPlayer = null;
            if (_beTownPlayers.ContainsKey(objId))
            {
                townPlayer = _beTownPlayers[objId];

                for (int i = 0; i < sceneObj.dirtyFields.Count; i++)
                {
                    SceneObjectAttr objAttr = (SceneObjectAttr)sceneObj.dirtyFields[i];

                    if (objAttr == SceneObjectAttr.SOA_LEVEL)
                    {
                        _beTownPlayers[objId].SetPlayerRoleLv(sceneObj.level);
                        continue;
                    }
                    else if (objAttr == SceneObjectAttr.SOA_SEASON_LEVEL)
                    {
                        _beTownPlayers[objId].SetPlayerPKRank((int)sceneObj.seasonLevel, (int)sceneObj.seasonStar);
                        continue;
                    }
                    else if (objAttr == SceneObjectAttr.SOA_SEASON_STAR)
                    {
                        _beTownPlayers[objId].SetPlayerPKRank((int)sceneObj.seasonLevel, (int)sceneObj.seasonStar);
                        continue;
                    }
                    else if (objAttr == SceneObjectAttr.SOA_TITLE)
                    {
                        _beTownPlayers[objId].SetPlayerTittle(sceneObj.title);
                        continue;
                    }
                    else if (objAttr == SceneObjectAttr.SOA_GUILD_NAME)
                    {
                        _beTownPlayers[objId].SetPlayerGuildName(sceneObj.guildName);
                        continue;
                    }
                    else if (objAttr == SceneObjectAttr.SOA_GUILD_POST)
                    {
                        _beTownPlayers[objId].SetPlayerGuildDuty(sceneObj.guildPost);
                        continue;
                    }
                    else if (objAttr == SceneObjectAttr.SOA_PET_FOLLOW)
                    {
                        _beTownPlayers[objId].CreatePet((int)sceneObj.followPetDataId);
                        //ClientSystemTown.AddToAsyncLoadPetList(_beTownPlayers[objId], (int)sceneObj.followPetDataId);
                    }
                    else if (objAttr == SceneObjectAttr.SOA_NAME)
                    {
                        _beTownPlayers[objId].SetPlayerName(sceneObj.name);
                    }
                    else if (objAttr == SceneObjectAttr.SOA_ZONEID)
                    {
                        _beTownPlayers[objId].SetPlayerZoneID((int)sceneObj.zoneId);
                    }
                    else if (objAttr == SceneObjectAttr.SOA_ADVENTURE_TEAM_NAME)
                    {
                        if (townPlayer != null)
                        {
                            townPlayer.SetAdventureTeamName(sceneObj.adventureTeamName);
                        }
                    }
                    else if(objAttr == SceneObjectAttr.SOA_NEW_TITLE_NAME)
                    {
                        if(townPlayer != null)
                        {
                          townPlayer.SetTitleName(sceneObj.wearedTitleInfo);
                        }
                    }
                    else if(objAttr == SceneObjectAttr.SOA_GUILD_EMBLEM)
                    {
                        if(townPlayer != null)
                        {
                            townPlayer.SetGuildEmblemLv((int)sceneObj.guildEmblemLvl);
                        }
                    }
                }
            }
            else
            {
                townPlayer = _CreateTownPlayer(PlayerOthersData[objId], CurrentSceneID);
                if (townPlayer != null)
                {
                    _beTownPlayers.Add(objId, townPlayer);
                    townPlayer.Init3v3RoomPlayerJiaoDiGuangQuan();
                }

                _AddDisplayActor(objId);
            }
        }

        protected void _CreateTownPlayer(SceneObject sceneObj)
        {
            if (sceneObj == null)
            {
                return;
            }

            BeTownPlayer beTownPlayer = _CreateTownPlayer(sceneObj, CurrentSceneID);

            if (beTownPlayer != null)
            {
                ulong objID = sceneObj.id;

                //if (!_beTownPlayers.ContainsKey(objID))
                //{
                //    _beTownPlayers.Add(objID, beTownPlayer);
                //}

                // 更新信息记录
                if (PlayerOthersData.ContainsKey(objID))
                {
                    PlayerOthersData[objID] = sceneObj;
                    if (_beTownPlayers.ContainsKey(objID))
                    {
                        _beTownPlayers[objID].Dispose();
                        _beTownPlayers[objID] = beTownPlayer;
                    }

                    if (_beDisplayPlayerList.Contains(beTownPlayer.ActorData.GUID))
                    {
                        beTownPlayer.InitGeActor(_geScene);
                        beTownPlayer.Init3v3RoomPlayerJiaoDiGuangQuan();
                    }
                    else
                    {
                        beTownPlayer.RemoveGeActor();
                    }
                }
                else
                {
                    PlayerOthersData.Add(objID, sceneObj);

                    if (!_beTownPlayers.ContainsKey(objID))
                    {
                        _beTownPlayers.Add(objID, beTownPlayer);
                        beTownPlayer.Init3v3RoomPlayerJiaoDiGuangQuan();
                    }

                    _AddDisplayActor(objID);
                }

                Logger.LogProcessFormat("other player ID:{0}", objID);
            }
        }

        List<SceneObject> mObjQueue = new List<SceneObject>();

        protected void _OnAddDelayCreateCache(SceneObject obj)
        {
            mObjQueue.Add(obj);
        }
        protected void _OnUpdateDelayCreateCache()
        {
            if (mObjQueue.Count > 0)
            {
                var current = mObjQueue[0];
                mObjQueue.RemoveAt(0);
                _CreateTownPlayer(current);
            }
        }

        protected void _OnRemoveDelayCreateCache(ulong objID)
        {
            mObjQueue.RemoveAll((item) => { return item.id == objID; });
        }

        protected void _OnClearDelayCreateCache()
        {
            mObjQueue.Clear();
        }

        // 同步其他玩家出现在视野范围内
        protected void _OnSyncAddPlayerOthers(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnSyncAddPlayerOthers ==>> msg is nil");
                return;
            }

            Logger.LogProcessFormat("add other players.....");

            int pos = 0;
            Dictionary<ulong, SceneObject> sceneObjs = SceneObjectDecoder.DecodeSyncSceneObjectMsg(msg.bytes, ref pos, msg.bytes.Length);
            if (sceneObjs == null)
            {
                return;
            }

            Dictionary<ulong, SceneObject>.Enumerator enumerator = sceneObjs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ulong objID = enumerator.Current.Key;
                if (PlayerBaseData.GetInstance().RoleID == objID)
                {
                    Logger.LogErrorFormat("出现这个提示，请通知小月月！！");
                    continue;
                }

                _OnAddDelayCreateCache(enumerator.Current.Value);
                //mObjQueue.Add(enumerator.Current.Value);
                /*
                SceneObject sceneObj = enumerator.Current.Value;
                BeTownPlayer beTownPlayer = _CreateTownPlayer(sceneObj, CurrentSceneID);
                if (beTownPlayer != null)
                {
                    // 更新信息记录
                    if (PlayerOthersData.ContainsKey(objID))
                    {
                        PlayerOthersData[objID] = sceneObj;
                        _beTownPlayers[objID].Dispose();
                        _beTownPlayers[objID] = beTownPlayer;
                    }
                    else
                    {
                        PlayerOthersData.Add(objID, sceneObj);
                        _beTownPlayers.Add(objID, beTownPlayer);
                    }
                    Logger.LogProcessFormat("other player ID:{0}", objID);
                }
                */
            }
        }

        // 同步其他玩家消失
        protected void _OnSyncRemovePlayerOthers(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnSyncRemovePlayerOthers ==>> msg is nil");
                return;
            }

            Logger.LogProcessFormat("remove other players.....");

            int pos = 0;
            while (msg.bytes.Length - pos > 8)
            {
                ulong objID = 0;
                BaseDLL.decode_uint64(msg.bytes, ref pos, ref objID);

                Logger.LogProcessFormat("other player ID:{0}", objID);

                // 删除数据
                PlayerOthersData.Remove(objID);

                // 删除对象
                BeTownPlayer player;
                _beTownPlayers.TryGetValue(objID, out player);
                if (player != null)
                {
                    _RemoveDisplayActor(objID);
                    player.Dispose();
                    _beTownPlayers.Remove(objID);
                }

                if (_beDisplayPlayerList.Count < _beDisplayNum && _beDisplayPlayerList.Count < _beTownPlayers.Count)
                {
                    Dictionary<ulong, BeTownPlayer>.Enumerator it = _beTownPlayers.GetEnumerator();
                    while (it.MoveNext())
                    {
                        BeTownPlayer curPlayer = it.Current.Value;
                        if (null != curPlayer)
                            _AddDisplayActor(objID);

                        if (_beDisplayPlayerList.Count == _beDisplayNum)
                            break;
                    }
                }

                _OnRemoveDelayCreateCache(objID);
            }
        }

        // 同步其他玩家移动
        protected void _OnSyncMovePlayerOthers(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnSyncMovePlayers ==>> msg is nil");
                return;
            }

            SceneSyncPlayerMove msgMove = new SceneSyncPlayerMove();
            msgMove.decode(msg.bytes);

            if (msgMove.dir.x == 0 && msgMove.dir.y == 0)
            {
                if (msgMove.id == PlayerBaseData.GetInstance().RoleID)
                {
                    Logger.Log("rece self stopped.");
                }
                else
                {
                    Logger.LogFormat("rece object {0} stopped.", msgMove.id);
                }
            }

            if (_beTownPlayers.ContainsKey(msgMove.id))
            {
                BeTownPlayer player = _beTownPlayers[msgMove.id];
                // 服务器不支持负数，转了一下坐标系！！服务器用场景左下角为原点，客户端用的就是编辑器编辑的逻辑原点
                player.AddMoveCommand(
                    new Vector3(msgMove.pos.x / _axisScale, 0.0f, msgMove.pos.y / _axisScale) + player.ActorData.MoveData.PosServerToClient,
                    new Vector3(msgMove.dir.x / _axisScale, 0.0f, msgMove.dir.y / _axisScale),
                    msgMove.dir.faceRight >= 1
                    );
            }
        }

        void _InitOtherPlayerData(MsgDATA a_msg, uint a_sceneID)
        {
            PlayerOthersData.Clear();
            if (a_msg != null)
            {
                int pos = 0;
                Dictionary<ulong, SceneObject> sceneObjs = SceneObjectDecoder.DecodeSyncSceneObjectMsg(a_msg.bytes, ref pos, a_msg.bytes.Length);
                if (sceneObjs != null)
                {
                    Dictionary<ulong, SceneObject>.Enumerator enumerator = sceneObjs.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ulong key = enumerator.Current.Key;
                        if (enumerator.Current.Value.sceneId == a_sceneID)
                        {
                            if (PlayerOthersData.ContainsKey(key))
                            {
                                PlayerOthersData[key] = enumerator.Current.Value;
                            }
                            else
                            {
                                PlayerOthersData.Add(key, enumerator.Current.Value);
                            }
                            Logger.LogProcessFormat("other player ID:{0}", key);
                        }
                    }
                }
            }
        }

        /*public object GetSceneObjetByPlayerID(ulong playerID)
        {
            if(playerID == PlayerBaseData.GetInstance().RoleID)
            {
                return MainPlayer;
            }

            if(PlayerOthersData == null)
            {
                return null;
            }

            SceneObject sceneObj = null;
            if (!PlayerOthersData.TryGetValue(playerID, out sceneObj))
            {
                return null;
            }

            return sceneObj;
        }*/

        public static void _BeginChangeJobDialog()
        {
            //MissionManager.GetInstance().CreateDialogFrame(10003, 0, new TaskDialogFrame.OnDialogOver().AddListener(_OpenJobChangeSelectFrame));
            _OpenJobChangeSelectFrame();
        }

        public static void _BeginAwakeDialog()
        {
            MissionManager.GetInstance().CreateDialogFrame(10007, 0, new TaskDialogFrame.OnDialogOver().AddListener(_OpenAwakeFrame));
        }

        public sealed override void OnStart(SystemContent systemContent)
        {
            SystemSwitchEventManager.GetInstance().RemoveEvent(SystemEventType.SYSETM_EVENT_SELECT_ROLE);
            SystemSwitchEventManager.GetInstance().RemoveEvent(SystemEventType.SYSTEM_EVENT_ON_SWITCH_FAILED);

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                ProtoTable.CitySceneTable TownTableData = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(systemTown.CurrentSceneID);
                if (TownTableData != null && TownTableData.SceneSubType == CitySceneTable.eSceneSubType.BUDO)
                {
                    if (BudoManager.GetInstance().CanAcqured)
                    {
                        BudoResultFrameData data = new BudoResultFrameData
                        {
                            bOver = true,
                            bNeedOpenBudoInfo = true
                        };
                        BudoResultFrame.Open(data);
                    }
                    else if (BudoManager.GetInstance().ReturnFromPk)
                    {
                        BudoResultFrameData data = new BudoResultFrameData
                        {
                            bOver = false,
                            bNeedOpenBudoInfo = false,
                            eResult = BudoManager.GetInstance().pkResult
                        };
                        BudoResultFrame.Open(data);
                        BudoManager.GetInstance().ReturnFromPk = false;
                    }
                }
            }

            if (systemContent != null)
            {
                if (systemContent.onStart != null)
                {
                    systemContent.onStart.Invoke();
                    systemContent.onStart = null;
                }
            }
        }

        public static void _OpenJobChangeSelectFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<ChangeJobSelectFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ChangeJobSelectFrame>();
            }

            ChangeJobType param = ChangeJobType.ChangeJobMission;

            ClientSystemManager.GetInstance().OpenFrame<ChangeJobSelectFrame>(FrameLayer.Middle, param);
        }

        public static void _OpenAwakeFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<AwakeFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<AwakeFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<AwakeFrame>(FrameLayer.Middle);
        }
#endregion

#endregion

#region npcDialog
        NpcDialogComponent m_kCurrentDialogComponent = null;

        void _UpdateNpcDialog(float timeElapsed)
        {

#if !LOGIC_SERVER
            float fMaxDistance = 4.80f;
            if (_beTownPlayerMain != null)
            {
                if (m_kCurrentDialogComponent == null)
                {
                    for (int i = 0; i < _beTownNPCs.Count; ++i)
                    {
                        var curNpc = _beTownNPCs[i];
                        if (null != curNpc && curNpc.GraphicActor != null && null != curNpc.GraphicActor.NpcDialogComponent)
                        {
                            var curDialogComponent = curNpc.GraphicActor.NpcDialogComponent;
                            if (curDialogComponent != null)
                            {
                                var disPos = curNpc.ActorData.MoveData.Position - _beTownPlayerMain.ActorData.MoveData.Position;
                                disPos.y = 0.0f;
                                float dis = Mathf.Sqrt(disPos.sqrMagnitude);
                                if (fMaxDistance >= dis)
                                {
                                    if (m_kCurrentDialogComponent == null ||
                                        m_kCurrentDialogComponent.TickPower > curDialogComponent.TickPower
                                        || m_kCurrentDialogComponent.TickPower == curDialogComponent.TickPower &&
                                        m_kCurrentDialogComponent.NextTick > curDialogComponent.NextTick)
                                    {
                                        m_kCurrentDialogComponent = curDialogComponent;
                                    }
                                }
                            }
                        }
                    }

                    if (m_kCurrentDialogComponent != null)
                    {
                        m_kCurrentDialogComponent.BeginTick();
                    }
                }

                if (m_kCurrentDialogComponent != null)
                {
                    if (m_kCurrentDialogComponent.InTick)
                    {
                        m_kCurrentDialogComponent.Tick(timeElapsed);
                    }
                    else
                    {
                        m_kCurrentDialogComponent.EndTick();
                        m_kCurrentDialogComponent = null;
                    }
                }
            }
#endif
        }
#endregion

        class PetLoadDesc
        {
            public BeTownPlayer m_petOwner;
            public int m_petID;
        }
        static List<PetLoadDesc> m_PetLoadList = new List<PetLoadDesc>();
        static public void AddToAsyncLoadPetList(BeTownPlayer player, int petID)
        {
            if (null != player)
            {
                PetLoadDesc newDesc = new PetLoadDesc
                {
                    m_petOwner = player,
                    m_petID = petID
                };
                m_PetLoadList.Add(newDesc);
            }
        }

        static void _UpdateAsyncLoadPetList()
        {
            if (m_PetLoadList.Count > 0)
            {
                PetLoadDesc cur = m_PetLoadList[0];
                m_PetLoadList.RemoveAt(0);
                cur.m_petOwner.CreatePet(cur.m_petID);
                cur.m_petOwner.CreateFollow();
            }
        }

        static void _ClearLoadPetList()
        {
            m_PetLoadList.Clear();
        }

        //////////////////////////////////////////////////////////////////////////
        void _AddDisplayActor(ulong ID)
        {
            if (_beDisplayPlayerList.Count < _beDisplayNum)
            {
                if (!_beDisplayPlayerList.Contains(ID))
                {
                    BeTownPlayer beTownPlayer = null;

                    if (_beTownPlayers.TryGetValue(ID, out beTownPlayer))
                    {
                        if (null == beTownPlayer.GraphicActor)
                        {
                            beTownPlayer.InitGeActor(_geScene);
                            beTownPlayer.Init3v3RoomPlayerJiaoDiGuangQuan();
                            if(beTownPlayer.GraphicActor == null)
                            {
                                var actorData = beTownPlayer.ActorData as BeTownPlayerData;
                                if(actorData != null)
                                {
                                    Logger.LogErrorFormat("InitPlayer failed {0} {1} {2} {3}",actorData.GUID,actorData.Name,actorData.ZoneID,actorData.JobID);
                                }
                                else
                                {
                                    Logger.LogErrorFormat("InitBasePlayer failed!");
                                }
                                return;
                            }

                            if (beTownPlayer.ActorData.ActionData.ActionName.Contains("Idle"))
                            {
                                beTownPlayer.GraphicActor.ChangeAction(beTownPlayer.ActorData.AniNames[(int)ActorActionType.AT_IDLE], 1, true, true, true);
                            }
                            else
                            {
                                string actionName = Global.Settings.townPlayerRun ?
                                    beTownPlayer.ActorData.AniNames[(int)ActorActionType.AT_RUN] :
                                    beTownPlayer.ActorData.AniNames[(int)ActorActionType.AT_WALK];

                                beTownPlayer.GraphicActor.ChangeAction(actionName, 1, true, true, true);
                            }
                        }

                        _beDisplayPlayerList.Add(ID);
                    }
                }
            }
        }

        void _RemoveDisplayActor(ulong ID)
        {
            if (_beDisplayPlayerList.Contains(ID))
            {
                BeTownPlayer beTownPlayer = null;
                if (_beTownPlayers.TryGetValue(ID, out beTownPlayer))
                {
                    if (null != beTownPlayer.GraphicActor)
                        beTownPlayer.RemoveGeActor();
                    _beDisplayPlayerList.Remove(ID);
                }
            }
        }

#region ExtraSceneNet
        //在切换场景的协议返回时 to 场景初始化成功的时候，这一段时间没有监听网络消息，可能会导致其他玩家的数据丢失
        private Dictionary<ulong, SceneObject> _extraOtherPlayerData = new Dictionary<ulong, SceneObject>();

        //添加额外收到的玩家数据
        private void AddExtraOtherPlayerData()
        {
            if (_extraOtherPlayerData == null || _extraOtherPlayerData.Count <= 0)
                return;

            var enumerator = _extraOtherPlayerData.GetEnumerator();

            while (enumerator.MoveNext())
            {
                SceneObject sceneObj = enumerator.Current.Value;
                var otherPlayerGuid = enumerator.Current.Key;

                //guid为0
                if (otherPlayerGuid == 0)
                    continue;
                //数据为null
                if (sceneObj == null)
                    continue;
                //角色场景Id与当前的场景Id不一致
                if (sceneObj.sceneId != CurrentSceneID)
                    continue;

                _OnAddDelayCreateCache(sceneObj);
            }
            _extraOtherPlayerData.Clear();
        }

        //同步其他玩家
        private void OnExtraSceneSyncSceneObject(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            int pos = 0;
            Dictionary<ulong, SceneObject> sceneObjs = SceneObjectDecoder.DecodeSyncSceneObjectMsg(msgData.bytes, ref pos, msgData.bytes.Length);
            if (sceneObjs == null)
                return;

            Dictionary<ulong, SceneObject>.Enumerator enumerator = sceneObjs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var otherPlayerGuid = enumerator.Current.Key;
                if (PlayerBaseData.GetInstance().RoleID == otherPlayerGuid)
                    continue;
                if (enumerator.Current.Value == null)
                    continue;

                _extraOtherPlayerData[otherPlayerGuid] = enumerator.Current.Value;
            }
        }

        //删除其他玩家
        private void OnExtraSceneDeleteSceneObject(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            int pos = 0;
            while (msgData.bytes.Length - pos > 8)
            {
                ulong otherPlayerGuid = 0;
                BaseDLL.decode_uint64(msgData.bytes, ref pos, ref otherPlayerGuid);

                _extraOtherPlayerData.Remove(otherPlayerGuid);
            }
        }

        //同步其他玩家的属性
        private void OnExtraSceneSyncObjectProperty(MsgDATA msgData)
        {
            if (msgData == null)
                return;

            int pos = 0;
            UInt64 otherPlayerGuid = 0;
            BaseDLL.decode_uint64(msgData.bytes, ref pos, ref otherPlayerGuid);

            SceneObject sceneObj = null;

            if (_extraOtherPlayerData.TryGetValue(otherPlayerGuid, out sceneObj) == false)
                return;

            if (sceneObj == null)
                return;

            SceneObjectDecoder.DecodePropertys(ref sceneObj, msgData.bytes, ref pos, msgData.bytes.Length);
            _extraOtherPlayerData[otherPlayerGuid] = sceneObj;
        }

        //绑定消息，只是用来接受数据(处于切场景协议和正常的场景消息绑定之前的消息)
        private void OnBindExtraSceneNetMessages()
        {
            OnUnBindExtraSceneNetMessages();
            NetProcess.AddMsgHandler(SceneSyncSceneObject.MsgID, OnExtraSceneSyncSceneObject);
            NetProcess.AddMsgHandler(SceneDeleteSceneObject.MsgID, OnExtraSceneDeleteSceneObject);
            NetProcess.AddMsgHandler(SceneSyncObjectProperty.MsgID, OnExtraSceneSyncObjectProperty);
        }

        private void OnUnBindExtraSceneNetMessages()
        {
            if (_extraOtherPlayerData != null)
                _extraOtherPlayerData.Clear();
            NetProcess.RemoveMsgHandler(SceneSyncSceneObject.MsgID, OnExtraSceneSyncSceneObject);
            NetProcess.RemoveMsgHandler(SceneDeleteSceneObject.MsgID, OnExtraSceneDeleteSceneObject);
            NetProcess.RemoveMsgHandler(SceneSyncObjectProperty.MsgID, OnExtraSceneSyncObjectProperty);
        }
#endregion

#region AttackCityMonster
        //攻城怪物相关

        //创建场景初始化时的攻城怪物
        private void CreateAttackCityMonsters()
        {

            List<SceneNpc> sceneNpcList =
                AttackCityMonsterDataManager.GetInstance().GetSceneNpcsListBySceneId(CurrentSceneID);

            if (sceneNpcList == null || sceneNpcList.Count <= 0)
                return;

            foreach (var sceneNpc in sceneNpcList)
            {
                CreateAttackCityMonsterBySceneNpc(sceneNpc);
            }

        }

        private void OnResetAttackCityMonsterByUiEvent(UIEvent uiEvent)
        {
            ResetTownAttackCityMonsterList();

            CreateAttackCityMonsters();
        }

        //添加攻城怪物
        private void OnAddAttackCityMonsterByUiEvent(UIEvent uiEvent)
        {
            List<SceneNpc> sceneNpcList =
                AttackCityMonsterDataManager.GetInstance().GetSceneNpcsListBySceneId(CurrentSceneID);

            if (sceneNpcList == null || sceneNpcList.Count <= 0)
                return;

            for(var i = 0; i < sceneNpcList.Count; i++)
            {
                //null
                var sceneNpc = sceneNpcList[i];
                if(sceneNpc == null)
                    continue;

                bool flag = IsAttackCityMonsterAlreadyExist(sceneNpc.guid);
                //已经创建
                if (flag == true)
                    continue;

                CreateAttackCityMonsterBySceneNpc(sceneNpc);
            }
        }

        //删除怪物
        private void OnDeleteAttackCityMonsterByUiEvent(UIEvent uiEvent)
        {
            for (var i = _beTownAttackCityMonsters.Count - 1; i >= 0; i--)
            {
                var townAttackCityMonster = _beTownAttackCityMonsters[i];
                if(townAttackCityMonster == null)
                    continue;

                var attackCityMonsterData = townAttackCityMonster.ActorData as BeTownNPCData;
                if(attackCityMonsterData == null)
                    continue;

                bool flag = AttackCityMonsterDataManager.GetInstance().IsSceneDataContainNpcData(
                    attackCityMonsterData.GUID,
                    (uint)CurrentSceneID);
                if(flag)
                    continue;

                //怪物数值被删除，则删除场景中对应的怪物                
                townAttackCityMonster.Dispose();
                _beTownAttackCityMonsters.RemoveAt(i);
            }
        }

        //重置攻城怪物列表
        private void ResetTownAttackCityMonsterList()
        {
            for(var i = 0; i < _beTownAttackCityMonsters.Count; i++)
            {
                var townAttackCityMonster = _beTownAttackCityMonsters[i];
                if(townAttackCityMonster == null)
                    continue;

                townAttackCityMonster.Dispose();
            }
            _beTownAttackCityMonsters.Clear();
        }

        //根据guid判断对应的怪物是否已经创建
        private bool IsAttackCityMonsterAlreadyExist(UInt64 guid)
        {
            for(var i = 0; i < _beTownAttackCityMonsters.Count; i++)
            {
                var beTownAttackCityMonster = _beTownAttackCityMonsters[i];
                if (beTownAttackCityMonster != null)
                {
                    var attackCityMonsterData = beTownAttackCityMonster.ActorData as BeTownNPCData;
                    if (attackCityMonsterData != null)
                    {
                        if (attackCityMonsterData.GUID == guid)
                            return true;
                    }
                }
            }

            return false;
        }

        //根据SceneNpc的数据来创建一个攻城怪物
        private void CreateAttackCityMonsterBySceneNpc(SceneNpc sceneNpc)
        {
            if (sceneNpc == null)
                return;

            var npcItem = TableManager.GetInstance().GetTableItem<NpcTable>((int)sceneNpc.id);
            if (npcItem == null)
            {
                Logger.LogErrorFormat("NpcItem is null, guid is {0} npcId is {1}", sceneNpc.guid, sceneNpc.id);
                return;
            }

            var data = new BeTownNPCData
            {
                GUID = sceneNpc.guid,
                NpcID = (int)sceneNpc.id,
                ResourceID = npcItem.ResID,
                Name = npcItem.NpcName,
                NameColor = PlayerInfoColor.TOWN_NPC,
                TownNpcType = ESceneActorType.AttackCityMonster,
            };

            data.MoveData.Position = AttackCityMonsterDataManager.GetInstance()
                .GetAttackCityMonsterScenePosition(sceneNpc.pos);

            var beTownAttackCityMonster = new BeTownAttackCityMonster(data, this);
            beTownAttackCityMonster.InitGeActor(_geScene);
            _beTownAttackCityMonsters.Add(beTownAttackCityMonster);
        }

        public BeTownNPC GetTownAttackCityMonsterByGuid(UInt64 guid)
        {
            for(var i = 0; i < _beTownAttackCityMonsters.Count; i++)
            {
                var attackCityMonster = _beTownAttackCityMonsters[i];
                if(attackCityMonster == null)
                    continue;

                var npcData = attackCityMonster.ActorData as BeTownNPCData;
                if(npcData == null)
                    continue;

                if (npcData.GUID == guid)
                    return attackCityMonster;
            }

            return null;
        }

#endregion

#region 黑市商人NPC

        private void OnSyncBlackMarketMerchantNPCType(UIEvent uiEvent)
        {
            CreatBlackMarketMerchant();
        }

        /// <summary>
        /// 重置黑市商人NPC
        /// </summary>
        private void OnResetBlackMarketMerchantNPC()
        {
            if (_beTownBlackMarketMerchantNpc != null)
            {
                _beTownBlackMarketMerchantNpc.Dispose();
            }
        }

        /// <summary>
        /// 创建黑市商人NPC
        /// </summary>
        private void CreatBlackMarketMerchant()
        {
            bool whetherCanCreatNPC = BlackMarketMerchantDataManager.GetInstance().CreatBlackMarketMerchantNPC(CurrentSceneID);
            if (whetherCanCreatNPC)
            {
                OnResetBlackMarketMerchantNPC();

                BlackMarketType blackMarketType = BlackMarketMerchantDataManager.BlackMarketType;
                switch (blackMarketType)
                {
                    case BlackMarketType.BmtInvalid:
                        break;
                    case BlackMarketType.BmtFixedPrice:
                    case BlackMarketType.BmtAuctionPrice:
                        CreatBlackMarketMerchantNPC(blackMarketType);
                        break;
                    default:
                        break;
                }
            }
        }

        private void CreatBlackMarketMerchantNPC(BlackMarketType type)
        {
            var mBlackMarketMerchantTable = TableManager.GetInstance().GetTableItem<BlackMarketTable>((int)type);
            if (mBlackMarketMerchantTable == null)
            {
                Logger.LogErrorFormat("BlackMarketTable is null, Id is {0}", (int)type);
                return;
            }

            var mNpcTable = TableManager.GetInstance().GetTableItem<NpcTable>(mBlackMarketMerchantTable.NpcID);
            if (mNpcTable == null)
            {
                Logger.LogErrorFormat("NpcItem is null, npcId is {0}", mBlackMarketMerchantTable.NpcID);
                return;
            }

            var data = new BeTownNPCData
            {
                NpcID = (int)mNpcTable.ID,
                ResourceID = mNpcTable.ResID,
                Name = mNpcTable.NpcName,
                NameColor = PlayerInfoColor.TOWN_NPC,
                TownNpcType = ESceneActorType.BlackMarketMerchanNpc,
            };

            data.MoveData.Position = BlackMarketMerchantDataManager.GetInstance().GetBlackMarketMerchantPostion(mNpcTable);

            _beTownBlackMarketMerchantNpc = new BeTownBlackMarketMerchanNPC(data, this);
            _beTownBlackMarketMerchantNpc.InitGeActor(_geScene);
        }

        /// <summary>
        /// 得到黑市商人NPC
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public BeTownNPC GetBlackMarketMerchanNpcById(int npcId)
        {
            if (_beTownBlackMarketMerchantNpc != null)
            {
                var npcData = (_beTownBlackMarketMerchantNpc.ActorData as BeTownNPCData);
                if (npcData == null)
                {
                    return null;
                }
               if (npcData.NpcID == npcId)
               {
                    return _beTownBlackMarketMerchantNpc;
               }
            }

            return null;
        }
#endregion

        public void ResetAction()
        {
            var actors = BeBaseActor.Actors;

            foreach(var actor in actors)
            {
                if (actor != null && actor.GeActor != null && actor.ActorData != null && actor.ActorData.ActionData != null)
                {
                    actor.ActorData.ActionData.isDirty = true;
                    actor.ActorData.ActionData.isForce = true;
                    for (int i = 0; i < actor.ActorData.arrAttachmentData.Count; ++i)
                        {
                            AttachmentPlayData attachData = actor.ActorData.arrAttachmentData[i];
                            if (attachData != null)
                            {
                                attachData.isDirty = true;
                                attachData.isForce = true;
                            }
                                
                        }
                }
            }
        }

        private string[] m_PrewarmList = new string[]
            {
                // 背包
                "UIFlatten/Prefabs/Package/PackageNew",
                "UIFlatten/Prefabs/Package/BG",
                "UIFlatten/Prefabs/Package/Bottom",
                "UIFlatten/Prefabs/Package/Tabs",

                // 技能
                "UIFlatten/Prefabs/Skill/SkillTreeFrame",
                "UIFlatten/Prefabs/Skill/JobSkillTree",
                // 组队
                "UIFlatten/Prefabs/Team/TeamList",
                // 魔罐
                "UIFlatten/Prefabs/Jar/PocketJar",
                // 袖珍罐
                "UIFlatten/Prefabs/Jar/ActivityJar",

                // 商城
                "UIFlatten/Prefabs/MallNew/MallNewFrame",
                "UIFlatten/Prefabs/MallNew/PropertyMallView",

                // 日常
                "UIFlatten/Prefabs/Activity/Dungeon/ActivityDungeon",
                "UIFlatten/Prefabs/Mission/MissionDailyFrame",

                // 时装合成
                "UIFlatten/Prefabs/SmithShop/FashionSmithShop/FashionMergeNewFrame", 

                // 锻冶
                "UIFlatten/Prefabs/SmithShop/Smithshop", 

                // 地下城选择，信息，及章节宝箱
                "UIFlatten/Prefabs/Chapter/Select/ChapterSelect",
                "UIFlatten/Prefabs/Chapter/Normal/ChapterNormalHalf",
                "UIFlatten/Prefabs/Chapter/SelectReward/ChapterSelectRewardFrame",
            };

        private IEnumerator _PrewarmFrames(IASyncOperation systemOperation)
        {
            for (int i = 0, icnt = m_PrewarmList.Length; i < icnt; ++i)
            {
                //CGameObjectPool.instance.PrepareGameObject(prewarmList[i], enResourceType.UIPrefab, 1);

                if (AssetLoader.instance.PreLoadRes(m_PrewarmList[i], typeof(GameObject), false))
                {
                    AssetLoader.instance.LockAsset(m_PrewarmList[i], (int)AssetLoader.AssetLockFlag.LockGroup_Town);
                }

                systemOperation.SetProgress(0.7f + i * 0.3f / m_PrewarmList.Length);

                yield return null;
            }
        }

        private void _unLockFrames()
        {
            for (int i = 0, icnt = m_PrewarmList.Length; i < icnt; ++i)
            {
                AssetLoader.instance.LockAsset(m_PrewarmList[i], 0);
            }
        }

        public BeTownPlayer GetTownPlayer(ulong playerID)
        {
            if(playerID == PlayerBaseData.GetInstance().RoleID)
            {
                return MainPlayer;
            }
            BeTownPlayer townPlayer = null;
            _beTownPlayers.TryGetValue(playerID, out townPlayer);
            return townPlayer;
        }
    }
}
