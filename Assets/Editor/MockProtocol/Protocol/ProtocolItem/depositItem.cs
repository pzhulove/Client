using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  寄存物品
	/// </summary>
	[AdvancedInspector.Descriptor(" 寄存物品", " 寄存物品")]
	public class depositItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  guid
		/// </summary>
		[AdvancedInspector.Descriptor(" guid", " guid")]
		public UInt64 guid;
		/// <summary>
		///  创建时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 创建时间", " 创建时间")]
		public UInt32 createTime;
		/// <summary>
		///  奖励物品
		/// </summary>
		[AdvancedInspector.Descriptor(" 奖励物品", " 奖励物品")]
		public ItemReward itemReward = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_uint32(buffer, ref pos_, createTime);
			itemReward.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref createTime);
			itemReward.decode(buffer, ref pos_);
		}


		#endregion

	}

}
