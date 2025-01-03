using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 目标身上满足一定buffID时，攻击无视被击方属抗
/// </summary>
public class Mechanism2037 : BeMechanism
{
    private int[] buffIDs;
    private int defenceValue;
    public Mechanism2037(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        buffIDs = new int[data.ValueA.Length];
        for (int i = 0; i < buffIDs.Length; i++)
        {
            buffIDs[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        }
        defenceValue = TableManager.GetValueFromUnionCell(data.ValueB[0], level)/1000;
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.OnChangeAttributeDefence, (args) => 
        {
            /*int[] array = args[0] as int[];
            if (HaveAllBuff())
            {
                array[0] -= defenceValue;
            }*/
            
            if (HaveAllBuff())
            {
                args.m_Int -= defenceValue;
            }
        });
    }

    private bool HaveAllBuff()
    {
        for (int i = 0; i < buffIDs.Length; i++)
        {
            if (owner.buffController.HasBuffByID(buffIDs[i]) == null)
                return false;
        }
        return true;
    }
}
