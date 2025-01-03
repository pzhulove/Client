using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism1061 : BeMechanism
{

    public Mechanism1061(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnStart()
    {
        base.OnStart();
#if !SERVER_LOGIC
        if (owner.m_pkGeActor != null)
        {
            owner.m_pkGeActor.SetHeadInfoVisible(false);
            if (owner.attachModel != null)
                owner.attachModel.CustomActive(false);
        }
#endif
    }

    public override void OnFinish()
    {
        base.OnFinish();
#if !SERVER_LOGIC
        if (owner.m_pkGeActor != null)
        {
            owner.m_pkGeActor.SetHeadInfoVisible(true);
            if (owner.attachModel != null)
                owner.attachModel.CustomActive(true);
        }
#endif
    }
}
