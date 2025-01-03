using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  地下城结算
	/// </summary>
	[AdvancedInspector.Descriptor(" 地下城结算", " 地下城结算")]
	public class RelaySvrDungeonRaceEndReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1300008;
		public UInt32 Sequence;

		public DungeonRaceEndInfo raceEndInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			raceEndInfo.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			raceEndInfo.decode(buffer, ref pos_);
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
