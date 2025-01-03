using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  招募
	/// </summary>
	[AdvancedInspector.Descriptor(" 招募", " 招募")]
	public class TeamCopyRecruitReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100060;
		public UInt32 Sequence;
		/// <summary>
		///  队伍模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍模式", " 队伍模式")]
		public UInt32 teamModel;
		/// <summary>
		///  队伍Id
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍Id", " 队伍Id")]
		public UInt32 teamId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
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
