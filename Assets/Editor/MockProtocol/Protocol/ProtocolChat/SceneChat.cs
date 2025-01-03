using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneChat : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500801;
		public UInt32 Sequence;

		public byte channel;

		public UInt64 targetId;

		public string word;

		public byte bLink;

		public string voiceKey;

		public byte voiceDuration;

		public byte isShare;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, channel);
			BaseDLL.encode_uint64(buffer, ref pos_, targetId);
			byte[] wordBytes = StringHelper.StringToUTF8Bytes(word);
			BaseDLL.encode_string(buffer, ref pos_, wordBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, bLink);
			byte[] voiceKeyBytes = StringHelper.StringToUTF8Bytes(voiceKey);
			BaseDLL.encode_string(buffer, ref pos_, voiceKeyBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, voiceDuration);
			BaseDLL.encode_int8(buffer, ref pos_, isShare);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref channel);
			BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
			UInt16 wordLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref wordLen);
			byte[] wordBytes = new byte[wordLen];
			for(int i = 0; i < wordLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref wordBytes[i]);
			}
			word = StringHelper.BytesToString(wordBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref bLink);
			UInt16 voiceKeyLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref voiceKeyLen);
			byte[] voiceKeyBytes = new byte[voiceKeyLen];
			for(int i = 0; i < voiceKeyLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref voiceKeyBytes[i]);
			}
			voiceKey = StringHelper.BytesToString(voiceKeyBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref voiceDuration);
			BaseDLL.decode_int8(buffer, ref pos_, ref isShare);
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
