using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->battle 迷失地牢战场PK请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->battle 迷失地牢战场PK请求", " client->battle 迷失地牢战场PK请求")]
	public class SceneLostDungeonPkReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510011;
		public UInt32 Sequence;

		public UInt64 dstId;

		public UInt32 battleID;

		public UInt32 dungeonID;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, dstId);
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonID);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonID);
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
