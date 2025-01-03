using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism2041 : Mechanism2018
{
    public Mechanism2041(int mid, int lv) : base(mid, lv) { }

    private VInt xOffset = 0;       //特效相对于怪物位置偏移
    private int hurtId = 0;         //出发致死效果后 触发效果表id
    private VInt damageX = 0;       //伤害区域偏移
    private int deadDelay;          //死亡延迟时间

    private bool hurtFlag = false;

    public override void OnInit()
    {
        base.OnInit();
        xOffset = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[2], level), GlobalLogic.VALUE_1000);
        damageX = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[3], level), GlobalLogic.VALUE_1000);
        hurtId = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        deadDelay = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        if (deadDelay < 0)
            deadDelay = 0;
    }

    public override void OnReset()
    {
        hurtFlag = false;
    }

    public override void OnStart()
    {
        hurtFlag = false;
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (!hurtFlag)
        {
            VInt3 pos = owner.GetPosition();
            if (!owner.GetFace())
            {
                pos.x += xOffset.i;
            }
            else
            {
                pos.x -= xOffset.i;
            }
            if (owner.CurrentBeScene.IsInBlockPlayer(pos))
            {
                hurtFlag = true;
                DoDamage();
                owner.delayCaller.DelayCall(deadDelay, DoOwnerDead);
            }
        }
    }

    void DoOwnerDead()
    {
        if (!owner.IsDead())
        {
            owner.DoDead();
        }
    }

    private void DoDamage()
    {
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindFaceTargetsX(list, owner, boxXY[0] + damageX);
        for (int i = 0; i < list.Count; ++i)
        {
            owner._onHurtEntity(list[i], list[i].GetPosition(), hurtId);
        }
        GamePool.ListPool<BeActor>.Release(list);
    }
}
