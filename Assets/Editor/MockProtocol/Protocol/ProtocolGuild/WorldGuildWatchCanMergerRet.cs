using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world -> client  查看可兼并的公会列表返回
	/// </summary>
	[AdvancedInspector.Descriptor("world -> client  查看可兼并的公会列表返回", "world -> client  查看可兼并的公会列表返回")]
	public class WorldGuildWatchCanMergerRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601978;
		public UInt32 Sequence;

		public UInt16 start;

		public UInt16 totalNum;

		public GuildEntry[] guilds = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, start);
			BaseDLL.encode_uint16(buffer, ref pos_, totalNum);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guilds.Length);
			for(int i = 0; i < guilds.Length; i++)
			{
				guilds[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint16(buffer, ref pos_, ref start);
			BaseDLL.decode_uint16(buffer, ref pos_, ref totalNum);
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
