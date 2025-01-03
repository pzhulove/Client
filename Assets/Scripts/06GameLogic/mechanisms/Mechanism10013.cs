using System;
using System.Collections.Generic;

//几率附加伤害
public class Mechanism10013 : BeMechanism
{
    private int _attachChance;
    private int _attachDamageFix;
    private VFactor _attachDamageRate;
    private int _attachDamageLimit;

    public Mechanism10013(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        _attachChance = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        _attachDamageFix = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        _attachDamageRate = new VFactor(TableManager.GetValueFromUnionCell(data.ValueC[0], level), GlobalLogic.VALUE_1000);
        _attachDamageLimit = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onAfterFinalDamageNew, param =>
        {
            if (FrameRandom.Range1000() < _attachChance)
                return;
            int attachDamage = Math.Min(_attachDamageFix + param.m_Int * _attachDamageRate, _attachDamageLimit);
            param.m_Int += attachDamage;
            List<int> attachValues = param.m_Obj2 as List<int>;
            attachValues.Add(attachDamage);
        });
    }
}
