using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//遥感控制召唤的怪物，如圣徒的棒子
public class Mechanism10023 : BeMechanism
{
    private int mSkillId = 0;
    private int monsterID = 0;
    private BeActor currMonster = null;

    public Mechanism10023(int mid, int lv) : base(mid, lv) { }
    
    
    public override void OnInit()
    {
        mSkillId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        monsterID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onCastSkill, (args) =>
        {
            int skillID = args.m_Int;

            if (mSkillId == skillID)
            {
                var skill = owner.GetSkill(mSkillId);
                if (skill != null)
                {
                    skill.pressMode = SkillPressMode.PRESS_JOYSTICK;
                    skill.isCooldown = false;
                    if (currMonster != null && !currMonster.IsDead())
                    {
                        currMonster.m_cpkEntityInfo.Reset();
                        currMonster.DoDead();
                    }
                }
            }
            
        });
        handleB = owner.RegisterEventNew(BeEventType.onSummon, (args) =>
        {
            BeActor monster = args.m_Obj as BeActor;
            AddMonster(monster);
        });
    }
     
    void AddMonster(BeActor actor)
    {
        if (owner != null && actor != null && actor.GetEntityData().MonsterIDEqual(monsterID))
        {
            currMonster = actor;
            var handle1 = actor.RegisterEventNew(BeEventType.onDead, (args2) =>
            {
                currMonster = null;
            });
            
            var currSkill = owner.GetSkill(mSkillId);
            if (currSkill != null)
            {
                currSkill.SetInnerState(BeSkill.InnerState.LAUNCH);
                currSkill.StartCoolDown();

                VInt3 projectPos = actor.GetPosition();
                VInt3 effectPos = currSkill.GetEffectPos();

                if (owner.GetFace())
                    projectPos.x = effectPos.x;
                else
                    projectPos.x = effectPos.x;
                projectPos.y = effectPos.y;
                        
                if (owner.CurrentBeScene.IsInBlockPlayer(projectPos))
                {
                    var pos  = BeAIManager.FindStandPositionNew(projectPos, owner.CurrentBeScene, false, false, 50);
                    projectPos.x = pos.x;
                    projectPos.y = pos.y;
                }
                actor.SetPosition(projectPos);
                actor.SetFace(owner.GetFace(), true);
            }
        }
    }
    
    public override void OnReset()
    {
        currMonster = null;
    }
    public override void OnFinish()
    {
        currMonster = null;
    }
}
