using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  充值商城类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 充值商城类型", " 充值商城类型")]
	public enum ChargeMallType
	{
		/// <summary>
		///  充值商品
		/// </summary>
		[AdvancedInspector.Descriptor(" 充值商品", " 充值商品")]
		Charge = 0,
		/// <summary>
		///  人民币礼包
		/// </summary>
		[AdvancedInspector.Descriptor(" 人民币礼包", " 人民币礼包")]
		Packet = 1,
		/// <summary>
		///  理财
		/// </summary>
		[AdvancedInspector.Descriptor(" 理财", " 理财")]
		FinancialPlan = 2,
		/// <summary>
		///  福利:每日充值
		/// </summary>
		[AdvancedInspector.Descriptor(" 福利:每日充值", " 福利:每日充值")]
		DayChargeWelfare = 3,
		/// <summary>
		///  冒险通行证王者版
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险通行证王者版", " 冒险通行证王者版")]
		AdventurePassKing = 4,
		/// <summary>
		///  礼遇特权卡
		/// </summary>
		[AdvancedInspector.Descriptor(" 礼遇特权卡", " 礼遇特权卡")]
		GiftRightCard = 5,
	}


	public enum ChargeGoodsTag
	{
		/// <summary>
		///  推荐
		/// </summary>
		[AdvancedInspector.Descriptor(" 推荐", " 推荐")]
		Recommend = 1,
		/// <summary>
		///  福利
		/// </summary>
		[AdvancedInspector.Descriptor(" 福利", " 福利")]
		Welfare = 2,
	}

}
