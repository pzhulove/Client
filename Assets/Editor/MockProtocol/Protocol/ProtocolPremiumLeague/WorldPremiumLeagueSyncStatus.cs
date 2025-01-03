using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步赏金联赛状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步赏金联赛状态", " 同步赏金联赛状态")]
	public class WorldPremiumLeagueSyncStatus : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607701;
		public UInt32 Sequence;

		public PremiumLeagueStatusInfo info = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			info.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			info.decode(buffer, ref pos_);
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
