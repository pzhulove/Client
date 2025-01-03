using System;
using System.Text;

namespace Mock.Protocol
{

	public enum RedPacketType
	{
		/// <summary>
		///  公会红包
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会红包", " 公会红包")]
		GUILD = 1,
		/// <summary>
		///  新年红包
		/// </summary>
		[AdvancedInspector.Descriptor(" 新年红包", " 新年红包")]
		NEW_YEAR = 2,
	}

	/// <summary>
	///  红包状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 红包状态", " 红包状态")]
	public enum RedPacketStatus
	{
		/// <summary>
		///  初始状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 初始状态", " 初始状态")]
		INIT = 0,
		/// <summary>
		///  未达成
		/// </summary>
		[AdvancedInspector.Descriptor(" 未达成", " 未达成")]
		UNSATISFY = 1,
		/// <summary>
		///  等待别人领取红包
		/// </summary>
		[AdvancedInspector.Descriptor(" 等待别人领取红包", " 等待别人领取红包")]
		WAIT_RECEIVE = 2,
		/// <summary>
		///  已抢
		/// </summary>
		[AdvancedInspector.Descriptor(" 已抢", " 已抢")]
		RECEIVED = 3,
		/// <summary>
		///  已被领完
		/// </summary>
		[AdvancedInspector.Descriptor(" 已被领完", " 已被领完")]
		EMPTY = 4,
		/// <summary>
		///  可摧毁
		/// </summary>
		[AdvancedInspector.Descriptor(" 可摧毁", " 可摧毁")]
		DESTORY = 5,
	}

}
