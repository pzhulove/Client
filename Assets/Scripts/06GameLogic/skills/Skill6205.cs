using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Skill6205 : BeSkill
{
    int[] entityID = new int[] {10442,10443,10444,10445,10446 };
    public Skill6205(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args) => 
        {
            BeProjectile actor = args.m_Obj as BeProjectile;
            if (actor != null&& owner.aiManager!=null && Array.IndexOf(entityID,actor.m_iResID)!=-1)
            {
                if (owner.aiManager.aiTarget != null)
                {
                    actor.SetPosition(owner.aiManager.aiTarget.GetPosition(), true);
                }
                else
                {
                    BeActor target = FindMaxResentment();
                    if (target != null)
                    {
                        actor.SetPosition(target.GetPosition(), true);
                    }
                }
            }
        });
    }

    private BeActor FindMaxResentment()
    {
        List<BeActor> targets = new List<BeActor>();
        owner.CurrentBeScene.FindActorInRange(targets, owner.GetPosition(), int.MaxValue);
        if (targets.Count == 0)
        {
            return null;
        }
        owner.CurrentBeScene.SortResentmentList(targets);
        return targets[0];
    }
}
