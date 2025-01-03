using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//虎袭继承疾风打
public class Skill3504 : Skill3601
{
    public Skill3504(int sid, int skillLevel) : base(sid, skillLevel){}
    protected int[] hurtIdArr = new int[] {35041 , 35044} ;

    public override void OnInit()
    {
        base.OnInit();
        startSwitchId = "3504101";
        endSwitchId = "3504102";
    }

    public override void OnStart()
    {
        base.OnStart();
        handleB = owner.RegisterEventNew(BeEventType.onEndGrab, (args) => 
        {
            BeActor target = args.m_Obj as BeActor;
            
            if (target == null || target.IsRemoved())
                return;
            int curHurtId = switchFlag ? hurtIdArr[1] : hurtIdArr[0];
            owner.DoAttackTo(target, curHurtId, false); 
        });
    }
}

//疾风打技能特写
public class Skill3601 : BeSkill
{
    protected string startSwitchId;
    protected string endSwitchId;

    protected bool switchFlag = false;

    private int interruptFrame = 8;
    private List<int> interruptIdList = new List<int>();

    public Skill3601(int sid, int skillLevel) : base(sid, skillLevel){}

    public override void OnInit()
    {
        base.OnInit();
        startSwitchId = "360101";
        endSwitchId = "360102";
        interruptIdList.Clear();
        if (skillData.ValueA.Count > 0)
        {
            for(int i = 0; i < skillData.ValueA.Count; i++)
            {
                interruptIdList.Add(TableManager.GetValueFromUnionCell(skillData.ValueA[i],level));
            }
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        switchFlag = false;
        handleA = owner.RegisterEventNew(BeEventType.onSkillCurFrame, (args) =>
        //handleA = owner.RegisterEvent(BeEventType.onSkillCurFrame, (object[] args) => 
        {
            string flag = args.m_String;
            if (flag == startSwitchId)
            {
                switchFlag = true;
                SwitchState();
            }
            else if(flag == endSwitchId)
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
        if (curPhase == 2)
            return;
        ResetButtonEffect();
        ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
    }

    //疾风打在指定帧以后可以被指定技能打断
    public override bool CanBeInterrupt(int skillId)
    {
        bool origionFlag = base.CanBeInterrupt(skillId);
        if (curPhase == 2 && owner.GetCurrentFrame() > interruptFrame && interruptIdList.Contains(skillId))
            origionFlag = true;
        return origionFlag;
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
