using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 前16强玩家数据
	/// </summary>
	[AdvancedInspector.Descriptor("前16强玩家数据", "前16强玩家数据")]
	public class ChampionTop16Player : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 roleID;

		public UInt32 pos;

		public string name;

		public byte occu;

		public PlayerAvatar avatar = null;

		public string server;

		public UInt32 zoneID;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleID);
			BaseDLL.encode_uint32(buffer, ref pos_, pos);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			avatar.encode(buffer, ref pos_);
			byte[] serverBytes = StringHelper.StringToUTF8Bytes(server);
			BaseDLL.encode_string(buffer, ref pos_, serverBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, zoneID);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref pos);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			avatar.decode(buffer, ref pos_);
			UInt16 serverLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref serverLen);
			byte[] serverBytes = new byte[serverLen];
			for(int i = 0; i < serverLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref serverBytes[i]);
			}
			server = StringHelper.BytesToString(serverBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref zoneID);
		}


		#endregion

	}

}
