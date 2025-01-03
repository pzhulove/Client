using System;
using System.Text;

namespace Mock.Protocol
{

	public class RaceEquipScheme : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte type;

		public UInt32 id;

		public byte isWear;

		public UInt64[] equips = new UInt64[0];

		public UInt64 title;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, isWear);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equips.Length);
			for(int i = 0; i < equips.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, equips[i]);
			}
			BaseDLL.encode_uint64(buffer, ref pos_, title);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref isWear);
			UInt16 equipsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref equipsCnt);
			equips = new UInt64[equipsCnt];
			for(int i = 0; i < equips.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref equips[i]);
			}
			BaseDLL.decode_uint64(buffer, ref pos_, ref title);
		}


		#endregion

	}

}
