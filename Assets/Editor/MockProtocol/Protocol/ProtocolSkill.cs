using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  技能配置方案
	/// </summary>
	[AdvancedInspector.Descriptor(" 技能配置方案", " 技能配置方案")]
	public enum SkillConfigType
	{

		SKILL_CONFIG_PVE = 1,

		SKILL_CONFIG_PVP = 2,

		SKILL_CONFIG_EQUAL_PVP = 3,
	}

}
