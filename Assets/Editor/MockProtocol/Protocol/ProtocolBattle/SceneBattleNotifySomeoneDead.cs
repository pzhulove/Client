using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneBattleNotifySomeoneDead : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508919;
		public UInt32 Sequence;

		public UInt32 battleID;

		public UInt64 playerID;

		public UInt64 killerID;

		public UInt32 reason;

		public UInt32 kills;

		public UInt32 suvivalNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			BaseDLL.encode_uint64(buffer, ref pos_, playerID);
			BaseDLL.encode_uint64(buffer, ref pos_, killerID);
			BaseDLL.encode_uint32(buffer, ref pos_, reason);
			BaseDLL.encode_uint32(buffer, ref pos_, kills);
			BaseDLL.encode_uint32(buffer, ref pos_, suvivalNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref killerID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref reason);
			BaseDLL.decode_uint32(buffer, ref pos_, ref kills);
			BaseDLL.decode_uint32(buffer, ref pos_, ref suvivalNum);
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
