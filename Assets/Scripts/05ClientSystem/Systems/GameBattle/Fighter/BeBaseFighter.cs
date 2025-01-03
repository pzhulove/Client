using System;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class BeBaseFighter : IDisposable
    {
        public DelayCaller delayCaller = new DelayCaller();
        protected GeActorEx _geActor;
        protected GeSceneEx _geScene;
        protected bool m_bDestroyed = false;
        protected BeBaseActorData _data;
        protected ClientSystemGameBattle _battle;
        private bool m_bRemoved = false;
        public virtual void Dispose()
        {
            m_bDestroyed = true;
            if (_geActor != null)
            {
                _geActor.Destroy();
                _geScene.DestroyActor(_geActor);
                _geActor = null;
            }
        }
        public bool IsRemoved { get { return m_bRemoved; } }
        public BeBaseActorData ActorData
        {
            get { return _data; }
        }
        public void Remove() { m_bRemoved = true; }
        public void CancelRemove() { m_bRemoved = false; } // 取消删掉 by Wangbo
        public virtual void OnRemove(){}
        public UInt64 GUID { get; set; }
        public virtual bool IsValid()
        {
            return m_bDestroyed == false;
        }
        public GeActorEx GraphicActor
        {
            get
            {
                return _geActor;
            }
        }
        public BeBaseFighter(BeBaseActorData data, ClientSystemGameBattle systemTown)
        {
            _data = data;
            _battle = systemTown;
        }
        public virtual void CommandMoveForward(Vector3 targetDirection)
        {
            targetDirection.y = 0.0f;
            //Logger.LogFormat("CommandMoveForward {0} {1} {2}\n", targetDirection.x, targetDirection.y, targetDirection.z);
            ActorData.MoveData.MoveType = EActorMoveType.TargetDir;
            ActorData.MoveData.TargetDirection = targetDirection;
            if (targetDirection.x > 0)
            {
                ActorData.MoveData.FaceRight = true;
            }
            else if (targetDirection.x < 0)
            {
                ActorData.MoveData.FaceRight = false;
            }

            DoActionWalk();
        }
        public void SetTargetDirection(Vector3 kTarget)
        {
            if (_geActor != null)
            {
                GameObject goCharactor = _geActor.GetEntityNode(GeEntity.GeEntityNodeType.Charactor);
                if (goCharactor != null)
                {
                    kTarget -= _data.MoveData.Position;
                    goCharactor.transform.localScale = new Vector3(kTarget.x > 0 ? 1.0f : -1.0f, 1.0f, 1.0f);
                }
            }
        }
        uint alphaEffectHandle = uint.MaxValue;
        const string alphaEffectshaderName = "隐匿";
        public enum GRASS_STATUS
        {
            NONE,
            MAIN_ROLE_IN_GRASS,
            SAME_WITH_MAINROLE_IN_GRASS,
            IN_GRASS,
        }
        GRASS_STATUS m_grassStat = GRASS_STATUS.NONE;
        public GRASS_STATUS GrassStatus { get { return m_grassStat; } }
        public virtual void SetGrassStat(GRASS_STATUS stat)
        {
            if (m_grassStat == stat) return;
            GRASS_STATUS lastStat = m_grassStat;
            m_grassStat = stat;
            if (_geActor == null) return;
            switch (lastStat)
            {
                case GRASS_STATUS.IN_GRASS:
                    {
                        _geActor.HideActor(false);
                        //引擎做了优化，在actorvisble为false的时候，动画是不做更新的
                        if (ActorData.ActionData.ActionName.Contains("Idle"))
                        {
                            _geActor.ChangeAction(ActorData.AniNames[(int)ActorActionType.AT_IDLE], 1, true, true, true);
                        }
                        else
                        {
                            string actionName = ActorData.AniNames[(int)ActorActionType.AT_RUN];
                            _geActor.ChangeAction(actionName, 1, true, true, true);
                        }
                        break;
                    }
                case GRASS_STATUS.SAME_WITH_MAINROLE_IN_GRASS:
                case GRASS_STATUS.MAIN_ROLE_IN_GRASS:
                    {
                        _geActor.RemoveSurface(alphaEffectHandle);
                        alphaEffectHandle = uint.MaxValue;
                        break;
                    }
            }
            switch (m_grassStat)
            {
                case GRASS_STATUS.IN_GRASS:
                    {
                        _geActor.HideActor(true);
                        break;
                    }
                case GRASS_STATUS.SAME_WITH_MAINROLE_IN_GRASS:
                case GRASS_STATUS.MAIN_ROLE_IN_GRASS:
                    {
                        alphaEffectHandle = _geActor.ChangeSurface(alphaEffectshaderName, 0);
                        break;
                    }
            }
        }
        public virtual void Update(float deltaTime)
        {
            delayCaller.Update((int)(deltaTime * 1000.0f));
            if (IsRemoved) return;
            UpdateMove(deltaTime);

            UpdateGeActor(deltaTime);
        }
        public virtual void CommandMoveTo(Vector3 targetPosition)
        {
            targetPosition.y = 0.0f;
            Logger.LogProcessFormat("CommandMoveTo {0} {1} {2}\n", targetPosition.x, targetPosition.y, targetPosition.z);
            ActorData.MoveData.MoveType = EActorMoveType.TargetPos;
            ActorData.MoveData.MovePath = PathFinding.FindPath(ActorData.MoveData.Position, targetPosition, _battle.GridInfo);
            ActorData.MoveData.TargetPosition = ActorData.MoveData.Position;
            _TryFollowPath();
        }
        public virtual void UpdateMove(float timeElapsed)
        {
            ActorMoveData moveData = ActorData.MoveData;
            if (moveData.MoveType == EActorMoveType.Invalid)
            {
                return;
            }

            if (moveData.MoveType == EActorMoveType.TargetDir)
            {
                Vector3 speed = moveData.TargetDirection;
                speed.x *= moveData.MoveSpeed.x;
                speed.y *= moveData.MoveSpeed.y;
                speed.z *= moveData.MoveSpeed.z;
                Vector3 newPosition = moveData.Position + speed * timeElapsed;
                if (_TryMove(ref newPosition, moveData.Position))
                {
                    moveData.Position = newPosition;
                }
            }
            else if (moveData.MoveType == EActorMoveType.TargetPos)
            {
                // 检测是否到达目标点
                if (_CheckPosEqual(moveData.TargetPosition, moveData.Position))
                {
                    if (_TryFollowPath() == false)
                    {
                        return;
                    }
                }

                // 更新位置
                Vector3 offset = moveData.TargetPosition - moveData.Position;
                Vector3 speed = offset.normalized;
                speed.x *= moveData.MoveSpeed.x;
                speed.y *= moveData.MoveSpeed.y;
                speed.z *= moveData.MoveSpeed.z;
                Vector3 newPosition = moveData.Position + speed * timeElapsed;

                // 检测是否越过目标点，如果是，则表示已到达目标点
                Vector3 offsetNew = moveData.TargetPosition - newPosition;
                if (offset.x * offsetNew.x <= 0.0f)
                {
                    newPosition.x = moveData.TargetPosition.x;
                }

                newPosition.y = 0.0f;

                if (offset.z * offsetNew.z <= 0.0f)
                {
                    newPosition.z = moveData.TargetPosition.z;
                }

                // 设置位置
                moveData.Position = newPosition;

                // 检测是否到达目标点
                if (_CheckPosEqual(moveData.TargetPosition, moveData.Position))
                {
                    if (_TryFollowPath() == false)
                    {
                        return;
                    }
                }
            }
        }
        public virtual void CommandStopMove()
        {
            ActorData.MoveData.MoveType = EActorMoveType.Invalid;
            ActorData.MoveData.TargetPosition = Vector3.zero;
            ActorData.MoveData.TargetDirection = Vector3.zero;
            ActorData.MoveData.MovePath = null;

            DoActionIdle();
        }
        public void ResetMoveCommand()
        {
            ActorData.MoveData.MoveType = EActorMoveType.Invalid;
            ActorData.MoveData.TargetPosition = Vector3.zero;
            ActorData.MoveData.TargetDirection = Vector3.zero;
            ActorData.MoveData.MovePath = null;
        }
        protected bool _TryFollowPath()
        {
            List<Vector3> movePath = ActorData.MoveData.MovePath;
            if (movePath == null || movePath.Count <= 0)
            {
                CommandStopMove();
                return false;
            }
            else
            {
                bool bSuccess = true;
                while (true)
                {
                    if (movePath.Count <= 0)
                    {
                        bSuccess = false;
                        break;
                    }
                    ActorData.MoveData.TargetPosition = movePath[0];
                    movePath.RemoveAt(0);

                    if (_CheckPosEqual(ActorData.MoveData.TargetPosition, ActorData.MoveData.Position) == false)
                    {
                        break;
                    }
                }

                if (bSuccess)
                {
                    if (ActorData.MoveData.TargetPosition.x > ActorData.MoveData.Position.x)
                    {
                        ActorData.MoveData.FaceRight = true;
                    }
                    else if (ActorData.MoveData.TargetPosition.x < ActorData.MoveData.Position.x)
                    {
                        ActorData.MoveData.FaceRight = false;
                    }

                    DoActionWalk();
                    return true;
                }
                else
                {
                    CommandStopMove();
                    return false;
                }
            }
        }

        protected bool _TryMove(ref Vector3 newPos, Vector3 currentPos)
        {

            ISceneData levelData = _battle.LevelData;
            float gridSizeX = levelData.GetGridSize().x;
            float gridSizeZ = levelData.GetGridSize().y;
            if (gridSizeX <= 0.01f || gridSizeZ <= 0.01f)
            {
                Logger.LogErrorFormat("level: {0} gridsize can not less than 0.01!!", levelData.GetName());
                return false;
            }

            Vector3 finalNewPos = currentPos;
            Vector3 startPos = currentPos;
            Vector3 targetPos = newPos;

            while (true)
            {
                int rateX = 1;
                int rateZ = 1;
                float offsetX = targetPos.x - startPos.x;
                while (Mathf.Abs(offsetX) >= gridSizeX)
                {
                    offsetX *= 0.5f;
                    rateX *= 2;
                }
                float offsetZ = targetPos.z - startPos.z;
                while (Mathf.Abs(offsetZ) >= gridSizeZ)
                {
                    offsetZ *= 0.5f;
                    rateZ *= 2;
                }

                Vector3 offset = targetPos - startPos;
                targetPos = startPos + offset / (float)(rateX > rateZ ? rateX : rateZ);

                if (_TryMoveOneGrid(ref targetPos, startPos))
                {
                    finalNewPos = targetPos;

                    if (rateX == 1 && rateZ == 1)
                    {
                        break;
                    }
                    startPos = targetPos;
                    targetPos = newPos;
                }
                else
                {
                    break;
                }
            }

            newPos = finalNewPos;

            return !_CheckPosEqual(newPos, currentPos);
        }

        protected bool _TryMoveOneGrid(ref Vector3 newPos, Vector3 currentPos)
        {
            if (_CheckPosEqual(newPos, currentPos))
            {
                return false;
            }

            ISceneData levelData = _battle.LevelData;
            float gridSizeX = levelData.GetGridSize().x;
            float gridSizeZ = levelData.GetGridSize().y;

            float currGridMinX = (int)Math.Floor(currentPos.x / gridSizeX) * gridSizeX;
            float currGridMaxX = currGridMinX + gridSizeX;
            float currGridMinZ = (int)Math.Floor(currentPos.z / gridSizeZ) * gridSizeZ;
            float currGridMaxZ = currGridMinZ + gridSizeZ;

            if (
                currGridMinX <= newPos.x && newPos.x <= currGridMaxX - 0.01f &&
                currGridMinZ <= newPos.z && newPos.z <= currGridMaxZ - 0.01f
                )
            {
                return true;
            }

            int newGridX = (int)Math.Floor(newPos.x / gridSizeX);
            int newGridZ = (int)Math.Floor(newPos.z / gridSizeZ);

            int currGridX = (int)Math.Floor(currentPos.x / gridSizeX);
            int currGridZ = (int)Math.Floor(currentPos.z / gridSizeZ);

            if (Mathf.Abs(newPos.x - currentPos.x) > 0.000001f)
            {
                bool bLeft = newPos.x < currentPos.x;

                Vector3 borderA = new Vector3(bLeft ? currGridMinX : currGridMaxX, 0.0f, currGridMinZ);
                Vector3 borderB = new Vector3(bLeft ? currGridMinX : currGridMaxX, 0.0f, currGridMaxZ);

                if (_Intersect(currentPos, newPos, borderA, borderB))
                {

                    if (_GridCanMove(newGridX, currGridZ))
                    {
                        if (_GridCanMove(newGridX, newGridZ))
                        {
                            newPos.x = Mathf.Clamp(newPos.x, newPos.x, (newGridX + 1) * gridSizeX - 0.01f);
                            newPos.z = Mathf.Clamp(newPos.z, newPos.z, (newGridZ + 1) * gridSizeZ - 0.01f);
                            return !_CheckPosEqual(newPos, currentPos);
                        }
                        else
                        {
                            newPos.z = newGridZ < currGridZ ? currGridMinZ : currGridMaxZ - 0.01f;
                        }
                    }
                    else
                    {
                        newPos.x = bLeft ? currGridMinX : currGridMaxX - 0.01f;
                    }
                }
            }

            if (Mathf.Abs(newPos.z - currentPos.z) > 0.000001f)
            {
                bool bBottom = newPos.z < currentPos.z;
                Vector3 borderA = new Vector3(currGridMinX, 0.0f, bBottom ? currGridMinZ : currGridMaxZ);
                Vector3 borderB = new Vector3(currGridMaxX, 0.0f, bBottom ? currGridMinZ : currGridMaxZ);

                if (_Intersect(currentPos, newPos, borderA, borderB))
                {
                    if (_GridCanMove(currGridX, newGridZ))
                    {
                        if (_GridCanMove(newGridX, newGridZ))
                        {
                            newPos.x = Mathf.Clamp(newPos.x, newPos.x, (newGridX + 1) * gridSizeX - 0.01f);
                            newPos.z = Mathf.Clamp(newPos.z, newPos.z, (newGridZ + 1) * gridSizeZ - 0.01f);
                            return !_CheckPosEqual(newPos, currentPos);
                        }
                        else
                        {
                            newPos.x = newGridX < currGridX ? currGridMinX : currGridMaxX - 0.01f;
                        }
                    }
                    else
                    {
                        newPos.z = bBottom ? currGridMinZ : currGridMaxZ - 0.01f;
                    }
                }
            }

            return !_CheckPosEqual(newPos, currentPos);
        }

        protected bool _CheckPosEqual(Vector3 posA, Vector3 posB)
        {
            posA.y = 0.0f;
            posB.y = 0.0f;
            if ((posA - posB).sqrMagnitude <= 0.000002f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool _GridCanMove(int gridX, int gridZ)
        {
            ISceneData levelData = _battle.LevelData;
            if (gridX < levelData.GetLogicXmin() || gridX >= levelData.GetLogicXmax())
            {
                return false;
            }
            if (gridZ < levelData.GetLogicZmin() || gridZ >= levelData.GetLogicZmax())
            {
                return false;
            }

            int x = gridX - levelData.GetLogicXmin();
            int y = gridZ - levelData.GetLogicZmin();

            int index = (levelData.GetLogicXmax() - levelData.GetLogicXmin()) * y + x;
            if (index >= 0 && index < levelData.GetBlockLayerLength())
            {
                return levelData.GetBlockLayer(index) == 0;
            }
            else
            {
                return false;
            }
        }


        protected double _Mult(Vector3 a, Vector3 b, Vector3 c)
        {
            return (a.x - c.x) * (b.y - c.y) - (b.x - c.x) * (a.y - c.y);
        }

        protected bool _Intersect(Vector3 aa, Vector3 bb, Vector3 cc, Vector3 dd)
        {
            if (Mathf.Max(aa.x, bb.x) < Mathf.Min(cc.x, dd.x))
            {
                return false;
            }
            if (Mathf.Max(aa.y, bb.y) < Mathf.Min(cc.y, dd.y))
            {
                return false;
            }
            if (Mathf.Max(cc.x, dd.x) < Mathf.Min(aa.x, bb.x))
            {
                return false;
            }
            if (Mathf.Max(cc.y, dd.y) < Mathf.Min(aa.y, bb.y))
            {
                return false;
            }
            if (_Mult(cc, bb, aa) * _Mult(bb, dd, aa) < 0)
            {
                return false;
            }
            if (_Mult(aa, dd, cc) * _Mult(dd, bb, cc) < 0)
            {
                return false;
            }
            return true;
        }
        public virtual void InitGeActor(GeSceneEx geScene)
        {
            _geScene = geScene;
            alphaEffectHandle = uint.MaxValue;
            ActorData.MoveData.TransformDirty = true;
            UpdateGeActor(0.0f);
        }

        public virtual void UpdateGeActor(float timeElapsed)
        {
            if (_geActor != null)
            {
                if (ActorData.MoveData.TransformDirty == true)
                {
                    _geActor.SetPosition(ActorData.MoveData.GraphPosition);
                    _geActor.SetFaceLeft(!ActorData.MoveData.FaceRight);
                    ActorData.MoveData.TransformDirty = false;
                }

                if (ActorData.ActionData.isDirty == true)
                {
                    _geActor.ChangeAction(ActorData.ActionData.ActionName, ActorData.ActionData.ActionSpeed, ActorData.ActionData.ActionLoop, false);
                    ActorData.ActionData.isDirty = false;
                }


                for (int i = 0; i < ActorData.arrAttachmentData.Count; ++i)
                {
                    AttachmentPlayData attachData = ActorData.arrAttachmentData[i];
                    if (attachData.isDirty == true)
                    {
                        _geActor.SetAttachmentVisible(attachData.attachmentName, attachData.visible);
                        attachData.isDirty = false;
                    }
                }

                if (_geActor != null)
                {
                    _geActor.Update((int)(timeElapsed * (float)GlobalLogic.VALUE_1000));
                }
            }
        }

        public virtual void DoActionIdle()
        {
            ActorData.ActionData.ActionName = ActorData.AniNames[(int)ActorActionType.AT_IDLE];
            ActorData.ActionData.ActionSpeed = 1.0f;
        }

        public virtual void DoActionWalk()
        {
            ActorData.ActionData.ActionName = ActorData.AniNames[(int)ActorActionType.AT_RUN];
            ActorData.ActionData.ActionSpeed = Global.Settings.townActionSpeed;
        }
        public virtual void DoActionDead()
        {
            ActorData.ActionData.ActionName = ActorData.AniNames[(int)ActorActionType.AT_DEAD];
            ActorData.ActionData.ActionSpeed = 1.0f;
            ActorData.ActionData.ActionLoop = false;
        }
        public virtual void CommandDirectMoveTo(Vector3 targetPosition)
        {
            targetPosition.y = 0.0f;
            ActorData.MoveData.MoveType = EActorMoveType.TargetPos;
            ActorData.MoveData.MovePath = new List<Vector3>();
            ActorData.MoveData.MovePath.Add(targetPosition);
            ActorData.MoveData.TargetPosition = ActorData.MoveData.Position;
            _TryFollowPath();
        }
    }
    public class BeFighterManager<T> where T : BeBaseFighter
    {
        List<T> m_Fighters = new List<T>();
        Dictionary<UInt64, T> m_mapFighters = new Dictionary<UInt64, T>();

        public void AddFighter(UInt64 guid, T fighter)
        {
            if (m_mapFighters.ContainsKey(guid))
            {
                return;
            }

            m_Fighters.Add(fighter);
            m_mapFighters.Add(guid,fighter);
        }

        public void AddFighter(T fighter)
        {
            m_Fighters.Add(fighter);
        }

        public int GetFightCount() { return m_Fighters.Count;}


        public T GetFighter(int index)
        {
            if (index < 0 || index >= m_Fighters.Count) return null;
            if (m_Fighters[index] == null || m_Fighters[index].IsRemoved) return null;
            return m_Fighters[index];
        }

        public void Update(float deltaTime)
        {
            bool needRemove = false;
            for (int i = 0; i < m_Fighters.Count;i++)
            {
                var fighter = m_Fighters[i];
                if (!fighter.IsRemoved)
                {
                    fighter.Update(deltaTime);
                }
                else
                {
                    needRemove = true;
                }
            }

            if (needRemove)
            {
                m_Fighters.RemoveAll(figher =>
                {
                    if (figher != null && figher.IsRemoved)
                    {
                        figher.OnRemove();
                        figher.Dispose();
                        //if(m_mapFighters.ContainsKey(figher.GUID))
                        //{
                        //    m_mapFighters.Remove(figher.GUID);
                        //}
                    }
                    return figher == null || figher.IsRemoved;
                });
            }
        }

        public void Refresh()
        {
            m_Fighters.RemoveAll(figher =>
            {
                if (figher != null && figher.IsRemoved)
                {
                    figher.OnRemove();
                }
                return figher == null || figher.IsRemoved;
            });
        }

        public T GetFighter(UInt64 guid)
        {
            if(m_mapFighters.ContainsKey(guid))
            {
                return m_mapFighters[guid];
            }

            return null;
        }

        public void RemoveFighter(UInt64 guid)
        {
            if (m_mapFighters.ContainsKey(guid))
            {
                m_mapFighters[guid].Remove();
                m_mapFighters.Remove(guid);
            }
        }

        public void RemoveFighter(int index)
        {
            if (index < 0 || index >= m_Fighters.Count)
            {
                return;
            }

            var figher = m_Fighters[index];
            if(figher != null)
            {
                figher.Remove();
                if (m_mapFighters.ContainsKey(figher.ActorData.GUID))
                {
                    m_mapFighters.Remove(figher.ActorData.GUID);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < m_Fighters.Count; i++)
            {
                var fighter = m_Fighters[i];
                if (fighter != null)
                {
                    fighter.Dispose();
                    fighter.Remove();
                }
            }
            m_Fighters.Clear();
            m_mapFighters.Clear();
        }
    }

}

