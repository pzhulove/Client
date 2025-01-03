using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//圣骑士特殊属性加成装备机制
public class Mechanism2027 : BeMechanism
{
    public Mechanism2027(int sid, int skillLevel) : base(sid, skillLevel) { }

    private int typeCount;
    public int[] addValueArr = null;    //增加的固定值
    public int[] addRateArr = null;         //增加的千分比
    public int[] selfExtraAddRateArr = null;  //自己的额外增加比率列表
    public int[] coefficientArr = null;  //系数列表

    public List<int> skillList = new List<int>();
    public override void OnInit()
    {
        base.OnInit();
        InitData();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            int index = TableManager.GetValueFromUnionCell(data.ValueA[i], level) - 1;
            addValueArr[index] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            addRateArr[index] = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
            selfExtraAddRateArr[index] = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
            coefficientArr[index] = TableManager.GetValueFromUnionCell(data.ValueE[i], level);
        }

        for (int i = 0; i < data.ValueF.Count; i++)
        {
            skillList.Add(TableManager.GetValueFromUnionCell(data.ValueF[i], level));
        }
    }

    public override void OnReset()
    {
        typeCount = 0;
        addValueArr = null;
        addRateArr = null;
        selfExtraAddRateArr = null;
        coefficientArr = null;
        skillList.Clear();
}

    public bool IsContainSkillID(int skillid)
    {
        return skillList.Contains(skillid);
    }

    private void InitData()
    {
        typeCount = (int)Mechanism1017.ChangeAttType.Count - 1;
        addValueArr = new int[typeCount];
        addRateArr = new int[typeCount];
        selfExtraAddRateArr = new int[typeCount];
        coefficientArr = new int[typeCount];
    }
}
