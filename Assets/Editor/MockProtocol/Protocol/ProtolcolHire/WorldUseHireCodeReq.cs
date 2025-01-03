using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 使用邀请码
	/// </summary>
	[AdvancedInspector.Descriptor("使用邀请码", "使用邀请码")]
	public class WorldUseHireCodeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601784;
		public UInt32 Sequence;
		/// <summary>
		/// 邀请码
		/// </summary>
		[AdvancedInspector.Descriptor("邀请码", "邀请码")]
		public string code;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] codeBytes = StringHelper.StringToUTF8Bytes(code);
			BaseDLL.encode_string(buffer, ref pos_, codeBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 codeLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref codeLen);
			byte[] codeBytes = new byte[codeLen];
			for(int i = 0; i < codeLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref codeBytes[i]);
			}
			code = StringHelper.BytesToString(codeBytes);
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
