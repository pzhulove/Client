using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using GameClient;
using System;

/// <summary>
/// 炫纹融合
/// 
/// 1.可中断普攻释放技能
/// 2.此技能可蓄力释放，蓄力时间上限0.5s,蓄力成功增加基本攻击力
/// 3.施法成功命中后对自身增加智力和魔法暴击率的buff
/// 1.立即使用时，会将两个炫纹融合成一个大范围爆炸炫纹对周围造成爆炸
/// 2.蓄力成功后，会将两个炫纹融合成一个旋转炫纹在目标身上，对目标造成多段伤害，此伤害无被击状态
/// 3.保留炫纹属性，可以融合任意炫纹
/// 7.buff增加的智力/buff增加的魔法暴击率/buff持续时间/炫纹融合的基本攻击力/蓄力成功时的基本攻击力 随技能等级提升
/// 8.不考虑炫纹融合的组合形式，蓄力成功后的旋转炫纹为之前两个炫纹攻击力之和的n%(具体有徐鑫伟给到公式写在代码内)
/// </summary>
public class Skill2310 : BeSkill
{
    //创建的实体ID
    protected int[] m_EntityIdArr = new int[4]; //创建的炫纹融合实体ID列表（(非蓄力PVE|非蓄力PVP|蓄力PVE|蓄力PVP)）
    protected int[] m_CheckHurtIdArr = new int[4];   //用来检测创建实体的触发效果ID列表（(非蓄力PVE|非蓄力PVP|蓄力PVE|蓄力PVP)）
    //需要替换的触发效果ID
    protected int[] m_BaseHurtIdArr = new int[4];   //基础替换的触发效果ID列表（(非蓄力PVE|非蓄力PVP|蓄力PVE|蓄力PVP)）
    protected int[] m_NormalHurtIdArr = new int[(int)Mechanism2072.ChaserType.Max];
    protected int[] m_BigHurtIdArr = new int[(int)Mechanism2072.ChaserType.Max];
    protected int[] m_TargetAddMechanismIdArr = new int[2];

    private readonly Vector3 mEndPosOffset = new Vector3(0, 2f, 0);
    private Mechanism2072 mChaserMgr = null;
    private readonly int ChaserCount = 2;

    private List<Mechanism2072.ChaserData> m_ChaserList = null;
    protected List<BeEvent.BeEventHandleNew> m_HandleNewList = new List<BeEvent.BeEventHandleNew>();

    protected BeEvent.BeEventHandleNew m_EntityRemoveHandle = null;
    protected VFactor m_AddDamagePercent = VFactor.zero;
    private int m_ChaserLevel = 1;

#if !LOGIC_SERVER
    protected string[] m_EffectPathArr = new string[(int)Mechanism2072.ChaserType.Max];
    public string[] EffectPathArr { get { return m_EffectPathArr; } }
#endif

    public Skill2310(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        m_EntityIdArr[0] = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        m_EntityIdArr[1] = TableManager.GetValueFromUnionCell(skillData.ValueA[1], level);
        m_EntityIdArr[2] = TableManager.GetValueFromUnionCell(skillData.ValueA[2], level);
        m_EntityIdArr[3] = TableManager.GetValueFromUnionCell(skillData.ValueA[3], level);

        m_BaseHurtIdArr[0] = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        m_BaseHurtIdArr[1] = TableManager.GetValueFromUnionCell(skillData.ValueB[1], level);
        m_BaseHurtIdArr[2] = TableManager.GetValueFromUnionCell(skillData.ValueB[2], level);
        m_BaseHurtIdArr[3] = TableManager.GetValueFromUnionCell(skillData.ValueB[3], level);

        for (int i = 0; i < skillData.ValueC.Count && i < m_NormalHurtIdArr.Length; i++)
        {
            m_NormalHurtIdArr[i] = TableManager.GetValueFromUnionCell(skillData.ValueC[i], level);
        }

        for (int i = 0; i < skillData.ValueD.Count && i < m_BigHurtIdArr.Length; i++)
        {
            m_BigHurtIdArr[i] = TableManager.GetValueFromUnionCell(skillData.ValueD[i], level);
        }

        m_TargetAddMechanismIdArr[0] = TableManager.GetValueFromUnionCell(skillData.ValueE[0], level);
        m_TargetAddMechanismIdArr[1] = TableManager.GetValueFromUnionCell(skillData.ValueE[1], level);

        m_CheckHurtIdArr[0] = TableManager.GetValueFromUnionCell(skillData.ValueF[0], level);
        m_CheckHurtIdArr[1] = TableManager.GetValueFromUnionCell(skillData.ValueF[1], level);
        m_CheckHurtIdArr[2] = TableManager.GetValueFromUnionCell(skillData.ValueF[2], level);
        m_CheckHurtIdArr[3] = TableManager.GetValueFromUnionCell(skillData.ValueF[3], level);
        
        int damagePercent = BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(skillData.ValueG[1], level) : TableManager.GetValueFromUnionCell(skillData.ValueG[0], level);
        m_AddDamagePercent = VFactor.NewVFactor(damagePercent, GlobalLogic.VALUE_1000);
    }

    public override void OnPostInit()
    {
        base.OnPostInit();
        InitEffectData();
        GetChaserMgr();
    }

    public override bool CanUseSkill()
    {
        bool baseCanUse = base.CanUseSkill();
        // 技能时序问题，可能取不到
        GetChaserMgr();
        if (mChaserMgr == null)
            return false;
        
        return mChaserMgr.GetChaserCount() >= ChaserCount && baseCanUse;
    }

    /// <summary>
    /// 初始化炫纹特效数据
    /// </summary>
    protected void InitEffectData()
    {
#if !LOGIC_SERVER
        m_EffectPathArr[0] = "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_rh_01_wu";
        m_EffectPathArr[1] = "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_rh_01_guang";
        m_EffectPathArr[2] = "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_rh_01_fire";
        m_EffectPathArr[3] = "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_rh_01_ice";
        m_EffectPathArr[4] = "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_rh_01_an";
#endif
    }

    protected void RegisterHandle()
    {
        if (owner == null)
            return;
        m_HandleNewList.Add(owner.RegisterEventNew(BeEventType.onChangeMagicElement, RegsiterChangeMagicElement));
        m_HandleNewList.Add(owner.RegisterEventNew(BeEventType.onChangeMagicElementList, RegisterChangeMagicElementList));
        m_HandleNewList.Add(owner.RegisterEventNew(BeEventType.onAfterFinalDamageNew, HitOther));
        //m_HandleList.Add(owner.RegisterEvent(BeEventType.onAfterFinalDamageNew, HitOther));
        m_HandleNewList.Add(owner.RegisterEventNew(BeEventType.onReplaceHurtTableDamageData, ReplaceHurtTableDamageData));
    }

    public override void OnStart()
    {
        GetChaserLevel();
        base.OnStart();
        StartChaserMix();
        Mechanism2072.AddBuff(m_ChaserList, battleType, owner, m_ChaserLevel);
        RemoveHandle();
        RegisterHandle();
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
    /// 开始炫纹融合
    /// </summary>
    private void StartChaserMix()
    {
        GetChaserMgr();
        if (mChaserMgr == null)
            return;
        
        if (m_ChaserList != null)
        {
            GamePool.ListPool<Mechanism2072.ChaserData>.Release(m_ChaserList);
            m_ChaserList = null;
        }
        m_ChaserList = GamePool.ListPool<Mechanism2072.ChaserData>.Get();
        m_ChaserList.Clear();
        mChaserMgr.LaunchChaser(ChaserCount, m_ChaserList);
        if (m_ChaserList != null)
        {
            mChaserMgr.ReduceChaserCount(m_ChaserList.Count);
        }
        CompoundChaser();
    }

    /// <summary>
    /// 修改属性攻击类型
    /// </summary>
    /// <param name="args"></param>
    protected void RegsiterChangeMagicElement(BeEvent.BeEventParam param)
    {
        int hurtId = param.m_Int2;
        int baseIndex = Array.IndexOf(m_BaseHurtIdArr, hurtId);
        if (baseIndex < 0)
            return;

        //取属强最高值
        List<MagicElementType> magicElementTypeList = GamePool.ListPool<MagicElementType>.Get();
        GetMagicElementList(magicElementTypeList);
        MagicElementType type = MagicElementType.NONE;
        int maxElementValue = -1;
        for (int i = 1; i < (int)MagicElementType.MAX; ++i)
        {
            int element = owner.attribute.battleData.magicELements[i];
            int value = owner.attribute.battleData.magicElementsAttack[i];
            if (element > 0 || magicElementTypeList.Contains((MagicElementType)i) && value > maxElementValue)
            {
                maxElementValue = value;
                type = (MagicElementType)(i);
            }
        }
        param.m_Int = (int)type;
        GamePool.ListPool<MagicElementType>.Release(magicElementTypeList);
    }

    /// <summary>
    /// 添加属性攻击类型
    /// </summary>
    /// <param name="param"></param>
    protected void RegisterChangeMagicElementList(BeEvent.BeEventParam param)
    {
        int hurtId = param.m_Int;
        int baseIndex = Array.IndexOf(m_BaseHurtIdArr, hurtId);
        if (baseIndex < 0)
            return;
        List<int> list = (List<int>)param.m_Obj;

        List<MagicElementType> magicElementTypeList = GamePool.ListPool<MagicElementType>.Get();
        GetMagicElementList(magicElementTypeList);
        for(int i=0; i<magicElementTypeList.Count; i++)
        {
            if (!list.Contains((int)magicElementTypeList[i]))
            {
                list.Add((int)magicElementTypeList[i]);
            }
        }
        GamePool.ListPool<MagicElementType>.Release(magicElementTypeList);
    }

    /// <summary>
    /// 监听攻击命中
    /// </summary>
    protected void HitOther(BeEvent.BeEventParam param)
    {
        int hurtId = param.m_Int2;
        int baseIndex = Array.IndexOf(m_CheckHurtIdArr, hurtId);
        if (baseIndex < 0)
            return;

        var target = param.m_Obj as BeActor;
        if (target == null)
            return;
        VInt3 pos = param.m_Vint3;
        CreateEntity(baseIndex, pos);

        if (baseIndex > 1)
        {
            AddChargeMechanism(hurtId, target);
        }
    }

    /// <summary>
    /// 创建实体
    /// </summary>
    protected void CreateEntity(int hurtIdIndex,VInt3 pos)
    {
        if(owner == null)
            return;
        
        int entityId = m_EntityIdArr[hurtIdIndex];
        owner.AddEntity(entityId, pos, level);
    }

    /// <summary>
    /// 蓄力状态下 添加多段伤害机制
    /// </summary>
    protected void AddChargeMechanism(int hurtId, BeActor target)
    {
        //给目标添加被击特效
        AddEffectToTarget(m_ChaserList, target);

        int[] hurtIdArr = GetChaserMixHurtIdArr();

        int mechanismId = BattleMain.IsModePvP(battleType) ? m_TargetAddMechanismIdArr[1] : m_TargetAddMechanismIdArr[0];
        var mechansm = target.AddMechanism(mechanismId, level) as Mechanism2088;
        if (mechansm != null)
        {
            mechansm.InitData(hurtIdArr, owner);
        }
    }

    /// <summary>
    /// 根据炫纹融合类型修改伤害数值
    /// </summary>
    /// <param name="param"></param>
    protected void ReplaceHurtTableDamageData(BeEvent.BeEventParam param)
    {
        int hurtId = param.m_Int;
        int baseIndex = Array.IndexOf(m_BaseHurtIdArr, hurtId);
        if (baseIndex < 0 || baseIndex > 1)
            return;
        int[] hurtIdArr = GetChaserMixHurtIdArr();
        if (hurtIdArr == null)
            return;

        int damageValue = 0;
        VPercent damagePervent = VPercent.zero;
        bool isPvPMode = BattleMain.IsModePvP(battleType);
        for(int i=0;i< hurtIdArr.Length; i++)
        {
            ProtoTable.EffectTable hurtData = TableManager.instance.GetTableItem<ProtoTable.EffectTable>(hurtIdArr[i]);
            damageValue += TableManager.GetValueFromUnionCell(isPvPMode ? hurtData.DamageFixedValuePVP : hurtData.DamageFixedValue, m_ChaserLevel);
            damagePervent += TableManager.GetValueFromUnionCell(isPvPMode ? hurtData.DamageRatePVP : hurtData.DamageRate, m_ChaserLevel);
        }
        param.m_Int2 = damageValue *  m_AddDamagePercent;
        var addFactor = damagePervent.precent * m_AddDamagePercent;
        param.m_Percent = new VPercent(addFactor.single);
        
    }

    /// <summary>
    /// 获取融合炫纹类型对应的触发效果ID列表
    /// </summary>
    protected int[] GetChaserMixHurtIdArr()
    {
        int[] hurtIdArr = new int[2];
        if (m_ChaserList == null)
        {
            Logger.LogErrorFormat("炫纹融合数据为空，请检查");
            return null;
        }
        for(int i = 0; i < m_ChaserList.Count; i++)
        {
            var chaserData = m_ChaserList[i];
            int typeIndex = (int)chaserData.type;
            int sizeType = (int)chaserData.sizeType;
            hurtIdArr[i] = sizeType == 0 ? m_NormalHurtIdArr[typeIndex] : m_BigHurtIdArr[typeIndex];
        }
        return hurtIdArr;
    }

    /// <summary>
    /// 获取攻击属性列表
    /// </summary>
    /// <param name="list"></param>
    protected void GetMagicElementList(List<MagicElementType> list)
    {
        list.Clear();
        if (m_ChaserList == null)
        {
            Logger.LogErrorFormat("炫纹融合数据为空，请检查");
            return;
        }
        for (int i = 0; i < m_ChaserList.Count; i++)
        {
            var chaserData = m_ChaserList[i];
            int type = (int)chaserData.type;
            list.Add((MagicElementType)type);
        }
    }

    /// <summary>
    /// 融合炫纹，飞行动画
    /// </summary>
    private void CompoundChaser()
    {
#if !LOGIC_SERVER
        if(m_ChaserList == null)
            return;
			
        float speed = 1f;
        if (skillSpeed > 0)
        {
            speed = 1000f / skillSpeed;
        }
        float duration = 0.5f * speed;
        var quence = DOTween.Sequence();
        if (quence == null)
            return;
			
        for (int i = 0; i < m_ChaserList.Count; i++)
        {
            var data = m_ChaserList[i];
            if (data != null && data.effect != null && data.effect.GetRootNode() != null && owner != null && owner.m_pkGeActor != null)
            {
                quence.Join(data.effect.GetRootNode().transform.DOLocalMove(mEndPosOffset, duration).OnComplete(() =>
                {
                    if (data.effect != null && owner != null && owner.m_pkGeActor != null)
                    {
                        owner.m_pkGeActor.DestroyEffect(data.effect);
                    }
                }));    
            }
        }
        //quence.OnComplete(ReduceChaser);
        quence.Play();
#endif

        //owner.delayCaller.DelayCall(duration, ReduceChaser);
    }

    /// <summary>
    /// 给炫纹融合的目标添加一个特效
    /// </summary>
    /// <param name="list">融合的炫纹类型列表</param>
    /// <param name="actor">目标</param>
    public void AddEffectToTarget(List<Mechanism2072.ChaserData> list,BeActor actor)
    {
#if !LOGIC_SERVER
        if (actor == null)
            return;
        if (actor.m_pkGeActor == null)
            return;
        for(int i=0; i < list.Count; i++)
        {
            int chaserType = (int)list[i].type;
            var path = m_EffectPathArr[chaserType];
            GeEffectEx effect = actor.m_pkGeActor.CreateEffect(path, "[actor]Orign", 3400, Vec3.zero, 1, 1, true, actor.GetFace(), EffectTimeType.SYNC_ANIMATION);
            if (effect != null)
            {
                if (i > 0)
                    effect.SetLocalRotation(Quaternion.Euler(new Vector3(0, 0, 180)));
                Vector3 initPos = new Vector3(0, actor.m_pkGeActor.GetOverHeadPosition().y / 2, 0);
                effect.SetLocalPosition(initPos);

                actor.delayCaller.DelayCall(3400, () =>
                {
                    if (effect != null)
                    {
                        effect.Remove();
                    }
                });
            }
        }
#endif
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
    }
}
