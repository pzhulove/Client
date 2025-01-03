using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  battle->client 战场PK返回
	/// </summary>
	[AdvancedInspector.Descriptor(" battle->client 战场PK返回", " battle->client 战场PK返回")]
	public class SceneLostDungeonPkRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510012;
		public UInt32 Sequence;

		public UInt32 retCode;

		public UInt64 roleId;

		public UInt64 dstId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint64(buffer, ref pos_, dstId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
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
