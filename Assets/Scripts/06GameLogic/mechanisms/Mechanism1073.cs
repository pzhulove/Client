using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 召唤兽的Buff附加伤害、伤害增加、暴击伤害增加继承玩家的
/// </summary>
public class Mechanism1073 : BeMechanism
{
    public Mechanism1073(int id, int level) : base(id, level) { }

    protected VFactor[] percentArr = new VFactor[3];    //Buff附加伤害系数|伤害增加系数|暴击伤害系数

    public override void OnInit()
    {
        base.OnInit();
        
        for(int i = 0; i < data.ValueA.Count; i++)
        {
            percentArr[i] = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueA[i], level), GlobalLogic.VALUE_1000);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onChangeSummonMonsterAttach, ChangeSummonMonsterAttach);
        handleB = owner.RegisterEventNew(BeEventType.onChangeSummonMonsterAddDamage, ChangeSummonMonsterAddDamage);
        handleC = owner.RegisterEventNew(BeEventType.onChangeSummonMonsterAddCritiDamage, ChangeSummonMonsterAddCritiDamage);
    }

    /// <summary>
    /// 修改召唤兽Buff附加伤害
    /// </summary>
    protected void ChangeSummonMonsterAttach(BeEvent.BeEventParam args)
    {
        BeEntity summoner = owner.GetOwner();
        if (summoner == null || summoner.attribute == null || summoner.attribute.battleData == null)
            return;

        List<AddDamageInfo> attachAddDamageFixNew = (List<AddDamageInfo>)args.m_Obj;
        List<AddDamageInfo> attachDamagePercentNew = (List<AddDamageInfo>)args.m_Obj2;

        //附加伤害固定值
        for (int i=0; i< summoner.attribute.battleData.attachAddDamageFix.Count; i++)
        {
            AddDamageInfo damageInfo = new AddDamageInfo();
            damageInfo.attackType = summoner.attribute.battleData.attachAddDamageFix[i].attackType;
            damageInfo.value = summoner.attribute.battleData.attachAddDamageFix[i].value * percentArr[0];
            attachAddDamageFixNew.Add(damageInfo);
        }

        //附加伤害百分比
        for(int i = 0; i < summoner.attribute.battleData.attachAddDamagePercent.Count; i++)
        {
            AddDamageInfo damageInfo = new AddDamageInfo();
            damageInfo.attackType = summoner.attribute.battleData.attachAddDamagePercent[i].attackType;
            damageInfo.value = summoner.attribute.battleData.attachAddDamagePercent[i].value * percentArr[0];
            attachDamagePercentNew.Add(damageInfo);
        }
    }

    /// <summary>
    /// 修改召唤兽伤害加成
    /// </summary>
    protected void ChangeSummonMonsterAddDamage(BeEvent.BeEventParam args)
    {
        BeEntity summoner = owner.GetOwner();
        if (summoner == null || summoner.attribute == null || summoner.attribute.battleData == null)
            return;

        List<AddDamageInfo> addDamageFixFixNew = (List<AddDamageInfo>)args.m_Obj;
        List<AddDamageInfo> addDamagePercentNew = (List<AddDamageInfo>)args.m_Obj2;

        //伤害加成固定值
        for (int i = 0; i < summoner.attribute.battleData.addDamageFix.Count; i++)
        {
            AddDamageInfo damageInfo = new AddDamageInfo();
            damageInfo.attackType = summoner.attribute.battleData.addDamageFix[i].attackType;
            damageInfo.value = summoner.attribute.battleData.addDamageFix[i].value * percentArr[1];
            addDamageFixFixNew.Add(damageInfo);
        }

        //伤害加成百分比
        for (int i = 0; i < summoner.attribute.battleData.addDamagePercent.Count; i++)
        {
            AddDamageInfo damageInfo = new AddDamageInfo();
            damageInfo.attackType = summoner.attribute.battleData.addDamagePercent[i].attackType;
            damageInfo.value = summoner.attribute.battleData.addDamagePercent[i].value * percentArr[1];
            addDamagePercentNew.Add(damageInfo);
        }
    }

    /// <summary>
    /// 修改召唤兽暴击伤害加成
    /// </summary>
    protected void ChangeSummonMonsterAddCritiDamage(BeEvent.BeEventParam args)
    {
        BeEntity summoner = owner.GetOwner();
        if (summoner == null || summoner.attribute == null || summoner.attribute.battleData == null)
            return;
        /*int[] critiDamageArrNew = (int[])args[0];
        critiDamageArrNew[0] += summoner.attribute.battleData.criticalPercent * percentArr[2];*/
        args.m_Int += summoner.attribute.battleData.criticalPercent * percentArr[2];
    }
}
