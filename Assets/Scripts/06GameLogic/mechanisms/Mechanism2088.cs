using System;
using System.Collections.Generic;
using GameClient;

/// <summary>
/// 炫纹融合蓄力多段伤害机制
/// </summary>
public class Mechanism2088 : BeMechanism
{
    public Mechanism2088(int id, int level) : base(id, level) { }

    protected int m_HurtId = 0;
    protected int m_TimeAcc = 0;
    protected int m_MaxHitNum = 0;
    protected VFactor m_AddDamagePercent = VFactor.zero;

    protected int m_CurHitNum = 0;
    protected int[] m_HurtIdArr = null;
    protected BeActor m_Attacker = null;
    private int m_ChaserLevel = 1;

    protected BeEvent.BeEventHandleNew m_ReplaceHurtDataHandle = null;

    public override void OnInit()
    {
        m_HurtId = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        m_TimeAcc = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        m_MaxHitNum = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        m_AddDamagePercent = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueD[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnReset()
    {
        m_CurHitNum = 0;
        m_HurtIdArr = null;
        m_Attacker = null;
        m_ChaserLevel = 1;
        if (m_ReplaceHurtDataHandle != null)
        {
            m_ReplaceHurtDataHandle.Remove();
            m_ReplaceHurtDataHandle = null;
        }
    }

    public void InitData(int[] hurtIdArr, BeActor actor)
    {
        m_HurtIdArr = hurtIdArr;
        m_Attacker = actor;
        InitRegister();
        GetChaserLevel();
    }

    public override void OnStart()
    {
        base.OnStart();
        InitTimeAcc(m_TimeAcc);
       
    }

    /// <summary>
    /// 取下炫纹发射的等级,如果没有技能取1级
    /// </summary>
    private void GetChaserLevel()
    {
        if (m_Attacker != null)
        {
            int level = m_Attacker.GetSkillLevel(2302);
            if (level == 0)
            {
                m_ChaserLevel = 1;
            }
            else
            {
                m_ChaserLevel = level;
            }
        }
    }
    
    public override void OnUpdateTimeAcc()
    {
        base.OnUpdateTimeAcc();
        if (m_CurHitNum >= m_MaxHitNum)
        {
            Finish();
        }
        else
        {
            DoAttack();
            m_CurHitNum++;
        }
    }

    protected void InitRegister()
    {
        if (m_Attacker == null)
            return;
        handleA = m_Attacker.RegisterEventNew(BeEventType.onReplaceHurtTableDamageData, ReplaceHurtTableDamageData);
    }

    protected void DoAttack()
    {
        if (m_Attacker == null)
            return;
        m_Attacker.DoAttackTo(owner, m_HurtId);
    }

    protected void ReplaceHurtTableDamageData(BeEvent.BeEventParam param)
    {
        int hurtId = param.m_Int;
        if (hurtId != m_HurtId)
            return;
        if (m_HurtIdArr == null)
            return;
        int damageValue = 0;
        VPercent damagePercent = VPercent.zero;
        bool isPvPMode = BattleMain.IsModePvP(battleType);
        for (int i = 0; i < m_HurtIdArr.Length; i++)
        {
            ProtoTable.EffectTable hurtData = TableManager.instance.GetTableItem<ProtoTable.EffectTable>(m_HurtIdArr[i]);
            damageValue += TableManager.GetValueFromUnionCell(isPvPMode ? hurtData.DamageFixedValuePVP : hurtData.DamageFixedValue, m_ChaserLevel);
            damagePercent += TableManager.GetValueFromUnionCell(isPvPMode ? hurtData.DamageRatePVP : hurtData.DamageRate, m_ChaserLevel);
        }
        param.m_Int2 = damageValue * m_AddDamagePercent;
        var addFactor = damagePercent.precent * m_AddDamagePercent;
        param.m_Percent = new VPercent(addFactor.single);
    }
}

