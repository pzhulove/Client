using System;
using System.Collections.Generic;
//宝藏关卡boss逃离机制
public class Mechanism2073 : BeMechanism
{
    public Mechanism2073(int mid, int lv) : base(mid, lv){ }
    public override void OnStart()
    {
        base.OnStart();
        if (owner != null)
        {
            owner.m_iEntityLifeState = (int)EntityLifeState.ELS_CANREMOVE;
            owner.m_iRemoveTime = 0;
            if(owner.CurrentBeBattle != null)
            {
                var curBattle = owner.CurrentBeBattle as GameClient.TreasureMapBattle;
                if(curBattle != null)
                {
                    curBattle.OnBossFleeAway(owner);
                }
            }
        }
    }

}

