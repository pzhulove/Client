using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

public class Skill7382 : BeSkill {

    BeEvent.BeEventHandleNew handle = null;
    private int distance = 1000;
    private int skillId = 1500;
    private int delayTime = 1500;
    private int interTime = 1000;
    private bool hitOther = false;
    private List<BeActor> actorList = new List<BeActor>();
    public Skill7382(int sid, int skillLevel) : base(sid, skillLevel)
    { }

    public override void OnInit()
    {
        base.OnInit();
        skillId = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        distance = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        delayTime = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        interTime = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        handle = owner.RegisterEventNew(BeEventType.onHitOther, args =>
        //handle = owner.RegisterEvent(BeEventType.onHitOther, (object[] args) => 
        {
            if (hitOther) return;
            hitOther = true;
            BeActor actor = args.m_Obj as BeActor;
            if (actor != null)
            {               
                owner.CurrentBeScene.FindMainActor(actorList);
                actorList.Remove(actor);

                owner.delayCaller.DelayCall(delayTime, () => 
                {
                    AttackOther(actorList); 
                });
            }
        });
    }

    private void AttackOther(List<BeActor> list)
    {
        if (list.Count <= 0) return;
        BeActor actor = list[0];
        list.RemoveAt(0);
        if (actor.IsDead())
            AttackOther(list);
        else
        {
            owner.SetFace(actor.GetFace(), true);

            var newPos = new VInt3(actor.GetFace() ? 1 : -1 * IntMath.Float2IntWithFixed(distance / 1000.0f), 0, 0) + actor.GetPosition();
            VInt3 pos = BeAIManager.FindStandPosition(newPos, owner.CurrentBeScene, owner.GetFace(), false, 30);
            owner.SetPosition(pos);
            if (owner.CanUseSkill(skillId))
            {
                owner.UseSkill(skillId);
                owner.delayCaller.DelayCall(interTime, () =>
                {
                    if (!owner.IsInPassiveState())
                    {
                        AttackOther(list);
                    }
                });
            }
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
        hitOther = false;
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
    }
}
