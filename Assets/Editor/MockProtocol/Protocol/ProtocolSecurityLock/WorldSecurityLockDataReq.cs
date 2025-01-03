using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  申请中(强制解锁)
	/// </summary>
	/// <summary>
	///  请求安全锁信息(登录发送该消息)
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求安全锁信息(登录发送该消息)", " 请求安全锁信息(登录发送该消息)")]
	public class WorldSecurityLockDataReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608402;
		public UInt32 Sequence;
		/// <summary>
		///  设备ID，客户端没有id时，发送空字符
		/// </summary>
		[AdvancedInspector.Descriptor(" 设备ID，客户端没有id时，发送空字符", " 设备ID，客户端没有id时，发送空字符")]
		public string deviceID;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] deviceIDBytes = StringHelper.StringToUTF8Bytes(deviceID);
			BaseDLL.encode_string(buffer, ref pos_, deviceIDBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 deviceIDLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref deviceIDLen);
			byte[] deviceIDBytes = new byte[deviceIDLen];
			for(int i = 0; i < deviceIDLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref deviceIDBytes[i]);
			}
			deviceID = StringHelper.BytesToString(deviceIDBytes);
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
