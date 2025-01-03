using System;
using System.Text;

namespace Mock.Protocol
{

	public enum AccountCounterType
	{

		ACC_COUNTER_INVALID = 0,
		/// <summary>
		///  赐福水晶
		/// </summary>
		[AdvancedInspector.Descriptor(" 赐福水晶", " 赐福水晶")]
		ACC_COUNTER_BLESS_CRYSTAL = 1,
		/// <summary>
		///  赐福经验
		/// </summary>
		[AdvancedInspector.Descriptor(" 赐福经验", " 赐福经验")]
		ACC_COUNTER_BLESS_CRYSTAL_EXP = 2,
		/// <summary>
		///  传承祝福
		/// </summary>
		[AdvancedInspector.Descriptor(" 传承祝福", " 传承祝福")]
		ACC_COUNTER_INHERIT_BLESS = 3,
		/// <summary>
		///  传承经验
		/// </summary>
		[AdvancedInspector.Descriptor(" 传承经验", " 传承经验")]
		ACC_COUNTER_INHERIT_BLESS_EXP = 4,
		/// <summary>
		///  精力货币
		/// </summary>
		[AdvancedInspector.Descriptor(" 精力货币", " 精力货币")]
		ACC_COUNTER_ENERGY_COIN = 5,
		/// <summary>
		///  赏金
		/// </summary>
		[AdvancedInspector.Descriptor(" 赏金", " 赏金")]
		ACC_COUNTER_BOUNTY = 6,
		/// <summary>
		///  公会红包日领取上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会红包日领取上限", " 公会红包日领取上限")]
		ACC_GUILD_REDPACKET_DAILY_MAX = 7,
		/// <summary>
		///  冒险通信证邮件发送
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险通信证邮件发送", " 冒险通信证邮件发送")]
		ACC_ADVENTURE_PASS_MAIL_SEND = 8,
		/// <summary>
		///  新服礼包打折标记
		/// </summary>
		[AdvancedInspector.Descriptor(" 新服礼包打折标记", " 新服礼包打折标记")]
		ACC_NEW_SERVER_GIFT_DISCOUNT = 9,
		/// <summary>
		///  招募硬币
		/// </summary>
		[AdvancedInspector.Descriptor(" 招募硬币", " 招募硬币")]
		ACC_COUNTER_HIRE_COIN = 10,
		/// <summary>
		///  招募推送
		/// </summary>
		[AdvancedInspector.Descriptor(" 招募推送", " 招募推送")]
		ACC_COUNTER_HIRE_PUS = 11,
	}

}
