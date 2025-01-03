using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 给一个指定的触发效果	ID附加额外的	暴击几率	（千分比	）
/// </summary>
public class Mechanism1114 : BeMechanism
{
    private VRate rate = VRate.zero;
    private int hurtId = 0;

    protected BeEvent.BeEventHandleNew mHandleNew = null;
    
    public Mechanism1114(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        hurtId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        rate = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
       
    }
    public override void OnStart()
    {
        mHandleNew = owner.RegisterEventNew(BeEventType.onReplaceHurtTableCiriticalData, OnChangeCiritical);
    }

    protected void OnChangeCiritical(BeEvent.BeEventParam param)
    {
        int hurtId = param.m_Int;
        if (hurtId == this.hurtId)
        {
            param.m_Rate += rate;
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (mHandleNew != null)
        {
            mHandleNew.Remove();
            mHandleNew = null;
        }
    }
}

