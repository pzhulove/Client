using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// server->client 通知被邀请玩家 邀请信息
	/// </summary>
	[AdvancedInspector.Descriptor("server->client 通知被邀请玩家 邀请信息", "server->client 通知被邀请玩家 邀请信息")]
	public class WorldSyncRoomInviteInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607804;
		public UInt32 Sequence;
		/// <summary>
		/// 房间号
		/// </summary>
		[AdvancedInspector.Descriptor("房间号", "房间号")]
		public UInt32 roomId;
		/// <summary>
		/// 房间名
		/// </summary>
		[AdvancedInspector.Descriptor("房间名", "房间名")]
		public string roomName;
		/// <summary>
		/// 房间类型
		/// </summary>
		[AdvancedInspector.Descriptor("房间类型", "房间类型")]
		public byte roomType;
		/// <summary>
		/// 邀请者ID
		/// </summary>
		[AdvancedInspector.Descriptor("邀请者ID", "邀请者ID")]
		public UInt64 inviterId;
		/// <summary>
		/// 邀请者名字
		/// </summary>
		[AdvancedInspector.Descriptor("邀请者名字", "邀请者名字")]
		public string inviterName;
		/// <summary>
		/// 邀请者职业
		/// </summary>
		[AdvancedInspector.Descriptor("邀请者职业", "邀请者职业")]
		public byte inviterOccu;
		/// <summary>
		/// 邀请者觉醒
		/// </summary>
		[AdvancedInspector.Descriptor("邀请者觉醒", "邀请者觉醒")]
		public byte inviterAwaken;
		/// <summary>
		/// 邀请者等级
		/// </summary>
		[AdvancedInspector.Descriptor("邀请者等级", "邀请者等级")]
		public UInt16 inviterLevel;
		/// <summary>
		/// 房间人数
		/// </summary>
		[AdvancedInspector.Descriptor("房间人数", "房间人数")]
		public UInt32 playerSize;
		/// <summary>
		/// 房间最大人数
		/// </summary>
		[AdvancedInspector.Descriptor("房间最大人数", "房间最大人数")]
		public UInt32 playerMaxSize;
		/// <summary>
		/// 队伍
		/// </summary>
		[AdvancedInspector.Descriptor("队伍", "队伍")]
		public byte slotGroup;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, roomId);
			byte[] roomNameBytes = StringHelper.StringToUTF8Bytes(roomName);
			BaseDLL.encode_string(buffer, ref pos_, roomNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, roomType);
			BaseDLL.encode_uint64(buffer, ref pos_, inviterId);
			byte[] inviterNameBytes = StringHelper.StringToUTF8Bytes(inviterName);
			BaseDLL.encode_string(buffer, ref pos_, inviterNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, inviterOccu);
			BaseDLL.encode_int8(buffer, ref pos_, inviterAwaken);
			BaseDLL.encode_uint16(buffer, ref pos_, inviterLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, playerSize);
			BaseDLL.encode_uint32(buffer, ref pos_, playerMaxSize);
			BaseDLL.encode_int8(buffer, ref pos_, slotGroup);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
			UInt16 roomNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref roomNameLen);
			byte[] roomNameBytes = new byte[roomNameLen];
			for(int i = 0; i < roomNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref roomNameBytes[i]);
			}
			roomName = StringHelper.BytesToString(roomNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
			BaseDLL.decode_uint64(buffer, ref pos_, ref inviterId);
			UInt16 inviterNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref inviterNameLen);
			byte[] inviterNameBytes = new byte[inviterNameLen];
			for(int i = 0; i < inviterNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref inviterNameBytes[i]);
			}
			inviterName = StringHelper.BytesToString(inviterNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref inviterOccu);
			BaseDLL.decode_int8(buffer, ref pos_, ref inviterAwaken);
			BaseDLL.decode_uint16(buffer, ref pos_, ref inviterLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerSize);
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerMaxSize);
			BaseDLL.decode_int8(buffer, ref pos_, ref slotGroup);
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
