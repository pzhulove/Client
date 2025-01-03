using UnityEngine;
using System.Collections.Generic;
using ProtoTable;

/*
 * 冰属性怪物被含有火属性的角色攻击会增加伤害
*/
public class Mechanism56 : BeMechanism
{
    protected VFactor mDamageAddRate = VFactor.zero;

    public Mechanism56(int mid, int lv) : base(mid, lv)
    {
        
    }
    public override void OnInit()
    {
        mDamageAddRate = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
    }
    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onBeHitAfterFinalDamage, RegisterBeHit);
        //handleA = owner.RegisterEvent(BeEventType.onBeHitAfterFinalDamage, RegisterBeHit);
    }

    protected void RegisterBeHit(GameClient.BeEvent.BeEventParam param)
    {
        var magicElementTypeList = param.m_Obj2 as List<int>;
        if (magicElementTypeList.Contains((int)MagicElementType.FIRE))
        {
            //int[] damage = args[0] as int[];
            int damageValue = param.m_Int;
            var attachValues = param.m_Obj3 as List<int>;
            if (attachValues != null)
            {
                for (int i = 0; i < attachValues.Count; i++)
                {
                    damageValue -= attachValues[i];
                }
            }
            int addValue = damageValue * mDamageAddRate;
            param.m_Int += addValue;
        }
    }
}
