using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client  同步师徒拜师收徒惩罚时间戳
	/// </summary>
	[AdvancedInspector.Descriptor("world->client  同步师徒拜师收徒惩罚时间戳", "world->client  同步师徒拜师收徒惩罚时间戳")]
	public class WorldSyncMasterDisciplePunishTime : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601751;
		public UInt32 Sequence;

		public UInt64 apprenticMasterPunishTime;

		public UInt64 recruitDisciplePunishTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, apprenticMasterPunishTime);
			BaseDLL.encode_uint64(buffer, ref pos_, recruitDisciplePunishTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref apprenticMasterPunishTime);
			BaseDLL.decode_uint64(buffer, ref pos_, ref recruitDisciplePunishTime);
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
