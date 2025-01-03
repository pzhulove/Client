using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//圣骑士生命源泉装备机制
public class Mechanism2028: BeMechanism
{
    public Mechanism2028(int sid, int skillLevel) : base(sid, skillLevel){ }

    public List<int> impactBuffInfoList = new List<int>();  //影响的Buff信息列表
    public List<int> buffInfoRadiusAddRateList = new List<int>();   //光环Buff范围增加千分比列表

    public override void OnInit()
    {
        base.OnInit();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            impactBuffInfoList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }

        for (int i = 0; i < data.ValueB.Count; i++)
        {
            buffInfoRadiusAddRateList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
    }

    public override void OnReset()
    {
        impactBuffInfoList.Clear();
        buffInfoRadiusAddRateList.Clear();
    }
}
