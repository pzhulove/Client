using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  自动同意金主
	/// </summary>
	[AdvancedInspector.Descriptor(" 自动同意金主", " 自动同意金主")]
	public class TeamCopyAutoAgreeGoldReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100050;
		public UInt32 Sequence;
		/// <summary>
		///  (0：不同意，1：同意)
		/// </summary>
		[AdvancedInspector.Descriptor(" (0：不同意，1：同意)", " (0：不同意，1：同意)")]
		public UInt32 isAutoAgree;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, isAutoAgree);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref isAutoAgree);
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
