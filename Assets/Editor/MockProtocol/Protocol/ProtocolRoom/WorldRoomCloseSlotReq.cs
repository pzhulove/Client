using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 请求关闭打开位置
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server 请求关闭打开位置", " client->server 请求关闭打开位置")]
	public class WorldRoomCloseSlotReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607829;
		public UInt32 Sequence;
		/// <summary>
		/// 队伍
		/// </summary>
		[AdvancedInspector.Descriptor("队伍", "队伍")]
		public byte slotGroup;
		/// <summary>
		/// 位置
		/// </summary>
		[AdvancedInspector.Descriptor("位置", "位置")]
		public byte index;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
			BaseDLL.encode_int8(buffer, ref pos_, index);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
			BaseDLL.decode_int8(buffer, ref pos_, ref index);
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
