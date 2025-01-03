using System;
using System.Text;

namespace Mock.Protocol
{

	public class RetinueInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 id;

		public UInt32 dataId;

		public byte level;

		public byte starLevel;

		public RetinueSkill[] skills = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			BaseDLL.encode_int8(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, starLevel);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skills.Length);
			for(int i = 0; i < skills.Length; i++)
			{
				skills[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			BaseDLL.decode_int8(buffer, ref pos_, ref level);
			BaseDLL.decode_int8(buffer, ref pos_, ref starLevel);
			UInt16 skillsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref skillsCnt);
			skills = new RetinueSkill[skillsCnt];
			for(int i = 0; i < skills.Length; i++)
			{
				skills[i] = new RetinueSkill();
				skills[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
