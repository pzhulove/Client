using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  雕像数据
	/// </summary>
	[AdvancedInspector.Descriptor(" 雕像数据", " 雕像数据")]
	public class FigureStatueInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 accId;

		public UInt64 roleId;

		public string name;

		public byte occu;

		public PlayerAvatar avatar = null;

		public byte statueType;

		public string guildName;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, accId);
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			avatar.encode(buffer, ref pos_);
			BaseDLL.encode_int8(buffer, ref pos_, statueType);
			byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
			BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref accId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
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
			BaseDLL.decode_int8(buffer, ref pos_, ref statueType);
			UInt16 guildNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
			byte[] guildNameBytes = new byte[guildNameLen];
			for(int i = 0; i < guildNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
			}
			guildName = StringHelper.BytesToString(guildNameBytes);
		}


		#endregion

	}

}
