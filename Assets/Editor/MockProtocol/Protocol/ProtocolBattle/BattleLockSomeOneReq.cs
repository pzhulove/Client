using System;
using System.Text;

namespace Mock.Protocol
{

	public class BattleLockSomeOneReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 2200013;
		public UInt32 Sequence;

		public UInt64 roleId;

		public UInt64 dstId;

		public UInt32 battleID;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint64(buffer, ref pos_, dstId);
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
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