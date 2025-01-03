using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill5679 : BeSkill {

    private int monsterID = 30080011;//怪物ID
    private int distance = 4;//两个组队的怪物距离
    private int protectID = 30230011;//保护罩ID
    List<BeActor> monsterList = new List<BeActor>();
    BeActor partnerMonster = null;
    BeActor protectMonster = null;
    Skill5680 skill = null;

    public Skill5679(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnPostInit()
    {
        base.OnPostInit();
    }

    public override void OnStart()
    {
        base.OnStart();

        owner.CurrentBeScene.FindMonsterByID(monsterList, monsterID);     
        for (int i = 0; i < monsterList.Count; i++)
        {
            if (monsterList[i].GetPID() == owner.GetPID()) continue;
            partnerMonster = monsterList[i];
        }

        VInt3 scenePos = owner.CurrentBeScene.GetSceneCenterPosition();
        owner.SetFace(owner.GetPosition().x>scenePos.x,true);

        if (partnerMonster != null)
        {
            skill =   partnerMonster.GetSkill(5680) as Skill5680;
            if (skill != null)
            {
                skill.partnerMonster = owner;
            }
        }
    }

    public override bool CanUseSkill()
    {
        owner.CurrentBeScene.FindMonsterByID(monsterList, monsterID);
        return base.CanUseSkill() && monsterList.Count>1;
    }



    public override void OnEnterPhase(int phase)
    {
        base.OnEnterPhase(phase);

        if (phase == 1)
        {
            partnerMonster.UseSkill(5680, true);
        }
        if (phase == 2)
        {
            if (partnerMonster != null)
            {
                partnerMonster.SetFace(!owner.GetFace(), true);
                VInt xpos = IntMath.Float2IntWithFixed(distance);
                VInt3 newPos = owner.GetPosition() + new VInt3(owner.GetFace() ? -xpos.i : xpos.i, 0, 0);
                if (owner.CurrentBeScene.IsInBlockPlayer(newPos))
                {
                    newPos = BeAIManager.FindStandPositionNew(newPos,owner.CurrentBeScene);
                }
                partnerMonster.SetPosition(newPos, true);
                
                owner.RegisterEventNew(BeEventType.onSummon, (args) =>
                {
                    protectMonster = args.m_Obj as BeActor;
                    if (protectMonster != null)
                    {
                        if (skill != null)
                        {
                            skill.protectMonster = protectMonster;
                        }
                        protectMonster.SetPosition((owner.GetPosition() + partnerMonster.GetPosition()) * 0.5f);
                        protectMonster.RegisterEventNew(BeEventType.onDead, eventParam =>
                        {
                            owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
                            if (partnerMonster != null)
                                partnerMonster.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);

                        });
                    }
                });
                owner.DoSummon(protectID);

            }
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();
        StopSkill();
    }

    public override void OnFinish()
    {
        StopSkill();
        base.OnFinish();
    }
    private void StopSkill()
    {
        if (partnerMonster != null)
        {
            partnerMonster.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
        }
        if (protectMonster != null)
        {
            protectMonster.DoDead();
        }
        protectMonster = null;
        partnerMonster = null;
    }
}
