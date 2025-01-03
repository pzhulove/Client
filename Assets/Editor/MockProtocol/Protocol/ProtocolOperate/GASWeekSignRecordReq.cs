using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  周签到活动抽奖记录请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 周签到活动抽奖记录请求", " 周签到活动抽奖记录请求")]
	public class GASWeekSignRecordReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 707401;
		public UInt32 Sequence;
		/// <summary>
		///  活动类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 活动类型", " 活动类型")]
		public UInt32 opActType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, opActType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
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
