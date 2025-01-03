using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  与NPC交易返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 与NPC交易返回", " 与NPC交易返回")]
	public class SceneBattleNpcTradeRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508931;
		public UInt32 Sequence;

		public UInt32 retCode;

		public UInt32 npcId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			BaseDLL.encode_uint32(buffer, ref pos_, npcId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			BaseDLL.decode_uint32(buffer, ref pos_, ref npcId);
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
