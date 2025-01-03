using System;
using System.Text;

namespace Mock.Protocol
{

	public class ScenePosition : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 x;

		public UInt32 y;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, x);
			BaseDLL.encode_uint32(buffer, ref pos_, y);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref x);
			BaseDLL.decode_uint32(buffer, ref pos_, ref y);
		}


		#endregion

	}

}
