using System;
using System.Text;

namespace Mock.Protocol
{

	public class Frame : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 sequence;

		public _fighterInput[] data = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, sequence);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)data.Length);
			for(int i = 0; i < data.Length; i++)
			{
				data[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref sequence);
			UInt16 dataCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dataCnt);
			data = new _fighterInput[dataCnt];
			for(int i = 0; i < data.Length; i++)
			{
				data[i] = new _fighterInput();
				data[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
