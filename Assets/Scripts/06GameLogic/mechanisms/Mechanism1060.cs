using System;
using System.Collections.Generic;
//当进入战斗后，记录场上所有敌人的出生时的防御力；						
//当攻击敌人时，计算敌人当前的防御力，然后拿出生时的防御力-敌人当前的防御力，						
//然后根据计算点数，计算出需要增加的伤害
//公式为：（出生防御力-当前防御力）/计算点数=增加的伤害

public class Mechanism1060 : BeMechanism
{
    VFactor addValueFactor = VFactor.zero;
    public Mechanism1060(int mid, int lv) : base(mid, lv)
    {

    }
    public override void OnInit()
    {
        int factor = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        addValueFactor = new VFactor(factor,GlobalLogic.VALUE_1000);
    }
    public override void OnStart()
    {
        base.OnStart();
        if (owner == null) return;
        handleA = owner.RegisterEventNew(BeEventType.onAfterFinalDamageNew, onDamage);
        //handleA = owner.RegisterEvent(BeEventType.onAfterFinalDamageNew,onDamage);
    }
    private void onDamage(GameClient.BeEvent.BeEventParam param)
    {
        var target = param.m_Obj as BeActor;
        if (target == null) return;
        if (!target.IsMonster()) return;
        var damageType = (ProtoTable.EffectTable.eDamageType)param.m_Int3;
        //var vals = (int[])args[0];
        int damage = param.m_Int;
        if (damageType == ProtoTable.EffectTable.eDamageType.MAGIC)
        {
            int magicDefenceDelta = target.GetEntityData().battleData.initMagicDefence - target.GetEntityData().battleData.magicDefence;
            if (magicDefenceDelta > 0)
            {
                var addDamageValue = magicDefenceDelta * addValueFactor;
                damage += addDamageValue;
            }

        }
        else
        {
            int defenceDelta = target.GetEntityData().battleData.initDefence - target.GetEntityData().battleData.defence;
            if (defenceDelta > 0)
            {
                var addDamageValue = defenceDelta * addValueFactor;
                damage += addDamageValue;
            }
        }
        param.m_Int = damage;
    }

}

