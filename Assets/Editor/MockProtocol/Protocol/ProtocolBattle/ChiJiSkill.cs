using System;
using System.Text;

namespace Mock.Protocol
{

	public class ChiJiSkill : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 skillId;

		public UInt32 skillLvl;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, skillId);
			BaseDLL.encode_uint32(buffer, ref pos_, skillLvl);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref skillId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref skillLvl);
		}


		#endregion

	}

}
