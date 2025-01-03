using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  创建团队返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 创建团队返回", " 创建团队返回")]
	public class TeamCopyCreateTeamRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100004;
		public UInt32 Sequence;
		/// <summary>
		///  团队Id
		/// </summary>
		[AdvancedInspector.Descriptor(" 团队Id", " 团队Id")]
		public UInt32 teamId;

		public UInt32 retCode;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
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
