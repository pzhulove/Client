using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//引导用的新技能
public class Skill2012 : Skill2103 {
	public Skill2012(int sid, int skillLevel):base(sid, skillLevel){}
}
public class Skill2014 : Skill2103 {
	public Skill2014(int sid, int skillLevel):base(sid, skillLevel){}
}
public class Skill2015 : Skill2103 {
	public Skill2015(int sid, int skillLevel):base(sid, skillLevel){}
}

public class Skill2107 : Skill2103 {
	public Skill2107(int sid, int skillLevel):base(sid, skillLevel){}
}

public class Skill2109 : Skill2103 {
	public Skill2109(int sid, int skillLevel):base(sid, skillLevel){}
}

public class Skill2111 : Skill2103 {
	public Skill2111(int sid, int skillLevel):base(sid, skillLevel){}
}

public class Skill2118 : Skill2103 {
	public Skill2118(int sid, int skillLevel):base(sid, skillLevel){}
}



public class Skill2103 : BeSkill {

	int monsterID = 9060031;
	int monsterSkillID = 5353;
	BeActor summonedMonster;

	public Skill2103(int sid, int skillLevel):base(sid, skillLevel){}

	public override void OnInit ()
	{
		monsterID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
		monsterSkillID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
	}
    private bool IsTargetActionValid(BeActor target)
    {

        if (target != null && (target.stateController != null &&
            !target.stateController.CanMove() ||
            !target.stateController.CanAttack() ||
            target.stateController.WillBeGrab() ||
            target.stateController.IsGrabbing() ||
            target.stateController.IsBeingGrab()) ||
            target.sgGetCurrentState() == (int)ActionState.AS_HURT)
        {
            return false;
        }
        var monsterSkill = summonedMonster.GetSkill(monsterSkillID);
        if (monsterSkill == null) return false;
        return monsterSkill.CanUseSkill();
    }
    public override void OnClickAgain()
    {
        if (this.skillID == 2111 ||
        this.skillID == 2118 ||
        this.skillID == 2103 ||
        this.skillID == 2109)
        {
            bool isValid = IsTargetActionValid(summonedMonster);
            if (summonedMonster != null && !summonedMonster.IsDead() && isValid)
            {
                ReleaseSummonSkill(summonedMonster);
                SetButtonEffect(false);
            }
        }
        else if (summonedMonster != null && !summonedMonster.IsDead() && summonedMonster.CanUseSkill(monsterSkillID))
        {
            ReleaseSummonSkill(summonedMonster);
            SetButtonEffect(false);
        }
    }

	public override void OnFinish ()
	{
		GetSummonedMonster();
	}

	public override void OnCancel ()
	{
		GetSummonedMonster();
	}

	void GetSummonedMonster()
	{
		summonedMonster = null;

		List<BeActor> summons = GamePool.ListPool<BeActor>.Get();
		owner.CurrentBeScene.GetSummonBySummoner(summons, owner);

		for(int i=0; i<summons.Count; ++i)
		{
			if (!summons[i].IsDead() && BeUtility.IsMonsterIDEqual(summons[i].GetEntityData().monsterID, monsterID))
			{

				summonedMonster = summons[i];
				break;
			}
		}

		GamePool.ListPool<BeActor>.Release(summons);

		if (summonedMonster == null)
		{
			skillButtonState = SkillState.NORMAL;
		}
		else {
			summonedMonster.RegisterEventNew(BeEventType.onDead, eventParam =>
			{
				SetButtonEffect(false);
				skillButtonState = SkillState.NORMAL;
				summonedMonster = null;
			});

			summonedMonster.RegisterEventNew(BeEventType.onExecuteAICmd, (args)=>{
				if (summonedMonster != null && summonedMonster.CanUseSkill(monsterSkillID))
				{
					SetButtonEffect(true);
				}	
			});
		}
	}

	void SetButtonEffect(bool open)
	{
#if !SERVER_LOGIC 

		if (button != null)
		{
			if (open)
				button.AddEffect(ETCButton.eEffectType.onContinue);
			else
				button.RemoveEffect(ETCButton.eEffectType.onContinue);
		}

 #endif


	}

	static readonly VInt value = new VInt(1.5f);
	public void ReleaseSummonSkill(BeActor monster)
	{
		if (monster == null)
			return;

		monster.UseSkill(monsterSkillID, true);	

		var curPos = owner.GetPosition();
		curPos.x += owner.GetFace() ? -value.i:value.i;

		if (monster.CurrentBeScene.IsInBlockPlayer(curPos))
			curPos = owner.GetPosition();

		monster.SetPosition(curPos);
		monster.SetFace(owner.GetFace());
	}
}
