using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class Skill1615 : BeSkill {

	int startPercent = 40;
	int powerInitValue = 10;
	int powerLevelValue = 1;
	int avoidanceInitValue = 10;
	int avoidanceLevelValue = 1;

	int savedPercent = 0;
	int savedPower = 0;
	int savedAvoidance = 0;

	BeEntityData entityData;

    IBeEventHandle handle = null;
    IBeEventHandle m_RebornHandle = null;

	public Skill1615(int sid, int skillLevel):base(sid, skillLevel)
	{

	}

	public override void OnInit ()
	{
		startPercent = BattleMain.IsModePvP(battleType) ? 
            TableManager.GetValueFromUnionCell(skillData.ValueA[1], level) :
            TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);

	

        InitValue();
    }

	public override void OnPostInit ()
    {
        InitValue();
        entityData = owner.GetEntityData();

        //Logger.LogErrorFormat("init power:{0} init:avoidance:{1}", entityData.battleData._baseAtk, entityData.battleData.dodge);
        RemoveHandle();
        handle = owner.RegisterEventNew(BeEventType.onHPChange, (args)=>{
			DoWork();
		});

        m_RebornHandle = owner.RegisterEventNew(BeEventType.onReborn, OnRebornHandle);

    }

    void RemoveHandle()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }

        if (m_RebornHandle != null)
        {
            m_RebornHandle.Remove();
            m_RebornHandle = null;
        }
    }

    void InitValue()
    {
        //重新执行的时候也获取下
        avoidanceInitValue = BattleMain.IsModePvP(battleType) ? 
            TableManager.GetValueFromUnionCell(skillData.ValueD[1], level) :
            TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
        avoidanceLevelValue = BattleMain.IsModePvP(owner.battleType) ?
            TableManager.GetValueFromUnionCell(skillData.ValueG[0], level) :
            TableManager.GetValueFromUnionCell(skillData.ValueE[0], level);

        powerInitValue = BattleMain.IsModePvP(battleType) ?
        TableManager.GetValueFromUnionCell(skillData.ValueB[1], level) :
        TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);

        powerLevelValue = BattleMain.IsModePvP(owner.battleType) ?
                          TableManager.GetValueFromUnionCell(skillData.ValueF[0], level) :
                          TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        if (owner != null && owner.buffController.HasBuffByID(1400325) != null)
        {
            startPercent = 50;
        }
    }

    protected void OnRebornHandle(BeEvent.BeEventParam eventParam)
    {
        Restore();
    }

    void DoWork()
	{
		VFactor hpRate = owner.GetEntityData().GetHPRate() * 100;
		//!! 这里有问题！！
		int curPercent = hpRate.integer;//Mathf.CeilToInt(hpRate * (float)(GlobalLogic.VALUE_100));

        //Logger.LogErrorFormat("hprate:{0} percent:{1}", hpRate, curPercent);
        //Logger.LogErrorFormat("DoWork power:{0} init:avoidance:{1}", entityData.battleData._baseAtk, entityData.battleData.dodge);

        if (curPercent > startPercent)
		{
			Restore();
			return;
		}
			
		if (curPercent != savedPercent)
		{
			Restore();
			savedPercent = curPercent;
			SetValue(curPercent);
		}
	}

	void Restore()
	{
		if (entityData == null)
			return;


		if (savedPower > 0)
		{
			entityData.SetAttributeValue(AttributeType.baseAtk, -savedPower, true);
			savedPower = 0;
		}

		if (savedAvoidance > 0)
		{
			entityData.SetAttributeValue(AttributeType.dodge, -savedAvoidance, true);
			savedAvoidance = 0;
		}

		savedPercent = 0;
    }

	void SetValue(int percent)
	{
		int power = powerInitValue + (startPercent - percent)*powerLevelValue;
		int avoidance = avoidanceInitValue + (startPercent - percent)*avoidanceLevelValue;

		//Logger.LogErrorFormat("percent:{0} power:{1}, avoidance:{2}", percent, power, avoidance);


		savedPower = power;
		entityData.SetAttributeValue(AttributeType.baseAtk, savedPower, true);

		savedAvoidance = avoidance;
		entityData.SetAttributeValue(AttributeType.dodge, savedAvoidance, true);

		//Logger.LogErrorFormat("after power:{0} init:avoidance:{1}", entityData.battleData._baseAtk, entityData.battleData.dodge);
	}

	public override void OnCancel ()
	{
		base.OnCancel ();
		Restore();
	}
}
