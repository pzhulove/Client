using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会职务
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会职务", " 公会职务")]
	public enum GuildPost
	{
		/// <summary>
		///  无效值
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效值", " 无效值")]
		GUILD_INVALID = 0,
		/// <summary>
		///  普通成员
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通成员", " 普通成员")]
		GUILD_POST_NORMAL = 1,
		/// <summary>
		///  精英
		/// </summary>
		[AdvancedInspector.Descriptor(" 精英", " 精英")]
		GUILD_POST_ELITE = 2,
		/// <summary>
		///  长老
		/// </summary>
		[AdvancedInspector.Descriptor(" 长老", " 长老")]
		GUILD_POST_ELDER = 11,
		/// <summary>
		///  副会长
		/// </summary>
		[AdvancedInspector.Descriptor(" 副会长", " 副会长")]
		GUILD_POST_ASSISTANT = 12,
		/// <summary>
		///  会长
		/// </summary>
		[AdvancedInspector.Descriptor(" 会长", " 会长")]
		GUILD_POST_LEADER = 13,
	}

	/// <summary>
	///  公会属性
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会属性", " 公会属性")]
	public enum GuildAttr
	{
		/// <summary>
		///  无效属性
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效属性", " 无效属性")]
		GA_INVALID = 0,
		/// <summary>
		///  名字	string	
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字	string	", " 名字	string	")]
		GA_NAME = 1,
		/// <summary>
		///  等级	UInt8	
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级	UInt8	", " 等级	UInt8	")]
		GA_LEVEL = 2,
		/// <summary>
		///  宣言 string
		/// </summary>
		[AdvancedInspector.Descriptor(" 宣言 string", " 宣言 string")]
		GA_DECLARATION = 3,
		/// <summary>
		///  部落资金 Int32
		/// </summary>
		[AdvancedInspector.Descriptor(" 部落资金 Int32", " 部落资金 Int32")]
		GA_FUND = 4,
		/// <summary>
		///  公告 string
		/// </summary>
		[AdvancedInspector.Descriptor(" 公告 string", " 公告 string")]
		GA_ANNOUNCEMENT = 5,
		/// <summary>
		///  公会建筑 GuildBuilding
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会建筑 GuildBuilding", " 公会建筑 GuildBuilding")]
		GA_BUILDING = 6,
		/// <summary>
		///  解散时间 UInt32
		/// </summary>
		[AdvancedInspector.Descriptor(" 解散时间 UInt32", " 解散时间 UInt32")]
		GA_DISMISS_TIME = 7,
		/// <summary>
		///  成员数量 UInt16
		/// </summary>
		[AdvancedInspector.Descriptor(" 成员数量 UInt16", " 成员数量 UInt16")]
		GA_MEMBER_NUM = 8,
		/// <summary>
		///  会长名字 string
		/// </summary>
		[AdvancedInspector.Descriptor(" 会长名字 string", " 会长名字 string")]
		GA_LEADER_NAME = 9,
		/// <summary>
		///  报名领地ID UInt8
		/// </summary>
		[AdvancedInspector.Descriptor(" 报名领地ID UInt8", " 报名领地ID UInt8")]
		GA_ENROLL_TERRID = 10,
		/// <summary>
		///  公会战分数 UInt32
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战分数 UInt32", " 公会战分数 UInt32")]
		GA_BATTLE_SCORE = 11,
		/// <summary>
		///  公会占领领地 UInt8
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会占领领地 UInt8", " 公会占领领地 UInt8")]
		GA_OCCUPY_TERRID = 12,
		/// <summary>
		///  公会战鼓舞次数 UInt8
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战鼓舞次数 UInt8", " 公会战鼓舞次数 UInt8")]
		GA_INSPIRE = 13,
		/// <summary>
		///  公会战胜利抽奖几率 UInt8
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战胜利抽奖几率 UInt8", " 公会战胜利抽奖几率 UInt8")]
		GA_WIN_PROBABILITY = 14,
		/// <summary>
		///  公会战失败抽奖几率 UInt8
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战失败抽奖几率 UInt8", " 公会战失败抽奖几率 UInt8")]
		GA_LOSE_PROBABILITY = 15,
		/// <summary>
		///  公会战仓库放入物品 UInt8
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战仓库放入物品 UInt8", " 公会战仓库放入物品 UInt8")]
		GA_STORAGE_ADD_POST = 16,
		/// <summary>
		///  公会战仓库删除物品 UInt8
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战仓库删除物品 UInt8", " 公会战仓库删除物品 UInt8")]
		GA_STORAGE_DEL_POST = 17,
		/// <summary>
		///  公会占领跨服领地 UInt8
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会占领跨服领地 UInt8", " 公会占领跨服领地 UInt8")]
		GA_OCCUPY_CROSS_TERRID = 18,
		/// <summary>
		///  工会历史占领领地 UInt8
		/// </summary>
		[AdvancedInspector.Descriptor(" 工会历史占领领地 UInt8", " 工会历史占领领地 UInt8")]
		GA_HISTORY_TERRID = 19,
		/// <summary>
		///  加入公会等级 UInt32
		/// </summary>
		[AdvancedInspector.Descriptor(" 加入公会等级 UInt32", " 加入公会等级 UInt32")]
		GA_JOIN_LEVEL = 20,
		/// <summary>
		///  公会副本难度 UInt32
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会副本难度 UInt32", " 公会副本难度 UInt32")]
		GA_DUNGEON_TYPE = 21,
		/// <summary>
		///  上周增加的繁荣度 UInt32
		/// </summary>
		[AdvancedInspector.Descriptor(" 上周增加的繁荣度 UInt32", " 上周增加的繁荣度 UInt32")]
		GA_LAST_WEEK_ADD_FUND = 22,
		/// <summary>
		///  本周增加的繁荣度 UInt32
		/// </summary>
		[AdvancedInspector.Descriptor(" 本周增加的繁荣度 UInt32", " 本周增加的繁荣度 UInt32")]
		GA_THIS_WEEK_ADD_FUND = 23,
	}

	/// <summary>
	/// 公会战类型
	/// </summary>
	[AdvancedInspector.Descriptor("公会战类型", "公会战类型")]
	public enum GuildBattleType
	{
		/// <summary>
		///  无效
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效", " 无效")]
		GBT_INVALID = 0,
		/// <summary>
		///  普通
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通", " 普通")]
		GBT_NORMAL = 1,
		/// <summary>
		///  宣战
		/// </summary>
		[AdvancedInspector.Descriptor(" 宣战", " 宣战")]
		GBT_CHALLENGE = 2,
		/// <summary>
		/// 跨服
		/// </summary>
		[AdvancedInspector.Descriptor("跨服", "跨服")]
		GBT_CROSS = 3,

		GBT_MAX = 4,
	}

	/// <summary>
	///  工会战领地类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 工会战领地类型", " 工会战领地类型")]
	public enum GuildTerritoryType
	{
		/// <summary>
		///  无效
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效", " 无效")]
		GTT_INVALID = 0,
		/// <summary>
		/// 普通
		/// </summary>
		[AdvancedInspector.Descriptor("普通", "普通")]
		GTT_NORMAL = 1,
		/// <summary>
		/// 跨服
		/// </summary>
		[AdvancedInspector.Descriptor("跨服", "跨服")]
		GTT_CROSS = 2,

		GTT_MAX = 3,
	}

	/// <summary>
	///  公会战状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会战状态", " 公会战状态")]
	public enum GuildBattleStatus
	{
		/// <summary>
		///  无
		/// </summary>
		[AdvancedInspector.Descriptor(" 无", " 无")]
		GBS_INVALID = 0,
		/// <summary>
		///  报名
		/// </summary>
		[AdvancedInspector.Descriptor(" 报名", " 报名")]
		GBS_ENROLL = 1,
		/// <summary>
		///  准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 准备", " 准备")]
		GBS_PREPARE = 2,
		/// <summary>
		///  战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗", " 战斗")]
		GBS_BATTLE = 3,
		/// <summary>
		///  领奖
		/// </summary>
		[AdvancedInspector.Descriptor(" 领奖", " 领奖")]
		GBS_REWARD = 4,

		GBS_MAX = 5,
	}

	/// <summary>
	///  公会建筑类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会建筑类型", " 公会建筑类型")]
	public enum GuildBuildingType
	{
		/// <summary>
		///  主城
		/// </summary>
		[AdvancedInspector.Descriptor(" 主城", " 主城")]
		MAIN = 0,
		/// <summary>
		///  商店
		/// </summary>
		[AdvancedInspector.Descriptor(" 商店", " 商店")]
		SHOP = 1,
		/// <summary>
		///  圆桌会议
		/// </summary>
		[AdvancedInspector.Descriptor(" 圆桌会议", " 圆桌会议")]
		TABLE = 2,
		/// <summary>
		///  地下城
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城", " 地下城")]
		DUNGEON = 3,
		/// <summary>
		///  雕像
		/// </summary>
		[AdvancedInspector.Descriptor(" 雕像", " 雕像")]
		STATUE = 4,
		/// <summary>
		///  战争坊
		/// </summary>
		[AdvancedInspector.Descriptor(" 战争坊", " 战争坊")]
		BATTLE = 5,
		/// <summary>
		///  福利社
		/// </summary>
		[AdvancedInspector.Descriptor(" 福利社", " 福利社")]
		WELFARE = 6,
		/// <summary>
		///  荣耀殿堂
		/// </summary>
		[AdvancedInspector.Descriptor(" 荣耀殿堂", " 荣耀殿堂")]
		HONOUR = 7,
		/// <summary>
		///  征战祭祀
		/// </summary>
		[AdvancedInspector.Descriptor(" 征战祭祀", " 征战祭祀")]
		FETE = 8,
	}

	/// <summary>
	///  公会操作类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会操作类型", " 公会操作类型")]
	public enum GuildOperation
	{
		/// <summary>
		///  修改公会宣言
		/// </summary>
		[AdvancedInspector.Descriptor(" 修改公会宣言", " 修改公会宣言")]
		MODIFY_DECLAR = 0,
		/// <summary>
		///  修改公会名
		/// </summary>
		[AdvancedInspector.Descriptor(" 修改公会名", " 修改公会名")]
		MODIFY_NAME = 1,
		/// <summary>
		///  修改公会公告
		/// </summary>
		[AdvancedInspector.Descriptor(" 修改公会公告", " 修改公会公告")]
		MODIFY_ANNOUNCE = 2,
		/// <summary>
		///  发送公会邮件
		/// </summary>
		[AdvancedInspector.Descriptor(" 发送公会邮件", " 发送公会邮件")]
		SEND_MAIL = 3,
		/// <summary>
		///  升级建筑
		/// </summary>
		[AdvancedInspector.Descriptor(" 升级建筑", " 升级建筑")]
		UPGRADE_BUILDING = 4,
		/// <summary>
		///  捐献
		/// </summary>
		[AdvancedInspector.Descriptor(" 捐献", " 捐献")]
		DONATE = 5,
		/// <summary>
		///  兑换
		/// </summary>
		[AdvancedInspector.Descriptor(" 兑换", " 兑换")]
		EXCHANGE = 6,
		/// <summary>
		///  升级技能
		/// </summary>
		[AdvancedInspector.Descriptor(" 升级技能", " 升级技能")]
		UPGRADE_SKILL = 7,
		/// <summary>
		///  解散工会
		/// </summary>
		[AdvancedInspector.Descriptor(" 解散工会", " 解散工会")]
		DISMISS = 8,
		/// <summary>
		///  取消解散工会
		/// </summary>
		[AdvancedInspector.Descriptor(" 取消解散工会", " 取消解散工会")]
		CANCEL_DISMISS = 9,
		/// <summary>
		///  膜拜
		/// </summary>
		[AdvancedInspector.Descriptor(" 膜拜", " 膜拜")]
		ORZ = 10,
		/// <summary>
		///  圆桌会议
		/// </summary>
		[AdvancedInspector.Descriptor(" 圆桌会议", " 圆桌会议")]
		TABLE = 11,
		/// <summary>
		///  自费红包
		/// </summary>
		[AdvancedInspector.Descriptor(" 自费红包", " 自费红包")]
		PAY_REDPACKET = 12,
	}

	/// <summary>
	///  捐献
	/// </summary>
	[AdvancedInspector.Descriptor(" 捐献", " 捐献")]
	public enum GuildDonateType
	{
		/// <summary>
		///  金币捐献
		/// </summary>
		[AdvancedInspector.Descriptor(" 金币捐献", " 金币捐献")]
		GOLD = 0,
		/// <summary>
		///  点劵捐献
		/// </summary>
		[AdvancedInspector.Descriptor(" 点劵捐献", " 点劵捐献")]
		POINT = 1,
	}

	/// <summary>
	///  膜拜类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 膜拜类型", " 膜拜类型")]
	public enum GuildOrzType
	{
		/// <summary>
		///  普通膜拜
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通膜拜", " 普通膜拜")]
		GUILD_ORZ_LOW = 0,
		/// <summary>
		///  中级膜拜
		/// </summary>
		[AdvancedInspector.Descriptor(" 中级膜拜", " 中级膜拜")]
		GUILD_ORZ_MID = 1,
		/// <summary>
		///  高级膜拜
		/// </summary>
		[AdvancedInspector.Descriptor(" 高级膜拜", " 高级膜拜")]
		GUILD_ORZ_HIGH = 2,
	}

	/// <summary>
	///  雕像类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 雕像类型", " 雕像类型")]
	public enum FigureStatueType
	{

		FST_INVALID = 0,

		FST_GUILD = 1,

		FST_GUILD_ASSISTANT = 2,

		FST_GUILD_ASSISTANT_TWO = 3,
		/// <summary>
		///  公会地下城雕像
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城雕像", " 公会地下城雕像")]
		FST_GUILD_DUNGEON_FIRST = 4,

		FST_GUILD_DUNGEON_SECOND = 5,

		FST_GUILD_DUNGEON_THIRD = 6,
	}

	/// <summary>
	///  公会仓库设置类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会仓库设置类型", " 公会仓库设置类型")]
	public enum GuildStorageSetting
	{

		GUILD_POST_INVALID = 0,
		/// <summary>
		///  胜利抽奖几率
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜利抽奖几率", " 胜利抽奖几率")]
		GSS_WIN_PROBABILITY = 1,
		/// <summary>
		///  失败抽奖几率
		/// </summary>
		[AdvancedInspector.Descriptor(" 失败抽奖几率", " 失败抽奖几率")]
		GSS_LOSE_PROBABILITY = 2,
		/// <summary>
		///  仓库增加权限
		/// </summary>
		[AdvancedInspector.Descriptor(" 仓库增加权限", " 仓库增加权限")]
		GSS_STORAGE_ADD_POST = 3,
		/// <summary>
		///  仓库删除权限
		/// </summary>
		[AdvancedInspector.Descriptor(" 仓库删除权限", " 仓库删除权限")]
		GSS_STORAGE_DEL_POST = 4,

		GSS_MAX = 5,
	}

	/// <summary>
	///  公会成员抽奖状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会成员抽奖状态", " 公会成员抽奖状态")]
	public enum GuildBattleLotteryStatus
	{
		/// <summary>
		///  无效
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效", " 无效")]
		GBLS_INVALID = 0,
		/// <summary>
		///  不能抽奖
		/// </summary>
		[AdvancedInspector.Descriptor(" 不能抽奖", " 不能抽奖")]
		GBLS_NOT = 1,
		/// <summary>
		///  可以抽奖
		/// </summary>
		[AdvancedInspector.Descriptor(" 可以抽奖", " 可以抽奖")]
		GBLS_CAN = 2,
		/// <summary>
		///  已经抽奖
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经抽奖", " 已经抽奖")]
		GBLS_FIN = 3,

		GBLS_MAX = 4,
	}


	public enum GuildStorageOpType
	{

		GSOT_NONE = 0,
		/// <summary>
		///  获得
		/// </summary>
		[AdvancedInspector.Descriptor(" 获得", " 获得")]
		GSOT_GET = 1,
		/// <summary>
		///  存入
		/// </summary>
		[AdvancedInspector.Descriptor(" 存入", " 存入")]
		GSOT_PUT = 2,
		/// <summary>
		///  购买并存入
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买并存入", " 购买并存入")]
		GSOT_BUYPUT = 3,
	}

	/// <summary>
	/// *****************公会地下城*************************************
	/// </summary>
	/// <summary>
	///  公会地下城状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会地下城状态", " 公会地下城状态")]
	public enum GuildDungeonStatus
	{
		/// <summary>
		///  结束关闭
		/// </summary>
		[AdvancedInspector.Descriptor(" 结束关闭", " 结束关闭")]
		GUILD_DUNGEON_END = 0,
		/// <summary>
		///  准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 准备", " 准备")]
		GUILD_DUNGEON_PREPARE = 1,
		/// <summary>
		///  开始
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始", " 开始")]
		GUILD_DUNGEON_START = 2,
		/// <summary>
		///  发奖
		/// </summary>
		[AdvancedInspector.Descriptor(" 发奖", " 发奖")]
		GUILD_DUNGEON_REWARD = 3,
	}

	/// <summary>
	///  公会地下城难度等级
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会地下城难度等级", " 公会地下城难度等级")]
	public enum GuildDungeonLvl
	{
		/// <summary>
		///  初级
		/// </summary>
		[AdvancedInspector.Descriptor(" 初级", " 初级")]
		GUILD_DUNGEON_LOW = 1,
		/// <summary>
		///  中级
		/// </summary>
		[AdvancedInspector.Descriptor(" 中级", " 中级")]
		GUILD_DUNGEON_MID = 2,
		/// <summary>
		///  高级
		/// </summary>
		[AdvancedInspector.Descriptor(" 高级", " 高级")]
		GUILD_DUNGEON_HIGUH = 3,
	}


	public enum GuildAuctionType
	{
		/// <summary>
		///  无效值
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效值", " 无效值")]
		G_AUCTION_INVALID = 0,
		/// <summary>
		///  公会拍
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会拍", " 公会拍")]
		G_AUCTION_GUILD = 1,
		/// <summary>
		///  世界拍
		/// </summary>
		[AdvancedInspector.Descriptor(" 世界拍", " 世界拍")]
		G_AUCTION_WORLD = 2,
	}


	public enum GuildAuctionItemState
	{
		/// <summary>
		///  无效值
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效值", " 无效值")]
		GAI_STATE_INVALID = 0,
		/// <summary>
		///  拍卖准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖准备", " 拍卖准备")]
		GAI_STATE_PREPARE = 1,
		/// <summary>
		///  拍卖中
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖中", " 拍卖中")]
		GAI_STATE_NORMAL = 2,
		/// <summary>
		///  成交
		/// </summary>
		[AdvancedInspector.Descriptor(" 成交", " 成交")]
		GAI_STATE_DEAL = 3,
		/// <summary>
		///  流拍
		/// </summary>
		[AdvancedInspector.Descriptor(" 流拍", " 流拍")]
		GAI_STATE_ABORATION = 4,
	}

}
