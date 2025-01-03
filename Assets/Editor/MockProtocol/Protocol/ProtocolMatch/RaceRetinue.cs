using System;
using System.Text;

namespace Mock.Protocol
{

	public class RaceRetinue : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 dataId;

		public byte level;

		public byte star;

		public byte isMain;

		public UInt32[] buffIds = new UInt32[0];

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			BaseDLL.encode_int8(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, star);
			BaseDLL.encode_int8(buffer, ref pos_, isMain);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffIds.Length);
			for(int i = 0; i < buffIds.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, buffIds[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			BaseDLL.decode_int8(buffer, ref pos_, ref level);
			BaseDLL.decode_int8(buffer, ref pos_, ref star);
			BaseDLL.decode_int8(buffer, ref pos_, ref isMain);
			UInt16 buffIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref buffIdsCnt);
			buffIds = new UInt32[buffIdsCnt];
			for(int i = 0; i < buffIds.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffIds[i]);
			}
		}


		#endregion

	}

}
