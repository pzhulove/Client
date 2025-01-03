using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回充值礼包商品
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回充值礼包商品", " 返回充值礼包商品")]
	public class WorldBillingChargePacketRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 604010;
		public UInt32 Sequence;

		public ChargePacket[] goods = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)goods.Length);
			for(int i = 0; i < goods.Length; i++)
			{
				goods[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 goodsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref goodsCnt);
			goods = new ChargePacket[goodsCnt];
			for(int i = 0; i < goods.Length; i++)
			{
				goods[i] = new ChargePacket();
				goods[i].decode(buffer, ref pos_);
			}
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
