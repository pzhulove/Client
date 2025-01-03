using System;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class LogicTrail
{
    public VInt3 startPoint;
    public VInt3 endPoint;
    public VInt moveSpeed;
    public VInt3 moveDir;
    public VInt3 currentPos;

    public bool canRemove = false;

    protected int timeAcc = 0;

    public virtual void Init() { }

    public virtual void OnTick(int deltaTime) { }

    public virtual void Remove()
    {
        canRemove = true;
    }
}

public sealed class LinearLogicTrail : LogicTrail
{
    public override void Init()
    {
        currentPos = startPoint;
        moveDir = (endPoint - startPoint).NormalizeTo((int)IntMath.kIntDen);
        var v = (endPoint.vector3 - startPoint.vector3).normalized;
        timeAcc = 0;
    }

    public override void OnTick(int deltaTime)
    {
        timeAcc += deltaTime;
        VInt3 moveDist = moveDir * (VFactor.NewVFactor(deltaTime, (long)1000) * moveSpeed.factor);
        currentPos += moveDist;
    }
}

//双鹰回旋
public sealed class BoomerangTrail : LogicTrail
{
    public enum Phase
    {
        GO,
        STAY,
        BACK,
        END
    }

    Phase state;
    public int stayDuration = 2000;

    public int userData = 0;
    bool end = false;

    public BeActor target;
    public BeProjectile owner;

    public override void Init()
    {
        end = false;

        InitStat(Phase.GO);
    }

    public void InitStat(Phase state)
    {
        this.state = state;
        if (state == Phase.GO)
        {
            currentPos = startPoint;
            moveDir = (endPoint - startPoint).NormalizeTo((int)IntMath.kIntDen);
            moveDir.z = 0;
            moveDir = moveDir.NormalizeTo((int)IntMath.kIntDen);
        }
        else if (state == Phase.STAY)
        {
            timeAcc = 0;
        }
        else if (state == Phase.BACK)
        {
            startPoint = endPoint;

            if (target != null)
            {
                endPoint = target.GetPosition();
            }

            currentPos = startPoint;

            moveDir = (endPoint - startPoint).NormalizeTo((int)IntMath.kIntDen);
            moveDir.z = 0;
            moveDir = moveDir.NormalizeTo((int)IntMath.kIntDen);
        }
    }

    public override void OnTick(int deltaTime)
    {
        if (state == Phase.GO)
        {
            timeAcc += deltaTime;

            VInt3 moveDist = moveDir * (VFactor.NewVFactor(deltaTime, (long)1000) * moveSpeed.factor);

            currentPos += moveDist;

            if (ReachEndPoint())
            {
                InitStat(Phase.STAY);
            }
        }
        else if (state == Phase.STAY)
        {
            timeAcc += deltaTime;
            if (timeAcc >= stayDuration)
            {
                InitStat(Phase.BACK);
            }
        }
        else if (state == Phase.BACK)
        {
            timeAcc += deltaTime;
            VInt3 moveDist = moveDir * (VFactor.NewVFactor(deltaTime, (long)1000) * moveSpeed.factor);

            currentPos += moveDist;

            if (target != null && target.CurrentBeScene != null)
            {
                if (ReachTargetActorPos() && !end && target.CurrentBeScene.IsInLogicScene(owner.GetPosition()))
                {
                    end = true;
                    //target.TriggerEvent(BeEventType.onBoomerangHit, new object[] { owner });
                    target.TriggerEventNew(BeEventType.onBoomerangHit, new EventParam(){m_Obj = owner});
                }
            }
        }
    }

    public bool ReachEndPoint()
    {
        var dis = (currentPos - endPoint);
        int magnitude = dis.magnitude;

        return magnitude < new VInt(0.2f);
    }

    public bool ReachTargetActorPos()
    {
        var targetPos = target.GetPosition();
        return Math.Abs(targetPos.x - currentPos.x) < new VInt(0.2f);
    }
}

//引力类型 用于光炎剑
public sealed class GravitationTrail : LogicTrail
{
    public VInt3 speedVec = VInt3.zero;
    public int speed = 15;
    public int acc = 1200;

    public BeActor target;
    public BeProjectile owner;

    public override void Init()
    {
        speedVec = new VInt3(speed * (int)IntMath.fIntDen, 0, 0);
        speed = 7;
        currentPos = startPoint;
    }

    public override void OnTick(int deltaTime)
    {
        if (target != null)
        {
            VInt3 targetPos = target.GetPosition();
            targetPos.z = 0;
            VInt3 ownerPos = owner.GetPosition();
            ownerPos.z = 0;
            var accSpeed = (targetPos - ownerPos).NormalizeTo((int)IntMath.kIntDen) * new VFactor(acc, GlobalLogic.VALUE_1000);
            speedVec = (speedVec + accSpeed).NormalizeTo((int)IntMath.kIntDen) * speed;
            var pos = owner.GetPosition() + speedVec * new VFactor(deltaTime, GlobalLogic.VALUE_1000);
            currentPos = pos;
        }
    }
}

public sealed class FollowTargetTrail : LogicTrail
{
    public BeActor target;
    public BeProjectile owner;
    public bool nearRemove = false; //靠近目标移除实体

    public override void Init()
    {
        base.Init();
        currentPos = startPoint;
    }

    public override void OnTick(int deltaTime)
    {
        base.OnTick(deltaTime);
        if (owner.IsDead())
        {
            return;
        }

        VInt3 targetPos = target.GetPosition();
        targetPos.z = 0;
        VInt3 ownerPos = owner.GetPosition();
        ownerPos.z = 0;

        if (CheckNearNotMove((targetPos - ownerPos).magnitude))
        {
            owner.DoDie();
        }

        VInt3 dis = targetPos - ownerPos;
        var del = dis.NormalizeTo(moveSpeed.i);
        currentPos = owner.GetPosition() + del;
    }

    private bool CheckNearNotMove(int dis)
    {
        if (!nearRemove)
            return false;
        if (dis <= 10000)
            return true;
        return false;
    }
}

/// <summary>
/// 固定移动时间，移动到指定对象位置
/// 支持偏移高度
/// </summary>
public sealed class FixedTimeFollowTargetTrail : LogicTrail
{
    public BeActor target;
    public BeProjectile owner;
    public int moveTime = 0;
    private int curTime = 0;
    public VInt offsetHeight = 0;

    public override void Init()
    {
        base.Init();
        currentPos = startPoint;
        curTime = 0;
    }

    public override void OnTick(int deltaTime)
    {
        if (owner.IsDead())
        {
            return;
        }

        VInt3 targetPos = target.GetPosition() + new VInt3(0, 0, offsetHeight.i);

        curTime += deltaTime;
        VInt3 toPos = VInt3.Lerp(startPoint, targetPos, VFactor.NewVFactor(curTime * GlobalLogic.VALUE_1000 / moveTime, GlobalLogic.VALUE_1000));
        currentPos = toPos;

        if (curTime >= moveTime)
        {
            owner.DoDie();
        }
    }
}


/// <summary>
/// 加速移动，移动到指定对象位置
/// 支持偏移高度
/// </summary>
public sealed class AccelerateFollowTargetTrail : LogicTrail
{
    public BeActor target;
    public BeProjectile owner;
    public VInt offsetHeight = 0;
    public VInt m_InitSpeed;        // 初始速度
    public VInt m_AccSpeed;         // 加速度
    private VInt m_CurSpeed;        // 当前速度

    public override void Init()
    {
        base.Init();
        currentPos = startPoint;
        m_CurSpeed = m_InitSpeed;
    }

    public override void OnTick(int deltaTime)
    {
        if (owner.IsDead())
            return;

        VInt3 targetPos = target.GetPosition() + new VInt3(0, 0, offsetHeight.i);
        var deltaPos = targetPos - currentPos;
        if (CheckNearNotMove(deltaPos.magnitude))
        {
            currentPos = targetPos;
            owner.SetPosition(currentPos);
            owner.DoDie();
            return;
        }

        VInt3 moveDir = deltaPos.NormalizeTo(m_CurSpeed.i);
        m_CurSpeed += m_AccSpeed;

        if (moveDir.magnitude > deltaPos.magnitude)
        {
            moveDir = deltaPos;
        }

        currentPos += moveDir;
    }

    private bool CheckNearNotMove(int dis)
    {
        if (dis <= 5000)
            return true;
        return false;
    }
}

/// <summary>
/// 位置始终与目标一致
/// </summary>
public sealed class ChaseTargetTrail : LogicTrail
{
    public BeActor target;
    public VInt3 offset;

    public override void Init()
    {
        currentPos = startPoint;
    }

    public override void OnTick(int deltaTime)
    {
        if (target == null)
        {
            return;
        }

        var pos = target.GetPosition() + offset;
        currentPos.x = pos.x;
        currentPos.y = pos.y;
    }
}

/// <summary>
/// 旋转追踪目标位置
/// </summary>
public sealed class RotateChaseTargetTrail : LogicTrail
{
    public BeProjectile pkProjectile;
    public BeActor target;
    public int beginAngle;
    public int changeMaxAngle;
    public int moveSpeed;
    public int chaseTime;
    private int currAngle;
    private int passTime;

    public override void Init()
    {
        if (pkProjectile.GetFace())
            beginAngle = 1800000 - beginAngle;
        currentPos = startPoint;
        currAngle = beginAngle;
        passTime = 0;
        VFactor radian = VFactor.pi * beginAngle / 180 / IntMath.kIntDen;
        var sx = moveSpeed * IntMath.cos(radian.nom, radian.den);
        var sy = moveSpeed * IntMath.sin(radian.nom, radian.den);
        moveDir = new VInt3(sx, sy,0);
    }

    private long CrossZValue(VInt3 lhs, VInt3 rhs)
    {
        long Lx = lhs.x;
        long Ly = lhs.y;
        long Rx = rhs.x;
        long Ry = rhs.y;
        return (Lx * Ry) - (Ly * Rx);
    }

    public override void OnTick(int deltaTime)
    {
        VInt3 moveDist = moveDir * (VFactor.NewVFactor(deltaTime, (long)1000));
        passTime += deltaTime;
        if (passTime < chaseTime && target != null)
        {
            var deltaPos = target.GetPosition() - currentPos;
            int isAddAngle = 0;
            int tempCurrAngle = currAngle;
            if (CrossZValue(moveDir, deltaPos) > 0)
            {
                tempCurrAngle += changeMaxAngle;
                isAddAngle = 1;
            }
            else if (CrossZValue(moveDir, deltaPos) < 0)
            {
                tempCurrAngle -= changeMaxAngle;
                isAddAngle = -1;
            }
            var radian = VFactor.pi * tempCurrAngle / 180 / IntMath.kIntDen;
            var sx = moveSpeed * IntMath.cos(radian.nom, radian.den);
            var sy = moveSpeed * IntMath.sin(radian.nom, radian.den);
            var moveDirTemp = new VInt3(sx, sy,0);
            var deltaPosTemp = target.GetPosition() - currentPos + moveDist;
            if (CrossZValue(moveDirTemp, deltaPosTemp) > 0 && isAddAngle == 1)
            {
                moveDir = moveDirTemp;
                currAngle = tempCurrAngle;
            }
            else if (CrossZValue(moveDirTemp, deltaPosTemp) < 0 && isAddAngle == -1)
            {
                moveDir = moveDirTemp;
                currAngle = tempCurrAngle;
            }
        }

        if (pkProjectile.isAngleWithEffect)
        {
            pkProjectile.SetMoveSpeedXLocal(moveDir.x);
            pkProjectile.SetMoveSpeedY(moveDir.y);
            pkProjectile.InitLocalRotation();
        }

        currentPos += moveDist;
    }
}

/// <summary>
/// 反弹型实体
/// </summary>
public sealed class ReboundTrail : LogicTrail
{
    public BeProjectile pkProjectile;
    public int maxReboundCount;
    public int beginAngle;
    public int moveSpeed;
    private int currReboundCount;

    public override void Init()
    {
        currentPos = startPoint;
        VFactor radian = VFactor.pi * beginAngle / 180 / IntMath.kIntDen;
        var sx = moveSpeed * IntMath.cos(radian.nom, radian.den);
        var sy = moveSpeed * IntMath.sin(radian.nom, radian.den);
        moveDir = new VInt3(sx, sy,0);
        currReboundCount = maxReboundCount;
    }

    public override void OnTick(int deltaTime)
    {
        
        if (pkProjectile.isAngleWithEffect)
        {
            pkProjectile.SetMoveSpeedXLocal(moveDir.x);
            pkProjectile.SetMoveSpeedY(moveDir.y);
            pkProjectile.InitLocalRotation();
        }
        
        var tempCurrentPos = currentPos + moveDir * (VFactor.NewVFactor(deltaTime, (long)1000));
        if (!pkProjectile.CurrentBeScene.IsInBlockPlayer(tempCurrentPos))
        {
            currentPos = tempCurrentPos;
        }
        else
        {
            var moveDirTemp = new VInt3(moveDir.x, -moveDir.y,0);
            tempCurrentPos = currentPos + moveDirTemp * (VFactor.NewVFactor(deltaTime, (long)1000));
            if (!pkProjectile.CurrentBeScene.IsInBlockPlayer(tempCurrentPos))
            {
                moveDir = moveDirTemp;
                currentPos = tempCurrentPos;
                currReboundCount--;
            }
            else
            {
                moveDirTemp = new VInt3(-moveDir.x, moveDir.y,0);
                tempCurrentPos = currentPos + moveDirTemp * (VFactor.NewVFactor(deltaTime, (long)1000));
                if (!pkProjectile.CurrentBeScene.IsInBlockPlayer(tempCurrentPos))
                {
                    moveDir = moveDirTemp;
                    currentPos = tempCurrentPos;
                    currReboundCount--;
                }
            }
        }

        if (currReboundCount <= 0)
        {
            pkProjectile.DoDie();
        }
    }
}