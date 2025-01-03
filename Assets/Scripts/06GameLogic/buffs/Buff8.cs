using UnityEngine;
using System.Collections.Generic;
using GameClient;

//冰冻
public class Buff8 : BeBuff
{


    BeEvent.BeEventHandleNew handler = null;

    public Buff8(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
    { }

    public override void OnStart()
    {
        RemoveHanlder();
        handler = owner.RegisterEventNew(BeEventType.onHit, args =>
        //handler = owner.RegisterEvent(BeEventType.onHit, (object[] args) =>
        {
            List<int> magicElementTypeList = args.m_Obj2 as List<int>;
            if (magicElementTypeList.Contains((int)MagicElementType.FIRE))          //如果有火属性攻击  则可以破冰
            {
                Finish();
                   
            }
        });
    }

    public override void OnFinish()
    {
        RemoveHanlder();
    }

    void RemoveHanlder()
    {
        if (handler != null)
        {
            handler.Remove();
            handler = null;
        }
    }
}
