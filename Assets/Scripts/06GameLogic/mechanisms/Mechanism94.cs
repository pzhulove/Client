using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 根据武器强化等级添加Buff(只可用于一个部位上)

public class Mechanism94 : BeMechanism
{
    public Mechanism94(int id, int level) : base(id, level) { }

    protected int wearSlotType = 0;
    protected int buffId = 0;

    public override void OnInit()
    {
        wearSlotType = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        buffId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        CheckEquipStrength();
        handleA = owner.RegisterEventNew(BeEventType.OnChangeWeaponEnd, (args) => 
        {
            CheckEquipStrength();
        });
    }

    private void CheckEquipStrength()
    {
        BeBuff buff = owner.buffController.HasBuffByID(buffId);
        if (buff != null)
            owner.buffController.RemoveBuff(buff);
        int equipStrength = BeUtility.GetEquipsStrengthBySlot(owner, wearSlotType);
        if (equipStrength > 0)
            owner.buffController.TryAddBuff(buffId, int.MaxValue, equipStrength);
    }

    protected void RemoveBuff()
    {
        owner.buffController.RemoveBuff(buffId);
    }

    public override void OnFinish()
    {
        RemoveBuff();
    }
}
