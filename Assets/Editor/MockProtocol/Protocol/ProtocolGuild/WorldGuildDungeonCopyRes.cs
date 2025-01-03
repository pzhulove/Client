using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会地下城副本信息返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会地下城副本信息返回", " 公会地下城副本信息返回")]
	public class WorldGuildDungeonCopyRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608506;
		public UInt32 Sequence;
		/// <summary>
		///  战斗记录
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗记录", " 战斗记录")]
		public GuildDungeonBattleRecord[] battleRecord = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battleRecord.Length);
			for(int i = 0; i < battleRecord.Length; i++)
			{
				battleRecord[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 battleRecordCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref battleRecordCnt);
			battleRecord = new GuildDungeonBattleRecord[battleRecordCnt];
			for(int i = 0; i < battleRecord.Length; i++)
			{
				battleRecord[i] = new GuildDungeonBattleRecord();
				battleRecord[i].decode(buffer, ref pos_);
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
