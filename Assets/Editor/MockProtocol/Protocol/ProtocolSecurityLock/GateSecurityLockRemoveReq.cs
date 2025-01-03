using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// **********************************************
	/// </summary>
	/// <summary>
	///  网关解锁安全锁
	/// </summary>
	[AdvancedInspector.Descriptor(" 网关解锁安全锁", " 网关解锁安全锁")]
	public class GateSecurityLockRemoveReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 308401;
		public UInt32 Sequence;
		/// <summary>
		///  密码
		/// </summary>
		[AdvancedInspector.Descriptor(" 密码", " 密码")]
		public string passwd;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] passwdBytes = StringHelper.StringToUTF8Bytes(passwd);
			BaseDLL.encode_string(buffer, ref pos_, passwdBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 passwdLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref passwdLen);
			byte[] passwdBytes = new byte[passwdLen];
			for(int i = 0; i < passwdLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref passwdBytes[i]);
			}
			passwd = StringHelper.BytesToString(passwdBytes);
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
