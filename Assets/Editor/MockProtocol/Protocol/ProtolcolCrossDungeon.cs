using System;
using System.Text;

namespace Mock.Protocol
{

	public enum CrossDungeonEndReason
	{

		CDER_INVALID = 0,

		CDER_TEAMCOPY_FILED_DESTORY = 1,
		/// <summary>
		/// 团本据点被歼灭
		/// </summary>
		[AdvancedInspector.Descriptor("团本据点被歼灭", "团本据点被歼灭")]
		CDER_TEAMCOPY_SETTLE = 2,
	}

}
