using System;
using System.Text;

namespace Mock.Protocol
{

	public class GiftPackSyncInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public UInt32 filterType;

		public Int32 filterCount;

		public GiftSyncInfo[] gifts = null;

		public byte uiType;

		public string description;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, filterType);
			BaseDLL.encode_int32(buffer, ref pos_, filterCount);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)gifts.Length);
			for(int i = 0; i < gifts.Length; i++)
			{
				gifts[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_int8(buffer, ref pos_, uiType);
			byte[] descriptionBytes = StringHelper.StringToUTF8Bytes(description);
			BaseDLL.encode_string(buffer, ref pos_, descriptionBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref filterType);
			BaseDLL.decode_int32(buffer, ref pos_, ref filterCount);
			UInt16 giftsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref giftsCnt);
			gifts = new GiftSyncInfo[giftsCnt];
			for(int i = 0; i < gifts.Length; i++)
			{
				gifts[i] = new GiftSyncInfo();
				gifts[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_int8(buffer, ref pos_, ref uiType);
			UInt16 descriptionLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref descriptionLen);
			byte[] descriptionBytes = new byte[descriptionLen];
			for(int i = 0; i < descriptionLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref descriptionBytes[i]);
			}
			description = StringHelper.BytesToString(descriptionBytes);
		}


		#endregion

	}

}
