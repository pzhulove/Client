using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  工会站结束信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 工会站结束信息", " 工会站结束信息")]
	public class GuildBattleEndInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte terrId;

		public string terrName;

		public UInt64 guildId;

		public string guildName;

		public string guildLeaderName;

		public string guildServerName;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, terrId);
			byte[] terrNameBytes = StringHelper.StringToUTF8Bytes(terrName);
			BaseDLL.encode_string(buffer, ref pos_, terrNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint64(buffer, ref pos_, guildId);
			byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
			BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
			byte[] guildLeaderNameBytes = StringHelper.StringToUTF8Bytes(guildLeaderName);
			BaseDLL.encode_string(buffer, ref pos_, guildLeaderNameBytes, (UInt16)(buffer.Length - pos_));
			byte[] guildServerNameBytes = StringHelper.StringToUTF8Bytes(guildServerName);
			BaseDLL.encode_string(buffer, ref pos_, guildServerNameBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
			UInt16 terrNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref terrNameLen);
			byte[] terrNameBytes = new byte[terrNameLen];
			for(int i = 0; i < terrNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref terrNameBytes[i]);
			}
			terrName = StringHelper.BytesToString(terrNameBytes);
			BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
			UInt16 guildNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
			byte[] guildNameBytes = new byte[guildNameLen];
			for(int i = 0; i < guildNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
			}
			guildName = StringHelper.BytesToString(guildNameBytes);
			UInt16 guildLeaderNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref guildLeaderNameLen);
			byte[] guildLeaderNameBytes = new byte[guildLeaderNameLen];
			for(int i = 0; i < guildLeaderNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref guildLeaderNameBytes[i]);
			}
			guildLeaderName = StringHelper.BytesToString(guildLeaderNameBytes);
			UInt16 guildServerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref guildServerNameLen);
			byte[] guildServerNameBytes = new byte[guildServerNameLen];
			for(int i = 0; i < guildServerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref guildServerNameBytes[i]);
			}
			guildServerName = StringHelper.BytesToString(guildServerNameBytes);
		}


		#endregion

	}

}
