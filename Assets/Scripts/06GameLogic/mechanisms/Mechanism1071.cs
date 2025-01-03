using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 修改攻击字体类型机制，显示方式为主玩家
/// </summary>
public class Mechanism1071 : BeMechanism
{
    private IBeEventHandle onChangeHitTextType; //改变攻击字体handle
    public Mechanism1071(int id, int level) : base(id, level) { }

    public override void OnReset()
    {
        ClearEventHandle();
    }

    public override void OnStart()
    {
        ClearEventHandle();
        BeEntity topOwner = null;
        BeActor player = null;
        if(owner != null)
        {
            topOwner = owner.GetTopOwner(owner.GetOwner());
            if (topOwner != null)
            {
                player = topOwner as BeActor;
            }
            else
            {
                player = owner.GetOwner() as BeActor;
            }
        }
        
        if(owner != null && player != null && player.isLocalActor)
        {
            onChangeHitTextType = owner.RegisterEventNew(BeEventType.onChangeHitNumberType, args =>
            {
                /*var tempBoolArray = (bool[])args[0];
                if (owner.GetOwner() != null && tempBoolArray != null && tempBoolArray.Length > 0) 
                {
                    tempBoolArray[0] = true;
                }*/

                if (owner.GetOwner() != null)
                {
                    args.m_Bool = true;
                }
            });
        }
    }

    public override void OnFinish()
    {
        ClearEventHandle();
    }

    private void ClearEventHandle()
    {
        if(onChangeHitTextType!= null)
        {
            onChangeHitTextType.Remove();
            onChangeHitTextType = null;
        }
    }
}
