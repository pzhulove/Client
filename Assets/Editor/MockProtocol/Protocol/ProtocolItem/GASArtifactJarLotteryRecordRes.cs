using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  神器罐活动抽奖记录返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 神器罐活动抽奖记录返回", " 神器罐活动抽奖记录返回")]
	public class GASArtifactJarLotteryRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 700902;
		public UInt32 Sequence;
		/// <summary>
		///  罐子ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 罐子ID", " 罐子ID")]
		public UInt32 jarId;
		/// <summary>
		///  记录列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 记录列表", " 记录列表")]
		public ArtifactJarLotteryRecord[] lotteryRecordList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, jarId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)lotteryRecordList.Length);
			for(int i = 0; i < lotteryRecordList.Length; i++)
			{
				lotteryRecordList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
			UInt16 lotteryRecordListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref lotteryRecordListCnt);
			lotteryRecordList = new ArtifactJarLotteryRecord[lotteryRecordListCnt];
			for(int i = 0; i < lotteryRecordList.Length; i++)
			{
				lotteryRecordList[i] = new ArtifactJarLotteryRecord();
				lotteryRecordList[i].decode(buffer, ref pos_);
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
