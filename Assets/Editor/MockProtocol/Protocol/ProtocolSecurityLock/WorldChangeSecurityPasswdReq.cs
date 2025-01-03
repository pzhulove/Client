using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// ***********************************************
	/// </summary>
	/// <summary>
	///  请求修改安全锁密码
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求修改安全锁密码", " 请求修改安全锁密码")]
	public class WorldChangeSecurityPasswdReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608406;
		public UInt32 Sequence;
		/// <summary>
		///  旧密码
		/// </summary>
		[AdvancedInspector.Descriptor(" 旧密码", " 旧密码")]
		public string oldPasswd;
		/// <summary>
		///  新密码
		/// </summary>
		[AdvancedInspector.Descriptor(" 新密码", " 新密码")]
		public string newPasswd;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] oldPasswdBytes = StringHelper.StringToUTF8Bytes(oldPasswd);
			BaseDLL.encode_string(buffer, ref pos_, oldPasswdBytes, (UInt16)(buffer.Length - pos_));
			byte[] newPasswdBytes = StringHelper.StringToUTF8Bytes(newPasswd);
			BaseDLL.encode_string(buffer, ref pos_, newPasswdBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 oldPasswdLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref oldPasswdLen);
			byte[] oldPasswdBytes = new byte[oldPasswdLen];
			for(int i = 0; i < oldPasswdLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref oldPasswdBytes[i]);
			}
			oldPasswd = StringHelper.BytesToString(oldPasswdBytes);
			UInt16 newPasswdLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref newPasswdLen);
			byte[] newPasswdBytes = new byte[newPasswdLen];
			for(int i = 0; i < newPasswdLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref newPasswdBytes[i]);
			}
			newPasswd = StringHelper.BytesToString(newPasswdBytes);
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
