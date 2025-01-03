using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 领取运营活动任务奖励
	/// </summary>
	[AdvancedInspector.Descriptor("领取运营活动任务奖励", "领取运营活动任务奖励")]
	public class TakeOpActTaskReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501148;
		public UInt32 Sequence;

		public UInt32 activityDataId;

		public UInt32 taskDataId;

		public UInt64 param;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, activityDataId);
			BaseDLL.encode_uint32(buffer, ref pos_, taskDataId);
			BaseDLL.encode_uint64(buffer, ref pos_, param);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref activityDataId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskDataId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref param);
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
