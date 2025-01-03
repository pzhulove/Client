using System.Collections.Generic;
using FlatBuffers;
using GameClient;
using Spine;

/// <summary>
/// 与1050类似，但支持召唤物完成击杀，召唤物加buff
/// </summary>
public class Mechanism1513 : BeMechanism
{
    public Mechanism1513(int mid, int lv) : base(mid, lv)
    {
    }
    
    int buffInfoId = 0;

    public override void OnInit()
    {
        buffInfoId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }
    public override void OnStart()
    {
        if (owner == null || owner.CurrentBeScene == null)
            return;
        handleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onKill, OnKillMonster);
    }
    private void OnKillMonster(BeEvent.BeEventParam param)
    {
        BeActor attacker = null;
        if (param.m_Obj2 is BeProjectile projectile)
        {
            var o = projectile.GetOwner();
            if(o != null)
                attacker = o as BeActor;
        }
        else
        {
            attacker = param.m_Obj2 as BeActor;
        }
        
        if (attacker == null)
            return;
        if (attacker.GetPID() == owner.GetPID())
        {
            owner.buffController?.TryAddBuff(buffInfoId);
        }
    }
}