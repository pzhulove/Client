using System;
using System.Collections.Generic;
using UnityEngine;

//当克里克血量降低到一定比例，释放某技能
class Mechanism136 : BeMechanism
{
    VFactor hpRate;
    int skillId;

    public Mechanism136(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        hpRate = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        skillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        if (!owner.HasSkill(skillId))
            return;

        handleA = owner.RegisterEventNew(BeEventType.onHPChange, args =>
        {
            if (owner.GetEntityData().GetHPRate() <= hpRate)
            {
                owner.UseSkill(skillId);
            }
        });

        handleB = owner.RegisterEventNew(BeEventType.onChangeModelFinish, args =>
        {
            handleA.Remove();
            handleA = null;
        });
    }
}