using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 被击伤害累积达到一定数值后触发一个帧效果
/// </summary>
public class Mechanism1077 : BeMechanism
{
    public Mechanism1077(int id, int level) : base(id, level) { }

    protected int totalDamage = 0;      //累积伤害数值
    protected int effectId = 0;     //帧效果ID

    protected int curTotalDamage = 0;   //当前血量累计值

    public override void OnInit()
    {
        base.OnInit();
        totalDamage = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        effectId = TableManager.GetValueFromUnionCell(data.ValueB[0],level);
    }

    public override void OnReset()
    {
        curTotalDamage = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        InitData();
        handleA = owner.RegisterEventNew(BeEventType.onHPChange, OnHpChange);
    }

    protected void InitData()
    {
        curTotalDamage = 0;
    }

    /// <summary>
    /// 监听血量变化
    /// </summary>
    protected void OnHpChange(BeEvent.BeEventParam args)
    {
        int value = args.m_Int;
        //回血暂时不考虑 只记录扣血数据
        if (value > 0)
            return;
        curTotalDamage += -value;
        if (curTotalDamage < totalDamage)
            return;
        curTotalDamage = 0;
        owner.DealEffectFrame(owner, effectId);
    }
}
