using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill1700 : BeSkill
{
	protected IBeEventHandle handle = null;
	protected Mechanism22 runeManager = null;

	int runeAttackAddBuff = 0;
	int runeExplodeAttackAddBuff = 0;

	int runeLifeTimeAdd = 0;
	int runeAttackCountAdd = 0;
	VFactor runeSizeAdd = VFactor.zero;

	int guiyinzhuID = 60011;
	int guiyinzhuBaoZaID = 60012;

	int runeNum = -1;

    public Skill1700(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

	public override void OnInit ()
	{
		if (BattleMain.IsModePvP(battleType))
		{
			runeAttackAddBuff = TableManager.GetValueFromUnionCell(skillData.ValueE[0], level);
			runeExplodeAttackAddBuff = TableManager.GetValueFromUnionCell(skillData.ValueE[1], level);
		}
		else {
			runeAttackAddBuff = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
			runeExplodeAttackAddBuff = TableManager.GetValueFromUnionCell(skillData.ValueD[1], level);
		}

		runeLifeTimeAdd = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
		runeAttackCountAdd = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
		runeSizeAdd = new VFactor(TableManager.GetValueFromUnionCell(skillData.ValueC[0], level),GlobalLogic.VALUE_1000);
	}

	public override void OnStart ()
	{
		runeNum = -1;
		RemoveHandle();

		handle = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args)=>{
			BeProjectile projectile = args.m_Obj as BeProjectile;
			if(projectile != null && (projectile.m_iResID == guiyinzhuID || projectile.m_iResID == guiyinzhuBaoZaID) )
			{
				if (runeNum == -1)
				{
					if (runeManager == null)
						SetRuneManager();

					int runeCount = 0;
					if (runeManager != null)
						runeCount = runeManager.GetRuneCount();
					var param = owner.TriggerEventNew(BeEventType.onCalcRuneAddDamage, new GameClient.EventParam() { m_Int = skillID, m_Int2 = runeCount });
					runeCount = param.m_Int2;
					runeNum = runeCount;
				}

				AddRuneBuff(runeNum, projectile);

				if (projectile.m_iResID == guiyinzhuID)
				{
					//清除所有的波动刻印
					runeManager.RemoveRune(true);
					for(int i=0; i<runeNum; ++i)
						projectile.AddSkillBuff(runeAttackAddBuff);
				}
				else
				{
					for(int i=0; i<runeNum; ++i)
						projectile.AddSkillBuff(runeExplodeAttackAddBuff);
				}
					
				/*
				//处理爆炸伤害
				projectile.RegisterEvent(BeEventType.onAfterGenBullet, (object[] args3)=>{
					BeProjectile explode = args[0] as BeProjectile;
					if (explode != null && explode.m_iResID == guiyinzhuBaoZaID )
					{
						for(int j=0; j<runeCount; ++j)
							explode.AddSkillBuff(runeExplodeAttackAddBuff);
					}

				});*/
			}
		});
	}


	public override void OnCancel ()
	{
		//RemoveHandle();
	}

	public override void OnFinish ()
	{
		//RemoveHandle();
	}


	public override bool CanUseSkill ()
	{
		bool hasRune = false;

		if (runeManager == null)
			SetRuneManager();

		if (runeManager != null)
			hasRune = runeManager.GetRuneCount() > 0;

		return base.CanUseSkill () && hasRune;
	}
    public override BeSkillManager.SkillCannotUseType GetCannotUseType()
    {
        bool hasRune = false;
        if (runeManager != null)
            hasRune = runeManager.GetRuneCount() > 0;

        if (!hasRune)
            return BeSkillManager.SkillCannotUseType.NO_KEYING;

        return base.GetCannotUseType();
    }

    void SetRuneManager()
	{
		if (runeManager != null)
			return;
		
		if (owner != null)
		{
			var skill = owner.GetSkill(Global.BODONGKEYIN_SKILL_ID) as Skill1710;
			if (skill != null)
			{
				runeManager = skill.runeManager;
			}
		}
	}

	void RemoveHandle()
	{
		if (handle != null)
		{
			handle.Remove();
			handle = null;
		}
	}

	void AddRuneBuff(int count, BeProjectile projectile)
	{
		//持续时间
		projectile.m_fLifes += runeLifeTimeAdd * count;

		//攻击次数
		projectile.totoalHitCount += runeAttackCountAdd * count;

        //比例
        VInt originalScale = projectile.GetScale();
        projectile.SetScale(originalScale.i * (VFactor.one + runeSizeAdd*5));
	}
}
