using System;
using System.Text;

namespace Mock.Protocol
{

	public class ChangeSkill : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt16 id;

		public byte dif;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, dif);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint16(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref dif);
		}


		#endregion

	}

}
