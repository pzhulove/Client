using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 监听状态变化
 */
public class Mechanism29 : BeMechanism 
{
    protected int m_EffectHp = 0;               //监听血量变化
    protected int m_SkillId = 5574;             //触发技能ID
    protected int m_EffectCountMax = 1;         //最大触发次数
    protected int m_BossInvincibleTime = 0;     //Boss无敌时间
    protected int m_StopAI = 0;                 //停止AI 0 不停止 1停止 
    protected int m_DunfuBuffId = -1;           //蹲伏检测buff


    protected int m_CurrEffectCount = 0;        //触发次数
    
    public Mechanism29(int mid, int lv):base(mid, lv){}

    public override void OnReset()
    {
        m_StopAI = 0; 
        m_DunfuBuffId = -1;
        m_CurrEffectCount = 0;
    }

    public override void OnInit()
    {
        m_CurrEffectCount = 0;
        m_EffectHp = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_SkillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        m_EffectCountMax = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        m_BossInvincibleTime = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        if (data.ValueE.Count > 0)
        {
            m_StopAI = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        }
        if (data.ValueF.Count > 0)
        {
            m_DunfuBuffId = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        }
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onHPChange, (args) =>
        {
            if (owner.GetEntityData().GetHP() <= m_EffectHp)
            {
                owner.buffController.RemoveInPassiveBuff();
                if (m_CurrEffectCount < m_EffectCountMax && owner.HasSkill(m_SkillId))
                {
                    bool sucess = owner.UseSkill(m_SkillId,true);
                    if (sucess)
                    {
                        if (m_DunfuBuffId != -1)
                        {
                            owner.buffController.TryAddBuff(m_DunfuBuffId, -1, level);
                        }

                        if (m_StopAI == 1)
                        {
                            owner.aiManager.Stop();
                        }

                        owner.protectManager.SetEnable(false);
                        owner.GetEntityData().SetHP(1);
                        m_CurrEffectCount += 1;
                        if (m_BossInvincibleTime > 0)
                        {
                            owner.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE, m_BossInvincibleTime);
                        }
                    }
                }
            }
        });
    }
}
