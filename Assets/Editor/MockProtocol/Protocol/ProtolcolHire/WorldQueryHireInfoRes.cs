using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 
	/// </summary>
	[AdvancedInspector.Descriptor("", "")]
	public class WorldQueryHireInfoRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601783;
		public UInt32 Sequence;
		/// <summary>
		/// 身份
		/// </summary>
		[AdvancedInspector.Descriptor("身份", "身份")]
		public byte identity;
		/// <summary>
		/// 邀请码
		/// </summary>
		[AdvancedInspector.Descriptor("邀请码", "邀请码")]
		public string code;
		/// <summary>
		/// 是否已绑定
		/// </summary>
		[AdvancedInspector.Descriptor("是否已绑定", "是否已绑定")]
		public byte isBind;
		/// <summary>
		/// 是否有别人绑定我
		/// </summary>
		[AdvancedInspector.Descriptor("是否有别人绑定我", "是否有别人绑定我")]
		public byte isOtherBindMe;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, identity);
			byte[] codeBytes = StringHelper.StringToUTF8Bytes(code);
			BaseDLL.encode_string(buffer, ref pos_, codeBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, isBind);
			BaseDLL.encode_int8(buffer, ref pos_, isOtherBindMe);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref identity);
			UInt16 codeLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref codeLen);
			byte[] codeBytes = new byte[codeLen];
			for(int i = 0; i < codeLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref codeBytes[i]);
			}
			code = StringHelper.BytesToString(codeBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref isBind);
			BaseDLL.decode_int8(buffer, ref pos_, ref isOtherBindMe);
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
