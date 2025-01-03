using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill5288 : Skill5077
{
	public Skill5288(int sid, int skillLevel) : base(sid, skillLevel)
	{
		
	}
}

public class Skill5077 : BeSkill
{
    int conductMonsterID;
    int conductSkillID;
    int delayTime;

    int delayTimeAcc = 0;
    bool startCheck = false;

    public Skill5077(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        if (skillData != null)
        {
            conductMonsterID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
            conductSkillID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
            delayTime = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        }
    }

    public override void OnStart()
    {
        delayTimeAcc = 0;
        startCheck = true;
    }

    public override void OnUpdate(int iDeltime)
    {
        if (startCheck)
        {
            delayTimeAcc += iDeltime;
            if (delayTimeAcc > delayTime)
            {
                startCheck = false;
                DoWork();
            }
        }
    }

    void DoWork()
    {
		List<BeActor> monsters = GamePool.ListPool<BeActor>.Get();
		owner.CurrentBeScene.FindMonsterByID(monsters, conductMonsterID);
        for (int i = 0; i < monsters.Count; ++i)
        {
            var sidetick = monsters[i];

            //if (sidetick.IsInPassiveState() && !sidetick.IsCastingSkill())
            //    Logger.LogErrorFormat("is in passive but no castin skill");

            if (monsters[i] != null && (!sidetick.IsInPassiveState() || sidetick.IsCastingSkill()))
            {
                var skill = monsters[i].GetSkill(conductSkillID);
                if (skill != null)
                {
                    skill.ResetCoolDown();
                    monsters[i].UseSkill(conductSkillID, true);
                }
            }
        }

		GamePool.ListPool<BeActor>.Release(monsters);
    }
}
