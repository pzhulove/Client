using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  神器罐活动抽奖记录请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 神器罐活动抽奖记录请求", " 神器罐活动抽奖记录请求")]
	public class GASArtifactJarLotteryRecordReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 700901;
		public UInt32 Sequence;
		/// <summary>
		///  罐子ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 罐子ID", " 罐子ID")]
		public UInt32 jarId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, jarId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
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
