using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 运营活动变量同步
	/// </summary>
	[AdvancedInspector.Descriptor("运营活动变量同步", "运营活动变量同步")]
	public class SceneOpActivityVarSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507401;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		[AdvancedInspector.Descriptor(" 运营活动id", " 运营活动id")]
		public UInt32 opActId;
		/// <summary>
		///  变量key
		/// </summary>
		[AdvancedInspector.Descriptor(" 变量key", " 变量key")]
		public string key;
		/// <summary>
		///  变量value
		/// </summary>
		[AdvancedInspector.Descriptor(" 变量value", " 变量value")]
		public string value;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
			BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
			byte[] valueBytes = StringHelper.StringToUTF8Bytes(value);
			BaseDLL.encode_string(buffer, ref pos_, valueBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
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
