using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  处理公会加入请求返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 处理公会加入请求返回", " 处理公会加入请求返回")]
	public class WorldGuildProcessRequesterRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601913;
		public UInt32 Sequence;

		public UInt32 result;
		/// <summary>
		///  新成员信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 新成员信息", " 新成员信息")]
		public GuildMemberEntry entry = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			entry.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			entry.decode(buffer, ref pos_);
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
