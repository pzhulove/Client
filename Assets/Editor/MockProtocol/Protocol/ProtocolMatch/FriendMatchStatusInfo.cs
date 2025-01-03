using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  好友状态信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 好友状态信息", " 好友状态信息")]
	public class FriendMatchStatusInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 roleId;
		/// <summary>
		///  状态，对应枚举（FriendMatchStatus）
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态，对应枚举（FriendMatchStatus）", " 状态，对应枚举（FriendMatchStatus）")]
		public byte status;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_int8(buffer, ref pos_, status);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
		}


		#endregion

	}

}
