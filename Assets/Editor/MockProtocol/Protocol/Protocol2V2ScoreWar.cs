using System;
using System.Text;

namespace Mock.Protocol
{

	public enum ScoreWar2V2Status
	{
		/// <summary>
		/// 未开始
		/// </summary>
		[AdvancedInspector.Descriptor("未开始", "未开始")]
		SWS_2V2_INVALID = 0,
		/// <summary>
		/// 准备状态
		/// </summary>
		[AdvancedInspector.Descriptor("准备状态", "准备状态")]
		SWS_2V2_PREPARE = 1,
		/// <summary>
		/// 战斗状态
		/// </summary>
		[AdvancedInspector.Descriptor("战斗状态", "战斗状态")]
		SWS_2V2_BATTLE = 2,
		/// <summary>
		/// 等待结束
		/// </summary>
		[AdvancedInspector.Descriptor("等待结束", "等待结束")]
		SWS_2V2_WAIT_END = 3,
		/// <summary>
		/// 最大
		/// </summary>
		[AdvancedInspector.Descriptor("最大", "最大")]
		SWS_2V2_MAX = 4,
	}

}