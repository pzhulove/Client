using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 徒弟uid
	/// </summary>
	/// <summary>
	/// 请求代付
	/// </summary>
	[AdvancedInspector.Descriptor("请求代付", "请求代付")]
	public class AddonPayReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601722;
		public UInt32 Sequence;

		public UInt32 goodId;

		public UInt64 tarId;

		public string tarName;

		public byte tarOccu;

		public UInt32 tarLevel;

		public string words;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, goodId);
			BaseDLL.encode_uint64(buffer, ref pos_, tarId);
			byte[] tarNameBytes = StringHelper.StringToUTF8Bytes(tarName);
			BaseDLL.encode_string(buffer, ref pos_, tarNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, tarOccu);
			BaseDLL.encode_uint32(buffer, ref pos_, tarLevel);
			byte[] wordsBytes = StringHelper.StringToUTF8Bytes(words);
			BaseDLL.encode_string(buffer, ref pos_, wordsBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref goodId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref tarId);
			UInt16 tarNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref tarNameLen);
			byte[] tarNameBytes = new byte[tarNameLen];
			for(int i = 0; i < tarNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref tarNameBytes[i]);
			}
			tarName = StringHelper.BytesToString(tarNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref tarOccu);
			BaseDLL.decode_uint32(buffer, ref pos_, ref tarLevel);
			UInt16 wordsLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref wordsLen);
			byte[] wordsBytes = new byte[wordsLen];
			for(int i = 0; i < wordsLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref wordsBytes[i]);
			}
			words = StringHelper.BytesToString(wordsBytes);
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
