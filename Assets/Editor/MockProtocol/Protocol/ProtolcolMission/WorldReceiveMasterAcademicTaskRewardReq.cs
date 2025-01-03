using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 徒弟领取师门成长奖励请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 徒弟领取师门成长奖励请求", "client->world 徒弟领取师门成长奖励请求")]
	public class WorldReceiveMasterAcademicTaskRewardReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601767;
		public UInt32 Sequence;

		public UInt32 giftDataId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, giftDataId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref giftDataId);
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
