using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  换座位
	/// </summary>
	[AdvancedInspector.Descriptor(" 换座位", " 换座位")]
	public class TeamCopyChangeSeatReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100041;
		public UInt32 Sequence;

		public UInt32 srcSeat;

		public UInt32 destSeat;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, srcSeat);
			BaseDLL.encode_uint32(buffer, ref pos_, destSeat);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref srcSeat);
			BaseDLL.decode_uint32(buffer, ref pos_, ref destSeat);
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
