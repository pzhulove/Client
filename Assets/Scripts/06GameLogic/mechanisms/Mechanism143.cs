using System;
using System.Collections.Generic;

public class Mechanism143 : BeMechanism
{
    public int damageValue = 0;
    public Mechanism143(int mid, int lv) : base(mid, lv) { }
    public override void OnInit()
    {
        damageValue = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }
    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onChangeHurtValue, args =>
        //handleA = owner.RegisterEvent(BeEventType.onChangeHurtValue, args =>
        {
            //int[] hurtValueArr = (int[])args[0];
            args.m_Int = damageValue;
        });
    }
}
