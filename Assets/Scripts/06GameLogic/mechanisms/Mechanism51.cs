using UnityEngine;
using System.Collections.Generic;

/*
 * 指定怪物ID{ValueA}在技能ID{ValueB}持续中，则拥有者不释放技能ID{ValueC}
*/
public class Mechanism51 : BeMechanism
{
    int monsterID;
    int skill1;
    int skill2;
    List<int> skillList;
    BeActor monster;
    public Mechanism51(int mid, int lv) : base(mid, lv){}

    public override void OnReset()
    {
        if (skillList != null) skillList.Clear();
        monster = null;
        
    }
    public override void OnInit()
    {
        monsterID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        skill1 = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        skill2 = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        skillList = new List<int>();
        skillList.Add(skill2);
        owner.aiManager.SetSkillsEnable(skillList, false);
    }
    
    public override void OnUpdate(int deltaTime)
    {
        if (owner != null)
        {
            if (monster == null)
            {
                List<BeActor> monsters = GamePool.ListPool<BeActor>.Get();
                owner.CurrentBeScene.FindMonsterByID(monsters, monsterID);

                if (monsters.Count > 0)
                {
                    monster = monsters[0];

                    owner.aiManager.SetSkillsEnable(skillList, true);

                    monster.RegisterEventNew(BeEventType.onStateChange, (param) =>
                    {
                        if (owner.IsDead()) return;
                        ActionState state = (ActionState)param.m_Int;
                        if (state == ActionState.AS_CASTSKILL && monster.GetCurSkillID() == skill1)
                        {
                            owner.aiManager.SetSkillsEnable(skillList, false);
                        }
                        else if(state != ActionState.AS_DEAD)
                        {
                            owner.aiManager.SetSkillsEnable(skillList, true);
                        }
                    });
                }
                else
                {
                    owner.aiManager.SetSkillsEnable(skillList, false);
                }

                GamePool.ListPool<BeActor>.Release(monsters);
            }
        }
    }
}
