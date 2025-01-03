using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneBattleThrowSomeoneItemReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508920;
		public UInt32 Sequence;

		public UInt32 battleID;

		public UInt64 targetID;

		public UInt64 itemGuid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			BaseDLL.encode_uint64(buffer, ref pos_, targetID);
			BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref targetID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
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
