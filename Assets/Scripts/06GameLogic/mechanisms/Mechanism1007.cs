using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

//近战攻击全部附带火属性攻击
public class Mechanism1007 : BeMechanism
{
    public Mechanism1007(int id, int lv) : base(id, lv) { }

    protected MagicElementType magicElementType = MagicElementType.NONE;
    protected List<int> effectList = new List<int>();

    protected GameClient.BeEvent.BeEventHandleNew mChangeMagicElementHandle;
    public override void OnInit()
    {
        magicElementType = (MagicElementType)TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        for(int i = 0; i < data.ValueB.Count; i++)
        {
            effectList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
    }

    public override void OnReset()
    {
        effectList.Clear();
        removeHandle();
    }

    public override void OnStart()
    {
        base.OnStart();
        mChangeMagicElementHandle = owner.RegisterEventNew(BeEventType.onChangeMagicElement, (GameClient.BeEvent.BeEventParam beEventParam) => 
        {
            
            int hurtType = beEventParam.m_Int;
            int hurtId = beEventParam.m_Int2;
            EffectTable hurtData = TableManager.instance.GetTableItem<EffectTable>(hurtId);
            if (hurtData == null || hurtData.DamageDistanceType != EffectTable.eDamageDistanceType.NEAR)
                return;
            if (!effectList.Contains(hurtId))
                return;
            beEventParam.m_Int = (int)magicElementType;
        });
    }

    public override void OnFinish()
    {
        removeHandle();
    }

    void removeHandle()
    {
        if(mChangeMagicElementHandle != null)
        {
            mChangeMagicElementHandle.Remove();
            mChangeMagicElementHandle = null;
        }
    }
}
