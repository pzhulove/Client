using System.Collections.Generic;
using GameClient;
using System;

/// <summary>
/// 力法被动
///
/// 功能：使用破军或龙刀时，
///      1.有概率消耗0-3个炫纹给受击的敌人加成炫纹合成计算的伤害，
///      2.在这2个技能开始时添加对应炫纹的buff，技能结束时删除
///      3.添加消耗炫纹的通用buffinfo
/// 
/// 配置: A：消耗概率
///       B：5个普通炫纹BuffID + 5个大炫纹BuffID  (无光火冰按)
///       C: 伤害系数
///       D：5个小炫纹伤害计算表 + 5个大炫纹伤害计算表 (无光火冰按)
///       E: 5个消耗炫纹通用buffInfo
///       F: 特效BuffID
///       G: 监听的HurtID列表
/// </summary>
public class Skill2320 : BeSkill
{
    // A：消耗概率
    private int m_UserChaserPro = 0;
    // B：5个BuffID, 5
    private int[] m_NormalChaserBuffId = new int[(int) Mechanism2072.ChaserType.Max];
    // B:5个大炫纹BuffID
    private int[] m_BigChaserBuffId = new int[(int) Mechanism2072.ChaserType.Max];
    // C:伤害系数
    private VFactor m_DamageRate;
    // D：5个小炫纹伤害计算表
    private int[] m_NormalHurtIdArr = new int[(int) Mechanism2072.ChaserType.Max];
    // D：5个大炫纹伤害计算表
    private int[] m_BigHurtIdArr = new int[(int) Mechanism2072.ChaserType.Max];
    // E: 5个消耗炫纹通用buffInfo
    private int[] m_CommonBuffInfoId = new int[(int) Mechanism2072.ChaserType.Max];
    // F: 特效BuffID
    private int m_EffectBuffId = 0;
    // G: 监听的HurtID列表
    private HashSet<int> m_targetHurtID = new HashSet<int>();

    private ProtoTable.EffectTable[] m_CacheNormalChaserTable = new ProtoTable.EffectTable[(int) Mechanism2072.ChaserType.Max];
    private ProtoTable.EffectTable[] m_CacheBigChaserTable = new ProtoTable.EffectTable[(int) Mechanism2072.ChaserType.Max];
    
    private Mechanism2072 mChaserMgr = null;
    private List<Mechanism2072.ChaserData> m_ChaserList = new List<Mechanism2072.ChaserData>();
    
    protected List<IBeEventHandle> m_HandleList = new List<IBeEventHandle>();
    protected List<BeEvent.BeEventHandleNew> m_HandleNewList = new List<BeEvent.BeEventHandleNew>();
    protected ProtoTable.EffectTable[] m_MixHurtTableList;
    private int m_ChaserLevel = 1;

    public Skill2320(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        
        m_UserChaserPro = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        if ((m_NormalChaserBuffId.Length + m_BigChaserBuffId.Length) != skillData.ValueB.Count)
        {
            Logger.LogError("Skill2320 ValueB Config Error");
        }
        else
        {
            for (int i = 0; i < m_NormalChaserBuffId.Length; i++)
            {
                m_NormalChaserBuffId[i] = TableManager.GetValueFromUnionCell(skillData.ValueB[i], level);
            }
            for (int i = 0; i < m_BigChaserBuffId.Length; i++)
            {
                m_BigChaserBuffId[i] = TableManager.GetValueFromUnionCell(skillData.ValueB[i + m_NormalChaserBuffId.Length], level);
            }
        }

        m_DamageRate = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(skillData.ValueC[0], level), GlobalLogic.VALUE_1000);
        if ((m_NormalHurtIdArr.Length + m_BigHurtIdArr.Length) != skillData.ValueD.Count)
        {
            Logger.LogError("Skill2320 ValueD Config Error");
        }
        else
        {
            for (int i = 0; i < m_NormalHurtIdArr.Length; i++)
            {
                m_NormalHurtIdArr[i] = TableManager.GetValueFromUnionCell(skillData.ValueD[i], level);
            }
            for (int i = 0; i < m_BigHurtIdArr.Length; i++)
            {
                m_BigHurtIdArr[i] = TableManager.GetValueFromUnionCell(skillData.ValueD[i + m_NormalHurtIdArr.Length], level);
            }
        }

        for (int i = 0; i < skillData.ValueE.Count && i < m_CommonBuffInfoId.Length; i++)
        {
            m_CommonBuffInfoId[i] = TableManager.GetValueFromUnionCell(skillData.ValueE[i], level);
        }

        if (skillData.ValueF.Count > 0)
        {
            m_EffectBuffId = TableManager.GetValueFromUnionCell(skillData.ValueF[0], level);
        }

        for (int i = 0; i < skillData.ValueG.Count; i++)
        {
            m_targetHurtID.Add(TableManager.GetValueFromUnionCell(skillData.ValueG[i], level));
        }
    }

    public override void OnPostInit()
    {
        base.OnPostInit();
        // PVP 关闭
        if (BattleMain.IsModePvP(battleType))
            return;
        GetChaserMgr();
        RemoveHandle();
        RegisterHandle();
        CacheChaserHurtTable();
    }

    private void CacheChaserHurtTable()
    {
        for (int i = 0; i < m_NormalHurtIdArr.Length; i++)
        {
            m_CacheNormalChaserTable[i] = TableManager.instance.GetTableItem<ProtoTable.EffectTable>(m_NormalHurtIdArr[i]);
        }
        
        for (int i = 0; i < m_BigHurtIdArr.Length; i++)
        {
            m_CacheBigChaserTable[i] = TableManager.instance.GetTableItem<ProtoTable.EffectTable>(m_BigHurtIdArr[i]);
        }
    }

    protected void RegisterHandle()
    {
        if (owner == null)
            return;
        
        m_HandleList.Add(owner.RegisterEventNew(BeEventType.onCastSkill, OnSkillStart));
        m_HandleList.Add(owner.RegisterEventNew(BeEventType.onSkillCancel, OnSkillFinish));
        m_HandleList.Add(owner.RegisterEventNew(BeEventType.onCastSkillFinish, OnSkillFinish));
        
        m_HandleNewList.Add(owner.RegisterEventNew(BeEventType.onReplaceHurtTableDamageData, ReplaceHurtTableDamageData));
        
    }

    private void OnSkillStart(BeEvent.BeEventParam args)
    {
        // int skillId = (int) args[0];
        int skillId = args.m_Int;
        if (skillId == 2313 || skillId == 2309)
        {
            GetChaserLevel();
            ReduceChaser();
            AddBuff();
            AddCommonBuff();
            m_MixHurtTableList = GetChaserMixHurtIdArr();
        }
    }
    
    private void OnSkillFinish(BeEvent.BeEventParam args)
    {
        // int skillId = (int) args[0];
        int skillId = args.m_Int;
        if (skillId == 2313 || skillId == 2309)
        {
            RemoveBuff();
        }
    }

    /// <summary>
    /// 消耗炫纹
    /// </summary>
    private void ReduceChaser()
    {
        m_ChaserList.Clear();
        GetChaserMgr();
        if (mChaserMgr == null)
            return;

        int count = GetUseChaserCount();
        if(count <= 0)
            return;
       
        mChaserMgr.LaunchChaser(count, m_ChaserList);
        mChaserMgr.ReduceChaserCount(m_ChaserList.Count);

        DestroyChaserEffect();
    }

    /// <summary>
    /// N%的概率消耗1-3个炫纹
    /// </summary>
    /// <returns></returns>
    private int GetUseChaserCount()
    {
        int count = 0;
        if (mChaserMgr == null)
            return count;
 
        if (mChaserMgr.GetChaserCount() <= 0)
            return count;
        
        if (FrameRandom.Range1000() < m_UserChaserPro)
        {
            count = FrameRandom.InRange(0, Math.Min(3, mChaserMgr.GetChaserCount())) + 1;
        }
        return count;
    }
    
    /// <summary>
    /// 添加消耗炫纹的通用buff
    /// （该功能。本该放机制2072，合并融合与强压的消耗，但炫纹发射的消耗已经策划自己配好了）
    /// </summary>
    private void AddCommonBuff()
    {
        if(m_ChaserList.Count <= 0)
            return;
        
        for (int i = 0; i < m_ChaserList.Count; i++)
        {
            var type = (int)m_ChaserList[i].type;
            if (type < m_CommonBuffInfoId.Length)
                owner.buffController.TryAddBuffInfo(m_CommonBuffInfoId[type], owner, m_ChaserLevel);
        }
    }
    
    private void AddBuff()
    {
        if(m_ChaserList.Count <= 0)
            return;

        for (int i= 0; i < m_ChaserList.Count; i++)
        {
            var chaser = m_ChaserList[i];
            int buffId;
            if (chaser.sizeType == Mechanism2072.ChaseSizeType.Normal)
            {
                buffId = m_NormalChaserBuffId[(int) chaser.type];
            }
            else
            {
                buffId = m_BigChaserBuffId[(int) chaser.type];
            }
            owner.buffController.TryAddBuff(buffId, -1, m_ChaserLevel);
        }
        
        if(m_EffectBuffId > 0)
            owner.buffController.TryAddBuff(m_EffectBuffId, -1, level);
    }

    private void RemoveBuff()
    {
        if(m_ChaserList.Count <= 0)
            return;

        for (int i= 0; i < m_ChaserList.Count; i++)
        {
            var chaser = m_ChaserList[i];
            
            int buffId;
            if (chaser.sizeType == Mechanism2072.ChaseSizeType.Normal)
            {
                buffId = m_NormalChaserBuffId[(int) chaser.type];
            }
            else
            {
                buffId = m_BigChaserBuffId[(int) chaser.type];
            }
            owner.buffController.RemoveBuff(buffId);
        }
        
        if(m_EffectBuffId > 0)
            owner.buffController.RemoveBuff(m_EffectBuffId);
    }
    
    /// <summary>
    /// 取下炫纹发射的等级,如果没有技能取1级
    /// </summary>
    private void GetChaserLevel()
    {
        if (owner != null)
        {
            int level = owner.GetSkillLevel(2302);
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

    /// <summary>
    /// 根据炫纹类型修改伤害数值
    /// </summary>
    /// <param name="param"></param>
    protected void ReplaceHurtTableDamageData(BeEvent.BeEventParam param)
    {
        if(m_MixHurtTableList == null || m_MixHurtTableList.Length <= 0)
            return;

        int hurtId = param.m_Int;
        if(!m_targetHurtID.Contains(hurtId))
            return;

        int damageValue = 0;
        VPercent damagePervent = VPercent.zero;
        bool isPvPMode = BattleMain.IsModePvP(battleType);
        for(int i=0;i< m_MixHurtTableList.Length; i++)
        {
            var hurtData = m_MixHurtTableList[i];
            damageValue += TableManager.GetValueFromUnionCell(isPvPMode ? hurtData.DamageFixedValuePVP : hurtData.DamageFixedValue, m_ChaserLevel);
            damagePervent += TableManager.GetValueFromUnionCell(isPvPMode ? hurtData.DamageRatePVP : hurtData.DamageRate, m_ChaserLevel);
        }
        param.m_Int2 += damageValue * m_DamageRate;
        var addFactor = damagePervent.precent * m_DamageRate;
        param.m_Percent += new VPercent(addFactor.single);
        
    }

    /// <summary>
    /// 获取炫纹类型对应的触发效果ID列表
    /// </summary>
    protected ProtoTable.EffectTable[] GetChaserMixHurtIdArr()
    {
        if (m_ChaserList == null)
        {
            Logger.LogErrorFormat("力法被动数据为空，请检查");
            return null;
        }
        var hurtList = GamePool.ListPool<ProtoTable.EffectTable>.Get();
        for(int i = 0; i < m_ChaserList.Count; i++)
        {
            var chaserData = m_ChaserList[i];
            int typeIndex = (int)chaserData.type;
            int sizeType = (int)chaserData.sizeType;
            hurtList.Add(sizeType == 0 ? m_CacheNormalChaserTable[typeIndex] : m_CacheBigChaserTable[typeIndex]);
        }
        var result = hurtList.ToArray();
        GamePool.ListPool<ProtoTable.EffectTable>.Release(hurtList);
        return result;
    }

    private void GetChaserMgr()
    {
        if(owner == null)
            return ;

        if (mChaserMgr != null)
            return;
      
        var baseMech = owner.GetMechanism(Mechanism2072.ChaserMgrID);
        if (baseMech == null)
            return;
        
        var mechanism = baseMech as Mechanism2072;
        mChaserMgr = mechanism;
    }
    

    protected void RemoveHandle()
    {
        for(int i=0;i< m_HandleNewList.Count; i++)
        {
            if(m_HandleNewList[i] != null)
            {
                m_HandleNewList[i].Remove();
                m_HandleNewList[i] = null;
            }
        }
        m_HandleNewList.Clear();

        for(int i=0;i< m_HandleList.Count; i++)
        {
            if (m_HandleList[i] != null)
            {
                m_HandleList[i].Remove();
                m_HandleList[i] = null;
            }
        }
        m_HandleList.Clear();
    }
    
    private void DestroyChaserEffect()
    {
#if !LOGIC_SERVER
        for( int i = 0; i < m_ChaserList.Count; i++)
        {
            if (m_ChaserList[i].effect != null && owner.m_pkGeActor != null)
            {
                owner.m_pkGeActor.DestroyEffect(m_ChaserList[i].effect);
                m_ChaserList[i].effect = null;
            }
        }
#endif
    }
}
