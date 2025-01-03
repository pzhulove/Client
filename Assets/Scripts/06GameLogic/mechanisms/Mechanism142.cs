using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//改变落地弹跳时的Z轴速度
public class Mechanism142 : BeMechanism
{
    public Mechanism142(int mid, int lv) : base(mid, lv) { }

    protected VInt speed = 10;
    protected int removeBuffId = 0;

    public override void OnInit()
    {
        base.OnInit();
        speed = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        removeBuffId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onChangeClickForce, (GameClient.BeEvent.BeEventParam param) => 
        {
            //VInt[] clickForceArr = (VInt[])args[0];
            param.m_Vint = speed;
            owner.buffController.RemoveBuff(removeBuffId);
        });
    }
}
