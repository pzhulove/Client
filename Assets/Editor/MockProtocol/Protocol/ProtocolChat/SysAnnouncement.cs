using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  系统公告, 服务器主动发出
	/// </summary>
	[AdvancedInspector.Descriptor(" 系统公告, 服务器主动发出", " 系统公告, 服务器主动发出")]
	public class SysAnnouncement : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 25;
		public UInt32 Sequence;

		public UInt32 id;

		public string word;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			byte[] wordBytes = StringHelper.StringToUTF8Bytes(word);
			BaseDLL.encode_string(buffer, ref pos_, wordBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			UInt16 wordLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref wordLen);
			byte[] wordBytes = new byte[wordLen];
			for(int i = 0; i < wordLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref wordBytes[i]);
			}
			word = StringHelper.BytesToString(wordBytes);
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
