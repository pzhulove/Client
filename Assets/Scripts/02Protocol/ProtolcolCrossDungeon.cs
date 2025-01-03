using System;
using System.Text;

namespace Protocol
{
	public enum CrossDungeonEndReason
	{
		CDER_INVALID = 0,
		CDER_TEAMCOPY_FILED_DESTORY = 1,
		/// <summary>
		/// 团本据点被歼灭
		/// </summary>
		CDER_TEAMCOPY_SETTLE = 2,
	}

}
