using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 冠军赛状态
	/// </summary>
	[AdvancedInspector.Descriptor("冠军赛状态", "冠军赛状态")]
	public enum ChampionStatus
	{
		/// <summary>
		///  无状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 无状态", " 无状态")]
		CS_NULL = 0,
		/// <summary>
		///  报名
		/// </summary>
		[AdvancedInspector.Descriptor(" 报名", " 报名")]
		CS_ENROLL = 1,

		CS_PREPARE_BEGIN = 10,
		/// <summary>
		///  海选准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 海选准备", " 海选准备")]
		CS_SEA_SELECT_PREPARE = 11,
		/// <summary>
		///  复赛准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 复赛准备", " 复赛准备")]
		CS_RE_SEA_SELECT_PREPARE = 12,
		/// <summary>
		///  64强准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 64强准备", " 64强准备")]
		CS_64_SELECT_PREPARE = 13,
		/// <summary>
		///  16强准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 16强准备", " 16强准备")]
		CS_16_SELECT_PREPARE = 14,
		/// <summary>
		///  8强准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 8强准备", " 8强准备")]
		CS_8_SELECT_PREPARE = 15,
		/// <summary>
		///  4强准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 4强准备", " 4强准备")]
		CS_4_SELECT_PREPARE = 16,
		/// <summary>
		///  半决赛准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 半决赛准备", " 半决赛准备")]
		CS_2_SELECT_PREPARE = 17,
		/// <summary>
		///  决赛准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 决赛准备", " 决赛准备")]
		CS_1_SELECT_PREPARE = 18,

		CS_PREPARE_END = 19,

		CS_BATTLE_BEGIN = 50,
		/// <summary>
		///  海选战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 海选战斗", " 海选战斗")]
		CS_SEA_SELECT_BATTLE = 51,
		/// <summary>
		///  复赛战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 复赛战斗", " 复赛战斗")]
		CS_RE_SEA_SELECT_BATTLE = 52,
		/// <summary>
		///  64强战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 64强战斗", " 64强战斗")]
		CS_64_SELECT_BATTLE = 53,
		/// <summary>
		///  16强战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 16强战斗", " 16强战斗")]
		CS_16_SELECT_BATTLE = 54,
		/// <summary>
		///  8强战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 8强战斗", " 8强战斗")]
		CS_8_SELECT_BATTLE = 55,
		/// <summary>
		///  4强战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 4强战斗", " 4强战斗")]
		CS_4_SELECT_BATTLE = 56,
		/// <summary>
		///  半决赛战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 半决赛战斗", " 半决赛战斗")]
		CS_2_SELECT_BATTLE = 57,
		/// <summary>
		///  决赛战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 决赛战斗", " 决赛战斗")]
		CS_1_SELECT_BATTLE = 58,

		CS_BATTLE_END = 59,
		/// <summary>
		/// 下面阶段是给客户端显示用的
		/// </summary>
		[AdvancedInspector.Descriptor("下面阶段是给客户端显示用的", "下面阶段是给客户端显示用的")]
		CS_PRE_BEGIN = 80,
		/// <summary>
		///  海选开启前
		/// </summary>
		[AdvancedInspector.Descriptor(" 海选开启前", " 海选开启前")]
		CS_SEA_SELECT_PRE = 81,
		/// <summary>
		///  复赛开启前
		/// </summary>
		[AdvancedInspector.Descriptor(" 复赛开启前", " 复赛开启前")]
		CS_RE_SEA_SELECT_PRE = 82,
		/// <summary>
		///  64强开启前
		/// </summary>
		[AdvancedInspector.Descriptor(" 64强开启前", " 64强开启前")]
		CS_64_SELECT_PRE = 83,
		/// <summary>
		///  16强开启前
		/// </summary>
		[AdvancedInspector.Descriptor(" 16强开启前", " 16强开启前")]
		CS_16_SELECT_PRE = 84,
		/// <summary>
		///  8强开启前
		/// </summary>
		[AdvancedInspector.Descriptor(" 8强开启前", " 8强开启前")]
		CS_8_SELECT_PRE = 85,
		/// <summary>
		///  4强开启前
		/// </summary>
		[AdvancedInspector.Descriptor(" 4强开启前", " 4强开启前")]
		CS_4_SELECT_PRE = 86,
		/// <summary>
		///  半决赛开启前
		/// </summary>
		[AdvancedInspector.Descriptor(" 半决赛开启前", " 半决赛开启前")]
		CS_2_SELECT_PRE = 87,
		/// <summary>
		///  决赛开启前
		/// </summary>
		[AdvancedInspector.Descriptor(" 决赛开启前", " 决赛开启前")]
		CS_1_SELECT_PRE = 88,

		CS_PRE_END = 89,
		/// <summary>
		///  比赛结束展示状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 比赛结束展示状态", " 比赛结束展示状态")]
		CS_END_SHOW = 95,
		/// <summary>
		///  数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 数量", " 数量")]
		CS_NUM = 60,
	}


	public enum GambleType
	{
		/// <summary>
		/// 猜冠军
		/// </summary>
		[AdvancedInspector.Descriptor("猜冠军", "猜冠军")]
		GT_CHAMPION = 1,
		/// <summary>
		/// 猜单场胜负
		/// </summary>
		[AdvancedInspector.Descriptor("猜单场胜负", "猜单场胜负")]
		GT_1V1 = 2,
		/// <summary>
		/// 猜总比赛数				
		/// </summary>
		[AdvancedInspector.Descriptor("猜总比赛数				", "猜总比赛数				")]
		GT_BATTLE_COUNT = 3,
		/// <summary>
		/// 猜总决赛比分			
		/// </summary>
		[AdvancedInspector.Descriptor("猜总决赛比分			", "猜总决赛比分			")]
		GT_CHAMPION_BATTLE_SCORE = 4,
	}

}
