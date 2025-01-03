using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知客户端被踢
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知客户端被踢", " 通知客户端被踢")]
	public class GateNotifyKickoff : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 300404;
		public UInt32 Sequence;

		public UInt32 errorCode;

		public string msg;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			byte[] msgBytes = StringHelper.StringToUTF8Bytes(msg);
			BaseDLL.encode_string(buffer, ref pos_, msgBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			UInt16 msgLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref msgLen);
			byte[] msgBytes = new byte[msgLen];
			for(int i = 0; i < msgLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref msgBytes[i]);
			}
			msg = StringHelper.BytesToString(msgBytes);
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
