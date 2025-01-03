using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//改变杀意波动机制2
public class Mechanism1005 : BeMechanism
{
    public Mechanism1005(int mid, int lv) : base(mid, lv) { }
    
    public int addEntityCount = 0;      //增加实体数量
    public int addRadius = 0;           //增加范围(千分比)
    public int addTimeAccRate = 0;       //增加时间间隔(千分比)
    public List<int> impactMechanismIdList = new List<int>();           //影响的机制ID列表

    public override void OnInit()
    {
        base.OnInit();
        if (data.ValueA.Count>0)
            addEntityCount = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        if(data.ValueB.Count > 0)
            addRadius = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        if (data.ValueC.Count > 0)
            addTimeAccRate = TableManager.GetValueFromUnionCell(data.ValueC[0],level);
        if (data.ValueD.Count > 0)
            impactMechanismIdList.Add(TableManager.GetValueFromUnionCell(data.ValueD[0], level));
    }

    public override void OnReset()
    {
        addEntityCount = 0;
        addRadius = 0;
        addTimeAccRate = 0;
        impactMechanismIdList.Clear();
    }
}
