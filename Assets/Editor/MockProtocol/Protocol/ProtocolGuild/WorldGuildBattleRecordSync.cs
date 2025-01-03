using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  领地战斗记录同步
	/// </summary>
	[AdvancedInspector.Descriptor(" 领地战斗记录同步", " 领地战斗记录同步")]
	public class WorldGuildBattleRecordSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601950;
		public UInt32 Sequence;

		public GuildBattleRecord record = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			record.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			record.decode(buffer, ref pos_);
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
