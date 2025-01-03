using System;
using System.Text;

namespace Mock.Protocol
{

	public enum DigType
	{
		/// <summary>
		/// 无效
		/// </summary>
		[AdvancedInspector.Descriptor("无效", "无效")]
		DIG_INVALID = 0,
		/// <summary>
		/// 金挖宝点
		/// </summary>
		[AdvancedInspector.Descriptor("金挖宝点", "金挖宝点")]
		DIG_GLOD = 1,
		/// <summary>
		/// 银挖宝点
		/// </summary>
		[AdvancedInspector.Descriptor("银挖宝点", "银挖宝点")]
		DIG_SILVER = 2,
	}


	public enum DigStatus
	{
		/// <summary>
		/// 无效
		/// </summary>
		[AdvancedInspector.Descriptor("无效", "无效")]
		DIG_STATUS_INVALID = 0,
		/// <summary>
		/// 初始状态
		/// </summary>
		[AdvancedInspector.Descriptor("初始状态", "初始状态")]
		DIG_STATUS_INIT = 1,
		/// <summary>
		/// 打开状态
		/// </summary>
		[AdvancedInspector.Descriptor("打开状态", "打开状态")]
		DIG_STATUS_OPEN = 2,
	}

}
