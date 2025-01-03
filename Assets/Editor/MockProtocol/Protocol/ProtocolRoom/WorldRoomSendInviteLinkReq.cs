using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 发送邀请信息
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server 发送邀请信息", " client->server 发送邀请信息")]
	public class WorldRoomSendInviteLinkReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607841;
		public UInt32 Sequence;

		public UInt32 roomId;

		public byte channel;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, roomId);
			BaseDLL.encode_int8(buffer, ref pos_, channel);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
			BaseDLL.decode_int8(buffer, ref pos_, ref channel);
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
