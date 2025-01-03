using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  发送公会邮件
	/// </summary>
	[AdvancedInspector.Descriptor(" 发送公会邮件", " 发送公会邮件")]
	public class WorldGuildSendMail : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601924;
		public UInt32 Sequence;

		public string content;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] contentBytes = StringHelper.StringToUTF8Bytes(content);
			BaseDLL.encode_string(buffer, ref pos_, contentBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 contentLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref contentLen);
			byte[] contentBytes = new byte[contentLen];
			for(int i = 0; i < contentLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref contentBytes[i]);
			}
			content = StringHelper.BytesToString(contentBytes);
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
