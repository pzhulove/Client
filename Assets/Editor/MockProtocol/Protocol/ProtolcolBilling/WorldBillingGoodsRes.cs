using System;
using System.Text;

namespace Mock.Protocol
{

	public class WorldBillingGoodsRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 604008;
		public UInt32 Sequence;

		public ChargeGoods[] goods = null;

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
			goods = new ChargeGoods[goodsCnt];
			for(int i = 0; i < goods.Length; i++)
			{
				goods[i] = new ChargeGoods();
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
