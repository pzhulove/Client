using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 请求创建或更新房间
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server 请求创建或更新房间", " client->server 请求创建或更新房间")]
	public class WorldUpdateRoomReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607813;
		public UInt32 Sequence;
		/// <summary>
		/// 房间号
		/// </summary>
		[AdvancedInspector.Descriptor("房间号", "房间号")]
		public UInt32 roomId;
		/// <summary>
		/// 房间类型
		/// </summary>
		[AdvancedInspector.Descriptor("房间类型", "房间类型")]
		public byte roomType;
		/// <summary>
		/// 房间名
		/// </summary>
		[AdvancedInspector.Descriptor("房间名", "房间名")]
		public string name;
		/// <summary>
		/// 房间密码
		/// </summary>
		[AdvancedInspector.Descriptor("房间密码", "房间密码")]
		public string password;
		/// <summary>
		/// 是否启用房间限制等级
		/// </summary>
		[AdvancedInspector.Descriptor("是否启用房间限制等级", "是否启用房间限制等级")]
		public byte isLimitPlayerLevel;
		/// <summary>
		/// 房间限制等级
		/// </summary>
		[AdvancedInspector.Descriptor("房间限制等级", "房间限制等级")]
		public UInt16 limitPlayerLevel;
		/// <summary>
		/// 是否启用房间限制段位
		/// </summary>
		[AdvancedInspector.Descriptor("是否启用房间限制段位", "是否启用房间限制段位")]
		public byte isLimitPlayerSeasonLevel;
		/// <summary>
		/// 房间限制段位
		/// </summary>
		[AdvancedInspector.Descriptor("房间限制段位", "房间限制段位")]
		public UInt32 limitPlayerSeasonLevel;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, roomId);
			BaseDLL.encode_int8(buffer, ref pos_, roomType);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			byte[] passwordBytes = StringHelper.StringToUTF8Bytes(password);
			BaseDLL.encode_string(buffer, ref pos_, passwordBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, isLimitPlayerLevel);
			BaseDLL.encode_uint16(buffer, ref pos_, limitPlayerLevel);
			BaseDLL.encode_int8(buffer, ref pos_, isLimitPlayerSeasonLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, limitPlayerSeasonLevel);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
			BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			UInt16 passwordLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref passwordLen);
			byte[] passwordBytes = new byte[passwordLen];
			for(int i = 0; i < passwordLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref passwordBytes[i]);
			}
			password = StringHelper.BytesToString(passwordBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref isLimitPlayerLevel);
			BaseDLL.decode_uint16(buffer, ref pos_, ref limitPlayerLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref isLimitPlayerSeasonLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref limitPlayerSeasonLevel);
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
