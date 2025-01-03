using System;
using System.Text;

namespace Mock.Protocol
{

	public class PetInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 id;

		public UInt32 dataId;

		public UInt16 level;

		public UInt32 exp;

		public UInt16 hunger;

		public byte skillIndex;

		public byte pointFeedCount;

		public byte goldFeedCount;

		public byte selectSkillCount;

		public UInt32 petScore;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_uint32(buffer, ref pos_, exp);
			BaseDLL.encode_uint16(buffer, ref pos_, hunger);
			BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
			BaseDLL.encode_int8(buffer, ref pos_, pointFeedCount);
			BaseDLL.encode_int8(buffer, ref pos_, goldFeedCount);
			BaseDLL.encode_int8(buffer, ref pos_, selectSkillCount);
			BaseDLL.encode_uint32(buffer, ref pos_, petScore);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			BaseDLL.decode_uint32(buffer, ref pos_, ref exp);
			BaseDLL.decode_uint16(buffer, ref pos_, ref hunger);
			BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
			BaseDLL.decode_int8(buffer, ref pos_, ref pointFeedCount);
			BaseDLL.decode_int8(buffer, ref pos_, ref goldFeedCount);
			BaseDLL.decode_int8(buffer, ref pos_, ref selectSkillCount);
			BaseDLL.decode_uint32(buffer, ref pos_, ref petScore);
		}


		#endregion

	}

}
