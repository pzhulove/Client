using System;
using System.Text;

namespace Mock.Protocol
{

	public class RaceSkillInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt16 id;

		public byte level;

		public byte slot;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, slot);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint16(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref level);
			BaseDLL.decode_int8(buffer, ref pos_, ref slot);
		}


		#endregion

	}

}
