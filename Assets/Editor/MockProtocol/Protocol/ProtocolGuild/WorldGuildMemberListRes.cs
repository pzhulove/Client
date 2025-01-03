using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回公会成员列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回公会成员列表", " 返回公会成员列表")]
	public class WorldGuildMemberListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601920;
		public UInt32 Sequence;
		/// <summary>
		///  成员列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 成员列表", " 成员列表")]
		public GuildMemberEntry[] members = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
			for(int i = 0; i < members.Length; i++)
			{
				members[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 membersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
			members = new GuildMemberEntry[membersCnt];
			for(int i = 0; i < members.Length; i++)
			{
				members[i] = new GuildMemberEntry();
				members[i].decode(buffer, ref pos_);
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
