using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 请求踢出房间
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server 请求踢出房间", " client->server 请求踢出房间")]
	public class WorldKickOutRoomReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607819;
		public UInt32 Sequence;

		public UInt64 playerId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
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
