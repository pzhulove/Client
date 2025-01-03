using System;
using System.Text;

namespace Mock.Protocol
{

	public class AdminLoginVerifyReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 200201;
		public UInt32 Sequence;

		public UInt32 version;

		public string source1;

		public UInt32 append1;

		public string source2;

		public byte append2;

		public byte[] tableMd5 = new byte[16];

		public UInt32 svnVersion;

		public byte[] append3 = new byte[12];

		public string param;

		public byte[] hashValue = new byte[20];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, version);
			byte[] source1Bytes = StringHelper.StringToUTF8Bytes(source1);
			BaseDLL.encode_string(buffer, ref pos_, source1Bytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, append1);
			byte[] source2Bytes = StringHelper.StringToUTF8Bytes(source2);
			BaseDLL.encode_string(buffer, ref pos_, source2Bytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, append2);
			for(int i = 0; i < tableMd5.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, tableMd5[i]);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, svnVersion);
			for(int i = 0; i < append3.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, append3[i]);
			}
			byte[] paramBytes = StringHelper.StringToUTF8Bytes(param);
			BaseDLL.encode_string(buffer, ref pos_, paramBytes, (UInt16)(buffer.Length - pos_));
			for(int i = 0; i < hashValue.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, hashValue[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref version);
			UInt16 source1Len = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref source1Len);
			byte[] source1Bytes = new byte[source1Len];
			for(int i = 0; i < source1Len; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref source1Bytes[i]);
			}
			source1 = StringHelper.BytesToString(source1Bytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref append1);
			UInt16 source2Len = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref source2Len);
			byte[] source2Bytes = new byte[source2Len];
			for(int i = 0; i < source2Len; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref source2Bytes[i]);
			}
			source2 = StringHelper.BytesToString(source2Bytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref append2);
			for(int i = 0; i < tableMd5.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref tableMd5[i]);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref svnVersion);
			for(int i = 0; i < append3.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref append3[i]);
			}
			UInt16 paramLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref paramLen);
			byte[] paramBytes = new byte[paramLen];
			for(int i = 0; i < paramLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref paramBytes[i]);
			}
			param = StringHelper.BytesToString(paramBytes);
			for(int i = 0; i < hashValue.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref hashValue[i]);
			}
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
