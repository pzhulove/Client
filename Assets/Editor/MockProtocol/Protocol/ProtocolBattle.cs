using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  活动状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 活动状态", " 活动状态")]
	public enum BattleType
	{

		BTY_NONE = 0,
		/// <summary>
		///  吃鸡
		/// </summary>
		[AdvancedInspector.Descriptor(" 吃鸡", " 吃鸡")]
		BTY_CHIJI = 1,
		/// <summary>
		///  地牢
		/// </summary>
		[AdvancedInspector.Descriptor(" 地牢", " 地牢")]
		BTY_DILAO = 2,
	}

}
