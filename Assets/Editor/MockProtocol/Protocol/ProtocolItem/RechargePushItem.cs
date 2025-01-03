using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 充值推送购买
	/// </summary>
	[AdvancedInspector.Descriptor("充值推送购买", "充值推送购买")]
	public class RechargePushItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 pushId;

		public UInt32 itemId;

		public UInt32 itemCount;

		public UInt32 buyTimes;

		public UInt32 maxTimes;

		public UInt32 price;

		public UInt32 discountPrice;
		/// <summary>
		/// 截至时间戳
		/// </summary>
		[AdvancedInspector.Descriptor("截至时间戳", "截至时间戳")]
		public UInt32 validTimestamp;

		public UInt32 lastTimestamp;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, pushId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemCount);
			BaseDLL.encode_uint32(buffer, ref pos_, buyTimes);
			BaseDLL.encode_uint32(buffer, ref pos_, maxTimes);
			BaseDLL.encode_uint32(buffer, ref pos_, price);
			BaseDLL.encode_uint32(buffer, ref pos_, discountPrice);
			BaseDLL.encode_uint32(buffer, ref pos_, validTimestamp);
			BaseDLL.encode_uint32(buffer, ref pos_, lastTimestamp);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref pushId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemCount);
			BaseDLL.decode_uint32(buffer, ref pos_, ref buyTimes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref maxTimes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref price);
			BaseDLL.decode_uint32(buffer, ref pos_, ref discountPrice);
			BaseDLL.decode_uint32(buffer, ref pos_, ref validTimestamp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref lastTimestamp);
		}


		#endregion

	}

}
