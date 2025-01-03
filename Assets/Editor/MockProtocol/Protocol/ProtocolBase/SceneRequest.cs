using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  发出请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 发出请求", " 发出请求")]
	public class SceneRequest : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500804;
		public UInt32 Sequence;
		/// <summary>
		///  类型(对应枚举RequestType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 类型(对应枚举RequestType)", " 类型(对应枚举RequestType)")]
		public byte type;
		/// <summary>
		///  目标ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标ID", " 目标ID")]
		public UInt64 target;
		/// <summary>
		///  目标名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标名字", " 目标名字")]
		public string targetName;
		/// <summary>
		///  附加参数
		/// </summary>
		[AdvancedInspector.Descriptor(" 附加参数", " 附加参数")]
		public UInt32 param;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint64(buffer, ref pos_, target);
			byte[] targetNameBytes = StringHelper.StringToUTF8Bytes(targetName);
			BaseDLL.encode_string(buffer, ref pos_, targetNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, param);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint64(buffer, ref pos_, ref target);
			UInt16 targetNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref targetNameLen);
			byte[] targetNameBytes = new byte[targetNameLen];
			for(int i = 0; i < targetNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref targetNameBytes[i]);
			}
			targetName = StringHelper.BytesToString(targetNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param);
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
