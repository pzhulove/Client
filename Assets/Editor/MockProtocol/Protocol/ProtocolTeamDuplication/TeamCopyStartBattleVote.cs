using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  开战投票
	/// </summary>
	[AdvancedInspector.Descriptor(" 开战投票", " 开战投票")]
	public class TeamCopyStartBattleVote : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100016;
		public UInt32 Sequence;
		/// <summary>
		///  非0同意，0拒绝
		/// </summary>
		[AdvancedInspector.Descriptor(" 非0同意，0拒绝", " 非0同意，0拒绝")]
		public UInt32 vote;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, vote);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref vote);
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
