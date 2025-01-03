using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill5169 : BeSkill
{
    private enum eState
    {
        Pre,
        Start,
        End,
    }

    private BeActor mDup = null;
    private VInt3 mPos;

    private int mSplitRate = 1000;
    private int mAttackRate = 500;

    private eState mState = eState.Pre;

    public Skill5169(int sid, int skillLevel) : base(sid, skillLevel)
    {
        mState = eState.Pre;
    }

    public override void OnStart()
    {
        mState = eState.Pre;
        mSplitRate = TableManager.GetValueFromUnionCell(skillData.ValueA[0], 1);
        mAttackRate = TableManager.GetValueFromUnionCell(skillData.ValueB[0], 1);
		if (mAttackRate == 0) mAttackRate = GlobalLogic.VALUE_500;
    }

    private void _setState(BeActor actor, bool isControl)
    {
        if (null != actor && !actor.IsDeadOrRemoved())
        {
            if (isControl)
            {
                actor.sgForceSwitchState(new BeStateData() { _State = (int)ActionState.AS_WALK, _timeout = -1 });
                actor.sgSetCurrentStatesTimeout(int.MaxValue);
                actor.hasAI = false;
            }
            else
            {
                actor.sgForceSwitchState(new BeStateData() { _State = (int)ActionState.AS_IDLE});
                actor.sgSetCurrentStatesTimeout(1);
                actor.hasAI = true;
            }
        }
    }

    public override bool CheckSpellCondition(ActionState state)
    {
        var entity = owner.GetEntityData();

        return owner.stateController.CanDuplicate() && owner.CurrentBeScene.GetSummonCountByID(entity.monsterID) <= 0;
    }

    private void _createFake()
    {
        var entity = owner.GetEntityData();
		int monsterID = entity.monsterID + entity.level * GlobalLogic.VALUE_100;

        //var dup = owner.CurrentBeScene.SummonMonster(monsterID, owner.GetPostion(), owner.GetCamp(), owner, false, true); 
		var dup = owner.CurrentBeScene.DuplicateMonster(owner, new VFactor(mAttackRate,GlobalLogic.VALUE_1000)); 

        if (null != dup)
        {
            mDup = dup;
            mPos = owner.GetPosition();

            mDup.GetEntityData().type = (int)ProtoTable.UnitTable.eType.MONSTER;


#if !LOGIC_SERVER
            if (mDup.m_pkGeActor.goFootInfo != null)
            {
                mDup.m_pkGeActor.goFootInfo.SetActive(false);
            }

            if (mDup.m_pkGeActor.goInfoBar != null)
            {
                mDup.m_pkGeActor.goInfoBar.SetActive(false);
            }
#endif

            _setState(mDup, true);
            _setState(owner, true);
        }
        else
        {
            mDup = null;
        }
    }

    public override void OnUpdate(int iDeltime)
    {
        if (null != mDup && mState == eState.Start)
        {
			owner.SetMoveSpeedX(
                VInt.NewVInt(owner.GetFace() ? mSplitRate : -mSplitRate,GlobalLogic.VALUE_1000));
			mDup.SetMoveSpeedX(
                VInt.NewVInt(owner.GetFace() ? -mSplitRate : mSplitRate,GlobalLogic.VALUE_1000));

            mDup.SetFace(!owner.GetFace());

            // HACK 这里每一帧播放Walk02的第一帧动画
            if (owner.HasAction("Walk02"))
            {
                owner.PlayAction("Walk02");
            }

            if (mDup.HasAction("Walk02"))
            {
                mDup.PlayAction("Walk02");
            }
        }
    }

    public override void OnFinish()
    {
        if (null != mDup)
        {
            _setState(mDup, false);
            _setState(owner, false);
            mDup = null;
        }
        mState = eState.End;
    }

    public override void OnCancel()
    {
        if (null != mDup)
        {
            _setState(mDup, false);
            _setState(owner, false);
            mDup = null;
        }

        mState = eState.End;
    }

    public override void OnEnterPhase(int phase)
    {
        if (phase == 1)
        {
            mState = eState.Pre;
        }
        else if (phase == 2)
        {
            mState = eState.Start;
            _createFake();
        }
    }
}
