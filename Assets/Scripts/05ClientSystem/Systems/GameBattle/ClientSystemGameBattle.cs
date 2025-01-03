using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
namespace GameClient
{
    //吃鸡场景
    public class ClientSystemGameBattle : ClientSystem
    {
        class DoorNode
        {
            public int TargetSceneIndex;
            public ISceneTownDoorData Door;
        }

        class SceneNode
        {
            public int SceneID;
            public List<DoorNode> DoorNodes;
            public bool HasVisited;
        }

        protected ISceneData _levelData;

        public ISceneData LevelData
        {
            get { return _levelData; }
        }
        List<SceneNode> m_sceneNodes = new List<SceneNode>();
        protected GeSceneEx _geScene;
        protected Vector3 _logicToGraph;
        protected Vector3 _serverToClient;
        protected BeFighterManager<BeFighter> mFighters = new BeFighterManager<BeFighter>();
        protected BeFighterMain mMainPlayer;
        public Dictionary<ulong, SceneObject> PlayerOthersData = new Dictionary<ulong, SceneObject>();
        protected Dictionary<ulong, BeFightBuffManager> _beTownPlayerBuffs = new Dictionary<ulong, BeFightBuffManager>();
        protected List<ulong> _beDisplayPlayerList = new List<ulong>();
        protected int _beDisplayNum = 0;
        public bool IsNet = true;
        protected float _axisScale = 10000.0f;
        private bool _isSwithScene = false;
        private bool mIsTownSceneSwitching = false;
        float fChijiTimeInterval = 0.15f;
        float fChijiItemRefreshTime = 0.0f;
        protected uint m_BgmHandle = uint.MaxValue;
        protected uint m_EnvGgmHandle = uint.MaxValue;
        private int _currentSceneId;
        protected BeFighterManager<BeItem> _beTownItems = new BeFighterManager<BeItem>();
        protected BeFighterManager<BeNPC> _beNPCs = new BeFighterManager<BeNPC>();
        protected BeFighterManager<BeBattleProjectile> _beProjectiles = new BeFighterManager<BeBattleProjectile>();
    //    protected BeFighterManager<BeRobotFighter> _beRobots = new BeFighterManager<BeRobotFighter>();
        private BePoison mPoisonRing = null;
        private int m_targetSceneID = -1;
        List<SceneObject> mObjQueue = new List<SceneObject>();
        List<BeItem> mItemQueue = new List<BeItem>();

        public int ChijiNearItemNum = 0;
        public float AxisScale { get { return _axisScale; } }
        public Vector3 ServerToClient { get { return _serverToClient; } }
        public BePoison PoisonRing { get { return mPoisonRing; } }
        public BeFighterManager<BeItem> Items { get { return _beTownItems; } }
        private List<BeItem> mBuffItem = new List<BeItem>();
        private List<int> mRemoveBuffItem = new List<int>();
        public BeFighterManager<BeFighter> OtherFighters { get { return mFighters; } }
        public BeFighterManager<BeBattleProjectile> Projectiles { get { return _beProjectiles; } }
        public Dictionary<ulong, BeFightBuffManager> OtherFighterBuffs { get { return _beTownPlayerBuffs; } }
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

        private bool isTownSceneSwitching
        {
            get
            {
                return mIsTownSceneSwitching;
            }

            set
            {
                Logger.LogProcessFormat("[FightIsSceneSwitching] {0} -> {1}", mIsTownSceneSwitching, value);
                mIsTownSceneSwitching = value;
            }
        }
        public BeFighterMain MainPlayer
        {
            get
            {
                return mMainPlayer;
            }
        }
        public void SetTargetSceneID(int iTargetSceneID)
        {
            m_targetSceneID = iTargetSceneID;
        }

        public void SetPoisonRing(int x, int y, int radius, uint timeStamp, int durTime, Vector2 lastCenter, float lastRadius)
        {
          //  Logger.LogErrorFormat("Set Posison Ring x {0} y {1} radius {2} timeStamp {3} durTime {4}", x, y, radius, timeStamp, durTime);
            if (mPoisonRing == null)
            {
                var newData = new BePoison.BePoisonData
                {
                    radius = radius / _axisScale,
                    x = x,
                    y = y,
                    startTime = timeStamp,
                    durTime = durTime,
                    realPos = new Vector3(x / _axisScale, 0.0f, y / _axisScale)
                };
                mPoisonRing = new BePoison(newData, this);
                mPoisonRing.ActorData.MoveData.PosLogicToGraph = _logicToGraph;
                mPoisonRing.ActorData.MoveData.PosServerToClient = _serverToClient;
                var data = mPoisonRing.ActorData as BePoison.BePoisonData;
                data.MoveData.ServerPosition = newData.realPos;
                //    Logger.LogErrorFormat("lastradius {0} startcenter {1}", lastRadius, lastCenter);
                mPoisonRing.ResetStartInfo(lastRadius, lastCenter);
                mPoisonRing.InitGeActor(_geScene);
            }
            else
            {
                mPoisonRing.ResetCircle();
                var data = mPoisonRing.ActorData as BePoison.BePoisonData;
                data.x = x;
                data.y = y;
                data.radius = radius / _axisScale;
                data.startTime = timeStamp;
                data.durTime = durTime;
                data.realPos = new Vector3(data.x / _axisScale, 0.0f, data.y / _axisScale);
                mPoisonRing.StartShrink();
            }
        }

        public sealed override string GetMainUIPrefabName()
        {
            return string.Empty;///"UIFlatten/Prefabs/Chiji/ChijiWaitingRoomFrame";
        }
        public sealed override void GetEnterCoroutine(AddCoroutine enter)
        {
            enter(_baseSystemLoadingCoroutine);
            enter(FightLoadingCoroutine);
        }
        public PathFinding.GridInfo GridInfo { get; private set; }
        protected sealed override string _GetLevelName()
        {
            return "Town";
        }
        protected IEnumerator FightLoadingCoroutine(IASyncOperation systemOperation)
        {
            Logger.LogProcessFormat("return to town Loading...this is not error, just a log of checking process");
            int beginTimeStamp =  Environment.TickCount;
            Time.timeScale = 1.0f;

            ClientSystemManager.instance.delayCaller.Clear();

            if (SystemManager.CurrentSystem == null)
            {
                yield return _SystemInitWithoutNet(systemOperation);
            }
            else
            {
                ClientSystemManager.GetInstance().OnSwitchSystemFinished.RemoveAllListeners();
                ClientSystemManager.GetInstance().OnSwitchSystemFinished.AddListener(_NextFuncOpen);
                ClientSystemManager.GetInstance().OnSwitchSystemFinished.AddListener(OnSceneLoadEnd);
                _BindUIEvent();
                ItemDataManager.GetInstance().AddSystemInvoke();
                Type systemType = SystemManager.CurrentSystem.GetType();
                if (systemType == typeof(ClientSystemTown) || ChijiDataManager.GetInstance().IsToPrepareScene)
                {
                    m_targetSceneID = 10101;
                }
                else if (systemType == typeof(ClientSystemBattle))
                {
                    m_targetSceneID = 10100;
                }

                ChijiDataManager.GetInstance().IsToPrepareScene = false;

                yield return _SystemInitEnterToChijiSystem(systemOperation);
            }

            //城镇预加载
            yield return ClientSystemManager._PreloadRes(systemOperation);

            /// 使用预加载界面
            if (EngineConfig.usePrewarmFrame)
            {
                yield return _PrewarmFrames();
            }
            yield return Yielders.EndOfFrame;
            ReplayServer.GetInstance().Clear();
            int endTimeStamp = Environment.TickCount;
            Logger.LogErrorFormat(string.Format("吃鸡流程日志----FightLoadingCoroutine consumeTime {0}",endTimeStamp - beginTimeStamp));
        }
        public sealed override void OnEnter()
        {
            if (Global.Settings.displayHUD)
                HUDInfoViewer.instance.Init();
            base.OnEnter();
            NetManager.instance.ClearReSendData();
            AssetGabageCollectorHelper.instance.SetGCPurgeEnable(AssetGCTickType.Asset, true);
            AssetGabageCollectorHelper.instance.SetGCPurgeEnable(AssetGCTickType.SceneActor, true);
            AssetGabageCollectorHelper.instance.SetGCPurgeEnable(AssetGCTickType.UIFrame, true);

            CameraAspectAdjust.MarkDirty();
            _resetIsSwiching();
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

        private void TriggerBattleExitEvent()
        {
            if (BattleMain.instance == null)
                return;
            if (BattleMain.instance.GetDungeonManager() == null)
                return;
            var scene = BattleMain.instance.GetDungeonManager().GetBeScene();
            if (scene == null)
                return;
            scene.TriggerEventNew(BeEventSceneType.onBattleExit);
        }

        public sealed override void OnExit()
        {
            // marked by ckm
            TriggerBattleExitEvent();
            
            _resetIsSwiching();


            StopBGM();

            AssetGabageCollectorHelper.instance.SetGCPurgeEnable(AssetGCTickType.Asset, false);
            AssetGabageCollectorHelper.instance.SetGCPurgeEnable(AssetGCTickType.SceneActor, false);
            AssetGabageCollectorHelper.instance.SetGCPurgeEnable(AssetGCTickType.UIFrame, false);
            ClearScene();
            BeFighterMain.CommandStopAutoMove();
            _UnBindUIEvent();
            _ClearData();
            ClearBaseMainFrame();
            if (SystemManager.TargetSystem != null)
            {
                Type systemType = SystemManager.TargetSystem.GetType();
                if (systemType != null)
                {
                    if (systemType != typeof(ClientSystemBattle))
                    {
                        ManualPoolCollector.instance.Clear();
                    }
                }
            }
            base.OnExit();
        }
        protected sealed override void _OnUpdate(float timeElapsed)
        {
            UpdateScene(timeElapsed);
            _UpdateData(timeElapsed);
            _UpdateNpcDialog(timeElapsed);
            _OnUpdateDelayCreateCache();
            _OnUpdateDelayCreateItemCache();
            _OnUpdateChiji(timeElapsed);
        }
        public BeNPC GetNpcByGuid(Int32 iNpcId, UInt64 _npcGuid)
        {
            for (var i = 0; i < _beNPCs.GetFightCount(); ++i)
            {
                var curNpc = _beNPCs.GetFighter(i);
                if(curNpc == null)
                {
                    continue;
                }

                var npcData = curNpc.ActorData as BeNPCData;
                if (npcData == null)
                {
                    continue;
                }

                if (npcData.NpcID == iNpcId && npcData.GUID == _npcGuid)
                {
                    return curNpc;
                }
            }

            return null;
        }

        NpcDialogComponent m_kCurrentDialogComponent = null;
        void _UpdateNpcDialog(float timeElapsed)
        {

#if !LOGIC_SERVER
            float fMaxDistance = 4.80f;
            if (this.MainPlayer != null)
            {
                if (m_kCurrentDialogComponent == null)
                {
                    for (int i = 0; i < _beNPCs.GetFightCount(); ++i)
                    {
                        var curNpc = _beNPCs.GetFighter(i);
                        if (null != curNpc && curNpc.GraphicActor != null && null != curNpc.GraphicActor.NpcDialogComponent)
                        {
                            var curDialogComponent = curNpc.GraphicActor.NpcDialogComponent;
                            if (curDialogComponent != null)
                            {
                                var disPos = curNpc.ActorData.MoveData.Position - MainPlayer.ActorData.MoveData.Position;
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
        private void _UpdateData(float delta)
        {
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
            PlayerBaseData.GetInstance().BuffMgr.Update(delta);
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
        protected void UpdateScene(float timeElapsed)
        {
            /// 缓冲加载列表
            if(mMainPlayer !=null)
            {
                mMainPlayer.Update(timeElapsed);
            }
            mFighters.Update(timeElapsed);
            if (_geScene != null)
            {
                _geScene.Update((int)(timeElapsed * (float)GlobalLogic.VALUE_1000));
            }
            _beNPCs.Update((int)(timeElapsed * (float)GlobalLogic.VALUE_1000));
    //        _beRobots.Update(timeElapsed);
    //  UnityEngine.Profiling.Profiler.BeginSample("TownItem");
            mRemoveBuffItem.Clear();
            bool needRefresh = false;
            for (int i = 0; i < mBuffItem.Count;i++)
            {
                var item = mBuffItem[i];
                if (item != null)
                {
                    if (!item.IsRemoved)
                    {
                        item.Update(timeElapsed);
                    }
                    else
                    {
                        _beTownItems.RemoveFighter(item.ActorData.GUID);
                        needRefresh = true;
                        this.mRemoveBuffItem.Add(i);
                    }
                }
                else
                {
                    needRefresh = true;
                    this.mRemoveBuffItem.Add(i);
                }
            }
            if(needRefresh)
            {
                _beTownItems.Refresh();
            }
            for(int i = mRemoveBuffItem.Count - 1; i >= 0;i--)
            {
                mBuffItem.RemoveAt(mRemoveBuffItem[i]);
            }
            //_beTownItems.Update(timeElapsed);
         //   UnityEngine.Profiling.Profiler.EndSample();
            _beProjectiles.Update(timeElapsed);
            if(mPoisonRing != null)
            {
                mPoisonRing.Update(timeElapsed);
            }
            if (mMainPlayer != null)
            {
                PlayerBaseData.GetInstance().Pos = mMainPlayer.ActorData.MoveData.ServerPosition;
                PlayerBaseData.GetInstance().FaceRight = mMainPlayer.ActorData.MoveData.FaceRight;
            }
        }
        public void DoTrap(UInt64 itemObjID,uint count = 0)
        {
            if (mMainPlayer == null) return;
            SceneBattlePlaceTrapsReq req = new SceneBattlePlaceTrapsReq
            {
                //battleID = ChijiDataManager.GetInstance().ChijiBattleID,
                //playerID = mMainPlayer.ActorData.GUID,
                itemGuid = itemObjID,
                itemCount = count,
                x = (uint)(mMainPlayer.ActorData.MoveData.ServerPosition.x * _axisScale),
                y = (uint)(mMainPlayer.ActorData.MoveData.ServerPosition.z * _axisScale)
            };
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }
        public void DoTrapEffect(UInt32 x,UInt32 y)
        {
#if !LOGIC_SERVER
            string str = "Effects/Scene_effects/Scene_Chiji/Prefab/Eff_Scene_Chiji_baozha";
            Vector3 serverPos = new Vector3( x / _axisScale,0.0f, y / _axisScale);
            Vector3 clientPos = serverPos + _serverToClient;
            Vec3 effectPos = new Vec3(clientPos.x, clientPos.z,clientPos.y);
            _geScene.CreateEffect(str, 0.0f, effectPos);
#endif
        }
        protected void _OnUpdateChiji(float timeElapsed)
        {
            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(CurrentSceneID);
            if (scenedata == null)
            {
                return;
            }

            if (scenedata.SceneSubType != CitySceneTable.eSceneSubType.Battle)
            {
                return;
            }

            fChijiItemRefreshTime += timeElapsed;
            if (fChijiItemRefreshTime < fChijiTimeInterval)
            {
                return;
            }

            fChijiItemRefreshTime = 0.0f;

            List<BeItem> nearitems = MainPlayer.FindNearestTownItems();

            if (nearitems != null && nearitems.Count > 0)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<MapItemFrame>())
                {
                    MapItemFrame frame = ClientSystemManager.GetInstance().GetFrame(typeof(MapItemFrame)) as MapItemFrame;

                    if (MainPlayer.MoveState >= BeFighterMain.EMoveState.Moveing || ChijiNearItemNum != nearitems.Count)
                    {
                        frame.RefreshNearItemListCount();
                        ChijiNearItemNum = nearitems.Count;
                    }
                }
                else
                {
                    ClientSystemManager.GetInstance().OpenFrame<MapItemFrame>(FrameLayer.BelowMiddle);
                }
            }
            else
            {
                ClientSystemManager.GetInstance().CloseFrame<MapItemFrame>();
            }
        }
        private void _resetIsSwiching()
        {
            isTownSceneSwitching = false;
            _isSwithScene = false;
        }
        protected void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JobIDReset, _OnJobIDChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetChanged, _OnPetChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateChijiNpcData, _OnUpdateChijiNpcData);

        }

        protected void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JobIDReset, _OnJobIDChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetChanged, _OnPetChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateChijiNpcData, _OnUpdateChijiNpcData);
        }
        void _OnJobIDChanged(UIEvent iEvent)
        {
#if MG_TEST
            ExceptionManager.GetInstance().RecordLog(string.Format("测试服吃鸡:收到职业id {0}",PlayerBaseData.GetInstance().JobTableID));
            GameStatisticManager.GetInstance().UploadLocalLogToServer(string.Format("测试服吃鸡:收到职业id {0}",PlayerBaseData.GetInstance().JobTableID));
#endif
            DestroyMainPlayer();
            CreateMainPlayer();
//             for (int i = 0; i < 20; i++)
//             {
//                 //BeRobotFighter fighter = new BeRobotFighter(new BeFighterData(), this);
//                 //fighter.InitGeActor(_geScene);
//                //// _beRobots.AddFighter(fighter);
//             }
            _InitializeCameraController();
        }
        protected void _InitializeCameraController()
        {
            if (mMainPlayer == null)
            {
                Logger.LogError("_InitializeCameraController ==> _beTownPlayerMain == null");
                return;
            }

            if (_levelData == null)
            {
                Logger.LogError("_InitializeCameraController ==> _levelData == null");
                return;
            }

            _geScene.GetCamera().GetController().AttachTo(mMainPlayer.GraphicActor, _levelData.GetCameraLookHeight(), _levelData.GetCameraAngle(), _levelData.GetCameraDistance());
            _geScene.initScrollScript();
        }
        protected void _InitializeBGM(string path, string environmentBGMPath=null)
        {
            Logger.LogProcessFormat("player bgm with path {0}", path);



            StopBGM();

            if (!string.IsNullOrEmpty(path))
                m_BgmHandle = AudioManager.instance.PlaySound(path, AudioType.AudioStream, 1, true);

            if (!string.IsNullOrEmpty(environmentBGMPath))
                m_EnvGgmHandle = AudioManager.instance.PlaySound(environmentBGMPath, AudioType.AudioEnvironment, 1, true);
        }
        private void DestroyMainPlayer()
        {
            if (mMainPlayer != null)
            {
                mMainPlayer.Dispose();
                mMainPlayer = null;
            }
        }
        private void CreateMainPlayer()
        {
            Vec3 speed = Utility.IRepeate2Vector(TableManager.instance.gst.townRunSpeed);
            PetInfo petInfo = PetDataManager.GetInstance().GetFollowPet();
            BeFighterData data = new BeFighterData
            {
                GUID = PlayerBaseData.GetInstance().RoleID,
                Name = PlayerBaseData.GetInstance().Name,
                JobID = PlayerBaseData.GetInstance().JobTableID,
                RoleLv = PlayerBaseData.GetInstance().Level,
                pkRank = SeasonDataManager.GetInstance().seasonLevel,
                NameColor = PlayerInfoColor.TOWN_PLAYER,
                tittle = 0,
                GuildPost = PlayerBaseData.GetInstance().guildDuty,
                GuildName = PlayerBaseData.GetInstance().guildName,
                petID = petInfo != null ? (int)petInfo.dataId : 0,
                ZoneID = PlayerBaseData.GetInstance().ZoneID,
                AdventureTeamName = AdventureTeamDataManager.GetInstance().GetAdventureTeamName(),
                WearedTitleInfo = PlayerBaseData.GetInstance().WearedTitleInfo,
                GuildEmblemLv = (int)PlayerBaseData.GetInstance().GuildEmblemLv
            };
            data.MoveData.PosLogicToGraph = _logicToGraph;
            data.MoveData.PosServerToClient = _serverToClient;
            data.MoveData.ServerPosition = PlayerBaseData.GetInstance().Pos;
            data.MoveData.FaceRight = PlayerBaseData.GetInstance().FaceRight;
            data.MoveData.MoveSpeed = new Vector3(speed.x, speed.z, speed.y) * PlayerBaseData.GetInstance().MoveSpeedRate;
            //Logger.LogErrorFormat("CreateMainPlayer occu {0} pos {1}", PlayerBaseData.GetInstance().JobTableID, data.MoveData.ServerPosition);

            mMainPlayer = new BeFighterMain(data, this);
            mMainPlayer.InitGeActor(_geScene);
            var buffMgr = PlayerBaseData.GetInstance().BuffMgr;
            for (int i = 0; i < buffMgr.Count(); i++)
            {
                var buff = buffMgr.Get(i);
                if (buff != null)
                {
                    buff.Start(mMainPlayer.GraphicActor);
                }
            }

            if (ChijiDataManager.GetInstance().IsMainPlayerDead)
            {
                mMainPlayer.SetDead();
#if MG_TEST || UNITY_EDITOR
                Logger.LogErrorFormat("吃鸡时序测试----MainPlayerDead, BattleID = {0}, name = {1}, id = {2} from [CreateMainPlayer]", ChijiDataManager.GetInstance().ChijiBattleID, data.Name, data.GUID);
#endif
            }

            //显示正在寻路提示
            BeFighterMain.OnMoveing.RemoveListener(_onMainPlayerMoveing);
            BeFighterMain.OnMoveing.AddListener(_onMainPlayerMoveing);
            BeFighterMain.OnAutoMoving.RemoveListener(_onMainPlayerAutoMoving);
            BeFighterMain.OnAutoMoving.AddListener(_onMainPlayerAutoMoving);

            BeFighterMain.OnAutoMoveFail.RemoveListener(_onMainPlayerAutoMoveFail);
            BeFighterMain.OnAutoMoveFail.AddListener(_onMainPlayerAutoMoveFail);

            BeFighterMain.OnAutoMoveSuccess.RemoveListener(_onMainPlayerAutoMoveSucc);
            BeFighterMain.OnAutoMoveSuccess.AddListener(_onMainPlayerAutoMoveSucc);
        }
        private void _SetMainPlayerShowFindPath(bool isShow)
        {
            if (mMainPlayer != null && mMainPlayer.GraphicActor != null)
                mMainPlayer.GraphicActor.ShowFindPath(isShow);
        }
        private void _onMainPlayerAutoMoveSucc()
        {
            _SetMainPlayerShowFindPath(false);
        }
        private void _onMainPlayerAutoMoveFail()
        {
            _SetMainPlayerShowFindPath(false);
        }
        private void _onMainPlayerAutoMoving(Vector3 pos)
        {
            _SetMainPlayerShowFindPath(true);
        }
        private void _onMainPlayerMoveing(Vector3 pos)
        {
            _SetMainPlayerShowFindPath(false);
        }
        void _OnPetChanged(UIEvent a_event)
        {
            if (MainPlayer != null)
            {
                uint id = (uint)a_event.Param1;
                MainPlayer.CreatePet((int)id);
            }
        }

        void _OnUpdateChijiNpcData(UIEvent a_event)
        {
            _CreateDynamicNpcs();

            List<BattleNpc> npcDataList = ChijiDataManager.GetInstance().NpcDataList;

#if MG_TEST || UNITY_EDITOR
//             for (int i = 0; i < npcDataList.Count; ++i)
//             {
//                 Logger.LogErrorFormat("吃鸡时序测试----收到事件消息创建npc, BattleID = {0}, npcId = {1}, npcGuid = {2}, opType = {3}", ChijiDataManager.GetInstance().ChijiBattleID, npcDataList[i].npcId, npcDataList[i].npcGuid, npcDataList[i].opType);
//             }
#endif
        }

        void _ClearData()
        {
            fChijiItemRefreshTime = 0.0f;
        }
        protected static void _NextFuncOpen()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NextFuncOpen);
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
                netMsgs = msgRets;
                eEnterState = EEnterGameState.Success;
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
            if (SwitchFunctionUtility.IsOpen(103))
            {
                InitializeScene((int)msgEnterScene.mapid, msgEnterScene.mapid == 10100);
            }
            else
            {
                InitializeScene((int)msgEnterScene.mapid);
            }
            yield return Yielders.EndOfFrame;
        }
        protected IEnumerator _SystemInitEnterToChijiSystem(IASyncOperation systemOperation)
        {
            Logger.LogProcessFormat("begin to return to town...this is not error");
            {
                Logger.LogProcessFormat("begin to return to town...this is not error");
                {
                    NetManager.instance.Disconnect(ServerType.RELAY_SERVER);
                    {
                        Logger.LogProcessFormat("由普通战斗返回");
#region net
                        Logger.LogProcessFormat("发送返回城镇的消息");
                        MessageEvents msgEvents = new MessageEvents();
                        msgEvents.AddMessage(SceneSyncSceneObject.MsgID);
                        SceneNotifyEnterScene msgEnterScene = new SceneNotifyEnterScene();
                        if (m_targetSceneID != -1 && m_targetSceneID != 6000)
                        {
                            ScenePlayerChangeSceneReq changeScene = new ScenePlayerChangeSceneReq
                            {
                                curDoorId = 0,
                                dstSceneId = (uint)m_targetSceneID,
                                dstDoorId = 0
                            };
                            ///   mIsReturnToChiji = false;
                            //  msgEvents.AddMessage(SceneItemList.MsgID);
                            yield return MessageUtility.WaitWithResend<ScenePlayerChangeSceneReq, SceneNotifyEnterScene>(ServerType.GATE_SERVER, msgEvents, changeScene, msgEnterScene, true, 8.0f);
                        }
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

                        Logger.LogProcessFormat("收到返回城镇的所有消息");
                        IsNet = true;

                        int currentSceneID = 0;
                        Dictionary<ulong, SceneObject> sceneObjs = null;

                        // 初始化场景信息，其他玩家数据
                        Logger.LogProcessFormat("初始化场景信息，其他玩家数据");

                        //Logger.LogErrorFormat("吃鸡时序测试----接收到服务器消息返回，城镇系统切到吃鸡系统成功");
                        currentSceneID = m_targetSceneID;
                        _InitOtherPlayerData(msgEvents.GetMessageData(SceneSyncSceneObject.MsgID), (uint)currentSceneID);
                        systemOperation.SetProgress(0.7f);
                        yield return Yielders.EndOfFrame;
#endregion
                        PlayerBaseData.GetInstance().Pos = new Vector3(
                          msgEnterScene.pos.x / PlayerBaseData.GetInstance().AxisScale,
                          0.0f,
                          msgEnterScene.pos.y / PlayerBaseData.GetInstance().AxisScale
                          );
                        Logger.LogErrorFormat("init main role pos {0}", PlayerBaseData.GetInstance().Pos);
                        Logger.LogProcessFormat("初始化场景");
                        if (SwitchFunctionUtility.IsOpen(103))
                        {
                            InitializeScene(currentSceneID, currentSceneID == 10100);
                        }
                        else
                        {
                            InitializeScene(currentSceneID);
                        }

                        if (m_targetSceneID == 10100)
                        {
                            ChijiDataManager.GetInstance().SendChijiBattleID();
                        }
                        m_targetSceneID = -1;

                    }
                    yield return Yielders.EndOfFrame;
                    Logger.LogProcessFormat("返回城镇成功");
                }
            }
        }
        private void SetTownSceneSwitchState(bool flag)
        {
            isTownSceneSwitching = flag;
            ClientSystemManager.GetInstance().SendSceneNotifyLoadingInfoByTownSwitchScene(flag);
        }
        public IEnumerator _NetSyncChangeScene(SceneParams data, bool bReturnScene = false/*这个参数不要随便填，“从其他场景返回主城和利亚房间才填true，其他场景之间的相互切换都填false”*/)
        {
            if (isTownSceneSwitching)
            {
                yield break;
            }
            SetTownSceneSwitchState(true);
            var isUseLoadingFrame = true;
            if (MainPlayer != null)
            {
                MainPlayer.SetEnable(false);
            }

            bool bNeedHideBottomLayer = (data.targetSceneID == GuildDataManager.nGuildAreanScenceID);
            if (bNeedHideBottomLayer)
            {
                if (ClientSystemManager.GetInstance().BottomLayer != null)
                {
                    ClientSystemManager.GetInstance().BottomLayer.CustomActive(false);
                }
            }
            _UnBindSceneNetMsgs();

#region SendNetMessage
            //发送切换场景的消息到服务器
            MessageEvents msgEvents = new MessageEvents();
            msgEvents.AddMessage(SceneSyncSceneObject.MsgID);
            SceneNotifyEnterScene msgEnterScene = new SceneNotifyEnterScene();
            bool isPrepareToChijiRoom = false;
            if (this.CurrentSceneID == 10101u && data.targetSceneID == 10100u)
            {
                isPrepareToChijiRoom = true;
            }
            ScenePlayerChangeSceneReq changeScene = new ScenePlayerChangeSceneReq
            {
                curDoorId = isPrepareToChijiRoom ? ChijiDataManager.GetInstance().ChijiBattleID : (uint)data.currDoorID,
                dstSceneId = (uint)data.targetSceneID,
                dstDoorId = isPrepareToChijiRoom ? ChijiDataManager.GetInstance().SceneNodeId : (uint)data.targetDoorID
            };

            // 从吃鸡场景切回到吃鸡准备场景,清空上一局的数据
            if(data.targetSceneID == 10101)
            {
                ChijiDataManager.GetInstance().ClearAllRelatedSystemData();

#if UNITY_EDITOR
                Logger.LogError("吃鸡时序测试----吃鸡场景[开始]返回吃鸡准备场景");
#endif
            } 

            yield return MessageUtility.WaitWithResend<ScenePlayerChangeSceneReq, SceneNotifyEnterScene>(ServerType.GATE_SERVER, msgEvents, changeScene, msgEnterScene, true, 8.0f);
#endregion

#region DealWithReceivedMessage
            //处理接收到的消息

            if (msgEvents.IsAllMessageReceived() == false)
            {
                // 切换场景失败时，若干系统的数据需要重置
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

                ChijiDataManager.GetInstance().SwitchingChijiSceneToPrepare = false;
                ChijiDataManager.GetInstance().SwitchingPrepareToChijiScene = false;

                Logger.LogErrorFormat("[城镇] 切换场景失败(消息未全收到) 待在原地  当前场景ID : {0}, 当前门 : {1}, 目标场景: {2}, 目标门ID {3}", data.currSceneID, data.currDoorID, data.targetSceneID, data.targetDoorID);
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

                yield break;
            }

            if (msgEnterScene.mapid <= 0)
            {
                // 切换场景失败时，若干系统的数据需要重置
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

                ChijiDataManager.GetInstance().SwitchingChijiSceneToPrepare = false;
                ChijiDataManager.GetInstance().SwitchingPrepareToChijiScene = false;

                Logger.LogErrorFormat("[城镇] 切换场景失败(场景id无效) 待在原地  当前场景ID : {0}, 当前门 : {1}, 目标场景: {2}, 目标门ID {3}", data.currSceneID, data.currDoorID, data.targetSceneID, data.targetDoorID);

                yield break;
            }
#endregion

#region LoadingFrame
            //加载loading页面
            var loadingFrame = OpenTownFadingFrame(isUseLoadingFrame);
            loadingFrame.FadingIn(0.4f);

            // 消息返回

            // 从吃鸡准备场景进入到吃鸡场景
            if (data.targetSceneID == 10100)
            {
                ChijiDataManager.GetInstance().ClearPlayerIntrinsicData();
            }

            msgEnterScene.decode(msgEvents.GetMessageData(SceneNotifyEnterScene.MsgID).bytes);
            //初始化网络人物信息
            _InitOtherPlayerData(msgEvents.GetMessageData(SceneSyncSceneObject.MsgID), msgEnterScene.mapid);

            while (loadingFrame.IsOpened() == false)
            {
                yield return Yielders.EndOfFrame;
            }
#endregion

            Logger.LogFormat("change scene rece:{0}", msgEnterScene.mapid);

            // 先处理是否关闭非常驻界面
            ClientSystemManager.GetInstance().CloseFrames();

            var tableData = TableManager.GetInstance().GetTableItem<CitySceneTable>((int)msgEnterScene.mapid);
#region switchScene
            // 根据场景类型做不同处理
            if (tableData != null)
            {
#region LoadSceneInfo
                //服务器返回位置
                PlayerBaseData.GetInstance().Pos = new Vector3(msgEnterScene.pos.x / PlayerBaseData.GetInstance().AxisScale,
                    0.0f,
                    msgEnterScene.pos.y / PlayerBaseData.GetInstance().AxisScale);

                Logger.LogFormat("change scene rece pos:({0},{1})", PlayerBaseData.GetInstance().Pos.x, PlayerBaseData.GetInstance().Pos.z);

                //初始化场景
                if(isPrepareToChijiRoom)
                {
                    var req = new SceneBattleEnterBattleReq
                    {
                        battleID = ChijiDataManager.GetInstance().ChijiBattleID
                    };
                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
                }
                if (SwitchFunctionUtility.IsOpen(103))
                {
                    InitializeScene((int)msgEnterScene.mapid, msgEnterScene.mapid == 10100);
                }
                else
                {
                    InitializeScene((int)msgEnterScene.mapid);
                }
                //关闭其他Frame
                ClearBaseMainFrame();
#endregion

                if (tableData.SceneType == CitySceneTable.eSceneType.BATTLEPEPARE ||
                    tableData.SceneType == CitySceneTable.eSceneType.BATTLE)
                {
                    ShowPkPrePareFrme(tableData);
                }
                var uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                uiEvent.EventParams.CurrentSceneID = (int)msgEnterScene.mapid;
                uiEvent.EventID = EUIEventID.SceneChangedFinish;
                UIEventSystem.GetInstance().SendUIEvent(uiEvent);

            }
#endregion

            loadingFrame.FadingOut(0.2f);
            while (loadingFrame.IsClosed() == false)
            {
                yield return Yielders.EndOfFrame;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SwitchToMainScene);
            GameFrameWork.instance.TownNameShow(tableData.Name);
            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.SceneChangedFinish);


            if (bNeedHideBottomLayer)
            {
                if (ClientSystemManager.GetInstance().BottomLayer != null)
                {
                    ClientSystemManager.GetInstance().BottomLayer.CustomActive(true);
                }
            }

            if (data.onSceneLoadFinish != null)
            {
                data.onSceneLoadFinish.Invoke();
            }

            Logger.LogProcessFormat("[城镇] 切换场景结束 当前场景ID : {0}, 当前门 : {1}, 目标场景: {2}, 目标门ID {3}", data.currSceneID, data.currDoorID, data.targetSceneID, data.targetDoorID);
            _isSwithScene = false;
            //Logger.LogError("NetSyncEnd");
            SetTownSceneSwitchState(false);
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
        protected IEnumerator _errorProcess(eEnumError type, string msg)
        {
            Logger.LogErrorFormat("城镇错误 {0}, {1}", type, msg);
            ClientSystemManager.instance.QuitToLoginSystem(8501);
            yield break;
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

        protected void InitializeScene(int currentSceneId,bool isPKToGame = false)
        {

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
            if (isPKToGame)
            {
                _levelData = ChijiDataManager.GetInstance().sceneData;
                if (_levelData == null)
                {
                    _levelData = DungeonUtility.LoadSceneData(targetCityTable.ResPath);
                }
            }
            else
            {
                _levelData = DungeonUtility.LoadSceneData(targetCityTable.ResPath);
            }
            if (_levelData == null)
            {
                Logger.LogError("_level data is nil");
                return;
            }

            _logicToGraph = _levelData.GetLogicPos();
            _serverToClient = new Vector3(_levelData.GetLogicXSize().x, 0.0f, _levelData.GetLogicZSize().x);
            GridInfo = new PathFinding.GridInfo
            {
                GridSize = _levelData.GetGridSize(),
                GridMinX = _levelData.GetLogicXmin(),
                GridMaxX = _levelData.GetLogicXmax(),
                GridMinY = _levelData.GetLogicZmin(),
                GridMaxY = _levelData.GetLogicZmax(),
                GridBlockLayer = _levelData.GetRawBlockLayer()
            };
            GridInfo.GridDiagonalLength = GridInfo.GridSize.magnitude;

            // 创建场景
            _geScene = new GeSceneEx();
            Logger.LogAsset("Load scene!");
            _geScene.LoadScene(_levelData,false,isPKToGame);
            GeWeatherManager.instance.ChangeWeather(_levelData.GetWeatherMode());
            GeGraphicSetting.instance.GetSetting("PlayerDisplayNum", ref _beDisplayNum);
            CreateSceneObjects();
            // 初始化相机控制器
            _InitializeCameraController();

            _InitializeBGM(targetCityTable.BGMPath, targetCityTable.EnvironmenBGMtPath);

            // 从战斗和吃鸡准备场景切过来都会触发
            _BindSceneNetMsgs();
            // EventSystem被拆成 UIEventSystem 和 GlobalEventSystem
            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.SceneJumpFinished, CurrentSceneID);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TownSceneInited);
        }
        void _RegisterEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MoveSpeedChanged, _OnPlayerMainSpeedChanged);
        }
        void _UnRegisterEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MoveSpeedChanged, _OnPlayerMainSpeedChanged);
        }
        void _OnPlayerMainSpeedChanged(UIEvent a_event)
        {
            if (MainPlayer != null)
            {
                Vec3 speed = Utility.IRepeate2Vector(TableManager.instance.gst.townRunSpeed);
                MainPlayer.ActorData.MoveData.MoveSpeed = new Vector3(speed.x, speed.z, speed.y) * PlayerBaseData.GetInstance().MoveSpeedRate;
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
        bool _DFS(List<SceneNode> nodes, int current, int target, ref List<Vector3> path, DoorNode door, ref List<int> SceneIDList, ref List<int> DoorIDList)
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

            int targetIndex = _Locate(m_sceneNodes, targetSceneID);

            if (targetIndex >= 0)
            {
                for (int i = 0; i < m_sceneNodes.Count; ++i)
                {
                    m_sceneNodes[i].HasVisited = false;
                }

                List<int> sceneIDList = new List<int>();
                List<int> doorIDList = new List<int>();

                int currentIndex = _Locate(m_sceneNodes, CurrentSceneID);
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

        void ClearBaseMainFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<ChijiWaitingRoomFrame>();
            ClientSystemManager.GetInstance().CloseFrame<ChijiMainFrame>();
        }

        private void ShowPkPrePareFrme(CitySceneTable tableData)
        {
            switch (tableData.SceneSubType)
            {
                case CitySceneTable.eSceneSubType.BattlePrepare:
                    {
                        var chijiRoomData = new ChijiRoomData
                        {
                            SceneSubType = tableData.SceneSubType,
                            CurrentSceneID = tableData.ID,
                            TargetTownSceneID = tableData.BirthCity,
                        };

                        ClientSystemManager.GetInstance().OpenFrame<ChijiWaitingRoomFrame>(FrameLayer.Bottom, chijiRoomData);

                        break;
                    }
                case CitySceneTable.eSceneSubType.Battle:
                    {
                        var chijiRoomData = new ChijiRoomData
                        {
                            SceneSubType = tableData.SceneSubType,
                            CurrentSceneID = tableData.ID,
                            TargetTownSceneID = tableData.BirthCity,
                        };

                        ChijiDataManager.GetInstance().SendOccuListReq();
                        ClientSystemManager.GetInstance().OpenFrame<ChijiMainFrame>(FrameLayer.Bottom, chijiRoomData);
                        //ClientSystemManager.GetInstance().OpenFrame<SelectOccupationFrame>(FrameLayer.Middle);
                        break;
                    }
                default:
                    break;
            }
        }
        public void OnGraphicSettingChange(int displayNum)
        {
            return;
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
                for(int i = 0; i < mFighters.GetFightCount();i++)
                {
                    var fighter = mFighters.GetFighter(i);
                    _AddDisplayActor(fighter.ActorData.GUID);
                    if (_beDisplayPlayerList.Count == _beDisplayNum)
                        break;
                }
            }
        }
        // 清理场景
        protected void ClearScene(bool bNeedGC = true)
        {
            if (this.PoisonRing != null)
            {
                this.PoisonRing.Dispose();
            }
            if (mMainPlayer != null)
            {
                mMainPlayer.Dispose();
            }
         //   _beRobots.Clear();
            mPoisonRing = null;
            mMainPlayer = null;
            mFighters.Clear();
            _beTownPlayerBuffs.Clear();
            _beTownItems.Clear();
            _beNPCs.Clear();
            mBuffItem.Clear();
            mRemoveBuffItem.Clear();
            _beProjectiles.Clear();
            _beDisplayPlayerList.Clear();
            _beDisplayNum = 0;

            // 清空场景
            _levelData = null;
            GridInfo = null;
            if (_geScene != null)
            {
                if (SwitchFunctionUtility.IsOpen(103))
                {
                    _geScene.UnloadScene(bNeedGC, CurrentSceneID == 10100);
                }
                else
                {
                    _geScene.UnloadScene(bNeedGC);
                }
                _geScene = null;
            }
            CurrentSceneID = -1;
            _UnBindSceneNetMsgs();
            _UnRegisterEvent();
            _OnClearDelayCreateCache();
            _OnClearDelayCreateItemCache();
        }

        protected Dictionary<uint, Action<MsgDATA>> _mapBindMsgHandle = new Dictionary<uint, Action<MsgDATA>>();

        protected void _BindSceneNetMsgs()
        {
            if (IsNet == true)
            {
                _UnBindSceneNetMsgs();
                _mapBindMsgHandle.Add(SceneSyncObjectProperty.MsgID, new Action<MsgDATA>(this._OnSyncPlayerOthers));
                _mapBindMsgHandle.Add(SceneSyncSceneObject.MsgID, new Action<MsgDATA>(this._OnSyncAddPlayerOthers));
                _mapBindMsgHandle.Add(SceneDeleteSceneObject.MsgID, new Action<MsgDATA>(this._OnSyncRemovePlayerOthers));
                _mapBindMsgHandle.Add(SceneSyncPlayerMove.MsgID, new Action<MsgDATA>(this._OnSyncMovePlayerOthers));
                _mapBindMsgHandle.Add(SceneItemList.MsgID, new Action<MsgDATA>(this._OnSyncSceneItemList));
                Dictionary<uint, Action<MsgDATA>>.KeyCollection keys = _mapBindMsgHandle.Keys;
                IEnumerator enumerator = keys.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    uint msgID = (uint)enumerator.Current;
                    NetProcess.AddMsgHandler(msgID, _mapBindMsgHandle[msgID]);
                }
            }
        }
        public void OnRecvSyncSceneItemAdd(SceneItemAdd res)
        {
            for (int i = 0; i < res.data.Length; i++)
            {
                var mapId = res.data[i].sceneId;
                var items = res.data[i].items;
                for (int j = 0; j < items.Length; j++)
                {
                    var curItem = items[j];
                    BeItemData itemData = new BeItemData
                    {
                        mDropItem = curItem
                    };
                   // var dropTableItem = TableManager.instance.GetTableItem<ProtoTable.ItemTable>((int)curItem.data_id);
                 //   Logger.LogErrorFormat("OnRecvSyncSceneItemAdd uid {0} itemId {1} posX {2} posY{3} name {4}", curItem.guid, curItem.data_id, curItem.pos.x, curItem.pos.y, dropTableItem.Name);
                    BeItem item = new BeItem(itemData, this);
                    _onAddDelayCreateItemCache(item);
                }
            }
        }
        public void OnRecvSceneItemDel(SceneItemDel res)
        {
            if (res == null)
            {
                Logger.LogError("_OnSyncSceneItemDel ==>> msg is nil");
                return;
            }

            for (int i = 0; i < res.guids.Length; i++)
            {
                _onRemoveDelayCreateItemCache(res.guids[i]);
                //Logger.LogErrorFormat("OnRecvSceneItemDel uid {0} ", res.guids[i]);
                for (int j = 0; j < _beTownItems.GetFightCount(); j++)
                {
                    var item = _beTownItems.GetFighter(j);
                    if (item != null && item.ActorData.GUID == res.guids[i])
                    {
                        item.Dispose();
                        _beTownItems.RemoveFighter(j);
                        break;
                    }
                }
            }
            _beTownItems.Refresh();
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
        private void CreateSceneObjects()
        {
            CreateSceneNpcs();
            _CreateDynamicNpcs();

            List<BattleNpc> npcDataList = ChijiDataManager.GetInstance().NpcDataList;

#if MG_TEST
            ExceptionManager.GetInstance().RecordLog(string.Format("测试服吃鸡:城镇初始化 {0}",PlayerBaseData.GetInstance().JobTableID));
            GameStatisticManager.GetInstance().UploadLocalLogToServer(string.Format("测试服吃鸡:城镇初始化 {0}",PlayerBaseData.GetInstance().JobTableID));
#endif

            //创建主角
            CreateMainPlayer();
            // 创建其他玩家
            CreateSceneOtherPlayers();
       //     CreateItemList();
        }

        private void CreateSceneNpcs()
        {
            for (var i = 0; i < _levelData.GetNpcInfoLength(); ++i)
            {
                var info = _levelData.GetNpcInfo(i);

                var npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(info.GetEntityInfo().GetResid());
                if (npcItem == null)
                {
                    Logger.LogErrorFormat("[npc]data.NpcID = {0} is not existed! to 张亮!", info.GetEntityInfo().GetResid());
                    continue;
                }

                if (npcItem.Function == NpcTable.eFunction.guildDungeonActivityChest && !GuildDataManager.GetInstance().IsShowChestModel())
                {
                    continue;
                }

                var data = new BeNPCData
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
                    else if (npcItem.Function == NpcTable.eFunction.guildGuardStatue)
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

                var beTownNpc = new BeNPC(data, this);

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
                _beNPCs.AddFighter(beTownNpc);
            }
        }

        void _CreateDynamicNpcs()
        {
            CitySceneTable targetCityTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(CurrentSceneID);
            if (targetCityTable == null || targetCityTable.SceneType != CitySceneTable.eSceneType.BATTLE 
                || targetCityTable.SceneSubType != CitySceneTable.eSceneSubType.Battle)
            {
                return;
            }

            List<BattleNpc> npcDataList = ChijiDataManager.GetInstance().NpcDataList;
           
            for (int i = 0; i < npcDataList.Count; ++i)
            {
                var npcItem = TableManager.GetInstance().GetTableItem<NpcTable>((int)npcDataList[i].npcId);
                if (npcItem == null)
                {
                    Logger.LogErrorFormat("[npc]data.NpcID = {0} is not existed in Chiji", npcDataList[i].npcId);
                    continue;
                }

                if (npcItem.Function != NpcTable.eFunction.Chiji)
                {
                    continue;
                }

                // 需要显示
                if (npcDataList[i].opType == 1)
                {
                    BeNPC npc = _beNPCs.GetFighter(npcDataList[i].npcGuid);

                    if(npc != null)
                    {
                        // 有对应存在的npc，但是被标记为已移除，那么要改回其状态
                        if(npc.IsRemoved)
                        {
                            npc.CancelRemove();
                        }
                        else
                        {
                            // 对于本身已存在并且也没有移除状态的npc，当服务器数据需要其显示时，不需要做任何处理
                        }
                    }
                    else // 没有对应已存在的npc就创建出来
                    {
                        var data = new BeNPCData
                        {
                            NpcID = (int)npcDataList[i].npcId,
                            ResourceID = npcItem.ResID,
                            Name = npcItem.NpcName,
                            NameColor = PlayerInfoColor.TOWN_NPC
                        };

                        data.GUID = npcDataList[i].npcGuid;
                        data.MoveData.PosLogicToGraph = _logicToGraph;
                        data.MoveData.PosServerToClient = _serverToClient;

                        Vector3 pos = new Vector3(npcDataList[i].pos.x / _axisScale, 0.1f, npcDataList[i].pos.y / _axisScale);
                        data.MoveData.ServerPosition = pos;

                        var beChijiNpc = new BeNPC(data, this);
                        beChijiNpc.InitGeActor(_geScene);

                        if (beChijiNpc.GraphicActor != null)
                        {
                            beChijiNpc.GraphicActor.SetScale(1.0f);
                        }

                        _beNPCs.AddFighter(npcDataList[i].npcGuid, beChijiNpc);
                    }
                }
                else // 需要隐藏
                {
                    BeNPC npc = _beNPCs.GetFighter(npcDataList[i].npcGuid);

                    // 只有已存在的并且没有被标记为已移除的npc才执行移除操作
                    if (npc != null && !npc.IsRemoved)
                    {
                        _beNPCs.RemoveFighter(npcDataList[i].npcGuid);
                    }
                    else
                    {
                        // 本身就不存在的npc，当服务器数据需要其隐藏时，不需要做任何处理
                    }
                }
            }
        }

        protected void _CreateTownItem(BeItem item)
        {
            if (item.IsRemoved) return;
            item.ActorData.MoveData.PosLogicToGraph = _logicToGraph;
            item.ActorData.MoveData.PosServerToClient = _serverToClient;
            var itemData = item.ActorData as BeItemData;
            itemData.MoveData.ServerPosition = new Vector3(itemData.mDropItem.pos.x / _axisScale, 0.1f, itemData.mDropItem.pos.y / _axisScale);
          
            item.InitGeActor(_geScene);
            if(item.IsBuffItem)
            {
                mBuffItem.Add(item);
               // var itemTable = item.ItemTableData.TableData;
                //Logger.LogErrorFormat("_CreateTownItem uid {0} itemId {1} posX {2} posY{3} name {4} pos {5}", itemData.GUID, itemData.mDropItem.data_id, itemData.mDropItem.pos.x, itemData.mDropItem.pos.y, itemTable.Name, itemData.MoveData.ServerPosition);
            }
            _beTownItems.AddFighter(itemData.GUID, item);
        }

        protected void _onAddDelayCreateItemCache(BeItem obj)
        {
            mItemQueue.Add(obj);
        }
        protected void _onRemoveDelayCreateItemCache(ulong guid)
        {
            mItemQueue.RemoveAll((item) => { return item.ActorData.GUID == guid; });
        }
        protected void _OnClearDelayCreateItemCache()
        {
            mItemQueue.Clear();
        }
        protected void _OnUpdateDelayCreateItemCache()
        {
            if (mItemQueue.Count > 0)
            {
                int refreshCount = Mathf.Min(5, mItemQueue.Count);
                for (int i = 0; i < refreshCount && mItemQueue.Count > 0; i++)
                {
                    var current = mItemQueue[0];
                    mItemQueue.RemoveAt(0);
                    _CreateTownItem(current);
                }
            }
        }
        private void CreateItemList()
        {
            for (int i = 0; i < _beTownItems.GetFightCount(); i++)
            {
                var curItem = _beTownItems.GetFighter(i);
                if (curItem != null)
                {
                    curItem.ActorData.MoveData.PosLogicToGraph = _logicToGraph;
                    curItem.ActorData.MoveData.PosServerToClient = _serverToClient;
                    var data = curItem.ActorData as BeItemData;
                    data.MoveData.ServerPosition = new Vector3(data.mDropItem.pos.x / _axisScale, 0.0f, data.mDropItem.pos.y / _axisScale);
                    curItem.InitGeActor(_geScene);
                    if(curItem.IsBuffItem)
                    {
                        mBuffItem.Add(curItem);
                    }
                }
            }
        }
        private void CreateSceneOtherPlayers()
        {
            if (PlayerOthersData == null)
                return;

            var enumerator = PlayerOthersData.GetEnumerator();

            while (enumerator.MoveNext())
            {
                SceneObject sceneObj = enumerator.Current.Value;
                BeFighter beTownPlayer = _CreateTownPlayer(sceneObj, CurrentSceneID);
                if (beTownPlayer != null)
                {
                    mFighters.AddFighter(beTownPlayer.ActorData.GUID, beTownPlayer);
                    _AddDisplayActor(beTownPlayer.ActorData.GUID);
                }
            }
        }
        void _AddDisplayActor(ulong ID)
        {
          ////  if (_beDisplayPlayerList.Count < _beDisplayNum)
          ///  {
                if (!_beDisplayPlayerList.Contains(ID))
                {
                    BeFighter beTownPlayer = mFighters.GetFighter(ID);

                    if (beTownPlayer != null)
                    {
                        if (null == beTownPlayer.GraphicActor)
                        {
                            beTownPlayer.InitGeActor(_geScene);
                           // Logger.LogErrorFormat("_AddDisplayActor ID {0}", ID);
                            if (beTownPlayer.ActorData.ActionData.ActionName.Contains("Idle"))
                            {
                                beTownPlayer.GraphicActor.ChangeAction(beTownPlayer.ActorData.AniNames[(int)ActorActionType.AT_IDLE], 1, true, true, true);
                            }
                            else
                            {
                                string actionName = beTownPlayer.ActorData.AniNames[(int)ActorActionType.AT_RUN];
                                beTownPlayer.GraphicActor.ChangeAction(actionName, 1, true, true, true);

                            }
                        }

                        _beDisplayPlayerList.Add(ID);
                        BeFightBuffManager buffs = null;
                        if (_beTownPlayerBuffs.ContainsKey(ID))
                        {
                            buffs = _beTownPlayerBuffs[ID];
                        }
                        if (buffs == null) return;
                        for (int i = 0; i < buffs.Count(); i++)
                        {
                            var buff = buffs.Get(i);
                            if (buff != null)
                            {
                                buff.Start(beTownPlayer.GraphicActor);
                            }
                        }
                        var otherDeadPlayers = ChijiDataManager.GetInstance().OtherDeadPlayers;
                        for (int i = 0; i < otherDeadPlayers.Count; i++)
                        {
                            if (otherDeadPlayers[i] == beTownPlayer.ActorData.GUID)
                            {
                                beTownPlayer.SetDead();
#if MG_TEST || UNITY_EDITOR
                            Logger.LogErrorFormat("吃鸡时序测试----显示其他死亡玩家, BattleID = {0}, 死者name = {1}, 死者playerid = {2} form [_AddDisplayActor]", ChijiDataManager.GetInstance().ChijiBattleID, beTownPlayer.ActorData.Name, beTownPlayer.ActorData.GUID);
#endif
                            }
                        }
                    }
                }
           //// }
        }
        void _RemoveDisplayActor(ulong ID)
        {
            if (_beDisplayPlayerList.Contains(ID))
            {
                BeFighter beTownPlayer = mFighters.GetFighter(ID);
                if (beTownPlayer != null)
                {
                    if (null != beTownPlayer.GraphicActor)
                        beTownPlayer.RemoveGeActor();
                    _beDisplayPlayerList.Remove(ID);
                }
            }
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
        protected void _OnAddDelayCreateCache(SceneObject obj)
        {
            mObjQueue.Add(obj);
        }
        protected void _OnRemoveDelayCreateCache(ulong objID)
        {
            mObjQueue.RemoveAll((item) => { return item.id == objID; });
        }

        protected void _OnClearDelayCreateCache()
        {
            mObjQueue.Clear();
        }

        private IEnumerator _PrewarmFrames()
        {
            string[] prewarmList = new string[]
            {
                "UIFlatten/Prefabs/Package/PackageNew",
                "UIFlatten/Prefabs/Package/BG",
                "UIFlatten/Prefabs/Package/Bottom",
                "UIFlatten/Prefabs/Package/Tabs",
            };

            for (int i = 0, icnt = prewarmList.Length; i < icnt; ++i)
            {
                GameObject go = AssetLoader.instance.LoadResAsGameObject(prewarmList[i]);
                GameObject.Destroy(go);
                yield return null;
            }
        }

        public void OnSceneLoadEnd()
        {
            CitySceneTable TownTableData = TableManager.instance.GetTableItem<CitySceneTable>(CurrentSceneID);
            if (TownTableData == null)
            {
                Logger.LogErrorFormat("排查主界面角色信息未初始化原因2--CurrentSceneID = {0}", CurrentSceneID);
                return;
            }

            if (TownTableData.SceneType == CitySceneTable.eSceneType.BATTLEPEPARE)
            {
                UpdateChijiWaitingRoomFrame(TownTableData);
            }
            else if(TownTableData.SceneType == CitySceneTable.eSceneType.BATTLE)
            {
                ClientSystemManager.GetInstance().CloseFrame<ChijiWaitingRoomFrame>();
                UpdateChijiMainFrame(TownTableData);
            }

            //头像框请求
            HeadPortraitFrameDataManager.GetInstance().OnSendSceneHeadFrameReq();
        }

        void UpdateChijiWaitingRoomFrame(CitySceneTable TownTableData)
        {
            if (!ClientSystemManager.GetInstance().IsFrameOpen<ChijiWaitingRoomFrame>())
            {
                ChijiRoomData RoomData = new ChijiRoomData
                {
                    SceneSubType = TownTableData.SceneSubType,
                    CurrentSceneID = TownTableData.ID,
                    TargetTownSceneID = TownTableData.BirthCity
                };

                ClientSystemManager.GetInstance().OpenFrame<ChijiWaitingRoomFrame>(FrameLayer.Bottom, RoomData);

            }
        }

        void UpdateChijiMainFrame(CitySceneTable TownTableData)
        {
            if (!ClientSystemManager.GetInstance().IsFrameOpen<ChijiMainFrame>())
            {
                ChijiRoomData RoomData = new ChijiRoomData
                {
                    SceneSubType = TownTableData.SceneSubType,
                    CurrentSceneID = TownTableData.ID,
                    TargetTownSceneID = TownTableData.BirthCity
                };

                ClientSystemManager.GetInstance().OpenFrame<ChijiMainFrame>(FrameLayer.Bottom, RoomData);
            }
        }
        private void _RefreshSceneObject(SceneObject originData, SceneObject sceneObj,UInt64 objId)
        {
            for (int i = 0; i < sceneObj.dirtyFields.Count; i++)
            {
                SceneObjectAttr objAttr = (SceneObjectAttr)sceneObj.dirtyFields[i];
                if (!originData.dirtyFields.Contains(sceneObj.dirtyFields[i]))
                {
                    originData.dirtyFields.Add(sceneObj.dirtyFields[i]);
                }
                if (objAttr == SceneObjectAttr.SOA_LEVEL)
                {
                    originData.level = sceneObj.level;
                    continue;
                }
                else if (objAttr == SceneObjectAttr.SOA_SEASON_LEVEL)
                {
                    originData.seasonLevel = sceneObj.seasonLevel;
                    originData.seasonStar = sceneObj.seasonStar;
                    continue;
                }
                else if (objAttr == SceneObjectAttr.SOA_SEASON_STAR)
                {
                    originData.seasonLevel = sceneObj.seasonLevel;
                    originData.seasonStar = sceneObj.seasonStar;
                }
                else if (objAttr == SceneObjectAttr.SOA_TITLE)
                {
                    originData.title = sceneObj.title;
                }
                else if (objAttr == SceneObjectAttr.SOA_GUILD_NAME)
                {
                    originData.guildName = sceneObj.guildName;
                }
                else if (objAttr == SceneObjectAttr.SOA_GUILD_POST)
                {
                    originData.guildPost = sceneObj.guildPost;
                }
                else if (objAttr == SceneObjectAttr.SOA_PET_FOLLOW)
                {
                    originData.followPetDataId = sceneObj.followPetDataId;
                }
                else if (objAttr == SceneObjectAttr.SOA_NAME)
                {
                    originData.name = sceneObj.name;
                }
                else if (objAttr == SceneObjectAttr.SOA_ZONEID)
                {
                    originData.zoneId = sceneObj.zoneId;
                }
                else if (objAttr == SceneObjectAttr.SOA_BUFFS)
                {
                    //string str = string.Empty;
                    //str += string.Format("NoCreateTownplayer id {0} buffList Count {1} ", objId, sceneObj.buffList == null ? 999999999 : sceneObj.buffList.Count);
                    //for (int j = 0; sceneObj.buffList != null && j < sceneObj.buffList.Count; j++)
                    //{
                    //    var buff = sceneObj.buffList[j];
                    //    str += string.Format("[uid {0} id {1}] ", buff.id, buff.uid);
                    //}
                    //Logger.LogError(str);
                    originData.buffList = sceneObj.buffList;

                }
                else if (objAttr == SceneObjectAttr.SOA_AVATAR)
                {
                    originData.avatar = sceneObj.avatar;
                }
                else if (objAttr == SceneObjectAttr.SOA_OCCU)
                {
                    originData.occu = sceneObj.occu;
                }
                else if (objAttr == SceneObjectAttr.SOA_ADVENTURE_TEAM_NAME)
                {
                    originData.adventureTeamName = sceneObj.adventureTeamName;
                }
                else if (objAttr == SceneObjectAttr.SOA_NEW_TITLE_NAME)
                {
                    originData.wearedTitleInfo = sceneObj.wearedTitleInfo;
                }
                else if (objAttr == SceneObjectAttr.SOA_GUILD_EMBLEM)
                {
                    originData.guildEmblemLvl = sceneObj.guildEmblemLvl;
                }
                else if (objAttr == SceneObjectAttr.SOA_GAMEOPTIONS)
                {
                    originData.gameOptions = sceneObj.gameOptions;
                }
            }
        }
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
             //   Logger.LogErrorFormat("objId {0} player add", objID);
                _OnAddDelayCreateCache(enumerator.Current.Value);

#if MG_TEST || UNITY_EDITOR
                Logger.LogErrorFormat("吃鸡时序测试----同步添加周围其他玩家, BattleID = {0}, player name = {1}", ChijiDataManager.GetInstance().ChijiBattleID, enumerator.Current.Value.name);
#endif
            }
        }
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
             ///还未创建的玩家，信息还是要同步的   Logger.LogWarningFormat("_OnSyncPlayerOthers ==>> can't find scene object {0}", objId);
                var delayPlayer = mObjQueue.Find((item) => { return item.id == objId; });
                if (delayPlayer != null)
                { 
                    sceneObj = new SceneObject();
                    SceneObjectDecoder.DecodePropertys(ref sceneObj, msg.bytes, ref pos, msg.bytes.Length);
                    _RefreshSceneObject(delayPlayer, sceneObj,objId);
                }
                return;
            }
            if (sceneObj == null)
            {
                return;
            }

            SceneObjectDecoder.DecodePropertys(ref sceneObj, msg.bytes, ref pos, msg.bytes.Length);

            BeFighter fighter = mFighters.GetFighter(objId);
            if (fighter != null)
            {
                for (int i = 0; i < sceneObj.dirtyFields.Count; i++)
                {
                    SceneObjectAttr objAttr = (SceneObjectAttr)sceneObj.dirtyFields[i];

                    if (objAttr == SceneObjectAttr.SOA_LEVEL)
                    {
                        fighter.SetPlayerRoleLv(sceneObj.level);
                        continue;
                    }
                    else if (objAttr == SceneObjectAttr.SOA_SEASON_LEVEL)
                    {
                        fighter.SetPlayerPKRank((int)sceneObj.seasonLevel, (int)sceneObj.seasonStar);
                        continue;
                    }
                    else if (objAttr == SceneObjectAttr.SOA_SEASON_STAR)
                    {
                        fighter.SetPlayerPKRank((int)sceneObj.seasonLevel, (int)sceneObj.seasonStar);
                        continue;
                    }
                    else if (objAttr == SceneObjectAttr.SOA_TITLE)
                    {
                        fighter.SetPlayerTittle(sceneObj.title);
                        continue;
                    }
                    else if (objAttr == SceneObjectAttr.SOA_GUILD_NAME)
                    {
                        fighter.SetPlayerGuildName(sceneObj.guildName);
                        continue;
                    }
                    else if (objAttr == SceneObjectAttr.SOA_GUILD_POST)
                    {
                        fighter.SetPlayerGuildDuty(sceneObj.guildPost);
                        continue;
                    }
                    else if (objAttr == SceneObjectAttr.SOA_PET_FOLLOW)
                    {
                        fighter.CreatePet((int)sceneObj.followPetDataId);
                    }
                    else if (objAttr == SceneObjectAttr.SOA_NAME)
                    {
                        fighter.SetPlayerName(sceneObj.name);
                    }
                    else if (objAttr == SceneObjectAttr.SOA_ZONEID)
                    {
                        fighter.SetPlayerZoneID((int)sceneObj.zoneId);
                    }
                    else if (objAttr == SceneObjectAttr.SOA_BUFFS)
                    {
                        //string str = string.Empty;
                        //str += string.Format("Townplayer id {0} name {1} buffList Count {2} ", objId, fighter.ActorData.Name, sceneObj.buffList == null ? 999999999 : sceneObj.buffList.Count);
                        //for (int j = 0; sceneObj.buffList != null && j < sceneObj.buffList.Count; j++)
                        //{
                        //    var buff = sceneObj.buffList[j];
                        //    str += string.Format("[uid {0} id {1}] ", buff.id, buff.uid);
                        //}
                        //Logger.LogError(str);
                        if (sceneObj.buffList == null) return;
                        if (!_beTownPlayerBuffs.ContainsKey(objId))
                        {
                            _beTownPlayerBuffs.Add(objId, new BeFightBuffManager());
                        }
                        var buffs = _beTownPlayerBuffs[objId];
                        var updateBuffList = sceneObj.buffList;
                        List<int> removeBuffList = new List<int>();
                        for (int j = 0; j < buffs.Count(); j++)
                        {
                            var buff = buffs.Get(j);
                            bool find = false;
                            for (int k = 0; k < updateBuffList.Count; k++)
                            {
                                if (updateBuffList[k].uid == buff.GUID)
                                {
                                    find = true;
                                    break;
                                }
                            }
                            if (!find)
                            {
                                removeBuffList.Add(j);
                            }
                        }
                        for (int j = removeBuffList.Count - 1; j >= 0; j--)
                        {
                            var buff = buffs.Get(removeBuffList[j]);
                            if (buff != null)
                            {
                               // Logger.LogErrorFormat("RemoveBuff GUID {0} buffID {1}", buff.GUID, buff.BuffID);
                                buff.Finish(fighter.GraphicActor);
                            }
                            buffs.RemoveAt(removeBuffList[j]);
                        }
                        for (int j = 0; j < updateBuffList.Count; ++j)
                        {
                            var curBuff = updateBuffList[j];
                            var townBuff = buffs.GetBuffByGUID(curBuff.uid);
                            if (townBuff == null)
                            {
                                var newBuff = buffs.AddBuff(curBuff.id, curBuff.uid, objId,curBuff.duration,curBuff.overlay);
                                if (newBuff != null)
                                {
                                    newBuff.Start(fighter.GraphicActor);
                                }
                            }
                            else
                            {
                                townBuff.Refresh(curBuff.duration, curBuff.overlay);
                            }
                        }
                    }
                    else if (objAttr == SceneObjectAttr.SOA_AVATAR || objAttr == SceneObjectAttr.SOA_OCCU)
                    {
                        // 删除对象
                        //Logger.LogErrorFormat("objAttr {0} objId {1}", objAttr, objId);
                        _RemoveDisplayActor(objId);
                        fighter.Dispose();
                        mFighters.RemoveFighter(objId);
                        _OnRemoveDelayCreateCache(objId);
                        // 重新创建
                        BeFighter beTownPlayer = _CreateTownPlayer(sceneObj, CurrentSceneID);
                        if (beTownPlayer != null)
                        {
                            mFighters.AddFighter(beTownPlayer.ActorData.GUID, beTownPlayer);
                            _AddDisplayActor(beTownPlayer.ActorData.GUID);
                        }
                    }
                    else if (objAttr == SceneObjectAttr.SOA_ADVENTURE_TEAM_NAME)
                    {
                        fighter.SetAdventureTeamName(sceneObj.adventureTeamName);
                    }
                    else if (objAttr == SceneObjectAttr.SOA_NEW_TITLE_NAME)
                    {
                        fighter.SetTitleName(sceneObj.wearedTitleInfo);
                    }
                    else if (objAttr == SceneObjectAttr.SOA_GUILD_EMBLEM)
                    {
                        fighter.SetGuildEmblemLv((int)sceneObj.guildEmblemLvl);
                    }
                }
            }
            else
            {
                var originData = PlayerOthersData[objId];
                _RefreshSceneObject(originData, sceneObj,objId);
                fighter = _CreateTownPlayer(PlayerOthersData[objId], CurrentSceneID);
                if (fighter != null)
                {
                    mFighters.AddFighter(objId, fighter);
                }
                _AddDisplayActor(objId);
            }
        }
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
                BeFighter player = mFighters.GetFighter(objID);

#if MG_TEST || UNITY_EDITOR
                if (player != null)
                {
                    Logger.LogErrorFormat("吃鸡时序测试----删除周围其他玩家, BattleID = {0}, player name = {1}, playerid = {2}, IsDead = {3}", ChijiDataManager.GetInstance().ChijiBattleID, player.GetPlayerName(), player.GUID, player.IsDead);
                }
#endif
                
                if (player != null)
                {
                    _RemoveDisplayActor(objID);
                    player.Dispose();
                    mFighters.RemoveFighter(objID);
                    _beTownPlayerBuffs.Remove(objID);
                }

               // if (_beDisplayPlayerList.Count < _beDisplayNum && _beDisplayPlayerList.Count < mFighters.GetFightCount())
              //  {
                    for (int i = 0; i < mFighters.GetFightCount(); i++)
                    {
                        var fighter = mFighters.GetFighter(i);
                        if (fighter != null)
                        {
                            _AddDisplayActor(fighter.ActorData.GUID);
                            if (_beDisplayPlayerList.Count == _beDisplayNum)
                                break;
                        }
                    }
             //   }
                _OnRemoveDelayCreateCache(objID);
            }
        }
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
            BeFighter fighter = mFighters.GetFighter(msgMove.id);
            if(fighter != null)
            {
                fighter.AddMoveCommand(
                  new Vector3(msgMove.pos.x / _axisScale, 0.0f, msgMove.pos.y / _axisScale) + fighter.ActorData.MoveData.PosServerToClient,
                  new Vector3(msgMove.dir.x / _axisScale, 0.0f, msgMove.dir.y / _axisScale),
                  msgMove.dir.faceRight >= 1
                  );
            }
        }
        private void _OnSyncSceneItemList(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnSyncSceneItemList ==>> msg is nil");
                return;
            }
            var sceneItemList = new SceneItemList();
            sceneItemList.decode(msg.bytes);
            _beTownItems.Clear();
            mItemQueue.Clear();

            for (int i = 0; i < sceneItemList.infoes.Length; i++)
            {
                for (int j = 0; j < sceneItemList.infoes[i].items.Length; j++)
                {
                    //Logger.LogErrorFormat("_OnSyncSceneItemList {0} {1}", sceneItemList.infoes[i].items[j].data_id, sceneItemList.infoes[i].items[j].guid);
                //    string addItemLog = string.Format("_OnSyncSceneItemList AddItem {0} {1}", sceneItemList.infoes[i].items[j].data_id, sceneItemList.infoes[i].items[j].guid);
                //    ChijiDataManager.GetInstance().PushLog(addItemLog);
                    BeItemData itemData = new BeItemData
                    {
                        mDropItem = sceneItemList.infoes[i].items[j]
                    };
                    BeItem item = new BeItem(itemData, this);
                    _beTownItems.AddFighter(sceneItemList.infoes[i].items[j].guid, item);
                }
            }
            CreateItemList();
        }
        protected BeFighter _CreateTownPlayer(SceneObject sceneObj, int a_nSceneID)
        {
            BeFighter beTownPlayer = null;
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
                BeFighterData data = new BeFighterData
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
                    awaken = sceneObj.awaken
                };
                if (sceneObj.vip != null)
                    data.vip = sceneObj.vip.level;
                data.MoveData.PosLogicToGraph = _logicToGraph;
                data.MoveData.PosServerToClient = _serverToClient;
                data.MoveData.ServerPosition = new Vector3(sceneObj.pos.x / _axisScale, 0.0f, sceneObj.pos.y / _axisScale);
                data.MoveData.FaceRight = sceneObj.dir.faceRight >= 1;
                Vec3 speed = Global.Settings.townPlayerRun ? Utility.IRepeate2Vector(TableManager.instance.gst.townRunSpeed) : Utility.IRepeate2Vector(TableManager.instance.gst.townWalkSpeed);
                data.MoveData.MoveSpeed = new Vector3(speed.x, speed.z, speed.y) * (sceneObj.moveSpeed / (float)GlobalLogic.VALUE_1000);
                data.MoveData.TargetDirection = new Vector3(sceneObj.dir.x / _axisScale, 0.0f, sceneObj.dir.y / _axisScale);
                if (sceneObj.buffList != null)
                {
                    if (!_beTownPlayerBuffs.ContainsKey(sceneObj.id))
                    {
                        _beTownPlayerBuffs.Add(sceneObj.id, new BeFightBuffManager());
                    }
                    var buffMgr = _beTownPlayerBuffs[sceneObj.id];

                    for (int j = 0; j < sceneObj.buffList.Count; ++j)
                    {
                        var curBuff = sceneObj.buffList[j];
                        var newBuff = buffMgr.GetBuffByGUID(curBuff.uid);
                        if (newBuff != null)
                        {
                            newBuff.Refresh(curBuff.duration, curBuff.overlay);
                        }
                        else
                        {
                            buffMgr.AddBuff(curBuff.id, curBuff.uid, sceneObj.id, curBuff.duration, curBuff.overlay);
                        }
                    }
                }
                // 创建其他玩家
                beTownPlayer = new BeFighter(data, this);

            }
            catch (Exception e)
            {
                Logger.LogError("TownPlayer Create Error!");
            }
            return beTownPlayer;
        }

        protected void _CreateTownPlayer(SceneObject sceneObj)
        {
            if (sceneObj == null)
            {
                return;
            }

            BeFighter fighter = _CreateTownPlayer(sceneObj, CurrentSceneID);

            if (fighter != null)
            {
                ulong objID = sceneObj.id;
                // 更新信息记录
                if (PlayerOthersData.ContainsKey(objID))
                {
                    PlayerOthersData[objID] = sceneObj;
                    var originalFighter = mFighters.GetFighter(objID);
                    if (originalFighter != null)
                    {
                        originalFighter.Dispose();
                        mFighters.RemoveFighter(objID);
                        mFighters.AddFighter(objID,fighter);
                    }

                    if (_beDisplayPlayerList.Contains(fighter.ActorData.GUID))
                    {
                        fighter.InitGeActor(_geScene);
                        BeFightBuffManager buffs = null;
                        if (_beTownPlayerBuffs.ContainsKey(fighter.ActorData.GUID))
                        {
                            buffs = _beTownPlayerBuffs[fighter.ActorData.GUID];
                        }
                        if (buffs == null) return;
                        for (int i = 0; i < buffs.Count(); i++)
                        {
                            var buff = buffs.Get(i);
                            if (buff != null)
                            {
                                buff.Start(fighter.GraphicActor);
                            }
                        }
                        var otherDeadPlayers = ChijiDataManager.GetInstance().OtherDeadPlayers;
                        for (int i = 0; i < otherDeadPlayers.Count; i++)
                        {
                            if (otherDeadPlayers[i] == fighter.ActorData.GUID)
                            {
                                fighter.SetDead();
#if MG_TEST || UNITY_EDITOR
                                Logger.LogErrorFormat("吃鸡时序测试----显示其他死亡玩家, BattleID = {0}, 死者name = {1}, 死者playerid = {2} form [_CreateTownPlayer]", ChijiDataManager.GetInstance().ChijiBattleID, fighter.ActorData.Name, fighter.ActorData.GUID);
#endif
                            }
                        }
                    }
                    else
                    {
                        fighter.RemoveGeActor();
                    }
                }
                else
                {
                    PlayerOthersData.Add(objID, sceneObj);
                    var originalFighter = mFighters.GetFighter(objID);
                    if (originalFighter == null)
                    {
                        mFighters.AddFighter(objID, fighter);
                    }
                    _AddDisplayActor(objID);
                }

                Logger.LogProcessFormat("other player ID:{0}", objID);
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

        public BeFighter GetPlayer(ulong playerID)
        {
            if (playerID == PlayerBaseData.GetInstance().RoleID)
            {
                return MainPlayer;
            }
            return mFighters.GetFighter(playerID);
        }
        public void OnNotifyTownPlayerLvChanged(UInt32 iPlayerID, Int32 iLevel)
        {
            if (MainPlayer != null)
            {
                BeFighterData curMainData = MainPlayer.ActorData as BeFighterData;
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

            BeFighter curPlayer = mFighters.GetFighter(iPlayerID);
            if (curPlayer != null)
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    curPlayer.GraphicActor.OnLevelChanged(PlayerBaseData.GetInstance().Level);
                    Logger.LogProcessFormat("[Tittle] OtherPlayer TittleChanged!");
                }
            }
        }

        public void OnNotifyTownPlayerGuildNameChanged(UInt32 iPlayerID, string name)
        {
            if (MainPlayer != null)
            {
                BeFighterData curMainData = MainPlayer.ActorData as BeFighterData;
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

            BeFighter curPlayer = mFighters.GetFighter(iPlayerID);
            if (curPlayer != null)
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
                BeFighterData curMainData = MainPlayer.ActorData as BeFighterData;
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

            BeFighter curPlayer = mFighters.GetFighter(iPlayerID);
            if (curPlayer != null)
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    curPlayer.GraphicActor.UpdateAdventTeamName(name);
                    Logger.LogProcessFormat("[Tittle] OtherPlayer AdventTeam TittleChanged!");
                }
            }
        }

        public void OnNotifyTownPlayerTitleNameChanged(UInt32 iPlayerID, Protocol.PlayerWearedTitleInfo playerWearedTitleInfo)
        {
            if (MainPlayer != null)
            {
                BeFighterData curMainData = MainPlayer.ActorData as BeFighterData;
                if (curMainData != null && (curMainData.GUID == iPlayerID || iPlayerID == 0))
                {
                    if (MainPlayer.GraphicActor != null)
                    {
                        MainPlayer.GraphicActor.UpdateTitleName(playerWearedTitleInfo);
                        Logger.LogProcessFormat("[Tittle] MainRole Title Changed!");
                    }
                    return;
                }
            }
            BeFighter curPlayer = mFighters.GetFighter(iPlayerID);
            if (curPlayer != null)
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    curPlayer.GraphicActor.UpdateTitleName(playerWearedTitleInfo);
                    Logger.LogProcessFormat("[Tittle] MainRole Title Changed!");
                }
            }
        }
        public void OnNotifyTownPlayerGuildLvChanged(UInt32 iPlayerID, uint guildEmblemLv)
        {
            if (MainPlayer != null)
            {
                BeFighterData curMainData = MainPlayer.ActorData as BeFighterData;
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

            BeFighter curPlayer = mFighters.GetFighter(iPlayerID);
            if (curPlayer != null)
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
                BeFighterData curMainData = MainPlayer.ActorData as BeFighterData;
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

            BeFighter curPlayer = mFighters.GetFighter(iPlayerID);
            if (curPlayer != null)
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    curPlayer.GraphicActor.OnGuildPostChanged(duty);
                    Logger.LogProcessFormat("[Tittle] OtherPlayer Duty Changed!");
                }
            }
        }
        public void OnNotifyTownPlayerTittleChanged(UInt32 iPlayerID, Int32 iTittle)
        {
            if (MainPlayer != null)
            {
                BeFighterData curMainData = MainPlayer.ActorData as BeFighterData;
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


            BeFighter curPlayer = mFighters.GetFighter(iPlayerID);
            if (curPlayer != null)
            {
                if (curPlayer != null && curPlayer.GraphicActor != null)
                {
                    
                    curPlayer.GraphicActor.OnTittleChanged(iTittle);
                    if (curPlayer.GrassStatus == BeFighter.GRASS_STATUS.IN_GRASS)
                    {
                        curPlayer.GraphicActor.SetActorVisible(false);
                    }
                    Logger.LogProcessFormat("[Tittle] OtherPlayer TittleChanged!");
                }
            }
        }
    }
}

