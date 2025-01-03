using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  server->client 通知玩家密码信息
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client 通知玩家密码信息", " server->client 通知玩家密码信息")]
	public class WorldSyncRoomPasswordInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607809;
		public UInt32 Sequence;

		public UInt32 roomId;

		public string password;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, roomId);
			byte[] passwordBytes = StringHelper.StringToUTF8Bytes(password);
			BaseDLL.encode_string(buffer, ref pos_, passwordBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
			UInt16 passwordLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref passwordLen);
			byte[] passwordBytes = new byte[passwordLen];
			for(int i = 0; i < passwordLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref passwordBytes[i]);
			}
			password = StringHelper.BytesToString(passwordBytes);
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
