using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//纷影连环踢
public class Skill3211 : BeSkill
{
    string frame = "321120";
    bool frameFlag = false;
    IBeEventHandle handle;

    public Skill3211(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnStart()
    {
        handle = owner.RegisterEventNew(BeEventType.onSkillCurFrame, args =>
        //handle = owner.RegisterEvent(BeEventType.onSkillCurFrame, args =>
        {
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
            var phases = new int[4] { 3211, 32112, 32113, 32114 };
            owner.skillController.SetCurrentSkillPhases(phases);

            if (curPhase == 2)
                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();

            frameFlag = false;
            Release();
        }
    }

    void Release()
    {
        skillButtonState = SkillState.NORMAL;

        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }

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
