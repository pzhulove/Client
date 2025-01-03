using System;
using System.Text;

namespace Mock.Protocol
{

	public enum AuctionType
	{

		Item = 0,

		Gold = 1,
	}


	public enum AuctionSortType
	{
		/// <summary>
		///  按价格升序
		/// </summary>
		[AdvancedInspector.Descriptor(" 按价格升序", " 按价格升序")]
		PriceAsc = 0,
		/// <summary>
		///  按价格降序
		/// </summary>
		[AdvancedInspector.Descriptor(" 按价格降序", " 按价格降序")]
		PriceDesc = 1,
	}


	public enum AuctionSellDuration
	{
		/// <summary>
		///  24小时
		/// </summary>
		[AdvancedInspector.Descriptor(" 24小时", " 24小时")]
		Hour_24 = 0,
		/// <summary>
		///  48小时
		/// </summary>
		[AdvancedInspector.Descriptor(" 48小时", " 48小时")]
		Hour_48 = 1,
	}


	public enum AuctionMainItemType
	{
		/// <summary>
		///  无效
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效", " 无效")]
		AMIT_INVALID = 0,
		/// <summary>
		///  武器
		/// </summary>
		[AdvancedInspector.Descriptor(" 武器", " 武器")]
		AMIT_WEAPON = 1,
		/// <summary>
		///  防具
		/// </summary>
		[AdvancedInspector.Descriptor(" 防具", " 防具")]
		AMIT_ARMOR = 2,
		/// <summary>
		///  首饰
		/// </summary>
		[AdvancedInspector.Descriptor(" 首饰", " 首饰")]
		AMIT_JEWELRY = 3,
		/// <summary>
		///  消耗品
		/// </summary>
		[AdvancedInspector.Descriptor(" 消耗品", " 消耗品")]
		AMIT_COST = 4,
		/// <summary>
		///  材料
		/// </summary>
		[AdvancedInspector.Descriptor(" 材料", " 材料")]
		AMIT_MATERIAL = 5,
		/// <summary>
		///  其它
		/// </summary>
		[AdvancedInspector.Descriptor(" 其它", " 其它")]
		AMIT_OTHER = 6,
		/// <summary>
		///  数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 数量", " 数量")]
		AMIT_NUM = 7,
	}

	/// <summary>
	///  拍卖行刷新原因
	/// </summary>
	[AdvancedInspector.Descriptor(" 拍卖行刷新原因", " 拍卖行刷新原因")]
	public enum AuctionRefreshReason
	{
		/// <summary>
		///  购买
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买", " 购买")]
		SRR_BUY = 0,
		/// <summary>
		///  上架
		/// </summary>
		[AdvancedInspector.Descriptor(" 上架", " 上架")]
		SRR_SELL = 1,
		/// <summary>
		///  下架
		/// </summary>
		[AdvancedInspector.Descriptor(" 下架", " 下架")]
		SRR_CANCEL = 2,
		/// <summary>
		///  抢购
		/// </summary>
		[AdvancedInspector.Descriptor(" 抢购", " 抢购")]
		SRR_RUSY_BUY = 3,
		/// <summary>
		///  扫货
		/// </summary>
		[AdvancedInspector.Descriptor(" 扫货", " 扫货")]
		SRR_SYS_RECOVERY = 4,
	}

	/// <summary>
	///  商品拍卖状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 商品拍卖状态", " 商品拍卖状态")]
	public enum AuctionGoodState
	{
		/// <summary>
		///  无效
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效", " 无效")]
		AGS_INVALID = 0,
		/// <summary>
		///  公示
		/// </summary>
		[AdvancedInspector.Descriptor(" 公示", " 公示")]
		AGS_PUBLIC = 1,
		/// <summary>
		///  上架
		/// </summary>
		[AdvancedInspector.Descriptor(" 上架", " 上架")]
		AGS_ONSALE = 2,
	}

	/// <summary>
	/// 拍卖行关注操作
	/// </summary>
	[AdvancedInspector.Descriptor("拍卖行关注操作", "拍卖行关注操作")]
	public enum AuctionAttentOpType
	{
		/// <summary>
		/// 关注
		/// </summary>
		[AdvancedInspector.Descriptor("关注", "关注")]
		ATOT_ATTENT = 1,
		/// <summary>
		/// 取消关注
		/// </summary>
		[AdvancedInspector.Descriptor("取消关注", "取消关注")]
		ATOT_CANCEL = 2,
	}

	/// <summary>
	/// 拍卖行关注类型
	/// </summary>
	[AdvancedInspector.Descriptor("拍卖行关注类型", "拍卖行关注类型")]
	public enum AuctionAttentType
	{
		/// <summary>
		/// 非关注
		/// </summary>
		[AdvancedInspector.Descriptor("非关注", "非关注")]
		ATT_NOTATTENT = 0,
		/// <summary>
		/// 关注
		/// </summary>
		[AdvancedInspector.Descriptor("关注", "关注")]
		ATT_ATTENT = 1,
	}

}
