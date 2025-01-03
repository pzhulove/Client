using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 请求进入房间
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server 请求进入房间", " client->server 请求进入房间")]
	public class WorldJoinRoomReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607815;
		public UInt32 Sequence;
		/// <summary>
		///  房间号
		/// </summary>
		[AdvancedInspector.Descriptor(" 房间号", " 房间号")]
		public UInt32 roomId;
		/// <summary>
		/// 房间类型
		/// </summary>
		[AdvancedInspector.Descriptor("房间类型", "房间类型")]
		public byte roomType;
		/// <summary>
		/// 密码
		/// </summary>
		[AdvancedInspector.Descriptor("密码", "密码")]
		public string password;
		/// <summary>
		/// 创建时间
		/// </summary>
		[AdvancedInspector.Descriptor("创建时间", "创建时间")]
		public UInt32 createTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, roomId);
			BaseDLL.encode_int8(buffer, ref pos_, roomType);
			byte[] passwordBytes = StringHelper.StringToUTF8Bytes(password);
			BaseDLL.encode_string(buffer, ref pos_, passwordBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, createTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
			BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
			UInt16 passwordLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref passwordLen);
			byte[] passwordBytes = new byte[passwordLen];
			for(int i = 0; i < passwordLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref passwordBytes[i]);
			}
			password = StringHelper.BytesToString(passwordBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref createTime);
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
