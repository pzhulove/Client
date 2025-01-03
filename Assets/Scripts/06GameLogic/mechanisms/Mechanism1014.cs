using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 暂停保护机制
/// </summary>
public class Mechanism1014 : BeMechanism
{
    List<BeActor> list = new List<BeActor>();
    public Mechanism1014(int mid, int lv):base(mid, lv){
    }

    public override void OnReset()
    {
        list.Clear();
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = OwnerRegisterEventNew(BeEventType.onHitOther, (args) =>
        //handleA = owner.RegisterEvent(BeEventType.onHitOther, (args) => 
        {
            BeActor actor = args.m_Obj as BeActor;
            if (actor != null && !list.Contains(actor) && actor.protectManager.IsEnable())
            {
                list.Add(actor);
                if (actor.protectManager != null)
                    actor.protectManager.SetEnable(false);
            }
        });
    }

    public override void OnDead()
    {
        base.OnDead();
        ContinueProtect();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        ContinueProtect();
    }

    private void ContinueProtect()
    {
        for (int i = 0; i < list.Count; i++)
        {
            if(list[i].protectManager!=null)
                list[i].protectManager.SetEnable(true);
        }
    }
}
