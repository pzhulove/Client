using System;
using GameClient;

/// <summary>
/// 强加技能
/// 加强下一次召唤类技能，替换其技能ID
/// </summary>
public class Skill1429 : BeSkill
{
	public Skill1429(int sid, int skillLevel) : base(sid, skillLevel)
	{
	}

	private int[] m_SkillIds;
	private int m_BuffId;

	private bool m_IsStronger = false;
	private IBeEventHandle m_Handle;
	
	public bool CanAndUseStronger()
	{
		var ret = m_IsStronger;
		m_IsStronger = false;
		return ret;
	}

	public override void OnInit()
	{
		base.OnInit();
		m_SkillIds = new int[skillData.ValueA.Count];
		for (int i = 0; i < skillData.ValueA.Count; i++)
		{
			m_SkillIds[i] = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
		}

		m_BuffId = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
	}

	public override void OnPostInit()
	{
		base.OnPostInit();
		if (m_Handle != null)
		{
			m_Handle.Remove();
			m_Handle = null;
		}
		m_Handle = owner.RegisterEventNew(BeEventType.onCastSkill, OnSkillStart);
	}

	private void OnSkillStart(BeEvent.BeEventParam param)
	{
		if(Array.IndexOf(m_SkillIds, param.m_Int) < 0)
			return;

		m_IsStronger = false;
		var buff = owner.buffController.HasBuffByID(m_BuffId);
		if (buff != null)
		{
			owner.buffController.RemoveBuff(buff);
			m_IsStronger = true;
		}
	}
}
