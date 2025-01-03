using System;
using System.Text;

namespace Mock.Protocol
{

	public class DigInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 index;

		public byte type;

		public byte status;

		public UInt32 refreshTime;

		public UInt32 changeStatusTime;

		public UInt32 openItemId;

		public UInt32 openItemNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, index);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_uint32(buffer, ref pos_, refreshTime);
			BaseDLL.encode_uint32(buffer, ref pos_, changeStatusTime);
			BaseDLL.encode_uint32(buffer, ref pos_, openItemId);
			BaseDLL.encode_uint32(buffer, ref pos_, openItemNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			BaseDLL.decode_uint32(buffer, ref pos_, ref refreshTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref changeStatusTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref openItemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref openItemNum);
		}


		#endregion

	}

}
