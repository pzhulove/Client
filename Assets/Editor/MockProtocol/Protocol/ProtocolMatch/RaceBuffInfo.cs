using System;
using System.Text;

namespace Mock.Protocol
{

	public class RaceBuffInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public UInt32 overlayNums;

		public UInt64 startTime;

		public UInt32 duration;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, overlayNums);
			BaseDLL.encode_uint64(buffer, ref pos_, startTime);
			BaseDLL.encode_uint32(buffer, ref pos_, duration);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref overlayNums);
			BaseDLL.decode_uint64(buffer, ref pos_, ref startTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref duration);
		}


		#endregion

	}

}
