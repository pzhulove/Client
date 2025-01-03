using System;
using System.Text;

namespace Mock.Protocol
{

	public class ScenePet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 id;

		public UInt32 dataId;

		public UInt16 level;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			BaseDLL.encode_uint16(buffer, ref pos_, level);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
		}


		#endregion

	}

}
