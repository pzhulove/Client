using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  赏金联赛状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 赏金联赛状态", " 赏金联赛状态")]
	public enum PremiumLeagueStatus
	{
		/// <summary>
		///  初始状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 初始状态", " 初始状态")]
		PLS_INIT = 0,
		/// <summary>
		///  报名
		/// </summary>
		[AdvancedInspector.Descriptor(" 报名", " 报名")]
		PLS_ENROLL = 1,
		/// <summary>
		///  预赛
		/// </summary>
		[AdvancedInspector.Descriptor(" 预赛", " 预赛")]
		PLS_PRELIMINAY = 2,
		/// <summary>
		///  八强准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 八强准备", " 八强准备")]
		PLS_FINAL_EIGHT_PREPARE = 3,
		/// <summary>
		///  八强赛
		/// </summary>
		[AdvancedInspector.Descriptor(" 八强赛", " 八强赛")]
		PLS_FINAL_EIGHT = 4,
		/// <summary>
		///  四强赛
		/// </summary>
		[AdvancedInspector.Descriptor(" 四强赛", " 四强赛")]
		PLS_FINAL_FOUR = 5,
		/// <summary>
		///  决赛
		/// </summary>
		[AdvancedInspector.Descriptor(" 决赛", " 决赛")]
		PLS_FINAL = 6,
		/// <summary>
		///  决赛结束等待清除
		/// </summary>
		[AdvancedInspector.Descriptor(" 决赛结束等待清除", " 决赛结束等待清除")]
		PLS_FINAL_WAIT_CLEAR = 7,
	}

	/// <summary>
	///  赏金联赛奖励类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 赏金联赛奖励类型", " 赏金联赛奖励类型")]
	public enum PremiumLeagueRewardType
	{
		/// <summary>
		///  第1名的奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 第1名的奖励", " 第1名的奖励")]
		PL_REWARD_NO_1 = 0,
		/// <summary>
		///  第2名的奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 第2名的奖励", " 第2名的奖励")]
		PL_REWARD_NO_2 = 1,
		/// <summary>
		///  3-4名的奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 3-4名的奖励", " 3-4名的奖励")]
		PL_REWARD_NO_3_4 = 2,
		/// <summary>
		///  5-8名的奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 5-8名的奖励", " 5-8名的奖励")]
		PL_REWARD_NO_5_8 = 3,
		/// <summary>
		///  9-20名的奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 9-20名的奖励", " 9-20名的奖励")]
		PL_REWARD_NO_9_20 = 4,
	}

}
