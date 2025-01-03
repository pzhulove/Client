using System;
using System.Text;

namespace Mock.Protocol
{

	public enum RetinueUpType
	{
		/// <summary>
		///  升级
		/// </summary>
		[AdvancedInspector.Descriptor(" 升级", " 升级")]
		RUT_UPLEVEL = 1,
		/// <summary>
		/// 升星
		/// </summary>
		[AdvancedInspector.Descriptor("升星", "升星")]
		RUT_UPSTAR = 2,
	}

}
