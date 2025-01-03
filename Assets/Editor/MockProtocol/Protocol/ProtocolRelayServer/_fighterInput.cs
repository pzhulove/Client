using System;
using System.Text;

namespace Mock.Protocol
{

	public class _fighterInput : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte seat;

		public _inputData input = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, seat);
			input.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref seat);
			input.decode(buffer, ref pos_);
		}


		#endregion

	}

}
