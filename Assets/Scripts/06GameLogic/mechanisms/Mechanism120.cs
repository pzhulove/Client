using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少某个技能阶段攻击框的大小（千分比）
class Mechanism120 : BeMechanism
{
    public List<int> skillPhaseIdList = new List<int>();
    public int align;//0-中心，1-面朝方向
    public int xRate;
    public int yRate;
    public int zDimRate;

    public Mechanism120(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        skillPhaseIdList.Clear();
        
    }
    public override void OnInit()
    {
        for (int i = 0; i < data.ValueA.Length; i++)
        {
            var skillPhaseId = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            skillPhaseIdList.Add(skillPhaseId);
        }
        align = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        xRate = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        yRate = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        zDimRate = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
    }
	
	public override void OnStart()
    {
        handleA = OwnerRegisterEventNew(BeEventType.onChangeAttackDBox, (args) =>
        {
            var skillId = args.m_Int;
            if (skillPhaseIdList.Contains(skillId))
            {
                var array = (IList<int>)args.m_Obj;
                array[0] = align;
                array[1] += xRate;
                array[2] += yRate;
            }
        });

        handleB = OwnerRegisterEventNew(BeEventType.onChangeAttackZDim, args =>
        {
            var skillId = args.m_Int;
            if (skillPhaseIdList.Contains(skillId))
            {
                args.m_Int2 += zDimRate;
            }
        });
    }
	
    public override void OnFinish()
    {
        skillPhaseIdList.Clear();
    }

}
