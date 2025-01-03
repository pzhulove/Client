using System;
using System.Text;

namespace Mock.Protocol
{

	public class DungeonAdditionInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32[] addition = new UInt32[10];

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			for(int i = 0; i < addition.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, addition[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			for(int i = 0; i < addition.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref addition[i]);
			}
		}


		#endregion

	}

}
