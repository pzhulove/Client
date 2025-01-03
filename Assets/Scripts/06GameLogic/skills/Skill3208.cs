using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//升龙拳
public class Skill3208 : BeSkill
{
    IBeEventHandle handle;
    string frameFlag = "320801";

    public Skill3208(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        
    }

    public override void OnStart()
    {
        handle = owner.RegisterEventNew(BeEventType.onSkillCurFrame, args =>
        //handle = owner.RegisterEvent(BeEventType.onSkillCurFrame, args =>
        {
            string flag = args.m_String;
            if (flag.Equals(frameFlag))
            {
                SetSkillSpeedWithSkillData();
            }
        });
    }
    public override void OnCancel()
    {
        RemoveHandles();
    }

    public override void OnFinish()
    {
        RemoveHandles();
    }

    void RemoveHandles()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
    }
}
