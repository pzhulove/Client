using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步请求", " 同步请求")]
	public class SceneSyncRequest : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500805;
		public UInt32 Sequence;
		/// <summary>
		///  类型(对应枚举RequestType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 类型(对应枚举RequestType)", " 类型(对应枚举RequestType)")]
		public byte type;
		/// <summary>
		///  请求者
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求者", " 请求者")]
		public UInt64 requester;
		/// <summary>
		///  请求者名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求者名字", " 请求者名字")]
		public string requesterName;
		/// <summary>
		///  请求者性别
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求者性别", " 请求者性别")]
		public byte requesterOccu;
		/// <summary>
		///  请求者等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求者等级", " 请求者等级")]
		public UInt16 requesterLevel;
		/// <summary>
		///  附带参数1
		/// </summary>
		[AdvancedInspector.Descriptor(" 附带参数1", " 附带参数1")]
		public string param1;
		/// <summary>
		/// vip等级
		/// </summary>
		[AdvancedInspector.Descriptor("vip等级", "vip等级")]
		public byte requesterVipLv;
		/// <summary>
		/// 外观信息
		/// </summary>
		[AdvancedInspector.Descriptor("外观信息", "外观信息")]
		public PlayerAvatar avatar = null;
		/// <summary>
		/// 在线时间类型
		/// </summary>
		[AdvancedInspector.Descriptor("在线时间类型", "在线时间类型")]
		public byte activeTimeType;
		/// <summary>
		/// 师傅类型
		/// </summary>
		[AdvancedInspector.Descriptor("师傅类型", "师傅类型")]
		public byte masterType;
		/// <summary>
		/// 地区id
		/// </summary>
		[AdvancedInspector.Descriptor("地区id", "地区id")]
		public byte regionId;
		/// <summary>
		/// 收徒或拜师宣言
		/// </summary>
		[AdvancedInspector.Descriptor("收徒或拜师宣言", "收徒或拜师宣言")]
		public string declaration;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint64(buffer, ref pos_, requester);
			byte[] requesterNameBytes = StringHelper.StringToUTF8Bytes(requesterName);
			BaseDLL.encode_string(buffer, ref pos_, requesterNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, requesterOccu);
			BaseDLL.encode_uint16(buffer, ref pos_, requesterLevel);
			byte[] param1Bytes = StringHelper.StringToUTF8Bytes(param1);
			BaseDLL.encode_string(buffer, ref pos_, param1Bytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, requesterVipLv);
			avatar.encode(buffer, ref pos_);
			BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
			BaseDLL.encode_int8(buffer, ref pos_, masterType);
			BaseDLL.encode_int8(buffer, ref pos_, regionId);
			byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
			BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint64(buffer, ref pos_, ref requester);
			UInt16 requesterNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref requesterNameLen);
			byte[] requesterNameBytes = new byte[requesterNameLen];
			for(int i = 0; i < requesterNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref requesterNameBytes[i]);
			}
			requesterName = StringHelper.BytesToString(requesterNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref requesterOccu);
			BaseDLL.decode_uint16(buffer, ref pos_, ref requesterLevel);
			UInt16 param1Len = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref param1Len);
			byte[] param1Bytes = new byte[param1Len];
			for(int i = 0; i < param1Len; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref param1Bytes[i]);
			}
			param1 = StringHelper.BytesToString(param1Bytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref requesterVipLv);
			avatar.decode(buffer, ref pos_);
			BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
			BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
			BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
			UInt16 declarationLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
			byte[] declarationBytes = new byte[declarationLen];
			for(int i = 0; i < declarationLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
			}
			declaration = StringHelper.BytesToString(declarationBytes);
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
