using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 更新阶段礼包状态
	/// </summary>
	[AdvancedInspector.Descriptor("更新阶段礼包状态", "更新阶段礼包状态")]
	public class SceneSyncPhaseGift : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501141;
		public UInt32 Sequence;

		public UInt32 giftId;

		public byte canTake;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, giftId);
			BaseDLL.encode_int8(buffer, ref pos_, canTake);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref giftId);
			BaseDLL.decode_int8(buffer, ref pos_, ref canTake);
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
