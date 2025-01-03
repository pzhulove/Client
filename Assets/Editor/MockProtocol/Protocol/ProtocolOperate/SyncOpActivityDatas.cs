using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 任务状态变量
	/// </summary>
	/// <summary>
	/// 同步运营活动data
	/// </summary>
	[AdvancedInspector.Descriptor("同步运营活动data", "同步运营活动data")]
	public class SyncOpActivityDatas : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501145;
		public UInt32 Sequence;

		public OpActivityData[] datas = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)datas.Length);
			for(int i = 0; i < datas.Length; i++)
			{
				datas[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 datasCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref datasCnt);
			datas = new OpActivityData[datasCnt];
			for(int i = 0; i < datas.Length; i++)
			{
				datas[i] = new OpActivityData();
				datas[i].decode(buffer, ref pos_);
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
