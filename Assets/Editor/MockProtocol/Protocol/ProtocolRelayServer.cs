using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  单局命令帧类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 单局命令帧类型", " 单局命令帧类型")]
	public enum FrameCommandID
	{
		/// <summary>
		///  战斗开始
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗开始", " 战斗开始")]
		GameStart = 0,
		/// <summary>
		///  移动
		/// </summary>
		[AdvancedInspector.Descriptor(" 移动", " 移动")]
		Move = 1,
		/// <summary>
		///  停止
		/// </summary>
		[AdvancedInspector.Descriptor(" 停止", " 停止")]
		Stop = 2,
		/// <summary>
		///  放技能
		/// </summary>
		[AdvancedInspector.Descriptor(" 放技能", " 放技能")]
		Skill = 3,
		/// <summary>
		///  玩家离开战斗
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家离开战斗", " 玩家离开战斗")]
		Leave = 4,
		/// <summary>
		///  玩家复活
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家复活", " 玩家复活")]
		Reborn = 5,
		/// <summary>
		///  开始重连
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始重连", " 开始重连")]
		ReconnectBegin = 6,
		/// <summary>
		///  重连结束
		/// </summary>
		[AdvancedInspector.Descriptor(" 重连结束", " 重连结束")]
		ReconnectEnd = 7,
		/// <summary>
		///  使用物品
		/// </summary>
		[AdvancedInspector.Descriptor(" 使用物品", " 使用物品")]
		UseItem = 8,
		/// <summary>
		/// 升级
		/// </summary>
		[AdvancedInspector.Descriptor("升级", "升级")]
		LevelChange = 9,
		/// <summary>
		/// 自动战斗
		/// </summary>
		[AdvancedInspector.Descriptor("自动战斗", "自动战斗")]
		AutoFight = 10,
		/// <summary>
		/// 双击配置
		/// </summary>
		[AdvancedInspector.Descriptor("双击配置", "双击配置")]
		DoublePressConfig = 11,
		/// <summary>
		///  玩家退出战斗(真正的退出)
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家退出战斗(真正的退出)", " 玩家退出战斗(真正的退出)")]
		PlayerQuit = 12,
		/// <summary>
		///  战斗结束
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗结束", " 战斗结束")]
		RaceEnd = 13,
		/// <summary>
		///  网络质量
		/// </summary>
		[AdvancedInspector.Descriptor(" 网络质量", " 网络质量")]
		NetQuality = 14,
		/// <summary>
		///  暂停帧，目前给单局使用
		/// </summary>
		[AdvancedInspector.Descriptor(" 暂停帧，目前给单局使用", " 暂停帧，目前给单局使用")]
		RacePause = 15,
		/// <summary>
		///  场景切换的帧，用于死亡之塔的验证服务器
		/// </summary>
		[AdvancedInspector.Descriptor(" 场景切换的帧，用于死亡之塔的验证服务器", " 场景切换的帧，用于死亡之塔的验证服务器")]
		SceneChangeArea = 16,
		/// <summary>
		/// 中断技能
		/// </summary>
		[AdvancedInspector.Descriptor("中断技能", "中断技能")]
		StopSkill = 17,
		/// <summary>
		/// 技能产生攻击
		/// </summary>
		[AdvancedInspector.Descriptor("技能产生攻击", "技能产生攻击")]
		DoAttack = 18,
		/// <summary>
		/// 匹配玩家投票
		/// </summary>
		[AdvancedInspector.Descriptor("匹配玩家投票", "匹配玩家投票")]
		MatchRoundVote = 19,
		/// <summary>
		///  经过传送门
		/// </summary>
		[AdvancedInspector.Descriptor(" 经过传送门", " 经过传送门")]
		PassDoor = 20,
		/// <summary>
		/// 切换武器
		/// </summary>
		[AdvancedInspector.Descriptor("切换武器", "切换武器")]
		ChangeWeapon = 21,
		/// <summary>
		/// 同步镜头
		/// </summary>
		[AdvancedInspector.Descriptor("同步镜头", "同步镜头")]
		SyncSight = 22,
		/// <summary>
		/// boss阶段变化
		/// </summary>
		[AdvancedInspector.Descriptor("boss阶段变化", "boss阶段变化")]
		BossPhaseChange = 23,
		/// <summary>
		/// 地下城被歼灭
		/// </summary>
		[AdvancedInspector.Descriptor("地下城被歼灭", "地下城被歼灭")]
		DungeonDestory = 24,
		/// <summary>
		/// 团本结束比赛
		/// </summary>
		[AdvancedInspector.Descriptor("团本结束比赛", "团本结束比赛")]
		TeamCopyRaceEnd = 25,
		/// <summary>
		/// 团本贝西莫斯之心进度
		/// </summary>
		[AdvancedInspector.Descriptor("团本贝西莫斯之心进度", "团本贝西莫斯之心进度")]
		TeamCopyBimsProgress = 26,
	}

	/// <summary>
	///  比赛结束原因
	/// </summary>
	[AdvancedInspector.Descriptor(" 比赛结束原因", " 比赛结束原因")]
	public enum RaceEndReason
	{
		/// <summary>
		///  正常退出
		/// </summary>
		[AdvancedInspector.Descriptor(" 正常退出", " 正常退出")]
		Normal = 0,
		/// <summary>
		///  对战持续时间超时
		/// </summary>
		[AdvancedInspector.Descriptor(" 对战持续时间超时", " 对战持续时间超时")]
		Timeout = 1,
		/// <summary>
		///  等待开始超时
		/// </summary>
		[AdvancedInspector.Descriptor(" 等待开始超时", " 等待开始超时")]
		LoginTimeout = 2,
		/// <summary>
		///  异常结束
		/// </summary>
		[AdvancedInspector.Descriptor(" 异常结束", " 异常结束")]
		Errro = 3,
		/// <summary>
		///  系统解散
		/// </summary>
		[AdvancedInspector.Descriptor(" 系统解散", " 系统解散")]
		System = 4,
		/// <summary>
		///  等待结束超时
		/// </summary>
		[AdvancedInspector.Descriptor(" 等待结束超时", " 等待结束超时")]
		WaitRaceEndTimeout = 5,
		/// <summary>
		///  由于参战方离线
		/// </summary>
		[AdvancedInspector.Descriptor(" 由于参战方离线", " 由于参战方离线")]
		GamerOffline = 6,
		/// <summary>
		///  帧校验超时
		/// </summary>
		[AdvancedInspector.Descriptor(" 帧校验超时", " 帧校验超时")]
		FrameChecksumTimeout = 7,
		/// <summary>
		///  帧校验不一致
		/// </summary>
		[AdvancedInspector.Descriptor(" 帧校验不一致", " 帧校验不一致")]
		FrameChecksumDifferent = 8,
	}

}
