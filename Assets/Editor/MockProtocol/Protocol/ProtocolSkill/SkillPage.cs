using System;
using System.Text;

namespace Mock.Protocol
{

	public class SkillPage : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public Skill[] skillList = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skillList.Length);
			for(int i = 0; i < skillList.Length; i++)
			{
				skillList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 skillListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref skillListCnt);
			skillList = new Skill[skillListCnt];
			for(int i = 0; i < skillList.Length; i++)
			{
				skillList[i] = new Skill();
				skillList[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
