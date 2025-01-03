using System;
using System.Text;

namespace Mock.Protocol
{

	public class RacePet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 dataId;

		public UInt16 level;

		public UInt16 hunger;

		public byte skillIndex;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_uint16(buffer, ref pos_, hunger);
			BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			BaseDLL.decode_uint16(buffer, ref pos_, ref hunger);
			BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
		}


		#endregion

	}

}
