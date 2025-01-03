using System;
using System.Text;

namespace Mock.Protocol
{

	public class GuildStorageItemInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 uid;

		public UInt32 dataId;

		public UInt16 num;

		public byte str;

		public byte equipType;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, uid);
			BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			BaseDLL.encode_uint16(buffer, ref pos_, num);
			BaseDLL.encode_int8(buffer, ref pos_, str);
			BaseDLL.encode_int8(buffer, ref pos_, equipType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			BaseDLL.decode_int8(buffer, ref pos_, ref str);
			BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
		}


		#endregion

	}

}
