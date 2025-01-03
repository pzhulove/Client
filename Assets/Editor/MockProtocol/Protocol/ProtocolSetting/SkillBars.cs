using System;
using System.Text;

namespace Mock.Protocol
{

	public class SkillBars : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte index;

		public SkillBar[] bar = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, index);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)bar.Length);
			for(int i = 0; i < bar.Length; i++)
			{
				bar[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref index);
			UInt16 barCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref barCnt);
			bar = new SkillBar[barCnt];
			for(int i = 0; i < bar.Length; i++)
			{
				bar[i] = new SkillBar();
				bar[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
