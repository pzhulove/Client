using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonHardInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public byte unlockedHardType;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, unlockedHardType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref unlockedHardType);
		}


		#endregion

	}

}
