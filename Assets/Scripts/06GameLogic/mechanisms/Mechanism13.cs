using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 自爆
*/

public class Mechanism13 : BeMechanism {

	VInt distanceExplode = 0;

	BeActor mTarget;

	bool playExpdeaded = false;
    protected bool actorVisibleFlag = true;                //自爆时尸体是否隐藏（默认隐藏）
    bool needRemove = false;
    bool m_delayRemove = false;
    private DelayCallUnitHandle m_delayCalleerHandler;
    public Mechanism13(int mid, int lv):base(mid, lv){}

	public override void OnInit ()
	{
		distanceExplode = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level),GlobalLogic.VALUE_1000);
        if (data.ValueB.Count > 0)
        {
            int flag = TableManager.GetValueFromUnionCell(data.ValueB[0],level);
            actorVisibleFlag = flag == 0 ? true : false;
            needRemove = flag == 2;
            m_delayRemove = flag == 3;
        }
	}

	public override void OnStart ()
	{
		mTarget = _findActor();

		owner.aiManager.StopCurrentCommand();
		owner.aiManager.Stop();
		owner.SetForceRunMode(true);
		owner.ChangeRunMode(true);

		owner.delayCaller.DelayCall(100, ()=>{
			if(owner != null)
			{
				if (!owner.IsDead() && !owner.IsGrabed())
				{
					if (owner.IsGrabed())
					{
						owner.sgPushState(new BeStateData((int)ActionState.AS_RUN));
					}
					else
					{
						owner.Locomote(new BeStateData((int)ActionState.AS_RUN));
					}
				}
			}
		});

		//owner.buffController.RemoveBuff(1);
	}

	public override void OnUpdate (int deltaTime)
	{
		if (null != owner && !owner.IsDead())
		{
			if (null == mTarget)
			{
				var target = _findActor();
				if (null != target && target != mTarget)
				{
					mTarget = target;
				}
			}

			if (!owner.IsInPassiveState())
			{
				if (null != mTarget)
				{
					//owner.ResetMoveCmd();
					_moveTo(mTarget, deltaTime);

					if((mTarget.GetPosition() - owner.GetPosition()).magnitude <= distanceExplode.i)
					{
						_DoDead();
					}
					//if (Vec3.Distance(mTarget.GetPosition(), owner.GetPosition()) <= distanceExplode)
				}
			}
		}
	}

	public override void OnFinish ()
	{
		_DoDead();
        m_delayCalleerHandler.SetRemove(true);
    }


	private void _DoDead()
	{
		if (!owner.IsDead() && !playExpdeaded)
        {
            _tryDoEffect();

            if (needRemove)
            {
                _onSetDead();
            }
            else if(m_delayRemove)
            {
                if(owner.CurrentBeScene != null)
                    m_delayCalleerHandler = owner.CurrentBeScene.DelayCaller.DelayCall(0, _onSetDead);
            }
            else
            {
                owner.delayCaller.DelayCall(30, () =>
                {
					if(owner != null)
					{
						owner.DoDead();
					}
                });
            }
		}
	}
    private void _onSetDead()
    {
		if(owner == null)
		{
			return;
		}
        if (owner.m_iEntityLifeState != (int)EntityLifeState.ELS_CANREMOVE)
        {
            owner.SetIsDead(true);
            owner.OnDead();
            owner.SetLifeState(EntityLifeState.ELS_CANREMOVE);
        }
    }


	private void _tryDoEffect()
	{
		playExpdeaded = true;
        if (actorVisibleFlag)
        {
            owner.m_pkGeActor.SetActorVisible(false);
        }
        owner.PlayAction("ExpDead");
#if !LOGIC_SERVER
        if (owner.m_pkGeActor != null)
        {
          //  owner.m_pkGeActor.SetHPDamage(owner.GetEntityData().GetHP());
            owner.m_pkGeActor.RemoveHPBar();
        }
#endif 
	}

	private BeActor _findActor()
	{
		List<BeActor> list = GamePool.ListPool<BeActor>.Get();
		owner.CurrentBeScene.FindActorInRange(list, owner.GetPosition(), VInt.Float2VIntValue(15), 0);

		BeActor target = null;

		if (list.Count > 0)
		{
			target = list[0];
		}

		GamePool.ListPool<BeActor>.Release(list);

		return target;
	}

	private int mDealCount = 0;
	private VInt3 mPos;
	private void _moveTo(BeActor actor, int delta)
	{
		mDealCount += delta;
		if (mDealCount > 450)
		{
			mPos = actor.GetPosition() - owner.GetPosition();
			mDealCount = 0;
		}
		else 
			return;

		owner.ResetMoveCmd();

		if (mPos.x > VInt.half)
		{
			owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
		}
		else if (mPos.x < -VInt.half)
		{
			owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
		}

		if (mPos.y > VInt.half)
		{
			owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y, true);
		}
		else if (mPos.y < -VInt.half)
		{
			owner.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y_NEG, true);
		}
	}


}
