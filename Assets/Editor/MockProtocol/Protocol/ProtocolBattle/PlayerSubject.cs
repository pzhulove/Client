using System;
using System.Text;

namespace Mock.Protocol
{

	public class PlayerSubject : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 accId;

		public UInt64 playerId;

		public string playerName;

		public byte occu;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, accId);
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
			BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref accId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			UInt16 playerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
			byte[] playerNameBytes = new byte[playerNameLen];
			for(int i = 0; i < playerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
			}
			playerName = StringHelper.BytesToString(playerNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
		}


		#endregion

	}

}
