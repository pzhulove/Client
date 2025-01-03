using System;
using System.Collections.Generic;
using UnityEngine;

//旋风腿
class Skill3113 : BeSkill
{
    string frame = "311310";
    bool frameFlag = false;
    IBeEventHandle handle;

    public Skill3113(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnStart()
    {
        owner.SetMoveSpeedZ(0);

        if (owner.GetPosition().z > GlobalLogic.VALUE_5000)
        {
            var phases = new int[2] { 3113, 31131 };
            owner.SetCurrSkillPhase(phases);
        }

        handle = owner.RegisterEventNew(BeEventType.onSkillCurFrame, args =>
        {
            // string flag = (string)args[0];
            string flag = args.m_String;
            if (flag == frame)
            {
                frameFlag = true;
#if !LOGIC_SERVER
                if (button != null)
                    button.AddEffect(ETCButton.eEffectType.onContinue);
#endif
            }
        });
    }

    public override void OnClickAgain()
    {
        if (frameFlag)
        {
            var phases = new int[4] { 3113, 31131, 31133, 31134 };
            owner.SetCurrSkillPhase(phases);

            if (curPhase == 1)
            {
                var pos = owner.GetPosition();
                pos.z += GlobalLogic.VALUE_5000;
                owner.SetPosition(pos);

                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
            }
            else if (curPhase == 2)
            {
                var pos = owner.GetPosition();
                pos.z += GlobalLogic.VALUE_5000;
                owner.SetPosition(pos);

                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
            }

            frameFlag = false;
            Release();
        }
    }

    public override bool CanUseSkill()
    {
        //临时解决方案，暂时没有找到更好的办法
        return base.CanUseSkill() && owner.moveZSpeed < 72000;
    }

    void Release()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }

        skillButtonState = SkillState.NORMAL;

#if !LOGIC_SERVER
        if (button != null)
            button.RemoveEffect(ETCButton.eEffectType.onContinue);
#endif
    }

    public override void OnCancel()
    {
        Release();
    }

    public override void OnFinish()
    {
        Release();
    }
}
