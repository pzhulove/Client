using UnityEngine;
using System.Collections.Generic;
using GameClient;

public class Mechanism73 : BeMechanism
{
    IBeEventHandle beforeBeHit;
    int possibility = 0;
    int skillID = 0;   
    int castSkillCD = 0;
    bool canCastBlockSkill = true;

    private int dis;

    public int Distance
    {
        get { return dis; }
        set { dis = value; }
    }

    private int duration = 800;

    public int Duration
    {
        get { return duration;}
        set { duration = value; }
    }

    public Mechanism73(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        beforeBeHit = null;
        possibility = 0;
        skillID = 0; 
        castSkillCD = 0;
        canCastBlockSkill = true;
        dis = 0;
        duration = 800;
    }

    public override void OnInit()
    {
        if(data.ValueA.Count>0)
           skillID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);

        if (data.ValueB.Count > 0)
            possibility = TableManager.GetValueFromUnionCell(data.ValueB[0], level);

        if (data.ValueC.Count > 0)
        {
            duration = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
           
        }

        if (data.ValueD.Count > 0)
        {
            dis = TableManager.GetValueFromUnionCell(data.ValueD[0], level);

        }

        if (data.ValueE.Count > 0)
        {
            castSkillCD = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        }

    }

    public override void OnStart()
    {
        RemoveHandle();
        beforeBeHit = owner.RegisterEventNew(BeEventType.onBeforeOtherHit, args =>
        //beforeBeHit = owner.RegisterEvent(BeEventType.onBeforeOtherHit, (object[] args) =>
        {
            if (!canCastBlockSkill) return;

            ProtoTable.EffectTable hurtData = args.m_Obj2 as ProtoTable.EffectTable;
            if (hurtData != null && hurtData.IsFriendDamage == 1) return;
            if (owner.FrameRandom.Range1000() < possibility && owner.CanUseSkill(skillID))
            {
                BeActor actor = args.m_Obj as BeActor;
                if (actor != null)
                {
                    owner.SetFace(!actor.GetFace(), true, true);
                }
                owner.UseSkill(skillID);

                Skill1515 skill = owner.GetSkill(skillID) as Skill1515;
                if (skill != null)
                {
                    skill.isAutoBlock = true;
                }
                canCastBlockSkill = false;
            }
        });
    }

    int timer = 0;
    public override void OnUpdate(int deltaTime)
    {
        if (!canCastBlockSkill)
        {
            timer += deltaTime;
            if (timer >= castSkillCD)
            {
                canCastBlockSkill = true;
                timer = 0;
            }
        }
    }

    public override void OnFinish()
    {
        RemoveHandle();
    }

    protected void RemoveHandle()
    {
        if (beforeBeHit != null)
        {
            beforeBeHit.Remove();
            beforeBeHit = null;
        }
    }
}
