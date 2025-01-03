using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//特定的触发效果攻击到目标以后给自己或者自己的召唤者上一个BuffInfo
public class Mechanism1009 : BeMechanism
{
    public Mechanism1009(int mid, int lv) : base(mid, lv) { }

    int effectId = 0;
    List<int> buffInfoIdList = new List<int>();
    bool isPlayer = false;
    bool addToOwner = false;

    public override void OnInit()
    {
        base.OnInit();
        effectId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        if (data.ValueB.Count > 0)
        {
            for(int i = 0; i < data.ValueB.Count; i++)
            {
                buffInfoIdList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
            }
        }
        if(data.ValueC.Count>0)
            isPlayer = TableManager.GetValueFromUnionCell(data.ValueC[0], level) == 0 ? false : true;
        if(data.ValueD.Count>0)
            addToOwner = TableManager.GetValueFromUnionCell(data.ValueD[0], level) == 0 ? false : true;
    }

    public override void OnReset()
    {
        buffInfoIdList.Clear();
        isPlayer = false;
        addToOwner = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onHitOther,args =>
        //owner.RegisterEvent(BeEventType.onHitOther, (object[] args) =>
        {
            BeActor target = args.m_Obj as BeActor;
            int hurtId = args.m_Int;
            if (hurtId != effectId)
                return;
            if (CheckTargetCondition(target))
                AddBuffInfo();
        });
    }

    bool CheckTargetCondition(BeActor actor)
    {
        if (actor == null)
            return false;
        if (isPlayer && actor.attribute != null)
            return !actor.IsMonster() && !actor.attribute.isSummonMonster;
        return true;
    }

    void AddBuffInfo()
    {
        if (buffInfoIdList.Count <= 0)
            return;
        BeActor ownerActor = (BeActor)owner.GetOwner();
        BeActor buffTarget = owner;
        if (addToOwner && ownerActor != null)
            buffTarget = ownerActor;
        for(int i=0;i< buffInfoIdList.Count; i++)
        {
            buffTarget.buffController.TryAddBuff(buffInfoIdList[i]);
        }
    }
}
