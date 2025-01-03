using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求购买商品(这里只判断能不能买)
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求购买商品(这里只判断能不能买)", " 请求购买商品(这里只判断能不能买)")]
	public class WorldBillingChargeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 604011;
		public UInt32 Sequence;
		/// <summary>
		///  商城类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 商城类型", " 商城类型")]
		public byte mallType;
		/// <summary>
		///  商品ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品ID", " 商品ID")]
		public UInt32 goodsId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, mallType);
			BaseDLL.encode_uint32(buffer, ref pos_, goodsId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref mallType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref goodsId);
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
