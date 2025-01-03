using System;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class BeGrabManager
{
    protected List<BeEntity> grabbedEntityList = new List<BeEntity>();
    public List<BeActor> grabbedActorList = new List<BeActor>();
    public bool IsSuplexGrab = false;   //是否背摔抓取

    public BeActor owner { get; protected set; }
    protected BeActor grabber = null;
    protected BDGrabData grabData;
    protected int graberBuffInfoIdToSelf = 0;   //抓取判定到目标给自己添加的BuffInfoId

    public bool grabPos = false;

    public VInt3 absorbTargetPos = VInt3.zero;
    public bool isAbsorb = false;
    public VInt3 absorbPos = VInt3.zero;
    public VInt absorbLen = VInt.zero;
    public VInt absorbSpeed;

    Queue<VInt3> targetPosList = new Queue<VInt3>();
    VInt3 grabberStartPos;
    public bool useTargetPosList;

    public BeGrabManager(BeActor actor)
    {
        owner = actor;
    }

    public void Update(int deltaTime)
    {
        if (isAbsorb)
        {
            UpdateAbsorb(deltaTime);
        }
        else
        {
            UpdateGrab();
        }
    }

    public void UpdateTargetPos()
    {
        if (useTargetPosList)
        {
            if (targetPosList.Count > 0)
            {
                var pos = targetPosList.Dequeue();
                pos.x *= owner._getFaceCoff();
                pos += grabberStartPos;
                for (int i = 0; i < grabbedEntityList.Count; i++)
                {
                    grabbedEntityList[i].SetPosition(pos, false, false);
                }
            }
        }
    }

    public void SetGrabInfo(BeActor grabber, BDGrabData grabData)
	{
		this.grabber = grabber;
		this.grabData = grabData;
	}

	public BeActor GetGrabber()
	{
		return grabber;
	}

	public BDGrabData GetGrabData()
	{
		return grabData;
	}

    public void TryReleaseGrabbedEntity()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeGrabManager.TryReleaseGrabbedEntity"))
        {
#endif
            if (owner.GetStateGraph().CurrentStateHasExTag((int)DSFEntityStateTag.GRAPRELEASE))
            {
                EndGrab();
            }
#if ENABLE_PROFILER
        }
#endif
    }

    private bool CanGrab(BeEntity pkEntity)
    {
        if (pkEntity == null)
            return false;

        if (pkEntity.IsGrabed())
            return false;

        if (owner.m_cpkCurEntityActionInfo == null)
            return false;

        if (owner.m_cpkCurEntityActionFrameData == null)
            return false;

        if (owner.stateController == null)
            return false;

        if (grabbedEntityList == null)
            return false;

        if (owner.m_cpkCurEntityActionFrameData.kFlag == null)
            return false;

        if (owner.m_cpkCurEntityActionInfo.grabData == null)
            return false;

        return true;
    }

    public bool CheckGrab(BeEntity pkEntity)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeGrabManager.CheckGrab"))
        {
#endif
            if (!CanGrab(pkEntity))
                return false;

            if (owner.m_cpkCurEntityActionFrameData.kFlag.HasFlag((int)DSFGrapOp.GRAP_JUDGE) ||
                owner.m_cpkCurEntityActionFrameData.kFlag.HasFlag((int)DSFGrapOp.GRAP_JUDGE_SKIP_PHASE) ||
                owner.m_cpkCurEntityActionFrameData.kFlag.HasFlag((int)DSFGrapOp.GRAP_JUDGE_EXECUTE) ||
                owner.stateController.CanGrab())
            {

                if (IsGrabbed(pkEntity) || grabbedEntityList.Count >= owner.m_cpkCurEntityActionInfo.grabData.grabNum)
                    return true;
                //如果该抓取技能不能抓取霸体玩家
                BeActor actor = pkEntity as BeActor;
                if (actor != null)
                {
                    var eventData = actor.TriggerEventNew(BeEventType.OnJudgeGrab, new EventParam() { m_Bool = true, m_Int = owner.m_cpkCurEntityActionInfo.skillID });
                    if (!eventData.m_Bool)
                        return true;

                    if (owner.m_cpkCurEntityActionInfo.grabData.notGrabBati && actor.buffController.HaveBatiBuff())
                        return true;
                    int disX = actor.GetPosition().x - owner.GetPosition().x;
                    bool geDangFlag = (disX >= 0 && actor.GetFace()) || (disX <= 0 && !actor.GetFace());
                    if (owner.m_cpkCurEntityActionInfo.grabData.notGrabGeDang && actor.buffController.HasBuffByID((int)GlobalBuff.GEDANG) != null && geDangFlag)
                        return true;
                }

                var sc = pkEntity.stateController;
                pkEntity.IsBeGrabbed = true;
                if (sc != null && !sc.CanBeGrab())
                    return true;

                grabbedEntityList.Add(pkEntity);

                if (actor != null && !grabbedActorList.Contains(actor))
                {
                    if (owner.m_cpkCurEntityActionInfo.grabData.buffInfoIDToOther != 0 && actor.buffController != null)
                    {
                        actor.buffController.TryAddBuff(owner.m_cpkCurEntityActionInfo.grabData.buffInfoIDToOther);
                    }
                    grabbedActorList.Add(actor);
                    owner.TriggerEventNew(BeEventType.OnGrab, new EventParam() { m_Obj = actor });

                    //SetGrabPositionInfo(owner.GetPosition(), Global.Settings.recordPositionList);
                    //actor.grabController.useTargetPosList = true;
                }

                if (sc != null)
                {
                    sc.SetGrabState(GrabState.WILL_BEGRAB);
                }

                //抓取判定到目标以后给自己添加一个无敌BuffInfoId
                if (owner.m_cpkCurEntityActionInfo.grabData.buffInfoIdToSelf != 0 && grabbedActorList.Count > 0 && owner.buffController != null)
                {
                    graberBuffInfoIdToSelf = owner.m_cpkCurEntityActionInfo.grabData.buffInfoIdToSelf;
                    owner.buffController.TryAddBuff(graberBuffInfoIdToSelf);
                }

                if (owner.m_cpkCurEntityActionFrameData.kFlag.HasFlag((int)DSFGrapOp.GRAP_JUDGE_EXECUTE) || owner.stateController.CanGrab())
                {
                    ExecuteGrab();
                }
                else if (owner.m_cpkCurEntityActionFrameData.kFlag.HasFlag((int)DSFGrapOp.GRAP_JUDGE_SKIP_PHASE) && owner.GetStateGraph() != null)
                {
                    (owner.GetStateGraph() as BeActorStateGraph).ExecuteNextPhaseSkill();
                }

                return true;
            }

            return false;
#if ENABLE_PROFILER
        }
#endif
    }

    public void ExecuteGrab()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeGrabManager.ExecuteGrab"))
        {
#endif
            VInt3 pos;
            BDGrabData grabData = owner.m_cpkCurEntityActionInfo.grabData;

            owner.stateController.SetGrabState(GrabState.GRABBING);
            for (int i = 0; i < grabbedEntityList.Count; ++i)
            {
                var entity = grabbedEntityList[i] as BeActor;

                if (entity == null || (entity.IsDead() && !entity.IsBoss() && entity.deadType != DeadType.NORMAL))
                {
                    continue;
                }

                //抓取时添加BuffInfo
                if (grabData.buffInfoId != 0)
                {
                    entity.buffController.TryAddBuff(grabData.buffInfoId);
                }

                entity.grabController.isAbsorb = false;

                if (grabData.grabMoveSpeed > 0)
                {
                    isAbsorb = true;
                    absorbSpeed = new VInt(grabData.grabMoveSpeed);
                    pos = owner.GetPosition();
                    pos.x += VInt.Float2VIntValue(grabData.posx) * owner._getFaceCoff();
                    absorbTargetPos = pos;
                }
                else
                {
                    entity.grabController.SetGrabInfo(owner, grabData);
                    entity.RecordGrabPosition();
                }

                if (!IsSuplexGrab)
                {
                    entity.SetFace(entity.GetPosition().x > owner.GetPosition().x);
                }

                var grabEventData = owner.TriggerEventNew(BeEventType.onExcuteGrab, new EventParam { m_Int = owner.GetCurSkillID(), m_Obj = entity, m_Int2 = grabData.duraction });
                int grabTime = grabEventData.m_Int2;
                //被执行抓取
                entity.TriggerEventNew(BeEventType.onBeExcuteGrab, new EventParam { m_Obj = owner });

                entity.Locomote(new BeStateData((int)ActionState.AS_GRABBED) { _StateData = grabData.action, _timeout = grabTime, _timeoutForce = true });
                entity.RestoreWeight();

                entity.stateController.SetGrabState(GrabState.BEING_GRAB);

                if (grabData.quickPressDismis)
                {
                    entity.StartPressCount(BeActor.QuickPressType.GRAB, owner);
                }

                Logger.LogWarningFormat("execute grab target:{0}", entity.m_pkGeActor.GetResName());
            }
#if ENABLE_PROFILER
        }
#endif
    }

    public void EndGrab()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeGrabManager.EndGrab"))
        {
#endif
            owner.stateController.SetGrabState(GrabState.ENDGRABBING);
            if (grabbedEntityList.Count > 0)
            {
                for (int i = 0; i < grabbedEntityList.Count; ++i)
                {
                    BeEntity entity = grabbedEntityList[i];
                    BeActor actor = entity as BeActor;
                    if (actor != null)
                    {
                        actor.EndPressCount();
#if !LOGIC_SERVER
                        if (actor.m_pkGeActor != null)
                        {
                            var objActor = actor.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);
                            if (objActor != null)
                            {
                                objActor.transform.localRotation = UnityEngine.Quaternion.identity;
                            }
                        }
#endif
                    }
                    entity.JugePositionAfterGrab();
                }

                if (owner.m_cpkCurEntityActionInfo == null)
                {
                    //Logger.LogError("EndGrab : m_cpkCurEntityActionInfo is Null \n");

                    for (int j = 0; j < grabbedEntityList.Count; ++j)
                    {
                        var entity = grabbedEntityList[j];
                        //抓取结束 清除抓取数据
                        BeActor actor = entity as BeActor;
                        if (actor != null)
                        {
                            actor.grabController.SetGrabInfo(null, null);
                            actor.grabController.grabPos = false;
                        }

                        //抓取结束时 如果光明使者处于假死状态下则不切换到倒地状态
                        if (!CannotFallHaveBuff101(entity))
                        {
                            //entity.Locomote(new BeStateData((int)ActionState.AS_FALL, 0, 0, VInt.Float2VIntValue(1f), 0, 0, GlobalLogic.VALUE_300), true);
                            entity.Locomote(new BeStateData((int)ActionState.AS_FALL) { _StateData3 = VInt.Float2VIntValue(1f), _timeout = GlobalLogic.VALUE_300 }, true);
                        }
                    }

                    ClearGrabbedEntity();
                    return;
                }

                var grabData = owner.m_cpkCurEntityActionInfo.grabData;
                int tostate = grabData.endForceType == 1 ? (int)ActionState.AS_FALL : (int)ActionState.AS_HURT;
                int time = 0;

                float endForcey = 0.01f;
                for (int i = 0; i < grabbedEntityList.Count; ++i)
                {
                    var entity = grabbedEntityList[i] as BeActor;
                    if (entity == null || (/*entity.IsDead() && */!entity.IsBoss() && entity.deadType != DeadType.NORMAL))
                        continue;
                    //移除抓取时添加的Buff
                    if (grabData.buffInfoId != 0)
                        entity.buffController.RemoveBuffByBuffInfoID(grabData.buffInfoId);

                    endForcey = grabData.endForcey;
                    if (entity.GetPosition().z > 0)
                        tostate = (int)ActionState.AS_FALL;
                    if (tostate == (int)ActionState.AS_FALL && grabData.endForcey <= 0)
                        endForcey = 0.01f;

                    //阵鬼鬼影鞭或者身上带有Buff75的话 抓取释放后不进入倒地
                    if (owner.GetCurSkillID() == 1818 || owner.buffController.HasBuffByID(75) != null)
                    {
                        if (tostate == (int)ActionState.AS_FALL)
                            tostate = (int)ActionState.AS_HURT;
                    }

                    if (tostate == (int)ActionState.AS_HURT)
                        time = IntMath.Float2Int(owner.m_cpkCurEntityActionInfo.hurtTime, GlobalLogic.VALUE_1000);

                    //抓取结束 清除抓取数据
                    entity.grabController.SetGrabInfo(null, null);

                    //抓取结束时 如果光明使者处于假死状态下则不切换到倒地状态
                    if (!CannotFallHaveBuff101(entity))
                    {
                        owner.TriggerEventNew(BeEventType.onEndGrab, new EventParam { m_Obj = entity, m_Int = owner.GetCurSkillID() });

                        int faceCoff = owner._getFaceCoff();
                        if (grabData.hitSpreadOut)
                        {
                            if (entity.GetPosition().x > owner.GetPosition().x)
                                faceCoff = 1;
                            else
                                faceCoff = -1;
                        }
                        
                        entity.Locomote(new BeStateData(tostate)
                        {
                            _StateData2 = VInt.Float2VIntValue(grabData.endForcex * faceCoff),
                            _StateData3 = VInt.Float2VIntValue(endForcey),
                            _timeout = time
                        }, true);
                    }

                    entity.grabController.useTargetPosList = false;

                    //	Logger.LogErrorFormat("end grab entity:{0}", entity.m_pkGeActor.GetResName());
                }

                //移除抓取判定时给自己添加的BuffInfo
                if (graberBuffInfoIdToSelf != 0 && owner.buffController != null)
                {
                    owner.buffController.RemoveBuffByBuffInfoID(graberBuffInfoIdToSelf);
                    graberBuffInfoIdToSelf = 0;
                }
                
                ClearGrabbedEntity();

                useTargetPosList = false;
                targetPosList.Clear();
            }
#if ENABLE_PROFILER
        }
#endif
    }

    void UpdateGrab()
    {
        // if (useTargetPosList)
        // {

        // }
        // else if (grabber != null && !grabber.grabController.IsSuplexGrab)
        if (grabber != null && !grabber.grabController.IsSuplexGrab)
        {
            var pos = grabber.GetPosition();

            if (!grabber.grabController.grabPos && !grabData.notUseGrabSetPos && !owner.IsDead())
            {
                VInt3 targetPos = new VInt3(pos.x + VInt.Float2VIntValue(grabData.posx * grabber._getFaceCoff()), pos.y, VInt.Float2VIntValue(grabData.posy));
                if (grabData.notGrabToBlock && owner.CurrentBeScene != null && owner.CurrentBeScene.IsInBlockPlayer(targetPos))
                {

                }
                else
                {
                    owner.SetPosition(targetPos, false, false);
                }
            }
        }

        if (grabber == null ||
            grabber != null && (grabber.IsDead() || grabber.sgGetCurrentState() != (int)ActionState.AS_CASTSKILL))
        {
            //大于3秒才结束   
            if (owner.GetStateGraph().GetCurrentStatesTime() > 3000)
            {
                owner.GetStateGraph().SetCurrentStatesTimeout(0);
            }
        }
    }

    public void SetAbsorbInfo(VInt absorbSpeed, VInt3 absorbTargetPos, bool isAbsorb)
    {
        this.isAbsorb = isAbsorb;
        this.absorbSpeed = absorbSpeed;
        this.absorbTargetPos = absorbTargetPos;
    }

    public void StartAbsorb()
    {
        if (!isAbsorb)
        {
            return;
        }

        var pos = owner.GetPosition();
        VInt2 del = new VInt2(absorbTargetPos.x - pos.x, absorbTargetPos.y - pos.y);
        absorbPos = pos;
        VInt2 srcDel = new VInt2(absorbPos.x - absorbTargetPos.x, absorbPos.y - absorbTargetPos.y);
        absorbLen = srcDel.magnitude;
        VInt2 vec = del.NormalizeTo(absorbSpeed.i);
        owner.SetMoveSpeedX(vec.x);
        owner.SetMoveSpeedY(vec.y);
    }

    void UpdateAbsorb(int deltaTime)
    {
        if (!isAbsorb)
        {
            return;
        }

        if (owner.moveXAcc == 0 && owner.moveYAcc == 0 && owner.moveXSpeed == 0 && owner.moveYSpeed == 0)
        {
            //大于3秒才结束 
            if (owner.GetStateGraph().GetCurrentStatesTime() > 3000)
            {
                owner.GetStateGraph().SetCurrentStatesTimeout(0);
            }
        }

        var pos = owner.GetPosition();
        VInt2 del = new VInt2(absorbPos.x - pos.x, absorbPos.y - pos.y);
        if (absorbLen <= del.magnitude)
        {
            isAbsorb = false;
            owner.ClearMoveSpeed();
        }
    }

    void SetGrabPositionInfo(VInt3 startPos, List<VInt3> posList)
    {
        if (targetPosList.Count == 0)
        {
            useTargetPosList = true;
            grabberStartPos = startPos;
            for (int i = 0; i < posList.Count; i++)
            {
                targetPosList.Enqueue(posList[i]);
            }
        }
    }

    public void ResetGrabPositionInfo()
    {
        targetPosList.Clear();
    }

    /// <summary>
    /// 获取被抓取的目标列表
    /// </summary>
    /// <param name="list"></param>
    public void GetGrabTargetList(List<BeActor> list)
    {
        list.Clear();
        if (grabbedEntityList == null)
            return;
        for (int i = 0; i < grabbedEntityList.Count; i++)
        {
            BeActor target = grabbedEntityList[i] as BeActor;
            if (target == null || target.IsDead())
                continue;
            list.Add(target);
        }
    }

    public bool HasGrabbedEntity()
    {
        return grabbedEntityList.Count > 0;
    }

    public void ClearGrabbedEntity()
    {
        for (int j = 0; j < grabbedEntityList.Count; ++j)
        {
            var entity = grabbedEntityList[j];
            if (entity != null)
            {
                entity.IsBeGrabbed = false;
            }
        }
        grabbedEntityList.Clear();
        grabbedActorList.Clear();
    }

    public bool IsGrabbed(BeEntity entity)
    {
        for (int i = 0; i < grabbedEntityList.Count; ++i)
        {
            if (grabbedEntityList[i] == entity)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 指定角色身上是否有圣骑士假死Buff
    /// </summary>
    /// <returns></returns>
    protected bool CannotFallHaveBuff101(BeEntity entity)
    {
        var actor = entity as BeActor;
        if (actor == null)
            return false;
        return actor.buffController.HasBuffByID(101) != null;
    }

}
