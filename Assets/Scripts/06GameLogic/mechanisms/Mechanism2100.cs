using System;
using GameClient;
using System.Collections.Generic;

/// <summary>
/// 己方可见，对方不可见机制
/// </summary>
public class Mechanism2100 : BeMechanism
{
    public Mechanism2100(int mid, int lv) : base(mid, lv)    { }

    private bool m_IsHide = false;

    public override void OnReset()
    {
        m_IsHide = false;
    }

    public override void OnStart()
    {
#if !LOGIC_SERVER
        if (!IsVisible())
        {
            SetVisible(false);
            m_IsHide = true;
        }
#endif
    }

    public override void OnFinish()
    {
#if !LOGIC_SERVER
        if (m_IsHide)
        {
            m_IsHide = false;
            SetVisible(true);
        }
#endif
    }

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <returns></returns>
    public bool IsVisible()
    {
        var battle = owner.CurrentBeBattle;
        if (battle == null)
            return false;

        var mgr = battle.dungeonPlayerManager;
        if (mgr == null)
            return false;

        var player = mgr.GetMainPlayer();
        if (player == null)
            return false;

        var actor = player.playerActor;
        if (actor == null)
            return false;
        
        if (actor.m_iCamp != owner.m_iCamp)
            return true;

        return false;
    }

    public void SetVisible(bool isVisible)
    {
        if(owner.m_pkGeActor == null)
            return;

        owner.m_pkGeActor.SetActorVisible(isVisible);
        /*var node = owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Transform);
        if(node == null)
            return;
        owner.m_pkGeActor.SetFootIndicatorVisible(isVisible);
        owner.m_pkGeActor.SetHeadInfoVisible(isVisible);
        owner.m_pkGeActor.SetActorVisible(isVisible);
        if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.HideObjUseSetlayer))
        {
            node.SetLayer(isVisible ? 0 : 19);
        }
        else
        {
            node.SetActive(isVisible);
        }*/
    }
}