using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism89 : BeMechanism
{
    List<int> skillIds = new List<int>();
    IBeEventHandle handle;

    public Mechanism89(int id, int level) : base(id, level) { }

    public override void OnReset()
    {
        skillIds.Clear();
        handle = null;
    }

    public override void OnInit()
    {
        skillIds.Clear();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            int id = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            skillIds.Add(id);
        }
    }

    public override void OnStart()
    {
        handle = owner.RegisterEventNew(BeEventType.onCastSkillFinish, args =>
        {
            int castSkillID = args.m_Int;
            if (skillIds.Contains(castSkillID))
            {
                SkillStartCoolDown();
            }
        });
    }

    void SkillStartCoolDown()
    {
        for (int i = 0; i < skillIds.Count; i++)
        {
            var skill = owner.GetSkill(skillIds[i]);
            if (skill != null)
            {
                skill.StartCoolDown();
            }
        }
    }

    public override void OnFinish()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
    }
}
