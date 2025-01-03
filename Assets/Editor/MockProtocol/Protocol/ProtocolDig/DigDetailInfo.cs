using System;
using System.Text;

namespace Mock.Protocol
{

	public class DigDetailInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public DigInfo simpleInfo = null;

		public DigItemInfo[] digItems = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			simpleInfo.encode(buffer, ref pos_);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)digItems.Length);
			for(int i = 0; i < digItems.Length; i++)
			{
				digItems[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			simpleInfo.decode(buffer, ref pos_);
			UInt16 digItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref digItemsCnt);
			digItems = new DigItemInfo[digItemsCnt];
			for(int i = 0; i < digItems.Length; i++)
			{
				digItems[i] = new DigItemInfo();
				digItems[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
