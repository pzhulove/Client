using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill20210 : BeSkill
{
    private string effect = "Effects/Hero_Zhaohuanshi/Luyisi/Prefab/Eff_Zhaohuanluyisi_jiaodi";
    private List<BeActor> list = new List<BeActor>();
    private int time = 600;
    public Skill20210(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        base.OnInit();
        
    }

    public override void OnStart()
    {
        base.OnStart();
        owner.CurrentBeScene.FindMainActor(list);
    }

    public override void OnUpdate(int iDeltime)
    {
        base.OnUpdate(iDeltime);
    }

    public override void OnEnterPhase(int phase)
    {
        base.OnEnterPhase(phase);
        if (phase == 1) return;
        BeActor target = SelectTarget();
        if (target == null) return;
        owner.delayCaller.DelayCall(500, () => 
        {
            if (owner == null || owner.IsDead()) return;
            if (target != null)
            {
                owner.SetFace(target.GetPosition().x < owner.GetPosition().x, true);
#if !SERVER_LOGIC
                GeEffectEx geEffect = owner.CurrentBeScene.currentGeScene.CreateEffect(8, target.GetPosition().vec3, false, time / 1000.0f);
                geEffect.SetScale(2.4f, 0, 1.7f);
#endif
                BeMoveTo moveTo = BeMoveTo.Create(owner, time, owner.GetPosition(), target.GetPosition(),false);
                owner.actionManager.RunAction(moveTo);
            }
        });
    }

    private BeActor SelectTarget()
    {
        if (list.Count == 0)
        {
            owner.CurrentBeScene.FindMainActor(list);
            if (list.Count <= 0) return null;
        }
        int index = owner.FrameRandom.InRange(0,list.Count);
        BeActor tartget = list[index];   
        list.Remove(list[index]);
        return tartget;
    }
}
