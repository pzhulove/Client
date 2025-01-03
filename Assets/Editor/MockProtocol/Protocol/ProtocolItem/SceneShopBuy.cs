using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneShopBuy : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500924;
		public UInt32 Sequence;

		public byte shopId;

		public UInt32 shopItemId;

		public UInt16 num;

		public ItemInfo[] costExtraItems = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, shopId);
			BaseDLL.encode_uint32(buffer, ref pos_, shopItemId);
			BaseDLL.encode_uint16(buffer, ref pos_, num);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)costExtraItems.Length);
			for(int i = 0; i < costExtraItems.Length; i++)
			{
				costExtraItems[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref shopId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref shopItemId);
			BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			UInt16 costExtraItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref costExtraItemsCnt);
			costExtraItems = new ItemInfo[costExtraItemsCnt];
			for(int i = 0; i < costExtraItems.Length; i++)
			{
				costExtraItems[i] = new ItemInfo();
				costExtraItems[i].decode(buffer, ref pos_);
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
