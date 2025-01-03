using System;
using System.Text;

namespace Mock.Protocol
{

	public class ShooterHistoryRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 708306;
		public UInt32 Sequence;
		/// <summary>
		///  射手id
		/// </summary>
		[AdvancedInspector.Descriptor(" 射手id", " 射手id")]
		public UInt32 id;
		/// <summary>
		///  历史战绩列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 历史战绩列表", " 历史战绩列表")]
		public ShooterRecord[] recordList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)recordList.Length);
			for(int i = 0; i < recordList.Length; i++)
			{
				recordList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			UInt16 recordListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref recordListCnt);
			recordList = new ShooterRecord[recordListCnt];
			for(int i = 0; i < recordList.Length; i++)
			{
				recordList[i] = new ShooterRecord();
				recordList[i].decode(buffer, ref pos_);
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
