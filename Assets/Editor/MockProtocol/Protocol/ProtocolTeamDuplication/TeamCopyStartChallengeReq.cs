using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  开始挑战
	/// </summary>
	[AdvancedInspector.Descriptor(" 开始挑战", " 开始挑战")]
	public class TeamCopyStartChallengeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100027;
		public UInt32 Sequence;
		/// <summary>
		///  据点id
		/// </summary>
		[AdvancedInspector.Descriptor(" 据点id", " 据点id")]
		public UInt32 fieldId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, fieldId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref fieldId);
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