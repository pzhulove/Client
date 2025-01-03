using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Network;
using Protocol;
using ProtoTable;

namespace GameClient
{
    /// <summary>
    /// 春节活动 藏宝图副本
    /// </summary>
    public class TreasureMapBattle:BaseBattle
    {
        private int mDoorShiftCount = 0; //切换门的次数
        private int mBossX = 0;
        private int mBossY = 0;
        private int mBossIndex = -1;
        private int mEndX = 0;
        private int mEndY = 0;
        private uint mInitSeed = 0;
        private bool mFirstShiftDoor = true;
        private bool mIsNormalFinsh = false;
        private bool mIsChapterNoPassed = false;
        private int mThisRoomMonsterCreatedCount;
        private int mStartPassDoor = 0;
        private bool mStarted = false;
        private bool mIsBossDead = false;
        private int mGlobalRegionIdGen = 0;
        private IBeEventHandle mBossEventHandle = null;     //boss死亡事件句柄
        private int mBossId = -1;
        private int mCurBossHP = -1;

#if !LOGIC_SERVER
        private Coroutine mDungeonRaceEndReqCoroutine = null;
#endif
        private List<BattlePlayer> mCachBeActor = new List<BattlePlayer>();
        private List<int> mRegionIdLibrary = new List<int>();  //法阵随机库
        private List<int> mReduceRegionIds = new List<int>();  //有损随机库
        private List<int> mCurRegionIdLibary = new List<int>(); //当前法阵随机库
        private List<int> mCurReduceRegionIds = new List<int>();//当前法阵有损随机库
        private int mDropItemCount = 0;                         //宝箱总数（全地图）
        public void AddRegionIdLibary(int id)
        {
            if (!mRegionIdLibrary.Contains(id))
            {
                mRegionIdLibrary.Add(id);
                mCurRegionIdLibary.Add(id);
            }
        }
        public void AddReduceIdLibary(int id)
        {
            if (!mReduceRegionIds.Contains(id))
            {
                mReduceRegionIds.Add(id);
                mCurReduceRegionIds.Add(id);
            }
        }
        /// 打破宝箱后随机法阵规则
        // 整个关卡中必然在有损随机库中必然出现一次法阵
        // 非有损随机库中如果随机出来的法阵在有损随机库中 那么删除该非有损随机库中随机出来的元素
        public void GenerateRegion(VInt3 pos)
        {
            if(mDropItemCount <= mCurReduceRegionIds.Count && mCurReduceRegionIds.Count > 0)
            {
                int regionId = mCurReduceRegionIds[mCurReduceRegionIds.Count - 1];
                mCurReduceRegionIds.RemoveAt(mCurReduceRegionIds.Count - 1);
                AddRegionInfo(regionId, pos);
                mDropItemCount--;
            }
            else if(mCurRegionIdLibary.Count > 0)
            {
                var index = FrameRandom.InRange(0, mCurRegionIdLibary.Count);
                int regionId = mCurRegionIdLibary[index];
                if (mCurReduceRegionIds.Contains(regionId))
                {
                    mCurRegionIdLibary.RemoveAt(index);
                    mCurReduceRegionIds.Remove(regionId);
                }
                AddRegionInfo(regionId, pos);
                mDropItemCount--;
            }
            
        }
        public TreasureMapBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {
        }
        protected sealed override void _onStart()
        {
            GetChapterPassed();
            _updateDungeonMap();
        }

        private void _updateDungeonMap()
        {
#if !LOGIC_SERVER
            var frame = _getValidTreasureMapFrame();
            if (frame != null
                && frame.TreasureDungeonMap != null
                && mDungeonManager != null
                && mDungeonManager.GetDungeonDataManager() != null)
            {
                frame.TreasureDungeonMap.SetDungeonData(mDungeonManager.GetDungeonDataManager().asset);
            }

            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDungeonMap>();
            if (battleUI != null && battleUI.dungeonMapCom != null)
            {
                battleUI.dungeonMapCom.InitTreasureMapDungeonUI();
            }
#endif
        }

        private void GetChapterPassed()
        {
            mIsChapterNoPassed = ComCommonChapterInfo._isAllDiffNoScores(dungeonManager.GetDungeonDataManager().id.dungeonID);
        }
        public override void OnAfterSeedInited()
        {
            if (dungeonManager != null && dungeonManager.GetDungeonDataManager() != null)
            {
                mInitSeed = FrameRandom.GetSeed();
                dungeonManager.GetDungeonDataManager().OnInitDungeonData(FrameRandom);
                dungeonManager.GetDungeonDataManager().GetBossRoom(ref mBossX,ref mBossY,ref mBossIndex);
                dungeonManager.GetDungeonDataManager().GetEndRoom(ref mEndX, ref mEndY);
                mDropItemCount = dungeonManager.GetDungeonDataManager().GetRoomCountByType((byte)TreasureMapGenerator.ROOM_TYPE.DROPITEM_ROOM);
            }
        }
        static readonly Dictionary<int, List<int[]>> linkDir = new Dictionary<int, List<int[]>>
        {
            { (int)TreasureMapGenerator.POS_TYPE.LEFT_TOP_CORNER, new List<int[]>{ new int[] { 1, 0}, new int[] { 0,1} } },//左上
            { (int)TreasureMapGenerator.POS_TYPE.LEFT_CORNER, new List<int[]>{ new int[] { 1, 0 }, new int[] { 0, 1}, new int[] { 0, -1} } },//左
            { (int)TreasureMapGenerator.POS_TYPE.LEFT_BOTTOM_CORNER, new List<int[]>{ new int[] { 1,0}, new int[] { 0, -1 } } },//左下
            { (int)TreasureMapGenerator.POS_TYPE.RIGHT_TOP_CORNER, new List<int[]>{new int[] { -1, 0}, new int[] { 0, 1 } } },//右上
            { (int)TreasureMapGenerator.POS_TYPE.RIGHT_CORNER,new List<int[]>{new int[] { -1,0}, new int[] { 0, 1 }, new int[] { 0,-1 } } },//右
            { (int)TreasureMapGenerator.POS_TYPE.RIGHT_BOTTOM_CORNER,new List<int[]>{new int[] { -1,0},new int[] { 0, -1 } } },//右下
            { (int)TreasureMapGenerator.POS_TYPE.TOP_CORNER,new List<int[]>{new int[] { 1, 0 }, new int[] { 0, 1 }, new int[] { -1, 0 }} },//上
            { (int)TreasureMapGenerator.POS_TYPE.BOTTOM_CORNER,new List<int[]>{new int[] { 0, -1 }, new int[] { -1, 0 }, new int[] { 1, 0 } } },//下
            { (int)TreasureMapGenerator.POS_TYPE.NORMAL, new List<int[]>{new int[] { -1, 0 },new int[] { 0, -1 },new int[] {1,0 },new int[] {0,1 } } },//居中
        };

        //连通图 八方向
        static readonly Dictionary<int, List<int[]>> crossLinkDir = new Dictionary<int, List<int[]>> //八方向
        {
            { (int)TreasureMapGenerator.POS_TYPE.LEFT_TOP_CORNER, new List<int[]>{ new int[] { 1, 0 }, new int[] { 0, 1} ,new int[] {1,1} } },//左上
            { (int)TreasureMapGenerator.POS_TYPE.LEFT_CORNER, new List<int[]>{ new int[] { 1, 0 }, new int[] { 0, 1}, new int[] { 0, -1},new int[] { 1,1},new int[] { 1,-1} } },//左
            { (int)TreasureMapGenerator.POS_TYPE.LEFT_BOTTOM_CORNER, new List<int[]>{ new int[] { 1, 0}, new int[] { 0, -1 },new int[] { 1,-1} } },//左下
            { (int)TreasureMapGenerator.POS_TYPE.RIGHT_TOP_CORNER, new List<int[]>{new int[] { -1, 0}, new int[] { 0, 1 },new int[] { -1,1} } },//右上
            { (int)TreasureMapGenerator.POS_TYPE.RIGHT_CORNER,new List<int[]>{new int[] { -1, 0} ,new int[] { 0, -1 }, new int[] { 0,1},new int[] { -1,-1},new int[] { -1,1} } },//右
            { (int)TreasureMapGenerator.POS_TYPE.RIGHT_BOTTOM_CORNER,new List<int[]>{new int[] { -1, 0},new int[] { 0,-1 },new int[] {-1,-1} } },//右下
            { (int)TreasureMapGenerator.POS_TYPE.TOP_CORNER,new List<int[]>{new int[] { 1, 0 },new int[] { -1, 0 },new int[] { 0,1 } ,new int[] { 1,1},new int[] {-1,1 } } },//上
            { (int)TreasureMapGenerator.POS_TYPE.BOTTOM_CORNER,new List<int[]>{new int[] { -1, 0 },new int[] { 0, -1 },new int[] { 1, 0 },new int[] { -1,-1},new int[] { 1,-1} } },//下
            { (int)TreasureMapGenerator.POS_TYPE.NORMAL, new List<int[]>{new int[] { -1, 0 },new int[] {0,-1 },new int[] { 1,0 },new int[] {0,1 },new int[] { -1,-1 },new int[] { 1,1},new int[] { 1,-1},new int[] {1,1} } },//居中
        };

        static public TreasureMapGenerator.POS_TYPE GetPosType(int x, int y)
        {
            if (x == 0 && y == 0)
            {
                return TreasureMapGenerator.POS_TYPE.LEFT_TOP_CORNER;
            }
            else if (y == TreasureMapGenerator.MAX_ROW - 1 && x == 0)
            {
                return TreasureMapGenerator.POS_TYPE.LEFT_BOTTOM_CORNER;
            }
            else if (y == 0 && x == TreasureMapGenerator.MAX_COL - 1)
            {
                return TreasureMapGenerator.POS_TYPE.RIGHT_TOP_CORNER;
            }
            else if (x == TreasureMapGenerator.MAX_COL - 1 && y == TreasureMapGenerator.MAX_ROW - 1)
            {
                return TreasureMapGenerator.POS_TYPE.RIGHT_BOTTOM_CORNER;
            }
            else if (y == 0)
            {
                return TreasureMapGenerator.POS_TYPE.TOP_CORNER;
            }
            else if (y == TreasureMapGenerator.MAX_ROW - 1)
            {
                return TreasureMapGenerator.POS_TYPE.BOTTOM_CORNER;
            }
            else if (x == 0)
            {
                return TreasureMapGenerator.POS_TYPE.LEFT_CORNER;
            }
            else if (x == TreasureMapGenerator.MAX_COL - 1)
            {
                return TreasureMapGenerator.POS_TYPE.RIGHT_CORNER;
            }
            return TreasureMapGenerator.POS_TYPE.NORMAL;
        }
        //如果已经在boss房间了，那么boss不需要移动
        public bool IsInBossRoom()
        {
            var dungeonDataManager = dungeonManager.GetDungeonDataManager();
            if (dungeonDataManager != null)
            {
                var data = dungeonDataManager.CurrentDataConnect();
                if (data == null) return false;
                if (data.GetPositionX() == mBossX && data.GetPositionY() == mBossY)
                {
                    return true;
                }
            }
            return false;
        }
        public void AddRegionInfo(int resId,VInt3 pos)
        {
            if (mDungeonManager == null || mDungeonManager.GetDungeonDataManager() == null||mDungeonManager.GetBeScene() == null) return;
            mGlobalRegionIdGen++;
            var regionInfo =  new CustomSceneRegionInfo(resId, pos, mGlobalRegionIdGen);
            mDungeonManager.GetDungeonDataManager().AddDynamicRegion(regionInfo);
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var regions = mDungeonData.CurrentDynamicRegions();
             mBeScene.CreateDynamicRegion(_doorTriggerAllPlayers, regionInfo, regions);
          
        }
        //是否在宝藏房间
        public bool IsInEndRoom()
        {
            var dungeonDataManager = dungeonManager.GetDungeonDataManager();
            if (dungeonDataManager != null)
            {
                var data = dungeonDataManager.CurrentDataConnect();
                if (data == null) return false;
                if (data.GetPositionX() == mEndX && data.GetPositionY() == mEndY)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// boss逃离
        /// </summary>
        public void OnBossFleeAway(BeActor boss)
        { 
            _MoveBoss();
            if (boss != null && !boss.IsDead())
            {
                mCurBossHP = boss.attribute.GetHP();
            }
#if !LOGIC_SERVER
            var treasureMap = _getValidTreasureMapFrame();
            if (treasureMap != null && treasureMap.TreasureDungeonMap != null)
            {
                treasureMap.TreasureDungeonMap.RefreshBoss(mBossX, mBossY, IsNearBoss());
            }
#endif
        }
        //是否当前房间在boss房间周围（8方向）
        public bool IsNearBoss()
        {
            if (mIsBossDead) return false;
            var dungeonDataManager = dungeonManager.GetDungeonDataManager();
            if(dungeonDataManager != null)
            {
               var data =  dungeonDataManager.CurrentDataConnect();
                if (data == null) return false;
                if (data.GetPositionX() == mBossX && data.GetPositionY() == mBossY)
                {
                    return false;
                }
                var posType = GetPosType(data.GetPositionX(), data.GetPositionY());
                if (crossLinkDir.ContainsKey((int)posType))
                {
                    var linkList = crossLinkDir[(int)posType];
                    for(int i = 0; i < linkList.Count;i++)
                    {
                        var moveDir = linkList[i];
                        var nextX = data.GetPositionX() + moveDir[0];
                        var nextY = data.GetPositionY() + moveDir[1];
                        if (nextX == mBossX && nextY == mBossY)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private void _MoveBoss()
        {
            var dungeonDataManager = dungeonManager.GetDungeonDataManager();
            if (dungeonDataManager == null) return;
            var oldBossX = mBossX;
            var oldBossY = mBossY;
            int nextX = 0;
            int nextY = 0;
            int nextBossIndex = -1;
            //通过dungeonDataMananger 领接信息随机领接房间，如果失败，通过领接图直接暴力随机坐标点()
            if (dungeonDataManager.GenBossNextRoom(mBossIndex,FrameRandom, ref nextX,ref nextY,ref nextBossIndex))
            {

                mBossX = nextX;
                mBossY = nextY;
                mBossIndex = nextBossIndex;
                return;
            }
            Logger.LogErrorFormat("GenBossNextRoom failed :{0} {1} {2} {3}", mBossIndex,mBossX,mBossY,mInitSeed);
            var posType = GetPosType(mBossX,mBossY);
            bool findPath = false;
            if (linkDir.ContainsKey((int)posType))
            {
                var linkList = linkDir[(int)posType];
                int index = FrameRandom.InRange(0, linkList.Count);
                var dir = linkList[index];
                nextX = mBossX + dir[0];
                nextY = mBossY + dir[1];
                int loopCount = linkList.Count;
                while (dungeonDataManager != null && loopCount > 0 )
                {
                    //临近可以走的房间一定不能是宝藏房间
                    if ((nextX != mEndX || nextY != mEndY) && dungeonDataManager.CanWalkToRoom(mBossX, mBossY,nextX, nextY, ref nextBossIndex))
                    {
                        findPath = true;
                        mBossX = nextX;
                        mBossY = nextY;
                        mBossIndex = nextBossIndex;
                        break;
                    }
                    index = linkList.Count - loopCount;
                    dir = linkList[index];
                    nextX = mBossX + dir[0];
                    nextY = mBossY + dir[1];
                    loopCount--;
                }
                
            }
            if (!findPath)
            {
                Logger.LogErrorFormat("Boss Can not Move at {0} {1} Type {2} End {3} {4} Seed {5}", mBossX, mBossY, posType, mEndX, mEndY, mInitSeed);
            }
            else
            {
                Logger.LogErrorFormat("Boss from  {0} {1} to {2} {3} shiftDoor {4}", oldBossX, oldBossY, mBossX, mBossY, mDoorShiftCount);
            }
        }

#if !LOGIC_SERVER
        TreasureMapFrame treasureMapFrame;
        private TreasureMapFrame _getValidTreasureMapFrame()
        {

            if(treasureMapFrame == null)
            {
                treasureMapFrame = ClientSystemManager.instance.OpenFrame<TreasureMapFrame>(FrameLayer.Middle) as TreasureMapFrame;
            }
            return treasureMapFrame;
        }
#endif
        protected sealed override void _createEntitys()
        {
            base._createEntitys();

            // 玩家朝向问题
            var mBeScene = mDungeonManager.GetBeScene();
            if (null != mBeScene)
            {
                BeActor monster = mBeScene.FindAPendingMonster();
                if (null != monster)
                {
                    bool faceFlag = monster.GetFace();
                    var players = mDungeonPlayers.GetAllPlayers();
                    if (null != players)
                    {
                        int countPlayer = players.Count;
                        for (int i = 0; i < countPlayer; i++)
                        {
                            BeActor curActor = players[i].playerActor;
                            if (null != curActor)
                            {
                                bool faceLeft = (monster.GetPosition().x - curActor.GetPosition().x) > 0 ? false : true;
                                curActor.SetFace(faceLeft);
                            }
                        }
                    }
                }
            }

#if !LOGIC_SERVER
            if (mStarted)
            {
                var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
                if (battleUI != null)
                {
                    battleUI.ShowLevelTip(mDungeonManager.GetBeScene().sceneData.GetTipsID());
                }
            }
#endif
        }
        protected sealed override void _onEnd()
        {

            mStarted = false;
            _clearBossData();
#if !LOGIC_SERVER
            if (mDungeonRaceEndReqCoroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(mDungeonRaceEndReqCoroutine);
            }
#endif
        }
        protected sealed override void _createDoors()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var doors = mDungeonData.CurrentDoors();

            var chanceDoorType = mBeScene.GetChanceDoorType();


            for (int i = 0; i < doors.Count; ++i)
            {
                if (null != doors[i].door)
                {
                    mBeScene.AddTransportDoor(doors[i].door.GetRegionInfo(), _doorTriggerAllPlayers, _doorCallback, doors[i].isconnectwithboss, doors[i].isvisited, doors[i].doorType, doors[i].isEggDoor, doors[i].materialPath);
                }
            }
        }
        protected void _resetPlayerActor(bool force = true)
        {
            var players = mDungeonPlayers.GetAllPlayers();

            for (int i = 0; i < players.Count; ++i)
            {
                var beActor = players[i].playerActor;
                if (null != beActor)
                {
                    if (force)
                    {
                        beActor.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
                    }
                    else
                    {
                        beActor.sgPushState(new BeStateData((int)ActionState.AS_IDLE));
                    }

                    beActor.Reset();
                    beActor.SetAttackButtonState(ButtonState.RELEASE);
                }
            }
        }
        protected void _changeAreaFade(uint fadein, uint fadeout, DungeonFadeCallback cb, DungeonFadeCallback finishcb = null)
        {
            mDungeonManager.OpenFade(
                () =>
                {
                    if (FrameSync.instance != null)
                    {
                        // 在这里重置一些东西
                        FrameSync.instance.ResetMove();

                        // 禁用输入
                        FrameSync.instance.isFire = false;
                    }

                    _resetPlayerActor(false);
                    InputManager.isForceLock = true;
                },
                cb,
                () =>
                {
                    if (null != finishcb)
                    {
                        finishcb.Invoke();
                    }
                    if (FrameSync.instance != null)
                    {
                        // 恢复输入
                        FrameSync.instance.isFire = true;
                    }
                    InputManager.isForceLock = false;
#if !LOGIC_SERVER
                    var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
                    if (battleUI != null && battleUI.comShowHit != null)
                    {
                        battleUI.comShowHit.ClearHitNumber();
                    }
#endif
                },
                fadein,
                fadeout
            );

        }
        private void DoSpecialGCClear(bool clearKey = false)
        {
            //对某些特殊的关卡 在过门的时候添加GC
            if (mDungeonManager.GetDungeonDataManager().table != null && mDungeonManager.GetDungeonDataManager().table.NeedForceGC)
            {
                // Logger.LogErrorFormat("did:{0} 执行GC!!!!", did);
                if (clearKey)
                    AssetGabageCollector.instance.ClearUnusedAsset(CResPreloader.instance.priorityGameObjectKeys);
                else
                    AssetGabageCollector.instance.ClearUnusedAsset();
            }
        }
        protected override void _onCreateScene(BeEvent.BeEventParam args)
        {
            _updateDungeonState(false);
        }

        protected override void _onDoorStateChange(BeEvent.BeEventParam args)
        {
            _updateDungeonState(args.m_Bool);
        }

        protected void _updateDungeonState(bool isOpen)
        {
#if !LOGIC_SERVER
            var frame = _getValidTreasureMapFrame();
            if (frame != null)
            {
                Logger.LogProcessFormat("update dungeon map state with state {0}", isOpen);

                //curSystem.SetDungeonMapActive(true);

                var item = mDungeonManager.GetDungeonDataManager().CurrentDataConnect();
                if (item == null)
                {
                    return;
                }

                if (frame.TreasureDungeonMap != null)
                {

                    frame.TreasureDungeonMap.SetStartPosition(item.GetPositionX(), item.GetPositionY());

                    if (isOpen)
                    {
                        frame.TreasureDungeonMap.SetOpenPosition(item.GetPositionX(), item.GetPositionY());
                    }
                    frame.TreasureDungeonMap.SetViewPortData(item.GetPositionX(), item.GetPositionY());
                }
            }
#endif
        }

        private bool _doorCallback(ISceneRegionInfoData info, BeRegionTarget target)
        {
#if !LOGIC_SERVER
            mStartPassDoor = (int)Time.realtimeSinceStartup;
#endif
            //宝藏关卡胜利后不需要切门了   （判断scene是防止玩家移动速度过快还没有进入结算判断时机就直接切门）
            if (mDungeonManager == null || mDungeonManager.IsFinishFight() || mDungeonManager.GetBeScene() == null || mDungeonManager.GetBeScene().state >= BeSceneState.onBulletTime) return false;
            bool ret = true;
            
          
            var beScene = mDungeonManager.GetBeScene();
            //录像反馈在切门时出现宝箱死亡，切门给宝箱加无敌
            if (beScene != null)
            {
                var bigBoxMonster = beScene.FindMonsterByID(12550031);
                if (bigBoxMonster != null)
                {
                    bigBoxMonster.buffController.TryAddBuff(29, -1);
                }
            }
            ISceneTransportDoorData door = info as ISceneTransportDoorData;
            if (null != door)
            {
                _changeAreaFade(600, 300,
                () =>
                {
             
#if !LOGIC_SERVER
                    //内存大于2G并且是高配就不GC
                    //if (!PluginManager.instance.IsLargeMemoryDevice() || !GeGraphicSetting.instance.IsHighLevel())
                    //{

                        AssetGabageCollector.instance.ClearUnusedAsset(CResPreloader.instance.priorityGameObjectKeys,true);
                    //}

                    //else
                    //{
                    //    DoSpecialGCClear(true);
                    //}
                    if (RecordServer.instance != null)
                        RecordServer.instance.FlushProcess();
#endif
                    _clearBossData();
                    var players = mDungeonPlayers.GetAllPlayers();
                    for (int i = 0; i < players.Count; ++i)
                    {
                        players[i].playerActor.TriggerEventNew(BeEventType.OnBeforePassDoor);
                    }
#if STAT_EXTRA_INFO
                    bool nextAreaVisited = mDungeonManager.GetDungeonDataManager().IsNextAreaVisited(door.GetDoortype());
#endif
                    if (mDungeonManager.GetDungeonDataManager().NextArea(door.GetDoortype()))
                    {
                        for (int i = 0; i < players.Count; ++i)
                        {
                            players[i].playerActor.TriggerEventNew(BeEventType.onStartPassDoor);
                        }

#if !LOGIC_SERVER
                        SystemNotifyManager.ClearDungeonSkillTip();
#endif

                        if (recordServer != null && recordServer.IsProcessRecord())
                        {
                            recordServer.RecordProcess("[BATTLE]_doorCallback FROM {0} TO {1} area:{2}",
                                door.GetDoortype(), door.GetNextdoortype(), mDungeonManager.GetDungeonDataManager().CurrentScenePath());
                            recordServer.MarkString(0x3798516, door.GetDoortype().ToString(), door.GetNextdoortype().ToString(), mDungeonManager.GetDungeonDataManager().CurrentScenePath());
                            // Mark:0x3798516 [BATTLE]_doorCallback FROM {0} TO {1} area:{2}
                        }
                        mDoorShiftCount++;
                        _createBase();
                        _createEntitys();
#if !LOGIC_SERVER
                        if (Global.PRELOAD_MODE != PreloadMode.ALL)
                            PreloadMonster();
#endif
                        _onSceneStart();
                        mDungeonManager.StartFight();
//#if !LOGIC_SERVER
//                        var curSystem = _getValidSystem();
//                        if (curSystem != null && curSystem.dungeonMapCom != null)
//                        {
//                            curSystem.dungeonMapCom.ResizeMap();
//                        }
//#endif


#if STAT_EXTRA_INFO
#if !LOGIC_SERVER
	                    if (!nextAreaVisited)
	                    {
	                        var passDoorDuration = (int)Time.realtimeSinceStartup - mStartPassDoor;
	                        Logger.LogErrorFormat("passDoorDuration:{0}", passDoorDuration);
	                        GameStatisticManager.GetInstance().DoStatSmallPackageInfo(GameStatisticManager.ExtraInfo.PASSDOOR_DUNGEON, passDoorDuration.ToString());
	                    }           
#endif
#endif
                    }
                    else
                    {
                        // TOOD change area error
                    }
                },
                () =>
                {

                    if (mLevelMgr != null)
                    {
                        mLevelMgr.PassedDoor();
                    }

                    var players = mDungeonPlayers.GetAllPlayers();
                    for (int i = 0; i < players.Count; ++i)
                    {
                        players[i].playerActor.ShowTransport(false);
                        players[i].playerActor.TriggerEventNew(BeEventType.onPassedDoor);

                        if (players[i].playerActor.pet != null)
                        {
                            players[i].playerActor.SetPetAlongside();
                        }

                        var beActor = players[i].playerActor;
                        if (beActor.aiManager != null && beActor.aiManager.isAutoFight)
                        {
                            beActor.aiManager.StopCurrentCommand();
                            if (beActor != null && beActor.IsProcessRecord())
                            {
                                beActor.GetRecordServer().RecordProcess("[AI]PID:{0} StopCurrentComand", beActor.GetPID());
                                beActor.GetRecordServer().MarkInt(0x3297516, beActor.GetPID());
                                // Mark:0x3297516 [AI]PID:{0} StopCurrentComand
                            }
                        }

                    }
                    _showActivityMonsterTips();

                    if(IsInEndRoom())
                    {
                        beScene.onEnterEndRoom();
                    }


                });
            }

            return ret;
        }

        private void _showActivityMonsterTips()
        {
#if !LOGIC_SERVER
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var monsters = mDungeonData.CurrentMonsters();

            for (int i = 0; i < monsters.Count; ++i)
            {
                Battle.DungeonMonster monsterUnit = monsters[i];

                if (null != monsterUnit)
                {
                    string monsterName = string.Empty;
                    MonsterID monsterID = new MonsterID
                    {
                        resID = monsterUnit.typeId,
                        monsterLevel = 0
                    };

                    ProtoTable.UnitTable unit = TableManager.instance.GetTableItem<ProtoTable.UnitTable>(monsterID.resID);

                    if (null != unit)
                    {
                        monsterName = unit.Name;
                    }

                    if (!string.IsNullOrEmpty(monsterName))
                    {
                        switch (monsterUnit.monsterType)
                        {
                            case DEntityType.ACTIVITY_BOSS_POS:
                                SystemNotifyManager.SystemNotify(1281, monsterName);
                                break;
                            case DEntityType.ACTIVITY_ELITE_POS:
                                SystemNotifyManager.SystemNotify(1280, monsterName);
                                break;
                        }
                    }
                }
            }
#endif
        }

        protected List<BattlePlayer> _doorTriggerAllPlayers()
        {
            if (null != mDungeonPlayers)
            {
                var players = mDungeonPlayers.GetAllPlayers();

                mCachBeActor.Clear();

                for (int i = 0; i < players.Count; ++i)
                {
                    mCachBeActor.Add(players[i]);
                }
            }

            return mCachBeActor;
        }

        protected sealed override void _createMonsters()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var monsters = mDungeonData.CurrentMonsters();
            var monsterCreatedCount = mBeScene.CreateMonsterList(monsters, mDungeonData.IsBossArea(), mDungeonData.GetBirthPosition(),false);
            mThisRoomMonsterCreatedCount = monsterCreatedCount;
            ///从玩家第6次移动时，魔王首次移动（移动就是根据上下左右可通的路线进行移动，更换房间）
            ///之后玩家每移动2次，魔王会移动1次！
            //优先判断玩家移动，其次判断魔王移动，有可能会出现中间隔着一个房间，但玩家向左，魔王向右，则会遭遇魔王！
            ////但玩家如果先主动移动到魔王房间，魔王则不会进行移动判断，直接进入战斗
            bool needBossMove = !mIsBossDead && !IsInBossRoom() && mDoorShiftCount != 0;
            if (needBossMove)
            {
                if (mFirstShiftDoor)
                {
                    if (mDoorShiftCount > 5)
                    {
                        mFirstShiftDoor = false;
                        needBossMove = true;
                    }
                    else
                    {
                        needBossMove = false;
                    }
                }
                else
                {
                    var mod = (mDoorShiftCount - 6) % 2;
                    if (mod == 0)
                    {
                        needBossMove = true;
                    }
                    else
                    {
                        needBossMove = false;
                    }
                }
            }
            if (needBossMove)
            {
                _MoveBoss();
            }

#if !LOGIC_SERVER
            var frame = _getValidTreasureMapFrame();
            if (frame != null && frame.TreasureDungeonMap != null)
            {
                if (mIsBossDead)
                {
                    frame.TreasureDungeonMap.RefreshBoss(-1, -1, IsNearBoss());
                }
                else
                {
                    frame.TreasureDungeonMap.RefreshBoss(mBossX, mBossY, IsNearBoss());
                }
            }
#endif

            if (IsInEndRoom() || mIsBossDead)
            {
                //无须考虑因为创建boss而清除门状态
                if (mThisRoomMonsterCreatedCount <= 0)
                    mBeScene.ClearEventAndState();
                return;
            }
           
            if (IsInBossRoom())
            {
                //创建Boss
                var boss = mBeScene.CreateMonster(mBossId);
                if (boss == null)
                {
                    Logger.LogErrorFormat("Can not create boss {0}", mBossId);
                    return;
                }
                if(mCurBossHP < 0)
                {
                    mCurBossHP = boss.attribute.GetHP();
                }
                else
                {
                    boss.attribute.SetHP(mCurBossHP);
#if !LOGIC_SERVER
                    if (boss.m_pkGeActor != null)
                    {
                        boss.m_pkGeActor.isSyncHPMP = true;
                        boss.m_pkGeActor.SyncHPBar();
                        boss.m_pkGeActor.isSyncHPMP = false;
                    }
#endif
                }
                mBossEventHandle = boss.RegisterEventNew(BeEventType.onDead, onBossDead);
                var sceneData = mDungeonData.CurrentSceneData();
                if(sceneData != null)
                {
                    if (sceneData.GetBirthPosition() != null)
                    {
                        var pos = new VInt3(sceneData.GetBirthPosition().GetPosition());
                        boss.SetPosition(pos,true);
#if !LOGIC_SERVER
                        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
                        if (battleUI != null)
                        {
                            battleUI.ShowTreasureBossWarning();
                        }
#endif
                    }
                }
            }
            else
            {
                //无须考虑因为创建boss而清除门状态
                if (mThisRoomMonsterCreatedCount <= 0)
                    mBeScene.ClearEventAndState();
            }
         
        }
        private void _clearBossData()
        {
            if (mBossEventHandle != null)
            {
                mBossEventHandle.Remove();
                mBossEventHandle = null;
            }
        }
        private void onBossDead(BeEvent.BeEventParam eventParam)
        {
            mIsBossDead = true;
            mCurBossHP = -1;
#if !LOGIC_SERVER
            var frame = _getValidTreasureMapFrame();
            if (frame != null && frame.TreasureDungeonMap != null)
            {
                frame.TreasureDungeonMap.RefreshBoss(-1, -1, IsNearBoss());
            }
#endif
            _clearBossData();
        }

        protected sealed override void _createDestructs()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var destructs = mDungeonData.CurrentDestructs();
            mBeScene.CreateDestructList2(destructs);
        }

        protected sealed override void _createRegions()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var regions = mDungeonData.CurrentRegions();
            var dynamicRegionInfoes = mDungeonData.CurrentDynamicRegionInfoes();
            var dynamicRegions = mDungeonData.CurrentDynamicRegions();
            if (null != mBeScene)
            {
                mBeScene.CreateRegions(_doorTriggerAllPlayers, regions);
                for(int i = 0; i < dynamicRegionInfoes.Count;i++)
                {
                    mBeScene.CreateDynamicRegion(_doorTriggerAllPlayers, dynamicRegionInfoes[i], dynamicRegions);
                }
            }
        }

        protected sealed override void _createPlayers()
        {
            var mBeScene = mDungeonManager.GetBeScene();
            var mDungeonData = mDungeonManager.GetDungeonDataManager();
            var players = mDungeonPlayers.GetAllPlayers();


#region get pos
            VInt3 bornPos = mDungeonData.GetBirthPosition();
            VInt3[] poses = new VInt3[5];
            poses[0] = bornPos;

            int count = 1;

            for (int i = 1; i <= players.Count - 1; ++i)
            {
                var tmp = bornPos;
                tmp.x += BeAIManager.DIR_VALUE2[i - 1, 0] * VInt.one.i;
                tmp.y += BeAIManager.DIR_VALUE2[i - 1, 1] * VInt.one.i;

                if (!mBeScene.IsInBlockPlayer(tmp))
                {
                    poses[count++] = tmp;
                }
            }

            for (int i = count; i <= players.Count - 1; ++i)
                poses[i] = bornPos;
            #endregion
            int playerLevel = 0;
            for (int i = 0; i < players.Count; ++i)
            {
                var battlePlayer = players[i];



                if (battlePlayer.playerActor == null)
                {
                    var racePlayer = battlePlayer.playerInfo;

                    var petData = BattlePlayer.GetPetData(racePlayer, false);

                    bool isLocalActor = racePlayer.accid == ClientApplication.playerinfo.accid;
                    bool isShowFashionWeapon = racePlayer.avatar.isShoWeapon == 1 ? true : false;
                    bool isAIRobot = racePlayer.robotAIType > 0 ? true : false;

                    var actor = mBeScene.CreateCharacter(
                        isLocalActor,
                        racePlayer.occupation,
                        racePlayer.level,
                        (int)ProtoTable.UnitTable.eCamp.C_HERO,
                        BattlePlayer.GetSkillInfo(racePlayer),
                        BattlePlayer.GetEquips(racePlayer, false),
                        BattlePlayer.GetBuffList(racePlayer),
                        racePlayer.seat,
                        racePlayer.name,
                        BattlePlayer.GetWeaponStrengthenLevel(racePlayer),
                        BattlePlayer.GetRankBuff(racePlayer),
                        petData,
                        BattlePlayer.GetSideEquips(racePlayer, false),
                        BattlePlayer.GetAvatar(racePlayer),
                        isShowFashionWeapon,
                        isAIRobot
                    );
                    playerLevel += racePlayer.level;
                    actor.SetScale(VInt.Float2VIntValue(Global.Settings.charScale));
                    if (actor.GetEntityData() != null)
                        actor.GetEntityData().SetCrystalNum(BattlePlayer.GetCrsytalNum(racePlayer));
                    battlePlayer.playerActor = actor;

                    actor.skillController.skillSlotMap = BattlePlayer.GetSkillSlotMap(racePlayer);
                    actor.SetPosition(mDungeonData.GetBirthPosition(), true);
                    actor.isMainActor = true;
                    actor.UseProtect();
                    actor.m_iRemoveTime = Int32.MaxValue;

                    if (null != actor.m_pkGeActor)
                    {
                        actor.m_pkGeActor.AddSimpleShadow(Vector3.one);
                    }

#if !LOGIC_SERVER
                    actor.m_pkGeActor.AddTittleComponent(BattlePlayer.GetTitleID(racePlayer), racePlayer.name, 0, "", racePlayer.level, (int)racePlayer.seasonLevel, PlayerInfoColor.LEVEL_PLAYER);
                    if (racePlayer.accid == ClientApplication.playerinfo.accid)
                        actor.m_pkGeActor.CreateInfoBar(racePlayer.name, PlayerInfoColor.TOWN_PLAYER, racePlayer.level);
                    else
                        actor.m_pkGeActor.CreateInfoBar(racePlayer.name, PlayerInfoColor.LEVEL_PLAYER, racePlayer.level);
#endif


                    if (racePlayer.accid == ClientApplication.playerinfo.accid)
                    {
#if !LOGIC_SERVER
                        mDungeonManager.GetGeScene().AttachCameraTo(actor.m_pkGeActor);
#endif


                        //初始化伤害统计数据
                        actor.skillDamageManager.InitData(mBeScene);

                        actor.isLocalActor = true;
                        actor.UseActorData();
                        actor.m_pkGeActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_guo");
                    }
                    else
                    {

#if !LOGIC_SERVER
                        //画质设置：低配不显示友军特效
                        if (GeGraphicSetting.instance.IsLowLevel())
                        {
                            var effectManager = actor.m_pkGeActor.GetEffectManager();
                            if (effectManager != null)
                            {
                                effectManager.useCube = true;
                            }
                        }
#endif
                    }

                    // TODO battle Player Dead
                    actor.RegisterEventNew(BeEventType.onAfterDead, arsg =>
                    {
                        if (battlePlayer.state != BattlePlayer.EState.Dead)
                        {
                            _onPlayerDead(battlePlayer);
                        }


#if !LOGIC_SERVER
                        GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.PLAYER_DEAD,
                                mDungeonManager.GetDungeonDataManager().id.dungeonID,
                                mDungeonManager.GetDungeonDataManager().CurrentAreaID(),
                                string.Format("{0}, {1}", battlePlayer.playerInfo.roleId, battlePlayer.statistics.data.deadCount));
#endif

                    });
                    actor.RegisterEventNew(BeEventType.onAddBuff, args =>
                    {
                        //进入钥匙关卡后增加buff 对buffid进行判别是否拥有钥匙
                        ///570206
#if !LOGIC_SERVER
                        var buff = args.m_Obj as BeBuff;
                        var frame = _getValidTreasureMapFrame();
                        if (buff != null && buff.buffID == 570206)
                        {
                            ///这里处理打开全地图消息
                            
                            if (frame != null && frame.TreasureDungeonMap != null)
                            {
                                frame.TreasureDungeonMap.OpenAllRoom();
                            }
                        }
                        if(buff != null && frame != null && frame.TreasureMapBuff != null)
                        {
                            frame.TreasureMapBuff.HideLock(buff.buffID);
                        }
                        if (buff != null && frame != null && frame.TreasureDungeonMap != null)
                        {
                            if(buff.buffID == 570205)
                            {
                                frame.TreasureDungeonMap.ClearCurrentRoomSpecialTag(TreasureMapGenerator.ROOM_TYPE.KEY_ROOM);
                            }
                            if (buff.buffID == 570206)
                            {
                                frame.TreasureDungeonMap.ClearCurrentRoomSpecialTag(TreasureMapGenerator.ROOM_TYPE.MAP_ROOM);
                            }
                        }
#endif
                    });

                    mBeScene.RegisterEventNew(BeEventSceneType.onMonsterDead, args => 
                    {
#if !LOGIC_SERVER
                        //if(args.Length > 0)
                        {
                            var tempMonster = args.m_Obj as BeActor;
                            if(tempMonster != null)
                            {
                                if (tempMonster.GetEntityData().MonsterIDEqual(12540011))
                                {
                                    var frame = _getValidTreasureMapFrame();
                                    if(frame != null && frame.TreasureDungeonMap != null)
                                    {
                                        frame.TreasureDungeonMap.ClearCurrentRoomSpecialTag(TreasureMapGenerator.ROOM_TYPE.DROPITEM_ROOM);
                                    }
                                }
                            }
                        }
#endif
                    });

                    actor.RegisterEventNew(BeEventType.onDeadProtectEnd, args =>
                    {
#if !LOGIC_SERVER
                        RecordServer.instance.PushReconnectCmd(string.Format("onDeadProtectEnd {0}", battlePlayer.playerActor.GetPID()));
#endif
                        _CheckFightEnd();
                    });

                    actor.RegisterEventNew(BeEventType.onDead, eventParam =>
                    {
                        if (battlePlayer.state != BattlePlayer.EState.Dead)
                        {
#if !LOGIC_SERVER

                            RecordServer.instance.PushReconnectCmd(string.Format("BeEventType.onDead {0}", battlePlayer.playerActor.GetPID()));
#endif
                            bool isAllPlayerDead = true;
                            int loopIndex = 0;
                            for (loopIndex = 0; loopIndex < players.Count; loopIndex++)
                            {
                                var player = players[loopIndex];
                                if (!player.playerActor.IsDead())
                                {
                                    isAllPlayerDead = false;
                                    break;
                                }
                            }

                            if (isAllPlayerDead)
                            {
                                for (loopIndex = 0; loopIndex < players.Count; loopIndex++)
                                {
                                    var player = players[loopIndex];
                                    player.playerActor.StartDeadProtect();
                                }
                            }
                        }
                    });

                    actor.RegisterEventNew(BeEventType.onHit, args => {
                    //actor.RegisterEvent(BeEventType.onHit, args => {
                    if (args != null)
                    {
                        var e = args.m_Obj as BeEntity;
                        if (e != null && e.m_iCamp != battlePlayer.playerActor.m_iCamp)
                        {
                            _onPlayerHit(battlePlayer);
                        }
                    }
                    else
                    {
                        _onPlayerHit(battlePlayer);
                    }
                    });

                    //actor.RegisterEvent(BeEventType.onHitOther, args => {

                    //});
                    actor.RegisterEventNew(BeEventType.onHitOther, _OnHitOther);

                    actor.RegisterEventNew(BeEventType.onReborn, args => {
                        _onPlayerReborn(battlePlayer);


#if !LOGIC_SERVER
                        GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.PLAYER_REBORN,
                                mDungeonManager.GetDungeonDataManager().id.dungeonID,
                                mDungeonManager.GetDungeonDataManager().CurrentAreaID(),
                                string.Format("{0}, {1}", battlePlayer.playerInfo.roleId, battlePlayer.statistics.data.rebornCount));
#endif

                    });

                    if (petData != null)
                        actor.SetPetData(petData);
                    actor.CreateFollowMonster();

                    InitAutoFight(actor);
                    actor.SetForceRunMode(false);
#if DEBUG_SETTING
                    if (Global.Settings.isDebug)
                    {
                        if (Global.Settings.playerHP > 0)
                        {
                            actor.GetEntityData().SetHP(Global.Settings.playerHP);
                            actor.GetEntityData().SetMaxHP(Global.Settings.playerHP);
                            actor.m_pkGeActor.ResetHPBar();
                        }
                    }
#endif
                }
                else
                {
                    // set transport birth position

                    battlePlayer.playerActor.ResetMoveCmd();
                    if (battlePlayer.playerActor.actionManager != null)
                        battlePlayer.playerActor.actionManager.StopAll();

                }

                battlePlayer.playerActor.SetPosition(poses[i], true);

                mBeScene.InitFriendActor(mDungeonData.GetBirthPosition());
            }
            if (playerLevel > 0 && players.Count > 0)
            {
                var monsterId = new MonsterIDData(12530031);
                mBossId = monsterId.GenMonsterID(playerLevel / players.Count);
            }
        }

        private void _OnHitOther(BeEvent.BeEventParam param)
        {
                var skillID = param.m_Int2;
                int id = param.m_Int3;
                _onPlayerHitOther(skillID, id);
        }

        protected void InitAutoFight(BeActor actor)
        {
            var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(actor.attribute.professtion);
            if (jobData != null)
            {
                actor.InitAutoFight(jobData.AIConfig1, jobData.AIConfig2, jobData.AIConfig3);
            }
        }

        protected virtual void _onPlayerHit(BattlePlayer player)
        {
            player.statistics.OnHit();
        }

        public Action<int, int> playerHitCallBack;
        protected virtual void _onPlayerHitOther(int skillID, int id)
        {
            if (playerHitCallBack != null)
                playerHitCallBack(skillID, id);
        }

        protected virtual void _onPlayerReborn(BattlePlayer player)
        {
            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]mid:{0} player reborn", player.playerActor.m_iID);
                recordServer.MarkInt(0x3197516, player.playerActor.m_iID);
                // Mark:0x3197516 [BATTLE]mid:{0} player reborn
            }
            var players = mDungeonPlayers.GetAllPlayers();
            for (int loopIndex = 0; loopIndex < players.Count; loopIndex++)
            {
                var curPlayer = players[loopIndex];
                curPlayer.playerActor.EndDeadProtect();
            }

#if !LOGIC_SERVER
            byte seat = player.playerInfo.seat;
            byte mainPlayerSeat = mDungeonPlayers.GetMainPlayer().playerInfo.seat;


            if (BattleMain.IsModeMultiplayer(GetMode()) && seat == mainPlayerSeat)
            {
                mDungeonManager.GetGeScene().AttachCameraTo(player.playerActor.m_pkGeActor);
            }
#endif
            player.state = BattlePlayer.EState.Normal;
            player.statistics.Reborn();


#if !LOGIC_SERVER

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRebornSuccess, player.playerInfo.seat);

            if (mainPlayerSeat == seat)
            {
                if (ClientSystemManager.instance.IsFrameOpen<DungeonRebornFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                }
            }
#endif
            _CheckFightEnd();
        }

        protected void _CheckFightEnd()
        {
            if (mDungeonManager == null || mDungeonPlayers == null)
            {
                return;
            }
            if (mDungeonManager.IsFinishFight()) return;

            List<BattlePlayer> players = mDungeonPlayers.GetAllPlayers();
            bool isAllPlayerDead = true;
#if !LOGIC_SERVER
            byte mainPlayerSeat = mDungeonPlayers.GetMainPlayer().playerInfo.seat;
#endif
            bool isAllEnemyDead = mDungeonManager.GetBeScene().isAllEnemyDead();
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                if (!player.playerActor.IsDead() ||
                    player.playerActor.IsInDeadProtect)
                {
                    isAllPlayerDead = false;
                    break;
                }
            }
#if !LOGIC_SERVER
            RecordServer.instance.PushReconnectCmd(string.Format("CheckFightEnd isAllEnemyDead : {0} isAllPlayerDead: {1} IsBossArea :{2}", isAllEnemyDead, isAllPlayerDead, mDungeonManager.GetDungeonDataManager().IsBossArea()));
#endif
            if (isAllEnemyDead && mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
#if !LOGIC_SERVER
                _sendDungeonRaceEndReq();
#else
                 var req = _getDungeonRaceEndTeamReq();
                LogicServer.ReportRaceEndToLogicServer(req);
#endif

            }
            else if (isAllPlayerDead)
            {
#if !LOGIC_SERVER
                _sendDungeonRaceEndReq(true);
#else
                var req = _getDungeonRaceEndTeamReq();
                LogicServer.ReportRaceEndToLogicServer(req);
#endif
            }
            if (isAllEnemyDead && mDungeonManager.GetDungeonDataManager().IsBossArea() || isAllPlayerDead)
            {
                mDungeonManager.FinishFight();
            }

        }
        protected void _sendDungeonRaceEndReq(bool dead = false)
        {
#if !LOGIC_SERVER
            mIsNormalFinsh = !dead;
            if (_isNeedSendNet())
            {
                if (ClientSystemManager.instance.IsFrameOpen<DungeonRebornFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                }

                if (eDungeonMode.SyncFrame == GetMode())
                {
                    //
                    //如果取消复活，并且当前没有活着的人就结束战斗
                    if (!dead)
                        mDungeonRaceEndReqCoroutine = GameFrameWork.instance.StartCoroutine(_sendDungeonTeamRaceEndReqIter());
                    else
                    {
                        if (mDungeonPlayers.IsAllPlayerDead())
                        {
                            GameClient.ClientReconnectManager.instance.canReconnectRelay = false;
                            mDungeonRaceEndReqCoroutine = GameFrameWork.instance.StartCoroutine(_sendDungeonTeamRaceEndReqIter());
                        }
                        else
                        {
                            //
                            //ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                        }
                    }
                }
                else
                {
                    GameFrameWork.instance.StartCoroutine(_sendDungeonRaceEndReqIter());
                }
            }
            else
            {
                SceneDungeonRaceEndRes res = new SceneDungeonRaceEndRes
                {
                    result = 0
                };
                _onSceneDungeonRaceEndRes(res);
            }

            ClearBgm();

            _playDungeonFinish();
#endif
        }
        protected override void _onAreaClear(BeEvent.BeEventParam args)
        {
#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
            if (battleUI != null)
            {
                battleUI.CloseLevelTip();
            }
#endif

            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[SCENE]_onAreaClear");
                recordServer.Mark(0xb8522742);
                // Mark:0xb8522742 [SCENE]_onAreaClear
            }

            if (mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
                _CheckFightEnd();
            }
            else
            {
#if !LOGIC_SERVER
                if (mThisRoomMonsterCreatedCount > 0)
                {
                    SystemNotifyManager.SystemNotify(6000);
                    PlaySound(5);
                }
                var index = mDungeonManager.GetDungeonDataManager().CurrentIndex();
                _updateDungeonState(true);
#endif
            }
        }
        protected virtual void _onPlayerDead(BattlePlayer player)
        {
            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]mid:{0} player dead", player.playerActor.m_iID);
                recordServer.MarkInt(0x3297576, player.playerActor.m_iID);
                // Mark:0x3297576 [BATTLE]mid:{0} player dead
            }

            _playDungeonDead();

            player.state = BattlePlayer.EState.Dead;
            player.statistics.Dead();
            byte seat = player.playerInfo.seat;
#if !LOGIC_SERVER
            byte mainPlayerSeat = mDungeonPlayers.GetMainPlayer().playerInfo.seat;
            if (seat == mainPlayerSeat)
            {
                if (BattleMain.IsModeMultiplayer(GetMode()))
                {
                    BattlePlayer alivePlayer = mDungeonPlayers.GetFirstAlivePlayer();
                    if (null != alivePlayer)
                    {
                        mDungeonManager.GetGeScene().AttachCameraTo(alivePlayer.playerActor.m_pkGeActor);
                    }
                }
                else
                {
                }
            }

            if (mDungeonPlayers.IsAllPlayerDead() || seat == mainPlayerSeat)
            {
                _startPlayerDeadProcess(player);
            }
#endif


            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattlePlayerDead);
        }

        private UnityEngine.Coroutine mDeadProcess = null;

        private void _stopPlayerDeadProcess()
        {
#if !LOGIC_SERVER
            if (null != mDeadProcess)
            {
                GameFrameWork.instance.StopCoroutine(mDeadProcess);
                mDeadProcess = null;
            }
#endif
        }
        private void _startPlayerDeadProcess(BattlePlayer player)
        {
#if !LOGIC_SERVER
            _stopPlayerDeadProcess();

            if (!mDungeonManager.IsFinishFight())
            {
                mDeadProcess = GameFrameWork.instance.StartCoroutine(_playerDeadProcess(player));
            }
#endif
        }

        protected override void _onPlayerLeave(BattlePlayer player)
        {
            if (player != null)
            {
                player.netState = BattlePlayer.eNetState.Offline;

                if (recordServer != null && recordServer.IsProcessRecord())
                {
                    recordServer.RecordProcess("[BATTLE]PID:{0} playerName:{1} _onPlayerLeave", player.playerActor.m_iID, player.playerInfo.name);
                    recordServer.Mark(0x3197216, new[] { player.playerActor.m_iID }, player.playerInfo.name);
                    // Mark:0x3197216 [BATTLE]PID:{0} playerName:{1} _onPlayerLeave
                }
            }
            if (mDungeonPlayers != null && mDungeonPlayers.IsAllPlayerDead())
            {
                _startPlayerDeadProcess(player);
            }
        }

        protected sealed override void _onPlayerBack(BattlePlayer player)
        {
            player.netState = BattlePlayer.eNetState.Online;

            //player.playerActor.ReturnFromBattle();
            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]PID:{0} playerName:{1} _onPlayerBack", player.playerActor.m_iID, player.playerInfo.name);
                recordServer.Mark(0x3197016, new[] { player.playerActor.m_iID }, player.playerInfo.name);
                // Mark:0x3197016 [BATTLE]PID:{0} playerName:{1} _onPlayerBack
            }
        }

        protected virtual void _onPlayerCancelReborn(BattlePlayer player)
        {
            if (player != null)
            {
                player.playerActor.EndDeadProtect();
            }
#if !LOGIC_SERVER
            RecordServer.instance.PushReconnectCmd(string.Format("_onPlayerCancelReborn {0}", player.playerActor.GetPID()));
#endif
            _CheckFightEnd();
        }

        private IEnumerator _playerDeadProcess(BattlePlayer player)
        {
#if !LOGIC_SERVER
            if (player != null && player.IsLocalPlayer())
            {
                if (ClientSystemManager.instance.IsFrameOpen<DungeonRebornFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                }

                ClientSystemManager.instance.OpenFrame<DungeonRebornFrame>(FrameLayer.Middle);
            }

            yield return Yielders.EndOfFrame;
#endif

            while (DungeonRebornFrame.sState == DungeonRebornFrame.eState.None)
            {
                yield return Yielders.EndOfFrame;
            }

            if (DungeonRebornFrame.sState == DungeonRebornFrame.eState.Cancel)
            {
                _onPlayerCancelReborn(player);
#if !LOGIC_SERVER
                if (ClientSystemManager.instance.IsFrameOpen<DungeonRebornFrame>())
                {
                    ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
                }
#endif
            }
        }
        protected sealed override void _onPostStart()
        {
#if !LOGIC_SERVER
            if (GetMode() != eDungeonMode.Test)
            {

                if (ItemDataManager.GetInstance().IsPackageFull())
                {
                    SystemNotifyManager.SystemNotify(1033);
                }
            }
            if (mDungeonManager == null
                || mDungeonManager.GetDungeonDataManager() == null
                || mDungeonManager.GetBeScene() == null
                || mDungeonManager.GetBeScene().sceneData == null) return;
            string diff = ChapterUtility.GetHardString(3);
            SystemNotifyManager.SystemNotify(6001,
                mDungeonManager.GetDungeonDataManager().table.Name,
                diff
            );

            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
            if (battleUI != null)
            {
                battleUI.ShowLevelTip(mDungeonManager.GetBeScene().sceneData.GetTipsID());
            }
            mStarted = true;
#endif

            var mBeScene = mDungeonManager.GetBeScene();
            if (mBeScene != null)
            {
                mBeScene.RegisterEventNew(BeEventSceneType.onBossDead, (args) => {

                    var players = mDungeonPlayers.GetAllPlayers();
                    for (int i = 0; i < players.Count; ++i)
                    {
                        var actor = players[i].playerActor;
                        if (actor != null && !actor.IsDead())
                            actor.SetAutoFight(false);
                        if (actor != null && actor.IsDead() && actor.GetRecordServer().IsProcessRecord())
                        {
                            actor.GetRecordServer().RecordProcess("this actor is dead");
                            actor.GetRecordServer().Mark(0xb7522742);
                            // Mark:0xb7522742 this actor is dead
                        }
                    }
                });
            }
        }

        //单人关卡结算
        private IEnumerator _sendDungeonRaceEndReqIter(bool modifyScore = false, Protocol.DungeonScore score = Protocol.DungeonScore.C)
        {
            yield return _fireRaceEndOnLocalFrameIter();
            yield return _sendDungeonReportDataIter();

            var msgEvent = new MessageEvents();
            var res = new SceneDungeonRaceEndRes();
            var req = _getDungeonRaceEndReq();

            if (modifyScore)
            {
                req.score = (byte)score;
                req.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", req.score, req.beHitCount, req.usedTime));
            }

            //Logger.LogErrorFormat("[单人关卡] 发送结算");
            if (mDungeonManager.GetDungeonDataManager() != null)
            {
                mDungeonManager.GetDungeonDataManager().LockBattleEnd();
            }

            yield return _sendMsgWithResend<SceneDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.GATE_SERVER, msgEvent, req, res, true, 3, 3, 33);

            //收到结算
            if (msgEvent.IsAllMessageReceived())
            {
                //Logger.LogErrorFormat("[单人关卡] 收到结算");
                _onSceneDungeonRaceEndRes(res);
            }
            else
            {
                ClientSystemManager.instance.QuitToLoginSystem(9991);
                //Logger.LogErrorFormat("[单人关卡] 没有收到结算的消息");
            }
        }

        protected virtual SceneDungeonRaceEndReq _getDungeonRaceEndReq()
        {
            var mainPlayer = mDungeonPlayers.GetMainPlayer();
            var msg = new SceneDungeonRaceEndReq
            {
                beHitCount = (ushort)mDungeonStatistics.HitCount(mainPlayer.playerInfo.seat),
                usedTime = (uint)mDungeonStatistics.AllFightTime(true),
                score = (byte)mDungeonStatistics.FinalDungeonScore(),
                maxDamage = mDungeonStatistics.GetAllMaxHurtDamage(),
                skillId = mDungeonStatistics.GetAllMaxHurtSkillID(),
                param = mDungeonStatistics.GetAllMaxHurtID(),
                totalDamage = mDungeonStatistics.GetAllHurtDamage(),
                lastFrame = mDungeonManager.GetDungeonDataManager().GetFinalFrameDataIndex()
            };
            msg.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", msg.score, msg.beHitCount, msg.usedTime));
#if !LOGIC_SERVER
            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]Race End _getDungeonRaceEndReq lastFrame {0}", msg.lastFrame);
                recordServer.MarkInt(0x3197200, (int)msg.lastFrame);
                // Mark:0x3197200 [BATTLE]Race End _getDungeonRaceEndReq lastFrame {0}
            }
#endif
            return msg;
        }

        private IEnumerator _sendDungeonTeamRaceEndReqIter(bool modifyScore = false, Protocol.DungeonScore score = Protocol.DungeonScore.C)
        {
#if !LOGIC_SERVER
            var msgEvent = new MessageEvents();
            var res = new SceneDungeonRaceEndRes();
            var req = _getDungeonRaceEndTeamReq();

            if (modifyScore)
            {
                for (int i = 0; i < req.raceEndInfo.infoes.Length; ++i)
                {
                    DungeonPlayerRaceEndInfo info = req.raceEndInfo.infoes[i];

                    info.score = (byte)score;
                    info.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", info.score, info.beHitCount, req.raceEndInfo.usedTime));
                }
            }

            BattleMain.instance.WaitForResult();

            if (SwitchFunctionUtility.IsOpen(51))
            {
                yield return MessageUtility.Wait<RelaySvrDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.RELAY_SERVER, msgEvent, req, res, false, 60.0f);

                if (msgEvent.IsAllMessageReceived())
                {
                    _onSceneDungeonRaceEndRes(res);
                }
                else
                {
                    ClientSystemManager.instance.QuitToLoginSystem(9991);
                    yield break;
                }
            }
            else
            {
                yield return MessageUtility.Wait<RelaySvrDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.RELAY_SERVER, msgEvent, req, res, false);

                if (msgEvent.IsAllMessageReceived())
                {
                    _onSceneDungeonRaceEndRes(res);
                }
            }
#else
            yield break;
#endif
        }

        private RelaySvrDungeonRaceEndReq _getDungeonRaceEndTeamReq()
        {

            RelaySvrDungeonRaceEndReq msg = new RelaySvrDungeonRaceEndReq();
            //msg.roleId = PlayerBaseData.GetInstance().RoleID;
#if !LOGIC_SERVER
            msg.raceEndInfo.sessionId = ClientApplication.playerinfo.session;
#else
            msg.raceEndInfo.sessionId = this.recordServer != null ? Convert.ToUInt64(recordServer.sessionID) : 0UL;
#endif
            msg.raceEndInfo.dungeonId = (uint)mDungeonManager.GetDungeonDataManager().id.dungeonID;
            msg.raceEndInfo.usedTime = (uint)mDungeonStatistics.AllFightTime(true);

            var m_battlePlayers = mDungeonPlayers.GetAllPlayers();

            msg.raceEndInfo.infoes = new DungeonPlayerRaceEndInfo[m_battlePlayers.Count];

            for (int i = 0; i < m_battlePlayers.Count; ++i)
            {
                RacePlayerInfo source = m_battlePlayers[i].playerInfo;
                DungeonPlayerRaceEndInfo target = new DungeonPlayerRaceEndInfo
                {
                    roleId = source.roleId,
                    pos = source.seat,
                    score = (byte)mDungeonStatistics.FinalDungeonScore(),
                    beHitCount = (ushort)mDungeonStatistics.HitCount(source.seat)
                };
                target.md5 = DungeonUtility.GetBattleEncryptMD5(string.Format("{0}{1}{2}", target.score, target.beHitCount, msg.raceEndInfo.usedTime));
                msg.raceEndInfo.infoes[i] = target;
            }

            return msg;
        }


        protected IEnumerator _sendDungeonReportDataIter()
        {
            if (GetMode() == eDungeonMode.LocalFrame)
            {
                if (mDungeonManager == null)
                    yield break;
                if (mDungeonManager.GetDungeonDataManager() == null)
                    yield break;

                mDungeonManager.GetDungeonDataManager().PushFinalFrameData();

                mDungeonManager.GetDungeonDataManager().SendWorldDungeonReportFrame();

                yield return null;

                while (!mDungeonManager.GetDungeonDataManager().IsAllReportDataServerRecived())
                {
                    yield return null;
                }

                mDungeonManager.GetDungeonDataManager().UnlockUpdateCheck();
            }
        }

        protected virtual void _onSceneDungeonRaceEndRes(SceneDungeonRaceEndRes res)
        {
#if !LOGIC_SERVER
            ClientReconnectManager.instance.canReconnectRelay = false;
            RecordServer.instance.PushReconnectCmd(string.Format("_onSceneDungeonRaceEndRes : {0} Begin-----------", res.result));
            Logger.LogProcessFormat("[PVE战斗] 收到结算消息 {0}", ObjectDumper.Dump(res));

            if (recordServer != null && recordServer.IsProcessRecord())
            {
                recordServer.RecordProcess("[BATTLE]Race End");
                recordServer.Mark(0xb7122742);
                // Mark:0xb7122742 [BATTLE]Race End
            }

            if (res.result == 0)
            {

                BattleDataManager.GetInstance().ConvertDungeonBattleEndInfo(res);
                var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPauseBtn>();
                if (battleUI != null)
                {
                    battleUI.HidePauseButton();
                }
                GameFrameWork.instance.StartCoroutine(_finishProcess(res));

                // TODO 这里的逻辑移出去
                if ((byte)Protocol.DungeonScore.C == res.score)
                {
                    Logger.LogProcessFormat("[战斗] 失败");
                    DevelopGuidanceManager.GetInstance().SignalGuidanceEntrace();
                }
            }
            else
            {
                Logger.LogErrorCode(res.result);
            }
            RecordServer.instance.PushReconnectCmd(string.Format("_onSceneDungeonRaceEndRes : {0} End-----------", res.result));
#endif
        }

        private IEnumerator _finishProcess(SceneDungeonRaceEndRes res)
        {
#if !LOGIC_SERVER
            RecordServer.instance.PushReconnectCmd(string.Format("_finishProcess : {0} Begin-----------", res.result));
            if (0 != res.hasRaceEndDrop)
            {
                yield return _requestRaceEndDrops(res.raceEndDropBaseMulti);
            }

            if (null == BattleMain.instance)
            {
                yield break;
            }

            if (mDungeonManager != null && mDungeonManager.GetDungeonDataManager() != null &&
                mDungeonManager.GetDungeonDataManager().table != null)
            {
                Vec3 pos = mDungeonManager.GetBeScene().GeGDeadBossPosition();
                pos.x += 2.4f; 
                mDungeonManager.GetBeScene().DropItems(mDungeonManager.GetDungeonDataManager().GetRaceEndDrops(), new VInt3(pos), false);
                yield return new WaitForSeconds(1.5f);
            }

            RecordServer.instance.PushReconnectCmd("_openDungeonFinishFrame");
            if (null == BattleMain.instance)
            {
                yield break;
            }
            _openDungeonFinishFrame(res);

            yield return (_waitForFrameClose(typeof(DungeonFinishFrame)));

            if (null == BattleMain.instance)
            {
                yield break;
            }

            if (mDungeonManager == null || mDungeonManager.GetDungeonDataManager() == null) yield break;

            var tableData = dungeonManager.GetDungeonDataManager().table;
            int id = dungeonManager.GetDungeonDataManager().id.dungeonID;
            if (tableData != null && mIsNormalFinsh &&
                ChapterUtility.PreconditionIDList(id).Count != 0 &&
                mIsChapterNoPassed && tableData.SubType != DungeonTable.eSubType.S_WEEK_HELL)
            {
                ClientSystemManager.instance.OpenFrame<BossMissionCompletePromptFrame>(FrameLayer.Middle);

                yield return (_waitForFrameClose(typeof(BossMissionCompletePromptFrame)));
            }
            if (null == BattleMain.instance)
            {
                yield break;
            }

            RecordServer.instance.PushReconnectCmd("openDungeonMenuFrame");
            ClientSystemManager.instance.OpenFrame<DungeonMenuFrame>();

            yield return Yielders.EndOfFrame;
            RecordServer.instance.PushReconnectCmd(string.Format("_finishProcess : {0} End-----------", res.result));
#else
            yield break;
#endif

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRewardFinish);
        }
        private void _openDungeonFinishFrame(SceneDungeonRaceEndRes res)
        {
#if !LOGIC_SERVER
            //存储战斗结果
            PveBattleResult = (DungeonScore)res.score == DungeonScore.C ? BattleResult.Fail : BattleResult.Success;
            var frame = GameClient.ClientSystemManager.instance.OpenFrame<GameClient.DungeonFinishFrame>(GameClient.FrameLayer.Middle) as GameClient.DungeonFinishFrame;
            frame.SetData(res);

            frame.SetDrops(_getAllRewardItems(res).ToArray());
#endif
        }
        protected List<ComItemList.Items> _getAllRewardItems(SceneDungeonRaceEndRes res)
        {
            List<ComItemList.Items> list = _getTeamRewardItem(res);
            List<ComItemList.Items> raceend = _getRaceEndDropItem();
            List<ComItemList.Items> droplist = _getPickedDropItems();
            list.AddRange(raceend);
            list.AddRange(droplist);
            return list;
        }

        /// <summary>
        /// 已经获得的道具列表
        /// </summary>
        /// <returns></returns>
        protected List<ComItemList.Items> _getPickedDropItems()
        {
            List<Battle.DungeonDropItem> allGots = BattleDataManager.GetInstance().BattleInfo.dropCacheItemIds;
            List<ComItemList.Items> list = new List<ComItemList.Items>();

            for (int i = 0; i < allGots.Count; ++i)
            {
                ComItemList.Items item = list.Find(x =>
                {
                    return x.id == (int)allGots[i].typeId;
                });

                if (null != item)
                {
                    item.count += (uint)allGots[i].num;
                }
                else
                {
                    list.Add(new ComItemList.Items()
                    {
                        id = allGots[i].typeId,
                        count = (uint)allGots[i].num,
                        type = ComItemList.eItemType.Custom,
                        strenthLevel = allGots[i].strenthLevel,
                        equipType = allGots[i].equipType
                    });
                }
            }

            return list;
        }

        protected List<ComItemList.Items> _getRaceEndDropItem()
        {

            List<ComItemList.Items> list = new List<ComItemList.Items>();
            if (mDungeonManager == null || mDungeonManager.GetDungeonDataManager() == null)
            {
                return list;
            }
            List<Battle.DungeonDropItem> drops = mDungeonManager.GetDungeonDataManager().GetRaceEndDrops();
            for (int i = 0; i < drops.Count; ++i)
            {
                ComItemList.Items item = new ComItemList.Items
                {
                    count = (uint)drops[i].num,
                    id = (int)drops[i].typeId,
                    type = ComItemList.eItemType.Custom,
                    strenthLevel = drops[i].strenthLevel,
                    equipType = drops[i].equipType
                };

                list.Add(item);
            }

            return list;
        }

        protected List<ComItemList.Items> _getTeamRewardItem(SceneDungeonRaceEndRes res)
        {
            List<ComItemList.Items> list = new List<ComItemList.Items>();

            if (null != res.teamReward && res.teamReward.id > 0)
            {
                ComItemList.Items item = new ComItemList.Items
                {
                    count = res.teamReward.num,
                    id = (int)res.teamReward.id,
                    type = ComItemList.eItemType.Custom,
                    flag = ComItemList.eItemExtraFlag.ExtraReward,
                    strenthLevel = res.teamReward.strength,
                    equipType = (EEquipType)res.teamReward.equipType
                };
                list.Insert(0, item);
            }
            if (null != res.veteranReturnReward && res.veteranReturnReward.id > 0)
            {
                ComItemList.Items item = new ComItemList.Items
                {
                    count = res.veteranReturnReward.num,
                    id = (int)res.veteranReturnReward.id,
                    type = ComItemList.eItemType.Custom,
                    flag = ComItemList.eItemExtraFlag.ExtraReward,
                    strenthLevel = res.veteranReturnReward.strength,
                    equipType = (EEquipType)res.veteranReturnReward.equipType
                };
                list.Insert(0, item);
            }
            return list;
        }
        private IEnumerator _waitForFrameClose(Type type)
        {
#if !LOGIC_SERVER
            yield return Yielders.EndOfFrame;

            while (ClientSystemManager.instance.IsFrameOpen(type))
            {
                yield return Yielders.EndOfFrame;
            }
#else
            yield break;
#endif
        }

        protected IEnumerator _requestRaceEndDrops(int multi)
        {
#if !LOGIC_SERVER
            if (!_isNeedSendNet())
            {
                yield break;
            }

            var msg = new MessageEvents();
            var req = new SceneDungeonEndDropReq();
            var res = new SceneDungeonEndDropRes();

            req.multi = (byte)multi;

            yield return (_sendMsgWithResend<SceneDungeonEndDropReq, SceneDungeonEndDropRes>(ServerType.GATE_SERVER, msg, req, res, true, 10));

            if (msg.IsAllMessageReceived())
            {
            }
#else
            yield break;
#endif
        }
    }
}