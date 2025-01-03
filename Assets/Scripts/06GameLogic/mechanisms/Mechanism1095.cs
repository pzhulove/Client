using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

//装备机制 当玩家触发某个buffInfo的时候，给玩家添加一个buff
public class Mechanism1095 : BeMechanism
{
    private int mTagBuffInfoID;
    private int mAddBuffInfo;

    public Mechanism1095(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        mTagBuffInfoID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        mAddBuffInfo = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }
    
    public override void OnStart()
    {
        if(owner != null)
        {
            handleA = owner.RegisterEventNew(BeEventType.onAddTriggerBuff, OnAddTriggerBuffEvent);
        }
    }
    
    private void OnAddTriggerBuffEvent(BeEvent.BeEventParam args)
    {
        var buffInfo = args.m_Obj as BuffInfoData;
        if(buffInfo != null && buffInfo.buffInfoID == mTagBuffInfoID)
        {
            owner.buffController.TryAddBuffInfo(mAddBuffInfo, owner, buffInfo.level);
        }
    }
}

