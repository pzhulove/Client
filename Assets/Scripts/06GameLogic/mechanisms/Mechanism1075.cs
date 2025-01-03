using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 圣骑士被动觉醒机制
/// </summary>
public class Mechanism1075 : BeMechanism
{
    public Mechanism1075(int id, int level) : base(id, level) { }

    protected int addBuffId = 0;
    protected int removeBuffId = 0;

    public override void OnInit()
    {
        base.OnInit();
        addBuffId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        removeBuffId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        ChangeBuff();
    }

    /// <summary>
    /// 改变buff数据
    /// </summary>
    protected void ChangeBuff()
    {
        int oldBuffCount = owner.buffController.GetBuffCountByID(removeBuffId);
        owner.buffController.RemoveBuff(removeBuffId, 1);
        if (oldBuffCount <= 1)
        {
            owner.buffController.TryAddBuff(addBuffId, -1,level);
        }
    }
}
