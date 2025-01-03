using System;
using System.Text;

namespace Mock.Protocol
{

	public enum WeatherType
	{

		WEATHER_RAIN = 1,
		/// <summary>
		///  雨天
		/// </summary>
		[AdvancedInspector.Descriptor(" 雨天", " 雨天")]
		WEATHER_SUNNY = 2,
		/// <summary>
		///  晴天
		/// </summary>
		[AdvancedInspector.Descriptor(" 晴天", " 晴天")]
		WEATHER_FOGGY = 3,
	}

	/// <summary>
	///  雾天
	/// </summary>
	[AdvancedInspector.Descriptor(" 雾天", " 雾天")]
	public enum ShooterStatusType
	{

		SHOOTER_STATUS_UNKNOWN = 0,
		/// <summary>
		///  未知
		/// </summary>
		[AdvancedInspector.Descriptor(" 未知", " 未知")]
		SHOOTER_STATUS_EXCELLENT = 1,
		/// <summary>
		///  优秀(红色)
		/// </summary>
		[AdvancedInspector.Descriptor(" 优秀(红色)", " 优秀(红色)")]
		SHOOTER_STATUS_GOOD = 2,
		/// <summary>
		///  良好(黄色)
		/// </summary>
		[AdvancedInspector.Descriptor(" 良好(黄色)", " 良好(黄色)")]
		SHOOTER_STATUS_INSTABLE = 3,
		/// <summary>
		///  不稳定(蓝色)
		/// </summary>
		[AdvancedInspector.Descriptor(" 不稳定(蓝色)", " 不稳定(蓝色)")]
		SHOOTER_STATUS_COMMONLY = 4,
	}

	/// <summary>
	///  表现平平(绿色)
	/// </summary>
	[AdvancedInspector.Descriptor(" 表现平平(绿色)", " 表现平平(绿色)")]
	public enum BetHorsePhaseType
	{

		PHASE_TYPE_READY = 1,
		/// <summary>
		///  准备
		/// </summary>
		[AdvancedInspector.Descriptor(" 准备", " 准备")]
		PHASE_TYPE_STAKE = 2,
		/// <summary>
		///  押注阶段(1-90分钟)
		/// </summary>
		[AdvancedInspector.Descriptor(" 押注阶段(1-90分钟)", " 押注阶段(1-90分钟)")]
		PHASE_TYPE_ADJUST = 3,
		/// <summary>
		///  调整阶段(91-100分钟)
		/// </summary>
		[AdvancedInspector.Descriptor(" 调整阶段(91-100分钟)", " 调整阶段(91-100分钟)")]
		PHASE_TYPE_RESULT = 4,
		/// <summary>
		///  结果阶段(101-120分钟)
		/// </summary>
		[AdvancedInspector.Descriptor(" 结果阶段(101-120分钟)", " 结果阶段(101-120分钟)")]
		PHASE_TYPE_DAY_END = 5,
	}

}
