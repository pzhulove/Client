using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  领地战斗记录返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 领地战斗记录返回", " 领地战斗记录返回")]
	public class WorldGuildBattleRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601949;
		public UInt32 Sequence;

		public UInt32 result;

		public GuildBattleRecord[] records = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
			for(int i = 0; i < records.Length; i++)
			{
				records[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			UInt16 recordsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
			records = new GuildBattleRecord[recordsCnt];
			for(int i = 0; i < records.Length; i++)
			{
				records[i] = new GuildBattleRecord();
				records[i].decode(buffer, ref pos_);
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
