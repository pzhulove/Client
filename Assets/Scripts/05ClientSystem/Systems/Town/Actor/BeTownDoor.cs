using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    public class BeTownDoorData : BeBaseActorData
    {
        public ISceneTownDoorData Door;
    }

    public class OnDoorTrigger : UnityEvent<ISceneTownDoorData> { }

    class BeTownDoor : BeBaseActor
    {
        ISceneTownDoorData m_doorData;

        int m_nLevelLimit = 0;
        bool m_bOpened = false;
        bool m_bDirty = true;
        bool opened
        {
            get { return m_bOpened; }
            set
            {
                if (m_bOpened != value)
                {
                    m_bOpened = value;
                    m_bDirty = true;
                }
            }
        }

        BeRegionState mState = BeRegionState.Out;
        GeTownDoorEffect m_geDoorEffect = null;

        public OnDoorTrigger OnTrigger = new OnDoorTrigger();

        public BeTownDoor(BeTownDoorData data, ClientSystemTown systemTown)
        : base(data, systemTown)
        {
            m_doorData = data.Door;
            if (_isInRegion(_systemTown.MainPlayer.ActorData.MoveData.Position))
            {
                mState = BeRegionState.In;
                Logger.LogProcessFormat("init trigger in!!! Scene:{0} Door:{1}", m_doorData.GetSceneID(), m_doorData.GetDoorID());
            }
            else
            {
                mState = BeRegionState.Out;
                Logger.LogProcessFormat("init trigger out!!! Scene:{0} Door:{1}", m_doorData.GetSceneID(), m_doorData.GetDoorID());
            }

            var tableScene = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(m_doorData.GetTargetSceneID());
            if (tableScene == null)
            {
                Logger.LogErrorFormat("BeTownDoor targetScene table id {0} is not exist!!", m_doorData.GetTargetSceneID());
                return;
            }
            opened = PlayerBaseData.GetInstance().Level >= tableScene.LevelLimit;
        }

        public override void InitGeActor(GeSceneEx geScene)
        {
            if (geScene == null)
            {
                return;
            }

            if (_geActor == null)
            {
                ProtoTable.SceneRegionTable regionTable = TableManager.GetInstance().GetTableItem<ProtoTable.SceneRegionTable>(m_doorData.GetRegionInfo().GetEntityInfo().GetResid());
                if (regionTable != null)
                {
                    _geActor = geScene.CreateActor(regionTable.ResID, 0, 0, false, false);

                    var node = _geActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);
                    if (null != node)
                    {
                        m_geDoorEffect = node.GetComponentInChildren<GeTownDoorEffect>();
                    }
                }
                else
                {
                    Logger.LogErrorFormat("door id:{0} can not find!", m_doorData.GetRegionInfo().GetEntityInfo().GetResid());
                }
            }

            base.InitGeActor(geScene);
        }

        public override void Update(float deltaTime)
        {
            if (m_bDestroied)
            {
                return;
            }

            opened = PlayerBaseData.GetInstance().Level >= m_nLevelLimit;

            if (_systemTown.MainPlayer == null)
            {
                return;
            }

            if (mState == BeRegionState.Out)
            {
                if (_isInRegion(_systemTown.MainPlayer.ActorData.MoveData.Position))
                {
                    // trigger
                    Logger.LogProcessFormat("trigger in!!! Scene:{0} Door:{1}", m_doorData.GetSceneID(), m_doorData.GetDoorID());
                    if (IsDoorCanTrigger() == true)
                    {
                        OnTrigger.Invoke(m_doorData);
                        mState = BeRegionState.In;
                    }
                }
            }
            else if (mState == BeRegionState.In)
            {
                if (!_isInRegion(_systemTown.MainPlayer.ActorData.MoveData.Position))
                {
                    Logger.LogProcessFormat("trigger out!!! Scene:{0} Door:{1}", m_doorData.GetSceneID(), m_doorData.GetDoorID());
                    mState = BeRegionState.Out;
                }
            }
        }

        public override void UpdateGeActor(float timeElapsed)
        {
            if (m_bDirty)
            {
                if (m_geDoorEffect != null)
                {
                    m_geDoorEffect.SetDoorOpen(m_bOpened);
                    m_bDirty = false;
                }
            }

            base.UpdateGeActor(timeElapsed);
        }


        public override void Dispose()
        {
            base.Dispose();
        }

        bool _isInRegion(Vector3 pos)
        {
            var p = pos;

            if (m_doorData.GetRegionInfo().GetRegiontype() == DRegionInfo.RegionType.Circle)
            {
                var dis = Vector3.Distance(m_doorData.GetRegionInfo().GetEntityInfo().GetPosition(), p);
                return dis <= m_doorData.GetRegionInfo().GetRadius();
            }
            else if (m_doorData.GetRegionInfo().GetRegiontype() == DRegionInfo.RegionType.Rectangle)
            {
                Vector3 position = m_doorData.GetRegionInfo().GetEntityInfo().GetPosition();
                Vector2 rect     = m_doorData.GetRegionInfo().GetRect();

                Vector2 min = new Vector2(position.x - rect.x / 2, position.z - rect.y / 2);
                Vector2 max = new Vector2(position.x + rect.x / 2, position.z + rect.y / 2);

                if (p.x < min.x) return false;
                if (p.z < min.y) return false;
                if (p.x > max.x) return false;
                if (p.z > max.y) return false;

                return true;
            }
            return false;
        }

        /// <summary>
        /// 传送门是否可以触发
        /// 当MainPlayer在朝着固定的位置（同一个场景中的某个传送门或者某个固定的位置）寻路时：
        /// 如果经过了一个非目标传送门（即目标位置和传送门位置不一致），此时传送门不触发；否则传送门触发
        /// </summary>
        /// <returns></returns>
        private bool IsDoorCanTrigger()
        {
            //正在自动寻路，并且是朝着目标节点走动
            if (_systemTown.MainPlayer.MoveState == BeTownPlayerMain.EMoveState.AutoMoving
                && _systemTown.MainPlayer.ActorData.MoveData.MoveType == EActorMoveType.TargetPos)
            {
                //目标位置
                var targetPosition = _systemTown.MainPlayer.ActorData.MoveData.TargetPosition;
                //当前传送门位置
                var doorPosition = m_doorData.GetRegionInfo().GetEntityInfo().GetPosition();

                //目标位置并不是当前的传送门，则传送门不触发
                if (Vector3.SqrMagnitude(targetPosition - doorPosition) > 0.0001)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
