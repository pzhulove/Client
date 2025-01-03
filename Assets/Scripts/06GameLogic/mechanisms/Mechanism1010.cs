using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//将攻击到的对象移动到和攻击者同一水平线上
public class Mechanism1010 : BeMechanism
{
    public Mechanism1010(int mid, int lv) : base(mid, lv) { }
    
    private int hurtId = 0;         //攻击到人的触发效果ID
    private int yPos = 0;        //释放技能时玩家的Y轴坐标偏移

    public override void OnInit()
    {
        yPos = 0;
        hurtId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnStart()
    {
        yPos = owner.GetPosition().y;
        handleB = OwnerRegisterEventNew(BeEventType.onHitOther, args =>
        //handleB = owner.RegisterEvent(BeEventType.onHitOther, (object[] args) =>
        {
            BeActor target = args.m_Obj as BeActor;
            int id = args.m_Int;
            if (target == null || id != hurtId)
                return;
            if (target.stateController == null)
                return;
            if (!target.stateController.CanMove())
                return;
            if (target.stateController.CanNotAbsorbByBlockHole())
                return;
            VInt3 originPos = target.GetPosition();
            target.SetPosition(new VInt3(originPos.x, yPos,originPos.z));
        });
    }
}
