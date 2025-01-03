using UnityEngine;
using System.Collections.Generic;

/*
 * 指定怪物ID{ValueA}不在技能ID{ValueB}持续中，移除某些BUFF{ValueC}
*/
public class Mechanism49 : BeMechanism
{
    int monsterID;
    int skillID;
    BeActor monster;
    public Mechanism49(int mid, int lv) : base(mid, lv){}

    public override void OnInit()
    {
        monsterID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        skillID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        monster = null;
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

                    if (monster != null)
                    {
                        monster.RegisterEventNew(BeEventType.onStateChange, (param) =>
                        {
                            ActionState state = (ActionState)param.m_Int;
                            if (state != ActionState.AS_CASTSKILL ||
                                state == ActionState.AS_CASTSKILL && monster.GetCurSkillID() != skillID)
                            {
                                for (int i = 0; i < data.ValueC.Count; ++i)
                                {
                                    int buffID = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
                                    owner.buffController.RemoveBuff(buffID);
                                }
                            }
                        });
                    }
                }

                GamePool.ListPool<BeActor>.Release(monsters);
            }
        }
    }
}
