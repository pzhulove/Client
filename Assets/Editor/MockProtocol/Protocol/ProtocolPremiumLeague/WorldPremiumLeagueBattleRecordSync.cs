using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ͬ���ͽ�����ս����¼
	/// </summary>
	[AdvancedInspector.Descriptor(" ͬ���ͽ�����ս����¼", " ͬ���ͽ�����ս����¼")]
	public class WorldPremiumLeagueBattleRecordSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607711;
		public UInt32 Sequence;

		public PremiumLeagueRecordEntry record = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			record.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			record.decode(buffer, ref pos_);
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
