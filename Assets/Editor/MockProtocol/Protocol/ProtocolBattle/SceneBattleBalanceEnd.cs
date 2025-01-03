using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneBattleBalanceEnd : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508905;
		public UInt32 Sequence;

		public UInt64 roleId;

		public UInt32 rank;

		public UInt32 playerNum;

		public UInt32 kills;

		public UInt32 survivalTime;

		public UInt32 score;

		public UInt32 battleID;

		public UInt32 getHonor;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint32(buffer, ref pos_, rank);
			BaseDLL.encode_uint32(buffer, ref pos_, playerNum);
			BaseDLL.encode_uint32(buffer, ref pos_, kills);
			BaseDLL.encode_uint32(buffer, ref pos_, survivalTime);
			BaseDLL.encode_uint32(buffer, ref pos_, score);
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref rank);
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref kills);
			BaseDLL.decode_uint32(buffer, ref pos_, ref survivalTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref score);
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
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
