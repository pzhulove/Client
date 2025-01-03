using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//式神殇 技能特写
public class Skill3615 : BeSkill
{
    public Skill3615(int sid, int skillLevel) : base(sid, skillLevel) { }

    protected string startSwitchId;
    protected string endSwitchId;
    protected bool switchFlag = false;

    public override void OnInit()
    {
        base.OnInit();
        startSwitchId = "361501";
        endSwitchId = "361502";
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onSkillCurFrame, (args) =>
        //handleA = owner.RegisterEvent(BeEventType.onSkillCurFrame, (object[] args) =>
        {
            string flag = args.m_String;
            if (flag == startSwitchId)
            {
                switchFlag = true;
                SwitchState();
            }
            else if (flag == endSwitchId)
            {
                switchFlag = false;
                ResetButtonEffect();
            }
        });
    }

    public override void OnClickAgain()
    {
        base.OnClickAgain();
        if (!skillState.IsRunning() || !switchFlag)
            return;
        switchFlag = false;
        ResetButtonEffect();
        if (curPhase == 2)
        {
            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
        }
        else if(curPhase == 3)
        {
            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
        }
    }

    public override void OnEnterPhase(int phase)
    {
        base.OnEnterPhase(phase);
        curPhase = phase;
    }

    protected void SwitchState()
    {
        skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        ChangeButtonEffect();
    }

    public override void OnCancel()
    {
        base.OnCancel();
        ResetButtonEffect();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        ResetButtonEffect();
    }
}
