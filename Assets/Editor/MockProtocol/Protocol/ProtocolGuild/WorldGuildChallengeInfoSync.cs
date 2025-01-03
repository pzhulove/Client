using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会宣战信息同步
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会宣战信息同步", " 公会宣战信息同步")]
	public class WorldGuildChallengeInfoSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601962;
		public UInt32 Sequence;

		public GuildTerritoryBaseInfo info = null;

		public UInt64 enrollGuildId;

		public string enrollGuildName;

		public string enrollGuildleaderName;

		public byte enrollGuildLevel;

		public UInt32 itemId;

		public UInt32 itemNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			info.encode(buffer, ref pos_);
			BaseDLL.encode_uint64(buffer, ref pos_, enrollGuildId);
			byte[] enrollGuildNameBytes = StringHelper.StringToUTF8Bytes(enrollGuildName);
			BaseDLL.encode_string(buffer, ref pos_, enrollGuildNameBytes, (UInt16)(buffer.Length - pos_));
			byte[] enrollGuildleaderNameBytes = StringHelper.StringToUTF8Bytes(enrollGuildleaderName);
			BaseDLL.encode_string(buffer, ref pos_, enrollGuildleaderNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, enrollGuildLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			info.decode(buffer, ref pos_);
			BaseDLL.decode_uint64(buffer, ref pos_, ref enrollGuildId);
			UInt16 enrollGuildNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref enrollGuildNameLen);
			byte[] enrollGuildNameBytes = new byte[enrollGuildNameLen];
			for(int i = 0; i < enrollGuildNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref enrollGuildNameBytes[i]);
			}
			enrollGuildName = StringHelper.BytesToString(enrollGuildNameBytes);
			UInt16 enrollGuildleaderNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref enrollGuildleaderNameLen);
			byte[] enrollGuildleaderNameBytes = new byte[enrollGuildleaderNameLen];
			for(int i = 0; i < enrollGuildleaderNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref enrollGuildleaderNameBytes[i]);
			}
			enrollGuildleaderName = StringHelper.BytesToString(enrollGuildleaderNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref enrollGuildLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
