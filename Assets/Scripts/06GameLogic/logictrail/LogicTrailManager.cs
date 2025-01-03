using System;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class LogicTrailManager
{
    List<LogicTrail> trailList = new List<LogicTrail>();

    public void AddLogicTrail(LogicTrail trail)
    {
        if (trail != null)
        {
            trailList.Add(trail);
        }
    }

    public LogicTrail AddLinearLogicTrial(BeEntity pkEntity, BeProjectile pkProjectile, VInt2 range, VInt moveSpeed)
    {
        if (pkEntity == null)
            return null;

        if (pkProjectile == null)
            return null;

        BeScene mainLogic = pkEntity.CurrentBeScene;
        if (null != mainLogic)
        {
            BeActor attacker = GetProjectileOwner(pkEntity);
            BeActor target = mainLogic.FindNearestFacedTarget(attacker, range);
            if (target != null /*&& TMath.Abs(target.GetPosition().x - pkEntity.GetPosition().x) > VInt.onehalf*/)
            {
                var trail = new LinearLogicTrail();
                trail.startPoint = pkProjectile.GetPosition();
                var targetPos = target.GetPosition();
                targetPos.z = trail.startPoint.z;
                trail.endPoint = targetPos;

                pkProjectile.SetFace(trail.endPoint.x < trail.startPoint.x);

                if (Math.Abs(trail.endPoint.y - trail.startPoint.y) <= VInt.zeroDotOne)
                {
                    trail.endPoint.y = trail.startPoint.y;
                }

                trail.moveSpeed = moveSpeed;

                trail.Init();

                trailList.Add(trail);

                return trail;
            }
        }
        return null;
    }

    public LogicTrail AddBoomerangLogicTrail(BeEntity pkEntity, BeProjectile pkProjectile, VInt2 emitposition, VInt2 range, int moveSpeed, int maxDistance, int stayDuration)
    {
        if (pkEntity == null)
            return null;

        BeActor attacker = GetProjectileOwner(pkEntity);
        if (attacker == null)
            return null;

        if (pkProjectile == null)
            return null;

        BeScene scene = pkEntity.CurrentBeScene;
        if (scene != null)
        {
            BeActor target = scene.FindNearestFacedTarget(attacker, range);

            VInt3 dir = new VInt3(pkEntity.GetFace() ? -1.0f : 1.0f, 0, 0).NormalizeTo((int)IntMath.kIntDen);
            var startPos = pkProjectile.GetPosition();
            var ownerStartPos = attacker.GetPosition();
            ownerStartPos.z = startPos.z;

            if (target != null && Math.Abs(target.GetPosition().x - pkEntity.GetPosition().x) > new VInt(1.5f))
            {
                var targetPos = target.GetPosition();
                targetPos.z = startPos.z;

                dir = (targetPos - ownerStartPos).NormalizeTo((int)IntMath.kIntDen);
            }

            var trail = new BoomerangTrail();
            trail.startPoint = startPos;
            trail.stayDuration = stayDuration;

            /*int[] stayDurationArray = new int[1];
            stayDurationArray[0] = trail.stayDuration;
            pkEntity.TriggerEvent(BeEventType.onChangeBoomerangStayDuration, new object[] { attacker.GetCurSkillID(), stayDurationArray });
            trail.stayDuration = stayDurationArray[0];*/
            
            var eventData = pkEntity.TriggerEventNew(BeEventType.onChangeBoomerangStayDuration, new EventParam(){m_Int = attacker.GetCurSkillID(), m_Int2 = trail.stayDuration});
            trail.stayDuration = eventData.m_Int2;

            trail.endPoint = dir * new VInt(maxDistance).factor + ownerStartPos;
            trail.endPoint.y += emitposition.y * 2;
            trail.endPoint.x += emitposition.x;

            trail.moveSpeed = moveSpeed;

            trail.target = attacker;
            trail.owner = pkProjectile;

            trail.Init();

            trailList.Add(trail);

            return trail;
        }

        return null;
    }

    public LogicTrail AddGravitationTrail(BeEntity pkEntity, BeProjectile pkProjectile, VInt speed)
    {
        if (pkEntity == null)
            return null;

        BeActor attacker = GetProjectileOwner(pkEntity);
        if (attacker == null)
            return null;

        if (pkProjectile == null)
            return null;

        BeScene scene = pkEntity.CurrentBeScene;
        if (scene != null)
        {
            BeActor target = scene.FindNearestRangeTarget(attacker.GetPosition(), attacker, GlobalLogic.VALUE_100 * GlobalLogic.VALUE_10000);
            if (target != null)
            {
                var trail = new GravitationTrail();
                trail.startPoint = pkProjectile.GetPosition();
                pkProjectile.ClearMoveSpeed();
                var targetPos = target.GetPosition();
                targetPos.z = trail.startPoint.z;
                trail.endPoint = targetPos;
                //trail.speed = speed.i;
                if (attacker.GetFace())
                {
                    trail.speed = -trail.speed;
                }
                trail.target = target;
                trail.owner = pkProjectile;
                trail.Init();

                trailList.Add(trail);

                return trail;
            }
        }

        return null;
    }

    public LogicTrail AddFollowTargetTrail(BeEntity pkEntity, BeProjectile pkProjectile, VInt speed, bool nearRemove = false)
    {
        if (pkEntity == null)
            return null;
        
        BeActor attacker = GetProjectileOwner(pkEntity);
        if (attacker == null)
            return null;

        if (pkProjectile == null)
            return null;

        BeScene scene = pkEntity.CurrentBeScene;
        if (scene != null)
        {
            BeActor target = scene.FindNearestRangeTarget(attacker.GetPosition(), attacker, GlobalLogic.VALUE_100 * GlobalLogic.VALUE_10000);
            if (target != null)
            {
                var trail = new FollowTargetTrail();
                trail.startPoint = pkProjectile.GetPosition();
                pkProjectile.ClearMoveSpeed();
                var targetPos = target.GetPosition();
                targetPos.z = trail.startPoint.z;
                trail.endPoint = targetPos;
                trail.moveSpeed = speed;
                trail.nearRemove = nearRemove;
                trail.owner = pkProjectile;
                trail.target = target;
                trail.Init();

                trailList.Add(trail);

                return trail;
            }
        }

        return null;
    }

    public LogicTrail AddFixedTimeFollowTargetTrail(BeEntity pkEntity, BeProjectile pkProjectile, int moveTime, VInt offsetHeight)
    {
        if (pkEntity == null)
            return null;

        BeActor attacker = GetProjectileOwner(pkEntity);
        if (attacker == null)
            return null;

        if (pkProjectile == null)
            return null;

        pkProjectile.ClearMoveSpeed();

        var trail = new FixedTimeFollowTargetTrail();
        trail.startPoint = pkProjectile.GetPosition();
        trail.endPoint = attacker.GetPosition() + new VInt3(0, 0, offsetHeight.i);
        trail.moveTime = moveTime;
        trail.offsetHeight = offsetHeight;
        trail.owner = pkProjectile;
        trail.target = attacker;
        trail.Init();

        trailList.Add(trail);

        return trail;
    }

    public LogicTrail AddAccelerateFollowTargetTrail(BeEntity pkEntity, BeProjectile pkProjectile, BeActor target, VInt chaserAccSpeed)
    {
        if (pkEntity == null)
            return null;

        BeActor attacker = GetProjectileOwner(pkEntity);
        if (attacker == null)
            return null;

        if (pkProjectile == null)
            return null;

        pkProjectile.ClearMoveSpeed();

        var trail = new AccelerateFollowTargetTrail();
        trail.startPoint = pkProjectile.GetPosition();
        trail.m_AccSpeed = chaserAccSpeed;
        if (target.attribute != null)
        {
            trail.offsetHeight = target.attribute.height / 2;
        }
        trail.Init();
        trail.owner = pkProjectile;
        trail.target = target;

        trailList.Add(trail);

        return trail;
    }

    public LogicTrail  AddChaseTargetTrail(BeEntity pkEntity, BeProjectile pkProjectile, VInt3 offset)
    {
        if (pkEntity == null)
            return null;

        BeActor attacker = GetProjectileOwner(pkEntity);
        if (attacker == null)
            return null;

        if (pkProjectile == null)
            return null;

        pkProjectile.ClearMoveSpeed();

        BeScene scene = pkEntity.CurrentBeScene;
        if (scene != null)
        {
            BeActor target = scene.FindNearestRangeTarget(attacker.GetPosition(), attacker, GlobalLogic.VALUE_100 * GlobalLogic.VALUE_10000);
            if (target != null)
            {
                var trail = new ChaseTargetTrail()
                {
                    startPoint = pkProjectile.GetPosition(),
                    target = target,
                };

                trail.Init();

                trailList.Add(trail);

                return trail;
            }
        }

        return null;
    }
    
    public LogicTrail  AddRotateChaseTargetTrail(BeEntity pkEntity, BeProjectile pkProjectile, int beginAngle, int changeMaxAngle, int moveSpeed, int chaseTime)
    {
        if (pkEntity == null)
            return null;

        BeActor attacker = GetProjectileOwner(pkEntity);
        if (attacker == null)
            return null;

        if (pkProjectile == null)
            return null;

        pkProjectile.ClearMoveSpeed();

        BeScene scene = pkEntity.CurrentBeScene;
        if (scene != null)
        {
            BeActor target = scene.FindNearestRangeTarget(attacker.GetPosition(), attacker, GlobalLogic.VALUE_100 * GlobalLogic.VALUE_10000);
            if (target != null)
            {
                var trail = new RotateChaseTargetTrail()
                {
                    pkProjectile = pkProjectile,
                    startPoint = pkProjectile.GetPosition(),
                    target = target,
                    beginAngle = beginAngle, 
                    changeMaxAngle = changeMaxAngle, 
                    moveSpeed = moveSpeed, 
                    chaseTime = chaseTime,
                };

                trail.Init();
                trailList.Add(trail);

                return trail;
            }
        }

        return null;
    }
    
    public LogicTrail  AddReboundTrail(BeEntity pkEntity, BeProjectile pkProjectile, int beginAngle, int moveSpeed, int maxReboundCount)
    {
        if (pkEntity == null)
            return null;

        BeActor attacker = GetProjectileOwner(pkEntity);
        if (attacker == null)
            return null;

        if (pkProjectile == null)
            return null;

        pkProjectile.ClearMoveSpeed();

        BeScene scene = pkEntity.CurrentBeScene;
        if (scene != null)
        {
            BeActor target = scene.FindNearestRangeTarget(attacker.GetPosition(), attacker, GlobalLogic.VALUE_100 * GlobalLogic.VALUE_10000);
            if (target != null)
            {
                var trail = new ReboundTrail()
                {
                    pkProjectile = pkProjectile,
                    startPoint = pkProjectile.GetPosition(),
                    beginAngle = beginAngle,
                    moveSpeed = moveSpeed, 
                    maxReboundCount = maxReboundCount,
                };

                trail.Init();
                trailList.Add(trail);

                return trail;
            }
        }

        return null;
    }

    public void Update(int iDeltaTime)
    {
        bool isDirty = false;
        for (int i = 0; i < trailList.Count; i++)
        {
            if (CheckCanRemove(trailList[i]))
            {
                isDirty = true;
            }
            else
            {
                trailList[i].OnTick(iDeltaTime);
            }
        }
        if (isDirty)
        {
            trailList.RemoveAll(CheckCanRemove);
        }
    }

    public void ClearAll()
    {
        trailList.Clear();
    }

    private bool CheckCanRemove(LogicTrail t)
    {
        return t.canRemove;
    }

    protected BeActor GetProjectileOwner(BeEntity entity)
    {
        BeActor actor = entity as BeActor;
        if (actor != null)
            return actor;
        actor = entity.GetOwner() as BeActor;
        if (actor != null)
            return actor;
#if UNITY_EDITOR
        Logger.LogErrorFormat("没有找到Id:{0},实体的召唤者，策划请检查配置", entity.m_iResID);
#endif
        return null;
    }
}
