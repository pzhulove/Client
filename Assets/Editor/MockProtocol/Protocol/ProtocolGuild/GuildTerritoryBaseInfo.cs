using System;
using System.Text;

namespace Mock.Protocol
{

	public class GuildTerritoryBaseInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  领地ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 领地ID", " 领地ID")]
		public byte terrId;
		/// <summary>
		///  服务器名称
		/// </summary>
		[AdvancedInspector.Descriptor(" 服务器名称", " 服务器名称")]
		public string serverName;
		/// <summary>
		///  占领公会名称
		/// </summary>
		[AdvancedInspector.Descriptor(" 占领公会名称", " 占领公会名称")]
		public string guildName;
		/// <summary>
		///  已经报名数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经报名数量", " 已经报名数量")]
		public UInt32 enrollSize;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, terrId);
			byte[] serverNameBytes = StringHelper.StringToUTF8Bytes(serverName);
			BaseDLL.encode_string(buffer, ref pos_, serverNameBytes, (UInt16)(buffer.Length - pos_));
			byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
			BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, enrollSize);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
			UInt16 serverNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref serverNameLen);
			byte[] serverNameBytes = new byte[serverNameLen];
			for(int i = 0; i < serverNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref serverNameBytes[i]);
			}
			serverName = StringHelper.BytesToString(serverNameBytes);
			UInt16 guildNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
			byte[] guildNameBytes = new byte[guildNameLen];
			for(int i = 0; i < guildNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
			}
			guildName = StringHelper.BytesToString(guildNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref enrollSize);
		}


		#endregion

	}

}
