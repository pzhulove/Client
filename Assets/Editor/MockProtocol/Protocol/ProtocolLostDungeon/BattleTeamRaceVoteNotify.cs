using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// battle->client 通知开始地下城投票
	/// </summary>
	[AdvancedInspector.Descriptor("battle->client 通知开始地下城投票", "battle->client 通知开始地下城投票")]
	public class BattleTeamRaceVoteNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 2210002;
		public UInt32 Sequence;

		public UInt32 dungeonId;

		public UInt32 teamId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
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
