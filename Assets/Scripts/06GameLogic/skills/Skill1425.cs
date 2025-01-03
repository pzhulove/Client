using System;
using GameClient;

public class Skill1425 : BeSkill
{
    public Skill1425(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }
    
    private int[] m_ChargingSkills;        // 充能技能
    private int[] m_ReplaceSkillPhases;    // 每格能量对应的技能阶段   
    private int m_OriginalSkillPhaseId;    // 被替换的技能阶段

    private int m_EnergyCount = 0;        // 当前能量值
    private int m_EnergyFullCount;        // 能量上限 

    public int EnergyCount
    {
        get => m_EnergyCount;
        set
        {
            m_EnergyCount = value;
            SetNvDaQiangEnergyBar(value);
        }
    }

    private int m_UseSkillPhaseId;
    private IBeEventHandle m_CastSkillHandle;
    
    public override void OnInit()
    {
        m_ChargingSkills = new int[skillData.ValueA.Count];
        for (int i = 0; i < skillData.ValueA.Count; i++)
        {
            m_ChargingSkills[i] = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
        }
        
        m_ReplaceSkillPhases = new int[skillData.ValueB.Count];
        for (int i = 0; i < skillData.ValueB.Count; i++)
        {
            m_ReplaceSkillPhases[i] = TableManager.GetValueFromUnionCell(skillData.ValueB[i], level);
        }

        m_OriginalSkillPhaseId = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        
        m_EnergyFullCount = m_ReplaceSkillPhases.Length - 1;
        InitNvDaQiangEnergyBar(m_EnergyFullCount);
        EnergyCount = 0;
    }

    public override void OnPostInit()
    {
        if (m_CastSkillHandle != null)
        {
            m_CastSkillHandle.Remove();
            m_CastSkillHandle = null;
        }
        m_CastSkillHandle = owner.RegisterEventNew(BeEventType.onCastSkill, OnStartSkill);
    }

    public override void OnStart()
    {
        UseEnergy();
        handleA = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, OnReplaceSkill);
    }

    private void UseEnergy()
    {
        m_UseSkillPhaseId = 0;
        if (EnergyCount >= m_ReplaceSkillPhases.Length)
        {
            Logger.LogErrorFormat("充能数异常 {0}, {1}", EnergyCount , m_ReplaceSkillPhases.Length);
            return;
        }
        m_UseSkillPhaseId = m_ReplaceSkillPhases[EnergyCount];
        EnergyCount = 0;
    }
    private void OnReplaceSkill(BeEvent.BeEventParam param)
    {
        if(m_UseSkillPhaseId <= 0)
            return;
        
        if (param.m_Int == m_OriginalSkillPhaseId)
        {
            param.m_Int = m_UseSkillPhaseId;
        }
    }

    private void OnStartSkill(BeEvent.BeEventParam param)
    {
        if (Array.IndexOf(m_ChargingSkills, param.m_Int) >= 0)
        {
            Charging();
        }
    }

    private void Charging()
    {
        if (m_EnergyCount >= m_EnergyFullCount)
        {
            // 充满了
        }
        else
        {
            EnergyCount++;
        }
    }

    private void InitNvDaQiangEnergyBar(int n)
    {
#if !LOGIC_SERVER
        if (owner != null && owner.isLocalActor)
        {
            var battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIProfession>();
            if (battleUI != null)
            {
                battleUI.InitNvDaQiangEnergyBar(n);
            }
        }
#endif
    }
    
    private void SetNvDaQiangEnergyBar(int times)
    {
#if !LOGIC_SERVER
        if (owner != null && owner.isLocalActor)
        {
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIProfession>();
            if (battleUI != null)
            {
                battleUI.SetNvDaQiangEnergyBar2(times);
            }
        }
#endif
    }
}

