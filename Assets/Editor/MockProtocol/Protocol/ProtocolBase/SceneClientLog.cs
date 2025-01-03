using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  客户端埋点
	/// </summary>
	[AdvancedInspector.Descriptor(" 客户端埋点", " 客户端埋点")]
	public class SceneClientLog : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500631;
		public UInt32 Sequence;
		/// <summary>
		///  名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字", " 名字")]
		public string name;
		/// <summary>
		///  参数1
		/// </summary>
		[AdvancedInspector.Descriptor(" 参数1", " 参数1")]
		public string param1;
		/// <summary>
		///  参数2
		/// </summary>
		[AdvancedInspector.Descriptor(" 参数2", " 参数2")]
		public string param2;
		/// <summary>
		///  参数3
		/// </summary>
		[AdvancedInspector.Descriptor(" 参数3", " 参数3")]
		public string param3;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			byte[] param1Bytes = StringHelper.StringToUTF8Bytes(param1);
			BaseDLL.encode_string(buffer, ref pos_, param1Bytes, (UInt16)(buffer.Length - pos_));
			byte[] param2Bytes = StringHelper.StringToUTF8Bytes(param2);
			BaseDLL.encode_string(buffer, ref pos_, param2Bytes, (UInt16)(buffer.Length - pos_));
			byte[] param3Bytes = StringHelper.StringToUTF8Bytes(param3);
			BaseDLL.encode_string(buffer, ref pos_, param3Bytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			UInt16 param1Len = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref param1Len);
			byte[] param1Bytes = new byte[param1Len];
			for(int i = 0; i < param1Len; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref param1Bytes[i]);
			}
			param1 = StringHelper.BytesToString(param1Bytes);
			UInt16 param2Len = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref param2Len);
			byte[] param2Bytes = new byte[param2Len];
			for(int i = 0; i < param2Len; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref param2Bytes[i]);
			}
			param2 = StringHelper.BytesToString(param2Bytes);
			UInt16 param3Len = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref param3Len);
			byte[] param3Bytes = new byte[param3Len];
			for(int i = 0; i < param3Len; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref param3Bytes[i]);
			}
			param3 = StringHelper.BytesToString(param3Bytes);
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
