using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world -> client  返回本公会收到的兼并申请列表
	/// </summary>
	[AdvancedInspector.Descriptor("world -> client  返回本公会收到的兼并申请列表", "world -> client  返回本公会收到的兼并申请列表")]
	public class WorldGuildWatchHavedMergerRequestRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601984;
		public UInt32 Sequence;

		public GuildEntry[] guilds = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guilds.Length);
			for(int i = 0; i < guilds.Length; i++)
			{
				guilds[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 guildsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref guildsCnt);
			guilds = new GuildEntry[guildsCnt];
			for(int i = 0; i < guilds.Length; i++)
			{
				guilds[i] = new GuildEntry();
				guilds[i].decode(buffer, ref pos_);
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
