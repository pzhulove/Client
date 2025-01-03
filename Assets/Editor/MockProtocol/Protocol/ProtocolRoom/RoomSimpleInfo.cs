using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 房间简单信息
	/// </summary>
	[AdvancedInspector.Descriptor("房间简单信息", "房间简单信息")]
	public class RoomSimpleInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public string name;

		public byte roomStatus;

		public byte roomType;

		public byte isLimitPlayerLevel;

		public UInt16 limitPlayerLevel;

		public byte isLimitPlayerSeasonLevel;

		public UInt32 limitPlayerSeasonLevel;

		public byte playerSize;

		public byte playerMaxSize;

		public byte isPassword;

		public UInt64 ownerId;

		public byte ownerOccu;

		public UInt32 ownerSeasonLevel;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, roomStatus);
			BaseDLL.encode_int8(buffer, ref pos_, roomType);
			BaseDLL.encode_int8(buffer, ref pos_, isLimitPlayerLevel);
			BaseDLL.encode_uint16(buffer, ref pos_, limitPlayerLevel);
			BaseDLL.encode_int8(buffer, ref pos_, isLimitPlayerSeasonLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, limitPlayerSeasonLevel);
			BaseDLL.encode_int8(buffer, ref pos_, playerSize);
			BaseDLL.encode_int8(buffer, ref pos_, playerMaxSize);
			BaseDLL.encode_int8(buffer, ref pos_, isPassword);
			BaseDLL.encode_uint64(buffer, ref pos_, ownerId);
			BaseDLL.encode_int8(buffer, ref pos_, ownerOccu);
			BaseDLL.encode_uint32(buffer, ref pos_, ownerSeasonLevel);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref roomStatus);
			BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
			BaseDLL.decode_int8(buffer, ref pos_, ref isLimitPlayerLevel);
			BaseDLL.decode_uint16(buffer, ref pos_, ref limitPlayerLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref isLimitPlayerSeasonLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref limitPlayerSeasonLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref playerSize);
			BaseDLL.decode_int8(buffer, ref pos_, ref playerMaxSize);
			BaseDLL.decode_int8(buffer, ref pos_, ref isPassword);
			BaseDLL.decode_uint64(buffer, ref pos_, ref ownerId);
			BaseDLL.decode_int8(buffer, ref pos_, ref ownerOccu);
			BaseDLL.decode_uint32(buffer, ref pos_, ref ownerSeasonLevel);
		}


		#endregion

	}

}
