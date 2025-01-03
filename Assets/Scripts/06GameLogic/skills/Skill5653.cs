using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//魔剑插地技能保护
public class Skill5653 : BeSkill
{
    int monsterId = 5670021;
    int nextSkillId = 5654;
    List<BeActor> monsters = new List<BeActor>();
    bool isMonsterDead = false;
    int timer = 0;

    public Skill5653(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnStart()
    {
        isMonsterDead = false;
        timer = 0;
    }

    public override void OnUpdate(int iDeltime)
    {
        if (!isMonsterDead)
        {
            if (timer >= GlobalLogic.VALUE_1000)
            {
                if (monsters.Count <= 0)
                {
                    owner.CurrentBeScene.FindActorById(monsters, monsterId);
                    if (monsters.Count > 0)
                    {
                        monsters[0].RegisterEventNew(BeEventType.onDead, eventParam =>
                        {
                            isMonsterDead = true;
                        });
                    }
                }

                timer = 0;
            }
        }
        else
        {
            if (timer >= GlobalLogic.VALUE_3000)//怪物死亡3秒以后，强制取消技能并释放下一个技能
            {
                Cancel();
                owner.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
                owner.UseSkill(nextSkillId, true);
            }
        }
        timer += iDeltime;
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill()&&!owner.IsDead();
    }

    public override void OnFinish()
    {
        monsters.Clear();
    }
}
