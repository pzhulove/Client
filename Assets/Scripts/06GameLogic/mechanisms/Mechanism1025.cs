using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// 释放技能添加BUFFINFO
/// </summary>
public class Mechanism1025 : BeMechanism
{
    public Mechanism1025(int mid, int lv) : base(mid, lv) { }
    private int buffInfoID = 0;
    private int[] skillIDs;
    public override void OnInit()
    {
        base.OnInit();
        buffInfoID =  TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        skillIDs =new int [ data.ValueB.Count];
        for (int i = 0; i < data.ValueB.Count; i++)
        {
            skillIDs[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onCastSkill, (args) =>
        {
            int skillID = args.m_Int;
            if (Array.IndexOf(skillIDs, skillID) != -1)
            {
                owner.buffController.TryAddBuffInfo(buffInfoID,owner,level);
            }
        });
    }
    
   
}
