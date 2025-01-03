using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

/*
 攻击碰撞检测机制
 */
public class Mechanism35 : BeMechanism
{
    protected int m_BombEntityId = 0;                               //Buff信息表Id

    //protected BeEventHandle m_HitOtherHandle = null;                //坚定到自己攻击到别人

    public Mechanism35(int mid, int lv) : base(mid, lv){}
    public override void OnInit()
    {
        m_BombEntityId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnStart()
    {
        handleA = OwnerRegisterEventNew(BeEventType.onCollide, _OnCollide);
    }

    private void _OnCollide(BeEvent.BeEventParam param)
    {
        BeActor actor = owner.GetOwner() as BeActor;
        if (actor != null && !actor.IsDeadOrRemoved())
        {
            actor.AddEntity(m_BombEntityId, owner.GetPosition());
        }
        owner.DoDead();

#if !LOGIC_SERVER
        owner.m_pkGeActor.SetActorVisible(false);
#endif
    }

    //public override void OnFinish()
    //{
    //    if (m_HitOtherHandle != null)
    //    {
    //        m_HitOtherHandle.Remove();
    //    }
    //}
}