using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill7386 : BeSkill
{
    private GameClient.BeEvent.BeEventHandleNew handle;
    //private BeMechanism mechanism;
    private int mechanismPID;

    private int monsterID = 4730011;
    public Skill7386(int sid, int skillLevel) : base(sid, skillLevel)
    { }

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnStart()
    {
        base.OnStart();
        handle = owner.RegisterEventNew(BeEventType.onHitOther, args =>
        //handle = owner.RegisterEvent(BeEventType.onHitOther, (object[] args) => 
        {
           var mechanism = owner.AddMechanism(1178);
           if (mechanism != null)
                mechanismPID = mechanism.PID;
        });

    }

    public override void OnEnterPhase(int phase)
    {
        base.OnEnterPhase(phase);
        if (phase == 2)
        {
            List<BeActor> list = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindMonsterByID(list, monsterID);
            if (list.Count > 0)
            {
                BeActor actor = list[0];
                actor.DoDead();
                bool faceLeft = owner.GetFace();
                float offset = faceLeft ? 1.0f : -1.0f;
                owner.SetPosition(actor.GetPosition()+new VInt3(offset, 0,0), true);
            }
            GamePool.ListPool<BeActor>.Release(list);
        }
       
    }

    public override void OnCancel()
    {
        RemoveHandle();
        base.OnCancel();
    }

    public override void OnFinish()
    {
        RemoveHandle();
        base.OnFinish();
    }

    private void RemoveHandle()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }

        if (owner.GetMechanismByPID(mechanismPID) != null)
        {
            owner.RemoveMechanism(1178);
        }
    }
}
