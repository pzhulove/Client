using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// *******************************************
	/// </summary>
	/// <summary>
	///  请求操作安全锁
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求操作安全锁", " 请求操作安全锁")]
	public class WorldSecurityLockOpReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608404;
		public UInt32 Sequence;
		/// <summary>
		///  锁操作类型(LockOpType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 锁操作类型(LockOpType)", " 锁操作类型(LockOpType)")]
		public UInt32 lockOpType;
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
			BaseDLL.encode_uint32(buffer, ref pos_, lockOpType);
			byte[] passwdBytes = StringHelper.StringToUTF8Bytes(passwd);
			BaseDLL.encode_string(buffer, ref pos_, passwdBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref lockOpType);
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
