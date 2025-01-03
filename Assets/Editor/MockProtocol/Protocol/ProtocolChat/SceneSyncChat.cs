using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 同步聊天
	/// </summary>
	[AdvancedInspector.Descriptor("同步聊天", "同步聊天")]
	public class SceneSyncChat : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500803;
		public UInt32 Sequence;

		public byte channel;

		public UInt64 objid;

		public byte sex;

		public byte occu;

		public UInt16 level;

		public byte viplvl;

		public string objname;

		public UInt64 receiverId;

		public string word;

		public byte bLink;

		public byte isGm;

		public string voiceKey;

		public byte voiceDuration;

		public UInt32 mask;

		public UInt32 headFrame;

		public UInt32 zoneId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, channel);
			BaseDLL.encode_uint64(buffer, ref pos_, objid);
			BaseDLL.encode_int8(buffer, ref pos_, sex);
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, viplvl);
			byte[] objnameBytes = StringHelper.StringToUTF8Bytes(objname);
			BaseDLL.encode_string(buffer, ref pos_, objnameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint64(buffer, ref pos_, receiverId);
			byte[] wordBytes = StringHelper.StringToUTF8Bytes(word);
			BaseDLL.encode_string(buffer, ref pos_, wordBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, bLink);
			BaseDLL.encode_int8(buffer, ref pos_, isGm);
			byte[] voiceKeyBytes = StringHelper.StringToUTF8Bytes(voiceKey);
			BaseDLL.encode_string(buffer, ref pos_, voiceKeyBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, voiceDuration);
			BaseDLL.encode_uint32(buffer, ref pos_, mask);
			BaseDLL.encode_uint32(buffer, ref pos_, headFrame);
			BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref channel);
			BaseDLL.decode_uint64(buffer, ref pos_, ref objid);
			BaseDLL.decode_int8(buffer, ref pos_, ref sex);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			BaseDLL.decode_int8(buffer, ref pos_, ref viplvl);
			UInt16 objnameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref objnameLen);
			byte[] objnameBytes = new byte[objnameLen];
			for(int i = 0; i < objnameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref objnameBytes[i]);
			}
			objname = StringHelper.BytesToString(objnameBytes);
			BaseDLL.decode_uint64(buffer, ref pos_, ref receiverId);
			UInt16 wordLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref wordLen);
			byte[] wordBytes = new byte[wordLen];
			for(int i = 0; i < wordLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref wordBytes[i]);
			}
			word = StringHelper.BytesToString(wordBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref bLink);
			BaseDLL.decode_int8(buffer, ref pos_, ref isGm);
			UInt16 voiceKeyLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref voiceKeyLen);
			byte[] voiceKeyBytes = new byte[voiceKeyLen];
			for(int i = 0; i < voiceKeyLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref voiceKeyBytes[i]);
			}
			voiceKey = StringHelper.BytesToString(voiceKeyBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref voiceDuration);
			BaseDLL.decode_uint32(buffer, ref pos_, ref mask);
			BaseDLL.decode_uint32(buffer, ref pos_, ref headFrame);
			BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
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
