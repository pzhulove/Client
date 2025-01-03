using UnityEngine;
using System.Collections;

public class Skill1614 : BeSkill {

	new int skillID = 1509;
	int buffID = 5;
	int buffProb;

	public Skill1614(int sid, int skillLevel):base(sid, skillLevel)
	{

	}

	public sealed override void OnPostInit()
	{
		buffProb = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);

		var skill = owner.GetSkill(skillID);
        if (skill != null)
        {
            BuffInfoData info = new BuffInfoData
            {
                buffID = buffID,
                prob = buffProb
            };
			//如果之前存在 则先移除然后再添加 避免因为换武器导致报错
            if (skill.buffEnhanceList.ContainsKey(buffID))
            {
                skill.buffEnhanceList.Remove(buffID);
            }
			skill.buffEnhanceList.Add(buffID, info);
		}	
	}
}
