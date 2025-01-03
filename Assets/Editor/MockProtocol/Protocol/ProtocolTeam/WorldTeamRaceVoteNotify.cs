using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知开始地下城投票
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知开始地下城投票", " 通知开始地下城投票")]
	public class WorldTeamRaceVoteNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601642;
		public UInt32 Sequence;
		/// <summary>
		///  地下城ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城ID", " 地下城ID")]
		public UInt32 dungeonId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
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
