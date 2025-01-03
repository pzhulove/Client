using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 监听血量变化添加BuffInfo 
/// </summary>

public class Mechanism91 : BeMechanism
{
    

    protected int hpPercent = 0;                                //血量千分比
    protected List<int> buffInfoIdList = new List<int>();       //血量升高到千分比以上时添加BuffInfo
    protected List<int> removeBuffIdList = new List<int>();     //血量降到千分比以下时移除Buff

    protected VFactor hpPercentFactor = VFactor.zero;           //条件血量千分比

    protected IBeEventHandle hpChangeHandle = null;

    public Mechanism91(int id, int level) : base(id, level) { }
    public override void OnReset()
    {
        buffInfoIdList.Clear();
        removeBuffIdList.Clear();
        hpPercentFactor = VFactor.zero; 
        hpChangeHandle = null;
    }
    public override void OnInit()
    {
        hpPercent = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        for (int i=0;i<data.ValueB.Count;i++)
        {
            buffInfoIdList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i],level));
        }

        for(int i = 0; i < data.ValueC.Count; i++)
        {
            removeBuffIdList.Add(TableManager.GetValueFromUnionCell(data.ValueC[i],level));
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        hpPercentFactor = new VFactor(hpPercent,GlobalLogic.VALUE_1000);
        CheckHp();
        hpChangeHandle = owner.RegisterEventNew(BeEventType.onHPChange,(args)=>
        {
            CheckHp();
        });
    }

    protected void CheckHp()
    {
        int curHp = owner.GetEntityData().GetHP();
        int maxHp = owner.GetEntityData().GetMaxHP();
        VFactor curPercent = new VFactor(curHp, maxHp);
        if (curPercent > hpPercentFactor)
        {
            AddBuffInfo();
        }
        else
        {
            RemoveBuff();
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RemoveHandle();
    }

    protected void AddBuffInfo()
    {
        for(int i = 0; i < buffInfoIdList.Count; i++)
        {
            if (owner.buffController.HasBuffByID(removeBuffIdList[i]) ==null)
            {
                owner.buffController.TryAddBuff(buffInfoIdList[i]);
            }
        }
    }

    protected void RemoveBuff()
    {
        for (int i=0;i< removeBuffIdList.Count;i++)
        {
            owner.buffController.RemoveBuff(removeBuffIdList[i]);
        }
    }

    protected void RemoveHandle()
    {
        if (hpChangeHandle != null)
        {
            hpChangeHandle.Remove();
            hpChangeHandle = null;
        }
    }
}
