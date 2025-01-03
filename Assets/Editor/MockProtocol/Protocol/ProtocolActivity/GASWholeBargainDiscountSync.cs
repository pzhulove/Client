using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  全民抢购最终折扣通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 全民抢购最终折扣通知", " 全民抢购最终折扣通知")]
	public class GASWholeBargainDiscountSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 707406;
		public UInt32 Sequence;
		/// <summary>
		///  折扣百分比
		/// </summary>
		[AdvancedInspector.Descriptor(" 折扣百分比", " 折扣百分比")]
		public UInt32 discount;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, discount);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref discount);
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
