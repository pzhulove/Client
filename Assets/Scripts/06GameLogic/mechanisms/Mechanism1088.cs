using System;
using System.Collections.Generic;
using GameClient;

public class Mechanism1088 : BeMechanism
{
    int castSkillId = 0;
    List<int> interruptSkillIds = new List<int>();
    public Mechanism1088(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        interruptSkillIds.Clear();
        castSkillId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        for (int i = 0; i < data.ValueB.Count; ++i)
        {
            interruptSkillIds.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
    }
    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onSkillCanBeInterrupt, onSkillCanBeInterrupt);
    }
    private void onSkillCanBeInterrupt(BeEvent.BeEventParam args)
    {
        var castSkill = args.m_Obj as BeSkill;
        if (castSkill == null) return;
        if (castSkill.skillID != castSkillId) return;
        if(interruptSkillIds.Contains(args.m_Int))
        {
            args.m_Int2 = 1;
        }
    }
}

