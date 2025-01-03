using UnityEngine;
using System.Collections.Generic;
using ProtoTable;

public class Mechanism57 : BeMechanism
{
    protected IBeEventHandle handle = null;

    public Mechanism57(int mid, int lv) : base(mid, lv){}

    public override void OnStart()
    {
        RemoveHandle();
        handle = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args) =>
        {
            var project = args.m_Obj as BeProjectile;
            if (project != null && project.IsGenRune())
            {
                owner.TriggerEventNew(BeEventType.onAddRune);
            }
        });
    }

    public override void OnFinish()
    {
        RemoveHandle();
    }

    protected void RemoveHandle()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
    }
}
