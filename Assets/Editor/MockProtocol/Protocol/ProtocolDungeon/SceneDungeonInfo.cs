using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public byte bestScore;

		public UInt32 bestRecordTime;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, bestScore);
			BaseDLL.encode_uint32(buffer, ref pos_, bestRecordTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref bestScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref bestRecordTime);
		}


		#endregion

	}

}
