using System;
using System.Text;

namespace Mock.Protocol
{

	public class DigMapInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 mapId;

		public UInt32 goldDigSize;

		public UInt32 silverDigSize;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, mapId);
			BaseDLL.encode_uint32(buffer, ref pos_, goldDigSize);
			BaseDLL.encode_uint32(buffer, ref pos_, silverDigSize);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref goldDigSize);
			BaseDLL.decode_uint32(buffer, ref pos_, ref silverDigSize);
		}


		#endregion

	}

}
