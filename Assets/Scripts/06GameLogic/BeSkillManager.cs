using System;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

public class BeSkillManager
{
	public enum SkillCannotUseType
	{
		CAN_USE = 0,
		NO_HP,
		NO_MP,
		IN_CD,
		NO_RIGHT_WEAPON,

		// 特殊技能使用(
		NO_KEYING,
		//没有无色晶体
		NO_CRYSTAL,
		//不在残影凯贾状态
		NO_CYZKJ,
		//召唤数量限制
		MONSTER_COUNT_LIMITM,
		//召唤距离限制
		MONSTER_DIS_LIMITM,
		//技能不能使用
		CAN_NOT_USE,
	}

	private static List<int> LiGuiSkillList = new List<int>() { 1901, 1902, 1903, 1904 };
	private static List<int> NormalSkillList = new List<int>() { 1500, 1501, 1502 };
	private static List<int> AttackComboSkillList = new List<int>() { 1506, 1507, 1508, 1522, 1523, 1913, 1914, 1915, 1916, 1917 };

	protected BeActor owner;
	protected bool needCost;

	protected int m_iPreSkillID;
	protected int m_iCurSkillID;

	protected Dictionary<int, BeSkill> skillList = new Dictionary<int, BeSkill>();
	protected List<BeSkill> updateSkillList = new List<BeSkill>();

	public int skillPhase
	{
		set
		{
			_skillPhase = value;
		}
		get
		{
			return _skillPhase;
		}
	}
	protected int _skillPhase;
	protected int[] skillPhases;

	public int[] SkillPhaseArray => skillPhases;

	//slot为key 对应skill id
	public Dictionary<int, int> skillSlotMap = null;

	public BeSkillManager(BeActor owner)
	{
		this.owner = owner;
	}

	public Dictionary<int, BeSkill> GetSkills()
	{
		return skillList;
	}

	public List<BeSkill> GetSkillList()
	{
		return updateSkillList;
	}

	public void ClearSkillList()
    {
		skillList.Clear();
		updateSkillList.Clear();
	}

	public void SetSkillPhases(int skillID)
	{
		BDEntityActionInfo info = owner.GetActionInfoBySkillID(skillID);
		if (info != null)
		{
			if (info.skillPhases.Length > 0)
			{
				skillPhases = info.skillPhases;
			}
			else
			{
				skillPhases = new int[1];
				skillPhases[0] = skillID;
			}
		}
		else
		{
			skillPhases = null;
		}
	}

	public void ResetSkillPhase(int skillID)
	{
		SetSkillPhases(skillID);
		skillPhase = 0;
	}

	public int GetSkillPhaseId()
	{
		int skillid = -1;

		if (skillPhases != null)
		{
			int totalSkillPhase = skillPhases.Length;
			if (skillPhase < totalSkillPhase)
			{
				skillid = skillPhases[skillPhase];
				skillPhase++;
			}
		}
		return skillid;
	}

	//设置当前的技能阶段数组
	public void SetCurrentSkillPhases(int[] phases)
	{
		skillPhases = phases;
	}

	public void LoadOneSkillAndConfig(int skillID, int skillLevel, int resID = 0)
	{
		var skillInfos = new Dictionary<int, int>();
		skillInfos.Add(skillID, 1);
		owner.LoadSkillConfig(skillInfos, true, resID);
		LoadOneSkill(skillID, skillLevel);
	}

	public bool LoadOneSkill(int skillID, int skillLevel)
	{
		string skillName = "Skill" + skillID;
		Type skillType = TypeTable.GetType(skillName);
		BeSkill skill;
		if (null != skillType)
			skill = (BeSkill)Activator.CreateInstance(skillType, skillID, skillLevel);
		else
			skill = new BeSkill(skillID, skillLevel);

		if (skill == null || TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(skillID) == null)
			return false;

		AddSkill(skillID, skill);
		return true;
	}

	public void LoadSkill(Dictionary<int, int> skillInfos, bool loadConfigBySkills = false, int resID = 0)
	{
		if (owner != null)
		{
			owner.LoadSkillConfig(skillInfos, loadConfigBySkills, resID);
		}

		Dictionary<int, int>.Enumerator enumerator = skillInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int skillID = enumerator.Current.Key;
			int skillLevel = enumerator.Current.Value;

			LoadOneSkill(skillID, skillLevel);
		}
	}

	public void AddSkill(int sid, BeSkill skill)
	{
		if (!skillList.ContainsKey(sid))
		{
			skill.owner = owner;
			skill.Init();

			skillList.Add(sid, skill);
			updateSkillList.Add(skill);
		}
		else
		{
			Logger.Log("已经有一个ID " + sid + "的技能了");
		}
	}

	public void UpdateSkill(int deltaTime)
    {
		//Dictionary<int, BeSkill>.Enumerator enumerator = skillList.GetEnumerator();
		//while(enumerator.MoveNext())
		//{
		//    var skill = enumerator.Current.Value;
		//    skill.Update(deltaTime);
		//}
		for (int i = 0; i < updateSkillList.Count; i++)
		{
			updateSkillList[i].Update(deltaTime);
		}
	}

	public void StartInitCDForSkill(bool inTown = false)
	{
		if (BattleMain.IsModeTrain(owner.battleType))
			return;

		bool isPvPMode = BattleMain.IsModePvP(owner.battleType);

		Dictionary<int, BeSkill>.Enumerator enumerator = skillList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			BeSkill skill = enumerator.Current.Value;
			skill.inTown = inTown;
			if (skill != null)
			{
				skill.StartInitCD(isPvPMode);
			}
		}
	}

	public void PostInitSkills(bool inTown = false)
	{
		bool isPvPMode = BattleMain.IsModePvP(owner.battleType);

		string errorReporter = string.Empty;
		Dictionary<int, BeSkill>.Enumerator enumerator = skillList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BeSkill skill = enumerator.Current.Value;
				skill.inTown = inTown;

				if (skill != null)
				{
					errorReporter = string.Format("occu {0} skillId {1}", owner.professionID, skill.skillID);
					skill.PostInit();
					//skill.StartInitCD(isPvPMode);
				}
			}
		}
		catch (Exception e)
		{
#if !SERVER_LOGIC
			if (ClientSystemManager.instance != null && ClientSystemManager.instance.CurrentSystem != null)
				Logger.LogErrorFormat("PostInitSkills occur error {0} reason {1} currentSystem{2}", errorReporter, e.ToString(), ClientSystemManager.instance.CurrentSystem.GetName());
#else
            Logger.LogErrorFormat("PostInitSkills occur error {0} reason {1}", errorReporter, e.ToString());
#endif
		}
		if (!BattleMain.IsModePvP(owner.battleType))
			StartInitCDForSkill(inTown);

        //全技能cd缩减处理
        //var data = owner.GetEntityData();
        //VRate cdReduceAdd = VRate.zero;
        //if (data != null && data.battleData.cdReduceRate > 0)
        //{
        //    cdReduceAdd = (int)data.battleData.cdReduceRate;
        //}

        //enumerator = skillList.GetEnumerator();
        //while (enumerator.MoveNext())
        //{
        //    BeSkill skill = enumerator.Current.Value;
        //    if (skill != null)
        //    {
        //        skill.cdReduceRate += cdReduceAdd;
        //    }
        //}


        //更新技能等级信息
        enumerator = skillList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int sid = enumerator.Current.Key;
			BeSkill skill = enumerator.Current.Value;
			if (owner.GetEntityData().skillLevelInfo.ContainsKey(sid))
			{
				owner.GetEntityData().skillLevelInfo[sid] = skill.level;
			}
		}
	}

	public bool HasSkill(int skillID)
	{
		return skillList.ContainsKey(skillID);
	}

	public BeSkill GetSkill(int skillID)
	{
		if (!HasSkill(skillID))
			return null;

		return skillList[skillID];
	}

	public void SetCurSkillID(int skillID)
    {
		m_iCurSkillID = skillID;
    }

	public int GetCurSkillID()
    {
		return m_iCurSkillID;
	}

	public BeSkill GetCurrentSkill()
    {
		return GetSkill(GetCurSkillID());
	}

	public void SetPreSkillID(int skillID)
    {
		m_iPreSkillID = skillID;
    }

	public int GetPreSkillID()
    {
		return m_iPreSkillID;
    }

	public void PostInitOneSkill(int skillID)
	{
		var skill = GetSkill(skillID);
		if (skill == null)
			return;

		bool isPvPMode = BattleMain.IsModePvP(owner.battleType);

		skill.PostInit();
		skill.StartInitCD(isPvPMode);


		//全技能cd缩减处理
		var data = owner.GetEntityData();
		VRate cdReduceAdd = VRate.zero;
		if (data != null && data.battleData.cdReduceRate > 0)
		{
			cdReduceAdd = (int)data.battleData.cdReduceRate;
		}

		skill.cdReduceRate += cdReduceAdd;

		if (!owner.GetEntityData().skillLevelInfo.ContainsKey(skillID))
		{
			owner.GetEntityData().skillLevelInfo.Add(skillID, skill.level);
		}

		data.UpdateLevel(skillID, skill.level);
	}


	public SkillCannotUseType GetSkillCannotUseType(BeSkill skill)
	{
		SkillCannotUseType type = SkillCannotUseType.CAN_USE;
		if (skill != null)
		{
			if (skill.isCooldown)
			{
				type = SkillCannotUseType.IN_CD;
			}
			else
			{
				bool mpRet = skill.GetMPCost() <= owner.attribute.GetMP();
				bool hpRet = skill.GetHPCost(owner.attribute.GetMaxHP()) <= owner.attribute.GetHP();
				bool crystalRet = skill.GetCrystalCost() <= owner.attribute.GetCrystalNum();

				switch (skill.costMode)
				{
					case SkillCostMode.ALL:
						if (!mpRet)
							type = SkillCannotUseType.NO_MP;
						else if (!hpRet)
							type = SkillCannotUseType.NO_HP;
						else if (!crystalRet)
							type = SkillCannotUseType.NO_CRYSTAL;
						break;
					case SkillCostMode.HP_ONLY:
						if (!hpRet)
							type = SkillCannotUseType.NO_HP;
						break;
					case SkillCostMode.MP_ONLY:
						if (!mpRet)
							type = SkillCannotUseType.NO_MP;
						break;
				};
			}

			if (type == SkillCannotUseType.CAN_USE)
			{
				type = skill.GetCannotUseType();
			}
		}

		return type;
	}

	public void SetNeedCost(bool needCost)
    {
		this.needCost = needCost;
    }

	public bool CanCost(BeSkill skill)
	{
		// by duanduan,
		// 宏控制调试功能
#if DEBUG_FIGHT || UNITY_EDITOR
		if (!Global.Settings.skillHasCooldown)
			return true;
#endif

		if (skill != null && skill.NeedCost() && needCost)
		{
			bool ret = false;
			bool mpRet = skill.GetMPCost() <= owner.attribute.GetMP();
			bool hpRet = skill.GetHPCost(owner.attribute.GetMaxHP()) < owner.attribute.GetHP();
			bool crystalRet = skill.GetCrystalCost() <= owner.attribute.GetCrystalNum();//无色晶体

			if (skill.NeedCost())
			{
				switch (skill.costMode)
				{
					case SkillCostMode.ALL:
						ret = hpRet && mpRet && crystalRet;
						break;
					case SkillCostMode.HP_ONLY:
						ret = hpRet;
						break;
					case SkillCostMode.MP_ONLY:
						ret = mpRet;
						break;
				};

				return ret;
			}
		}

		return true;
	}

	public bool CheckMuscleShift(int curSkillID, int skillID)
	{
		bool isPvPMode = BattleMain.IsModePvP(owner.battleType);
		var b = owner.buffController.HasBuffByID(isPvPMode ? 183212 : 183211);
		if (b != null)
		{
			var buff = b as Buff183211;
			return buff.CanUseSkill(curSkillID, skillID);
		}
		return false;
	}

	public void UseMuscleShift(int curSkillId, int skillId)
	{
		bool isPvPMode = BattleMain.IsModePvP(owner.battleType);
		var b = owner.buffController.HasBuffByID(isPvPMode ? 183212 : 183211);
		if (b != null)
		{
			var buff = b as Buff183211;
			buff.Decrease(curSkillId, skillId);
			owner.buffController.TryAddBuff(isPvPMode ? 1832060 : 183206);
		}
	}

	public void ResetSkillCoolDown()
	{
		Dictionary<int, BeSkill>.Enumerator enumerator = skillList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var skill = enumerator.Current.Value;
			skill.ResetCoolDown();
		}
	}

	public bool CanUseSkill(int skillID)
	{
		if (!owner.stateController.CanAttack())
			return false;

		if (HasSkill(skillID))
		{
			var skill = GetSkill(skillID);

			if (!skill.CanUseSkill())
				return false;

			if (!CanCost(skill))
				return false;

			ActionState currentStat = (ActionState)owner.sgGetCurrentState();

			bool canAbortCurSkill = true;

			if (currentStat == ActionState.AS_CASTSKILL)
			{
				if (skillID == GetCurSkillID())
				{
					canAbortCurSkill = false;
				}
				else
				{
					var curSkill = GetSkill(GetCurSkillID());
					if (curSkill != null)
					{
						canAbortCurSkill =  curSkill.CanBePositiveAbort() ||
											skill.CanInterrupt(GetCurSkillID(), curSkill.hit) ||
											CheckMuscleShift(GetCurSkillID(), skillID) ||
											curSkill.CanBeInterrupt(skillID);
					}
				}

				if (!skill.CheckSpellCondition(currentStat))
                {
					canAbortCurSkill = false;
				}
			}
			//决斗场机器人在被击状态也可以使用格挡技能
			else if (skill.CheckSpellCondition(currentStat) || (owner.isPkRobot && skillID == 1515 && currentStat == ActionState.AS_HURT))
			{
				canAbortCurSkill = true;
			}
			else
			{
				canAbortCurSkill = false;
			}

			if (skill.CanForceUseSkill())
			{
				canAbortCurSkill = true;
			}

			return canAbortCurSkill;
		}
		else
		{
			if (skillID > GlobalLogic.VALUE_1000)
			{
				if (owner.aiManager != null && owner.aiManager.isAutoFight)
				{

				}
#if UNITY_EDITOR && !LOGIC_SERVER
				else
				{
					Logger.LogErrorFormat("{0} {1}没有技能{2}", owner.GetName(), owner.GetEntityData().monsterID, skillID);
				}
#endif
			}
		}
		return false;
	}

	public int CheckComboSkill(int skillID = 0)
	{
		ActionState currentStat = (ActionState)owner.sgGetCurrentState();
		if (currentStat == ActionState.AS_CASTSKILL)
		{
			if (owner.m_cpkCurEntityActionInfo != null && owner.m_cpkCurEntityActionInfo.comboSkillID > 0)
			{
				var eventData = owner.TriggerEventNew(BeEventType.onReplaceComboSkill, new EventParam() { m_Int = owner.m_cpkCurEntityActionInfo.comboSkillID, m_Int2 = owner.m_cpkCurEntityActionInfo.comboStartFrame });
				int skillId = eventData.m_Int;
				int startFrame = eventData.m_Int2;

				if (owner.GetCurrentFrame() >= startFrame)
				{
					int nextSkillId = skillId;

					//三段斩和上挑特写
					if (SwitchFunctionUtility.IsOpen(26) && !owner.IsMonster() && owner.professionID == 10 && (nextSkillId == 1522 || nextSkillId == 1524)
#if !LOGIC_SERVER
						&& owner.battleType != BattleType.NewbieGuide
#endif
						)
					{
						nextSkillId = 0;
					}

					return nextSkillId;
				}
			}
		}

		return 0;
	}

	public void DoSkillCost(BeSkill skill)
	{
		if (!needCost || skill == null || !skill.NeedCost())
        {
			return;
        }

		int mpCost = skill.GetMPCost();
		int hpCost = skill.GetHPCost(owner.attribute.GetMaxHP());
		int crystalCost = skill.GetCrystalCost();

		//TODO 无色晶体消耗
		switch (skill.costMode)
		{
			case SkillCostMode.ALL:
				if (mpCost > 0)
				{
					owner.DoMPChange(-mpCost, false);
				}
				if (hpCost > 0)
				{
					owner.DoHPChange(-hpCost, false);
				}
				if (crystalCost > 0)
				{
					owner.GetEntityData().ModifyCrystalessNum(-crystalCost);
					owner.TriggerEventNew(BeEventType.OnUseCrystal);
					//如果是本地玩家才向服务器消耗无色晶体
					if (owner.isLocalActor)
					{
						//TODO
						if (!BattleMain.IsModeTrain(BattleMain.battleType) && !ReplayServer.GetInstance().IsReplay())
						{
							BeUtility.UseItemInBattle(Global.CRYSTAL_ITEM_ID, skill.skillID, crystalCost);
						}
					}
				}
				break;
			case SkillCostMode.HP_ONLY:
				if (hpCost > 0)
				{
					owner.DoHPChange(-hpCost, false);
				}
				break;
			case SkillCostMode.MP_ONLY:
				if (mpCost > 0)
				{
					owner.DoMPChange(-mpCost, false);
				}
				break;
		};
	}

	public bool UseSkill(int skillID, bool force = false)
	{
		var skill = GetSkill(skillID);
#if MG_TEST_EXTENT
        if (owner.IsProcessRecord())
        {
            if (skill != null && !BattleMain.IsModePvP(owner.battleType) && owner.pauseAI && owner.isMainActor)
            {
				owner.GetRecordServer().RecordProcess("PID:{0}-{1},forceUseSkill:{2},CanUseSkill:{3},skill.CanUseSkill:{4},skill.isCoolDown:{5}", owner.GetPID(), owner.GetName(), force, CanUseSkill(skillID), skill.CanUseSkill(), skill.isCooldown);
            }

        }
#endif
        if (force || CanUseSkill(skillID) && CheckCondition(skillID))
		{
			if (skill != null && skill.CanUseSkill() && owner.FrameRandom.Range1000() <= skill.useRate)
			{

				if (owner.IsCastingSkill())
				{
					var curSkill = GetCurrentSkill();
					if (curSkill != null && (
						curSkill.CanBePositiveAbort() && !skill.CanBePositiveAbort()
						|| skill.CanInterrupt(GetCurSkillID())
						|| curSkill.CanBeInterrupt(skillID)))
					{
						if (owner.m_pkGeActor != null)
							owner.m_pkGeActor.CreateSnapshot(Color.white, Global.Settings.snapDuration);
					}
					else if (curSkill != null && CheckMuscleShift(GetCurSkillID(), skillID))
					{
						if (owner.m_pkGeActor != null)
							owner.m_pkGeActor.CreateSnapshot(Color.white, Global.Settings.snapDuration);
						UseMuscleShift(GetCurSkillID(), skillID);
					}
				}

				DoSkillCost(skill);

				//Logger.LogErrorFormat("use skill id:{0}", skillID);
				skill.pressedForwardMove = owner.IsPressForwardMoveCmd();

				int skillId = skillID;
				var handle = owner.TriggerEventNew(BeEventType.onReplaceSkill, new EventParam() { m_Int = skillId });
				skillId = handle.m_Int;

				BeStateData state = new BeStateData((int)ActionState.AS_CASTSKILL) { _StateData = skillId };
				owner.Locomote(state);

				return true;
			}
		}

		if (skill != null && owner.isLocalActor/* && !skill.CanUseSkill()*/)
		{
			var type = GetSkillCannotUseType(skill);

			if (type != SkillCannotUseType.CAN_USE)
			{

				bool needShock = true;
                var param = owner.TriggerEventNew(BeEventType.onChangeShock, new EventParam() { m_Bool = needShock });

				if(param.m_Bool)
				{
					owner.AddShock(Global.Settings.playerSkillCDShockData);
				}
				
				string picName = "";
				switch (type)
				{
					case SkillCannotUseType.IN_CD:
						picName = "UI/Font/new_font/pic_incd.png";
						break;
					case SkillCannotUseType.NO_HP:
						picName = "UI/Font/new_font/pic_nohp.png";
						break;
					case SkillCannotUseType.NO_MP:
						picName = "UI/Font/new_font/pic_nomp.png";
						break;
					case SkillCannotUseType.NO_KEYING:
						picName = "UI/Font/new_font/pic_qsksy.png";
						break;
					case SkillCannotUseType.NO_RIGHT_WEAPON:
						picName = $"UI/Font/new_font/pic_sycs_{skill.skillData.NeedWeaponType}.png:pic_sycs_{skill.skillData.NeedWeaponType}_0";
						break;
					case SkillCannotUseType.NO_CRYSTAL:
						picName = "UI/Font/new_font/pic_wsjtbz.png";
						break;
					case SkillCannotUseType.NO_CYZKJ:
						picName = "UI/Font/new_font/pic_cyzkj.png";
						break;
					case SkillCannotUseType.MONSTER_COUNT_LIMITM:
						picName = "UI/Font/new_font/pic_zhslsx.png";
						break;
					case SkillCannotUseType.MONSTER_DIS_LIMITM:
						picName = "UI/Font/new_font/pic_yqtbh.png";
						break;
					case SkillCannotUseType.CAN_NOT_USE:
						picName = "UI/Font/new_font/pic_sycs.png:pic_sycs";
						break;
				}
				if (owner.m_pkGeActor != null)
				{
					owner.m_pkGeActor.CreateHeadText(HitTextType.SKILL_CANNOTUSE, picName);
				}
			}
		}

		return false;
	}

	private bool CheckCondition(int skillID)
	{
		if (owner.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
		{
			if ((LiGuiSkillList.Contains(GetCurSkillID()) && NormalSkillList.Contains(skillID))
			|| (LiGuiSkillList.Contains(skillID) && NormalSkillList.Contains(GetCurSkillID())))
			{
				return CanInterrupSkill();
			}
		}
		return true;
	}

	private bool CanInterrupSkill()
	{
		BDEntityActionInfo actionInfo = owner.GetActionInfoBySkillID(GetCurSkillID());
		if (actionInfo == null) return true;
		return owner.GetCurrentFrame() >= actionInfo.comboStartFrame && actionInfo.comboStartFrame > 0;
	}

	public bool IsSkillCoolDown(int skillId)
	{
		var skill = GetSkill(skillId);
		if (skill == null)
			return false;
		return skill.isCooldown;
	}

	public void ResetSkillCoolDown(int skillId)
	{
		var skill = GetSkill(skillId);
		if (skill == null)
			return;
		skill.ResetCoolDown();
	}

	public void CancelSkills()
	{
		Dictionary<int, BeSkill>.Enumerator enumerator = skillList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			BeSkill skill = enumerator.Current.Value;
			if (skill != null)
			{
				skill.Cancel();
			}
		}
	}

	public void RemoveSkill(int skillId)
	{
		if (skillList.ContainsKey(skillId))
		{
			if (skillList[skillId] != null)
			{
				try
				{
					skillList[skillId].DeInit();
				}
				catch (Exception e)
				{
					Logger.LogErrorFormat("try remove skill {0} failed reason {1}", skillId, e.ToString());
				}
			}

			skillList.Remove(skillId);
			int index = GetUpdateSkillIndex(skillId);
			if (index >= 0)
			{
				updateSkillList.RemoveAt(index);
			}

		}
	}
	private int GetUpdateSkillIndex(int skillID)
	{
		for (int i = 0; i < updateSkillList.Count; i++)
		{
			if (updateSkillList[i].skillID == skillID)
			{
				return i;
			}
		}
		return -1;
	}
}
