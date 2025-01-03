using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buff800002 : Buff800001
{
    public Buff800002(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
    {
    }
}

public class Buff800001 : BeBuff
{
    private VInt mRadius = VInt.one;
    private int mEffectID = -1;
    private int mCamp = -1;

    private BeActor mTarget;

    public Buff800001(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
    {
    }

    public override void OnReset()
    {
        mRadius = VInt.one;
        mEffectID = -1;
        mCamp = -1;
        mDealCount = 0;
        mPos = VInt3.zero;
    }

    public override void OnInit()
    {
        base.OnInit();

        if (buffData.ValueA.Count >= 1)
        {
            mEffectID = TableManager.GetValueFromUnionCell(buffData.ValueA[0], 1);
        }

        if (buffData.ValueB.Count >= 1)
        {
            mRadius = TableManager.GetValueFromUnionCell(buffData.ValueB[0], 1);
        }

        if (buffData.ValueC.Count >= 1)
        {
            mCamp = TableManager.GetValueFromUnionCell(buffData.ValueC[0], 1);
        }
    }

    public override void OnStart()
    {
        base.OnStart();

        mTarget = _findActor();

        owner.ChangeRunMode(true);
        owner.aiManager.Stop();
        //owner.aiManager.keepDistance = 1;
        //owner.aiManager.followTarget = mTarget;
        //owner.aiManager.forceFollow = true;

        //owner.SetMoveSpeedXRate(1.5f);
        //owner.SetMoveSpeedYRate(1.5f);
    }

    private void _tryDoEffect()
    {
        owner.m_pkGeActor.SetActorVisible(false);
        owner.PlayAction("ExpDead");
        //owner.sgSetCurrentStatesTimeout(30);

        //if (mEffectID > 0)
        //{
        //    var list = owner.CurrentBeScene.FindActorInRange(owner.GetPostion(), mRadius, mCamp);
        //    for (int i = 0; i < list.Count; ++i)
        //    {
        //        owner.DoAttackTo(list[i], mEffectID);
        //    }
        //}
    }

    private BeActor _findActor()
    {
		List<BeActor> list = GamePool.ListPool<BeActor>.Get();
		
		owner.CurrentBeScene.FindActorInRange(list, owner.GetPosition(), 15 * (int)IntMath.kIntDen, mCamp);

		BeActor target = null;

        if (list.Count > 0)
        {
			target = list[0];
        }

		GamePool.ListPool<BeActor>.Release(list);

        return target;
    }

    public override void OnFinish()
    {
        if (!owner.IsDead())
        {
            _tryDoEffect();

            owner.delayCaller.DelayCall(30, () =>
            {
                owner.DoDead(true);
            });

            //owner.SetMoveSpeedXRate(1f);
            //owner.SetMoveSpeedYRate(1f);
        }
    }

    private int mDealCount = 0;
    private VInt3 mPos;

    private static readonly VInt dis01 = VInt.Float2VIntValue(0.1f);
    private void _moveTo(BeActor actor, int delta)
    {
        mDealCount += delta;
        if (mDealCount > 450)
        {
            mPos = actor.GetPosition() - owner.GetPosition();
            mDealCount = 0;
        }

        owner.ResetMoveCmd();
        

        if (mPos.x > dis01)
        {
            owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
        }
        else if (mPos.x < -dis01)
        {
            owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
        }

        if (mPos.y > dis01)
        {
            owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y, true);
        }
        else if (mPos.y < -dis01)
        {
            owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y_NEG, true);
        }
    }

    public override void OnUpdate(int delta)
    {
        base.OnUpdate(delta);

        if (null != owner && !owner.IsDead())
        //if (null != owner && !owner.IsDead() && owner.sgGetCurrentState() == (int)ActionState.AS_WALK)
        {
            if (null == mTarget)
            {
                var target = _findActor();
                if (null != target && target != mTarget)
                {
                    mTarget = target;
                }
            }

            if (null != mTarget)
            {
                _moveTo(mTarget, delta);

                //if (Vec3.Distance(mTarget.GetPostion(), owner.GetPostion()) <= 0.5f)
                //{
                //    Finish();
                //}
            }
        }
    }
}
