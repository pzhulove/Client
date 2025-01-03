using System.Collections.Generic;
using GameClient;
using ProtoTable;

/// <summary>
/// Dota拍拍熊机制
/// 在连续攻击同一目标会有伤害加深效果。对方头顶有特效标签。在攻击中断一段时间或攻击目标转移，则效果清0
/// </summary>
public class Mechanism1517 : BeMechanism
{
    public Mechanism1517(int mid, int lv) : base(mid, lv)
    {
    }

    struct ComboInfo
    {
        public int m_StartCombo;
        public int m_HurtID;
        public int m_EffectID;
    }
    
    private int m_AttackSkillId;
    private BeActor m_MarkActor;
    private int m_CurCombo;
    private List<ComboInfo> m_ComboData = new List<ComboInfo>();
    private BeActor m_LastAttacked;
    private GeEffectEx m_MarkEffect;
    private bool m_HasAttackMark = false;
    private ComboInfo m_LastComboInfo;
    
    private int m_MarkDuration;
    private int m_MarkTimeAcc;
    
    public override void OnInit()
    {
        base.OnInit();
        m_AttackSkillId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        for (int i = 0; i < data.ValueB.Count || i < data.ValueC.Count || i < data.ValueD.Count; i++)
        {
            var info = new ComboInfo();
            info.m_StartCombo = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            info.m_HurtID = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
            info.m_EffectID = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
            m_ComboData.Add(info);
        }
        
        m_MarkDuration = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
    }

    public override void OnReset()
    {
        m_MarkActor = null;
        m_CurCombo = 0;
        m_ComboData.Clear();
        m_LastAttacked = null;
        m_MarkEffect = null;
        m_HasAttackMark = false;
        m_LastComboInfo.m_StartCombo = 0;
        m_LastComboInfo.m_HurtID = 0;
        m_LastComboInfo.m_EffectID = 0;
        m_MarkTimeAcc = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onCastSkill, OnStartSkill);
        
        handleB = owner.RegisterEventNew(BeEventType.onSkillCancel, OnStartEnd);
        handleC = owner.RegisterEventNew(BeEventType.onCastSkillFinish, OnStartEnd);
        
        if(owner.CurrentBeScene != null)
            sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onHurtEntity, OnBeforeAttackOther);

        m_CurCombo = 0;
        m_MarkTimeAcc = 0;
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (m_MarkActor != null && m_MarkEffect != null)
        {
            if (m_MarkActor.IsDeadOrRemoved())
            {
                ResetMark();
            }
            else
            {
                m_MarkTimeAcc += deltaTime;
                if (m_MarkTimeAcc >= m_MarkDuration)
                {
                    ResetMark();
                }
            }
        }
    }

    private void OnStartSkill(BeEvent.BeEventParam param)
    {
        if (param.m_Int == m_AttackSkillId)
        {
            m_HasAttackMark = false;
            m_LastAttacked = null;
            m_LastComboInfo = GetCurComboInfo();
        }
    }
    
    private void OnBeforeAttackOther(BeEvent.BeEventParam param)
    {
        if(param.m_Obj != owner)
            return;

        var hurtData = param.m_Obj3 as EffectTable;
        if (hurtData != null && hurtData.SkillID == m_AttackSkillId)
        {
            var other = param.m_Obj2 as BeActor;
            if (m_MarkActor == null)
            {
                // 当之前没有标记怪
                DoCombo(param);
                DoMark(other);
                m_MarkActor = other;
            }
            else if (m_MarkActor == other)
            {
                // 打中了已经标记的怪
                DoCombo(param);
                DoMark(other);
            }

            m_LastAttacked = other;
        }
    }
    
    private void OnStartEnd(BeEvent.BeEventParam param)
    {
        if (param.m_Int == m_AttackSkillId)
        {
            // 当一次攻击完成，还没有打到标记怪。则攻击最后一个受击者
            if (!m_HasAttackMark)
            {
                if (m_LastAttacked != null) 
                {
                    m_CurCombo = 1;
                    DoMark(m_LastAttacked);
                    m_MarkActor = m_LastAttacked;
                }
                else
                {
                    //如果击空
                    ResetMark();
                }
            }
        }
    }

    private void DoCombo(BeEvent.BeEventParam param)
    {
        m_HasAttackMark = true;
        m_CurCombo++;
        var info = GetCurComboInfo();
        param.m_Int = info.m_HurtID;
        param.m_Obj3 = TableManager.GetInstance().GetTableItem<EffectTable>(info.m_HurtID);
    }

    private void DoMark(BeActor actor)
    {
        if(actor == null)
            return;

        m_MarkTimeAcc = 0;
        var info = GetCurComboInfo();
        if (m_MarkActor != actor)
        {
            DestroyMarkEffect();
            m_MarkEffect = actor.m_pkGeActor?.CreateEffect(info.m_EffectID, Vec3.zero);
        }
        else
        {
            if (info.m_EffectID != m_LastComboInfo.m_EffectID)
            {
                DestroyMarkEffect();
                m_MarkEffect = actor.m_pkGeActor?.CreateEffect(info.m_EffectID, Vec3.zero);
            }
        }
    }

    private void ResetMark()
    {
        DestroyMarkEffect();
        m_MarkActor = null;
        m_CurCombo = 0;
        m_MarkTimeAcc = 0;
    }

    private void DestroyMarkEffect()
    {
        if (m_MarkEffect != null)
        {
            m_MarkActor?.m_pkGeActor?.DestroyEffect(m_MarkEffect);
            m_MarkEffect = null;
        }
    }
    
    private ComboInfo GetCurComboInfo()
    {
        if (m_ComboData.Count <= 0)
        {
            Logger.LogErrorFormat("combo data error:please check table config B-C-D");
            return default;
        }
        
        var ret = default(ComboInfo);
        for (int i = 0; i < m_ComboData.Count; i++)
        {
            var info = m_ComboData[i];
            if (m_CurCombo >= info.m_StartCombo)
            {
                ret = info;
            }
        }

        return ret;
    }
    
}