using System.Collections.Generic;
using GameClient;

/// <summary>
/// 配置
/// A：生命系数(可不填，默认1000)
/// B：主角召召唤物的技能ID
/// </summary>
public class Mechanism1503 : BeMechanism
{
    private VFactor hpRate = VFactor.one;
    private BeActor actor;
    private List<IBeEventHandle> mHandleList = new List<IBeEventHandle>();
    private int mOwnerSkillId;
    private bool mNeedChangeSta = false;
	public Mechanism1503(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        if (data.ValueA.Count > 0)
        {
            hpRate = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        }

        mOwnerSkillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        hpRate = VFactor.one;
        actor = null;
        removeHandle();
        mOwnerSkillId = 0;
        mNeedChangeSta = false;
    }

    public override void OnStart()
    {
        if (owner.attribute != null && owner.attribute.battleData != null)
        {
            mNeedChangeSta = owner.attribute.battleData.GetNeedChangeSta();
            owner.attribute.battleData.SetNeedChangeSta(true);
            owner.attribute.SetAttributeValue(AttributeType.maxHp, owner.attribute.battleData.maxHp);
            SyncHpBar(owner);
        }
        actor = owner.GetOwner() as BeActor;
        if (actor != null)
        {
            AdjustSummonMonsterAttribute(actor, owner);
            SyncSummonSkillBuff();
            
            // 同步属性
            mHandleList.Add(actor.RegisterEventNew(BeEventType.OnChangeWeaponEnd, args => { AdjustAttribute(); }));
            mHandleList.Add(actor.RegisterEventNew(BeEventType.onAddBuff, args => { AdjustAttribute(); }));
            mHandleList.Add(actor.RegisterEventNew(BeEventType.onRemoveBuff, args => { AdjustAttribute(); }));
            mHandleList.Add(actor.RegisterEventNew(BeEventType.onChangeEquipEnd, args => { AdjustAttribute(); }));
            
            // 技能Buff同步
            mHandleList.Add(actor.RegisterEventNew(BeEventType.OnBuffAddSkillAttr, OnBuffAddSkillAttr));
            mHandleList.Add(actor.RegisterEventNew(BeEventType.OnBuffRemoveSkillAttr, OnBuffRemoveSkillAttr));
            
            // 附加伤害与增伤
            mHandleList.Add(owner.RegisterEventNew(BeEventType.onChangeSummonMonsterAttach, ChangeSummonMonsterAttach));
            mHandleList.Add(owner.RegisterEventNew(BeEventType.onChangeSummonMonsterAddDamage, ChangeSummonMonsterAddDamage));
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (owner.attribute != null && owner.attribute.battleData != null)
        {
            owner.attribute.battleData.SetNeedChangeSta(mNeedChangeSta);
        }

        removeHandle();

        actor = null;
    }

    void removeHandle()
    {
        for (int i = 0; i < mHandleList.Count; i++)
        {
            mHandleList[i].Remove();
        }
        mHandleList.Clear();
    }

    private void AdjustAttribute()
    {
        AdjustSummonMonsterAttribute(actor, owner);
    }

    private void OnBuffAddSkillAttr(BeEvent.BeEventParam param)
    {
        if(actor == null)
            return;
        
        if (((BeSkill) param.m_Obj).skillID == mOwnerSkillId) 
        {
            SyncSummonSkillBuff();
        }
    }
    
    private void OnBuffRemoveSkillAttr(BeEvent.BeEventParam param)
    {
        if(actor == null)
            return;
        
        if (((BeSkill) param.m_Obj).skillID == mOwnerSkillId) 
        {
            SyncSummonSkillBuff();
        }
    }

    void AdjustSummonMonsterAttribute(BeActor owner, BeActor monster)
    {
        if (owner == null || monster == null)
            return;

        // 四维
        monster.attribute.SetAttributeValue(AttributeType.baseAtk, owner.attribute.GetAttributeValue(AttributeType.baseAtk));
        monster.attribute.SetAttributeValue(AttributeType.baseInt, owner.attribute.GetAttributeValue(AttributeType.baseInt));
        monster.attribute.SetAttributeValue(AttributeType.sta, owner.attribute.GetAttributeValue(AttributeType.sta) * hpRate);
        monster.attribute.SetAttributeValue(AttributeType.spr, owner.attribute.GetAttributeValue(AttributeType.spr));
        
        // 攻击
        monster.attribute.SetAttributeValue(AttributeType.attack, owner.attribute.GetAttributeValue(AttributeType.attack));
        monster.attribute.SetAttributeValue(AttributeType.magicAttack, owner.attribute.GetAttributeValue(AttributeType.magicAttack));
        monster.attribute.SetAttributeValue(AttributeType.ignoreDefAttackAdd, owner.attribute.GetAttributeValue(AttributeType.ignoreDefAttackAdd));
        monster.attribute.SetAttributeValue(AttributeType.ignoreDefMagicAttackAdd, owner.attribute.GetAttributeValue(AttributeType.ignoreDefMagicAttackAdd));
        monster.attribute.SetAttributeValue(AttributeType.dex, owner.attribute.GetAttributeValue(AttributeType.dex));
        monster.attribute.SetAttributeValue(AttributeType.attackAddRate, owner.attribute.GetAttributeValue(AttributeType.attackAddRate));
        monster.attribute.SetAttributeValue(AttributeType.magicAttackAddRate, owner.attribute.GetAttributeValue(AttributeType.magicAttackAddRate));
        
        // 暴击
        monster.attribute.SetAttributeValue(AttributeType.ciriticalAttack, owner.attribute.GetAttributeValue(AttributeType.ciriticalAttack));
        monster.attribute.SetAttributeValue(AttributeType.ciriticalMagicAttack, owner.attribute.GetAttributeValue(AttributeType.ciriticalMagicAttack));
        monster.attribute.SetAttributeValue(AttributeType.criticalPercent, owner.attribute.GetAttributeValue(AttributeType.criticalPercent));
        
        // 独立攻击/固定攻击
        monster.attribute.SetAttributeValue(AttributeType.baseIndependence, owner.attribute.GetAttributeValue(AttributeType.baseIndependence));
        monster.attribute.SetAttributeValue(AttributeType.ingoreIndependence, owner.attribute.GetAttributeValue(AttributeType.ingoreIndependence));
        
        // MaxHp
        //monster.attribute.SetAttributeValue(AttributeType.maxHp, owner.attribute.GetAttributeValue(AttributeType.maxHp) * hpRate);
        
        // 属强
        for (int i = 1; i < (int)MagicElementType.MAX; i++)
            monster.attribute.battleData.magicElementsAttack[i] = owner.attribute.battleData.magicElementsAttack[i];

        for (int i = 1; i < (int)MagicElementType.MAX; i++)
            monster.attribute.battleData.magicELements[i] = owner.attribute.battleData.magicELements[i];

        SyncHpBar(monster);
    }

    private void SyncSummonSkillBuff()
    {
        var ownerSkill = actor.GetSkill(mOwnerSkillId);
        if(ownerSkill == null)
            return;
        
        var skills = owner.skillController.GetSkillList();
        for (int i = 0; i < skills.Count; i++)
        {
            BeSkill skill = skills[i];
            skill.hitRateAdd = ownerSkill.hitRateAdd;
            skill.criticalHitRateAdd = ownerSkill.criticalHitRateAdd;
            skill.attackAddRate = ownerSkill.attackAddRate;
            skill.attackAdd = ownerSkill.attackAdd;
            skill.attackAddFix = ownerSkill.attackAddFix;
        }
    }
    
    /// <summary>
    /// 修改召唤兽Buff附加伤害
    /// </summary>
    protected void ChangeSummonMonsterAttach(BeEvent.BeEventParam args)
    {
        BeEntity summoner = actor;
        if (summoner == null || summoner.attribute == null || summoner.attribute.battleData == null)
            return;

        List<AddDamageInfo> attachAddDamageFixNew = (List<AddDamageInfo>)args.m_Obj;
        List<AddDamageInfo> attachDamagePercentNew = (List<AddDamageInfo>)args.m_Obj2;

        //附加伤害固定值
        for (int i=0; i< summoner.attribute.battleData.attachAddDamageFix.Count; i++)
        {
            AddDamageInfo damageInfo = new AddDamageInfo();
            damageInfo.attackType = summoner.attribute.battleData.attachAddDamageFix[i].attackType;
            damageInfo.value = summoner.attribute.battleData.attachAddDamageFix[i].value ;
            attachAddDamageFixNew.Add(damageInfo);
        }

        //附加伤害百分比
        for(int i = 0; i < summoner.attribute.battleData.attachAddDamagePercent.Count; i++)
        {
            AddDamageInfo damageInfo = new AddDamageInfo();
            damageInfo.attackType = summoner.attribute.battleData.attachAddDamagePercent[i].attackType;
            damageInfo.value = summoner.attribute.battleData.attachAddDamagePercent[i].value ;
            attachDamagePercentNew.Add(damageInfo);
        }
    }

    /// <summary>
    /// 修改召唤兽伤害加成
    /// </summary>
    protected void ChangeSummonMonsterAddDamage(BeEvent.BeEventParam args)
    {
        BeEntity summoner = actor;
        if (summoner == null || summoner.attribute == null || summoner.attribute.battleData == null)
            return;

        List<AddDamageInfo> addDamageFixFixNew = (List<AddDamageInfo>)args.m_Obj;
        List<AddDamageInfo> addDamagePercentNew = (List<AddDamageInfo>)args.m_Obj2;

        //伤害加成固定值
        for (int i = 0; i < summoner.attribute.battleData.addDamageFix.Count; i++)
        {
            AddDamageInfo damageInfo = new AddDamageInfo();
            damageInfo.attackType = summoner.attribute.battleData.addDamageFix[i].attackType;
            damageInfo.value = summoner.attribute.battleData.addDamageFix[i].value;
            addDamageFixFixNew.Add(damageInfo);
        }

        //伤害加成百分比
        for (int i = 0; i < summoner.attribute.battleData.addDamagePercent.Count; i++)
        {
            AddDamageInfo damageInfo = new AddDamageInfo();
            damageInfo.attackType = summoner.attribute.battleData.addDamagePercent[i].attackType;
            damageInfo.value = summoner.attribute.battleData.addDamagePercent[i].value;
            addDamagePercentNew.Add(damageInfo);
        }
    }

    private void SyncHpBar(BeActor monster)
    {
#if !LOGIC_SERVER
        if (monster.m_pkGeActor != null)
        {
            monster.m_pkGeActor.SyncHPBar();
        }
#endif
    }
}
