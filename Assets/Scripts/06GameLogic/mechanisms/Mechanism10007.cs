using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 跳至目标位置，然后切换技能阶段
/// </summary>
public class Mechanism10007 : BeMechanism
{
    public Mechanism10007(int mid, int lv) : base(mid, lv) { }

    string mWarningEffectPath;      //预警特效路径
    VInt mStartSpeedZ;              //向上的初始速度
    VInt mStartMoveSpeed;           //向前的初始速度
    VInt2 mRange;                   //目标判定范围
    int mTimeToExecute;             //离到达目标多长时间切阶段
    int mMoveDuration;              //整个移动需要的时间（计算出来的）
    int mDelay;                     //判定时间
    int mEffectDuration;            //预警特效持续时间
    VInt3 mTargetPos;               //目标位置（计算出来的）
    int mEffectInfoId;              //buff信息表ID

    public override void OnInit()
    {
        mWarningEffectPath = data.StringValueA[0];
        mStartSpeedZ = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        mStartMoveSpeed = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0], level), GlobalLogic.VALUE_1000);
        if (mStartMoveSpeed <= 0)
        {
            mStartMoveSpeed = VInt.one;
        }
        if (data.ValueCLength > 1)
        {
            mRange = new VInt2(
                VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[0], level), GlobalLogic.VALUE_1000).i,
                VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[1], level), GlobalLogic.VALUE_1000).i
            );
        }
        mTimeToExecute = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        mDelay = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        mEffectDuration = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        mEffectInfoId = TableManager.GetValueFromUnionCell(data.ValueG[0], level);
    }

    public override void OnStart()
    {
        if (owner == null)
            return;

        if (owner.IsDead())
            return;

        mMoveDuration = 0;
        mTargetPos = VInt3.zero;

        var target = owner.CurrentBeScene.FindNearestFacedTarget(owner, mRange);
        if (target != null)
        {
            mTargetPos = target.GetPosition();
#if !LOGIC_SERVER
            var pos = new Vec3(mTargetPos.fx, mTargetPos.fy, 0f);
            if (mEffectInfoId > 0)
            {
                owner.CurrentBeScene.currentGeScene.CreateEffect(mEffectInfoId, pos, false, mEffectDuration / 1000f);
            }
            else
            {
                owner.CurrentBeScene.currentGeScene.CreateEffect(mWarningEffectPath, mEffectDuration / 1000f, pos);
            }
#endif
        }
    }

    void StartJump()
    {
        if (mTargetPos != VInt3.zero)
        {
            var vec = mTargetPos - owner.GetPosition();
            mMoveDuration = 1000 * vec.magnitude / mStartMoveSpeed.i;

            AddParabolaTrail(owner, mMoveDuration, mStartSpeedZ, mTargetPos);

            handleA = owner.RegisterEventNew(BeEventType.onTouchGround, eventParam =>
            {
                owner.ClearMoveSpeed();
                owner.ResetWeight();
            });

            handleB = owner.RegisterEventNew(BeEventType.onHit, eventParam =>
            {
                owner.RemoveMechanism(this);
            });
        }
    }

    void AddParabolaTrail(BeEntity pkEntity, int duration, VInt moveSpeedZ, VInt3 targetPos)
    {
        if (pkEntity == null)
            return;

        if (duration == 0)
        {
            Logger.LogError("抛物线飞行时间不能为零！！！");
            return;
        }

        var startPos = pkEntity.GetPosition();
        var vec = targetPos - startPos;
        pkEntity.SetMoveSpeedX(vec.x * 1000 / duration);
        pkEntity.SetMoveSpeedY(vec.y * 1000 / duration);
        pkEntity.SetMoveSpeedZ(moveSpeedZ.i);
        int acc = 2 * (moveSpeedZ.i * duration / 1000 + startPos.z) * 1000 / duration * 1000 / duration;
        pkEntity.SetMoveSpeedZAcc(acc);
    }

    public override void OnUpdate(int deltaTime)
    {
        if (mDelay > 0)
        {
            mDelay -= deltaTime;
            if (mDelay <= 0)
            {
                StartJump();
            }
        }

        if (mMoveDuration > 0)
        {
            mMoveDuration -= deltaTime;
            if (mMoveDuration <= mTimeToExecute)
            {
                mMoveDuration = 0;
                if (owner.IsCastingSkill())
                {
                    (owner.GetStateGraph() as BeActorStateGraph).ExecuteNextPhaseSkill();
                }

                owner.RemoveMechanism(this);
            }
        }
    }

    public override void OnFinish()
    {
        owner.ResetWeight();
    }
}
