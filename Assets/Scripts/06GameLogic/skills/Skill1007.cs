using UnityEngine;
using System.Collections;
using GameClient;
using ProtoTable;

public class Skill2508 : Skill1007
{
    public Skill2508(int sid, int skillLevel) : base(sid, skillLevel)
    { }
}


public class Skill1007 : BeSkill
{

    protected IBeEventHandle handle = null;
    protected IBeEventHandle handle2 = null;

    protected int originSkillPhaseID = 0;
    protected int replaceSkillPhaseID = 0;
    protected int originSkillPhaseID2 = 0;
    protected int replaceSkillPhaseID2 = 0;

    protected InputManager.PressDir currentDir = InputManager.PressDir.RIGHT;

    public Skill1007(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        originSkillPhaseID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        replaceSkillPhaseID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);

        originSkillPhaseID2 = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        replaceSkillPhaseID2 = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
    }

    public override void OnPostInit()
    {
#if !LOGIC_SERVER
        base.OnPostInit();
        if (!owner.isLocalActor)
            return;
        canSlide = SettingManager.GetInstance().GetSlideMode("1007") == InputManager.SlideSetting.SLIDE;       //设置技能摇杆不显示
        if (!canSlide)
        {
            joystickMode = SkillJoystickMode.NONE;
        }
#endif
    }

    public override void OnStart()
    {
        ClearHandle();

        handle = owner.RegisterEventNew(BeEventType.onActionLoop, (args) =>
        {
            if (!owner.IsCastingSkill())
                return;

            ChooseRightSkill(currentDir);

        });
    }

    public override void OnUpdateJoystick(int degree)
    {
        currentDir = InputManager.GetDir(degree);
    }

    public override void OnEnterPhase(int phase)
    {
        RemoveHandle2();
        if (phase == 2 || phase == 3)
        {
            handle2 = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) =>
            {
                //int[] skillIdList = (int[])args[0];
                int curSkillId = param.m_Int;

                if (currentDir == InputManager.PressDir.TOP)
                {
                    if (curSkillId == originSkillPhaseID)
                        param.m_Int = replaceSkillPhaseID;
                    else if (curSkillId == originSkillPhaseID2)
                        param.m_Int = replaceSkillPhaseID2;
                }

                RemoveHandle2();
            });
        }
    }

    public override void OnFinish()
    {
        CleanUP();
    }

    public override void OnCancel()
    {
        CleanUP();
    }

    protected void RemoveHandle2()
    {
        if (handle2 != null)
        {
            handle2.Remove();
            handle2 = null;
        }
    }

    protected void ChooseRightSkill(InputManager.PressDir dir)
    {
        int targetPhaseSkillIndex = originSkillPhaseID;

        if (dir == InputManager.PressDir.TOP)
        {
            targetPhaseSkillIndex = replaceSkillPhaseID;
        }

        if (owner.m_cpkCurEntityActionInfo != null && owner.m_cpkCurEntityActionInfo.skillID != targetPhaseSkillIndex)
        {
            owner.delayCaller.DelayCall(10, () =>
            {
                //Logger.LogErrorFormat("play {0} t:{1}", targetPhaseSkillIndex, Time.realtimeSinceStartup);
                owner.PlaySkillAction(targetPhaseSkillIndex);
            });
        }
    }

    protected void RemoveHandle()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
    }

    protected void ClearHandle()
    {
        RemoveHandle();
        RemoveHandle2();
    }

    protected void CleanUP()
    {
        ClearHandle();
        currentDir = InputManager.PressDir.NONE;

    }
}
