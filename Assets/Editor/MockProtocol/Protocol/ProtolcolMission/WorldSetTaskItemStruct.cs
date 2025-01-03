using System;
using System.Text;

namespace Mock.Protocol
{

	public class WorldSetTaskItemStruct : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 itemGuid;

		public UInt32 itemId;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
		}


		#endregion

	}

}
