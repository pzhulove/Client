using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  周签到活动抽奖记录返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 周签到活动抽奖记录返回", " 周签到活动抽奖记录返回")]
	public class GASWeekSignRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 707402;
		public UInt32 Sequence;
		/// <summary>
		///  活动类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 活动类型", " 活动类型")]
		public UInt32 opActType;
		/// <summary>
		///  抽奖记录
		/// </summary>
		[AdvancedInspector.Descriptor(" 抽奖记录", " 抽奖记录")]
		public WeekSignRecord[] record = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, opActType);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)record.Length);
			for(int i = 0; i < record.Length; i++)
			{
				record[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
			UInt16 recordCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref recordCnt);
			record = new WeekSignRecord[recordCnt];
			for(int i = 0; i < record.Length; i++)
			{
				record[i] = new WeekSignRecord();
				record[i].decode(buffer, ref pos_);
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
