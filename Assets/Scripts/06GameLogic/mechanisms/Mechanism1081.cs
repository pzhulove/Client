using System;
using System.Collections.Generic;
using GameClient;

/// <summary>
/// 攻击时，a%的几率斩杀血量低于b的单位，若斩杀成功，则增加自身c点力量，持续d秒
/// </summary>
public class Mechanism1081 : BeMechanism
{
    protected int m_KillDamage = 0;     //斩杀血量固定值
    protected VFactor m_KillDamagePercent;  //斩杀千分比
    protected int m_AddBuffInfoId = 0;  //斩杀成功后添加的Buff信息Id

    public Mechanism1081(int mid, int lv) : base(mid, lv) { }
    public override void OnInit()
    {
        base.OnInit();
        m_KillDamage = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_KillDamagePercent = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueB[0], level), GlobalLogic.VALUE_1000);
        m_AddBuffInfoId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        //handleA = owner.RegisterEvent(BeEventType.onHitOther, RegisterEvent);
        handleA = OwnerRegisterEventNew(BeEventType.onHitOther, RegisterEvent);
    }

    /// <summary>
    /// 监听攻击到别人的事件
    /// </summary>
    protected void RegisterEvent(BeEvent.BeEventParam param)
    {
        var target = param.m_Obj as BeActor;
        if (target == null)
            return;
        int damage = param.m_Int4;
        int currentHp = target.attribute.GetHP();
        VFactor hpPercent = target.attribute.GetHPRate();
        if (m_KillDamage != 0 && currentHp > m_KillDamage)
            return;
        if (m_KillDamagePercent != VFactor.zero && hpPercent > m_KillDamagePercent)
            return;

        //第一套方案添加一个新的事件 用于boss那边的结算监听
        if (owner.CurrentBeBattle != null && !owner.CurrentBeBattle.HasFlag(GameClient.BattleFlagType.ZhanshaMechanismFlag))
        {
            target.GetEntityData().SetHP(-1);
            target.DoDead();
            target.TriggerEventNew(BeEventType.onSpecialDead);
        }
        //第二套保底方案 会有飘字 但是比较稳定
        else
        {
            target.DoHurt(currentHp);
        }

        if (m_AddBuffInfoId != 0)
        {
            owner.buffController.TryAddBuffInfo(m_AddBuffInfoId, owner, level);
        }
    }
}