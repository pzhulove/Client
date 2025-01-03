using System;
using System.Text;

namespace Mock.Protocol
{

	public class BattleRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 708313;
		public UInt32 Sequence;
		/// <summary>
		///  比赛历史记录列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 比赛历史记录列表", " 比赛历史记录列表")]
		public BattleRecord[] BattleRecordList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)BattleRecordList.Length);
			for(int i = 0; i < BattleRecordList.Length; i++)
			{
				BattleRecordList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 BattleRecordListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref BattleRecordListCnt);
			BattleRecordList = new BattleRecord[BattleRecordListCnt];
			for(int i = 0; i < BattleRecordList.Length; i++)
			{
				BattleRecordList[i] = new BattleRecord();
				BattleRecordList[i].decode(buffer, ref pos_);
			}
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
