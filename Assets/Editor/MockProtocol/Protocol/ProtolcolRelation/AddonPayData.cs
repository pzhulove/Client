using System;
using System.Text;

namespace Mock.Protocol
{

	public class AddonPayData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501708;
		public UInt32 Sequence;

		public UInt64 id;

		public byte type;
		/// <summary>
		/// 0.自己找人付 1.别人找自己付
		/// </summary>
		[AdvancedInspector.Descriptor("0.自己找人付 1.别人找自己付", "0.自己找人付 1.别人找自己付")]
		public byte relationType;
		/// <summary>
		/// 社会关系
		/// </summary>
		[AdvancedInspector.Descriptor("社会关系", "社会关系")]
		public string name;

		public byte occu;

		public UInt32 level;

		public UInt32 overdueTime;

		public byte status;
		/// <summary>
		/// 0.未响应 1.同意 2.拒绝
		/// </summary>
		[AdvancedInspector.Descriptor("0.未响应 1.同意 2.拒绝", "0.未响应 1.同意 2.拒绝")]
		public UInt32 payItemId;

		public UInt32 payItemNum;

		public string words;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, relationType);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_uint32(buffer, ref pos_, level);
			BaseDLL.encode_uint32(buffer, ref pos_, overdueTime);
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_uint32(buffer, ref pos_, payItemId);
			BaseDLL.encode_uint32(buffer, ref pos_, payItemNum);
			byte[] wordsBytes = StringHelper.StringToUTF8Bytes(words);
			BaseDLL.encode_string(buffer, ref pos_, wordsBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref relationType);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			BaseDLL.decode_uint32(buffer, ref pos_, ref level);
			BaseDLL.decode_uint32(buffer, ref pos_, ref overdueTime);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			BaseDLL.decode_uint32(buffer, ref pos_, ref payItemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref payItemNum);
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
