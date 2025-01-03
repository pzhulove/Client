using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求发送喇叭
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求发送喇叭", " 请求发送喇叭")]
	public class SceneChatHornReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500808;
		public UInt32 Sequence;
		/// <summary>
		///  喇叭内容
		/// </summary>
		[AdvancedInspector.Descriptor(" 喇叭内容", " 喇叭内容")]
		public string content;
		/// <summary>
		///  一次性发送的喇叭数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 一次性发送的喇叭数量", " 一次性发送的喇叭数量")]
		public byte num;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] contentBytes = StringHelper.StringToUTF8Bytes(content);
			BaseDLL.encode_string(buffer, ref pos_, contentBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, num);
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
			BaseDLL.decode_int8(buffer, ref pos_, ref num);
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
