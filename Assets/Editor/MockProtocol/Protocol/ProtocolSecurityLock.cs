using System;
using System.Text;

namespace Mock.Protocol
{

	public enum LockOpType
	{

		LT_LOCK = 1,
		/// <summary>
		///  上锁
		/// </summary>
		[AdvancedInspector.Descriptor(" 上锁", " 上锁")]
		LT_UNLOCK = 2,
		/// <summary>
		///  解锁
		/// </summary>
		[AdvancedInspector.Descriptor(" 解锁", " 解锁")]
		LT_FORCE_UNLOCK = 3,
		/// <summary>
		///  强制解锁
		/// </summary>
		[AdvancedInspector.Descriptor(" 强制解锁", " 强制解锁")]
		LT_CANCAL_APPLY = 4,
	}

	/// <summary>
	///  取消申请
	/// </summary>
	[AdvancedInspector.Descriptor(" 取消申请", " 取消申请")]
	public enum SecurityLockState
	{

		SECURITY_STATE_UNLOCK = 0,
		/// <summary>
		///  没锁
		/// </summary>
		[AdvancedInspector.Descriptor(" 没锁", " 没锁")]
		SECURITY_STATE_LOCK = 1,
		/// <summary>
		///  锁住
		/// </summary>
		[AdvancedInspector.Descriptor(" 锁住", " 锁住")]
		SECURITY_STATE_APPLY = 2,
	}

}
