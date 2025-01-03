using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求公会成员列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求公会成员列表", " 请求公会成员列表")]
	public class WorldGuildMemberListReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601919;
		public UInt32 Sequence;
		/// <summary>
		/// 0为查询本行会成员 别的值为查询别的行会成员，仅供公会兼并使用
		/// </summary>
		[AdvancedInspector.Descriptor("0为查询本行会成员 别的值为查询别的行会成员，仅供公会兼并使用", "0为查询本行会成员 别的值为查询别的行会成员，仅供公会兼并使用")]
		public UInt64 guildID;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guildID);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guildID);
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
