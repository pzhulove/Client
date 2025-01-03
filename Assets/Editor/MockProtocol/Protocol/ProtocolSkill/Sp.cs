using System;
using System.Text;

namespace Mock.Protocol
{

	public class Sp : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32[] spList = new UInt32[0];

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)spList.Length);
			for(int i = 0; i < spList.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, spList[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 spListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref spListCnt);
			spList = new UInt32[spListCnt];
			for(int i = 0; i < spList.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref spList[i]);
			}
		}


		#endregion

	}

}
