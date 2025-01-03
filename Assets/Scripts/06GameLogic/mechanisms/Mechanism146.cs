using System;
using System.Collections.Generic;
using GameClient;

public class Mechanism146 : BeMechanism
{
    //List<int> mAttackActorIDs = new List<int>();
    public Mechanism146(int mid, int lv) : base(mid, lv) { }

   public override void OnStart()
    {
        if (owner != null)
        {
            handleA = owner.RegisterEventNew(BeEventType.onDead, eventParam =>
            {
                if(owner != null && owner.CurrentBeBattle != null)
                {
                    owner.CurrentBeBattle.OnCriticalElementDisappear();
                }
            });
            if(owner.CurrentBeScene != null)
            {
                handleB = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onBossDead, onBossDead);
            }
            
        }
    }
    private void onBossDead(BeEvent.BeEventParam args)
    {
        owner.buffController.TryAddBuff(29,-1);
    }
}

