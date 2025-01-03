using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知修改红包状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知修改红包状态", " 通知修改红包状态")]
	public class WorldNotifySyncRedPacketStatus : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607305;
		public UInt32 Sequence;
		/// <summary>
		///  id
		/// </summary>
		[AdvancedInspector.Descriptor(" id", " id")]
		public UInt64 id;
		/// <summary>
		///  状态(对应枚举RedPacketStatus)
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态(对应枚举RedPacketStatus)", " 状态(对应枚举RedPacketStatus)")]
		public byte status;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, status);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
