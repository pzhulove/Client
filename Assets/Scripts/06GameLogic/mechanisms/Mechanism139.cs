using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

//召唤师的召唤兽属性随动
public class Mechanism139 : BeMechanism
{
    public Mechanism139(int mid, int lv) : base(mid, lv) { }

    public override void OnStart()
    {
        var actor = owner.GetOwner() as BeActor;
        if (actor != null)
        {
            handleA = actor.RegisterEventNew(BeEventType.OnChangeWeaponEnd, args =>
            {
                AdjustSummonMonsterAttribute(actor, owner);
            });
            handleB = actor.RegisterEventNew(BeEventType.onAddBuff, args =>
            {
                AdjustSummonMonsterAttribute(actor, owner);
            });
            handleC = actor.RegisterEventNew(BeEventType.onRemoveBuff, args =>
            {
                AdjustSummonMonsterAttribute(actor, owner);
            });
            handleD = actor.RegisterEventNew(BeEventType.onChangeEquipEnd, ChangeWeaponEnd);
        }
    }

    protected void ChangeWeaponEnd(BeEvent.BeEventParam param)
    {
        var actor = owner.GetOwner() as BeActor;
        if (actor == null)
            return;
        AdjustSummonMonsterAttribute(actor, owner);
    }

    void AdjustSummonMonsterAttribute(BeActor owner, BeActor monster)
    {
        if (owner == null || monster == null)
            return;

        monster.attribute.SetAttributeValue(AttributeType.attack, owner.attribute.GetAttributeValue(AttributeType.magicAttack) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.magicAttack, monster.attribute.monsterData), GlobalLogic.VALUE_100000));
        monster.attribute.SetAttributeValue(AttributeType.magicAttack, owner.attribute.GetAttributeValue(AttributeType.magicAttack) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.magicAttack, monster.attribute.monsterData), GlobalLogic.VALUE_100000));
        monster.attribute.SetAttributeValue(AttributeType.ignoreDefAttackAdd, owner.attribute.GetAttributeValue(AttributeType.ignoreDefMagicAttackAdd) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.ignoreDefMagicAttackAdd, monster.attribute.monsterData), GlobalLogic.VALUE_100000));
        monster.attribute.SetAttributeValue(AttributeType.ignoreDefMagicAttackAdd, owner.attribute.GetAttributeValue(AttributeType.ignoreDefMagicAttackAdd) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.ignoreDefMagicAttackAdd, monster.attribute.monsterData), GlobalLogic.VALUE_100000));
        monster.attribute.SetAttributeValue(AttributeType.baseAtk, owner.attribute.GetAttributeValue(AttributeType.baseInt) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.baseInt, monster.attribute.monsterData), GlobalLogic.VALUE_100000));
        monster.attribute.SetAttributeValue(AttributeType.baseInt, owner.attribute.GetAttributeValue(AttributeType.baseInt) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.baseInt, monster.attribute.monsterData), GlobalLogic.VALUE_100000));
        monster.attribute.SetAttributeValue(AttributeType.ciriticalAttack, owner.attribute.GetAttributeValue(AttributeType.ciriticalAttack) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.ciriticalAttack, monster.attribute.monsterData), GlobalLogic.VALUE_100000));
        monster.attribute.SetAttributeValue(AttributeType.ciriticalMagicAttack, owner.attribute.GetAttributeValue(AttributeType.ciriticalMagicAttack) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.ciriticalMagicAttack, monster.attribute.monsterData), GlobalLogic.VALUE_100000));
        monster.attribute.SetAttributeValue(AttributeType.baseIndependence, owner.attribute.GetAttributeValue(AttributeType.baseIndependence) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.baseIndependence, monster.attribute.monsterData), GlobalLogic.VALUE_100000));
        for (int i = 1; i < (int)MagicElementType.MAX; i++)
            monster.attribute.battleData.magicElementsAttack[i] = owner.attribute.battleData.magicElementsAttack[i] * VRate.Factor(GlobalLogic.VALUE_1000);

        if (monster.attribute.simpleMonsterID == 900 || monster.attribute.simpleMonsterID == 905 || monster.attribute.simpleMonsterID == 909)//哥布林、花妖、黑骑士特殊处理
        {
            for (int i = 1; i < (int)MagicElementType.MAX; i++)
                monster.attribute.battleData.magicELements[i] = owner.attribute.battleData.magicELements[i];
        }
    }
}
