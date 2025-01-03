using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  远征状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 远征状态", " 远征状态")]
	public enum ExpeditionStatus
	{
		/// <summary>
		///  准备出发
		/// </summary>
		[AdvancedInspector.Descriptor(" 准备出发", " 准备出发")]
		EXPEDITION_STATUS_PREPARE = 0,
		/// <summary>
		///  远征中
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征中", " 远征中")]
		EXPEDITION_STATUS_IN = 1,
		/// <summary>
		///  远征完毕
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征完毕", " 远征完毕")]
		EXPEDITION_STATUS_OVER = 2,
	}

}
