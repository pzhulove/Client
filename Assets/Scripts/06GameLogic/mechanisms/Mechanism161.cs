using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻击1000-2000范围内的敌人，伤害将会增加5%
/// </summary>
public class Mechanism161 : BeMechanism
{
    List<VInt> innerRanges = new List<VInt>();
    List<VInt> outerRanges = new List<VInt>();
    List<VFactor> damageFactors = new List<VFactor>();
    public Mechanism161(int mid, int lv) : base(mid, lv) { }
   
    public override void OnReset()
    {
        innerRanges.Clear();
        outerRanges.Clear();
        damageFactors.Clear();
    }
    public override void OnInit()
    {
        for (int i = 0; i < data.ValueA.Length; i++)
        {
            innerRanges.Add(VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[i], level), GlobalLogic.VALUE_1000));
        }
        for (int i = 0; i < data.ValueB.Length; i++)
        {
            outerRanges.Add(VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[i], level), GlobalLogic.VALUE_1000));
        }
        for (int i = 0; i < data.ValueC.Length; i++)
        {
            int factor = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
            damageFactors.Add(new VFactor(factor, GlobalLogic.VALUE_1000));
        }

    }
    public override void OnStart()
    {
        if (owner == null)
            return;

        handleA = owner.RegisterEventNew(BeEventType.onAfterFinalDamageNew, args =>
        //handleA = owner.RegisterEvent(BeEventType.onAfterFinalDamageNew, (object[] args) =>
        {
            //int[] vals = args[0] as int[];
            BeActor target = args.m_Obj as BeActor;
            if (target == null) return;
            var distDir = target.GetPosition2() - owner.GetPosition2();
            var dist = distDir.magnitude;
            for (int i = 0; i < innerRanges.Count; i++)
            {
                var innerRange = innerRanges[i];
                if (i >= outerRanges.Count)
                {
                    Logger.LogErrorFormat("mechanismID {0} out range is not right number {1}", this.mechianismID, outerRanges.Count);
                    break;
                }
                var outerRange = outerRanges[i];
                if (i >= damageFactors.Count)
                {
                    Logger.LogErrorFormat("mechanismID {0} vFactor is not  right number {1}", this.mechianismID, damageFactors.Count);
                    break;
                }
                if (dist >= innerRange && dist <= outerRange)
                {
                    args.m_Int = args.m_Int * (VFactor.one + damageFactors[i]);
                    return;
                }
            }
        });
    }
}
