using System;
using System.Text;

namespace Mock.Protocol
{

	public class ReplayRaceResult : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte pos;

		public byte result;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, pos);
			BaseDLL.encode_int8(buffer, ref pos_, result);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref pos);
			BaseDLL.decode_int8(buffer, ref pos_, ref result);
		}


		#endregion

	}

}
