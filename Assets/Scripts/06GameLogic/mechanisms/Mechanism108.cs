using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//巨型爆弹监听机制
public class Mechanism108 : BeMechanism
{
    int[] hpArray;
    int[] effectArray1;
    int[] timeArray;
    int[] effectArray2;
    readonly int[] changePhaseArray = new int[3] { 521390, 521391, 521392 };//各阶段变身机制
    int index;
    int timer;

    public Mechanism108(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        effectArray1 = new int[data.ValueB.Length];
        for (int i = 0; i < data.ValueB.Length; i++)
        {
            effectArray1[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        }

        timeArray = new int[data.ValueC.Length];
        for (int i = 0; i < data.ValueC.Length; i++)
        {
            timeArray[i] = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
        }

        effectArray2 = new int[data.ValueD.Length];
        for (int i = 0; i < data.ValueD.Length; i++)
        {
            effectArray2[i] = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
        }
    }

    public override void OnStart()
    {
        hpArray = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
        {
            var f = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueA[i], level), GlobalLogic.VALUE_1000);
            var hp = owner.GetEntityData().GetMaxHP() * f;
            hpArray[i] = hp;
        }

        index = 0;
        timer = timeArray[index];
        
        handleA = owner.RegisterEventNew(BeEventType.onHPChange, args =>
        {
            if (index >= hpArray.Length)
                return;

            if (owner.GetEntityData().GetHP() <= hpArray[index])
            {
                owner.DealEffectFrame(owner, effectArray1[index]);

                //变身
                if (index < changePhaseArray.Length)
                    owner.buffController.TryAddBuff(changePhaseArray[index], GlobalLogic.VALUE_1000);

                index++;
                if (index < timeArray.Length)
                    timer = timeArray[index];
            }
        });
    }

    public override void OnUpdate(int deltaTime)
    {
        if (index >= timeArray.Length)
            return;

        timer -= deltaTime;
        if (timer <= 0)
        {
            owner.DealEffectFrame(owner, effectArray2[index]);

            var damage = owner.GetEntityData().GetHP() - hpArray[index];
            owner.m_pkGeActor.SetHPDamage(damage);
            if (hpArray[index] == 0)
            {
                owner.GetEntityData().SetHP(-1);
                owner.DoDead();
            }
            else
            {
                owner.GetEntityData().SetHP(hpArray[index]);
            }

            //变身
            if (index < changePhaseArray.Length)
                owner.buffController.TryAddBuff(changePhaseArray[index], GlobalLogic.VALUE_1000);

            index++;
            if (index < timeArray.Length)
                timer = timeArray[index];
        }
    }
}
