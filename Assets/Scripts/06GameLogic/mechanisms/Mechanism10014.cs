using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism10014 : BeMechanism
{
    private List<int> aimEntityIds = new List<int>();
    private List<int> aimBuffIds = new List<int>();
    private List<int> changeBuffIds = new List<int>();

    public Mechanism10014(int mid, int lv) : base(mid, lv) { }
    
    public override void OnInit()
    {
        for (int index = 0; index < data.ValueA.Count; ++index)
        {
            aimEntityIds.Add(TableManager.GetValueFromUnionCell(data.ValueA[index], level));
        }
        for (int index = 0; index < data.ValueA.Count; ++index)
        {
            aimBuffIds.Add(TableManager.GetValueFromUnionCell(data.ValueB[index], level));
        }
        for (int index = 0; index < data.ValueB.Count; ++index)
        {
            int buffInfoId = TableManager.GetValueFromUnionCell(data.ValueC[index], level);
            var table = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(buffInfoId);
            if (null != table)
                changeBuffIds.Add(table.BuffID);
        }
    }

    public override void OnStart()
    {
        handleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.OnChangeBuff, param =>
        {
            var actor = param.m_Obj as BeActor;
            if (null == actor)
                return;
            if (actor.GetOwner() == owner)
            {
                //不包含在目标召唤物内
                if (!aimEntityIds.Contains(actor.GetEntityData().monsterID))
                    return;
                for (int index = 0; index < aimBuffIds.Count; ++index)
                {
                    if (param.m_Int == aimBuffIds[index] && null != changeBuffIds && changeBuffIds.Count > index)
                    {
                        param.m_Int = 0;
                        //给自己加上对应的buff
                        owner.buffController.TryAddBuffInfo(changeBuffIds[index], owner, level);
                        return;
                    }
                }
            }
        });
    }
}
