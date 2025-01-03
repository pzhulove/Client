using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 请求响应房间交换位置
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server 请求响应房间交换位置", " client->server 请求响应房间交换位置")]
	public class WorldRoomResponseSwapSlotReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607833;
		public UInt32 Sequence;

		public byte isAccept;

		public byte slotGroup;

		public byte slotIndex;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, isAccept);
			BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
			BaseDLL.encode_int8(buffer, ref pos_, slotIndex);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref isAccept);
			BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
			BaseDLL.decode_int8(buffer, ref pos_, ref slotIndex);
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
