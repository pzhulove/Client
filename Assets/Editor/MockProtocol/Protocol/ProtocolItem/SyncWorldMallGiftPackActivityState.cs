using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 错误码
	/// </summary>
	/// <summary>
	/// 同步商城礼包活动状态
	/// </summary>
	[AdvancedInspector.Descriptor("同步商城礼包活动状态", "同步商城礼包活动状态")]
	public class SyncWorldMallGiftPackActivityState : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602817;
		public UInt32 Sequence;
		/// <summary>
		/// 对应枚举MallGiftPackActivityState
		/// </summary>
		[AdvancedInspector.Descriptor("对应枚举MallGiftPackActivityState", "对应枚举MallGiftPackActivityState")]
		public byte state;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, state);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref state);
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
