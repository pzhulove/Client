using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 移动角色模型位置
/// </summary>
public class Mechanism1056 : BeMechanism {

    private float pos;
    GameObject actorNode = null;
    public Mechanism1056(int mid, int lv):base(mid, lv)
	{
    }

    public override void OnInit()
    {
        base.OnInit();
        pos =TableManager.GetValueFromUnionCell(data.ValueA[0], level)/1000.0f; 
    }

    public override void OnReset()
    {
        actorNode = null;
    }

    public override void OnStart()
    {
        base.OnStart();
#if !SERVER_LOGIC
        if (owner.m_pkGeActor != null)
        {
            owner.m_pkGeActor.SetHeadInfoVisible(false);
            actorNode = owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);
            actorNode.transform.localPosition = (owner.m_pkGeActor.GetPosition()+new Vector3(0,pos,0));
        }
#endif
    }

    public override void OnFinish()
    {
        base.OnFinish();
#if !SERVER_LOGIC
        if (owner.m_pkGeActor != null && actorNode!=null)
        {
            owner.m_pkGeActor.SetHeadInfoVisible(true);
            actorNode.transform.localPosition = Vector3.zero;
        }
#endif
    }
}
