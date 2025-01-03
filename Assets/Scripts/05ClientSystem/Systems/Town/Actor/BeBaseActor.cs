using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameClient
{
    public enum EActorMoveType
    {
        Invalid = -1,
        TargetPos = 0,
        TargetDir,
    }

    public enum MovingDirectionType
    {
        MDT_LEFT = 0,
        MDT_RIGHT,
        MDT_UP,
        MDT_DOWN,
    }

	public enum ActorActionType
	{
		[Description("Anim_Idle01")]
		AT_IDLE = 0,
		[Description("Anim_Walk")]
		AT_WALK,
		[Description("Anim_Run")]
		AT_RUN,
        [Description("Anim_Dead")]
        AT_DEAD,

        AT_COUNT,
	}
		
    public class ActorMoveData
    {
        protected Vector3 _position = Vector3.zero;
        protected Vector3 _graphPosition = Vector3.zero;
        protected Vector3 _posLogicToGraph = Vector3.zero;
        protected bool _transformDirty = true;
        protected bool _faceRight = true;

        protected Vector3 _moveSpeed = Vector3.zero;
        protected EActorMoveType _moveType = EActorMoveType.Invalid;
        protected Vector3 _targetPosition = Vector3.zero;
        protected Vector3 _targetDirection = Vector3.zero;
        protected List<Vector3> _targetPath = new List<Vector3>();

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    _graphPosition = _position + _posLogicToGraph;
                    _transformDirty = true;
                }
            }
        }

        public MovingDirectionType MovingDirection
        {
            get
            {
                var current = (_targetPosition - _position).normalized;
                if(Mathf.Abs(current.x) > Mathf.Abs(current.y))
                {
                    return current.x < 0.0f ? MovingDirectionType.MDT_LEFT : MovingDirectionType.MDT_RIGHT;
                }
                return current.y < 0.0f ? MovingDirectionType.MDT_UP : MovingDirectionType.MDT_DOWN;
            }
        }

        public Vector3 GraphPosition
        {
            get { return _graphPosition; }
        }

        public Vector3 PosLogicToGraph
        {
            set
            {
                _posLogicToGraph = value;
                _graphPosition = _position + _posLogicToGraph;
                _transformDirty = true;
            }

            get { return _posLogicToGraph; }
        }

        // 服务器不支持负数，转了一下坐标系！！服务器用场景左下角为原点，客户端用的就是编辑器编辑的逻辑原点
        // 好无奈，好担心会出错
        public Vector3 PosServerToClient { get; set; }
        public Vector3 ServerPosition
        {
            set
            {
                Position = value + PosServerToClient;
            }

            get
            {
                return Position - PosServerToClient;
            }
        }

        public bool FaceRight
        {
            get { return _faceRight; }
            set
            {
                if (_faceRight != value)
                {
                    _faceRight = value;
                    _transformDirty = true;
                }
            }
        }

        public bool TransformDirty
        {
            get { return _transformDirty; }
            set { _transformDirty = value; }
        }

        public Vector3 MoveSpeed
        {
            get { return _moveSpeed; }
            set { _moveSpeed = value; }
        }

        public EActorMoveType MoveType
        {
            get { return _moveType; }
            set { _moveType = value; }
        }

        public Vector3 TargetPosition
        {
            get { return _targetPosition; }
            set { _targetPosition = value; }
        }

        public Vector3 TargetDirection
        {
            get { return _targetDirection; }
            set { _targetDirection = value.normalized; }
        }
        public List<Vector3> MovePath { get; set; }
    }

    public class ActionPlayData
    {
        protected string actionName = "Anim_Idle01";
        protected bool actionLoop = true;
        protected float actionSpeed = 1.0f;
        protected bool m_bDirty = true;
        protected bool force = false;

        public string ActionName
        {
            get { return actionName; }
            set
            {
                if (actionName != value)
                {
                    actionName = value;
                    m_bDirty = true;
                }
            }
        }

        public float ActionSpeed
        {
            get { return actionSpeed; }
            set
            {
                if (actionSpeed != value)
                {
                    actionSpeed = value;
                    m_bDirty = true;
                }
            }
        }

        public bool ActionLoop
        {
            get { return actionLoop; }
            set
            {
                if (actionLoop != value)
                {
                    actionLoop = value;
                    m_bDirty = true;
                }
            }
        }

        public bool isDirty
        {
            get { return m_bDirty; }
            set { m_bDirty = value; }
        }

        public bool isForce
        {
            get {return force;}
            set {force = value;}
        }
    }

    public class AttachmentPlayData : ActionPlayData
    {
        protected string m_strAttachmentName = "";
        protected bool m_bVisible = true;

        public string attachmentName
        {
            get { return m_strAttachmentName; }
            set
            {
                m_strAttachmentName = value;
            }
        }

        public bool visible
        {
            get { return m_bVisible; }
            set
            {
                if (m_bVisible != value)
                {
                    m_bVisible = value;
                    m_bDirty = true;
                }
            }
        }
    }

    public class BeBaseActorData
    {
        public ulong GUID { get; set; }
        public float Scale { get; set; }
        public virtual string Name { get; set; }
		public string []AniNames = new string[(int)ActorActionType.AT_COUNT];
        public PlayerInfoColor NameColor { get; set; }


		public BeBaseActorData()
		{
			AniNames[(int)ActorActionType.AT_IDLE] = ActorActionType.AT_IDLE.GetDescription();
			AniNames[(int)ActorActionType.AT_WALK] = ActorActionType.AT_WALK.GetDescription();
			AniNames[(int)ActorActionType.AT_RUN] = ActorActionType.AT_RUN.GetDescription();
		}

        protected ActorMoveData _moveData = new ActorMoveData();
        public ActorMoveData MoveData
        {
            get { return _moveData; }
        }

        protected ActionPlayData _actionPlayData = new ActionPlayData();
        public ActionPlayData ActionData
        {
            get { return _actionPlayData; }
        }

        protected List<AttachmentPlayData> m_arrAttachmentPlayData = new List<AttachmentPlayData>();
        public List<AttachmentPlayData> arrAttachmentData
        {
            get { return m_arrAttachmentPlayData; }
        }

        public void SetAttachmentVisible(string a_strAttachment, bool a_bVisible)
        {
            for (int i = 0; i < arrAttachmentData.Count; ++i)
            {
                if (arrAttachmentData[i].attachmentName == a_strAttachment)
                {
                    arrAttachmentData[i].visible = a_bVisible;
                    return;
                }
            }

            AttachmentPlayData data = new AttachmentPlayData();
            data.attachmentName = a_strAttachment;
            data.visible = a_bVisible;
            arrAttachmentData.Add(data);
        }

        public void PlayAttachmentAction(string a_strAttachment, string a_strAction)
        {
            for (int i = 0; i < arrAttachmentData.Count; ++i)
            {
                if (arrAttachmentData[i].attachmentName == a_strAttachment)
                {
                    arrAttachmentData[i].ActionName = a_strAction;
                    return;
                }
            }

            AttachmentPlayData data = new AttachmentPlayData();
            data.attachmentName = a_strAttachment;
            data.ActionName = a_strAction;
            arrAttachmentData.Add(data);
        }


        public void SetAniInfos(string idle, string walk, string run, string dead)
		{
			AniNames[(int)ActorActionType.AT_IDLE] = idle;
			AniNames[(int)ActorActionType.AT_RUN] = run;
			AniNames[(int)ActorActionType.AT_WALK] = walk;
            AniNames[(int)ActorActionType.AT_DEAD] = dead;
        }
    }

    /*
    请确保所有的函数，在相同输入的情况下，能有相同的输出
    如：不要使用本地时间、随机数、float计算
    */
    class BeBaseActor : IDisposable
    {
        protected GeActorEx _geActor;
        public GeActorEx GeActor { get { return _geActor; } }
        protected GeSceneEx _geScene;
        public GeActorEx GraphicActor
        {
            get
            {
                /*
                if (_geActor == null)
                {
                    Logger.LogError("_geActor is nil");
                }
                */
                return _geActor;
            }
        }

        protected BeBaseActorData _data;
        public BeBaseActorData ActorData
        {
            get { return _data; }
        }

        public DelayCaller delayCaller = new DelayCaller();

        protected ClientSystemTown _systemTown;

        static List<BeBaseActor> ms_actors = new List<BeBaseActor>();
        static public List<BeBaseActor> Actors
        {
            get { return ms_actors; }
        }

        protected bool m_bDestroied = false;

        public BeBaseActor(BeBaseActorData data, ClientSystemTown systemTown)
        {
            _data = data;
            _systemTown = systemTown;

            ms_actors.Add(this);
        }

        public virtual void Dispose()
        {
            m_bDestroied = true;
            if (_geActor != null)
            {
                //_geActor.Destroy();
                _geScene.DestroyActor(_geActor);
                _geActor = null;
            }

            ms_actors.Remove(this);
        }

        public virtual bool IsValid()
        {
            return m_bDestroied == false;
        }

        public void SetTargetDirection(Vector3 kTarget)
        {
            if(_geActor != null)
            {
                GameObject goCharactor = _geActor.GetEntityNode(GeEntity.GeEntityNodeType.Charactor);
                if (goCharactor != null)
                {
                    kTarget -= _data.MoveData.Position;
                    //kTarget.y = 0.0f;
                    //kTarget.z = 0.0f;
                    //float fRadio = Mathf.Atan2(-kTarget.z, kTarget.x) * Mathf.Rad2Deg;
                    //goCharactor.transform.localRotation = Quaternion.Euler(0.0f, fRadio, 0.0f);
                    goCharactor.transform.localScale = new Vector3(kTarget.x > 0 ? 1.0f : -1.0f,1.0f,1.0f);
                }
            }
        }

        public virtual void Update(float timeElapsed)
        {
            delayCaller.Update((int)(timeElapsed * 1000.0f));
            
            UpdateMove(timeElapsed);
            
            UpdateGeActor(timeElapsed);
        }

        #region Actor Action      
        public void SetAction(ActorActionType eActorActionType,float fSpeed = 1.0f,bool bLoop = false)
        {
            if(eActorActionType >= ActorActionType.AT_IDLE && eActorActionType < ActorActionType.AT_COUNT)
            {
                ActorData.ActionData.ActionName = eActorActionType.GetDescription();
                ActorData.ActionData.ActionSpeed = fSpeed;
                ActorData.ActionData.ActionLoop = bLoop;
            }
        }
        #endregion

        #region Actor Move

        public virtual void CommandMoveTo(Vector3 targetPosition)
        {
            targetPosition.y = 0.0f;
            Logger.LogProcessFormat("CommandMoveTo {0} {1} {2}\n", targetPosition.x, targetPosition.y, targetPosition.z);
            ActorData.MoveData.MoveType = EActorMoveType.TargetPos;
            ActorData.MoveData.MovePath = PathFinding.FindPath(ActorData.MoveData.Position, targetPosition, _systemTown.GridInfo);
            ActorData.MoveData.TargetPosition = ActorData.MoveData.Position;
            _TryFollowPath();
        }

        public virtual void CommandDirectMoveTo(Vector3 targetPosition)
        {
            targetPosition.y = 0.0f;
            //Logger.LogFormat("CommandDirectMoveTo {0} {1} {2}\n", targetPosition.x, targetPosition.y, targetPosition.z);
            ActorData.MoveData.MoveType = EActorMoveType.TargetPos;
            ActorData.MoveData.MovePath = new List<Vector3>();
            ActorData.MoveData.MovePath.Add(targetPosition);
            ActorData.MoveData.TargetPosition = ActorData.MoveData.Position;
            _TryFollowPath();
        }

        public virtual void CommandMoveForward(Vector3 targetDirection)
        {
            targetDirection.y = 0.0f;
            //Logger.LogFormat("CommandMoveForward {0} {1} {2}\n", targetDirection.x, targetDirection.y, targetDirection.z);
            ActorData.MoveData.MoveType = EActorMoveType.TargetDir;
            ActorData.MoveData.TargetDirection = targetDirection;
            if(targetDirection.x > 0)
            {
                ActorData.MoveData.FaceRight = true;
            }
            else if(targetDirection.x < 0)
            {
                ActorData.MoveData.FaceRight = false;
            }

			DoActionWalk();
        }

        public virtual void CommandStopMove()
        {
            //Logger.LogFormat("CommandStopMove");
            ActorData.MoveData.MoveType = EActorMoveType.Invalid;
            ActorData.MoveData.TargetPosition = Vector3.zero;
            ActorData.MoveData.TargetDirection = Vector3.zero;
            ActorData.MoveData.MovePath = null;

			DoActionIdle();
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
            ISceneData levelData = _systemTown.LevelData;
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

            int maxLoopCount = 32;

            while (--maxLoopCount > 0)
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

            ISceneData levelData = _systemTown.LevelData;
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
            ISceneData levelData = _systemTown.LevelData;
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


        #endregion

        #region Actor Graphi
        public virtual void InitGeActor(GeSceneEx geScene)
        {
            _geScene = geScene;
            ActorData.MoveData.TransformDirty = true;
            UpdateGeActor(0.0f);
        }

        public virtual void UpdateGeActor(float timeElapsed)
        {
            if (_geActor != null)
            {
                if(ActorData.MoveData.TransformDirty == true)
                {
                    _geActor.SetPosition(ActorData.MoveData.GraphPosition);
                    _geActor.SetFaceLeft(!ActorData.MoveData.FaceRight);
                    ActorData.MoveData.TransformDirty = false;
                }
                
                if (ActorData.ActionData.isDirty == true)
                {
                    _geActor.ChangeAction(ActorData.ActionData.ActionName, ActorData.ActionData.ActionSpeed, ActorData.ActionData.ActionLoop, false, ActorData.ActionData.isForce);
                    ActorData.ActionData.isForce = false;
                    ActorData.ActionData.isDirty = false;
                }


                for (int i = 0; i < ActorData.arrAttachmentData.Count; ++i)
                {
                    AttachmentPlayData attachData = ActorData.arrAttachmentData[i];
                    if (attachData.isDirty == true)
                    {
                        _geActor.SetAttachmentVisible(attachData.attachmentName, attachData.visible);
						/*
                        if (attachData.visible)
                        {
                            _geActor.PlayAttachmentAnimation(attachData.attachmentName, attachData.ActionName);
                        }*/

                        attachData.isDirty = false;
                    }
                }
              
                if(_geActor != null)
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
            ActorData.ActionData.ActionName = Global.Settings.townPlayerRun ?
                ActorData.AniNames[(int)ActorActionType.AT_RUN] :
                ActorData.AniNames[(int)ActorActionType.AT_WALK];
            ActorData.ActionData.ActionSpeed = Global.Settings.townActionSpeed;
        }
        #endregion
    }
}
