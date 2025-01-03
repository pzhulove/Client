using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 1.可领取, 0.未完成
	/// </summary>
	/// <summary>
	/// 领取阶段礼包
	/// </summary>
	[AdvancedInspector.Descriptor("领取阶段礼包", "领取阶段礼包")]
	public class SceneTakePhaseGift : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501142;
		public UInt32 Sequence;

		public UInt32 giftId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, giftId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref giftId);
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
