using UnityEngine;
using System.Collections;
using GameClient;

//瞬间隐身
public class Buff66 : BeBuff
{
    public Buff66(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
    { }

    public override void OnStart()
    {
#if !LOGIC_SERVER
        if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.HideObjUseSetlayer))
        {
            owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Transform).SetLayer(19);
            if (owner.m_pkGeActor.goInfoBar != null)
            {
                owner.m_pkGeActor.goInfoBar.SetLayer(19);
            }
        }
        else
        {
            owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Transform).SetActive(false);
            owner.m_pkGeActor.SetHeadInfoVisible(false);
        }
        
#endif
    }

    public override void OnFinish()
    {
#if !LOGIC_SERVER
        if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.HideObjUseSetlayer))
        {
            owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Transform).SetLayer(0);
            if (owner.m_pkGeActor.goInfoBar != null)
            {
                owner.m_pkGeActor.goInfoBar.SetLayer(13);
            }
        }
        else
        {
            owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Transform).SetActive(true);
            owner.m_pkGeActor.SetHeadInfoVisible(true);
        }
#endif
    }

}
