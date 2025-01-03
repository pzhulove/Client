using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 设置师傅问卷
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 设置师傅问卷", "client->world 设置师傅问卷")]
	public class WorldSetMasterQuestionnaireReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601741;
		public UInt32 Sequence;
		/// <summary>
		/// 活跃时间类型
		/// </summary>
		[AdvancedInspector.Descriptor("活跃时间类型", "活跃时间类型")]
		public byte activeTimeType;
		/// <summary>
		/// 我是什么类型师傅
		/// </summary>
		[AdvancedInspector.Descriptor("我是什么类型师傅", "我是什么类型师傅")]
		public byte masterType;
		/// <summary>
		/// 所在地区id
		/// </summary>
		[AdvancedInspector.Descriptor("所在地区id", "所在地区id")]
		public byte regionId;
		/// <summary>
		/// 宣言
		/// </summary>
		[AdvancedInspector.Descriptor("宣言", "宣言")]
		public string declaration;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
			BaseDLL.encode_int8(buffer, ref pos_, masterType);
			BaseDLL.encode_int8(buffer, ref pos_, regionId);
			byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
			BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
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
