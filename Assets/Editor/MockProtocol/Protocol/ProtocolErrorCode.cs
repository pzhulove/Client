using System;
using System.Text;

namespace Mock.Protocol
{

	public enum ProtoErrorCode
	{
		/// <summary>
		///  成功
		/// </summary>
		[AdvancedInspector.Descriptor(" 成功", " 成功")]
		SUCCESS = 0,
		/// <summary>
		///  功能未开启
		/// </summary>
		[AdvancedInspector.Descriptor(" 功能未开启", " 功能未开启")]
		SYS_NOT_OPEN = 5,
		/// <summary>
		///  登录验证
		/// </summary>
		[AdvancedInspector.Descriptor(" 登录验证", " 登录验证")]
		LOGIN = 100000,
		/// <summary>
		///  服务器未就绪
		/// </summary>
		[AdvancedInspector.Descriptor(" 服务器未就绪", " 服务器未就绪")]
		LOGIN_SERVER_UNREADY = 100001,
		/// <summary>
		///  未知账号，账号名错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 未知账号，账号名错误", " 未知账号，账号名错误")]
		LOGIN_UNKNOW_ACCOUNT = 100002,
		/// <summary>
		///  重复登录
		/// </summary>
		[AdvancedInspector.Descriptor(" 重复登录", " 重复登录")]
		LOGIN_REPEAT = 100003,
		/// <summary>
		///  密码错
		/// </summary>
		[AdvancedInspector.Descriptor(" 密码错", " 密码错")]
		LOGIN_WRONG_PASSWD = 100004,
		/// <summary>
		///  验证超时
		/// </summary>
		[AdvancedInspector.Descriptor(" 验证超时", " 验证超时")]
		LOGIN_VERIFY_TIMEOUT = 100005,
		/// <summary>
		///  服务器繁忙
		/// </summary>
		[AdvancedInspector.Descriptor(" 服务器繁忙", " 服务器繁忙")]
		LOGIN_SERVER_BUSY = 100006,
		/// <summary>
		///  版本号错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 版本号错误", " 版本号错误")]
		LOGIN_ERROR_VERSION = 100007,
		/// <summary>
		///  封号
		/// </summary>
		[AdvancedInspector.Descriptor(" 封号", " 封号")]
		LOGIN_FORBID_LOGIN = 100008,
		/// <summary>
		///  数据库错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 数据库错误", " 数据库错误")]
		LOGIN_DB_ERROR = 100009,
		/// <summary>
		///  排队中
		/// </summary>
		[AdvancedInspector.Descriptor(" 排队中", " 排队中")]
		LOGIN_WAIT = 100010,
		/// <summary>
		///  服务器禁止登录
		/// </summary>
		[AdvancedInspector.Descriptor(" 服务器禁止登录", " 服务器禁止登录")]
		LOGIN_BUSY = 100011,
		/// <summary>
		///  进入游戏
		/// </summary>
		[AdvancedInspector.Descriptor(" 进入游戏", " 进入游戏")]
		LOGIN_VERIFY_ERROR = 1002,
		/// <summary>
		///  进入游戏
		/// </summary>
		[AdvancedInspector.Descriptor(" 进入游戏", " 进入游戏")]
		ENTERGAME = 200000,
		/// <summary>
		///  角色信息不合法
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色信息不合法", " 角色信息不合法")]
		ENTERGAME_UNVALID_ROLEINFO = 200001,
		/// <summary>
		///  服务器繁忙
		/// </summary>
		[AdvancedInspector.Descriptor(" 服务器繁忙", " 服务器繁忙")]
		ENTERGAME_SERVER_BUSY = 200002,
		/// <summary>
		///  太多角色
		/// </summary>
		[AdvancedInspector.Descriptor(" 太多角色", " 太多角色")]
		ENTERGAME_TOOMANY_ROLES = 200003,
		/// <summary>
		///  重复名
		/// </summary>
		[AdvancedInspector.Descriptor(" 重复名", " 重复名")]
		ENTERGAME_DUPLICATE_NAME = 200004,
		/// <summary>
		///  删除角色或进入游戏时提示
		/// </summary>
		[AdvancedInspector.Descriptor(" 删除角色或进入游戏时提示", " 删除角色或进入游戏时提示")]
		ENTERGAME_NOROLE = 200005,
		/// <summary>
		///  场景未就绪
		/// </summary>
		[AdvancedInspector.Descriptor(" 场景未就绪", " 场景未就绪")]
		ENTERGAME_SCENE_UNREADY = 200006,
		/// <summary>
		///  角色初始化失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色初始化失败", " 角色初始化失败")]
		ENTERGAME_INIT_FAILED = 200007,
		/// <summary>
		///  重复
		/// </summary>
		[AdvancedInspector.Descriptor(" 重复", " 重复")]
		ENTERGAME_REPEAT = 200008,
		/// <summary>
		///  角色名不合法
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色名不合法", " 角色名不合法")]
		ENTERGAME_UNVALID_NAME = 200009,
		/// <summary>
		///  不允许建号
		/// </summary>
		[AdvancedInspector.Descriptor(" 不允许建号", " 不允许建号")]
		ENTERGAME_NO_CREATEROLE = 200010,
		/// <summary>
		///  需要有角色达到20级
		/// </summary>
		[AdvancedInspector.Descriptor(" 需要有角色达到20级", " 需要有角色达到20级")]
		ENTERGAME_NEED_LEVEL_20 = 200011,
		/// <summary>
		///  需要有角色达到40级
		/// </summary>
		[AdvancedInspector.Descriptor(" 需要有角色达到40级", " 需要有角色达到40级")]
		ENTERGAME_NEED_LEVEL_40 = 200012,
		/// <summary>
		///  超过今日创建角色最大数
		/// </summary>
		[AdvancedInspector.Descriptor(" 超过今日创建角色最大数", " 超过今日创建角色最大数")]
		ENTERGAME_TODAY_TOOMANY_ROLE = 200013,
		/// <summary>
		///  请求恢复的角色不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求恢复的角色不存在", " 请求恢复的角色不存在")]
		ENTERGAME_RECOVER_ROLE_UNEXIST = 200014,
		/// <summary>
		///  请求恢复的角色已经删除了（超过保存时间）
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求恢复的角色已经删除了（超过保存时间）", " 请求恢复的角色已经删除了（超过保存时间）")]
		ENTERGAME_RECOVER_ROLE_DELETED = 200015,
		/// <summary>
		///  请求恢复的角色并没有被删除
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求恢复的角色并没有被删除", " 请求恢复的角色并没有被删除")]
		ENTERGAME_RECOVER_ROLE_NOT_DELETE = 200016,
		/// <summary>
		///  请求删除的角色已经被删除了
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求删除的角色已经被删除了", " 请求删除的角色已经被删除了")]
		ENTERGAME_DELETE_ROLE_DELETED = 200017,
		/// <summary>
		///  请求删除的角色不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求删除的角色不存在", " 请求删除的角色不存在")]
		ENTERGAME_DELETE_ROLE_UNEXIST = 200018,
		/// <summary>
		///  当前正在删除的角色达到上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 当前正在删除的角色达到上限", " 当前正在删除的角色达到上限")]
		ENTERGAME_DELETE_ROLE_MAX_NUM = 200019,
		/// <summary>
		/// 请求删除的角色受限（时间限制）
		/// </summary>
		[AdvancedInspector.Descriptor("请求删除的角色受限（时间限制）", "请求删除的角色受限（时间限制）")]
		ENTERGAME_DELETE_ROLE_LIMIT = 200020,
		/// <summary>
		/// 请求恢复的角色受限（时间限制）
		/// </summary>
		[AdvancedInspector.Descriptor("请求恢复的角色受限（时间限制）", "请求恢复的角色受限（时间限制）")]
		ENTERGAME_RECOVER_ROLE_LIMIT = 200021,
		/// <summary>
		/// 角色栏位达到上限
		/// </summary>
		[AdvancedInspector.Descriptor("角色栏位达到上限", "角色栏位达到上限")]
		ENTERGAME_ROLE_FIELD_REACN_UPPER_LIMIT = 200022,
		/// <summary>
		/// 角色栏位未全部使用
		/// </summary>
		[AdvancedInspector.Descriptor("角色栏位未全部使用", "角色栏位未全部使用")]
		ENTERGAME_ROLE_FIELD_NOT_ALL_USED = 200023,
		/// <summary>
		/// 可扩展角色栏位解锁失败
		/// </summary>
		[AdvancedInspector.Descriptor("可扩展角色栏位解锁失败", "可扩展角色栏位解锁失败")]
		ENTERGAME_EXTENSIBLE_ROLE_FIELD_UNLOCK_FAILED = 200024,
		/// <summary>
		///  场景相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 场景相关", " 场景相关")]
		SCENE = 300000,
		/// <summary>
		///  重复的场景
		/// </summary>
		[AdvancedInspector.Descriptor(" 重复的场景", " 重复的场景")]
		SCENE_DUPLICATE = 300001,
		/// <summary>
		///  建动态场景时下线
		/// </summary>
		[AdvancedInspector.Descriptor(" 建动态场景时下线", " 建动态场景时下线")]
		SCENE_NOOWNER = 300002,
		/// <summary>
		///  档案错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 档案错误码", " 档案错误码")]
		RECORD = 400000,
		/// <summary>
		///  数据库错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 数据库错误", " 数据库错误")]
		RECORD_ERROR = 400001,
		/// <summary>
		///  重复名
		/// </summary>
		[AdvancedInspector.Descriptor(" 重复名", " 重复名")]
		RECORD_DUPLICATE_NAME = 400002,
		/// <summary>
		///  没有名字列
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有名字列", " 没有名字列")]
		RECORD_NO_NAMECOLUMN = 400003,

		RECORD_TIMEOUT = 400004,
		/// <summary>
		///  relayserver错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" relayserver错误码", " relayserver错误码")]
		RELAY = 500000,
		/// <summary>
		///  系统错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 系统错误", " 系统错误")]
		RELAY_LOGIN_SYSTEMERROR = 500001,
		/// <summary>
		///  无效的gamesession
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的gamesession", " 无效的gamesession")]
		RELAY_LOGIN_INVALIDSESSION = 500002,
		/// <summary>
		///  无效的参战者
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的参战者", " 无效的参战者")]
		RELAY_LOGIN_INVALIDFIGHTER = 500003,
		/// <summary>
		///  系统错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 系统错误", " 系统错误")]
		RELAY_RECONNECT_SYSTEMERROR = 500004,
		/// <summary>
		///  玩家还在线上
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家还在线上", " 玩家还在线上")]
		RELAY_RECONNECT_PLAYER_ONLINE = 500005,
		/// <summary>
		///  无效的gamesession
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的gamesession", " 无效的gamesession")]
		RELAY_RECONNECT_INVALIDSESSION = 500006,
		/// <summary>
		///  无效的参战者
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的参战者", " 无效的参战者")]
		RELAY_RECONNECT_INVALIDFIGHTER = 500007,
		/// <summary>
		///  匹配相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 匹配相关", " 匹配相关")]
		MATCH = 600000,
		/// <summary>
		///  系统错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 系统错误", " 系统错误")]
		MATCH_START_SYSTEMERROR = 600001,
		/// <summary>
		///  已经在匹配中了
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经在匹配中了", " 已经在匹配中了")]
		MATCH_START_REPEAT = 600002,
		/// <summary>
		///  匹配失败，超时
		/// </summary>
		[AdvancedInspector.Descriptor(" 匹配失败，超时", " 匹配失败，超时")]
		MATCH_TIMEOUT = 600003,
		/// <summary>
		///  不在PK准备区中
		/// </summary>
		[AdvancedInspector.Descriptor(" 不在PK准备区中", " 不在PK准备区中")]
		MATCH_START_NOT_IN_PK_PARPARE = 600004,
		/// <summary>
		///  组队状态不能匹配
		/// </summary>
		[AdvancedInspector.Descriptor(" 组队状态不能匹配", " 组队状态不能匹配")]
		MATCH_START_IN_TEAM = 600005,
		/// <summary>
		///  未加入武道大会
		/// </summary>
		[AdvancedInspector.Descriptor(" 未加入武道大会", " 未加入武道大会")]
		MATCH_START_WUDAO_NOT_JOIN = 600006,
		/// <summary>
		///  武道大会已经完成
		/// </summary>
		[AdvancedInspector.Descriptor(" 武道大会已经完成", " 武道大会已经完成")]
		MATCH_START_WUDAO_COMPLETE = 600007,
		/// <summary>
		///  不在匹配列表中
		/// </summary>
		[AdvancedInspector.Descriptor(" 不在匹配列表中", " 不在匹配列表中")]
		MATCH_CANCLE_NOT_MATCHING = 600008,
		/// <summary>
		///  玩家已经在游戏中了
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家已经在游戏中了", " 玩家已经在游戏中了")]
		MATCH_CANCLE_RACING = 600009,
		/// <summary>
		/// 尚未设置公平竞技场技能
		/// </summary>
		[AdvancedInspector.Descriptor("尚未设置公平竞技场技能", "尚未设置公平竞技场技能")]
		MATCH_NOT_SET_EQUAL_SKILL = 600010,
		/// <summary>
		/// 尚未开始
		/// </summary>
		[AdvancedInspector.Descriptor("尚未开始", "尚未开始")]
		MATCH_NOT_BEGIN = 600011,
		/// <summary>
		///  技能相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 技能相关", " 技能相关")]
		SKILL = 700000,
		/// <summary>
		///  ERROR
		/// </summary>
		[AdvancedInspector.Descriptor(" ERROR", " ERROR")]
		SKILL_ERROR = 700001,
		/// <summary>
		///  保存到数据库失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 保存到数据库失败", " 保存到数据库失败")]
		SKILL_SAVE_DB_ERROR = 700002,
		/// <summary>
		///  没有这个技能
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有这个技能", " 没有这个技能")]
		SKILL_NOT_FOUNT = 700003,
		/// <summary>
		///  错误的技能类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误的技能类型", " 错误的技能类型")]
		SKILL_TYPE_ERROR = 700004,
		/// <summary>
		///  SP不够
		/// </summary>
		[AdvancedInspector.Descriptor(" SP不够", " SP不够")]
		SKILL_SP_NOT_ENOUGH = 700005,
		/// <summary>
		///  移除SP失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 移除SP失败", " 移除SP失败")]
		SKILL_SP_REMOVE_ERROR = 700006,
		/// <summary>
		///  超过最大等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 超过最大等级", " 超过最大等级")]
		SKILL_MAX_SKILL_LEVEL = 700007,
		/// <summary>
		///  职业不合法
		/// </summary>
		[AdvancedInspector.Descriptor(" 职业不合法", " 职业不合法")]
		SKILL_OCCU_ERROR = 700008,
		/// <summary>
		///  检查玩家等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 检查玩家等级", " 检查玩家等级")]
		SKILL_PLAYER_LEVEL = 700009,
		/// <summary>
		///  前置技能错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 前置技能错误", " 前置技能错误")]
		SKILL_NEED_SKILL_ERROR = 700010,
		/// <summary>
		///  没有需要的物品或BUFF
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有需要的物品或BUFF", " 没有需要的物品或BUFF")]
		SKILL_NEED_ITEM_ERROR = 700011,
		/// <summary>
		///  后置技能错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 后置技能错误", " 后置技能错误")]
		SKILL_NEXT_SKILL_ERROR = 700012,
		/// <summary>
		///  超过最小等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 超过最小等级", " 超过最小等级")]
		SKILL_MIN_SKILL_LEVEL = 700013,
		/// <summary>
		///  设置相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 设置相关", " 设置相关")]
		SETTING = 800000,
		/// <summary>
		///  ERROR
		/// </summary>
		[AdvancedInspector.Descriptor(" ERROR", " ERROR")]
		SETTING_ERROR = 800001,
		/// <summary>
		///  索引错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 索引错误", " 索引错误")]
		SETTING_INDEX_ERROR = 800002,
		/// <summary>
		///  槽位索引错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 槽位索引错误", " 槽位索引错误")]
		SETTING_SLOT_ERROR = 800003,
		/// <summary>
		///  技能重复
		/// </summary>
		[AdvancedInspector.Descriptor(" 技能重复", " 技能重复")]
		SETTING_SKILL_REPEAT = 800004,
		/// <summary>
		///  技能不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 技能不存在", " 技能不存在")]
		SETTING_SKILL_ERROR = 800005,
		/// <summary>
		///  地下城相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城相关", " 地下城相关")]
		DUNGEON = 900000,
		/// <summary>
		///  创建比赛失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 创建比赛失败", " 创建比赛失败")]
		DUNGEON_START_CREATE_RACE_FAILED = 900001,
		/// <summary>
		///  地下城不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城不存在", " 地下城不存在")]
		DUNGEON_START_DUNGEON_NOT_EXIST = 900002,
		/// <summary>
		///  未达到等级要求
		/// </summary>
		[AdvancedInspector.Descriptor(" 未达到等级要求", " 未达到等级要求")]
		DUNGEON_START_LEVEL_LIMIT = 900003,
		/// <summary>
		///  没有疲劳了
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有疲劳了", " 没有疲劳了")]
		DUNGEON_START_NO_FATIGUE = 900004,
		/// <summary>
		///  不满足进入条件（前置任务，前置关卡等）
		/// </summary>
		[AdvancedInspector.Descriptor(" 不满足进入条件（前置任务，前置关卡等）", " 不满足进入条件（前置任务，前置关卡等）")]
		DUNGEON_START_CONDITION = 900005,
		/// <summary>
		///  难度未开放
		/// </summary>
		[AdvancedInspector.Descriptor(" 难度未开放", " 难度未开放")]
		DUNGEON_START_HARD_NOT_OPEN = 900006,
		/// <summary>
		///  不在选择关卡的场景
		/// </summary>
		[AdvancedInspector.Descriptor(" 不在选择关卡的场景", " 不在选择关卡的场景")]
		DUNGEON_START_NOT_IN_ENTRY_SCENE = 900007,
		/// <summary>
		///  开始比赛失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始比赛失败", " 开始比赛失败")]
		DUNGEON_START_RACE_FAILED = 900008,
		/// <summary>
		///  门票不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 门票不足", " 门票不足")]
		DUNGEON_START_NO_TICKET = 900009,
		/// <summary>
		///  没有深渊模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有深渊模式", " 没有深渊模式")]
		DUNGEON_START_NO_HELL_MODE = 900010,
		/// <summary>
		///  没有足够的深渊票
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有足够的深渊票", " 没有足够的深渊票")]
		DUNGEON_START_NO_HELL_TICKET = 900011,
		/// <summary>
		///  队伍成员不在线
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍成员不在线", " 队伍成员不在线")]
		DUNGEON_START_TEAM_MEMBER_OFFLINE = 900012,
		/// <summary>
		///  背包空余位置不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 背包空余位置不足", " 背包空余位置不足")]
		DUNGEON_START_BAG_FULL = 900013,
		/// <summary>
		///  不在开放时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 不在开放时间", " 不在开放时间")]
		DUNGEON_START_NOT_OPEN_TIME = 900014,
		/// <summary>
		///  次数用完
		/// </summary>
		[AdvancedInspector.Descriptor(" 次数用完", " 次数用完")]
		DUNGEON_START_NO_TIMES = 900015,
		/// <summary>
		///  系统错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 系统错误", " 系统错误")]
		DUNGEON_ENTER_AREA_SYSTEM_ERROR = 900016,
		/// <summary>
		///  已经离开地下城了
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经离开地下城了", " 已经离开地下城了")]
		DUNGEON_ENTER_AREA_NOT_IN_DUNGEON = 900017,
		/// <summary>
		///  重复进入
		/// </summary>
		[AdvancedInspector.Descriptor(" 重复进入", " 重复进入")]
		DUNGEON_ENTER_AREA_REPEAT = 900018,
		/// <summary>
		///  进入不存在的区域
		/// </summary>
		[AdvancedInspector.Descriptor(" 进入不存在的区域", " 进入不存在的区域")]
		DUNGEON_ENTER_AREA_NOT_EXIST = 900019,
		/// <summary>
		///  复活的目标不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 复活的目标不存在", " 复活的目标不存在")]
		DUNGEON_REVIVE_PLAYER_NOT_EXIST = 900020,
		/// <summary>
		///  重复复活
		/// </summary>
		[AdvancedInspector.Descriptor(" 重复复活", " 重复复活")]
		DUNGEON_REVIVE_REPEAT = 900021,
		/// <summary>
		///  没有足够的复活币
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有足够的复活币", " 没有足够的复活币")]
		DUNGEON_REVIVE_NOT_ENOUGH_REVIVE_COIN = 900022,
		/// <summary>
		///  开始关卡匹配失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始关卡匹配失败", " 开始关卡匹配失败")]
		DUNGEON_MATCH_START_FAILED = 900023,
		/// <summary>
		///  重复开始地下城
		/// </summary>
		[AdvancedInspector.Descriptor(" 重复开始地下城", " 重复开始地下城")]
		DUNGEON_TEAM_START_VOTE_REPEAT = 900024,
		/// <summary>
		///  地下城不能组队开始
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城不能组队开始", " 地下城不能组队开始")]
		DUNGEON_TEAM_TARGET_MUST_SINGLE = 900025,
		/// <summary>
		///  超过地下城最大人数
		/// </summary>
		[AdvancedInspector.Descriptor(" 超过地下城最大人数", " 超过地下城最大人数")]
		DUNGEON_TEAM_TOO_MANY_MEMBER = 900026,
		/// <summary>
		///  人数不足，无法开始地下城
		/// </summary>
		[AdvancedInspector.Descriptor(" 人数不足，无法开始地下城", " 人数不足，无法开始地下城")]
		DUNGEON_TEAM_NOT_ENOUGH_MEMBER = 900027,
		/// <summary>
		///  等待其他人投票
		/// </summary>
		[AdvancedInspector.Descriptor(" 等待其他人投票", " 等待其他人投票")]
		DUNGEON_TEAM_WAIT_OTHER_VOTE = 900028,
		/// <summary>
		///  无法购买次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 无法购买次数", " 无法购买次数")]
		DUNGEON_TIMES_CANT_BUY = 900029,
		/// <summary>
		///  无法购买次数，剩余次数不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 无法购买次数，剩余次数不足", " 无法购买次数，剩余次数不足")]
		DUNGEON_TIMES_NO_REMAIN_TIMES = 900030,
		/// <summary>
		///  无法购买次数，钱不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 无法购买次数，钱不够", " 无法购买次数，钱不够")]
		DUNGEON_TIMES_NO_ENOUGH_MONEY = 900031,
		/// <summary>
		///  开始比赛失败，这种情况下不需要提示
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始比赛失败，这种情况下不需要提示", " 开始比赛失败，这种情况下不需要提示")]
		DUNGEON_TEAM_START_RACE_FAILED = 901001,
		/// <summary>
		///  道具相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具相关", " 道具相关")]
		ITEM = 1000000,
		/// <summary>
		///  数据非法,包括各种相关空指正判断
		/// </summary>
		[AdvancedInspector.Descriptor(" 数据非法,包括各种相关空指正判断", " 数据非法,包括各种相关空指正判断")]
		ITEM_DATA_INVALID = 1000001,
		/// <summary>
		///  没有操作理由
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有操作理由", " 没有操作理由")]
		ITEM_NO_REASON = 1000002,
		/// <summary>
		///  item数量非法
		/// </summary>
		[AdvancedInspector.Descriptor(" item数量非法", " item数量非法")]
		ITEM_NUM_INVALID = 1000003,
		/// <summary>
		///  添加item背包满
		/// </summary>
		[AdvancedInspector.Descriptor(" 添加item背包满", " 添加item背包满")]
		ITEM_ADD_PACK_FULL = 1000004,
		/// <summary>
		///  金钱添加达到上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 金钱添加达到上限", " 金钱添加达到上限")]
		ITEM_MONEY_ADD_FULL = 1000005,
		/// <summary>
		///  使用道具失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 使用道具失败", " 使用道具失败")]
		ITEM_USE_FAIL = 1000006,
		/// <summary>
		///  不能装备
		/// </summary>
		[AdvancedInspector.Descriptor(" 不能装备", " 不能装备")]
		ITEM_CAN_NOT_EQUIP = 1000007,
		/// <summary>
		///  装备加锁
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备加锁", " 装备加锁")]
		ITEM_LOCKED = 1000008,
		/// <summary>
		///  装备所处的包裹错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备所处的包裹错误", " 装备所处的包裹错误")]
		ITEM_PACK_INVALID = 1000009,
		/// <summary>
		///  背包格子达到上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 背包格子达到上限", " 背包格子达到上限")]
		ITEM_PACKSIZE_MAX = 1000010,
		/// <summary>
		///  道具类别错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具类别错误", " 道具类别错误")]
		ITEM_TYPE_INVALID = 1000011,
		/// <summary>
		///  道具分解失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具分解失败", " 道具分解失败")]
		ITEM_DECOMPOSE_FAIL = 1000012,
		/// <summary>
		///  装备强化等级错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备强化等级错误", " 装备强化等级错误")]
		ITEM_STRENTH_LV_INVALID = 1000013,
		/// <summary>
		///  金币不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 金币不够", " 金币不够")]
		ITEM_NOT_ENOUGH_GOLD = 1000014,
		/// <summary>
		///  材料不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 材料不够", " 材料不够")]
		ITEM_NOT_ENOUGH_MAT = 1000015,
		/// <summary>
		///  装备强化失败无惩罚
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备强化失败无惩罚", " 装备强化失败无惩罚")]
		ITEM_STRENTH_FAIL = 1000016,
		/// <summary>
		///  装备强化失败扣等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备强化失败扣等级", " 装备强化失败扣等级")]
		ITEM_STRENTH_FAIL_MINUS = 1000017,
		/// <summary>
		///  装备强化失败等级归零
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备强化失败等级归零", " 装备强化失败等级归零")]
		ITEM_STRENTH_FAIL_ZERO = 1000018,
		/// <summary>
		///  装备强化失败破碎
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备强化失败破碎", " 装备强化失败破碎")]
		ITEM_STRENTH_FAIL_BROKE = 1000019,
		/// <summary>
		///  存仓库空间满了
		/// </summary>
		[AdvancedInspector.Descriptor(" 存仓库空间满了", " 存仓库空间满了")]
		ITEM_PUSH_STORAGE_FULL = 1000020,
		/// <summary>
		///  存取仓库数量错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 存取仓库数量错误", " 存取仓库数量错误")]
		ITEM_STORAGE_NUM_ERR = 1000021,
		/// <summary>
		///  装备职业不符
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备职业不符", " 装备职业不符")]
		ITEM_EQUIP_OCCU = 1000022,
		/// <summary>
		///  装备等级不符
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备等级不符", " 装备等级不符")]
		ITEM_EQUIP_LV = 1000023,
		/// <summary>
		///  装备强化失败扣2级
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备强化失败扣2级", " 装备强化失败扣2级")]
		ITEM_STRENTH_FAIL_TWO = 1000024,
		/// <summary>
		///  装备强化失败扣2级
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备强化失败扣2级", " 装备强化失败扣2级")]
		ITEM_STRENTH_FAIL_THREE = 1000025,
		/// <summary>
		///  装备强化失败扣4级
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备强化失败扣4级", " 装备强化失败扣4级")]
		ITEM_STRENTH_FAIL_FOUR = 1000026,
		/// <summary>
		///  点券不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 点券不够", " 点券不够")]
		ITEM_NOT_ENOUGH_POINT = 1000027,
		/// <summary>
		///  强化类型错误(称号)addedbyhuchenhui2016.06.30
		/// </summary>
		[AdvancedInspector.Descriptor(" 强化类型错误(称号)addedbyhuchenhui2016.06.30", " 强化类型错误(称号)addedbyhuchenhui2016.06.30")]
		ITEM_STRENTH_FAIL_TITLE = 1000028,
		/// <summary>
		///  装备不能分解
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备不能分解", " 装备不能分解")]
		ITEM_CNA_NOT_DECOMPOSE = 1000029,
		/// <summary>
		///  道具使用CD中
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具使用CD中", " 道具使用CD中")]
		ITEM_USE_CD = 1000030,
		/// <summary>
		///  封装次数达到上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 封装次数达到上限", " 封装次数达到上限")]
		ITEM_SEAL_COUNT_MAX = 1000031,
		/// <summary>
		///  装备已经是封装了
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备已经是封装了", " 装备已经是封装了")]
		ITEM_ALREADY_SEAL = 1000032,
		/// <summary>
		///  装备封装品质不符
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备封装品质不符", " 装备封装品质不符")]
		ITEM_SEAL_QUALITY_ERR = 1000033,
		/// <summary>
		///  勇者之魂不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 勇者之魂不够", " 勇者之魂不够")]
		ITEM_NOT_ENOUGH_WARRIOR_SOUL = 1000034,
		/// <summary>
		///  决斗币不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 决斗币不够", " 决斗币不够")]
		ITEM_NOT_ENOUGH_PKCOIN = 1000035,
		/// <summary>
		///  附魔部位错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 附魔部位错误", " 附魔部位错误")]
		ITEM_ADDMAGIC_PART_ERR = 1000036,
		/// <summary>
		///  附魔卡合成品质不同
		/// </summary>
		[AdvancedInspector.Descriptor(" 附魔卡合成品质不同", " 附魔卡合成品质不同")]
		ITEM_MAGCARD_COMP_DIF_COLOR = 1000037,
		/// <summary>
		///  一键分解所需等级不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 一键分解所需等级不够", " 一键分解所需等级不够")]
		ITEM_ONEKEY_DECOMPOSE_LV_NOT_ENOUGH = 1000038,
		/// <summary>
		///  开罐子日金钱次数已满
		/// </summary>
		[AdvancedInspector.Descriptor(" 开罐子日金钱次数已满", " 开罐子日金钱次数已满")]
		ITEM_OPEN_JAR_DAYCOUNT = 1000039,
		/// <summary>
		///  不能出售
		/// </summary>
		[AdvancedInspector.Descriptor(" 不能出售", " 不能出售")]
		ITEM_NOT_SELL = 1000040,
		/// <summary>
		///  出售物品不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 出售物品不存在", " 出售物品不存在")]
		ITEM_SELL_ITEM_NOT_EXIST = 1000041,
		/// <summary>
		///  公会贡献不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会贡献不够", " 公会贡献不够")]
		ITEM_NOT_ENOUGH_GUILD_CONTRI = 1000042,
		/// <summary>
		///  使用道具buff已经存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 使用道具buff已经存在", " 使用道具buff已经存在")]
		ITEM_USE_BUFFALREADYEXIST = 1000043,
		/// <summary>
		///  货币不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 货币不足", " 货币不足")]
		ITEM_NOT_ENOUGH_MONEY = 1000044,
		/// <summary>
		///  强化券等级错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 强化券等级错误", " 强化券等级错误")]
		ITEM_STRTICKET_LV_ERR = 1000045,
		/// <summary>
		///  强化券扣除失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 强化券扣除失败", " 强化券扣除失败")]
		ITEM_STRTICKET_REDUCE_ERR = 1000046,
		/// <summary>
		///  道具日使用次数完了
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具日使用次数完了", " 道具日使用次数完了")]
		ITEM_DAYUSENUM = 1000047,
		/// <summary>
		///  强化券强化失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 强化券强化失败", " 强化券强化失败")]
		ITEM_SPECIAL_STRENTH_FAIL = 1000048,
		/// <summary>
		///  errcode49
		/// </summary>
		[AdvancedInspector.Descriptor(" errcode49", " errcode49")]
		ITEM_ERRCODE_49 = 1000049,
		/// <summary>
		///  errcode50
		/// </summary>
		[AdvancedInspector.Descriptor(" errcode50", " errcode50")]
		ITEM_ERRCODE_50 = 1000050,
		/// <summary>
		///  errcode51
		/// </summary>
		[AdvancedInspector.Descriptor(" errcode51", " errcode51")]
		ITEM_ERRCODE_51 = 1000051,
		/// <summary>
		///  errcode52
		/// </summary>
		[AdvancedInspector.Descriptor(" errcode52", " errcode52")]
		ITEM_ERRCODE_52 = 1000052,
		/// <summary>
		///  errcode53
		/// </summary>
		[AdvancedInspector.Descriptor(" errcode53", " errcode53")]
		ITEM_ERRCODE_53 = 1000053,
		/// <summary>
		///  地下城不能使用该道具
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城不能使用该道具", " 地下城不能使用该道具")]
		ITEM_CAN_NOT_USE_IN_DUNGEON = 1000054,
		/// <summary>
		///  城镇内不能使用该道具
		/// </summary>
		[AdvancedInspector.Descriptor(" 城镇内不能使用该道具", " 城镇内不能使用该道具")]
		ITEM_CAN_NOT_USE_IN_TOWN = 1000055,
		/// <summary>
		///  道具废弃
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具废弃", " 道具废弃")]
		ITEM_ABANDON = 1000056,
		/// <summary>
		/// 已售完
		/// </summary>
		[AdvancedInspector.Descriptor("已售完", "已售完")]
		ITEM_SOLD_OUT = 1000066,
		/// <summary>
		///  份额不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 份额不足", " 份额不足")]
		ITEM_COPIES_NOT_ENOUGH = 1000067,
		/// <summary>
		///  装备转移 装备品质不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备转移 装备品质不够", " 装备转移 装备品质不够")]
		ITEM_EQUIPTSF_COLOR = 1000068,
		/// <summary>
		///  超出装备回收次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 超出装备回收次数", " 超出装备回收次数")]
		ITEM_EQUIPRECO_BEYOND = 1000069,
		/// <summary>
		///  装备回收赎回积分不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备回收赎回积分不够", " 装备回收赎回积分不够")]
		ITEM_EQUIPRECO_REDEEM = 1000070,
		/// <summary>
		///  装备背包空间满了
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备背包空间满了", " 装备背包空间满了")]
		ITEM_EQUIPPACK_FULL = 1000071,
		/// <summary>
		///  装备回收积分提升失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备回收积分提升失败", " 装备回收积分提升失败")]
		ITEM_EQUIPRECO_UPSCFAIL = 1000072,
		/// <summary>
		/// 商品下架
		/// </summary>
		[AdvancedInspector.Descriptor("商品下架", "商品下架")]
		ITEM_OFF_SALE = 1000073,
		/// <summary>
		/// 装备回收积分提升货币不够
		/// </summary>
		[AdvancedInspector.Descriptor("装备回收积分提升货币不够", "装备回收积分提升货币不够")]
		ITEM_EQUIPRECO_UPSCMONY = 1000074,
		/// <summary>
		/// 装备回收积分提升积分已经最大
		/// </summary>
		[AdvancedInspector.Descriptor("装备回收积分提升积分已经最大", "装备回收积分提升积分已经最大")]
		ITEM_EQUIPRECO_UPSCMAX = 1000075,
		/// <summary>
		/// 提交装备数量不能为0
		/// </summary>
		[AdvancedInspector.Descriptor("提交装备数量不能为0", "提交装备数量不能为0")]
		ITEM_EQUIPRECO_EMPTY = 1000076,
		/// <summary>
		/// 装备回收积分罐子不能打开
		/// </summary>
		[AdvancedInspector.Descriptor("装备回收积分罐子不能打开", "装备回收积分罐子不能打开")]
		ITEM_EQUIPRECO_JARCANNOTOPEN = 1000077,
		/// <summary>
		/// 装备回收积分罐子已打开过
		/// </summary>
		[AdvancedInspector.Descriptor("装备回收积分罐子已打开过", "装备回收积分罐子已打开过")]
		ITEM_EQUIPRECO_JAROPENED = 1000078,
		/// <summary>
		/// 夺宝商品全部售完
		/// </summary>
		[AdvancedInspector.Descriptor("夺宝商品全部售完", "夺宝商品全部售完")]
		ITEM_GAMBLE_ALL_SOLD_OUT = 1000079,
		/// <summary>
		/// 消耗道具类魔罐礼包道具不足
		/// </summary>
		[AdvancedInspector.Descriptor("消耗道具类魔罐礼包道具不足", "消耗道具类魔罐礼包道具不足")]
		ITEM_CONS_JARGIFT_ITEM_INSUFFIC = 1000080,
		/// <summary>
		/// 良师值不足
		/// </summary>
		[AdvancedInspector.Descriptor("良师值不足", "良师值不足")]
		ITEM_NOT_ENOUGH_TEACHVALUE = 1000081,
		/// <summary>
		/// 角色绑定装备未镶嵌转移石不能放入仓库
		/// </summary>
		[AdvancedInspector.Descriptor("角色绑定装备未镶嵌转移石不能放入仓库", "角色绑定装备未镶嵌转移石不能放入仓库")]
		ITEM_ROLEBIND_EQ_CANT_PUTSTORE = 1000082,
		/// <summary>
		/// 未封装称号不能放入仓库
		/// </summary>
		[AdvancedInspector.Descriptor("未封装称号不能放入仓库", "未封装称号不能放入仓库")]
		ITEM_TITLE_NOTSEAL_CANT_PUTSTORE = 1000083,
		/// <summary>
		/// 保护券不存在
		/// </summary>
		[AdvancedInspector.Descriptor("保护券不存在", "保护券不存在")]
		ITEM_PROTECT_NOT_EXIST = 1000120,
		/// <summary>
		///  商店相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 商店相关", " 商店相关")]
		SHOP = 1100000,
		/// <summary>
		///  商店查询错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 商店查询错误", " 商店查询错误")]
		SHOP_QUERY_ERR = 1100001,
		/// <summary>
		///  商店刷新错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 商店刷新错误", " 商店刷新错误")]
		SHOP_REFRESH_ERR = 1100002,
		/// <summary>
		///  商店购买错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 商店购买错误", " 商店购买错误")]
		SHOP_BUY_ERR = 1100003,
		/// <summary>
		///  商店购买金币不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 商店购买金币不足", " 商店购买金币不足")]
		SHOP_BUY_NOT_ENOUGH_GOLD = 1100004,
		/// <summary>
		///  商店购买点券不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 商店购买点券不足", " 商店购买点券不足")]
		SHOP_BUY_NOT_ENOUGH_POINT = 1100005,
		/// <summary>
		///  商店购买道具不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 商店购买道具不足", " 商店购买道具不足")]
		SHOP_BUY_NOT_ENOUGH_ITEM = 1100006,
		/// <summary>
		///  商店购买背包空间不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 商店购买背包空间不足", " 商店购买背包空间不足")]
		SHOP_BUY_NOT_ENOUGH_PACKSIZE = 1100007,
		/// <summary>
		///  商店购买售罄
		/// </summary>
		[AdvancedInspector.Descriptor(" 商店购买售罄", " 商店购买售罄")]
		SHOP_BUY_SALE_OUT = 1100008,
		/// <summary>
		///  商店刷新点券不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 商店刷新点券不足", " 商店刷新点券不足")]
		SHOP_REFRESH_NOT_ENOUGH_MONEY = 1100009,
		/// <summary>
		///  死亡塔层数不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 死亡塔层数不够", " 死亡塔层数不够")]
		SHOP_NOT_ENOUGH_TOWER_LEVEL = 1100010,
		/// <summary>
		///  角斗场积分不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 角斗场积分不足", " 角斗场积分不足")]
		SHOP_NOT_ENOUGH_DUEL_POINT = 1100011,
		/// <summary>
		///  商店刷新次数不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 商店刷新次数不足", " 商店刷新次数不足")]
		SHOP_REFRESH_COUNT = 1100012,
		/// <summary>
		///  公会商店不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会商店不存在", " 公会商店不存在")]
		SHOP_GUIlD_SHOP_NOT_EXIST = 1100013,
		/// <summary>
		///  工会商店等级不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 工会商店等级不足", " 工会商店等级不足")]
		SHOP_GUIlD_NOT_ENOUGH_LV = 1100014,
		/// <summary>
		///  邮件相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 邮件相关", " 邮件相关")]
		MAIL = 1200000,
		/// <summary>
		///  邮件系统错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 邮件系统错误", " 邮件系统错误")]
		MAIL_ERROR = 1200001,
		/// <summary>
		///  标题错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 标题错误", " 标题错误")]
		MAIL_TITLE_ERROR = 1200002,
		/// <summary>
		///  标题长度错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 标题长度错误", " 标题长度错误")]
		MAIL_TITLE_LENGTH_ERROR = 1200003,
		/// <summary>
		///  内容错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 内容错误", " 内容错误")]
		MAIL_CONTENT_ERROR = 1200004,
		/// <summary>
		///  内容长度错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 内容长度错误", " 内容长度错误")]
		MAIL_CONTENT_LENGTH_ERROR = 1200005,
		/// <summary>
		///  发件人字符串错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 发件人字符串错误", " 发件人字符串错误")]
		MAIL_SENDER_NAME_ERROR = 1200006,
		/// <summary>
		///  发件人字符串长度错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 发件人字符串长度错误", " 发件人字符串长度错误")]
		MAIL_SENDER_NAME_LENGTH_ERROR = 1200007,
		/// <summary>
		///  奖励错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 奖励错误", " 奖励错误")]
		MAIL_REWARD_ERROR = 1200008,
		/// <summary>
		///  组队相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 组队相关", " 组队相关")]
		TEAM = 1300000,
		/// <summary>
		///  组队系统错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 组队系统错误", " 组队系统错误")]
		TEAM_ERROR = 1300001,
		/// <summary>
		///  创建队伍，已经有队伍了
		/// </summary>
		[AdvancedInspector.Descriptor(" 创建队伍，已经有队伍了", " 创建队伍，已经有队伍了")]
		TEAM_CREATE_TEAM_HAS_TEAM = 1300002,
		/// <summary>
		///  创建队伍，无效的队伍目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 创建队伍，无效的队伍目标", " 创建队伍，无效的队伍目标")]
		TEAM_CREATE_TEAM_INVALID_TARGET = 1300003,
		/// <summary>
		///  加入队伍，超时
		/// </summary>
		[AdvancedInspector.Descriptor(" 加入队伍，超时", " 加入队伍，超时")]
		TEAM_JOIN_TIMEOUT = 1300004,
		/// <summary>
		///  加入队伍，队伍不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 加入队伍，队伍不存在", " 加入队伍，队伍不存在")]
		TEAM_JOIN_TEAM_UNEXIST = 1300005,
		/// <summary>
		///  加入队伍，队伍满
		/// </summary>
		[AdvancedInspector.Descriptor(" 加入队伍，队伍满", " 加入队伍，队伍满")]
		TEAM_JOIN_TEAM_FULL = 1300006,
		/// <summary>
		///  加入队伍，队长离线
		/// </summary>
		[AdvancedInspector.Descriptor(" 加入队伍，队长离线", " 加入队伍，队长离线")]
		TEAM_JOIN_TEAM_MASTER_OFFLINE = 1300007,
		/// <summary>
		///  加入队伍，已经有队伍了
		/// </summary>
		[AdvancedInspector.Descriptor(" 加入队伍，已经有队伍了", " 加入队伍，已经有队伍了")]
		TEAM_JOIN_TEAM_HAS_TEAM = 1300008,
		/// <summary>
		///  加入队伍，密码错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 加入队伍，密码错误", " 加入队伍，密码错误")]
		TEAM_JOIN_TEAM_PASSWD_ERROR = 1300009,
		/// <summary>
		///  加入队伍，队伍有密码
		/// </summary>
		[AdvancedInspector.Descriptor(" 加入队伍，队伍有密码", " 加入队伍，队伍有密码")]
		TEAM_JOIN_TEAM_HAS_PASSWD = 1300010,
		/// <summary>
		///  加入队伍，等级不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 加入队伍，等级不足", " 加入队伍，等级不足")]
		TEAM_JOIN_LEVEL_LIMIT = 1300011,
		/// <summary>
		///  密码长度不对
		/// </summary>
		[AdvancedInspector.Descriptor(" 密码长度不对", " 密码长度不对")]
		TEAM_PASSWD_ERROR_LENGTH = 1300012,
		/// <summary>
		///  密码只能是数字
		/// </summary>
		[AdvancedInspector.Descriptor(" 密码只能是数字", " 密码只能是数字")]
		TEAM_PASSWD_ONLY_NUM = 1300013,
		/// <summary>
		///  名字太长
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字太长", " 名字太长")]
		TEAM_NAME_TOO_LONG = 1300014,
		/// <summary>
		///  无效的名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的名字", " 无效的名字")]
		TEAM_NAME_INVALID = 1300015,
		/// <summary>
		///  无效的目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的目标", " 无效的目标")]
		TEAM_TARGET_INVALID = 1300016,
		/// <summary>
		///  切换目标失败，人数太多
		/// </summary>
		[AdvancedInspector.Descriptor(" 切换目标失败，人数太多", " 切换目标失败，人数太多")]
		TEAM_TOO_MANY_PLAYER = 1300017,
		/// <summary>
		///  处理申请者，你不是队长
		/// </summary>
		[AdvancedInspector.Descriptor(" 处理申请者，你不是队长", " 处理申请者，你不是队长")]
		TEAM_REPLY_NOT_MASTER = 1300018,
		/// <summary>
		///  处理申请者，玩家不在线
		/// </summary>
		[AdvancedInspector.Descriptor(" 处理申请者，玩家不在线", " 处理申请者，玩家不在线")]
		TEAM_REPLY_PLAYER_OFFLINE = 1300019,
		/// <summary>
		///  处理申请者，该玩家未申请加入
		/// </summary>
		[AdvancedInspector.Descriptor(" 处理申请者，该玩家未申请加入", " 处理申请者，该玩家未申请加入")]
		TEAM_REPLY_PLAYER_INVALID = 1300020,
		/// <summary>
		///  处理申请者，队伍已满
		/// </summary>
		[AdvancedInspector.Descriptor(" 处理申请者，队伍已满", " 处理申请者，队伍已满")]
		TEAM_REPLY_TEAM_FULL = 1300021,
		/// <summary>
		///  邀请玩家，自己没有队伍
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请玩家，自己没有队伍", " 邀请玩家，自己没有队伍")]
		TEAM_INVITE_NO_TEAM = 1300022,
		/// <summary>
		///  邀请玩家，自己不是队长
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请玩家，自己不是队长", " 邀请玩家，自己不是队长")]
		TEAM_INVITE_NOT_TEAM_MASTER = 1300023,
		/// <summary>
		///  邀请玩家，队伍已满
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请玩家，队伍已满", " 邀请玩家，队伍已满")]
		TEAM_INVITE_TEAM_FULL = 1300024,
		/// <summary>
		///  邀请玩家，队伍正在战斗中
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请玩家，队伍正在战斗中", " 邀请玩家，队伍正在战斗中")]
		TEAM_INVITE_TEAM_IN_RACE = 1300025,
		/// <summary>
		///  邀请玩家，对方不在线
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请玩家，对方不在线", " 邀请玩家，对方不在线")]
		TEAM_INVITE_TARGET_OFFLINE = 1300026,
		/// <summary>
		///  邀请玩家，对方已经在队伍中
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请玩家，对方已经在队伍中", " 邀请玩家，对方已经在队伍中")]
		TEAM_INVITE_TARGET_IN_TEAM = 1300027,
		/// <summary>
		///  邀请玩家，目标正忙
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请玩家，目标正忙", " 邀请玩家，目标正忙")]
		TEAM_INVITE_TARGET_BUSY = 1300028,
		/// <summary>
		///  邀请玩家，重复邀请
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请玩家，重复邀请", " 邀请玩家，重复邀请")]
		TEAM_INVITE_REPEAT = 1300029,
		/// <summary>
		///  加入队伍，队伍正在战斗中
		/// </summary>
		[AdvancedInspector.Descriptor(" 加入队伍，队伍正在战斗中", " 加入队伍，队伍正在战斗中")]
		TEAM_JOIN_RACING = 1300030,
		/// <summary>
		///  等级不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级不足", " 等级不足")]
		TEAM_MIN_LEVEL = 1300031,
		/// <summary>
		///  邀请玩家，对方等级不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请玩家，对方等级不足", " 邀请玩家，对方等级不足")]
		TEAM_INVITE_TARGET_MIN_LEVEL = 1300032,
		/// <summary>
		///  邀请玩家，太频繁
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请玩家，太频繁", " 邀请玩家，太频繁")]
		TEAM_INVITE_FREQUENTLY = 1300033,
		/// <summary>
		///  开始匹配失败，这种情况不需要提示
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始匹配失败，这种情况不需要提示", " 开始匹配失败，这种情况不需要提示")]
		TEAM_MATCH_START_FAILED = 1301001,
		/// <summary>
		///  只有队长能操作
		/// </summary>
		[AdvancedInspector.Descriptor(" 只有队长能操作", " 只有队长能操作")]
		TEAM_MATCH_ONLY_MASTER = 1301002,
		/// <summary>
		///  开始匹配失败，已经在匹配中
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始匹配失败，已经在匹配中", " 开始匹配失败，已经在匹配中")]
		TEAM_MATCH_START_IN_MATCHING = 1301003,
		/// <summary>
		///  取消匹配失败，不在匹配中
		/// </summary>
		[AdvancedInspector.Descriptor(" 取消匹配失败，不在匹配中", " 取消匹配失败，不在匹配中")]
		TEAM_MATCH_CANCEL_NOT_IN_MATCHING = 1301004,
		/// <summary>
		///  次元石相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 次元石相关", " 次元石相关")]
		WARPSTONE = 1400000,
		/// <summary>
		///  银币不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 银币不够", " 银币不够")]
		WARP_STONE_SILVER_ERROR = 1400001,
		/// <summary>
		///  没有解锁
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有解锁", " 没有解锁")]
		WARP_STONE_UNLOCK_ERROR = 1400002,
		/// <summary>
		///  到达最大等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 到达最大等级", " 到达最大等级")]
		WARP_STONE_LEVEL_MAX = 1400003,
		/// <summary>
		///  次元石没找到
		/// </summary>
		[AdvancedInspector.Descriptor(" 次元石没找到", " 次元石没找到")]
		WARP_STONE_NOT_FOUNT = 1400004,
		/// <summary>
		///  次元石解锁等级不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 次元石解锁等级不够", " 次元石解锁等级不够")]
		WARP_STONE_PLAYER_LEVEL_ERROR = 1400005,
		/// <summary>
		///  能量石没有找到
		/// </summary>
		[AdvancedInspector.Descriptor(" 能量石没有找到", " 能量石没有找到")]
		ENERGY_STONE_NOT_FOUNT = 1400006,
		/// <summary>
		///  能量石不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 能量石不够", " 能量石不够")]
		ENERGY_STONE_NOT_ENOUGH = 1400007,
		/// <summary>
		///  能量石类型错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 能量石类型错误", " 能量石类型错误")]
		ENERGY_STONE_TYPE_ERROR = 1400008,
		/// <summary>
		///  随从相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 随从相关", " 随从相关")]
		RETINUE = 1500000,
		/// <summary>
		///  没有对应的玩家
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有对应的玩家", " 没有对应的玩家")]
		RETINUE_NOT_PLAYER = 1500001,
		/// <summary>
		///  随从数据表不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 随从数据表不存在", " 随从数据表不存在")]
		RETINUE_DATA_NOT_EXIST = 1500002,
		/// <summary>
		///  随从不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 随从不存在", " 随从不存在")]
		RETINUE_NOT_EXIST = 1500003,
		/// <summary>
		///  随从已经存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 随从已经存在", " 随从已经存在")]
		RETINUE_IS_EXIST = 1500004,
		/// <summary>
		///  玩家等级不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家等级不够", " 玩家等级不够")]
		RETINUE_PLAYER_LEVEL = 1500005,
		/// <summary>
		///  洗练物品不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 洗练物品不够", " 洗练物品不够")]
		RETINUE_NOT_ITEM = 1500006,
		/// <summary>
		///  解锁物品不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 解锁物品不够", " 解锁物品不够")]
		RETINUE_UNLOCK_NOT_ITEM = 1500007,
		/// <summary>
		///  随从等级不存在.
		/// </summary>
		[AdvancedInspector.Descriptor(" 随从等级不存在.", " 随从等级不存在.")]
		RETINUE_LEVEL_DATA_NOT_EXIST = 1500008,
		/// <summary>
		///  升级物品不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 升级物品不够", " 升级物品不够")]
		RETINUE_LEVEL_NOT_ITEM = 1500009,
		/// <summary>
		///  设置随从位置错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 设置随从位置错误", " 设置随从位置错误")]
		RETINUE_RETINUE_INDEX_ERROR = 1500010,
		/// <summary>
		///  勇士之魂不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 勇士之魂不够", " 勇士之魂不够")]
		RETINUE_WARRIOR_SOUL_ERROR = 1500011,
		/// <summary>
		///  不能超过最大星级
		/// </summary>
		[AdvancedInspector.Descriptor(" 不能超过最大星级", " 不能超过最大星级")]
		RETINUE_MAX_STAR_ERROR = 1500012,
		/// <summary>
		///  没有这个星级
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有这个星级", " 没有这个星级")]
		RETINUE_STAR_LEVEL_NOT_EXIST = 1500013,
		/// <summary>
		///  升星碎片不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 升星碎片不够", " 升星碎片不够")]
		RETINUE_UP_STAR_NOT_ITEM = 1500014,
		/// <summary>
		///  没有可洗练的技能
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有可洗练的技能", " 没有可洗练的技能")]
		RETINUE_NOT_CHANGE_SKILL_ERROR = 1500015,
		/// <summary>
		///  没有对应的洗练库
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有对应的洗练库", " 没有对应的洗练库")]
		RETINUE_NOT_SKILL_GROUP_ERROR = 1500016,
		/// <summary>
		///  洗练库是一个环
		/// </summary>
		[AdvancedInspector.Descriptor(" 洗练库是一个环", " 洗练库是一个环")]
		RETINUE_SKILL_GROUP_RING_ERROR = 1500017,
		/// <summary>
		///  升级类型不正确
		/// </summary>
		[AdvancedInspector.Descriptor(" 升级类型不正确", " 升级类型不正确")]
		RETINUE_UP_TYPE_ERROR = 1500018,
		/// <summary>
		///  没有主随从
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有主随从", " 没有主随从")]
		RETINUE_NOT_MAIN_ERROR = 1500019,
		/// <summary>
		///  无法下阵随从
		/// </summary>
		[AdvancedInspector.Descriptor(" 无法下阵随从", " 无法下阵随从")]
		RETINUE_NOT_DOWN_ERROR = 1500020,
		/// <summary>
		///  重连
		/// </summary>
		[AdvancedInspector.Descriptor(" 重连", " 重连")]
		RECONNECT = 1600000,
		/// <summary>
		///  角色数据已经删除
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色数据已经删除", " 角色数据已经删除")]
		RECONNECT_PLAYER_DELETED = 1600001,
		/// <summary>
		///  无效的sequence
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的sequence", " 无效的sequence")]
		RECONNECT_INVALID_SEQUENCE = 1600002,
		/// <summary>
		///  账号还在线
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号还在线", " 账号还在线")]
		RECONNECT_PLAYER_ONLINE = 1600003,
		/// <summary>
		///  错误的连接
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误的连接", " 错误的连接")]
		RECONNECT_NO_CONNECTION = 1600004,
		/// <summary>
		///  商城
		/// </summary>
		[AdvancedInspector.Descriptor(" 商城", " 商城")]
		MALL = 1700000,
		/// <summary>
		///  商城商品查询失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 商城商品查询失败", " 商城商品查询失败")]
		MALL_QUERY_FAIL = 1700001,
		/// <summary>
		///  购买数量错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买数量错误", " 购买数量错误")]
		MALL_BUYNUM_ERR = 1700002,
		/// <summary>
		///  找不到要购买的物品
		/// </summary>
		[AdvancedInspector.Descriptor(" 找不到要购买的物品", " 找不到要购买的物品")]
		MALL_CANNOT_FIND_ITEM = 1700003,
		/// <summary>
		///  找不到要触发的限时礼包
		/// </summary>
		[AdvancedInspector.Descriptor(" 找不到要触发的限时礼包", " 找不到要触发的限时礼包")]
		MALL_CANNOT_FIND_GIFT_PACK = 1700004,
		/// <summary>
		///  商城限时礼包已触发
		/// </summary>
		[AdvancedInspector.Descriptor(" 商城限时礼包已触发", " 商城限时礼包已触发")]
		MALL_GIFT_PACK_ACTIVATED = 1700005,
		/// <summary>
		///  玩家相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家相关", " 玩家相关")]
		PLAYER = 1800000,
		/// <summary>
		///  转职等级不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 转职等级不够", " 转职等级不够")]
		PLAYER_TRANSFORM_OCCU_LEVEL_ERROR = 1800001,
		/// <summary>
		///  觉醒等级不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 觉醒等级不够", " 觉醒等级不够")]
		PLAYER_AWAKEN_LEVEL_ERROR = 1800002,
		/// <summary>
		///  已经觉醒
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经觉醒", " 已经觉醒")]
		PLAYER_AWAKEN_EXIST = 1800003,
		/// <summary>
		///  还未转职
		/// </summary>
		[AdvancedInspector.Descriptor(" 还未转职", " 还未转职")]
		PLAYER_AWAKEN_NOT_TRANSFORM_OCCU = 1800004,
		/// <summary>
		///  vip购买时,vip等级错误
		/// </summary>
		[AdvancedInspector.Descriptor(" vip购买时,vip等级错误", " vip购买时,vip等级错误")]
		PLAYER_VIP_BUY_LEVEL_ERROR = 1800005,
		/// <summary>
		///  vip购买礼包,点卷不够
		/// </summary>
		[AdvancedInspector.Descriptor(" vip购买礼包,点卷不够", " vip购买礼包,点卷不够")]
		PLAYER_VIP_BUY_GIFT_ENOUGH_POINT = 1800006,
		/// <summary>
		///  vip礼包为空
		/// </summary>
		[AdvancedInspector.Descriptor(" vip礼包为空", " vip礼包为空")]
		PLAYER_VIP_GIFT_EMPTY = 1800007,
		/// <summary>
		///  vip购买礼包背包空间不足
		/// </summary>
		[AdvancedInspector.Descriptor(" vip购买礼包背包空间不足", " vip购买礼包背包空间不足")]
		PLAYER_VIP_BUY_NOT_ENOUGH_PACKSIZE = 1800008,
		/// <summary>
		///  vip购买礼包消耗点卷失败
		/// </summary>
		[AdvancedInspector.Descriptor(" vip购买礼包消耗点卷失败", " vip购买礼包消耗点卷失败")]
		PLAYER_VIP_BUY_REMOVE_POINT_ERROR = 1800009,
		/// <summary>
		///  vip已经购买这个礼包
		/// </summary>
		[AdvancedInspector.Descriptor(" vip已经购买这个礼包", " vip已经购买这个礼包")]
		PLAYER_VIP_BUY_ALREADY = 1800010,
		/// <summary>
		///  vip等级不足
		/// </summary>
		[AdvancedInspector.Descriptor(" vip等级不足", " vip等级不足")]
		PLAYER_VIPLV_NOT_ENOUGH = 1800011,
		/// <summary>
		///  快速购买
		/// </summary>
		[AdvancedInspector.Descriptor(" 快速购买", " 快速购买")]
		QUICKBUY = 1900000,
		/// <summary>
		///  系统错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 系统错误", " 系统错误")]
		QUICK_BUY_SYSTEM_ERROR = 1900001,
		/// <summary>
		///  上一个事务还没结束
		/// </summary>
		[AdvancedInspector.Descriptor(" 上一个事务还没结束", " 上一个事务还没结束")]
		QUICK_BUY_LAST_TRANS_NOT_FINISH = 1900002,
		/// <summary>
		///  超时
		/// </summary>
		[AdvancedInspector.Descriptor(" 超时", " 超时")]
		QUICK_BUY_TIMEOUT = 1900003,
		/// <summary>
		///  无效的类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的类型", " 无效的类型")]
		QUICK_BUY_INVALID_TYPE = 1900004,
		/// <summary>
		///  钱不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 钱不够", " 钱不够")]
		QUICK_BUY_NOT_ENOUGH_MONEY = 1900005,
		/// <summary>
		///  道具不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具不存在", " 道具不存在")]
		QUICK_BUY_INVALID_ITEM = 1900006,
		/// <summary>
		///  道具数量不正确
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具数量不正确", " 道具数量不正确")]
		QUICK_BUY_INVALID_NUM = 1900007,
		/// <summary>
		///  背包空间不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 背包空间不足", " 背包空间不足")]
		QUICK_BUY_BAG_FULL = 1900008,
		/// <summary>
		///  不在比赛中
		/// </summary>
		[AdvancedInspector.Descriptor(" 不在比赛中", " 不在比赛中")]
		QUICK_BUY_REVIVE_NOT_IN_RACE = 1900009,
		/// <summary>
		///  任务相关
		/// </summary>
		[AdvancedInspector.Descriptor(" 任务相关", " 任务相关")]
		TASK = 2000000,
		/// <summary>
		///  提交任务物品类型错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 提交任务物品类型错误", " 提交任务物品类型错误")]
		TASK_SET_ITEM_TASK_TYPE_ERROR = 2000001,
		/// <summary>
		///  提交错误的任务物品
		/// </summary>
		[AdvancedInspector.Descriptor(" 提交错误的任务物品", " 提交错误的任务物品")]
		TASK_SET_ITEM_ERROR = 2000002,
		/// <summary>
		///  提交的任务物品数量不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 提交的任务物品数量不够", " 提交的任务物品数量不够")]
		TASK_SET_ITEM_NUM_ERROR = 2000003,
		/// <summary>
		///  任务不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 任务不存在", " 任务不存在")]
		TASK_NOT_EXIST = 2000004,
		/// <summary>
		///  任务脚本不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 任务脚本不存在", " 任务脚本不存在")]
		TASK_SCRIPT_NOT_EXIST = 2000005,
		/// <summary>
		///  任务不再接取状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 任务不再接取状态", " 任务不再接取状态")]
		TASK_NOT_UNFINISH = 2000006,
		/// <summary>
		///  任务类型错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 任务类型错误", " 任务类型错误")]
		TASK_TYPE_ERROR = 2000007,
		/// <summary>
		///  循环任务不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 循环任务不存在", " 循环任务不存在")]
		TASK_CYCLE_NOT_EXIST = 2000008,
		/// <summary>
		///  消耗资源不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 消耗资源不足", " 消耗资源不足")]
		TASK_CYCLE_REFRESH_NOT_CONSUME = 2000009,
		/// <summary>
		///  每日任务积分奖励箱子不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 每日任务积分奖励箱子不存在", " 每日任务积分奖励箱子不存在")]
		TASK_DATILY_TASK_SCORE_BOX_NOT_FOUNT = 2000010,
		/// <summary>
		///  每日任务积分奖励积分不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 每日任务积分奖励积分不够", " 每日任务积分奖励积分不够")]
		TASK_DATILY_TASK_SCORE_BOX_SCORE = 2000011,
		/// <summary>
		///  每日任务积分奖励已经领取
		/// </summary>
		[AdvancedInspector.Descriptor(" 每日任务积分奖励已经领取", " 每日任务积分奖励已经领取")]
		TASK_DATILY_TASK_SCORE_BOX_RECEIVE = 2000012,
		/// <summary>
		///  死亡之塔
		/// </summary>
		[AdvancedInspector.Descriptor(" 死亡之塔", " 死亡之塔")]
		TOWER = 2100000,
		/// <summary>
		///  没有重置次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有重置次数", " 没有重置次数")]
		TOWER_RESET_NO_REMAIN_COUNT = 2100001,
		/// <summary>
		///  正在扫荡中
		/// </summary>
		[AdvancedInspector.Descriptor(" 正在扫荡中", " 正在扫荡中")]
		TOWER_DOING_WIPEOUT = 2100002,
		/// <summary>
		///  没有层数去扫荡
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有层数去扫荡", " 没有层数去扫荡")]
		TOWER_NO_FLOOR_WIPEOUT = 2100003,
		/// <summary>
		///  没有在扫荡中
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有在扫荡中", " 没有在扫荡中")]
		TOWER_NOT_DOING_WIPEOUT = 2100004,
		/// <summary>
		///  扫荡未完成
		/// </summary>
		[AdvancedInspector.Descriptor(" 扫荡未完成", " 扫荡未完成")]
		TOWER_WIPEOUT_NOT_FINISH = 2100005,
		/// <summary>
		///  扫荡已完成
		/// </summary>
		[AdvancedInspector.Descriptor(" 扫荡已完成", " 扫荡已完成")]
		TOWER_WIPEOUT_FINISHED = 2100006,
		/// <summary>
		///  没有足够的点劵
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有足够的点劵", " 没有足够的点劵")]
		TOWER_WIPEOUT_NOT_ENOUGH_POINT = 2100007,
		/// <summary>
		///  未通过该层
		/// </summary>
		[AdvancedInspector.Descriptor(" 未通过该层", " 未通过该层")]
		TOWER_AWARD_NOT_PASS_FLOOR = 2100008,
		/// <summary>
		///  无效的层数，没有对应奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的层数，没有对应奖励", " 无效的层数，没有对应奖励")]
		TOWER_AWARD_INVALID_FLOOR = 2100009,
		/// <summary>
		///  重复领奖
		/// </summary>
		[AdvancedInspector.Descriptor(" 重复领奖", " 重复领奖")]
		TOWER_AWARD_REPEAT_RECEIVE = 2100010,
		/// <summary>
		///  不需要重置
		/// </summary>
		[AdvancedInspector.Descriptor(" 不需要重置", " 不需要重置")]
		TOWER_NO_NEED_RESET = 2100011,
		/// <summary>
		///  没有VIP权限
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有VIP权限", " 没有VIP权限")]
		TOWER_RESET_NO_VIP_PRIVILEGE = 2100012,
		/// <summary>
		///  PK相关
		/// </summary>
		[AdvancedInspector.Descriptor(" PK相关", " PK相关")]
		PK = 2200000,
		/// <summary>
		///  组队中
		/// </summary>
		[AdvancedInspector.Descriptor(" 组队中", " 组队中")]
		PK_CHALLENGE_IN_TEAM = 2200001,
		/// <summary>
		///  不在PK准备区中
		/// </summary>
		[AdvancedInspector.Descriptor(" 不在PK准备区中", " 不在PK准备区中")]
		PK_CHALLENGE_NOT_IN_PK_PREPARE = 2200002,
		/// <summary>
		///  目标在忙
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标在忙", " 目标在忙")]
		PK_CHALLENGE_TARGET_BUSY = 2200003,
		/// <summary>
		///  目标不在线
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标不在线", " 目标不在线")]
		PK_CHALLENGE_TARGET_NOT_ONLINE = 2200004,
		/// <summary>
		///  重复挑战
		/// </summary>
		[AdvancedInspector.Descriptor(" 重复挑战", " 重复挑战")]
		PK_CHALLENGE_REPEAT = 2200005,
		/// <summary>
		///  等级太低
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级太低", " 等级太低")]
		PK_CHALLENGE_LEVEL_LIMIT = 2200006,
		/// <summary>
		///  系统错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 系统错误", " 系统错误")]
		PK_WUDAO_SYSTEM_ERROR = 2200007,
		/// <summary>
		///  活动未开始
		/// </summary>
		[AdvancedInspector.Descriptor(" 活动未开始", " 活动未开始")]
		PK_WUDAO_ACTIVITY_NOT_OPEN = 2200008,
		/// <summary>
		///  不满足活动开启条件
		/// </summary>
		[AdvancedInspector.Descriptor(" 不满足活动开启条件", " 不满足活动开启条件")]
		PK_WUDAO_ACTIVITY_COND = 2200009,
		/// <summary>
		///  没有门票
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有门票", " 没有门票")]
		PK_WUDAO_NO_TICKET = 2200010,
		/// <summary>
		///  已经参加了
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经参加了", " 已经参加了")]
		PK_WUDAO_JOINED = 2200011,
		/// <summary>
		///  武道大会未完成
		/// </summary>
		[AdvancedInspector.Descriptor(" 武道大会未完成", " 武道大会未完成")]
		PK_WUDAO_NOT_COMPLETE = 2200012,
		/// <summary>
		///  公会
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会", " 公会")]
		GUILD = 2300000,
		/// <summary>
		///  没有对应的权限
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有对应的权限", " 没有对应的权限")]
		GUILD_NO_POWER = 2300001,
		/// <summary>
		///  公会已满
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会已满", " 公会已满")]
		GUILD_FULL = 2300002,
		/// <summary>
		///  不在公会中
		/// </summary>
		[AdvancedInspector.Descriptor(" 不在公会中", " 不在公会中")]
		GUILD_NOT_IN_GUILD = 2300003,
		/// <summary>
		///  系统错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 系统错误", " 系统错误")]
		GUILD_SYS_ERROR = 2300004,
		/// <summary>
		///  没有足够的钱
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有足够的钱", " 没有足够的钱")]
		GUILD_NOT_ENOUGH_MONEY = 2300005,
		/// <summary>
		///  次数用完
		/// </summary>
		[AdvancedInspector.Descriptor(" 次数用完", " 次数用完")]
		GUILD_NOT_ENOUGH_TIMES = 2300006,
		/// <summary>
		///  公会战期间不能离开公会
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战期间不能离开公会", " 公会战期间不能离开公会")]
		GUILD_BATTLE_NOT_LEAVE = 2300007,
		/// <summary>
		///  名字重复
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字重复", " 名字重复")]
		GUILD_NAME_REPEAT = 2301001,
		/// <summary>
		///  名字中有屏蔽字
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字中有屏蔽字", " 名字中有屏蔽字")]
		GUILD_NAME_INVALID = 2301002,
		/// <summary>
		///  宣言中有屏蔽字
		/// </summary>
		[AdvancedInspector.Descriptor(" 宣言中有屏蔽字", " 宣言中有屏蔽字")]
		GUILD_DECLARATION_INVALID = 2301003,
		/// <summary>
		///  公告中有屏蔽字
		/// </summary>
		[AdvancedInspector.Descriptor(" 公告中有屏蔽字", " 公告中有屏蔽字")]
		GUILD_ANNOUNCEMENT_INVALID = 2301004,
		/// <summary>
		///  公告中有屏蔽字
		/// </summary>
		[AdvancedInspector.Descriptor(" 公告中有屏蔽字", " 公告中有屏蔽字")]
		GUILD_MAIL_INVALID = 2301005,
		/// <summary>
		///  重复创建
		/// </summary>
		[AdvancedInspector.Descriptor(" 重复创建", " 重复创建")]
		GUILD_CREATE_REPEAT = 2302001,
		/// <summary>
		///  已经在公会中
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经在公会中", " 已经在公会中")]
		GUILD_CREATE_ALREADY_HAS_GUILD = 2302002,
		/// <summary>
		///  无效的公会名
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的公会名", " 无效的公会名")]
		GUILD_CREATE_INVALID_NAME = 2302003,
		/// <summary>
		///  无效的公会宣言
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的公会宣言", " 无效的公会宣言")]
		GUILD_CREATE_INVALID_DECLARATION = 2302004,
		/// <summary>
		///  没有足够的钱
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有足够的钱", " 没有足够的钱")]
		GUILD_CREATE_NOT_ENOUGH_MONEY = 2302005,
		/// <summary>
		///  等级不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级不足", " 等级不足")]
		GUILD_CREATE_MIN_LEVEL = 2302006,
		/// <summary>
		///  刚离开公会，还在冷却时间中
		/// </summary>
		[AdvancedInspector.Descriptor(" 刚离开公会，还在冷却时间中", " 刚离开公会，还在冷却时间中")]
		GUILD_CREATE_COLDTIME = 2302007,
		/// <summary>
		///  公会名为空
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会名为空", " 公会名为空")]
		GUILD_CREATE_NAME_EMPTY = 2302008,
		/// <summary>
		///  公会宣言为空
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会宣言为空", " 公会宣言为空")]
		GUILD_CREATE_DECLARATION_EMPTY = 2302009,
		/// <summary>
		///  重复加入
		/// </summary>
		[AdvancedInspector.Descriptor(" 重复加入", " 重复加入")]
		GUILD_JOIN_REPEAT = 2303001,
		/// <summary>
		///  已经在公会中
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经在公会中", " 已经在公会中")]
		GUILD_JOIN_ALREADY_HAS_GUILD = 2303002,
		/// <summary>
		///  等级不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级不足", " 等级不足")]
		GUILD_JOIN_MIN_LEVEL = 2303003,
		/// <summary>
		///  公会不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会不存在", " 公会不存在")]
		GUILD_JOIN_NOT_EXIST = 2303004,
		/// <summary>
		///  刚离开公会，还在冷却时间中
		/// </summary>
		[AdvancedInspector.Descriptor(" 刚离开公会，还在冷却时间中", " 刚离开公会，还在冷却时间中")]
		GUILD_JOIN_COLDTIME = 2303005,
		/// <summary>
		///  公会申请列表已满
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会申请列表已满", " 公会申请列表已满")]
		GUILD_JOIN_REQUEST_QUEUE_FULL = 2303006,
		/// <summary>
		///  公会正在解散
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会正在解散", " 公会正在解散")]
		GUILD_JOIN_IN_DISMISS = 2303007,
		/// <summary>
		///  先转让会长
		/// </summary>
		[AdvancedInspector.Descriptor(" 先转让会长", " 先转让会长")]
		GUILD_LEAVE_TRANSFER_LEADER = 2304001,
		/// <summary>
		///  没有这个请求者
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有这个请求者", " 没有这个请求者")]
		GUILD_REPLY_REQUESTER_UNEXIST = 2305001,
		/// <summary>
		///  已经在其他公会中了
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经在其他公会中了", " 已经在其他公会中了")]
		GUILD_REPLY_IN_OTHER_GUILD = 2305002,
		/// <summary>
		///  刚离开公会，还在冷却时间中
		/// </summary>
		[AdvancedInspector.Descriptor(" 刚离开公会，还在冷却时间中", " 刚离开公会，还在冷却时间中")]
		GUILD_REPLY_COLDTIME = 2305003,
		/// <summary>
		///  目标职务人数已满
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标职务人数已满", " 目标职务人数已满")]
		GUILD_POST_FULL = 2306001,
		/// <summary>
		///  需要会长连续7天不在线
		/// </summary>
		[AdvancedInspector.Descriptor(" 需要会长连续7天不在线", " 需要会长连续7天不在线")]
		GUILD_POST_LEADER_LEAVE_TIME = 2306002,
		/// <summary>
		///  超过主城等级，先升级主城等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 超过主城等级，先升级主城等级", " 超过主城等级，先升级主城等级")]
		GUILD_BUILDING_UPGRADE_MAIN_FIRST = 2307001,
		/// <summary>
		///  已经到满级
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经到满级", " 已经到满级")]
		GUILD_BUILDING_TOP_LEVEL = 2307002,
		/// <summary>
		///  帮会资金不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 帮会资金不足", " 帮会资金不足")]
		GUILD_BUILDING_NOT_ENOUGH_FUND = 2307003,
		/// <summary>
		///  剩余次数不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余次数不足", " 剩余次数不足")]
		GUILD_DONATE_NO_REMAIN_TIMES = 2308001,
		/// <summary>
		///  还在CD中
		/// </summary>
		[AdvancedInspector.Descriptor(" 还在CD中", " 还在CD中")]
		GUILD_EXCHANGE_CD = 2309001,
		/// <summary>
		///  剩余次数不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余次数不足", " 剩余次数不足")]
		GUILD_EXCHANGE_NO_REMAIN_TIMES = 2309002,
		/// <summary>
		///  贡献不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 贡献不足", " 贡献不足")]
		GUILD_EXCHANGE_NOT_ENOUGH_CONTRI = 2309003,
		/// <summary>
		///  已经到达最高等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经到达最高等级", " 已经到达最高等级")]
		GUILD_SKILL_TOP_LEVEL = 2310001,
		/// <summary>
		///  正在解散中
		/// </summary>
		[AdvancedInspector.Descriptor(" 正在解散中", " 正在解散中")]
		GUILD_DISMISS_IN_DISMISS = 2311001,
		/// <summary>
		///  不在解散中
		/// </summary>
		[AdvancedInspector.Descriptor(" 不在解散中", " 不在解散中")]
		GUILD_NOT_IN_DISMISS = 2311002,
		/// <summary>
		///  圆桌已满
		/// </summary>
		[AdvancedInspector.Descriptor(" 圆桌已满", " 圆桌已满")]
		GUILD_TABLE_FULL = 2312001,
		/// <summary>
		///  位子已经被占
		/// </summary>
		[AdvancedInspector.Descriptor(" 位子已经被占", " 位子已经被占")]
		GUILD_TABLE_SEAT_HAS_PLAYER = 2312002,
		/// <summary>
		///  已经在位子上了
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经在位子上了", " 已经在位子上了")]
		GUILD_TABLE_REPEAT = 2312003,
		/// <summary>
		///  无效的位子
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的位子", " 无效的位子")]
		GUILD_TABLE_SEAT_INVALID = 2312004,
		/// <summary>
		///  膜拜类型错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 膜拜类型错误", " 膜拜类型错误")]
		GUILD_ORZ_INVALID_TYPE = 2313001,
		/// <summary>
		///  没有这个VIP权限
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有这个VIP权限", " 没有这个VIP权限")]
		GUILD_ORZ_VIP_PRIVILEGE = 2313002,
		/// <summary>
		///  没有找到玩家
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有找到玩家", " 没有找到玩家")]
		GUILD_BATTLE_NOT_PLAYER = 2314001,
		/// <summary>
		///  玩家没有公会
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家没有公会", " 玩家没有公会")]
		GUILD_BATTLE_NOT_EXIST = 2314002,
		/// <summary>
		///  玩家不是公会成员
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家不是公会成员", " 玩家不是公会成员")]
		GUILD_BATTLE_NOT_IS_MEMBER = 2314003,
		/// <summary>
		///  公会战报名公会找不到
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战报名公会找不到", " 公会战报名公会找不到")]
		GUILD_BATTLE_ENROLL_GUILD_NOT_FIND = 2314004,
		/// <summary>
		///  玩家报名没有权限
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家报名没有权限", " 玩家报名没有权限")]
		GUILD_BATTLE_ENROLL_NOT_POWER = 2314005,
		/// <summary>
		///  公会战没有开始报名
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战没有开始报名", " 公会战没有开始报名")]
		GUILD_BATTLE_ENROLL_NOT_ENROLL = 2314006,
		/// <summary>
		///  公会战报名人数已满
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战报名人数已满", " 公会战报名人数已满")]
		GUILD_BATTLE_ENROLL_FULL = 2314007,
		/// <summary>
		///  公会战报名公会等级不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战报名公会等级不够", " 公会战报名公会等级不够")]
		GUILD_BATTLE_ENROLL_GUILD_LEVEL = 2314008,
		/// <summary>
		///  公会战报名占领领地等级太低
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战报名占领领地等级太低", " 公会战报名占领领地等级太低")]
		GUILD_BATTLE_ENROLL_TERRITORY_LEVEL_LOW = 2314009,
		/// <summary>
		///  公会战报名占领领地等级太高
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战报名占领领地等级太高", " 公会战报名占领领地等级太高")]
		GUILD_BATTLE_ENROLL_TERRITORY_LEVEL_HIGH = 2314010,
		/// <summary>
		///  公会战不能重复报名
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战不能重复报名", " 公会战不能重复报名")]
		GUILD_BATTLE_ENROLL_EXIST = 2314011,
		/// <summary>
		///  公会战报名正在处理事务
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战报名正在处理事务", " 公会战报名正在处理事务")]
		GUILD_BATTLE_ENROLL_TRANSACTION = 2314012,
		/// <summary>
		///  领地不存在或者公会没有报名
		/// </summary>
		[AdvancedInspector.Descriptor(" 领地不存在或者公会没有报名", " 领地不存在或者公会没有报名")]
		GUILD_BATTLE_TERRITORY_NOT_EXIST = 2314013,
		/// <summary>
		///  公会没有报名
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会没有报名", " 公会没有报名")]
		GUILD_BATTLE_NOT_ENROLL = 2314014,
		/// <summary>
		///  公会战鼓舞已经最高次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战鼓舞已经最高次数", " 公会战鼓舞已经最高次数")]
		GUILD_BATTLE_INSPIRE_MAX_COUNT = 2314015,
		/// <summary>
		///  公会战鼓舞道具不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战鼓舞道具不足", " 公会战鼓舞道具不足")]
		GUILD_BATTLE_INSPIRE_ITEM = 2314016,
		/// <summary>
		///  公会战鼓舞正在处理事务
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战鼓舞正在处理事务", " 公会战鼓舞正在处理事务")]
		GUILD_BATTLE_INSPIRE_TRANSACTION = 2314017,
		/// <summary>
		///  公会战成员匹配次数不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战成员匹配次数不足", " 公会战成员匹配次数不足")]
		GUILD_BATTLE_MEMBER_MATCH_COUNT = 2314018,
		/// <summary>
		///  战斗结束找不到玩家
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗结束找不到玩家", " 战斗结束找不到玩家")]
		GUILD_BATTLE_RACE_END_NOT_MEMBER = 2314019,
		/// <summary>
		///  公会战已经结束
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战已经结束", " 公会战已经结束")]
		GUILD_BATTLE_IS_END = 2314020,
		/// <summary>
		///  领取奖励找不到玩家
		/// </summary>
		[AdvancedInspector.Descriptor(" 领取奖励找不到玩家", " 领取奖励找不到玩家")]
		GUILD_BATTLE_GIVE_REWARD_NOT_MEMBER = 2314021,
		/// <summary>
		///  领取奖励没有找到数据项
		/// </summary>
		[AdvancedInspector.Descriptor(" 领取奖励没有找到数据项", " 领取奖励没有找到数据项")]
		GUILD_BATTLE_GIVE_REWARD_DATA_ERROR = 2314022,
		/// <summary>
		///  领取奖励积分不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 领取奖励积分不足", " 领取奖励积分不足")]
		GUILD_BATTLE_GIVE_REWARD_SCORE_ERROR = 2314023,
		/// <summary>
		///  奖励已经领取
		/// </summary>
		[AdvancedInspector.Descriptor(" 奖励已经领取", " 奖励已经领取")]
		GUILD_BATTLE_GIVE_REWARD_ALREADY = 2314024,
		/// <summary>
		///  公会战上一场战斗没有结束
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战上一场战斗没有结束", " 公会战上一场战斗没有结束")]
		GUILD_BATTLE_RACE_NOT_END = 2314025,
		/// <summary>
		///  公会战匹配正在处理事务
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战匹配正在处理事务", " 公会战匹配正在处理事务")]
		GUILD_BATTLE_MATCH_TRANSACTION = 2314026,
		/// <summary>
		///  公会战报名不能解散
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战报名不能解散", " 公会战报名不能解散")]
		GUILD_BATTLE_ENROLL_NOT_DISMISS = 2314027,
		/// <summary>
		///  公会战解散中不能报名
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战解散中不能报名", " 公会战解散中不能报名")]
		GUILD_BATTLE_DISMISS_NOT_ENROLL = 2314028,
		/// <summary>
		/// 公会地下城
		/// </summary>
		/// <summary>
		///  公会地下城错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城错误", " 公会地下城错误")]
		GUILD_DUNGEON_ERROR = 2317001,
		/// <summary>
		///  公会地下城配置错误			
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城配置错误			", " 公会地下城配置错误			")]
		GUILD_DUNGEON_CFG_ERROR = 2317002,
		/// <summary>
		///  公会地下城没有通关
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城没有通关", " 公会地下城没有通关")]
		GUILD_DUNGEON_NOT_FINISH = 2317003,
		/// <summary>
		///  公会地下城奖励错误	
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城奖励错误	", " 公会地下城奖励错误	")]
		GUILD_DUNGEON_REWARD_ERROR = 2317004,
		/// <summary>
		///  公会地下城已经领取了奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城已经领取了奖励", " 公会地下城已经领取了奖励")]
		GUILD_DUNGEON_REWARD_GET_REPEATED = 2317005,
		/// <summary>
		///  公会地下城开启不能离开公会	
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城开启不能离开公会	", " 公会地下城开启不能离开公会	")]
		GUILD_DUNGEON_NOT_LEAVE = 2317006,
		/// <summary>
		///  公会等级不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会等级不足", " 公会等级不足")]
		GUILD_DUNGEON_MIN_LEVEL = 2317007,
		/// <summary>
		///  公会副本，玩家等级不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会副本，玩家等级不足", " 公会副本，玩家等级不足")]
		GUILD_DUNGEON_PLAYER_LEVEL_LIMIT = 2317008,
		/// <summary>
		///  公会副本，玩家不是同一公会
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会副本，玩家不是同一公会", " 公会副本，玩家不是同一公会")]
		GUILD_DUNGEON_NOT_SAME_GUILD = 2317009,
		/// <summary>
		///  公会地下城开启不能踢人
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城开启不能踢人", " 公会地下城开启不能踢人")]
		GUILD_DUNGEON_NOT_KICK = 2317010,
		/// <summary>
		///  公会地下城未开启
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城未开启", " 公会地下城未开启")]
		GUILD_DUNGEON_NOT_OPEN = 2317011,
		/// <summary>
		///  公会地下城已通关
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城已通关", " 公会地下城已通关")]
		GUILD_DUNGEON_GATE_FINISH = 2317012,
		/// <summary>
		///  公会地下城开启不能加入公会
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城开启不能加入公会", " 公会地下城开启不能加入公会")]
		GUILD_DUNGEON_NOT_JOIN = 2317013,
		/// <summary>
		///  公会地下城CD
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会地下城CD", " 公会地下城CD")]
		GUILD_DUNGEON_CD = 2317014,
		/// <summary>
		///  公会拍卖
		/// </summary>
		/// <summary>
		///  公会拍卖类型错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会拍卖类型错误", " 公会拍卖类型错误")]
		GUILD_AUCTION_TYPE_ERROR = 2318001,
		/// <summary>
		///  公会拍卖品不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会拍卖品不存在", " 公会拍卖品不存在")]
		GUILD_AUCTION_ITEM_NOT_EXIST = 2318002,
		/// <summary>
		///  公会拍卖竞价被超过				
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会拍卖竞价被超过				", " 公会拍卖竞价被超过				")]
		GUILD_AUCTION_BID_BE_OVER = 2318003,
		/// <summary>
		///  公会拍卖竞价出价错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会拍卖竞价出价错误", " 公会拍卖竞价出价错误")]
		GUILD_AUCTION_BID_PRICE_ERROR = 2318004,
		/// <summary>
		///  公会拍卖错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会拍卖错误", " 公会拍卖错误")]
		GUILD_AUCTION_ERROR = 2318005,
		/// <summary>
		///  不在拍卖状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 不在拍卖状态", " 不在拍卖状态")]
		GUILD_AUCTION_NOT_SEAL = 2318006,
		/// <summary>
		/// 公会兼并
		/// </summary>
		/// <summary>
		/// 请求已存在 
		/// </summary>
		[AdvancedInspector.Descriptor("请求已存在 ", "请求已存在 ")]
		GUILD_MERGER_REQUEST_EXISTS = 2320001,
		/// <summary>
		/// 请求不存在	 
		/// </summary>
		[AdvancedInspector.Descriptor("请求不存在	 ", "请求不存在	 ")]
		GUILD_MERGER_REQUEST_NOT_EXISTS = 2320002,
		/// <summary>
		/// 非法操作    
		/// </summary>
		[AdvancedInspector.Descriptor("非法操作    ", "非法操作    ")]
		GUILD_MERGER_INVAILD = 2320003,
		/// <summary>
		/// 只能同时同意一个兼并申请            
		/// </summary>
		[AdvancedInspector.Descriptor("只能同时同意一个兼并申请            ", "只能同时同意一个兼并申请            ")]
		GUILD_MERGER_REQUEST_ONE = 2320004,
		/// <summary>
		/// 距离上一次兼并时间过短      
		/// </summary>
		[AdvancedInspector.Descriptor("距离上一次兼并时间过短      ", "距离上一次兼并时间过短      ")]
		GUILD_MERGER_TIME_SHORT = 2320005,
		/// <summary>
		///  道具事务
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具事务", " 道具事务")]
		ITEM_TRANS = 2400000,
		/// <summary>
		///  失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 失败", " 失败")]
		ITEM_TRANS_FAILED = 2400001,
		/// <summary>
		///  钱不够
		/// </summary>
		[AdvancedInspector.Descriptor(" 钱不够", " 钱不够")]
		ITEM_TRANS_NOT_ENOUGH_MONEY = 2400002,
		/// <summary>
		///  复活币不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 复活币不足", " 复活币不足")]
		ITEM_TRANS_NOT_ENOUGH_REVIVE_COIN = 2400003,
		/// <summary>
		///  道具不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具不足", " 道具不足")]
		ITEM_TRANS_NOT_ENOUGH_ITEM = 2400004,
		/// <summary>
		///  次数不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 次数不足", " 次数不足")]
		ITEM_TRANS_NOT_ENOUGH_TIMES = 2400005,
		/// <summary>
		///  红包
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包", " 红包")]
		RED_PACKET = 2500000,
		/// <summary>
		///  系统错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 系统错误", " 系统错误")]
		RED_PACKET_SYS_ERROR = 2500001,
		/// <summary>
		///  无效的红包
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效的红包", " 无效的红包")]
		RED_PACKET_INVALID = 2500002,
		/// <summary>
		///  红包不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包不存在", " 红包不存在")]
		RED_PACKET_NOT_EXIST = 2500003,
		/// <summary>
		///  红包不是你的
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包不是你的", " 红包不是你的")]
		RED_PACKET_NOT_OWNER = 2500004,
		/// <summary>
		///  红包已经发出去了
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包已经发出去了", " 红包已经发出去了")]
		RED_PACKET_ALREADY_SEND = 2500005,
		/// <summary>
		///  公会红包，不在公会中
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会红包，不在公会中", " 公会红包，不在公会中")]
		RED_PACKET_NOT_IN_GUILD = 2500006,
		/// <summary>
		///  红包无法打开
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包无法打开", " 红包无法打开")]
		RED_PACKET_CANT_OPEN = 2500007,
		/// <summary>
		///  红包已被抢完
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包已被抢完", " 红包已被抢完")]
		RED_PACKET_EMPTY = 2500008,
		/// <summary>
		///  红包个数错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包个数错误", " 红包个数错误")]
		RED_PACKET_INVALID_NUM = 2500009,
		/// <summary>
		///  红包名字错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包名字错误", " 红包名字错误")]
		RED_PACKET_INVALID_NAME = 2500010,
		/// <summary>
		///  充值
		/// </summary>
		[AdvancedInspector.Descriptor(" 充值", " 充值")]
		BILLING = 2600000,
		/// <summary>
		///  玩家不在线
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家不在线", " 玩家不在线")]
		BILLING_PLAYER_OFFLINE = 2600001,
		/// <summary>
		///  货物不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 货物不存在", " 货物不存在")]
		BILLING_GOODS_UNEXIST = 2600002,
		/// <summary>
		///  没有次数了
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有次数了", " 没有次数了")]
		BILLING_GOODS_NUM_LIMIT = 2600003,
		/// <summary>
		///  月卡时间未到无法购买
		/// </summary>
		[AdvancedInspector.Descriptor(" 月卡时间未到无法购买", " 月卡时间未到无法购买")]
		BILLING_GOODS_MONTH_CARD_CANT_BUY = 2600004,
		/// <summary>
		///  被禁言了
		/// </summary>
		[AdvancedInspector.Descriptor(" 被禁言了", " 被禁言了")]
		HORN_FORBID_TALK = 2800004,
		/// <summary>
		///  客服系统
		/// </summary>
		[AdvancedInspector.Descriptor(" 客服系统", " 客服系统")]
		CUSTOM_SERVICE = 3100000,
		/// <summary>
		///  服务器异常
		/// </summary>
		[AdvancedInspector.Descriptor(" 服务器异常", " 服务器异常")]
		CUSTOM_SERVICE_SERVER_ERROR = 3100001,
		/// <summary>
		///  错误的参数
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误的参数", " 错误的参数")]
		CUSTOM_SERVICE_INVALID_PARAM = 3100002,
		/// <summary>
		///  客服系统功能关闭
		/// </summary>
		[AdvancedInspector.Descriptor(" 客服系统功能关闭", " 客服系统功能关闭")]
		CUSTOM_SERVICE_CLOSED = 3100003,
		/// <summary>
		/// 出师不是徒弟
		/// </summary>
		[AdvancedInspector.Descriptor("出师不是徒弟", "出师不是徒弟")]
		FISNISH_SCHOOL_NOTDISCIPLE = 3200004,
		/// <summary>
		/// 徒弟不在线
		/// </summary>
		[AdvancedInspector.Descriptor("徒弟不在线", "徒弟不在线")]
		RELATION_DISCIPLE_NOTONLINE = 3200005,
		/// <summary>
		/// 出师组队出错
		/// </summary>
		[AdvancedInspector.Descriptor("出师组队出错", "出师组队出错")]
		FISNISH_SCHOOL_TEAMERR = 3200006,
		/// <summary>
		/// 出师徒弟等级不足
		/// </summary>
		[AdvancedInspector.Descriptor("出师徒弟等级不足", "出师徒弟等级不足")]
		FISNISH_SCHOOL_DISCIPLE_LEVELINSUFFIC = 3200007,
		/// <summary>
		/// 发布师门任务失败
		/// </summary>
		[AdvancedInspector.Descriptor("发布师门任务失败", "发布师门任务失败")]
		MASTER_PUBlISHTASK_FAIL = 3200008,
		/// <summary>
		/// 发布师门任务失败,已经发布
		/// </summary>
		[AdvancedInspector.Descriptor("发布师门任务失败,已经发布", "发布师门任务失败,已经发布")]
		MASTER_PUBlISHTASK_FAIL_PUBISHED = 3200009,
		/// <summary>
		/// 发布师门任务失败,未领取
		/// </summary>
		[AdvancedInspector.Descriptor("发布师门任务失败,未领取", "发布师门任务失败,未领取")]
		MASTER_PUBlISHTASK_FAIL_UNREIEVE = 3200010,
		/// <summary>
		/// 获取徒弟师门任务数据失败 
		/// </summary>
		[AdvancedInspector.Descriptor("获取徒弟师门任务数据失败 ", "获取徒弟师门任务数据失败 ")]
		GETDISCIPLE_MASTERTASKS_FAIL = 3200011,
		/// <summary>
		/// 汇报师门任务失败
		/// </summary>
		[AdvancedInspector.Descriptor("汇报师门任务失败", "汇报师门任务失败")]
		MASTER_DAILYTASK_REPORTFAIIL = 3200012,
		/// <summary>
		/// 汇报师门任务失败，师门任务未完成
		/// </summary>
		[AdvancedInspector.Descriptor("汇报师门任务失败，师门任务未完成", "汇报师门任务失败，师门任务未完成")]
		REPORT_DAILYTASK_FAIL_NOTCOMPLETE = 3200013,
		/// <summary>
		/// 领取师门日常任务奖励失败，任务状态不对
		/// </summary>
		[AdvancedInspector.Descriptor("领取师门日常任务奖励失败，任务状态不对", "领取师门日常任务奖励失败，任务状态不对")]
		RECEIEVE_MASTER_DAILYTASK_REWARD_STATEFIAL = 3200014,
		/// <summary>
		/// 没找到师傅
		/// </summary>
		[AdvancedInspector.Descriptor("没找到师傅", "没找到师傅")]
		MASTERSYS_NOTFIND_MASTER = 3200015,
		/// <summary>
		/// 没找到徒弟
		/// </summary>
		[AdvancedInspector.Descriptor("没找到徒弟", "没找到徒弟")]
		MASTERSYS_NOTFIND_DISICIPLE = 3200016,
		/// <summary>
		/// 没找到玩家
		/// </summary>
		[AdvancedInspector.Descriptor("没找到玩家", "没找到玩家")]
		MASTERSYS_NOTFIND_PLAYER = 3200017,
		/// <summary>
		/// 领取礼包失败
		/// </summary>
		[AdvancedInspector.Descriptor("领取礼包失败", "领取礼包失败")]
		MASTERSYS_RECEIEVEGIFT_FAIL = 3200018,
		/// <summary>
		/// 领取礼包失败,成长值不足
		/// </summary>
		[AdvancedInspector.Descriptor("领取礼包失败,成长值不足", "领取礼包失败,成长值不足")]
		MASTERSYS_RECEIEVEGIFT_GROWTHFAIL = 3200019,
		/// <summary>
		/// 领取礼包失败,已经领取过
		/// </summary>
		[AdvancedInspector.Descriptor("领取礼包失败,已经领取过", "领取礼包失败,已经领取过")]
		MASTERSYS_RECEIEVEGIFT_RECVED = 3200020,
		/// <summary>
		/// 自动出师失败,对方在线
		/// </summary>
		[AdvancedInspector.Descriptor("自动出师失败,对方在线", "自动出师失败,对方在线")]
		MASTERSYS_AUTOFINSCHFAIL_TATGETONLINE = 3200021,
		/// <summary>
		/// 自动出师失败,对方离线时间不够
		/// </summary>
		[AdvancedInspector.Descriptor("自动出师失败,对方离线时间不够", "自动出师失败,对方离线时间不够")]
		MASTERSYS_AUTOFINSCHFAIL_TATGETOFFLINETIME = 3200022,
		/// <summary>
		/// 设置备注失败，新旧备注一样
		/// </summary>
		[AdvancedInspector.Descriptor("设置备注失败，新旧备注一样", "设置备注失败，新旧备注一样")]
		RELATION_SETREMARK_UNCHANGE_ERROR = 3200023,
		/// <summary>
		/// 好友备注长度错误
		/// </summary>
		[AdvancedInspector.Descriptor("好友备注长度错误", "好友备注长度错误")]
		RELATION_SETREMARK_LEN_ERROR = 3200024,
		/// <summary>
		/// 问卷设置宣言长度错误
		/// </summary>
		[AdvancedInspector.Descriptor("问卷设置宣言长度错误", "问卷设置宣言长度错误")]
		MASTERSYS_SET_QUEST_DECL_LEN_ERROR = 3200025,
		/// <summary>
		/// 问卷设置宣言有屏蔽字
		/// </summary>
		[AdvancedInspector.Descriptor("问卷设置宣言有屏蔽字", "问卷设置宣言有屏蔽字")]
		MASTERSYS_SET_QUEST_DECL_FORBID_ERROR = 3200026,
		/// <summary>
		/// 对方不在线，不能赠送礼物
		/// </summary>
		[AdvancedInspector.Descriptor("对方不在线，不能赠送礼物", "对方不在线，不能赠送礼物")]
		RELATION_PRESENTGIVE_OFFLINE = 3200027,
		/// <summary>
		/// 对方不是你的好友
		/// </summary>
		[AdvancedInspector.Descriptor("对方不是你的好友", "对方不是你的好友")]
		RELATION_ERR_NOTFRIEND = 3200028,
		/// <summary>
		/// 你不是对方好友,不能赠送
		/// </summary>
		[AdvancedInspector.Descriptor("你不是对方好友,不能赠送", "你不是对方好友,不能赠送")]
		RELATION_PRESENTGIVE_SELFNOTFRIEND = 3200029,
		/// <summary>
		/// 好友赠礼已发送过
		/// </summary>
		[AdvancedInspector.Descriptor("好友赠礼已发送过", "好友赠礼已发送过")]
		RELATION_PRESENTGIVE_ALREADYSEND = 3200030,
		/// <summary>
		/// 好友备注名字不合法
		/// </summary>
		[AdvancedInspector.Descriptor("好友备注名字不合法", "好友备注名字不合法")]
		RELATION_SETREMARK_UNVALID_NAME = 3200031,
		/// <summary>
		///  无法参数预选赛，已经输了
		/// </summary>
		[AdvancedInspector.Descriptor(" 无法参数预选赛，已经输了", " 无法参数预选赛，已经输了")]
		PREMIUM_LEAGUE_PRELIMINAY_ALREADY_LOSE = 3302003,
		/// <summary>
		///  没有房间可以快速加入
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有房间可以快速加入", " 没有房间可以快速加入")]
		ROOM_SYSTEM_QUIT_JOIN_ROOM_NOT_EXIST = 3401020,
		/// <summary>
		///  房间密码不能为空
		/// </summary>
		[AdvancedInspector.Descriptor(" 房间密码不能为空", " 房间密码不能为空")]
		ROOM_SYSTEM_PASSWORD_NOT_EMPTY = 3401025,
		/// <summary>
		///  没有匹配房间可以快速加入
		/// </summary>
		[AdvancedInspector.Descriptor(" 没有匹配房间可以快速加入", " 没有匹配房间可以快速加入")]
		ROOM_SYSTEM_QUIT_JOIN_MATCH_NOT_EXIST = 3401026,
		/// <summary>
		///  玩家拒绝邀请
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家拒绝邀请", " 玩家拒绝邀请")]
		ROOM_SYSTEM_BE_INVITE_REFUSE = 3403007,
		/// <summary>
		///  不在押注阶段
		/// </summary>
		[AdvancedInspector.Descriptor(" 不在押注阶段", " 不在押注阶段")]
		BET_HORSE_NOT_STAKE_PHASE = 3800001,
		/// <summary>
		///  押注数量达到上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 押注数量达到上限", " 押注数量达到上限")]
		BET_HORSE_STAKE_MAX = 3800002,
		/// <summary>
		///  押注失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 押注失败", " 押注失败")]
		BET_HORSE_STAKE_FAILED = 3800003,
		/// <summary>
		///  射手未参赛
		/// </summary>
		[AdvancedInspector.Descriptor(" 射手未参赛", " 射手未参赛")]
		BET_HORSE_SHOOTER_NOT_JOIN = 3800004,
		/// <summary>
		///  安全锁操作失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 安全锁操作失败", " 安全锁操作失败")]
		SECURITY_LOCK_FAILED = 3900001,
		/// <summary>
		///  安全锁解锁密码错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 安全锁解锁密码错误", " 安全锁解锁密码错误")]
		SECURITY_LOCK_PASSWD_ERROR = 3900002,
		/// <summary>
		///  绑定设备数量达上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 绑定设备数量达上限", " 绑定设备数量达上限")]
		BIND_DEVICE_MAX = 3900003,
		/// <summary>
		///  安全锁禁止操作
		/// </summary>
		[AdvancedInspector.Descriptor(" 安全锁禁止操作", " 安全锁禁止操作")]
		SECURITY_LOCK_FORBID_OP = 3900004,
		/// <summary>
		///  删除角色安全锁锁住
		/// </summary>
		[AdvancedInspector.Descriptor(" 删除角色安全锁锁住", " 删除角色安全锁锁住")]
		SECURITY_LOCK_DEL_ROLE = 3900005,
		/// <summary>
		///  密码错误次数达到最大
		/// </summary>
		[AdvancedInspector.Descriptor(" 密码错误次数达到最大", " 密码错误次数达到最大")]
		SECURITY_LOCK_PASSWD_ERROR_MAX_NUM = 3900006,
		/// <summary>
		///  安全锁修改密码错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 安全锁修改密码错误", " 安全锁修改密码错误")]
		SECURITY_LOCK_CHANGE_PASSWD_ERROR = 3900007,
		/// <summary>
		///  安全锁密码长度错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 安全锁密码长度错误", " 安全锁密码长度错误")]
		SECURITY_LOCK_PASSWD_LENGTH_ERROR = 3900008,
		/// <summary>
		///  百变换装节时装合成失败，活动未开启
		/// </summary>
		[AdvancedInspector.Descriptor(" 百变换装节时装合成失败，活动未开启", " 百变换装节时装合成失败，活动未开启")]
		ITEM_FASHCOM_CHANGEACTIV_NOTOPEN = 1000084,
		/// <summary>
		///  百变换装节时装合成失败，换装卷不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 百变换装节时装合成失败，换装卷不足", " 百变换装节时装合成失败，换装卷不足")]
		ITEM_FASHCOM_CHANGEACTIV_TICKETNO = 1000085,
		/// <summary>
		///  装备未找到镶嵌孔
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备未找到镶嵌孔", " 装备未找到镶嵌孔")]
		ITEM_PREABEAD_HOLE_NOTFINND = 1000086,
		/// <summary>
		///  宝珠镶嵌,装备已经镶嵌改类型宝珠
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝珠镶嵌,装备已经镶嵌改类型宝珠", " 宝珠镶嵌,装备已经镶嵌改类型宝珠")]
		ITEM_PREABEAD_EQ_PBTYPE_HAVEED = 1000087,
		/// <summary>
		///  宝珠摘除失败,孔里没有宝珠
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝珠摘除失败,孔里没有宝珠", " 宝珠摘除失败,孔里没有宝珠")]
		ITEM_PREABEAD_HOLE_NOHAVEED = 1000088,
		/// <summary>
		///  宝珠摘除失败,材料不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝珠摘除失败,材料不足", " 宝珠摘除失败,材料不足")]
		ITEM_PREABEAD_MATERIAL_NOENOUGH = 1000089,
		/// <summary>
		///  宝珠摘除失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝珠摘除失败", " 宝珠摘除失败")]
		ITEM_PREABEAD_EXTRIPE_FAIL = 1000090,
		/// <summary>
		///  宝珠镶嵌失败，类型不对
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝珠镶嵌失败，类型不对", " 宝珠镶嵌失败，类型不对")]
		ITEM_PREABEAD_MOUNTFAIL_TYPEERR = 1000091,
		/// <summary>
		///  宝珠升级失败，消耗材料不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝珠升级失败，消耗材料不足", " 宝珠升级失败，消耗材料不足")]
		ITEM_PREABEAD_UPLEVFAIL_MATER_INSUFF = 1000092,
		/// <summary>
		///  冒险队队名非法
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险队队名非法", " 冒险队队名非法")]
		ADVENTURE_TEAM_NAME_INVALID = 4000001,
		/// <summary>
		///  冒险队队名相同
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险队队名相同", " 冒险队队名相同")]
		ADVENTURE_TEAM_NAME_SAME = 4000002,
		/// <summary>
		///  冒险队队名已存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险队队名已存在", " 冒险队队名已存在")]
		ADVENTURE_TEAM_NAME_EXIST = 4000003,
		/// <summary>
		///  冒险队改名失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险队改名失败", " 冒险队改名失败")]
		ADVENTURE_TEAM_RENAME_FAILED = 4000004,
		/// <summary>
		///  赐福水晶不足
		/// </summary>
		[AdvancedInspector.Descriptor(" 赐福水晶不足", " 赐福水晶不足")]
		BLESS_CRYSTAL_NOT_ENOUGH = 4000005,
		/// <summary>
		///  传承祝福使用错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 传承祝福使用错误", " 传承祝福使用错误")]
		INHERIT_BLESS_USE_ERROR = 4000006,
		/// <summary>
		///  远征地图错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征地图错误", " 远征地图错误")]
		EXPEDITION_MAP_ERROR = 4000007,
		/// <summary>
		///  远征角色等级限制
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征角色等级限制", " 远征角色等级限制")]
		EXPEDITION_ROLE_LEVEL_ERR = 4000008,
		/// <summary>
		///  冒险队等级限制
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险队等级限制", " 冒险队等级限制")]
		ADVENTURE_TEAM_LEVEL_LIMIT = 4000009,
		/// <summary>
		///  远征队成员数错误
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征队成员数错误", " 远征队成员数错误")]
		EXPEDITION_MEMBER_NUM_ERR = 4000010,
		/// <summary>
		///  远征时间错误	
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征时间错误	", " 远征时间错误	")]
		EXPEDITION_DURATION_ERROR = 4000011,
		/// <summary>
		///  派遣远征队失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 派遣远征队失败", " 派遣远征队失败")]
		EXPEDITION_TEAM_DISPATCH_FAILED = 4000012,
		/// <summary>
		///  远征队成员不存在
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征队成员不存在", " 远征队成员不存在")]
		EXPEDITION_TEAM_MEMBER_INEXISTENCE = 4000013,
		/// <summary>
		///  取消远征失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 取消远征失败", " 取消远征失败")]
		EXPEDITION_CANCEL_FAILED = 4000014,
		/// <summary>
		///  不存在远征
		/// </summary>
		[AdvancedInspector.Descriptor(" 不存在远征", " 不存在远征")]
		EXPEDITION_INEXISTENCE = 4000015,
		/// <summary>
		///  远征还在进行中
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征还在进行中", " 远征还在进行中")]
		EXPEDITION_STILL_PROCESS = 4000016,
		/// <summary>
		///  领取远征奖励失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 领取远征奖励失败", " 领取远征奖励失败")]
		EXPEDITION_TAKE_REWARD_FAILED = 4000017,
		/// <summary>
		///  无法对镶嵌了宝珠的装备进行该操作
		/// </summary>
		[AdvancedInspector.Descriptor(" 无法对镶嵌了宝珠的装备进行该操作", " 无法对镶嵌了宝珠的装备进行该操作")]
		ITEM_PREABEAD_OPEAR_FAIL_MOUNTEDBEAD = 1000093,
		/// <summary>
		///  无法放入镶嵌了宝珠的装备
		/// </summary>
		[AdvancedInspector.Descriptor(" 无法放入镶嵌了宝珠的装备", " 无法放入镶嵌了宝珠的装备")]
		ITEM_PREABEAD_PUSHFAIL_MOUNTEDBEAD = 1000094,
		/// <summary>
		///  租赁物品,操作非法
		/// </summary>
		[AdvancedInspector.Descriptor(" 租赁物品,操作非法", " 租赁物品,操作非法")]
		ITEM_IN_LEASING = 1000095,
		/// <summary>
		/// 商品租赁中
		/// </summary>
		[AdvancedInspector.Descriptor("商品租赁中", "商品租赁中")]
		SHOP_IN_LEASEING = 1100018,
		/// <summary>
		/// 使用冒险队改名卡失败
		/// </summary>
		[AdvancedInspector.Descriptor("使用冒险队改名卡失败", "使用冒险队改名卡失败")]
		ITEM_USE_ADVENTURE_TEAM_RENAME_FAILED = 1000096,
		/// <summary>
		/// 使用角色栏位扩展卡失败
		/// </summary>
		[AdvancedInspector.Descriptor("使用角色栏位扩展卡失败", "使用角色栏位扩展卡失败")]
		ITEM_USE_EXTENSIBLE_ROLE_FIELD_UNLOCK_FAILED = 1000097,
		/// <summary>
		/// 拍卖行交易冷却中,操作非法
		/// </summary>
		[AdvancedInspector.Descriptor("拍卖行交易冷却中,操作非法", "拍卖行交易冷却中,操作非法")]
		ITEM_IN_AUCTIONCOOL_OPFAIL = 1000098,
		/// <summary>
		/// 宝珠摘取次数不足
		/// </summary>
		[AdvancedInspector.Descriptor("宝珠摘取次数不足", "宝珠摘取次数不足")]
		ITEM_PREABEAD_EXTRIPE_CNT_NOT_ENOUGH = 1000099,
		/// <summary>
		/// 赐福水晶不足
		/// </summary>
		[AdvancedInspector.Descriptor("赐福水晶不足", "赐福水晶不足")]
		SHOP_BUY_BLESS_CRYSTAL_NOT_ENOUGH = 1100019,
		/// <summary>
		/// 商品已下架
		/// </summary>
		[AdvancedInspector.Descriptor("商品已下架", "商品已下架")]
		SHOP_ITEM_IS_OFF_SALE = 1100020,
		/// <summary>
		/// 地精纪念币不足
		/// </summary>
		[AdvancedInspector.Descriptor("地精纪念币不足", "地精纪念币不足")]
		SHOP_BUY_GNOME_COIN_NOT_ENOUGH = 1100021,
		/// <summary>
		/// 赏金不足
		/// </summary>
		[AdvancedInspector.Descriptor("赏金不足", "赏金不足")]
		SHOP_BUY_BOUNTY_NOT_ENOUGH = 1100022,
		/// <summary>
		/// 神器罐子活动关闭
		/// </summary>
		[AdvancedInspector.Descriptor("神器罐子活动关闭", "神器罐子活动关闭")]
		ITEM_ARTIFACT_JAR_ACTIVITY_CLOSE = 1000104,
		/// <summary>
		/// 神器罐子折扣抽取失败
		/// </summary>
		[AdvancedInspector.Descriptor("神器罐子折扣抽取失败", "神器罐子折扣抽取失败")]
		ITEM_ARTIFACT_JAR_DISCOUNT_EXTRACT_FAILE = 1000105,
		/// <summary>
		/// 拍卖行关注数量满额
		/// </summary>
		[AdvancedInspector.Descriptor("拍卖行关注数量满额", "拍卖行关注数量满额")]
		AUCTION_ATTENT_FULL = 2700005,
		/// <summary>
		/// 拍卖行物品已经下架或售出
		/// </summary>
		[AdvancedInspector.Descriptor("拍卖行物品已经下架或售出", "拍卖行物品已经下架或售出")]
		AUCTION_OFFSALE = 2700006,
		/// <summary>
		/// 时装锁,上锁的时装不能用来合成时装
		/// </summary>
		[AdvancedInspector.Descriptor("时装锁,上锁的时装不能用来合成时装", "时装锁,上锁的时装不能用来合成时装")]
		ITEM_FASH_CMPS_FAIL_ITEMLOCK = 1000107,
		/// <summary>
		/// 黑市商人
		/// </summary>
		/// <summary>
		/// 活动已经结束
		/// </summary>
		[AdvancedInspector.Descriptor("活动已经结束", "活动已经结束")]
		BLACK_MARKET_OVERED = 4100001,
		/// <summary>
		/// 申请交易失败
		/// </summary>
		[AdvancedInspector.Descriptor("申请交易失败", "申请交易失败")]
		BLACK_MARKET_TRANS_REQ_FAIL = 4100002,
		/// <summary>
		/// 取消交易失败
		/// </summary>
		[AdvancedInspector.Descriptor("取消交易失败", "取消交易失败")]
		BLACK_MARKET_TRANS_CANCEL_FAIL = 4100003,
		/// <summary>
		/// 申请交易价格不在有效范围
		/// </summary>
		[AdvancedInspector.Descriptor("申请交易价格不在有效范围", "申请交易价格不在有效范围")]
		BLACK_MARKET_TRANS_REQ_FAIL_PRICE = 4100004,
		/// <summary>
		/// 玩家等级不够
		/// </summary>
		[AdvancedInspector.Descriptor("玩家等级不够", "玩家等级不够")]
		BLACK_MARKET_PLAYE_LV_FAIL = 4100005,
		/// <summary>
		///  不满足进入周常深渊前置关卡条件（前置关卡任务）
		/// </summary>
		[AdvancedInspector.Descriptor(" 不满足进入周常深渊前置关卡条件（前置关卡任务）", " 不满足进入周常深渊前置关卡条件（前置关卡任务）")]
		DUNGEON_START_CONDITION_WEEK = 900032,
		/// <summary>
		///  只有角色绑定的称号才能镶嵌宝珠
		/// </summary>
		[AdvancedInspector.Descriptor(" 只有角色绑定的称号才能镶嵌宝珠", " 只有角色绑定的称号才能镶嵌宝珠")]
		ITEM_MOUNT_TITLE_BEAD_FAIL = 1000109,
		/// <summary>
		///  时装锁禁止操作
		/// </summary>
		[AdvancedInspector.Descriptor(" 时装锁禁止操作", " 时装锁禁止操作")]
		ITEM_FASHION_LOCK_FORBID_OP = 1000110,
		/// <summary>
		///  头衔
		/// </summary>
		/// <summary>
		///  穿戴失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 穿戴失败", " 穿戴失败")]
		NEW_TITLE_TAKE_UP_FAIL = 4200001,
		/// <summary>
		///  脱掉失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 脱掉失败", " 脱掉失败")]
		NEW_TITLE_TAKE_OFF_FAIL = 4200002,
		/// <summary>
		/// 附魔卡
		/// </summary>
		/// <summary>
		/// 附魔卡升级失败,概率上失败
		/// </summary>
		[AdvancedInspector.Descriptor("附魔卡升级失败,概率上失败", "附魔卡升级失败,概率上失败")]
		ITEM_MAGIC_UP_FAIL = 1000111,
		/// <summary>
		/// 附魔卡不能升级
		/// </summary>
		[AdvancedInspector.Descriptor("附魔卡不能升级", "附魔卡不能升级")]
		ITEM_MAGIC_CANNOT_UP = 1000112,
		/// <summary>
		/// 附魔卡不能升级失败,已满级
		/// </summary>
		[AdvancedInspector.Descriptor("附魔卡不能升级失败,已满级", "附魔卡不能升级失败,已满级")]
		ITEM_MAGIC_UP_FAIL_MAXLEV = 1000113,
		/// <summary>
		/// 附魔卡不能升级失败,副卡类型不对
		/// </summary>
		[AdvancedInspector.Descriptor("附魔卡不能升级失败,副卡类型不对", "附魔卡不能升级失败,副卡类型不对")]
		ITEM_MAGIC_UP_FAIL_MATERCARD_TYPE = 1000114,
		/// <summary>
		/// 附魔卡不能升级失败,副卡数量不够
		/// </summary>
		[AdvancedInspector.Descriptor("附魔卡不能升级失败,副卡数量不够", "附魔卡不能升级失败,副卡数量不够")]
		ITEM_MAGIC_UP_FAIL_MATERCARD_NUM = 1000115,
		/// <summary>
		/// 附魔卡不能升级失败,货币数量不够
		/// </summary>
		[AdvancedInspector.Descriptor("附魔卡不能升级失败,货币数量不够", "附魔卡不能升级失败,货币数量不够")]
		ITEM_MAGIC_UP_FAIL_MONEY = 1000116,
		/// <summary>
		/// 附魔卡一键合成失败,道具不足 
		/// </summary>
		[AdvancedInspector.Descriptor("附魔卡一键合成失败,道具不足 ", "附魔卡一键合成失败,道具不足 ")]
		ITEM_MAGIC_COMP_ONEKEY_FAIL_NOITEMS = 1000117,
		/// <summary>
		/// 自然月签到
		/// </summary>
		/// <summary>
		/// 已经签到了
		/// </summary>
		[AdvancedInspector.Descriptor("已经签到了", "已经签到了")]
		ALREADY_SIGN = 4500001,
		/// <summary>
		/// 已达到当月补签上限
		/// </summary>
		[AdvancedInspector.Descriptor("已达到当月补签上限", "已达到当月补签上限")]
		REACH_LIMIT = 4500002,
		/// <summary>
		/// 补签材料不足
		/// </summary>
		[AdvancedInspector.Descriptor("补签材料不足", "补签材料不足")]
		NOT_ENOUGH_STUFF = 4500003,
		/// <summary>
		/// 累计签到奖励已领取
		/// </summary>
		[AdvancedInspector.Descriptor("累计签到奖励已领取", "累计签到奖励已领取")]
		ALREADY_COLLECT = 4500004,
		/// <summary>
		/// 累计签到天数不够
		/// </summary>
		[AdvancedInspector.Descriptor("累计签到天数不够", "累计签到天数不够")]
		NOT_ENOUGH_DAYS = 4500005,
	}

}
