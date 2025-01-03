using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDir : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public Int16 x;

		public Int16 y;

		public byte faceRight;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int16(buffer, ref pos_, x);
			BaseDLL.encode_int16(buffer, ref pos_, y);
			BaseDLL.encode_int8(buffer, ref pos_, faceRight);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int16(buffer, ref pos_, ref x);
			BaseDLL.decode_int16(buffer, ref pos_, ref y);
			BaseDLL.decode_int8(buffer, ref pos_, ref faceRight);
		}


		#endregion

	}

}
