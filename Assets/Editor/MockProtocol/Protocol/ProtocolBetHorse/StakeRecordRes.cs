using System;
using System.Text;

namespace Mock.Protocol
{

	public class StakeRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 708311;
		public UInt32 Sequence;
		/// <summary>
		///  押注历史列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 押注历史列表", " 押注历史列表")]
		public StakeRecord[] StakeRecordList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)StakeRecordList.Length);
			for(int i = 0; i < StakeRecordList.Length; i++)
			{
				StakeRecordList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 StakeRecordListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref StakeRecordListCnt);
			StakeRecordList = new StakeRecord[StakeRecordListCnt];
			for(int i = 0; i < StakeRecordList.Length; i++)
			{
				StakeRecordList[i] = new StakeRecord();
				StakeRecordList[i].decode(buffer, ref pos_);
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
