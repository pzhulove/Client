using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneNotifyTaskVarRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501110;
		public UInt32 Sequence;

		public UInt32 taskID;

		public string key;

		public string value;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskID);
			byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
			BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
			byte[] valueBytes = StringHelper.StringToUTF8Bytes(value);
			BaseDLL.encode_string(buffer, ref pos_, valueBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
			UInt16 keyLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref keyLen);
			byte[] keyBytes = new byte[keyLen];
			for(int i = 0; i < keyLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref keyBytes[i]);
			}
			key = StringHelper.BytesToString(keyBytes);
			UInt16 valueLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref valueLen);
			byte[] valueBytes = new byte[valueLen];
			for(int i = 0; i < valueLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref valueBytes[i]);
			}
			value = StringHelper.BytesToString(valueBytes);
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
