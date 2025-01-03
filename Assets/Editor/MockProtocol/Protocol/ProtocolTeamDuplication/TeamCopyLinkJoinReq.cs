using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  链接加入
	/// </summary>
	[AdvancedInspector.Descriptor(" 链接加入", " 链接加入")]
	public class TeamCopyLinkJoinReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100062;
		public UInt32 Sequence;
		/// <summary>
		///  队伍Id
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍Id", " 队伍Id")]
		public UInt32 teamId;
		/// <summary>
		///  是否金主(非0是金主)
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否金主(非0是金主)", " 是否金主(非0是金主)")]
		public UInt32 isGold;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			BaseDLL.encode_uint32(buffer, ref pos_, isGold);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref isGold);
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
