using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  server->client 请求房间交换位置返回
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client 请求房间交换位置返回", " server->client 请求房间交换位置返回")]
	public class WorldRoomSwapSlotRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607832;
		public UInt32 Sequence;

		public UInt32 result;

		public UInt64 playerId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
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
