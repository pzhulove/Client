using System;
using System.Text;

namespace Mock.Protocol
{

	public class SkillBarGrid : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte slot;

		public UInt16 id;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, slot);
			BaseDLL.encode_uint16(buffer, ref pos_, id);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref slot);
			BaseDLL.decode_uint16(buffer, ref pos_, ref id);
		}


		#endregion

	}

}
