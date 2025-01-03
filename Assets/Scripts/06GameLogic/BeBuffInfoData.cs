using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using GameClient;

public class BeBuffInfoData {
}


public enum BuffTarget
{
	SELF = 0,
	ENEMY =1,
	SKILL =2,
	RANGE_ENEMY = 3,                //范围目标
	RANGE_FRIEND = 4,               //范围友军不包含自己
	FRIEND = 5,                     //范围友军包含自己(仅范围BuffCondition填了生效)
    FRIEND_NOTSELF = 6,             //范围友军不包含自己(仅范围BuffCondition填了生效)
    RANGE_FRIEND_ADNSELF = 7,       //范围友军包含自己
    RANGE_FRIENDHERO = 8,           //友军英雄
    RANGE_ENEMYHERO = 9,            //敌方英雄
    RANGE_FRIEND_NOTSUMMON = 10,    //范围友军包含自己不包含召唤兽
    OUT_OF_RANGE_ENEMY = 11,        //范围外的目标
    RANGE_OWNER = 12,        //范围内拥有者
    COUNT = 13,
}

public enum BuffCondition
{
	ENTERBATTLE = -1,
	NONE = 0,
	ATTACK,
	BEHIT,
	RELEASE_SKILL,
	RANGE,
	PHYSIC_ATTACK,
	MAGIC_ATTACK,
	RELEASE_SEPCIFY_SKILL,
	GETUP,//起身
	START_RUN,//开始跑步
	BREAK_ACTION,//破招
	CRITICAL_HIT,//暴击
	BE_CRITICAL_HIT,//被暴击
	BACK_HIT,//背击
	BE_BACK_HIT,//被背击
	RELEASE_SEPCIFY_SKILL_HIT,//指定技能命中

    NEARATTACK,             //近战
    FARATTACK,             //远程
    LIGHTATTACK,            //光属性攻击
    FIREATTACK,             //火属性攻击
    ICEATTACK,              //冰属性攻击
    DARKATTACK,             //暗属性攻击
    ATTACKMISS,             //攻击时Miss
    BEHITMISS,              //被击时Miss

    RELEASE_SEPCIFY_SKILL_COMPLETE,//指定技能释放完成

    PLAYER_DEAD,    //玩家死亡
    COUNT
}


public class BuffInfoData
{
	public CrypticInt32 buffInfoID;
	public CrypticInt32 buffID;
	public CrypticInt32 level;		//等级
	public CrypticInt32 abnormalLevel;	//用于判断是否能上buff
	public BuffTarget target;
	public CrypticInt32 buffTargetRangeRadius;//目标选择范围
	public CrypticInt32 prob;		//概率 1000
	public CrypticInt32 duration;	//buff时间 1000
	public CrypticInt32 attack;		//攻击力
	public CrypticInt32 attackPercent;//攻击百分比 1000
	public BuffCondition condition;
	public List<int> skillIDs;
	public int buffRangeRadius; //范围半径
	public List<int> conditionSkillIDs;//条件:指定技能ID
	public int checkInterval;//范围buff check间隔
	public Dictionary<int, int> mapMonsterTypeBuff = null;//new Dictionary<int, int>();
	public string effectName;
	public string effectLocatorName;
    public CrypticInt32 CD;
    public CrypticInt32 startCD;
    public int delay;
    public ProtoTable.BuffInfoTable data;
    int CHECK_INTERVAL = GlobalLogic.VALUE_1000;
	int timeAcc;
	bool isCooldown = false;
	int cdAcc;
    public int monsterMode = 0;  //怪物模式
    GeEffectEx effect = null;
	List<BeActor> inRangers = null;//new List<BeActor>();

	List<BeActor> targets = null;


	public BuffInfoData()
	{
		level = 1;
		abnormalLevel = level;
		target = BuffTarget.SELF;
		prob = GlobalLogic.VALUE_1000;
		duration = -1;//max
		condition = BuffCondition.NONE;
		delay = 0;
	}

	public BuffInfoData(int buffInfoID, int externLevel = 0, int addLevel = 0)
	{
		this.buffInfoID = buffInfoID;
		if (buffInfoID != 0)
		{
		    data = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(buffInfoID);
			if (data != null)
			{
				buffID = data.BuffID;
				level = TableManager.GetValueFromUnionCell(data.BuffLevel, externLevel);
				abnormalLevel = TableManager.GetValueFromUnionCell(data.BuffLevel, externLevel);

                if (data.BuffLevel.valueType != ProtoTable.UnionCellType.union_fix && externLevel > 0)
                    level = externLevel;

                level = Mathf.Max(1, externLevel);

				level += addLevel;
				target = (BuffTarget)data.BuffTarget;
				prob = TableManager.GetValueFromUnionCell(data.AttachBuffRate, level);
				duration = TableManager.GetValueFromUnionCell(data.AttachBuffTime, level);

				attack = TableManager.GetValueFromUnionCell(data.BuffAttack, level);

				//如果有externalLevel，这里的buff伤害取externLevel的等级
// 				if (externLevel > 0)
//                 {
//                     attack = TableManager.GetValueFromUnionCell(data.BuffAttack, externLevel);
//                     prob = TableManager.GetValueFromUnionCell(data.AttachBuffRate, externLevel);
//                     duration = TableManager.GetValueFromUnionCell(data.AttachBuffTime, externLevel);
//                					

				condition = (BuffCondition)data.BuffCondition;
				buffRangeRadius = TableManager.GetValueFromUnionCell(data.BuffRangeRadius, level);
				buffTargetRangeRadius = data.BuffTargetRadius;
				CD = TableManager.GetValueFromUnionCell(data.BuffInfoCD, level);//data.BuffInfoCD
                startCD = TableManager.GetValueFromUnionCell(data.BuffInfoStartCD, level);
                if (startCD > 0)
                    StartInitCD();
                delay = data.BuffDelay;

				effectName = data.EffectName;
				effectLocatorName = data.EffectLocateName;

				// if (data.SkillID.Count > 0 && data.SkillID[0] > 0)
				// {
					if (skillIDs == null)
						skillIDs = new List<int>();
					for(int i=0; i<data.SkillID.Count; ++i)
					{
						if (data.SkillID[i] > 0)
							skillIDs.Add(data.SkillID[i]);
					}
				// }

				if (condition == BuffCondition.RELEASE_SEPCIFY_SKILL || 
					condition == BuffCondition.RELEASE_SEPCIFY_SKILL_HIT
                    || condition == BuffCondition.RELEASE_SEPCIFY_SKILL_COMPLETE)
				{
					if (data.ConditionSkillID.Count > 0 && data.ConditionSkillID[0] > 0)
					{
						if (conditionSkillIDs == null)
							conditionSkillIDs = new List<int>();
						for(int i=0; i<data.ConditionSkillID.Count; ++i)
							conditionSkillIDs.Add(data.ConditionSkillID[i]);
					}	
				}

                monsterMode = data.monsterModeType;
                if (data.MonsterTypeMap.Count>=1 && data.MonsterTypeMap[0].valueType == ProtoTable.UnionCellType.union_fixGrow)
				{
					if (mapMonsterTypeBuff == null)
						mapMonsterTypeBuff = new Dictionary<int, int>();
					for(int i=0; i<data.MonsterTypeMap.Count; ++i)
					{
						int key = data.MonsterTypeMap[i].fixInitValue;
						int value = data.MonsterTypeMap[i].fixLevelGrow;
						if (!mapMonsterTypeBuff.ContainsKey(key))
							mapMonsterTypeBuff.Add(key, value);
					}
				}

				checkInterval = data.BuffRangeCheckInterval==0?CHECK_INTERVAL:data.BuffRangeCheckInterval;
			}
		}
	}


	public void OnAdd(BeActor owner)
	{
		if (condition == BuffCondition.RANGE)
		{
			if (Utility.IsStringValid(effectName))
			{
				effect = owner.m_pkGeActor.CreateEffect(effectName, effectLocatorName, 9999999, new Vec3(0, 0, 0));
			}
        }

        /*int[] radiusArray = new int[1];
        radiusArray[0] = GlobalLogic.VALUE_1000;
        owner.TriggerEvent(BeEventType.onChangeBuffRangeRadius, new object[] { (int)buffInfoID, radiusArray });
        buffRangeRadius *= VFactor.NewVFactor(radiusArray[0], GlobalLogic.VALUE_1000);*/
        
        var eventData = owner.TriggerEventNew(BeEventType.onChangeBuffRangeRadius, new EventParam(){m_Int = buffInfoID, m_Int2 = GlobalLogic.VALUE_1000});
        buffRangeRadius *= VFactor.NewVFactor(eventData.m_Int2, GlobalLogic.VALUE_1000);
	}

    public void OnRemove(BeActor owner)
	{
		if (condition == BuffCondition.RANGE)
		{
			if (effect != null)
			{
				owner.m_pkGeActor.DestroyEffect(effect);
				effect = null;
			}
		}
	}

	public void DoEnhance(BuffInfoData enhance, bool addLevel=false)
	{
		attack 	+= enhance.attack;
		if (addLevel){
			level 	+= enhance.level;
			abnormalLevel += enhance.level;
		}

		prob	+= enhance.prob;
		duration += enhance.duration;
		if (enhance.attackPercent != 0)
		{
			attack += IntMath.Float2Int(attack * enhance.attackPercent/(float)(GlobalLogic.VALUE_1000));
		}
		
	}




	public bool ContainSkillID(int skillID)
	{
		if (conditionSkillIDs == null)
			return false;
		return conditionSkillIDs.Contains(skillID);
	}


	private bool checkCanRemoveInrangers(BeActor item)
	{
		if (item.IsDead() || !targets.Contains(item))
		{
			//Logger.LogErrorFormat("out range name:{0}", item.GetName());
			item.buffController.RemoveBuff(buffID);
			return true;
		}

		return false;
	}

	public void UpdateCheckRange(int delta, BeActor owner)
	{
		if (owner == null)
			return;

		timeAcc += delta;
		if (timeAcc >= checkInterval)
		{
			timeAcc -= checkInterval;

			//var targets = new List<BeActor>();
			if (targets == null)
				targets = new List<BeActor>();

			if (target == BuffTarget.ENEMY)
			{
				owner.CurrentBeScene.FindTargets(targets, owner, VInt.NewVInt(buffRangeRadius, (long)GlobalLogic.VALUE_1000));
			}
			else if (target == BuffTarget.SELF)
			{
				owner.buffController.TryAddBuff(buffID, duration, level, prob, attack);
				return;
			}
			else if (target == BuffTarget.FRIEND)
			{
				owner.CurrentBeScene.FindTargets(targets, owner, VInt.NewVInt(buffRangeRadius, (long)GlobalLogic.VALUE_1000), true);
			}
			else if (target == BuffTarget.FRIEND_NOTSELF)
			{
				owner.CurrentBeScene.FindTargets(targets, owner, VInt.NewVInt(buffRangeRadius, (long)GlobalLogic.VALUE_1000), true);
				if (targets.Contains(owner))
				{
					targets.Remove(owner);
				}
			}
			else if (target == BuffTarget.RANGE_FRIEND_ADNSELF)
			{
				owner.CurrentBeScene.FindTargets(targets, owner, VInt.NewVInt(buffRangeRadius, (long)GlobalLogic.VALUE_1000), true);
			}
			//查找友方玩家
			else if (target == BuffTarget.RANGE_FRIENDHERO)
			{
				BeUtility.GetAllFriendPlayers(owner, targets);
			}
			else if (target == BuffTarget.RANGE_ENEMYHERO)
			{
				BeUtility.GetAllEnemyPlayers(owner, targets);
			}
			else if (target == BuffTarget.RANGE_FRIEND_NOTSUMMON)
			{
				BeGetRangeFriendNotSummon filter = new BeGetRangeFriendNotSummon();
				owner.CurrentBeScene.FindTargets(targets, owner, VInt.NewVInt(buffRangeRadius, (long)GlobalLogic.VALUE_1000), true, filter);
			}
			else if (target == BuffTarget.OUT_OF_RANGE_ENEMY)
			{
				BeGetConcentricCircleTarget filter = new BeGetConcentricCircleTarget();
				filter.m_Owner = owner;
				filter.m_OwnerPosXY = new VInt2(owner.GetPosition().x, owner.GetPosition().y);
				filter.m_MinCircleRadius = VInt.NewVInt(buffRangeRadius, GlobalLogic.VALUE_1000);
				filter.m_MaxCircleRadius = BeGetConcentricCircleTarget.LargeMaxCirleRadius;
				owner.CurrentBeScene.GetFilterTarget(targets, filter);
			}
			else if (target == BuffTarget.RANGE_OWNER)
			{
				targets.Clear();
				var topOwner = owner.GetTopOwner(owner) as BeActor;
				if (topOwner != null && topOwner.GetPID() != owner.GetPID())
				{
					if (topOwner.GetDistance(owner) < VInt.NewVInt(buffRangeRadius, (long)GlobalLogic.VALUE_1000))
						targets.Add(topOwner);
				}
			}

			if (inRangers == null)
				inRangers = new List<BeActor>();

            for (int i=0; i<targets.Count; ++i)
			{
				if (!inRangers.Contains(targets[i])) {
					//Logger.LogErrorFormat("in range name:{0}", targets[i].GetName());
					inRangers.Add(targets[i]);
				}
			}

			inRangers.RemoveAll(checkCanRemoveInrangers);

			for(int i=0; i<inRangers.Count; ++i)
			{
				if (inRangers[i].buffController.HasBuffByID(buffID) == null)
					inRangers[i].buffController.TryAddBuff(buffID, -1, level, GlobalLogic.VALUE_1000, attack);
			}
		}
	}

	public void DoRelease()
	{
		if (inRangers != null)
		{
			inRangers.RemoveAll(
				item =>
				{
					item.buffController.RemoveBuff(buffID);
					return true;
				}
			);
		}
			
	}

	public void Update(int delta)
	{
		if (isCooldown)
		{
			cdAcc += delta;
			if (cdAcc >= CD)
			{
				StopCD();
			}
		}
	}

    public void StartInitCD()
    {
        SetCDRemain(CD - startCD);
    }

	public void StartCD()
	{
		if (!NeedCD())
			return;

		Logger.LogProcessFormat("buffinfo:{0} startcd:{1}", buffInfoID, CD);
		isCooldown = true;
		cdAcc = 0;
	}

	public void StopCD()
	{
		Logger.LogProcessFormat("buffinfo:{0} stopcd:{1}", buffInfoID, CD);
		isCooldown = false;
		cdAcc = 0;
	}

	public bool IsCD()
	{
		return isCooldown;
	}

	public bool NeedCD()
	{
		return CD > 0;
	}

	public int GetCDAcc()
	{
		return cdAcc;
	}

    public void SetCDRemain(int cdRemain)
    {
        isCooldown = true;
        cdAcc = cdRemain;
    }
}

