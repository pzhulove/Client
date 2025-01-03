using System;
using System.Text;

namespace Mock.Protocol
{

	public class SkillMgr : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 pageCnt;

		public UInt32 currentPage;

		public SkillPage[] skillPages = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, pageCnt);
			BaseDLL.encode_uint32(buffer, ref pos_, currentPage);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skillPages.Length);
			for(int i = 0; i < skillPages.Length; i++)
			{
				skillPages[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref pageCnt);
			BaseDLL.decode_uint32(buffer, ref pos_, ref currentPage);
			UInt16 skillPagesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref skillPagesCnt);
			skillPages = new SkillPage[skillPagesCnt];
			for(int i = 0; i < skillPages.Length; i++)
			{
				skillPages[i] = new SkillPage();
				skillPages[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
