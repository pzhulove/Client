using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 
	/// </summary>
	[AdvancedInspector.Descriptor("", "")]
	public class WorldQueryTaskStatusRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601787;
		public UInt32 Sequence;
		/// <summary>
		/// 招募任务情况
		/// </summary>
		[AdvancedInspector.Descriptor("招募任务情况", "招募任务情况")]
		public HireInfoData[] hireTaskInfoList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)hireTaskInfoList.Length);
			for(int i = 0; i < hireTaskInfoList.Length; i++)
			{
				hireTaskInfoList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 hireTaskInfoListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref hireTaskInfoListCnt);
			hireTaskInfoList = new HireInfoData[hireTaskInfoListCnt];
			for(int i = 0; i < hireTaskInfoList.Length; i++)
			{
				hireTaskInfoList[i] = new HireInfoData();
				hireTaskInfoList[i].decode(buffer, ref pos_);
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
