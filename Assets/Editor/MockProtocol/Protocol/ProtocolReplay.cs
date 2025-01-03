using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  录像评价
	/// </summary>
	[AdvancedInspector.Descriptor(" 录像评价", " 录像评价")]
	public enum ReplayEvaluation
	{

		Invalid = 0,
	}

	/// <summary>
	///  录像列表类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 录像列表类型", " 录像列表类型")]
	public enum ReplayListType
	{
		/// <summary>
		///  无效值
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效值", " 无效值")]
		Invalid = 0,
		/// <summary>
		///  自己对战记录
		/// </summary>
		[AdvancedInspector.Descriptor(" 自己对战记录", " 自己对战记录")]
		Self = 1,
		/// <summary>
		///  高手对决
		/// </summary>
		[AdvancedInspector.Descriptor(" 高手对决", " 高手对决")]
		Master = 2,
		/// <summary>
		///  收藏
		/// </summary>
		[AdvancedInspector.Descriptor(" 收藏", " 收藏")]
		Collection = 3,
	}

	/// <summary>
	///  PK结果
	/// </summary>
	[AdvancedInspector.Descriptor(" PK结果", " PK结果")]
	public enum PkRaceResult
	{
		/// <summary>
		///  无效
		/// </summary>
		[AdvancedInspector.Descriptor(" 无效", " 无效")]
		Invalid = 0,
		/// <summary>
		///  胜利
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜利", " 胜利")]
		Win = 1,
		/// <summary>
		///  失败
		/// </summary>
		[AdvancedInspector.Descriptor(" 失败", " 失败")]
		Lose = 2,
		/// <summary>
		///  平局
		/// </summary>
		[AdvancedInspector.Descriptor(" 平局", " 平局")]
		Dogfall = 3,
	}

}
