using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 淘汰赛战斗对象
	/// </summary>
	[AdvancedInspector.Descriptor("淘汰赛战斗对象", "淘汰赛战斗对象")]
	public class KnockoutPlayer : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 roleID;

		public string name;

		public byte occu;

		public string server;

		public UInt32 score;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleID);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			byte[] serverBytes = StringHelper.StringToUTF8Bytes(server);
			BaseDLL.encode_string(buffer, ref pos_, serverBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, score);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleID);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			UInt16 serverLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref serverLen);
			byte[] serverBytes = new byte[serverLen];
			for(int i = 0; i < serverLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref serverBytes[i]);
			}
			server = StringHelper.BytesToString(serverBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref score);
		}


		#endregion

	}

}