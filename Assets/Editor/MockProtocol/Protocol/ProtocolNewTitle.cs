using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 称谓名称类型
	/// </summary>
	[AdvancedInspector.Descriptor("称谓名称类型", "称谓名称类型")]
	public enum NewTitleType
	{

		TLETP_NONE = 0,
		/// <summary>
		/// 固定
		/// </summary>
		[AdvancedInspector.Descriptor("固定", "固定")]
		TLETP_FIXED = 1,
		/// <summary>
		/// 服务器拼接
		/// </summary>
		[AdvancedInspector.Descriptor("服务器拼接", "服务器拼接")]
		TLETP_JOINT = 2,

		TLETP_MAX = 3,
	}

}
