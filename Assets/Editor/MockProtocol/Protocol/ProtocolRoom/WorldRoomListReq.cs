using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->server 请求房间列表
	/// </summary>
	[AdvancedInspector.Descriptor("client->server 请求房间列表", "client->server 请求房间列表")]
	public class WorldRoomListReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607811;
		public UInt32 Sequence;
		/// <summary>
		///  玩家等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家等级", " 玩家等级")]
		public UInt16 limitPlayerLevel;
		/// <summary>
		///  玩家段位
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家段位", " 玩家段位")]
		public UInt32 limitPlayerSeasonLevel;
		/// <summary>
		///  房间状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 房间状态", " 房间状态")]
		public byte roomStatus;
		/// <summary>
		///  房间类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 房间类型", " 房间类型")]
		public byte roomType;
		/// <summary>
		///  是否有密码
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否有密码", " 是否有密码")]
		public byte isPassword;
		/// <summary>
		///  开始位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始位置", " 开始位置")]
		public UInt32 startIndex;
		/// <summary>
		///  个数
		/// </summary>
		[AdvancedInspector.Descriptor(" 个数", " 个数")]
		public UInt32 count;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, limitPlayerLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, limitPlayerSeasonLevel);
			BaseDLL.encode_int8(buffer, ref pos_, roomStatus);
			BaseDLL.encode_int8(buffer, ref pos_, roomType);
			BaseDLL.encode_int8(buffer, ref pos_, isPassword);
			BaseDLL.encode_uint32(buffer, ref pos_, startIndex);
			BaseDLL.encode_uint32(buffer, ref pos_, count);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint16(buffer, ref pos_, ref limitPlayerLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref limitPlayerSeasonLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref roomStatus);
			BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
			BaseDLL.decode_int8(buffer, ref pos_, ref isPassword);
			BaseDLL.decode_uint32(buffer, ref pos_, ref startIndex);
			BaseDLL.decode_uint32(buffer, ref pos_, ref count);
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
