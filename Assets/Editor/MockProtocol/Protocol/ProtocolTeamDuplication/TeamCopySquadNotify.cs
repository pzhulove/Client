using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  小队通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 小队通知", " 小队通知")]
	public class TeamCopySquadNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100038;
		public UInt32 Sequence;
		/// <summary>
		///  小队id
		/// </summary>
		[AdvancedInspector.Descriptor(" 小队id", " 小队id")]
		public UInt32 squadId;
		/// <summary>
		///  小队状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 小队状态", " 小队状态")]
		public UInt32 squadStatus;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, squadId);
			BaseDLL.encode_uint32(buffer, ref pos_, squadStatus);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref squadId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref squadStatus);
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
