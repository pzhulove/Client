using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  与NPC交易请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 与NPC交易请求", " 与NPC交易请求")]
	public class SceneBattleNpcTradeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508930;
		public UInt32 Sequence;

		public UInt64 npcGuid;

		public UInt32 npcId;

		public UInt64[] paramVec = new UInt64[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, npcGuid);
			BaseDLL.encode_uint32(buffer, ref pos_, npcId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)paramVec.Length);
			for(int i = 0; i < paramVec.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, paramVec[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref npcGuid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref npcId);
			UInt16 paramVecCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref paramVecCnt);
			paramVec = new UInt64[paramVecCnt];
			for(int i = 0; i < paramVec.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref paramVec[i]);
			}
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
