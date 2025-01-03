using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  团队数据请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 团队数据请求", " 团队数据请求")]
	public class TeamCopyTeamDataReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100005;
		public UInt32 Sequence;
		/// <summary>
		///  团队Id
		/// </summary>
		[AdvancedInspector.Descriptor(" 团队Id", " 团队Id")]
		public UInt32 teamId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
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
