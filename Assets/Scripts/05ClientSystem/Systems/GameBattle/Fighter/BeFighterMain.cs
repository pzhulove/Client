using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Protocol;
using ProtoTable;
using Network;
namespace GameClient
{
    public class BeFighterMain:BeFighter
    {
        public enum EMoveState
        {
            Invalid = -1,
            Idle = 0,
            Moveing,
            AutoMoving,
        }
        public class MoveStateChangedEvent : UnityEvent<EMoveState, EMoveState> { }
        public class AutoMoveSuccessEvent : UnityEvent { }
        public class AutoMoveFailEvent : UnityEvent { }
        public class MoveingEvent : UnityEvent<Vector3> { }
        public class AutoMoveingEvent : UnityEvent<Vector3> { }
        static public MoveStateChangedEvent OnMoveStateChanged { get; set; }
        static public AutoMoveSuccessEvent OnAutoMoveSuccess { get; set; }
        static public AutoMoveFailEvent OnAutoMoveFail { get; set; }
        static public MoveingEvent OnMoveing { get; set; }
        static public AutoMoveingEvent OnAutoMoving { get; set; }
        public EMoveState MoveState { get; set; }
        bool m_bEnable = true;
        private List<BeItem> mNeareastItems = new List<BeItem>();
        protected Vector3 m_moveDir = Vector3.zero;
        protected Vector3 m_oldMoveDir = Vector3.zero;
        protected Vector3 m_oldTargetPos = Vector3.zero;
        protected EActorMoveType m_oldMoveType = EActorMoveType.Invalid;
        protected float m_syncElapsed = 0.0f;
        protected float m_axisScale = 10000.0f;
        static UnityEngine.Coroutine m_autoMoveCoroutine;
        static bool m_bAutoMoveSuccessed = true;
        private float xGlobalMinDist = 5.0f;
        private float yGlobalMinDist = 8.0f;
        public BeFighterMain(BeFighterData data, ClientSystemGameBattle systemTown)
            : base(data, systemTown)
        {
            if (OnAutoMoveSuccess == null)
            {
                OnAutoMoveSuccess = new AutoMoveSuccessEvent();
            }

            if (OnMoveStateChanged == null)
            {
                OnMoveStateChanged = new MoveStateChangedEvent();
            }

            if (OnAutoMoveFail == null)
            {
                OnAutoMoveFail = new AutoMoveFailEvent();
            }

            if (OnMoveing == null)
            {
                OnMoveing = new MoveingEvent();
            }

            if (OnAutoMoving == null)
            {
                OnAutoMoving = new AutoMoveingEvent();
            }

            MoveState = EMoveState.Idle;
            var clientfunctabe = TableManager.GetInstance().GetTableItem<ProtoTable.SwitchClientFunctionTable>(62);
            if (clientfunctabe != null)
            {
                xGlobalMinDist = clientfunctabe.ValueA / 1000.0f;
                yGlobalMinDist = clientfunctabe.ValueB / 1000.0f;
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AvatarChanged, UpdateAvatar);
        }
        public sealed override void InitGeActor(GeSceneEx geScene)
        {
            base.InitGeActor(geScene);

            if (_geActor != null)
            {
                CreateFootIndicator();
                UpdateEquip();
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AvatarChanged, UpdateAvatar);
            }
        }
        public void UpdateEquip()
        {

            var avatar = PlayerBaseData.GetInstance().avatar;
            var jobID = PlayerBaseData.GetInstance().JobTableID;
            if (avatar != null)
            {
                SetGrassStat(GRASS_STATUS.NONE);
                m_grassId = 0;
                PlayerBaseData.GetInstance().AvatarEquipFromItems(null,
                    avatar.equipItemIds,
                    jobID,
                    (int)(avatar.weaponStrengthen),
                    _geActor,
                    false,
                    avatar.isShoWeapon);
            }
            else
            {
                Logger.LogErrorFormat("avatar is null in BeTownPlayerMain UpdateEquip");
            }
        }
        private void UpdateAvatar(UIEvent data = null)
        {
            UpdateEquip();
        }
        public void SetEnable(bool bEnable)
        {
            m_bEnable = bEnable;
        }
        public List<BeItem> FindNearestTownItems()
        {
            mNeareastItems.Clear();

            ClientSystemGameBattle gameBattle = this._battle;
            if(gameBattle == null)
            {
                Logger.LogError("Chiji this._battle == null in [FindNearestTownItems]");
                return mNeareastItems;
            }

            if(ActorData == null)
            {
                Logger.LogError("Chiji ActorData == null in [FindNearestTownItems]");
                return mNeareastItems;
            }

            var items = gameBattle.Items;      
            for (int i = 0; i < items.GetFightCount(); i++)
            {
                var curItem = items.GetFighter(i);
                if (curItem == null || curItem.IsBuffItem)
                {
                    continue;
                }

                var itemData = curItem.ActorData as BeItemData;
                if(itemData == null)
                {
                    Logger.LogErrorFormat("Chiji itemData == null in [FindNearestTownItems], ActorData.GUID = {0}, ActorData.Name = {1}", ActorData.GUID, ActorData.Name);
                    continue;
                }

                SceneItem dropItem = itemData.mDropItem;
                if(dropItem == null)
                {
                    Logger.LogErrorFormat("Chiji dropItem == null in [FindNearestTownItems], ActorData.GUID = {0}, ActorData.Name = {1}", ActorData.GUID, ActorData.Name);
                    continue;
                }

                if(ActorData.GUID == 0)
                {
                    Logger.LogErrorFormat("Chiji ActorData.GUID == 0 in [FindNearestTownItems], ActorData.Name = {0}", ActorData.Name);
                    continue;
                }

                if (dropItem.owner != ActorData.GUID)
                {
                    var dir = ActorData.MoveData.Position.xz() - curItem.ActorData.MoveData.Position.xz();
                    if (dir.magnitude <= 1.5f)
                    {
                        mNeareastItems.Add(curItem);
                    }
                }
            }

            return mNeareastItems;
        }
        public ulong FindNearestPlayer()
        {
            if (_battle.OtherFighters.GetFightCount() > 0)
            {
                var fighters = _battle.OtherFighters;
                float xMinDist = xGlobalMinDist;
                float yMinDist = yGlobalMinDist;
                ulong guid = 0ul;
                for (int i = 0; i < fighters.GetFightCount();i++)
                {
                    var fighter = fighters.GetFighter(i);
                    if (fighter != null && !fighter.IsDead && ((fighter.GrassId == 0) || (fighter.GrassId != 0 && GrassId == fighter.GrassId)))
                    {
                        if (_battle.OtherFighterBuffs.ContainsKey(fighter.ActorData.GUID))
                        {
                            var buffMgr = _battle.OtherFighterBuffs[fighter.ActorData.GUID];
                            if (buffMgr.HasBuffByID(400000000) || buffMgr.HasBuffByID(400000001))
                            {
                                continue;
                            }
                            var selfPos = this.ActorData.MoveData.Position.xz();
                            var otherPos = fighter.ActorData.MoveData.Position.xz();
                            float curDist = (selfPos - otherPos).magnitude;
                            var xDist = Mathf.Abs(selfPos.x - otherPos.x);
                            var yDist = Mathf.Abs(selfPos.y - otherPos.y);
                          //  Logger.LogErrorFormat("xDist {0} yDist {1}", xDist, yDist);
                            if (xDist <= xMinDist && yDist <= yMinDist)
                            {
                                if (xDist <= xMinDist)
                                {
                                    xMinDist = xDist;
                                }
                                if (yDist <= yMinDist)
                                {
                                    yMinDist = yDist;
                                }
                                guid = fighter.ActorData.GUID;
                            }
                        }
                    }
                }
                return guid;
            }
            return 0ul;
        }
        public sealed override void CommandMoveForward(Vector3 targetDirection)
        {

            targetDirection.y = 0.0f;
            Logger.LogFormat("Command Move Forward ==> target dir:({0},{1},{2})", targetDirection.x, targetDirection.y, targetDirection.z);
            base.CommandMoveForward(targetDirection);
            _UpdateMoveState();
        }

        public sealed override void CommandDirectMoveTo(Vector3 targetPosition)
        {
            targetPosition.y = 0.0f;
            Logger.LogProcessFormat("Command direct move to ==> target pos:({0},{1},{2})", targetPosition.x, targetPosition.y, targetPosition.z);
            base.CommandDirectMoveTo(targetPosition);
            _UpdateMoveState();
        }

        public sealed override void CommandMoveTo(Vector3 targetPosition)
        {
            targetPosition.y = 0.0f;
            Logger.LogProcessFormat("Command move to ==> target pos:({0},{1},{2})", targetPosition.x, targetPosition.y, targetPosition.z);
            base.CommandMoveTo(targetPosition);
            _UpdateMoveState();
        }

        public sealed override void CommandStopMove()
        {
            Logger.LogProcessFormat("Command stop move");
            base.CommandStopMove();
            _UpdateMoveState();
        }
        protected EMoveState _CalculateMoveState()
        {
            EMoveState newMoveState = EMoveState.Idle;
            if (ActorData.MoveData.MoveType == EActorMoveType.Invalid)
            {
                newMoveState = EMoveState.Idle;
            }
            else if (ActorData.MoveData.MoveType == EActorMoveType.TargetDir)
            {
                newMoveState = EMoveState.Moveing;
            }
            else if (ActorData.MoveData.MoveType == EActorMoveType.TargetPos)
            {
                newMoveState = EMoveState.AutoMoving;
            }
            return newMoveState;
        }

        protected void _UpdateMoveState()
        {
            EMoveState newMoveState = _CalculateMoveState();
            if (MoveState != newMoveState)
            {
                EMoveState oldMoveState = MoveState;
                MoveState = newMoveState;
                Logger.LogProcessFormat("移动状态发生变化 >>> 老的状态:{0} 新的状态:{1}", oldMoveState.ToString(), MoveState.ToString());
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerMoveStateChanged, oldMoveState, MoveState);

                Logger.LogProcessFormat("移动状态发生变化 >>> 老的状态:{0} 新的状态:{1}", oldMoveState.ToString(), MoveState.ToString());
            }
        }

        public sealed override void UpdateMove(float timeElapsed)
        {
            if (m_bEnable == false)
            {
                return;
            }

            if (_battle.IsNet)
            {
                _SyncMove();
            }

            base.UpdateMove(timeElapsed);

            if (MoveState == EMoveState.Moveing)
            {
                OnMoveing.Invoke(ActorData.MoveData.Position);
            }
            else if (MoveState == EMoveState.AutoMoving)
            {
                OnAutoMoving.Invoke(ActorData.MoveData.Position);
            }
        }

        protected void _SyncMove()
        {

            ActorMoveData moveData = ActorData.MoveData;
            if (m_oldMoveType != moveData.MoveType)
            {
                if (moveData.MoveType == EActorMoveType.TargetDir)
                {
                    m_oldMoveDir = moveData.TargetDirection;
                    m_oldTargetPos = Vector3.zero;
                    m_moveDir = moveData.TargetDirection;
                }
                else if (moveData.MoveType == EActorMoveType.TargetPos)
                {
                    m_oldMoveDir = Vector3.zero;
                    m_oldTargetPos = moveData.TargetPosition;
                    m_moveDir = (moveData.TargetPosition - moveData.Position).normalized;
                }
                else
                {
                    m_oldMoveDir = Vector3.zero;
                    m_oldTargetPos = Vector3.zero;
                    m_moveDir = Vector3.zero;
                }

                m_oldMoveType = moveData.MoveType;
                _SendSyncMoveMessage();
            }
            else
            {
                if (moveData.MoveType != EActorMoveType.Invalid)
                {
                    bool bSended = false;
                    if (moveData.MoveType == EActorMoveType.TargetDir && m_oldMoveDir != moveData.TargetDirection)
                    {
                        m_oldMoveDir = moveData.TargetDirection;
                        m_moveDir = moveData.TargetDirection;
                        _SendSyncMoveMessage();
                        bSended = true;
                    }
                    else if (moveData.MoveType == EActorMoveType.TargetPos && m_oldTargetPos != moveData.TargetPosition)
                    {
                        m_oldTargetPos = moveData.TargetPosition;
                        Vector3 newDir = (moveData.TargetPosition - moveData.Position).normalized;
                        if (m_moveDir != newDir)
                        {
                            m_moveDir = newDir;
                            _SendSyncMoveMessage();
                            bSended = true;
                        }
                    }

                    if (bSended == false)
                    {
                        m_syncElapsed += Time.deltaTime;
                        if (m_syncElapsed > 0.05f)
                        {
                            _SendSyncMoveMessage();
                            m_syncElapsed = 0.0f;
                        }
                    }
                }
            }
        }

        protected void _SendSyncMoveMessage()
        {
            SceneMoveRequire req = new SceneMoveRequire
            {
                dir = new SceneDir
                {
                    x = (short)(m_moveDir.x * m_axisScale),
                    y = (short)(m_moveDir.z * m_axisScale),
                    faceRight = (byte)(ActorData.MoveData.FaceRight ? 1 : 0)
                },
                pos = new ScenePosition
                {
                    x = (uint)(ActorData.MoveData.ServerPosition.x * m_axisScale),
                    y = (uint)(ActorData.MoveData.ServerPosition.z * m_axisScale)
                }  
            };

            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if(current != null)
            {
                req.clientSceneId = (uint)current.CurrentSceneID;
            }

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

            Logger.LogFormat(
                "send move dir:({0},{1}) face right:{4} pos:({2},{3})",
                req.dir.x, req.dir.y,
                ActorData.MoveData.Position.x,
                ActorData.MoveData.Position.z,
                req.dir.faceRight
                );
        }
        public void CommandMoveToScene(int a_sceneID, Vector3 a_vecPos)
        {
            if (m_bEnable == false)
            {
                return;
            }
            CommandStopAutoMove();

            Logger.LogProcessFormat("CommandMoveToScene ID:{0} pos:{1}\n", a_sceneID, a_vecPos.ToString2());
            m_autoMoveCoroutine = GameFrameWork.instance.StartCoroutine(_AsyncMoveToScene(_battle, a_sceneID, a_vecPos));
        }
        protected static bool _CheckSystemValid(ClientSystemGameBattle a_system)
        {
            if (a_system == null)
            {
                Logger.LogProcessFormat("systemGameBattle is null! stop!!");
                return false;
            }
            if (a_system.MainPlayer == null)
            {
                Logger.LogProcessFormat("main player is null! stop!!");
                return false;
            }

            return true;
        }
        static IEnumerator _AsyncAcrossScenes(ClientSystemGameBattle systemTown, int a_nTargetSceneID)
        {
            m_bAutoMoveSuccessed = true;

            Logger.LogProcessFormat("跨地图寻路 >>> 目标场景ID：{0}", a_nTargetSceneID);

            if (_CheckSystemValid(systemTown) == false)
            {
                Logger.LogProcessFormat("跨地图寻路 >>> 检测失败");
                m_bAutoMoveSuccessed = false;
                yield break;
            }

            if (systemTown.CurrentSceneID == a_nTargetSceneID)
            {
                m_bAutoMoveSuccessed = true;
                yield break;
            }

            List<Vector3> doorList = systemTown.GetMovePath(a_nTargetSceneID);
            if (doorList.Count <= 0)
            {
                SystemNotifyManager.SysNotifyMsgBoxOK(TR.Value("common_cannot_auto_move"));
                Logger.LogErrorFormat("跨地图寻路 >>> 无法自动移动到目标场景（ID:{0}）", a_nTargetSceneID);
                m_bAutoMoveSuccessed = false;
                yield break;
            }

            while (doorList.Count > 0)
            {
                Logger.LogProcessFormat("跨地图寻路 >>> 剩余传送门数量{0}", doorList.Count);
                if (_CheckSystemValid(systemTown) == false)
                {
                    Logger.LogProcessFormat("跨地图寻路 >>> 检测失败");
                    m_bAutoMoveSuccessed = false;
                    yield break;
                }

                Logger.LogProcessFormat("跨地图寻路 >>> 当前场景{0} >>> 开始", systemTown.CurrentSceneID);
                Vector3 targetPos = doorList[0];
                doorList.RemoveAt(0);
                systemTown.MainPlayer.CommandMoveTo(targetPos);
                if (systemTown.MainPlayer.MoveState != EMoveState.AutoMoving)
                {
                    Logger.LogProcessFormat("跨地图寻路 >>> 失败，当前的移动状态{0}", systemTown.MainPlayer.MoveState.ToString());
                    m_bAutoMoveSuccessed = false;
                    yield break;
                }

                Logger.LogProcessFormat("跨地图寻路 >>> 当前场景{0} >>> 移动中", systemTown.CurrentSceneID);
                int oldSceneID = systemTown.CurrentSceneID;
                while (oldSceneID == systemTown.CurrentSceneID)
                {
                    if (_CheckSystemValid(systemTown) == false)
                    {
                        Logger.LogProcessFormat("跨地图寻路 >>> 检测失败");
                        m_bAutoMoveSuccessed = false;
                        yield break;
                    }

                    if (systemTown.MainPlayer.MoveState == EMoveState.AutoMoving)
                    {
                        yield return Yielders.EndOfFrame;
                    }
                    else if (systemTown.MainPlayer.MoveState == EMoveState.Moveing)
                    {
                        Logger.LogProcessFormat("AutoMoveFail (interrupted) ==> move state EMoveState.Moveing");
                        m_bAutoMoveSuccessed = false;
                        yield break;
                    }
                    else
                    {
                        if (systemTown.MainPlayer._IsInSameGrid(targetPos, systemTown.MainPlayer.ActorData.MoveData.Position) == false)
                        {
                            Logger.LogProcessFormat("AutoMoveFail (interrupted) ==> move state EMoveState.Idle");
                            m_bAutoMoveSuccessed = false;
                            yield break;
                        }
                        else
                        {
                            yield return Yielders.EndOfFrame;
                        }
                    }
                }
            }
        }
        static IEnumerator _AsyncMoveToScene(ClientSystemGameBattle a_systemTown, int a_nTargetSceneID, Vector3 a_vecTargetPos)
        {
            yield return _AsyncAcrossScenes(a_systemTown, a_nTargetSceneID);

            if (m_bAutoMoveSuccessed == false)
            {
                BeFighterMain.OnAutoMoveFail.Invoke();
                m_autoMoveCoroutine = null;
                m_bAutoMoveSuccessed = true;
                yield break;
            }

            yield return _AsyncMoveToTargetPos(a_systemTown, a_vecTargetPos);

            if (m_bAutoMoveSuccessed == false)
            {
                BeFighterMain.OnAutoMoveFail.Invoke();
            }
            else
            {
                BeFighterMain.OnAutoMoveSuccess.Invoke();
            }

            m_autoMoveCoroutine = null;
            m_bAutoMoveSuccessed = true;
        }
        static IEnumerator _AsyncMoveToTargetPos(ClientSystemGameBattle a_systemTown, Vector3 a_pos)
        {
            m_bAutoMoveSuccessed = true;

            Logger.LogProcessFormat("当前地图寻路 >>> 当前场景{0} ", a_systemTown.CurrentSceneID);
            if (_CheckSystemValid(a_systemTown) == false)
            {
                Logger.LogProcessFormat("当前地图寻路 >>> 检测失败");
                m_bAutoMoveSuccessed = false;
                yield break;
            }
            a_systemTown.MainPlayer.CommandMoveTo(a_pos);

            while (true)
            {
                if (_CheckSystemValid(a_systemTown) == false)
                {
                    Logger.LogProcessFormat("当前地图寻路 >>> 检测失败");
                    m_bAutoMoveSuccessed = false;
                    yield break;
                }
                if (a_systemTown.MainPlayer.MoveState == EMoveState.AutoMoving)
                {
                    yield return Yielders.EndOfFrame;
                }
                else if (a_systemTown.MainPlayer.MoveState == EMoveState.Moveing)
                {
                    Logger.LogProcessFormat("AutoMoveFail (interrupted) ==> move state EMoveState.Moveing");
                    m_bAutoMoveSuccessed = false;
                    yield break;
                }
                else
                {
                    if (a_systemTown.MainPlayer._IsInSameGrid(a_pos, a_systemTown.MainPlayer.ActorData.MoveData.Position))
                    {
                        Logger.LogProcessFormat("当前地图寻路 >>> 移动成功");
                        m_bAutoMoveSuccessed = true;
                    }
                    else
                    {
                        Logger.LogProcessFormat("AutoMoveFail (interrupted) ==> move state EMoveState.Idle");
                        m_bAutoMoveSuccessed = false;
                    }
                    yield break;
                }
            }
        }
        protected bool _IsInSameGrid(Vector3 posA, Vector3 posB)
        {
            PathFinding.Grid gridA = new PathFinding.Grid(_battle.GridInfo, posA);
            PathFinding.Grid gridB = new PathFinding.Grid(_battle.GridInfo, posB);
            return gridA.X == gridB.X && gridA.Y == gridB.Y;
        }
        public static void CommandStopAutoMove()
        {
            if (m_autoMoveCoroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(m_autoMoveCoroutine);
                m_autoMoveCoroutine = null;

                Logger.LogProcessFormat("AutoMoveFail (interrupted) ==> move state EMoveState.AutoMove");
                BeFighterMain.OnAutoMoveFail.Invoke();
            }
        }
        //获得当前区域对应的地下城id
        public int GetPKDungeonID()
        {
            var enumerator = TableManager.GetInstance().GetTable<ChiJiPkSceneTable>().GetEnumerator();
            float posX = ActorData.MoveData.ServerPosition.x * PlayerBaseData.GetInstance().AxisScale;
            float posZ = ActorData.MoveData.ServerPosition.z * PlayerBaseData.GetInstance().AxisScale;
            while (enumerator.MoveNext())
            {
                var curTable = enumerator.Current.Value as ChiJiPkSceneTable;
                if (curTable == null) continue;
                if (curTable.SceneRangeLength != 4) continue;
                int minX = curTable.SceneRangeArray(0);
                int maxX = curTable.SceneRangeArray(1);
                int minY = curTable.SceneRangeArray(2);
                int maxY = curTable.SceneRangeArray(3);
                if (minX <= posX && posX <= maxX &&
                   minY <= posZ && posZ <= maxY)
                {
                    return curTable.DungeonID;

                }
            }
            return 60;
        }
    }
}
