using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;

//魔笛-分身
public class Skill5573 : BeSkill
{
    private enum eState
    {
        Pre,
        Start,
        End,
    }

    private BeActor mFront = null;
    private BeActor mBack = null;

    private VInt mSplitRate = VInt.NewVInt(3000,1000);
    private List<int> m_UnUseSkillList = new List<int>();

    private eState mState = eState.Pre;

    public Skill5573(int sid, int skillLevel): base(sid, skillLevel)
	{
        
	}

    public override void OnInit()
    {
        m_UnUseSkillList.Clear();
        m_UnUseSkillList.Add(5568);
        m_UnUseSkillList.Add(5569);
        m_UnUseSkillList.Add(5573);
		m_UnUseSkillList.Add(5570);
    }

    public override void OnStart()
    {
        mState = eState.Pre;
    }

    private void _setState(BeActor actor, bool isControl)
    {
        if (null != actor&&!actor.IsDead())
        {
            if (isControl)
            {
                actor.sgForceSwitchState(new BeStateData() { _State = (int)ActionState.AS_WALK, _timeout = -1 });
                actor.sgSetCurrentStatesTimeout(int.MaxValue);
                actor.hasAI = false;
            }
            else
            {
                actor.sgForceSwitchState(new BeStateData() { _State = (int)ActionState.AS_IDLE });
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

        owner.aiManager.SetSkillsEnable(m_UnUseSkillList,false);

        mFront = owner.CurrentBeScene.DuplicateMonster(owner);
        if (null != mFront)
        {
            //创建出来的分身没有 混乱和点名技能
            mFront.aiManager.SetSkillsEnable(m_UnUseSkillList, false);
            mFront.skillController.RemoveSkill(5574);
            mFront.GetEntityData().type = (int)ProtoTable.UnitTable.eType.MONSTER;
            _setState(mFront, true);
        }
        else
        {
            mFront = null;
        }


        mBack = owner.CurrentBeScene.DuplicateMonster(owner);
        if (null != mBack)
        {
            //创建出来的分身没有 混乱和点名技能
            mBack.aiManager.SetSkillsEnable(m_UnUseSkillList, false);
            mBack.skillController.RemoveSkill(5574);
            mBack.GetEntityData().type = (int)ProtoTable.UnitTable.eType.MONSTER;
            _setState(mBack, true);
        }
        else
        {
            mBack = null;
        }
    }

    public override void OnUpdate(int iDeltime)
    {
        if (null != mFront && mState == eState.Start)
        {
			mFront.SetMoveSpeedX((!owner.GetFace() ? -mSplitRate : mSplitRate));
            mFront.SetFace(true);

            // HACK 这里每一帧播放Walk的第一帧动画
            if (mFront.HasAction("Walk"))
            {
                mFront.PlayAction("Walk");
            }
        }

        if (null != mBack && mState == eState.Start)
        {
			mBack.SetMoveSpeedX((owner.GetFace() ? -mSplitRate : mSplitRate));
            mBack.SetFace(false);

            // HACK 这里每一帧播放Walk的第一帧动画
            if (mBack.HasAction("Walk"))
            {
                mBack.PlayAction("Walk");
            }
        }
    }

    public override void OnFinish()
    {
        if (null != mFront)
        {
            _setState(mFront, false);
            mFront = null;
        }

        if (null != mBack)
        {
            _setState(mBack, false);
            mBack = null;
        }
        mState = eState.End;
    }

    public override void OnCancel()
    {
        if (null != mFront)
        {
            _setState(mFront, false);
            mFront = null;
        }

        if (null != mBack)
        {
            _setState(mBack, false);
            mBack = null;
        }
        mState = eState.End;
    }

    public override void OnEnterPhase(int phase)
    {
        if (phase == 1)
        {
            mState = eState.Pre;
        }
        else 
        if (phase == 2)
        {
            mState = eState.Start;
            _createFake();
        }
    }

}
