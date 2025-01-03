using System;
using System.Collections.Generic;
using UnityEngine;

//刷怪机制or流程上机制的机制
public class Mechanism1101 : BeMechanism
{
    private List<int> mSummonMechanismList = new List<int>();//順序刷怪机制List
    private int mAddMechanismIndex = 0;
    
    public Mechanism1101(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        mAddMechanismIndex = 0;
        mSummonMechanismList.Clear();
        for(int i = 0; i < data.ValueA.Count; ++i)
        {
            mSummonMechanismList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
    }

    public override void OnStart()
    {
        TryAddOnceMechanism();
    }

    public void TryAddOnceMechanism()
    {
        if (owner != null && mAddMechanismIndex < mSummonMechanismList.Count)//召个怪 更新个Index
        {
            owner.AddMechanism(mSummonMechanismList[mAddMechanismIndex], level);
            AddIndex();
        }
    }

    private void AddIndex()
    {
        mAddMechanismIndex++;
        if(mAddMechanismIndex >= mSummonMechanismList.Count)
        {
            owner.RemoveMechanism(this);
        }
    }
}

