using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 免疫指定触发效果伤害
/// </summary>
public class Mechanism1048 : BeMechanism
{
    public Mechanism1048(int id, int level) : base(id, level) { }

    private int hurtId = 0;    //免疫的触发效果ID

    public override void OnInit()
    {
        base.OnInit();
        hurtId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onBeHitAfterFinalDamage, RegisterBeHit);
        //handleA = owner.RegisterEvent(BeEventType.onBeHitAfterFinalDamage, RegisterBeHit);
    }

    /// <summary>
    /// 监听技能伤害
    /// </summary>
    private void RegisterBeHit(GameClient.BeEvent.BeEventParam param)
    {
        int id = param.m_Int2;
        //bool[] absorbFlag = (bool[])args[2];
        if (hurtId != id)
            return;
        param.m_Bool = true;
    }
}
