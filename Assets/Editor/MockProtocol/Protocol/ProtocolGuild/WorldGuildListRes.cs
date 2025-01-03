using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回公会列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回公会列表", " 返回公会列表")]
	public class WorldGuildListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601908;
		public UInt32 Sequence;
		/// <summary>
		/// 开始位置
		/// </summary>
		[AdvancedInspector.Descriptor("开始位置", "开始位置")]
		public UInt16 start;
		/// <summary>
		/// 总数
		/// </summary>
		[AdvancedInspector.Descriptor("总数", "总数")]
		public UInt16 totalnum;
		/// <summary>
		/// 部落列表
		/// </summary>
		[AdvancedInspector.Descriptor("部落列表", "部落列表")]
		public GuildEntry[] guilds = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, start);
			BaseDLL.encode_uint16(buffer, ref pos_, totalnum);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guilds.Length);
			for(int i = 0; i < guilds.Length; i++)
			{
				guilds[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint16(buffer, ref pos_, ref start);
			BaseDLL.decode_uint16(buffer, ref pos_, ref totalnum);
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
