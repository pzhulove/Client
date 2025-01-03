using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;
//当HP低于X%时，添加一个BUFF信息
public class Mechanism1041: BeMechanism
{
    public Mechanism1041(int id, int level) : base(id, level) { }
    VFactor fHpPercent;
    private List<int> buffInfoIds = new List<int>();
    public override void OnInit()
    {
        int hpPercent = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        fHpPercent = new VFactor(hpPercent, GlobalLogic.VALUE_1000);
        buffInfoIds.Clear();
        if (data.ValueB.Count > 0)
        {
            for (int i = 0; i < data.ValueB.Count; i++)
            {
                buffInfoIds.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
            }
        }
    }
    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onHPChange, onHpChange);
    }
    private void onHpChange(BeEvent.BeEventParam args)
    {
        if(owner.IsDead())
        {
            return;
        }

        if (owner.attribute.GetHPRate() < fHpPercent)
        {
            for (int i = 0; i < buffInfoIds.Count;i++)
            {
                owner.buffController.TryAddBuff(buffInfoIds[i]);
            }
        }
    }
}
