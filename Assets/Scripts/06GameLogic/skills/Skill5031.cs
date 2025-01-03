using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

//式神 玄武抓取攻击技能
class Skill5031 : BeSkill
{
    public Skill5031(int sid, int skillLevel) : base(sid, skillLevel) { }

    protected int pressDeltaTime = 100;
    protected bool checkBeGraberFlag = false;
    protected BeActor beGraber = null;
    bool checkLifeTime = false;
    bool pressFlag = false;
    int timer = 0;
    int grabTime = 0;
    int pressCount = 0;
    int pressTotalCount = 20;

    private bool isPressInCD = false;
    private int pressDeltaTimeSec = 0;
    private IBeEventHandle addPressCountEvent;

    public override void OnInit()
    {
        base.OnInit();
        pressDeltaTime = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        if(pressDeltaTime == 0)
        {
            pressDeltaTime = 100;
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        checkBeGraberFlag = false;
        checkLifeTime = true;
        pressFlag = false;
        timer = 0;
        grabTime = 0;
        if (BattleMain.IsModePvP(battleType) && BeClientSwitch.FunctionIsOpen(ClientSwitchType.XuanWuEndToFallProtect))
        {
            handleA = owner.RegisterEventNew(BeEventType.onExcuteGrab, (args) =>
            {
                BeActor actor = args.m_Obj as BeActor;
                if (actor != null && !actor.IsDead())
                {
                    handleB = actor.RegisterEventNew(BeEventType.onHurt, args1 =>
                    //handleB = actor.RegisterEvent(BeEventType.onHurt, (object[] args1) =>
                    {
                        int hurtValue = args1.m_Int;
                        if (hurtValue != 0)
                        {
                            actor.SetTag((int)AState.ACS_FALL, true);
                            actor.protectManager.StartFallHurtCount();      //触发浮空保护
                        }
                    });
                }
            });
        }
    }

    public override void OnEnterPhase(int phase)
    {
        base.OnEnterPhase(phase);
        if (phase == 2 && owner.grabController.grabbedActorList.Count > 0)
        {
            beGraber = owner.grabController.grabbedActorList[0];
            checkBeGraberFlag = true;
        }
        else if (phase == 3)
        {
            checkBeGraberFlag = false;
            if(pressCount > 0 && beGraber != null)
            {
                beGraber.EndPressCount();
            }
        }
    }

    public override void OnUpdate(int iDeltime)
    {
        base.OnUpdate(iDeltime);
        if (checkBeGraberFlag)
        {
            timer += iDeltime;
            if (beGraber == null || beGraber.IsDead())
                owner.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            if (beGraber != null && !beGraber.IsDead())
            {
                if (!pressFlag)
                {
                    grabTime = owner.GetStateGraph().GetCurrentStateData().uiTimeOut; //抓取阶段技能总时长
                    pressTotalCount = grabTime / (2 * pressDeltaTime);
                    beGraber.StartPressCount(BeActor.QuickPressType.GRAB, owner);
                    addPressCountEvent = beGraber.RegisterEventNew(BeEventType.onGrabPressCountAdd,(args)=> 
                    {
                        args.m_Int = 0;
                        if (!isPressInCD)
                        {
                            pressCount++;
                            
                            isPressInCD = true;
                        }
                    });
                    pressFlag = true;
                }

                UpdatePressCD(iDeltime);
                if (grabTime > 0 && 
                    2 * pressTotalCount * timer + pressCount * grabTime > 2 * pressTotalCount * grabTime) 
                    //timer/grapTime >1-pressCount/2pressTotalCount 技能阶段时间百分比 > 1 - 按键百分比  ==》》释放被抓取目标
                {
                    beGraber.EndPressCount();
                    owner.CancelSkill(skillID, false);
                    owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
                }
            }
            
        }
        CheckMonsterLife();
    }

    public override void OnCancel()
    {
        base.OnCancel();
        checkLifeTime = false;
        pressFlag = false;
        timer = 0;
        grabTime = 0;
        pressTotalCount = 20;
        pressCount = 0;
        pressDeltaTimeSec = 0;
        isPressInCD = false;
        ClearHandleEvent();
        if (beGraber != null)
        {
            beGraber.buffController.RemoveBuff(361212);
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        checkLifeTime = false;
        pressFlag = false;
        timer = 0;
        grabTime = 0;
        pressTotalCount = 20;
        pressCount = 0;
        pressDeltaTimeSec = 0;
        isPressInCD = false;
        ClearHandleEvent();
        if (beGraber != null)
        {
            beGraber.buffController.RemoveBuff(361212);
        }
    }

    protected void CheckMonsterLife()
    {
        if (!checkLifeTime)
            return;
        if (owner == null)
            return;
        BeBuff buff12 = owner.buffController.HasBuffByID(12);
        if (buff12 == null)
            return;
        if (buff12.duration - buff12.runTime < GlobalLogic.VALUE_500)
            buff12.duration += GlobalLogic.VALUE_500;
    }


    //这是一个摇杆减少被控时间的upDate函数
    private void UpdatePressCD(int iDeltaTime)
    {
        if (isPressInCD)
        {
            pressDeltaTimeSec += iDeltaTime;

            if (pressDeltaTimeSec >= pressDeltaTime)
            {
                pressDeltaTimeSec = 0;
                isPressInCD = false;
            }
        }
    }

    private void ClearHandleEvent()
    {
        if(addPressCountEvent != null)
        {
            addPressCountEvent.Remove();
            addPressCountEvent = null;
        }
    }
}
