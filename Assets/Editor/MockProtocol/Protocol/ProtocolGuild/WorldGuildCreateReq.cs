using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  创建公会
	/// </summary>
	[AdvancedInspector.Descriptor(" 创建公会", " 创建公会")]
	public class WorldGuildCreateReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601901;
		public UInt32 Sequence;
		/// <summary>
		/// 公会名
		/// </summary>
		[AdvancedInspector.Descriptor("公会名", "公会名")]
		public string name;
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
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
			BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
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
